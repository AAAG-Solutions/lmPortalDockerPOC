using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;

using LMWholesale.Common;
using LMWholesale.resource.clients;


namespace LMWholesale.WholesaleContent
{
    public partial class AccountPreferences : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        //private readonly BLL.WholesaleContent.Preferences.Preferences BLL;
        private readonly DASClient dasClient;
        private readonly ListingClient listingClient;
        private readonly LookupClient lookupClient;
        private readonly WholesaleClient wholesaleClient;

        public AccountPreferences()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            //BLL = BLL ?? new BLL.WholesaleContent.Preferences.Preferences();
            //dasClient = dasClient ?? new DASClient();
            //listingClient = listingClient ?? new ListingClient();
            //lookupClient = lookupClient ?? new LookupClient();
            //wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }

        public static AccountPreferences Self
        {
            get { return instance; }
        }
        private static readonly AccountPreferences instance = new AccountPreferences();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Preferences";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);
            HttpSessionState Session = HttpContext.Current.Session;

            if (!IsPostBack)
            {
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];

                bool isInternal = Self.userBLL.CheckPermission("LMIInternal");
                if (!isInternal)
                {
                    MarketPlaceInfo.Style["display"] = "none";
                    //BlackoutWindows.Style["display"] = "none";
                }
            }
        }

    }
}