using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using LMWholesale.Lookup;

namespace LMWholesale.resource.clients.interfaces
{
    [ServiceContract]
    public interface ILookupClient
    {
        [OperationContract]
        lmReturnValue GetDealerList(string kSession);
        [OperationContract]
        lmReturnValue GetListingOptionList(string kSession, int kListing);
        [OperationContract]
        lmReturnValue GetMakeList(string kSession);
        [OperationContract]
        lmReturnValue GetModelList(string kSession, int kMake);
        [OperationContract]
        lmReturnValue InspectionCompanyListGet(string kSession, int kDealer, int kListing);
        [OperationContract]
        lmReturnValue GetListingsByVIN(string kSession, string VIN);
        [OperationContract]
        lmReturnValue GetMultiAuctionCredentialsByDealer(string kSession, string kDealer);
        [OperationContract]
        lmReturnValue GetAuctionCredentialsByDealerByAuction(string kSession, string kDealer, string kWholesaleAuction);
        [OperationContract]
        lmReturnValue GetWholesaleAuctionFuelType(string kSession);
        [OperationContract]
        lmReturnValue WholesaleUserDefaultUserGet(string kSession, int kDealer);
        [OperationContract]
        lmReturnValue GridDescriptionGet(string kSession, int kDealer, int kPerson, string GridDescription);
        [OperationContract]
        lmReturnValue UserListGet(string kSession, int kExceptDealer);
        [OperationContract]
        lmReturnValue PhotoTagListGet(string kSession);
        [OperationContract]
        lmReturnValue CertificationListGet(string kSession);
    }
}
