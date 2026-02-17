using LMWholesale.resource.clients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class ManagePhotos : System.Web.UI.Page
    {
        private readonly BLL.WholesaleContent.Vehicle.ManagePhotos managePhotosBll;

        public ManagePhotos() => managePhotosBll = managePhotosBll ?? new BLL.WholesaleContent.Vehicle.ManagePhotos();

        public static ManagePhotos Self
        {
            get { return instance; }
        }
        private static readonly ManagePhotos instance = new ManagePhotos();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageSecurityManager.DoPageSecurity(this);

            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];
            int kListing = int.Parse(Request.QueryString["kListing"]);
            Session["MP_kListing"] = kListing;
            string[] controls = managePhotosBll.BuildPhotoCards(kSession, kListing);
            panVehPhotoArea.Controls.Add(new LiteralControl(controls[0]));
            panDamPhotoArea.Controls.Add(new LiteralControl(controls[1]));

            btnUploadPhotos.OnClientClick = $"javascript: PhotoPopup({kListing.ToString()}); return false;";
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, object> SavePhotoChanges(string PhotoList)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            return Self.managePhotosBll.SavePhotoChanges(Session["kSession"].ToString(), Session["MP_kListing"].ToString(), PhotoList);
        }
    }
}