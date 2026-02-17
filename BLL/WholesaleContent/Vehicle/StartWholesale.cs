using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.SessionState;

using LMWholesale.resource.clients;
using LMWholesale.resource.factory;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class StartWholesale
    {
        public readonly AuctionFactory factory;
        private readonly WholesaleClient wholesaleClient;
        private readonly DealerClient dealerClient;
        private readonly ListingClient listingClient;

        public StartWholesale()
        {
            factory = factory ?? new AuctionFactory();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            listingClient = listingClient ?? new ListingClient();
            dealerClient = dealerClient ?? new DealerClient();
        }

        public StartWholesale(WholesaleClient wholesaleClient, ListingClient listingClient, DealerClient dealerClient, AuctionFactory factory)
        {
            this.factory = factory;
            this.wholesaleClient = wholesaleClient;
            this.listingClient = listingClient;
            this.dealerClient = dealerClient;
        }

        internal static readonly StartWholesale instance = new StartWholesale();
        public StartWholesale Self
        {
            get { return instance; }
        }

        public DataRow GetListingDetails(int kListing)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            Listing.lmReturnValue vehicleDetail = Self.listingClient.ListingDetailGet(kSession, kDealer, kListing, 0);
            if (vehicleDetail.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                DataRow dr = vehicleDetail.Data.Tables["VehicleData"].Rows[0];
                return dr;
            }

            // Default return if we fail for some reason
            return new DataTable().NewRow();
        }

        public Dictionary<string, object> GetListingAuctionData(string kSession, int kDealer, int kListing)
        {
            Dictionary<string, object> returnItems = new Dictionary<string, object>
            {
                { "ListingData", null},
                { "AvailableAuctions", null},
                { "DefaultAuctionSettings", null}
            };

            Dictionary<string, string> DefaultAuctionSettings = new Dictionary<string, string>()
            {
                { "UseInventoryPrice", "" },
                { "ForceWholesalePricing", "" },
                { "WholesaleBidIncrement", "" }
            };

            Listing.lmReturnValue result = Self.listingClient.ListingAuctionDataGet(kSession, kDealer, kListing);
            if (result.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                List<Dictionary<string, string>> auctions = LMWholesale.WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 1);
                if (auctions.Count != 0)
                {
                    bool isOVE = false;
                    returnItems["AvailableAuctions"] = auctions;
                    auctions.ForEach(auction => { if (auction["WholesaleAuctionName"] == "OVE") { isOVE = true; } });

                    // Default kWholesaleAuction
                    int kWholesaleAuction = 1;
                    if (!isOVE)
                        kWholesaleAuction = int.Parse(auctions[0]["kWholesaleAuction"].ToString());

                    DataRow dr = factory.GetAuctionService(kWholesaleAuction).GetAuctionInfo(kSession, kDealer).DataSet.Tables[0].Rows[0];
                    DefaultAuctionSettings["UseInventoryPrice"] = dr["UseInventoryPrice"].ToString();
                    DefaultAuctionSettings["ForceWholesalePricing"] = dr["ForceWholesalePricing"].ToString();
                    DefaultAuctionSettings["kWholesaleAuction"] = kWholesaleAuction.ToString();
                    DefaultAuctionSettings["WholesaleInspectionCompany"] = dr["WholesaleInspectionCompany"].ToString();
                    DefaultAuctionSettings["RelistCount"] = dr["RelistCount"].ToString();
                    DefaultAuctionSettings["VehicleLocation"] = DetermineVehicleLocation(kWholesaleAuction, dr["WholesaleLocationIndicator"].ToString());

                    returnItems["DefaultAuctionSettings"] = DefaultAuctionSettings;

                    string selectedListingType = dr["WholesaleListingType"].ToString().Substring(1, dr["WholesaleListingType"].ToString().IndexOf("]") - 1);
                    string lstBid = "[]1:1|";
                    LMWholesale.WholesaleSystem.PopulateList(
                        dr["WholesaleBidIncrement"].ToString() == "[]" ? lstBid : dr["WholesaleBidIncrement"].ToString(), "", "lstBidIncrement", '|', "1");
                    LMWholesale.WholesaleSystem.GetDefaultListingTypes("lstListingType", selectedListingType);
                    LMWholesale.WholesaleSystem.PopulateList(dr["WholesaleInspectionCompany"].ToString(), "", "lstInspectionCompany", '|', "1");
                }
                else
                {
                    returnItems["AvailableAuctions"] = new List<Dictionary<string, string>>();
                    LMWholesale.WholesaleSystem.GetBidIncrements("lstBidIncrement");
                    LMWholesale.WholesaleSystem.PopulateList("[]0:1|", "", "lstInspectionCompany", '|', "0");
                }
                returnItems["ListingData"] = result.Data.Tables[0];
            }

            return returnItems;
        }

        public string SubmitToListMultiAuction(object[] AuctionList, Dictionary<string, object> additionalInfo, ref Dictionary<string, object> returnInfo)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            List<string> columns = new List<string>
            {
                "kWholesaleAuction","RemoveFromAuction","kWholesaleListingType","kWholesaleLocationIndicator","kWholesaleLocationCode",
                "kWholesaleFacilitatedAuctionCode","kWholesaleBidIncrement","StartPrice","FloorPrice","BuyNowPrice","SellerID","BuyerGroup",
                "StartDate","EndDate","EmailAddress","ContactPerson","VehicleLocationAddress","VehicleLocationCity","VehicleLocationState",
                "VehicleLocationZIP","VehicleLocationPhone","VehicleLocationFAX","TransitEstArrDate","RequestInspection","kWholesaleListingCategory",
                "kAASale","kWholesaleInspectionCompany","ServiceProviderName","ServiceProviderID","CarGroupID", "ForceWholesalePricing",
                "LimitedArbitrationPowertrainPledge", "RelistCount"
            };

            DataSet ds = CreateAuctionDataSet(columns);
            DataTable dt = ds.Tables[0];

            StringBuilder errorMsgs = new StringBuilder();

            foreach (Dictionary<string, object> listingInfo in AuctionList)
            {
                int kWholesaleAuction = int.Parse(listingInfo["kWholesaleAuction"].ToString(), 0);
                DataRow dr = dt.NewRow();

                IAuctionService service = factory.GetAuctionService(kWholesaleAuction);
                DataTable auctionTbl = service.GetCredentials(kSession, kDealer).DataSet.Tables[0];
                DataRow auctionInfo = service.GetAuctionInfo(kSession, kDealer).DataSet.Tables[0].Rows[0];
                dr["kWholesaleAuction"] = kWholesaleAuction;
                dr["RemoveFromAuction"] = 0;

                for (int i = 2; i < columns.Count; i++)
                    dr[columns[i]] = listingInfo[columns[i]];

                DataRow[] auctionCred;
                if (!string.IsNullOrEmpty(additionalInfo["lotLocation"].ToString()))
                {
                    string location = additionalInfo["lotLocation"].ToString().Replace("&apos;", "'");
                    // In order for us to perform a Select operation, we need to 'escape' special characters
                    // ' = '' (single quote to double single quote)
                    auctionCred = auctionTbl.Select($"InvLotLocation = '{location.Replace("'","''")}'");
                }
                else
                    auctionCred = auctionTbl.Select($"InvLotLocation = '[ANY]' OR InvLotLocation = ''");

                int kWholesaleListingType = int.Parse(dr["kWholesaleListingType"].ToString());
                int StartPrice = int.Parse(dr["StartPrice"].ToString(), NumberStyles.AllowThousands);
                int BINPrice = int.Parse(dr["BuyNowPrice"].ToString(), NumberStyles.AllowThousands);
                int ReservePrice = int.Parse(dr["FloorPrice"].ToString(), NumberStyles.AllowThousands);

                #region Ensure auction information is correct
                if (kWholesaleAuction == 10)
                    dr["kWholesaleListingType"] = 4; // TurnAuctions always submit as Bid/Offer
                else if (kWholesaleAuction == 7)
                    dr["kWholesaleListingType"] = 6; // AuctionEdge always submit as Buy/Offer
                else if (kWholesaleAuction == 6)
                    dr["kWholesaleListingType"] = 1; // COPART always submit as Bid
                else if (kWholesaleAuction == 13)
                    dr["kWholesaleListingType"] = 2; // IAA always submit as Buy
                else if (kWholesaleAuction == 16)
                    dr["kWholesaleListingType"] = 6; // AuctionOS always submit as Buy/Offer
                else if (kWholesaleAuction == 17)
                    dr["kWholesaleListingType"] = 5; // Carmigo always submit as Bid/Buy/Offer
                else if (kWholesaleAuction == 18)
                    dr["kWholesaleListingType"] = 5; // Carmigo always submit as Bid/Buy/Offer
                else if (kWholesaleListingType == 200)
                {
                    if (kWholesaleAuction == 1 || kWholesaleAuction == 12)
                        dr["kWholesaleListingType"] = 3; // OVE or eDealer Direct Bid/Buy
                    else
                        dr["kWholesaleListingType"] = 5; // Bid/Buy/Offer
                }
                else if (kWholesaleListingType == 100) // Bid/Buy/Offer 'OVE Buy/Offer'
                {
                    if (kWholesaleAuction == 1)
                        dr["kWholesaleListingType"] = 6;
                    else if (kWholesaleAuction == 12)
                        dr["kWholesaleListingType"] = 3; // eDealer Direct Bid/Buy
                    else
                        dr["kWholesaleListingType"] = 5;
                }
                else if (kWholesaleListingType == 110) // Bid/Offer OVE Offer Only
                {
                    if (kWholesaleAuction == 1)
                        dr["kWholesaleListingType"] = 7;
                    else if (kWholesaleAuction == 12)
                        dr["kWholesaleListingType"] = 1; // eDealer Direct Bid
                    else
                        dr["kWholesaleListingType"] = 4;
                }
                else if (kWholesaleListingType == 120) // Buy/Offer OVE Offer Only
                {
                    if (kWholesaleAuction == 1)
                        dr["kWholesaleListingType"] = 7;
                    else
                        dr["kWholesaleListingType"] = 6;
                }
                else if (kWholesaleListingType == 4 && kWholesaleAuction == 12) // Bid/Offer
                    dr["kWholesaleListingType"] = 1;
                else if (kWholesaleListingType == 5 && kWholesaleAuction == 12) // Bid/Buy/Offer
                    dr["kWholesaleListingType"] = 5;

                // Run through each listing type to make sure that the correct info is sent
                if (kWholesaleListingType == 1) // Bid
                {
                    if (kWholesaleAuction == 2
                        || kWholesaleAuction == 12 || kWholesaleAuction == 6
                        || (additionalInfo["isOLDealerAccount"].ToString() == "True" && kWholesaleAuction == 4))
                    {
                        StartPrice = ReservePrice;
                    }
                    else if (additionalInfo["isOVEDealerAccount"].ToString() == "True" && kWholesaleAuction == 1)
                    {
                        if (ReservePrice > (StartPrice + int.Parse(dr["BidIncrement"].ToString()) * 16))
                            StartPrice = ReservePrice - (int.Parse(dr["BidIncrement"].ToString()) * 16);
                    }
                    BINPrice = 0;
                }
                else if (kWholesaleListingType == 2) // Buy
                {
                    StartPrice = 0;
                    ReservePrice = 0;
                }
                else if (kWholesaleListingType == 3) // Bid/Buy
                {
                    if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                        || (additionalInfo["isOLDealerAccount"].ToString() == "True" && kWholesaleAuction == 4))
                        StartPrice = ReservePrice;
                    else if (additionalInfo["isOVEDealerAccount"].ToString() == "True" && kWholesaleAuction == 1)
                    {
                        if (ReservePrice > (StartPrice + int.Parse(dr["BidIncrement"].ToString()) * 16))
                            StartPrice = ReservePrice - (int.Parse(dr["BidIncrement"].ToString()) * 16);
                    }

                    if (kWholesaleAuction == 10) // Turn Auctions
                        BINPrice = 0;
                }
                else if (kWholesaleListingType == 4) // Bid/Offer
                {
                    if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                        || (additionalInfo["isOLDealerAccount"].ToString() == "True" && kWholesaleAuction == 4))
                        StartPrice = ReservePrice;
                    else if (additionalInfo["isOVEDealerAccount"].ToString() == "True" && kWholesaleAuction == 1)
                    {
                        if (ReservePrice > (StartPrice + int.Parse(dr["BidIncrement"].ToString()) * 16))
                            StartPrice = ReservePrice - (int.Parse(dr["BidIncrement"].ToString()) * 16);
                    }
                    BINPrice = 0;
                }
                else if (kWholesaleListingType == 110) // Bid/Offer 'OVE Offer Only'
                {
                    if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                        || (additionalInfo["isOLDealerAccount"].ToString() == "True" && kWholesaleAuction == 4))
                        StartPrice = ReservePrice;
                    else if (kWholesaleAuction == 1)
                    {
                        StartPrice = 0;
                        if (int.Parse(additionalInfo["MMRGoodPrice"].ToString()) > 0)
                            ReservePrice = (int)(Math.Floor((int.Parse(additionalInfo["MMRGoodPrice"].ToString()) * 0.81) / 100) * 100);
                        else
                            ReservePrice = 100;
                    }
                    BINPrice = 0;
                }
                else if (kWholesaleListingType == 5 || kWholesaleListingType == 200) // Bid/Buy/Offer 'OVE Bid/Buy'
                {
                    if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                        || (additionalInfo["isOLDealerAccount"].ToString() == "True" && kWholesaleAuction == 4))
                        StartPrice = ReservePrice;
                    else if (additionalInfo["isOVEDealerAccount"].ToString() == "True" && kWholesaleAuction == 1)
                    {
                        if (ReservePrice > (StartPrice + int.Parse(dr["BidIncrement"].ToString()) * 16))
                            StartPrice = ReservePrice - (int.Parse(dr["BidIncrement"].ToString()) * 16);
                    }
                    if (kWholesaleAuction == 10)
                        BINPrice = 0;
                }
                else if (kWholesaleListingType == 100) // Bid/Buy/Offer 'OVE Buy/Offer'
                {
                    if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                        || (additionalInfo["isOLDealerAccount"].ToString() == "True" && kWholesaleAuction == 4))
                        StartPrice = ReservePrice;
                    else if (kWholesaleAuction == 1)
                    {
                        StartPrice = 0;
                        ReservePrice = 0;
                    }

                    if (kWholesaleAuction == 10)
                        BINPrice = 0;
                }
                else if (kWholesaleListingType == 6) // Buy/Offer
                {
                    StartPrice = 0;
                    ReservePrice = 0;
                }
                else if (kWholesaleListingType == 120) // Buy/Offer 'OVE Offer Only'
                {
                    StartPrice = 0;
                    if (kWholesaleAuction == 1)
                    {
                        if (int.Parse(additionalInfo["MMRGoodPrice"].ToString()) > 0)
                            ReservePrice = (int)(Math.Floor((int.Parse(additionalInfo["MMRGoodPrice"].ToString()) * 0.81) / 100) * 100);
                        else
                            ReservePrice = 100;
                        BINPrice = 0;
                    }
                    else
                        ReservePrice = 0;
                }

                // Set prices back to the DataRow
                dr["StartPrice"] = StartPrice;
                dr["BuyNowPrice"] = BINPrice;
                dr["FloorPrice"] = ReservePrice;

                if (dr["VehicleLocationState"].ToString() == "-- Select a State --")
                    dr["VehicleLocationState"] = "";

                // Implement ForceWholesalePricing

                // Default to no limited arb just in case it gets set for some reason
                dr["LimitedArbitrationPowertrainPledge"] = 0;
                #endregion

                // #TODO: Need to make sure that we select the correct auction credentials
                if (auctionCred.Length > 0)
                    SetAuctionSpecificData(ref dr, kWholesaleAuction, auctionInfo, errorMsgs, auctionCred[0], true);
                else
                    SetAuctionSpecificData(ref dr, kWholesaleAuction, auctionInfo, errorMsgs);

                dt.Rows.Add(dr);
            }

            Wholesale.lmReturnValue submit =
                Self.wholesaleClient.SubmitToMultipleAuctions(kSession, kDealer, int.Parse(additionalInfo["kListing"].ToString(), 0), dt.DataSet);
            if (submit.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                returnInfo["errormsgs"] = errorMsgs.ToString();
                returnInfo["success"] = 1;
            }

            // If we are unsuccessful for some reason, return success = 0 with error messages
            returnInfo["errormsgs"] = String.IsNullOrEmpty(errorMsgs.ToString()) ? submit.ResultString.Replace("DB Error:", "") : errorMsgs.ToString();
            return Util.serializer.Serialize(returnInfo);
        }

        public void SetAuctionSpecificData(ref DataRow dr, int kWholesaleAuction, DataRow auctionInfo, StringBuilder errorMsgs, DataRow auctionCreds = null, bool hasCreds = false)
        {
            if (auctionCreds != null)
            {
                dr["SellerID"] = auctionCreds["SellerID"].ToString();
                dr["BuyerGroup"] = auctionCreds["BuyerGroup"].ToString();
                dr["ServiceProviderName"] = auctionCreds["ServiceProviderName"].ToString();
                dr["ServiceProviderID"] = auctionCreds["ServiceProviderID"].ToString();
                dr["CarGroupID"] = auctionCreds["CarGroupID"].ToString();
            }

            switch (kWholesaleAuction)
            {
                case 1: // OVE
                    int oveCount = 0;
                    // Check to make sure that we have selected options for Facilicated Auction and Physical Location Codes
                    if (auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().StartsWith("[]"))
                    {
                        errorMsgs.Append("You must select a Facilitated Auction Code for OVE listings\r\n");
                        oveCount++;
                    }
                    if (auctionInfo["kWholesaleLocationCode"].ToString().StartsWith("[]"))
                    {
                        errorMsgs.Append("You must select a Physical Location Code for OVE listings\r\n");
                        oveCount++;
                    }

                    if (oveCount != 0)
                        break;

                    if (!hasCreds)
                    {
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = auctionInfo["BuyerGroup"].ToString();
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }

                    dr["kAASale"] = "";
                    // Check to make sure that we have selected options for Facilicated Auction and Physical Location Codes
                    string OVEFacilitatedAuctionCode = auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().Substring(1, auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().IndexOf("]") - 1);
                    if (OVEFacilitatedAuctionCode == "-- Select a Location Code --" || OVEFacilitatedAuctionCode == "")
                        dr["kWholesaleFacilitatedAuctionCode"] = "";
                    else
                    {
                        Dictionary<string, string> locationDict = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString(), "", '|', OVEFacilitatedAuctionCode);
                        dr["kWholesaleFacilitatedAuctionCode"] = locationDict["Selected"].ToString();
                    }
                    string OVELocationCode = auctionInfo["kWholesaleLocationCode"].ToString().Substring(1, auctionInfo["kWholesaleLocationCode"].ToString().IndexOf("]") - 1);
                    if (OVELocationCode == "-- Select a Location Code --" || OVELocationCode == "")
                        dr["kWholesaleLocationCode"] = "";
                    else
                    {
                        Dictionary<string, string> locationDict = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleLocationCode"].ToString(), "", '|', OVELocationCode);
                        dr["kWholesaleLocationCode"] = locationDict["Selected"].ToString();
                    }
                    break;
                case 2: // SmartAuction
                    if (!hasCreds)
                    {
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = "";
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }

                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";

                    if (auctionInfo["kWholesaleLocationCode"].ToString().StartsWith("[]"))
                    {
                        errorMsgs.Append("You must select a Location Code for SmartAuction listings\r\n");
                        break;
                    }

                    // Check to make sure that we have selected options for Physical Location Codes
                    string SALocationCode = auctionInfo["kWholesaleLocationCode"].ToString().Substring(1, auctionInfo["kWholesaleLocationCode"].ToString().IndexOf("]") - 1);
                    if (SALocationCode == "-- Select a Location Code --" || SALocationCode == "")
                        dr["kWholesaleLocationCode"] = "";
                    else
                    {
                        Dictionary<string, string> locationDict = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleLocationCode"].ToString(), "", '|', SALocationCode);
                        dr["kWholesaleLocationCode"] = locationDict["Selected"].ToString();
                    }
                    break;
                case 4: // OpenLane
                    if (!hasCreds)
                    {
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = auctionInfo["BuyerGroup"].ToString();
                        dr["ServiceProviderName"] = auctionInfo["ServiceProviderName"].ToString();
                        dr["ServiceProviderID"] = auctionInfo["ServiceProviderID"].ToString();
                        dr["CarGroupID"] = auctionInfo["OrganizationName"].ToString();
                    }

                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                case 6: // COPART
                    if (!hasCreds)
                    {
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = auctionInfo["BuyerGroup"].ToString();
                        dr["ServiceProviderName"] = auctionInfo["ServiceProviderName"].ToString();
                        dr["CarGroupID"] = "";
                        dr["ServiceProviderID"] = "";
                    }
                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                case 7: // AuctionEdge
                    if (!hasCreds)
                    {
                        dr["BuyerGroup"] = "";
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }
                    dr["kWholesaleLocationCode"] = "";
                    dr["kAASale"] = "";

                    if (auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().StartsWith("[]"))
                    {
                        errorMsgs.Append("You must select a Facilitated Auction Code for Auction Edge listings\r\n");
                        break;
                    }

                    // Check to make sure that we have selected options for Facilicated Auction
                    string AELocationCode = auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().Substring(1, auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().IndexOf("]") - 1);
                    if (AELocationCode == "-- Select a Location Code --" || AELocationCode == "")
                        dr["kWholesaleFacilitatedAuctionCode"] = "";
                    else
                    {
                        Dictionary<string, string> locationDict = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString(), "", '|', AELocationCode);
                        dr["kWholesaleFacilitatedAuctionCode"] = locationDict["Selected"].ToString();
                    }
                    break;
                case 10: // TurnAuctions
                    dr["SellerID"] = auctionInfo["SellerID"].ToString();
                    dr["BuyerGroup"] = "";
                    dr["kWholesaleLocationCode"] = "";

                    dr["kAASale"] = "";
                    dr["ServiceProviderName"] = "";
                    dr["ServiceProviderID"] = "";
                    dr["CarGroupID"] = "";

                    string TALocationCode = auctionInfo["kWholesaleLocationCode"].ToString().Substring(1, auctionInfo["kWholesaleLocationCode"].ToString().IndexOf("]") - 1);
                    if (TALocationCode == "-- Select a Location Code --" || TALocationCode == "")
                        dr["kWholesaleLocationCode"] = "";
                    else
                    {
                        Dictionary<string, string> locationDict = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleLocationCode"].ToString(), "", '|', TALocationCode);
                        dr["kWholesaleLocationCode"] = locationDict["Selected"].ToString();
                    }
                    break;
                case 11: // ACV Auctions
                    if (!hasCreds)
                    {
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = "";
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }
                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                case 12: // eDealer Direct
                    if (!hasCreds)
                    {
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = "";
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }
                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                case 13: // IAA
                    if (!hasCreds)
                    {
                        dr["BuyerGroup"] = "";
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }
                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                case 14: // Auction Simplified
                    if (!hasCreds)
                    {
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = "";
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }
                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                case 15: // IAS
                    if (!hasCreds)
                    {
                        if (String.IsNullOrEmpty(auctionInfo["SellerID"].ToString()))
                        {
                            errorMsgs.Append("You must have a Seller ID for IAS listings\r\n");
                            break;
                        }
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = "";
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }

                    dr["kWholesaleLocationCode"] = "";
                    dr["kAASale"] = "";

                    if (auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().StartsWith("[]"))
                    {
                        errorMsgs.Append("You must select a Facilitated Auction Code for Auction Edge listings\r\n");
                        break;
                    }

                    // Check to make sure that we have selected options for Facilicated Auction
                    string IASLocationCode = auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().Substring(1, auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString().IndexOf("]") - 1);
                    if (IASLocationCode == "-- Select a Location Code --" || IASLocationCode == "")
                        dr["kWholesaleFacilitatedAuctionCode"] = "";
                    else
                    {
                        Dictionary<string, string> locationDict = LMWholesale.WholesaleSystem.BuildList(auctionInfo["kWholesaleFacilitatedAuctionCode"].ToString(), "", '|', IASLocationCode);
                        dr["kWholesaleFacilitatedAuctionCode"] = locationDict["Selected"].ToString();
                    }
                    break;
                case 17: // Carmigo
                    if (!hasCreds)
                    {
                        if (String.IsNullOrEmpty(auctionInfo["SellerID"].ToString()))
                        {
                            errorMsgs.Append("You must have a Seller ID for Carmigo listings\r\n");
                            break;
                        }
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = "";
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }

                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                case 18: // CarOffer
                    if (!hasCreds)
                    {
                        if (String.IsNullOrEmpty(auctionInfo["SellerID"].ToString()))
                        {
                            errorMsgs.Append("You must have a Seller ID for CarOffer listings\r\n");
                            break;
                        }
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = "";
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }

                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                case 19: // RemarketingPlus
                    if (!hasCreds)
                    {
                        if (String.IsNullOrEmpty(auctionInfo["SellerID"].ToString()))
                        {
                            errorMsgs.Append("You must have a Seller ID for Remarketing+ listings\r\n");
                            break;
                        }
                        dr["SellerID"] = auctionInfo["SellerID"].ToString();
                        dr["BuyerGroup"] = "";
                        dr["ServiceProviderName"] = "";
                        dr["ServiceProviderID"] = "";
                        dr["CarGroupID"] = "";
                    }

                    dr["kWholesaleLocationCode"] = "";
                    dr["kWholesaleFacilitatedAuctionCode"] = "";
                    dr["kAASale"] = "";
                    break;
                default:
                    // Nothing to do here if we send in an auction we don't know about
                    // or there is nothing for us to do
                    break;
            }
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

        private string DetermineVehicleLocation(int kWholesaleAuction, string VehicleLocations)
        {
            // At Dealership
            string defaultLocation = "1";

            string defaultVehicleLocations = "1:At Dealership|2:At Auction|3:In Transit|4:At Distribution Center|5:Unspecified|";
            if (kWholesaleAuction != 1)
            {
                return LMWholesale.WholesaleSystem.BuildList(VehicleLocations + defaultVehicleLocations, "", '|')["Selected"];
            }

            return LMWholesale.WholesaleSystem.BuildList(VehicleLocations, "", '|')["Selected"];
        }
    }
}