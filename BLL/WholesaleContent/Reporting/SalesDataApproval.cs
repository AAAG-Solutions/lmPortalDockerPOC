using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using LMWholesale.resource.clients;

namespace LMWholesale.BLL.WholesaleContent.Reporting
{
    public class SalesDataApproval
    {
        private WholesaleUser.WholesaleUser userBLL;
        private WholesaleClient wholesaleClient;

        public SalesDataApproval()
        {
            userBLL = userBLL ?? new WholesaleUser.WholesaleUser();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
        }
        internal static readonly SalesDataApproval instance = new SalesDataApproval();
        public SalesDataApproval Self
        {
            get { return instance; }
        }

        public bool SetSalesData(string kSession, string JsonData, ref Dictionary<string, int> returnData, bool isSend = true)
        {
            bool isSuccess = true;
            object[] data = (object[])Util.serializer.DeserializeObject(JsonData);

            int success = 0;
            int fail = 0;
            foreach (Dictionary<string, object> item in data)
            {
                item.Add("kSession", kSession);
                if (isSend)
                {
                    Wholesale.lmReturnValue returnValue = Self.wholesaleClient.SalesDataApprovalSet(Util.serializer.Serialize(item));
                    if (returnValue.Result != Wholesale.ReturnCode.LM_SUCCESS)
                    {
                        fail++;
                        isSuccess = false;
                    }
                    else
                        success++;
                }
                else
                {
                    Wholesale.lmReturnValue returnValue = Self.wholesaleClient.SalesDataApprovalMark(Util.serializer.Serialize(item));
                    if (returnValue.Result != Wholesale.ReturnCode.LM_SUCCESS)
                    {
                        fail++;
                        isSuccess = false;
                    }
                    else
                        success++;
                }
            }

            returnData = new Dictionary<string, int> { { "success", success }, { "fail", fail } };
            return isSuccess;
        }

        public string GetSalesData(Dictionary<string, string> filter, string sortField, string sortOrder)
        {
            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.SalesDataApprovalGet(Util.serializer.Serialize(filter));
            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable salesData = returnValue.Data.Tables[0];
                if (salesData.Rows.Count > 0)
                    return $"{salesData.Rows.Count}|{Util.serializer.Serialize(FormatData(salesData, sortField, sortOrder))}";
            }

            return "0|{}";
        }

        private List<Dictionary<string, object>> FormatData(DataTable SalesData, string sortField, string sortOrder)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();

            // Sort before we format
            Dictionary<string, string> sortMapping = new Dictionary<string, string>()
            {
                { "chkSales", "ApprovedStatus"},
                { "YearMakeModelStyle", "Year,Make,Model,Style" },
                { "SellerAuctionAccessEntry", "SellerAuctionAccess" },
                { "BuyerAuctionAccessEntry", "BuyerAuctionAccess" },
            };
            string sort = sortMapping.ContainsKey(sortField) ? sortMapping[sortField] : sortField;
            if (!string.IsNullOrEmpty(sortOrder) && sortOrder.Contains("desc"))
                sort += " desc";

            if (sortField == "YearMakeModelStyle" && !string.IsNullOrEmpty(sortOrder) && sortOrder.Contains("desc"))
                sort = sort.Replace(",", " desc,");

            DataView dv = SalesData.DefaultView;
            dv.Sort = sort;
            DataTable filteredDT = dv.ToTable();

            int iCnt = 0;
            foreach (DataRow row in filteredDT.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in filteredDT.Columns)
                {
                    dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));
                }

                if (row["ApprovedStatus"].ToString() == "0")
                {
                    dict["chkSales"] = $"<input type='checkbox' id='chkSalesRow_{iCnt}' name='chkSalesRow_{iCnt}' kval='{row["kWholesaleSoldHistory"]}' />";
                    dict["chkSales"] += " <label>Send Sales Data</label>";
                    if (row["StatusMessage"].ToString() != "")
                        dict["chkSales"] += $"<br />Status: {row["StatusMessage"]}";

                    string onlyRead = row["SellerAuctionAccess"].ToString() == "" ? "" : "readonly";
                    dict["SellerAuctionAccessEntry"] =
                        $"<input type='text' id='txtSellerAuctionAccess_{iCnt}' name='txtSellerAuctionAccess_{iCnt}' value='{row["SellerAuctionAccess"]}' style='font-size:10px;height:20px;width:50px;' {onlyRead}>";

                    onlyRead = row["BuyerAuctionAccess"].ToString() == "" ? "" : "readonly";
                    dict["BuyerAuctionAccessEntry"] =
                        $"<input type='text' id='txtBuyerAuctionAccess_{iCnt}' name='txtBuyerAuctionAccess_{iCnt}' value='{row["BuyerAuctionAccess"]}' style='font-size:10px;height:20px;width:50px;' {onlyRead}>";
                    dict["Mileage"] =
                        $"<input type='text' id='txtMileage_{iCnt}' name='txtMileage_{iCnt}' value='{row["Mileage"]}' style='font-size:10px;height:20px;width:50px;' {onlyRead}>";
                }
                else
                {
                    dict["chkSales"] = $"<label id='chkSalesRow_{iCnt}'>Sent:</label><br />{row["StatusMessage"]}";
                    dict["SellerAuctionAccessEntry"] = row["SellerAuctionAccess"].ToString();
                    dict["BuyerAuctionAccessEntry"] = row["BuyerAuctionAccess"].ToString();
                }

                // Just chopping off 'Email' usernames
                if (row["ApprovedBy"].ToString().Contains("@"))
                    dict["ApprovedBy"] = row["ApprovedBy"].ToString().Substring(0, row["ApprovedBy"].ToString().IndexOf("@"));
                dict["YearMakeModelStyle"] = $"{row["Year"]} {row["Make"]} {row["Model"]}<br />{row["Style"]}";

                dict["Marketplace"] = GetAuctionName(dict["Marketplace"].ToString());

                iCnt += 1;
                returnList.Add(dict);
            }

            return returnList;
        }

        private string GetAuctionName(string Num)
        {
            Dictionary<string, string> auctions = new Dictionary<string, string>()
            {
                { "1", "OVE" },
                { "2", "SmartAuction" },
                { "4", "ADESA" },
                { "6", "COPART" },
                { "7", "Auction Edge" },
                { "10", "Turn Auctions" },
                { "11", "ACV Auctions" },
                { "12", "eDealer Direct" },
                { "13", "IAA" },
                { "14", "Auction Simplified" },
                { "15", "IAS" },
                { "16", "AuctionOS" },
                { "17", "Carmigo" },
                { "18", "CarOffer" },
                { "19", "Remarketing+" } // might have to change this
            };
            return auctions[Num];
        }

        public Dictionary<string, string> ExportInventory(string filterString)
        {
            string stringDate = DateTime.Now.ToString("yyyyMMddTHHmmss");
            DataTable dt = new DataTable();
            StringBuilder header = new StringBuilder();
            StringBuilder content = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            string fileName = $"SalesDataExport_{stringDate}.csv";

            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.SalesDataApprovalGet(filterString);

            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                dt = returnValue.Data.Tables[0];

            // Just get general user VehicleManagement Columns
            string WholesaleGridColumns = "SaleDate:Sale Date|VIN:VIN|Year:Year|Make:Make|Model:Model|Style:Style|Mileage:Mileage|SellerName:Seller Name|SellerAuctionAccess:Seller #|BuyerName:Buyer Name|BuyerAddress:Buyer Address|BuyerAuctionAccess:Buyer #|SalePrice:Sale Price|ApprovedBy:Approved By|ApprovedDate:Approved Date";
            IEnumerable<string[]> lstColumns = WholesaleGridColumns.Split('|').Select(column => column.Split(':'));
            int count = 0;

            foreach (DataRow dr in dt.Rows)
            {
                foreach (string[] column in lstColumns)
                {
                    if (column[1] == "")
                        column[1] = column[0];

                    content.Append(Util.CreateCSV(dr[column[0]].ToString()) + ",");
                    if (count < lstColumns.Count())
                    {
                        header.Append(column[1] + ",");
                        count += 1;
                    }
                }

                content.Remove(content.Length - 1, 1);
                content.AppendLine();
            }

            // Combine header and content
            sb.Append(header.ToString());
            sb.AppendLine();
            sb.Append(content.ToString());

            Dictionary<string, string> rv = new Dictionary<string, string>
            {
                { "fileName", fileName },
                { "sb", sb.ToString() }
            };

            return rv;
        }
    }
}