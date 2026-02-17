using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;

using LMWholesale.BLL.WholesaleUser;
using LMWholesale.Common;
using LMWholesale.resource.model.Wholesale;

namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class Search : lmPage
    {
        private readonly WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Vehicle.Search searchBLL;

        public Search()
        {
            userBLL = userBLL ?? new WholesaleUser();
            searchBLL = searchBLL ?? new BLL.WholesaleContent.Vehicle.Search();
        }

        public static Search Self
        {
            get { return instance; }
        }

        private static readonly Search instance = new Search();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Vehicle Search";
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                Control accountName = Page.Master.FindControl("headerAccountName");
                if (accountName != null)
                    accountName.Visible = false;

                Control accountNameDropdown = Page.Master.FindControl("accountNameDropdown");
                if (accountNameDropdown != null)
                    accountNameDropdown.Visible = false;

                Session["VSPageReload"] = true;

                jsGridBuilder searchGrid = new jsGridBuilder()
                {
                    MethodURL = "Search.aspx/SearchAccounts",
                    Sorting = false,
                    HTMLElement = "jsGrid",
                    Filtering = false
                };

                searchGrid.ExtraFunctionality = $@"
                    LCGridProcess()
                ";

                // #TODO: Move GirdDef to db
                string searchVehicleGridColumns = ":::120|:DealerName:Account:120|:StockNumber:Stock #:120|:VIN::150|:YearMakeModel:Year/Make/Model:120|:Mileage::120|:ListingStatus:Listing Status:120|";
                searchGrid.SetFieldListFromGridDef(searchVehicleGridColumns, "kListing", true);

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "vehicleSearchGrid", searchGrid.RenderGrid());
            }
        }

        [WebMethod]
        public static string SearchAccounts(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Convert.ToString(Session["kSession"])))
                WholesaleUser.ClearUser();
            if (String.IsNullOrEmpty(Convert.ToString(filter)))
                return "0 | {}";

            Dictionary<string, object> oFilter = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);

            if ((bool)Session["VSPageReload"])
            {
                Session["VSVin"] = "";
                Session["VSPageReload"] = false;
            }
            else
            {
                if (oFilter.ContainsKey("VIN"))
                    Session["VSVin"] = oFilter["VIN"].ToString();
            }

            // On initial load, we default to searching for nothing
            if (Session["VSVin"].ToString() != "")
            {
                if (IsLiquidConnect())
                    return Self.searchBLL.GetListingsByVIN(Session["kSession"].ToString(), Session["VSVin"].ToString(), Session["kDealer"].ToString());
                return Self.searchBLL.GetListingsByVIN(Session["kSession"].ToString(), Session["VSVin"].ToString());
            }

            // If we fail, return nothing
            return "0 | {}";
        }

        protected void GoToVehicle(object sender, EventArgs e)
        {
            SwitchDealerContext();
            VehicleButton.Enabled = true;
            if ((int)Session["kDealer"] > 0 && VehiclekListing.Value != "")
                Response.Redirect($"/WholesaleContent/Vehicle/Update.aspx?kListing={VehiclekListing.Value}");
        }

        protected void GoToAccount(object sender, EventArgs e)
        {
            SwitchDealerContext();
            AccountButton.Enabled = true;

            InventoryFilter.Filter filter = new InventoryFilter.Filter(Session["kSession"].ToString(), int.Parse(DealerText.Value));
            filter.TextFilter = VehicleVin.Value;

            // Save to Session
            Session["filters"] = Util.serializer.Serialize(filter);

            if ((int)Session["kDealer"] > 0)
                Response.Redirect("/WholesaleContent/VehicleManagement.aspx");
        }

        private void SwitchDealerContext()
        {
            string successMsg = "";
            string errorMsg = "";

            // Check if we have any previous Session items saved
            // SelectedVH, advancedFilterSet, VMFilters
            if (DealerText.Value != Session["kDealer"].ToString())
                Session["SelectedVH"] = Session["advancedFilterSet"] = Session["filters"] = Session["VMFilters"] = null;

            Int32.TryParse(DealerText.Value, out int kDealer);
            Self.userBLL.GetDealersInfo(Session, ref successMsg, ref errorMsg, "", kDealer);
        }
    }
}