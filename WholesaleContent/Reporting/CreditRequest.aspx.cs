using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;

using LMWholesale.Common;
using LMWholesale.resource.clients;

namespace LMWholesale.WholesaleContent.Reporting
{
    public partial class CreditRequest : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Reporting.CreditRequest crBLL;
        private readonly WholesaleClient wholesaleClient;

        public CreditRequest()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            crBLL = crBLL ?? new BLL.WholesaleContent.Reporting.CreditRequest();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }

        public static CreditRequest Self
        {
            get { return instance; }
        }
        private static readonly CreditRequest instance = new CreditRequest();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Credit Request";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];

                jsGridBuilder currentlyListedGrid = new jsGridBuilder
                {
                    MethodURL = "CreditRequest.aspx/CreditRequestGet",
                    HTMLElement = "jsGrid",
                    Filtering = false,
                    Sorting = false
                };

                string searchVehicleGridColumns = ":RequestDate:Request Date:80|:VIN::100|:Reason::60|:RequestedBy:Requested By:80|:SellingAccount:Selling Account:100|:SoldDate:Date Sold:100|:Result::80|";
                currentlyListedGrid.SetFieldListFromGridDef(searchVehicleGridColumns, "", true);

                StringBuilder lstAuctions = new StringBuilder();
                List<Dictionary<string, string>> auctions = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 1);

                foreach (Dictionary<string, string> auction in auctions)
                {
                    if (auction["WholesaleAuctionName"] == "CarOffer")
                        continue;
                    lstAuctions.Append($"{auction["kWholesaleAuction"]}:{(auction["WholesaleAuctionName"] == "RemarketingPlus" ? "Remarketing+" : auction["WholesaleAuctionName"])}|");
                }

                WholesaleSystem.PopulateList(lstAuctions.ToString(), "-- Select Marketplace --", "lstMarketPlace", '|');

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "creditRequestGrid", currentlyListedGrid.RenderGrid());
            }
        }

        [WebMethod(EnableSession = true)]
        public static string CreditRequestGet(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            //Dictionary<string, object> filter = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);

            return Self.crBLL.CreditRequestGet(kSession, kDealer);
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, object> CreditRequestSet(string data)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            int kAccountType = (int)Session["kAccountType"];

            string resultString = "";
            IsSuccess = Self.crBLL.CreditRequestSet(kSession, kDealer, kAccountType, (Dictionary<string, object>)Util.serializer.DeserializeObject(data), ref resultString);
            Message = resultString;

            return ReturnResponse();
        }
    }
}