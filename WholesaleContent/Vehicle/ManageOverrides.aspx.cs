using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using LMWholesale.Common;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class ManageOverrides : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Vehicle.ManageOverrides manageOverridesBLL;
        private readonly IInventoryClient inventoryClient = new InventoryClient();

        public ManageOverrides()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            manageOverridesBLL = manageOverridesBLL ?? new BLL.WholesaleContent.Vehicle.ManageOverrides();
        }

        public static ManageOverrides Self
        {
            get { return instance; }
        }
        private static readonly ManageOverrides instance = new ManageOverrides();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Manage Overrides";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                    BLL.WholesaleUser.WholesaleUser.ClearUser();

                hfkListing.Value = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";

                if (!IsPostBack)
                {
                    if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
                    {
                        Dictionary<string, string> result = Self.manageOverridesBLL.GetOverrides(Session["kSession"].ToString(), int.Parse(Request.QueryString["kListing"]));
                        foreach (KeyValuePair<string, string> item in result)
                        {
                            switch (item.Key)
                            {
                                case "ImportOverride":
                                    DisFeed.Checked = item.Value == "1";
                                    break;
                                case "ListPriceOverride":
                                    DisList.Checked = item.Value == "1";
                                    break;
                                case "DetailDescOverride":
                                    DisDetail.Checked = item.Value == "1";
                                    break;
                                case "InternetPriceOverride":
                                    DisInternet.Checked = item.Value == "1";
                                    break;
                                case "InventoryStatusOverride":
                                    DisStatus.Checked = item.Value == "1";
                                    break;
                                case "InvAdded":
                                    InventoryAddDate.Text = string.IsNullOrEmpty(item.Value) ? "" : Convert.ToDateTime(item.Value).ToString("yyyy-MM-dd");
                                    break;
                                case "InvAddImport":
                                    DisAge.Checked = item.Value == "1";
                                    break;
                                case "WholesaleFloorOverride":
                                    DisFloor.Checked = item.Value == "1";
                                    break;
                                case "WholesaleBuyNowOverride":
                                    DisBIN.Checked = item.Value == "1";
                                    break;
                                case "WholesaleStartPriceOverride":
                                    DisStart.Checked = item.Value == "1";
                                    break;
                                case "MsrpOverride":
                                    DisMSRP.Checked = item.Value == "1";
                                    break;
                                case "CostOverride":
                                    DisCost.Checked = item.Value == "1";
                                    break;
                                case "MMROverride":
                                    DisMMR.Checked = item.Value == "1";
                                    break;
                                case "ALPriceOverride":
                                    OvrAutoLaunch.Checked = item.Value == "1";
                                    break;
                            }

                        }
                    }
                }
            }
        }

        [WebMethod(Description = "Save Overrides")]
        public static string SaveOverrides(string jsonData)
        {
            return Self.manageOverridesBLL.SetOverrides(HttpContext.Current.Session["kSession"].ToString(), jsonData);
        }
    }
}