using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.Dealer;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.resource.clients
{
    public class DealerClient : IDealerClient
    {
        private DealerSoapClient _dealerClient;
        private static readonly string client = "Dealer";

        // Default Constructor
        public DealerClient() { }

        public DealerClient(DealerSoapClient client) => _dealerClient = client;

        public DealerSoapClient GetClient()
        {
            if (_dealerClient == null)
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 10, 0)
                };
                EndpointAddress epa = new EndpointAddress(Util.GetIniEntry(client));
                _dealerClient = new DealerSoapClient(httpBinding, epa);
            }

            return _dealerClient;
        }
        public lmReturnValue GetDealerInfo(string kSession, int kDealer, string SingleTable, string MultiTableName)
        {
            return GetClient().GetDealerInfo(kSession, kDealer, SingleTable, MultiTableName);
        }
        public lmReturnValue LotLocationListGet(string kSession, int kDealer)
        {
            return GetClient().LotLocationListGet(kSession, kDealer);
        }
        public lmReturnValue ImportConfigGet(string kSession, int kDealer)
        {
            return GetClient().ImportConfigGet(kSession, kDealer);

        }
        public lmReturnValue ImportSystemGet(string kSession, string InventoryVehicleAccount)
        {
            return GetClient().ImportSystemGet(kSession, InventoryVehicleAccount);

        }
        public lmReturnValue GetDealerPaths(string kSession, int kDealer)
        {
            return GetClient().GetDealerPaths(kSession, kDealer);

        }
        public lmReturnValue DealerBaseSet(string kSession, int kDealer, string jsonData)
        {
            return GetClient().DealerBaseSet(kSession, kDealer.ToString(), jsonData);
        }
        public lmReturnValue GetUserWidgetPreferences(string kSession, int kDealer, int kWidget)
        {
            return GetClient().GetUserWidgetPreferences(kSession, kDealer, kWidget);
        }
        public lmReturnValue GetDashInventoryHealth(string kSession, int kDealer)
        {
            return GetClient().GetDashInventoryHealth(kSession, kDealer);
        }
        public lmReturnValue GetImportHistoryData(string kSession, int kDealer, int Days)
        {
            return GetClient().GetImportHistoryData(kSession, kDealer, Days);
        }
        public lmReturnValue GetSignupPending(string kSession)
        {
            return GetClient().GetSignupPending(kSession);
        }
        public lmReturnValue GetSignupAuctions(string kSession)
        {
            return GetClient().GetSignupAuctions(kSession);
        }
        public lmReturnValue GetPendingSetup(string kSession, int kWholesaleSignup)
        {
            return GetClient().GetPendingSetup(kSession, kWholesaleSignup);
        }
        public lmReturnValue SetSignup(string kSession, string jsonData)
        {
            return GetClient().SetSignup(kSession, jsonData);
        }

        public lmReturnValue DealerNameCheck(string kSession, string DealerName)
        {
            return GetClient().DealerNameCheck(kSession, DealerName);
        }
    }
}