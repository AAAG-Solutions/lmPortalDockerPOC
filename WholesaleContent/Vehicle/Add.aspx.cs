using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using System.Web.Services;

using LMWholesale.Common;


namespace LMWholesale.WholesaleContent.Vehicle
{
    public partial class Add : lmPage
    {
        private readonly BLL.WholesaleUser.WholesaleUser userBLL;
        private readonly BLL.WholesaleContent.Vehicle.Add addBLL;

        public Add()
        {
            userBLL = userBLL ?? new BLL.WholesaleUser.WholesaleUser();
            addBLL = addBLL ?? new BLL.WholesaleContent.Vehicle.Add();
        }

        public static Add Self
        {
            get { return instance; }
        }
        private static readonly Add instance = new Add();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = "Add Vehicle";
            Self.userBLL.CheckDealer();
            PageSecurityManager.DoPageSecurity(this);

            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string dealerName = Session["DealerName"].ToString();
            string kSession = (string)Session["kSession"];

            InputStock.Attributes["oninput"] = WholesaleSystem.onInputString;
            InputCost.Attributes["oninput"] = WholesaleSystem.onInputNumber;

            if (!String.IsNullOrEmpty(Request.QueryString["VIN"]))
            {
                InputVIN.Text = Request.QueryString["VIN"];
            }

            AccountName.Text = dealerName;
        }

        [WebMethod(Description = "Return a list of Makes for a given Y")]
        public static string GetListMake(string year)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];            

            return Self.addBLL.MakeListGet(kSession, year, "3");
        }

        [WebMethod(Description = "Return a list of Models given a YM")]
        public static string GetListModel(string year, string make)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];

            return Self.addBLL.ModelListGet(kSession, year, make, "3");
        }

        [WebMethod(Description = "Return a list of Style given a YMM")]
        public static string GetListStyle(string year, string make, string model)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];

            return Self.addBLL.StyleListGet(kSession, year, make, model, "3");
        }

        [WebMethod(Description = "Perform ChromeData check on VIN")]
        public static string VINCheck(string vin, string styleCode)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];

            return Self.addBLL.CheckVIN(kSession, vin, styleCode);
        }

        [WebMethod]
        public static string ResolveBlackbook(int kListing, int kBlackbook, string year, string make, string model, string series, string style, int mode)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];

            return Self.addBLL.ResolveBlackBook(kSession, kListing, kBlackbook, year, make, model, series, style, mode);
        }

        [WebMethod]
        public static string ResolveBluebook(int kListing, int mode)
        {
            HttpSessionState Session = HttpContext.Current.Session;
            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string kSession = (string)Session["kSession"];
            return Self.addBLL.ResolveBlueBook(kSession, kListing, mode);
        }

        [WebMethod(Description = "Add VIN to dealer inventory")]
        public static Dictionary<string, object> AddInventory(string vehicleInfo)
        {
            Dictionary<string, object> info = (Dictionary<string, object>)Util.serializer.DeserializeObject(vehicleInfo);
            HttpSessionState Session = HttpContext.Current.Session;

            if (String.IsNullOrEmpty(Session["kSession"].ToString()))
                BLL.WholesaleUser.WholesaleUser.ClearUser();

            string message = "";
            string kListing = "0";
            IsSuccess = Self.addBLL.AddToInventory((string)Session["kSession"], (int)Session["kDealer"], info, ref message, ref kListing);
            Value = kListing;
            Message = message;

            return ReturnResponse();
        }
    }
}