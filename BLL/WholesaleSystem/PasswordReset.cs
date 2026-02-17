using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.BLL.WholesaleSystem
{
    public class PasswordReset
    {
        private AuthenticationClient authenticationClient;
        public PasswordReset(AuthenticationClient authenticationClient) => this.authenticationClient = authenticationClient;
        public PasswordReset() => authenticationClient = authenticationClient ?? new AuthenticationClient();

        internal static readonly PasswordReset instance = new PasswordReset();
        public PasswordReset Self
        {
            get { return instance; }
        }

        public bool Request(string username, string email)
        {
            Authenticate.lmReturnValue returnValue = Self.authenticationClient.ResetPasswordRequest(username, email);
            if (returnValue.Result == Authenticate.ReturnCode.LM_SUCCESS)
            {
                return true;
            }

            return false;
        }

        public int Set(string NewPassword, string ValCode)
        {
            Authenticate.lmReturnValue reset = Self.authenticationClient.ResetPasswordSet(NewPassword, ValCode);
            if (reset.Result == Authenticate.ReturnCode.LM_SUCCESS)
            {
                return 1;
            }
            return 0;
        }
    }
}