using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.Dealer;

namespace LMWholesale.resource.clients.interfaces
{
    [ServiceContract]
    public interface IDealerClient
    {
        [OperationContract]
        lmReturnValue GetDealerInfo(string kSession, int kDealer, string SingleTable, string MultiTableName);
        [OperationContract]
        lmReturnValue LotLocationListGet(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue ImportConfigGet(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue ImportSystemGet(string kSession, string InventoryVehicleAccount);
        [OperationContract]
        lmReturnValue GetDealerPaths(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue DealerBaseSet(string kSession, int kDealer, string jsonData);
        [OperationContract]
        lmReturnValue GetUserWidgetPreferences(string kSession, int kDealer, int kWidget);
        [OperationContract]
        lmReturnValue GetDashInventoryHealth(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue GetImportHistoryData(string kSession, int kDealer, int days);
        [OperationContract]
        lmReturnValue GetSignupPending(string kSession);
        [OperationContract]
        lmReturnValue GetSignupAuctions(string kSession);
        [OperationContract]
        lmReturnValue GetPendingSetup(string kSession, int kWholesaleSignup);
        [OperationContract]
        lmReturnValue SetSignup(string kSession, string jsonData);
        [OperationContract]
        lmReturnValue DealerNameCheck(string kSession, string DealerName);
    }
}