using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using LMWholesale.OldDAS;

namespace LMWholesale.resource.clients.interfaces
{
    public interface IOldDASClient
    {
        [OperationContract]
        lmReturnValue DealerImport(string SessionID, int Operation, string Dealer, string VehicleInvAcc, string DealerFilePath, string FilePath, string Delimiter, int kDealerImport, int ImportType);
    }
}
