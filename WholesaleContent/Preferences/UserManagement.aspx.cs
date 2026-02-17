using LMWholesale.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;


namespace LMWholesale.WholesaleContent.Preferences
{
    public partial class UserManagement : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Preferences.UserManagement userManagement;

        public UserManagement()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            userManagement = userManagement ?? new BLL.WholesaleContent.Preferences.UserManagement();
        }

        public static UserManagement Self
        {
            get { return instance; }
        }
        private static readonly UserManagement instance = new UserManagement();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Preferences";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack)
            {
                HttpSessionState Session = HttpContext.Current.Session;
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];
                int kDealerGaggle = (int)Session["kDealerGaggle"];

                accountName.InnerHtml = "Account Name: " + (string)Session["DealerName"];
                if (kDealerGaggle != 244)
                {
                    alOverrideLbl.Style["display"] = "table-cell";
                    alOverrideChk.Style["display"] = "table-cell";
                }

                // Gather and populate lists
                WholesaleSystem.PopulateList(Self.userManagement.GetDealerRelations(kSession, kDealer), "-- Select a Relationship --", "userRelationship", '|');
                WholesaleSystem.PopulateList(Self.userManagement.GetInspectionCompanyList(kSession), "-- Select Inspection Company --", "lstInspectionCompany", '|');

                if (Self.userBLL.CheckPermission("WholesaleBuyerProduct") && Self.userBLL.CheckPermission("WholesaleSellerProduct"))
                    Wholesale.Attributes["display"] = "initial";
                if (Self.userBLL.CheckPermission("Appraisal"))
                    Appraisal.Attributes["display"] = "initial";

                jsGridBuilder usersGrid = new jsGridBuilder
                {
                    MethodURL = "UserManagement.aspx/GetDealerUsers",
                    OnRowSelectFunction = "UsersGridRowSelected",
                    OnClearRowSelectFunction = "ClearRowSelection",
                    OnDoubleClickFunction = "EditUser();",
                    HTMLElement = "usersGrid",
                    PageSize = 250,
                    Filtering = false
                };

                usersGrid.SetFieldListFromGridDef(":FName:First Name:75|:LName:Last Name:75|:PhoneNumber:Phone Number:90|:EmailLink:Email:150|:UserID::150|:RelationDesc:Relationship:80|!:Email:Email:80|!:kRelation:kRelation:40|!:LeadMember:LeadMember:40|!:LMAdmin:LMAdmin:40|!:InvAdmin:InvAdmin:40|:MDC:MDC:60|!:WholesaleAdmin:WholesaleAdmin:40|!:Inspector:Inspector:40|!:WholesaleBuyer:WholesaleBuyer:40|!:WholesaleSeller:WholesaleSeller:40|!:AuctionAccessID:AuctionAccessID:40|!:GvmtID:GvmtID:40|!:AppraisalAppraiser:AppraisalAppraiser:60|!:AppraisalSales:AppraisalSales:50|!:kPerson:kPerson:100|!:IsInspector:Inspector:60|!:kWholesaleInspectionCompany:kWholesaleInspectionCompany:60|:ManheimUserName:ManheimUserID:75|!:CellNumber:CellNumber:100|:SMSTexting:SMS:50|!:SALPricingBypass::10|", "", true);

                // Gather Users and pre-populate table
                jsGridBuilder searchGrid = new jsGridBuilder
                {
                    MethodURL = "UserManagement.aspx/GetUsers",
                    OnRowSelectFunction = "SearchGridRowSelected",
                    OnClearRowSelectFunction = "ClearRowSelection",
                    OnDoubleClickFunction = "AddDealerUser();",
                    HTMLElement = "searchGrid",
                    Filtering = false,
                    jsGridIdentifier = "1"
                };

                searchGrid.SetFieldListFromGridDef(":FName:First Name:75|:LName:Last Name:75|!:PhoneNumber:Phone Number:90|:Email::150|:UserID::150|!:Email:Email:80|!:kRelation:kRelation:40|!:AuctionAccessID:AuctionAccessID:40|!:kPerson:kPerson:100|!:ManheimUserName:ManheimUserID:75|!:CellNumber:CellNumber:100|!:SMSTexting:SMS:50|", "", true);

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "usersGrid", usersGrid.RenderGrid());
                    ClientScript.RegisterStartupScript(this.GetType(), "searchGrid", searchGrid.RenderGrid());
                }
            }
        }

        [WebMethod]
        public static string GetDealerUsers(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            if (string.IsNullOrEmpty((string)Session["kSession"]))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            int num = 0;
            string items = "{}";

            DataTable dt = Self.userManagement.GetDealerUsers((string)Session["kSession"], (int)Session["kDealer"], (Dictionary<string, object>)Util.serializer.DeserializeObject(filter));
            if (dt != null && dt.Rows.Count > 0)
            {
                num = dt.Rows.Count;
                items = Util.serializer.Serialize(FormatData(dt));
            }

            return $"{num}|{items}";
        }

        [WebMethod]
        public static string GetUsers(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            if (string.IsNullOrEmpty((string)Session["kSession"]))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            int num = 0;
            string items = "{}";

            DataTable dt = Self.userManagement.GetUsers((string)Session["kSession"], (Dictionary<string, object>)Util.serializer.DeserializeObject(filter));
            if (dt != null && dt.Rows.Count > 0)
            {
                num = dt.Rows.Count;
                items = Util.serializer.Serialize(FormatData(dt.DefaultView.ToTable()));
            }

            return $"{num}|{items}";
        }

        [WebMethod]
        public static Dictionary<string, object> GetUserInfo(int kPerson)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            if (string.IsNullOrEmpty((string)Session["kSession"]))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            int kDealer = (int)Session["kDealer"];

            // Get cache and find user
            DataTable dealerUsers = (DataTable)WholesaleSystem.GetCachedObject(kDealer + "DealerUsers");
            DataRow[] user = dealerUsers.Select("kPerson = " + kPerson);
            if (user.Length != 0)
            {
                Dictionary<string, object> userInfo = new Dictionary<string, object>();
                foreach (DataColumn dc in dealerUsers.Columns)
                    userInfo.Add(dc.ColumnName, user[0][dc.ColumnName]);

                Value = userInfo;
                IsSuccess = true;
                Message = "";
            }
            else
            {
                IsSuccess = false;
                Message = "Unable to find specified User! Please contact Support!";
            }

            return ReturnResponse();
        }

        [WebMethod]
        public static Dictionary<string, object> UserInfoSave(string json, string op)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            if (string.IsNullOrEmpty((string)Session["kSession"]))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string returnString = "";
            IsSuccess = Self.userManagement.UserInfoSave((string)Session["kSession"], (int)Session["kDealer"], op, (Dictionary<string, object>)Util.serializer.DeserializeObject(json));
            Message = returnString;

            return ReturnResponse();
        }

        private static List<Dictionary<string, object>> FormatData(DataTable dt)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                    dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));

                dict["EmailLink"] = $"<a href='mailto:{row["Email"]}' title='Email Prospect'>{row["Email"]}</a>";

                returnList.Add(dict);
            }

            return returnList;
        }
    }
}