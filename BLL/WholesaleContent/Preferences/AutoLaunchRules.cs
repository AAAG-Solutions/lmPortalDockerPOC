using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.SessionState;
using LMWholesale.resource.clients;
using LMWholesale.resource.factory;


namespace LMWholesale.BLL.WholesaleContent.Preferences
{
    public class AutoLaunchRules
    {
        private readonly AuctionFactory auctionFactory;
        private readonly DASClient dasClient;
        private readonly LookupClient lookupClient;
        private readonly WholesaleClient wholesaleClient;
        private readonly WholesaleUser.WholesaleUser userBLL;

        public AutoLaunchRules()
        {
            dasClient = dasClient ?? new DASClient();
            lookupClient = lookupClient ?? new LookupClient();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            auctionFactory = auctionFactory ?? new AuctionFactory();
            userBLL = userBLL ?? new WholesaleUser.WholesaleUser();
        }

        public AutoLaunchRules(DASClient dasClient, LookupClient lookupClient, WholesaleClient wholesaleClient, AuctionFactory auctionFactory, WholesaleUser.WholesaleUser userBLL)
        {
            this.dasClient = dasClient;
            this.lookupClient = lookupClient;
            this.wholesaleClient = wholesaleClient;
            this.auctionFactory = auctionFactory;
            this.userBLL = userBLL;
        }

        internal static readonly AutoLaunchRules instance = new AutoLaunchRules();
        public AutoLaunchRules Self
        {
            get { return instance; }
        }

        public object AutoLaunchItemGet(string kSession, int kDealer, int kValue, bool isSimple, ref bool isSuccess, ref string message)
        {
            Wholesale.lmReturnValue returnValue = null;
            if (isSimple)
            {
                Dictionary<string, string> simpleSettings = new Dictionary<string, string>
                {
                    { "AuctionRuleSet", "" }, { "MinVehicleAge", "" }, { "MaxVehicleAge", "" }, { "MinGrade", "" }, { "MaxGrade", "" },
                    { "FloorPricingStrategy", "" }, { "FloorAdjustmentType", "" }, { "FloorAdjustmentAmt", "" }, { "BuyNowAdjustment", "" },
                    { "MMRPercentage", "" }, { "isOVE", "" }, { "isSmartAuction", "" }, { "isOpenLane", "" }, { "isCOPART", "" }, { "isAuctionEdge", "" },
                    { "isACVAuctions", "" }, { "iseDealerDirect", "" }, { "isIAA", "" }, { "isAuctionSimplified", "" }, { "isIntegratedAuctionSolutions", "" },
                    { "isAuctionOS", "" }, { "isCarmigo", ""}, { "isRemarketingPlus", "" }//, { "isCarOffer", "" }
                };

                returnValue = Self.wholesaleClient.WholesaleAutoLaunchGetData(kSession, kDealer, true);
                if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    DataTable dt = returnValue.Data.Tables[0];
                    DataRow dr = dt.Select($"kWholesaleAuctionRuleSet = {kValue}")[0];
                    List<string> selectedAuctions = new List<string>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.Contains("is"))
                        {
                            if (dr[dc.ColumnName].ToString() == "1")
                            {
                                if (dc.ColumnName.Contains("Integrated"))
                                    selectedAuctions.Add("IAS");
                                else if (dc.ColumnName.Contains("OpenLane"))
                                    selectedAuctions.Add("ADESA");
                                else
                                    selectedAuctions.Add(dc.ColumnName.Replace("is", ""));
                            }
                        }
                        simpleSettings[dc.ColumnName] = dr[dc.ColumnName].ToString();
                    }

                    isSuccess = true;

                    simpleSettings["selectedAuctions"] = string.Join(",", selectedAuctions);
                    return simpleSettings;
                }
                else if (returnValue.Result == Wholesale.ReturnCode.LM_INVALIDSESSION)
                    WholesaleUser.WholesaleUser.ClearUser();
            }
            else
            {
                returnValue = Self.wholesaleClient.WholesaleAutoLaunchGetData(kSession, kDealer, false);
                Dictionary<string, string> alFilters = new Dictionary<string, string>
                {
                    { "kWholesaleAuction", "" }, { "MotorYear", "" }, { "MotorYearMax", "" },
                    { "InvLotLocation", "" }, { "Make", "" }, { "Model", "" },
                    { "kDealerWholesaleCredential", "" }, { "kInventoryStatus", "" }, { "kWholesaleVehicleType", "" },
                    { "kWholesaleTitleStatus", "" }, { "MinMileage", "" }, { "MaxMileage", "" },
                    { "AgeLow", "" }, { "AgeHigh", "" }, { "kFuelType", "" },
                    { "RequireConditionReport", "" }, { "MinimumGrade", "" }, { "MaximumGrade", "" },
                };
                Dictionary<string, string> alAddSettings = new Dictionary<string, string>
                {
                    { "kWholesaleListingTag", "" }, { "kWholesaleListingCategory", "" }, { "Duration", "" },
                    { "kListingCategoryNoCR", "" }, { "kWholesaleLocationIndicator", "" }, { "kWholesaleBidIncrement", "" },
                    { "kWholesaleFacilitatedAuction", "" }, { "kWholesaleLocationCode", "" }, { "PostAsYellowLight", "" }
                };
                Dictionary<string, string> alPriceSettings = new Dictionary<string, string>
                {
                    { "MinimumPricingAdjustment", "" },
                    { "StartPricingType", "" }, { "StartPricingAdjustment", "" }, { "StartPricingPercentage", "" },
                    { "FloorPricingType", "" }, { "FloorPricingAdjustment", "" }, { "FloorPricingPercentage", "" },
                    { "BuyNowPricingType", "" }, { "BuyNowPricingAdjustment", "" }, { "BuyNowPricingPercentage", "" },
                    { "MMRStartType", "" }, { "MMRStartAdj", "" }, { "MMRStartPct", "" },
                    { "MMRFloorType", "" }, { "MMRFloorAdj", "" }, { "MMRFloorPct", "" },
                    { "MMRBuyNowType", "" }, { "MMRBuyNowAdj", "" }, { "MMRBuyNowPct", "" },
                };

                if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    // Find specific AutoLaunch settings and populate dictionaries
                    DataRow row = returnValue.Data.Tables[0].Select($"kWholesaleAutoLaunch = '{kValue}'")[0];
                    foreach (DataColumn col in returnValue.Data.Tables[0].Columns)
                    {
                        if (alFilters.ContainsKey(col.ColumnName))
                            alFilters[col.ColumnName] = row[col.ColumnName].ToString();
                        else if (alAddSettings.ContainsKey(col.ColumnName))
                            alAddSettings[col.ColumnName] = row[col.ColumnName].ToString();
                        else
                            alPriceSettings[col.ColumnName] = row[col.ColumnName].ToString();
                    }

                    // Bundle up Dictionaries and serialize

                    Dictionary<string, Dictionary<string, string>> results = new Dictionary<string, Dictionary<string, string>>
                    {
                        { "filters", alFilters },
                        { "additional", alAddSettings },
                        { "prices", alPriceSettings }
                    };

                    return results;
                }
                else if (returnValue.Result == Wholesale.ReturnCode.LM_INVALIDSESSION)
                    WholesaleUser.WholesaleUser.ClearUser();

                // return empty dictionary if we fail
                message = returnValue.ResultString;
            }


            // Return false if we fail for some reason
            return false;
        }

        public List<Dictionary<string, string>> GetAutoLaunchRuleSets(string kSession, int kDealer, int kDealerGaggle, int kGaggleSubGroup)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Wholesale.lmReturnValue rv = Self.wholesaleClient.WholesaleDealerAutoLaunchRuleSetGet(kSession, kDealer, kDealerGaggle, kGaggleSubGroup);
            if (rv.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = rv.Data.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, string> info = new Dictionary<string, string>();
                    foreach (DataColumn dc in dt.Columns)
                        info.Add(dc.ColumnName, row[dc.ColumnName].ToString());
                    list.Add(info);
                }
            }
            else if (rv.Result == Wholesale.ReturnCode.LM_INVALIDSESSION)
                WholesaleUser.WholesaleUser.ClearUser();
            return list;
        }

        public bool GetAuctionCredentials(string kSession, int kDealer, int kWholesaleAuction, ref StringBuilder list, ref string message)
        {
            Lookup.lmReturnValue AuctionCreds = Self.lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), kWholesaleAuction.ToString());
            if (AuctionCreds.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = AuctionCreds.Data.Tables[0];

                foreach (DataRow dr in dt.Rows)
                    list.Append(dr["kDealerWholesaleCredential"].ToString() + ":" + dr["CredentialName"].ToString());

                return true;
            }
            else if (AuctionCreds.Result == Lookup.ReturnCode.LM_INVALIDSESSION)
                WholesaleUser.WholesaleUser.ClearUser();

            message = AuctionCreds.ResultString;
            return false;
        }

        public bool SaveAutoLaunchRuleItem(HttpSessionState Session, bool isSimple, string op, Dictionary<string, object> data, ref string returnMessage)
        {
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            if (isSimple)
            {
                // Explicit set of potential SmartAuction selection on this rule set
                // Car-Rac Tier 5 can ONLY apply to platforms other than SmartAuction (OVE/AuctionEdge/etc.)
                if (data["AuctionRuleSet"].ToString() == "19")
                    data["isSmartAuction"] = "0";

                Wholesale.lmReturnValue rvSet = Self.wholesaleClient.WholesaleDealerAutoLaunchRuleSetSet(kSession, kDealer, op, data);
                if (rvSet.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    if (op != "delete")
                        data["kWholesaleAuctionRuleSet"] = rvSet.Values.GetValue("kWholesaleAuctionRuleSet", "");
                    List<Wholesale.lmReturnValue> responses = SimpleSanitizeAndShip(kSession, kDealer, (int)Session["kDealerGaggle"], (int)Session["kGaggleSubGroup"], op, data);
                    foreach (Wholesale.lmReturnValue rv in responses)
                    {
                        if (rv.Result == Wholesale.ReturnCode.LM_INVALIDSESSION)
                            WholesaleUser.WholesaleUser.ClearUser();
                        else if (rv.Result == Wholesale.ReturnCode.LM_GENERICERROR)
                            returnMessage += $"{rv.ResultString}\n\t";
                    }

                    // Loop through responses list to make sure that none of them failed for some reason
                    return true;
                }
                else if (rvSet.Result == Wholesale.ReturnCode.LM_INVALIDSESSION)
                    WholesaleUser.WholesaleUser.ClearUser();
                else
                    returnMessage += rvSet.ResultString;
            }
            else
            {
                string WholesaleAuctionName = data["WholesaleAuctionName"].ToString();
                data.Remove("WholesaleAuctionName");
                Wholesale.lmReturnValue result = SanitizeAndShip(kSession, kDealer, op, data);
                if (result.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    return true;
                }
                else if (result.Result == Wholesale.ReturnCode.LM_DBERROR)
                    returnMessage = $"{WholesaleAuctionName}: {result.ResultString}";
            }

            return false;
        }

        public DataRow GetAuctionInfo(string kSession, int kDealer, int kWholesaleAuction)
        {
            IAuctionService auctionService = Self.auctionFactory.GetAuctionService(kWholesaleAuction);
            return auctionService.GetAuctionInfo(kSession, kDealer).Rows[0];
        }

        public string GatherAutoLaunchRules(string session, int kDealer)
        {
            List<string> stringList = new List<string>();

            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleAutoLaunchTextGet(session, kDealer);

            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = returnValue.Data.Tables[0];
                int NumRows = dt.Rows.Count;

                if (NumRows >= 1)
                {
                    string[] KeyWords = { "criteria", "settings" };
                    int RuleNum = 1;
                    string RuleString = "<span class='rules'>Rule #RULENUM: </span><br/>";

                    // Initial Rule #
                    stringList.Add(RuleString.Replace("RULENUM", RuleNum.ToString()));
                    int count = 1;

                    foreach (DataRow row in dt.Rows)
                    {
                        string r = row[1].ToString();

                        if (r == "")
                        {
                            RuleNum += 1;
                            count += 1;
                            stringList.Add(RuleString.Replace("RULENUM", RuleNum.ToString()));
                            continue;
                        }
                        else if (count == NumRows - 1)
                            break;

                        if (!(KeyWords.Any(r.Contains)))
                            stringList.Add($"<span>&emsp;{r}</span><br/>");
                        else
                            stringList.Add($"{r}<br/>");

                        count += 1;
                    }
                }
                else
                    stringList.Add("<span class='rules'>There are no AutoLaunch Rules set!</span><br/>");
            }

            return String.Join("", stringList.ToArray());
        }

        public string BuildListForm(List<Dictionary<string, object>> Data, List<Dictionary<string, string>> Auctions)
        {
            //#TODO: Figure out why the non-concat version was not working and refactor
            string retString = "<div style='display: flex;flex-wrap: wrap;'>";
            int counter = 1;
            foreach(Dictionary<string, object> item in Data)
            {
                string auctionName = Auctions.First(d => d.ContainsValue(item["kWholesaleAuction"].ToString()))["WholesaleAuctionName"];
                int yearMin = Convert.ToInt32(item["MotorYear"]);
                int yearMax = Convert.ToInt32(item["MotorYearMax"]);
                string lotLocation = item["InvLotLocation"].ToString() == "[ANY]" ? "Any" : item["InvLotLocation"].ToString();
                if ((counter - 1) % 2 == 0)
                    retString += $"<div style='display: flex; width: 100%;'>";
                retString += $"<div style='border:solid 5px black;margin:2px;padding:10px;font-size:12pt;font-weight:bold;width:50%;'>Rule #{counter}";
                retString += "<br /><br />";
                retString += $"<span>{auctionName} - Vehicles meeting the following criteria</span><br />";
                if (yearMin != 0 && yearMax != 0)
                    retString += $"<span>&emsp;Motor Year: Between {yearMin} and {yearMax}</span><br />";
                else if (yearMin == 0 && yearMax != 0)
                    retString += $"<span>&emsp;Motor Year: Before {yearMax}</span><br />";
                else if (yearMin != 0 && yearMax == 0)
                    retString += $"<span>&emsp;Motor Year: After {yearMin}</span><br />";
                else
                    retString += $"<span>&emsp;Motor Year: Any</span><br />";
                if(item["Make"].ToString() == "Any Make" || item["Make"].ToString() == "")
                    retString += $"<span>&emsp;Make: Any</span><br />";
                else
                    retString += $"<span>&emsp;Make: {item["Make"]}</span><br />";
                if (item["Model"].ToString() == "Any Model" || item["Model"].ToString() == "")
                    retString += $"<span>&emsp;Model: Any</span><br />";
                else
                    retString += $"<span>&emsp;Model: {item["Model"]}</span><br />";
                retString += $"<span>&emsp;Mileage: Between {item["MinMileage"]} and {item["MaxMileage"]}</span><br />";
                retString += $"<span>&emsp;Vehicle Age: Between {item["AgeLow"]} and {item["AgeHigh"]}</span><br />";
                retString += $"<span>&emsp;Vehicle Type: {LMWholesale.WholesaleSystem.GetVehicleType(item["kWholesaleVehicleType"].ToString())}</span><br />"; //TODO: Fill in vehicle type
                retString += $"<span>&emsp;Lot Location: {lotLocation}</span><br />";
                retString += "<span>Will launch with the following settings:</span><br />";
                retString += $"<span>&emsp;Listing Type: {LMWholesale.WholesaleSystem.GetListingType(item["kWholesaleListingTag"].ToString())}</span><br />";
                retString += $"<span>&emsp;Category: {LMWholesale.WholesaleSystem.GetListingCategory(item["kWholesaleListingCategory"].ToString())}</span><br />";
                retString += $"<span>&emsp;Duration: {item["Duration"]}</span><br />";
                retString += $"<span>&emsp;Physical Location: {LMWholesale.WholesaleSystem.GetPhysicalLocation(item["kWholesaleLocationIndicator"].ToString())}</span><br />";
                retString += "<span>With the following pricing rules:</span><br />";
                retString += "<span>&emsp;Primary Pricing:</span><br />";
                if (item["StartPricingType"].ToString() != "0")
                    retString += $"<span>&emsp;&emsp;Starting Price Type: {LMWholesale.WholesaleSystem.GetPricingType(item["StartPricingType"].ToString())} Adjustment: ${Convert.ToDecimal(item["StartPricingAdjustment"].ToString()).ToString("F")}</span><br />";
                else
                    retString += $"<span>&emsp;&emsp;Starting Price Type: None</span><br />";
                if (item["FloorPricingType"].ToString() != "0")
                    retString += $"<span>&emsp;&emsp;Floor Price Type: {LMWholesale.WholesaleSystem.GetPricingType(item["FloorPricingType"].ToString())} Adjustment: ${Convert.ToDecimal(item["FloorPricingAdjustment"].ToString()).ToString("F")}</span><br />";
                else
                    retString += $"<span>&emsp;&emsp;Floor Price Type: None</span><br />";
                if (item["BuyNowPricingType"].ToString() != "0")
                    retString += $"<span>&emsp;&emsp;Buy Now Price Type: {LMWholesale.WholesaleSystem.GetPricingType(item["BuyNowPricingType"].ToString())} Adjustment: ${Convert.ToDecimal(item["BuyNowPricingAdjustment"].ToString()).ToString("F")}</span><br />";
                else
                    retString += $"<span>&emsp;&emsp;Buy Now Price Type: None</span><br />";
                if (item["MMRStartType"].ToString() != "0" || item["MMRFloorType"].ToString() != "0" || item["MMRBuyNowType"].ToString() != "0")
                {
                    retString += "<span>&emsp;Secondary Pricing:</span><br />";
                    if (item["MMRStartType"].ToString() != "0")
                        retString += $"<span>&emsp;&emsp;Starting Price Type: {LMWholesale.WholesaleSystem.GetPricingType(item["MMRStartType"].ToString())} Adjustment: ${Convert.ToDecimal(item["MMRStartAdj"].ToString()).ToString("F")}</span><br />";
                    else
                        retString += $"<span>&emsp;&emsp;Starting Price Type: None</span><br />";
                    if (item["MMRFloorType"].ToString() != "0")
                        retString += $"<span>&emsp;&emsp;Floor Price Type: {LMWholesale.WholesaleSystem.GetPricingType(item["MMRFloorType"].ToString())} Adjustment: ${Convert.ToDecimal(item["MMRFloorAdj"].ToString()).ToString("F")}</span><br />";
                    else
                        retString += $"<span>&emsp;&emsp;Floor Price Type: None</span><br />";
                    if (item["MMRBuyNowType"].ToString() != "0")
                        retString += $"<span>&emsp;&emsp;Buy Now Price Type: {LMWholesale.WholesaleSystem.GetPricingType(item["MMRBuyNowType"].ToString())} Adjustment: ${Convert.ToDecimal(item["MMRBuyNowAdj"].ToString()).ToString("F")}</span><br />";
                    else
                        retString += $"<span>&emsp;&emsp;Buy Now Price Type: None</span><br />";
                }
                retString += "</div>";
                if ((counter - 1) % 2 == 1)
                    retString += $"</div>";
                counter++;
                
            }
                
            if ((counter - 2) % 2 != 1)
                retString += $"</div>";

            retString += "</div>";
            return retString;
        }

        public bool WholesaleAutoLaunchRuleTest(string kSession, int kDealer, Dictionary<string, object> json, ref Dictionary<string, int> counts, ref string returnMessage)
        {
            json.Add("kDealer", kDealer);
            Wholesale.lmReturnValue vehicleCounts = Self.wholesaleClient.WholesaleAutoLaunchRuleTest(kSession, Util.serializer.Serialize(json));
            if (vehicleCounts.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataRow dt = vehicleCounts.Data.Tables[0].Rows[0];
                counts["total"] = int.Parse(dt["TotalInventoryCount"].ToString());
                counts["filtered"] = int.Parse(dt["TotalFiltered"].ToString());

                return true;
            }
            else if (vehicleCounts.Result == Wholesale.ReturnCode.LM_INVALIDSESSION)
                WholesaleUser.WholesaleUser.ClearUser();

            return false;
        }

        public bool UpdateAuctionDropdowns(string kSession, int kDealer, int kWholesaleAuction, ref Dictionary<string, string> items, ref string message)
        {
            Wholesale.lmReturnValue auctionInfo = Self.wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, kWholesaleAuction);
            if (auctionInfo.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataRow dr = auctionInfo.Data.Tables[0].Rows[0];
                Dictionary<string, string> auctionCodes = LMWholesale.WholesaleSystem.BuildList(dr["kWholesaleFacilitatedAuctionCode"].ToString(), "", '|');
                Dictionary<string, string> bidIncrement = LMWholesale.WholesaleSystem.BuildList(dr["WholesaleBidIncrement"].ToString(), "", '|');
                Dictionary<string, string> locationCodes = LMWholesale.WholesaleSystem.BuildList(dr["kWholesaleLocationCode"].ToString(), "", '|');
                string lstLocationCodes = dr["kWholesaleLocationCode"].ToString();
                lstLocationCodes = lstLocationCodes.Substring(lstLocationCodes.IndexOf("]") + 1);

                items["AuctionCode"] = auctionCodes["Selected"];
                items["LocationCodes"] = lstLocationCodes;
                items["LocationSelection"] = locationCodes["Selected"];
                items["BidIncrement"] = bidIncrement["Selected"];

                return true;
            }

            message = auctionInfo.ResultString;
            return false;
        }

        private Wholesale.lmReturnValue SanitizeAndShip(string kSession, int kDealer, string op, Dictionary<string, object> data)
        {

            if (data["kWholesaleAuction"].ToString() == "2")
            {
                data["StartPricingType"] = 0;
                data["StartPricingAdjustment"] = 0;
                data["StartPricingPercentage"] = "0.00";
                data["AltStartType"] = 0;
                data["AltStartAdj"] = 0;
                data["AltStartPct"] = "0.00";
            }

            if (data["kWholesaleAuction"].ToString() != "5" && data["kWholesaleAuction"].ToString() != "13")
            {
                // Maybe something goes here?
            }
            else
            {
                data["FloorPricingType"] = 0;
                data["FloorPricingAdjustment"] = 0;
                data["FloorPricingPercentage"] = "0.00";
                data["AltStartType"] = 0;
                data["AltStartAdj"] = 0;
                data["AltStartPct"] = "0.00";
            }

            if (data["kWholesaleAuction"].ToString() != "0")
            {

            }

            switch (data["kWholesaleAuction"].ToString())
            {
                case "2":
                    data["kWholesaleFacilitatedAuctionCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "4":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleFacilitatedAuctionCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "5":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleFacilitatedAuctionCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "7":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "10":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "11":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleFacilitatedAuctionCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "12":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleFacilitatedAuctionCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "13":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleFacilitatedAuction"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "14":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleFacilitatedAuctionCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "15":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "16":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleFacilitatedAuctionCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                case "17":
                    data["kWholesaleLocationCode"] = 0;
                    data["kWholesaleFacilitatedAuctionCode"] = 0;
                    data["kWholesaleBidIncrement"] = 0;
                    break;
                default:
                    // Do nothing if we do not know what auction
                    break;
            }

            data["PostAsYellowLight"] = data["kWholesaleAuction"].ToString() != "2" || data["kWholesaleAuction"].ToString() != "12" ? data["PostAsYellowLight"] : 0;
            data["RequireCR"] = data["kWholesaleAuction"].ToString() == "2" ? 1 : data["RequireCR"];
            data["kListingCategoryNoCR"] = data["RequireCR"].ToString() == "1" ? data["kWholesaleListingCategory"].ToString() : data["kListingCategoryNoCR"].ToString();

            if (op == "delete")
                data["Disable"] = 1;

            // After all is said and done, we finally send to db
            string json = Util.serializer.Serialize(data);

            int a = 0;
            return Self.wholesaleClient.WholesaleAutoLaunchSetData(kSession, kDealer, json);
        }

        private List<Wholesale.lmReturnValue> SimpleSanitizeAndShip(string kSession, int kDealer, int kDealerGaggle, int kGaggleSubGroup, string op, Dictionary<string, object> data)
        {
            // Gather all relevant rule set options and save appropriate AutoLaunch rules
            List<Dictionary<string, string>> ruleSets = Self.GetAutoLaunchRuleSets(kSession, kDealer, kDealerGaggle, kGaggleSubGroup)
                .FindAll(ruleSet => ruleSet["AuctionRuleSet"] == data["AuctionRuleSet"].ToString());

            List<Dictionary<string, object>> AutoLaunchRules = new List<Dictionary<string, object>>();
            List<Wholesale.lmReturnValue> responses = new List<Wholesale.lmReturnValue>();

            List<Dictionary<string, string>> sets = ruleSets.FindAll(set => set["kWholesaleAuction"] == "");
            List<Dictionary<string, string>> saSets = ruleSets.FindAll(set => set["kWholesaleAuction"] == "2");
            string adjustment = "";
            if (data["FloorAdjustmentType"].ToString() == "2")
                adjustment = "-" + data["FloorAdjustmentAmt"].ToString();
            else if (data["FloorAdjustmentType"].ToString() == "1")
                adjustment = data["FloorAdjustmentAmt"].ToString();
            else
                adjustment = "0";

            int binAdjustment = 0;
            if (data["FloorAdjustmentType"].ToString() == "2" && int.Parse(data["FloorAdjustmentAmt"].ToString()) > 100)
                binAdjustment = int.Parse("-" + data["FloorAdjustmentAmt"].ToString()) + int.Parse(data["BuyNowAdjustment"].ToString());
            else if (data["FloorAdjustmentType"].ToString() == "1")
                binAdjustment = int.Parse(data["FloorAdjustmentAmt"].ToString()) + int.Parse(data["BuyNowAdjustment"].ToString());
            else
                binAdjustment = int.Parse(data["BuyNowAdjustment"].ToString());

            // Gather all previous AutoLaunch Rules to divide and conquer
            DataSet previousRules = Self.wholesaleClient.WholesaleAutoLaunchGetData(kSession, kDealer, false).Data;
            List<Dictionary<string, string>> auctions = LMWholesale.WholesaleSystem.GetAvailableAuctions(kSession, kDealer, wholesaleClient, 1);

            // Check each auction, format info, add to list, sanitize and ship to db
            #region OVE
            bool hasOVE = auctions.Any(auction => auction["kWholesaleAuction"] == "1");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasOVE)
                Self.DeleteOldRules(kSession, kDealer, 1, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasOVE)
                Self.AutoLaunchFlagSet(kSession, kDealer, 1, 0);

            if (data.ContainsKey("isOVE") && data["isOVE"].ToString() == "1" && op != "delete")
            {
                // Enable AutoLaunch
                Self.AutoLaunchFlagSet(kSession, kDealer, 1, 1);

                // Get Auction Info for defaults
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 1);
                Dictionary<string, string> bidIncrement = LMWholesale.WholesaleSystem.BuildList(auctionInfo["WholesaleBidIncrement"].ToString(), "", '|');
                Dictionary<string, string> locationCode = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleLocationCode"].ToString(), "", '|');
                Dictionary<string, string> auctionCode = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString(), "", '|');

                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {
                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 1 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 3 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale",  0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", bidIncrement["Selected"] },
                        { "kWholesaleLocationCode", locationCode["Selected"] },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", auctionCode["Selected"] },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", 0.0 },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region SmartAuction
            bool hasSA = auctions.Any(auction => auction["kWholesaleAuction"] == "2");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasSA)
                Self.DeleteOldRules(kSession, kDealer, 2, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasSA)
                Self.AutoLaunchFlagSet(kSession, kDealer, 2, 0);

            if (data.ContainsKey("isSmartAuction") && data["isSmartAuction"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 2, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 2);
                Dictionary<string, string> locationCode = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleLocationCode"].ToString(), "", '|');

                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in saSets)
                {
                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 2 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", locationCode["Selected"] },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", 0 },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", data["FloorPricingStrategy"].ToString() },
                        { "FloorPricingAdjustment", adjustment },
                        { "FloorPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", set["isLAPP"] },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", 1 },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region OpenLane
            bool hasADESA = auctions.Any(auction => auction["kWholesaleAuction"] == "4");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasADESA)
                Self.DeleteOldRules(kSession, kDealer, 4, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasADESA)
                Self.AutoLaunchFlagSet(kSession, kDealer, 4, 0);

            if (data.ContainsKey("isOpenLane") && data["isOpenLane"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 4, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 4);
                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {
                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 4 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", 0 },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region COPART
            bool hasCOPART = auctions.Any(auction => auction["kWholesaleAuction"] == "6");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasCOPART)
                Self.DeleteOldRules(kSession, kDealer, 6, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasCOPART)
                Self.AutoLaunchFlagSet(kSession, kDealer, 6, 0);

            if (data.ContainsKey("isCOPART") && data["isCOPART"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 6, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 6);
                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {
                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 6 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", 1 },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", 0 },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", data["FloorPricingStrategy"].ToString() },
                        { "FloorPricingAdjustment", adjustment },
                        { "FloorPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "BuyNowPricingType", 0 },
                        { "BuyNowPricingAdjustment", 0 },
                        { "BuyNowPricingPercentage", "0.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region AuctionEdge
            bool hasAE = auctions.Any(auction => auction["kWholesaleAuction"] == "7");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasAE)
                Self.DeleteOldRules(kSession, kDealer, 7, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasAE)
                Self.AutoLaunchFlagSet(kSession, kDealer, 7, 0);

            if (data.ContainsKey("isAuctionEdge") && data["isAuctionEdge"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 7, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 7);
                Dictionary<string, string> auctionCode = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString(), "", '|');

                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {
                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 7 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", auctionCode["Selected"] },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region ACV Auctions
            bool hasACV = auctions.Any(auction => auction["kWholesaleAuction"] == "11");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasACV)
                Self.DeleteOldRules(kSession, kDealer, 11, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasACV)
                Self.AutoLaunchFlagSet(kSession, kDealer, 11, 0);

            if (data.ContainsKey("isACVAuctions") && data["isACVAuctions"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 11, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 11);
                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {
                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 11 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", 0 },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region IAA
            bool hasIAA = auctions.Any(auction => auction["kWholesaleAuction"] == "13");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasIAA)
                Self.DeleteOldRules(kSession, kDealer, 13, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasIAA)
                Self.AutoLaunchFlagSet(kSession, kDealer, 13, 0);

            if (data.ContainsKey("isIAA") && data["isIAA"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 13, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 13);
                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {
                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 13 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", 2 },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", 0 },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region Auction Simplified
            bool hasAS = auctions.Any(auction => auction["kWholesaleAuction"] == "14");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasAS)
                Self.DeleteOldRules(kSession, kDealer, 14, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasAS)
                Self.AutoLaunchFlagSet(kSession, kDealer, 14, 0);

            if (data.ContainsKey("isAuctionSimplified") && data["isAuctionSimplified"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 14, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 14);
                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {
                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 14 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", 0 },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region IAS
            bool hasIAS = auctions.Any(auction => auction["kWholesaleAuction"] == "15");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasIAS)
                Self.DeleteOldRules(kSession, kDealer, 15, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasIAS)
                Self.AutoLaunchFlagSet(kSession, kDealer, 15, 0);

            if (data.ContainsKey("isIAS") && data["isIAS"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 15, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 15);
                Dictionary<string, string> auctionCode = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString(), "", '|');

                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {

                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 15 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", auctionCode["Selected"] },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region Auction OS
            bool hasAOS = auctions.Any(auction => auction["kWholesaleAuction"] == "16");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasAOS)
                Self.DeleteOldRules(kSession, kDealer, 16, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasAOS)
                Self.AutoLaunchFlagSet(kSession, kDealer, 16, 0);

            if (data.ContainsKey("isAuctionOS") && data["isAuctionOS"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 16, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 16);

                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {

                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 16 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", 0 },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region Carmigo
            bool hasCarmigo = auctions.Any(auction => auction["kWholesaleAuction"] == "17");
            if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasCarmigo)
                Self.DeleteOldRules(kSession, kDealer, 17, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            else if (previousRules == null && hasCarmigo)
                Self.AutoLaunchFlagSet(kSession, kDealer, 17, 0);

            if (data.ContainsKey("isCarmigo") && data["isCarmigo"].ToString() == "1" && op != "delete")
            {
                // Get Auction Info for defaults
                Self.AutoLaunchFlagSet(kSession, kDealer, 17, 1);
                DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 17);

                // Further filtering of Rule Sets
                foreach (Dictionary<string, string> set in sets)
                {

                    Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
                    {
                        { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
                        { "kWholesaleAuction", 17 },
                        { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
                        { "AgeLow", data["MinVehicleAge"].ToString() },
                        { "AgeHigh",  data["MaxVehicleAge"].ToString() },
                        { "Make", "" },
                        { "Model", "" },
                        { "InvLotLocation", "[ANY]" },
                        { "kAASale", 0 },
                        { "kInventoryStatus", 1 },
                        { "kWholesaleListingTag", set["kWholesaleListingTag"] },
                        { "kWholesaleTitleStatus", 0 },
                        { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
                        { "kWholesaleBidIncrement", 0 },
                        { "kWholesaleLocationCode", 0 },
                        { "kWholesaleLocationIndicator", 1 },
                        { "kWholesaleFacilitatedAuctionCode", 0 },
                        { "StartPricingType", 0 },
                        { "StartPricingAdjustment", 0 },
                        { "StartPricingPercentage", "0.0" },
                        { "FloorPricingType", 0 },
                        { "FloorPricingAdjustment", 0 },
                        { "FloorPricingPercentage", "0.0" },
                        { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
                        { "BuyNowPricingAdjustment", binAdjustment },
                        { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
                        { "AltStartType", 0 },
                        { "AltStartAdj", 0 },
                        { "AltStartPct", 0.0 },
                        { "AltFloorType", 0 },
                        { "AltFloorAdj", 0 },
                        { "AltFloorPct", 0.0 },
                        { "AltBuyNowType", 0 },
                        { "AltBuyNowAdj", 0 },
                        { "AltBuyNowPct", 0.0 },
                        { "Disable", 0 },
                        { "PricingAdjMin", data["MinimumPricingAdjustment"] },
                        { "kFuelType", 0 },
                        { "MinMileage", set["MinMileage"] },
                        { "MaxMileage", set["MaxMileage"] },
                        { "MotorYearMin", set["MinYear"] },
                        { "MotorYearMax", set["MaxYear"] },
                        { "kWholesaleVehicleType", 0 },
                        { "kDealerWholesaleCredential", 0 },
                        { "PostAsYellowLight", 0 },
                        { "MinGrade", data["MinGrade"].ToString() },
                        { "MaxGrade", data["MaxGrade"].ToString() },
                        { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
                        { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
                        { "isInternal", 0 },
                        { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
                    };

                    AutoLaunchRules.Add(AutoLaunchRuleStruct);
                }
            }
            #endregion

            #region CarOffer
            //bool hasCarOffer = auctions.Any(auction => auction["kWholesaleAuction"] == "18");
            //if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && hasCarOffer)
            //    Self.DeleteOldRules(kSession, kDealer, 18, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            //else if (previousRules == null && hasCarOffer)
            //    Self.AutoLaunchFlagSet(kSession, kDealer, 18, 0);
            //
            //if (data.ContainsKey("isCarOffer") && data["isCarOffer"].ToString() == "1" && op != "delete")
            //{
            //    // Get Auction Info for defaults
            //    Self.AutoLaunchFlagSet(kSession, kDealer, 18, 1);
            //    DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 18);
            //
            //    // Further filtering of Rule Sets
            //    foreach (Dictionary<string, string> set in sets)
            //    {
            //
            //        Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
            //        {
            //            { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
            //            { "kWholesaleAuction", 18 },
            //            { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
            //            { "AgeLow", data["MinVehicleAge"].ToString() },
            //            { "AgeHigh",  data["MaxVehicleAge"].ToString() },
            //            { "Make", "" },
            //            { "Model", "" },
            //            { "InvLotLocation", "[ANY]" },
            //            { "kAASale", 0 },
            //            { "kInventoryStatus", 1 },
            //            { "kWholesaleListingTag", set["kWholesaleListingTag"] },
            //            { "kWholesaleTitleStatus", 0 },
            //            { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
            //            { "kWholesaleBidIncrement", 0 },
            //            { "kWholesaleLocationCode", 0 },
            //            { "kWholesaleLocationIndicator", 1 },
            //            { "kWholesaleFacilitatedAuctionCode", 0 },
            //            { "StartPricingType", 0 },
            //            { "StartPricingAdjustment", 0 },
            //            { "StartPricingPercentage", "0.0" },
            //            { "FloorPricingType", 0 },
            //            { "FloorPricingAdjustment", 0 },
            //            { "FloorPricingPercentage", "0.0" },
            //            { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
            //            { "BuyNowPricingAdjustment", binAdjustment },
            //            { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
            //            { "AltStartType", 0 },
            //            { "AltStartAdj", 0 },
            //            { "AltStartPct", 0.0 },
            //            { "AltFloorType", 0 },
            //            { "AltFloorAdj", 0 },
            //            { "AltFloorPct", 0.0 },
            //            { "AltBuyNowType", 0 },
            //            { "AltBuyNowAdj", 0 },
            //            { "AltBuyNowPct", 0.0 },
            //            { "Disable", 0 },
            //            { "PricingAdjMin", data["MinimumPricingAdjustment"] },
            //            { "kFuelType", 0 },
            //            { "MinMileage", set["MinMileage"] },
            //            { "MaxMileage", set["MaxMileage"] },
            //            { "MotorYearMin", set["MinYear"] },
            //            { "MotorYearMax", set["MaxYear"] },
            //            { "kWholesaleVehicleType", 0 },
            //            { "kDealerWholesaleCredential", 0 },
            //            { "PostAsYellowLight", 0 },
            //            { "MinGrade", data["MinGrade"].ToString() },
            //            { "MaxGrade", data["MaxGrade"].ToString() },
            //            { "RequireCR", auctionInfo["RequireConditionReport"].ToString() },
            //            { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
            //            { "isInternal", 0 },
            //            { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
            //        };
            //
            //        AutoLaunchRules.Add(AutoLaunchRuleStruct);
            //    }
            //}
            #endregion

            #region RemarketingPlus
            //bool hasRemarketingPlus = auctions.Any(auction => auction["kWholesaleAuction"] == "19");
            //if (previousRules != null && data["kWholesaleAuctionRuleSet"].ToString() != "" && RemarketingPlus)
            //    Self.DeleteOldRules(kSession, kDealer, 19, data["kWholesaleAuctionRuleSet"].ToString(), previousRules.Tables[0]);
            //else if (previousRules == null && RemarketingPlus)
            //    Self.AutoLaunchFlagSet(kSession, kDealer, 19, 0);
            //
            //if (data.ContainsKey("isRemarketingPlus") && data["isRemarketingPlus"].ToString() == "1" && op != "delete")
            //{
            //    // Get Auction Info for defaults
            //    Self.AutoLaunchFlagSet(kSession, kDealer, 19, 1);
            //    DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, 19);
            //
            //    // Further filtering of Rule Sets
            //    foreach (Dictionary<string, string> set in sets)
            //    {
            //
            //        Dictionary<string, object> AutoLaunchRuleStruct = new Dictionary<string, object>
            //        {
            //            { "kWholesaleAutoLaunch", 0 } ,// We cannot determine if a previous AutoLaunch rule existed, default to new unless otherwise stated
            //            { "kWholesaleAuction", 19 },
            //            { "Duration", data["Allow1Day"].ToString() == "1" ? 1 : 5 },
            //            { "AgeLow", data["MinVehicleAge"].ToString() },
            //            { "AgeHigh",  data["MaxVehicleAge"].ToString() },
            //            { "Make", "" },
            //            { "Model", "" },
            //            { "InvLotLocation", "[ANY]" },
            //            { "kAASale", 0 },
            //            { "kInventoryStatus", 1 },
            //            { "kWholesaleListingTag", set["kWholesaleListingTag"] },
            //            { "kWholesaleTitleStatus", 0 },
            //            { "kWholesaleListingCategory", set["kWholesaleListingCategory"] },
            //            { "kWholesaleBidIncrement", 0 },
            //            { "kWholesaleLocationCode", 0 },
            //            { "kWholesaleLocationIndicator", 1 },
            //            { "kWholesaleFacilitatedAuctionCode", 0 },
            //            { "StartPricingType", 0 },
            //            { "StartPricingAdjustment", 0 },
            //            { "StartPricingPercentage", "0.0" },
            //            { "FloorPricingType", 0 },
            //            { "FloorPricingAdjustment", 0 },
            //            { "FloorPricingPercentage", "0.0" },
            //            { "BuyNowPricingType", data["FloorPricingStrategy"].ToString() },
            //            { "BuyNowPricingAdjustment", binAdjustment },
            //            { "BuyNowPricingPercentage", data["FloorPricingStrategy"].ToString() == "24" ? data["MMRPercentage"].ToString() : "1.0" },
            //            { "AltStartType", 0 },
            //            { "AltStartAdj", 0 },
            //            { "AltStartPct", 0.0 },
            //            { "AltFloorType", 0 },
            //            { "AltFloorAdj", 0 },
            //            { "AltFloorPct", 0.0 },
            //            { "AltBuyNowType", 0 },
            //            { "AltBuyNowAdj", 0 },
            //            { "AltBuyNowPct", 0.0 },
            //            { "Disable", 0 },
            //            { "PricingAdjMin", data["MinimumPricingAdjustment"] },
            //            { "kFuelType", 0 },
            //            { "MinMileage", set["MinMileage"] },
            //            { "MaxMileage", set["MaxMileage"] },
            //            { "MotorYearMin", set["MinYear"] },
            //            { "MotorYearMax", set["MaxYear"] },
            //            { "kWholesaleVehicleType", 0 },
            //            { "kDealerWholesaleCredential", 0 },
            //            { "PostAsYellowLight", 0 },
            //            { "MinGrade", data["MinGrade"].ToString() },
            //            { "MaxGrade", data["MaxGrade"].ToString() },
            //            { "RequireCR", 1 },
            //            { "kListingCategoryNoCR", set["kWholesaleListingCategory"] },
            //            { "isInternal", 0 },
            //            { "kWholesaleAuctionRuleSet", data["kWholesaleAuctionRuleSet"].ToString() }
            //        };
            //
            //        AutoLaunchRules.Add(AutoLaunchRuleStruct);
            //    }
            //}
            #endregion

            foreach (Dictionary<string, object> rule in AutoLaunchRules)
                responses.Add(Self.SanitizeAndShip(kSession, kDealer, op, rule));

            // We will dissect responses after we gather them all
            return responses;
        }

        private void DeleteOldRules(string kSession, int kDealer, int kWholesaleAuction, string kWholesaleAuctionRuleSet, DataTable oldRules)
        {
            // Loop through previously created rules and disable them for future rules
            DataRow[] rules = oldRules.Select($"kWholesaleAuction = {kWholesaleAuction} AND kWholesaleAuctionRuleSet = {kWholesaleAuctionRuleSet} AND isInternal <> 1");

            foreach (DataRow dr in rules)
            {
                Dictionary<string, object> oldRule = new Dictionary<string, object>();
                foreach (DataColumn dc in oldRules.Columns)
                    oldRule.Add(dc.ColumnName, dr[dc.ColumnName]);

                oldRule["Disable"] = 1;

                // Inefficient looping but we are doing what is needing to be done
                Self.wholesaleClient.WholesaleAutoLaunchSetData(kSession, kDealer, Util.serializer.Serialize(oldRule));
            }

            DataRow[] existingRules = oldRules.Select($"kWholesaleAuction = {kWholesaleAuction} AND kWholesaleAuctionRuleSet <> {kWholesaleAuctionRuleSet} AND isInternal <> 1");
            if (existingRules.Length == 0)
                Self.AutoLaunchFlagSet(kSession, kDealer, kWholesaleAuction, 0);
        }

        // Per Request of Kelly
        private void AutoLaunchFlagSet(string kSession, int kDealer, int kWholesaleAuction, int enable = 1)
        {
            DataRow auctionInfo = Self.GetAuctionInfo(kSession, kDealer, kWholesaleAuction);
            if (auctionInfo["AutoLaunchEnabled"].ToString() != enable.ToString())
            {
                Wholesale.lmReturnValue rv = Self.wholesaleClient.WholesaleAuctionByDealerAutoLaunchSet(kSession, kDealer, kWholesaleAuction, enable);
                if (rv.Result == Wholesale.ReturnCode.LM_SUCCESS)
                    return;
            }
        }
    }
}