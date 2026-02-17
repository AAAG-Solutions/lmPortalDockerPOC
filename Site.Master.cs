using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

using LMWholesale.BLL.WholesaleUser;

namespace LMWholesale
{
    public partial class SiteMaster : MasterPage
    {
        private readonly WholesaleUser userBLL;
        private readonly Dictionary<string, string> helpIconDict = new Dictionary<string, string> {
                { "AutoLaunchRules", "/WholesaleData/TrainingVideo.aspx?VideoName=AutoLaunchRules" }
            };

        public SiteMaster() => userBLL = userBLL ?? new WholesaleUser();

        public static SiteMaster Self
        {
            get { return instance; }
        }

        private static readonly SiteMaster instance = new SiteMaster();

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string Page = System.IO.Path.GetFileNameWithoutExtension(HttpContext.Current.Request.Url.AbsolutePath);

            string[] noSessionCheck = new string[3] { "Login", "PasswordReset", "CachePop" };
            if (Session["kSession"] == null && !noSessionCheck.Contains(Page))
                WholesaleUser.ClearUser("Invalid Session");

            // Here is the start of assigning User Permissions on a global scale because of how the life cycle of the page is for ASP.NET
            if (Session["kSession"] != null)
            {
                bool isInternal = Self.userBLL.CheckPermission("LMIInternal");
                HttpContext.Current.Items.Add("IsInternal", isInternal);

                showPreferences.Value = Self.userBLL.CheckPermission("preferences").ToString();
                internalFlag.Value = isInternal.ToString() == "" ? "False" : "True";
                gtagkDealer.Value = Session["kDealer"].ToString();
                gtagkPerson.Value = Session["kPerson"].ToString();
            }

            if (Page != "WholesaleDefault") {

                // Hide VinSearch anchor tag if on the same page
                if (Page == "Search")
                {
                    vinSearch.Visible = false;
                    vehicleSearch.Visible = false;
                }
                else if (Page == "SalesDataApproval" && Regex.IsMatch(Request.UrlReferrer.AbsolutePath, "WholesaleDefault.aspx$"))
                {
                    vinSearch.Visible = false;
                    vehicleSearch.Visible = false;
                    headerAccountName.Visible = false;
                }

                if (Session["kSession"] != null)
                {
                    if (Session["kDealer"] != null)
                    {
                        if (String.IsNullOrEmpty(Session["kDealer"].ToString()) || Session["kDealer"].ToString() == "-1")
                        {
                            lblHeaderDealerName.Visible = false;
                            headerAccountName.Visible = false;
                        }
                        else
                        {
                            string dealerName = Session["DealerName"].ToString();
                            string accountPrefix = "Account Name: ";
                            if (dealerName.Length > 38)
                            {
                                string headerName = dealerName.Substring(0, 37);
                                headerAccountName.Text = accountPrefix + headerName + "...";
                                headerAccountName.ToolTip = dealerName;
                            }
                            else
                            {
                                headerAccountName.Text = accountPrefix + dealerName;
                            }
                            lblHeaderDealerName.Text = dealerName;
                        }
                    }

                    // sanitize server names because we don't want to expose it to any witty customer
                    Identity.Text = Server.MachineName.Replace("PORTAL", "");

                    SessionID.Text = Session["kSession"].ToString();
                    AccountID.Text = Session["kDealer"].ToString();
                    UserID.Text = Session["kPerson"].ToString();
                    SourceBranch.Text = Util.GetRegistryString("SourceBranch", "portal");
                }
            }
            else
            {
                vinSearch.Visible = true;
                vehicleSearch.Visible = false;
                headerAccountName.Visible = false;
                dealerSelect.Visible = false;
                SessionID.Text = Session["kSession"].ToString();
                UserID.Text = Session["kPerson"].ToString();
                Identity.Text = Server.MachineName;
                SourceBranch.Text = Util.GetRegistryString("SourceBranch", "portal");
            }

            if (helpIconDict.ContainsKey(Page))
            {
                helpIcon.Style["display"] = "inline";
                HelpLink.Attributes["href"] = helpIconDict[Page];
                HelpLink.Target = "_blank";
            }
        }

        protected void Logout(object sender, CommandEventArgs e)
        {
            WholesaleUser.ClearUser("User Successfully Logged Out");
        }
    }
}