using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.UI;

using LMWholesale.Common;


namespace LMWholesale
{
    public partial class PasswordReset : lmPage
    {
        private readonly BLL.WholesaleSystem.PasswordReset passwordresetBLL;
        public PasswordReset() => passwordresetBLL = passwordresetBLL ?? new BLL.WholesaleSystem.PasswordReset();

        public static PasswordReset Self
        {
            get { return instance; }
        }
        private static readonly PasswordReset instance = new PasswordReset();

        protected void Page_Load(object sender, EventArgs e)
        {
            Control masterPageHeader = Page.Master.FindControl("WholesalePortalHeader");
            if (masterPageHeader != null)
                masterPageHeader.Visible = false;

            if (!String.IsNullOrEmpty(Request.QueryString["mode"]))
            {
                PasswordContent.Visible = false;
                ResetContent.Visible = true;
                ResetText.InnerText = "Please enter a new password for your account";
                passRequirement.InnerHtml = @"<br/><span>&emsp;Password must meet expected criteria:</span><br/>
                            <span id='passUL'>&emsp;- Must include upper and lower case character</span><br/>
                            <span id='passNum'>&emsp;- Must include a number</span><br/>
                            <span id='passSpecial'>&emsp;- Must include a special character</span><br/>
                            <span id='passLen'>&emsp;- Must be at least 8 characters in length</span><br/>
                            <span id='passMatch'>&emsp;- Passwords match</span><br/>";
            }
            else
            {
                PasswordContent.Visible = true;
                ResetContent.Visible = false;
                FirstText.InnerText = "Enter the username and email that were used when you registered.\n";
                SecondText.InnerText = "After clicking submit, an email will be sent to you with further instructions.\n";
            }
        }

        [WebMethod]
        public static Dictionary<string, object> ResetRequest(string username, string email)
        {
            Dictionary<string, object> returnValue = new Dictionary<string, object>
            {
                { "success", 0 },
                { "message", "" }
            };

            // Validate if we aren't tempting to reset a password for an invalid username/email combo
            if (String.IsNullOrEmpty(username))
            {
                Message = "Username is required";
                IsSuccess = false;
            }
            if (String.IsNullOrEmpty(email))
            {
                Message = "Email is required";
                IsSuccess = false;
            }

            if (Self.passwordresetBLL.Request(username, email))
            {
                Message = $@"<fieldset class='sectionFieldset'>
                        <legend>Password Reset Request</legend>
                            <div style='text-align:center;'>
                                The email has been sent to:<br/>
                                {email}<br/><br/>
                                Please follow the instructions sent in a follow up email.<br/><br/>
                                If you do not recieve an email within 5 minutes, please check your spam folder, submit request again, or contact support for further assistance.
                                <br/><br/>
                                <a href='/WholesaleSystem/Login.aspx'>Back to Login Page</a>
                            </div>
                    </fieldset>";
            }
            else
            {
                Message = "Something went wrong! Please contact support!";
                IsSuccess = false;
            }

            return ReturnResponse();
        }

        [WebMethod]
        public static int ResetSet(string newPass, string valCode)
        {
            Regex rgx = new Regex(@"^(.{0,7}|[^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*)$");

            if (String.IsNullOrEmpty(valCode))
                return 0;

            // Exit early if for some reason we don't catch a non-compliant password attempt in the UI
            if (rgx.IsMatch(newPass))
                return -1;

            return Self.passwordresetBLL.Set(newPass, valCode);
        }
    }
}