using System.Collections.Generic;
using System.Data;

namespace LMWholesale.resource.factory
{
    public class DefaultClass : IAuctionService
    {
        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return "";
        }

        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            // Return empty DataTable if we fail for some reason
            return new DataTable();
        }

        public DataTable GetCredentials(string kSession, int kDealer)
        {
            // Return empty DataTable if we fail for some reason
            return new DataTable();
        }

        public jsGridBuilder GetJsGridBuilderInfo(string methodUrl)
        {
            return new jsGridBuilder();
        }
    }
}