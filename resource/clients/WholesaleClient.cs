using System;
using System.Collections.Generic;
using System.ServiceModel;

using LMWholesale.resource.clients.interfaces;
using LMWholesale.resource.model.Wholesale;
using LMWholesale.Wholesale;

namespace LMWholesale.resource.clients
{
    public class WholesaleClient : IWholesaleClient
    {
        private WholesaleSoapClient _wholesaleClient;
        private static readonly string client = "Wholesale";

        // Default Constructor
        public WholesaleClient() { }

        public WholesaleClient(WholesaleSoapClient client) => _wholesaleClient = client;

        public WholesaleSoapClient GetClient()
        {
            if (_wholesaleClient == null)
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 10, 0),
                    SendTimeout = new TimeSpan(0, 5, 0)
                };

                EndpointAddress epa = new EndpointAddress(Util.GetIniEntry(client));
                _wholesaleClient = new WholesaleSoapClient(httpBinding, epa);

            }

            return _wholesaleClient;
        }

        public lmReturnValue GetWholesaleWP(InventoryFilter.Filter filter, InventoryFilter.AdvancedFilter advancedFilter)
        {
            // Flatten each dictionary to a simple json
            Dictionary<string, string> jsonString = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kp in filter.Flatten())
                jsonString.Add(kp.Key, kp.Value);
            foreach (KeyValuePair<string, string> kp in advancedFilter.Flatten())
                jsonString.Add(kp.Key, kp.Value);

            return GetClient().GetWholesaleWP(Util.serializer.Serialize(jsonString));
        }
        public lmReturnValue SubmitToMultipleAuctions(string kSession, int kDealer, int kListing, System.Data.DataSet AuctionDataSet)
        {
            return GetClient().SubmitToMultipleAuctions(kSession, kDealer, kListing, AuctionDataSet);
        }
        public lmReturnValue SubmitMultiListingToAuction(string kSession, int kDealer, System.Data.DataSet AuctionDataSet)
        {
            return GetClient().SubmitMultiListingToAuction(kSession, kDealer, AuctionDataSet);
        }
        public lmReturnValue WholesaleAutoLaunchTextGet(string kSession, int kDealer)
        {
            return GetClient().WholesaleAutoLaunchTextGet(kSession, kDealer);
        }
        public lmReturnValue WholesaleAutoLaunchGetData(string kSession, int kDealer, bool isSimple)
        {
            return GetClient().WholesaleAutoLaunchGetData(kSession, kDealer.ToString(), isSimple);
        }
        public lmReturnValue WholesaleAutoLaunchSingleGet(string kSession, int kDealer, string kWholesaleAuction)
        {
            return new lmReturnValue();
            //return GetClient().WholesaleAutoLaunchSingleGet(kSession, kDealer.ToString(), kWholesaleAutoLaunch);
        }
        public lmReturnValue WholesaleAutoLaunchSetData(string kSession, int kDealer, string jsonString)
        {
            return GetClient().WholesaleAutoLaunchSetData(kSession, kDealer, jsonString);
        }
        public lmReturnValue WholesaleBlackoutTextGet(string kSession, int kDealer)
        {
            return GetClient().WholesaleBlackoutTextGet(kSession, kDealer);
        }
        public lmReturnValue WholesaleBlackoutGetData(string kSession, string kDealer)
        {
            return GetClient().WholesaleBlackoutGetData(kSession, kDealer);
        }
        public lmReturnValue WholesaleBlackoutSetData(string kSession, int kDealer, string jsonData)
        {
            return GetClient().WholesaleBlackoutSetData(kSession, kDealer, jsonData);
        }
        public lmReturnValue InspectionCompanyGet(string kSession)
        {
            return GetClient().InspectionCompanyGet(kSession);
        }
        public lmReturnValue InspectionCompanySet(string kSession, int kDealer, int kWholesaleInspectionCompany, string Username, string UserPassword, string ContactName, string ContactEmail, string ContactPhone)
        {
            return GetClient().InspectionCompanySet(kSession, kDealer, kWholesaleInspectionCompany, Username, UserPassword, ContactName, ContactEmail, ContactPhone);
        }
        public lmReturnValue WholesaleInspectionRequest(string kSession, int kDealer, int kWholesaleInspectionCompany, int kListing)
        {
            return GetClient().WholesaleInspectionRequest(kSession, kDealer, kWholesaleInspectionCompany, kListing);
        }
        public lmReturnValue WholesaleAuctionListDealerGet(string kSession, int kDealer)
        {
            return GetClient().WholesaleAuctionListDealerGet(kSession, kDealer);
        }
        public lmReturnValue WholesaleAuctionByDealerGet(string kSession, int kDealer, int kWholesaleAuction)
        {
            return GetClient().WholesaleAuctionByDealerGet(kSession, kDealer, kWholesaleAuction);
        }
        public lmReturnValue WholesaleAuctionByDealerSet(string kSession, string jsonData)
        {
            return GetClient().WholesaleAuctionByDealerSet(kSession, jsonData);
        }
        public lmReturnValue WholesaleAuctionByDealerAutoLaunchSet(string kSession, int kDealer, int kWholesaleAuction, int enabled)
        {
            return GetClient().WholesaleAuctionByDealerAutoLaunchSet(kSession, kDealer, kWholesaleAuction, enabled);
        }
        public lmReturnValue WholesaleMultiListingsGet(InventoryFilter.MultiPageFilter multiPageFilter)
        {
            return GetClient().WholesaleMultiListingsGet(Util.serializer.Serialize(multiPageFilter.Flatten()));
        }
        public lmReturnValue WholesaleMultiListingsSet(string kSession, int kDealer, int kListing, int kWholesaleAuction, DateTime StartDate, DateTime EndDate, int kListingType, int kWholesaleListingCategory, string sStartPrice, string sFloorPrice, string sBuyNowPrice)
        {
            return GetClient().WholesaleMultiListingsSet(kSession, kDealer, kListing, kWholesaleAuction, StartDate, EndDate, kListingType, kWholesaleListingCategory, sStartPrice, sFloorPrice, sBuyNowPrice);
        }
        public lmReturnValue WholesaleUserFilterGet(string kSession, int kDealer, int kPerson)
        {
            return GetClient().WholesaleUserFilterGet(kSession, kDealer, kPerson);
        }
        public lmReturnValue WholesaleUserFilterSet(string jsonString)
        {
            return GetClient().WholesaleUserFilterSet(jsonString);
        }
        public lmReturnValue InspectionDataGet(string kSession, int kDealer, string kListing)
        {
            return GetClient().InspectionDataGet(kSession, kDealer, kListing);
        }
        public lmReturnValue WPWholesaleAuctionPaintSet(string kSession, int kDealer, string kListing, string jsonString)
        {
            return GetClient().WPWholesaleAuctionPaintSet(kSession, kDealer, kListing, jsonString);
        }
        public lmReturnValue WPWholesaleAuctionDamageSet(string kSession, int kDealer, string kListing, string jsonString)
        {
            return GetClient().WPWholesaleAuctionDamageSet(jsonString, kSession, kDealer, kListing);
        }
        public lmReturnValue InspectionDataSet(string jsonString, string kSession, int kDealer, string kListing)
        {
            return GetClient().InspectionDataSet(jsonString, kSession, kDealer, kListing);
        }
        public lmReturnValue GetInspectionMappings(string kSession)
        {
            return GetClient().GetInspectionMappings(kSession);
        }
        public lmReturnValue GetDropdownValues(string kSession)
        {
            return GetClient().GetDropdownValues(kSession);
        }
        public lmReturnValue SalesDataApprovalSet(string JsonData)
        {
            return GetClient().SalesDataApprovalSet(JsonData);
        }
        public lmReturnValue SalesDataApprovalGet(string JsonData)
        {
            return GetClient().SalesDataApprovalGet(JsonData);
        }
        public lmReturnValue SalesDataApprovalMark(string JsonData)
        {
            return GetClient().SalesDataApprovalMark(JsonData);
        }
        public lmReturnValue SetWholesaleAuctionCredential(string kSession, int kDealer, string jsonData)
        {
            return GetClient().SetWholesaleAuctionCredential(kSession, kDealer, jsonData);
        }
        public lmReturnValue WholesaleCreditRequestGet(string kSession, int kDealer)
        {
            return GetClient().WholesaleCreditRequestGet(kSession, kDealer);
        }
        public lmReturnValue WholesaleCreditRequestSet(string kSession, int kDealer, Dictionary<string, object> info)
        {
            return GetClient().WholesaleCreditRequestSet(kSession, kDealer, int.Parse(info["kWholesaleAuction"].ToString()), info["VIN"].ToString(), info["Reason"].ToString());
        }
        public lmReturnValue WholesaleAutoLaunchRuleTest(string kSession, string json)
        {
            return GetClient().WholesaleAutoLaunchRuleTest(kSession, json);
        }
        public lmReturnValue WholesaleDealerAutoLaunchRuleSetGet(string kSession, int kDealer, int kDealerGaggle, int kGaggleSubGroup)
        {
            return GetClient().WholesaleDealerAutoLaunchRuleSetGet(kSession, kDealer.ToString(), kDealerGaggle.ToString(), kGaggleSubGroup.ToString());
        }
        public lmReturnValue WholesaleDealerAutoLaunchRuleSetSet(string kSession, int kDealer, string op, Dictionary<string, object> json)
        {
            json.Add("kDealer", kDealer.ToString());
            json.Add("operation", op);

            if (json.ContainsKey("isADESA"))
            {
                json.Remove("isADESA");
                json.Add("isOpenLane", 1);
            }
            string jsonString = Util.serializer.Serialize(json);
            return GetClient().WholesaleDealerAutoLaunchRuleSetSet(kSession, jsonString);
        }
    }
}