using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using LMWholesale.Common;

namespace LMWholesale.WholesaleData
{
    public partial class TrainingVideo : lmPage
    {
        private readonly Dictionary<string, string> videos = new Dictionary<string, string>()
        {
            // General Trainings
            //{ "AutoLaunchRules", "/Video/LM_UMAL_Training_Video.mp4" },
            { "TrainingOTG", "/Video/OTG_Training.mp4" },
            { "WholesalePortalOverview", "/Video/General_Overview_WholesalePortal.mp4" },
            { "LC", "/Video/LC_App.webm" },

            // Quick One-Off Subjects
            { "AddOTG", "/Video/OTG_Add.mp4" },
            { "AutoLaunchOTG", "/Video/OTG_AutoLaunch.mp4" },
            { "ListManual", "/Video/Manual_List.mp4" },
            { "DeactivateListings", "/Video/Deactivate_Listings.mp4" }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            PageSecurityManager.DoPageSecurity(this);

            Response.Expires = 0;
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (!string.IsNullOrEmpty(Request.QueryString["VideoName"]))
            {
                string name = Request.QueryString["VideoName"];
                string kSession = (string)Session["kSession"];
                int kDealer = (int)Session["kDealer"];

                VideoPlayer.Src = videos[name];
            }
        }
    }
}