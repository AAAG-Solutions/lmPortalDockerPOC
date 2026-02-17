using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace LMWholesale.WholesaleData
{
    public class UploadPhotosHandler : Aurigma.ImageUploaderFlash.UploadHandler, IHttpHandler, IRequiresSessionState
    {
        private readonly BLL.WholesaleData.UploadPhotos uploadPhotos;
        private int kListing;

        public UploadPhotosHandler()
        {
            FileUploaded += OnFileUploaded;
            AllFilesUploaded += OnAllFilesUploaded;
            uploadPhotos = uploadPhotos ?? new BLL.WholesaleData.UploadPhotos();
            kListing = int.Parse(HttpContext.Current.Request.QueryString["kListing"].ToString());
        }

        public void OnFileUploaded(object sender, Aurigma.ImageUploaderFlash.FileUploadedEventArgs e)
        {
            HttpSessionState session = ((HttpContext)sender.GetType().GetProperty("Context").GetValue(sender)).Session;
            try
            {
                Dictionary<string, object> sessionInfo =
                    (Dictionary<string, object>)Util.serializer.DeserializeObject(session["UploadPhotosInfo"].ToString());

                string vin = sessionInfo["VIN"].ToString();
                string imagePath = sessionInfo["UploadPath"].ToString();

                if (int.Parse(session["NextPhotoNum"].ToString()) == 0)
                    session["NextPhotoNum"] = int.Parse(sessionInfo["NextPhoto"].ToString());

                List<Aurigma.ImageUploaderFlash.ConvertedFile> files = EnumToList(e.UploadedFile.ConvertedFiles);
                string now = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (files.Count == 1)
                {
                    string filenameOut = string.Format("{0}_{1:000}_{2}_Orig.jpg", vin, int.Parse(session["NextPhotoNum"].ToString()), now);
                    e.UploadedFile.ConvertedFiles[0].SaveAs(System.IO.Path.Combine(System.IO.Path.Combine(imagePath, "Originals"), filenameOut));
                    session["FileList"] = session["FileList"].ToString() + $"|{filenameOut}";
                    session["NextPhotoNum"] = int.Parse(session["NextPhotoNum"].ToString()) + 1;
                }
                else
                {
                    string originalOut =
                        string.Format("Originals\\{0}_{1:000}_{2}.jpg", vin, int.Parse(session["NextPhotoNum"].ToString()), now);
                    string[] outFrames =
                    {
                        string.Format("{0}_{1:000}_{2}_LG.jpg", vin, int.Parse(session["NextPhotoNum"].ToString()), now),
                        string.Format("{0}_{1:000}_{2}_SM.jpg", vin, int.Parse(session["NextPhotoNum"].ToString()), now),
                        string.Format("{0}_{1:000}_{2}_TH.jpg", vin, int.Parse(session["NextPhotoNum"].ToString()), now),
                        string.Format("{0}_{1:000}_{2}_IC.jpg", vin, int.Parse(session["NextPhotoNum"].ToString()), now)
                    };

                    e.UploadedFile.ConvertedFiles[0].SaveAs(System.IO.Path.Combine(imagePath, originalOut));
                    for (int i = 0; i < files.Count; i++)
                        e.UploadedFile.ConvertedFiles[i].SaveAs(System.IO.Path.Combine(imagePath, outFrames[i]));
                    session["FileList"] = session["FileList"].ToString() + $"|{outFrames[0].Replace("_LG.", ".")}";
                    session["NextPhotoNum"] = int.Parse(session["NextPhotoNum"].ToString()) + 1;
                }
            }
            catch (Exception ex)
            {
                session["ErrorPageName"] = "UploadPhotos.aspx";
                session["ErrorSource"] = "OnFileUploaded";
                session["ErrorMessage"] = $"Message - {ex.Message} | StackTrace - {ex.StackTrace}";
            }
        }

        public void OnAllFilesUploaded(object sender, Aurigma.ImageUploaderFlash.AllFilesUploadedEventArgs e)
        {
            HttpSessionState session = ((HttpContext)sender.GetType().GetProperty("Context").GetValue(sender)).Session;
            try
            {
                string kSession = (string)session["kSession"];
                Dictionary<string, object> sessionInfo =
                    (Dictionary<string, object>)Util.serializer.DeserializeObject(session["UploadPhotosInfo"].ToString());

                Listing.lmReturnValue success =
                    uploadPhotos.ExplicitAddPhotos(kSession, kListing, sessionInfo["UploadPath"].ToString(), session["FileList"].ToString(), "_LG|_SM|_TH|_IC");
                if (success.Result == Listing.ReturnCode.LM_SUCCESS)
                    session["PhotoUpload_Complete"] = "success";
                else
                {
                    if (success.Result == Listing.ReturnCode.LM_INVALIDSESSION)
                    {
                        session["PhotoUpload_Complete"] = "invalid";
                        session["PhotoUpload_ErrorString"] = success.ResultString;
                        session["PhotoUpload_ErrorDetail"] = success.Result;
                    }
                    else
                    {
                        session["PhotoUpload_Complete"] = "error";
                        session["PhotoUpload_ErrorString"] = success.ResultString;
                        session["PhotoUpload_ErrorDetail"] = success.Result;
                    }
                }

                // Clear all the things
                session["FileList"] = "";
                session["NextPhotoNum"] = 0;
            }
            catch(Exception ex)
            {
                session["ErrorPageName"] = "UploadPhotos.aspx";
                session["ErrorSource"] = "OnAllFilesUploaded";
                session["ErrorMessage"] = $"Message - {ex.Message} | StackTrace - {ex.StackTrace}";
            }
        }

        private List<Aurigma.ImageUploaderFlash.ConvertedFile> EnumToList(Aurigma.ImageUploaderFlash.ConvertedFileCollection files)
        {
            List<Aurigma.ImageUploaderFlash.ConvertedFile> result = new List<Aurigma.ImageUploaderFlash.ConvertedFile>();
            foreach (Aurigma.ImageUploaderFlash.ConvertedFile file in files)
                result.Add(file);

            return result;
        }
    }
}