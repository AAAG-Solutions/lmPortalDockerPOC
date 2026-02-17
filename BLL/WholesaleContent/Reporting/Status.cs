using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace LMWholesale.BLL.WholesaleContent.Reporting
{
    public class Status
    {
        private DealerClient dealerClient;
        private DASClient dasClient;
        private WholesaleClient wholesaleClient;
        private WholesaleUser.WholesaleUser userBLL;

        public Status()
        {
            dealerClient = new DealerClient();
            dasClient = new DASClient();
            wholesaleClient = new WholesaleClient();
            userBLL = new WholesaleUser.WholesaleUser();
        }

        public Status(DealerClient dealerClient, DASClient dasClient, WholesaleClient wholesaleClient, WholesaleUser.WholesaleUser userBLL)
        {
            this.dealerClient = dealerClient;
            this.dasClient = dasClient;
            this.wholesaleClient = wholesaleClient;
            this.userBLL = userBLL;
        }
        internal static readonly Status instance = new Status();
        public Status Self
        {
            get { return instance; }
        }

        public List<double> GetWidgetPreferences(string session, int kDealer)
        {
            List<double> retList = new List<double>();

            Dealer.lmReturnValue returnVal = Self.dealerClient.GetUserWidgetPreferences(session, kDealer, 6);
            if (returnVal.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                if (returnVal.Data.Tables[0].Rows.Count > 0 && returnVal.Data.Tables[0].Rows[0]["WidgetSettings"].ToString() != "")
                {
                    foreach (string item in returnVal.Data.Tables[0].Rows[0]["WidgetSettings"].ToString().Split(';'))
                    {
                        if (!string.IsNullOrEmpty(item))
                            retList.Add(Convert.ToDouble(item));
                    }
                }
            }

            if (retList.Count == 0)
            {
                foreach (string item in "85;85;85;7;7;7;98;98;98;98;98;98;85;85;85;7;7;7;99;99;99".Split(';'))
                {
                    if (!string.IsNullOrEmpty(item))
                        retList.Add(Convert.ToDouble(item));
                }
            }
            return retList;
        }

        public DataTable GetInventoryHealthData(string session, int kDealer)
        {

            Dealer.lmReturnValue returnVal = Self.dealerClient.GetDashInventoryHealth(session, kDealer);
            if (returnVal.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                return returnVal.Data.Tables[0];
            }
            
            throw new Exception("Error on retrieving Inventory Health Data: \n" + returnVal.ResultString);
        }

        public string GetImportHistory(string kSession, int kDealer)
        {
            string tmpFail = "0 | {}";

            Dealer.lmReturnValue returnVal = Self.dealerClient.GetImportHistoryData(kSession, kDealer, 7);
            if (returnVal.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                DataTable imports = returnVal.Data.Tables[0];
                return imports.Rows.Count + "|" + Util.serializer.Serialize(FormatData(imports));
            }

            return tmpFail;
        }

        public Dictionary<string, string> GetExcelExport(string kSession, int kDealer, string type, string dealerName, DateTime? FirstDateMonth = null, int kGaggleSubGroup = -1)
        {
            string stringDate = DateTime.Now.ToString("yyyyMMddTHHmmss");
            DataTable dt = new DataTable();
            StringBuilder header = new StringBuilder();
            StringBuilder content = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            List<Dictionary<string, string>> auctions = null;
            HttpSessionState session = HttpContext.Current.Session;
            string gridDef = Self.userBLL.GetGridDef(kSession, GetGridType(type));
            //if (type == "")
            //    gridDef = Self.userBLL.GetGridDef(kSession, GetGridType(type));
            //else
            //    gridDef = GetExcessiveGridDef("WP-Health Report Summary");

            // Cut DealerName to 30 chars; otherwise we will have long csv names
            if (dealerName.Length > 30)
                dealerName = dealerName.Substring(0, 29);

            string fileName = $"{dealerName}_{type}_{stringDate}.csv";

            DAS.lmReturnValue returnValue;

            List<string> IncludedColumns = new List<string>();
            foreach (string option in gridDef.Split('|'))
            {
                string[] optionSplit = option.Split(':');
                header.Append(optionSplit[2] + ",");
                IncludedColumns.Add(optionSplit[1]);
            }

            switch (type)
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
                    return new Dictionary<string, string> { { "Message", "No report of that type has been configured." } };
            }
            if (returnValue.Result == DAS.ReturnCode.LM_SUCCESS)
                dt = returnValue.Data.Tables[0];
            else
                return new Dictionary<string, string> { { "Message", returnValue.ResultString } };

            

            foreach (DataRow dr in dt.Rows)
            {
                foreach (string column in IncludedColumns)
                {
                    if (column.Contains("Active"))
                        content.Append(Util.CreateCSV(dr[column].ToString() == "1" ? "Yes" : "No") + ",");
                    else if (column == "kWholesaleAuction")
                    {
                        if (auctions == null)
                            auctions = LMWholesale.WholesaleSystem.GetAvailableAuctions((string)session["kSession"], (int)session["kDealer"], Self.wholesaleClient, 0);
                        content.Append(Util.CreateCSV(auctions.First(d => d.ContainsValue(dr[column].ToString()))["WholesaleAuctionName"]) + ",");
                    }
                    else
                        content.Append(Util.CreateCSV(dr[column].ToString()) + ",");
                }

                content.Remove(content.Length - 1, 1);
                content.AppendLine();
            }

            // Combine header and content
            sb.Append(header.ToString());
            sb.AppendLine();
            sb.Append(content.ToString());

            Dictionary<string, string> rv = new Dictionary<string, string>
            {
                { "fileName", fileName },
                { "sb", sb.ToString() }
            };

            return rv;
        }

        public List<Dictionary<string, object>> FormatData(DataTable Imports)
        {
            List<Dictionary<string, object>> retObj = new List<Dictionary<string, object>>();

            foreach (DataRow row in Imports.Rows)
            {
                Dictionary<string, object> item = new Dictionary<string, object>
                {
                    { "ImportDesc", row["ImportDesc"].ToString() },
                    { "Feed", row["Feed"].ToString() },
                    { "ImportBy", row["ImportBy"].ToString() },
                    { "StartTime", row["StartTime"].ToString() },
                    { "EndTime", row["EndTime"].ToString() },
                    { "Records", row["Records"].ToString() },
                    { "Imported", row["Imported"].ToString() }
                };

                retObj.Add(item);
            }

            return retObj;
        }

        private string GetGridType(string mode)
        {
            Dictionary<string, string> map = new Dictionary<string, string>
            {
                { "healthReportMin", "WP-Health Report Min" },
                //{ "offerReport", "WP-Offer Report" },
                //{ "healthReportSummary", "WP-Health Report Summary" },
                { "healthReportDetail", "WP-Health Report Detail" },
                { "WholesaleActiveListings", "WP-Wholesale Active Listings" },
                { "WholesaleAuctionGroupActiveListings", "WP-Wholesale Auction Group Active Listings" }
            };

            return map[mode];
        }

        public string GetCurrentResults(string kSession, int kDealer)
        {
            string tmpFail = "0 | {}";

            DAS.lmReturnValue returnVal = Self.dasClient.CurrentResultsWidgetGet(kSession, kDealer);
            if (returnVal.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable currentResults = returnVal.Data.Tables[0];
                return currentResults.Rows.Count + "|" + Util.serializer.Serialize(FormatCurrentResults(currentResults));
            }

            return tmpFail;
        }

        public List<Dictionary<string, object>> FormatCurrentResults(DataTable CurrentResults)
        {
            List<Dictionary<string, object>> retObj = new List<Dictionary<string, object>>();

            foreach (DataRow row in CurrentResults.Rows)
            {
                Dictionary<string, object> item = new Dictionary<string, object>();
                item["Auction"] = row["WholesaleAuctionName"].ToString();
                item["TotalListed"] = row["TotalListed"].ToString();
                item["SoldMTD"] = row["SoldMTD"].ToString();
                item["ErrorCount"] = row["ErrorCount"].ToString();
                retObj.Add(item);
            }

            return retObj;
        }

        //private string GetExcessiveGridDef(string grid)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (grid == "WP-Health Report Summary")
        //    {
        //        // This Grid Definition cannot live in
        //        sb.Append(":::40|:Photo::80|:VIN::140|<:MakeModelStyle:Make/Model/Style:200|:MotorYear:Year:50:number|:Status::85|");
        //        sb.Append(":InvDays:Age:50:number|:Miles:Mileage:55:number|:InvListPrice:Retail Listing:80:number|:InternetPrice:Retail Internet:80:number|");
        //        sb.Append(":MMRGoodPrice:MMR:60:number|:StockNumber:Stock #:150|:WholesaleStartPrice:Wholesale Start Price:90:number|:WholesaleFloor:Wholesale Reserve:80:number|");
        //        sb.Append(":WholesaleBuyNow:Wholesale Buy Now:80:number|:InvCost:Cost:50:number|:VehicleGrade:Vehicle Grade:100|:WholesaleStatus:Auction Status:125|");
        //        sb.Append(":ConditionReportLink:Condition Report:110|:LotLocation:Lot Location:150|*:ErrorMsg:Message:150|!:kListing:kListing:50|!:kListingStatus:kListingStatus:50");
        //    }
        //
        //    return sb.ToString();
        //}
    }
}