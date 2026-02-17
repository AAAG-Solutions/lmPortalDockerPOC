using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

using static LMWholesale.BLL.WholesaleContent.Preferences.MarketPlaceInfo;

namespace LMWholesale.resource.factory
{
    public interface IAuctionService
    {
        string BuildListingInfo(Auction.Info auctionInfo);

        DataTable GetAuctionInfo(string kSession, int kDealer);

        DataTable GetCredentials(string kSession, int kDealer);

        jsGridBuilder GetJsGridBuilderInfo(string methodUrl);
    }
}