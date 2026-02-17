using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.Inventory;

namespace LMWholesale.resource.clients.interfaces
{
    [ServiceContract]
    public interface IInventoryClient
    {
        [OperationContract]
        lmReturnValue GetInventoryOverrides(string Session, int kListing);
        [OperationContract]
        lmReturnValue SetInventoryOverrides(string Session, string JsonData);
    }
}