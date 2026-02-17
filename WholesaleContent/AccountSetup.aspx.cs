using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;

using LMWholesale.BLL.WholesaleUser;
using LMWholesale.Common;
using LMWholesale.Dealer;


namespace LMWholesale.WholesaleContent
{
    public partial class AccountSetup : lmPage
    {
        private readonly WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.AccountSetup accountSetupBLL;

        public AccountSetup()
        {
            userBLL = new WholesaleUser();
            accountSetupBLL = new BLL.WholesaleContent.AccountSetup();
        }

        public AccountSetup(WholesaleUser userBLL, BLL.WholesaleContent.AccountSetup accountSetupBLL)
        {
            this.userBLL = userBLL;
            this.accountSetupBLL = accountSetupBLL;
        }

        public static AccountSetup Self
        {
            get { return instance; }
        }

        private static AccountSetup instance = new AccountSetup();

        protected void Page_PreRender(object sender, EventArgs args)
        {
            ViewState["RefreshCheck"] = Session["RefreshCheck"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Account Setup";
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                jsGridBuilder accountSetupGrid = new jsGridBuilder
                {
                    HTMLElement = "jsGrid",
                    Filtering = false,
                    OnRowSelectFunction = "GridRowSelected",
                    OnClearRowSelectFunction = "ClearRowSelection",
                    OnDoubleClickFunction = "RowDoubleClick",
                    MethodURL = "AccountSetup.aspx/ApprovalRequests",
                    ExtraFunctionality = "document.getElementById(\"jsGrid\").children[1].style.height = \"calc((100vh - 220px)*.33)\"",
                    PageSize = int.MaxValue
                };
                // #TODO: replace with actual field names
                string wholesaleDefaultGridDef = ":kWholesaleSignup:kWholesaleSignup:100|:RequestUser:Requesting User:100|:GaggleSubGroupName:Account:100|:DealerName:Requested Name:100|:Posted:Posted:100";
                accountSetupGrid.SetFieldListFromGridDef(wholesaleDefaultGridDef, "kWholesaleSetup", true);

                WholesaleSystem.BuildStateDropdown("ddlState");
                string auctions = Self.accountSetupBLL.GetSignupAuctions(Session["kSession"].ToString());
                WholesaleSystem.PopulateList(auctions, "--Select an Auction--", "ddlAuction", '|', (auctions.Count(f => f == '|') == 1 ? auctions.Substring(2, auctions.IndexOf(":") - 2) : ""));
                WholesaleSystem.PopulateList(Self.accountSetupBLL.GetImportSystems(Session["kSession"].ToString(), 1), "", "ddlCompanyName", '|');
                WholesaleSystem.PopulateList("", "--No Account Template--", "ddlAccountTemplate", '|');

                if (Self.userBLL.CheckPermission("AccountApprove") == false && Self.userBLL.CheckPermission("Developer") == false)
                    fsApproval.Visible = false;

                Session["RefreshCheck"] = Server.UrlDecode(DateTime.Now.ToString());

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "accountSetupGrid", accountSetupGrid.RenderGrid());
            }
        }

        [WebMethod(EnableSession = true)]
        public static string ApprovalRequests(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            return Self.accountSetupBLL.GetApprovalGridData((string)Session["kSession"]);
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, object> GetSpecificSignup(string kWholesaleSignup)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            Dictionary<string, string> result = Self.accountSetupBLL.GetSpecificSignup(Session["kSession"].ToString(), kWholesaleSignup);
            IsSuccess = result["Result"] == "success" ? true : false;
            Message = result["ResultString"];
            result.Remove("Result");
            result.Remove("ResultString");
            Value = result;
            return ReturnResponse();
        }

        [WebMethod(EnableSession = true)]
        public static Dictionary<string, object> GetAuctionTemplates(string kDealerSubGaggle)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string result = Self.accountSetupBLL.GetTemplateDealers(Session["kSession"].ToString(), kDealerSubGaggle);
            IsSuccess = !result.Contains("Error:");
            Message = result.Contains("Error:") ? result.Substring(6) : "Success";
            Value = result;
            return ReturnResponse();
        }

        protected void bntSubmitSignup(object sender, EventArgs args)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (string.IsNullOrEmpty(Session["kSession"].ToString()))
                WholesaleUser.ClearUser();

            string alert = "";
            bool isSuccess = true;
            // Attempting to detect when a page is manually refreshed
            if (ViewState["RefreshCheck"].ToString() == Session["RefreshCheck"].ToString())
            {
                try
                {
                    string DealerName = txtDealer.Text;
                    if (SignupSource.Value == "" || DealerName == "")
                    {
                        alert = "alert('Please enter a valid Dealership Name.')";
                        isSuccess = false;
                    }

                    // Ensure we are submitting a valid DealerName
                    string CleanWhiteSpace = "^\\s+|\\s+$|\\s+(?=\\s)";
                    string ValidateName = "[^A-Z a-z,\\[\\].0-9\\&(\\)\'\\/-]";

                    DealerName = Regex.Replace(DealerName, CleanWhiteSpace, "");
                    Match InvalidMatch = Regex.Match(DealerName, ValidateName);
                    if (InvalidMatch.Success)
                    {
                        alert = "alert('Dealer Name contains an invalid character.')";
                        isSuccess = false;
                    }

                    bool isValidDealerName = true;
                    if (SignupSource.Value == "approve")
                        isValidDealerName = Self.accountSetupBLL.ValidDealerName(Session["kSession"].ToString(), DealerName);

                    if (!isValidDealerName)
                    {
                        alert = "alert('Dealer Name is invalid or already in use.')";
                        isSuccess = false;
                    }

                    if (isSuccess)
                    {
                        Dictionary<string, object> SignupObj = new Dictionary<string, object>()
                        {
                            { "kDealerSubGaggle", ddlAuction.SelectedItem.Value },
                            { "DealerName", txtDealer.Text },
                            { "Address1", txtStreet.Text },
                            { "Address2", "" },
                            { "City", txtCity.Text },
                            { "State", ddlState.SelectedItem.Value },
                            { "Zip", txtZipCode.Text },
                            { "Phone", txtPhone.Text },
                            { "ContactFName", txtName.Text },
                            { "ContactLName", "" },
                            { "ContactWPhone", txtOfficeNumber.Text },
                            { "ContactCPhone", txtCellNumber.Text },
                            { "ContactEmail", txtEmail.Text },
                            { "FeedCompany", ddlCompanyName.SelectedItem.Text },
                            { "kWholesaleLocationIndicator", chkDealership.Checked ? 1 : 2 },
                            { "LiquidConnect", chkYesLC.Checked ? 1 : 0 },
                            { "Notes", txtNotes.Text },
                            { "kOriginalRequest", 0 },
                            { "Approved", 0 },
                            { "kTemplateDealer", 0 },
                            { "CustomerType", "" },
                            { "kBillingGroup", "" },
                            { "CopyFlags", "" },
                            { "SuppressAuctionEmails", 0 },
                            { "ManheimUserIDs", "" },
                        };

                        if (SignupSource.Value != "submit")
                        {
                            SignupObj["kOriginalRequest"] = hfApprovalId.Value == "" ? 0 : int.Parse(hfApprovalId.Value);
                            SignupObj["Approved"] = SignupSource.Value == "approve" ? 1 : 0;
                            SignupObj["kTemplateDealer"] = kTemplate.Value;
                            SignupObj["CopyFlags"] = GetCopyFlags();
                            SignupObj["SuppressAuctionEmails"] = fsApprovalBox.Style["display"] == "none" && chkSuppressEmails.Checked ? 1 : 0;
                        }

                        lmReturnValue result = Self.accountSetupBLL.SetSignupData(Session["kSession"].ToString(), SignupObj);
                        if (result.Result == ReturnCode.LM_SUCCESS)
                        {
                            if (SignupObj["kOriginalRequest"].ToString() != "0")
                            {
                                if (SignupObj["Approved"].ToString() == "1")
                                    alert = "alert('Account Request has been approved')";
                                else
                                    alert = "alert('Account Request has been denied')";
                            }
                            else
                                alert = "alert('Signup Form Submitted')";
                        }
                        else if (result.Result == ReturnCode.LM_INVALIDSESSION)
                            WholesaleUser.ClearUser();
                        else
                        {
                            string response = string.Format("Unable to perform request due to the following error: {0}. Please try again or call support for assistance.)", result.ResultString);
                            alert = "alert('" + response + "')";
                        }
                    }
                }
                catch (Exception ex)
                {
                    string error = string.Format("Something went wrong: Account Setup Submit [ Message - {0} | StackTrace - {1} ]", ex.Message, ex.StackTrace);
                    WholesaleSystem.Logger.LogLine(Session["kSession"].ToString(), error);
                    alert = "alert('" + error + "')";
                }

                Response.Write("<script>if(!" + alert + "){window.location.href = \"/WholesaleContent/AccountSetup.aspx\";}</script>");
                Session["RefreshCheck"] = Server.UrlDecode(DateTime.Now.ToString());
                //Response.Redirect("/WholesaleContent/AccountSetup.aspx");
            }
        }

        private string GetCopyFlags()
        {
            string retVal = "";
            if (kTemplate.Value == "0" || kTemplate.Value == "")
                return "000000000";

            retVal += chkAddress.Checked ? "1" : "0";
            retVal += chkUsers.Checked ? "1" : "0";
            retVal += chkContactGroups.Checked ? "1" : "0";
            retVal += chkLotLocations.Checked ? "1" : "0";
            retVal += chkWholesaleAuctions.Checked ? "1" : "0";
            retVal += chkAlternateCredentials.Checked ? "1" : "0";
            retVal += chkAutoLaunchRules.Checked ? "1" : "0";
            retVal += chkBlackoutRules.Checked ? "1" : "0";
            retVal += chkProducts.Checked ? "1" : "0";

            return retVal;
        }
    }
}