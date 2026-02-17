using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleData
{
    public class UploadPhotos
    {
        private readonly ListingClient listingClient;
        private readonly DealerClient dealerClient;

        // Default Constructor
        public UploadPhotos()
        {
            listingClient = listingClient ?? new ListingClient();
            dealerClient = dealerClient ?? new DealerClient();
        }

        public UploadPhotos(ListingClient listingClient, DealerClient dealerClient)
        {
            this.listingClient = listingClient;
            this.dealerClient = dealerClient;
        }

        internal static readonly UploadPhotos instance = new UploadPhotos();
        public UploadPhotos Self
        {
            get { return instance; }
        }

        public Listing.lmReturnValue ExplicitAddPhotos(string kSession, int kListing, string uploadPath, string photoList, string extList, int photoType = 0)
        {
            return Self.listingClient.AddPhotographsExplicit(kSession, kListing, uploadPath, photoList, extList, photoType);
        }

        public DataRow GetListingPaths(string kSession, string kListing)
        {
            Listing.lmReturnValue photoPaths = Self.listingClient.GetListingPaths(kSession, int.Parse(kListing));
            if (photoPaths.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                return photoPaths.Data.Tables[0].Rows[0];
            }

            // Return default empty row
            return new DataTable().NewRow();
        }

        public Dictionary<string, DataRow> GetDealerRelatedInfo(string kSession, int kDealer)
        {
            Dictionary<string, DataRow> returnRows = new Dictionary<string, DataRow>();

            Dealer.lmReturnValue returnValue = Self.dealerClient.GetDealerInfo(kSession, kDealer, null, "DealerGeneral,DealerImageInfo");
            if (returnValue.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                DataSet dsPrefs = returnValue.Data;
                returnRows.Add("DealerGeneral", dsPrefs.Tables["DealerGeneral"].Rows[0]);
                returnRows.Add("DealerImageInfo", dsPrefs.Tables["DealerImageInfo"].Rows[0]);
            }

            return returnRows;
        }
    }
}