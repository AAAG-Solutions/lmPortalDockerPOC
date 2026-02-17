using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleContent.Preferences
{
    public class General
    {
        private readonly DealerClient dealerClient;
        private readonly WholesaleUser.WholesaleUser userBLL;

        public General()
        {
            dealerClient = dealerClient ?? new DealerClient();
            userBLL = userBLL ?? new WholesaleUser.WholesaleUser();
        }

        public General(DealerClient dealerClient, WholesaleUser.WholesaleUser userBLL)
        {
            this.dealerClient = dealerClient;
            this.userBLL = userBLL;
        }

        internal static readonly General instance = new General();
        public General Self
        {
            get { return instance; }
        }

        public Dealer.lmReturnValue GetDealerInfo()
        {
            HttpSessionState session = HttpContext.Current.Session;
            string kSession = (string)session["kSession"];
            int kDealer = (int)session["kDealer"];

            return Self.dealerClient.GetDealerInfo(kSession, kDealer, "", "DealerBase,DealerProduct,DealerSubGroup,DistributorList,DealerGaggleList");
        }

        public Dealer.lmReturnValue DealerBaseSet(string JsonInfo)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            string kSession = (string)Session["kSession"];
            int kDealer = (int)Session["kDealer"];

            return Self.dealerClient.DealerBaseSet(kSession, kDealer, JsonInfo);
        }
    }
}