using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.DAS;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.resource.clients
{
    public class DASClient : IDASClient
    {
        private DASSoapClient _dasClient;
        private static readonly string client = "DAS";

        // Default Constructor
        public DASClient() { }

        public DASClient(DASSoapClient client) => _dasClient = client;

        public DASSoapClient GetClient()
        {
            if (_dasClient == null)
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 10, 0)
                };
                EndpointAddress epa = new EndpointAddress(Util.GetIniEntry(client));
                _dasClient = new DASSoapClient(httpBinding, epa);

            }

            return _dasClient;
        }

        public lmReturnValue ImportStatusGet(string kSession, int kDealer)
        {
            return GetClient().ImportStatusGet(kSession, kDealer);
        }
        public lmReturnValue ImportStatusSet(string kSession, string JsonData)
        {
            return GetClient().ImportStatusSet(kSession, JsonData);
        }
        public lmReturnValue DASCreateInventory(string Session,
                    string kDealer,
                    string StockNumber,
                    string VIN,
                    string MotorYear,
                    string Miles,
                    string InventoryCost,
                    string Make,
                    string Model,
                    string Style,
                    string StyleID,
                    string InventoryListPrice,
                    string Drilldown,
                    string StockType,
                    string Status,
                    string ImportOverride,
                    string InternetPrice)
        {
            return GetClient().DASCreateInventory(Session,
                    kDealer,
                    StockNumber,
                    VIN,
                    MotorYear,
                    Miles,
                    InventoryCost,
                    Make,
                    Model,
                    Style,
                    StyleID,
                    InventoryListPrice,
                    Drilldown,
                    StockType,
                    Status,
                    ImportOverride,
                    InternetPrice);
        }
        public lmReturnValue DASDeleteInventory(string kSession, int kListing)
        {
            return GetClient().DASDeleteInventory(kSession, kListing);
        }
        public lmReturnValue DASGetVinExplosion(string kSession, string VIN)
        {
            return GetClient().DASGetVinExplosion(kSession, VIN);
        }
        public lmReturnValue DASGetMotorYears(string kSession, string Source)
        {
            return GetClient().DASGetMotorYears(kSession, Source);
        }
        public lmReturnValue DASGetMotorMakes(string kSession, string Source, string Year)
        {
            return GetClient().DASGetMotorMakes(kSession, Source, Year);
        }
        public lmReturnValue DASGetMotorModels(string kSession, string Source, string Year, string Make)
        {
            return GetClient().DASGetMotorModels(kSession, Source, Year, Make);
        }
        public lmReturnValue DASGetMotorStyles(string kSession, string Source, string Year, string Make, string Model)
        {
            return GetClient().DASGetMotorStyles(kSession, Source, Year, Make, Model);
        }
        public lmReturnValue DASResolveBlackbook(string kSession, int kIdentity, int kBlackbook, string Year, string Make, string Model, string Style, string Series, int Mode)
        {
            return GetClient().DASResolveBlackbook(kSession, kIdentity, kBlackbook, Year, Make, Model, Style, Series, Mode);
        }
        public lmReturnValue DASResolveBluebook(string kSession, int kIdentity, int kVehicleID, int Mode)
        {
            return GetClient().DASResolveBluebook(kSession, kIdentity, kVehicleID, Mode);
        }
        public lmReturnValue DASResolveChromeYMMS(string kSession, string VIN)
        {
            return GetClient().DASResolveChromeYMMS(kSession, VIN);
        }
        public lmReturnValue DASUpdateInventory(string kSession, string data)
        {
            return GetClient().DASUpdateInventory(kSession, data);
        }
        public lmReturnValue GetAnalyticsReRun(string kSession, string Operation, string Listing)
        {
            return GetClient().GetAnalyticsReRun(kSession, Operation, Listing);
        }
        public lmReturnValue GetHealthReportMin(string kSession, int kDealer)
        {
            return GetClient().GetHealthReportMin(kSession, kDealer);
        }
        public lmReturnValue GetOfferReport(string kSession, int kDealer)
        {
            return GetClient().GetOfferReport(kSession, kDealer);
        }
        public lmReturnValue GetHealthReportSummary(string kSession, int kDealer)
        {
            return GetClient().GetHealthReportSummary(kSession, kDealer);
        }
        public lmReturnValue GetHealthReportDetail(string kSession, int kDealer, DateTime FirstDateMonth)
        {
            return GetClient().GetHealthReportDetail(kSession, kDealer, FirstDateMonth);
        }
        public lmReturnValue GetAACurrentlyPosted(string kSession, int kGaggleSubGroup, int kDealer)
        {
            return GetClient().GetAACurrentlyPosted(kSession, kGaggleSubGroup, kDealer);
        }
        public lmReturnValue GetDealerActiveWholesale(string kSession, int kDealer)
        {
            return GetClient().GetDealerActiveWholesale(kSession, kDealer);
        }
        public lmReturnValue CurrentResultsWidgetGet(string kSession, int kDealer)
        {
            return GetClient().CurrentResultsWidgetGet(kSession, kDealer);
        }
    }
}