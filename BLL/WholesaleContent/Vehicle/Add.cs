using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using LMWholesale.Common;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class Add
    {
        private readonly WholesaleUser.WholesaleUser userBLL;
        private readonly DASClient dasClient;

        public Add()
        {
            userBLL = userBLL ?? new WholesaleUser.WholesaleUser();
            dasClient = dasClient ?? new DASClient();
        }

        public Add(WholesaleUser.WholesaleUser userBLL, DASClient dasClient)
        {
            this.dasClient = dasClient;
            this.userBLL = userBLL;
        }
        internal static readonly Add instance = new Add();
        public Add Self
        {
            get { return instance; }
        }

        public bool AddToInventory(string kSession, int kDealer, Dictionary<string, object> vehicleInfo, ref string message, ref string kListing)
        {
            Dictionary<string, string> returnJson = new Dictionary<string, string> { { "kListing", "" }, { "message", "" } };

            // First we check to make sure that we do not return multiple makes/trims for a given VIN
            // If so, then we do additional logic to narrow down what details the VIN contains
            DAS.lmReturnValue InventoryResponse =
                Self.dasClient.DASCreateInventory(
                    kSession, kDealer.ToString(), vehicleInfo["StockNumber"].ToString(), vehicleInfo["vin"].ToString(), vehicleInfo["year"].ToString(),
                    vehicleInfo["mileage"].ToString(), vehicleInfo["InvCost"].ToString(), vehicleInfo["make"].ToString(), vehicleInfo["model"].ToString(),
                    vehicleInfo["style"].ToString().Replace("&apos;", "'"), vehicleInfo["StyleId"].ToString(), vehicleInfo["InvListPrice"].ToString(), vehicleInfo["drilldown"].ToString(),
                    vehicleInfo["StockType"].ToString(), vehicleInfo["status"].ToString(), vehicleInfo["ImportOverride"].ToString(), vehicleInfo["InternetPrice"].ToString());

            if (InventoryResponse.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                message = "Successfully Added Vehicle!";
                kListing = InventoryResponse.Values.GetValue("kListing", "");
                if (vehicleInfo["vin"].ToString() == "recover" || vehicleInfo["StockNumber"].ToString() == "recover")
                    message = "Successfully Recovered Vehicle!";

                return true;
            }

            // return default empty json struct if we fail for some reason
            return false;
        }

        public void GetMotorYears(string kSession, string source)
        {
            DAS.lmReturnValue MotorYears = Self.dasClient.DASGetMotorYears(kSession, "3");
            string YearsString = "[-- Select a Year --]0:-- Select a Year --|";
            if (MotorYears.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable years = MotorYears.Data.Tables["Years"];
                DataColumnCollection columns = years.Columns;
                foreach (DataRow row in years.Rows)
                    YearsString += Convert.ToString(row[columns[1]]) + ":" + Convert.ToString(row[columns[0]]) + "|";
            }

            LMWholesale.WholesaleSystem.PopulateList(YearsString, "", "YearList", '|', "0");
        }

        public string MakeListGet(string kSession, string year, string source)
        {
            StringBuilder s = new StringBuilder("[-- Select a Make --]0:-- Select a Make --|");

            DAS.lmReturnValue MotorMakes = Self.dasClient.DASGetMotorMakes(kSession, source, year);
            if (MotorMakes.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable makes = MotorMakes.Data.Tables["Makes"];
                foreach (DataRow row in makes.Rows)
                    s.Append($"{row["Feature"]}:{row["Makes"]}|");
            }

            return s.ToString();
        }

        public string ModelListGet(string kSession, string year, string make, string source)
        {
            StringBuilder s = new StringBuilder("[-- Select a Model --]0:-- Select a Model --|");

            DAS.lmReturnValue MotorModels = Self.dasClient.DASGetMotorModels(kSession, "3", year, make);
            if (MotorModels.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable models = MotorModels.Data.Tables["Models"];
                foreach (DataRow row in models.Rows)
                    s.Append($"{row["Feature"]}:{row["Models"]}|");
            }

            return s.ToString();
        }

        public string StyleListGet(string kSession, string year, string make, string model, string source)
        {
            StringBuilder s = new StringBuilder("[-- Select a Style --]0:-- Select a Style --|");

            DAS.lmReturnValue MotorStyles = Self.dasClient.DASGetMotorStyles(kSession, source, year, make, model);
            if (MotorStyles.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable styles = MotorStyles.Data.Tables["Styles"];
                foreach (DataRow row in styles.Rows)
                    s.Append($"{row["StyleID"]}:{row["Styles"]}|");
            }

            return s.ToString();
        }

        public string CheckVIN(string kSession, string vin, string styleCode)
        {
            Dictionary<string, string> returnJson = new Dictionary<string, string>
            {
                { "year", "0" },
                { "make", "" },
                { "model", "" },
                { "style", "" },
                { "styleId", "0" }
            };

            if (vin == "recover")
                return Util.serializer.Serialize(returnJson);

            DAS.lmReturnValue ChromeStyles = Self.dasClient.DASResolveChromeYMMS(kSession, vin);
            if (ChromeStyles.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable ChromeMulti = ChromeStyles.Data.Tables["ChromeMulti"];

                if (ChromeMulti.Rows.Count == 1 || ChromeMulti.AsEnumerable().Any(x => x[0].ToString() == styleCode))
                {
                    DataRow r = ChromeMulti.Rows.Count == 1 ? ChromeMulti.Rows[0] : ChromeMulti.AsEnumerable().Where(x => x[0].ToString() == styleCode).First();
                    returnJson["year"] = r["ModelYear"].ToString();
                    returnJson["make"] = r["Make"].ToString();
                    returnJson["model"] = r["Model"].ToString();
                    returnJson["style"] = r["Style"].ToString();
                    returnJson["styleId"] = r["StyleID"].ToString();

                    return Util.serializer.Serialize(returnJson);
                }
                else if (ChromeMulti.Rows.Count > 1)
                {
                    // Initially get info from styles list
                    // since all records have the same YM
                    DataRow r = ChromeMulti.Rows[0];
                    string MotorYear = r["ModelYear"].ToString();
                    string MotorMake = r["Make"].ToString();
                    string MotorModel = r["Model"].ToString();

                    string options = FormatOptions(ChromeMulti, "blackbook");

                    return $@"<fieldset id='vehicleConfiguration' class='sectionFieldset'>
                        <legend>Vehicle Configuration</legend>
                        <div style='display:table;margin:auto;'>
                            <div style='display:table-row;'>
                                <div class='boldTableCell'>Year:</div>
                                <div id='configYear' style='padding:2px;display:table-cell;'>{MotorYear}</div>
                            </div>
                            <div style='display:table-row;'>
                                <div class='boldTableCell'>Make:</div>
                                <div id='configMake' style='padding:2px;display:table-cell;'>{MotorMake}</div>
                            </div>
                            <div style='display:table-row;'>
                                <div class='boldTableCell'>Model:</div>
                                <div id='configModel' style='padding:2px;display:table-cell;'>{MotorModel}</div>
                            </div>
                            <div style='display:table-row;'>
                                <div class='boldTableCell'>Style:</div>
                                <div style='padding:2px;display:table-cell;'></div>
                            </div>
                        </div>
                        {options}
                    </fieldset>";
                }
            }

            // Return empty default json struct if we fail for some reason
            return Util.serializer.Serialize(returnJson);
        }

        public string ResolveBlueBook(string kSession, int kListing, int mode)
        {
            DAS.lmReturnValue BluebookResponse = Self.dasClient.DASResolveBluebook(kSession, kListing, 0, mode);

            if (BluebookResponse.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable ReturnMenu = BluebookResponse.Data.Tables["ReturnMenu"];

                if (ReturnMenu.Rows.Count > 1)
                {
                    // Initially get info from styles list
                    // since all records have the same YMM
                    DataRow r = ReturnMenu.Rows[0];
                    string MotorYear = r["ModelYear"].ToString();
                    string MotorMake = r["Make"].ToString();
                    string MotorModel = r["Model"].ToString();

                    string options = FormatOptions(ReturnMenu, "bluebook");

                    return $@"<fieldset id='vehicleConfiguration' class='sectionFieldset'>
                        <legend>Vehicle Configuration</legend>
                        <div style='display:table;margin:auto;'>
                            <div style='display:table-row;'>
                                <div class='boldTableCell'>Year:</div>
                                <div style='padding:2px;display:table-cell;'>{MotorYear}</div>
                            </div>
                            <div style='display:table-row;'>
                                <div class='boldTableCell'>Make:</div>
                                <div style='padding:2px;display:table-cell;'>{MotorMake}</div>
                            </div>
                            <div style='display:table-row;'>
                                <div class='boldTableCell'>Model:</div>
                                <div style='padding:2px;display:table-cell;'>{MotorModel}</div>
                            </div>
                            <div style='display:table-row;'>
                                <div class='boldTableCell'>Style:</div>
                                <div style='padding:2px;display:table-cell;'></div>
                            </div>
                        </div>
                        {options}
                    </fieldset>";
                }
                else
                    return "1";
            }

            return "";
        }

        public string ResolveBlackBook(string kSession, int kListing, int kBlackbook, string year, string make, string model, string series, string style, int mode)
        {
            DAS.lmReturnValue BlackbookResponse = Self.dasClient.DASResolveBlackbook(kSession, kListing, kBlackbook, year, make, model, style, series, mode);

            if (BlackbookResponse.Result == DAS.ReturnCode.LM_SUCCESS)
                return "1";

            return "";
        }

        private static string FormatOptions(DataTable t, string book)
        {
            string options = "";
            int count = 1;

            if (book == "blackbook")
            {
                foreach (DataRow dr in t.Rows)
                {
                    string model = dr["Model"].ToString();
                    string style = dr["Style"].ToString();
                    string styleId = dr["StyleID"].ToString();

                    string click = $"onclick='SetStyle(\"{count}\");'";

                    string TableRow = $@"
                    <div style='display:table-row;'>
                        <div id='option_{count}'><button class='submitButton' {click}>Option {count}</button>:</div>
                        <div id='StyleOption_{count}' style='padding:2px;display:table-cell;'>{style}</div>
                        <div id='StyleId_{count}' style='padding:2px;display:table-cell;display:none;'>{styleId}</div>
                    </div>";

                    count += 1;
                    options += TableRow;
                }
            }
            else if (book == "bluebook")
            {
                foreach (DataRow dr in t.Rows)
                {
                    string trim = dr["Trim"].ToString();
                    string vehicleId = dr["VehicleId"].ToString();

                    string click = $"onclick='SetTrim(\"{count}\");'";

                    string TableRow = $@"
                        <div style='display:table-row;'>
                            <div id='option_{count}'><button {click}>Option {count}</button>:</div>
                            <div id='TrimOption_{count}' style='padding:2px;display:table-cell;'>{trim}</div>
                            <div id='VehicleId_{count}' style='padding:2px;display:table-cell;display:none;'>{vehicleId}</div>
                        </div>";

                    count += 1;
                    options += TableRow;
                }
            }

            string optionsTemplate = $"<div style='display:table;margin:auto;'>{options}</div>";

            return optionsTemplate;
        }
    }
}