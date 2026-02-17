using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LMWholesale.Common
{
	public class lmPage : Page
	{
		private string pageTitle = "";
		public string PageTitle
		{
			get { return pageTitle; }
			set
			{
				pageTitle = value;
				Label lblTitle = (Label)Master.FindControl("lblPageTitle");
				if (lblTitle != null)
					lblTitle.Text = pageTitle;
			}
		}

		public static bool IsSuccess { get; set; } = true;
		public static string Message { get; set; } = "";
		public static object Value { get; set; } = null;
		public static Dictionary<string, object> ReturnResponse()
		{
			return new Dictionary<string, object>
			{
				{ "success", IsSuccess },
				{ "message", Message },
				{ "value", Value }
			};
        }

		public static bool IsLiquidConnect()
		{
			string baseUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(8).Split('/')[0].ToLower();

            if (baseUrl.All(x => !char.IsLetter(x))||
				baseUrl.Contains("liquidconnect") ||
                baseUrl.Contains("lc"))
			{
				return true;
			}
			return false;
		}
    }
}