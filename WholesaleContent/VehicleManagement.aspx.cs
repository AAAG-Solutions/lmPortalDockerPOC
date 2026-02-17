using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Reflection;

using LMWholesale.Common;
using LMWholesale.resource.clients;
using LMWholesale.resource.model.Wholesale;
using EO.WebBrowser.DOM;

namespace LMWholesale
{
    public partial class VehicleManagement : lmPage
    {
        private readonly BLL.WholesaleContent.VehicleManagement vehicleManagementBLL;
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly WholesaleClient wholesaleClient;

        public VehicleManagement()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            vehicleManagementBLL = vehicleManagementBLL ?? new BLL.WholesaleContent.VehicleManagement();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }
        public VehicleManagement(BLL.WholesaleUser.WholesaleUser _userBLL, BLL.WholesaleContent.VehicleManagement _vehicleBLL)
        {
            userBLL = _userBLL;
            vehicleManagementBLL = _vehicleBLL;
        }

        public static VehicleManagement Self
        {
            get { return instance; }
        }

        private static readonly VehicleManagement instance = new VehicleManagement();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Vehicle Management";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];
                string kPerson = (string)Session["kPerson"];

                Session["FileList"] = "";
                Session["NextPhotoNum"] = 0;

                Dictionary<string, object> selectedObject = new Dictionary<string, object>()
                {
                    { "kListing", "" },
                    { "page", "" }
                };

                if (Session["SelectedVH"] == null)
                    Session["SelectedVH"] = Util.serializer.Serialize(selectedObject);
                else
                {
                    selectedObject = (Dictionary<string, object>)Util.serializer.DeserializeObject(Session["SelectedVH"].ToString());
                    SelectedkListing.Value = selectedObject["kListing"].ToString();
                    selectedObject["kListing"] = "";
                    Session["SelectedVH"] = Util.serializer.Serialize(selectedObject);
                }

                jsGridBuilder wholesalegrid = new jsGridBuilder
                {
                    MethodURL = "VehicleManagement.aspx/WholesaleInventory",
                    OnRowSelectFunction = "GridRowSelected",
                    OnClearRowSelectFunction = "ClearRowSelection",
                    OnDoubleClickFunction = "RowDoubleClick();",
                    HTMLElement = "vehicleManagementJSGrid",
                    Filtering = false,
                    PageIndex = int.Parse(!string.IsNullOrEmpty(selectedObject["page"].ToString()) ? selectedObject["page"].ToString() : "1"),
                    PageSize = int.Parse(Session["filters"] != null ? ((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["filters"].ToString()))["ItemsPerPage"].ToString() : "50"),
                    PagerFormat = "Pages: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} of {pageCount} &nbsp;&nbsp; Totalitems: {itemCount} &nbsp;&nbsp;&nbsp; {pageSize} &nbsp; {pageButton}",
                    ExtraFunctionality = $@"
                            var kListing = $('#MainContent_SelectedkListing')[0].value;
                            var gridData = $('#vehicleManagementJSGrid').data('JSGrid').data;

                            if (kListing == '' && gridData.length >= 1)
                            {{
                                // Little hack to bypass header issue when attempting to sort
                                $('#vehicleManagementJSGrid')[0].children[1].children[0].children[0].children[gridData.length - 1].scrollIntoView(false);
                                $('#vehicleManagementJSGrid')[0].children[1].children[0].children[0].children[0].scrollIntoView(false);
                                return false;
                            }}

                            for (let i = 0; i < gridData.length; i++)
                            {{
                                if (gridData[i].kListing == kListing)
                                {{
                                    $('#vehicleManagementJSGrid').data('JSGrid').rowClick({{ event: {{ target: $('#' + gridData[i].kListing + '_action_menu')[0] }}, item: gridData [i] }});
                                    $('#vehicleManagementJSGrid')[0].children[1].children[0].children[0].children[Math.min(i + 3, gridData.length - 1)].scrollIntoView(false);
                                    $('#MainContent_SelectedkListing')[0].value = '';
                                    break;
                                }}
                            }}"
                };

                // #TODO: Implement user-defined grid definitions
                string WholesaleGridColumns = null;
                if (IsLiquidConnect())
                    WholesaleGridColumns = Self.userBLL.GetGridDef(kSession, "LC-VehicleManagement", kDealer, int.Parse(kPerson));
                else
                    WholesaleGridColumns = Self.userBLL.GetGridDef(kSession, "WP-VehicleManagement", kDealer, int.Parse(kPerson));

                wholesalegrid.NotSortableFields.AddRange(new string[] { "VehicleGrade", "ConditionReportLink", "ErrorMsg" });

                Session["VMPageReload"] = true;

                // User Permission checks
                List<string> removeColumn = new List<string>();

                // Hide MMR field
                if (Self.userBLL.CheckPermission("WholesaleMMR") == false)
                    removeColumn.Add("MMSDisplay");

                // Hide action menu and start/end wholesale 
                if (Self.userBLL.CheckPermission("ReadOnly") == true)
                    removeColumn.Add("Actions");

                // Remove Columns from GridDef string
                WholesaleGridColumns = string.Join("|", WholesaleGridColumns.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Where(r => !removeColumn.Contains(r.Split(":".ToCharArray())[1])).ToArray());

                wholesalegrid.SetFieldListFromGridDef(WholesaleGridColumns, "klisting", true);

                SetupVehicleManagement(Session, kSession, kDealer, kPerson);

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "wholesaleGrid", wholesalegrid.RenderGrid());

                if (Session["No3rdpartyExport"].ToString() == "1")
                    Response.Write("<script>alert(\"Notice: This account is currently ON HOLD. Please contact Support to enable full functionality.\");</script>");
            }
        }

        private void SetupVehicleManagement(HttpSessionState Session, string kSession, int kDealer, string kPerson)
        {
            bool existsFilters = Session["filters"] != null;
            bool existsAdvancedFilter = Session["advancedFilter"] != null;

            string selectedLot = "";
            string selectedListingStatus = "";
            string selectedInspectionStatus = "";
            txtSearch.Attributes["oninput"] = WholesaleSystem.onInputString;

            // Real Quick check for text in Search Vehicles box
            if (existsFilters)
                txtSearch.Text = ((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["filters"].ToString()))["TextFilter"].ToString();

            InventoryFilter.AdvancedFilter advancedFilter;
            if (existsAdvancedFilter)
            {
                // Get Advanced Filter from Session
                advancedFilter = new InventoryFilter.AdvancedFilter((Dictionary<string, object>)Util.serializer.DeserializeObject(Session["advancedFilter"].ToString()));
            }
            else
            {
                // Get Advanced Filter from the database
                advancedFilter = Self.vehicleManagementBLL.GetAdvancedFilters(kSession, kDealer, int.Parse(kPerson));
                Session["advancedFilter"] = Util.serializer.Serialize(advancedFilter);
            }

            // Set Checkboxes and filter tokens
            PropertyInfo[] props = advancedFilter.GetType().GetProperties();
            int statusCnt = 0;
            int typeCnt = 0;
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name.StartsWith("Status") && advancedFilter.GetType().GetProperty(prop.Name).GetValue(advancedFilter, null).ToString() == "1")
                    statusCnt++;
                if (prop.Name.StartsWith("Type") && advancedFilter.GetType().GetProperty(prop.Name).GetValue(advancedFilter, null).ToString() == "1")
                    typeCnt++;
            }

            foreach (PropertyInfo prop in props)
            {
                // DropDowns
                if (prop.Name == "LotLocation")
                {
                    if (advancedFilter.LotLocation == "ALL")
                    {
                        selectedLot = "0";
                        tokenLotLocation.InnerText = "Any Lot Location";
                    }
                    else
                    {
                        selectedLot = advancedFilter.LotLocation;
                        tokenLotLocation.InnerText = $"Lot Location: {advancedFilter.LotLocation}";
                    }

                    continue;
                }
                else if (prop.Name == "ListingStatus")
                {
                    selectedListingStatus = advancedFilter.ListingStatus.ToString();
                    continue;
                }
                else if (prop.Name == "InspectionStatus")
                {
                    selectedInspectionStatus = advancedFilter.InspectionStatus.ToString();
                    continue;
                }
                else
                {
                    // Checkboxes
                    HtmlInputCheckBox chk = (HtmlInputCheckBox)((Page)HttpContext.Current.Handler).Master.FindControl("MainContent").FindControl(prop.Name);
                    if (prop.GetValue(advancedFilter, null).ToString() == "1")
                        chk.Checked = true;

                    if (prop.Name.StartsWith("Status"))
                    {
                        if (statusCnt == 6 || statusCnt == 0)
                        {
                            tokenAllInventory.Style["display"] = "initial";
                            continue;
                        }
                        else if (prop.GetValue(advancedFilter, null).ToString() == "1")
                            ((HtmlGenericControl)((Page)HttpContext.Current.Handler).Master.FindControl("MainContent").FindControl($"token{prop.Name}")).Style["display"] = "initial";
                    }
                    if (prop.Name.StartsWith("Type"))
                    {
                        if (typeCnt == 3 || typeCnt == 0)
                        {
                            tokenAllVehicleTypes.Style["display"] = "initial";
                            continue;
                        }
                        else if (prop.GetValue(advancedFilter, null).ToString() == "1")
                            ((HtmlGenericControl)((Page)HttpContext.Current.Handler).Master.FindControl("MainContent").FindControl($"token{prop.Name}")).Style["display"] = "initial";
                    }
                }
            }

            // Listing Status Filter Dropdown
            List<Dictionary<string, string>> lstAuction = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 1);
            string lstAuctions = "";
            foreach (Dictionary<string, string> auction in lstAuction)
                lstAuctions += $"{auction["kWholesaleAuction"]}:Not Listed on {(auction["WholesaleAuctionName"] == "RemarketingPlus" ? "Remarketing+" : auction["WholesaleAuctionName"])}|";

            if (lstAuctions.Length == 0)
            {
                MStart.Style.Add("opacity", "0.5");
                MStart.Style.Add("pointer-events", "none");
                //MEnd.Style.Add("opacity", "0.5");
                //MEnd.Style.Add("pointer-events", "none");
            }

            string lstStatus = $"[]0:Any Listing Status|-1:Listed Vehicles|-2:Non-Listed Vehicles|{lstAuctions}";
            WholesaleSystem.PopulateList(lstStatus, "", "lstListingStatus", '|', selectedListingStatus);
            string listingStatus = WholesaleSystem.BuildList(lstStatus, "", '|').FirstOrDefault(x => x.Value == selectedListingStatus).Key;
            if (selectedListingStatus != "0")
                tokenListingStatus.InnerText = listingStatus;
            else
                tokenListingStatus.InnerText = "Any Listing Status";

            // Inspection Status Filter Dropdown
            string lstInspection = "[]-1:Any Inspection Status|1:Inspected Vehicles|0:Not Inspected Vehicles|";
            WholesaleSystem.PopulateList(lstInspection, "", "lstInspectionStatus", '|', selectedInspectionStatus);
            string inspectionStatus = WholesaleSystem.BuildList(lstStatus, "", '|').FirstOrDefault(x => x.Value == selectedInspectionStatus).Key;
            if (selectedInspectionStatus == "-1")
                tokenInspectionStatus.InnerText = "Any Inspection Status";
            else if (selectedInspectionStatus == "1")
                tokenInspectionStatus.InnerText = "Inspected Vehicles";
            else
                tokenInspectionStatus.InnerText = "Not Inspected Vehicles";

            // Listing Lot Location Results
            Self.vehicleManagementBLL.GetListingLotLocationList(kSession, kDealer, ref selectedLot);
        }

        [WebMethod(Description = "Search all inventory that a dealer/user has access to see")]
        public static string WholesaleInventory(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            Util.serializer.MaxJsonLength = Int32.MaxValue;

            // Redirect to Login screen if we lose session
            if (String.IsNullOrEmpty((string)HttpContext.Current.Session["kSession"]))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            //// We assume no filters are being applied and return default pagination
            //if (String.IsNullOrEmpty(filter))
            //    return Self.vehicleManagementBLL.GetWholesaleInventory(Session, new Dictionary<string, object>());

            if (IsLiquidConnect())
                return Self.vehicleManagementBLL.GetWholesaleInventory(Session, Util.serializer.Deserialize<Dictionary<string, object>>(filter))
                    .Replace("openActionsContent", "lcOpenActionsContent").Replace("|LCHIDE|", "display: none;").Replace("|LCDim|", "45px").Replace("padding-top: 5px", "padding-top: 15px");
            return Self.vehicleManagementBLL.GetWholesaleInventory(Session, Util.serializer.Deserialize<Dictionary<string, object>>(filter)).Replace("|LCHIDE|", "").Replace("|LCDim|", "35px");
        }

        [WebMethod(Description = "Mark Vehicle as either Sold or Available")]
        public static Dictionary<string, object> SellUnsellVehicle(int kListing, int kInventoryStatus)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            if (String.IsNullOrEmpty(Convert.ToString(Session["kSession"])))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            if (String.IsNullOrEmpty(Convert.ToString(kListing))) {
                Message = "Specific vehicle is required";
                IsSuccess = false;

                return ReturnResponse();
            }

            string rv = Self.vehicleManagementBLL.SellUnsellVehicle(Session["kSession"].ToString(), kListing, kInventoryStatus);
            if (!string.IsNullOrEmpty(rv))
            {
                Message = rv;
                IsSuccess = false;
            }
            Value = kInventoryStatus == 1 ? "unsold" : "sold";

            return ReturnResponse();
        }

        [WebMethod(Description = "Set Advanced Filter Set")]
        public static Dictionary<string, object> SetAdvancedFilter(string filter, string operation)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            if (String.IsNullOrEmpty(Convert.ToString(Session["kSession"])))
                BLL.WholesaleUser.WholesaleUser.ClearUser();
            if (String.IsNullOrEmpty(Convert.ToString(Session["kDealer"])))
            {
                IsSuccess = false;
                Message = "Selected Dealer is required";

                return ReturnResponse();
            }

            string returnString = "";
            if (operation.Equals("clearFilters") || operation.Equals("applyFilters"))
            {
                // Set Advanced Filter to Session's advancedFilter
                // Note: should we instantiate a new instance of Inventory.AdvancedFilter to ensure structures match?
                Session["advancedFilter"] = filter;
                return ReturnResponse();
            }
            else if (operation == "defaultFilters")
            {
                IsSuccess = Self.vehicleManagementBLL.SetDefaultFilters(Session, ref returnString);
                Value = Session["advancedFilter"];
                Message = returnString;
                return ReturnResponse();
            }

            IsSuccess = Self.vehicleManagementBLL.SetAdvancedFilters(Session, (Dictionary<string, object>)Util.serializer.DeserializeObject(filter), ref returnString);
            Message = returnString;

            return ReturnResponse();
        }

        [WebMethod]
        public static Dictionary<string, object> VehicleAuctionInfoGet(int kListing, int kWholesaleAuction)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            Value = Self.vehicleManagementBLL.VehicleAuctionInfoGet(kSession, kListing, kDealer, kWholesaleAuction);

            return ReturnResponse();
        }

        protected void ExportInventory(object Sender, EventArgs e)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            string kPerson = (string)Session["kPerson"];
            string dealerName = (string)Session["Dealername"];

            if (String.IsNullOrEmpty(Convert.ToString(Session["kSession"])))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            Dictionary<string, string> dict = Self.vehicleManagementBLL.ExportInventory(kSession, kDealer, dealerName, int.Parse(kPerson));

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