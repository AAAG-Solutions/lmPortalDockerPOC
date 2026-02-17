using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;

using LMWholesale.Common;
using LMWholesale.resource.clients;

namespace LMWholesale.WholesaleContent.Reporting
{
    public partial class SalesDataApproval : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Reporting.SalesDataApproval sdaBLL;
        private readonly LookupClient lookupClient;
        private readonly WholesaleClient wholesaleClient;

        public SalesDataApproval()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            sdaBLL = sdaBLL ?? new BLL.WholesaleContent.Reporting.SalesDataApproval();
            lookupClient = lookupClient ?? new LookupClient();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }

        public static SalesDataApproval Self
        {
            get { return instance; }
        }
        private static readonly SalesDataApproval instance = new SalesDataApproval();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Sales Data Approval";

            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                if (Regex.IsMatch(Request.UrlReferrer.AbsolutePath, "WholesaleDefault.aspx$"))
                {
                    breadCrumb.Style["display"] = "none";
                    BackToWholesaleDefault.Style["display"] = "initial";

                    BackToWholesaleDefault.InnerHtml = $"&#60; &#8722; <a class='backBreadcrumb' href=\"javascript: window.location.href='/WholesaleContent/WholesaleDefault.aspx';\">Back To Wholesale</a>";
                }

                jsGridBuilder rulegrid = new jsGridBuilder
                {
                    MethodURL = "SalesDataApproval.aspx/GetSalesData",
                    OnRowSelectFunction = "GridRowSelected",
                    OnClearRowSelectFunction = "ClearRowSelection",
                    OnDoubleClickFunction = "RowDoubleClick();",
                    ExtraFunctionality = "ResizeGrid();",
                    HTMLElement = "jsGrid",
                    Filtering = false,
                    PageSize = int.MaxValue,
                    NotSortableFields = new List<string>() { "VIN", "SellerName", "BuyerName", "BuyerAddress", "BuyerAddress", "SalePrice", "ApprovedBy", "ApprovedDate" }
                };
                string lstSaleTransactions = "[]1:Not Sent|3:Sent|0:All|";
                WholesaleSystem.PopulateList(lstSaleTransactions, "", "ddlSaleTransactions", '|');
                string lstMarketPlace = "[]0:All|1:OVE|2:SmartAuction|";
                WholesaleSystem.PopulateList(lstMarketPlace, "", "ddlMarketplace", '|');
                string lstTitle = "[]1:Select All|2:Deselect All|";
                WholesaleSystem.PopulateList(lstTitle, "-- Quick Select Transactions --", "ddlFilters", '|', "0");

                rulegrid.SetFieldListFromGridDef("|!:kWholesaleSoldHistory:kWholesaleSoldHistory:0|!:kInventory:kInventory:0|<:chkSales:Status:125|:SaleDate:Sale Date:75|:Marketplace:Marketplace:90|:VIN:VIN:130|!:Year:Year:0|!:Make:Make:0|!:Model:Model:0|!:Style:Style:0|:YearMakeModelStyle:Vehicle:190|:Mileage:Mileage:60|:SellerName:Seller Name:180|!:SellerAuctionAccess:Seller #:50|:SellerAuctionAccessEntry:Seller #:75|:BuyerName:Buyer Name:180|:BuyerAddress:Buyer Address:150|!:BuyerAuctionAccess:Buyer #:50|:BuyerAuctionAccessEntry:Buyer #:75|:SalePrice:Sale Price:100|!:ApprovedStatus:Status:50|:ApprovedBy:Approved By:180|:ApprovedDate:Approved Date:75|!:StatusMessage:StatusMessage:0|!:ApprovedStatusDisplay:ApprovedStatusDisplay:0", "", true);
                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "SalesDataGrid", rulegrid.RenderGrid());
            }
        }

        [WebMethod(Description = "Get Sales Data")]
        public static string GetSalesData(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty((string)HttpContext.Current.Session["kSession"]))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];
            Dictionary<string, object> data = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);

            bool isFirstRun = !data.Keys.Contains("kSaleTransactions");
            Dictionary<string, string> dataFilter = new Dictionary<string, string>()
            {
                { "kSaleTransactions", "" },
                { "StartDate", "" },
                { "EndDate", "" },
                { "kWholesaleAuction", "" }
            };

            if (Session["SDAFilters"] != null && !data.ContainsKey("filterSave"))
            {
                Dictionary<string, string> savedFilter = (Dictionary<string, string>)Session["SDAFilters"];
                dataFilter["kSaleTransactions"] = savedFilter["kSaleTransactions"].ToString();
                dataFilter["StartDate"] = savedFilter["StartDate"].ToString();
                dataFilter["EndDate"] = savedFilter["EndDate"].ToString();
                dataFilter["kWholesaleAuction"] = savedFilter["kWholesaleAuction"].ToString();
            }
            else
            {
                dataFilter["kSaleTransactions"] = isFirstRun ? "1" : data["kSaleTransactions"].ToString();
                dataFilter["StartDate"] = isFirstRun ? "1970-01-01" : data["StartDate"].ToString();
                dataFilter["EndDate"] = isFirstRun ? "1970-01-01" : data["EndDate"].ToString();
                dataFilter["kWholesaleAuction"] = isFirstRun ? "0" : data["kWholesaleAuction"].ToString();
            }

            Session["SDAFilters"] = dataFilter;
            string sortField = data.ContainsKey("sortField") ? data["sortField"].ToString() : "";
            string sortOrder = data.ContainsKey("sortOrder") ? data["sortOrder"].ToString() : "";
            dataFilter["kSession"] = kSession;

            return Self.sdaBLL.GetSalesData(dataFilter, sortField, sortOrder);
        }

        [WebMethod(Description = "Set Sales Data")]
        public static Dictionary<string, object> SetSalesData(string JsonData, string op)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            string kSession = (string)Session["kSession"];
            Dictionary<string, int> returnData = null;
            IsSuccess = Self.sdaBLL.SetSalesData(kSession, JsonData, ref returnData, op == "mark" ? false : true);
            Value = returnData;

            return ReturnResponse();
        }

        protected void ExportSalesData(object Sender, EventArgs e)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];

            if (String.IsNullOrEmpty(Convert.ToString(Session["kSession"])))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            Dictionary<string, string> tmp = new Dictionary<string, string>()
            {
                { "kSession", kSession },
                { "kSaleTransactions", kSales.Value },
                { "StartDate", txtStartDate.Text },
                { "EndDate", txtEndDate.Text },
                { "kWholesaleAuction", kMarket.Value }
            };

            Dictionary<string, string> dict = Self.sdaBLL.ExportInventory(Util.serializer.Serialize(tmp));

            // Download CSV file
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={dict["fileName"]}");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(dict["sb"]);

            // Take out the trash
            Response.Flush();
            Response.End();
        }
    }
}