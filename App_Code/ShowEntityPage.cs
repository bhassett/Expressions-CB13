// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for ShowEntityPage.
    /// 
    /// NOTE: the sql creation statements neeed to be cleaned up. they are pretty unreadible and inefficient at the moment due to substitutions
    /// 
    /// </summary>
    public class ShowEntityPage
    {

        private EntitySpecs m_EntitySpecs;

        private String m_EntityInstanceID;
        private String m_EntityInstanceName;
        private String m_EntityInstanceNameForDisplay;
        private EntityHelper m_EntityHelper;
        private SkinBase m_SkinBase;
        private String m_ResourcePrefix;
        private String m_XmlPackage;
        private XmlNode n;

        private String m_SectionFilterID = String.Empty;
        private String m_CategoryFilterID = String.Empty;
        private String m_ManufacturerFilterID = String.Empty;
        private String m_ProductTypeFilterID = String.Empty;
        private String m_AttributeFilterID = String.Empty;

        private String m_AttributeFilter = String.Empty;

        private string _pageOutput = string.Empty;

        public ShowEntityPage(EntitySpecs eSpecs, SkinBase sb)
        {
            m_EntitySpecs = eSpecs;
            m_SkinBase = sb;
            m_EntityHelper = AppLogic.LookupHelper(m_SkinBase.EntityHelpers, m_EntitySpecs.m_EntityName);
            m_ResourcePrefix = String.Format("show{0}.aspx.", m_EntitySpecs.m_EntityName.ToLowerInvariant());
            m_EntityInstanceID = CommonLogic.QueryStringCanBeDangerousContent(m_EntityHelper.GetEntitySpecs.m_EntityName + "ID");
        }

        public void Page_Load(object sender, System.EventArgs e)
        {
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }
            n = m_EntityHelper.m_TblMgr.SetContext(m_EntityInstanceID);
            
            if (n == null)
            {
                HttpContext.Current.Response.Redirect("t-error404.aspx");
            }

            m_CategoryFilterID = CommonLogic.QueryStringCanBeDangerousContent("CategoryFilterID");
            m_SectionFilterID = CommonLogic.QueryStringCanBeDangerousContent("SectionFilterID");
            m_ProductTypeFilterID = CommonLogic.QueryStringCanBeDangerousContent("ProductTypeFilterID");
            m_ManufacturerFilterID = CommonLogic.QueryStringCanBeDangerousContent("ManufacturerFilterID");
            m_AttributeFilterID = CommonLogic.QueryStringCanBeDangerousContent("AttributeFilterID");
            m_AttributeFilter = CommonLogic.GetAttributeFilter();

            if (CommonLogic.QueryStringCanBeDangerousContent("CategoryFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0  && CommonLogic.CookieUSInt("CategoryFilterID") != 0)
                {
                    m_CategoryFilterID = CommonLogic.CookieCanBeDangerousContent("CategoryFilterID", false);
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("SectionFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0  && CommonLogic.CookieUSInt("SectionFilterID") != 0)
                {
                    m_SectionFilterID = CommonLogic.CookieCanBeDangerousContent("SectionFilterID", false);
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("ProductTypeFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 &&  CommonLogic.CookieUSInt("ProductTypeFilterID") != 0)
                {
                    m_ProductTypeFilterID = CommonLogic.CookieCanBeDangerousContent("ProductTypeFilterID", false);
                }
                if (m_ProductTypeFilterID != String.Empty && !AppLogic.ProductTypeHasVisibleProducts(m_ProductTypeFilterID))
                {
                    m_ProductTypeFilterID = String.Empty;
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("ManufacturerFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 &&  CommonLogic.CookieUSInt("ManufacturerFilterID") != 0)
                {
                    m_ManufacturerFilterID = CommonLogic.CookieCanBeDangerousContent("ManufacturerFilterID", false);
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("AttributeFilterID").Length == 0)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length == 0 && CommonLogic.CookieUSInt("AttributeFilterID") != 0)
                {
                    m_AttributeFilterID = CommonLogic.CookieCanBeDangerousContent("AttributeFilterID", false);
                }
            }

            if (CommonLogic.QueryStringCanBeDangerousContent("ResetFilters").Length != 0)
            {
                m_CategoryFilterID = String.Empty;
                m_SectionFilterID = String.Empty;
                m_ManufacturerFilterID = String.Empty;
                m_ProductTypeFilterID = String.Empty;
                m_AttributeFilterID = String.Empty;
                m_AttributeFilter = String.Empty;
            }

            m_EntityInstanceName = m_EntityHelper.m_TblMgr.CurrentName(n, m_SkinBase.ThisCustomer.LocaleSetting);
            m_EntityInstanceNameForDisplay = CommonLogic.IIF(m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "Description", m_SkinBase.ThisCustomer.LocaleSetting) != String.Empty, m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "Description", m_SkinBase.ThisCustomer.LocaleSetting), m_EntityHelper.m_TblMgr.CurrentName(n, m_SkinBase.ThisCustomer.LocaleSetting)); 

            AppLogic.SetCookie("LastViewedEntityName", m_EntitySpecs.m_EntityName, new TimeSpan(1, 0, 0, 0, 0));
            AppLogic.SetCookie("LastViewedEntityInstanceID", m_EntityInstanceID.ToString(), new TimeSpan(1, 0, 0, 0, 0));
            AppLogic.SetCookie("LastViewedEntityInstanceName", m_EntityInstanceNameForDisplay, new TimeSpan(1, 0, 0, 0, 0));

            m_SkinBase.SETitle = m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "SETitle", m_SkinBase.ThisCustomer.LocaleSetting);
            if (m_SkinBase.SETitle.Length == 0)
            {
                m_SkinBase.SETitle = HttpContext.Current.Server.HtmlEncode(AppLogic.AppConfig("StoreName") + " - " + m_EntityInstanceName);
            }
            m_SkinBase.SEDescription = m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "SEDescription", m_SkinBase.ThisCustomer.LocaleSetting);
            if (m_SkinBase.SEDescription.Length == 0)
            {
                m_SkinBase.SEDescription = HttpContext.Current.Server.HtmlEncode(m_EntityInstanceName);
            }
            m_SkinBase.SEKeywords = m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "SEKeywords", m_SkinBase.ThisCustomer.LocaleSetting);
            if (m_SkinBase.SEKeywords.Length == 0)
            {
                m_SkinBase.SEKeywords = HttpContext.Current.Server.HtmlEncode(m_EntityInstanceName);
            }
            m_SkinBase.SENoScript = m_EntityHelper.m_TblMgr.CurrentFieldByLocale(n, "SENoScript", m_SkinBase.ThisCustomer.LocaleSetting);

            m_SkinBase.SectionTitle = "<span class=\"SectionTitleText\">";
            String ParentName = String.Empty;
            String ParentID = m_EntityHelper.GetParentEntity(m_EntityInstanceID);
            while (ParentID != String.Empty)
            {
                ParentName = CommonLogic.IIF(m_EntityHelper.GetEntityField(ParentID, "Description", m_SkinBase.ThisCustomer.LocaleSetting) != String.Empty, 
                                                m_EntityHelper.GetEntityField(ParentID, "Description", m_SkinBase.ThisCustomer.LocaleSetting), 
                                                m_EntityHelper.GetEntityName(ParentID, m_SkinBase.ThisCustomer.LocaleSetting));

                m_SkinBase.SectionTitle = "<a class=\"SectionTitleText\" href=\"" + SE.MakeEntityLink(m_EntitySpecs.m_EntityName, ParentID, ParentName) + "\">" + ParentName + 
                                          "</a> &rarr; " + m_SkinBase.SectionTitle;
                ParentID = m_EntityHelper.GetParentEntity(ParentID);
            }
            m_SkinBase.SectionTitle += m_EntityInstanceNameForDisplay;
            m_SkinBase.SectionTitle += "</span>";
            AppLogic.LogEvent(m_SkinBase.ThisCustomer.CustomerCode, 9, m_EntityInstanceID.ToString());

            //Include for mobile manufacturer
            if (CurrentContext.IsRequestingFromMobileMode(m_SkinBase.ThisCustomer))
            {
                m_XmlPackage = m_EntityHelper.m_TblMgr.CurrentField(n, "MobileXmlPackage").ToLowerInvariant();
                if (m_XmlPackage.IsNullOrEmptyTrimmed())
                {
                    m_XmlPackage = m_EntityHelper.m_TblMgr.CurrentField(n, "XmlPackage").ToLowerInvariant();
                }
            }
            else
            {
                m_XmlPackage = m_EntityHelper.m_TblMgr.CurrentField(n, "XmlPackage").ToLowerInvariant();
            }

            if (m_XmlPackage.Length == 0)
            {
                m_XmlPackage = AppLogic.ro_DefaultEntityXmlPackage; // provide a default for backwards compatibility
            }

            GeneratePageOutput();
        }

        private void GeneratePageOutput()
        {
            string cacheName = string.Format("entityname={0}&entitycode={1}&localesetting={2}&pagenum={3}&affiliateid={4}",
                                          m_EntitySpecs.m_EntityName,
                                          m_EntityInstanceID,
                                          m_SkinBase.ThisCustomer.LocaleSetting,
                                          CommonLogic.QueryStringUSInt("PageNum").ToString(),
                                          m_SkinBase.ThisCustomer.AffiliateID.ToString());

            if (AppLogic.AppConfigBool("CacheEntityPageHTML"))
            {
                _pageOutput = CachingFactory.ApplicationCachingEngineInstance.GetItem<string>(cacheName);
                if (CommonLogic.IsStringNullOrEmpty(_pageOutput))
                {
                    _pageOutput = GetPageOutput();
                    CachingFactory.ApplicationCachingEngineInstance.AddItem(cacheName, _pageOutput, AppLogic.CacheDurationMinutes());
                }
            }
            else
            {
                CachingFactory.ApplicationCachingEngineInstance.RemoveItem(cacheName);
                _pageOutput = GetPageOutput();
            }
        }

        private string GetPageOutput()
        {
            StringBuilder output = new StringBuilder();
            output.Append("<!-- XmlPackage: " + m_XmlPackage + " -->\n");

            if (m_XmlPackage.Length == 0)
            {
                output.Append("<p><b><font color=red>XmlPackage format was chosen, but no XmlPackage was specified!</font></b></p>");
            }
            else
            {
                if ("ATTRIBUTE".Equals(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase))
                {

                    if ("CATEGORY".Equals(CommonLogic.QueryStringCanBeDangerousContent("EntityName").Trim().ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_CategoryFilterID = CommonLogic.QueryStringCanBeDangerousContent("EntityID");
                    }
                    else if ("DEPARTMENT".Equals(CommonLogic.QueryStringCanBeDangerousContent("EntityName").Trim().ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_SectionFilterID = CommonLogic.QueryStringCanBeDangerousContent("EntityID");
                    }

                    m_AttributeFilter = CommonLogic.GetAttributeFilter();

                }
                else 
                {
                    m_AttributeFilter = string.Empty;
                    m_AttributeFilterID = string.Empty;
                }
                List<XmlPackageParam> runtimeParams = new List<XmlPackageParam>();
                runtimeParams.Add(new XmlPackageParam("EntityName", m_EntitySpecs.m_EntityName));
                runtimeParams.Add(new XmlPackageParam("EntityID", m_EntityInstanceName));
                runtimeParams.Add(new XmlPackageParam("EntityCode", m_EntityInstanceID));
                runtimeParams.Add(new XmlPackageParam("CatCode", CommonLogic.IIF("CATEGORY".Equals(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase), m_EntityInstanceName, m_CategoryFilterID.ToString())));
                runtimeParams.Add(new XmlPackageParam("DepCode", CommonLogic.IIF("DEPARTMENT".Equals(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase), m_EntityInstanceName, m_SectionFilterID.ToString())));
                runtimeParams.Add(new XmlPackageParam("ManCode", CommonLogic.IIF("MANUFACTURER".Equals(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase), m_EntityInstanceName, m_ManufacturerFilterID.ToString())));
                runtimeParams.Add(new XmlPackageParam("AttCode", CommonLogic.IIF("ATTRIBUTE".Equals(m_EntitySpecs.m_EntityName.Trim().ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase), m_EntityInstanceName, m_AttributeFilterID.ToString())));
                runtimeParams.Add(new XmlPackageParam("AttributeFilter", m_AttributeFilter));
                
               // filter by websitecode
               runtimeParams.Add(new XmlPackageParam("WebSiteCode", InterpriseHelper.ConfigInstance.WebSiteCode));
                
                
                output.Append(
                    AppLogic.RunXmlPackage(
                        m_XmlPackage,
                        m_SkinBase.GetParser,
                        m_SkinBase.ThisCustomer,
                        m_SkinBase.SkinID,
                        String.Empty,
                        runtimeParams,
                        true,
                        true
                    )
                );
            }

            return output.ToString();
        }

        public void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write(_pageOutput);
        }

        
    }
}