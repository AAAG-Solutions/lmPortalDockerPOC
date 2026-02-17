using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.Services;

using LMWholesale.Common;
using LMWholesale.BLL.WholesaleUser;
using LMWholesale.resource.clients;

namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class StartWholesale : lmPage
    {
        private readonly BLL.WholesaleContent.Vehicle.StartWholesale startwholesaleBLL;
        private readonly WholesaleUser userBLL;
        private readonly DealerClient dealerClient;

        public StartWholesale()
        {
            startwholesaleBLL = startwholesaleBLL ?? new BLL.WholesaleContent.Vehicle.StartWholesale();
            userBLL = userBLL ?? new WholesaleUser();
            dealerClient = dealerClient ?? new DealerClient();
        }

        public StartWholesale(BLL.WholesaleContent.Vehicle.StartWholesale startwholesaleBLL, DealerClient dealerClient, WholesaleUser userBLL)
        {
            this.startwholesaleBLL = startwholesaleBLL;
            this.userBLL = userBLL;
            this.dealerClient = dealerClient;
        }

        public static StartWholesale Self
        {
            get { return instance; }
        }

        private static StartWholesale instance = new StartWholesale();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Start Wholesale";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["kListing"]))
                {
                    string kListing = !String.IsNullOrEmpty(Request.QueryString["kListing"]) ? Request.QueryString["kListing"] : "";

                    endWholesale.HRef = $"/WholesaleContent/Vehicle/EndWholesale.aspx?kListing={kListing}";
                    CurrenkListing.Value = kListing;

                    if (Regex.IsMatch(Request.UrlReferrer.AbsolutePath, "VehicleManagement.aspx$"))
                    {
                        BackToVehicle.InnerHtml = $"&#60; &#8722; <a class='backBreadcrumb' href=\"javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx';\">Back To Inventory</a>";
                        Cancel.InnerHtml = $"<input type=\"button\" class=\"actionBackground\" value=\"Cancel\" onclick=\"javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx';\">";
                    }
                    else
                    {
                        BackToVehicle.InnerHtml = $"&#60; &#8722; <a class='backBreadcrumb' href=\"javascript: window.location.href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}';\">Back To Vehicle</a>";
                        Cancel.InnerHtml = $"<input type=\"button\" class=\"actionBackground\" value=\"Cancel\" onclick=\"javascript: window.location.href='/WholesaleContent/Vehicle/Update.aspx?kListing={kListing}';\">";
                    }

                    DataRow dr = Self.startwholesaleBLL.GetListingDetails(int.Parse(kListing));
                    // Search UserPermissions to make sure that we have 'AutoGrade' enabled
                    bool hasAutoGrade = ((List<string>)Session["UserPermissions"]).Where(x => x.Contains("AutoGrade")).Count() > 0;

                    // Populate dropdowns and header
                    string yearString = dr["MotorYear"].ToString();
                    string makeString = dr["Make"].ToString();
                    string modelString = dr["Model"].ToString();
                    int costPrice = Convert.ToInt32(dr["InvCost"]);

                    // MMR
                    MMRValue.Text = dr["MMRGoodPrice"].ToString();
                    int mmrGoodPrice = int.Parse(dr["MMRGoodPrice"].ToString());

                    BINPrice.Value = dr["WholesaleBuyNow"].ToString();
                    ReservePrice.Value = dr["WholesaleFloor"].ToString();
                    StartPrice.Value = dr["WholesaleStartPrice"].ToString();
                    VehicleGrade.Text = Self.userBLL.GradeSet(dr, hasAutoGrade);

                    StartPrice.Attributes["oninput"] = ReservePrice.Attributes["oninput"] = BINPrice.Attributes["oninput"] = RelistCount.Attributes["oninput"] = WholesaleSystem.onInputNumber;

                    // Start Wholesale Info
                    HiddenVIN.Value = startVIN.Text = Util.cleanString(Convert.ToString(dr["VIN"]));
                    startDesc.Text = $"{yearString} {makeString} {modelString}";
                    StartCost.Text = $"{costPrice}";
                    LotLocation.Value = dr["LotLocation"].ToString();

                    // Gather all necessary info
                    Dictionary<string, object> returnItems = Self.startwholesaleBLL.GetListingAuctionData(kSession, kDealer, int.Parse(kListing));

                    //VehicleGrade.Text = GradeSet(dt.Rows[0]);
                    if (((List<Dictionary<string, string>>)returnItems["AvailableAuctions"]).Count != 0)
                    {
                        RelistCount.Value = ((Dictionary<string, string>)returnItems["DefaultAuctionSettings"])["RelistCount"].ToString();
                        if (((Dictionary<string, string>)returnItems["DefaultAuctionSettings"])["UseInventoryPrice"].ToString() == "1"
                            && ((Dictionary<string, string>)returnItems["DefaultAuctionSettings"])["ForceWholesalePricing"].ToString() == "1")
                        {
                            lblChkForce.Style["display"] = chkForceWholesalePrice.Style["display"] = "block";
                            chkForceWholesalePrice.Checked = true;
                            setStart.Disabled = setReserve.Disabled = setBIN.Disabled = true;
                            setStart.Style["opacity"] = setReserve.Style["opacity"] = setBIN.Style["opacity"] = "10%";
                            StartPrice.Attributes["disabled"] = "disabled";
                            BINPrice.Attributes["disabled"] = "disabled";
                            ReservePrice.Attributes["disabled"] = "disabled";
                        }

                        // Populate available auctions a dealer can list
                        string OnChange = "onchange=\"ChangeSelectedAuctions();\"";
                        string startAuctionString = "<div>";
                        int count = 0;
                        foreach (Dictionary<string, string> auction in (List<Dictionary<string, string>>)returnItems["AvailableAuctions"])
                        {
                            string canList = auction["ManualSuppress"] == "1" ? "disabled isListed='true'" : "checked";
                            string auctionName = auction["WholesaleAuctionName"];
                            if (auctionName == "ADESA")
                                auctionName = "OpenLane";
                            else if (auctionName.Contains("OVE"))
                                auctionName = "OVE";

                            if (auctionName == "CarOffer")
                                continue;

                            DataRow[] drList = new DataRow[0];
                            if (((DataTable)returnItems["ListingData"]).Rows.Count != 0)
                                drList = ((DataTable)returnItems["ListingData"]).Select($"AuctionName LIKE '%{auctionName}%'");

                            if (canList.Equals("checked") && drList.Length > 0)
                                canList = drList[0]["Status"].ToString() != "0" ? "disabled isListed='true'" : "checked";

                            if (auction["WholesaleAuctionName"] == "OVE")
                            {
                                int isDealerAccount = int.Parse(Self.startwholesaleBLL.factory.GetAuctionService("OVE")
                                                                    .GetAuctionInfo(kSession, kDealer).Rows[0]["IsDealerAccount"].ToString(), 0);
                                OVEIsDealerAccount.Value = isDealerAccount == 1 ? "True" : "False";
                                credDisclaimer2.Style["display"] = "block";
                            }

                            if (auction["WholesaleAuctionName"] == "ADESA")
                            {
                                int isDealerAccount = int.Parse(Self.startwholesaleBLL.factory.GetAuctionService("ADESA")
                                                                    .GetAuctionInfo(kSession, kDealer).Rows[0]["IsDealerAccount"].ToString(), 0);
                                OpenLaneIsDealerAccount.Value = isDealerAccount == 1 ? "True" : "False";
                                credDisclaimer3.Style["display"] = "block";
                                auctionName = "ADESA";
                            }

                            // Populate vehicle additional info
                            if (drList.Length > 0)
                            {
                                if (!string.IsNullOrEmpty(drList[0]["ContactPerson"].ToString()))
                                    contactName.Value = drList[0]["ContactPerson"].ToString();
                                if (!string.IsNullOrEmpty(drList[0]["VehicleLocationAddress"].ToString()))
                                    addressStreet.Value = drList[0]["VehicleLocationAddress"].ToString();
                                if (!string.IsNullOrEmpty(drList[0]["VehicleLocationCity"].ToString()))
                                    addressCity.Value = drList[0]["VehicleLocationCity"].ToString();
                                if (!string.IsNullOrEmpty(drList[0]["VehicleLocationZIP"].ToString()))
                                    addressZip.Value = drList[0]["VehicleLocationZIP"].ToString();
                                if (!string.IsNullOrEmpty(drList[0]["VehicleLocationPhone"].ToString()))
                                    contactPhone.Value = drList[0]["VehicleLocationPhone"].ToString();

                                string VehicleLocationState = drList[0]["VehicleLocationState"].ToString();
                                WholesaleSystem.BuildStateDropdown("lstState", !string.IsNullOrEmpty(VehicleLocationState) ? VehicleLocationState : "");
                            }

                            startAuctionString += $"<div class='ColRowSwap'>";
                            startAuctionString += $"&nbsp;<input id='{auctionName.Replace(" ", "")}CheckStart' type='checkbox' value='{auction["kWholesaleAuction"]}' class='SingleIndent' style='font-weight:bold;' {canList} {OnChange}/>&nbsp;";
                            startAuctionString += $"<label for='{auctionName.Replace(" ", "")}CheckStart' style='font-weight:bold'>{(auctionName == "RemarketingPlus" ? "Remarketing+" : auctionName)}</label>";
                            startAuctionString += $"<span style='display:none;' id='MaxMMRPct_{auctionName.Replace(" ", "")}'>{auction["MaxMMRPct"]}</span>";
                            startAuctionString += $"</div>";

                            if (count > 5)
                            {
                                startAuctionString += "</div>";
                                startAuctionString += "<div style='display:table-row;'>";
                                count = 0;
                            }

                            count++;
                        }

                        startAuctionList.InnerHtml = startAuctionString + "</div>";
                    }

                    // Default Values: "Standard"
                    string defaultListingCategory = "0";
                    if (((DataTable)returnItems["ListingData"]).Rows.Count != 0)
                    {
                        int count = 0;
                        // We loop through the previously listed entries to determine lowest Listing Category (e.g. 7 (As-Is) beats 8 (Standard))
                        DataRowCollection listingData = ((DataTable)returnItems["ListingData"]).Rows;
                        foreach (DataRow row in listingData)
                        {
                            if (count == 0)
                                defaultListingCategory = row["DefaultListingCategory"].ToString();
                            else if (defaultListingCategory != row["DefaultListingCategory"].ToString())
                            {
                                defaultListingCategory = "0";
                                break;
                            }
                            count++;
                        }
                    }

                    WholesaleSystem.GetDefaultListingCatergories(control: "lstListingCategory", defaultVal: defaultListingCategory != "0" ? defaultListingCategory : "0");
                    WholesaleSystem.VehicleLocationsGet("lstLocation", ((Dictionary<string, string>)returnItems["DefaultAuctionSettings"])["VehicleLocation"].ToString());
                }
            }
        }

        [WebMethod(Description = "Submit a single vehicle to an auction or multiple auctions")]
        public static string SubmitToListMultiAuction(string lstAuctions, string additionalInfo)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            Dictionary<string, object> returnInfo = new Dictionary<string, object>
            {
                { "success", 0 },
                { "errormsgs", "" }
            };

            if (String.IsNullOrEmpty(Convert.ToString(Session["kSession"])))
                WholesaleUser.ClearUser();

            if (String.IsNullOrEmpty(Convert.ToString(Session["kDealer"])))
            {
                returnInfo["errormsgs"] = "Selected dealer is required";
                return Util.serializer.Serialize(returnInfo);
            }

            Self.startwholesaleBLL.SubmitToListMultiAuction(
                                        (object[])Util.serializer.DeserializeObject(lstAuctions),
                                        (Dictionary<string, object>)Util.serializer.DeserializeObject(additionalInfo),
                                        ref returnInfo);

            return Util.serializer.Serialize(returnInfo);
        }
    }
}