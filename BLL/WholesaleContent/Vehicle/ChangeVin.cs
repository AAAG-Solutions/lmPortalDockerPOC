using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class ChangeVin
    {
        private readonly DASClient dasClient;
        private readonly ListingClient listingClient;

        public ChangeVin()
        {
            dasClient = dasClient ?? new DASClient();
            listingClient = listingClient ?? new ListingClient();
        }

        public ChangeVin(DASClient dasClient, ListingClient ListingClient)
        {
            this.dasClient = dasClient;
            this.listingClient = ListingClient;
        }
        internal static readonly ChangeVin instance = new ChangeVin();
        public ChangeVin Self
        {
            get { return instance; }
        }

        public string FormatOptions(DataTable t, string book)
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

                    string TableRow = $@"
                    <div style='display:table-row;'>
                        <div id='option_{count}' style=""display: table - cell; padding: 2px 15px 2px 15px; text - align:center;""><button class='submitButton' onclick='ButtonAction(""SetStyle_{count}""); return false;'>Option {count}</button></div>
                        <div id='StyleOption_{count}' style='display:table-cell;padding:2px;text-align:center;border-left:1px solid #517B97;'>{style}</div>
                        <div id='StyleId_{count}' style='display:none;padding:2px;text-align:center;'>{styleId}</div>
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

            return options;
        }

        public DataRow GetListingDetails(int kListing, int Thin)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            Listing.lmReturnValue vehicleDetail = Self.listingClient.ListingDetailGet(kSession, kDealer, kListing, Thin);
            if (vehicleDetail.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                DataRow dr = vehicleDetail.Data.Tables["VehicleData"].Rows[0];
                return dr;
            }

            // Default return if we fail for some reason
            return new DataTable().NewRow();
        }

        public DataTable ResolveChrome(string Vin)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string sessid = (string)Session["kSession"];

            DAS.lmReturnValue chrome = Self.dasClient.DASResolveChromeYMMS(sessid, Vin);
            if (chrome.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = chrome.Data.Tables["ChromeMulti"];
                return dt;
            }

            // Default return if we fail for some reason
            return new DataTable();
        }

        public string SaveDataProcess(string data)
        {

            HttpSessionState Session = HttpContext.Current.Session;
            string sessid = (string)Session["kSession"];

            DAS.lmReturnValue update = Self.dasClient.DASUpdateInventory(sessid, data);

            if (update.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                return "Success";
            }

            // Default return if we fail for some reason
            return "Fail";

        }

        public string AnalyticsReRun(string kListing)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string sessid = (string)Session["kSession"];

            DAS.lmReturnValue update = Self.dasClient.GetAnalyticsReRun(sessid, "1", kListing);

            if (update.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                return "Success";
            }

            // Default return if we fail for some reason
            return "Fail";
        }
    }
}