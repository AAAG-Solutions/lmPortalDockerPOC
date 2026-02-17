using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;

using LMWholesale.Common;


namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class ChangeVin : lmPage
    {
        private readonly BLL.WholesaleContent.Vehicle.ChangeVin changeVin;
        public ChangeVin() => changeVin = changeVin ?? new BLL.WholesaleContent.Vehicle.ChangeVin();
        public static ChangeVin Self
        {
            get { return instance; }
        }
        private static readonly ChangeVin instance = new ChangeVin();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Change Vin";

            if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
            {
                int kListing = int.Parse(Request.QueryString["kListing"]);
                hfkListing.Value = kListing.ToString();

                DataRow vehicleDetail = Self.changeVin.GetListingDetails(kListing, 0);

                tbVinNumber.Text = vehicleDetail["VIN"].ToString();
                hfCurrentVin.Value = vehicleDetail["VIN"].ToString();
                tbYear.Text = vehicleDetail["MotorYear"].ToString();
                tbMake.Text = vehicleDetail["Make"].ToString();
                tbModel.Text = vehicleDetail["Model"].ToString();
                tbStyle.Text = vehicleDetail["Style"].ToString();
                hfStyleId.Value = vehicleDetail["StyleId"].ToString();
                hfCost.Value = vehicleDetail["InvCost"].ToString();
                hfMiles.Value = vehicleDetail["Miles"].ToString();
                hfListPrice.Value = vehicleDetail["InvListPrice"].ToString();
                hfStockNumber.Value = vehicleDetail["StockNumber"].ToString();
                hfStockType.Value = vehicleDetail["StockType"].ToString();
                hfStatus.Value = vehicleDetail["Status"].ToString();
                hfDMI.Value = vehicleDetail["ImportOverride"].ToString();
            }
        }

        [WebMethod(Description = "Perform ChromeData check on VIN")]
        public static string VINCheck(string vin, string styleCode)
        {
            Dictionary<string, string> returnJson = new Dictionary<string, string>
            {
                { "year", "0" },
                { "make", "" },
                { "model", "" },
                { "style", "" },
                { "styleId", "0" },
                { "options", "" }
            };

            if (vin == "recover")
                return Util.serializer.Serialize(returnJson);

            DataTable ChromeMulti = Self.changeVin.ResolveChrome(vin);

            if (ChromeMulti.Rows.Count == 1 || ChromeMulti.AsEnumerable().Any(x => x[0].ToString() == styleCode))
            {
                DataRow r = ChromeMulti.Rows.Count == 1 ? ChromeMulti.Rows[0] : ChromeMulti.AsEnumerable().Where(x => x[0].ToString() == styleCode).First();
                returnJson["year"] = r["ModelYear"].ToString();
                returnJson["make"] = r["Make"].ToString();
                returnJson["model"] = r["Model"].ToString();
                returnJson["style"] = r["Style"].ToString();
                returnJson["styleId"] = r["StyleID"].ToString();
            }
            else if (ChromeMulti.Rows.Count > 1)
            {
                returnJson["year"] = ChromeMulti.Rows[0]["ModelYear"].ToString();
                returnJson["make"] = ChromeMulti.Rows[0]["Make"].ToString();
                returnJson["model"] = ChromeMulti.Rows[0]["Model"].ToString();
                returnJson["options"] = Self.changeVin.FormatOptions(ChromeMulti, "blackbook");
            }

            // Return empty default json struct if we fail for some reason
            return Util.serializer.Serialize(returnJson);
        }

        [WebMethod(Description = "Update Inventory data")]
        public static string UpdateInventory(string jsonData)
        {
            Dictionary<string, object> parsed = (Dictionary<string, object>)Util.serializer.DeserializeObject(jsonData);

            if (Self.changeVin.SaveDataProcess(Util.serializer.Serialize(parsed)) == "Success")
                return Self.changeVin.AnalyticsReRun(parsed["kListing"].ToString());
            else
                return "Failed";
        }
    }
}