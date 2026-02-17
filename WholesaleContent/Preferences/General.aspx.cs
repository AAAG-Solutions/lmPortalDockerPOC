using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;

using LMWholesale.Common;

namespace LMWholesale.WholesaleContent.Preferences
{
    public partial class General : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Preferences.General generalBll;

        public General()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            generalBll = generalBll ?? new BLL.WholesaleContent.Preferences.General();
        }

        public static General Self
        {
            get { return instance; }
        }
        private static readonly General instance = new General();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Preferences";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            if (!IsPostBack) {
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];

                Dealer.lmReturnValue dealerInfo = Self.generalBll.GetDealerInfo();

                if (dealerInfo.Result == Dealer.ReturnCode.LM_SUCCESS)
                {
                    DataRow DealerBase = dealerInfo.Data.Tables["DealerBase"].Rows[0];
                    DataTable ProductTable = dealerInfo.Data.Tables["DealerProduct"];

                    if (Self.userBLL.CheckPermission("LMIInternal"))
                    {
                        string productList = $"{ProductTable.Rows[0]["QBProductName"]}";
                        for (int i = 1; i < ProductTable.Rows.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(ProductTable.Rows[i]["Price"].ToString()))
                                productList += $", {ProductTable.Rows[i]["QBProductName"]}";
                        }
                        AccountProducts.Style["display"] = "table-row";
                        dealerProducts.Text = productList;
                    }

                    accountName.Text = DealerBase["DealerName"].ToString();
                    accountDisplay.Text = DealerBase["DisplayName"].ToString();
                    adressStreet.Text = DealerBase["DealerAddress1"].ToString();
                    addressCity.Text = DealerBase["DealerCity"].ToString();
                    addressZip.Text = DealerBase["DealerZip"].ToString();
                    dealerPhone.Text = DealerBase["DealerPhone"].ToString();
                    customType.Text = DealerBase["DealerCustomerType"].ToString();
                    chkTemplate.Checked = DealerBase["SimpleTemplateConfig"].ToString() == "1" ? true : false;
                    chkDemo.Checked = DealerBase["DemoFlag"].ToString() == "1" ? true : false;
                    dealerWebsite.Text = DealerBase["WebsiteURL"].ToString();

                    WholesaleSystem.BuildStateDropdown("lstState", DealerBase["DealerState"].ToString() == "" ? "0" : DealerBase["DealerState"].ToString());
                    WholesaleSystem.PopulateList(DealerBase["CountryList"].ToString(), "-- Select Country --", "lstCountry", '|');
                    WholesaleSystem.PopulateList(DealerBase["TimeZoneList"].ToString(), "", "lstTimezone", '|');
                    WholesaleSystem.PopulateList(DealerBase["DealerServiceList"].ToString(), "", "lstService", '|');
                    WholesaleSystem.PopulateList("[]1:Retail Dealer|2:Wholesale Dealer|3:Auto Auction|4:Auto Auction Dealer|5:Consignor|6:Billing Only|7:Website Rollup|8:Auto Auction Template|", "", "lstAccountType", '|');

                    var subGroups = dealerInfo.Data.Tables["DealerSubGroup"].AsEnumerable().Where(x => x["kDealerGaggle"].ToString() == DealerBase["kDealerGaggle"].ToString());
                    if (subGroups.Count() > 0)
                    {
                        string subGroup = "[]0:--Select A SubGroup--|";
                        foreach (var row in subGroups)
                            subGroup += row["kGaggleSubGroup"].ToString() + ":" + row["GaggleSubGroupName"].ToString() + "|";
                        WholesaleSystem.PopulateList(subGroup, "", "lstSubAccountGrp", '|', DealerBase["kGaggleSubGroup"].ToString());
                    }
                    else
                    {
                        WholesaleSystem.PopulateList("[]:--Select Subgroup--|", "", "lstSubAccountGrp", '|');
                        lstSubAccountGrp.Enabled = false;
                    }

                    string distributors = "[]0:--No Distributor--|";
                    foreach (DataRow row in dealerInfo.Data.Tables["DistributorList"].Rows)
                    {
                        distributors += row["kDistributor"].ToString() + ":" + row["DistributorName"].ToString() + "|";
                    }
                    WholesaleSystem.PopulateList(distributors, "", "lstDistributor", '|', DealerBase["kDistributor"].ToString());

                    string gaggles = "[]0:--No Account Group--|";
                    foreach (DataRow row in dealerInfo.Data.Tables["DealerGaggleList"].Rows)
                    {
                        gaggles += row["kDealerGaggle"].ToString() + ":" + row["DealerGaggleName"].ToString() + "|";
                    }
                    WholesaleSystem.PopulateList(gaggles, "", "lstAccountGrp", '|', DealerBase["kDealerGaggle"].ToString());
                    WholesaleSystem.PopulateList(gaggles, "", "lstAccountGrp", '|', DealerBase["kDealerGaggle"].ToString());

                    // Contact Section
                    // Dealer
                    DealerContactFName.Text = DealerBase["ContactFName"].ToString();
                    DealerContactLName.Text = DealerBase["ContactLName"].ToString();
                    DealerContactEmail.Text = DealerBase["ContactEmail"].ToString();
                    DealerContactPhone.Text = DealerBase["ContactPhone"].ToString();

                    // Rep
                    OwnerFName.Text = DealerBase["OwnerFName"].ToString();
                    OwnerLName.Text = DealerBase["OwnerLName"].ToString();
                    OwnerEmail.Text = DealerBase["OwnerEmail"].ToString();
                    OwnerPhone.Text = DealerBase["OwnerPhone"].ToString();

                    //WholesaleSystem.PopulateList(DealerBase[""].ToString(), "", "lstAccountGroup", '|');
                }
            }
        }

        [WebMethod(Description = "Save Dealer Base Information")]
        public static string SetDealerBase(string jsonInfo)
        {
            Dealer.lmReturnValue saveResult = Self.generalBll.DealerBaseSet(jsonInfo);
            if (saveResult.Result == Dealer.ReturnCode.LM_SUCCESS)
                return Util.serializer.Serialize(new Dictionary<string, object> { { "Success", 1 }, { "Message", "Information saved successfully" } });

            return Util.serializer.Serialize(new Dictionary<string, object> { { "Success", 0 }, { "Message", saveResult.ResultString } });
        }
    }
}