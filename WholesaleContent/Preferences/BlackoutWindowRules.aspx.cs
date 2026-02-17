using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.SessionState;
using System.Text;

using LMWholesale.Common;
using LMWholesale.resource.factory;
using LMWholesale.resource.clients;

namespace LMWholesale.WholesaleContent.Preferences
{
    public partial class BlackoutWindowRules : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Preferences.BlackoutWindowRules blackoutBLL;
        private readonly WholesaleClient wholesaleClient;
        private readonly DASClient dasClient;
        private readonly ListingClient listingClient;
        private readonly AuctionFactory auctionFactory;

        public BlackoutWindowRules()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            blackoutBLL = blackoutBLL ?? new BLL.WholesaleContent.Preferences.BlackoutWindowRules();
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            dasClient = dasClient ?? new DASClient();
            listingClient = listingClient ?? new ListingClient();
            auctionFactory = auctionFactory ?? new AuctionFactory();
        }

        public BlackoutWindowRules(BLL.WholesaleUser.WholesaleUser userBLL, BLL.WholesaleContent.Preferences.BlackoutWindowRules blackoutBLL, WholesaleClient wholesaleClient,
                                    DASClient dasClient, ListingClient listingClient, AuctionFactory auctionFactory)
        {
            this.userBLL = userBLL;
            this.blackoutBLL = blackoutBLL;
            this.wholesaleClient = wholesaleClient;
            this.dasClient = dasClient;
            this.listingClient = listingClient;
            this.auctionFactory = auctionFactory;
        }

        public static BlackoutWindowRules Self
        {
            get { return instance; }
        }
        private static readonly BlackoutWindowRules instance = new BlackoutWindowRules();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Preferences";
            Self.userBLL.CheckDealer();

            if (!IsPostBack)
            {
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];
                PageSecurityManager.DoPageSecurity(this);

                if (!String.IsNullOrEmpty(Request.QueryString["ListMode"]))
                {
                    hfViewList.Value = "1";
                }

                DealerName.Text = Session["DealerName"].ToString();

                jsGridBuilder rulegrid = new jsGridBuilder
                {
                    MethodURL = "BlackoutWindowRules.aspx/GetBlackoutWindowRules",
                    OnRowSelectFunction = "GridRowSelected",
                    OnClearRowSelectFunction = "ClearRowSelection",
                    OnDoubleClickFunction = "RowDoubleClick();",
                    HTMLElement = "jsGrid",
                    Filtering = false
                };

                rulegrid.ExtraFunctionality = $@"
                    var gridData = $('#jsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Suspended == ""1"") {{
                                $('#jsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
                ";

                rulegrid.SetFieldListFromGridDef(":Auction::100|:Make::100|:InvLotLocation:Lot Location:100|:StartDOW:Start Day:50|:StartTime:Start Time:100|:EndDOW:End Day:50|:EndTime:End Time:100|:MaxYear:Max Year:60|:MinYear:Min Year:60|:Type::80|:Interval::50|", "", true);

                if (!ClientScript.IsStartupScriptRegistered("JSScript"))
                    ClientScript.RegisterStartupScript(this.GetType(), "blackoutGrid", rulegrid.RenderGrid());

                // Gather AutoLaunch Roles for a given dealer account
                //RuleList.InnerHtml = GatherBlackoutWindowRules(sessid, kDealer);

                Page page = (Page)HttpContext.Current.Handler;
                Panel AuctionChecksPanel = (Panel)page.Master.FindControl("MainContent").FindControl("AuctionChecks");
                List<Dictionary<string, string>> auctions = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient);

                int numCols = Self.blackoutBLL.DetermineBestFit(auctions.Count);

                Panel newRow = new Panel() { CssClass = "tableRow" };
                for (int i = 0; i < auctions.Count; i++)
                {
                    if (auctions[i]["WholesaleAuctionName"] == "CarOffer")
                        continue;

                    if (i > 0 && i % numCols == 0)
                    {
                        AuctionChecksPanel.Controls.Add(newRow);
                        newRow = new Panel() { CssClass = "tableRow" };
                    }
                    Label label = new Label()
                    {
                        ID = "lbl" + auctions[i]["WholesaleAuctionName"],
                        Text = auctions[i]["WholesaleAuctionName"] == "RemarketingPlus" ? "Remarketing+" : auctions[i]["WholesaleAuctionName"] + ":",
                        CssClass = "tableCell ColRowSwap ColRowSwapLabel",
                    };
                    CheckBox checkBox = new CheckBox()
                    {
                        ID = "cb" + auctions[i]["WholesaleAuctionName"],
                        CssClass = "ColRowSwap"
                    };

                    checkBox.InputAttributes.Add("class", "SingleIndent");
                    newRow.Controls.Add(label);
                    newRow.Controls.Add(checkBox);
                }
                AuctionChecksPanel.Controls.Add(newRow);

                string lstAuction = "[]";
                foreach (Dictionary<string, string> auction in auctions)
                {
                    if (auction["WholesaleAuctionName"] == "CarOffer")
                        continue;
                    lstAuction += $"{auction["kWholesaleAuction"]}:{(auction["WholesaleAuctionName"] == "RemarketingPlus" ? "Remarketing+" : auction["WholesaleAuctionName"])}|";
                }
                WholesaleSystem.PopulateList(lstAuction, "-- Select and Auction --", "lstAuction", '|', "0");

                StringBuilder makes = new StringBuilder("[] :Any Make|");
                if (auctions.Count() > 0)
                {
                    IAuctionService auctionService = Self.auctionFactory.GetAuctionService(int.Parse(auctions[0]["kWholesaleAuction"], 0));
                    string chromeMake = auctionService.GetAuctionInfo(kSession, kDealer).Rows[0]["ChromeMake"].ToString();
                    WholesaleSystem.PopulateList(makes.Append(chromeMake.Replace("[]", "")).ToString(), "Any Make", "lstVehicleMake", '|', "0");
                }

                DAS.lmReturnValue years = Self.dasClient.DASGetMotorYears(kSession, "3");
                if (years.Result == DAS.ReturnCode.LM_SUCCESS)
                {
                    StringBuilder lstYears = new StringBuilder("[]");

                    DataTable dt = years.Data.Tables[0];
                    foreach (DataRow row in dt.Rows)
                        lstYears.Append($"{row["Years"]}:{row["Years"]}|");

                    WholesaleSystem.PopulateList(lstYears.ToString(), "Any Year", "lstMinYear", '|', "0");
                    WholesaleSystem.PopulateList(lstYears.ToString(), "Any Year", "lstMaxYear", '|', "0");
                }

                string days = "[]Mon:Mon|Tue:Tue|Wed:Wed|Thu:Thu|Fri:Fri|Sat:Sat|Sun:Sun|";
                WholesaleSystem.PopulateList(days, "", "lstStartDay", '|', "Mon");
                WholesaleSystem.PopulateList(days, "", "lstEndDay", '|', "Mon");
                WholesaleSystem.PopulateList(days, "", "lstIntervalMonthDay", '|', "Mon");

                WholesaleSystem.PopulateList("[]1:Weekly|2:Monthly|", "", "lstFrequency", '|', "Weekly");
                WholesaleSystem.PopulateList("[]1:Every Week|2:Every Two Weeks|3:Every Three Weeks|4:Every Four Weeks|5:Every Five Weeks|", "", "lstIntervalWeek", '|', "Every Week");
                WholesaleSystem.PopulateList("[]1:The First|2:The Second|3:The Third|4:The Fourth|5:The Fifth|-1:The Last|-2:The Next to Last|", "", "lstIntervalMonth", '|', "The First");

                Listing.lmReturnValue lotList = Self.listingClient.ListingLotLocationListGet(kSession, kDealer);
                string lotString = "[][ANY]:Any Lot Location|";
                if (lotList.Result == Listing.ReturnCode.LM_SUCCESS)
                {
                    if (lotList.Data != null)
                    {
                        DataTable ll = lotList.Data.Tables[0];
                        foreach (DataRow row in ll.Rows)
                        {
                            string location = Convert.ToString(row["InvLotLocation"]);
                            lotString += $"{location}:{location}|";
                        }
                    }

                    WholesaleSystem.PopulateList(lotString, "", "lstLotLocation", '|', "[ANY]");
                }

                Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleBlackoutGetData(kSession, kDealer.ToString());
                if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    DataTable rules = returnValue.Data.Tables[0];
                    ListForm.InnerHtml =
                        Self.blackoutBLL.BuildListForm(FormatData(rules, kSession, kDealer), auctions);
                }
            }
        }

        [WebMethod(Description = "Gather all Blackout Window Rules associated with a given dealer")]
        public static string GetBlackoutWindowRules(string filter)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            //Dictionary<string, object> oFilter = (Dictionary<string, object>)Util.serializer.DeserializeObject(filter);

            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleBlackoutGetData(kSession, kDealer.ToString());
            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS) {
                DataTable rules = returnValue.Data.Tables[0];
                return returnValue.Values.GetValue("TotalItems", "0") + "|" + Util.serializer.Serialize(FormatData(rules, kSession, kDealer));
            }

            return "0 | {}";
        }

        private static List<Dictionary<string, object>> FormatData(DataTable dt, string kSession, int kDealer)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();
            List<Dictionary<string, string>> auctions = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient);

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = Util.cleanString((Convert.ToString(row[col])));
                }

                dict["Auction"] = auctions.First(d => d.ContainsValue(row["kWholesaleAuction"].ToString()))["WholesaleAuctionName"];
                if (dict["Auction"].ToString() == "RemarketingPlus")
                    dict["Auction"] = "Remarketing+";
                dict["Type"] = row["kBlackoutWindowIntervalType"].ToString() == "1" ? "Weekly" : "Monthly";

                returnList.Add(dict);
            }

            return returnList;
        }

        private static string GatherBlackoutWindowRules(string session, int kDealer)
        {
            List<string> stringList = new List<string>();

            Wholesale.lmReturnValue returnValue = Self.wholesaleClient.WholesaleBlackoutTextGet(session, kDealer);

            if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = returnValue.Data.Tables[0];
                int NumRows = dt.Rows.Count;

                if (NumRows >= 1)
                {
                    string[] KeyWords = { "criteria", "settings" };
                    int RuleNum = 1;
                    //string RuleString = "<span class='rules'>Rule #RULENUM: </span>";
                    string RuleString = "<span class='rules' onclick='openRule(RULENUM)';return false;>Rule #RULENUM: </span><div id='ruleNum_RULENUM' class='ruleText'>";

                    // Initial Rule #
                    stringList.Add(RuleString.Replace("RULENUM", RuleNum.ToString()));
                    int count = 1;

                    foreach (DataRow row in dt.Rows)
                    {
                        string r = row[1].ToString();

                        if (r == "")
                        {
                            RuleNum += 1;
                            count += 1;
                            stringList.Add("</div>");
                            stringList.Add(RuleString.Replace("RULENUM", RuleNum.ToString()));
                            continue;
                        }
                        else if (count == NumRows - 1)
                        {
                            break;
                        }

                        if (!(KeyWords.Any(r.Contains)))
                        {
                            stringList.Add($"<span>&emsp;{r}</span><br/>");
                        }
                        else
                        {
                            stringList.Add($"{r}<br/>");
                        }

                        count += 1;
                    }
                    stringList.Add("</div>");
                }
                else
                {
                    stringList.Add("<span class='rules'>There are no Blackout Window Rules set!</span><br/>");
                    return String.Join("", stringList.ToArray());
                }
            }

            return String.Join("", stringList.ToArray());
        }

        [WebMethod(Description = "Save Blackout Window Rules")]
        public static string SaveRules(string DataIn)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            Dictionary<string,object> data = (Dictionary<string, object>)Util.serializer.DeserializeObject(DataIn.Replace("\\", ""));
            Wholesale.lmReturnValue returnValue = null;
            if (data["Auctions"] != null)
            {
                List<Dictionary<string, string>> auctions = WholesaleSystem.GetAvailableAuctions(kSession, kDealer, Self.wholesaleClient, 0);
                foreach (object item in (object[])data["Auctions"])
                {
                    Dictionary<string, string> DataOut = Self.blackoutBLL.BuildDataDictionary(auctions.Where(x => x["WholesaleAuctionName"] == item.ToString()).First()["kWholesaleAuction"], data);
                    returnValue = Self.wholesaleClient.WholesaleBlackoutSetData(kSession, kDealer, Util.serializer.Serialize(DataOut));
                    if (returnValue.Result != Wholesale.ReturnCode.LM_SUCCESS)
                        break;
                }
            }
            else
            {
                Dictionary<string, string> DataOut = Self.blackoutBLL.BuildDataDictionary(data["Auction"].ToString(), data);
                returnValue = Self.wholesaleClient.WholesaleBlackoutSetData(kSession, kDealer, Util.serializer.Serialize(DataOut));
            }

            if(returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                return Util.serializer.Serialize(new Dictionary<string, object> { { "Success", 1 }, { "Message", "" } });

            return Util.serializer.Serialize(new Dictionary<string, object> { { "Success", 0 }, { "Message", returnValue.ResultString } });
        }
    }
}