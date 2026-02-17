using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.Listing;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.resource.clients
{
    public class ListingClient : IListingClient
    {
        private ListingSoapClient _listingClient;
        private static readonly string client = "Listing";

        // Default Constructor
        public ListingClient() { }

        public ListingClient(ListingSoapClient client) => _listingClient = client;

        public ListingSoapClient GetClient()
        {
            if (_listingClient == null)
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 10, 0)
                };
                EndpointAddress epa = new EndpointAddress(Util.GetIniEntry(client));
                _listingClient = new ListingSoapClient(httpBinding, epa);

            }

            return _listingClient;
        }

        public lmReturnValue UpdateGet(string kSession, int kListing)
        {
            return GetClient().UpdateGet(kSession, kListing);
        }
        public lmReturnValue UpdateSet(string kSession, DataSet VehicleDataSet)
        {
            return GetClient().UpdateSet(kSession, VehicleDataSet);
        }
        public lmReturnValue WPUpdateSet(string kSession, string jsonData)
        {
            return GetClient().WPUpdateSet(kSession, jsonData);
        }
        public lmReturnValue ListingDetailGet(string kSession, int kDealer, int kListing, int Thin)
        {
            return GetClient().ListingDetailGet(kSession, kDealer, kListing, Thin);
        }
        public lmReturnValue ListingDetailSet(string kSession, int kListing, int InvListPrice, int InternetPrice, int kInventoryStatus, int FastQuote, string LotLocation, int RunPricing)
        {
            return GetClient().ListingDetailSet(kSession, kListing, InvListPrice, InternetPrice, kInventoryStatus, FastQuote, LotLocation, RunPricing);
        }
        public lmReturnValue ListingAuctionDataGet(string kSession, int kDealer, int kListing)
        {
            return GetClient().ListingAuctionDataGet(kSession, kDealer.ToString(), kListing.ToString());
        }
        public lmReturnValue ListingSingleAuctionDataGet(string kSession, int kWholesaleAuction, int kDealer, int kListing)
        {
            return GetClient().ListingSingleAuctionDataGet(kSession, kWholesaleAuction.ToString(), kDealer.ToString(), kListing.ToString());
        }
        public lmReturnValue SellUnsellVehicle(string kSession, int kListing, int kInventoryStatus)
        {
            return GetClient().SellUnsellVehicle(kSession, kListing.ToString(), kInventoryStatus.ToString());
        }
        public lmReturnValue DeleteVehicle(string kSession, int kDealer, string VIN)
        {
            return GetClient().DeleteVehicle(kSession, kDealer.ToString(), VIN);
        }
        public lmReturnValue PhotosToProcess(string kSession, int kListing)
        {
            return GetClient().PhotosToProcess(kSession, kListing);
        }
        public lmReturnValue EbayCategoryGet(string kSession, int kMake, int kModel, int StyleID, string CurrentCategory)
        {
            return GetClient().EbayCategoryGet(kSession, kMake, kModel, StyleID, CurrentCategory);
        }
        public lmReturnValue RandomTitleGet(string kSession, int kListing)
        {
            return GetClient().RandomTitleGet(kSession, kListing);
        }
        public lmReturnValue PhotoImport(string kSession, int kListing, string Source, int StartCount, int EndCount, int Padding, int Manual)
        {
            return GetClient().PhotoImport(kSession, kListing, Source, StartCount, EndCount, Padding, Manual);
        }
        public lmReturnValue GetListingPaths(string kSession, int kListing)
        {
            return GetClient().GetListingPaths(kSession, kListing);
        }
        public lmReturnValue GenerateTemplate(string kSession, int kListing)
        {
            return GetClient().GenerateTemplate(kSession, kListing);
        }
        public lmReturnValue AddPhotographs(string kSession, int kListing, string SaveLocation)
        {
            return GetClient().AddPhotographs(kSession, kListing, SaveLocation);
        }
        public lmReturnValue ListingLotLocationListGet(string kSession, int kListing)
        {
            return GetClient().ListingLotLocationListGet(kSession, kListing);
        }
        public lmReturnValue ListingVehicleNotesGet(string kSession, int kListing)
        {
            return GetClient().ListingVehicleNotesGet(kSession, kListing);
        }
        public lmReturnValue ListingVehicleNotesSet(string kSession, int kListing, int VehicleNoteID, string NoteString)
        {
            return GetClient().ListingVehicleNotesSet(kSession, kListing, VehicleNoteID, NoteString);
        }
        public lmReturnValue ListingVehicleGetPhotos(string kSession, int kListing)
        {
            return GetClient().ListingVehicleGetPhotos(kSession, kListing);
        }
        public lmReturnValue AddPhotographsExplicit(string kSession, int kListing, string uploadPath, string fileList, string extList, int photoType = 0)
        {
            return GetClient().AddPhotographsExplicit(kSession, kListing, uploadPath, fileList, extList, photoType);
        }
        public lmReturnValue AssignDamagePhoto(string kSession, string kListing, int OperationCode, int kPhoto, int kWholesaleListingDamage)
        {
            return GetClient().AssignDamagePhoto(kSession, kListing, OperationCode, kPhoto, kWholesaleListingDamage);
        }
        public lmReturnValue AddMakeModel(string kSession, int kMake, string Make, string Model)
        {
            return GetClient().AddMakeModel(kSession, kMake, Make, Model);
        }
        public lmReturnValue VehicleClassSet(string kSession, string kListing, string kVehicleClass)
        {
            return GetClient().VehicleClassSet(kSession, kListing, kVehicleClass);
        }
        public lmReturnValue ListingPhotosGet(string kSession, int kListing)
        {
            return GetClient().ListingGetPhotos(kSession, kListing);
        }
        public lmReturnValue MarkVehicleUnavailable(string kSession, int kListing)
        {
            return GetClient().MarkVehicleUnavailable(kSession, kListing.ToString());
        }
        public lmReturnValue ReorderPhotos(string kSession, string kListing, string PhotoOrder)
        {
            return GetClient().ReorderPhotos(kSession, kListing, PhotoOrder);
        }
    }
}