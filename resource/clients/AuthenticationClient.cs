using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.Authenticate;
using LMWholesale.resource.clients.interfaces;

namespace LMWholesale.resource.clients
{
    public class AuthenticationClient : IAuthenticateClient
    {
        private AuthenticateSoapClient _authClient;
        private static readonly string client = "Authenticate";

        // Default Constructor
        public AuthenticationClient() { }

        public AuthenticationClient(AuthenticateSoapClient client) => _authClient = client;

        public AuthenticateSoapClient GetClient()
        {
            if (_authClient == null)
            {
                BasicHttpBinding httpBinding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    ReceiveTimeout = new TimeSpan(0, 10, 0)
                };

                EndpointAddress epa = new EndpointAddress(Util.GetIniEntry(client));
                _authClient = new AuthenticateSoapClient(httpBinding, epa);

            }

            return _authClient;
        }

        public lmReturnValue Login(string Username, string UserPassword, string Platform, int isFleet = 0)
        {
            return GetClient().Login(Username, UserPassword, Platform, isFleet);
        }
        public lmReturnValue Logout(string kSession)
        {
            return GetClient().Logout(kSession);
        }
        public lmReturnValue ResetPasswordRequest(string Username, string Email)
        {
            return GetClient().ResetPasswordRequest(Username, Email, "lmi");
        }
        public lmReturnValue ResetPasswordSet(string NewUserPassword, string ValidationCode)
        {
            return GetClient().ResetPasswordSet(NewUserPassword, ValidationCode);
        }
        public lmReturnValue CheckUserPassword(string Username, string UserPassword)
        {
            return GetClient().CheckUserPassword(Username, UserPassword);
        }
        public lmReturnValue GetRelayCallCounts(string AuthToken)
        {
            return GetClient().GetRelayCallCounts(AuthToken);
        }
        public lmReturnValue PartnerLogin(string UserID, string UserPassword, string kDealerId, string PartnerKey, string HostIP)
        {
            return GetClient().PartnerLogin(UserID, UserPassword, kDealerId, PartnerKey, HostIP);
        }
        public lmReturnValue GetPermissions(string kSession, int kDealer)
        {
            return GetClient().GetPermissions(kSession, kDealer);
        }
        public bool SessionCheck(string kSession)
        {
            lmReturnValue rv = GetClient().SessionCheck(kSession);
            if (rv.Result == ReturnCode.LM_SUCCESS)
                return true;
            return false;
        }
    }
}