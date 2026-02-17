using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using System.Web.SessionState;

namespace LMWholesale.BLL.WholesaleContent
{
    public class ImportInventory
    {
        private readonly DASClient dasClient;
        private readonly DealerClient dealerClient;
        private readonly OldDASClient oldDASClient;

        public ImportInventory()
        {
            dasClient = dasClient ?? new DASClient();
            dealerClient = dealerClient ?? new DealerClient();
            oldDASClient = oldDASClient ?? new OldDASClient();
        }

        public ImportInventory(DASClient dasClient, DealerClient dealerClient, OldDASClient oldDASClient)
        {
            this.dasClient = dasClient;
            this.dealerClient = dealerClient;
            this.oldDASClient = oldDASClient;
        }

        internal static readonly ImportInventory instance = new ImportInventory();
        public static ImportInventory Self
        {
            get { return instance; }
        }

        internal Dictionary<string, string> GetDealerPaths(string kSession, int kDealer)
        {
            Dealer.lmReturnValue returnVal = Self.dealerClient.GetDealerPaths(kSession, kDealer);
            Dictionary<string, string> retDict = new Dictionary<string, string>() { { "Success", "0" }, { "Message", "" } };
            if(returnVal.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                foreach (DataColumn item in returnVal.Data.Tables[0].Columns)
                {
                    retDict[item.ColumnName] = returnVal.Data.Tables[0].Rows[0][item.ColumnName].ToString();
                }
                retDict["Success"] = "1";
            }
            else
                retDict["Message"] = "GetDealerPaths Filed: " + returnVal.ResultString;
            return retDict;
        }

        internal Dictionary<string, string> DealerImport(string kSession, string kDealer, string InvAcc, string FilePath, string FileName, string Delimiter, int kDealerImport, int ImportType)
        {
            OldDAS.lmReturnValue returnVal = Self.oldDASClient.DealerImport(kSession, 25, kDealer, InvAcc, FilePath, FileName, Delimiter, kDealerImport, ImportType);
            if(returnVal.Result == OldDAS.ReturnCodes.LM_SUCCESS)
                return new Dictionary<string, string>() { { "Success", "1" }, { "Message", "" } };
            else
                return new Dictionary<string, string>() { { "Success", "0" }, { "Message", returnVal.ResultString } };
        }

        internal Dictionary<string, string> ImportConfigGet(string kSession, int kDealer)
        {
            Dealer.lmReturnValue returnVal = Self.dealerClient.ImportConfigGet(kSession, kDealer);
            Dictionary<string, string> retDict = new Dictionary<string, string>() { { "Success", "0" }, { "Message", "" } };
            if (returnVal.Result == Dealer.ReturnCode.LM_SUCCESS)
            {
                foreach (DataColumn item in returnVal.Data.Tables[0].Columns)
                {
                    retDict[item.ColumnName] = returnVal.Data.Tables[0].Rows[0][item.ColumnName].ToString();
                }
                retDict["Success"] = "1";
            }
            else
                retDict["Message"] = "ImportConfigGet Filed: " + returnVal.ResultString;

            return retDict;
        }

        internal DAS.lmReturnValue ImportStatusGet(string kSession, int kDealer)
        {
            return Self.dasClient.ImportStatusGet(kSession, kDealer);
        }

        internal Dealer.lmReturnValue ImportSystemGet(string kSession, string VehicleInvAcc)
        {
            return Self.dealerClient.ImportSystemGet(kSession, VehicleInvAcc);   
        }
        
        internal Dealer.lmReturnValue ImportConfigGetLRV(string kSession, int kDealer)
        {
            return Self.dealerClient.ImportConfigGet(kSession, kDealer);
        }
    }
}