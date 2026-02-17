using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.AccessControl;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Antlr.Runtime.Tree;
using LMWholesale.Common;
using Soss.Client;

namespace LMWholesale
{
    public partial class CachePop : lmPage
    {
        public static CachePop Self
        {
            get { return instance; }
        }
        private static readonly CachePop instance = new CachePop();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Nothing to really do here. Just popping some cache.
            // Internal LMI Employees should only know about this.
        }

        [WebMethod]
        static public Dictionary<string, object> Pop(string cacheType, string kPerson, string cacheKey, string cacheKeyPart)
        {
            try
            {
                if (cacheType == "1")
                {
                    // kPerson
                    NamedCache cache = CacheFactory.GetCache(kPerson);
                    if (cacheKey == "ANY")
                    {
                        IEnumerable<CachedObjectId> ic = cache.Keys.Cast<CachedObjectId>();
                        for (int i = 0; i < ic.Count(); i++)
                            cache.Remove(ic.ElementAt(i));
                    }
                    else if (cacheKey == "DealerUsers")
                    {
                        string key = cacheKeyPart + cacheKey;
                        cache.Remove(key);
                    }
                    else if (cacheKey == "availableAuctions")
                    {
                        string key = cacheKey + cacheKeyPart;
                        cache.Remove(key);
                    }
                    else if (cacheKey == "AutoLaunchRuleSets")
                    {
                        string key = cacheKey + cacheKeyPart;
                        cache.Remove(key);
                    }
                    else
                        cache.Remove(cacheKey);
                }
                else
                {
                    // Global
                    // Old Portal pages (PortalHome/DealerDashboard) remove the same key
                    CacheFactory.GetCache("GLOBAL").Remove("DescriptionBuilderXML");
                    CacheFactory.GetCache("GLOBAL").Remove("DealerTradeCenterDef");
                    if (cacheKey == "DealerDashboard")
                    {
                        CacheFactory.GetCache("GLOBAL").Remove("CustomHeaderDef");
                        CacheFactory.GetCache("GLOBAL").Remove("CustomerTypeDef");
                        CacheFactory.GetCache("GLOBAL").Remove("PartnerSessionDef");
                        CacheFactory.GetCache("GLOBAL").Remove("AllySetup");
                    }
                }
            }
            catch
            {
                IsSuccess = false;
                Message = "Unable to clear specific cache key.";
            }

            return ReturnResponse();
        }
    }
}