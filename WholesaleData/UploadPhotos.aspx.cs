 using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using LMWholesale.Common;

namespace LMWholesale.WholesaleData
{
    public partial class UploadPhotos : lmPage
    {
        private readonly BLL.WholesaleData.UploadPhotos uploadPhotos;

        public UploadPhotos() => uploadPhotos = uploadPhotos ?? new BLL.WholesaleData.UploadPhotos();

        public static UploadPhotos Self
        {
            get { return instance; }
        }

        private static readonly UploadPhotos instance = new UploadPhotos();

        protected void Page_Load(object sender, EventArgs e)
        {
            PageSecurityManager.DoPageSecurity(this);

            Response.Expires = 0;
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["kListing"]))
                {
                    string kListing = Request.QueryString["kListing"];
                    string kSession = (string)Session["kSession"];
                    int kDealer = (int)Session["kDealer"];

                    if (kDealer == -1)
                        kDealer = (int)Session["AL_kDealer"];

                    Page.ClientScript.RegisterClientScriptInclude("aurigmahtml", "/Common/Aurigma/aurigma.htmluploader.control.js");
                    Page.ClientScript.RegisterClientScriptInclude("aurigmaflash", "/Common/Aurigma/aurigma.imageuploaderflash.min.js");

                    HtmlHead head = Page.Header;
                    HtmlLink link = new HtmlLink();

                    link.Attributes.Add("href", "/Common/Aurigma/aurigma.htmluploader.control.css");
                    link.Attributes.Add("type", "text/css");
                    link.Attributes.Add("rel", "stylesheet");
                    head.Controls.Add(link);

                    Dictionary<string, DataRow> returnRows = Self.uploadPhotos.GetDealerRelatedInfo(kSession, kDealer);

                    // Hide Top Pane
                    Aurigma.ImageUploaderFlash.TopPane topPane = PhotoUploader.TopPane;
                    topPane.Visible = false;

                    // Add Converters
                    Aurigma.ImageUploaderFlash.ConverterCollection converters = PhotoUploader.Converters;
                    int highResolution = int.Parse(returnRows["DealerGeneral"]["UseHiRes"].ToString());
                    int forceRatio = int.Parse(returnRows["DealerImageInfo"]["ForceRatio"].ToString());

                    if (highResolution == 1 && forceRatio != 0)
                        converters.Add(new Aurigma.ImageUploaderFlash.Converter { Mode = "*.*=SourceFile" });
                    else
                    {
                        Aurigma.ImageUploaderFlash.Converter nonHighRes = new Aurigma.ImageUploaderFlash.Converter
                        {
                            Mode = "*.*=Thumbnail",
                            ThumbnailFitMode = Aurigma.ImageUploaderFlash.ThumbnailFitMode.Fit,
                            ThumbnailUseExifPreview = false
                        };
                        if (highResolution == 2)
                        {
                            nonHighRes.ThumbnailHeight = 480;
                            nonHighRes.ThumbnailWidth = 640;
                        }
                        else if (highResolution == 3)
                        {
                            nonHighRes.ThumbnailHeight = 600;
                            nonHighRes.ThumbnailWidth = 800;
                        }
                        else if (highResolution == 4)
                        {
                            nonHighRes.ThumbnailHeight = 768;
                            nonHighRes.ThumbnailWidth = 1024;
                        }
                        else if (highResolution == 5)
                        {
                            nonHighRes.ThumbnailHeight = 960;
                            nonHighRes.ThumbnailWidth = 1280;
                        }
                        converters.Add(nonHighRes);

                        Aurigma.ImageUploaderFlash.ThumbnailFitMode fitMode = Aurigma.ImageUploaderFlash.ThumbnailFitMode.Width;
                        string[] heights = { "240", "96", "48" };
                        if (forceRatio == 0)
                            fitMode = Aurigma.ImageUploaderFlash.ThumbnailFitMode.Fit;
                        else
                        {
                            heights[0] = "0";
                            heights[1] = "0";
                            heights[2] = "0";
                        }

                        converters.Add(new Aurigma.ImageUploaderFlash.Converter { Mode = "*.*=Thumbnail", ThumbnailFitMode = fitMode,
                            ThumbnailWidth = 320, ThumbnailHeight = int.Parse(heights[0])});
                        converters.Add(new Aurigma.ImageUploaderFlash.Converter { Mode = "*.*=Thumbnail", ThumbnailFitMode = fitMode,
                            ThumbnailWidth = 128, ThumbnailHeight = int.Parse(heights[1]) });
                        converters.Add(new Aurigma.ImageUploaderFlash.Converter { Mode = "*.*=Thumbnail", ThumbnailFitMode = fitMode,
                            ThumbnailWidth = 64, ThumbnailHeight = int.Parse(heights[2]) });
                    }

                    // Add Restrictions on file exts
                    Aurigma.ImageUploaderFlash.Restrictions restict = PhotoUploader.Restrictions;
                    restict.FileMask = "[[\"Images\", \"*.jpg;*.jpeg\"]]";
                    restict.MaxFileSize = 104857600;

                    // Define UploadSettings
                    Aurigma.ImageUploaderFlash.UploadSettings uploadSettings = PhotoUploader.UploadSettings;
                    uploadSettings.ActionUrl = $"UploadPhotosHandler.ashx?kListing={kListing}";
                    uploadSettings.RedirectUrl = "UploadPhotos.aspx?complete=1";

                    // Load Session info and specific dealer upload paths to a Dictionary<string, object> in Session
                    LoadSessionData(kListing, kSession);
                }

                int complete = 0;

                try
                {
                    complete = int.Parse(Request.QueryString["complete"]);
                }
                catch { }

                if (complete == 1)
                {
                    if (Session["PhotoUpload_Complete"].ToString() == "success")
                        Response.Write("<script type=\"text/javascript\">alert(\"The photos were successfully uploaded\");window.close();</script>");
                    else if (Session["PhotoUpload_Complete"].ToString() == "invalid")
                    {
                        Response.Write("<script type=\"text/javascript\">alert(\"Something went wrong! Please contact support.\");window.close();</script>");
                        BLL.WholesaleUser.WholesaleUser.ClearUser();
                    }
                    else if (Session["PhotoUpload_Complete"].ToString() == "error")
                    {
                        string response = $"<script>alert(\"Unable to perform request due to the following error: {Session["PhotoUpload_ErrorString"]} ";
                        response += $"({Session["PhotoUpload_ErrorDetail"]}).  Please try again or call support for assistance.\");</script>";
                        Response.Write(response);
                    }

                    Session["PhotoUpload_ErrorString"] = "";
                    Session["PhotoUpload_ErrorDetail"] = "";
                }
            }
        }

        private void LoadSessionData(string kListing, string kSession)
        {
            DataRow dr = Self.uploadPhotos.GetListingPaths(kSession, kListing);
            Dictionary<string, string> sessionDict = new Dictionary<string, string>
            {
                { "VIN", dr["VIN"].ToString() },
                { "UploadPath", dr["PhotoPathProcessed"].ToString() },
                { "NextPhoto", dr["LastPhotoOrder"].ToString() },
            };

            string physicalPath = dr["PhotoPathProcessed"].ToString();
            string originalPath = $"{physicalPath}Originals\\";

            try
            {
                System.IO.Directory.CreateDirectory(physicalPath.Substring(physicalPath.LastIndexOf("=") + 1));
                System.IO.Directory.CreateDirectory(originalPath);
            }
            catch
            {
                // No error to report here, just making sure the remote directory exists
            }

            // Add to Session for a later date
            Session["UploadPhotosInfo"] = Util.serializer.Serialize(sessionDict);
        }
    }
}