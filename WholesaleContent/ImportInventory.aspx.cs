using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using LMWholesale.Common;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.IO;

namespace LMWholesale
{

    public partial class ImportInventory : lmPage
    {
        private static string fileName = "";
        private static string header = "";

        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.ImportInventory importInventory;

        public static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public ImportInventory() 
        { 
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            importInventory = importInventory ?? new BLL.WholesaleContent.ImportInventory();
        }

        public static ImportInventory Self
        {
            get { return instance; }
        }
        private static readonly ImportInventory instance = new ImportInventory();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Import Inventory";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];

                // Broken Service atm
                GetImportStatus(kSession, kDealer);

                GetImportTypes(kSession, kDealer);
            }
        }

        /// <summary>
        /// Gets the values for the most recent import
        /// </summary>
        private void GetImportStatus(string kSession, int kDealer)
        {
            var temp = HttpContext.Current.Request;

            DAS.lmReturnValue returnValue = importInventory.ImportStatusGet(kSession, kDealer);
            if (returnValue.Result == DAS.ReturnCode.LM_SUCCESS)
            {
                try
                {
                    DataRow dr = returnValue.Data.Tables["Status"].Rows[0];
                    lblPerson.Text = dr["Person"].ToString();
                    lblStartTime.Text = dr["StartTime"].ToString();
                    if (String.IsNullOrEmpty(dr["EndTime"].ToString()))
                    {
                        lblStatus.Text = "In Progress";
                        uplChooseFile.Enabled = false;
                        btnSubmit.Enabled = false;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(dr["Status"].ToString()))
                        {
                            lblStatus.Text = "Completed";
                        }
                        else
                        {
                            lblStatus.Text = dr["Status"].ToString();
                        }
                        uplChooseFile.Enabled = true;
                        btnSubmit.Enabled = true;
                        lblEndTime.Text = dr["EndTime"].ToString();
                    }
                    lblRecords.Text = dr["Records"].ToString();
                    lblImported.Text = dr["Imported"].ToString();
                    lblAnalyzed.Text = dr["Analyzed"].ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (returnValue.Result == DAS.ReturnCode.LM_INVALIDSESSION)
            {
                BLL.WholesaleUser.WholesaleUser.ClearUser(returnValue.ResultString);
            }
            else
            {
                string smsg = "<script>alert('Unable to perform request due to the following error: " + returnValue.ResultString + ".  Please try again or call support for assistance.');</script>";
                Response.Write(smsg);
            }
        }

        /// <summary>
        /// Gets the list of all available imports, imports configured for the dealer, and populate ddlFileFormat
        /// </summary>
        private void GetImportTypes(string kSession, int kDealer)
        {
            /* Gets the list of all available system imports  */
            Dealer.lmReturnValue returnValue = Self.importInventory.ImportConfigGetLRV(kSession, kDealer);
            if (returnValue.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                DataSet dsDealerConfig = returnValue.Data;
                DataTable dtDealerImports = returnValue.Data.Tables["Systems"];

                string VehicleInvAcc = dsDealerConfig.Tables["Config"].Rows[0]["VehicleInvAcc"].ToString();

                /* Gets the list of imports configured for this dealer  */
                returnValue = importInventory.ImportSystemGet(kSession, VehicleInvAcc);
                if (returnValue.Result == Dealer.ReturnCode.LM_SUCCESS)
                {
                    DataTable dtSystems = returnValue.Data.Tables["Systems"];
                    ddlFileFormat.Items.Clear();
                    int i;
                    Dictionary<int, int> dictImportType = new Dictionary<int, int>();

                    /* Create dictionary of ImportType to kDealerImport */
                    for (i = 0; i <= dtDealerImports.Rows.Count - 1; i++)
                    {
                        if (Util.SafeStringToInt(dtDealerImports.Rows[i]["Enabled"].ToString()) == 1)
                        {
                            dictImportType.Add(Util.SafeStringToInt(dtDealerImports.Rows[i]["kDealerImport"].ToString()), Util.SafeStringToInt(dtDealerImports.Rows[i]["ImportType"].ToString()));
                        }
                    }
                    ViewState["ImportTypeDict"] = dictImportType;

                    ListItem DDLItem;
                    Dictionary<int, string> dictDelimiter = new Dictionary<int, string>();
                    /* Create dictionary of delimiters to kDealerImport */
                    for (i = 0; i <= dtSystems.Rows.Count - 1; i++)
                    {
                        if (Util.SafeStringToInt(dtSystems.Rows[i]["Active"].ToString()) == 1)
                        {
                            if (dictImportType.ContainsKey(Util.SafeStringToInt(dtSystems.Rows[i]["kDealerImport"].ToString())))
                            {
                                DDLItem = new ListItem
                                {
                                    Value = dtSystems.Rows[i]["kDealerImport"].ToString(),
                                    Text = "Feed " + dictImportType[Util.SafeStringToInt(dtSystems.Rows[i]["kDealerImport"].ToString())] + " - " + dtSystems.Rows[i]["ImportDesc"].ToString()
                                };
                                ddlFileFormat.Items.Add(DDLItem);
                                dictDelimiter.Add(Util.SafeStringToInt(dtSystems.Rows[i]["kDealerImport"].ToString()), dtSystems.Rows[i]["Delimiter"].ToString());
                            }
                        }
                    }

                    /* Sort the dropdown list and limit the displayed values to those configured for the dealer */
                    WholesaleSystem.SortDropDownList(ref ddlFileFormat);
                    ViewState["DelimiterDict"] = dictDelimiter;

                    if (ddlFileFormat.Items.FindByText("Manual File") == null)
                    {
                        ddlFileFormat.Items.Insert(0, new ListItem("Manual File", "4"));
                    }
                }
                else
                {
                    string smsg = "<script>alert('Unable to perform request due to the following error: " + returnValue.ResultString + ".  Please try again or call support for assistance.');</script>";
                    Response.Write(smsg);
                }
            }
            else if (returnValue.Result == Dealer.ReturnCode.LM_INVALIDSESSION)
            {
                BLL.WholesaleUser.WholesaleUser.ClearUser(returnValue.ResultString);
            }
            else
            {
                string smsg = "<script>alert('Unable to perform request due to the following error: " + returnValue.ResultString + ".  Please try again or call support for assistance.');</script>";
                Response.Write(smsg);
            }
        }

        protected void ExportExampleFile(object Sender, EventArgs e)
        {
            string type = ExampleFileName.Value;

            if (type == "min") {
                fileName = "Example_File_minimum.csv";
                header = "VIN,Year,Make,Model,Miles,Cost,Listprice";
            } else {
                fileName = "Example_File.csv";

                // Split full example string
                header = "VIN,Year,Make,Model,Trim,StockNumber,StockType,Certified,Status,LotLocation,";
                header += "InventoryDate,Miles,Cost,Listprice,InternetPrice,MSRP,ExteriorColor,InteriorColor,";
                header += "Options,PhotoURL,Comments,Engine,Cylinders,Transmission";
            }

            // Download CSV file
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(header);

            // Take out the trash
            Response.Flush();
            Response.End();
        }

        protected void Upload(object sender, EventArgs e)
        {
            Self.userBLL.CheckSession();
            HttpSessionState Session = HttpContext.Current.Session;
            int kDealer = (int)Session["kDealer"];
            string sessid = (string)Session["kSession"];

            Dictionary<string, string> pathReturn = Self.importInventory.GetDealerPaths(sessid, kDealer);

            if (pathReturn["Success"] == "1")
            {
                string pathName = pathReturn["PhotoPath"];
                pathName = pathName.Substring(0, pathName.LastIndexOf('\\'));
                pathName = pathName.Substring(pathName.LastIndexOf('\\') + 1);
                string physicalPath = pathReturn["DealerImportPath"];
                physicalPath = physicalPath.Substring(physicalPath.LastIndexOf("=") + 1);

                if (!Directory.Exists(physicalPath))
                {
                    try
                    {
                        Directory.CreateDirectory(physicalPath);
                    }
                    catch (Exception)
                    {
                        Response.Write("<script>alert('Unable to create directory at specified upload location. Please contact support for assistance.');</script>");
                        return;
                    }
                }

                pathName = pathName + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToFileTime() + ".dmi";
                pathName = pathName.Replace(" ", "_");
                pathName = pathName.Replace("/", "_");

                if (ddlFileFormat.SelectedValue == "4")
                {
                    try
                    {
                        if (uplChooseFile.HasFile && uplChooseFile.PostedFile.InputStream.Length == 0)
                        {
                            Response.Write("<script>alert('The file was unable to be processed. The file that was specified is empty.');</script>");
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        Response.Write("<script>alert('The path provided did not match a file in your file system or was not able to be opened.');</script>");
                        return;
                    }

                    try
                    {
                        uplChooseFile.PostedFile.SaveAs(physicalPath + pathName);
                        hfFileName.Value = uplChooseFile.FileName;
                        hfServerPath.Value = physicalPath + pathName;

                        string filePath = physicalPath + pathName;
                        string fileName = uplChooseFile.FileName;
                        int iType = 1;
                        int iDealerType = Convert.ToInt32(ddlFileFormat.SelectedValue);
                        string delimiter = rbDelimiter.SelectedValue;

                        Dictionary<string, string> configReturn = Self.importInventory.ImportConfigGet(sessid, kDealer);

                        if (configReturn["Success"] == "1")
                        {
                            Dictionary<string, string> importReturn = Self.importInventory.DealerImport(sessid, kDealer.ToString(), configReturn["VehicleInvAcc"], filePath, fileName, delimiter, iDealerType, iType);
                            if (importReturn["Success"] == "1")
                            {
                                Response.Write("<script>alert('The inventory file was successfully uploaded. It will be imported into the system shortly. Use Refresh to check the status.')</script>");
                            }
                            else
                            {
                                Response.Write("<script>alert('An error occured while trying to upload the file: \"" + importReturn["Message"] + "\"');</script>");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<script>alert('An error occured while trying to upload the file: \"" + ex.Message + "\"');</script>");
                        return;
                    }
                }
            }
        }
    }
}