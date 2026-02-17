using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

using LMWholesale.Authenticate;

namespace LMWholesale.resource.clients.interfaces
{
    [ServiceContract]
    public interface IAuthenticateClient
    {
        [OperationContract]
        lmReturnValue Login(string UserID, string UserPassword, string Platform, int isFleet);
        [OperationContract]
        lmReturnValue Logout(string kSession);
        [OperationContract]
        lmReturnValue ResetPasswordRequest(string UserID, string Email);
        [OperationContract]
        lmReturnValue ResetPasswordSet(string NewUserPassword, string ValidationCode);
        [OperationContract]
        lmReturnValue CheckUserPassword(string Username, string UserPassword);
        [OperationContract]
        lmReturnValue GetRelayCallCounts(string AuthToken);
        [OperationContract]
        lmReturnValue PartnerLogin(string UserID, string UserPassword, string kDealerId, string PartnerKey, string HostIP);
        [OperationContract]
        lmReturnValue GetPermissions(string kSession, int kDealer);
        [OperationContract]
        bool SessionCheck(string kSession);
    }
}