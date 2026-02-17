using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.Services;

using LMWholesale.Common;


namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class Delete : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Vehicle.Delete deleteBLL;

        public Delete()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            deleteBLL = deleteBLL ?? new BLL.WholesaleContent.Vehicle.Delete();
        }

        public static Delete Self
        {
            get { return instance; }
        }
        private static readonly Delete instance = new Delete();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Delete Vehicle";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;

                Session["UV_kListing"] = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";
                if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
                {
                    int kListing = int.Parse(Request.QueryString["kListing"]);
                    string kSession = (string)Session["kSession"];
                    int kDealer = (int)Session["kDealer"];

                    Listing.lmReturnValue returnValue = Self.deleteBLL.ListingDetailGet(kSession, kDealer, kListing);

                    if (returnValue.Result == Listing.ReturnCode.LM_SUCCESS)
                    {
                        DataRow dr = returnValue.Data.Tables[0].Rows[0];

                        InputVIN.Text = Util.cleanString((Convert.ToString(dr["VIN"])));
                        InputStock.Text = dr["StockNumber"].ToString();
                        InputYear.Text = dr["Year"].ToString();
                        InputMake.Text = dr["Make"].ToString();
                        InputModel.Text = dr["Model"].ToString();
                        InputStyle.Text = dr["Style"].ToString();
                        kListingValue.Text = kListing.ToString();
                    }
                    else if (returnValue.Result == Listing.ReturnCode.LM_INVALIDSESSION)
                    {
                        BLL.WholesaleUser.WholesaleUser.ClearUser(returnValue.ResultString);
                    }
                    else
                    {
                        string smsg = "<script>alert('Unable to perform request due to the following error: " + returnValue.ResultString + ".  Please try again or call support for assistance.');</script>";
                        Response.Write(smsg);
                    }
                }
            }
        }

        [WebMethod]
        public static Dictionary<string, object> VehicleDelete(string kListing)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];

            IsSuccess = Self.deleteBLL.DeleteInventory(kSession, int.Parse(kListing));

            return ReturnResponse();
        }
    }
}