using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;

using LMWholesale.Common;
using LMWholesale.resource.clients;
using LMWholesale.resource.model.Wholesale;

namespace LMWholesale.WholesaleContent.Auction
{
    public partial class MultiStart : lmPage
    {
        private readonly BLL.WholesaleContent.Auction.MultiStart multiStartBLL;
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly WholesaleClient wholesaleClient;

        public MultiStart()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            multiStartBLL = multiStartBLL ?? new BLL.WholesaleContent.Auction.MultiStart();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }

        public static MultiStart Self
        {
            get { return instance; }
        }
        private static readonly MultiStart instance = new MultiStart();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "MultiStart Wholesale";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);
            Util.serializer.MaxJsonLength = Int32.MaxValue;

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                    BLL.WholesaleUser.WholesaleUser.ClearUser();

                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];

                // Gather Filter and Advanced Filters
                InventoryFilter.MultiPageFilter multiPageFilter = new InventoryFilter.MultiPageFilter(
                    new InventoryFilter.Filter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["filters"].ToString())),
                    new InventoryFilter.AdvancedFilter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["advancedFilter"].ToString()))
                    );

                // On first load, we want to always default to page 1; otherwise, this page acts weird
                Session["MultiFirstLoad"] = true;

                jsGridBuilder startGrid = new jsGridBuilder
                {
                    MethodURL = "MultiStart.aspx/StartWholesaleSearch",
                    HTMLElement = "jsGrid",
                    Filtering = false,
                    ExtraFunctionality = "ListingTypeChange();",
                    PageSize = multiPageFilter.ItemsPerPage,
                    NotSortableFields = new List<string>() { "Auctions", "VINStock", "InvCost", "Grade", "StartPriceBox", "ReservePriceBox", "BuyItNowPriceBox", "ErrMsg" }
                };

                string searchVehicleGridColumns = "<:Auctions::50::|!:VIN::10::|:VINStock:VIN / Stock:65::|:InvDays:Age:20:number:true|:LotLocation:Lot Location:50::true|:Mileage::30:number:true|!:InvLotLocation::10::|:InvCost:Cost:30::|:MMRGoodPrice:MMR $:30::|:Grade::30::|:StartPriceBox:Start $:30::|:ReservePriceBox:Reserve $:30::|:BuyItNowPriceBox:Buy Now $:30::|:ErrMsg:Error Messages:60::";
                startGrid.SetFieldListFromGridDef(searchVehicleGridColumns, "", true);
                WholesaleSystem.PopulateList("[]0:Use Default|2:OEM-CPO|4:Front Line Ready|5:As Described|7:As-Is|8:Standard|", "", "lstCategory", '|', "0");

                // Set Inventory Status and Vehicle Type checks
                StatusAvailable.Checked = multiPageFilter.StatusAvailable == 1 ? true : false;
                StatusInTransit.Checked = multiPageFilter.StatusInTransit == 1 ? true : false;
                TypeDealerCertified.Checked = multiPageFilter.TypeDealerCertified == 1 ? true : false;
                TypeManufacturerCertified.Checked = multiPageFilter.TypeManufacturerCertified == 1 ? true : false;
                TypePreOwned.Checked = multiPageFilter.TypePreOwned == 1 ? true : false;

                AuctionInfo ai = new AuctionInfo();
                Self.multiStartBLL.GetAuctionData(kSession, kDealer, ref ai);

                WholesaleSystem.PopulateList(ai.WholesaleListingType, "", "lstType", '|', "1");
                if (ai.AuctionCount != 0)
                {
                    WholesaleSystem.PopulateList(ai.QuickSelect, "", "lstSelect", '|', "0");
                    List<Dictionary<string, string>> lstAuction = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 1);
                    string lstAuctions = "";
                    foreach (Dictionary<string, string> auction in lstAuction)
                    {
                        if (auction["WholesaleAuctionName"] == "CarOffer")
                            continue;

                        lstAuctions += $"{auction["kWholesaleAuction"]}:Not Listed on {(auction["WholesaleAuctionName"] == "RemarketingPlus" ? "Remarketing+" : auction["WholesaleAuctionName"])}|";
                    }

                    string lstStatus = $"[]0:Any Listing Status|-1:Listed Vehicles|-2:Not Listed Vehicles|{lstAuctions}";
                    WholesaleSystem.PopulateList(lstStatus, "", "lstListingStatus", '|', multiPageFilter.ListingStatus.ToString());

                    // Inspection Status Filter Dropdown
                    string lstInspection = "[]-1:Any Inspection Status|1:Inspected Vehicles|0:Not Inspected Vehicles|";
                    WholesaleSystem.PopulateList(lstInspection, "", "lstInspectionStatus", '|', multiPageFilter.InspectionStatus.ToString());

                    // Listing Lot Location Results
                    Self.multiStartBLL.GetListingLotLocationList(kSession, kDealer, multiPageFilter.LotLocation);

                    WholesaleSystem.PopulateList(ai.WholesaleListingType, "", "lstType", '|', "1");
                    if (ai.AuctionCount != 0)
                    {
                        WholesaleSystem.PopulateList(ai.QuickSelect, "", "lstSelect", '|', "0");

                        // Save to Session in order to prevent another db hit
                        // We will only populate this on this PageLoad
                        Session["MSWAuctions"] = ai.AuctionString;

                        if (ai.UseInventoryPrice == 1 && ai.ForceWholesalePricing == 1)
                        {
                            chkForce.Style["display"] = "table-row";
                            chkForceWholesalePrice.Checked = true;
                            setStart.Disabled = setReserve.Disabled = setBIN.Disabled = true;
                            setStart.Style["opacity"] = setReserve.Style["opacity"] = setBIN.Style["opacity"] = "10%";
                        }
                        RelistCount.Value = ai.RelistCount;
                        MMRMaxThreshold.Value = ai.MMRMaxBuyNow;
                        MMRMinThreshold.Value = ai.MinMMRThreshold;
                        BidIncrement.Value = ai.BidIncrement;
                    }

                    if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                        ClientScript.RegisterStartupScript(this.GetType(), "startWholesaleGrid", startGrid.RenderGrid());
                }
            }
        }

        [WebMethod]
        public static string StartWholesaleSearch(string filter)
        {
            Dictionary<string, object> sortFilter = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            bool hasAutoGrade = ((List<string>)Session["UserPermissions"]).Where(x => x.Contains("AutoGrade")).Count() > 0;

            // Gather Filter and Advanced Filters
            InventoryFilter.MultiPageFilter multiPageFilter = new InventoryFilter.MultiPageFilter(
                        new InventoryFilter.Filter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["filters"].ToString())),
                        new InventoryFilter.AdvancedFilter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["advancedFilter"].ToString()))
            );

            bool firstLoad = false;
            if (bool.Parse(Session["MultiFirstLoad"].ToString()))
            {
                firstLoad = true;
                Session["MultiFirstLoad"] = false;

            }
            else {
                // If there exists a MultiPageFilter object in Session, use it and save it again to Session since it is not the first time we hit this page
                multiPageFilter = new InventoryFilter.MultiPageFilter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["MultiPageFilter"].ToString()));
            }

            Tuple< InventoryFilter.MultiPageFilter, string> returnvalues =
                Self.multiStartBLL.StartWholesaleSearch(kSession, kDealer, Session["MSWAuctions"].ToString(), multiPageFilter, hasAutoGrade, sortFilter, firstLoad);

            // Save to Session to we can use it when pagination is used
            Session["MultiPageFilter"] = Util.serializer.Serialize(returnvalues.Item1);

            return returnvalues.Item2;
        }

        [WebMethod]
        public static Dictionary<string, object> SubmitToMultipleAuctions(string info, string additionalInfo)
        {
            object[] items = (object[])Util.serializer.DeserializeObject(info);
            Dictionary<string, object> addInfo = Util.serializer.Deserialize<Dictionary<string, object>>(additionalInfo);

            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            string errorMsgs = "";
            int successCount = 0;
            int unSuccessCount = 0;
            if (!Self.multiStartBLL.SubmitToMultipleAuctions(kSession, kDealer, items, addInfo, ref errorMsgs, ref successCount, ref unSuccessCount))
                IsSuccess = false;
            Message = $"Vehicles successfully submitted: {successCount}\r\nVehicles failed to submit: {unSuccessCount}\r\nUnsuccessful listing error messages: \r\n{errorMsgs}";

            return ReturnResponse();
        }

        public class AuctionInfo
        {
            public int AuctionCount { get; set; }
            public string AuctionString { get; set; }
            public string QuickSelect { get; set; } = "[]0:-- Quick Select Inventory --|1:Select All|2:Deselect All|";
            public string WholesaleListingType { get; set; }
            public int UseInventoryPrice { get; set; } = 0;
            public int ForceWholesalePricing { get; set; } = 0;
            public string RelistCount { get; set; } = "1";
            public string MMRMaxReserve { get; set; }
            public string MMRMaxBuyNow { get; set; }
            public string MVGMaxReserve { get; set; }
            public string MVGMaxBuyNow { get; set; }
            public string MinMMRThreshold { get; set; }
            public string WholesaleBidIncrement { get; set; }
            public string BidIncrement { get; set; } = "1";

            public void AddDbInfo(DataRow dr, bool hasAdesa)
            {
                PropertyInfo[] props = GetType().GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (dr.Table.Columns.Contains(p.Name))
                    {
                        if (GetType().GetProperty(p.Name).PropertyType.Name == "Int32")
                            GetType().GetProperty(p.Name).SetValue(this, int.Parse(dr[p.Name].ToString()), null);
                        else
                            GetType().GetProperty(p.Name).SetValue(this, dr[p.Name].ToString(), null);
                    }
                }

                if (hasAdesa)
                {
                    string bidString = dr["WholesaleBidIncrement"].ToString();
                    BidIncrement = bidString.StartsWith("[]") ? "1" : bidString.Substring(1 + bidString.IndexOf("]") - 1);
                }
            }
        }
    }
}