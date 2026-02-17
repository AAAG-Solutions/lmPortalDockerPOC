using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace LMWholesale.BLL.WholesaleContent
{
    public class WholesaleDefault
    {
		// Default Constructor
		public WholesaleDefault() { }

        public string BuildDealerSelection(Dictionary<string, object> filter)
        {
            HttpSessionState session = HttpContext.Current.Session;

            bool pageChanged = !(filter.ContainsKey("AccountRep"));
            string AccountStatusFilter = filter.GetValue("AccountStatus", "Account Status").ToString();
            string CustomerTypeFilter = filter.GetValue("CustomerType", "Customer Type").ToString();
            string AccountGroupFilter = filter.GetValue("AccountGroup", "Account Group").ToString();
            string PersonOwnerFilter = filter.GetValue("AccountRep", "Account Rep").ToString();
            string DealerFilter = filter.GetValue("DealerName", "").ToString();

			string dtfilter = "";
			if (AccountStatusFilter.CompareTo("Account Status") != 0)
			{
				dtfilter = AppendClause(dtfilter, "AccountStatus = '" + AccountStatusFilter + "'");
			}
			if (CustomerTypeFilter.CompareTo("Customer Type") != 0)
			{
				dtfilter = AppendClause(dtfilter, "CustomerTypeDesc = '" + CustomerTypeFilter + "'");
			}
			if (AccountGroupFilter.CompareTo("Account Group") != 0)
			{
				if (AccountGroupFilter == "")
					dtfilter = AppendClause(dtfilter, "Isnull(DealerGaggleName,'Null Col') = 'Null Col'");
				else
					dtfilter = AppendClause(dtfilter, "DealerGaggleName = '" + AccountGroupFilter + "'");
			}
			if (PersonOwnerFilter.CompareTo("Account Rep") != 0)
			{
				dtfilter = AppendClause(dtfilter, "PersonOwner = '" + PersonOwnerFilter + "'");
			}
			if (DealerFilter.Length > 0)
			{
				dtfilter = AppendClause(dtfilter, "(DealerName like '%" + DealerFilter + "%' or Convert(kDealer, System.String) like '%" + DealerFilter + "%')");
			}
			if (dtfilter.Length == 0 && pageChanged)
			{
				dtfilter = (session["DealerFilter"] ?? "").ToString();
			}
			else
			{
				session["DealerFilter"] = dtfilter;
			}

			int pagesize = int.Parse(filter["pageSize"].ToString());
            int pageindex = int.Parse(filter["pageIndex"].ToString());

			DataTable dt = (DataTable)session["dsDealers"];
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			int returnCount = 0;
			if (dt.Rows.Count > 0)
			{
				if (dtfilter.Length > 0)
					dt.DefaultView.RowFilter = dtfilter;
				else
					dt.DefaultView.RowFilter = "";

				int firstitem = (pageindex - 1) * pagesize;
				int lastitem = Math.Min(dt.DefaultView.Count, firstitem + pagesize);

				for (int i = firstitem; i < lastitem; i++)
				{
					DataRow row = dt.DefaultView[i].Row;
					var dict = new Dictionary<string, object>();
					foreach (DataColumn col in dt.Columns)
					{
						dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));
					}
					string kDealer = Util.cleanString((Convert.ToString(row[0])));

					dict[""] = $"<a title='Vehicle Management' onclick='javascript:fnGoToWholesale({kDealer});'><img src='/Images/fa-icons/car.svg' style=\"height: 40px; width: 40px;\"/>";
					list.Add(dict);
				}
				returnCount = dt.DefaultView.Count;
			}

			return $"{returnCount}|{Util.serializer.Serialize(list)}";
        }

        public Dictionary<string, string> ExportInventory(DataTable dt, bool isInternal, string Rep, string GroupName)
        {
            string stringDate = DateTime.Now.ToString("yyyyMMddTHHmmss");
            StringBuilder header = new StringBuilder();
            StringBuilder content = new StringBuilder();
            StringBuilder sb = new StringBuilder();

			List<string> filters = new List<string>();
            string fileName = $"GROUP_REP_AccountExport_{stringDate}.csv";

            // Just get general user VehicleManagement Columns
            string WholesaleGridColumns = "kDealer:Account ID|PersonOwner:Account Rep|DealerName:Account Name|DealerAddress1:Account Address 1|DealerAddress2:Account Address 2|DealerCity:Account City|DealerState:Account State|DealerZip:Account Zip|ContactName:Dealer Contact Name|ContactPhone:Dealer Contact Phone|ContactEmail:Dealer Contact Email";

			// Rep formatting
			if (Rep == "Account Rep" || Rep == "")
				fileName = fileName.Replace("REP", "AllAccountReps");
			else
			{
				filters.Add($"PersonOwner = '" + Rep + "'");
				fileName = fileName.Replace("REP", Rep.Replace(" ", ""));
				WholesaleGridColumns = WholesaleGridColumns.Replace("PersonOwner:Account Rep|", "");
			}

			// Group formatting
			if (Rep == "Account Group" || Rep == "")
			{
				if (!isInternal)
					fileName = fileName.Replace("GROUP", "AllGroups");
				else
					fileName = fileName.Replace("GROUP_", "");
			}
			else
			{
				filters.Add($"DealerGaggleName = '" + GroupName + "'");
				fileName = fileName.Replace("GROUP", GroupName.Replace(" ", ""));
			}

			if (filters.Count > 0)
				dt.DefaultView.RowFilter = String.Join(" OR ", filters.ToArray());

            IEnumerable<string[]> lstColumns = WholesaleGridColumns.Split('|').Select(column => column.Split(':'));
            int count = 0;

            foreach (DataRow dr in dt.DefaultView.ToTable().Rows)
            {
                foreach (string[] column in lstColumns)
                {
                    if (column[1] == "")
                        column[1] = column[0];

                    string value = dr[column[0]].ToString();

                    content.Append(Util.CreateCSV(value) + ",");
                    if (count < lstColumns.Count())
                    {
                        header.Append(column[1] + ",");
                        count += 1;
                    }
                }

                content.Remove(content.Length - 1, 1);
                content.AppendLine();
            }

            // Combine header and content
            sb.Append(header.ToString());
            sb.AppendLine();
            sb.Append(content.ToString());

            Dictionary<string, string> rv = new Dictionary<string, string>
            {
                { "fileName", fileName },
                { "sb", sb.ToString() }
            };

            return rv;
        }

        private static string AppendClause(string currentvalue, string clause, string clausejoiner = " and ")
		{
			if (currentvalue.Length == 0)
				return clause;
			else
				return currentvalue + clausejoiner + clause;
		}
	}
}