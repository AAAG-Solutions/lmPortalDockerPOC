using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class PhotoGallery
    {
        private readonly ListingClient listingClient;

        public PhotoGallery() => listingClient = listingClient ?? new ListingClient();
        internal static readonly PhotoGallery instance = new PhotoGallery();
        public PhotoGallery Self
        {
            get { return instance; }
        }

        public Dictionary<string, string> BuildPhotoGallery(string kSession, int kListing)
        {
            Dictionary<string, string> returnDict = new Dictionary<string, string>
            {
                { "slider", "" },
                { "carousel", "" }
            };

            string slider = "<ul id='lightgallery' class='slides' style='width: 1200%; transition-duration: 0s; transform: translate3d(0px, 0px, 0px);'>REPLACE_ME</ul>";
            string carousel = "<ul class='slides' style='width: 1200%; transition-duration: 0s; transform: translate3d(0px, 0px, 0px);'>REPLACE_ME</ul>";
            StringBuilder sliderPics = new StringBuilder();
            StringBuilder carouselPics = new StringBuilder();
            int count = 1;

            Listing.lmReturnValue photos = Self.listingClient.ListingPhotosGet(kSession, kListing);
            if (photos.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                DataRowCollection rows = photos.Data.Tables[0].Rows;
                foreach (DataRow dr in rows)
                {
                    string lgPhotos = $"{dr["BaseURL"]}v2/ds1/szLG/po{dr["PhotoOrder"]}/pic.aspx";
                    string thPhotos = $"{dr["BaseURL"]}v2/ds1/szTH/po{dr["PhotoOrder"]}/pic.aspx";
                    string active = count == 1 ? "class='flex-active-slide'" : "";

                    sliderPics.Append($"<li {active} data-src='{lgPhotos}' style='width: 600px; float: left; display: block;'><a><img id='PhotoItem{count}' itemprop='image' src='{thPhotos}' draggable='false'></a></li>");
                    carouselPics.Append($"<li {active} style='width: 140px; float: left; display: block;'><img itemprop='image' src='{thPhotos}' draggable='false'></li>");
                    count++;
                }
            }

            returnDict["slider"] = slider.Replace("REPLACE_ME", sliderPics.ToString());
            returnDict["carousel"] = carousel.Replace("REPLACE_ME", carouselPics.ToString());

            return returnDict;
        }
    }
}