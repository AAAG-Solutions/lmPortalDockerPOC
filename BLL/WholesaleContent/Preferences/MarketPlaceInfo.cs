using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using LMWholesale.resource.clients;
using LMWholesale.resource.factory;

namespace LMWholesale.BLL.WholesaleContent.Preferences
{
    public class MarketPlaceInfo
    {
        private readonly WholesaleUser.WholesaleUser userBLL;
        private readonly WholesaleClient wholesaleClient;
        private readonly DealerClient dealerClient;
        private readonly AuctionFactory auctionFactory;

        public MarketPlaceInfo() {
            userBLL = userBLL ?? new WholesaleUser.WholesaleUser();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            dealerClient= dealerClient ?? new DealerClient();
            auctionFactory = auctionFactory ?? new AuctionFactory();
        }

        public MarketPlaceInfo(WholesaleUser.WholesaleUser userBLL, WholesaleClient wholesaleClient, DealerClient dealerClient, AuctionFactory auctionFactory)
        {
            this.userBLL = userBLL;
            this.wholesaleClient = wholesaleClient;
            this.dealerClient = dealerClient;
            this.auctionFactory = auctionFactory;
        }
        internal static readonly MarketPlaceInfo instance = new MarketPlaceInfo();
        public MarketPlaceInfo Self
        {
            get { return instance; }
        }

        public Wholesale.lmReturnValue SaveAuction(string kSession, int kDealer, Dictionary<string, object> dataIn)
        {
            Dictionary<string, string> dataOut = new Dictionary<string, string>() { { "kDealer", kDealer.ToString() }, { "kWholesaleFacilitatedCode", "0"} };

            // Each key should match paramter name in storedproc
            // If not, then fix it
            dataIn.ToList().ForEach(dp => { dataOut.Add(dp.Key, dp.Value.ToString()); });

            // This is a little hack due to naming convenstions between procs...
            // #TODO: Need to fix this someday...
            if (dataOut["kWholesaleAuction"] == "1" || dataOut["kWholesaleAuction"] == "7" || dataOut["kWholesaleAuction"] == "15")
                dataOut["kWholesaleFacilitatedCode"] = dataOut["kWholesaleFacilitatedAuctionCode"];
            return Self.wholesaleClient.WholesaleAuctionByDealerSet(kSession, Util.serializer.Serialize(dataOut));
        }

        public List<Dictionary<string, string>> GetAuctions(string kSession, int kDealer)
        {
            return LMWholesale.WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient);
        }

        public DataTable GetAuctionData(string kSession, int kDealer, int kWholesaleAuction)
        {
            // Should be only one row as it pertains to one dealer/account
            return Self.auctionFactory.GetAuctionService(kWholesaleAuction).GetAuctionInfo(kSession, kDealer).DataSet.Tables[0];
        }

        public DataTable GetAuctionCredentials(string kSession, int kDealer, string auction)
        {
            return Self.auctionFactory.GetAuctionService(auction).GetCredentials(kSession, kDealer);
        }

        public Wholesale.lmReturnValue SetAuctionCredential(string kSession, int kDealer, string jsonData)
        {
            return Self.wholesaleClient.SetWholesaleAuctionCredential(kSession, kDealer, jsonData);
        }

        public string BuildListingInfo(Dictionary<string, string> AuctionInfo, int kWholesaleAuction)
        {
            return Self.auctionFactory.GetAuctionService(kWholesaleAuction).BuildListingInfo(Auction.PopulateInfo(AuctionInfo));
        }

        public jsGridBuilder GetJsGridBuilderInfo(int kWholesaleAuction, string PassThroughString)
        {
            return Self.auctionFactory.GetAuctionService(kWholesaleAuction).GetJsGridBuilderInfo(PassThroughString);
        }

        public string GetListingLotLocationList(string kSession, int kDealer)
        {
            Dealer.lmReturnValue lstLot = Self.dealerClient.LotLocationListGet(kSession, kDealer);
            // DealerLotLocation Results
            StringBuilder lotString = new StringBuilder("[][ANY]:Any Lot Location|");
            if (lstLot.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                if (lstLot.Data != null)
                {
                    foreach (DataRow row in lstLot.Data.Tables[0].Rows)
                    {
                        if (row["LotDescription"].ToString() != "")
                            lotString.Append($"{row["InvLotLocation"]}:{row["InvLotLocation"]} ({row["LotDescription"]})|");
                        else
                            lotString.Append($"{row["InvLotLocation"]}:{row["InvLotLocation"]} ({row["InvLotLocation"]})|");
                    }
                }
            }

            return lotString.ToString();
        }

        public class Auction
        {
            /// <summary>
            ///     This is a common class object that is used within ALL auctions to display information
            /// </summary>
            public class Info
            {
                // CheckBox Values
                public string Enabled { get; set; } = "";
                public string AutoLaunchEnabled { get; set; } = "";
                public string ExemptTireDamage { get; set; } = "";
                public string AdhocEnabled { get; set; } = "";
                public string RequireConditionReport { get; set; } = "";
                public string AllowOverlay { get; set; } = "";
                public string ALRequireStyle { get; set; } = "";
                public string IncludeOwnerName { get; set; } = "";
                public string SuppressManualLaunch { get; set; } = "";
                public string SuppressOptionFilter { get; set; } = "";
                public string AllowSaturdayAuction { get; set; } = "";
                public string SwapStockNumber { get; set; } = "";
                public string UsePartialInventory { get; set; } = "";
                public string UseLMIURL { get; set; } = "";
                public string SendURLasCR { get; set; } = "";
                public string InvFeedOnly { get; set; } = "";
                public string IsDealerAccount { get; set; } = "";

                // Text Values
                public string SellerID { get; set; } = "";
                public string OrganizationName { get; set; } = "";
                public string BuyerGroup { get; set; } = "";
                public string ServiceProviderID { get; set; } = "";
                public string ServiceProviderName { get; set; } = "";
                public string CarGroupID { get; set; } = "";
                public string MaxMMRPct { get; set; } = "";

                // DropDown Values
                public string ContactGroup { get; set; } = "";
                public string kWholesaleFacilitatedAuctionCode { get; set; } = "";
                public string MMRRegionCode { get; set; } = "";
                public string kWholesaleLocationCode { get; set; } = "";
                public string WholesaleBidIncrement { get; set; } = "";

                public string BuildMarketInfoList(string value, string label)
                {
                    Dictionary<string, string> list = new Dictionary<string, string>();

                    string[] strTmp;
                    string defValue = "";
                    string selectedItem = "";
                    string listVal = "";
                    string options = "";
                    int pos;

                    string returnString = $@"
                    <select name='{label}' id='{label}' class='inputStyle'>
                        <option name='blank' id='blank'>--</option>
                        OPTIONS
                    </select>";

                    if (!(value.StartsWith("[]")))
                    {
                        pos = value.IndexOf("]");
                        if ((pos != -1))
                        {
                            selectedItem = value.Substring(1, pos - 1);
                            listVal = value.Substring(pos + 1);
                        }
                        else
                            // no brackets
                            listVal = value;
                    }
                    else if (value.StartsWith("[]"))
                    {
                        pos = value.IndexOf("]");
                        if ((pos != -1))
                            listVal = value.Substring(pos + 1);
                    }

                    if (listVal.Length > 0 && listVal != "[]")
                    {
                        strTmp = listVal.TrimEnd('|').Split('|');
                        if (strTmp.Length > 0)
                        {
                            foreach (var strElem in strTmp)
                            {
                                pos = strElem.IndexOf(":");
                                if (pos != -1)
                                {
                                    string sID = strElem.Substring(0, pos);
                                    string sText = strElem.Substring(pos + 1);

                                    if (sText == selectedItem)
                                        defValue = sID;

                                    if (!list.ContainsKey(sText))
                                        list.Add(sText, sID);
                                }
                                else
                                    list.Add(strElem, strElem);
                            }
                        }
                    }
                    else
                        list.Add("", "");

                    foreach (KeyValuePair<string, string> item in list)
                    {
                        if (item.Value == defValue)
                            options += $"<option value='{item.Value}' selected>{item.Key}</option>";
                        else
                            options += $"<option value='{item.Value}'>{item.Key}</option>";
                    }

                    return returnString.Replace("OPTIONS", options);
                }
            }

            public static Info PopulateInfo(Dictionary<string, string> info)
            {
                Dictionary<int, string> auctionMap = new Dictionary<int, string>
                {
                    { 1, "ove" }, { 2, "sa" }, { 4, "adesa" }, { 6, "copart" },
                    { 7, "ae" }, { 12, "ed" }, { 11, "acv" }, { 13, "iaa" },
                    { 14, "as" }, { 15, "ias" }, { 16, "aos" }, { 17, "carmigo" },
                    { 18, "caroffer" }, { 19, "remarketingPlus" }
                };
                Info aInfo = new Info();

                string[] dropdownList =
                    { "ContactGroup", "kWholesaleFacilitatedAuctionCode", "MMRRegionCode", "kWholesaleLocationCode", "WholesaleBidIncrement" };
                string[] textValues =
                    { "SellerID", "OrganizationName" , "BuyerGroup", "ServiceProviderID", "ServiceProviderName", "CarGroupID", "MaxMMRPct" };

                foreach (KeyValuePair<string, string> kp in info)
                {
                    if (aInfo.GetType().GetProperty(kp.Key) != null)
                    {
                        if (dropdownList.Contains(kp.Key))
                        {
                            string label = kp.Key;
                            if (label.Equals("ContactGroup") || label.Equals("WholesaleBidIncrement"))
                                label = $"k{label}";

                            string prefix = auctionMap[int.Parse(info["kWholesaleAuction"].ToString())];
                            aInfo.GetType().GetProperty(kp.Key).SetValue(aInfo, aInfo.BuildMarketInfoList(kp.Value.ToString(), $"{prefix}{label}"), null);
                            continue;
                        }
                        else if (textValues.Contains(kp.Key))
                        {

                            string val = kp.Key == "MaxMMRPct" ? $"value='{Convert.ToDouble(kp.Value) * 100}'" : $"value='{kp.Value}'";
                            aInfo.GetType().GetProperty(kp.Key).SetValue(aInfo, val, null);
                        }
                        else
                        {
                            string chk = kp.Value == "1" ? "checked" : "";
                            aInfo.GetType().GetProperty(kp.Key).SetValue(aInfo, chk, null);
                        }
                    }
                }

                return aInfo;
            }
        }
    }
}