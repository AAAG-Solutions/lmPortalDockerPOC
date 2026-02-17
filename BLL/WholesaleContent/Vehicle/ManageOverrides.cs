using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;
using System.Web.Script.Serialization;

namespace LMWholesale.BLL.WholesaleContent.Vehicle
{
    public class ManageOverrides
    {
        private readonly InventoryClient inventoryClient;

        public ManageOverrides(InventoryClient inventoryClient)
        {
            this.inventoryClient = inventoryClient;
        }

        public ManageOverrides() => inventoryClient = inventoryClient ?? new InventoryClient();
        internal static readonly ManageOverrides instance = new ManageOverrides();
        public ManageOverrides Self
        {
            get { return instance; }
        }

        internal Dictionary<string, string> GetOverrides(string Session, int kListing)
        {
            Dictionary<string, string> returnSet = new Dictionary<string, string>() { { "Success", "false" } };

            Inventory.lmReturnValue result = Self.inventoryClient.GetInventoryOverrides(Session, kListing);

            if (result.Result == Inventory.ReturnCode.LM_SUCCESS)
            {
                DataTable dt = result.Data.Tables["InventoryOverrides"];
                returnSet["Success"] = "true";
                
                foreach(DataColumn col in dt.Columns)
                    returnSet.Add(col.ColumnName, dt.Rows[0][col.ColumnName].ToString());
            }
            else
            {
                returnSet.Add("ErrorMessage", result.ResultString);
            }

            return returnSet;
        }

        internal string SetOverrides(string Session, string jsonData)
        {
            Dictionary<string, string> returnSet = new Dictionary<string, string>() { { "Success", "0" }, { "Message", "" } };

            Dictionary<string, object> dataIn = (Dictionary<string, object>)Util.serializer.DeserializeObject(jsonData);
            if (dataIn["InvAdded"] != null && dataIn["InvAdded"].ToString() != "")
                dataIn["InvAdded"] = Convert.ToDateTime(dataIn["InvAdded"]).ToString("yyyy-MM-dd HH:mm:ss:fff");

            Inventory.lmReturnValue result = Self.inventoryClient.SetInventoryOverrides(Session, Util.serializer.Serialize(dataIn));

            if (result.Result == Inventory.ReturnCode.LM_SUCCESS)
            {
                returnSet["Success"] = "1";
                returnSet["Message"] = "Overrides saved successfully";
            }
            else
            {
                returnSet["Message"] = result.ResultString;
            }

            return Util.serializer.Serialize(returnSet);
        }
    }
}