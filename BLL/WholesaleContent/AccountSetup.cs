using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using LMWholesale.resource.clients;

namespace LMWholesale.BLL.WholesaleContent
{
    public class AccountSetup
    {
        private DealerClient dealerClient;
        private LookupClient lookupClient;

        public AccountSetup()
        {
            dealerClient = dealerClient ?? new DealerClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public AccountSetup(DealerClient dealerClient, LookupClient lookupClient)
        {
            this.dealerClient = dealerClient;
            this.lookupClient = lookupClient;
        }

        internal static readonly AccountSetup instance = new AccountSetup();
        public AccountSetup Self
        {
            get { return instance; }
        }

        public bool ValidDealerName(string kSession, string DealerName)
        {
            Dealer.lmReturnValue ValidName = Self.dealerClient.DealerNameCheck(kSession, DealerName);
            if (ValidName.Result == Dealer.ReturnCode.LM_SUCCESS)
                return true;
            else if (ValidName.Result == Dealer.ReturnCode.LM_INVALIDSESSION)
                WholesaleUser.WholesaleUser.ClearUser();

            return false;
        }

        public string GetApprovalGridData(string kSession)
        {
            string tmpFail = "0 | {}";

            Dealer.lmReturnValue returnVal = Self.dealerClient.GetSignupPending(kSession);
            if (returnVal.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                DataTable pendingSignups = returnVal.Data.Tables[0];
                return pendingSignups.Rows.Count + "|" + Util.serializer.Serialize(FormatData(pendingSignups));
            }

            return tmpFail;
        }

        public string GetTemplateDealers(string kSession, string SelectedAuction)
        {
            string retVal = "";
            Lookup.lmReturnValue ret = lookupClient.GetDealerList(kSession);
            if (ret.Result == Lookup.ReturnCode.LM_SUCCESS)
            {
                DataTable table = ret.Data.Tables[0];
                table.DefaultView.RowFilter = "kGaggleSubGroup = " + SelectedAuction + " And kAccountType = 8";
                foreach (DataRow row in table.DefaultView.ToTable().Rows)
                {
                    retVal += row["kDealer"] + ":" + row["DealerName"] + "|";
                }
            }
            return retVal;
        }

        public string GetSignupAuctions(string kSession)
        {
            string retVal = "[]";
            Dealer.lmReturnValue ret = dealerClient.GetSignupAuctions(kSession);
            if (ret.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                foreach (DataRow row in ret.Data.Tables[0].Rows)
                {
                    retVal += row["kGaggleSubGroup"] + ":" + row["GaggleSubGroupName"] + "|";
                }
            }
            return retVal;
        }

        public string GetImportSystems(string kSession, int VehicleInvAccount)
        {
            string retVal = "";
            Dealer.lmReturnValue ret = dealerClient.ImportSystemGet(Util.GetIniEntry("AllySetupSession"), VehicleInvAccount.ToString());

            if (ret.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                retVal = "-1:Select Inventory Source|";
                if (!retVal.Contains("Liquid Connect"))
                    retVal += "-100:Liquid Connect|";
                foreach (DataRow row in ret.Data.Tables["Systems"].AsEnumerable().Where(x => x["Active"].ToString() == "1"))
                {
                    retVal += row["kDealerImport"] + ":" + row["ImportDesc"] + "|";
                }
                if (!retVal.Contains("Wayne Reaves"))
                    retVal += "-110:Wayne Reaves|";
            }
            
            return retVal;
        }

        public Dictionary<string, string> GetSpecificSignup(string kSession, string kWholesaleSignup)
        {
            return FormatJsonData(Self.dealerClient.GetPendingSetup(kSession, Convert.ToInt32(kWholesaleSignup)));
        }

        private Dictionary<string, string> FormatJsonData(Dealer.lmReturnValue retVal)
        {
            Dictionary<string, string> retDict = new Dictionary<string, string>
            {
                { "Result", retVal.Result == Dealer.ReturnCode.LM_SUCCESS ? "success" : "error" },
                { "ResultString", retVal.ResultString }
            };
            if (retVal.Data.Tables.Count > 0 && retVal.Data.Tables[0].Rows.Count > 0)
            {
                DataRow row = retVal.Data.Tables[0].Rows[0];
                foreach (DataColumn col in row.Table.Columns)
                {
                    retDict.Add(col.ColumnName, row[col.ColumnName].ToString());

                }
            }
            return retDict;
        }

        public Dealer.lmReturnValue SetSignupData(string kSession, Dictionary<string, object> data)
        {
            DataTable dealers = lookupClient.GetDealerList(kSession).Data.Tables[0];
            DataTable dtCustomerType = dealers.DefaultView.ToTable(true, "kGaggleSubGroup", "CustomerType", "kAccountType");
            DataTable dtBillingGroup = dealers.DefaultView.ToTable(true, "kGaggleSubGroup", "kBillingGroup", "kAccountType");
            dtCustomerType.DefaultView.RowFilter = "kGaggleSubGroup = " + data["kDealerSubGaggle"].ToString();
            dtBillingGroup.DefaultView.RowFilter = "kGaggleSubGroup = " + data["kDealerSubGaggle"].ToString();
            int numCustRec = dtCustomerType.DefaultView.Count;
            int numBillRec = dtBillingGroup.DefaultView.Count;
            int iCustomerType = 0;
            int iBillingGroup = 0;
            if (numCustRec == 1)
            {
                iCustomerType = Convert.ToInt32(dtCustomerType.DefaultView[0]["CustomerType"]);
            }
            else
            {
                dtCustomerType.DefaultView.RowFilter = "kGaggleSubGroup = " + data["kDealerSubGaggle"].ToString() + " and kAccountType = 3";
                numCustRec = dtCustomerType.DefaultView.Count;
                if (numCustRec == 1)
                    iCustomerType = Convert.ToInt32(dtCustomerType.DefaultView[0]["CustomerType"]);
            }


            if (numBillRec == 1)
                iBillingGroup = Convert.ToInt32(dtBillingGroup.DefaultView[0]["kBillingGroup"]);
            else
            {
                dtBillingGroup.DefaultView.RowFilter = "kGaggleSubGroup = " + data["kDealerSubGaggle"].ToString() + " and kAccountType = 3";
                numBillRec = dtBillingGroup.DefaultView.Count;
                if (numBillRec == 1) 
                    iBillingGroup = Convert.ToInt32(dtBillingGroup.DefaultView[0]["kBillingGroup"]);
            }

            data["CustomerType"] = iCustomerType;
            data["kBillingGroup"] = iBillingGroup;
            data.Add("kSession", kSession);

            return dealerClient.SetSignup(kSession, Util.serializer.Serialize(data));
        }

        public List<Dictionary<string, object>> FormatData(DataTable PendingSignups)
        {
            List<Dictionary<string, object>> retObj = new List<Dictionary<string, object>>();

            foreach (DataRow row in PendingSignups.Rows)
            {
                Dictionary<string, object> item = new Dictionary<string, object>
                {
                    { "kWholesaleSignup", row["kWholesaleSignup"].ToString() },
                    { "RequestUser", row["RequestUser"].ToString() },
                    { "RequestUserID", row["RequestUserID"].ToString() },
                    { "kDealerSubGaggle", row["kDealerSubGaggle"].ToString() },
                    { "GaggleSubGroupName", row["GaggleSubGroupName"].ToString() },
                    { "DealerName", row["DealerName"].ToString() },
                    { "Posted", row["Posted"].ToString() }
                };

                retObj.Add(item);
            }

            return retObj;
        }

        
    }
}