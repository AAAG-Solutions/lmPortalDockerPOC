using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

using LMWholesale.Common;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.WholesaleContent.Preferences
{
    public partial class AlternateCredentials : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Preferences.AlternateCredentials BLL;
        private readonly LookupClient lookupClient;

        public AlternateCredentials()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            BLL = BLL ?? new BLL.WholesaleContent.Preferences.AlternateCredentials();
        }

        public static AlternateCredentials Self
        {
            get { return instance; }
        }
        private static readonly AlternateCredentials instance = new AlternateCredentials();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Preferences";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);
            HttpSessionState Session = HttpContext.Current.Session;
            if (!IsPostBack)
            {
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];

                jsGridBuilder credentialGrid = new jsGridBuilder
                {
                    OnDoubleClickFunction = "EditAuctionCredential();",
                    OnRowSelectFunction = "GridRowSelected",
                    OnClearRowSelectFunction = "ClearRowSelection",
                    MethodURL = "AlternateCredentials.aspx/GetAlternateCredentials",
                    HTMLElement = "jsGrid",
                    Filtering = false,
                    Sorting = false
                };

                credentialGrid.SetFieldListFromGridDef(":AuctionName:Auction Name:40|:InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:DealerAccount:Dealer Account:100|:SellerID:Seller ID:100|:BuyerGroup:CarGroup ID:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|", "", true);

                if (!ClientScript.IsStartupScriptRegistered("credentialGridJsScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "jsGrid", credentialGrid.RenderGrid());
            }
        }

        [WebMethod(Description = "Get list of Auction Credentials for a given dealer and auction")]
        public static string GetAlternateCredentials(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            //Dictionary<string, object> oFilter = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);

            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            string tmpFail = "0 | {}";

            DataTable tblCredentials = Self.BLL.AlternateCredentialsGet(kSession, kDealer);
            if (tblCredentials.Rows.Count != 0)
                return tblCredentials.Rows.Count + "|" + FormatData(tblCredentials);
            else
                return tmpFail;
        }

        private static string FormatData(DataTable tblCredentials)
        {
            List<Dictionary<string, object>> lstCredential = new List<Dictionary<string, object>>();
            foreach (DataRow dr in tblCredentials.Rows)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (DataColumn column in tblCredentials.Columns)
                {
                    dict[column.ColumnName] = Util.cleanString((Convert.ToString(dr[column.ColumnName])));
                    dict["AuctionName"] = $"<input type='hidden' value='{dr["kWholesaleAuction"]}'>{(dr["kWholesaleAuction"].ToString() == "1" ? "OVE" : "AuctionEdge")}";
                }
                lstCredential.Add(dict);
            }

            return Util.serializer.Serialize(lstCredential);
        }
    }
}