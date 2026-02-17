using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class Delete
    {
        private readonly DASClient dasClient;
        private readonly WholesaleClient wholesaleClient;
        private readonly ListingClient listingClient;

        public Delete()
        {
            dasClient = dasClient ?? new DASClient();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            listingClient = listingClient ?? new ListingClient();
        }

        public Delete(DASClient dasClient, WholesaleClient wholesaleClient)
        {
            this.dasClient = dasClient;
            this.wholesaleClient = wholesaleClient;
        }
        internal static readonly Delete instance = new Delete();
        public Delete Self
        {
            get { return instance; }
        }

        public Listing.lmReturnValue ListingDetailGet(string kSession, int kDealer, int kListing)
        {
            return Self.listingClient.ListingDetailGet(kSession, kDealer, kListing, 1);
        }

        public bool DeleteInventory(string kSession, int kListing)
        {
            DAS.lmReturnValue rv = Self.dasClient.DASDeleteInventory(kSession, kListing);
            if (rv.Result == DAS.ReturnCode.LM_SUCCESS)
                return true;
            return false;
        }
    }
}