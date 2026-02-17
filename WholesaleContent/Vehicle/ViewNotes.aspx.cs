using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using LMWholesale.Common;

using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class ViewNotes : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Vehicle.ViewNotes notesBLL;

        public ViewNotes()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            notesBLL = notesBLL ?? new BLL.WholesaleContent.Vehicle.ViewNotes();
        }

        public static ViewNotes Self
        {
            get { return instance; }
        }
        private static readonly ViewNotes instance = new ViewNotes();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "View Vehicle Notes";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            Session["UV_kListing"] = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";
            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
                {
                    string kSession = (string)Session["kSession"];
                    int kDealer = (int)Session["kDealer"];
                    int kListing = int.Parse(Request.QueryString["kListing"]);

                    //  Back To Inventory
                    BackToVehicle.InnerHtml = $"&#60; &#8722; <a class='backBreadcrumb' href='javascript:history.back();'>Back</a>";

                    VehicleNotes.InnerHtml = Self.notesBLL.ListingVehicleNotesGet(kSession, kDealer, kListing);
                    DataRow dr = Self.notesBLL.ListingDetailGet(kSession, kDealer, kListing);

                    DealerName.Text = dr["DealerName"].ToString();
                    VehicleVIN.Text = dr["VIN"].ToString();
                    StockNumber.Text = dr["StockNumber"].ToString();
                }
            }
        }
    }
}