using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using LMWholesale.resource.clients;

namespace LMWholesale.BLL.WholesaleContent.Reporting
{
    public class CreditRequest
    {
        private readonly WholesaleClient wholesaleClient;

        public CreditRequest() => wholesaleClient = new WholesaleClient();

        public CreditRequest(WholesaleClient wholesaleClient) => this.wholesaleClient = wholesaleClient;

        internal static readonly CreditRequest instance = new CreditRequest();
        public CreditRequest Self
        {
            get { return instance; }
        }

        public bool CreditRequestSet(string kSession, int kDealer, int kAccountType, Dictionary<string, object> info, ref string resultString)
        {
            Wholesale.lmReturnValue creditApproval = Self.wholesaleClient.WholesaleCreditRequestSet(kSession, kDealer, info);
            if (creditApproval.Result == Wholesale.ReturnCode.LM_SUCCESS)
                return true;
            else
            {
                if (creditApproval.ResultString.Contains("85000"))
                    resultString = "The system did not find a billable sale record for the VIN provided on the selected marketplace during the last billing cycle or for the current billing cycle. If you have credit requests for a prior timeframe, please contact support@liquidmotors.com.";
                else if (creditApproval.ResultString.Contains("85001"))
                    resultString = "A credit request already exists for this VIN. If you have any questions, please contact support@liquidmotors.com.";
                else if (creditApproval.ResultString.Contains("85002"))
                    resultString = "Multiple sale records were found for this VIN and a credit could not be automatically applied. Please contact support@liquidmotors.com.";
                else if (creditApproval.ResultString.Contains("85003"))
                {
                    if (kAccountType == 3)
                        resultString = "An invoice has already been generated for this VIN. A credit will appear on your next invoice.";
                    else
                        resultString = "An invoice has already been generated for this VIN. If a credit is due, it will appear on your next invoice.";
                }
                else
                    resultString = $"Unable to perform request due to the following: {creditApproval.ResultString}";
            }

            return false;
        }

        public string CreditRequestGet(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue creditApprovals = Self.wholesaleClient.WholesaleCreditRequestGet(kSession, kDealer);

            if (creditApprovals.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable data = creditApprovals.Data.Tables[0];
                if (data.Rows.Count > 0)
                    return $"{data.Rows.Count}|{Util.serializer.Serialize(FormatData(data))}";
            }

            return "0|{}";
        }

        private List<Dictionary<string, object>> FormatData(DataTable dt)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                    dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));

                returnList.Add(dict);
            }

            return returnList;
        }
    }
}