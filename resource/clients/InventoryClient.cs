using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.Inventory;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.resource.clients
{
    public class InventoryClient : IInventoryClient
    {
        private InventorySoapClient _inventoryClient;
        private static readonly string client = "Inventory";

        // Default Constructor
        public InventoryClient() { }

        public InventoryClient(InventorySoapClient client) => _inventoryClient = client;

        public InventorySoapClient GetClient()
        {
            if (_inventoryClient == null)
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 10, 0)
                };
                EndpointAddress epa = new EndpointAddress(Util.GetIniEntry(client));
                _inventoryClient = new InventorySoapClient(httpBinding, epa);

            }

            return _inventoryClient;
        }

        public lmReturnValue GetInventoryOverrides(string kSession, int kListing)
        {
            return GetClient().GetInventoryOverrides(kSession, kListing);
        }
        public lmReturnValue SetInventoryOverrides(string kSession, string jsonData)
        {
            return GetClient().SetInventoryOverrides(kSession, jsonData);
        }
    }
}