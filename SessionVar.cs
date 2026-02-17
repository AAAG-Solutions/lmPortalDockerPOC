using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.SessionState;

namespace NewWholesale
{
    public class SessionVar
    {
        HttpSessionState Session = HttpContext.Current.Session;
        public static string kDealer = Session["kDealer"];

    }
}