using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LMWholesale.resource.model.Wholesale
{
    public class InventoryFilter
    {
        public class Filter
        {
            public Filter() { }

            // Use default ItemsPerPage and PageNumber
            public Filter(string kSession, int kDealer)
            {
                this.kSession = kSession;
                this.kDealer = kDealer;
            }

            public Filter(string kSession, int kDealer, int PageNumber, int ItemsPerPage, string Sort)
            {
                this.kSession = kSession;
                this.kDealer = kDealer;
                this.PageNumber = PageNumber;
                this.ItemsPerPage = ItemsPerPage;
                this.Sort = Sort;
            }

            public Filter(Dictionary<string, object> filter)
            {
                PropertyInfo[] props = GetType().GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (p.PropertyType.Name == "Int32")
                        p.SetValue(this, int.Parse(filter[p.Name].ToString()));
                    else
                        p.SetValue(this, filter[p.Name].ToString());
                }
            }

            public string kSession { get; set; }
            public int kDealer { get; set; }
            public int PageNumber { get; set; } = 1;
            public int ItemsPerPage { get; set; } = 50;
            public string Sort { get; set; } = "";
            public string TextFilter { get; set; } = "";

            public Dictionary<string, string> Flatten()
            {
                Dictionary<string, string> returnFilter = new Dictionary<string, string>();
                PropertyInfo[] props = GetType().GetProperties();
                props.ToList().ForEach(p => { returnFilter.Add(p.Name, GetType().GetProperty(p.Name).GetValue(this, null).ToString()); });
                return returnFilter;
            }

            public object GetValue(string propName)
            {
                PropertyInfo prop = GetType().GetProperty(propName);
                if (prop != null)
                    return prop.GetValue(this, null);
                return null;
            }
        }

        public class AdvancedFilter {
            public int NoStyle { get; set; } = 0;
            public int NoDescription { get; set; } = 0;
            public int NoPhotos { get; set; } = 0;
            public int NoListPrice { get; set; } = 0;
            public int NoInternetPrice { get; set; } = 0;
            public string LotLocation { get; set; } = "ALL";
            public int ListingStatus { get; set; } = 0;
            public int InspectionStatus { get; set; } = -1;
            public int StatusAvailable { get; set; } = 0;
            public int StatusUnavailable { get; set; } = 0;
            public int StatusSalePending { get; set; } = 0;
            public int StatusInTransit { get; set; } = 0;
            public int StatusDemo { get; set; } = 0;
            public int StatusSold { get; set; } = 0;
            public int TypeDealerCertified { get; set; } = 0;
            public int TypeManufacturerCertified { get; set; } = 0;
            public int TypePreOwned { get; set; } = 0;

            // Default Constructor
            public AdvancedFilter() { }

            public AdvancedFilter(Dictionary<string, object> filter)
            {
                PropertyInfo[] props = GetType().GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (p.PropertyType.Name == "Int32")
                        p.SetValue(this, int.Parse(filter[p.Name].ToString()));
                    else
                        p.SetValue(this, filter[p.Name].ToString());
                }
            }

            public object GetValue(string propName)
            {
                PropertyInfo prop = GetType().GetProperty(propName);
                if (prop != null)
                    return prop.GetValue(this, null);
                return null;
            }

            public Dictionary<string, string> Flatten()
            {
                Dictionary<string, string> returnFilter = new Dictionary<string, string>();
                PropertyInfo[] props = GetType().GetProperties();
                props.ToList().ForEach(p => { returnFilter.Add(p.Name, GetType().GetProperty(p.Name).GetValue(this, null).ToString()); });
                return returnFilter;
            }
        }

        public class MultiPageFilter
        {
            public string kSession { get; set; }
            public int kDealer { get; set; }
            public int PageNumber { get; set; } = 1;
            public int ItemsPerPage { get; set; } = 50;
            public string Sort { get; set; } = "";
            public string LotLocation { get; set; } = "ALL";
            public int ListingStatus { get; set; } = 0;
            public int InspectionStatus { get; set; } = -1;
            public int StatusAvailable { get; set; } = 0;
            public int StatusInTransit { get; set; } = 0;
            public int TypeDealerCertified { get; set; } = 0;
            public int TypeManufacturerCertified { get; set; } = 0;
            public int TypePreOwned { get; set; } = 0;

            public MultiPageFilter() { }

            public MultiPageFilter(Dictionary<string, object> multiPageFilter)
            {
                PropertyInfo[] props = GetType().GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (p.PropertyType.Name == "Int32")
                        p.SetValue(this, int.Parse(multiPageFilter[p.Name].ToString()));
                    else
                        p.SetValue(this, multiPageFilter[p.Name].ToString());
                }
            }

            public MultiPageFilter(Filter filter, AdvancedFilter advancedFilter)
            {
                PropertyInfo[] props = GetType().GetProperties();
                // Filter
                foreach (PropertyInfo p in props)
                {
                    if (p.PropertyType.Name == "Int32" && filter.GetValue(p.Name) != null)
                        p.SetValue(this, int.Parse(filter.GetValue(p.Name).ToString()));
                    else if (filter.GetValue(p.Name) != null)
                        p.SetValue(this, filter.GetValue(p.Name).ToString());
                }

                // AdvancedFilter
                foreach (PropertyInfo p in props)
                {
                    if (p.PropertyType.Name == "Int32" && advancedFilter.GetValue(p.Name) != null)
                    {
                        if (p.Name == "ListingStatus"
                            && (int.Parse(advancedFilter.GetValue(p.Name).ToString()) == -2 || int.Parse(advancedFilter.GetValue(p.Name).ToString()) == -1))
                            p.SetValue(this, 0);
                        else
                            p.SetValue(this, int.Parse(advancedFilter.GetValue(p.Name).ToString()));
                    }
                    else if (advancedFilter.GetValue(p.Name) != null)
                        p.SetValue(this, advancedFilter.GetValue(p.Name).ToString());
                }
            }

            public Dictionary<string, string> Flatten()
            {
                Dictionary<string, string> returnFilter = new Dictionary<string, string>();
                PropertyInfo[] props = GetType().GetProperties();
                props.ToList().ForEach(p => { returnFilter.Add(p.Name, GetType().GetProperty(p.Name).GetValue(this, null).ToString()); });
                return returnFilter;
            }
        }
    }
}