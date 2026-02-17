using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web;

using LMWholesale.resource.clients.interfaces;
using LMWholesale.OldDAS;

namespace LMWholesale.resource.clients
{
    public class OldDASClient : IOldDASClient
    {
        private OldDAS.DASSoapClient _wholesaleClient;
        private static readonly string client = "OldDAS";

        // Default Constructor
        public OldDASClient() { }

        public OldDASClient(OldDAS.DASSoapClient client) => _wholesaleClient = client;

        public OldDAS.DASSoapClient GetClient()
        {
            if (_wholesaleClient == null)
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 10, 0)
                };

                EndpointAddress epa = new EndpointAddress(Util.GetIniEntry(client));
                _wholesaleClient = new OldDAS.DASSoapClient(httpBinding, epa);

            }

            return _wholesaleClient;
        }

        public lmReturnValue DealerImport(string SessionID, int Operation, string Dealer, string VehicleInvAcc, string DealerFilePath, string FilePath, string Delimiter, int kDealerImport, int ImportType)
        {
            return GetClient().DealerImport(SessionID, Operation, Dealer, VehicleInvAcc, DealerFilePath, FilePath, Delimiter, kDealerImport, ImportType);
        }
    }
}