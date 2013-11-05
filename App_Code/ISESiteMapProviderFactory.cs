// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Caching;
using System.Collections.Generic;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for ISESiteMapProviderFactory
    /// </summary>
    public class ISESiteMapProviderFactory
    {
        #region Variable Declaration

        private static ISESiteMapProviderFactory _instance = new ISESiteMapProviderFactory();
        private List<string> _cacheKeys = new List<string>();
        private static object _mutex = new object();
        private string _xmlPackage = "page.menu.xml.config";

        #endregion

        #region Constructor

        private ISESiteMapProviderFactory() { }

        #endregion

        #region Properties
        
        public static ISESiteMapProviderFactory Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Methods

        public ISESiteMapProvider GetProvider(string locale)
        {
            locale = locale.ToLowerInvariant();

            // check if the appconfig:ShowStringresourceKeys is on
            if (AppLogic.AppConfigBool("ShowStringResourceKeys") == true)
            {
                return new ISESiteMapProvider(locale);
            }

            ISESiteMapProvider provider = null;

            string key = MenuKey(locale);
            if (AppLogic.CachingOn)
            {
                provider = HttpRuntime.Cache.Get(key) as ISESiteMapProvider;
            }

            if (provider == null)
            {
                provider = new ISESiteMapProvider(locale);
            }

            if (provider != null && 
                AppLogic.CachingOn)
            {
                if (HttpContext.Current != null && 
                    provider != null && 
                    HttpRuntime.Cache.Get(key) == null)
                {
                    string menuPackage = CommonLogic.SafeMapPath(string.Format("~/xmlpackages/{0}", _xmlPackage));
                    if (CommonLogic.FileExists(menuPackage))
                    {
                        CacheDependency fileDependency = new CacheDependency(menuPackage);

                        HttpRuntime.Cache.Add(key,
                            provider,
                            fileDependency,
                            DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()),
                            Cache.NoSlidingExpiration,
                            CacheItemPriority.High,
                            null);

                        if (!_cacheKeys.Contains(key))
                        {
                            _cacheKeys.Add(key);
                        }
                    }
                }
            }

            return provider;
        }

        private string MenuKey(string locale)
        {
            return string.Format("menu_{0}", locale);
        }

        public void Reset()
        {
            foreach (string key in _cacheKeys)
            {
                HttpRuntime.Cache.Remove(key);
            }

            _cacheKeys.Clear();
        }

        #endregion

    }

}
