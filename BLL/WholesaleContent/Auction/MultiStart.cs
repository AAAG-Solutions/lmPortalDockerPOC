using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using LMWholesale.resource.clients;
using LMWholesale.resource.factory;
using LMWholesale.resource.model.Wholesale;

namespace LMWholesale.BLL.WholesaleContent.Auction
{
    public class MultiStart
    {
        private readonly WholesaleUser.WholesaleUser userBLL;
        private readonly AuctionFactory auctionFactory;
        private readonly WholesaleClient wholesaleClient;
        private readonly DealerClient dealerClient;

        public MultiStart()
        {
            userBLL = userBLL ?? new WholesaleUser.WholesaleUser();
            auctionFactory = auctionFactory ?? new AuctionFactory();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            dealerClient = dealerClient ?? new DealerClient();
        }

        public MultiStart(WholesaleUser.WholesaleUser userBLL, AuctionFactory auctionFactory, WholesaleClient wholesaleClient, DealerClient dealerClient)
        {
            this.userBLL = userBLL;
            this.auctionFactory = auctionFactory;
            this.wholesaleClient = wholesaleClient;
            this.dealerClient = dealerClient;
        }

        public static MultiStart Self
        {
            get { return instance; }
        }
        private static readonly MultiStart instance = new MultiStart();

        public Tuple<InventoryFilter.MultiPageFilter, string> StartWholesaleSearch(string kSession, int kDealer, string mswAuctions, InventoryFilter.MultiPageFilter multiPageFilter,
                                        bool hasAutoGrade, Dictionary<string, object> sortFilter, bool firstLoad = false)
        {
            if (sortFilter.Keys.Contains("PageFilter"))
            {
                Dictionary<string, object> pageFilter = (Dictionary<string, object>)sortFilter["PageFilter"];
                multiPageFilter.LotLocation = pageFilter["LotLocation"].ToString();
                multiPageFilter.ListingStatus = int.Parse(pageFilter["ListingStatus"].ToString());
                multiPageFilter.InspectionStatus = int.Parse(pageFilter["InspectionStatus"].ToString());
                multiPageFilter.StatusAvailable = int.Parse(pageFilter["InvStatusAvail"].ToString());
                multiPageFilter.StatusInTransit = int.Parse(pageFilter["InvStatusInTransit"].ToString());
                multiPageFilter.TypeDealerCertified = int.Parse(pageFilter["DealerCert"].ToString());
                multiPageFilter.TypeManufacturerCertified = int.Parse(pageFilter["ManufacturerCert"].ToString());
                multiPageFilter.TypePreOwned = int.Parse(pageFilter["PreOwned"].ToString());
            }

            // InvDays, LotLocation, MMRGoodPrice. 'Grade' will come later due to complexity
            string[] availableSort = new string[4] { "InvDays", "LotLocation", "MMRGoodPrice", "Mileage" };
            string sort = "";
            if (availableSort.Contains(multiPageFilter.Sort.Replace("desc","")) || sortFilter.ContainsKey("sortField")) {
                if (availableSort.Contains(sortFilter["sortField"]))
                    sort = sortFilter["sortField"].ToString();

                if (sortFilter.ContainsKey("sortOrder") && sortFilter["sortOrder"].ToString().CompareTo("desc") == 0)
                    sort += " desc";
            }

            multiPageFilter.Sort = sort;
            multiPageFilter.PageNumber = firstLoad ? 1 : int.Parse(sortFilter["pageIndex"].ToString());

            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleMultiListingsGet(multiPageFilter);
            
            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable vehicles = returnValue.Data.Tables[0];

                string selectString = "";
                if (sortFilter.Keys.Contains("PageFilter")
                    && int.Parse(((Dictionary<string, object>)sortFilter["PageFilter"])["ListingStatus"].ToString()) > 0)
                {
                    Dictionary<string, string> auctionMap = new Dictionary<string, string>
                    {
                        { "1", "OVE" },
                        { "2", "SmartAuction" },
                        { "4", "ADESA" },
                        { "6", "COPART" },
                        { "7", "AuctionEdge" },
                        { "11", "ACVAuctions" },
                        { "13", "IAA" },
                        { "14", "AuctionSimplified" },
                        { "15", "IAS" },
                        { "16", "AuctionOS" },
                        { "17", "Carmigo" },
                        //,{ "18", "CarOffer" },
                        { "19", "RemarketingPlus" }
                    };
                    selectString = "isListedon" + auctionMap[((Dictionary<string, object>)sortFilter["PageFilter"])["ListingStatus"].ToString()] + " = 0";
                }

                DataRow[] rows = vehicles.Select(selectString, sort);
                //List<DataRow> sendRows = new List<DataRow>();
                //int startIndex = (multiPageFilter.PageNumber - 1) * multiPageFilter.ItemsPerPage;
                //
                //if (startIndex > rows.Count())
                //    return "0 | {}";
                //
                //for(int i = startIndex; i < Math.Min((startIndex + multiPageFilter.ItemsPerPage), rows.Count()); i++)
                //{
                //    sendRows.Add(rows[i]);
                //}

                string count = returnValue.Values.GetValue("TotalItems", "0");
                string data = Util.serializer.Serialize(FormatData(rows.ToArray(), mswAuctions, kDealer, kSession, hasAutoGrade));
                return Tuple.Create(multiPageFilter, $"{count} | {data}");
            }

            // If we fail, return nothing
            return Tuple.Create(multiPageFilter, "0 | {}");
        }

        public bool SubmitToMultipleAuctions(string kSession, int kDealer, object[] items, Dictionary<string, object> addInfo, ref string errorMsgs, ref int successCount, ref int unSuccessCount)
        {
            List<string> columns = new List<string>
            {
                "kWholesaleAuction","kListing","StartPrice","FloorPrice","BuyNowPrice","RelistCount","kWholesaleListingType","kWholesaleListingCategory",
                "StartDate","EndDate","CarGroupID", "ForceWholesalePricing", "LimitedArbitrationPowertrainPledge", "RowNumber"
            };

            DataSet ds = CreateSubmitDataSet(columns);
            DataTable dt = ds.Tables[0];

            // This is used for psuedo pagination due to the potential amount of vehicles we would submit
            int rowNumber = 1;
            try
            {
                foreach (Dictionary<string, object> vehicle in items)
                {
                    int kWholesaleAuction = int.Parse(vehicle["kWholesaleAuction"].ToString());
                    DataRow dr = dt.NewRow();

                    IAuctionService service = Self.auctionFactory.GetAuctionService(kWholesaleAuction);
                    DataRow auctionInfo = service.GetAuctionInfo(kSession, kDealer).DataSet.Tables[0].Rows[0];

                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (columns[i] == "RowNumber")
                            continue;
                        else if (columns[i].Contains("Date"))
                            dr[columns[i]] = DateTime.Parse(vehicle[columns[i]].ToString());
                        else
                            dr[columns[i]] = vehicle[columns[i]];
                    }

                    int kWholesaleListingType = int.Parse(dr["kWholesaleListingType"].ToString());

                    #region Check kWholesaleAuction and Prices
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
                    //else if (kWholesaleAuction == 18)
                    //    dr["kWholesaleListingType"] = 5; // CarOffer always submit as Bid/Buy/Offer
                    else if (kWholesaleAuction == 19)
                        dr["kWholesaleListingType"] = 5; // RemarketingPlus always submit as Bid/Buy/Offer
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
                        dr["kWholesaleListingType"] = 3;

                    // Run through each listing type to make sure that the correct info is sent
                    if (kWholesaleListingType == 1) // Bid
                    {
                        if (kWholesaleAuction == 2
                            || kWholesaleAuction == 12 || kWholesaleAuction == 6
                            || (int.Parse(addInfo["isOLDealerAccount"].ToString()) == 1 && kWholesaleAuction == 4))
                        {
                            dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString());
                        }
                        else if (int.Parse(addInfo["isOVEDealerAccount"].ToString()) == 1 && kWholesaleAuction == 1)
                        {
                            if (int.Parse(dr["FloorPrice"].ToString()) > (int.Parse(dr["StartPrice"].ToString()) + int.Parse(dr["BidIncrement"].ToString()) * 16))
                                dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString()) - (int.Parse(dr["BidIncrement"].ToString()) * 16);
                        }
                        dr["BuyNowPrice"] = 0;
                    }
                    else if (kWholesaleListingType == 2) // Buy
                    {
                        dr["StartPrice"] = 0;
                        dr["FloorPrice"] = 0;
                    }
                    else if (kWholesaleListingType == 3) // Bid/Buy
                    {
                        if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                            || (int.Parse(addInfo["isOLDealerAccount"].ToString()) == 1 && kWholesaleAuction == 4))
                            dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString());
                        else if (int.Parse(addInfo["isOVEDealerAccount"].ToString()) == 1 && kWholesaleAuction == 1)
                        {
                            if (int.Parse(dr["FloorPrice"].ToString()) > (int.Parse(dr["StartPrice"].ToString()) + int.Parse(dr["BidIncrement"].ToString()) * 16))
                                dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString()) - (int.Parse(dr["BidIncrement"].ToString()) * 16);
                        }

                        if (kWholesaleAuction == 10) // Turn Auctions
                            dr["BuyNowPrice"] = 0;
                    }
                    else if (kWholesaleListingType == 4) // Bid/Offer
                    {
                        if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                            || (int.Parse(addInfo["isOLDealerAccount"].ToString()) == 1 && kWholesaleAuction == 4))
                            dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString(), 0);
                        else if (int.Parse(addInfo["isOVEDealerAccount"].ToString()) == 1 && kWholesaleAuction == 1)
                        {
                            if (int.Parse(dr["FloorPrice"].ToString()) > (int.Parse(dr["StartPrice"].ToString()) + int.Parse(dr["BidIncrement"].ToString()) * 16))
                                dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString()) - (int.Parse(dr["BidIncrement"].ToString()) * 16);
                        }
                        dr["BuyNowPrice"] = 0;
                    }
                    else if (kWholesaleListingType == 110) // Bid/Offer 'OVE Offer Only'
                    {
                        if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                            || (int.Parse(addInfo["isOLDealerAccount"].ToString()) == 1 && kWholesaleAuction == 4))
                            dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString());
                        else if (kWholesaleAuction == 1)
                        {
                            dr["StartPrice"] = 0;
                            if (int.Parse(vehicle["MMRGoodPrice"].ToString()) > 0)
                                dr["FloorPrice"] = Math.Floor((int.Parse(vehicle["MMRGoodPrice"].ToString()) * 0.81) / 100) * 100;
                            else
                                dr["FloorPrice"] = 100;
                        }
                        dr["BuyNowPrice"] = 0;
                    }
                    else if (kWholesaleListingType == 5 || kWholesaleListingType == 200) // Bid/Buy/Offer 'OVE Bid/Buy'
                    {
                        if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                            || (int.Parse(addInfo["isOLDealerAccount"].ToString()) == 1 && kWholesaleAuction == 4))
                            dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString());
                        else if (int.Parse(addInfo["isOVEDealerAccount"].ToString()) == 1 && kWholesaleAuction == 1)
                        {
                            if (int.Parse(dr["FloorPrice"].ToString()) > (int.Parse(dr["StartPrice"].ToString()) + int.Parse(dr["BidIncrement"].ToString()) * 16))
                                dr["StartPrice"] = int.Parse(dr["FloorPrice"].ToString()) - (int.Parse(dr["BidIncrement"].ToString()) * 16);
                        }
                        if (kWholesaleAuction == 10)
                            dr["ByuNowPrice"] = 0;
                    }
                    else if (kWholesaleListingType == 100) // Bid/Buy/Offer 'OVE Buy/Offer'
                    {
                        if (kWholesaleAuction == 2 || kWholesaleAuction == 12 || kWholesaleAuction == 6
                            || (int.Parse(addInfo["isOLDealerAccount"].ToString()) == 1 && kWholesaleAuction == 4))
                            dr["StartPrice"] = int.Parse(dr["ReservePrice"].ToString(), 0);
                        else if (kWholesaleAuction == 1)
                        {
                            dr["StartPrice"] = 0;
                            dr["FloorPrice"] = 0;
                        }

                        if (kWholesaleAuction == 10)
                            dr["BuyNowPrice"] = 0;
                    }
                    else if (kWholesaleListingType == 6) // Buy/Offer
                    {
                        dr["StartPrice"] = 0;
                        dr["FloorPrice"] = 0;
                    }
                    else if (kWholesaleListingType == 120) // Buy/Offer 'OVE Offer Only'
                    {
                        dr["StartPrice"] = 0;
                        if (kWholesaleAuction == 1)
                        {
                            if (int.Parse(vehicle["MMRGoodPrice"].ToString()) > 0)
                                dr["FloorPrice"] = Math.Floor((int.Parse(vehicle["MMRGoodPrice"].ToString()) * 0.81) / 100) * 100;
                            else
                                dr["FloorPrice"] = 100;
                            dr["BuyNowPrice"] = 0;
                        }
                        else
                            dr["FloorPrice"] = 0;
                    }

                    // Default to no limited arb just in case it gets set for some reason
                    dr["LimitedArbitrationPowertrainPledge"] = "0";
                    #endregion

                    // For psuedo pagination
                    dr["RowNumber"] = rowNumber;
                    dt.Rows.Add(dr);

                    rowNumber++;
                }

                // Submit DataSet of all select Auctions and Vehicles
                // Implementing psuedo pagination due to limitations of large datasets
                int pages = Convert.ToInt32(Math.Ceiling(rowNumber / 25.00));
                bool isSuccess = true;
                for (int page = 1; page < pages + 1; page++)
                {
                    int low = ((page - 1) * 25) + 1;
                    int high = page * 25;
                    string filter = $"RowNumber >= {low} AND RowNumber <= {high}";

                    DataView dv = ds.Tables[0].DefaultView;
                    dv.RowFilter = filter;
                    DataSet filteredDS = new DataSet();
                    DataTable filteredDT = dv.ToTable();
                    filteredDS.Tables.Add(filteredDT);

                    Wholesale.lmReturnValue returnValue = Self.wholesaleClient.SubmitMultiListingToAuction(kSession, kDealer, filteredDS);
                    if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                    {
                        DataTable results = returnValue.Data.Tables[0];
                        foreach (DataRow dr in results.Rows)
                        {
                            if (int.Parse(dr["Result"].ToString()) != 0)
                            {
                                errorMsgs += $"kListing: {dr["ResultString"]}\r\n";
                                unSuccessCount += 1;
                            }
                            else
                                successCount += 1;
                        }
                    }
                    else if (returnValue.Result == Wholesale.ReturnCode.LM_INVALIDSESSION)
                        WholesaleUser.WholesaleUser.ClearUser();
                    else
                    {
                        errorMsgs = "Something went wrong! Please contact support!";
                        isSuccess = false;
                        break;
                    }
                }

                return isSuccess;
            }
            catch (Exception ex)
            {
                string error = string.Format("Something went wrong: MultiStart SubmitToMultipleAuctions [ Message - {0} | StackTrace - {1} ]", ex.Message, ex.StackTrace);
                LMWholesale.WholesaleSystem.Logger.LogLine(kSession, error);
                errorMsgs = "Something went wrong! Please contact support!";
                return false;
            }
        }

        private static List<Dictionary<string, object>> FormatData(DataRow[] drs, string auctionString, int kDealer, string session, bool hasAutoGrade)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();
            List<Dictionary<string, string>> auctions = LMWholesale.WholesaleSystem.GetAvailableAuctions(session, kDealer, Self.wholesaleClient, 1);

            foreach (DataRow row in drs)
            {
                var dict = new Dictionary<string, object>();

                // Store local copy of auctionString to reuse
                string updateString = auctionString;

                foreach (DataColumn col in row.Table.Columns)
                    dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));

                string vinStock = $"<div style='text-align:left;'>{row["Year"]} {row["Make"]} {row["Model"]}";
                vinStock += $"<br/><b>VIN:</b> {row["Vin"]}<br/><b>Stock #:</b> {row["StockNumber"]}";
                vinStock += $"<br/><b>OVE Bid Increment:</b> {row["BidIncrementOVE"]}";
                vinStock += $"<br/><b>CarGroup ID:</b> {row["CarGroupID"]}</div>";
                dict["VINStock"] = vinStock;

                // Set up Input Boxes
                dict["StartPriceBox"] = $"<input type='text' id='startprice_{row["kListing"]}' value='{row["StartPrice"]}' {LMWholesale.WholesaleSystem.onInputNumber} style='width:95%;'/>";
                dict["ReservePriceBox"] = $"<input type='text' id='reserveprice_{row["kListing"]}' value='{row["ReservePrice"]}' {LMWholesale.WholesaleSystem.onInputNumber} style='width:95%;'/>";
                dict["BuyItNowPriceBox"] = $"<input type='text' id='binprice_{row["kListing"]}' value='{row["BuyItNowPrice"]}' {LMWholesale.WholesaleSystem.onInputNumber} style='width:95%;'/>";

                // Set Lot Location info
                string isOve = "";
                if (!string.IsNullOrEmpty(row["OVEIsDealerAccount"].ToString()))
                    isOve = int.Parse(row["OVEIsDealerAccount"].ToString(), 0) == 1 ? "* OVE Dealer Account" : "";
                string mmr = row["MMROverride"].ToString() == "" ? "0" : row["MMROverride"].ToString();
                string supressMMR = int.Parse(mmr, 0) == 1 ? "<div style='color:red'>MMR Restriction Override</div>" : "";
                dict["LotLocation"] = $"<div style='text-align:left;'>{row["LotLocation"]}<br/><br/><b>{isOve}</b>{supressMMR}</div>";
                dict["InvLotLocation"] = row["LotLocation"].ToString();

                // Enable/Disable Auction checkboxes
                foreach (Dictionary<string, string> auction in auctions)
                {
                    if (auction["WholesaleAuctionName"] == "CarOffer")
                        continue;

                    string checkboxStatus = row[$"isListedon{auction["WholesaleAuctionName"].Replace(" ", "")}"].ToString() == "1"
                        || auction["ManualSuppress"] == "1" ? "disabled isListed='true'" : "";
                    updateString = updateString.Replace($"{auction["WholesaleAuctionName"].Replace(" ", "")}STATUS", checkboxStatus);
                }

                dict["Auctions"] = updateString.Replace("KLISTING", row["kListing"].ToString());
                dict["Grade"] = Self.userBLL.GradeSet(row, hasAutoGrade);

                dict["ErrMsg"] = $"<div id='errmsg_{row["kListing"]}' style='overflow:auto;height:150px;'></div>";

                returnList.Add(dict);
            }

            return returnList;
        }

        private static DataSet CreateSubmitDataSet(List<string> columns)
        {
            DataSet emptyDS = new DataSet("AuctionSubmit");
            DataTable dt = emptyDS.Tables.Add("Auction");

            foreach (string col in columns)
                dt.Columns.Add(col, typeof(string));

            return emptyDS;
        }

        public void GetAuctionData(string kSession, int kDealer, ref LMWholesale.WholesaleContent.Auction.MultiStart.AuctionInfo ai)
        {
            string auctionString = "";
            bool hasAdesa = false;

            List<Dictionary<string, string>> auctions = LMWholesale.WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 1);

            if (auctions.Count == 0)
                return;

            foreach (Dictionary<string, string> auction in auctions)
            {
                if (auction["WholesaleAuctionName"] == "ADESA")
                    hasAdesa = true;

                if (auction["WholesaleAuctionName"] == "CarOffer")
                    continue;

                string auctionName = auction["WholesaleAuctionName"];
                auctionString += "<div style='text-align-last:left;'>";
                auctionString += $"<input id='{auctionName.Replace(" ", "")}Check_KLISTING' {auctionName.Replace(" ", "")}STATUS type='checkbox' value='{auction["kWholesaleAuction"]}' style='font-weight:bold;'/>";
                auctionString += $"<span style='display:none;' id='MaxMMRPct_{auction["kWholesaleAuction"]}_KLISTING'>{auction["MaxMMRPct"]}</span>";
                auctionString += $"&nbsp;&nbsp;{(auctionName == "RemarketingPlus" ? "Remarketing+" : auctionName)}</div>";

                if (auction["ManualSuppress"] != "1")
                    ai.QuickSelect += $"1!{auctionName.Replace(" ", "")}:Select All {(auctionName == "RemarketingPlus" ? "Remarketing+" : auctionName)}|2!{auctionName.Replace(" ", "")}:Deselect All {(auctionName == "RemarketingPlus" ? "Remarketing+" : auctionName)}|";
            }

            // Populate default Relist Count and Listing Type
            string kWholesaleAuction = hasAdesa ? "4" : auctions[0]["kWholesaleAuction"].ToString();
            ai.AddDbInfo(Self.auctionFactory.GetAuctionService(int.Parse(kWholesaleAuction, 0)).GetAuctionInfo(kSession, kDealer).DataSet.Tables[0].Rows[0], hasAdesa);
            ai.AuctionString = auctionString;
            ai.AuctionCount = auctions.Count;
        }

        public void GetListingLotLocationList(string kSession, int kDealer, string defaultVal = "")
        {
            Dealer.lmReturnValue lotList = Self.dealerClient.LotLocationListGet(kSession, kDealer);
            StringBuilder lotString = new StringBuilder("[]ALL:Any Lot Location|");
            if (lotList.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                if (lotList.Data != null)
                {
                    DataTable ll = lotList.Data.Tables[0];
                    foreach (DataRow row in ll.Rows)
                        lotString.Append($"{row["InvLotLocation"]}:{row["InvLotLocation"]}|");
                }
            }

            if (defaultVal == "ALL")
                defaultVal = "";

            LMWholesale.WholesaleSystem.PopulateList(lotString.ToString(), "", "lstLotLocation", '|', defaultVal);
        }
    }
}