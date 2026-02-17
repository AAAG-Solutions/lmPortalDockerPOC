using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class Search
    {
        private readonly LookupClient lookupClient;

        public Search() => lookupClient = lookupClient ?? new LookupClient();

        public Search(LookupClient lookupClient) => this.lookupClient = lookupClient;

        internal static readonly Search instance = new Search();
        public Search Self
        {
            get { return instance; }
        }

        private static readonly Dictionary<int, string> ListingStatus = new Dictionary<int, string>
        {
            { 0, "Not Listed" },
            { 1, "Pending" },
            { 2, "Listed" },
            { 3, "De-Listing" },
            { 4, "In Transit" }
        };

        public string GetListingsByVIN(string kSession, string vin, string kDealer = null)
        {
            Lookup.lmReturnValue results = Self.lookupClient.GetListingsByVIN(kSession, vin);

            if (results.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                DataTable dt = results.Data.Tables["Listings"];

                foreach (DataRow row in dt.Rows)
                {
                    if (kDealer == null || (row["kDealer"].ToString() == kDealer))
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                            dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));

                        // Format Make/Model/Style to navigate to UpdateVehicle
                        string year = Util.cleanString((Convert.ToString(row["Year"])));
                        string makeName = Util.cleanString((Convert.ToString(row["Make"])));
                        string modelName = Util.cleanString((Convert.ToString(row["Model"])));

                        dict["YearMakeModel"] = $"{year} {makeName} {modelName}";

                        dict[""] = BuildActionMenu(row["kListing"].ToString(), row["kDealer"].ToString(), row["VIN"].ToString());
                        dict["ListingStatus"] = ListingStatus[int.Parse(row["ListingStatus"].ToString())];

                        list.Add(dict);
                    }
                }

                // We don't return total count due to pagination
                return $"0 | {Util.serializer.Serialize(list)}";
            }

            // Return empty if we fail for some reason
            return "0 | {}";
        }

        private static string BuildActionMenu(string kListing, string kDealer, string vin)
        {
            string returnValue = "";

            returnValue += $"<a title='edit' onclick='AssignkListing({kDealer}, {kListing});'><img title='Go To Details' src='/Images/fa-icons/edit.svg' class='smIcon' /></a>";
            returnValue += $"<a title='account' onclick='AssignkDealer({kDealer}, \"{vin}\");'><img title='Go To Account' src='/Images/fa-icons/car.svg' class='mdIcon' /></a>";

            return returnValue;
        }
    }
}