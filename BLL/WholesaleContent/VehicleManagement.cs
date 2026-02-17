using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

using LMWholesale.resource.clients;
using LMWholesale.resource.model.Wholesale;


namespace LMWholesale.BLL.WholesaleContent
{
    public class VehicleManagement
    {
        private AuthenticationClient authClient;
        private WholesaleClient wholesaleClient;
        private ListingClient listingClient;
        private LookupClient lookupClient;
        private DealerClient dealerClient;
        private WholesaleUser.WholesaleUser userBLL;

        private static readonly Dictionary<int, string> statusMapping = new Dictionary<int, string>
        {
            // Inventory Status
            { 0, "ALL" },
            { 1, "StatusAvailable" },
            { 2, "StatusUnavailable" },
            { 3, "HOLD" },
            { 4, "StatusDemo" },
            { 5, "RETURNED" },
            { 6, "StatusSalePending" },
            { 7, "StatusSold" },
            { 9, "StatusInTransit" }
        };

        private static readonly Dictionary<int, string> typeMapping = new Dictionary<int, string>
        {
            { 0, "ALL" },  // ALL
            { 31, "NEW" }, // New
            { 32, "TypeManufacturerCertified" }, // Certified
            { 33, "TypeDealerCertified" }, // Dealer Certified
            { 30, "TypePreOwned" }  // Pre-Owned
        };

        public VehicleManagement()
        {
            authClient = authClient ?? new AuthenticationClient();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            listingClient = listingClient ?? new ListingClient();
            lookupClient = lookupClient ?? new LookupClient();
            dealerClient = dealerClient ?? new DealerClient();
            userBLL = userBLL ?? new WholesaleUser.WholesaleUser();
        }

        public VehicleManagement(AuthenticationClient authClient, WholesaleClient wholesaleClient, ListingClient listingClient,
                                LookupClient lookupClient, DealerClient dealerClient, WholesaleUser.WholesaleUser userBLL)
        {
            this.authClient = authClient;
            this.wholesaleClient = wholesaleClient;
            this.listingClient = listingClient;
            this.lookupClient = lookupClient;
            this.dealerClient = dealerClient;
            this.userBLL = userBLL;
        }

        internal static readonly VehicleManagement instance = new VehicleManagement();
        public VehicleManagement Self
        {
            get { return instance; }
        }

        public string GetWholesaleInventory(HttpSessionState Session, Dictionary<string, object> filter)
        {
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            // Basic Inventory Filter
            InventoryFilter.Filter inventoryFilter;
            if (Session["filters"] != null)
                inventoryFilter = new InventoryFilter.Filter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["filters"].ToString()));
            else
                inventoryFilter = new InventoryFilter.Filter(kSession, kDealer);

            inventoryFilter.ItemsPerPage = int.Parse(filter["pageSize"].ToString());
            inventoryFilter.PageNumber = int.Parse(filter["pageIndex"].ToString());

            if (filter.ContainsKey("TextFilter"))
                inventoryFilter.TextFilter = filter["TextFilter"].ToString();

            if (filter.ContainsKey("sortField"))
                inventoryFilter.Sort = filter["sortField"].ToString();
            if (filter.ContainsKey("sortOrder") && filter["sortOrder"].ToString().Contains("desc"))
                inventoryFilter.Sort += " desc";

            // Advanced Inventory Filter
            InventoryFilter.AdvancedFilter advancedFilter;
            if (Session["advancedFilter"] != null)
                advancedFilter = new InventoryFilter.AdvancedFilter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["advancedFilter"].ToString()));
            else if (filter.ContainsKey("advancedFilter"))
                advancedFilter = new InventoryFilter.AdvancedFilter((Dictionary<string, object>)filter["advancedFilter"]);
            else
                advancedFilter = new InventoryFilter.AdvancedFilter();

            Dictionary<string, object> selected =
                (Dictionary<string, object>)Util.serializer.DeserializeObject((string)(Session["SelectedVH"] ?? ""));

            Session["SelectedVH"] = Util.serializer.Serialize(new Dictionary<string, object>(){ { "kListing", "" }, { "page", int.Parse(filter["pageIndex"].ToString()) } });

            // Save to Session
            Session["filters"] = Util.serializer.Serialize(inventoryFilter);
            Session["advancedFilter"] = Util.serializer.Serialize(advancedFilter);

            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.GetWholesaleWP(inventoryFilter, advancedFilter);
            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable vehicles = returnValue.Data.Tables[0];

                return returnValue.Values.GetValue("TotalItems", "0") + "|" + Util.serializer.Serialize(FormatData(vehicles, kSession, kDealer));
            }

            // If we fail, return nothing
            return "0 | {}";
        }

        public InventoryFilter.AdvancedFilter GetAdvancedFilters(string kSession, int kDealer, int kPerson)
        {
            InventoryFilter.AdvancedFilter advancedFilter = new InventoryFilter.AdvancedFilter();
            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleUserFilterGet(kSession, kDealer, kPerson);
            int filterExists = 0;
            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataRow row = returnValue.Data.Tables[0].Rows[0];

                // Set checkbox values
                PropertyInfo[] infos = advancedFilter.GetType().GetProperties();
                filterExists = int.Parse(row["FilterExists"].ToString());

                foreach (PropertyInfo prop in infos)
                {
                    // DropDowns
                    if (prop.Name == "LotLocation")
                    {
                        advancedFilter.LotLocation = row[prop.Name].ToString();
                        continue;
                    }
                    else if (prop.Name == "ListingStatus")
                    {
                        advancedFilter.ListingStatus = int.Parse(row[prop.Name].ToString());
                        continue;
                    }
                    else if (prop.Name == "InspectionStatus")
                    {
                        advancedFilter.InspectionStatus = int.Parse(row[prop.Name].ToString());
                        continue;
                    }
                    else
                    {
                        if (row[prop.Name].ToString() == "1")
                            advancedFilter.GetType().GetProperty(prop.Name).SetValue(advancedFilter, 1);
                    }
                }
            }

            // Since this is the first time we are loading advanced filters, we can check for account default filters
            if (filterExists == 0)
            {
                Lookup.lmReturnValue defaultResult = Self.lookupClient.WholesaleUserDefaultUserGet(kSession, kDealer);
                if (defaultResult.Result == Lookup.ReturnCode.LM_SUCCESS)
                {
                    DataRow dr = defaultResult.Data.Tables[0].Rows[0];
                    string type = typeMapping[int.Parse(dr["StockFilter"].ToString())];
                    string status = statusMapping[int.Parse(dr["StatusFilter"].ToString())];

                    if (!type.Equals("ALL") && !type.Equals("NEW"))
                        advancedFilter.GetType().GetProperty(type).SetValue(advancedFilter, 1);

                    if (!status.Equals("ALL") && !status.Equals("HOLD") && !status.Equals("RETURNED"))
                        advancedFilter.GetType().GetProperty(status).SetValue(advancedFilter, 1);
                }
            }

            // Return empty row with no table definition if we fail for some reason
            return advancedFilter;
        }

        public bool SetAdvancedFilters(HttpSessionState Session, Dictionary<string, object> filter, ref string returnString)
        {
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            string kPerson = (string)Session["kPerson"];

            // #TODO: Need to clean this up
            AdvancedFilter advFilters = new AdvancedFilter(kSession, kDealer, int.Parse(kPerson));

            // Loop through filter keys and assign to proper model field
            string[] keys = filter.Keys.ToArray();
            foreach (string key in keys)
            {
                PropertyInfo prop = advFilters.GetType().GetProperty(key);
                prop.SetValue(advFilters, Convert.ChangeType(filter[key], prop.PropertyType), null);
            }

            Wholesale.lmReturnValue result = Self.wholesaleClient.WholesaleUserFilterSet(Util.serializer.Serialize(advFilters));

            if (result.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                // To get around not passing the advanced filters when sorting,
                // we need to override the advanced filter set that is in Session
                Session["advancedFilter"] = Util.serializer.Serialize(advFilters.FormatToSession());

                returnString = "Successfully saved user specific filters!";
                return true;
            }
            else
            {
                returnString = "There was a problem saving advanced filters! Please contact support!";
                return false;
            }
        }

        public bool SetDefaultFilters(HttpSessionState Session, ref string returnString)
        {
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            // Create a new instance of filters and advanced filters
            InventoryFilter.AdvancedFilter advancedFilter = new InventoryFilter.AdvancedFilter();
            InventoryFilter.Filter filters = new InventoryFilter.Filter(kSession, kDealer);

            Lookup.lmReturnValue defaultResult = Self.lookupClient.WholesaleUserDefaultUserGet(kSession, kDealer);
            if (defaultResult.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                DataRow dr = defaultResult.Data.Tables[0].Rows[0];
                string type = typeMapping[int.Parse(dr["StockFilter"].ToString())];
                string status = statusMapping[int.Parse(dr["StatusFilter"].ToString())];

                if (!type.Equals("ALL") && !type.Equals("NEW"))
                    advancedFilter.GetType().GetProperty(type).SetValue(advancedFilter, 1);

                if (!status.Equals("ALL") && !status.Equals("HOLD") && !status.Equals("RETURNED"))
                    advancedFilter.GetType().GetProperty(status).SetValue(advancedFilter, 1);

                Session["advancedFilter"] = Util.serializer.Serialize(advancedFilter);
                Session["filters"] = Util.serializer.Serialize(filters);

                returnString = "Successfully applied Account Default Filters!";
                return true;
            }
            else
            {
                returnString = "Something went wrong setting default filters! Please Contact Support.";
                return false;
            }
        }

        public string SellUnsellVehicle(string kSession, int kListing, int kInventoryStatus)
        {
            Listing.lmReturnValue returnValue = Self.listingClient.SellUnsellVehicle(kSession, kListing, kInventoryStatus);

            if (returnValue.Result == Listing.ReturnCode.LM_SUCCESS)
                return "";
            else
                return returnValue.ResultString;
        }

        public void GetListingLotLocationList(string kSession, int kDealer, ref string lotLocation)
        {
            Dealer.lmReturnValue lotList = Self.dealerClient.LotLocationListGet(kSession, kDealer);
            StringBuilder lotString = new StringBuilder("[]0:Any Lot Location|");
            if (lotList.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                if (lotList.Data != null)
                {
                    DataTable ll = lotList.Data.Tables[0];
                    foreach (DataRow row in ll.Rows)
                        lotString.Append($"{row["InvLotLocation"]}:{row["InvLotLocation"]}|");
                }
            }
            LMWholesale.WholesaleSystem.PopulateList(lotString.ToString(), "Any Lot Location", "lstLotLocation", '|', lotLocation);
        }

        public string VehicleAuctionInfoGet(string kSession, int kListing, int kDealer, int kWholesaleAuction)
        {
            Regex success = new Regex(@"success", RegexOptions.IgnoreCase);
            Dictionary<string, string> auctionStatus = new Dictionary<string, string>
            {
                { "0", "Not Listed" },
                { "1", "Scheduled" },
                { "2", "Active" },
                { "3", "Cancelling" },
                { "4", "Pending" }
            };

            Listing.lmReturnValue auctioInfo = Self.listingClient.ListingAuctionDataGet(kSession, kDealer, kListing);
            if (auctioInfo.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                Dictionary<string, string> returnDict = new Dictionary<string, string>
                {
                    { "autoLaunch", "False" },
                    { "blackOutWindow", "No" },
                    { "errorMsg", "Unable to fetch vehicle data" },
                    { "mmrReservePrice", "0" },
                    { "mmrBuyNowPrice", "0" },
                    { "mmr", "0" },
                    { "startDate", "N/A" },
                    { "endDate", "N/A" },
                    { "status", "N/A" },
                    { "relistCount", "0" },
                    { "conditionReport", "No Condition Report" },
                    { "startPrice", "0" },
                    { "reservePrice", "0" },
                    { "buyNow", "0" },
                    { "listingType", "N/A" },
                    { "listingCategory", "N/A" },
                    { "posted", "N/A" }
                };

                DataRow[] drs = auctioInfo.Data.Tables[0].Select($"kWholesaleAuction = {kWholesaleAuction}");
                if (drs.Length == 0)
                    return Util.serializer.Serialize(returnDict);

                DataRow dr = drs[0];
                int mmr = int.Parse(dr["MMRPrice"].ToString(), 0);
                string mmrReservePrice = "% MMR: N/A";
                string mmrBuyNowPrice = "% MMR: N/A";

                if (mmr > 1)
                {
                    string reservePrice = dr["ReservePrice"].ToString() == "" ? "0" : dr["ReservePrice"].ToString();
                    string buyNow = dr["BuyNow"].ToString() == "" ? "0" : dr["BuyNow"].ToString();

                    mmrReservePrice = $"{Math.Ceiling(int.Parse(reservePrice, 0) * 100.00 / mmr)}% of MMR";
                    mmrBuyNowPrice = $"{Math.Ceiling(int.Parse(buyNow, 0) * 100.00 / mmr)}% of MMR";
                }

                returnDict["autoLaunch"] = dr["AutoLaunch"].ToString() == "1" ? "True" : "False";
                returnDict["blackOutWindow"] = dr["IsBlackOut"].ToString() == "1" ? "Yes" : "No";
                returnDict["errorMsg"] = (dr["ErrorMsg"].ToString() == "" || success.IsMatch(dr["ErrorMsg"].ToString())) ? "N/A" : dr["ErrorMsg"].ToString();
                returnDict["mmrReservePrice"] = mmrReservePrice;
                returnDict["mmrBuyNowPrice"] = mmrBuyNowPrice;
                returnDict["mmr"] = mmr.ToString();
                returnDict["startDate"] = dr["StartDate"].ToString();
                returnDict["endDate"] = dr["EndDate"].ToString();
                returnDict["status"] = auctionStatus[dr["Status"].ToString()];
                returnDict["relistCount"] = dr["RelistCount"].ToString();
                returnDict["conditionReport"] = (dr["ConditionReportDate"].ToString() == "01-01-1970") ? "No Condition Report" : dr["ConditionReportDate"].ToString();
                returnDict["startPrice"] = dr["StartPrice"].ToString();
                returnDict["reservePrice"] = dr["ReservePrice"].ToString();
                returnDict["buyNow"] = dr["BuyNow"].ToString();
                returnDict["listingType"] = dr["ListingType"].ToString();
                returnDict["listingCategory"] = dr["ListingCategory"].ToString();
                returnDict["posted"] = dr["Posted"].ToString();

                return Util.serializer.Serialize(returnDict);
            }

            // If we fail for some reason, return an empty dict
            return Util.serializer.Serialize(new Dictionary<string, string>());
        }

        public Dictionary<string, string> ExportInventory(string kSession, int kDealer, string dealerName, int kPerson)
        {
            string stringDate = DateTime.Now.ToString("yyyyMMddTHHmmss");
            DataTable dt = new DataTable();
            StringBuilder header = new StringBuilder();
            StringBuilder content = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            // Cut DealerName to 30 chars; otherwise we will have long csv names
            if (dealerName.Length > 30)
                dealerName = dealerName.Substring(0, 29);

            string fileName = $"{dealerName}_Inventory_{stringDate}.csv";

            // Default PageSize 500, customizable depending on request
            // TODO: Gather jsgrid filters for export so it matches visual jsgrid?
            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.GetWholesaleWP(new InventoryFilter.Filter(kSession, kDealer, 1, int.MaxValue, ""), new InventoryFilter.AdvancedFilter());

            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                dt = returnValue.Data.Tables[0];

            // TODO: Might be a good idea later to implement customizable list of columns
            // Wholesale.UserSetting user = ServiceAuth.UserSettings;
            // Wholesale.lmReturnValue returnValue = user.GetUserSettings(sessid, kDealer);

            // Just get general user VehicleManagement Columns
            string WholesaleGridColumns = Self.userBLL.GetGridDef(kSession, "WP-ExportInventory", kDealer, kPerson);
            IEnumerable<string[]> lstColumns = WholesaleGridColumns.Split('|').Select(column => column.Split(':'));
            List<string> auctionStatus = new List<string>
            {
                "OVEStatus", "OpenLaneStatus", "AutoAuctionStatus",
                "ExchangeStatus", "SmartAuctionStatus", "TurnAuctionsStatus",
                "AuctionEdgeStatus", "ACVAuctionStatus", "eDealerDirectStatus",
                "COPARTStatus", "IAAStatus", "AuctionSimplifiedStatus",
                "IASStatus", "AuctionOSStatus", "CarmigoStatus", "CarOfferStatus", "RemarketingPlusStatus"
            };
            int count = 0;

            foreach (DataRow dr in dt.Rows)
            {
                foreach(string[] column in lstColumns)
                {
                    if (column[1] == "")
                        column[1] = column[0];

                    string value = "";
                    if (column[0] == "ListingStatus")
                        foreach (string status in auctionStatus)
                        {
                            if (dr[status].ToString() != "0")
                            {
                                value = "Listed";
                                break;
                            }
                            else
                                value = "Not Listed";
                        }
                    else
                        value = DetermineColumnValue(dr, column[0].ToString());

                    content.Append(Util.CreateCSV(value) + ",");
                    if (count < lstColumns.Count())
                    {
                        header.Append(column[1] + ",");
                        count += 1;
                    }
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

        private List<Dictionary<string, object>> FormatData(DataTable dt, string kSession, int kDealer)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();
            HttpContext context = HttpContext.Current;
            HttpSessionState Session = context.Session;

            // Search DealerProducts to make sure that we have 'AutoGrade' enabled
            bool hasAutoGrade = ((List<string>)Session["UserPermissions"]).Where(x => x.Contains("AutoGrade")).Count() > 0;

            // Visible Decimal fields
            string[] decimalFields = { "WholesaleStartPrice", "WholesaleFloor", "WholesaleBuyNow" };
            string[] intFields = { "InvDays", "Miles", "WholesaleStartPrice", "WholesaleBuyNow", "InvCost", "MMRDisplay", "InvListPrice", "InternetPrice" };

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    if (Convert.ToString(row[col]) == "vin2")
                        continue;

                    // If string contains trailing cents
                    // Else if string is a number field and equals to 0 || ""
                    // TODO: Not sure if we want to remove this since we are returning ints instead of varchar("##.00")
                    if (Regex.IsMatch(row[col].ToString(), "[0-9]+[.][0-9][0-9]"))
                    {
                        if (decimalFields.Any(Convert.ToString(col).Contains))
                        {
                            dict[col.ColumnName] = Convert.ToInt32(Regex.Replace(Convert.ToString(row[col]), "([.][0-9][0-9])", ""));
                        }
                    }
                    else if ((row[col].ToString() == "" || row[col].ToString() == "0")
                      && intFields.Any(Convert.ToString(col).Contains))
                    {
                        dict[col.ColumnName] = 0;
                    }
                    else
                        dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));
                }
                string kListing = row["kListing"].ToString();

                // Format Photo and Count
                string photoCount = Util.cleanString((Convert.ToString(row["PhotoCnt"])));
                string firstPhoto = Util.cleanString((Convert.ToString(row["FirstPhoto"])));
                string displayCount = $"<div class='gridPhotoCount'>{photoCount}</div>";
                string displayPhoto = $"<div style='position:relative;'>{displayCount}<a title=\"{row["StockNumber"]} PhotoGallery\" style=\"cursor:pointer;\" onclick=\"javascript: window.open('{context.Request.Url.GetLeftPart(UriPartial.Authority)}/WholesaleContent/Vehicle/PhotoGallery.aspx?kListing={kListing}'), '', 'width=500,height=500'\">{firstPhoto}</a></div>";

                if (photoCount == "0" || photoCount == "")
                    displayPhoto = "";

                dict["Photo"] = displayPhoto;

                string carfax = $"<a target='_blank' href='{row["CarfaxPublicLink"]}'><br/><img style='height:25px;' src='/Images/carfax.gif' title='CARFAX Home Page' id='CarFaxLink_{row["StockNumber"]}' /></a>";
                // Adding a child to VIN so we can determine if we should navigate to Vehicle Update screen
                // or it is just someone copying the VIN
                dict["VIN"] = string.Format("<span id='vin_{0}'>{1}</span><br/><div style='text-align:-webkit-center;'>{2}</div>", row["VIN"].ToString(), row["VIN"].ToString(), carfax);

                // Format Make/Model/Style to navigate to UpdateVehicle
                string year = Util.cleanString((Convert.ToString(row["MotorYear"])));
                string makeName = Util.cleanString((Convert.ToString(row["Make"])));
                string modelName = Util.cleanString((Convert.ToString(row["Model"])));
                string styleName = Util.cleanString((Convert.ToString(row["Style"])));
                dict["MakeModelStyle"] =
                    $"<a class='updateLink' style=\"color: #000000\" href='Vehicle/Update.aspx?kListing={kListing}' onclick='javascript: toggleLoading(true, \"Loading Vehicle Information...\");'>{makeName} {modelName}<br>{styleName}</a>";

                dict["LcInfoBlock"]=
                    $"<a class='updateLink' style=\"color: #000000\" href='Vehicle/Update.aspx?kListing={kListing}' onclick='javascript: toggleLoading(true, \"Loading Vehicle Information...\");'>{year} {makeName} {modelName}<br>{styleName}<br>Vin: {row["VIN"].ToString()}<br>Stock#: {row["StockNumber"].ToString()}<br>#Pics: {photoCount}&nbsp;&nbsp;Ext: {row["ExteriorColor"].ToString()}</a>";

                // Format Vehicle Status to stop yelling at us
                dict["Status"] = Util.FormatString(Util.cleanString(Convert.ToString(row["Status"])), "lowercase");

                // Build Ellipsis menus
                string auctions = BuildAuctionMenu(kListing, row);
                dict["WholesaleStatus"] = auctions;
                dict[""] = BuildVehicleActionMenu(kListing, dict["Status"].ToString(), auctions);
                dict["ErrorMsg"] = BuildAuctionErrorMsgs(kListing, row);

                // Condition Report Link
                dict["ConditionReportLink"] = BuildConditionReport(kListing, row);

                dict["VehicleGrade"] = Self.userBLL.GradeSet(row, hasAutoGrade);

                returnList.Add(dict);
            }

            return returnList;
        }

        private string BuildVehicleActionMenu(string kListing, string status, string auctions)
        {
            string actionIconMenu = "";
            string onClickString = $"onclick =\"toggleCssClass([['{kListing}_actions', 'openActionsContent'],['{kListing}_action_menu','openVehicleActions']]); return false;\"";
            string vehicleActions = $@"
            <div class='actionsBar'>
                <button id='{kListing}_action_menu' class='vehicleActions' {onClickString}></button>
                <div id='{kListing}_actions' class='actionsContent actionBackground' style='padding-top: 5px;'>
                    VEHICLE_ACTIONS
                </div>
            </div>";

            string style = "cursor:pointer;";
            if (auctions.Contains("Not Listed"))
                style = "opacity:0.5;pointer-events:none !important;";

            actionIconMenu += $"<a style='cursor:pointer;' onclick=\"javascript: toggleLoading(true, 'Loading Vehicle Information...'); window.location.href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}'\"><img style='height:|LCDim|;width:|LCDim|;padding:3px;' src='/Images/fa-icons/edit.svg' title='Update Vehicle' id='{kListing}_UpdateVehicle' /></a>";
            actionIconMenu += $"<a style='cursor:pointer;' onclick='javascript: window.location.href=\"/WholesaleContent/Vehicle/ManagePhotos.aspx?kListing={kListing}\"'><img style='height:|LCDim|;width:|LCDim|;padding:3px;' src='/Images/fa-icons/camera.svg' title='Manage Photos' id='{kListing}_ManagePhotos' /></a>";
            actionIconMenu += $"<a style='cursor:pointer;|LCHIDE|' onclick=\"javascript: window.location.href='/WholesaleContent/Vehicle/StartWholesale.aspx?kListing={kListing}'\"><img style='height:35px;width:35px;padding:3px' src='/Images/fa-icons/calendar-plus.svg' title='Start Wholesale' id='{kListing}_StartWholesale' /></a>";
            actionIconMenu += $"<a style='{style}|LCHIDE|' onclick=\"javascript: window.location.href='/WholesaleContent/Vehicle/EndWholesale.aspx?kListing={kListing}'\"><img style='height:35px;width:35px;padding:3px;' src='/Images/fa-icons/calendar-minus.svg' title='End Wholesale' id='{kListing}_EndWholesale' /></a>";

            if (status == "Sold")
                actionIconMenu += $"<a style='cursor:pointer;|LCHIDE|' onclick=\"javascript:MarkVehicle({kListing}, 1); return false;\"><img style='height:35px;width:35px;padding:3px;' src='/Images/fa-icons/sack-xmark.svg' title='Mark Vehicle as Available' id='{kListing}_UnSellVehicle' /></a>";
            else
                actionIconMenu += $"<a style='cursor:pointer;|LCHIDE|' onclick=\"javascript:MarkVehicle({kListing}, 7); return false;\"><img style='height:35px;width:35px;padding:3px;' src='/Images/fa-icons/sack-dollar.svg' title='Mark Vehicle as Sold' id='{kListing}_SellVehicle' /></a>";

            actionIconMenu += $"<a style='|LCHIDE|' href='/WholesaleContent/Vehicle/Delete.aspx?kListing={kListing}'><img style='height:35px;width:35px;padding:3px;' src='/Images/fa-icons/trash.svg' title='Delete Vehicle' id='{kListing}_DeleteVehicle' /></a>";
            actionIconMenu += $"<a style='cursor:pointer;' onclick='javascript: window.location.href=\"/WholesaleContent/Vehicle/InspectVehicle.aspx?kListing={kListing}\"'><img style='height:|LCDim|;width:|LCDim|;padding:3px;' src='/Images/fa-icons/file.svg' title='Condition Report' id='{kListing}_CRVehicle' /></a>";
            actionIconMenu += $"<a style='cursor:pointer;|LCHIDE|' onclick='javascript: window.location.href=\"/WholesaleContent/Vehicle/ManageOverrides.aspx?kListing={kListing}\"'><img style='height:35px;width:35px;padding:3px;' src='/Images/fa-icons/list-check.svg' title='Manage Overrides' id='{kListing}_Overrides' /></a>";
            vehicleActions = vehicleActions.Replace("VEHICLE_ACTIONS", actionIconMenu);

            return vehicleActions;
        }

        private string BuildConditionReport(string klisting, DataRow row)
        {
            string returnValue = "";
            string link = "";
            bool hasCr = Convert.ToInt32(row["HasConditionReport"]) != 0;
            bool useDefault = Convert.ToInt32(row["UseDefaultCr"]) != 0;

            if (hasCr)
            {
                string internalCr = Convert.ToString(row["InternalConditionURL"]);
                string externalCr = Convert.ToString(row["ExternalConditionURL"]);
                //string pdfReport = Convert.ToString(row["ConditionReportURL"]);
                string date = Convert.ToString(row["ConditionReportDate"]);

                string ilink = $"<a ID='{klisting}_icrl' href='{internalCr}' target='_blank'><span>LMI Report</span></a><br/>";
                string elink = $"<a ID='{klisting}_ecrl' href='{externalCr}' target='_blank'><span>External Report</span></a><br/>";
                string dlink = $"<a ID='{klisting}_icrl' href='{internalCr}' target='_blank'><span>Default Condition Report Exists</span></a><br/>";

                if (externalCr != "")
                {
                    link += elink;
                    if (internalCr != "")
                        link += useDefault ? dlink : ilink;
                    else
                        link += useDefault ? dlink : ilink;
                }
                else
                    link += useDefault ? dlink : ilink;

                returnValue = $@"
                <img style='width: 98px;' src='/Images/condition_report_btn.jpg'/>
                <div>Report Date: <br/>{date}</div>
                {link}";
            }

            return returnValue;
        }

        private string BuildAuctionMenu(string kListing, DataRow row)
        {
            string auctionMenu = "";
            string auctionIcons = "";
            Dictionary<string, List<string>> auctionListIcons = new Dictionary<string, List<string>>()
            {
                { "OVEStatus", new List<string> { "1", "ove_ws.gif" } },
                { "OpenLaneStatus", new List<string> { "4", "openlane_ws.png" } },
                { "AutoAuctionStatus", new List<string> { "5", "aa_ws.gif" } },
                { "ExchangeStatus", new List<string> { "8", "exchange_ws.png" } },
                { "SmartAuctionStatus", new List<string> { "2", "smartauction_ws.png" } },
                { "TurnAuctionsStatus", new List<string> { "10", "turnauctions_ws.png" } },
                { "AuctionEdgeStatus", new List<string> { "7", "auctionEdge_ws.png" } },
                { "ACVAuctionStatus", new List<string> { "11", "ACVAuctions_ws.png" } },
                { "eDealerDirectStatus", new List<string> { "12", "eDealerDirect_ws.png" } },
                { "COPARTStatus", new List<string> { "6", "copart_ws.gif" } },
                { "IAAStatus", new List<string> { "13", "IAA_ws.png" } },
                { "AuctionSimplifiedStatus", new List<string> { "14", "ASimplified_ws.png" } },
                { "IASStatus", new List<string> { "15", "IAS_ws.png" } },
                { "AuctionOSStatus", new List<string> { "16", "AuctionOS_ws.png" } },
                { "CarmigoStatus", new List<string> { "17", "carmigo_png.png" } },
                { "CarOfferStatus", new List<string> { "18", "caroffer_png.png" } },
                { "RemarketingPlusStatus", new List<string> { "19", "remarketingplus_ws.png" } }
            };

            // Additional Auctions if list exceeds 3
            string additionalAuctionsCount = $"<div class='additionalAuctions'><a href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}&FocusAuction={auctionListIcons.First().Key.Replace("Status","")}'>ADDITIONAL_AUCTIONS</a></div>";
            int count = 0;
            int additionalCount = 0;
            foreach (KeyValuePair<string, List<string>> auction in auctionListIcons)
            {
                if (int.Parse(row[auction.Key].ToString()) == 0)
                    continue;

                // Default values
                string filterColor = "filter: invert(48%) sepia(79%) saturate(2476%) hue-rotate(86deg) brightness(118%) contrast(119%) drop-shadow(0px 3px 2px black);";
                string statusImage = "/Images/fa-icons/circle-check.svg";
                string hoverNote = "Active";
                //"filter: brightness(0) saturate(100%) invert(100%) sepia(0%) saturate(7470%) hue-rotate(116deg) brightness(109%) contrast(109%) drop-shadow(5px 5px 10px #ffffff)";
                if (int.Parse(row[auction.Key].ToString()) == 1)
                {
                    hoverNote = "Scheduled";
                    statusImage = "/Images/fa-icons/circle.svg";
                    filterColor = "filter: saturate(100%) invert(100%) sepia(0%) saturate(7470%) hue-rotate(116deg) brightness(109%) contrast(109%) drop-shadow(0px 3px 2px #000000);";
                }
                else if (int.Parse(row[auction.Key].ToString()) == 4)
                {
                    hoverNote = "Pending";
                    statusImage = "/Images/fa-icons/circle.svg";
                    filterColor = "filter: saturate(100%) invert(100%) sepia(0%) saturate(7470%) hue-rotate(116deg) brightness(109%) contrast(109%) drop-shadow(0px 3px 2px #000000);";
                }

                // #TODO: Need to move table structure to js so that we can enable pagination.
                // As of right now, the JS serializer will overflow on maxJsonLength
                // <div id='{kListing}_{auctionName}_auctionStatus' class='{statusClass}' title='{hoverNote}'></div>
                string auctionName = auction.Key.Replace("Status", "");
                string auctionIcon2 = $@"
                    <div class='auctionInfo row'>
                        <img class='xtraSmIcon' title='{hoverNote}' style='margin-right:4px;{filterColor}' src='{statusImage}' />
                        <img class='auctionView' onclick='auctionClick(""{kListing}_{auctionName}_auctionInfo"", {kListing},{auction.Value[0]});' id='AuctionClick_{kListing}_{auction.Value[0]}' style=""HEIGHT;BACKGROUND;"" src=""/Images/wholesale-icons/AUCTION_ICON""/>
                        <div id='{kListing}_{auctionName}_auctionInfo' class='smallHide auctionsContent actionBackground'>
                            <div style='display: table; margin: 0 auto;width:100%;'>
                                <div class='singleRow'>
                                    <div class='auctionTableHeader' style='cursor:pointer;'>{(auctionName == "RemarketingPlus" ? "Remarketing+" : auctionName)}</div>
                                </div>
                            </div>
                            <div id='auctionInfo'>
                                <div style='display: table; margin: 0 auto;width:100%;'>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>Start Date: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_startDate""></span></div>
                                    </div>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>End Date: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_endDate""></span></div>
                                    </div>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>Posted Date: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_posted""></span></div>
                                    </div>
                                </div>
                                <div style='display:table;margin: 0 auto;width:100%;'>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>Status: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_status""></span></div>
                                    </div>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>AutoLaunch: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_autoLaunch""></span></div>
                                    </div>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>Relist Count: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_relistCount""></span></div>
                                    </div>
                                </div>
                                <div style='display:table;margin: 0 auto;width:100%;'>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>Condition Report Date: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_conditionReport""></span></div>
                                    </div>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>BlackOut Window: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_blackOutWindow""></span></div>
                                    </div>
                                </div>
                                <div style='display: table; margin: 0 auto;width:100%;'>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader' style='width:27%;'>Start Price: </div>
                                        <div class='tableCell' style='width:38%;'><span id=""{kListing}_{auctionName}_startPrice"">$</span></div>
                                        <div class='tableCell' style='width:38%;'><span id=""{kListing}_{auctionName}_mmr"">MMR: $</span></div>
                                    </div>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader' style='width:27%;'>Reserve Price: </div>
                                        <div class='tableCell' style='width:38%;'><span id=""{kListing}_{auctionName}_reservePrice"">$</span></div>
                                        <div class='tableCell' style='width:38%;'><span id=""{kListing}_{auctionName}_mmrReservePrice""></span></div>
                                    </div>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader' style='width:27%;'>Buy Now: </div>
                                        <div class='tableCell' style='width:38%;'><span id=""{kListing}_{auctionName}_buyNow"">$</span></div>
                                        <div class='tableCell' style='width:38%;'><span id=""{kListing}_{auctionName}_mmrBuyNowPrice""></span></div>
                                    </div>
                                </div>
                                <div style='display:table;margin: 0 auto;width:100%;'>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>Listing Type: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_listingType""></span></div>
                                    </div>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'>Listing Category: </div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_listingCategory""></span></div>
                                    </div>
                                </div>
                                <div style='display:table;margin: 0 auto;width:100%;'>
                                    <div class='HeaderCol'>
                                        <div class='tableHeader'><span>Listing Error Message:</span></div>
                                        <div class='tableCell'><span id=""{kListing}_{auctionName}_errorMsg""></span></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                ";

                string height = "height:28px";
                string background = "";
                if (auctionName.Contains("Carmigo"))
                    height = "height:20px";
                else if (auctionName.Contains("RemarketingPlus")) // little hack due to how awkward the image is
                {
                    height = "height:16px;padding:1px;margin-top:5px";
                    background = "background:lightgrey";
                }
                else
                    height = "height:25px";
                if (auctionName.Contains("CarOffer"))
                {
                    height = "height:30px";
                    background = "background:grey";
                }

                if (auctionListIcons.ContainsKey(auction.Key))
                {
                    if (count >= 3)
                        additionalCount++;
                    else
                        auctionIcons += auctionIcon2.Replace("HEIGHT", height).Replace("BACKGROUND", background).Replace("AUCTION_ICON", auction.Value[1]);

                    count += 1;
                }
            }

            if (additionalCount != 0)
                auctionIcons +=
                    additionalAuctionsCount.Replace("ADDITIONAL_AUCTIONS", $"<a style='color:black;' href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}&FocusAuction=AuctionSection'>+{additionalCount} Auctions</a>");

            if (count != 0)
                auctionMenu += auctionIcons;
            else
                auctionMenu = "<b>Not Listed on Any Auction";

            return auctionMenu;
        }

        private string BuildAuctionErrorMsgs(string kListing, DataRow row)
        {
            string errorPopup = $@"
                <div class='errorMessage'>
                    MMRMSGSREPLACE
                    ERRORMSGSREPLACE
                </div>
            ";
            List<string> auctionList = new List<string>
            {
                "OVEErrMsg", "OpenLaneErrMsg", "AutoAuctionErrMsg",
                "ExchangeErrMsg", "SmartAuctionErrMsg", "TurnAuctionsErrMsg",
                "AuctionEdgeErrMsg", "ACVAuctionErrMsg", "eDealerDirectErrMsg",
                "COPARTErrMsg", "IAAErrMsg", "AuctionSimplifiedErrMsg",
                "IASErrMsg", "AuctionOSErrMsg", "CarmigoErrMsg", "RemarketingPlusErrMsg"
            };

            // Might need ot add additional regex 'words' when ignoring positive messages
            Regex success = new Regex(@"success", RegexOptions.IgnoreCase);

            string errorMsg = "";
            string errorMMRMsg = "";
            bool displayMMR = false;
            bool displayError = false;
            foreach (string auction in auctionList)
            {
                string error = Convert.ToString(row[auction]);
                DateTime submitDate = DateTime.Parse(Convert.ToString(row[auction.Replace("ErrMsg", "SubmitDate")]));

                if (error == "" || success.IsMatch(error) || ( 14 < ( DateTime.Now - submitDate).TotalDays ))
                    continue;

                if (error.Contains("MMR"))
                {
                    displayMMR = true;
                    errorMMRMsg += $@"
                    <div class='HeaderCol'>
                        <div class='tableHeader'>{auction.Replace("ErrMsg", "")}: </div>
                        <div class='tableCell'><span>{error}</span></div>
                    </div>";
                }
                else
                {
                    displayError = true;
                    errorMsg += $@"
                    <div class='HeaderCol'>
                        <div class='tableHeader'>{auction.Replace("ErrMsg", "")}: </div>
                        <div class='tableCell'><span>{error}</span></div>
                    </div>";
                }
            }

            if (!displayError && !displayMMR)
                return "";
            else
            {
                if (displayMMR)
                {
                    string msg = $@"
                        <img id=""mmrIcon"" onclick='errMsgsClick(""{kListing}_MMRErrorMsgs"");' src=""/Images/slash_mmr.png"" style=""display:initial !important;"" class=""errMessageView mdIcon alertIcon"" title=""Click to view MMR Error Messages""/>
                        <div id='{kListing}_MMRErrorMsgs' class='smallHide auctionsContent actionBackground'>
                            <div style='display: table; margin: 0 auto;width:100%;'>
                                <div class='singleRow'>
                                    <div class='auctionTableHeader' style='cursor:pointer;'>MMR Error Messages</div>
                                </div>
                            </div>
                            <div>
                                <div style='display: table; margin: 0 auto;width:100%;'>
                                    {errorMMRMsg}
                                </div>
                            </div>
                        </div>";

                    errorPopup = errorPopup.Replace("MMRMSGSREPLACE", msg);
                }
                else
                    errorPopup = errorPopup.Replace("MMRMSGSREPLACE", "");

                if (displayError)
                {
                    string msg = $@"
                        <img id=""noticeIcon"" onclick='errMsgsClick(""{kListing}_AuctionErrorMsgs"");' src=""/Images/fa-icons/circle-exclamation.svg"" style=""display:initial !important;"" class=""errMessageView smIcon alertIcon"" title=""Click to view messages""/>
                        <div id='{kListing}_AuctionErrorMsgs' class='smallHide auctionsContent actionBackground'>
                            <div style='display: table; margin: 0 auto;width:100%;'>
                                <div class='singleRow'>
                                    <div class='auctionTableHeader' style='cursor:pointer;'>Messages</div>
                                </div>
                            </div>
                            <div>
                                <div style='display: table; margin: 0 auto;width:100%;'>
                                    {errorMsg}
                                </div>
                            </div>
                        </div>";

                    errorPopup = errorPopup.Replace("ERRORMSGSREPLACE", msg);
                }
                else
                    errorPopup = errorPopup.Replace("ERRORMSGSREPLACE", "");

                return errorPopup;
            }
        }

        private string DetermineColumnValue(DataRow row, string columnName)
        {
            string[] customColumns = new string[] { "ErrorMsgs" };
            if (row.Table.Columns.Contains(columnName) || customColumns.Contains(columnName))
            {
                if (columnName == "ErrorMsgs")
                {
                    StringBuilder err = new StringBuilder();
                    List<string> auctionList = new List<string>
                    {
                        "OVEErrMsg", "OpenLaneErrMsg", "AutoAuctionErrMsg",
                        "ExchangeErrMsg", "SmartAuctionErrMsg", "TurnAuctionsErrMsg",
                        "AuctionEdgeErrMsg", "ACVAuctionErrMsg", "eDealerDirectErrMsg",
                        "COPARTErrMsg", "IAAErrMsg", "AuctionSimplifiedErrMsg",
                        "IASErrMsg", "AuctionOSErrMsg", "CarmigoErrMsg", "RemarketingPlusErrMsg"
                    };

                    // Might need ot add additional regex 'words' when ignoring positive messages
                    Regex success = new Regex(@"success", RegexOptions.IgnoreCase);

                    foreach (string auction in auctionList)
                    {
                        string error = Convert.ToString(row[auction]);

                        if (error == "" || success.IsMatch(error))
                            continue;

                        err.Append(error + ", ");
                    }

                    return err.ToString();
                }
                else
                    return row[columnName].ToString();
            }
            else // Return empty string if we don't find the column name in the DataTable
                return "";
        }
    }
} 