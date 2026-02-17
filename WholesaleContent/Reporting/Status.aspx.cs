using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI.WebControls;

using LMWholesale.Common;
using LMWholesale.resource.model.Wholesale;

namespace LMWholesale.WholesaleContent.Reporting
{
    public partial class Status : lmPage
    {
        private readonly BLL.WholesaleContent.Reporting.Status statusBLL;
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;

        public Status()
        {
            statusBLL = statusBLL ?? new BLL.WholesaleContent.Reporting.Status();
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
        }
        public Status(BLL.WholesaleContent.Reporting.Status _statusBLL, BLL.WholesaleUser.WholesaleUser _userBLL)
        {
            statusBLL = _statusBLL;
            userBLL = _userBLL;
        }

        public static Status Self
        {
            get { return instance; }
        }

        private static readonly Status instance = new Status();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Account Dashboard";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            int kAccountType = (int)Session["kAccountType"];

            DataTable info = statusBLL.GetInventoryHealthData(kSession, kDealer);
            FormatInventoryHealth(info.Rows[0], statusBLL.GetWidgetPreferences(kSession, kDealer), kAccountType);

            jsGridBuilder ImportHistoryGrid = new jsGridBuilder
            {
                MethodURL = "Status.aspx/GetImportHistory",
                HTMLElement = "jsGrid",
                Filtering = false,
                Sorting = false,
                ExtraFunctionality = "document.getElementById(\"jsGrid\").children[1].style.height = \"calc((100vh - 220px)*.45)\""
            };
            string ImportHistoryGridColumns = Self.userBLL.GetGridDef(kSession, "WP-ImportHistory");
            ImportHistoryGrid.SetFieldListFromGridDef(ImportHistoryGridColumns, "klisting", true);

            if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                ClientScript.RegisterStartupScript(this.GetType(), "jsGrid", ImportHistoryGrid.RenderGrid());

            //jsGridBuilder CurrentResultsGrid = new jsGridBuilder
            //{
            //    MethodURL = "Status.aspx/GetCurrentResults",
            //    HTMLElement = "CurrentResultsGrid",
            //    Filtering = false,
            //    Sorting = false,
            //    ExtraFunctionality = "document.getElementById(\"CurrentResultsGrid\").children[1].style.height = \"calc((100vh - 220px)*.45)\""
            //};
            //string CurrentResultsGridColumns = ":Auction:Auction:100|:TotalListed:Total Listed:100|:SoldMTD:Sold MTD:100|:ErrorCount:Error Count:100";
            //CurrentResultsGrid.SetFieldListFromGridDef(CurrentResultsGridColumns, "", true);
            //
            //if (!ClientScript.IsStartupScriptRegistered("JSScript"))
            //    ClientScript.RegisterStartupScript(this.GetType(), "CurrentResultsGrid", CurrentResultsGrid.RenderGrid());
        }

        private void FormatInventoryHealth(DataRow Info, List<double> Thresholds, int kAccountType)
        {
            int usedCount = Convert.ToInt32(Info["UsedCount"] == DBNull.Value ? "0" : Info["UsedCount"].ToString());
            int newCount = Convert.ToInt32(Info["NewCount"] == DBNull.Value ? "0" : Info["NewCount"].ToString());
            int totalCount = usedCount + newCount;

            hfNewCount.Value = newCount.ToString();
            hfUsedCount.Value = usedCount.ToString();
            hfThresholds.Value = String.Join(";", Thresholds);

            int usedHolder;
            int newHolder;
            double percentage;

            newHolder = Convert.ToInt32(Info["AvgDaysWithNOPicsNew"] == DBNull.Value ? "0" : Info["AvgDaysWithNOPicsNew"].ToString());
            //lblAvgDaysPhotosNew.Text = newHolder.ToString();
            usedHolder = Convert.ToInt32(Info["AvgDaysWithNOPicsUsed"] == DBNull.Value ? "0" : Info["AvgDaysWithNOPicsUsed"].ToString());
            lblAvgDaysPhotosUsed.Text = usedHolder.ToString();
            //lblAvgDaysPhotosTotal.Text = (newHolder + usedHolder).ToString();

            newHolder = Convert.ToInt32(Info["AvgDaysWithNONotesNew"] == DBNull.Value ? "0" : Info["AvgDaysWithNONotesNew"].ToString());
            //lblAvgDescNew.Text = newHolder.ToString();
            usedHolder = Convert.ToInt32(Info["AvgDaysWithNONotesUsed"] == DBNull.Value ? "0" : Info["AvgDaysWithNONotesUsed"].ToString());
            lblAvgDescUsed.Text = usedHolder.ToString();
            //lblAvgDescTotal.Text = (newHolder + usedHolder).ToString();

            newHolder = Convert.ToInt32(Info["NewInventoryWithoutDescription"] == DBNull.Value ? "0" : Info["NewInventoryWithoutDescription"].ToString());
            percentage = Math.Round(((double)newCount - newHolder) / (newCount) * 100, 2);
            //lblVehDescNew.Text = newHolder + " of " + newCount + " (" + (newCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            usedHolder = Convert.ToInt32(Info["UsedInventoryWithoutDescription"] == DBNull.Value ? "0" : Info["UsedInventoryWithoutDescription"].ToString());
            percentage = Math.Round(((double)usedCount - usedHolder) / (usedCount) * 100, 2);
            lblVehDescUsed.Text = usedCount - usedHolder + " of " + usedCount + " (" + (usedCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            percentage = Math.Round(((double)newHolder + usedHolder) / (totalCount) * 100, 2);
            //lblVehDescTotal.Text = (newHolder + usedHolder) + " of " + totalCount + " (" + (totalCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";

            newHolder = Convert.ToInt32(Info["NewWithInternetPrice"] == DBNull.Value ? "0" : Info["NewWithInternetPrice"].ToString());
            percentage = Math.Round(((double)newHolder) / (newCount) * 100, 2);
            //lblVehInternetNew.Text = newHolder + " of " + newCount + " (" + (newCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            usedHolder = Convert.ToInt32(Info["UsedWithInternetPrice"] == DBNull.Value ? "0" : Info["UsedWithInternetPrice"].ToString());
            percentage = Math.Round(((double)usedHolder) / (usedCount) * 100, 2);
            lblVehInternetUsed.Text = usedHolder + " of " + usedCount + " (" + (usedCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            percentage = Math.Round(((double)newHolder + usedHolder) / (totalCount) * 100, 2);
            //lblVehInternetTotal.Text = (newHolder + usedHolder) + " of " + totalCount + " (" + (totalCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";

            newHolder = Convert.ToInt32(Info["NewWithListPrice"] == DBNull.Value ? "0" : Info["NewWithListPrice"].ToString());
            percentage = Math.Round(((double)newHolder) / (newCount) * 100, 2);
            //lblVehListNew.Text = newHolder + " of " + newCount + " (" + (newCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            usedHolder = Convert.ToInt32(Info["UsedWithListPrice"] == DBNull.Value ? "0" : Info["UsedWithListPrice"].ToString());
            percentage = Math.Round(((double)usedHolder) / (usedCount) * 100, 2);
            lblVehListUsed.Text = usedHolder + " of " + usedCount + " (" + (usedCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            percentage = Math.Round(((double)newHolder + usedHolder) / (totalCount) * 100, 2);
            //lblVehListTotal.Text = (newHolder + usedHolder) + " of " + totalCount + " (" + (totalCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";

            newHolder = Convert.ToInt32(Info["NewVehiclesWithPhotos"] == DBNull.Value ? "0" : Info["NewVehiclesWithPhotos"].ToString());
            percentage = Math.Round(((double)newHolder) / (newCount) * 100, 2);
            //lblVehPhotosNew.Text = newHolder + " of " + newCount + " (" + (newCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            usedHolder = Convert.ToInt32(Info["UsedWithPics"] == DBNull.Value ? "0" : Info["UsedWithPics"].ToString());
            percentage = Math.Round(((double)usedHolder) / (usedCount) * 100, 2);
            lblVehPhotosUsed.Text = usedHolder + " of " + usedCount + " (" + (usedCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            percentage = Math.Round(((double)newHolder + usedHolder) / (totalCount) * 100, 2);
            //lblVehPhotosTotal.Text = (newHolder + usedHolder) + " of " + totalCount + " (" + (totalCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";

            newHolder = Convert.ToInt32(Info["NewInventoryWithoutStyle"] == DBNull.Value ? "0" : Info["NewInventoryWithoutStyle"].ToString());
            percentage = Math.Round(((double)newHolder) / (newCount) * 100, 2);
            //lblVehStyleNew.Text = newHolder + " of " + newCount + " (" + (newCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            usedHolder = Convert.ToInt32(Info["UsedInventoryWithoutStyle"] == DBNull.Value ? "0" : Info["UsedInventoryWithoutStyle"].ToString());
            percentage = Math.Round(((double)usedCount - usedHolder) / (usedCount) * 100, 2);
            lblVehStyleUsed.Text = usedCount - usedHolder + " of " + usedCount + " (" + (usedCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";
            percentage = Math.Round(((double)newHolder + usedHolder) / (totalCount) * 100, 2);
            //lblVehStyleTotal.Text = (newHolder + usedHolder) + " of " + totalCount + " (" + (totalCount == 0 ? "0" : Math.Round(percentage, 0).ToString()) + "%)";

            newHolder = Convert.ToInt32(Info["NewInventoryMarkedUnavailable"] == DBNull.Value ? "0" : Info["NewInventoryMarkedUnavailable"].ToString());
            //lblVehUnavailNew.Text = newHolder.ToString();
            usedHolder = Convert.ToInt32(Info["UsedInventoryMarkedUnavailable"] == DBNull.Value ? "0" : Info["UsedInventoryMarkedUnavailable"].ToString());
            lblVehUnavailUsed.Text = usedHolder.ToString();
            //lblVehUnavailTotal.Text = (newHolder + usedHolder).ToString();

            if (lstReportSelector.Items.Count == 0)
            {
                // #TODO: Removing this item due to excessive data set. Need to revisit on-demand reporting
                //lstReportSelector.Items.Add(new ListItem("Health Report Summary", "healthReportSummary", true));
                //lstReportSelector.Items.Add(new ListItem("Offer Report", "offerReport", true));

                if (kAccountType == 3)
                {
                    lstReportSelector.Items.Add(new ListItem("Health Report Minimum", "healthReportMin", true));
                    lstReportSelector.Items.Add(new ListItem("Health Report Detail", "healthReportDetail", true));
                }
                //lstReportSelector.Items.Add(new ListItem("Wholesale - Auto Launch Rules", "AutoLaunch", true));
                lstReportSelector.Items.Add(new ListItem("Wholesale - Active Listings", "WholesaleActiveListings", true));
                lstReportSelector.Items.Add(new ListItem("Wholesale - Auction/Group Active Listings", "WholesaleAuctionGroupActiveListings", true));
            }
        }

        [WebMethod(Description = "Get data for import history grid")]
        public static string GetImportHistory()
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            return Self.statusBLL.GetImportHistory(kSession, kDealer);
        }

        [WebMethod(Description = "Get data for current results grid")]
        public static string GetCurrentResults()
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            return Self.statusBLL.GetCurrentResults(kSession, kDealer);
        }

        protected void ExportToExcel(object Sender, EventArgs e)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            string dealerName = (string)Session["Dealername"];
            DateTime? runDate = null;
            int kGaggleSubGroup = (int)Session["kGaggleSubGroup"];

            if (String.IsNullOrEmpty(Convert.ToString(Session["kSession"])))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            if(!string.IsNullOrEmpty(tbRunDate.Text))
            {
                DateTime temp = Convert.ToDateTime(tbRunDate.Text);
                runDate = new DateTime(temp.Year, temp.Month, 1);
            }

            Dictionary<string, string> dict = Self.statusBLL.GetExcelExport(kSession, kDealer, lstReportSelector.SelectedValue, dealerName, runDate, kGaggleSubGroup);

            if (dict.Keys.Count > 1)
            {
                // Download CSV file
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename={dict["fileName"]}");
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(dict["sb"]);

                // Take out the trash
                Response.Flush();
                Response.End();
            }
            else
            {
                ErrorDisplay.InnerHtml = "Error: " + dict["Message"];
            }
        }

        [WebMethod(Description = "Set filters before redirecting to VehicleManagement")]
        public static Dictionary<string, object> SetAdvFilters(string Filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            InventoryFilter.AdvancedFilter advancedFilter = new InventoryFilter.AdvancedFilter();
            InventoryFilter.Filter filter = new InventoryFilter.Filter(Session["kSession"].ToString(), int.Parse(Session["kDealer"].ToString()));

            if (Filter == "Unavailable")
            {
                advancedFilter.StatusUnavailable = 1;

                Session["advancedFilter"] = Util.serializer.Serialize(advancedFilter);
                Session["filters"] = Util.serializer.Serialize(filter);
            }
            else
            {
                advancedFilter.GetType().GetProperty(Filter).SetValue(advancedFilter, 1);

                Session["advancedFilter"] = Util.serializer.Serialize(advancedFilter);
                Session["filters"] = Util.serializer.Serialize(filter);

                Message = "Applied search filters. Redirecting to Vehicle Management...";

                Session["advancedFilter"] = Util.serializer.Serialize(advancedFilter);
                Session["filters"] = Util.serializer.Serialize(filter);
            }
            Message = "Applied search filters. Redirecting to Vehicle Management...";

            return ReturnResponse();
        }
    }
}