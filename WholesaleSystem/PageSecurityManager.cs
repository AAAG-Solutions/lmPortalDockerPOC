using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using LMWholesale.BLL.WholesaleUser;

namespace LMWholesale
{
	public class PageSecurityManager
	{
		private readonly WholesaleUser userBLL;
		public PageSecurityManager() => userBLL = new WholesaleUser();

		public static PageSecurityManager Self
		{
			get { return instance; }
		}

		private static readonly PageSecurityManager instance = new PageSecurityManager();

		public static void DoPageSecurity(Page page)
		{
			GetUserSecurity();
			ApplySecurity(page);
		}

		private static void ApplySecurity(Page page)
		{
			DataTable dtSecurity = LoadSecurity(page);
			List<string> securities = GetUserSecurity();
			if (dtSecurity != null)
			{
				foreach (DataRow dr in dtSecurity.Rows)
				{
					Control ctrl = findbyName(dr["ControlName"].ToString(),page);
					if (ctrl != null && ctrl is WebControl)
					{
						WebControl wc = (WebControl)ctrl;

						string action = dr["Action"].ToString().ToLower();
						bool result = true;
						foreach (string security in dr["SecurityNames"].ToString().Split(new char[] { ',' }))
						{
							if (!securities.Contains(security))
							{
								result = false;
								break;
							}
						}

						if (action.CompareTo("disable") == 0)
						{
							wc.Enabled = result;
						}
						else if (action.CompareTo("hide") == 0)
						{
							wc.Visible = result;
						}
					}
				}

				if (!securities.Contains("lmiinternal"))
				{
					Control ctrl = findbyName("preferences_menu", page);
					//int a = 0;
				}
			}
		}

		private static List<string> GetUserSecurity()
		{
			List<string> result = new List<string>();
			var pst = HttpContext.Current.Session["UserPermissions"];
            if (pst == null)
            {
				Self.userBLL.GetPermissions();
				pst = HttpContext.Current.Session["UserPermissions"];
            }
            if(pst != null && pst is List<string>)
            {
                result = (List<string>)pst;
            }
			return result;
		}

		private static Control findbyName(string name, Control root)
		{
			Control ctrl = root.FindControl(name);
			if (ctrl != null)
			{
				return ctrl;
			}
			else
			{
				foreach (Control ctrl2 in root.Controls)
				{
					Control tmp = findbyName(name, ctrl2);
					if (tmp != null)
						return tmp;
				}
			}
			return null;
		}

		private static DataTable LoadSecurity(Page page)
		{
			DataTable result = null;
			DataTable holder = null;
			var pst = HttpContext.Current.Session["PageSecurityTable"];
			if (pst != null && pst is DataTable)
			{
				holder = (DataTable)pst;
			}
			else
			{
				// load from whatever source, local file for now
				DataSet ds = new DataSet();
                string tmp = page.Server.MapPath("~/WholesaleData/WholesaleSecurity.xml");
				ds.ReadXml(tmp);
				HttpContext.Current.Session["PageSecurityTable"] = ds.Tables[0];
				holder = ds.Tables[0].Clone();
			}
			result = holder.Clone();
			result.Clear();
			foreach (DataRow dr in holder.AsEnumerable().Where(r => r["Page"].ToString().ToLower().CompareTo(page.GetType().Name.ToLower()) == 0))
			{
				result.Rows.Add(dr.ItemArray);
			}
			return result;
		}
	}
}