using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using LMWholesale.Authenticate;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class ModifyPhotos
    {
        private readonly ListingClient listingClient;
        private readonly LookupClient lookupClient;

        public ModifyPhotos()
        {
            listingClient = listingClient ?? new ListingClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public ModifyPhotos(ListingClient listingClient, LookupClient lookupClient)
        {
            this.listingClient = listingClient;
            this.lookupClient = lookupClient;
        }

        public ModifyPhotos Self
        {
            get { return instance; }
        }
        internal static readonly ModifyPhotos instance = new ModifyPhotos();

        public string BuildPhotoList(string kSession, int kListing)
        {
            int count = 1;
            string photoList = "<ul id='ulPhotos'>";

            Listing.lmReturnValue photos = Self.listingClient.ListingPhotosGet(kSession, kListing);
            if (photos.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                DataRowCollection rows = photos.Data.Tables[0].Rows;
                foreach (DataRow dr in rows)
                {
                    photoList += string.Format("<li id='{1}' phototype='{3}'><div><img src='{0}'></div>{2}<div>{4}</div></li>", dr["ASPXPath"], dr["kPhoto"], BuildPhotoTag(kSession, dr["kPhoto"].ToString(), dr["kVehiclePhototag"].ToString(), dr["PhotoType"].ToString()), dr["PhotoType"], dr["PostedDisplay"].ToString().Replace("%20", " "));
                    count++;
                }
                photoList += "</ul>";
            }

            return photoList;
        }

        private string BuildPhotoTag(string kSession, string kPhoto, string kVehiclePhotoTag, string PhotoType)
        {
            string photoList = "";
            if (PhotoType.Equals("0") || PhotoType.Equals("4"))
            {
                Lookup.lmReturnValue tags = Self.lookupClient.PhotoTagListGet(kSession);
                if (tags.Result == Lookup.ReturnCode.LM_SUCCESS)
                {
                    DataTable dt = tags.Data.Tables[0];
                    // onchange='tagPhoto(\"{0}\");'
                    photoList = string.Format("<select id='ddl_{0}' name='ddl_{0}' >", kPhoto);
                    photoList += "<option value='0'>Not Set</option>";

                    foreach (DataRow dr in dt.Rows)
                    {
                        photoList += "<option ";
                        if (dr["kVehiclePhotoTag"].ToString().Equals(kVehiclePhotoTag))
                            photoList += "selected='selected'";
                        photoList += " value='" + dr["kVehiclePhotoTag"].ToString() + "'>" + dr["TagDescription"].ToString() + "</option>";
                    }
                    photoList += "</select>";
                }
                else if (tags.Result == Lookup.ReturnCode.LM_INVALIDSESSION)
                    WholesaleUser.WholesaleUser.ClearUser();

            }
            return photoList;
        }
    }
}