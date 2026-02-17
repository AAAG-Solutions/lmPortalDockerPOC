using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using LMWholesale.DAS;
using LMWholesale.resource.clients;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class Update
    {
        private readonly DealerClient dealerClient;
        private readonly ListingClient listingClient;
        private readonly LookupClient lookupClient;
        private readonly DASClient dasClient;

        public Update()
        {
            dealerClient = dealerClient ?? new DealerClient();
            listingClient = listingClient ?? new ListingClient();
            lookupClient = lookupClient ?? new LookupClient();
            dasClient = dasClient ?? new DASClient();
        }

        public Update(DealerClient dealerClient, ListingClient listingClient, LookupClient lookupClient, DASClient dasClient)
        {
            this.dealerClient = dealerClient;
            this.listingClient = listingClient;
            this.lookupClient = lookupClient;
            this.dasClient = dasClient;
        }
        internal static readonly Update instance = new Update();
        public Update Self
        {
            get { return instance; }
        }

        public DataTable GetDealerInvPref(string kSession, int kDealer)
        {
            // #TODO: Need to figure out why SingleTable breaks but MultiTable is fine
            Dealer.lmReturnValue dealerpref = Self.dealerClient.GetDealerInfo(kSession, kDealer, "", "DealerDiagnostic");
            if (dealerpref.Result == Dealer.ReturnCode.LM_SUCCESS)
                return dealerpref.Data.Tables[0];

            // return empty DataRow if we fail for some reason
            return new DataTable();
        }

        public Dictionary<string, object> ListingDetailGet(string kSession, int kDealer, int kListing)
        {
            Dictionary<string, object> returnInfo = new Dictionary<string, object> { { "ErrorResponse", "" } };

            Listing.lmReturnValue vehicleDetail = Self.listingClient.ListingDetailGet(kSession, kDealer, kListing, 0);
            if (vehicleDetail.Result == Listing.ReturnCode.LM_SUCCESS)
                returnInfo.Add("dr", vehicleDetail.Data.Tables["VehicleData"].Rows[0]);
            else if (vehicleDetail.Result == Listing.ReturnCode.LM_INVALIDSESSION)
                WholesaleUser.WholesaleUser.ClearUser(vehicleDetail.ResultString);
            else
            {
                returnInfo["ErrorResponse"] =
                    $"<script>alert('Unable to perform request due to the following error: {vehicleDetail.ResultString}. Please try again or call support for assistance.');</script>";
            }

            // Default return if we fail for some reason
            return returnInfo;
        }

        public Tuple<int, string> PopulateAuctionInfo(string kSession, int kDealer, int kListing, string AuctionName)
        {
            Listing.lmReturnValue result = Self.listingClient.ListingAuctionDataGet(kSession, kDealer, kListing);
            if (result.Result == Listing.ReturnCode.LM_SUCCESS)
                return PopulateAuctionInfo(result.Data.Tables[0], AuctionName);

            // Return default empty string if we fail for some reason
            return new Tuple<int, string>(-1, "");
        }

        public string LotLocationSetup(string kSession, int kDealer, ContentPlaceHolder mainContent)
        {
            Dealer.lmReturnValue lstLot = Self.dealerClient.LotLocationListGet(kSession, kDealer);

            // DealerLotLocation Results
            StringBuilder lotString = new StringBuilder("[]:|");
            if (lstLot.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                if (lstLot.Data != null)
                {
                    foreach (DataRow row in lstLot.Data.Tables[0].Rows)
                    {
                        if (row["LotDescription"].ToString() != "")
                            lotString.Append($"{row["InvLotLocation"]}:{row["InvLotLocation"]} ({row["LotDescription"]})|");
                        else
                            lotString.Append($"{row["InvLotLocation"]}:{row["InvLotLocation"]} ({row["InvLotLocation"]})|");
                    }
                }
            }

            return lotString.ToString();
        }

        public bool PopulateModels(string kSession, string year, string make)
        {
            StringBuilder modelsList = new StringBuilder();
            lmReturnValue modelsDt = Self.dasClient.DASGetMotorModels(kSession, "3", year, make);
            if (modelsDt.Result != ReturnCode.LM_SUCCESS && Regex.IsMatch(modelsDt.ResultString, "No Data$"))
            {
                return false;
            }
            else
            {
                DataTable models = modelsDt.Data.Tables["Models"];
                foreach (DataRow row in models.Rows)
                    modelsList.Append($"{row["Models"]}:{row["Feature"]}|");

                LMWholesale.WholesaleSystem.PopulateList(modelsList.ToString(), "-- Select Model --", "ModelLst", '|');
            }
            return true;
        }

        public void PopulateStyles(string kSession, string year, string make, string model)
        {
            StringBuilder stylesList = new StringBuilder();
            lmReturnValue stylesDt = Self.dasClient.DASGetMotorStyles(kSession, "3", year, make, model);
            if (stylesDt.Result == ReturnCode.LM_SUCCESS)
            {
                DataTable styles = stylesDt.Data.Tables["Styles"];
                foreach (DataRow row in styles.Rows)
                    stylesList.Append($"{row["StyleID"]}:{row["Styles"]}|");
            }

            LMWholesale.WholesaleSystem.PopulateList(stylesList.ToString(), "-- Select Style --", "StyleLst", '|');
        }

        public string GetMakeInfo(string kSession, string sender, bool returnItem = false)
        {
            Lookup.lmReturnValue tblMakes = Self.lookupClient.GetMakeList(kSession);
            if (tblMakes.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                StringBuilder lstMake = new StringBuilder("[]:|");
                DataTable dt = tblMakes.Data.Tables[0];
                foreach (DataRow row in dt.Rows)
                    lstMake.Append($"{row["kMake"]}:{row["MakeDesc"]}|");

                if (!returnItem)
                    LMWholesale.WholesaleSystem.PopulateList(lstMake.ToString(), "", sender, '|');
                return lstMake.ToString();
            }

            // Default return string
            return "";
        }

        // For Add Make/Model
        public string GetModelInfoLst(string kSession, string kMake)
        {
            if (string.IsNullOrEmpty(kMake))
                return "";

            Lookup.lmReturnValue tblModels = Self.lookupClient.GetModelList(kSession, int.Parse(kMake));
            if (tblModels.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                StringBuilder lstModel = new StringBuilder("[]:|");
                DataTable dt = tblModels.Data.Tables[0];
                foreach (DataRow row in dt.Rows)
                    lstModel.Append($"{row["kModel"]}:{row["ModelDesc"]}|");

                return lstModel.ToString();
            }
            return "";
        }

        public string GetModelInfo(string kSession, string year, string make, string source)
        {
            StringBuilder m = new StringBuilder("[]:|");

            lmReturnValue MotorModels = Self.dasClient.DASGetMotorModels(kSession, source, year, make);
            if (MotorModels.Result == ReturnCode.LM_SUCCESS)
            {
                DataTable dt = MotorModels.Data.Tables[0];
                foreach (DataRow row in dt.Rows)
                    m.Append($"{row["Feature"]}:{row["Models"]}|");

                return m.ToString();
            }
            return "";
        }


        public string GetStyleInfo(string kSession, string year, string make, string model, string source)
        {
            StringBuilder s = new StringBuilder("[]:|");

            lmReturnValue MotorStyles = Self.dasClient.DASGetMotorStyles(kSession, source, year, make, model);
            if (MotorStyles.Result == ReturnCode.LM_SUCCESS)
            {
                DataTable styles = MotorStyles.Data.Tables["Styles"];
                foreach (DataRow row in styles.Rows)
                    s.Append($"{row["StyleID"]}:{row["Styles"]}|");
            }

            return s.ToString();
        }

        public DataTable GetStyleInfo(string kSession, string year, string make, string model, string source, bool isDT = true)
        {

            lmReturnValue MotorStyles = Self.dasClient.DASGetMotorStyles(kSession, source, year, make, model);
            if (MotorStyles.Result == ReturnCode.LM_SUCCESS)
            {
                DataTable styles = MotorStyles.Data.Tables["Styles"];
                return styles;
            }

            return new DataTable();
        }

        public bool AddMakeModel(string kSession, int kMake, string Make, string Model)
        {
            Listing.lmReturnValue results = Self.listingClient.AddMakeModel(kSession, kMake, Make, Model);
            if (results.Result == Listing.ReturnCode.LM_SUCCESS)
                return true;
            return false;
        }

        public Tuple<int, string> PopulateAuctionInfo(DataTable dt, string Auction)
        {
            Regex success = new Regex(@"success", RegexOptions.IgnoreCase);
            Dictionary<string, string> auctionStatus = new Dictionary<string, string>
            {
                { "0", "Not Listed" },
                { "1", "Scheduled" },
                { "2", "Active" },
                { "3", "Cancelling" },
                { "4", "Pending" },
            };

            #region Format Auction info
            string info = "";
            bool first = true;
            int selectedAuctionNumber = -1;
            int counter = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (int.Parse(dr["Status"].ToString(), 0) == 0)
                    continue;

                int mmr = int.Parse(dr["MMRPrice"].ToString(), 0);
                string mmrReservePrice = "% MMR: N/A";
                string mmrBuyNowPrice = "% MMR: N/A";

                if (mmr > 0)
                {
                    mmrReservePrice = $"{Math.Ceiling(int.Parse(dr["ReservePrice"].ToString(), 0) * 100.00 / mmr)}% of MMR";
                    mmrBuyNowPrice = $"{Math.Ceiling(int.Parse(dr["BuyNow"].ToString(), 0) * 100.00 / mmr)}% of MMR";
                }

                string al = dr["AutoLaunch"].ToString() == "1" ? "True" : "False";
                string bow = dr["IsBlackOut"].ToString() == "1" ? "Yes" : "No";

                string auctionName = dr["AuctionName"].ToString();
                if (dr["AuctionName"].ToString().Contains("OVE"))
                    auctionName = "OVE";
                if (dr["AuctionName"].ToString() == "Integrated Auction Solutions")
                    auctionName = "IAS";
                if (dr["AuctionName"].ToString() == "OpenLane")
                    auctionName = "ADESA";
                if (dr["AuctionName"].ToString() == "RemarketingPlus")
                    auctionName = "Remarketing+";

                string err = dr["ErrorMsg"].ToString() == "" || success.IsMatch(dr["ErrorMsg"].ToString()) ? "N/A" : dr["ErrorMsg"].ToString();

                string className = "auctionInfo";
                if (string.IsNullOrEmpty(Auction))
                {
                    className = first ? "auctionInfo openAuctionInfo" : "auctionInfo";
                    first = false;
                }
                else
                {
                    string tempAuctionName = auctionName.Replace(" ", "");
                    Auction = Auction.Replace("Integrated Auction Solutions", "IAS").Replace("OpenLane", "ADESA");
                    if (tempAuctionName == Auction)
                    {
                        className = "auctionInfo openAuctionInfo";
                        selectedAuctionNumber = counter;
                    }
                    else
                    {
                        className = "auctionInfo";
                        counter++;
                    }
                }

                string listingCategory = dr["ListingCategory"].ToString() == "" ? "N/A" : dr["ListingCategory"].ToString();
                info += $@"
                    <div style='display: table; margin: 0 auto;width:100%;'>
                        <div class='singleRow' onclick=""toggleCssClass([['auctionInfo_{dr["kWholesaleAuction"]}','openAuctionInfo']]);"">
                            <div class='auctionTableHeader' style='cursor:pointer;'>{auctionName}</div>
                        </div>
                    </div>

                    <div id='auctionInfo_{dr["kWholesaleAuction"]}' class='{className}'>
                        <div style='display: table; margin: 0 auto;width:100%;'>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Start Date: </div>
                                <div class='tableCell'><span>{dr["StartDate"]}</span></div>
                            </div>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>End Date: </div>
                                <div class='tableCell'><span>{dr["EndDate"]}</span></div>
                            </div>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Posted Date: </div>
                                <div class='tableCell'><span>{dr["Posted"]}</span></div>
                            </div>
                        </div>
                        <div style='display:table;margin: 0 auto;width:100%;'>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Status: </div>
                                <div class='tableCell'><span>{auctionStatus[dr["Status"].ToString()]}</span></div>
                            </div>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>AutoLaunch: </div>
                                <div class='tableCell'><span>{al}</span></div>
                            </div>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Relist Count: </div>
                                <div class='tableCell'><span>{dr["RelistCount"]}</span></div>
                            </div>
                        </div>
                        <div style='display:table;margin: 0 auto;width:100%;'>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Condition Report Date: </div>
                                <div class='tableCell'><span>{dr["ConditionReportDate"]}</span></div>
                            </div>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>BlackOut Window: </div>
                                <div class='tableCell'><span>{bow}</span></div>
                            </div>
                        </div>
                        <div style='display: table; margin: 0 auto;width:100%;'>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Start Price: </div>
                                <div class='tableCell'><span>${dr["StartPrice"]}</span></div>
                                <div class='tableCell'><span>MMR: ${mmr}</span></div>
                            </div>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Reserve Price: </div>
                                <div class='tableCell'><span>${dr["ReservePrice"]}</span></div>
                                <div class='tableCell'><span>{mmrReservePrice}</span></div>
                            </div>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Buy Now: </div>
                                <div class='tableCell'><span>${dr["BuyNow"]}</span></div>
                                <div class='tableCell'><span>{mmrBuyNowPrice}</span></div>
                            </div>
                        </div>
                        <div style='display:table;margin: 0 auto;width:100%;'>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Listing Type: </div>
                                <div class='tableCell'><span>{dr["ListingType"]}</span></div>
                            </div>
                            <div class='HeaderCol'>
                                <div class='tableHeader'>Listing Category: </div>
                                <div class='tableCell'><span>{listingCategory}</span></div>
                            </div>
                        </div>
                        <div style='display:table;margin: 0 auto;width:100%;'>
                            <div class='HeaderCol'>
                                <div class='tableHeader'><span>Listing Error Message:</span></div>
                                <div class='tableCell'><span>{err}</span></div>
                            </div>
                        </div>
                    </div>";
            }
            #endregion

            return new Tuple<int, string>(selectedAuctionNumber, info);
        }

        public string PopulateVehicleNotes(string kSession, int kListing)
        {
            Listing.lmReturnValue results = Self.listingClient.ListingVehicleNotesGet(kSession, kListing);

            string notes = @"
                <div style='display: table; margin: 0 auto;width:100%;'>
                    <div class='singleRow'>
                        <div class='tableHeader'>User/Date/Note: </div>
                    </div>
                    VEHICLENOTES
                </div>";

            string record = "";
            if (results.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = results.Data.Tables[0];

                if (dt.Rows.Count == 0)
                {
                    record += $@"
                        <div class='singleRow'>
                            <div class='tableCell'><span style='text-decoration:underline;'>N/A</span></div>
                        </div>";
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime noteTime = Convert.ToDateTime(dr["NoteDate"].ToString());
                        record += $@"
                        <div class='singleRow'>
                            <div class='tableCell'><div style='display:table;width:100%;'><span class='HeaderCol' style='text-decoration:underline;width:25%;'>{dr["User"]}</span><span class='HeaderCol' style='width:25%'>{noteTime.ToString("MM/dd/yyyy hh:mm:ss tt")}</span><span class='HeaderCol' style='width:50%;'>{dr["Note"]}</span></div></div>
                        </div>";
                    }
                }
            }

            return notes.Replace("VEHICLENOTES", record);
        }

        public Dictionary<string, object> BuildPhotoGalleryHtml(string kSession, int kListing)
        {
            Dictionary<string, object> returnHtmlDict = new Dictionary<string, object>
            {
                { "slider", "" },
                { "carousel", "" },
                { "photoCnt", 0 },
                { "firstPhoto", "" }
            };

            string slider = "<ul id='lightgallery' class='slides' style='width: 1200%; transition-duration: 0s; transform: translate3d(0px, 0px, 0px);'>REPLACE_ME</ul>";
            //string carousel = "<ul class='slides' style='width: 1200%; transition-duration: 0s; transform: translate3d(0px, 0px, 0px);'>REPLACE_ME</ul>";
            StringBuilder sliderPics = new StringBuilder();
            StringBuilder carouselPics = new StringBuilder();
            int count = 1;

            Listing.lmReturnValue photos = Self.listingClient.ListingPhotosGet(kSession, kListing);
            if (photos.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                DataRowCollection rows = photos.Data.Tables[0].Rows;
                foreach (DataRow dr in rows)
                {
                    if (returnHtmlDict["firstPhoto"].ToString() == "")
                        returnHtmlDict["firstPhoto"] = dr["BaseURL"].ToString() + dr["LocationWM"].ToString();
                    string active = count == 1 ? "class='flex-active-slide'" : "";
                    sliderPics.Append($"<li {active} data-src='{dr["BaseURL"]}{dr["LocationWM"]}' style='width: 300px; float: left; display: block;'><a><img itemprop='image' src='{dr["BaseURL"]}{dr["LocationWM"]}' draggable='false'></a></li>");
                    //carouselPics.Append($"<li {active} style='width: 140px; float: left; display: block;'><img itemprop='image' src='{dr["BaseURL"]}{dr["LocationThumb"]}' draggable='false'></li>");
                    count++;
                }

                returnHtmlDict["slider"] = slider.Replace("REPLACE_ME", sliderPics.ToString());
                //returnHtmlDict["carousel"] = carousel.Replace("REPLACE_ME", carouselPics.ToString());
                returnHtmlDict["photoCnt"] = count;
            }
            else
                returnHtmlDict["photoCnt"] = 0;

            return returnHtmlDict;
        }

        public int ListingVehicleNoteSet(string kSession, Dictionary<string, object> noteInfo)
        {
            int kVehicleNoteId = int.Parse(noteInfo["kVehicleNoteId"].ToString(), 0);
            int kListing = int.Parse(noteInfo["kListing"].ToString(), 0);
            string noteString = noteInfo["noteString"].ToString();

            Listing.lmReturnValue returnValue = Self.listingClient.ListingVehicleNotesSet(kSession, kListing, kVehicleNoteId, noteString);
            if (returnValue.Result == Listing.ReturnCode.LM_SUCCESS)
                return 1;

            return 0;
        }

        public int ListingDetailSet(string kSession, Dictionary<string, object> vehicleInfo)
        {
            int kListing = int.Parse(vehicleInfo["kListing"].ToString(), 0);
            int listPrice = int.Parse(vehicleInfo["listPrice"].ToString(), 0);
            int internetPrice = int.Parse(vehicleInfo["internetPrice"].ToString(), 0);
            int fastQuote = int.Parse(vehicleInfo["fastQuote"].ToString(), 0);
            int inventoryStatus = int.Parse(vehicleInfo["inventoryStatus"].ToString(), 0);
            string lotLocation = vehicleInfo["lotLocation"].ToString();

            Listing.lmReturnValue result =
                Self.listingClient.ListingDetailSet(kSession, kListing, listPrice, internetPrice, inventoryStatus, fastQuote, lotLocation, 1);

            if (result.Result == Listing.ReturnCode.LM_SUCCESS)
                return 1;

            return 0;
        }

        public bool ExpandedDetailSave(string kSession, string JsonData, ref string errorMsg)
        {
            Dictionary<string, object> json = (Dictionary<string, object>)Util.serializer.DeserializeObject(JsonData);

            Listing.lmReturnValue result = Self.listingClient.WPUpdateSet(kSession, Util.serializer.Serialize(json));

            if (result.Result == Listing.ReturnCode.LM_SUCCESS)
                return true;

            errorMsg = result.ResultString;
            return false;
        }

        public Listing.lmReturnValue UpdateGet(int kListing, string kSession)
        {
            return Self.listingClient.UpdateGet(kSession, kListing);
        }

        public bool VehicleClassUpdate(string kSession, string kListing, string kVehicleClass)
        {
            Listing.lmReturnValue vc = Self.listingClient.VehicleClassSet(kSession, kListing, kVehicleClass);
            if (vc.Result == Listing.ReturnCode.LM_SUCCESS)
                return true;

            return false;
        }

        public Lookup.lmReturnValue GetListingOptionList(string kSession, int kListing)
        {
            return Self.lookupClient.GetListingOptionList(kSession, kListing);
        }

        public string CertificationListGet(string kSession)
        {
            StringBuilder list = new StringBuilder("[]:|");

            Lookup.lmReturnValue lstCert = Self.lookupClient.CertificationListGet(kSession);
            if (lstCert.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                DataTable certs = lstCert.Data.Tables["CertificationList"];
                foreach (DataRow row in certs.Rows)
                    list.Append($"{row["kCertification"]}:{row["CertificationName"]}|");
            }

            return list.ToString();
        }

        #region HelperMethods
        public string GetkValue(string configString, string selectedVal)
        {
            foreach (string pair in configString.Substring(configString.IndexOf("]") + 1).Split('|'))
            {
                string[] value = pair.Split(':');
                if (value[1] == selectedVal)
                    return value[0];
            }

            return "";
        }

        public string FormatOptions(DataRow[] rows, string type)
        {
            string rowHtml = "<div class='singleRow'>";
            int count = 1;
            int groupCount = 1;

            foreach (DataRow dr in rows)
            {
                string name = $"UpdateOptions{count}{type}_grp{groupCount}_chkboxLst{count}";
                string isChecked = dr["FeatureSelected"].ToString() == "1" ? "checked='checked'" : "";
                string checkBox = $"<div class='centerContent'><span style='white-space:normal;'><input ID='{name}' type='checkbox' {isChecked} value='{dr["kFeature"]}'></input>&nbsp;<label for='{name}'>{dr["FeatureDesc"]} OPTIONCODE</label></span></div>";

                checkBox = checkBox.Replace("OPTIONCODE", type == "vin" ? $"({dr["OptionCode"]})" : "");

                if (count == 4)
                {
                    rowHtml += checkBox;
                    rowHtml += $"</div><div class='singleRow'>";
                    count = 1;
                    groupCount++;
                }
                else
                {
                    rowHtml += checkBox;
                    count++;
                }
            }

            rowHtml += "</div>";

            return rowHtml;
        }

        public string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (!inside && let == '<')
                {
                    inside = true;
                    continue;
                }
                if (inside && let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
        #endregion HelperMethods
    }
}