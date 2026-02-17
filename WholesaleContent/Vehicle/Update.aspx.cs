using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using LMWholesale.Common;


namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class Update : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Vehicle.Update updateBLL;

        public Update()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            updateBLL = updateBLL ?? new BLL.WholesaleContent.Vehicle.Update();
        }

        public static Update Self
        {
            get { return instance; }
        }
        private static readonly Update instance = new Update();

        protected void Page_PreRender(object sender, EventArgs args)
        {
            ViewState["RefreshCheck"] = Session["RefreshCheck"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Update Vehicle";
            userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                    BLL.WholesaleUser.WholesaleUser.ClearUser();

                // Need to pass Page Control to async methods due to scope
                Page page = (Page)HttpContext.Current.Handler;
                ContentPlaceHolder mainContent = (ContentPlaceHolder)page.Master.FindControl("MainContent");

                Session["UV_kListing"] = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";

                Dictionary<string, object> selectedObject = new Dictionary<string, object>()
                {
                    { "kListing", "" },
                    { "page", "" },
                    { "usePage", "" }
                };

                if (Session["SelectedVH"] == null)
                {
                    selectedObject["kListing"] = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";
                    Session["SelectedVH"] = Util.serializer.Serialize(selectedObject);
                }
                else
                {
                    selectedObject = (Dictionary<string, object>)Util.serializer.DeserializeObject(Session["SelectedVH"].ToString());
                    selectedObject["kListing"] = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";
                    Session["SelectedVH"] = Util.serializer.Serialize(selectedObject);
                }

                // Instantiate Photo Session variables to be used later
                Session["FileList"] = "";
                Session["NextPhotoNum"] = 0;
                Session["RefreshCheck"] = Server.UrlDecode(DateTime.Now.ToString());

                if (!IsPostBack)
                {
                    if (!String.IsNullOrEmpty(Request.QueryString["kListing"]))
                    {
                        string kSession = (string)Session["kSession"];
                        int kDealer = (int)Session["kDealer"];
                        int kDealerGaggle = (int)Session["kDealerGaggle"];
                        int kListing = int.Parse(Request.QueryString["kListing"]);
                        string pathAndQuery = Request.Url.PathAndQuery;
                        string absUri = Request.Url.AbsoluteUri;
                        string photoGalleryLink = absUri.Substring(0, absUri.Length - pathAndQuery.Length) + "/WholesaleContent/Vehicle/PhotoGallery.aspx?kListing=" + kListing;
                        bool hasCorpDate = ((List<string>)Session["UserPermissions"]).Where(x => x.Contains("ViewCorpInvAdded")).Count() > 0;

                        // Create Tasks
                        LoadVehiclePhotoGallery slowPokePhotos = new LoadVehiclePhotoGallery(kSession, kListing, Self.updateBLL, mainContent, photoGalleryLink);
                        LoadDealerLotLocation slowPokeLot = new LoadDealerLotLocation(kSession, kDealer, Self.updateBLL, mainContent);
                        LoadVehicleNotes slowPokeNotes = new LoadVehicleNotes(kSession, kListing, Self.updateBLL, mainContent);
                        LoadAuctionInfo slowPokeAuction = new LoadAuctionInfo(kSession, kDealer, kListing, Self.updateBLL, Request.QueryString["FocusAuction"], page, mainContent);
                        LoadOptionsInfo slowPokeOptions = new LoadOptionsInfo(kSession, kListing, Self.updateBLL, mainContent);

                        // Register Tasks
                        RegisterAsyncTask(new PageAsyncTask(slowPokePhotos.OnBegin, slowPokePhotos.OnEnd, slowPokePhotos.OnTimeout, "slowPokePhotos", true));
                        RegisterAsyncTask(new PageAsyncTask(slowPokeLot.OnBegin, slowPokeLot.OnEnd, slowPokeLot.OnTimeout, "slowPokeLot", true));
                        RegisterAsyncTask(new PageAsyncTask(slowPokeNotes.OnBegin, slowPokeNotes.OnEnd, slowPokeNotes.OnTimeout, "slowPokeNotes", true));
                        RegisterAsyncTask(new PageAsyncTask(slowPokeAuction.OnBegin, slowPokeAuction.OnEnd, slowPokeAuction.OnTimeout, "slowPokeAuction", true));
                        RegisterAsyncTask(new PageAsyncTask(slowPokeOptions.OnBegin, slowPokeOptions.OnEnd, slowPokeOptions.OnTimeout, "slowPokeOptions", true));

                        // Fire off registered tasks
                        ExecuteRegisteredAsyncTasks();

                        DataTable dealerPrefInvMng = Self.updateBLL.GetDealerInvPref(kSession, kDealer);
                        List<object[]> customPriceFields = new List<object[]>();
                        if (dealerPrefInvMng.Rows.Count != 0)
                        {
                            // This table should only contain 1 row
                            DataRow dr = dealerPrefInvMng.Rows[0];

                            // For now, we need to just get CustomPrice fields
                            for (int i = 1; i < 11; i++)
                            {
                                if (dr[$"CustomPrice{i}Enabled"].ToString() == "1")
                                    customPriceFields.Add(new object[] { i, dr[$"CustomPrice{i}DisplayName"].ToString() });
                            }
                        }

                        // Vehicle Actions Button List
                        actionStartWholesale.OnClientClick = $"javascript: window.location.href=\"/WholesaleContent/Vehicle/StartWholesale.aspx?kListing={kListing}\"; return false";
                        actionEndWholesale.OnClientClick = $"javascript: window.location.href=\"/WholesaleContent/Vehicle/EndWholesale.aspx?kListing={kListing}\"; return false;";
                        actionCR.OnClientClick = $"javascript: window.location.href=\"/WholesaleContent/Vehicle/InspectVehicle.aspx?kListing={kListing}\"; return false;";
                        actionChangeVIN.OnClientClick = $"javascript: window.location.href=\"/WholesaleContent/Vehicle/ChangeVin.aspx?kListing={kListing}\"; return false;";
                        actionModifyPhotos.OnClientClick = $"javascript: window.location.href=\"/WholesaleContent/Vehicle/ModifyPhotos.aspx?kListing={kListing}\"; return false;";
                        actionUploadPhotos.OnClientClick = $"javascript: PhotoPopup({kListing}); return false;";
                        actionDeleteVehicle.OnClientClick = $"javascript: window.location.href=\"/WholesaleContent/Vehicle/Delete.aspx?kListing={kListing}\"; return false;";

                        Self.updateBLL.GetMakeInfo(kSession, "lstAddMake");
                        Listing.lmReturnValue rv = Self.updateBLL.UpdateGet(kListing, kSession);
                        if (rv.Result == Listing.ReturnCode.LM_SUCCESS)
                        {
                            DataRow dr = rv.Data.Tables[0].Rows[0];
                            string status = dr["Status"].ToString();
                            int listPrice = Convert.ToInt32(dr["InvListPrice"]);
                            int costPrice = Convert.ToInt32(dr["InvCost"]);

                            string carfax = $"<a target='_blank' href='{dr["CarfaxPublicLink"]}'><img class='carfaxLink' src='/Images/carfax.gif' title='CARFAX Home Page' id='CarFaxLink' /></a>";
                            HeaderID.Text = kListing.ToString();
                            InvLotLocation.Text = dr["InvLotLocation"].ToString();
                            LotLocation.Text = dr["InvLotLocation"].ToString();
                            HeaderVIN.Text = Util.cleanString((Convert.ToString(dr["VIN"]))) + "&nbsp;" + carfax;
                            HeaderStock.Text = dr["StockNumber"].ToString();
                            HeaderLocation.Text = dr["Dealer"].ToString();
                            tbStatus.Text = Util.FormatString(status, "lowercase");
                            HeaderAge.Text = dr["VehicleAge"].ToString();
                            HeaderRetailListPrice.Text = $"${String.Format("{0:n0}", listPrice)}";
                            HeaderMMR.Text = $"${String.Format("{0:n0}", dr["MMRGoodPrice"])}";
                            HeaderCost.Text = $"${String.Format("{0:n0}", costPrice)}";
                            InternetPrice.Text = $"${String.Format("{0:n0}", dr["InternetPrice"])}";
                            iPrice.Value = dr["InternetPrice"].ToString();
                            StockType.Text = Util.FormatString(dr["ListingType"].ToString(), "lowercase");
                            overImport.Checked = dr["ImportOverride"].ToString() == "1";

                            // #TODO: Commenting out for now as we do not know the status of BB and KBB in the new UI
                            // // Book Info
                            // // Blackbook
                            // Label bb = (Label)mainContent.FindControl(dr["BBCondition"].ToString());
                            // bb.BackColor = System.Drawing.Color.Blue;
                            // bb.ForeColor = System.Drawing.Color.White;
                            // 
                            // BBWXCleanValue.Text = dr["BBWholeXClean"].ToString() != "" ? $"${dr["BBWholeXClean"]}" : "N/A";
                            // BBWCleanValue.Text = dr["BBWholeClean"].ToString() != "" ? $"${dr["BBWholeClean"]}" : "N/A";
                            // BBWAverageValue.Text = dr["BBWholeAverage"].ToString() != "" ? $"${dr["BBWholeAverage"]}" : "N/A";
                            // BBWRoughValue.Text = dr["BBWholeRough"].ToString() != "" ? $"${dr["BBWholeRough"]}" : "N/A";
                            // 
                            // BBRXClean.Text = dr["BBRetailXClean"].ToString() != "" ? $"${dr["BBRetailXClean"]}" : "N/A";
                            // BBRClean.Text = dr["BBRetailClean"].ToString() != "" ? $"${dr["BBRetailClean"]}" : "N/A";
                            // BBRAverage.Text = dr["BBRetailAverage"].ToString() != "" ? $"${dr["BBRetailAverage"]}" : "N/A";
                            // BBRRough.Text = dr["BBRetailRough"].ToString() != "" ? $"${dr["BBRetailRough"]}" : "N/A";
                            // 
                            // BBWholesaleValue.Text = dr[dr["BBCondition"].ToString()].ToString() != "" ? $"${dr[dr["BBCondition"].ToString()]}" : "N/A";
                            // BBMileageValue.Text = dr["BBAdjMiles"].ToString() != "" ? $"${dr["BBAdjMiles"]}" : "N/A";
                            // BBAdjustedValue.Text = dr["BBAdjValue"].ToString() != "" ? $"${dr["BBAdjValue"]}" : "N/A";
                            // 
                            // // Kelly Blue Book
                            // Label kbb = (Label)mainContent.FindControl(dr["KBBCondition"].ToString());
                            // kbb.BackColor = System.Drawing.Color.Blue;
                            // kbb.ForeColor = System.Drawing.Color.White;

                            VehicleMileage.Text = VehicleMiles.Text = String.Format("{0:n0}", Convert.ToInt32(dr["Miles"]));
                            VehicleMileage.Attributes["oninput"] = VehicleMiles.Attributes["oninput"] = WholesaleSystem.onInputNumber;

                            KeyCode.Text = dr["KeyCode"].ToString();
                            VehicleCostGeneral.Text = String.Format("{0:n0}", Convert.ToInt32(dr["InvCost"]));
                            WholesaleStart.Text = String.Format("{0:n0}", Convert.ToInt32(dr["WholesaleStartPrice"]));
                            WholesaleFloor.Text = String.Format("{0:n0}", Convert.ToInt32(dr["WholesaleFloor"]));
                            WholesaleBIN.Text = String.Format("{0:n0}", Convert.ToInt32(dr["WholesaleBuyNow"]));
                            ListPriceGeneral.Text = String.Format("{0:n0}", Convert.ToInt32(dr["InvListPrice"]));
                            MMRGoodPrice.Text = $"${String.Format("{0:n0}", Convert.ToInt32(dr["MMRGoodPrice"]))}";
                            MSRP.Text = String.Format("{0:n0}", Convert.ToInt32(dr["MSRP"].ToString()));
                            InternetPriceGeneral.Text = String.Format("{0:n0}", Convert.ToInt32(dr["InternetPrice"]));
                            Payment.Text = dr["Payment"].ToString() == "" ? "0" : String.Format("{0:n0}", Convert.ToInt32(dr["Payment"]));
                            InvoicePrice.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomInvoice"]));

                            // Set Price fields to only contain digits
                            VehicleCostGeneral.Attributes["oninput"] = ListPriceGeneral.Attributes["oninput"] = MSRP.Attributes["oninput"] = InternetPriceGeneral.Attributes["oninput"] =
                            InvoicePrice.Attributes["oninput"] = Payment.Attributes["oninput"] = WholesaleStart.Attributes["oninput"] = WholesaleFloor.Attributes["oninput"] = WholesaleBIN.Attributes["oninput"] =
                            Class2EngineSizeTxt.Attributes["oninput"] = Class3EngineSizeTxt.Attributes["oninput"] = WholesaleSystem.onInputNumber;

                            // have to loop through custom price settings if enabled
                            CustomPriceFieldsSet(dr, customPriceFields);

                            // Vehicle Info
                            // Year
                            WholesaleSystem.PopulateList(dr["YearLst"].ToString(), "-- Select Year --", '|', YearLst, YearTxt);
                            WholesaleSystem.PopulateList(dr["MakeLst"].ToString(), "-- Select Make --", '|', MakeLst, MakeTxt, kMake);

                            string modelLst = dr["ModelLst"].ToString();
                            if (modelLst.Length > 0 && modelLst.CompareTo("[]") != 0)
                            {
                                string model_not_found = WholesaleSystem.PopulateList(modelLst, "-- Select Model --", '|', ModelLst, ModelTxt, kModel);
                                if (model_not_found != "" && ModelLst.Visible)
                                {
                                    ModelLst.ClearSelection();
                                    ModelLst.SelectedIndex = 0;
                                }
                            }

                            string styleDefault = SetStyleArrays(dr["Style2"].ToString());
                            if (YearLst.Visible)
                            {
                                if (ModelLst.SelectedIndex > 0)
                                    //FillStyles(kSession, dr["StyleLst"].ToString());
                                    FillStyles(kSession);
                                else
                                    PopulateStyles(styleDefault);
                            }
                            else
                                PopulateStyles(styleDefault);

                            // YMMS Logic
                            string year = GetListText(YearLst, YearTxt);
                            string make = GetListText(MakeLst, MakeTxt);
                            string model = GetListText(ModelLst, ModelTxt);

                            if (year == "")
                                MakeLst.Enabled = false;

                            string overstring = dr["SubModel"].ToString();
                            string longStyleName = dr["StyleLong"].ToString();

                            chkOverrideStyle.Checked = dr["StyleOverride"].ToString() == "1";
                            if (chkOverrideStyle.Checked)
                                StyleLst.Enabled = false;

                            string mode = dr["ModelStyleOverride"].ToString();
                            string style = ApplyStyleSelection(mode, overstring, longStyleName, dr["ImpStyleLong"].ToString(), dr["ImpSubModel"].ToString());

                            VehicleYear.Text = year;
                            VehicleMake.Text = make;
                            VehicleModel.Text = model;
                            VehicleStyle.Text = style;

                            headerVehicle.Text = $"{year} {make} {model} {style}";
                            VehicleNoteTitle.Text = $"{year} {make} {model} {style} ( {Util.cleanString((Convert.ToString(dr["VIN"])))} )";
                            FullDesc.Enabled = LimitDesc.Enabled = dr["StyleOverride"].ToString() == "1";

                            // Engine
                            string overrideVal = WholesaleSystem.PopulateList(dr["Engine"].ToString(), "-- Select Engine --", '|', EngineLst, VehicleEngine);
                            if (overrideVal.Length > 0)
                            {
                                chkOverrideEngine.Checked = true;
                                EngineLst.Enabled = false;
                                OverrideEngineDesc.Text = overrideVal;
                                VehicleEngine.Enabled = true;
                            }
                            overrideVal = "";

                            FillDrivetrainInfo(dr);

                            WholesaleSystem.PopulateList(dr["Body"].ToString(), "-- Select Body --", '|', BodyLst, VehicleBody);
                            WholesaleSystem.PopulateList(dr["ExteriorColor"].ToString(), "-- Select Exterior Color --", '|', ExtColorLst);
                            WholesaleSystem.PopulateList(dr["InteriorColor"].ToString(), "-- Select Interior Color --", '|', IntColorLst);
                            WholesaleSystem.PopulateList(dr["Doors"].ToString(), "-- Select Doors --", '|', DoorLst);
                            WholesaleSystem.PopulateList(dr["Roof"].ToString(), "-- Select Roof --", '|', RoofLst);
                            WholesaleSystem.PopulateList(dr["StatusList"].ToString(), "-- Select Status --", '|', VehicleStatusLst);
                            VehicleListStatusLst.Items.FindByText(Util.FormatString(dr["ListingType"].ToString(), "lowercase")).Selected = true;
                            kVehicleListStatus.Value = VehicleListStatusLst.SelectedItem.Value;

                            // Special logic for CorpInvAdded for DTN/Open Sales
                            if (hasCorpDate)
                            {
                                CorpInvAdded.Style["display"] = "table-row";
                                if (dr["CorpInvAdded"].ToString() != "")
                                    CorpInvAddedDate.Text = DateTime.Parse(dr["CorpInvAdded"].ToString(), new CultureInfo("en-US")).ToString("yyyy-MM-dd");
                                else
                                    CorpInvAddedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            }
                            else
                                CorpInvAddedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                            // Certification Logic
                            WholesaleSystem.PopulateList(Self.updateBLL.CertificationListGet(kSession), "", '|', lstCertificationType, null, kCertificationType);
                            ListItem listItem = lstCertificationType.Items.FindByValue(dr["kCertification"].ToString());
                            
                            if (listItem != null)
                            {
                                kCertificationType.Value = listItem.Value;
                                listItem.Selected = true;
                            }
                            else
                                kCertificationType.Value = "1";
                            
                            CertificationNumber.Text = dr["CertificationNumber"].ToString();
                            if (dr["CertificationDate"].ToString() != "")
                                CertificationDate.Text = DateTime.Parse(dr["CertificationDate"].ToString(), new CultureInfo("en-US")).ToString("yyyy-MM-dd");
                            else
                                CertificationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                            // Title Info
                            ListingTitle.Text = dr["ShortDesc"].ToString();
                            string temp = dr["DetailDesc"].ToString();
                            if (temp.Contains("###DETAILFILE=") && File.Exists(temp.Substring(14)))
                                VehicleDescriptionGeneral.Text = Self.updateBLL.StripTagsCharArray(File.ReadAllText(temp.Substring(14)));
                            else if (temp.Contains("###DETAILFILE=") && !File.Exists(temp.Substring(14)))
                                VehicleDescriptionGeneral.Text = "";
                            else
                                VehicleDescriptionGeneral.Text = Self.updateBLL.StripTagsCharArray(temp);

                            temp = dr["ConditionDesc"].ToString();
                            if (temp.Contains("###DETAILFILE=") && File.Exists(temp.Substring(14)))
                                txtConditionNotes.Text = Self.updateBLL.StripTagsCharArray(File.ReadAllText(temp.Substring(14)));
                            else if (temp.Contains("###DETAILFILE=") && !File.Exists(temp.Substring(14)))
                                txtConditionNotes.Text = "";
                            else
                                txtConditionNotes.Text = Self.updateBLL.StripTagsCharArray(temp);

                            temp = dr["OptionDesc"].ToString();
                            if (temp.Contains("###DETAILFILE=") && File.Exists(temp.Substring(14)))
                                OptionNotes.Text = Self.updateBLL.StripTagsCharArray(File.ReadAllText(temp.Substring(14)));
                            else if (temp.Contains("###DETAILFILE=") && !File.Exists(temp.Substring(14)))
                                OptionNotes.Text = "";
                            else
                                OptionNotes.Text = Self.updateBLL.StripTagsCharArray(temp);

                            MechanicalNotes.Text = dr["Mechanical"].ToString();
                            SafetyNotes.Text = dr["Safety"].ToString();
                            ExtNotes.Text = dr["Exterior"].ToString();
                            IntNotes.Text = dr["Interior"].ToString();

                            if (dr["StyleID"].ToString() == "0")
                                Response.Write("<script>alert('This vehicle has not been styled yet! Please make sure to style out this vehicle!');</script>");
                        }
                        else
                        {
                            Response.Write($"<script>alert('Unable to perform request due to the following error: {rv.ResultString}. Please try again or call support for assistance.');</script>");
                        }
                    }
                }
            }
        }

        #region WebMethods
        [WebMethod]
        public static int VehicleNoteSet(string note)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            Dictionary<string, object> vehicleNote = (Dictionary<string, object>)Util.serializer.DeserializeObject(note);

            string kSession = (string)Session["kSession"];
            return Self.updateBLL.ListingVehicleNoteSet(kSession, vehicleNote);
        }

        [WebMethod]
        public static Dictionary<string, object> GetList(string json)
        {
            Dictionary<string, object> info = (Dictionary<string, object>)Util.serializer.DeserializeObject(json);
            HttpSessionState Session = HttpContext.Current.Session;

            if (info["type"].Equals("Year"))
                Value = Self.updateBLL.GetMakeInfo(Session["kSession"].ToString(), "", true);
            else if (info["type"].Equals("Model"))
                Value = Self.updateBLL.GetModelInfo(Session["kSession"].ToString(), info["year"].ToString(), info["make"].ToString(), "3");
            else if (info["type"].Equals("Style"))
                Value = Self.updateBLL.GetStyleInfo(Session["kSession"].ToString(), info["year"].ToString(), info["make"].ToString(), info["model"].ToString(), "1");

            if (string.IsNullOrEmpty(Value.ToString()))
            {
                Message = "Something went wrong! Please contact support for assistance!";
                IsSuccess = false;
                return ReturnResponse();
            }

            return ReturnResponse();
        }

        [WebMethod]
        public static Dictionary<string, object> GetModelList(string json)
        {
            Dictionary<string, object> info = (Dictionary<string, object>)Util.serializer.DeserializeObject(json);
            HttpSessionState Session = HttpContext.Current.Session;

            Value = Self.updateBLL.GetModelInfoLst(Session["kSession"].ToString(), info["make"].ToString());
            if (string.IsNullOrEmpty(Value.ToString()))
            {
                Message = "Something went wrong! Please contact support for assistance!";
                IsSuccess = false;
                return ReturnResponse();
            }

            return ReturnResponse();
        }

        [WebMethod]
        public static Dictionary<string, object> AddMakeModel(string data)
        {
            Dictionary<string, object> mmData = (Dictionary<string, object>)Util.serializer.DeserializeObject(data);
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];

            IsSuccess = Self.updateBLL.AddMakeModel(kSession, int.Parse(mmData["kMake"].ToString()), mmData["Make"].ToString(), mmData["Model"].ToString());
            if (!IsSuccess)
            {
                if (mmData["op"].ToString() == "Make")
                    Message = "Unable to add new Make! Please contact support for assistance!";
                else
                    Message = "Unable to add new Model! Please contact support for assistance!";
                return ReturnResponse();
            }

            if (mmData["op"].ToString() == "Make")
            {
                Value = Self.updateBLL.GetMakeInfo(kSession, "", true);
                Message = $"Successfully added new Make: {mmData["Make"]}";
                return ReturnResponse();
            }
            else
            {
                Value = Self.updateBLL.GetModelInfoLst(kSession, mmData["kMake"].ToString());
                Message = $"Successfully added new Model for REPLACE_MAKE: {mmData["Model"]}";
                return ReturnResponse();
            }
        }

        [WebMethod]
        public static Dictionary<string, object> VehicleClassUpdate(string data)
        {
            Dictionary<string, object> type = (Dictionary<string, object>)Util.serializer.DeserializeObject(data);
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];

            IsSuccess = Self.updateBLL.VehicleClassUpdate(kSession, type["kListing"].ToString(), type["kVehicleClass"].ToString());
            if (!IsSuccess)
                Message = "Something went wrong with adjusting the Vehicle Type! Please contact support for assistance!";
            else
                Message = "Successfully adjusted the Vehicle Type!";

            return ReturnResponse();
        }

        //protected void YearLst_SelectedIndexChange(object sender, EventArgs args)
        //{
        //    string year = YearLst.SelectedItem.Value;
        //
        //    // Clear any items if previously selected
        //    MakeLst.Items.Clear();
        //    ModelLst.Items.Clear();
        //    StyleLst.Items.Clear();
        //
        //    if (year != "-- Select Year --")
        //    {
        //        Self.updateBLL.GetMakeInfo(Session["kSession"].ToString(), "MakeLst", false);
        //        //string[] makesplit = makes.Split('|');
        //        //
        //        //foreach (string info in makesplit)
        //        //{
        //        //    string[] infosplit = info.Split(':');
        //        //    MakeLst.Items.Add(new ListItem(infosplit[1], infosplit[0]));
        //        //}
        //    }
        //}
        //
        //protected void MakeLst_SelectedIndexChange(object sender, EventArgs args)
        //{
        //    string year = YearLst.SelectedItem.Value;
        //    string make = MakeLst.SelectedItem.Value;
        //
        //    ModelLst.Items.Clear();
        //    StyleLst.Items.Clear();
        //
        //    if (make != "")
        //    {
        //        Self.updateBLL.GetModelInfo(Session["kSession"].ToString(), year, make, "3");
        //    }
        //}

        protected void btnSubmitButton(object sender, EventArgs args)
        {
            HttpContext current = HttpContext.Current;
            HttpSessionState Session = current.Session;

            if (ViewState["RefreshCheck"].ToString() == Session["RefreshCheck"].ToString())
            {
                string kCertification = "1";
                string CertNum = "";
                string CertDate = "";
                // Dealer CPO || CPO
                if (kVehicleListStatus.Value == "30" || kVehicleListStatus.Value == "33")
                {
                    kCertification = kCertificationType.Value;
                    CertNum = CertificationNumber.Text;
                    CertDate = CertificationDate.Text;
                }
                else if (Session["kDealer"].ToString() == "16348" && VehicleStatusLst.SelectedItem.Value == "9")
                {
                    kCertification = "68";
                    CertDate = CertificationDate.Text;
                }

                try
                {
                    Dictionary<string, object> dataIn = new Dictionary<string, object>()
                    {
                        // General Vehicle Info
                        { "kDealer", Session["kDealer"] },
                        { "kListing", current.Request["kListing"] },

                        // General Info
                        { "StockNumber", HeaderStock.Text },
                        { "MotorYear", GetListVal(YearLst, YearTxt) },
                        { "kMake", GetListVal(MakeLst, MakeTxt, kMake) },
                        { "kModel", GetListVal(ModelLst, ModelTxt, kModel) },
                        { "Style", FullDesc.Text },
                        { "StyleOverride", chkOverrideStyle.Checked ? 1 : 0 },
                        { "SubModel", FullDesc.Text },
                        { "kListType", kVehicleListStatus.Value },
                        { "Status", VehicleStatusLst.SelectedItem.Value },
                        { "Miles", CleanIntField(VehicleMileage.Text) },
                        { "InvLotLocation", InvLotLocation.Text },
                        { "ImportOverride", overImport.Checked ? "1" : "0" },
                        { "KeyCode", KeyCode.Text },

                        // CorpInvAdded
                        //{ "CorpInvAdded", CorpInvAddedDate.Text },

                        // Certification Logic
                        { "kCertification", kCertification },
                        { "CertificationNumber", CertNum },
                        { "CertificationDate", CertDate },

                        // Pricing Values
                        { "InvCost", CleanIntField(VehicleCostGeneral.Text) },
                        { "MSRP", CleanIntField(MSRP.Text) },
                        { "InvListPrice", CleanIntField(ListPriceGeneral.Text) },
                        { "InternetPrice", CleanIntField(InternetPriceGeneral.Text) },
                        { "CustomInvoice", CleanIntField(InvoicePrice.Text) },
                        { "Payment", CleanIntField(Payment.Text) },
                        { "WholesaleFloor", CleanIntField(WholesaleFloor.Text) },
                        { "WholesaleBuyNow", CleanIntField(WholesaleBIN.Text) },
                        { "WholesaleStartPrice", CleanIntField(WholesaleStart.Text) },
                        { "CustomPrice1", CustomPriceValue1.Visible == true ? CleanIntField(CustomPriceValue1.Text) : "0" },
                        { "CustomPrice2", CustomPriceValue2.Visible == true ? CleanIntField(CustomPriceValue2.Text) : "0" },
                        { "CustomPrice3", CustomPriceValue3.Visible == true ? CleanIntField(CustomPriceValue3.Text) : "0" },
                        { "CustomPrice4", CustomPriceValue4.Visible == true ? CleanIntField(CustomPriceValue4.Text) : "0" },
                        { "CustomPrice5", CustomPriceValue5.Visible == true ? CleanIntField(CustomPriceValue5.Text) : "0" },
                        { "CustomPrice6", CustomPriceValue6.Visible == true ? CleanIntField(CustomPriceValue6.Text) : "0" },
                        { "CustomPrice7", CustomPriceValue7.Visible == true ? CleanIntField(CustomPriceValue7.Text) : "0" },
                        { "CustomPrice8", CustomPriceValue8.Visible == true ? CleanIntField(CustomPriceValue8.Text) : "0" },
                        { "CustomPrice9", CustomPriceValue9.Visible == true ? CleanIntField(CustomPriceValue9.Text) : "0" },
                        { "CustomPrice10", CustomPriceValue10.Visible == true ? CleanIntField(CustomPriceValue10.Text) : "0" },

                        // Notes
                        { "DetailDesc", VehicleDescriptionGeneral.Text },
                        { "ConditionDesc", txtConditionNotes.Text },

                        // Color/Body Info
                        { "kColorExterior", ExtColorLst.SelectedItem.Value},
                        { "kColorInterior", IntColorLst.SelectedItem.Value },
                        { "kDoors", GetListVal(DoorLst, DoorText, kDoor) },
                        { "kRoof", GetListVal(RoofLst, RoofText, kRoof) },
                        { "Body", VehicleBody.Text },
                        { "kBodyType", GetListVal(VehicleTypeLst, VehicleTypeTxt, kBodyType) },

                        // Drivetrain Info
                        { "Engine", chkOverrideEngine.Checked ? OverrideEngineDesc.Text : VehicleEngine.Text },
                        { "kFuelType", Class1FuelLst.Items.Count > 1 ? GetListVal(Class1FuelLst, Class1FuelText, kFuel) : GetListVal(Class1FuelLst, Class1FuelText, kFuel) },
                        { "kDrive", kDrive.Value == "" ? Class1DriveTrainLst.SelectedItem.Value : kDrive.Value },
                        { "kCyl", GetListVal(Class1CylinderLst, null, kCyl) },
                        { "kTransmission", Class1TransmissionLst.SelectedItem.Value },

                        // Options Info
                        { "OptionList", optionsLst.Value },
                        { "OptionDesc", OptionNotes.Text }
                    };

                    if (kVehicleClass.Value == "2")
                    {
                        dataIn.Add("kCycleType", Class2Cycle.SelectedItem.Value);
                        dataIn.Add("EngineSize", Class2EngineSizeTxt.Text);
                        dataIn["kColorExterior"] = Class2Color.SelectedItem.Value;
                    }
                    else if (kVehicleClass.Value == "3")
                    {
                        dataIn.Add("kTruckType", Class3TruckTypeLst.SelectedItem.Value);
                        dataIn.Add("kTruckClass", Class3TruckClassLst.SelectedItem.Value);
                        dataIn.Add("kEngineMake", Class3EngineMakeLst.SelectedItem.Value);
                        dataIn.Add("EngineSize", Class3EngineSizeTxt.Text);
                        dataIn.Add("kTranSpeed", Class3TransmissionSpeedLst.SelectedItem.Value);
                        dataIn.Add("kNumAxle", Class3AxleLst.SelectedItem.Value);
                        dataIn.Add("kSuspension", Class3SuspensionTypeLst.SelectedItem.Value);
                        dataIn.Add("kFuelType", Class3FuelLst.SelectedItem.Value);
                        dataIn.Add("kTireSize", Class3TireSizeLst.SelectedItem.Value);
                        dataIn["kCyl"] = Class3CylnderLst.SelectedItem.Value;
                        dataIn["kTransmission"] = Class3TransmissionLst.SelectedItem.Value;
                    }

                    string styleId = StyleLst.SelectedIndex == 0 || StyleLst.SelectedIndex == -1 ? "0" : StyleLst.SelectedItem.Value;
                    string styleTxt = styleId == "0" ? "" : StyleLst.SelectedItem.Text;

                    dataIn.Add("StyleID", styleId);
                    dataIn["Style"] = styleTxt;

                    if (chkOverrideStyle.Checked)
                    {
                        dataIn["Style"] = FullDesc.Text;
                        dataIn["SubModel"] = LimitDesc.Text;
                    }

                    string errorMsg = "";
                    if (!Self.updateBLL.ExpandedDetailSave(Session["kSession"].ToString(), Util.serializer.Serialize(dataIn), ref errorMsg))
                    {
                        IsSuccess = false;
                    }
                    Message = errorMsg;

                    if (!IsSuccess)
                        Response.Write("<script>alert('Something has gone wrong in the save process:\\n\"" + Message + "');toggleLoading(false, \"\");</script>");
                    else
                    {
                        string currentPage = string.Format("/WholesaleContent/Vehicle/Update.aspx?kListing={0}", current.Request["kListing"]);
                        Response.Write("<script>if(!alert('Data saved successfully. Refreshing...')){window.location.href = \"" + currentPage + "\";}</script>");
                        Session["RefreshCheck"] = Server.UrlDecode(DateTime.Now.ToString());
                    }
                }
                catch (Exception ex)
                {
                    WholesaleSystem.Logger.LogLine(Session["kSession"].ToString(), string.Format("Something went wrong: Update Vehicle Save Details [ Message - {0} | StackTrace - {1} ]", ex.Message, ex.StackTrace));
                    Response.Write("<script>alert('Something has gone wrong in the save process:\\n\"" + ex.Message + "');</script>");
                }
            }
        }
        #endregion

        #region Private Functions
        private string CleanIntField(string item)
        {
            if (item.Contains(','))
                return item.Replace(",", "");
            else if (item != "")
                return item;

            // return default 0 if we fail
            return "0";
        }

        private string SetStyleArrays(string styles)
        {
            string defValue = "";
            string selections;
            int pos;

            if (styles.Length < 1)
                return defValue;

            if (styles.StartsWith("[]"))
                selections = styles.Substring(2);
            else
            {
                pos = styles.IndexOf("]");

                if (pos >= 0)
                {
                    defValue = styles.Substring(1, pos - 1);
                    selections = styles.Substring(pos + 1);
                }
                else
                    selections = styles;
            }

           string[] items = selections.TrimEnd('|').Split('|');
            if (items.Length > 0)
            {
                foreach (string item in items)
                {
                    if (item.Length > 0)
                    {
                        string id;
                        string expStyle;
                        string subModel;
                        string text;

                        #if DEBUG
                        string copy = item;
                        #endif

                        pos = item.IndexOf(":");
                        id = item.Substring(0, pos);        // StyleID
                        text = item.Substring(pos + 1);     // Rest of Text

                        pos = text.LastIndexOf(":");
                        subModel = text.Substring(pos + 1); // Sub Model
                        text = text.Substring(0, pos);         // Rest of Text

                        pos = text.LastIndexOf(":");
                        expStyle = text.Substring(pos + 1); // Modified Style
                        if (pos != -1)
                            text = text.Substring(0, pos);         // Rest of Text?

                        MasterStyleLst.Items.Add(new ListItem( text, id ));
                        ExpStyleLst.Items.Add(new ListItem( expStyle, id ));
                        SubModelLst.Items.Add(new ListItem( subModel, id ));
                    }
                }
            }

            return defValue;
        }

        private string PopulateStyles(string defVal)
        {
            string returnValue = "";
            if (MasterStyleLst.Items.Count == 0)
            {
                StyleTxt.Text = "-- No Style Found --";
                StyleHidden.Value = "";
            }
            else if (MasterStyleLst.Items.Count == 1 && !YearLst.Visible)
            {
                StyleTxt.Text = MasterStyleLst.Items[0].Text;
                StyleHidden.Value = MasterStyleLst.Items[0].Value;

                StyleLst.Visible = false;
                StyleTxt.Visible = true;
            }
            else
            {
                StyleTxt.Visible = false;
                StyleLst.Visible = true;

                // Clear out existing items, if any
                StyleLst.Items.Clear();
                StyleLst.Items.Insert(0, new ListItem("-- Select Style --", "0"));

                string model = GetListText(ModelLst, ModelTxt);
                if (model.Length > 0)
                {
                    model += ":";
                    for (int i = 0; i < MasterStyleLst.Items.Count; i++)
                    {
                        if (MasterStyleLst.Items[i].Text.StartsWith(model))
                            StyleLst.Items.Add(new ListItem(MasterStyleLst.Items[i].Text, MasterStyleLst.Items[i].Value));
                    }

                    if (defVal.Length > 0)
                    {
                        ListItem item = StyleLst.Items.FindByText(defVal);
                        if (item != null)
                            item.Selected = true;
                        else
                            returnValue = defVal;
                    }
                    else
                        StyleLst.SelectedIndex = 0;
                }
                else
                    StyleLst.Enabled = false;
            }

            return returnValue;
        }

        //private void FillStyles(string kSession, string style)
        private void FillStyles(string kSession)
        {
            string year = YearLst.SelectedItem.Text;
            string make = MakeLst.SelectedItem.Text;
            string model = ModelLst.SelectedItem.Text;

            StyleLst.Items.Clear();
            if (model != "-- Select Model --" && model != "--NO MODELS FOUND--")
            {
                 DataTable StylesTbl = Self.updateBLL.GetStyleInfo(kSession, year, make, model, "3", true);
                if (StylesTbl.Rows.Count > 0)
                {
                    StyleTxt.Visible = false;
                    StyleLst.Visible = true;

                    StringBuilder sbStyle = new StringBuilder();
                    //sbStyle.Append("[" + style + "]");
                    foreach (DataRow row in StylesTbl.Rows)
                    {
                        sbStyle.Append(row["StyleID"] + ":");
                        //sbStyle.Append(model + ":" + row["Styles"] + ":" + row["Styles"] + ":");
                        sbStyle.Append(model + row["Styles"] + ":" + row["Styles"] + ":");
                        sbStyle.Append(_TrimStyle(sbStyle.ToString()) + "|");
                    }

                    string defVal = SetStyleArrays(sbStyle.ToString());
                    PopulateStyles(defVal);

                    if (chkOverrideStyle.Checked)
                        StyleLst.Enabled = false;
                }
            }
        }

        private string _TrimStyle(string style)
        {
            string returnVal = "";
            string numbers = "123456789";
            if (string.IsNullOrEmpty(style))
            {
                if (style.Length <= 12)
                    returnVal = style;
                else
                {
                    if (numbers.IndexOf(style.Substring(0, 1)) > -1)
                    {
                        if (style.Substring(1,2).ToUpper().CompareTo("DR") == 0)
                            style = style.Substring(3).TrimStart();
                    }

                    if (style.Length <= 12)
                        returnVal = style;
                    else
                    {
                        while(style.Length > 12)
                        {
                            int pos = style.LastIndexOf(" ");
                            if (pos > -1)
                            {
                                style = style.Substring(0, pos);
                                style.TrimEnd();
                            }
                            else
                                return "";
                        }
                        returnVal = style;
                    }
                }
            }

            if (returnVal == "-- No Style Found --")
                return "";

            // Default return
            return "";
        }

        private void CustomPriceFieldsSet(DataRow dr, List<object[]> fields)
        {
            bool enabled = false;
            foreach (object[] customPrice in fields)
            {
                switch (customPrice[0])
                {
                    case 1:
                        CustomPriceName1.Text = $"{customPrice[1]}: ";
                        CustomPriceValue1.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice1"]));
                        CustomPriceName1.Style["display"] = "initial";
                        CustomPriceValue1.Style["display"] = "initial";
                        CustomPriceValue1.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp1.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 2:
                        CustomPriceName2.Text = $"{customPrice[1]}: ";
                        CustomPriceValue2.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice2"]));
                        CustomPriceName2.Style["display"] = "initial";
                        CustomPriceValue2.Style["display"] = "initial";
                        CustomPriceValue2.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp2.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 3:
                        CustomPriceName3.Text = $"{customPrice[1]}: ";
                        CustomPriceValue3.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice3"]));
                        CustomPriceName3.Style["display"] = "initial";
                        CustomPriceValue3.Style["display"] = "initial";
                        CustomPriceValue3.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp3.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 4:
                        CustomPriceName4.Text = $"{customPrice[1]}: ";
                        CustomPriceValue4.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice4"]));
                        CustomPriceName4.Style["display"] = "initial";
                        CustomPriceValue4.Style["display"] = "initial";
                        CustomPriceValue4.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp4.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 5:
                        CustomPriceName5.Text = $"{customPrice[1]}: ";
                        CustomPriceValue5.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice5"]));
                        CustomPriceName5.Style["display"] = "initial";
                        CustomPriceValue5.Style["display"] = "initial";
                        CustomPriceValue5.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp5.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 6:
                        CustomPriceName6.Text = $"{customPrice[1]}: ";
                        CustomPriceValue6.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice6"]));
                        CustomPriceName6.Style["display"] = "initial";
                        CustomPriceValue6.Style["display"] = "initial";
                        CustomPriceValue6.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp6.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 7:
                        CustomPriceName7.Text = $"{customPrice[1]}: ";
                        CustomPriceValue7.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice7"]));
                        CustomPriceName7.Style["display"] = "initial";
                        CustomPriceValue7.Style["display"] = "initial";
                        CustomPriceValue7.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp7.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 8:
                        CustomPriceName8.Text = $"{customPrice[1]}: ";
                        CustomPriceValue8.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice8"]));
                        CustomPriceName8.Style["display"] = "initial";
                        CustomPriceValue8.Style["display"] = "initial";
                        CustomPriceValue8.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp8.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 9:
                        CustomPriceName9.Text = $"{customPrice[1]}: ";
                        CustomPriceValue9.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice9"]));
                        CustomPriceName9.Style["display"] = "initial";
                        CustomPriceValue9.Style["display"] = "initial";
                        CustomPriceValue9.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp9.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    case 10:
                        CustomPriceName10.Text = $"{customPrice[1]}: ";
                        CustomPriceValue10.Text = String.Format("{0:n0}", Convert.ToInt32(dr["CustomPrice10"]));
                        CustomPriceName10.Style["display"] = "initial";
                        CustomPriceValue10.Style["display"] = "initial";
                        CustomPriceValue10.Attributes["oninput"] = WholesaleSystem.onInputNumber;
                        cp10.Style["display"] = "initlal";
                        enabled = true;
                        break;
                    default:
                        enabled = false;
                        break;
                }
            }
            if (!enabled)
                customPrice.Visible = false;
        }

        private void FillDrivetrainInfo(DataRow dr)
        {
            int kClass = int.Parse(dr["kVehicleClass"].ToString());
            kVehicleClass.Value = kClass.ToString();
            VehicleTypeName.Text = dr["VehicleClassName"].ToString();
            switch (kClass)
            {
                case 2:
                    colorBodyTab.Style["display"] = "none";
                    VehicleClass2Cycle.Style["display"] = "table-row";
                    VehicleClass2Color.Style["display"] = "table-row";

                    // Class Type
                    WholesaleSystem.PopulateList(dr["CycleType"].ToString(), "- Select Cycle Type --", '|', Class2Cycle);

                    // Engine Size
                    Class2EngineSizeTxt.Text = string.Format("{0:n0}", dr["EngineSize"].ToString());

                    // Color
                    WholesaleSystem.PopulateList(dr["ExteriorColor"].ToString(), "- Select Color --", '|', Class2Color);

                    break;
                case 3:
                    colorBodyTab.Style["display"] = "none";
                    VehicleClass3Type.Style["display"] = "table-row";
                    VehicleClass3Engine.Style["display"] = "table-row";
                    VehicleClass3Suspension.Style["display"] = "table-row";
                    VehicleClass3Fuel.Style["display"] = "table-row";
                    VehicleClass3Trans.Style["display"] = "table-row";
                    VehicleClass3Cyl.Style["display"] = "table-row";

                    // Truck Type
                    WholesaleSystem.PopulateList(dr["TruckType"].ToString(), "-- Select Truck Type --", '|', Class3TruckTypeLst);

                    // Truck Class
                    WholesaleSystem.PopulateList(dr["TruckClass"].ToString(), "-- Select Truck Type --", '|', Class3TruckClassLst);

                    // Engine Make
                    WholesaleSystem.PopulateList(dr["EngineMake"].ToString(), "-- Select Engine Make --", '|', Class3EngineMakeLst);

                    // Engine Size
                    Class3EngineSizeTxt.Text = string.Format("{0:n0}", dr["EngineSize"].ToString());

                    // Axles
                    WholesaleSystem.PopulateList(dr["NumAxles"].ToString(), "-- Select Axles --", '|', Class3AxleLst);

                    // Suspension Type
                    WholesaleSystem.PopulateList(dr["Suspension"].ToString(), "-- Select Axles --", '|', Class3SuspensionTypeLst);

                    // Fuel Type
                    WholesaleSystem.PopulateList(dr["FuelType"].ToString(), "-- Select Fuel Type --", '|', Class3FuelLst, Class3FuelText, kFuel);

                    // Tire Size
                    WholesaleSystem.PopulateList(dr["TireSize"].ToString(), "-- Select Tire Size --", '|', Class3TireSizeLst);

                    // Transmission
                    WholesaleSystem.PopulateList(dr["Transmission"].ToString(), "-- Select Transmission --", '|', Class3TransmissionLst);

                    // Transmission Speed
                    WholesaleSystem.PopulateList(dr["TranSpeedA"].ToString(), "-- Select Transmission Speed --", '|', Class3TransmissionSpeedLst);

                    // Cylinders
                    WholesaleSystem.PopulateList(dr["Cyl"].ToString(), "", '|', Class3CylnderLst, Class3CylinderText, kCyl);

                    break;
                default:
                    VehicleClass1Drive.Style["display"] = "table-row";
                    VehicleClass1Cyl.Style["display"] = "table-row";
                    VehicleClass1Trans.Style["display"] = "table-row";

                    // DriveTrain
                    WholesaleSystem.PopulateList(dr["Drive"].ToString(), "-- Select DriveTrain --", '|', Class1DriveTrainLst, Class1DriveTrainTxt, kDrive);

                    // Cylinders
                    WholesaleSystem.PopulateList(dr["Cyl"].ToString(), "-- Select Cylinders --", '|', Class1CylinderLst, Class1CylindersTxt, kCyl);

                    // Transmission
                    WholesaleSystem.PopulateList(dr["Transmission"].ToString(), "-- Select Transmission --", '|', Class1TransmissionLst);

                    // Fuel
                    WholesaleSystem.PopulateList(dr["FuelType"].ToString(), "-- Select Fuel Type --", '|', Class1FuelLst, Class1FuelText, kFuel);

                    // Body Type
                    WholesaleSystem.PopulateList(dr["BodyType"].ToString(), "-- Select Body Type --", '|', VehicleTypeLst, VehicleTypeTxt, kBodyType);

                    break;
            }
        }

        private string GetListVal(DropDownList list, Label label = null, HiddenField hidden = null)
        {
            if (list.Visible)
            {
                if (list.SelectedIndex == 0)
                    return "0";
                else
                    return list.SelectedValue;
            }
            else
            {
                if (hidden != null && !string.IsNullOrEmpty(hidden.Value))
                    return hidden.Value;
                else
                    return label.Text;
            }
        }

        private string GetListText(DropDownList list, Label label = null, HiddenField hidden = null)
        {
            if (list.Visible)
            {
                if (list.SelectedIndex == 0)
                    return "";
                else
                    return list.SelectedItem.Text;
            }
            else
                return label.Text;
        }

        private string ApplyStyleSelection(string mode, string style, string styleOverride, string importStyle, string importModel)
        {
            string styleId = "";
            string chromeStyle = "";
            string chromeSubmodel = "";
            string reVal = "";

            if (!chkOverrideStyle.Checked)
            {
                if (StyleLst.Visible && StyleLst.SelectedIndex > 0)
                    styleId = StyleLst.SelectedItem.Value.ToString().ToUpper();
                else
                    styleId = StyleHidden.Value.ToString().ToUpper();

                if (styleId.Length > 0)
                {
                    for (int i = 0; i <= MasterStyleLst.Items.Count - 1; i++)
                    {
                        if (MasterStyleLst.Items[i].Value.ToString().ToUpper() == styleId)
                        {
                            chromeStyle = ExpStyleLst.Items[i].Text;
                            chromeSubmodel = SubModelLst.Items[i].Text;
                            break;
                        }
                    }
                }

                switch (mode)
                {
                    case "0":
                        FullDesc.Text = chromeStyle;
                        LimitDesc.Text = chromeSubmodel;
                        reVal = chromeStyle;
                        break;
                    case "1":
                        FullDesc.Text = importStyle;
                        LimitDesc.Text = importModel;
                        reVal = importStyle;
                        break;
                    case "2":
                        if (chromeStyle.Length > 0)
                        {
                            FullDesc.Text = chromeStyle;
                            LimitDesc.Text = chromeSubmodel;
                            reVal = chromeStyle;
                        }
                        else
                        {
                            FullDesc.Text = importStyle;
                            LimitDesc.Text = importModel;
                            reVal = importStyle;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                FullDesc.Text = styleOverride;
                LimitDesc.Text = style;

                reVal = styleOverride;
            }

            return reVal;
        }
        #endregion
    }

    #region Async Task Definitions
    public class LoadDealerLotLocation : WholesaleSystem.AsyncTask
    {
        private string kSession;
        private int kDealer;
        private BLL.WholesaleContent.Vehicle.Update updateBLL;
        private ContentPlaceHolder mainContent;

        public LoadDealerLotLocation() { }
        public LoadDealerLotLocation(string kSession, int kDealer, BLL.WholesaleContent.Vehicle.Update updateBLL, ContentPlaceHolder mainContent)
        {
            this.kSession = kSession;
            this.kDealer = kDealer;
            this.updateBLL = updateBLL;
            this.mainContent = mainContent;
        }

        public override void ExecuteAsyncTask()
        {
            // Setup LotLocation
            string lots = updateBLL.LotLocationSetup(kSession, kDealer, mainContent);
            BindtoDropDown(lots, "", "lstVehicleLotLoc", '|',  mainContent);
        }
    }

    public class LoadVehiclePhotoGallery : WholesaleSystem.AsyncTask
    {
        private string kSession;
        private int kListing;
        private BLL.WholesaleContent.Vehicle.Update updateBLL;
        private ContentPlaceHolder mainContent;
        private string photoGalleryLink;
        public LoadVehiclePhotoGallery() { }
        public LoadVehiclePhotoGallery(string kSession, int kListing, BLL.WholesaleContent.Vehicle.Update updateBLL, ContentPlaceHolder mainContent, string photoGalleryLink)
        {
            this.kSession = kSession;
            this.kListing = kListing;
            this.updateBLL = updateBLL;
            this.mainContent = mainContent;
            this.photoGalleryLink = photoGalleryLink;
        }

        public override void ExecuteAsyncTask()
        {
            // Vehicle Photo Gallery
            Dictionary<string, object> photoGalleries = updateBLL.BuildPhotoGalleryHtml(kSession, kListing);
            if ((int)photoGalleries["photoCnt"] != 0)
            {
                string link = photoGalleries["firstPhoto"].ToString();
                ((HtmlAnchor)mainContent.FindControl("PhotoLink")).Attributes["href"] = photoGalleryLink;
                ((HtmlImage)mainContent.FindControl("PhotoItem")).Attributes["src"] = link.Replace("szLG", "szSM");
            }
        }
    }

    public class LoadVehicleNotes : WholesaleSystem.AsyncTask
    {
        private string kSession;
        private int kListing;
        private BLL.WholesaleContent.Vehicle.Update updateBLL;
        private ContentPlaceHolder mainContent;

        public LoadVehicleNotes() { }
        public LoadVehicleNotes(string kSession, int kListing, BLL.WholesaleContent.Vehicle.Update updateBLL, ContentPlaceHolder mainContent)
        {
            this.kSession = kSession;
            this.kListing = kListing;
            this.updateBLL = updateBLL;
            this.mainContent = mainContent;
        }

        public override void ExecuteAsyncTask()
        {
            // Populate Vehicles Notes
            ((HtmlGenericControl)mainContent.FindControl("VehicleNotes")).InnerHtml = updateBLL.PopulateVehicleNotes(kSession, kListing);
        }
    }

    public class LoadAuctionInfo : WholesaleSystem.AsyncTask
    {
        private string kSession;
        private int kDealer;
        private int kListing;
        private BLL.WholesaleContent.Vehicle.Update updateBLL;
        private ContentPlaceHolder mainContent;
        private Page page;
        private string focusAuction;

        public LoadAuctionInfo() { }
        public LoadAuctionInfo(string kSession, int kDealer, int kListing, BLL.WholesaleContent.Vehicle.Update updateBLL, string focusAuction, Page page, ContentPlaceHolder mainContent )
        {
            this.kSession = kSession;
            this.kDealer = kDealer;
            this.kListing = kListing;
            this.updateBLL = updateBLL;
            this.mainContent = mainContent;
            this.focusAuction = focusAuction;
            this.page = page;
        }

        public override void ExecuteAsyncTask()
        {
            // Populate Auction Info
            Tuple<int, string> rs = updateBLL.PopulateAuctionInfo(kSession, kDealer, kListing, focusAuction);
            HtmlGenericControl ai = (HtmlGenericControl)mainContent.FindControl("AuctionInfo");

            if (String.IsNullOrEmpty(rs.Item2))
            {
                ai.Style["height"] = "auto !important";
                ai.InnerHtml = "Vehicle has not been listed to any auctions.";
            }
            else
                ai.InnerHtml = rs.Item2;

            if (!string.IsNullOrEmpty(focusAuction))
            {
                ((HtmlGenericControl)mainContent.FindControl("divListingInfo")).Style["display"] = "flex";
                ((HtmlGenericControl)mainContent.FindControl("divGeneral")).Style["display"] = "none";
                int scrollHeight = 0;
                if (rs.Item1 != -1)
                    scrollHeight = 25 * rs.Item1;
                page.ClientScript.RegisterClientScriptBlock(this.GetType(), "", "window.onload=function(){window.location = '#VehicleDetails'; document.getElementById('MainContent_AuctionInfo').scrollTop = " + scrollHeight + "};", true);
            }
        }
    }

    public class LoadOptionsInfo : WholesaleSystem.AsyncTask
    {
        private string kSession;
        private int kListing;
        private BLL.WholesaleContent.Vehicle.Update updateBLL;
        private ContentPlaceHolder mainContent;

        public LoadOptionsInfo() { }
        public LoadOptionsInfo(string kSession, int kListing, BLL.WholesaleContent.Vehicle.Update updateBLL, ContentPlaceHolder mainContent)
        {
            this.kSession = kSession;
            this.kListing = kListing;
            this.updateBLL = updateBLL;
            this.mainContent = mainContent;
        }

        public override void ExecuteAsyncTask()
        {
            // Options Info
            Lookup.lmReturnValue lookupResult = updateBLL.GetListingOptionList(kSession, kListing);

            if (lookupResult.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                DataTable optionTbl = lookupResult.Data.Tables[0];

                DataRow[] CommonRows = optionTbl.Select("OptionGroupName = 'Common Options'");
                DataRow[] MarketingRows = optionTbl.Select("OptionGroupName = 'Marketing Category'");
                DataRow[] VinExplosionRows = optionTbl.Select("OptionGroupName = 'VIN Explosion Options'");
                DataRow[] ImportedRows = optionTbl.Select("OptionGroupName = 'Imported Options'");

                ((HtmlGenericControl)mainContent.FindControl("CommonOptions")).InnerHtml = updateBLL.FormatOptions(CommonRows, "common");
                ((HtmlGenericControl)mainContent.FindControl("availableCommonOptions")).InnerHtml = $"<b>Common Options - {CommonRows.Length} available options" + (CommonRows.Length > 0 ? " &#9650;" : "") + "</b>";

                ((HtmlGenericControl)mainContent.FindControl("VinExplosionoptions")).InnerHtml = updateBLL.FormatOptions(VinExplosionRows, "vin");
                ((HtmlGenericControl)mainContent.FindControl("availableVinOptions")).InnerHtml = $"<b>VIN Explosion Options - {VinExplosionRows.Length} available options" + (VinExplosionRows.Length > 0 ? " &#9660;" : "") + "</b>";

                if (ImportedRows.Length != 0)
                {
                    ((HtmlGenericControl)mainContent.FindControl("ImportedOptions")).InnerHtml = updateBLL.FormatOptions(ImportedRows, "import");
                    ((HtmlGenericControl)mainContent.FindControl("availableImportedOptions")).InnerHtml = $"<b>Imported Options - {ImportedRows.Length} available options" + (ImportedRows.Length > 0 ? " &#9660;" : "") + "</b>";
                }
                else {
                    ((HtmlGenericControl)mainContent.FindControl("availableImportedOptions")).Style["display"] = "none";
                }
            }
        }
    }
    #endregion
}