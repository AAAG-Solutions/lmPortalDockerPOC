using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;
using LMWholesale.resource.factory;

namespace LMWholesale.BLL.WholesaleContent.Preferences
{
    public class BlackoutWindowRules
    {
        private readonly WholesaleClient wholesaleClient;

        public BlackoutWindowRules() => wholesaleClient = wholesaleClient ?? new WholesaleClient();

        public BlackoutWindowRules(WholesaleClient wholesaleClient) => this.wholesaleClient = wholesaleClient;

        internal static readonly BlackoutWindowRules instance = new BlackoutWindowRules();
        public BlackoutWindowRules Self
        {
            get { return instance; }
        }

        public int DetermineBestFit(int count)
        {
            List<int> colPossible = new List<int>() { 3, 4, 5 };
            Dictionary<int, int> results = new Dictionary<int, int>() { { 3, 100 }, { 4, 100 }, { 5, 100 } };
            foreach(int num in colPossible)
            {
                results[num] = count % num;
            }
            return results.OrderBy(x => x.Value).ThenByDescending(x => x.Key).First().Key;
        }

        public Dictionary<string, string> BuildDataDictionary(string kWholesaleAuction, Dictionary<string, object> data)
        {
            
            Dictionary<string, string> dataOut = new Dictionary<string, string>();
            dataOut["kWholesaleBlackOutWindow"] = data["kWholesaleBlackOutWindow"] != null ? data["kWholesaleBlackOutWindow"].ToString() : "0";
            dataOut["kWholesaleAuction"] = kWholesaleAuction;
            dataOut["kAASale"] = data["kAASale"] != null ? data["kAASale"].ToString() : "0";
            dataOut["kWholesaleFacilitatedAuctionCode"] = data["kWholesaleFacilitatedAuctionCode"] != null ? data["kWholesaleFacilitatedAuctionCode"].ToString() : "0";
            dataOut["StartDOW"] = data["StartDay"].ToString();
            dataOut["StartTime"] = data["StartTime"].ToString();
            dataOut["EndDOW"] = data["EndDay"].ToString();
            dataOut["EndTime"] = data["EndTime"].ToString();
            dataOut["InvLotLocation"] = data["LotLocation"].ToString();
            dataOut["RemoveAutoLaunchAuction"] = data["RemoveAutoLaunchAuction"].ToString();
            dataOut["RemoveManualAuction"] = data["RemoveManualAuction"].ToString();
            dataOut["Disable"] = data["Disable"].ToString();
            dataOut["Interval"] = data["Interval"].ToString();
            dataOut["IntervalStart"] = data["InitialIntervalDay"].ToString();
            dataOut["kBlackoutWindowIntervalType"] = data["Frequency"].ToString();
            dataOut["IntervalDOW"] = data["IntervalDay"] == null ? "" : data["IntervalDay"].ToString();
            dataOut["MinYear"] = data["MinYear"].ToString();
            dataOut["MaxYear"] = data["MaxYear"].ToString();
            dataOut["Make"] = data["Make"].ToString() == "0" ? "" : data["Make"].ToString();
            dataOut["Suspended"] = data["Suspended"].ToString();

            return dataOut;
        }

        public string BuildListForm(List<Dictionary<string, object>> Data, List<Dictionary<string, string>> Auctions)
        {
            //#TODO: Figure out why the non-concat version was not working and refactor
            string retString = "<div style='display: flex;flex-wrap: wrap;'>";
            int counter = 1;
            foreach (Dictionary<string, object> item in Data)
            {
                string auctionName = Auctions.First(d => d.ContainsValue(item["kWholesaleAuction"].ToString()))["WholesaleAuctionName"];
                int motorYear = Convert.ToInt32(item["MinYear"]);
                int motorYearMax = Convert.ToInt32(item["MaxYear"]);
                string lotLocation = item["InvLotLocation"].ToString() == "[ANY]" ? "Any" : item["InvLotLocation"].ToString();
                if ((counter - 1) % 2 == 0)
                    retString += $"<div style='display: flex; width: 100%;'>";
                retString += $"<div style='border:solid 5px black;margin:2px;padding:10px;font-size:12pt;font-weight:bold;width:50%;'>Rule #{counter}";
                retString += "<br /><br />";
                if (item["Suspended"].ToString() == "1")
                    retString += "<span style='color: red;'>***This rule is suspended***</span><br />";
                retString += $"<span>{auctionName} - Vehicles meeting the following criteria</span><br />";
                if (motorYear != 0)
                    retString += $"<span>&emsp;Motor Year: Between {motorYear} and {motorYearMax}</span><br />";
                else
                    retString += $"<span>&emsp;Motor Year: Any</span><br />";
                if (item["Make"] == null || item["Make"].ToString() == "")
                    retString += $"<span>&emsp;Make: Any</span><br />";
                else
                    retString += $"<span>&emsp;Make: {item["Make"]}</span><br />";
                retString += $"<span>&emsp;Lot Location: {lotLocation}</span><br />";
                retString += "<span>Blackout Window:</span><br />";
                string monthlyWeekly = item["kBlackoutWindowIntervalType"].ToString() == "1" ? "Weekly" : "Monthly";
                retString += $"<span>&emsp;Type: {monthlyWeekly}</span><br />";
                retString += $"<span>&emsp;Start Day: {item["StartDOW"]}</span><br />";
                retString += $"<span>&emsp;Start Time: {item["StartTime"]}</span><br />";
                retString += $"<span>&emsp;End Day: {item["EndDOW"]}</span><br />";
                retString += $"<span>&emsp;End Time: {item["EndTime"]}</span><br />";
                if (monthlyWeekly == "Weekly")
                {
                    retString += $"<span>&emsp;Interval: {GetInterval(monthlyWeekly, item["Interval"].ToString())}</span><br />";
                    retString += $"<span>&emsp;Starting: {item["IntervalStart"]}</span><br />";
                }
                else
                    retString += $"<span>&emsp;Interval: {GetInterval(monthlyWeekly, item["Interval"].ToString(), item["IntervalDOW"].ToString())}</span><br />";
                retString += "<span>Vehicles in auction:</span><br />";
                if (item["RemoveAutoLaunchAuction"].ToString() == "0")
                    retString += $"<span>&emsp;Autolaunch listings will *NOT* be removed from auction</span><br />";
                else
                    retString += $"<span>&emsp;Autolaunch listings will be removed from auction</span><br />";
                if (item["RemoveManualAuction"].ToString() == "0")
                    retString += $"<span>&emsp;Manually launched listings will *NOT* be removed from auction</span><br />";
                else
                    retString += $"<span>&emsp;Manually launched listings will be removed from auction</span><br />";

                retString += "</div>";
                if ((counter - 1) % 2 == 1)
                    retString += $"</div>";
                counter++;

            }

            if ((counter - 2) % 2 != 1)
                retString += $"</div>";

            retString += "</div>";
            return retString;
        }

        private string GetInterval(string MonthlyWeekly, string IntervalVal, string IntervalDOW = null)
        {
            if (MonthlyWeekly == "Weekly")
            {
                Dictionary<string, string> mapping = new Dictionary<string, string>
                {
                    { "1", "Every Week" },
                    { "2", "Every Two Weeks" },
                    { "3", "Every Three Weeks" },
                    { "4", "Every Four Weeks" },
                    { "5", "Every Five Weeks" },
                };

                return mapping[IntervalVal];
            }
            else
            {
                Dictionary<string, string> mapping = new Dictionary<string, string>
                {
                    { "-2", "Next to Last" },
                    { "-1", "Last" },
                    { "1", "First" },
                    { "2", "Second" },
                    { "3", "Third" },
                    { "4", "Fourth" },
                    { "5", "Fifth" },
                };

                return mapping[IntervalVal] + " " + IntervalDOW + " " + " of the Month";
            }
        }
    }
}