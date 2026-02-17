using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.SessionState;

using LMWholesale.Common;
using LMWholesale.BLL.WholesaleUser;
using LMWholesale.resource.clients;
using LMWholesale.resource.model.Wholesale;

namespace LMWholesale.WholesaleContent.Auction
{
    public partial class MultiEnd : lmPage
    {
        private readonly WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Auction.MultiEnd multiEndBLL;
        private readonly WholesaleClient wholesaleClient;
        private readonly WholesaleData.Logger logger;

        public MultiEnd()
        {
            userBLL = userBLL ?? new WholesaleUser();
            multiEndBLL = multiEndBLL ?? new BLL.WholesaleContent.Auction.MultiEnd();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            logger = logger ??  new WholesaleData.Logger($@"{WebConfigurationManager.AppSettings["LogDirectory"]}\\logs\\", bool.Parse(WebConfigurationManager.AppSettings["DebugMode"])); ;
        }

        public static MultiEnd Self
        {
            get { return instance; }
        }
        private static readonly MultiEnd instance = new MultiEnd();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "MultiEnd Wholesale";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                jsGridBuilder endGrid = new jsGridBuilder()
                {
                    MethodURL = "MultiEnd.aspx/EndWholesaleSearch",
                    HTMLElement = "jsGrid",
                    Filtering = false,
                    Sorting = false,
                    PageSize = int.Parse(((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["filters"].ToString()))["ItemsPerPage"].ToString())
                };

                Dictionary<string, string> auctionStrings = Self.multiEndBLL.GetAuctionStrings();

                endGrid.SetFieldListFromGridDef(auctionStrings["searchVehicleGridColumns"], "", true);
                WholesaleSystem.PopulateList(auctionStrings["quickSelect"], "", "lstSelect", '|', "0");

                // On first load, we want to always default to page 1; otherwise, this page acts weird
                Session["MultiFirstLoad"] = true;

                // Save to Session so we can use it else where
                Session["MEWAuctions"] = auctionStrings["auctionList"];

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "endWholesaleGrid", endGrid.RenderGrid());
            }
        }

        [WebMethod(EnableSession = true)]
        public static string EndWholesaleSearch(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            //Dictionary<string, object> sortFilter = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);

            // Gather Filter and Advanced Filters
            InventoryFilter.MultiPageFilter multiPageFilter = new InventoryFilter.MultiPageFilter(
                new InventoryFilter.Filter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["filters"].ToString())),
                new InventoryFilter.AdvancedFilter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["advancedFilter"].ToString()))
                );

            if (bool.Parse(Session["MultiFirstLoad"].ToString()))
            {
                multiPageFilter.PageNumber = 1;
                Session["MultiFirstLoad"] = false;
            }

            // Default to Listed on Any Platform
            multiPageFilter.ListingStatus = -1;

            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleMultiListingsGet(multiPageFilter);

            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable vehicles = returnValue.Data.Tables[0];
                string count = returnValue.Values.GetValue("TotalItems", "0");
                string data = Util.serializer.Serialize(FormatData(vehicles, Session["MEWAuctions"].ToString()));

                return $"{count}|{data}";
            }

            // If we fail, return nothing
            return "0|{}";
        }

        [WebMethod(Description = "Submit multiple Vehicles to unlist from multiple auctions")]
        public static Dictionary<string, object> RemoveFromMultipleAuctions(string vehicles)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            Dictionary<string, object> vehicleInfo = (Dictionary<string, object>)Util.serializer.DeserializeObject(vehicles);

            if (String.IsNullOrEmpty(Convert.ToString(Session["kSession"])))
                WholesaleUser.ClearUser();

            if (String.IsNullOrEmpty(Convert.ToString(Session["kDealer"])))
            {
                IsSuccess = false;
                Message = "Selected dealer is required";
                return ReturnResponse();
            }

            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            if(Convert.ToBoolean(vehicleInfo["MarkUnavailable"].ToString()) && !Self.multiEndBLL.MarkVehicleUnavailable(kSession, Convert.ToInt32(vehicleInfo["kListing"])))
            {
                IsSuccess = false;
                Message = "Something went wrong! Please contact support!";
                return ReturnResponse();
            }

            if (!Self.multiEndBLL.RemoveFromMultipleAuctions(kSession, kDealer, vehicleInfo))
            {
                IsSuccess = false;
                Message = "Something went wrong! Please contact support!";
            }

            return ReturnResponse();
        }

        private static List<Dictionary<string, object>> FormatData(DataTable dt, string auctionString)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in dt.Columns)
                    dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));

                List<string> auctionList = new List<string>(auctionString.Split(','));
                int count = 0;
                bool isListed = false;

                foreach (string auction in auctionList)
                {
                    string[] info = auction.Split('|');
                    if (info[0] == "")
                        continue;

                    string auctionName = info[0];
                    string isEnabled = row[$"isListedon{auctionName.Replace(" ", "")}"].ToString() == "1" ? "" : "disabled";
                    string checkBox = "<div style='text-align-last:center;'>";
                    checkBox += $"<input id='{info[0]}Check_{row["kListing"]}' type='checkbox' style='font-weight:bold;' value='{info[1]}' {isEnabled}/></div>";

                    if (row[$"isListedon{auctionName.Replace(" ", "")}"].ToString() == "1")
                    {
                        isListed = true;
                        count++;
                    }

                    dict[info[0]] = checkBox;
                }

                // Adding a 'VIN' ID so we can search for a specific VIN or partial VIN in the grid
                dict["YYMVin"] = $"<b>{row["Year"]} {row["Make"]} {row["Model"]}</b><br/><div id='VinNum_{row["kListing"]}'>{row["Vin"]}</div>";

                //if (count < 1)
                //    continue;
                 if (isListed)
                    returnList.Add(dict);
            }

            return returnList;
        }

        public class ErrorController : ApiController
        {
            [HttpPost]
            [Route("ErrorHandler")]
            public void ErrorHandler(ErrorData errorData)
            {
                HttpSessionState Session = HttpContext.Current.Session;

                // Build Exception log ling
                StringBuilder ex = new StringBuilder("Exception - [ ");
                ex.Append($"Page - 'MultiEnd' |");
                ex.Append($"kPerson - {Session["kPerson"]} |");
                ex.Append($"kDealer - {Session["kDealer"]} |");
                ex.Append($"Message - {errorData.message} |");
                ex.Append($"Source - {errorData.source} |");
                ex.Append($"Lineno - {errorData.lineno} | ");
                ex.Append($"Colno - {errorData.colno} |");
                ex.Append($"Error - {errorData.error} |");

                Self.logger.LogLine((string)Session["kSession"], ex.ToString());
            }
        }

        public class ErrorData
        {
            public string message { get; set; }
            public string source { get; set; }
            public int lineno { get; set; }
            public int colno { get; set; }
            public string error { get; set; }
        }
    }
}