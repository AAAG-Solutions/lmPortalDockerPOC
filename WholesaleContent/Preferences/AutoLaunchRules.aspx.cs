using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;

using LMWholesale.Common;
using LMWholesale.Dealer;
using LMWholesale.resource.clients;


namespace LMWholesale.WholesaleContent.Preferences
{
    public partial class AutoLaunchRules : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Preferences.AutoLaunchRules BLL;
        private readonly DASClient dasClient;
        private readonly ListingClient listingClient;
        private readonly LookupClient lookupClient;
        private readonly WholesaleClient wholesaleClient;

        public AutoLaunchRules()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            BLL = BLL ?? new BLL.WholesaleContent.Preferences.AutoLaunchRules();
            dasClient = dasClient ?? new DASClient();
            listingClient = listingClient ?? new ListingClient();
            lookupClient = lookupClient ?? new LookupClient();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }

        public static AutoLaunchRules Self
        {
            get { return instance; }
        }
        private static readonly AutoLaunchRules instance = new AutoLaunchRules();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Preferences";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);
            HttpSessionState Session = HttpContext.Current.Session;
            
            if (!IsPostBack)
            {
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];
                int kGaggleSubGroup = (int)Session["kGaggleSubGroup"];
                int kDealerGaggle = (int)Session["kDealerGaggle"];
                bool isInternal = Self.userBLL.CheckPermission("LMIInternal");
                Session["disableSimple"] = false;

                WholesaleMMR.Value = Self.userBLL.CheckPermission("WholesaleMMR") && Self.userBLL.CheckPermission("WholesaleMMREnforce") ? "true" : "false";
                if (Self.userBLL.CheckPermission("SimpleAutoLaunchPricingBypass"))
                    SimpleRuleBypass.Value = "true";

                if (!String.IsNullOrEmpty(Request.QueryString["ListMode"]))
                    hfViewList.Value = "1";

                DealerName.Text = Session["DealerName"].ToString();

                jsGridBuilder simpleRulegrid = new jsGridBuilder
                {
                    MethodURL = "AutoLaunchRules.aspx/GetAutoLaunchRules",
                    HTMLElement = "MainContent_simplejsGrid",
                    Filtering = false,
                    Sorting = false,
                    ExtraParameters = new Dictionary<string, string> { { "isSimple", "true" } }
                };

                jsGridBuilder advancedRulegrid = new jsGridBuilder
                {
                    MethodURL = "AutoLaunchRules.aspx/GetAutoLaunchRules",
                    HTMLElement = "MainContent_advancedjsGrid",
                    Filtering = false,
                    Sorting = false,
                    ExtraParameters = new Dictionary<string, string> { { "isSimple", "false" } }
                };

                DataRow auctionInfo = Self.BLL.GetAuctionInfo(kSession, kDealer, 1);
                string lstFacilitatedCode =
                    auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().Substring(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().IndexOf("]") + 1);

                // We assume Max MMR is the same for BIN and Reserve
                MaxMMRThreshold.Value = int.Parse(auctionInfo["MMRMaxBuyNow"].ToString(), System.Globalization.NumberStyles.AllowDecimalPoint).ToString();
                MinMMRThreshold.Value = int.Parse(auctionInfo["MinMMRThreshold"].ToString(), System.Globalization.NumberStyles.AllowDecimalPoint).ToString();

                WholesaleSystem.PopulateList(lstFacilitatedCode, "-- Select a Location Code --", "lstAuctionCode", '|');

                simpleRulegrid.OnRowSelectFunction = "SimpleGridRowSelected";
                simpleRulegrid.OnClearRowSelectFunction = "SimpleClearRowSelection";

                // External View
                AutoLaunchAdd.OnClientClick = "javascript: SimpleAddAutoLaunchRuleSet();return false;";
                AutoLaunchEdit.OnClientClick = "javascript: SimpleEditAutoLaunchRule();return false;";
                AutoLaunchDelete.OnClientClick = "javascript: SimpleDeleteAutoLaunchRule();return false;";
                simpleRulegrid.OnDoubleClickFunction = "SimpleEditAutoLaunchRule();";
                string simpleGridDef = ":SelectedAuctions:Selected Auctions:125|:AgeRange:Vehicle Age Range:100|:Policy:Arbitration Policy:125|";

                List<Dictionary<string, string>> ruleSets = Self.BLL.GetAutoLaunchRuleSets(kSession, kDealer, kDealerGaggle, kGaggleSubGroup);

                string ruleSetList = "";
                foreach (Dictionary<string, string> rule in ruleSets)
                    ruleSetList += $"{rule["AuctionRuleSet"]}:{rule["ArbitrationPolicyName"]}|";
                WholesaleSystem.PopulateList(ruleSetList, "-- Select Policy --", "simpleLstListingCategory", '|');

                BuildSimpleALRuleSet(kSession, kDealer, kDealerGaggle);
                simpleRulegrid.SetFieldListFromGridDef(simpleGridDef, "", true);

                // Internal View
                advancedRulegrid.OnRowSelectFunction = "AdvancedGridRowSelected";
                advancedRulegrid.OnClearRowSelectFunction = "AdvancedClearRowSelection";

                if (isInternal)
                {
                    AutoLaunchAdd.OnClientClick = "javascript: AdvancedAddAutoLaunchRule();return false;";
                    AutoLaunchEdit.OnClientClick = "javascript: AdvancedEditAutoLaunchRule();return false;";
                    AutoLaunchDelete.OnClientClick = "javascript: AdvancedDeleteAutoLaunchRule();return false;";
                }
                advancedRulegrid.OnDoubleClickFunction = "AdvancedEditAutoLaunchRule();";
                string advancedGridDef = ":Auction::100|:Make::100|:Model::100|:InvLotLocation:Lot Location:100|:YearRange:Year Range:100|:AgeRange:Age Range:100|:MileageRange:Mileage Range:100|:InvStatus:Inv Status:100|";

                BuildAdvancedALRuleSet(kSession, kDealer);
                advancedRulegrid.SetFieldListFromGridDef(advancedGridDef, "", true);

                // Determine who is looking at rules and setup page;
                if (!isInternal)
                {
                    InternalFlag.Value = "0";
                    simplejsGrid.Attributes["class"] = "submitButton";
                }
                else
                    simplejsGrid.Attributes["class"] = "Hide";

                Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleAutoLaunchGetData(kSession, kDealer, false);
                if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    DataTable rules = returnValue.Data.Tables[0];
                    ListForm.InnerHtml = Self.BLL.BuildListForm(FormatData(rules, kSession, kDealer, false), WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient));
                }

                if (!isInternal && Session["disableSimple"].ToString() == "True")
                {
                    Response.Write("<script>alert(\"Notice: Please contact Support to adjust AutoLaunch Rules. This page is for Simplified options only.\");</script>");
                    AutoLaunchList.Enabled = false;
                    hfViewList.Value = "1";
                }
                else if (isInternal && Session["disableSimple"].ToString() == "False")
                    noticeIcon.Style["display"] = "initial !important";

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "simpleAutolaunchGrid", simpleRulegrid.RenderGrid());
                    ClientScript.RegisterStartupScript(this.GetType(), "advancedAutolaunchGrid", advancedRulegrid.RenderGrid());
                }
            }
        }

        private void BuildSimpleALRuleSet(string kSession, int kDealer, int kDealerGaggle)
        {
            List<Dictionary<string, string>> auctions = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 1);
            string auctionSelect = "<div style='display:table-row;'>";
            int count = 1;

            foreach (Dictionary<string, string> auction in auctions)
            {
                string auctionName = auction["WholesaleAuctionName"];
                if (auctionName.Contains("OVE"))
                    auctionName = "OVE";

                if (auctionName == "CarOffer")
                    continue;

                auctionSelect += $"<div class='ColRowSwap'>";
                auctionSelect += $"&nbsp;<input id='{auctionName.Replace(" ", "")}CheckStart' type='checkbox' value='{auction["kWholesaleAuction"]}' class='SingleIndent' style='font-weight:bold;' onclick=\"AuctionCheck(this);\"/>&nbsp;";
                auctionSelect += $"<label for='{auctionName.Replace(" ", "")}CheckStart' style='font-weight:bold'>{(auctionName == "RemarketingPlus" ? "Remarketing+" : auctionName)}</label>";
                auctionSelect += $"<span style='display:none;' id='MaxMMRPct_{auction["kWholesaleAuction"]}'>{auction["MaxMMRPct"]}</span>";
                auctionSelect += $"</div>";

                if (count > 1)
                {
                    //startAuctionString += "<br class='smallHide'/>";
                    auctionSelect += "</div><div style='display:table-row;'>";
                    count = 0;
                }

                count++;
            }

            WholesaleSystem.AutoGradeScaleGet("minGrade", "0.0");
            WholesaleSystem.AutoGradeScaleGet("maxGrade", "5.0");

            // Per Request of Kelly to hide from America's Users
            if (kDealerGaggle == 244)
                GradeRow.Style["display"] = "none";

            AuctionSelect.InnerHtml = auctionSelect + "</div>";

            ageMin.Attributes["oninput"] = ageMax.Attributes["oninput"] = adjustmentDollar.Attributes["oninput"] = MMRPercentage.Attributes["oninput"] = WholesaleSystem.onInputNumber;
        }

        private void BuildAdvancedALRuleSet(string kSession, int kDealer)
        {
            string lstAuction = "[]";
            List<Dictionary<string, string>> auctions = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 0);
            foreach (Dictionary<string, string> auction in auctions)
            {
                if (auction["WholesaleAuctionName"] == "CarOffer")
                    continue;

                lstAuction += $"{auction["kWholesaleAuction"]}:{(auction["WholesaleAuctionName"] == "RemarketingPlus" ? "Remarketing+" : auction["WholesaleAuctionName"])}|";
            }

            WholesaleSystem.PopulateList(lstAuction, "-- Select an Auction --", "lstAuction", '|', "0");

            // Gather Makes from a Default Auction
            StringBuilder makes = new StringBuilder("[]0:Any Make|");
            if (auctions.Count() > 0)
            {
                string lstMakes = Self.BLL.GetAuctionInfo(kSession, kDealer, int.Parse(auctions[0]["kWholesaleAuction"], 0))["ChromeMake"].ToString().Replace("[]", "");
                WholesaleSystem.PopulateList(makes.Append(lstMakes).ToString(), "Any Make", "lstMake", '|', "0");
            }

            string lstStatus = "[]1:Available|2:Unavailable|4:Demo|8:Pending|";
            WholesaleSystem.PopulateList(lstStatus, "-- Select a Status --", "lstStatus", '|', "1");

            // Listing Lot Location Results
            Listing.lmReturnValue lotList = Self.listingClient.ListingLotLocationListGet(kSession, kDealer);
            string lotString = "[][ANY]:Any Lot Location|";
            if (lotList.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                if (lotList.Data != null)
                {
                    DataTable ll = lotList.Data.Tables[0];
                    foreach (DataRow row in ll.Rows)
                    {
                        string location = Convert.ToString(row["InvLotLocation"]);
                        lotString += $"{location}:{location}|";
                    }
                }

                WholesaleSystem.PopulateList(lotString, "", "lstLotLocation", '|', "[ANY]");
            }

            // Get Motor Years
            DAS.lmReturnValue years = Self.dasClient.DASGetMotorYears(kSession, "3");
            if (years.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                StringBuilder lstYears = new StringBuilder();
                lstYears.Append("[]");

                DataTable dt = years.Data.Tables[0];
                foreach (DataRow row in dt.Rows)
                    lstYears.Append($"{row["Years"]}:{row["Years"]}|");

                WholesaleSystem.PopulateList(lstYears.ToString(), "Any Year", "lstMinYear", '|', "0");
                WholesaleSystem.PopulateList(lstYears.ToString(), "Any Year", "lstMaxYear", '|', "0");
            }

            // Get Fuel Types
            Lookup.lmReturnValue fuelTypes = Self.lookupClient.GetWholesaleAuctionFuelType(kSession);
            if (fuelTypes.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                StringBuilder lstFuelTypes = new StringBuilder("[]");
                DataTable dt = fuelTypes.Data.Tables[0];
                foreach (DataRow row in dt.Rows)
                    lstFuelTypes.Append($"{row["kFuelType"]}:{row["FuelTypeDesc"]}|");

                WholesaleSystem.PopulateList(lstFuelTypes.ToString(), "Any Fuel Type", "lstFuelType", '|', "0");
            }

            // Build DropDowns
            string lstCreds = "[]0:Any Credential|";
            WholesaleSystem.PopulateList(lstCreds, "Any Credential", "lstCredentials", '|', "0");

            string lstTitle = "[]1:Branded|2:No Title|3:MSO|4:Title Present|5:Title Absent|6:Repo Affidavit|7:Salvage|";
            WholesaleSystem.PopulateList(lstTitle, "Any Title Status", "lstTitle", '|', "0");

            string lstCondition = "[]0:Allow Vehicles without CR|1:Require CR For All Vehicles|";
            WholesaleSystem.PopulateList(lstCondition, "Allow Vehicles without CR", "lstConditionRpt", '|', "0");

            WholesaleSystem.AutoGradeScaleGet("lstMinGrade", "0.0");
            WholesaleSystem.AutoGradeScaleGet("lstMaxGrade", "5.0");
            WholesaleSystem.VehicleLocationsGet("lstPhysLocation");
            WholesaleSystem.GetDefaultListingCatergories("advancedLstListingCategory", "-- Select a Listing Category --");
            WholesaleSystem.GetDefaultListingCatergories("lstCRCat", "Default Category");
            WholesaleSystem.GetBidIncrements("lstBidIncrement");
            WholesaleSystem.GetTitleStatuses("lstTitle");
            WholesaleSystem.GetVehicleTypes("lstVehicleType");
            WholesaleSystem.BuildPriceTypeDropdown("lstPrimeStartPrice");
            WholesaleSystem.BuildPriceTypeDropdown("lstPrimeFloorPrice");
            WholesaleSystem.BuildPriceTypeDropdown("lstPrimeBIN");
            WholesaleSystem.BuildPriceTypeDropdown("lstSecondStartPrice");
            WholesaleSystem.BuildPriceTypeDropdown("lstSecondFloorPrice");
            WholesaleSystem.BuildPriceTypeDropdown("lstSecondBIN");
        }

        [WebMethod(Description = "Attempt to test an AutoLaunch Rule before saving")]
        public static Dictionary<string, object> AutoLaunchRuleTest(string json)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            Dictionary<string, int> counts = new Dictionary<string, int> { { "total", 2 }, { "filtered", 1 } };
            string returnMessage = "";
            IsSuccess = Self.BLL.WholesaleAutoLaunchRuleTest(
                                    (string)Session["kSession"], (int)Session["kDealer"], (Dictionary<string, object>)Util.serializer.DeserializeObject(json), ref counts, ref returnMessage);
            Message = returnMessage;
            Value = counts;

            return ReturnResponse();
        }

        [WebMethod(Description = "Get AutoLaunch rule to edit")]
        public static Dictionary<string, object> GetAutoLaunchRule(int kValue, string isSimple)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            string message = "";
            bool isSuccess = false;
            Value = Self.BLL.AutoLaunchItemGet((string)Session["kSession"], (int)Session["kDealer"], kValue, bool.Parse(isSimple), ref isSuccess, ref message);
            Message = message;
            IsSuccess = IsSuccess;

            return ReturnResponse();
        }

        [WebMethod(Description = "Get List of Models for a given Year/Make")]
        public static string GetModelList(string year, string make)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];

            StringBuilder lstModels = new StringBuilder();
            DAS.lmReturnValue models = Self.dasClient.DASGetMotorModels(kSession, "3", year, make);
            if (models.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = models.Data.Tables[0];
                foreach (DataRow row in dt.Rows) {
                    if (row["Models"].ToString() == "ALL")
                        continue;
                    else
                        lstModels.Append($"{row["Feature"]}:{row["Models"]}|");
                }
            }

            return lstModels.ToString();
        }

        [WebMethod(Description = "Gather all AutoLaunch Rules associated with a given dealer")]
        public static string GetAutoLaunchRules(string filter, string ExtraParams)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            Dictionary<string, object> simpleDict = (Dictionary<string, object>)Util.serializer.DeserializeObject(ExtraParams);

            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            string tmpFail = "0 | {}";

            bool isSimple = bool.Parse(simpleDict["isSimple"].ToString());
            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleAutoLaunchGetData(kSession, kDealer, isSimple);
            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataSet rules = returnValue.Data;
                if (rules != null)
                    return returnValue.Values.GetValue("TotalItems", "0") + "|" + Util.serializer.Serialize(FormatData(rules.Tables[0], kSession, kDealer, isSimple));
                else
                    return "0|{}";
            }

            return tmpFail;
        }

        [WebMethod(Description = "Get list of Auction Credentials for a given dealer and auction")]
        public static Dictionary<string, object> GetAuctionCredentials(int kWholesaleAuction)
        {
            HttpSessionState Session = HttpContext.Current.Session;            

            StringBuilder auctionList = new StringBuilder("0:Any Credential|");
            string message = "";
            IsSuccess = Self.BLL.GetAuctionCredentials((string)Session["kSession"], (int)Session["kDealer"], kWholesaleAuction, ref auctionList, ref message);
            Message = message;
            Value = auctionList.ToString();

            return ReturnResponse();
        }

        [WebMethod(Description = "Save new/edited autolaunch rule")]
        public static Dictionary<string, object> SaveAutoLaunchRule(bool isSimple, string op, string data)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty((string)Session["kSession"]))
                LMWholesale.BLL.WholesaleUser.WholesaleUser.ClearUser();

            string returnMessage = "";
            IsSuccess = Self.BLL.SaveAutoLaunchRuleItem(Session, isSimple, op, (Dictionary<string, object>)Util.serializer.DeserializeObject(data), ref returnMessage);
            Message = returnMessage;

            return ReturnResponse();
        }

        [WebMethod(Description = "Update Facilitated auction and loction when auction changes")]
        public static Dictionary<string, object> UpdateAuctionDropdowns(int kWholesaleAuction)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            Dictionary<string, string> results = new Dictionary<string, string>
            {
                { "AuctionCode", "" },
                { "LocationCodes", "" },
                { "LocationSelection", "" },
                { "BidIncrement", "" }
            };

            string message = "";
            IsSuccess = Self.BLL.UpdateAuctionDropdowns((string)Session["kSession"], (int)Session["kDealer"], kWholesaleAuction, ref results, ref message);
            Message = message;
            Value = results;

            return ReturnResponse();
        }

        private static List<Dictionary<string, object>> FormatData(DataTable dt, string kSession, int kDealer, bool isSimple)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            int kGaggleSubGroup = (int)Session["kGaggleSubGroup"];
            int kDealerGaggle = (int)Session["kDealerGaggle"];

            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();
            List<Dictionary<string, string>> auctions = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 0);
            List<Dictionary<string, string>> ruleSets = Self.BLL.GetAutoLaunchRuleSets(kSession, kDealer, kDealerGaggle, kGaggleSubGroup);

            Session["disableSimple"] = false;
            foreach (DataRow row in dt.Rows)
            {
                List<string> simpleAuctions = new List<string>();
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    if (isSimple && col.ColumnName.Contains("is"))
                    {
                        if (row[col.ColumnName].ToString() == "1")
                        {
                            if (col.ColumnName.Contains("OpenLane"))
                                simpleAuctions.Add("ADESA");
                            else if (col.ColumnName.Contains("Integrated"))
                                simpleAuctions.Add("IAS");
                            else
                                simpleAuctions.Add(col.ColumnName.Replace("is", ""));
                        }
                    }
                    if (col.ColumnName.Contains("isInternal") && row[col.ColumnName].ToString() == "1")
                    {
                        // Since we display all rules in the list form, if any of them have been adjusted, this breaks the simplified paradigm and cannot maintain state.
                        // Therefore, we will disable any additional functionality
                        Session["disableSimple"] = true;
                    }
                    dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));
                }

                if (isSimple)
                {
                    Dictionary<string, string> set = ruleSets.Find(ruleSet => ruleSet["AuctionRuleSet"] == row["AuctionRuleSet"].ToString());
                    dict["Policy"] = $"<input type='hidden' value='{row["kWholesaleAuctionRuleSet"]}'>{(set == null ? "" : set["ArbitrationPolicyName"])}";
                    dict["AgeRange"] = row["MinVehicleAge"].ToString() + " - " + row["MaxVehicleAge"].ToString() + " Days";
                    dict["SelectedAuctions"] = string.Join(" | ", simpleAuctions.ToArray());
                }
                else
                {
                    if (row["kInventoryStatus"].ToString() == "1")
                        dict["InvStatus"] = "Available";
                    else if (row["kInventoryStatus"].ToString() == "2")
                        dict["InvStatus"] = "Unavailable";
                    else if (row["kInventoryStatus"].ToString() == "3")
                        dict["InvStatus"] = "On Hold";
                    else if (row["kInventoryStatus"].ToString() == "4")
                        dict["InvStatus"] = "Demo";
                    else if (row["kInventoryStatus"].ToString() == "8")
                        dict["InvStatus"] = "Pending";

                    dict["Make"] = row["Make"].ToString() == "" || row["Make"].ToString() == "0" ? "Any Make" : row["Make"].ToString();
                    dict["Model"] = row["Model"].ToString() == "" || row["Model"].ToString() == "0" ? "Any Model" : row["Model"].ToString();

                    string wholesaleAuctionName = auctions.Find(auction => auction["kWholesaleAuction"] == row["kWholesaleAuction"].ToString())["WholesaleAuctionName"];
                    dict["Auction"] = $"<input type='hidden' value='{row["kWholesaleAutoLaunch"]}'>{(wholesaleAuctionName == "RemarketingPlus" ? "Remarketing+" : wholesaleAuctionName)}";

                    // Range Logic
                    dict["AgeRange"] = dict["AgeRange"] = row["AgeLow"].ToString() + " - " + row["AgeHigh"].ToString() + " Days";
                    dict["MileageRange"] = row["MinMileage"].ToString() + " - " + row["MaxMileage"].ToString();

                    // Year Range Logic
                    string yearMax = row["MotorYearMax"].ToString();
                    string yearMin = row["MotorYear"].ToString();
                    if (yearMin == yearMax && (int.Parse(yearMin) != 0 && int.Parse(yearMax) != 0))
                        dict["YearRange"] = row["MotorYearMax"].ToString();
                    else if (string.IsNullOrEmpty(row["MotorYearMax"].ToString()) && string.IsNullOrEmpty(row["MotorYear"].ToString()))
                        dict["YearRange"] = "Any Year";
                    else if (int.Parse(yearMin) > int.Parse(yearMax))
                        dict["YearRange"] = yearMin + " - Any Year";
                    else if (int.Parse(yearMin) < int.Parse(yearMax) && int.Parse(yearMin) == 0)
                        dict["YearRange"] = "Any Year - " + yearMax;
                    else
                        dict["YearRange"] = "Any Year";
                }

                returnList.Add(dict);
            }

            return returnList;
        }
    }
}