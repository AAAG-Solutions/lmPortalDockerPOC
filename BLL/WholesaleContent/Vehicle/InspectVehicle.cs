using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using LMWholesale.BLL.WholesaleData;
using LMWholesale.resource.clients;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class InspectVehicle
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly ListingClient listingClient;
        private readonly DealerClient dealerClient;
        private readonly UploadPhotos uploadPhotos;
        public Dictionary<string, Dictionary<string, List<int>>> mappings = null;
        public Dictionary<string, Dictionary<int, string>> dropDownValues = null;

        public InspectVehicle()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            listingClient = listingClient ?? new ListingClient();
            dealerClient = dealerClient ?? new DealerClient();
            uploadPhotos = uploadPhotos ?? new UploadPhotos();
        }
        public InspectVehicle(WholesaleClient wholesaleClient, ListingClient listingClient, DealerClient dealerClient, UploadPhotos uploadPhotos)
        {
            this.wholesaleClient = wholesaleClient;
            this.listingClient = listingClient;
            this.dealerClient = dealerClient;
            this.uploadPhotos = uploadPhotos;
        }
        internal static readonly InspectVehicle instance = new InspectVehicle();
        public InspectVehicle Self
        {
            get { return instance; }
        }

        public string GetVehicleString(string kSession, int kDealer, int kListing, int thin)
        {
            Listing.lmReturnValue vehicleDetail = Self.listingClient.ListingDetailGet(kSession, kDealer, kListing, 0);
            if (vehicleDetail.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                var dr = vehicleDetail.Data.Tables["VehicleData"].Rows[0];
                return dr["MotorYear"].ToString() + " " + dr["Make"].ToString() + " " + dr["Model"].ToString();
            }
            return "";
        }

        public DataRow GetInspectionData(string kSession, int kDealer, string kListing)
        {
            Wholesale.lmReturnValue inspectionData = Self.wholesaleClient.InspectionDataGet(kSession, kDealer, kListing);
            if (inspectionData.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                return inspectionData.Data.Tables[0].Rows[0];
            }
            return new DataTable("EmptyTable").NewRow();
        }

        public object BuildActionButtons(string Source, int rowNum)
        {
            string vehicleActions = $@"
            <div class='actionsBar'>
                <div style=""display: table; margin: auto; "">
                    <div style=""display: table-row"">
                       <input type=""submit"" name=""{rowNum}$submitButton"" value=""Edit"" onclick=""ButtonAction('edit', '{Source}', '{rowNum}'); return false; "" id=""MainContent_submitButton"" class=""actionBackground"">
                       <input type=""submit"" name=""{rowNum}$submitButton"" value=""Delete"" onclick=""ButtonAction('delete', '{Source}', '{rowNum}'); return false;"" id=""MainContent_submitButton"" class=""actionBackground"">
                    </div>
                </div>
             </div>";

            return vehicleActions;
        }

        public string PrepareFromXML(string element)
        {
            string returnString = "";

            switch (element)
            {
                case "TitleStatus":
                    returnString += "[]0:Select Title Status|";
                    break;
                case "OdometerStatus":
                    returnString += "[]0:Select Odometer Status|";
                    break;
                case "AudioType":
                    returnString += "[]-1:Select Audio Type|";
                    break;
                case "InteriorType":
                    returnString += "[]-1:Select Interior Type|";
                    break;
                case "TireCondition":
                    returnString += "[]:Select Tire Condition|";
                    break;
                case "TireMfg":
                    returnString += "[]0:Select Tire Mfg|";
                    break;
                case "ExteriorDamageCategory":
                case "InteriorDamageCategory":
                    returnString += "[Select Damage Location]0:Select Damage Location|";
                    break;
                case "ExteriorDamageCondition":
                case "InteriorDamageCondition":
                    returnString += "[Select Damage Condition]0:Select Damage Condition|";
                    break;
                case "ExteriorDamageSeverity":
                case "InteriorDamageSeverity":
                    returnString += "[No Damage Severity Specified]0:No Damage Severity Specified|";
                    break;
                case "PriorPaintCategory":
                    returnString += "[Select Prior Paint Location]0:Select Prior Paint Location|";
                    break;
                case "PriorPaintCondition":
                    returnString += "[Select Prior Paint Condition]0:Select Prior Paint Condition|";
                    break;
            }

            foreach (var item in dropDownValues.Where(x => x.Key == element).FirstOrDefault().Value)
            {
                returnString += item.Key + ":" + item.Value + "|";
            }
            return returnString;
        }

        public List<Dictionary<string, object>> FormatData(DataTable Info, string Source)
        {
            List<Dictionary<string, object>> returnList = new List<Dictionary<string, object>>();

            int counter = 0;
            if (Source == "ext" || Source == "int")
            {
                foreach (DataRow row in Info.Rows)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string severity = row["DamageSeverity"].ToString() != "" ? row["DamageSeverity"].ToString() : "No Damage Severity Specified";
                    dict["Actions"] = BuildActionButtons(Source, counter);
                    dict["Damage"] = row["DamageCategory"].ToString();
                    dict["Condition"] = row["DamageCondition"].ToString();
                    dict["Severity"] = severity;
                    dict["Description"] = row["DamageDescription"].ToString();
                    dict["regionText"] = Source.Substring(0, 1).ToUpper() + Source.Substring(1) + "erior";
                    dict["neededAction"] = "None";
                    dict["PreviousInfo"] = row["DamageCategory"].ToString() + "|" + row["DamageCondition"].ToString() + "|" + severity;
                    dict["DamagePhoto"] = string.IsNullOrEmpty(row["DamagePhoto"].ToString()) ? "No Photo" : $"<img style='width: 10vh; height: auto;' src={row["DamagePhoto"].ToString()} />";
                    dict["PhotoInfo"] = string.IsNullOrEmpty(row["DamagePhoto"].ToString()) ? "" : "Existing Photo";
                    dict["PhotoFileName"] = "";
                    dict["PhotoData"] = "";
                    counter++;
                    returnList.Add(dict);
                }
            }
            else if (Source == "paint")
            {
                foreach (DataRow row in Info.Rows)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    dict["Actions"] = BuildActionButtons(Source, counter);
                    dict["Damage"] = row["PaintDescription"].ToString();
                    dict["Condition"] = row["PaintCondition"].ToString();
                    dict["neededAction"] = "None";
                    dict["PreviousInfo"] = row["PaintDescription"].ToString() + "|" + row["PaintCondition"].ToString();
                    counter++;
                    returnList.Add(dict);
                }
            }

            return returnList;
        }

        public string GetGridInfo(string session, int kDealer, string kListing, string gridType)
        {
            EnumerableRowCollection<DataRow> inspectionDataInformation = null;

            Wholesale.lmReturnValue inspectionData = Self.wholesaleClient.InspectionDataGet(session, kDealer, kListing);

            if (gridType != "paint")
                inspectionDataInformation = inspectionData.Data.Tables[1].AsEnumerable().Where(x => x["nIntExtType"].ToString() == (gridType == "ext" ? "0" : "1"));
            else
                inspectionDataInformation = inspectionData.Data.Tables[2].AsEnumerable();

            if (inspectionData.Result == Wholesale.ReturnCode.LM_SUCCESS && inspectionDataInformation.Count() > 0)
            {
                return "0 |" + Util.serializer.Serialize(FormatData(inspectionDataInformation.CopyToDataTable(), gridType));
            }

            // Return default fail value if error occurs
            return "0 | {}";
        }

        public bool SaveDamages(string jsonData, string session, string kListing, string kDealer, string PhotoPath)
        {
            Object[] data = (Object[])Util.serializer.DeserializeObject(jsonData);

            foreach (Object[] damageObj in data)
            {
                foreach (Dictionary<string, object> damage in damageObj.Where(x => ((Dictionary<string, object>)x)["neededAction"].ToString() == "Update" || ((Dictionary<string, object>)x)["neededAction"].ToString() == "Save"))
                {
                    if (damage["PhotoInfo"].ToString() != "Existing Photo" && damage["PhotoInfo"].ToString() != "")
                    {
                        Listing.lmReturnValue success =
                            uploadPhotos.ExplicitAddPhotos(session, Convert.ToInt32(kListing), PhotoPath, damage["PhotoFileName"].ToString(), "_LG|_SM|_TH|_IC", 4);
                    }
                    damage.Add("kPhoto", "0");
                }
            }

            Listing.lmReturnValue photos = listingClient.ListingPhotosGet(session, Convert.ToInt32(kListing));
            if (photos.Result == Listing.ReturnCode.LM_SUCCESS)
            {
                foreach (Object[] damageObj in data)
                {
                    foreach (Dictionary<string, object> damageData in damageObj.Where(x => ((Dictionary<string, object>)x)["PhotoInfo"].ToString() != "Existing Photo" && ((Dictionary<string, object>)x)["PhotoInfo"].ToString() != ""))
                    {
                        foreach (DataRow row in photos.Data.Tables[0].Rows)
                        {
                            if (row["LocationOrig"].ToString() == Path.Combine(PhotoPath, "Originals", damageData["PhotoFileName"].ToString()))
                            {
                                damageData["kPhoto"] = row["kPhoto"].ToString();
                            }
                        }
                    }
                }
            }

            foreach (Object[] damageObj in data)
            {
                foreach (Dictionary<string, object> damage in damageObj.Where(x => ((Dictionary<string, object>)x)["neededAction"].ToString() == "Delete"))
                {
                    damage["Description"] = damage["Description"].ToString().Replace("&apos;", "'").Replace("\"", "u0022");
                    Wholesale.lmReturnValue setVehicleDamages = Self.wholesaleClient.WPWholesaleAuctionDamageSet(session, int.Parse(kDealer), kListing, Util.serializer.Serialize(GetOutObjectDamage(damage)));
                    if (setVehicleDamages.Result != Wholesale.ReturnCode.LM_SUCCESS)
                    {
                        return false;
                    }
                }
                foreach (Dictionary<string, object> damage in damageObj.Where(x => ((Dictionary<string, object>)x)["neededAction"].ToString() == "Update"))
                {
                    damage["Description"] = damage["Description"].ToString().Replace("&apos;", "'").Replace("\"", "u0022");
                    Wholesale.lmReturnValue setVehicleDamages = Self.wholesaleClient.WPWholesaleAuctionDamageSet(session, int.Parse(kDealer), kListing, Util.serializer.Serialize(GetOutObjectDamage(damage)));

                    if (setVehicleDamages.Result != Wholesale.ReturnCode.LM_SUCCESS)
                    {
                        return false;
                    }
                }
                foreach (Dictionary<string, object> damage in damageObj.Where(x => ((Dictionary<string, object>)x)["neededAction"].ToString() == "Save"))
                {
                    damage["Description"] = damage["Description"].ToString().Replace("&apos;", "'").Replace("\"", "u0022");
                    Wholesale.lmReturnValue setVehicleDamages = Self.wholesaleClient.WPWholesaleAuctionDamageSet(session, int.Parse(kDealer), kListing, Util.serializer.Serialize(GetOutObjectDamage(damage)));

                    if (setVehicleDamages.Result != Wholesale.ReturnCode.LM_SUCCESS)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool SavePaintDamages(string jsonData, string session, string kListing, string kDealer)
        {
            Object[] data = (Object[])Util.serializer.DeserializeObject(jsonData);
            foreach (Dictionary<string, object> item in data.Where(x => ((Dictionary<string, object>)x)["neededAction"].ToString() == "Delete"))
            {
                Wholesale.lmReturnValue setVehiclePaint = Self.wholesaleClient.WPWholesaleAuctionPaintSet(session, int.Parse(kDealer), kListing, Util.serializer.Serialize(GetOutObjectPaint(item)));

                if (setVehiclePaint.Result != Wholesale.ReturnCode.LM_SUCCESS)
                {
                    return false;
                }
            }
            foreach (Dictionary<string, object> item in data.Where(x => ((Dictionary<string, object>)x)["neededAction"].ToString() == "Update"))
            {
                Wholesale.lmReturnValue setVehiclePaint = Self.wholesaleClient.WPWholesaleAuctionPaintSet(session, int.Parse(kDealer), kListing, Util.serializer.Serialize(GetOutObjectPaint(item)));

                if (setVehiclePaint.Result != Wholesale.ReturnCode.LM_SUCCESS)
                {
                    return false;
                }
            }
            foreach (Dictionary<string, object> item in data.Where(x => ((Dictionary<string, object>)x)["neededAction"].ToString() == "Save"))
            {
                Wholesale.lmReturnValue setVehiclePaint = Self.wholesaleClient.WPWholesaleAuctionPaintSet(session, int.Parse(kDealer), kListing, Util.serializer.Serialize(GetOutObjectPaint(item)));

                if (setVehiclePaint.Result != Wholesale.ReturnCode.LM_SUCCESS)
                {
                    return false;
                }
            }

            return true;
        }

        private Dictionary<string, object> GetOutObjectDamage(Dictionary<string, object> item2)
        {
            if (item2["PreviousInfo"].ToString() != "")
            {
                return new Dictionary<string, object>()
                    {
                        { "kDamageCategory", getValueByDisplayXML(item2["Damage"].ToString(), item2["regionText"].ToString() + "DamageCategory") },
                        { "kDamageCondition", getValueByDisplayXML(item2["Condition"].ToString(), item2["regionText"].ToString() + "DamageCondition") },
                        { "kDamageSeverity", getValueByDisplayXML(item2["Severity"].ToString(), item2["regionText"].ToString() + "DamageSeverity") },
                        { "DamageDescription", item2["Description"].ToString() },
                        { "RegionText", item2["regionText"].ToString() },
                        { "kPhoto", item2["kPhoto"].ToString() },
                        { "kUserPerson", "0" },
                        { "OperCode", item2["neededAction"].ToString() == "Delete" ? "0" : "1" },
                        { "PreviousDamageCategory", getValueByDisplayXML(item2["PreviousInfo"].ToString().Split('|')[0], item2["regionText"].ToString() + "DamageCategory") },
                        { "PreviousDamageCondition", getValueByDisplayXML(item2["PreviousInfo"].ToString().Split('|')[1], item2["regionText"].ToString() + "DamageCondition")},
                        { "PreviousDamageSeveritry", getValueByDisplayXML(item2["PreviousInfo"].ToString().Split('|')[2], item2["regionText"].ToString() + "DamageSeverity") }
                    };
            }
            else
            {
                return new Dictionary<string, object>()
                    {
                        { "kDamageCategory", getValueByDisplayXML(item2["Damage"].ToString(), item2["regionText"].ToString() + "DamageCategory") },
                        { "kDamageCondition", getValueByDisplayXML(item2["Condition"].ToString(), item2["regionText"].ToString() + "DamageCondition") },
                        { "kDamageSeverity", getValueByDisplayXML(item2["Severity"].ToString(), item2["regionText"].ToString() + "DamageSeverity") },
                        { "DamageDescription", item2["Description"].ToString() },
                        { "RegionText", item2["regionText"].ToString() },
                        { "kPhoto", item2["kPhoto"].ToString() },
                        { "kUserPerson", "0" },
                        { "OperCode", item2["neededAction"].ToString() == "Delete" ? "0" : "1" },
                        { "PreviousDamageCategory", "-1" },
                        { "PreviousDamageCondition", "-1" },
                        { "PreviousDamageSeveritry", "-1" }
                    };
            }
        }

        private Dictionary<string, object> GetOutObjectPaint(Dictionary<string, object> item)
        {
            if (item["PreviousInfo"].ToString() != "")
            {
                return new Dictionary<string, object>()
                    {
                        { "kPaintDescription", getValueByDisplayXML(item["Damage"].ToString(), "PriorPaintCategory") },
                        { "kPaintCondition", getValueByDisplayXML(item["Condition"].ToString(), "PriorPaintCondition") },
                        { "OperCode", item["neededAction"].ToString() == "Delete" ? "0" : "1" },
                        { "PreviousPaintDescription", getValueByDisplayXML(item["PreviousInfo"].ToString().Split('|')[0], "PriorPaintCategory") },
                        { "PreviousPaintCondition", getValueByDisplayXML(item["PreviousInfo"].ToString().Split('|')[1], "PriorPaintCondition") }
                    };
            }
            else
            {
                return new Dictionary<string, object>()
                    {
                        { "kPaintDescription", getValueByDisplayXML(item["Damage"].ToString(), "PriorPaintCategory") },
                        { "kPaintCondition", getValueByDisplayXML(item["Condition"].ToString(), "PriorPaintCondition") },
                        { "OperCode", item["neededAction"].ToString() == "Delete" ? "0" : "1" },
                        { "PreviousPaintDescription", "-1" },
                        { "PreviousPaintCondition", "-1" }
                    };
            }
        }

        public bool SaveInspection(string jsonData, string session, string kListing, int kDealer, string kPerson, string IsInspector)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)Util.serializer.DeserializeObject(jsonData);

            data["kUserPerson"] = kPerson;
            data["ReportedInspectionDate"] = "";
            data["ExternalCRLink"] = "";
            data["CRTypeFlag"] = "";
            data["kWholesaleInspectionCompany"] = "0";
            data["IsInspectionReport"] = IsInspector;

            Wholesale.lmReturnValue setInspectionData = Self.wholesaleClient.InspectionDataSet(Util.serializer.Serialize(data), session, kDealer, kListing);
            
            if (setInspectionData.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                return true;
            }

            return false;
        }

        private string getValueByDisplayXML(string displayString, string element)
        {
            switch (element)
            {
                case "TitleStatus":
                    if (displayString == "Select Title Status")
                        return "0";
                    break;
                case "OdometerStatus":
                    if (displayString == "Select Odometer Status")
                        return "0";
                    break;
                case "AudioType":
                    if (displayString == "Select Audio Type")
                        return "-1";
                    break;
                case "InteriorType":
                    if (displayString == "Select Interior Type")
                        return "-1";
                    break;
                case "TireCondition":
                    if (displayString == "Select Title Status")
                        return "-1";
                    break;
                case "TireMfg":
                    if (displayString == "Select Tire Mfg")
                        return "0";
                    break;
                case "ExteriorDamageCategory":
                case "InteriorDamageCategory":
                    if (displayString == "Select Damage Location")
                        return "0";
                    break;
                case "ExteriorDamageCondition":
                case "InteriorDamageCondition":
                    if (displayString == "Select Damage Condition")
                        return "0";
                    break;
                case "ExteriorDamageSeverity":
                case "InteriorDamageSeverity":
                    if (displayString == "No Damage Severity Specified")
                        return "0";
                    break;
                case "PriorPaintCategory":
                    if (displayString == "Select Prior Paint Location")
                        return "0";
                    break;
                case "PriorPaintCondition":
                    if (displayString == "Select Prior Paint Condition")
                        return "0";
                    break;
            }

            var entry = dropDownValues[element].Where(x => x.Value == displayString).FirstOrDefault();
            return entry.Equals(new KeyValuePair<int, string>()) ? null : entry.Key.ToString();
        }

        public bool FillMappings(string session)
        {
            if (mappings == null || mappings.Count() == 0)
            {
                mappings = new Dictionary<string, Dictionary<string, List<int>>>();
                Wholesale.lmReturnValue mappingReturn = Self.wholesaleClient.GetInspectionMappings(session);
                if (mappingReturn.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    foreach (DataRow map in mappingReturn.Data.Tables["Mappings"].Rows)
                    {
                        if (mappings.ContainsKey(map["Category"].ToString()))
                        {
                            if (mappings[map["Category"].ToString()].ContainsKey(map["Condition"].ToString()))
                                mappings[map["Category"].ToString()][map["Condition"].ToString()].Add(int.Parse(map["Severity"].ToString()));
                            else
                                mappings[map["Category"].ToString()].Add(map["Condition"].ToString(), new List<int>() { int.Parse(map["Severity"].ToString()) });
                        }
                        else
                        {
                            mappings.Add(map["Category"].ToString(), new Dictionary<string, List<int>>() {
                                                                            { map["Condition"].ToString(), new List<int>() { int.Parse(map["Severity"].ToString()) } } });
                        }
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        public bool FillDropdownValues(string session)
        {
            if (dropDownValues == null || dropDownValues.Count() == 0)
            {
                dropDownValues = new Dictionary<string, Dictionary<int, string>>();
                Wholesale.lmReturnValue returnedValues = Self.wholesaleClient.GetDropdownValues(session);
                if (returnedValues.Result == Wholesale.ReturnCode.LM_SUCCESS)
                {
                    foreach (DataRow value in returnedValues.Data.Tables["DropdownValues"].Rows)
                    {
                        if (dropDownValues.ContainsKey(value["Dropdown"].ToString()))
                        {
                            dropDownValues[value["Dropdown"].ToString()].Add(int.Parse(value["Value"].ToString()), value["DisplayString"].ToString());
                        }
                        else
                        {
                            dropDownValues.Add(value["Dropdown"].ToString(), new Dictionary<int, string>() { { int.Parse(value["Value"].ToString()), value["DisplayString"].ToString() } });
                        }
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        public Dealer.lmReturnValue GetDealerInfo(string kSession, int kDealer)
        {
            return Self.dealerClient.GetDealerInfo(kSession, kDealer, "", "DealerProduct");
        }
    }
}