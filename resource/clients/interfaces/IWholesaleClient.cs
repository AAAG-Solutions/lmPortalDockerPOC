using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using LMWholesale.resource.model.Wholesale;
using LMWholesale.Wholesale;

namespace LMWholesale.resource.clients.interfaces
{
    [ServiceContract]
    public interface IWholesaleClient
    {
        [OperationContract]
        lmReturnValue GetWholesaleWP(InventoryFilter.Filter filter, InventoryFilter.AdvancedFilter advancedFilter);
        [OperationContract]
        lmReturnValue SubmitToMultipleAuctions(string kSession, int kDealer, int kListing, System.Data.DataSet AuctionDataSet);
        [OperationContract]
        lmReturnValue SubmitMultiListingToAuction(string kSession, int kDealer, System.Data.DataSet AuctionDataSet);
        [OperationContract]
        lmReturnValue WholesaleAutoLaunchTextGet(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue WholesaleAutoLaunchGetData(string kSession, int kDealer, bool isSimple);
        [OperationContract]
        lmReturnValue WholesaleAutoLaunchSingleGet(string kSession, int kDealer, string kWholesaleAutoLaunch);
        [OperationContract]
        lmReturnValue WholesaleAutoLaunchSetData(string kSession, int kDealer, string jsonString);
        [OperationContract]
        lmReturnValue WholesaleBlackoutTextGet(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue WholesaleBlackoutGetData(string kSession, string kDealer);
        [OperationContract]
        lmReturnValue WholesaleBlackoutSetData(string kSession, int kDealer, string jsonData);
        [OperationContract]
        lmReturnValue InspectionCompanyGet(string kSession);
        [OperationContract]
        lmReturnValue InspectionCompanySet(string kSession, int kDealer, int kWholesaleInspectionCompany, string Username, string UserPassword, string ContactName, string ContactEmail, string ContactPhone);
        [OperationContract]
        lmReturnValue WholesaleInspectionRequest(string kSession, int kDealer, int kWholesaleInspectionCompany, int kListing);
        [OperationContract]
        lmReturnValue WholesaleAuctionListDealerGet(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue WholesaleAuctionByDealerGet(string kSession, int kDealer, int kWholesaleAuction);
        [OperationContract]
        lmReturnValue WholesaleAuctionByDealerSet(string kSession, string jsonData);
        [OperationContract]
        lmReturnValue WholesaleAuctionByDealerAutoLaunchSet(string kSession, int kDealer, int kWholesaleAuction, int enabled);
        [OperationContract]
        lmReturnValue WholesaleMultiListingsGet(InventoryFilter.MultiPageFilter multiPageFilter);
        [OperationContract]
        lmReturnValue WholesaleMultiListingsSet(string kSession, int kDealer, int kListing, int kWholesaleAuction, DateTime StartDate, DateTime EndDate, int kListingType, int kWholesaleListingCategory, string sStartPrice, string sFloorPrice, string sBuyNowPrice);
        [OperationContract]
        lmReturnValue WholesaleUserFilterGet(string kSession, int kDealer, int kPerson);
        [OperationContract]
        lmReturnValue WholesaleUserFilterSet(string jsonString);
        [OperationContract]
        lmReturnValue InspectionDataGet(string kSession, int kDealer, string kListing);
        [OperationContract]
        lmReturnValue WPWholesaleAuctionPaintSet(string kSession, int kDealer, string kListing, string jsonString);
        [OperationContract]
        lmReturnValue WPWholesaleAuctionDamageSet(string kSession, int kDealer, string kListing, string jsonString);
        [OperationContract]
        lmReturnValue InspectionDataSet(string jsonString, string kSession, int kDealer, string kListing);
        [OperationContract]
        lmReturnValue GetInspectionMappings(string kSession);
        [OperationContract]
        lmReturnValue GetDropdownValues(string kSession);
        [OperationContract]
        lmReturnValue SalesDataApprovalSet(string JsonData);
        [OperationContract]
        lmReturnValue SalesDataApprovalGet(string JsonData);
        [OperationContract]
        lmReturnValue SalesDataApprovalMark(string JsonData);
        [OperationContract]
        lmReturnValue SetWholesaleAuctionCredential(string kSession, int kDealer, string jsonData);
        [OperationContract]
        lmReturnValue WholesaleCreditRequestGet(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue WholesaleCreditRequestSet(string kSession, int kDealer, Dictionary<string, object> data);
        [OperationContract]
        lmReturnValue WholesaleAutoLaunchRuleTest(string kSession, string json);
        [OperationContract]
        lmReturnValue WholesaleDealerAutoLaunchRuleSetGet(string kSession, int kDealer, int kDealerGaggle, int kGaggleSubGroup);
        [OperationContract]
        lmReturnValue WholesaleDealerAutoLaunchRuleSetSet(string kSession, int kDealer, string op, Dictionary<string, object> json);
    }
}
