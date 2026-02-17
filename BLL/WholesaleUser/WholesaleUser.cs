using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;

using LMWholesale.resource.clients;

using Soss.Client;

namespace LMWholesale.BLL.WholesaleUser
{
    public class WholesaleUser
    {
        private readonly AuthenticationClient authClient;
        private readonly LookupClient lookupClient;
        private readonly DealerClient dealerClient;
        private static readonly string salt = ")+3Pk4y36`n{WmcM";

        public WholesaleUser()
        {
            authClient = authClient ?? new AuthenticationClient();
            lookupClient = lookupClient ?? new LookupClient();
            dealerClient = dealerClient ?? new DealerClient();
        }

        public WholesaleUser(AuthenticationClient authClient, LookupClient lookupClient, DealerClient dealerClient)
        {
            this.authClient = authClient;
            this.lookupClient = lookupClient;
            this.dealerClient = dealerClient;
        }

        internal static readonly WholesaleUser instance = new WholesaleUser();
        public WholesaleUser Self
        {
            get { return instance; }
        }

        public bool Login(string Username, string Password, string Platform, HttpSessionState Session)
        {
            Authenticate.lmReturnValue returnValue = Self.authClient.Login(Username, Password, Platform);
            if (returnValue.Result == Authenticate.ReturnCode.LM_SUCCESS)
            {
                Session.Clear();
                Session.Timeout = 720;

                Session["kSession"] = returnValue.Values.GetValue("Session", "");
                Session["FullName"] = returnValue.Values.GetValue("UserName", "");
                Session["kPerson"] = returnValue.Values.GetValue("kPerson", "");

                // Adding a bit of salt to the password hash
                Session["PHash"] = Tuple.Create(Password, salt).GetHashCode();
                return true;
            }

            return false;
        }

        public string PasswordCheck(string Username, string Password)
        {
            Authenticate.lmReturnValue returnValue = Self.authClient.CheckUserPassword(Username, Password);
            if (returnValue.Result == Authenticate.ReturnCode.LM_SUCCESS
                    && returnValue.ResultString == "PromptUser")
                return returnValue.Values.GetValue("kResetCode", "");
            return "";
        }

        public bool GetDealersInfo(HttpSessionState Session, ref string successMsg, ref string errorMsg, string sDealerName, int kDealer = -1)
        {
            Lookup.lmReturnValue returnValue = Self.lookupClient.GetDealerList(Session["kSession"].ToString());
            if (returnValue.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = returnValue.Data.Tables["Dealers"];
                DataRow dr = null;
                if (dt.Rows.Count < 1)
                    return false;

                Session["dsDealers"] = dt;
                if (kDealer != -1)
                    dr = dt.AsEnumerable().SingleOrDefault(r => r["kDealer"].ToString().CompareTo(kDealer.ToString()) == 0);
                else if (sDealerName != "")
                    dr = dt.AsEnumerable().SingleOrDefault(r => r["DealerName"].ToString().CompareTo(sDealerName) == 0);
                else if (dt.Rows.Count == 1)
                    dr = dt.Rows[0];

                if (dr != null)
                {
                    //single dealer available or selected
                    successMsg = "SingleDealer";
                    Session["kDealer"] = dr["kDealer"];
                    Session["DealerName"] = dr["DealerName"].ToString();
                    DataSet dsDealerPrefs = GetDealerPreferences(int.Parse(Session["kDealer"].ToString()));
                    if (dsDealerPrefs is null)
                    {
                        successMsg = "User has no permissions for the selected dealer.";
                        HttpContext.Current.Session.Clear();
                        return false;
                    }
                }
                else
                {
                    //get all dealers for user
                    successMsg = "MultiDealer";
                    Session["kDealer"] = -1;
                    Session["DealerName"] = "ALL DEALERS";
                    Session["CustomerType"] = 1;
                    return true;
                }
                GetPermissions();

                return true;
            }
            else if (returnValue.Result == Lookup.ReturnCode.LM_INVALIDSESSION)
            {
                errorMsg = "Your Session Has Expired";
                HttpContext.Current.Session.Clear();
                return false;
            }
            else
            {
                errorMsg = "Unable to perform request due to the following error: " + returnValue.ResultString + " .";
                HttpContext.Current.Session.Clear();
                return false;
            }
        }

        public void GetPermissions()
        {
            HttpSessionState Session = HttpContext.Current.Session;
            CheckSession();
            Session.Remove("UserPermissions");
            int iDealerID = int.Parse(Session["kDealer"].ToString());

            Authenticate.lmReturnValue rv = Self.authClient.GetPermissions(Session["kSession"].ToString(), iDealerID);
            if (rv.Result == Authenticate.ReturnCode.LM_SUCCESS)
            {
                List<string> permissions = rv.Data.Tables["Permissions"].AsEnumerable().Select(r => r[1].ToString().ToLower()).ToList();
                Session["UserPermissions"] = permissions;
            }
            else
            {
                ClearUser(rv.ResultString);
            }
        }

        public bool CheckPermission(string checkpermission)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            List<string> tmppermis = (List<string>)Session["UserPermissions"];
            if (tmppermis != null) 
                return tmppermis.Contains(checkpermission.ToLower());
            return false;
        }

        public DataSet GetDealerPreferences(int kDealer)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            DataSet dsPrefs = null;
            if (kDealer < 1)
                ClearUser();
            else
            {
                Dealer.lmReturnValue returnValue = Self.dealerClient.GetDealerInfo(Session["kSession"].ToString(), kDealer, null, "GridPrefs,DealerBase");
                if (returnValue.Result == Dealer.ReturnCode.LM_SUCCESS)
                {
                    // #TODO: Might be best to figure out what "DealerBase" info is really needed for all the pages
                    dsPrefs = returnValue.Data;
                    Session["GridPrefs"] = returnValue.Data.Tables["GridPrefs"];
                    Session["CustomerType"] = returnValue.Data.Tables["DealerBase"].Rows[0]["CustomerType"];
                    Session["kDealerGaggle"] = returnValue.Data.Tables["DealerBase"].Rows[0]["kDealerGaggle"];
                    Session["kAccountType"] = returnValue.Data.Tables["DealerBase"].Rows[0]["kAccountType"];
                    Session["kDistributor"] = returnValue.Data.Tables["DealerBase"].Rows[0]["kDistributor"];
                    Session["kGaggleSubGroup"] = returnValue.Data.Tables["DealerBase"].Rows[0]["kGaggleSubGroup"];
                    Session["WholesaleInspector"] = returnValue.Data.Tables["DealerBase"].Rows[0]["WholesaleInspector"];
                    Session["DealerState"] = returnValue.Data.Tables["DealerBase"].Rows[0]["DealerState"];
                    Session["No3rdpartyExport"] = returnValue.Data.Tables["DealerBase"].Rows[0]["No3rdpartyExport"];
                }
                else
                    ClearUser(returnValue.ResultString);
            }

            return dsPrefs;
        }

        public void CheckSession()
        {
            HttpSessionState Session = HttpContext.Current.Session;

            // Little hack to prevent webservice calls while isolating wholesaleportal vs portal for daytime deployments
            bool maint = bool.Parse(WebConfigurationManager.AppSettings["isMaintenance"]);
            if (maint)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Response.Redirect("/Maintenance.html", true);
            }

            if (Session["kSession"] != null)
            {
                if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                    ClearUser();
                else if (!Self.authClient.SessionCheck(Session["kSession"].ToString()))
                    ClearUser();
                else
                    return;
            }
            else
                ClearUser();
        }

        public void CheckDealer()
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (Session["kDealer"] != null)
            {
                CheckSession();
                if ((int)Session["kDealer"] < 1)
                    HttpContext.Current.Response.Redirect("/WholesaleContent/WholesaleDefault.aspx");
                else
                    return;
            }
            else
                ClearUser();
        }

        public bool CheckUserPass(string pass)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            int hash = Tuple.Create(pass, salt).GetHashCode();

            if (hash != int.Parse(Session["PHash"].ToString()))
                return false;

            return true;
        }

        public static void ClearUser(string e = "1")
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];

            if (Session != null || !string.IsNullOrEmpty(kSession))
            {
                HttpContext.Current.Session.Clear();
                //LMWholesale.WholesaleSystem.ClearCachedObjects();
            }

            HttpContext.Current.Response.Redirect("/WholesaleSystem/Login.aspx?e=" + e);
        }

        public void ChangeDealer(int kDealer, string DealerName)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            // Check if we have any previous Session items saved
            // SelectedVH, advancedFilter, filters
            if (kDealer != int.Parse(Session["kDealer"].ToString()))
                Session["SelectedVH"] = Session["advancedFilter"] = Session["filters"] =  Session["SelectedVH"] = null;

            Session["kDealer"] = kDealer;
            Session["DealerName"] = DealerName;

            GetDealerPreferences(kDealer);
        }

        public string GetGridDef(string kSession, string gridName, int kDealer = 0, int kPerson = 0)
        {
            Lookup.lmReturnValue grid = Self.lookupClient.GridDescriptionGet(kSession, kDealer, kPerson, gridName);
            return grid.Data.Tables[0].Rows[0]["GridDef"].ToString();
        }

        public string GradeSet(DataRow row, bool hasAutoGrade)
        {
            int VehicleGradeType = int.Parse(row["VehicleGradeType"].ToString());

            if (VehicleGradeType > 0 && row["AutoGrade"].ToString() != "" || row["ReportedGrade"].ToString() != "" || row["IndustryGrade"].ToString() != "")
            {
                if (!hasAutoGrade && VehicleGradeType == 1 && row["ReportedGrade"].ToString() != "")
                {
                    if (row["ReportedGrade"].ToString().Length == 1)
                        return string.Format("{0}.0", row["ReportedGrade"]);
                    else if (row["ReportedGrade"].ToString().Length == 2)
                        return string.Format("0.{0}", row["ReportedGrade"]);
                    else
                        return string.Format("{0}", row["ReportedGrade"]);
                }
                else if (!hasAutoGrade && (VehicleGradeType == 2
                    || VehicleGradeType == 3) && row["IndustryGrade"].ToString() != "")
                {
                    if (row["IndustryGrade"].ToString().Length == 1)
                        return string.Format("{0}.0", row["IndustryGrade"]);
                    else if (row["IndustryGrade"].ToString().Length == 2)
                        return string.Format("0.{0}", row["IndustryGrade"]);
                    else
                        return string.Format("{0}", row["IndustryGrade"]);
                }
                else if (VehicleGradeType == 4 && row["AutoGrade"].ToString() != "")
                {
                    if (row["AutoGrade"].ToString().Length == 1)
                        return string.Format("{0}.0", row["AutoGrade"]);
                    else if (row["AutoGrade"].ToString().Length == 2)
                        return string.Format("0.{0}", row["AutoGrade"]);
                    else
                        return string.Format("{0}", row["AutoGrade"]);
                }
                else
                    return "N/A";
            }
            else
                return "N/A";
        }

        #region CacheFactory Methods
        public object GetCachedObject(string Name, int CacheType = 1)
        {
            try
            {
                string cacheType = "";
                if (CacheType == 0) // GLOBAL
                    cacheType = "GLOBAL";
                else if (CacheType == 1)
                    cacheType = HttpContext.Current.Session["kPerson"].ToString();

                NamedCache nc = CacheFactory.GetCache(cacheType);
                return nc.Get(Name);
            }
            catch (Exception) { return null; }
        }

        public void SetCachedObject(string Name, object Value, int ExpireMinutes = 0, int CacheType = 1)
        {
            string cacheType = "";
            if (CacheType == 0) // GLOBAL
                cacheType = "GLOBAL";
            else if (CacheType == 1) // kPerson
            {
                cacheType = HttpContext.Current.Session["kPerson"].ToString();
                ExpireMinutes = 720;
            }

            NamedCache nc = CacheFactory.GetCache(cacheType);
            nc.DefaultCreatePolicy = new CreatePolicy(ExpireMinutes, false);
            nc.Add(Name, new CacheObject(Name, Value));
        }

        [Serializable]
        public class CacheObject
        {
            public string Name;
            public object Value;

            public CacheObject(string Name, object Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
        }
        #endregion
    }
}