using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Script.Serialization;

namespace LMWholesale
{
    public static class Extensions
    {
		public static object GetValue(this Dictionary<string,object> dict, string name, object defaultValue)
		{
			if (dict.ContainsKey(name))
			{
				return dict[name];
			}
			else
				return defaultValue;
		}

		public static string GetValue(this Authenticate.SerializableDictionary dict, string name, string defaultValue)
        {
            Authenticate.KeyValuePair kvp = dict.items.SingleOrDefault(r => String.Equals(r.Key, name, StringComparison.OrdinalIgnoreCase));
            if (kvp != null)
                return kvp.Value;

            return defaultValue;
        }

        public static string GetValue(this Wholesale.SerializableDictionary dict, string name, string defaultValue)
        {
            Wholesale.KeyValuePair kvp = dict.items.SingleOrDefault(r => String.Equals(r.Key, name, StringComparison.OrdinalIgnoreCase));
            if (kvp != null)
                return kvp.Value;

            return defaultValue;
        }

        public static string GetValue(this DAS.SerializableDictionary dict, string name, string defaultValue)
        {
            DAS.KeyValuePair kvp = dict.items.SingleOrDefault(r => String.Equals(r.Key, name, StringComparison.OrdinalIgnoreCase));
            if (kvp != null)
                return kvp.Value;

            return defaultValue;
        }

        public static string ToJSON(this DataTable table)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = (Convert.ToString(row[col]));
                }
                list.Add(dict);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(list);
        }

        public static string GridFilterResult(this DataTable dt, string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> oFilter = (Dictionary<string, object>)serializer.DeserializeObject(filter);
            int pagesize = int.Parse(oFilter["pageSize"].ToString());
            int pageindex = int.Parse(oFilter["pageIndex"].ToString());
            int firstitem = (pageindex - 1) * pagesize;

            // sortField, sortOrder
            if (oFilter.ContainsKey("sortField"))
            {
                string sort = oFilter["sortField"].ToString();
                if (oFilter.ContainsKey("sortOrder") && oFilter["sortOrder"].ToString().CompareTo("desc") == 0)
                    sort += " desc";

                dt.DefaultView.Sort = sort;
                dt = dt.DefaultView.ToTable();
            }
            else
                dt.DefaultView.Sort = "";


            string dtfilter = "";
            foreach (string key in oFilter.Keys)
            {
                if (dt.Columns.Contains(key) && oFilter[key].ToString().Length > 0)
                {
                    if (dtfilter.Length > 0)
                    {
                        dtfilter += " and ";
                    }
                    if (dt.Columns[key].DataType == typeof(int))
                    {
                        dtfilter += "Convert( " + key + ", 'System.String') like '%" + oFilter[key].ToString() + "%'";
                    }
                    else
                        dtfilter += key + " like '%" + oFilter[key].ToString() + "%'";
                }
            }

            var list = new List<Dictionary<string, object>>();
            DataRow[] rows;
            if (dtfilter.Length > 0) // we have a filter
            {
                rows = dt.Select(dtfilter);
            }
            else
            {
                rows = dt.AsEnumerable().ToArray();
            }

            for (int i = firstitem; i < firstitem + pagesize && i < rows.Length; i++)
            {
                DataRow row = rows[i];
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = (Convert.ToString(row[col]));
                }
                list.Add(dict);
            }

            string tmp = serializer.Serialize(list);
            return rows.Count().ToString() + "|" + tmp;

        }
    }
}