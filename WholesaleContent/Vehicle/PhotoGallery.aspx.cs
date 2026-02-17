using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

using LMWholesale.Common;
using LMWholesale.resource.clients;

namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class PhotoGallery : lmPage
    {
        private readonly BLL.WholesaleContent.Vehicle.PhotoGallery photoGalleryBLL;

        public PhotoGallery() => photoGalleryBLL = photoGalleryBLL ?? new BLL.WholesaleContent.Vehicle.PhotoGallery();

        public static PhotoGallery Self
        {
            get { return instance; }
        }
        private static readonly PhotoGallery instance = new PhotoGallery();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageSecurityManager.DoPageSecurity(this);

            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
                {
                    Dictionary<string, string> returnItems =
                        Self.photoGalleryBLL.BuildPhotoGallery((string)Session["kSession"], int.Parse(Request.QueryString["kListing"]));

                    lightgallerySlider.InnerHtml = returnItems["slider"];
                    lightgalleryCarousel.InnerHtml = returnItems["carousel"];
                }
            }
        }
    }
}