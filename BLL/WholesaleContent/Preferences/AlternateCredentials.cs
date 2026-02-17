using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Instrumentation;
using System.Web;

using LMWholesale.resource.clients;
using LMWholesale.resource.factory;

namespace LMWholesale.BLL.WholesaleContent.Preferences
{
    public class AlternateCredentials
    {
        private readonly LookupClient lookupClient;
        private readonly AuctionFactory auctionFactory;

        public AlternateCredentials()
        {
            lookupClient = lookupClient ?? new LookupClient();
            auctionFactory = auctionFactory ?? new AuctionFactory();
        }

        public AlternateCredentials(LookupClient lookupClient, AuctionFactory auctionFactory)
        {
            this.lookupClient = lookupClient;
            this.auctionFactory = auctionFactory;
        }

        internal static readonly AlternateCredentials instance = new AlternateCredentials();
        public AlternateCredentials Self
        {
            get { return instance; }
        }

        public DataTable AlternateCredentialsGet(string kSession, int kDealer)
        {
            DataTable credentialsOVE = Self.auctionFactory.GetAuctionService(1).GetCredentials(kSession, kDealer);
            DataTable credentialsAuctionEdge = Self.auctionFactory.GetAuctionService(7).GetCredentials(kSession, kDealer);

            // Consolidate both credential rows into a single table
            // Use OVE DataTable since we are getting it first. No Rhyme or reason
            foreach (DataRow dr in credentialsAuctionEdge.Rows)
                credentialsOVE.ImportRow(dr);

            credentialsOVE.DefaultView.Sort = "InvLotLocation asc";

            return credentialsOVE.DefaultView.ToTable();
        }
    }
}