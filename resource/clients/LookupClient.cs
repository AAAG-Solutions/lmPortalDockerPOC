using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.Lookup;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.resource.clients
{
    public class LookupClient : ILookupClient
    {
        private LookupSoapClient _lookupClient;
        private static readonly string client = "Lookup";

        // Default Constructor
        public LookupClient() { }

        public LookupClient(LookupSoapClient client) => _lookupClient = client;

        public LookupSoapClient GetClient()
        {
            if (_lookupClient == null)
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 10, 0)
                };
                EndpointAddress epa = new EndpointAddress(Util.GetIniEntry(client));
                _lookupClient = new LookupSoapClient(httpBinding, epa);
            }

            return _lookupClient;
        }

        public lmReturnValue GetDealerList(string kSession)
        {
            return GetClient().GetDealerList(kSession);
        }
        public lmReturnValue GetListingOptionList(string kSession, int kListing)
        {
            return GetClient().GetListingOptionList(kSession, kListing);
        }
        public lmReturnValue GetMakeList(string kSession)
        {
            return GetClient().GetMakeList(kSession);
        }
        public lmReturnValue GetModelList(string kSession, int kMake)
        {
            return GetClient().GetModelList(kSession, kMake);
        }
        public lmReturnValue InspectionCompanyListGet(string kSession, int kDealer, int kListing)
        {
            return GetClient().InspectionCompanyListGet(kSession, kDealer, kListing);
        }
        public lmReturnValue GetListingsByVIN(string kSession, string VIN)
        {
            return GetClient().GetListingsByVIN(kSession, VIN);
        }
        public lmReturnValue GetMultiAuctionCredentialsByDealer(string kSession, string kDealer)
        {
            return GetClient().GetMultiAuctionCredentialsByDealer(kSession, kDealer);
        }
        public lmReturnValue GetAuctionCredentialsByDealerByAuction(string kSession, string kDealer, string kWholesaleAuction)
        {
            return GetClient().GetAuctionCredentialsByDealerByAuction(kSession, kDealer, kWholesaleAuction);
        }
        public lmReturnValue GetWholesaleAuctionFuelType(string kSession)
        {
            return GetClient().GetWholesaleAuctionFuelType(kSession);
        }
        public lmReturnValue WholesaleUserDefaultUserGet(string kSession, int kDealer)
        {
            return GetClient().WholesaleUserDefaultUserGet(kSession, kDealer);
        }
        public lmReturnValue GridDescriptionGet(string kSession, int kDealer, int kPerson, string GridDescription)
        {
            return GetClient().GridDescriptionGet(kSession, kDealer, kPerson, GridDescription);
        }
        public lmReturnValue UserListGet(string kSession, int kExceptDealer)
        {
            return GetClient().UserListGet(kSession, kExceptDealer);
        }
        public lmReturnValue PhotoTagListGet(string kSession)
        {
            return GetClient().PhotoTagListGet(kSession);
        }
        public lmReturnValue CertificationListGet(string kSession)
        {
            return GetClient().CertificationListGet(kSession);
        }
    }
}