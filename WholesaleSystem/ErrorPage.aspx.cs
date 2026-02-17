using System;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.SessionState;

using System.Text;
using System.Collections.Generic;

namespace LMWholesale
{
    public partial class ErrorPage : Page
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;

        public ErrorPage()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
        }

        public static ErrorPage Self
        {
            get { return instance; }
        }
        private static readonly ErrorPage instance = new ErrorPage();

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            Dictionary<string, string> exObject = (Dictionary<string, string>)Session["ExceptionObject"];

            SessionId.Text = $"&nbsp;{Session["kSession"]}";
            UserName.Text = $"&nbsp;{Session["fullname"]}";
            PageName.Text = $"&nbsp;{exObject["ErrorPageName"]}";
            Source.Text = $"&nbsp;{exObject["ErrorSource"]}";
            Message.Text = $"&nbsp;{exObject["ErrorMessage"]}";

            // Build Exception log ling
            StringBuilder ex  = new StringBuilder("Exception - [ ");
            ex.Append($"Page - {exObject["ErrorPageName"]} |");
            ex.Append($"kPerson - {Session["kPerson"]} |");
            ex.Append($"kDealer - {Session["kDealer"]} |");
            ex.Append($"Message - {exObject["ErrorMessage"]} ]");

            // Write to log. First small info, then attempt to log full stack trace
            WholesaleSystem.Logger.LogLine(Session["kSession"].ToString(), ex.ToString());
            WholesaleSystem.Logger.LogLine(Session["kSession"].ToString(), $"Inner StackTrace - {exObject["ErrorInnerStackTrace"]}");
            WholesaleSystem.Logger.LogLine(Session["kSession"].ToString(), $"Outer StackTrace - {exObject["ErrorOuterStackTrace"]}\n\n");

            if (Self.userBLL.CheckPermission("LMIInternal") == false)
                lblStack.Visible = StackTrace.Visible = false;
            else
            {
                string stackTrace = !string.IsNullOrEmpty(exObject["ErrorInnerStackTrace"]) ? exObject["ErrorInnerStackTrace"] : "";
                if (stackTrace.Length > 0)
                    stackTrace = stackTrace.Replace("\r\n", "<br>&emsp;");
                StackTrace.Text = $"&nbsp;{stackTrace}";
            }

            // Once getting error, reset Session entry
            // just in case a new error comes a long as we server stale data
            Session["ExceptionObject"] = "";
        }
    }
}