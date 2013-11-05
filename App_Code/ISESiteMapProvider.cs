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
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using InterpriseSuiteEcommerceCommon;
using System.Text.RegularExpressions;
using System.Web.Caching;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Default SiteMapProvider for this site
    /// </summary>
    public class ISESiteMapProvider : StaticSiteMapProvider
    {
        #region Variable Declaration

        private static object _mutex = new object();
        private SiteMapNode _root = null;
        private string _xmlPackage = "page.menu.xml.config";

        private const int DEFAULT_SKINID = 1;
        private string _localeSetting = string.Empty;
        #endregion

        #region Constructor

        public ISESiteMapProvider() : this(Customer.Current.LocaleSetting) { } 

        /// <summary>
        /// Constructor for ISESiteMapProvider
        /// </summary>
        public ISESiteMapProvider(string locale) 
        {
            _localeSetting = locale;
        }

        #endregion

        #region Properties

        public string LocaleSetting
        {
            get { return _localeSetting; }
        }

        private string EntityMenu_Category()
        {
            if (AppLogic.AppConfigBool("ShowStringResourceKeys"))
            {
                return "(!menu.Categories!)";
            }

            return AppLogic.GetString("menu.Categories", DEFAULT_SKINID, this.LocaleSetting);
        }

        private string EntityMenu_Manufacturer()
        {
            if (AppLogic.AppConfigBool("ShowStringResourceKeys"))
            {
                return "(!menu.Manufacturers!)";
            }

            return AppLogic.GetString("menu.Manufacturers", DEFAULT_SKINID, this.LocaleSetting);
        }

        private string EntityMenu_Department()
        {
            if (AppLogic.AppConfigBool("ShowStringResourceKeys"))
            {
                return "(!menu.Sections!)";
            }

            return AppLogic.GetString("menu.Sections", DEFAULT_SKINID, this.LocaleSetting);
        }

        #endregion

        #region Methods

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection attributes)
        {
            _xmlPackage = attributes["xmlPackage"];
            if (CommonLogic.IsStringNullOrEmpty(_xmlPackage))
            {
                throw new ArgumentNullException("XmlPackage attribute not specified!!!");
            }

            base.Initialize(name, attributes);
        }

        #region BuildSiteMap
        /// <summary>
        /// Loads the site map information from persistent storage and builds it in memory.
        /// </summary>
        /// <returns></returns>
        public override SiteMapNode BuildSiteMap()
        {
            lock (_mutex)
            {
                if (null == _root)
                {
                    BuildSiteMapTree();
                }

                return _root;
            }
        }
        #endregion

        #region Flush        
        /// <summary>
        /// Flushes the current cache so that it will rebuild the tree again
        /// </summary>
        public void Flush()
        {
            lock (_mutex)
            {
                this.Clear();
                _root = null;
            }
        }
        #endregion

        #region BuildSiteMapTree
        /// <summary>
        /// Core function to build the sitemap tree together with it's hierarchy
        /// </summary>
        private void BuildSiteMapTree()
        {
            XPathNavigator nav = GetMenuXml();
            XPathNodeIterator homeIterator = nav.Select("siteMap/siteMapNode");

            if (homeIterator.MoveNext())
            {
                XPathNavigator navHome = homeIterator.Current;
                _root = NewSiteMapNode(navHome);
                BuildSiteMapTree(navHome, _root);
            }
            else
            {
                throw new InvalidOperationException("Home SiteMap root element missing!!!");
            }

            AddNode(_root);
        }
        
        #endregion

        #region BuildSiteMapTree
        /// <summary>
        /// Core function to build the sitemap tree together with it's hierarchy
        /// </summary>
        /// <param name="nav">The XPathNavigator object</param>
        /// <param name="parent">The parent node</param>
        private void BuildSiteMapTree(XPathNavigator nav, SiteMapNode parent)
        {
            XPathNodeIterator enumerator = nav.Select("siteMapNode");
            while (enumerator.MoveNext())
            {
                XPathNavigator current = enumerator.Current;

                if (IsEntityNode(current)) {
                    SiteMapNode entityNode = NewSiteMapNode(current);
                    AddNode(entityNode, parent);
                    BuildEntityTree(current, entityNode);
                }
                else {
                    SiteMapNode node = NewSiteMapNode(current);
                    AddNode(node, parent);

                    BuildSiteMapTree(current, node);
                }
            }
        }
        #endregion       

        #region NewSiteMapNode
        /// <summary>
        /// Returns a new instance of SiteMapNode using the title and url attributes
        /// </summary>
        /// <param name="nav">The XPathNavigator object</param>
        /// <returns></returns>
        private SiteMapNode NewSiteMapNode(XPathNavigator nav)
        {
            string title = ExtractTitle(nav);
            string url = nav.GetAttribute("url", string.Empty);

            return new SiteMapNode(this, NewKey(), url, title);
        }
        #endregion

        #region NewKey
        /// <summary>
        /// Returns a random autogenerated key
        /// </summary>
        /// <returns></returns>
        private string NewKey()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion

        #region GetMenuXml
        /// <summary>
        /// Returns an XmlNavigator object from the loaded menu xml package
        /// </summary>
        /// <returns></returns>
        private XPathNavigator GetMenuXml()
        {
            Customer thisCustomer = Customer.Current;
            string menuXml = AppLogic.RunXmlPackage(_xmlPackage, null, thisCustomer, DEFAULT_SKINID, string.Empty, null, false, false);

            if (CommonLogic.IsStringNullOrEmpty(menuXml))
            {
                throw new InvalidOperationException("Menu xml cannot be empty string!!!");
            }

            XmlDocument doc = new XmlDocument();

            try
            {
                Regex pattern = new Regex(@"\(!(.*?)!\)");
                MatchEvaluator findAndReplaceStringResourced = new MatchEvaluator(StringResourceMatch);

                menuXml = pattern.Replace(menuXml, findAndReplaceStringResourced);                
                doc.LoadXml(menuXml);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return doc.CreateNavigator();            
        }
        #endregion

        private string  StringResourceMatch(Match match)
        {
            String l = match.Groups[1].Value;
            string s = HttpUtility.HtmlEncode(AppLogic.GetString(l, DEFAULT_SKINID, Customer.Current.LocaleSetting));
            if (s == null || s.Length == 0 || s == l)
            {
                s = match.Value;
            }
            return XmlCommon.XmlEncode(s);
        }

        #region GetRootNodeCore
        /// <summary>
        /// Gets the root node
        /// </summary>
        /// <returns></returns>
        protected override SiteMapNode GetRootNodeCore()
        {
            return BuildSiteMap();
        }
        #endregion

        private bool IsEntityNode(XPathNavigator nav)
        {
            string entity = ExtractTitle(nav);
            return  EntityMenu_Category().Equals(entity, StringComparison.InvariantCultureIgnoreCase) ||
                    EntityMenu_Manufacturer().Equals(entity, StringComparison.InvariantCultureIgnoreCase) ||
                    EntityMenu_Department().Equals(entity, StringComparison.InvariantCultureIgnoreCase);
        }

        private string ExtractTitle(XPathNavigator nav)
        {
            return nav.GetAttribute("title", string.Empty);
        }


        private void BuildEntityTree(XPathNavigator current, SiteMapNode parentNode)
        {
            string whichEntity = ExtractTitle(current);

            if (EntityMenu_Category().Equals(whichEntity, StringComparison.InvariantCultureIgnoreCase))
            {
                LoadEntity("Category", parentNode);
            }
            else if (EntityMenu_Manufacturer().Equals(whichEntity, StringComparison.InvariantCultureIgnoreCase))
            {
                LoadEntity("Manufacturer", parentNode);
            }
            else if (EntityMenu_Department().Equals(whichEntity, StringComparison.InvariantCultureIgnoreCase))
            {
                LoadEntity("Department", parentNode);
            }
        }

        private XmlNode ExtractXmlNode(XPathNavigator nav)
        {
            return (nav as IHasXmlNode).GetNode();
        }
        
        private void LoadEntity(string entityName, SiteMapNode parentNode)
        {
            EntityHelper entity = AppLogic.LookupHelper(entityName);
            if (entity != null)
            {
                XmlDocument entityDoc = entity.m_TblMgr.XmlDoc;
                if (entityDoc != null)
                {
                    XPathNavigator nav = entityDoc.CreateNavigator();
                    ExtractEntity(entityName, nav.Select("root/Entity"), parentNode);
                }
            }
        }

        private void ExtractEntity(string entityName, XPathNodeIterator entityIterator, SiteMapNode parentNode)
        {
            while (entityIterator.MoveNext())
            {
                XmlNode entityNode = ExtractXmlNode(entityIterator.Current);

                int entityId = XmlCommon.XmlFieldNativeInt(entityNode, "EntityID");
                string name = XmlCommon.XmlFieldByLocale(entityNode, "Description", Customer.Current.LocaleSetting);
                string url = SE.MakeEntityLink(entityName, entityId.ToString(), name);

                SiteMapNode entitySiteMapNode = new SiteMapNode(this, NewKey(), url, name);
                AddNode(entitySiteMapNode, parentNode);

                // recurse
                ExtractEntity(entityName, entityNode.CreateNavigator().Select("Entity"), entitySiteMapNode);
            }
        }

        #endregion

    }
}

