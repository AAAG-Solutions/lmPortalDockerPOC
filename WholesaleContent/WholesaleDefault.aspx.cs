using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using LMWholesale.Common;

namespace LMWholesale
{
    public partial class WholesaleDefault : lmPage
    {
        private readonly BLL.WholesaleContent.WholesaleDefault defaultBLL;
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;

        public WholesaleDefault()
        {
            defaultBLL = new BLL.WholesaleContent.WholesaleDefault();
            userBLL = new BLL.WholesaleUser.WholesaleUser();
        }

        public WholesaleDefault(BLL.WholesaleUser.WholesaleUser userBLL, BLL.WholesaleContent.WholesaleDefault defaultBLL)
        {
            this.defaultBLL = defaultBLL;
            this.userBLL = userBLL;
        }

        public static WholesaleDefault Self
        {
            get { return instance; }
        }

        private static WholesaleDefault instance = new WholesaleDefault();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Dealer Selection";
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                Control accountName = Page.Master.FindControl("headerAccountName");
                if (accountName != null)
                    accountName.Visible = false;

                Control accountNameDropdown = Page.Master.FindControl("accountNameDropdown");
                if (accountNameDropdown != null)
                    accountNameDropdown.Visible = false;

                jsGridBuilder dealerselectgrid = new jsGridBuilder
                {
                    HTMLElement = "jsGrid",
                    Filtering = false,
                    OnRowSelectFunction = "GridRowSelected",
                    OnClearRowSelectFunction = "ClearRowSelection",
                    OnDoubleClickFunction = "RowDoubleClick();",
                    MethodURL = "WholesaleDefault.aspx/DealerSelection"
                };

                dealerselectgrid.ExtraFunctionality = $@"
                    var gridData = $('#jsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].AccountStatus == ""On-Hold"") {{
                                $('#jsGrid')[0].children[1].children[0].children[0].children[i].style = 'color: red;';
                            }}
                        }};
                    }}
                ";

                dealerselectgrid.NotSortableFields.AddRange(new string[] { "" });
                // #TODO: Move GridDef to db
                string wholesaleDefaultGridDef = ":kDealer:Account ID:60|:::60|:AccountStatus:Status:60|:DealerName:Account Name:130|:PersonOwner:Account Rep:60|!:AccountTypeDesc:Account Type:60|!:CustomerTypeDesc:Customer Type:70|:DealerGaggleName:Account Group:60";
                bool isInternal = Self.userBLL.CheckPermission("LMIInternal");

                // Hide the kDealer value if not Internal
                if (isInternal == false)
                    wholesaleDefaultGridDef = wholesaleDefaultGridDef.Replace(":DealerGaggleName:Account Group:60", "!:DealerGaggleName:Account Group:60");

                if (Self.userBLL.CheckPermission("AccountRequest") == false && Self.userBLL.CheckPermission("Developer") == false)
                    btnAccountSetup.Visible = false;

                dealerselectgrid.SetFieldListFromGridDef(wholesaleDefaultGridDef, "kDealer", true);

                if (((DataTable)Session["dsDealers"]).Rows.Count == 0)
                    BLL.WholesaleUser.WholesaleUser.ClearUser();

                // Set filters and get distinct values
                string AccountSearchValue = Convert.ToString(Session["AccountSearchFilter"]);
                if (!String.IsNullOrEmpty(AccountSearchValue))
                    txtSearch.Text = AccountSearchValue;

                DataTable dt = (DataTable)Session["dsDealers"];

                ddlAccountStatus.DataSource = new List<string> { "Account Status", "Active", "On-Hold" };
                ddlAccountStatus.DataBind();
                string AccountTypeValue = Convert.ToString(Session["AccountStatusFilter"]);
                if (!String.IsNullOrEmpty(AccountTypeValue))
                {
                    ddlAccountStatus.SelectedValue = AccountTypeValue;
                }

                ddlAccountGroup.DataSource = dt.AsEnumerable().Select(r => r["DealerGaggleName"].ToString()).Distinct().OrderBy(r => r);
                ddlAccountGroup.DataBind();
                ddlAccountGroup.Items.Insert(0, "Account Group");
                string AccountGroupValue = Convert.ToString(Session["AccountGroupFilter"]);
                if (!String.IsNullOrEmpty(AccountGroupValue))
                {
                    ddlAccountGroup.SelectedValue = AccountGroupValue;
                }

                ddlAccountRep.DataSource = dt.AsEnumerable().Select(r => r["PersonOwner"].ToString()).Distinct().OrderBy(r => r);
                ddlAccountRep.DataBind();
                ddlAccountRep.Items.Insert(0, "Account Rep");
                string AccountRepValue = Convert.ToString(Session["PersonOwnerFilter"]);
                if (!String.IsNullOrEmpty(AccountRepValue))
                {
                    ddlAccountRep.SelectedValue = AccountRepValue;
                }

                // Have to peform hiding after we fill in the dropdown menus
                if (isInternal == false)
                {
                    AccountGroupSelector.Style["display"] = "none !important";
                    btnSupportTool.Style["display"] = "none";
                    btnSupportToolspace.Style["display"] = "none";
                }

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "dealerselectGrid", dealerselectgrid.RenderGrid());
            }
        }

        [WebMethod(EnableSession = true)]
        public static string DealerSelection(string filter)
        {
            Dictionary<string, object> oFilter = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);
            // Add Security Checks

            return Self.defaultBLL.BuildDealerSelection(oFilter);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string successMsg = "";
            string errorMsg = "";
            Int32.TryParse(txtkDealer.Value, out int kDealer);

            // If we are switching dealers, we do not want user filters to bleed into another
            if (txtkDealer.Value != Session["kDealer"].ToString())
                Session["SelectedVH"] = Session["advancedFilter"] = Session["filters"] = null;

            Self.userBLL.GetDealersInfo(Session, ref successMsg, ref errorMsg, "", kDealer);
            btnSubmit.Enabled = true;
            if (kDealer > 0)
            {
                saveFilters();
                if (IsLiquidConnect())
                    Response.Redirect("/WholesaleContent/Vehicle/Search.aspx");
                Response.Redirect("/WholesaleContent/VehicleManagement.aspx");
            }
        }

        protected void btnSellDownAPI_Click(object sender, EventArgs e)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            userBLL.CheckSession();

            // Now time to get into some routing fun
            // #TODO: have to set kDealer to '1' or else we fail the CheckDealer on the page
            Session["kDealer"] = 1;
            Response.Redirect("/WholesaleContent/Reporting/SalesDataApproval.aspx");
        }

        protected void saveFilters()
        {
            HttpSessionState Session = HttpContext.Current.Session;
            Session["AccountSearchFilter"] = txtSearch.Text;
            Session["AccountStatusFilter"] = ddlAccountStatus.Text;
            //Session["AccountTypeFilter"] = ddlAccountType.SelectedValue;
            //Session["CustomerTypeFilter"] = ddlCustomerType.SelectedValue;
            Session["AccountGroupFilter"] = ddlAccountGroup.SelectedValue;
            Session["PersonOwnerFilter"] = ddlAccountRep.SelectedValue;
        }

        protected void ExportAccounts(object Sender, EventArgs e)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];

            if (String.IsNullOrEmpty(Convert.ToString(kSession)))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            bool isInternal = Self.userBLL.CheckPermission("LMIInternal");

            DataTable dt = (DataTable)Session["dsDealers"];
            Dictionary<string, string> dict = Self.defaultBLL.ExportInventory(dt, isInternal, ddlAccountRep.SelectedItem.Text, ddlAccountGroup.SelectedItem.Text);

            // Download CSV file
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={dict["fileName"]}");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Response.Output.Write(dict["sb"]);

            // Take out the trash
            Response.Flush();
            Response.End();
        }
    }
}