using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LMWholesale.resource.factory.services;

namespace LMWholesale.resource.factory
{
    public class AuctionFactory
    {
        public IAuctionService GetAuctionService(string auction)
        {
            if (auction == "ADESA")
                return new AdesaService();

            if (auction == "OVE")
                return new OVEService();
            
            if (auction == "SmartAuction")
                return new SmartAuctionService();

            if (auction == "AuctionEdge")
                return new AuctionEdgeService();

            if (auction == "ACV Auctions")
                return new ACVAuctionService();

            if (auction == "eDealer Direct")
                return new eDealerService();

            if (auction == "IAA")
                return new IaaService();

            if (auction == "COPART")
                return new CopartService();

            if (auction == "Auction Simplified")
                return new AuctionSimplifiedService();

            if (auction == "IAS")
                return new IasService();

            if (auction == "AuctionOS")
                return new AuctionOSService();

            if (auction == "Carmigo")
                return new CarmigoService();

            if (auction == "CarOffer")
                return new CarOfferService();

            if (auction == "RemarketingPlus")
                return new RemarketingPlusService();

            // Default return type
            // Returns an emptry service
            return new DefaultClass();
        }

        public IAuctionService GetAuctionService(int auction)
        {
            if (auction == 1)
                return new OVEService();

            if (auction == 2)
                return new SmartAuctionService();

            if (auction == 4)
                return new AdesaService();

            if (auction == 6)
                return new CopartService();

            if (auction == 7)
                return new AuctionEdgeService();

            if (auction == 11)
                return new ACVAuctionService();

            if (auction == 12)
                return new eDealerService();

            if (auction == 13)
                return new IaaService();;

            if (auction == 14)
                return new AuctionSimplifiedService();

            if (auction == 15)
                return new IasService();

            if (auction == 16)
                return new AuctionOSService();

            if (auction == 17)
                return new CarmigoService();

            if (auction == 18)
                return new CarOfferService();

            if (auction == 19)
                return new RemarketingPlusService();

            // Default return type
            // Returns an emptry service
            return new DefaultClass();
        }
    }
}