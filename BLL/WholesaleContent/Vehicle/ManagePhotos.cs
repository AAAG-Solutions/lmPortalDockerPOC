using LMWholesale.Listing;
using LMWholesale.resource.clients;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class ManagePhotos
    {
        private readonly ListingClient listingClient;
        public ManagePhotos()
        {
            listingClient = listingClient ?? new ListingClient();
        }

        public ManagePhotos(ListingClient listingClient)
        {
            this.listingClient = listingClient;
        }

        public string[] BuildPhotoCards(string kSession, int kListing)
        {
            Listing.lmReturnValue photos = listingClient.ListingPhotosGet(kSession, kListing);
            if (photos.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                string vehPhotoReturn = "";
                string damPhotoReturn = "";
                int count = 0;
                foreach (DataRow dr in photos.Data.Tables[0].Rows)
                {
                    string thPhotos = $"{dr["BaseURL"]}v2/ds1/szTH/po{dr["PhotoOrder"]}/pic.aspx";
                    string idName = "photoCard|" + dr["kPhoto"];
                    if (Convert.ToInt32(dr["PhotoType"].ToString()) == 4 && Convert.ToInt32(dr["PhotoOrder"]) > 999)
                    {
                        string photoCard = $@"
                            <div id=""{idName}"" class=""photoCard"">
                                <img src=""{thPhotos}"" class=""photoCardImg""></img>
                                <div class=""photoActions"">
                                    <button class=""submitButton smallButton"" onclick=""HidePhotoCard('{idName}'); return false;"" style=""width: 100%;"">Remove</button>
                                </div>
                            </div>
                        ";
                        damPhotoReturn += photoCard;
                    }
                    else if (Convert.ToInt32(dr["PhotoType"].ToString()) == 0 && Convert.ToInt32(dr["PhotoOrder"]) > 0)
                    {
                        string upClass = count == 0 ? "arrow hideArrow" : "arrow";
                        string dwnClass = count == photos.Data.Tables[0].Rows.Count - 1 ? "arrow hideArrow" : "arrow";
                        string photoCard = $@"
                            <div id=""{idName}"" class=""photoCard"">
                                <img src=""{thPhotos}"" class=""photoCardImg""></img>
                                <div class=""photoActions"">
                                    <img id=""{idName + "UpArrow"}"" src=""/Images/chevron-up.svg"" class=""{upClass}"" onclick=""IncreasePhotoOrder('{idName}'); return false;"" />
                                    <button class=""submitButton smallButton"" onclick=""HidePhotoCard('{idName}'); return false;"" style=""width: 100%;"">Remove</button>
                                    <img id=""{idName + "DwnArrow"}""  src=""/Images/chevron-down.svg"" class=""{dwnClass}"" onclick=""DecreasePhotoOrder('{idName}'); return false;"" />
                                </div>
                            </div>
                        ";
                        vehPhotoReturn += photoCard;
                        count++;
                    }
                }

                return new string[2] { vehPhotoReturn, damPhotoReturn };
            }
            return new string[2] { "", "" };
        }

        public Dictionary<string, object> SavePhotoChanges(string kSession, string kListing, string photoList)
        {
            lmReturnValue result = listingClient.ReorderPhotos(kSession, kListing, photoList);
            if (result.Result == ReturnCode.LM_SUCCESS)
            {
                return new Dictionary<string, object>()
                {
                    { "Success", true }
                };
            }
            else
            {
                return new Dictionary<string, object>()
                {
                    { "Success", false },
                    { "ErrorMessage", result.ResultString }
                };
            }
        }
    }
}