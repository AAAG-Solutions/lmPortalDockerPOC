using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using LMWholesale.Listing;

namespace LMWholesale.resource.clients.interfaces
{
    [ServiceContract]
    public interface IListingClient
    {
        [OperationContract]
        lmReturnValue UpdateGet(string kSession, int kListing);
        [OperationContract]
        lmReturnValue UpdateSet(string Session, System.Data.DataSet vehicle);
        [OperationContract]
        lmReturnValue ListingDetailGet(string kSession, int kDealer, int kListing, int Thin);
        [OperationContract]
        lmReturnValue ListingDetailSet(string kSession, int kListing, int InvListPrice, int InternetPrice, int kInventoryStatus, int FastQuote, string LotLocation, int RunPricing);
        [OperationContract]
        lmReturnValue ListingAuctionDataGet(string kSession, int kDealer, int kListing);
        [OperationContract]
        lmReturnValue ListingSingleAuctionDataGet(string kSession, int kWholesaleAuction, int kDealer, int kListing);
        [OperationContract]
        lmReturnValue SellUnsellVehicle(string kSession, int kListing, int kInventoryStatus);
        [OperationContract]
        lmReturnValue DeleteVehicle(string kSession, int kDealer, string VIN);
        [OperationContract]
        lmReturnValue PhotosToProcess(string kSession, int kListing);
        [OperationContract]
        lmReturnValue EbayCategoryGet(string kSession, int kMake, int kModel, int StyleID, string CurrentCategory);
        [OperationContract]
        lmReturnValue RandomTitleGet(string kSession, int kListing);
        [OperationContract]
        lmReturnValue PhotoImport(string kSession, int kListing, string Source, int StartCount, int EndCount, int Padding, int Manual);
        [OperationContract]
        lmReturnValue GetListingPaths(string kSession, int kListing);
        [OperationContract]
        lmReturnValue GenerateTemplate(string kSession, int kListing);
        [OperationContract]
        lmReturnValue AddPhotographs(string kSession, int kListing, string SaveLocation);
        [OperationContract]
        lmReturnValue ListingLotLocationListGet(string kSession, int kListing);
        [OperationContract]
        lmReturnValue ListingVehicleNotesGet(string kSession, int kListing);
        [OperationContract]
        lmReturnValue ListingVehicleNotesSet(string kSession, int kListing, int VehicleNoteID, string NoteString);
        [OperationContract]
        lmReturnValue ListingVehicleGetPhotos(string kSession, int kListing);
        [OperationContract]
        lmReturnValue WPUpdateSet(string kSession, string jsonData);
        [OperationContract]
        lmReturnValue AddPhotographsExplicit(string kSession, int kListing, string uploadPath, string fileList, string extList, int photoType);
        [OperationContract]
        lmReturnValue AssignDamagePhoto(string kSession, string kListing, int OperationType, int kPhoto, int kWholesaleDamagePhoto);
        [OperationContract]
        lmReturnValue AddMakeModel(string kSession, int kMake, string Make, string Model);
        [OperationContract]
        lmReturnValue VehicleClassSet(string kSession, string kListing, string kVehicleClass);
        [OperationContract]
        lmReturnValue ListingPhotosGet(string kSession, int kListing);
        [OperationContract]
        lmReturnValue MarkVehicleUnavailable(string kSession, int kListing);
        [OperationContract]
        lmReturnValue ReorderPhotos(string kSession, string kListing, string PhotoOrder);
    }
}
