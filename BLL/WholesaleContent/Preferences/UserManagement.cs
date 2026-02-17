using LMWholesale.Authenticate;
using LMWholesale.resource.clients;
using LMWholesale.resource.factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;

namespace LMWholesale.BLL.WholesaleContent.Preferences
{
    public class UserManagement
    {
        private readonly DASClient dasClient;
        private readonly LookupClient lookupClient;
        private readonly WholesaleClient wholesaleClient;
        private readonly DealerClient dealerClient;
        private readonly WholesaleUser.WholesaleUser userBLL;

        public UserManagement()
        {
            dasClient = dasClient ?? new DASClient();
            lookupClient = lookupClient ?? new LookupClient();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            dealerClient = dealerClient ?? new DealerClient();
            userBLL = userBLL ?? new WholesaleUser.WholesaleUser();
        }

        public UserManagement(DASClient dasClient, LookupClient lookupClient, WholesaleClient wholesaleClient, DealerClient dealerClient, WholesaleUser.WholesaleUser userBLL)
        {
            this.dasClient = dasClient;
            this.lookupClient = lookupClient;
            this.wholesaleClient = wholesaleClient;
            this.dealerClient = dealerClient;
            this.userBLL = userBLL;
        }

        internal static readonly UserManagement instance = new UserManagement();
        public UserManagement Self
        {
            get { return instance; }
        }

        public string GetInspectionCompanyList(string kSession)
        {
            DataTable tbl = null;
            object obj = LMWholesale.WholesaleSystem.GetCachedObject("InspectionCompanyLst");
            if (obj == null)
            {
                Wholesale.lmReturnValue rv = Self.wholesaleClient.InspectionCompanyGet(kSession);
                if (rv.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    tbl = rv.Data.Tables[0];
                    LMWholesale.WholesaleSystem.SetCachedObject("InspectionCompanyLst", tbl);
                }
                else if (rv.Result == Wholesale.ReturnCode.LM_INVALIDSESSION)
                    WholesaleUser.WholesaleUser.ClearUser();
                else if (rv.Result == Wholesale.ReturnCode.LM_DBERROR)
                    return null;
            }
            else
                tbl = (DataTable)obj;

            StringBuilder sb = new StringBuilder("[]0:-- Select Inspection Company --|");
            foreach (DataColumn dt in tbl.Columns)
            {
                foreach (DataRow dr in tbl.Rows)
                    sb.Append(dr["kWholesaleInspectionCompany"] + ":" + dr["InspectionCompany"] + "|");
            }

            // default return fail
            return sb.ToString();
        }

        public string GetDealerRelations(string kSession, int kDealer)
        {
            Dealer.lmReturnValue rv = Self.dealerClient.GetDealerInfo(kSession, kDealer, "", "RelationList");
            if (rv.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                DataTable relationsTbl = rv.Data.Tables[0];
                StringBuilder relations = new StringBuilder("[]0:-- Select a Relationship --|");
                foreach (DataColumn dt in relationsTbl.Columns)
                {
                    foreach (DataRow dr in relationsTbl.Rows)
                        relations.Append(dr["kRelation"].ToString() + ":" + dr["RelationDesc"].ToString() + "|");
                }
                return relations.ToString();
            }
            else if (rv.Result == Dealer.ReturnCode.LM_INVALIDSESSION)
                WholesaleUser.WholesaleUser.ClearUser();
            else if (rv.Result == Dealer.ReturnCode.LM_DBERROR)
                return "";

            // default return fail
            return "";
        }

        public DataTable GetDealerUsers(string kSession, int kDealer, Dictionary<string, object> filter)
        {
            string cacheKey = kDealer + "DealerUsers";
            object obj = LMWholesale.WholesaleSystem.GetCachedObject(cacheKey);

            if (obj == null)
            {
                Dealer.lmReturnValue rv = Self.dealerClient.GetDealerInfo(kSession, kDealer, "", "DealerUsers");
                if (rv.Result == Dealer.ReturnCode.LM_SUCCESS)
                {
                    DataTable dt = rv.Data.Tables[0];
                    LMWholesale.WholesaleSystem.SetCachedObject(cacheKey, dt);
                    return dt;
                }
                else if (rv.Result == Dealer.ReturnCode.LM_INVALIDSESSION)
                    WholesaleUser.WholesaleUser.ClearUser();
                else if (rv.Result == Dealer.ReturnCode.LM_DBERROR)
                    return null;
            }
            else
                return (DataTable)obj;

            // default return fail
            return null;
        }

        public DataTable GetUsers(string kSession, Dictionary<string, object> filter)
        {
            // Default value for now
            int kExceptDealer = 0;
            string textFilter = "LName Like 'a%'";

            if (filter.ContainsKey("textFilter") && filter["textFilter"].ToString() != "")
                textFilter = $"LName Like '%{filter["textFilter"]}%' OR UserID Like '%{filter["textFilter"]}%' OR Email Like '%{filter["textFilter"]}%'";

            string cacheKey = kSession + kExceptDealer;
            object obj = LMWholesale.WholesaleSystem.GetCachedObject(cacheKey);

            if (obj == null)
            {
                Lookup.lmReturnValue rv = Self.lookupClient.UserListGet(kSession, kExceptDealer);
                if (rv.Result == Lookup.ReturnCode.LM_SUCCESS) {
                
                    DataTable dt = rv.Data.Tables[0];
                    LMWholesale.WholesaleSystem.SetCachedObject(cacheKey, dt);
                    dt.DefaultView.RowFilter = textFilter;
                    return dt.DefaultView.ToTable();
                }
                else if (rv.Result == Lookup.ReturnCode.LM_INVALIDSESSION)
                    WholesaleUser.WholesaleUser.ClearUser();
                else if (rv.Result == Lookup.ReturnCode.LM_DBERROR)
                    return null;
            }
            else
            {
                DataTable dt = (DataTable)obj;
                dt.DefaultView.RowFilter = textFilter;
                return dt.DefaultView.ToTable();
            }

            // default return fail
            return null;
        }

        public bool UserInfoSave(string kSession, int kDealer, string op, Dictionary<string, object> json) {
            if (op == "addUser")
            {

            }
            else if (op == "editUser")
            {

            }
            else if (op == "addDealerUser")
            {

            }
            else {

            }

            // If for some reason we fail, just set to false
            return false;
        }
    }
}