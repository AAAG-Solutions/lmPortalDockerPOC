using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class ViewNotes
    {
        private readonly ListingClient listingClient;

        public ViewNotes() => listingClient = listingClient ?? new ListingClient();

        public ViewNotes(ListingClient listingClient) => this.listingClient = listingClient;
        internal static readonly ViewNotes instance = new ViewNotes();
        public ViewNotes Self
        {
            get { return instance; }
        }

        public string ListingVehicleNotesGet(string kSession, int kDealer, int kListing)
        {
            string notes = @"<div style='margin:5px 10px;border:1px solid #999999;padding:10px;text-align:left;'>
                            DATE<br/>
                            USER<br/>
                            NOTE<br/>
                        </div>";
            string record = "";

            Listing.lmReturnValue results = Self.listingClient.ListingVehicleNotesGet(kSession, kListing);
            if (results.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = results.Data.Tables[0];

                if (dt.Rows.Count == 0)
                {
                    record += $@"<div style='margin:5px 10px;border:1px solid #999999;padding:10px;'>
                                    There are no notes to display for this vehicle!<br/>
                                </div>";
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        record += notes.Replace("DATE", dr["NoteDate"].ToString())
                                        .Replace("USER", dr["User"].ToString())
                                        .Replace("NOTE", dr["Note"].ToString());
                    }
                }
            }

            return record;
        }

        public DataRow ListingDetailGet(string kSession, int kDealer, int kListing)
        {
            Listing.lmReturnValue vehicleDetail = Self.listingClient.ListingDetailGet(kSession, kDealer, kListing, 1);
            if (vehicleDetail.Result == Listing.ReturnCode.LM_SUCCESS)
                return vehicleDetail.Data.Tables[0].Rows[0];

            return new DataTable().NewRow();
        }
    }
}