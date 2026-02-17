using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;

using LMWholesale.Common;

namespace LMWholesale.WholesaleContent.Reporting
{
    public partial class ReportViewer : lmPage
    {
        private readonly BLL.WholesaleContent.Reporting.ReportViewer reportViewerBLL;
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;

        public ReportViewer()
        {
            reportViewerBLL = reportViewerBLL ?? new BLL.WholesaleContent.Reporting.ReportViewer();
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
        }
        public ReportViewer(BLL.WholesaleContent.Reporting.ReportViewer _reportViewerBLL, BLL.WholesaleUser.WholesaleUser _userBLL)
        {
            reportViewerBLL = _reportViewerBLL;
            userBLL = _userBLL;
        }

        public static ReportViewer Self
        {
            get { return instance; }
        }

        private static readonly ReportViewer instance = new ReportViewer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["Mode"]))
            {
                HttpSessionState Session = HttpContext.Current.Session;
                string kSession = (string)Session["kSession"];

                Session["RVMode"] = Request.QueryString["Mode"].ToString();
                if (!String.IsNullOrEmpty(Request.QueryString["Date"]))
                    Session["RVDate"] = Request.QueryString["Date"].ToString();

                lbTitle.Text = GetTitle(Request.QueryString["Mode"].ToString());

                Self.userBLL.CheckDealer();
                PageSecurityManager.DoPageSecurity(this);

                PageTitle = "Report Viewer";

                jsGridBuilder reportGrid = new jsGridBuilder
                {
                    MethodURL = "ReportViewer.aspx/GetReportData",
                    OnRowSelectFunction = "",
                    OnClearRowSelectFunction = "",
                    OnDoubleClickFunction = "",
                    HTMLElement = "jsGrid",
                    Filtering = false
                };

                reportGrid.SetFieldListFromGridDef(Self.userBLL.GetGridDef(kSession, reportViewerBLL.GetGridType(Request.QueryString["Mode"].ToString())), "", true);

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "reportGrid", reportGrid.RenderGrid());
            }
        }

        private string GetTitle(string type)
        {
            Dictionary<string, string> mapping = new Dictionary<string, string>
            {
                { "healthReportMin", "Health Report Minimum" },
                { "offerReport", "Offer Report" },
                { "healthReportSummary", "Health Report Summary" },
                { "healthReportDetail", "Health Report Detail" },
                { "WholesaleActiveListings", "Wholesale - Active Listings" },
                { "WholesaleAuctionGroupActiveListings", "Wholesale - Auction/Group Active Listings" }
            };

            return mapping[type];
        }

        [WebMethod(Description = "Get Report Data")]
        public static string GetReportData(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            string ReportVersion = (string)Session["RVMode"];
            DateTime FirstMonthDate = string.IsNullOrEmpty((string)Session["RVDate"]) ? DateTime.MinValue : Convert.ToDateTime((string)Session["RVDate"]);
            int kGaggleSubGroup = (int)Session["kGaggleSubGroup"];

            return Self.reportViewerBLL.GetGridData(ReportVersion, kSession, kDealer, FirstMonthDate, kGaggleSubGroup);
        }
    }
}