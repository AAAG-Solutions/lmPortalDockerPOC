using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Auction
{
    public class MultiEnd
    {
        private readonly WholesaleClient wholesaleClient;
        private ListingClient listingClient;

        public MultiEnd()
        {
            listingClient = listingClient ?? new ListingClient();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }

        public MultiEnd(ListingClient listingClient, WholesaleClient wholesaleClient)
        {
            this.listingClient = listingClient;
            this.wholesaleClient = wholesaleClient;
        }

        internal static readonly MultiEnd instance = new MultiEnd();
        public MultiEnd Self
        {
            get { return instance; }
        }

        public bool RemoveFromMultipleAuctions(string kSession, int kDealer, Dictionary<string, object> vehicleInfo)
        {
            List<string> columns = new List<string>
            {
                "RemoveFromAuction", "kWholesaleAuction", "kWholesaleListingType","kWholesaleLocationIndicator","kWholesaleLocationCode",
                "kWholesaleFacilitatedAuctionCode","kWholesaleBidIncrement","StartPrice","FloorPrice","BuyNowPrice","SellerID","BuyerGroup",
                "StartDate","EndDate","EmailAddress","ContactPerson","VehicleLocationAddress","VehicleLocationCity","VehicleLocationState",
                "VehicleLocationZIP","VehicleLocationPhone","VehicleLocationFAX","TransitEstArrDate","RequestInspection","kWholesaleListingCategory",
                "kAASale","kWholesaleInspectionCompany","ServiceProviderName","ServiceProviderID","CarGroupID", "ForceWholesalePricing",
                "LimitedArbitrationPowertrainPledge", "RelistCount"
            };

            DataSet ds = CreateAuctionDataSet(columns);
            DataTable dt = ds.Tables[0];
            object[] auctionList = (object[])vehicleInfo["auctions"];

            for (int i = 0; i < auctionList.Length; i++)
            {
                DataRow dr = dt.NewRow();
                dr["RemoveFromAuction"] = 1;
                dr["kWholesaleAuction"] = auctionList[i].ToString();

                for (int c = 2; c < columns.Count; c++)
                    dr[columns[c]] = "";

                // Default to no limited arb just in case it gets set for some reason
                dr["LimitedArbitrationPowertrainPledge"] = 0;
                dt.Rows.Add(dr);
            }

            Wholesale.lmReturnValue result = Self.wholesaleClient.SubmitToMultipleAuctions(kSession, kDealer, int.Parse(vehicleInfo["kListing"].ToString()), dt.DataSet);

            if (result.Result == Wholesale.ReturnCode.LM_SUCCESS)
                return true;

            return false;
        }

        public bool MarkVehicleUnavailable(string kSession, int kListing)
        {
            Listing.lmReturnValue returnValue = Self.listingClient.MarkVehicleUnavailable(kSession, kListing);
            if (returnValue.Result == Listing.ReturnCode.LM_SUCCESS)
                return true;

            return false;
        }

        public DataSet CreateAuctionDataSet(List<string> columns)
        {
            DataSet emptyDS = new DataSet("Auction");
            DataTable dt = emptyDS.Tables.Add("Auction");

            foreach (string col in columns)
                dt.Columns.Add(col, typeof(string));

            dt.PrimaryKey = new DataColumn[] { dt.Columns["kWholesaleAuction"] };

            return emptyDS;
        }

        public Dictionary<string, string> GetAuctionStrings()
        {
            HttpSessionState session = HttpContext.Current.Session;
            string sessid = (string)session["kSession"];
            int kDealer = (int)session["kDealer"];
            string searchVehicleGridColumns = ":StockNumber:Stock #:125|!:kListing::10|:YYMVin:Year/Make/Model<br/>VIN:150|:ExtColor:Color:80|:Mileage::60|:InvDays:Age:60|";

            List<Dictionary<string, string>> auctions = LMWholesale.WholesaleSystem.GetAvailableAuctions(sessid, kDealer, wholesaleClient, 1);

            string quickSelect = "[]0:-- Quick Select Inventory --|1:Select All|2:Deselect All|";
            string auctionList = "";

            foreach (Dictionary<string, string> auction in auctions)
            {
                string auctionName = auction["WholesaleAuctionName"].ToString();
                if (auctionName == "RemarketingPlus")
                    auctionName = "Remarketing+";

                if (auctionName == "CarOffer")
                    continue;

                quickSelect += $"1!{auctionName}:Select All {auctionName}|2!{auctionName}:Deselect All {auctionName}|";
                searchVehicleGridColumns += $":{auctionName}::90|";
                auctionList += $"{auctionName}|{auction["kWholesaleAuction"]},";
            }

            return new Dictionary<string, string>() { { "quickSelect", quickSelect }, { "searchVehicleGridColumns", searchVehicleGridColumns }, { "auctionList", auctionList } };
        }
    }
}