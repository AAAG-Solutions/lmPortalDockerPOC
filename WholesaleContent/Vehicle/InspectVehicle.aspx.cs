using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

using LMWholesale.Common;
using Microsoft.Ajax.Utilities;

namespace LMWholesale
{
    public partial class InspectVehicle : lmPage
    {
        private readonly BLL.WholesaleContent.Vehicle.InspectVehicle inspectVehicle;
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Vehicle.Update updateBLL;
        private readonly BLL.WholesaleData.UploadPhotos uploadPhotos;

        public InspectVehicle()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            inspectVehicle = inspectVehicle ?? new BLL.WholesaleContent.Vehicle.InspectVehicle();
            updateBLL = updateBLL ?? new BLL.WholesaleContent.Vehicle.Update();
            uploadPhotos = uploadPhotos ?? new BLL.WholesaleData.UploadPhotos();
        }

        public static InspectVehicle Self
        {
            get { return instance; }
        }

        private static readonly InspectVehicle instance = new InspectVehicle();

        private static List<DamageLocation> DamageLocationLookup = new List<DamageLocation>()
        {
            new DamageLocation(unchecked((int)0xFF100000),1,"Grill"),
            new DamageLocation(unchecked((int)0xFF300000),2,"Front Bumper"),
            new DamageLocation(unchecked((int)0xFF500000),3,"Headlight"),
            new DamageLocation(unchecked((int)0xFF700000),4,"Hood"),
            new DamageLocation(unchecked((int)0xFF900000),5,"Windshield"),
            new DamageLocation(unchecked((int)0xFFB00000),6,"Left Fender"),
            new DamageLocation(unchecked((int)0xFFD00000),7,"Left Front Wheel"),
            new DamageLocation(unchecked((int)0xFFF00000),8,"Left Front Door"),
            new DamageLocation(unchecked((int)0xFF200000),9,"Left Rear Door"),
            new DamageLocation(unchecked((int)0xFF400000),10,"Left Rocker Panel"),
            new DamageLocation(unchecked((int)0xFF600000),11,"Left Quarter Panel"),
            new DamageLocation(unchecked((int)0xFF800000),12,"Left Rear Wheel"),
            new DamageLocation(unchecked((int)0xFF002000),13,"Left Bed Side"),
            new DamageLocation(unchecked((int)0xFFA00000),14,"Deck Lid"),
            new DamageLocation(unchecked((int)0xFF004000),15,"Tail Gate"),
            new DamageLocation(unchecked((int)0xFF006000),16,"Lift Gate"),
            new DamageLocation(unchecked((int)0xFFC00000),17,"Tail Lamp"),
            new DamageLocation(unchecked((int)0xFFE00000),18,"Rear Bumper"),
            new DamageLocation(unchecked((int)0xFF001000),19,"Right Quarter Panel"),
            new DamageLocation(unchecked((int)0xFF003000),20,"Right Rear Wheel"),
            new DamageLocation(unchecked((int)0xFF008000),21,"Right Bed Side"),
            new DamageLocation(unchecked((int)0xFF005000),22,"Right Rear Door"),
            new DamageLocation(unchecked((int)0xFF007000),23,"Right Front Door"),
            new DamageLocation(unchecked((int)0xFF009000),24,"Right Rocker Panel"),
            new DamageLocation(unchecked((int)0xFF00B000),25,"Right Fender"),
            new DamageLocation(unchecked((int)0xFF00D000),26,"Right Front Wheel"),
            new DamageLocation(unchecked((int)0xFF00F000),27,"Roof"),
            new DamageLocation(unchecked((int)0xFF00A000),28,"Convertible Top")
        };

        private static List<DamageLocation> PaintLocationLookup = new List<DamageLocation>()
        {
            new DamageLocation(unchecked((int)0xFF300000),3,"Front Bumper"),
            new DamageLocation(unchecked((int)0xFF700000),18,"Hood"),
            new DamageLocation(unchecked((int)0xFFB00000),6,"Left Fender"),
            new DamageLocation(unchecked((int)0xFFF00000),7,"Left Front Door"),
            new DamageLocation(unchecked((int)0xFF200000),9,"Left Rear Door"),
            new DamageLocation(unchecked((int)0xFF400000),10,"Left Rocker Panel"),
            new DamageLocation(unchecked((int)0xFF600000),8,"Left Quarter Panel"),
            new DamageLocation(unchecked((int)0xFF002000),5,"Left Bed Side"),
            new DamageLocation(unchecked((int)0xFFA00000),17,"Deck Lid"),
            new DamageLocation(unchecked((int)0xFF004000),21,"Tail Gate"),
            new DamageLocation(unchecked((int)0xFF006000),20,"Lift Gate"),
            new DamageLocation(unchecked((int)0xFFE00000),4,"Rear Bumper"),
            new DamageLocation(unchecked((int)0xFF001000),14,"Right Quarter Panel"),
            new DamageLocation(unchecked((int)0xFF008000),11,"Right Bed Side"),
            new DamageLocation(unchecked((int)0xFF005000),15,"Right Rear Door"),
            new DamageLocation(unchecked((int)0xFF007000),13,"Right Front Door"),
            new DamageLocation(unchecked((int)0xFF009000),16,"Right Rocker Panel"),
            new DamageLocation(unchecked((int)0xFF00B000),12,"Right Fender"),
            new DamageLocation(unchecked((int)0xFF00F000),19,"Roof")
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Condition Report";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
                {
                    string kSession = (string)Session["kSession"];
                    int kDealer = (int)Session["kDealer"];
                    Session["IV_kListing"] = int.Parse(Request.QueryString["kListing"]);

                    DataRow uploadInfo = Self.uploadPhotos.GetListingPaths(kSession, Request.QueryString["kListing"].ToString());
                    Session["IV_VIN"] = uploadInfo["VIN"].ToString();
                    Session["IV_UploadPath"] = uploadInfo["PhotoPathProcessed"].ToString();
                    Session["IV_LastDamagePhotoOrder"] = uploadInfo["LastDamagePhotoOrder"].ToString();

                    if (Session["WholesaleInspector"] != null)
                        lstNAAAGrade.Enabled = Convert.ToBoolean(Session["WholesaleInspector"]);

                    Self.inspectVehicle.FillDropdownValues(kSession);
                    Self.inspectVehicle.FillMappings(kSession);

                    Dealer.lmReturnValue dealerInfo = Self.inspectVehicle.GetDealerInfo(kSession, kDealer);
                    bool hasAutoGrade = false;

                    if (dealerInfo.Result == Dealer.ReturnCode.LM_SUCCESS)
                    {
                        DataTable ProductTable = dealerInfo.Data.Tables["DealerProduct"];
                        hasAutoGrade = ProductTable.AsEnumerable().Where(x => x["QBProductName"].ToString() == "AutoGrade" && !string.IsNullOrEmpty(x["Price"].ToString())).Count() > 0;
                    }

                    hfHasAutoGrade.Value = hasAutoGrade ? "1" : "0";
                    ExteriorInteriorMapping.Value = Util.serializer.Serialize(Self.inspectVehicle.mappings);

                    #region JSGrid Initialization
                    Self.userBLL.CheckDealer();
                    PageSecurityManager.DoPageSecurity(this);
                    kListing.Value = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";

                    jsGridBuilder extGrid = new jsGridBuilder
                    {
                        MethodURL = "InspectVehicle.aspx/ExtDamageSearch",
                        HTMLElement = "jsGridExtDamage",
                        OnRowSelectFunction = "GridRowSelected",
                        OnDoubleClickFunction = "RowDoubleClickExt();",
                        OnClearRowSelectFunction = "ClearRowSelection",
                        Filtering = false,
                        Sorting = false
                    };

                    string extGridColumns = ":Actions::70|:Damage::70|:Condition::100|:Severity::60|:Description::100|:DamagePhoto:Photo:50|!:PhotoInfo::50";
                    List<string> removeColumn = new List<string>();
                    if (Self.userBLL.CheckPermission("ReadOnly") == true)
                        removeColumn.Add("Actions");
                    extGridColumns = string.Join("|", extGridColumns.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Where(r => !removeColumn.Contains(r.Split(":".ToCharArray())[1])).ToArray());
                    extGrid.SetFieldListFromGridDef(extGridColumns, "", true);

                    jsGridBuilder intGrid = new jsGridBuilder
                    {
                        MethodURL = "InspectVehicle.aspx/IntDamageSearch",
                        HTMLElement = "jsGridIntDamage",
                        OnRowSelectFunction = "GridRowSelected",
                        OnDoubleClickFunction = "RowDoubleClickInt();",
                        OnClearRowSelectFunction = "ClearRowSelection",
                        Filtering = false,
                        Sorting = false
                    };

                    string intGridColumns = ":Actions::70|:Damage::70|:Condition::100|:Severity::60|:Description::100|:DamagePhoto:Photo:50|!:PhotoInfo::50";
                    removeColumn = new List<string>();
                    if (Self.userBLL.CheckPermission("ReadOnly") == true)
                        removeColumn.Add("Actions");
                    intGridColumns = string.Join("|", intGridColumns.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Where(r => !removeColumn.Contains(r.Split(":".ToCharArray())[1])).ToArray());
                    intGrid.SetFieldListFromGridDef(intGridColumns, "", true);

                    jsGridBuilder paintGrid = new jsGridBuilder
                    {
                        MethodURL = "InspectVehicle.aspx/PaintSearch",
                        HTMLElement = "jsGridPaint",
                        OnRowSelectFunction = "GridRowSelected",
                        OnDoubleClickFunction = "RowDoubleClickPaint();",
                        OnClearRowSelectFunction = "ClearRowSelection",
                        Filtering = false,
                        Sorting = false
                    };

                    string paintGridColumns = ":Actions::40|:Damage::50|:Condition::50";
                    removeColumn = new List<string>();
                    if (Self.userBLL.CheckPermission("ReadOnly") == true)
                        removeColumn.Add("Actions");
                    paintGridColumns = string.Join("|", paintGridColumns.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Where(r => !removeColumn.Contains(r.Split(":".ToCharArray())[1])).ToArray());
                    paintGrid.SetFieldListFromGridDef(paintGridColumns, "", true);

                    if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "startExtGrid", extGrid.RenderGrid());
                        ClientScript.RegisterStartupScript(this.GetType(), "startIntGrid", intGrid.RenderGrid());
                        ClientScript.RegisterStartupScript(this.GetType(), "startPaintGrid", paintGrid.RenderGrid());
                    }
                    #endregion

                    #region Build Dropdowns
                    #region General Dropdowns
                    string stateString = "|AL:Alabama|AK:Alaska|AZ:Arizona|AR:Arkansas|CA:California|CO:Colorado|CT:Connecticut|DE:Delaware|DC:Washington D.C.|FL:Florida|GA:Georgia|HI:Hawaii|ID:Idaho|IL:Illinois|IN:Indiana|IA:Iowa|KS:Kansas|KY:Kentucky|LA:Louisiana|ME:Maine|MD:Maryland|MA:Massachusetts|MI:Michigan|MN:Minnesota|MS:Mississippi|MO:Missouri|MT:Montana|NE:Nebraska|NV:Nevada|NH:New Hampshire|NJ:New Jersey|NM:New Mexico|NY:New York|NC:North Carolina|ND:North Dakota|OH:Ohio|OK:Oklahoma|OR:Oregon|PA:Pennsylvania|RI:Rhode Island|SC:South Carolina|SD:South Dakota|TN:Tennessee|TX:Texas|UT:Utah|VT:Vermont|VA:Virginia|WA:Washington|WV:West Virgina|WI:Wisconsin|WY:Wyoming|";
                    string keysString = "0:Select # of Keys|1:1|2:2|3:3|4:4";
                    string vehicleTypeString = "0:Any Vehicle Type|8:Balloon|5:Company Car|10:Corporate Fleet|2:Dealer-Owned|7:Dealer-Owned (MCO)|9:Dealer-Owned Commercial|14:Employee Lease|1:Fleet|4:Lease|19:Manufacturer BuyBack|16:Pre-Term Purchase|13:Promotional|6:Rental|3:Repo|18:Theft Recovery|11:Trade In|17:Other|";
                    string fobsString = "-1:Select # of Fobs/Remotes|0:0|1:1|2:2|3:3|4:4|";
                    string naaaString = "-1:Not Set|5.0:5.0|4.9:4.9|4.8:4.8|4.7:4.7|4.6:4.6|4.5:4.5|4.4:4.4|4.3:4.3|4.2:4.2|4.1:4.1|4.0:4.0|3.9:3.9|3.8:3.8|3.7:3.7|3.6:3.6|3.5:3.5|3.4:3.4|3.3:3.3|3.2:3.2|3.1:3.1|3.0:3.0|2.9:2.9|2.8:2.8|2.7:2.7|2.6:2.6|2.5:2.5|2.4:2.4|2.3:2.3|2.2:2.2|2.1:2.1|2.0:2.0|1.9:1.9|1.8:1.8|1.7:1.7|1.6:1.6|1.5:1.5|1.4:1.4|1.3:1.3|1.2:1.2|1.1:1.1|1.0:1.0|0.9:0.9|0.8:0.8|0.7:0.7|0.6:0.6|0.5:0.5|0.4:0.4|0.3:0.3|0.2:0.2|0.1:0.1|0.0:0.0|";

                    WholesaleSystem.PopulateList("[Select Title State]:Select Title State" + stateString, "", "lstTitleState", '|');
                    WholesaleSystem.PopulateList("[Select License Plate State]:Select License Plate State" + stateString, "", "lstLPState", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TitleStatus"), "", "lstTitleStatus", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("OdometerStatus"), "", "lstOdoStat", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("AudioType"), "", "lstAudio", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("InteriorType"), "", "lstIntType", '|');
                    WholesaleSystem.PopulateList("[]" + keysString, "", "lstNumKeys", '|');
                    WholesaleSystem.PopulateList("[]" + vehicleTypeString, "", "lstVehicleType", '|');
                    WholesaleSystem.PopulateList("[]" + fobsString, "", "lstFobs", '|');
                    WholesaleSystem.PopulateList("[]" + naaaString, "", "lstNAAAGrade", '|');
                    #endregion General Dropdowns

                    #region T/W Dropdowns
                    string treadDepthString = "-1:Select Tire Depth|0:0|1:1|2:2|3:3|4:4|5:5|6:6|7:7|8:8|9:9|10:10|11:11|12:12|13:13|14:14|15:15|16:16|17:17|18:18|19:19|20:20|21:21|22:22|23:23|24:24|25:25|26:26|27:27|28:28|29:29|30:30|31:31|32:32|33:33|34:34|35:35|36:36|37:37|38:38|39:39|40:40|41:41|42:42|43:43|44:44|45:45|46:46|47:47|48:48|49:49|50:50|51:51|52:52|53:53|54:54|55:55|56:56|57:57|58:58|59:59|60:60|61:61|62:62|63:63|64:64|";
                    string wheelSizeString = "0:Select Wheel Size|13:13|14:14|15:15|16:16|17:17|18:18|19:19|20:20|21:21|22:22|23:23|24:24|25:25|26:26|27:27|28:28|29:29|30:30|31:31|32:32|33:33|34:34|35:35|36:36|37:37|38:38|39:39|40:40|";

                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireCondition"), "", "lstRFCondition", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireCondition"), "", "lstLFCondition", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireCondition"), "", "lstRRCondition", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireCondition"), "", "lstLRCondition", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireCondition"), "", "lstRRICondition", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireCondition"), "", "lstLRICondition", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireCondition"), "", "lstSPRCondition", '|');

                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireMfg"), "", "lstRRManufact", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireMfg"), "", "lstLFManufact", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireMfg"), "", "lstRFManufact", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireMfg"), "", "lstLRManufact", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireMfg"), "", "lstRRIManufact", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireMfg"), "", "lstLRIManufact", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("TireMfg"), "", "lstSPRManufact", '|');

                    WholesaleSystem.PopulateList("[]" + treadDepthString, "", "lstRRTD", '|');
                    WholesaleSystem.PopulateList("[]" + treadDepthString, "", "lstLFTD", '|');
                    WholesaleSystem.PopulateList("[]" + treadDepthString, "", "lstRFTD", '|');
                    WholesaleSystem.PopulateList("[]" + treadDepthString, "", "lstLRTD", '|');
                    WholesaleSystem.PopulateList("[]" + treadDepthString, "", "lstRRITD", '|');
                    WholesaleSystem.PopulateList("[]" + treadDepthString, "", "lstLRITD", '|');
                    WholesaleSystem.PopulateList("[]" + treadDepthString, "", "lstSPRTD", '|');

                    WholesaleSystem.PopulateList("[]" + wheelSizeString, "", "lstRRWheelSize", '|');
                    WholesaleSystem.PopulateList("[]" + wheelSizeString, "", "lstLFWheelSize", '|');
                    WholesaleSystem.PopulateList("[]" + wheelSizeString, "", "lstRFWheelSize", '|');
                    WholesaleSystem.PopulateList("[]" + wheelSizeString, "", "lstLRWheelSize", '|');
                    WholesaleSystem.PopulateList("[]" + wheelSizeString, "", "lstRRIWheelSize", '|');
                    WholesaleSystem.PopulateList("[]" + wheelSizeString, "", "lstLRIWheelSize", '|');
                    WholesaleSystem.PopulateList("[]" + wheelSizeString, "", "lstSPRWheelSize", '|');
                    #endregion T/W Dropdowns

                    #region ExtDamage Dropdowns
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("ExteriorDamageCategory"), "", "lstExteriorDamageCategory", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("ExteriorDamageCondition"), "", "lstExteriorDamageCondition", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("ExteriorDamageSeverity"), "", "lstExteriorDamageSeverity", '|');
                    if (hasAutoGrade)
                    {
                        for (int i = 1; i < lstExteriorDamageCondition.Items.Count; i++)
                            lstExteriorDamageCondition.Items[i].Attributes.Add("hidden", "hidden");

                        for (int i = 1; i < lstExteriorDamageSeverity.Items.Count; i++)
                            lstExteriorDamageSeverity.Items[i].Attributes.Add("hidden", "hidden");
                    }
                    #endregion ExtDamage Dropdowns

                    #region IntDamage Dropdowns
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("InteriorDamageCategory"), "", "lstInteriorDamageCategory", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("InteriorDamageCondition"), "", "lstInteriorDamageCondition", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("InteriorDamageSeverity"), "", "lstInteriorDamageSeverity", '|');
                    if (hasAutoGrade)
                    {
                        for (int i = 1; i < lstInteriorDamageCondition.Items.Count; i++)
                            lstInteriorDamageCondition.Items[i].Attributes.Add("hidden", "hidden");
                        for (int i = 1; i < lstInteriorDamageSeverity.Items.Count; i++)
                            lstInteriorDamageSeverity.Items[i].Attributes.Add("hidden", "hidden");
                    }
                    #endregion IntDamage Dropdowns

                    #region Paint Dropdowns
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("PriorPaintCategory"), "", "lstPriorPaintCategory", '|');
                    WholesaleSystem.PopulateList(Self.inspectVehicle.PrepareFromXML("PriorPaintCondition"), "", "lstPriorPaintCondition", '|');
                    #endregion Paint Dropdowns
                    #endregion Build Dropdowns

                    #region Fill Existing Data
                    var inspection = Self.inspectVehicle.GetInspectionData(kSession, kDealer, kListing.Value);
                    Listing.lmReturnValue rv = Self.updateBLL.UpdateGet(Convert.ToInt32(kListing.Value), kSession);
                    if (rv.Result == Listing.ReturnCode.LM_SUCCESS)
                    {
                        DataRow dr = rv.Data.Tables[0].Rows[0];
                        string doors = dr["Doors"].ToString();
                        string BodyStyle = dr["BodyType"].ToString();

                        if (BodyStyle.Contains("]"))
                        {
                            BodyStyle = BodyStyle.Substring(0, BodyStyle.IndexOf("]"));
                            if (BodyStyle.Contains("Convertible"))
                            {
                                ExteriorOutline.Src = "/Images/DamagePicker/convertible.png";
                                PaintOutline.Src = "/Images/DamagePicker/convertible.png";
                                hfBodyType.Value = "Convertible";
                            }
                            else if (BodyStyle.Contains("Coupe"))
                            {
                                ExteriorOutline.Src = "/Images/DamagePicker/coupe.png";
                                PaintOutline.Src = "/Images/DamagePicker/coupe.png";
                                hfBodyType.Value = "Coupe";
                            }
                            else if (BodyStyle.Contains("Van"))
                            {
                                ExteriorOutline.Src = "/Images/DamagePicker/van.png";
                                PaintOutline.Src = "/Images/DamagePicker/van.png";
                                hfBodyType.Value = "Van";
                            }
                            else if (BodyStyle.Contains("Truck"))
                            {
                                if (doors.Contains("]"))
                                {
                                    doors = doors.Substring(1, doors.IndexOf("]"));
                                    if (!string.IsNullOrEmpty(doors) && int.TryParse(doors, out _) && Convert.ToInt32(doors.Substring(0,1)) > 2)
                                    {
                                        ExteriorOutline.Src = "/Images/DamagePicker/crew_truck.png";
                                        PaintOutline.Src = "/Images/DamagePicker/crew_truck.png";
                                        hfBodyType.Value = "CrewTruck";
                                    }
                                    else
                                    {
                                        ExteriorOutline.Src = "/Images/DamagePicker/truck.png";
                                        PaintOutline.Src = "/Images/DamagePicker/truck.png";
                                        hfBodyType.Value = "Truck";
                                    }
                                }
                                else
                                {
                                    ExteriorOutline.Src = "/Images/DamagePicker/crew_truck.png";
                                    PaintOutline.Src = "/Images/DamagePicker/crew_truck.png";
                                    hfBodyType.Value = "CrewTruck";
                                }
                            }
                            else if (BodyStyle.Contains("SUV"))
                            {
                                ExteriorOutline.Src = "/Images/DamagePicker/suv.png";
                                PaintOutline.Src = "/Images/DamagePicker/suv.png";
                                hfBodyType.Value = "SUV";
                            }
                            else
                            {
                                ExteriorOutline.Src = "/Images/DamagePicker/sedan.png";
                                PaintOutline.Src = "/Images/DamagePicker/sedan.png";
                                hfBodyType.Value = "Sedan";
                            }
                        }
                        else
                        {
                            ExteriorOutline.Src = "/Images/DamagePicker/sedan.png";
                            PaintOutline.Src = "/Images/DamagePicker/sedan.png";
                            hfBodyType.Value = "Sedan";
                        }
                    }

                    CurrentReportURL.Value = string.IsNullOrEmpty(inspection["ExternalConditionURL"].ToString()) ? inspection["InternalConditionURL"].ToString() : inspection["ExternalConditionURL"].ToString();
                    if (string.IsNullOrEmpty(CurrentReportURL.Value))
                    {
                        NoReportLabel.Style["display"] = "block";
                        currentReportButton.Style["display"] = "none";
                    }

                    #region General Setup
                    labVin.Text = inspection["VIN"].ToString();
                    labVehicle.Text = Self.inspectVehicle.GetVehicleString(kSession, kDealer, int.Parse(kListing.Value), 0);

                    tbLicensePlate.Text = inspection["LicenseNumber"].ToString();
                    cbFrameDamage.Checked = inspection["FrameDamage"].ToString() == "1";
                    cbDriveable.Checked = inspection["Driveable"].ToString() == "1";
                    cbFloodDamage.Checked = inspection["FloodDamage"].ToString() == "1";
                    cbTheft.Checked = inspection["TheftRecovery"].ToString() == "1";
                    cbFireDamage.Checked = inspection["FireDamage"].ToString() == "1";
                    cbPriorPaint.Checked = inspection["PriorPaint"].ToString() == "1";
                    cbAirbagsDep.Checked = inspection["AirbagsDeployed"].ToString() == "1";
                    cbAirbagLight.Checked = inspection["AirbagLight"].ToString() == "1";
                    cbAirbagMiss.Checked = inspection["AirbagMissing"].ToString() == "1";
                    cbSmoker.Checked = inspection["SmokerFlag"].ToString() == "1";
                    cbCEL.Checked = inspection["CheckEngineLight"].ToString() == "1";
                    cbOdor.Checked = inspection["OtherOdor"].ToString() == "1";
                    cbPolice.Checked = inspection["PoliceUse"].ToString() == "1";
                    cbLivery.Checked = inspection["LiveryUse"].ToString() == "1";
                    cbGreyMarket.Checked = inspection["GreyMarket"].ToString() == "1";
                    cbTaxi.Checked = inspection["TaxiUse"].ToString() == "1";
                    cbManuals.Checked = inspection["HasManuals"].ToString() == "1";
                    cbCanadian.Checked = inspection["CanadianVehicle"].ToString() == "1";
                    cb5thWheel.Checked = inspection["Has5thWheel"].ToString() == "1";
                    cbVinPlateIss.Checked = inspection["VINPlateIssue"].ToString() == "1";
                    cbFactoryWarr.Checked = inspection["WarrantyCancelled"].ToString() == "1";
                    cbAlteredEx.Checked = inspection["ExhaustAltered"].ToString() == "1";
                    cbAlteredSus.Checked = inspection["SuspensionAltered"].ToString() == "1";
                    cbTirePressure.Checked = inspection["TPIPresent"].ToString() == "1";
                    cbPowerTrain.Checked = false;
                    cbHailDamage.Checked = inspection["HailDamage"].ToString() == "1";

                    // Set Selected based on their value to match the database return
                    lstTitleState.SelectedValue = inspection["TitleState"].ToString();
                    lstLPState.SelectedValue = inspection["LicenseState"].ToString();
                    lstTitleStatus.SelectedValue = inspection["TitleStatus"].ToString();
                    lstOdoStat.SelectedValue = inspection["WholesaleOdometer"].ToString();
                    lstAudio.SelectedValue = inspection["AudioType"].ToString();
                    lstIntType.SelectedValue = inspection["WholesaleInteriorType"].ToString();
                    lstNumKeys.SelectedValue = inspection["Keys"].ToString();
                    lstVehicleType.SelectedValue = inspection["kWholesaleVehicleType"].ToString();
                    lstFobs.SelectedValue = inspection["KeyFobs"].ToString();
                    lstNAAAGrade.SelectedValue = inspection["IndustryGrade"].ToString();
                    #endregion General Setup

                    #region T/W Setup
                    cbAllTiresMatch.Checked = inspection["AllTiresMatch"].ToString() == "1";
                    #region RF Tire
                    lstRFCondition.SelectedValue = inspection["RFTireCondition"].ToString();
                    lstRFTD.SelectedValue = inspection["RFTireDepth"].ToString();
                    lstRFManufact.SelectedValue = inspection["RFTireMfgr"].ToString();
                    lstRFWheelSize.SelectedValue = inspection["RFTireSize"].ToString();
                    #endregion RF Tire
                    #region LF Tire
                    lstLFCondition.SelectedValue = inspection["LFTireCondition"].ToString();
                    lstLFTD.SelectedValue = inspection["LFTireDepth"].ToString();
                    lstLFManufact.SelectedValue = inspection["LFTireMfgr"].ToString();
                    lstLFWheelSize.SelectedValue = inspection["LFTireSize"].ToString();
                    #endregion LF Tire
                    #region RR Tire
                    lstRRCondition.SelectedValue = inspection["RRTireCondition"].ToString();
                    lstRRTD.SelectedValue = inspection["RRTireDepth"].ToString();
                    lstRRManufact.SelectedValue = inspection["RRTireMfgr"].ToString();
                    lstRRWheelSize.SelectedValue = inspection["RRTireSize"].ToString();
                    #endregion RR Tire
                    #region LR Tire
                    lstLRCondition.SelectedValue = inspection["LRTireCondition"].ToString();
                    lstLRTD.SelectedValue = inspection["LRTireDepth"].ToString();
                    lstLRManufact.SelectedValue = inspection["LRTireMfgr"].ToString();
                    lstLRWheelSize.SelectedValue = inspection["LRTireSize"].ToString();
                    #endregion LR Tire
                    #region RRI Tire
                    lstRRICondition.SelectedValue = inspection["RRInnerTireCondition"].ToString();
                    lstRRITD.SelectedValue = inspection["RRInnerTireDepth"].ToString();
                    lstRRIManufact.SelectedValue = inspection["RRInnerTireMfgr"].ToString();
                    lstRRIWheelSize.SelectedValue = inspection["RRInnerTireSize"].ToString();
                    #endregion RRI Tire
                    #region LRI Tire
                    lstLRICondition.SelectedValue = inspection["LRInnerTireCondition"].ToString();
                    lstLRITD.SelectedValue = inspection["LRInnerTireDepth"].ToString();
                    lstLRIManufact.SelectedValue = inspection["LRInnerTireMfgr"].ToString();
                    lstLRIWheelSize.SelectedValue = inspection["LRInnerTireSize"].ToString();
                    #endregion LRI Tire
                    #region Spare Tire
                    lstSPRCondition.SelectedValue = inspection["SpareTireCondition"].ToString();
                    lstSPRTD.SelectedValue = inspection["SpareTireDepth"].ToString();
                    lstSPRManufact.SelectedValue = inspection["SpareTireMfgr"].ToString();
                    lstSPRWheelSize.SelectedValue = inspection["SpareTireSize"].ToString();
                    #endregion Spare Tire
                    #endregion T/W Setup

                    #endregion Fill Existing Data
                }
            }
        }

        [WebMethod(EnableSession = true)]
        public static string ExtDamageSearch(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            return Self.inspectVehicle.GetGridInfo(Session["kSession"].ToString(), (int)Session["kDealer"], Session["IV_kListing"].ToString(), "ext");
        }

        [WebMethod(EnableSession = true)]
        public static string IntDamageSearch(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            return Self.inspectVehicle.GetGridInfo(Session["kSession"].ToString(), (int)Session["kDealer"], Session["IV_kListing"].ToString(), "int");
        }

        [WebMethod(EnableSession = true)]
        public static string PaintSearch(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            return Self.inspectVehicle.GetGridInfo(Session["kSession"].ToString(), (int)Session["kDealer"], Session["IV_kListing"].ToString(), "paint");
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, bool> SaveInspection(string jsonData)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kListing = (int)Session["IV_kListing"];
            int kDealer = (int)Session["kDealer"];
            string kPerson = (string)Session["kPerson"];
            string isWholesaleInspector = Session["WholesaleInspector"] != null ? Session["WholesaleInspector"].ToString() : "0";

            bool returnBool = true;
            try
            {
                returnBool = Self.inspectVehicle.SaveInspection(jsonData, kSession, kListing.ToString(), kDealer, kPerson, isWholesaleInspector);
            }
            catch (Exception ex)
            {
                returnBool = false;
                WholesaleSystem.Logger.LogLine(kSession, string.Format("Something went wrong: Save Inspection [ Message - {0} | StackTrace - {1} ]", ex.Message, ex.StackTrace));
            }

            return new Dictionary<string, bool>() { { "successOrFail", returnBool} };
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, object> SaveVehicleDamages(string jsonData)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kListing = (int)Session["IV_kListing"];
            int kDealer = (int)Session["kDealer"];

            bool returnBool = true;
            string errorMessage = "";
            try
            {
                returnBool = Self.inspectVehicle.SaveDamages(jsonData, kSession, kListing.ToString(), kDealer.ToString(), Session["IV_UploadPath"].ToString());
            }
            catch (Exception ex)
            {
                returnBool = false;
                errorMessage = string.Format("Something went wrong: Save Vehicle Damages [ Message - {0} | StackTrace - {1} ]", ex.Message, ex.StackTrace);
                WholesaleSystem.Logger.LogLine(kSession, string.Format("Something went wrong: Save Vehicle Damages [ Message - {0} | StackTrace - {1} ]", ex.Message, ex.StackTrace));
            }

            return new Dictionary<string, object>() { { "successOrFail", returnBool }, { "errorMessage", errorMessage } };
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, bool> SavePaintDamages(string jsonData)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kListing = (int)Session["IV_kListing"];
            int kDealer = (int)Session["kDealer"];

            bool returnBool = true;
            try
            {
                returnBool = Self.inspectVehicle.SavePaintDamages(jsonData, kSession, kListing.ToString(), kDealer.ToString());
            }
            catch (Exception ex)
            {
                returnBool = false;
                WholesaleSystem.Logger.LogLine(kSession, string.Format("Something went wrong: Save Paint Damages [ Message - {0} | StackTrace - {1} ]", ex.Message, ex.StackTrace));
            }

            return new Dictionary<string, bool>() { { "successOrFail", returnBool } };
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, object> GetMappingLocation(string bodyType, string pickerType, int height, int width, int xCord, int yCord)
        {
            string imgSrc = "";
            if (bodyType == "Convertible")
                imgSrc = "~/Images/DamagePicker/converible_mask.png";
            else if (bodyType == "Coupe")
                imgSrc = "~/Images/DamagePicker/coupe_mask.png";
            else if (bodyType == "CrewTruck")
                imgSrc = "~/Images/DamagePicker/crew_truck_mask.png";
            else if (bodyType == "Sedan")
                imgSrc = "~/Images/DamagePicker/sedan_mask.png";
            else if (bodyType == "SUV")
                imgSrc = "~/Images/DamagePicker/suv_mask.png";
            else if (bodyType == "Truck")
                imgSrc = "~/Images/DamagePicker/truck_mask.png";
            else if (bodyType == "Van")
                imgSrc = "~/Images/DamagePicker/van_mask.png";

            using (var bitmap = new Bitmap(HttpContext.Current.Server.MapPath(imgSrc)))
            {
                int actualWidth = bitmap.Width;
                int actualHeight = bitmap.Height;

                // Convert click to actual image coordinate
                int scaledX = (int)Math.Round((double)xCord / width * actualWidth);
                int scaledY = (int)Math.Round((double)yCord / height * actualHeight);

                // Clamp to bounds
                scaledX = Clamp(scaledX, 0, actualWidth - 1);
                scaledY = Clamp(scaledY, 0, actualHeight - 1);

                Color clickedColor = bitmap.GetPixel(scaledX, scaledY);

                int region = RegionFromColor(pickerType, clickedColor.ToArgb());

                return new Dictionary<string, object>() 
                { 
                    { "success", true }, 
                    { "region", region }, 
                    { "message", "" } 
                };
            }
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static int RegionFromColor(string type, int color)
        {
            if (type != "paint")
            {
                DamageLocation location = DamageLocationLookup.Where(x => x.ColorValue == color).FirstOrDefault();
                if (location != null)
                    return location.kLocation;
                return -1;
            }
            else
            {
                DamageLocation location = PaintLocationLookup.Where(x => x.ColorValue == color).FirstOrDefault();
                if (location != null)
                    return location.kLocation;
                return -1;
            }
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, object> DamangePhotoUpload(string FileName, string photo)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            var commaIndex = photo.IndexOf(',');
            if (commaIndex >= 0)
            {
                photo = photo.Substring(commaIndex + 1);
            }

            byte[] fileBytes = Convert.FromBase64String(photo);

            var originalName = Path.GetFileName(FileName); // strips any path
            string now = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filenameOut = string.Format("{0}_{1:000}_{2}_Orig.jpg", Session["IV_VIN"], int.Parse(Session["IV_LastDamagePhotoOrder"].ToString()), now);
            var savePath = Path.Combine(Session["IV_UploadPath"].ToString(), "Originals", filenameOut);

            if (!Directory.Exists(Path.Combine(Session["IV_UploadPath"].ToString(), "Originals")))
            {
                Directory.CreateDirectory(Path.Combine(Session["IV_UploadPath"].ToString(), "Originals"));
            }

            try
            {
                File.WriteAllBytes(savePath, fileBytes);
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>()
                {
                    { "Success", false },
                    { "Error", ex.ToString() }
                };
            }

            Session["IV_LastDamagePhotoOrder"] = int.Parse(Session["IV_LastDamagePhotoOrder"].ToString()) + 1;

            return new Dictionary<string, object>()
            {
                { "Success", true },
                { "URL", filenameOut }
            };
        }
    }

    internal class DamageLocation
    {
        public int ColorValue;
        public string LocationName;
        public int kLocation;
        public DamageLocation(int cv, int kval, string desc)
        {
            ColorValue = cv;
            LocationName = desc;
            kLocation = kval;
        }
    }
}