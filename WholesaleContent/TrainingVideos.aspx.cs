using System;
using System.Web;
using System.Web.SessionState;

using LMWholesale.Common;
using LMWholesale.resource.clients;


namespace LMWholesale.WholesaleContent
{
    public partial class TrainingVideos : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        //private readonly BLL.WholesaleContent.Preferences.Preferences BLL;
        private readonly DASClient dasClient;
        private readonly ListingClient listingClient;
        private readonly LookupClient lookupClient;
        private readonly WholesaleClient wholesaleClient;

        public TrainingVideos()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            //BLL = BLL ?? new BLL.WholesaleContent.Preferences.Preferences();
            //dasClient = dasClient ?? new DASClient();
            //listingClient = listingClient ?? new ListingClient();
            //lookupClient = lookupClient ?? new LookupClient();
            //wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }

        public static TrainingVideos Self
        {
            get { return instance; }
        }
        private static readonly TrainingVideos instance = new TrainingVideos();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Training Videos";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);
            HttpSessionState Session = HttpContext.Current.Session;

            if (!IsPostBack)
            {
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];

            }
        }

    }
}