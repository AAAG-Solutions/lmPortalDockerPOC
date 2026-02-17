using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using LMWholesale.Common;

namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class ModifyPhotos : lmPage
    {
        private readonly BLL.WholesaleContent.Vehicle.ModifyPhotos modifyPhotosBLL;

        public ModifyPhotos() => modifyPhotosBLL = modifyPhotosBLL ?? new BLL.WholesaleContent.Vehicle.ModifyPhotos();

        public static ModifyPhotos Self
        {
            get { return instance; }
        }
        private static readonly ModifyPhotos instance = new ModifyPhotos();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Modify Photos";
            PageSecurityManager.DoPageSecurity(this);

            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
                {
                    int kListing = int.Parse(Request.QueryString["kListing"]);
                    string returnVal =
                        Self.modifyPhotosBLL.BuildPhotoList((string)Session["kSession"], kListing);

                    phPhotos.Controls.Add(new LiteralControl(returnVal));

                    if (Regex.IsMatch(Request.UrlReferrer.AbsolutePath, "VehicleManagement.aspx$"))
                    {
                        BackToVehicle.InnerHtml = $"&#60; &#8722; <a class='backBreadcrumb' href=\"javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx';\">Back To Inventory</a>";
                        //Cancel.InnerHtml = $"<input type=\"button\" class=\"actionBackground\" value=\"Cancel\" onclick=\"javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx';\">";
                    }
                    else
                    {
                        BackToVehicle.InnerHtml = $"&#60; &#8722; <a class='backBreadcrumb' href=\"javascript: window.location.href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}';\">Back To Vehicle</a>";
                        //Cancel.InnerHtml = $"<input type=\"button\" class=\"actionBackground\" value=\"Cancel\" onclick=\"javascript: window.location.href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}';\">";
                    }
                }
            }
        }
    }
}