using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using LMWholesale.Common;

namespace LMWholesale
{
    public partial class Login : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;

        public Login() => userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();

        public static Login Self
        {
            get { return instance; }
        }
        private static readonly Login instance = new Login();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Login";

            // This is to check to make sure if a new user attempts to hit wholesaleportal while 'live' maintenance is happening,
            // we just show the Maintenance page
            // Little hack to prevent webservice calls while isolating wholesaleportal vs portal for daytime deployments
            bool maint = bool.Parse(WebConfigurationManager.AppSettings["isMaintenance"]);
            if (maint)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Response.Redirect("/Maintenance.html", true);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["clearCache"]))
            {
                HttpContext.Current.Response.Redirect("/WholesaleSystem/CachePop.aspx", true);
            }

            Control masterPageHeader = Page.Master.FindControl("WholesalePortalHeader");
            if (masterPageHeader != null)
            {
                masterPageHeader.Visible = false;
            }
            Control loadingMask = Page.Master.FindControl("loadingHolder");
            if (loadingMask != null)
            {
                ((HtmlGenericControl)loadingMask).Attributes["class"] = "loadingHolderLogin";
            }
            Control MainBody = Page.Master.FindControl("MainBody");
            if (MainBody != null)
            {
                ((HtmlGenericControl)MainBody).Attributes["class"] = "page-wrapper-login";
            }

            if (!IsPostBack)
            {
                string err = Request.QueryString["e"];
                switch (err)
                    {
                    case "0":
                        break;
                    case "1":
                        lblMsg.Text = "Your Session is expired.";
                        break;
                    case "2":
                        lblMsg.Text = "Your Credentials do not match.";
                        break;
                    case "3":
                        lblMsg.Text = "Your validation code is invalid.";
                        break;
                    case "4":
                        lblMsg.Text = "Invalid Login.";
                        break;
                    default:
                        lblMsg.Text = err;
                        break;
                    }
            }

            // Add Maintenace Title and Message if there is a value
            MaintenanceTitle.Text = Util.GetRegistryString("MaintTitle", "portal");
            MaintenanceMessage.Text = Util.GetRegistryString("MaintMsg", "portal");
        }

        [WebMethod]
        public static Dictionary<string, object> UserLogin(string username, string password, string platform)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            // Validate if we are attempting to login without Username/Password
            if (String.IsNullOrEmpty(username))
            {
                Message = "Username is required";
                IsSuccess = false;
                return ReturnResponse();
            }
            if (String.IsNullOrEmpty(password))
            {
                Message = "Password is required";
                IsSuccess = false;
                return ReturnResponse();
            }

            string resetCode = Self.userBLL.PasswordCheck(username, password);
            if (!string.IsNullOrEmpty(resetCode))
            {
                Message = $@"<script>alert('Password has expired! Please update to a new password!');
                        window.location.href = ""/WholesaleSystem/PasswordReset.aspx?mode=0&ValCode={resetCode}"";</script>";
                IsSuccess = false;
                return ReturnResponse();
            }

            if (Self.userBLL.Login(username, password, platform, Session))
            {
                string errorMsg = "";
                string successMsg = "";
                bool success = Self.userBLL.GetDealersInfo(Session, ref successMsg, ref errorMsg, "");
                if (success)
                {
                    IsSuccess = true;
                    Value = successMsg;
                }
                else
                {
                    IsSuccess = false;
                    Message = errorMsg;
                }
            }
            else
            {
                Message = "Invalid Login";
                IsSuccess = false;
            }

            return ReturnResponse();
        }
    }
}