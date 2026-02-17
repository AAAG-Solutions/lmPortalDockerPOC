using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using LMWholesale.Common;

namespace LMWholesale
{
    public class Global : HttpApplication
    {
        #region Application Region
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Versioning so we can cache bust
            Application["ContentVersion"] = Util.GetRegistryString("SourceBranch", "portal");

            #if DEBUG
            BundleTable.EnableOptimizations = false;
            #endif
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            string outer = Server.GetLastError().StackTrace;
            Exception inner = Server.GetLastError().InnerException;

            if (inner != null || outer != null)
            {
                string url = context.Request.Url.ToString();
                Dictionary<string, string> exObject = new Dictionary<string, string>
                {
                    { "ErrorPageName", url },
                    { "ErrorSource", inner.Source },
                    { "ErrorMessage", inner.Message },
                    { "ErrorInnerStackTrace", inner.StackTrace },
                    { "ErrorOuterStackTrace", outer }
                };

                if (url.IndexOf("ErrorPage.aspx") == -1)
                {
                    Session["ExceptionObject"] = exObject;

                    context.Server.ClearError();
                    context.Server.Transfer("/WholesaleSystem/ErrorPage.aspx", true);
                }
            }
        }

        protected void Application_AuthenticationRequest(object sender, EventArgs e)
        {
            // Implement Me
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Reset lmPage Values
            lmPage.IsSuccess = true;
            lmPage.Message = string.Empty;
            lmPage.Value = null;

            HttpContext.Current.Items.Add("BeginRequest", DateTime.Now);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            DateTime BeginRequest = DateTime.Parse(HttpContext.Current.Items["BeginRequest"].ToString());
            TimeSpan ProcessingTime = DateTime.Now.Subtract(BeginRequest);

            //if (HttpContext.Current.Items.Contains("IsInternal") && bool.Parse(HttpContext.Current.Items["IsInternal"].ToString()))
            //{
            //    Page page = (Page)HttpContext.Current.Handler;
            //    if (page.Master != null)
            //    {
            //        ContentPlaceHolder mainContent = (ContentPlaceHolder)page.Master.FindControl("MainContent");
            //        ((HtmlGenericControl)mainContent.FindControl("VehicleNotes")).InnerHtml = "Request Time: " + ProcessingTime;
            //        HttpContext.Current.Items.Add("EndRequest", ProcessingTime);
            //    }
            //}
        }

        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            int a = 0;
            // Implement Me
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Implement Me
        }
        #endregion

        #region Session Region
        protected void Session_Start(object sender, EventArgs e)
        {
            // Implement Me
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Implement Me
        }
        #endregion
    }
}