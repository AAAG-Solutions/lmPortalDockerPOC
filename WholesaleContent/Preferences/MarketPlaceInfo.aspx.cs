using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

using LMWholesale.Common;

namespace LMWholesale.WholesaleContent.Preferences
{
    public partial class MarketPlaceInfo : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Preferences.MarketPlaceInfo marketPlaceInfo;

        public MarketPlaceInfo()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            marketPlaceInfo = marketPlaceInfo ?? new BLL.WholesaleContent.Preferences.MarketPlaceInfo();
        }

        public static MarketPlaceInfo Self
        {
            get { return instance; }
        }
        private static readonly MarketPlaceInfo instance = new MarketPlaceInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Preferences";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];
                List<Dictionary<string, string>> auctions = marketPlaceInfo.GetAuctions(kSession, kDealer);
                string auctionDropdown = "[]";
                Page page = (Page)HttpContext.Current.Handler;
                Panel AuctionSummariesPanel = (Panel)page.Master.FindControl("MainContent").FindControl("AuctionSummaries");
                Panel summaryHeaderPanel = new Panel() { CssClass = "tableRow" };

                summaryHeaderPanel.Controls.Add(new Label
                {
                    Text = "Auction Name:",
                    CssClass = "tableCellSideBorder"
                });
                summaryHeaderPanel.Controls.Add(new Label
                {
                    Text = "Enabled:",
                    CssClass = "tableCellSideBorder"
                });
                summaryHeaderPanel.Controls.Add(new Label
                {
                    Text = "Seller ID:",
                    CssClass = "tableCellSideBorder"
                });
                summaryHeaderPanel.Controls.Add(new Label
                {
                    Text = "Credentials Count:",
                    CssClass = "tableCellSideBorder"
                });

                AuctionSummariesPanel.Controls.Add(summaryHeaderPanel);

                string LotList = Self.marketPlaceInfo.GetListingLotLocationList(kSession, kDealer);
                // Until the storedProc, 'pWholesaleAuctionDealerGet', is more efficient, we will have to invoke the proc in a loop
                // FIXME: Highly inefficient!
                foreach (Dictionary<string, string> auction in auctions)
                {
                    DataTable auctionDataTable = marketPlaceInfo.GetAuctionData(kSession, kDealer, int.Parse(auction["kWholesaleAuction"], 0));
                    Dictionary<string, string> auctionInfo = new Dictionary<string, string>();

                    foreach (DataColumn dc in auctionDataTable.Columns)
                        auctionInfo[dc.ColumnName] = auctionDataTable.Rows[0][dc].ToString();

                    AucitonInfo.InnerHtml += marketPlaceInfo.BuildListingInfo(auctionInfo, int.Parse(auction["kWholesaleAuction"], 0));
                    auctionDropdown += $"{auction["WholesaleAuctionName"]}:{(auction["WholesaleAuctionName"] == "RemarketingPlus" ? "Remarketing+" : auction["WholesaleAuctionName"])}|";

                    // Initialize auction credential jsGrids on page
                    jsGridBuilder grid = marketPlaceInfo.GetJsGridBuilderInfo(int.Parse(auction["kWholesaleAuction"], 0), $"MarketPlaceInfo.aspx/GetAuctionCredentials");
                    if (!ClientScript.IsStartupScriptRegistered($"{auction["WholesaleAuctionName"].Replace(" ", "")}JsScript"))
                        ClientScript.RegisterStartupScript(this.GetType(), $"{auction["WholesaleAuctionName"].Replace(" ", "")}Grid", grid.RenderGrid());

                    Panel summaryPanel = new Panel() { CssClass = "tableRow" };

                    summaryPanel.Controls.Add(new Label
                    {
                        Text = auction["WholesaleAuctionName"] == "RemarketingPlus" ? "Remarketing+" : auction["WholesaleAuctionName"],
                        CssClass = "tableCell"
                    });
                    summaryPanel.Controls.Add(new CheckBox
                    {
                        Checked = auctionInfo["Enabled"] == "1" ? true : false,
                        CssClass = "tableCellSideBorder",
                        Enabled = false
                    });
                    summaryPanel.Controls.Add(new Label
                    {
                        Text = auctionInfo["SellerID"],
                        CssClass = "tableCellSideBorder"
                    });
                    summaryPanel.Controls.Add(new Label
                    {
                        Text = "",
                        CssClass = "tableCell",
                        ID = (auction["WholesaleAuctionName"].Replace(" ", "")) + "CredCount"
                    });

                    AuctionSummariesPanel.Controls.Add(summaryPanel);
                    WholesaleSystem.PopulateList(LotList, "", $"{auction["WholesaleAuctionName"].Replace(" ", "")}InvLotLocation", '|');
                    WholesaleSystem.PopulateList(GetListValues(auctionInfo["ContactGroup"].ToString()), "-- No Contact Group --", $"{auction["WholesaleAuctionName"].Replace(" ", "")}kContactGroup", '|');
                    WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleInspectionCompany"].ToString()), "-- Select an Inspection Company --", $"{auction["WholesaleAuctionName"].Replace(" ", "")}kWholesaleInspectionCompany", '|');

                    switch (auction["WholesaleAuctionName"])
                    {
                        // There is a lot of information that is dependent on OVE
                        // #TODO: Maybe seperate this dependency? Not sure how much rework it would take
                        case "OVE":
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["kWholesaleLocationCode"].ToString()), "-- Select Location Code --", "OVEkWholesaleLocationCode", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleListingType"].ToString()), "-- Select a Listing Type --", "OVEkWholesaleListingTag", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "OVEkWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString()), "-- Select Location Code --", "OVEkWholesaleFacilitatedAuctionCode", '|');

                            // Populate General Settings
                            WholesaleSystem.AutoGradeScaleGet("DefaultGrade", auctionInfo["MMRDefaultGrade"].ToString());
                            WholesaleSystem.PopulateList(auctionInfo["MMRRegionCode"].ToString(), "-- Select a Location Code --", "MMRRegions", '|');
                            WholesaleSystem.PopulateList(auctionInfo["WholesaleLocationIndicator"].ToString(), "-- Select a Physical Location --", "PhysicalLocations", '|');
                            WholesaleSystem.PopulateList(auctionInfo["WholesaleListingType"].ToString(), "-- Select a Listing Type --", "ListingType", '|');
                            WholesaleSystem.PopulateList(auctionInfo["WholesaleInspectionCompany"].ToString(), "-- Select an Inspection Company --", "InspectionCompany", '|');

                            // Check Boxes
                            AutoEndWholesale.Checked = auctionInfo["AutoEndWholesale"].ToString() != "0";
                            AutoEndeBay.Checked = auctionInfo["AutoEndeBay"].ToString() != "0";
                            PricingManual.Checked = auctionInfo["UseInventoryPrice"].ToString() != "0";
                            ForcePrice.Checked = auctionInfo["ForceWholesalePricing"].ToString() != "0";
                            IndustryGrade.Checked = auctionInfo["IndustryGradeIsAutoGrade"].ToString() != "0";
                            NAAAGrade.Checked = auctionInfo["ReportedGradeIsNAAAGrade"].ToString() != "0";

                            // Text Boxes
                            ListingDuration.Text = auctionInfo["Duration"].ToString();
                            AutoRelistCount.Text = auctionInfo["RelistCount"].ToString();
                            MinWholesale.Text = Convert.ToInt32(Convert.ToDouble(auctionInfo["MinWholesalePrice"])).ToString();
                            MinMMRThreshold.Text = Convert.ToInt32(Convert.ToDouble(auctionInfo["MinMMRThreshold"])).ToString();
                            MaxMMRReserve.Text = Convert.ToInt32(Convert.ToDouble(auctionInfo["MMRMaxReserve"])).ToString();
                            MaxMMRBIN.Text = Convert.ToInt32(Convert.ToDouble(auctionInfo["MMRMaxBuyNow"])).ToString();
                            MaxMVGReserve.Text = Convert.ToInt32(Convert.ToDouble(auctionInfo["MVGMaxReserve"])).ToString();
                            MaxMVGBIN.Text = Convert.ToInt32(Convert.ToDouble(auctionInfo["MVGMaxBuyNow"])).ToString();

                            // Auction Specifics that rely on info from OVE
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleListingType"].ToString()), "-- Select a Listing Type --", "ADESAkWholesaleListingTag", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleListingType"].ToString()), "-- Select a Listing Type --", "IASkWholesaleListingTag", '|');

                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString()), "-- Select Location Code --", "IASkWholesaleFacilitatedAuctionCode", '|');

                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "ADESAkWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "COPARTkWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "AuctionEdgekWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "ACVAuctionskWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "IAAkWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "AuctionSimplifiedkWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "IASkWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "AuctionOSkWholesaleLocationIndicator", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleLocationIndicator"].ToString()), "-- Select a Physical Location --", "SmartAuctionkWholesaleLocationIndicator", '|');
                            break;
                        case "SmartAuction":
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["WholesaleInspectionCompany"].ToString()), "-- Select an Inspection Company --", "SmartAuctionkWholesaleInspectionCompany", '|');
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["kWholesaleLocationCode"].ToString()), "-- Select Location Code --", "SmartAuctionkWholesaleLocationCode", '|');
                            break;
                        case "AuctionEdge":
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString()), "-- Select Location Code --", "AuctionEdgekWholesaleFacilitatedAuctionCode", '|');
                            break;
                        case "IAS":
                            WholesaleSystem.PopulateList(GetListValues(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString()), "-- Select Location Code --", "IASkWholesaleFacilitatedAuctionCode", '|');
                            break;
                        default:
                            break;
                    }
                }

                WholesaleSystem.PopulateList(auctionDropdown, "", "lstAuctionSelector", '|', "");
                WholesaleSystem.GetBidIncrements("OVEkWholesaleBidIncrement");
            }
        }

        // Credential GET WebMethod
        [WebMethod(Description = "Gather Gather all Auction Credentials associated for a given dealer with a given kWholesaleAuctionName")]
        public static string GetAuctionCredentials(string filter, string ExtraParams)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            Dictionary<string, object> extraParams = (Dictionary<string, object>)Util.serializer.DeserializeObject(ExtraParams);
            //Dictionary<string, object> oFilter = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);
            int num = 0;
            string items = "{}";

            DataTable dt = Self.marketPlaceInfo.GetAuctionCredentials((string)Session["kSession"], (int)Session["kDealer"], extraParams["kWholesaleAuctionName"].ToString());
            if (dt.Rows.Count > 0)
            {
                num = dt.Rows.Count;
                items = Util.serializer.Serialize(FormatData(dt));
            }

            return $"{num}|{items}";
        }

        // Credential SET WebMethod
        [WebMethod(Description = "Save Credentials given an Auction")]
        public static Dictionary<string, object> SetAuctionCredential(string jsonData)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            //Dictionary<string, object> json = (Dictionary<string, object>)Util.serializer.DeserializeObject(jsonData);
            Wholesale.lmReturnValue rv = Self.marketPlaceInfo.SetAuctionCredential(kSession, kDealer, jsonData);
            if (rv.Result != Wholesale.ReturnCode.LM_SUCCESS)
            {
                IsSuccess = false;
                Message = rv.ResultString.Replace("DB Error: ", "");
            }

            return ReturnResponse();
        }

        // Auction SET WebMethod
        [WebMethod(Description = "Save Settings given an Auction")]
        public static Dictionary<string, object> SaveAuctionSettings(string jsonData)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            Dictionary<string, object> dataIn = (Dictionary<string, object>)Util.serializer.DeserializeObject(jsonData);
            Wholesale.lmReturnValue saveResult = Self.marketPlaceInfo.SaveAuction(kSession, kDealer, dataIn);
            if (saveResult.Result != Wholesale.ReturnCode.LM_SUCCESS)
            {
                IsSuccess = false;
                Message = saveResult.ResultString;
            }

            WholesaleSystem.ClearCachedObject("availableAuctions1");
            WholesaleSystem.GetAvailableAuctions(kSession, kDealer, new resource.clients.WholesaleClient(), 1);
            return ReturnResponse();
        }

        // ALL Auction SET WebMethod
        // #TODO: Might be easier to consolidate
        [WebMethod(Description = "Save ALL Settings given ALL Auctions")]
        public static Dictionary<string, object> SaveAllAuctions(List<Dictionary<string, object>> jsonData)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            StringBuilder errors = new StringBuilder("Unable to save the following auctions:\n");
            foreach (Dictionary<string, object> auction in jsonData)
            {
                Wholesale.lmReturnValue result = Self.marketPlaceInfo.SaveAuction(kSession, kDealer, auction);
                if (result.Result != Wholesale.ReturnCode.LM_SUCCESS)
                {
                    errors.Append($"{result.ResultString}\n");
                    IsSuccess = false;
                }
            }

            WholesaleSystem.ClearCachedObject("availableAuctions1");
            WholesaleSystem.GetAvailableAuctions(kSession, kDealer, new resource.clients.WholesaleClient(), 1);

            Message = errors.ToString();
            return ReturnResponse();
        }

        private static List<Dictionary<string, object>> FormatData(DataTable dt)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                    dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));

                if (dict["InvLotLocation"].ToString() == "[ANY]")
                    dict["InvLotLocation"] = "Any Lot";

                returnList.Add(dict);
            }

            return returnList;
        }

        private string GetListValues(string dbValues)
        {
            dbValues = dbValues.Substring(dbValues.IndexOf(']'));
            dbValues = $"[{dbValues}";
            return dbValues;
        }
    }
}