using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.DAS;

namespace LMWholesale.resource.clients.interfaces
{
    [ServiceContract]
    public interface IDASClient
    {
        [OperationContract]
        lmReturnValue ImportStatusGet(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue ImportStatusSet(string kSession, string JsonData);
        [OperationContract]
        lmReturnValue DASCreateInventory(string Session,
                    string kDealer,
                    string StockNumber,
                    string VIN,
                    string MotorYear,
                    string Miles,
                    string InventoryCost,
                    string Make,
                    string Model,
                    string Style,
                    string StyleID,
                    string InventoryListPrice,
                    string Drilldown,
                    string StockType,
                    string Status,
                    string ImportOverride,
                    string InternetPrice);
        [OperationContract]
        lmReturnValue DASDeleteInventory(string kSession, int kListing);
        [OperationContract]
        lmReturnValue DASGetVinExplosion(string kSession, string VIN);
        [OperationContract]
        lmReturnValue DASGetMotorYears(string kSession, string Source);
        [OperationContract]
        lmReturnValue DASGetMotorMakes(string kSession, string Source, string Year);
        [OperationContract]
        lmReturnValue DASGetMotorModels(string kSession, string Source, string Year, string Make);
        [OperationContract]
        lmReturnValue DASGetMotorStyles(string kSession, string Source, string Year, string Make, string Model);
        [OperationContract]
        lmReturnValue DASResolveBlackbook(string kSession, int kIdentity, int kBlackbook, string Year, string Make, string Model, string Style, string Series, int Mode);
        [OperationContract]
        lmReturnValue DASResolveBluebook(string kSession, int kIdentity, int kVehicleID, int Mode);
        [OperationContract]
        lmReturnValue DASResolveChromeYMMS(string kSession, string VIN);
        [OperationContract]
        lmReturnValue DASUpdateInventory(string kSession, string data);
        [OperationContract]
        lmReturnValue GetAnalyticsReRun(string kSession, string Operation, string Listing);
        [OperationContract]
        lmReturnValue GetHealthReportMin(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue GetOfferReport(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue GetHealthReportSummary(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue GetHealthReportDetail(string kSession, int kDealer, DateTime FirstDateMonth);
        [OperationContract]
        lmReturnValue GetAACurrentlyPosted(string kSession, int kGaggleSubGroup, int kDealer);
        [OperationContract]
        lmReturnValue GetDealerActiveWholesale(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue CurrentResultsWidgetGet(string kSession, int kDealer);
    }
}