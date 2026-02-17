using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using LMWholesale.Listing;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class EndWholesale
    {
        private ListingClient listingClient;
        private WholesaleClient wholesaleClient;

        public EndWholesale()
        {
            listingClient = listingClient ?? new ListingClient();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }

        public EndWholesale(ListingClient listingClient, WholesaleClient wholesaleClient)
        {
            this.listingClient = listingClient;
            this.wholesaleClient = wholesaleClient;
        }
        internal static readonly EndWholesale instance = new EndWholesale();
        public EndWholesale Self
        {
            get { return instance; }
        }

        public Dictionary<string, object> ListingDetailGet(string kSession, int kDealer, int kListing)
        {
            Dictionary<string, object> returnInfo = new Dictionary<string, object> { { "ErrorResponse", "" } };

            Listing.lmReturnValue vehicleDetail = Self.listingClient.ListingDetailGet(kSession, kDealer, kListing, 0);
            if (vehicleDetail.Result == Listing.ReturnCode.LM_SUCCESS)
                returnInfo["dr"] = vehicleDetail.Data.Tables["VehicleData"].Rows[0];
            else if (vehicleDetail.Result == Listing.ReturnCode.LM_INVALIDSESSION)
                WholesaleUser.WholesaleUser.ClearUser(vehicleDetail.ResultString);
            else
                returnInfo["ErrorResponse"] =
                    $"<script>alert('Unable to perform request due to the following error: {vehicleDetail.ResultString}.  Please try again or call support for assistance.');</script>";

            return returnInfo;
        }

        public string ListingAuctionDataGet(string kSession, int kDealer, int kListing)
        {
            StringBuilder s = new StringBuilder();

            Listing.lmReturnValue result = Self.listingClient.ListingAuctionDataGet(kSession, kDealer, kListing);
            List<Dictionary<string, string>> auctions = LMWholesale.WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 1);
            if (result.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = result.Data.Tables[0];
                //VehicleGrade.Text = GradeSet(dt.Rows[0]);

                // Populate available auctions a dealer can list
                int count = 0;
                foreach (Dictionary<string, string> auction in auctions)
                {
                    string auctionName = auction["WholesaleAuctionName"];
                    if (auctionName == "ADESA")
                        auctionName = "OpenLane";
                    else if (auctionName.Contains("OVE"))
                        auctionName = "OVE";

                    if (auctionName == "CarOffer")
                        continue;

                    // Search existing Listing DataTable for info
                    DataRow[] dr = dt.Select($"AuctionName LIKE '%{auctionName}%'");

                    if (auction["WholesaleAuctionName"] == "Integrated Auction Solutions")
                        auctionName = "IAS";
                    if (auction["WholesaleAuctionName"] == "ADESA")
                        auctionName = "ADESA";

                    if (dr.Length == 0)
                    {
                        //if (count > 6)
                        //{
                        //    s.Append("<br/>");
                        //    count = 0;
                        //}

                        s.Append($"<div class='ColRowSwap'>");
                        s.Append($"&nbsp;<input id='{auctionName}CheckEnd' type='checkbox' value='{auction["kWholesaleAuction"]}' class='SingleInput' style='font-weight:bold;' disabled/>&nbsp;");
                        s.Append($"<label for='{auctionName}CheckEnd' style='font-weight:bold'>{(auctionName == "RemarketingPlus" ? "Remarketing+" : auctionName)}</label>");
                        s.Append($"</div>");
                    }
                    else
                    {
                        string canRemove = dr[0]["Status"].ToString() == "0" ? "disabled" : "checked";

                        //if (count > 6)
                        //{
                        //    s.Append("<br/>");
                        //    count = 0;
                        //}

                        s.Append($"<div class='ColRowSwap'>");
                        s.Append($"&nbsp;<input id='{auctionName}CheckEnd' type='checkbox' value='{auction["kWholesaleAuction"]}' style='font-weight:bold;' class='SingleInput' {canRemove}/>&nbsp;");
                        s.Append($"<label for='{auctionName}CheckEnd' style='font-weight:bold'>{(auctionName == "RemarketingPlus" ? "Remarketing+" : auctionName)}</label>");
                        s.Append($"</div>");
                    }

                    count++;
                }
            }

            return s.ToString();
        }

        public int SubmitToRemoveMultiAuction(string kSession, int kDealer, string kListing, object[] auctions)
        {
            List<string> columns = new List<string>
            {
                "kWholesaleAuction","RemoveFromAuction","kWholesaleListingType","kWholesaleLocationIndicator","kWholesaleLocationCode",
                "kWholesaleFacilitatedAuctionCode","kWholesaleBidIncrement","StartPrice","FloorPrice","BuyNowPrice","SellerID","BuyerGroup",
                "StartDate","EndDate","EmailAddress","ContactPerson","VehicleLocationAddress","VehicleLocationCity","VehicleLocationState",
                "VehicleLocationZIP","VehicleLocationPhone","VehicleLocationFAX","TransitEstArrDate","RequestInspection","kWholesaleListingCategory",
                "kAASale","kWholesaleInspectionCompany","RelistCount","ServiceProviderName","ServiceProviderID","CarGroupID"
            };

            DataSet ds = CreateAuctionDataSet(columns);
            DataTable dt = ds.Tables[0];

            foreach (string kWholesaleAuction in auctions)
            {
                DataRow dr = dt.NewRow();

                dr["kWholesaleAuction"] = kWholesaleAuction;
                dr["RemoveFromAuction"] = 1;

                for (int i = 2; i < columns.Count; i++)
                    dr[columns[i]] = "";

                dt.Rows.Add(dr);
            }

            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.SubmitToMultipleAuctions(kSession, kDealer, int.Parse(kListing), ds);
            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                return 1;

            return 0;
        }

        public string MarkVehicleUnavailable(string kSession, int kListing)
        {
            Listing.lmReturnValue returnValue = Self.listingClient.MarkVehicleUnavailable(kSession, kListing);
            if (returnValue.Result == Listing.ReturnCode.LM_SUCCESS)
                return "success";

            return returnValue.ResultString;
        }

        private static DataSet CreateAuctionDataSet(List<string> columns)
        {
            DataSet emptyDS = new DataSet("Auction");
            DataTable dt = emptyDS.Tables.Add("Auction");

            foreach (string col in columns)
                dt.Columns.Add(col, typeof(string));

            dt.PrimaryKey = new DataColumn[] { dt.Columns["kWholesaleAuction"] };

            return emptyDS;
        }
    }
}