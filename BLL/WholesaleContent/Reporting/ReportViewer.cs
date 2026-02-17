using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using LMWholesale.resource.clients;

namespace LMWholesale.BLL.WholesaleContent.Reporting
{
    public class ReportViewer
    {
        private readonly DASClient dasClient;
        private readonly WholesaleClient wholesaleClient;

        public ReportViewer()
        {
            dasClient = new DASClient();
            wholesaleClient = new WholesaleClient();
        }

        public ReportViewer(DASClient dasClient, WholesaleClient wholesaleClient)
        {
            this.dasClient = dasClient;
            this.wholesaleClient = wholesaleClient;
        }
        internal static readonly ReportViewer instance = new ReportViewer();
        public ReportViewer Self
        {
            get { return instance; }
        }

        public string GetGridData(string mode, string kSession, int kDealer, DateTime? FirstDateMonth = null, int kGaggleSubGroup = -1)
        {
            string defaultReturn =  "0 | {}";
            DAS.lmReturnValue returnValue;

            switch (mode)
            {
                case "healthReportMin":
                    returnValue = Self.dasClient.GetHealthReportMin(kSession, kDealer);
                    break;
                case "offerReport":
                    returnValue = Self.dasClient.GetOfferReport(kSession, kDealer);
                    break;
                case "healthReportSummary":
                    returnValue = Self.dasClient.GetHealthReportSummary(kSession, kDealer);
                    break;
                case "healthReportDetail":
                    returnValue = Self.dasClient.GetHealthReportDetail(kSession, kDealer, FirstDateMonth.Value);
                    break;
                case "WholesaleActiveListings":
                    returnValue = Self.dasClient.GetDealerActiveWholesale(kSession, kDealer);
                    break;
                case "WholesaleAuctionGroupActiveListings":
                    returnValue = Self.dasClient.GetAACurrentlyPosted(kSession, kGaggleSubGroup, kDealer);
                    break;
                default:
                    return "Not Configured For that report";
            }

            if (returnValue.Result == DAS.ReturnCode.LM_SUCCESS && returnValue.Data.Tables[0].Rows.Count > 0)
                return returnValue.Data.Tables[0].Rows.Count + "|" + Util.serializer.Serialize(FormatData(returnValue.Data.Tables[0], mode));
            else
                return defaultReturn;
        }

        internal string GetGridType(string mode)
        {
            Dictionary<string, string> map = new Dictionary<string, string>
            {
                { "healthReportMin", "WP-Health Report Min" },
                { "offerReport", "WP-Offer Report" },
                { "healthReportSummary", "WP-Health Report Summary" },
                { "healthReportDetail", "WP-Health Report Detail" },
                { "WholesaleActiveListings", "WP-Wholesale Active Listings" },
                { "WholesaleAuctionGroupActiveListings", "WP-Wholesale Auction Group Active Listings" }
            };

            return map[mode];
        }

        private List<Dictionary<string, string>> FormatData(DataTable data, string mode = "")
        {
            List<Dictionary<string, string>> retSet = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> auctions = null;
            HttpSessionState session = HttpContext.Current.Session;

            foreach (DataRow row in data.Rows)
            {
                Dictionary<string, string> item = new Dictionary<string, string>();
                foreach (DataColumn col in data.Columns)
                {
                    if (col.ColumnName.Contains("Active"))
                        item[col.ColumnName] = row[col].ToString() == "1" ? "Yes" : "No";
                    else if (col.ColumnName == "kWholesaleAuction" && mode != "WholesaleAuctionGroupActiveListings")
                    {
                        if (auctions == null)
                            auctions = LMWholesale.WholesaleSystem.GetAvailableAuctions((string)session["kSession"], (int)session["kDealer"], Self.wholesaleClient, 0);
                        item[col.ColumnName] = auctions.First(d => d.ContainsValue(row[col].ToString()))["WholesaleAuctionName"];
                    }
                    else if (col.ColumnName.Contains("URL"))
                        item[col.ColumnName] = "<a href=\"" + row[col].ToString() + "\"  target=\"_blank\" rel=\"noopener noreferrer\">" + row[col].ToString() + "</a>";
                    else
                        item[col.ColumnName] = row[col].ToString();
                }
                retSet.Add(item);
            }

            return retSet;
        }
    }
}