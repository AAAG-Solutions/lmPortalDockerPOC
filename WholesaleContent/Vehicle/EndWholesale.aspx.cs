using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.Services;

using LMWholesale.Common;
using LMWholesale.resource.factory;


namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class EndWholesale : lmPage
    {
        private readonly AuctionFactory factory;
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Vehicle.EndWholesale endWholesaleBLL;

        public EndWholesale()
        {
            factory = factory ?? new AuctionFactory();
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            endWholesaleBLL = endWholesaleBLL ?? new BLL.WholesaleContent.Vehicle.EndWholesale();
        }

        public static EndWholesale Self
        {
            get { return instance; }
        }

        private static readonly EndWholesale instance = new EndWholesale();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "End Wholesale";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                    BLL.WholesaleUser.WholesaleUser.ClearUser();

                int kDealer = (int)Session["kDealer"];
                string kSession = (string)Session["kSession"];

                string kListing = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";
            
                if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
                {
                    startWholesale.HRef = $"/WholesaleContent/Vehicle/StartWholesale.aspx?kListing={kListing}";
                    CurrenkListing.Value = kListing;

                    if (Regex.IsMatch(Request.UrlReferrer.AbsolutePath, "VehicleManagement.aspx$"))
                    {
                        BackToVehicle.InnerHtml = $"&#60; &#8722; <a class='backBreadcrumb' href=\"javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx';\">Back To Inventory</a>";
                        Cancel.InnerHtml = $"<input type=\"button\" class=\"actionBackground\" value=\"Cancel\" onclick=\"javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx';\">";
                    }
                    else
                    {
                        BackToVehicle.InnerHtml = $"&#60; &#8722; <a class='backBreadcrumb' href=\"javascript: window.location.href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}';\">Back To Vehicle</a>";
                        Cancel.InnerHtml = $"<input type=\"button\" class=\"actionBackground\" value=\"Cancel\" onclick=\"javascript: window.location.href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}';\">";
                    }

                    Dictionary<string, object> vehicleDetail = Self.endWholesaleBLL.ListingDetailGet(kSession, kDealer, int.Parse(kListing));

                    if (!String.IsNullOrEmpty(vehicleDetail["ErrorResponse"].ToString()))
                        Response.Write(vehicleDetail["ErrorResponse"].ToString());

                    // Populate dropdowns and header
                    string yearString = ((DataRow)vehicleDetail["dr"])["MotorYear"].ToString();
                    string makeString = ((DataRow)vehicleDetail["dr"])["Make"].ToString();
                    string modelString = ((DataRow)vehicleDetail["dr"])["Model"].ToString();

                    // End Wholesale Info
                    endVIN.Text = Util.cleanString((Convert.ToString(((DataRow)vehicleDetail["dr"])["VIN"]))); ;
                    endDesc.Text = $"{yearString} {makeString} {modelString}";

                    // Populate Auction Info
                    endAuctionList.InnerHtml = Self.endWholesaleBLL.ListingAuctionDataGet(kSession, kDealer, int.Parse(kListing));
                }
            }
        }

        [WebMethod(EnableSession = true)]
        public static string SubmitToRemoveMultiAuction(string info, string kListing)
        {
            Dictionary<string, object> input = (Dictionary<string, object>)Util.serializer.DeserializeObject(info);
            object[] auctions = (object[])input["Auctions"];
            Dictionary<string, object> returnInfo = new Dictionary<string, object>
            {
                { "success", 0},
                { "errormsgs", ""}
            };

            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            if (Convert.ToBoolean(input["MarkUnavailable"]))
            {
                string success = Self.endWholesaleBLL.MarkVehicleUnavailable(kSession, Convert.ToInt32(kListing));
                if(success != "success")
                {
                    returnInfo["errormsgs"] = "Something went wrong! Please contact support!";
                    return Util.serializer.Serialize(returnInfo);
                }
            }

            returnInfo["success"] = Self.endWholesaleBLL.SubmitToRemoveMultiAuction(kSession, kDealer, kListing, auctions);

            return Util.serializer.Serialize(returnInfo);
        }
    }
}