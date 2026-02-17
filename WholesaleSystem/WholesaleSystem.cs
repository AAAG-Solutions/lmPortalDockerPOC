using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


using LMWholesale.resource.clients.interfaces;

using Soss.Client;

namespace LMWholesale
{
    /// <summary>
    ///     This class should contain any and all GLOBAL convenience functionality such that we are always DRY
    ///     Don't Repeat Yourself
    /// </summary>
    public class WholesaleSystem
    {
        public static readonly string onInputString = "this.value = this.value.replace(/[^A-Za-z0-9- ]/g, '');";
        public static readonly string onInputNumber = "this.value = Number(this.value.replace(/[^0-9. ]/g, '').replace(/(\\..*)\\./g, '$1')).toLocaleString('en-US');";
        public static readonly WholesaleData.Logger Logger = new WholesaleData.Logger($@"{WebConfigurationManager.AppSettings["LogDirectory"]}\\logs\\", bool.Parse(WebConfigurationManager.AppSettings["DebugMode"]));

        public static void SortDropDownList(ref DropDownList ddList)
        {
            int i;
            int count = ddList.Items.Count;
            while (count > 1)
            {
                for (i = 0; i <= count - 2; i++)
                {
                    if (string.Compare(ddList.Items[i].Text, ddList.Items[i + 1].Text) > 0)
                        SwapDropDownListItems(ref ddList, i, i + 1);
                }
                count--;
            }
        }

        public static void SortListItems(HtmlSelect lc)
        {
            List<ListItem> t = new List<ListItem>();
            object compare = null;
            compare = new Comparison<ListItem>(CompareListItemsByText);
            foreach (ListItem lbItem in lc.Items)
                t.Add(lbItem);
            t.Sort((Comparison<ListItem>)compare);
            lc.Items.Clear();
            lc.Items.AddRange(t.ToArray());
        }

        private static int CompareListItemsByText(ListItem li1, ListItem li2)
        {
            return string.Compare(li1.Text, li2.Text);
        }

        private static int CompareListItemsByValue(ListItem li1, ListItem li2)
        {
            return string.Compare(li1.Value, li2.Value);
        }

        public static void SwapDropDownListItems(ref DropDownList ddList, int index1, int index2)
        {
            ListItem TempLI = new ListItem();
            TempLI = ddList.Items[index2];
            ddList.Items.RemoveAt(index2);
            ddList.Items.Insert(index1, TempLI);
        }


        public static void PopulateList(string item, string header, string sender, char delimiter, string defaultVal = "")
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            string[] strTmp;
            string listVal = "";
            int pos;

            if (!(item.StartsWith("[]")))
            {
                pos = item.IndexOf("]");
                if ((pos != -1))
                {
                    defaultVal = item.Substring(1, pos - 1);
                    listVal = item.Substring(pos + 1);
                }
                else
                {
                    // no brackets
                    defaultVal = "";
                    listVal = item;
                }
            }
            else if (item.StartsWith("[]") && item != "[]")
            {
                pos = item.IndexOf("]");
                if ((pos != -1))
                {
                    listVal = item.Substring(pos + 1);
                }
            }
            else
            {
                if (sender == "lstBidIncrement")
                {
                    listVal = "1|";
                }
                else
                {
                    listVal = "";
                    defaultVal = "";
                }
            }

            if (!String.IsNullOrEmpty(header))
                list.Add(header, "0");

            if (listVal.Length > 0 && listVal != "[]")
            {
                if (listVal.Contains('|'))
                {
                    strTmp = listVal.TrimEnd(delimiter).Split(delimiter);
                    if (strTmp.Length > 0)
                    {
                        foreach (var strElem in strTmp)
                        {
                            pos = strElem.IndexOf(":");
                            if (pos != -1)
                            {
                                string sID;
                                string sText;
                                sID = strElem.Substring(0, pos);
                                sText = strElem.Substring(pos + 1);

                                if (sText == defaultVal)
                                {
                                    defaultVal = sID;
                                }

                                if (!list.ContainsKey(sText))
                                {
                                    list.Add(sText, sID);
                                }
                            }
                            else
                            {
                                list.Add(strElem, strElem);
                            }
                        }
                    }
                }
            }
            else
            {
                list.Add("", "");
            }

            BindStringData(list, defaultVal, sender);
        }

        public static string PopulateList(string item, string header, char delimiter, DropDownList list, Label label = null, HiddenField hideValue = null, string defaultVal = "")
        {
            string returnVal = "";
            string listVal;
            string[] strTmp;
            int pos;

            if (item.Length > 0)
            {
                list.Visible = true;
                list.Items.Clear();

                if (!(item.StartsWith("[]")))
                {
                    pos = item.IndexOf("]");
                    if (pos != -1)
                    {
                        defaultVal = item.Substring(1, pos - 1);
                        listVal = item.Substring(pos + 1);
                    }
                    else // no brackets
                    {
                        defaultVal = "";
                        listVal = item;
                    }
                }
                else
                    listVal = item.Substring(2);

                strTmp = listVal.TrimEnd(delimiter).Split(delimiter);
                if (!String.IsNullOrEmpty(header))
                    list.Items.Add(new ListItem(header, "0"));

                if (strTmp.Length > 1)
                {
                    if (label != null)
                        label.Visible = false;

                    foreach (var strElem in strTmp)
                    {
                        pos = strElem.IndexOf(":");
                        if (pos != -1)
                        {
                            string sID;
                            string sText;
                            sID = strElem.Substring(0, pos);
                            sText = strElem.Substring(pos + 1);

                            list.Items.Add(new ListItem(sText, sID));
                        }
                        else
                            list.Items.Add(new ListItem(strElem));
                    }

                    if (!string.IsNullOrEmpty(defaultVal))
                    {
                        ListItem li = list.Items.FindByText(defaultVal);
                        if (li != null)
                            li.Selected = true;
                        else
                            returnVal = defaultVal;
                    }
                }
                else
                {
                    if (label != null)
                    {
                        label.Visible = true;
                        if (listVal.IndexOf(":") != -1)
                        {
                            strTmp = listVal.TrimEnd('|').Split(':');
                            label.Text = strTmp[1];
                            if (hideValue != null)
                                hideValue.Value = strTmp[0];
                            list.Visible = false;
                        }
                        else
                        {
                            label.Text = listVal.TrimEnd('|').Trim();
                            if (defaultVal != label.Text)
                                returnVal = defaultVal;
                            list.Visible = false;
                        }
                    }
                    else
                    {
                        if (listVal.IndexOf(":") != -1)
                        {
                            strTmp = listVal.TrimEnd('|').Split(':');
                            list.Items.Add(new ListItem(strTmp[1], strTmp[0]));
                            list.Enabled = true;

                            if (defaultVal.Length > 0)
                            {
                                ListItem li = list.Items.FindByText(defaultVal);
                                if (li != null)
                                    li.Selected = true;
                                else
                                    returnVal = defaultVal;
                            }
                        }
                        else
                            list.Items.Add(new ListItem(listVal.TrimEnd('|').Trim()));
                    }
                }
            }
            else
            {
                if (label != null)
                {
                    label.Text = "";
                    label.Visible = true;
                }
                list.Visible = false;
            }

            return returnVal;
        }

        public static Dictionary<string, string> BuildList(string item, string header, char delimiter, string defaultVal = "")
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            string[] strTmp;
            string listVal = "";
            int pos;

            if (!(item.StartsWith("[]")))
            {
                pos = item.IndexOf("]");
                if ((pos != -1))
                {
                    defaultVal = item.Substring(1, pos - 1);
                    listVal = item.Substring(pos + 1);
                }
                else
                {
                    // no brackets
                    defaultVal = "";
                    listVal = item;
                }
            }
            else if (item.StartsWith("[]"))
            {
                pos = item.IndexOf("]");
                if ((pos != -1))
                {
                    listVal = item.Substring(pos + 1);
                }
            }
            else
            {
                listVal = "";
                defaultVal = "";
            }

            if (!String.IsNullOrEmpty(header))
                list.Add(header, "0");

            if (listVal.Length > 0 && listVal != "[]")
            {
                if (listVal.Contains('|'))
                {
                    strTmp = listVal.TrimEnd(delimiter).Split(delimiter);
                    if (strTmp.Length > 0)
                    {
                        foreach (var strElem in strTmp)
                        {
                            pos = strElem.IndexOf(":");
                            if (pos != -1)
                            {
                                string sID;
                                string sText;
                                sID = strElem.Substring(0, pos);
                                sText = strElem.Substring(pos + 1);

                                if (sText == defaultVal)
                                    defaultVal = sID;

                                if (!list.ContainsKey(sText))
                                    list.Add(sText, sID);
                            }
                            else
                                list.Add(strElem, strElem);
                        }
                    }
                }
            }
            else
                list.Add("", "");

            list.Add("Selected", defaultVal);
            return list;
        }

        public static void GetDefaultListingTypes(string control, string selectedValue = "0", bool preferences = false)
        {
            if (selectedValue == "")
                selectedValue = "0";

            Dictionary<string, string> ListingTypes;
            if (preferences)
            {
                ListingTypes = new Dictionary<string, string>
                {
                    { "-- Select a Listing Type --", "0" },
                    { "Bid", "1" },
                    { "Buy", "2" },
                    { "Bid/Buy", "3" },
                    { "Bid/Offer", "4" },
                    { "Bid/Buy/Offer", "5" },
                    { "Bid/Buy/Offer (OVE Bid/Buy)", "200" },
                    { "Bid/Buy/Offer (OVE Buy/Offer)", "100" },
                    { "Buy/Offer", "6" },
                    { "Offer Only", "7" }
                };
            }
            else
            {
                ListingTypes = new Dictionary<string, string>
                {
                    { "-- Select a Listing Type --", "0" },
                    { "Bid", "1" },
                    { "Buy", "2" },
                    { "Bid/Buy", "3" },
                    { "Bid/Offer", "4" },
                    { "Bid/Offer (OVE Offer Only)", "110"},
                    { "Bid/Buy/Offer", "5" },
                    { "Bid/Buy/Offer (OVE Bid/Buy)", "200" },
                    { "Bid/Buy/Offer (OVE Buy/Offer)", "100" },
                    { "Buy/Offer", "6" },
                    { "Buy/Offer (OVE Offer Only)", "120" },
                    { "Offer Only", "7" }
                };
            }

            if (selectedValue != "0" || selectedValue != "")
                selectedValue = ListingTypes.FirstOrDefault(type => type.Key == selectedValue).Value;

            BindStringData(ListingTypes, selectedValue, control);
        }

        public static void GetDefaultListingCatergories(string control, string header = "-- Select a Listing Category --", string defaultVal = "0")
        {
            Dictionary<string, string> ListingTypes = new Dictionary<string, string>
            {
                { header, "0" },
                { "OEM-CPO",  "2" },
                { "Front Line Ready", "4" },
                { "As Described", "5" },
                //{ "Limited Arbitration", "6" },
                { "As-Is", "7" },
                { "Standard", "8" }
            };

            BindStringData(ListingTypes, defaultVal, control);
        }

        /// <summary>
        /// Uses a static list of States to bind to a DropdownList
        /// </summary>
        /// <returns>Returns an empty string when successful</returns>
        public static void BuildStateDropdown(string control, string defaultValue = "0") {

            Dictionary<string, string> stateList = new Dictionary<string, string>
            {
                ["--Select a State--"] = "0",
                ["Alabama"] = "AL",
                ["Alaska"] = "AK",
                ["Arkansas"] = "AR",
                ["Arizona"] = "AZ",
                ["California"] = "CA",
                ["Colorado"] = "CO",
                ["Connecticut"] = "CT",
                ["Delaware"] = "DE",
                ["Florida"] = "FL",
                ["Georgia"] = "GA",
                ["Hawaii"] = "HI",
                ["Iowa"] = "IA",
                ["Idaho"] = "ID",
                ["Illinois"] = "IL",
                ["Indiana"] = "IN",
                ["Kansas"] = "KS",
                ["Kentucky"] = "KY",
                ["Lousiana"] = "LA",
                ["Massachusetts"] = "MA",
                ["Maryland"] = "MD",
                ["Maine"] = "ME",
                ["Michigan"] = "MI",
                ["Minnesota"] = "MN",
                ["Missouri"] = "MO",
                ["Mississippi"] = "MS",
                ["Montana"] = "MT",
                ["North Carolina"] = "NC",
                ["North Dakota"] = "ND",
                ["Nebraska"] = "NE",
                ["New Hampshire"] = "NH",
                ["New Jersey"] = "NJ",
                ["New Mexico"] = "NM",
                ["Nevada"] = "NV",
                ["New York"] = "NY",
                ["Ohio"] = "OH",
                ["Oklahoma"] = "OK",
                ["Oregon"] = "OR",
                ["Pennsylvania"] = "PA",
                ["Puerto Rico"] = "PR",
                ["Rhode Island"] = "RI",
                ["South Carolina"] = "SC",
                ["South Dakota"] = "SD",
                ["Tennessee"] = "TN",
                ["Texas"] = "TX",
                ["Utah"] = "UT",
                ["Virginia"] = "VA",
                ["Virgin Islands"] = "VI",
                ["Vermont"] = "VT",
                ["Washington"] = "WA",
                ["Washington, D.C."] = "DC",
                ["Wisconsin"] = "WI",
                ["West Virginia"] = "WV",
                ["Wyoming"] = "WY",
            };

            BindStringData(stateList, defaultValue, control);
        }

        public static void BuildPriceTypeDropdown(string control, string defaultValue = "0")
        {
            Dictionary<string, string> lstPriceType = new Dictionary<string, string>
            {
                { "None", "0" },
                { "MMR Pricing", "24" },
                { "Wholesale Start", "1" },
                { "Wholesale Floor (Reserve)", "2" },
                { "Wholesale Buy Now", "3" },
                { "Inventory Cost", "4" },
                { "Inventory List Price", "5" },
                { "Inventory Internet Price", "6" },
                { "BlackBook Wholesale Avg.", "7" },
                { "BlackBook Wholesale Clean", "8" },
                { "Classified Price", "9" },
                { "Custom Price 1", "11" },
                { "Custom Price 2", "12" },
                { "Custom Price 3", "13" },
                { "Custom Price 4", "14" },
                { "Custom Price 5", "15" },
                { "Custom Price 6", "16" },
                { "Custom Price 7", "17" },
                { "Custom Price 8", "18" },
                { "Custom Price 9", "19" },
                { "Custom Price 10", "20" }
            };

            BindStringData(lstPriceType, defaultValue, control);
        }

        public static void VehicleLocationsGet(string control, string defaultVal = "1")
        {
            Dictionary<string, string> Locations = new Dictionary<string, string>
            {
                ["At Dealership"] = "1",
                ["At Auction"] = "2",
                ["In Transit"] = "3",
                ["At Distribution Center"] = "4",
                ["Unspecified"] = "5"
            };

            BindStringData(Locations, defaultVal, control);
        }

        public static void AutoGradeScaleGet(string control, string selectedValue = "-1")
        {
            Dictionary<string, string> Grades = new Dictionary<string, string>
            {
                ["Not Set"] = "-1",
                ["5.0"] = "5.0",
                ["4.9"] = "4.9",
                ["4.8"] = "4.8",
                ["4.7"] = "4.7",
                ["4.6"] = "4.6",
                ["4.5"] = "4.5",
                ["4.4"] = "4.4",
                ["4.3"] = "4.3",
                ["4.2"] = "4.2",
                ["4.1"] = "4.1",
                ["4.0"] = "4.0",
                ["3.9"] = "3.9",
                ["3.8"] = "3.8",
                ["3.7"] = "3.7",
                ["3.6"] = "3.6",
                ["3.5"] = "3.5",
                ["3.4"] = "3.4",
                ["3.3"] = "3.3",
                ["3.2"] = "3.2",
                ["3.1"] = "3.1",
                ["3.0"] = "3.0",
                ["2.9"] = "2.9",
                ["2.8"] = "2.8",
                ["2.7"] = "2.7",
                ["2.6"] = "2.6",
                ["2.5"] = "2.5",
                ["2.4"] = "2.4",
                ["2.3"] = "2.3",
                ["2.2"] = "2.2",
                ["2.1"] = "2.1",
                ["2.0"] = "2.0",
                ["1.9"] = "1.9",
                ["1.8"] = "1.8",
                ["1.7"] = "1.7",
                ["1.6"] = "1.6",
                ["1.5"] = "1.5",
                ["1.4"] = "1.4",
                ["1.3"] = "1.3",
                ["1.2"] = "1.2",
                ["1.1"] = "1.1",
                ["1.0"] = "1.0",
                ["0.9"] = "0.9",
                ["0.8"] = "0.8",
                ["0.7"] = "0.7",
                ["0.6"] = "0.6",
                ["0.5"] = "0.5",
                ["0.4"] = "0.4",
                ["0.3"] = "0.3",
                ["0.2"] = "0.2",
                ["0.1"] = "0.1",
                ["0.0"] = "0.0"
            };

            BindStringData(Grades, selectedValue, control);
        }

        public static void GetBidIncrements(string control, string header = "-- Select a Bid Increment --", string defaultVal = "0")
        {
            Dictionary<string, string> lstBidIncrement = new Dictionary<string, string>
            {
                { header, "0" },
                { "1", "1" },
                { "25", "2" },
                { "50", "3" },
                { "100", "4" },
                { "200", "5" },
                { "250", "6" },
                { "500", "7" },
                { "1000", "8" },
                { "1500", "9" },
                { "2000", "10" },
                { "2500", "11" },
                { "4000", "12" },
                { "5000", "13" },
                { "10000", "14" }
            };

            BindStringData(lstBidIncrement, defaultVal, control);
        }

        public static void GetTitleStatuses(string control, string header = "Any Title Status", string defaultVal = "0")
        {
            Dictionary<string, string> lstTitles = new Dictionary<string, string>
            {
                { header, "0"},
                { "Branded", "1" },
                { "No Title", "2" },
                { "MSO", "3" },
                { "Title Present", "4" },
                { "Title Absent", "5" },
                { "Repo Affidavit", "6" },
                { "Salvage", "7" }
            };

            BindStringData(lstTitles, defaultVal, control);
        }

        public static void GetVehicleTypes(string control, string header = "Any Vehicle Type", string defaultVal = "0")
        {
            Dictionary<string, string> lstVehicleTypes = new Dictionary<string, string>
            {
                { header, "0" },
                { "Balloon", "8" },
                { "Company Car", "5" },
                { "Corporate Fleet", "10" },
                { "Dealer-Owned", "2" },
                { "Dealer-Owned (MCO)", "7" },
                { "Dealer-Owned Commercial", "9" },
                { "Employee", "14" },
                { "Fleet", "1" },
                { "Lease", "4" },
                { "Manufacturer BuyBack", "19" },
                { "Pre-Term Purchase", "16" },
                { "Promotional", "13" },
                { "Rental", "6" },
                { "Repo", "3" },
                { "Theft Recovery", "18" },
                { "Trade In", "11" },
                { "Other", "17" }
            };

            BindStringData(lstVehicleTypes, defaultVal, control);
        }

        /// <summary>
        /// Gathers a list of enabled auctions for a dealer
        /// </summary>
        /// <param name="kSession"></param>
        /// <param name="kDealer"></param>
        /// <returns>List of enabled auctions for a given dealer</returns>
        public static List<Dictionary<string, string>> GetAvailableAuctions(string kSession, int kDealer, IWholesaleClient wholesaleClient, int enabled = 0)
        {
            ClearCachedObject("availableAuctions" + enabled);
            object cachedAuctions = GetCachedObject("availableAuctions" + enabled);
            if (cachedAuctions == null)
            {
                List<Dictionary<string, string>> auctionList = new List<Dictionary<string, string>>();
                Wholesale.lmReturnValue returnValue = wholesaleClient.WholesaleAuctionListDealerGet(kSession, kDealer);

                if (returnValue.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    DataTable auctions = returnValue.Data.Tables[0];

                    if (enabled == 1)
                    {
                        foreach (DataRow row in auctions.Rows)
                        {
                            if ((row["WholesaleAuctionName"].ToString()).Contains("OVE"))
                                row["WholesaleAuctionName"] = "OVE";
                            else if (row["WholesaleAuctionName"].ToString().Equals("OpenLane"))
                                row["WholesaleAuctionName"] = "ADESA";
                            else if (row["WholesaleAuctionName"].ToString().Equals("Integrated Auction Solutions"))
                                row["WholesaleAuctionName"] = "IAS";

                            if (row["WholesaleAuctionName"].ToString() == "eDealer Direct")
                                continue;

                            if (int.Parse(row["Enabled"].ToString()) == 1)
                            {
                                auctionList.Add(new Dictionary<string, string>
                                {
                                    { "kWholesaleAuction", row["kWholesaleAuction"].ToString() },
                                    { "WholesaleAuctionName", row["WholesaleAuctionName"].ToString() },
                                    { "ManualSuppress", row["IsManualSuppress"].ToString() },
                                    { "MaxMMRPct", row["MaxMMRPct"].ToString() }
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow row in auctions.Rows)
                        {
                            if ((row["WholesaleAuctionName"].ToString()).Contains("OVE"))
                                row["WholesaleAuctionName"] = "OVE";
                            else if (row["WholesaleAuctionName"].ToString() == "OpenLane")
                                row["WholesaleAuctionName"] = "ADESA";
                            else if (row["WholesaleAuctionName"].ToString().Equals("Integrated Auction Solutions"))
                                row["WholesaleAuctionName"] = "IAS";

                            if (row["WholesaleAuctionName"].ToString() == "eDealer Direct")
                                continue;

                            auctionList.Add(new Dictionary<string, string>
                            {
                                { "kWholesaleAuction", row["kWholesaleAuction"].ToString() },
                                { "WholesaleAuctionName", row["WholesaleAuctionName"].ToString() },
                                { "ManualSuppress", row["IsManualSuppress"].ToString() },
                                { "MaxMMRPct", row["MaxMMRPct"].ToString() }
                            });
                        }
                    }
                }

                SetCachedObject("availableAuctions" + enabled, auctionList);
                return auctionList;
            }
            else
                return (List<Dictionary<string, string>>)cachedAuctions;
        }

        /// <summary>
        /// Accepts a List of Dictionaries of Strings and finds a control on the page to bind data to a DropdownList
        /// </summary>
        /// <param name="list"></param>
        /// <param name="defVal"></param>
        /// <param name="sender"></param>
        /// <returns>Returns an empty string when successful</returns>
        private static void BindStringData(Dictionary<string, string> list, string defVal, string sender)
        {
            // Bind data to DropdownList
            Page page = (Page)HttpContext.Current.Handler;
            ContentPlaceHolder mainContent = (ContentPlaceHolder)page.Master.FindControl("MainContent");
            DropDownList myControl = (DropDownList)mainContent.FindControl(sender);

            if (list != null && !string.IsNullOrEmpty(defVal))
            {
                myControl.DataSource = list;
                myControl.DataTextField = "Key";
                myControl.DataValueField = "Value";
                myControl.SelectedValue = defVal;
                myControl.DataBind();
            }
            else if (list != null)
            {
                myControl.DataSource = list;
                myControl.DataTextField = "Key";
                myControl.DataValueField = "Value";
                myControl.DataBind();
            }
        }

        public static string GetPricingType(string kValue)
        {
            Dictionary<string, string> lstPriceType = new Dictionary<string, string>
            {
                { "0", "None" },
                { "24", "MMR Pricing" },
                { "1", "Wholesale Start" },
                { "2", "Wholesale Floor (Reserve)" },
                { "3", "Wholesale Buy Now" },
                { "4", "Inventory Cost" },
                { "5", "Inventory List Price" },
                { "6", "Inventory Internet Price" },
                { "7", "BlackBook Wholesale Avg." },
                { "8", "BlackBook Wholesale Clean" },
                { "9", "Classified Price" },
                { "11", "Custom Price 1" },
                { "12", "Custom Price 2" },
                { "13", "Custom Price 3" },
                { "14", "Custom Price 4" },
                { "15", "Custom Price 5" },
                { "16", "Custom Price 6" },
                { "17", "Custom Price 7" },
                { "18", "Custom Price 8" },
                { "19", "Custom Price 9" },
                { "20", "Custom Price 10" }
            };

            return lstPriceType[kValue];
        }

        public static string GetPhysicalLocation(string kValue)
        {
            Dictionary<string, string> Locations = new Dictionary<string, string>
            {
                { "1", "At Dealership" },
                { "2", "At Auction" },
                { "3",  "In Transit" },
                { "4",  "At Distribution Center" },
                { "5",  "Unspecified" }
            };

            return Locations[kValue];
        }

        public static string GetListingCategory(string kValue)
        {
            Dictionary<string, string> ListingTypes = new Dictionary<string, string>
            {
                { "2", "OEM-CPO" },
                { "4", "Front Line Ready" },
                { "5", "As Described" },
                //{ "6", "Limited Arbitration" },
                { "7", "As-Is" },
                { "8", "Standard" }
            };

            return ListingTypes[kValue];
        }

        public static string GetListingType(string kValue)
        {
            Dictionary<string, string> options = new Dictionary<string, string>
            {
                { "1", "Bid" },
                { "2", "Buy" },
                { "3", "Bid/Buy" },
                { "4", "Bid/Offer" },
                { "5", "Bid/Buy/Offer" },
                { "6", "Buy/Offer" },
                { "7", "Offer Only" }
            };
            return options[kValue];
        }

        public static string GetVehicleType(string kValue)
        {
            Dictionary<string, string> lstVehicleTypes = new Dictionary<string, string>
            {
                { "0", "Any" },
                { "1", "Fleet" },
                { "2", "Dealer-Owned" },
                { "3", "Repo" },
                { "4", "Lease" },
                { "5", "Company Car" },
                { "6", "Rental" },
                { "7", "Dealer-Owned (MCO)" },
                { "8", "Balloon" },
                { "9", "Dealer-Owned Commercial" },
                { "10", "Corporate Fleet" },
                { "11", "Trade In" },
                { "13", "Promotional" },
                { "14", "Employee" },
                { "16", "Pre-Term Purchase" },
                { "17", "Other" },
                { "18", "Theft Recovery" },
                { "19", "Manufacturer BuyBack" }
            };

            return lstVehicleTypes[kValue];
        }

        /// <summary>
        /// This public class is to provde the ability to asynchronously load lmPage info if there aren't
        /// dependent downstream processes
        /// </summary>
        public class AsyncTask
        {
            private string _taskProgress;
            private AsyncTaskDelegate _delegate;
            protected delegate void AsyncTaskDelegate();

            public string GetAsyncTaskProgress()
            {
                return _taskProgress;
            }
            public virtual void ExecuteAsyncTask()
            {
                // Override Me
            }

            public IAsyncResult OnBegin(object sender, EventArgs e, AsyncCallback callback, object extraData)
            {
                _taskProgress = $"AsyncTask stated at: {DateTime.Now}";
                _delegate = new AsyncTaskDelegate(ExecuteAsyncTask);
                IAsyncResult result = _delegate.BeginInvoke(callback, extraData);

                return result;
            }

            public void OnEnd(IAsyncResult asyncResult)
            {
                _taskProgress = $"AsynTask completed at: {DateTime.Now}";
                _delegate.EndInvoke(asyncResult);
            }

            public void OnTimeout(IAsyncResult asyncResult)
            {
                _taskProgress = "AsyncTask failed to complete due to AsyncTimeout parameter";
            }

            protected void BindtoDropDown(string item, string header, string sender, char delimiter, ContentPlaceHolder mainContent, string defaultVal = "")
            {
                Dictionary<string, string> list = new Dictionary<string, string>();

                string[] strTmp;
                string listVal = "";
                int pos;

                if (!(item.StartsWith("[]")))
                {
                    pos = item.IndexOf("]");
                    if ((pos != -1))
                    {
                        defaultVal = item.Substring(1, pos - 1);
                        listVal = item.Substring(pos + 1);
                    }
                    else
                    {
                        // no brackets
                        defaultVal = "";
                        listVal = item;
                    }
                }
                else if (item.StartsWith("[]") && item != "[]")
                {
                    pos = item.IndexOf("]");
                    if ((pos != -1))
                    {
                        listVal = item.Substring(pos + 1);
                    }
                }
                else
                {
                    if (sender == "lstBidIncrement")
                    {
                        listVal = "1|";
                    }
                    else
                    {
                        listVal = "";
                        defaultVal = "";
                    }
                }

                if (!String.IsNullOrEmpty(header))
                    list.Add(header, "0");

                if (listVal.Length > 0 && listVal != "[]")
                {
                    if (listVal.Contains('|'))
                    {
                        strTmp = listVal.TrimEnd(delimiter).Split(delimiter);
                        if (strTmp.Length > 0)
                        {
                            foreach (var strElem in strTmp)
                            {
                                pos = strElem.IndexOf(":");
                                if (pos != -1)
                                {
                                    string sID;
                                    string sText;
                                    sID = strElem.Substring(0, pos);
                                    sText = strElem.Substring(pos + 1);

                                    if (sText == defaultVal)
                                    {
                                        defaultVal = sID;
                                    }

                                    if (!list.ContainsKey(sText))
                                    {
                                        list.Add(sText, sID);
                                    }
                                }
                                else
                                {
                                    list.Add(strElem, strElem);
                                }
                            }
                        }
                    }
                }
                else
                {
                    list.Add("", "");
                }

                DropDownList myControl = (DropDownList)mainContent.FindControl(sender);

                if (list != null && !string.IsNullOrEmpty(defaultVal))
                {
                    myControl.DataSource = list;
                    myControl.DataTextField = "Key";
                    myControl.DataValueField = "Value";
                    myControl.SelectedValue = defaultVal;
                    myControl.DataBind();
                }
                else if (list != null)
                {
                    myControl.DataSource = list;
                    myControl.DataTextField = "Key";
                    myControl.DataValueField = "Value";
                    myControl.DataBind();
                }
            }
        }

        #region CacheFactory Methods
        public static object GetCachedObject(string Name, int CacheType = 1)
        {
            try
            {
                string cacheType = "";
                if (CacheType == 0) // GLOBAL
                    cacheType = "GLOBAL";
                else if (CacheType == 1)
                    cacheType = HttpContext.Current.Session["kPerson"].ToString();

                object obj = (CacheObject)CacheFactory.GetCache(cacheType).Get(Name);
                return obj == null ? null : ((CacheObject)obj).Value;
            }
            catch (Exception ex) { return null; }
        }

        public static void SetCachedObject(string Name, object Value, int ExpireMinutes = 0, int CacheType = 1)
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

        public static void ClearCachedObject(string Name, int CacheType = 1)
        {
            string cacheType = "";
            if (CacheType == 0) // GLOBAL
                cacheType = "GLOBAL";
            else if (CacheType == 1) // kPerson
                cacheType = HttpContext.Current.Session["kPerson"].ToString();

            NamedCache nc = CacheFactory.GetCache(cacheType);
            nc.Remove(Name);
        }

        public static void ClearCachedObjects(int CacheType = 1)
        {
            string cacheType = "";
            if (CacheType == 0) // GLOBAL
                cacheType = "GLOBAL";
            else if (CacheType == 1) // kPerson
                cacheType = HttpContext.Current.Session["kPerson"].ToString();

            NamedCache nc = CacheFactory.GetCache(cacheType);
            IEnumerable<CachedObjectId> ic = nc.Keys.Cast<CachedObjectId>();
            for (int i = 0; i < ic.Count(); i++)
                nc.Remove(ic.ElementAt(i));
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