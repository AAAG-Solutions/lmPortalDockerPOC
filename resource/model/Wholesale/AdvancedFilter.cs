using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace LMWholesale.resource.model.Wholesale
{
    public class AdvancedFilter
    {
        // Default Constructor
        public AdvancedFilter() { }

        // Use default ItemsPerPage and PageNumber
        public AdvancedFilter(string kSession, int kDealer, int kPerson)
        {
            this.kSession = kSession;
            this.kDealer = kDealer;
            this.kPerson = kPerson;
        }

        public Dictionary<string, string> FormatToSession()
        {
            Dictionary<string, string> sessionInfo = new Dictionary<string, string>();
            string[] removeList = { "kSession", "kDealer", "kPerson" };

            PropertyInfo[] props = GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                if (removeList.Contains(p.Name))
                    continue;
                else
                    sessionInfo.Add(p.Name, GetType().GetProperty(p.Name).GetValue(this, null).ToString());
            }

            return sessionInfo;
        }

        public string kSession { get; set; }
        public int kDealer { get; set; }
        public int kPerson { get; set; }
        public int NoStyle { get; set; } = 0;
        public int NoDescription { get; set; } = 0;
        public int NoPhotos { get; set; } = 0;
        public int NoListPrice { get; set; } = 0;
        public int NoInternetPrice { get; set; } = 0;
        public int ListingStatus { get; set; } = 0;
        public int InspectionStatus { get; set; } = -1;
        public string LotLocation { get; set; } = "ALL";
        public int StatusAvailable { get; set; } = 0;
        public int StatusUnavailable { get; set; } = 0;
        public int StatusSalePending { get; set; } = 0;
        public int StatusInTransit { get; set; } = 0;
        public int StatusDemo { get; set; } = 0;
        public int StatusSold { get; set; } = 0;
        public int TypeDealerCertified { get; set; } = 0;
        public int TypeManufacturerCertified { get; set; } = 0;
        public int TypePreOwned { get; set; } = 0;

    }
}