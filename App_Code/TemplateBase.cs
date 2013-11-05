// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Xml.Xsl;
using System.Text;
using System.Text.RegularExpressions;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    ///		This is the "code behind" for the skin user control (usually template.ascx) to provide menu building logic, or 
    ///		any other common logic required. The code is placed in this class, and the user control is derived from this class
    ///		so users can create new skins WITHOUT having to recompile, or even have VS.NET, etc...
    /// </summary>
    public class TemplateBase : System.Web.UI.UserControl
    {

        protected PlaceHolder PageContent;       
        protected ComponentArt.Web.UI.Menu PageMenu;
        protected ComponentArt.Web.UI.Menu VertMenu;
        protected ComponentArt.Web.UI.TreeView PageTree;

        public new SkinBase Page
        {
            get
            {
                return (SkinBase)base.Page;
            }
        }

        public PlaceHolder Content
        {
            get
            {
                return PageContent;
            }
        }

        public string CurrentEntity
        {
            get
            {
                return CommonLogic.Capitalize(CommonLogic.GetThisPageName(false).Replace("show", "").Replace(".aspx", "").ToLowerInvariant());
            }
        }

        public string ResourceMatchEvaluator(Match match)
        {
            string l = match.Groups[1].Value;
            string s = AppLogic.GetString(l, Page.SkinID, Page.ThisCustomer.LocaleSetting);
            if (s == null || s.Length == 0 || s == l)
            {
                s = match.Value;
            }
            return s;
        }

        public string ResourceMatchEvaluatorXmlEncoded(Match match)
        {
            string l = match.Groups[1].Value;
            string s = HttpUtility.HtmlEncode(AppLogic.GetString(l, Page.SkinID, Page.ThisCustomer.LocaleSetting));
            if (s == null || s.Length == 0 || s == l)
            {
                s = match.Value;
            }
            return XmlCommon.XmlEncode(s);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            //the first check is for the controls in the majority of the pages, but if we're on the homepage the control is on the page itself
            System.Web.UI.Control ctlStoreLocator = this.Page.Controls[0].FindControl("StoreLocatorLink") ?? this.Page.Controls[0].Page.FindControl("StoreLocatorLink");

            if (ctlStoreLocator != null)
            {
                ctlStoreLocator.Visible = AppLogic.AppConfigBool("ShowStoreLocator");
            }
         
            if (PageMenu != null)
            {
                // get menu config file:
                string MN = "menuData.xml";

                string CacheName = string.Format("menudoc_{0}_{1}_{2}_{3}", "false", Page.SkinID.ToString(), Page.ThisCustomer.LocaleSetting, MN);
                XmlDocument doc = null;
                if (AppLogic.CachingOn)
                {
                    doc = (XmlDocument)HttpContext.Current.Cache.Get(CacheName);
                }
                if (doc == null)
                {

                    doc = new XmlDocument();

                    string MenuConfigFileString = CommonLogic.ReadFile(CommonLogic.SafeMapPath("skins/skin_" + Page.SkinID.ToString() + "/" + MN), false);

                    doc.LoadXml(MenuConfigFileString);

                    HierarchicalTableMgr tblMgr;

                    XmlNode rootNode = doc.SelectSingleNode("/SiteMap");

                    // Find Manufacturers menu top
                    XmlNode mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Manufacturers!)']");
                    tblMgr = AppLogic.ManufacturerEntityHelper.m_TblMgr;
                    if (tblMgr.NumRootLevelNodes <= AppLogic.MaxMenuSize())
                    {
                        AddEntityMenuXsl(doc, "Manufacturer", tblMgr, mNode, 0, string.Empty);
                    }
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Manufacturers0!)']");
                    if (tblMgr.NumRootLevelNodes <= AppLogic.MaxMenuSize())
                    {
                        AddEntityMenuXsl(doc, "Manufacturer", tblMgr, mNode, 0, "TopItemLook");
                    }

                    // Find Categories menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Categories!)']");
                    AddEntityMenuXsl(doc, "Category", AppLogic.CategoryEntityHelper.m_TblMgr, mNode, 0, string.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Categories0!)']");
                    AddEntityMenuXsl(doc, "Category", AppLogic.CategoryEntityHelper.m_TblMgr, mNode, 0, "TopItemLook");


                    // Find Sections menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Sections!)']");
                    AddEntityMenuXsl(doc, "Department", AppLogic.SectionEntityHelper.m_TblMgr, mNode, 0, string.Empty); mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Sections0!)']");
                    AddEntityMenuXsl(doc, "Department", AppLogic.SectionEntityHelper.m_TblMgr, mNode, 0, "TopItemLook");

                    Regex m_ReMatch = new Regex(@"\(!(.*?)!\)");
                    MatchEvaluator m_ResourceMatch = new MatchEvaluator(ResourceMatchEvaluatorXmlEncoded);
                    doc.InnerXml = m_ReMatch.Replace(doc.InnerXml, m_ResourceMatch);

                    if (AppLogic.CachingOn)
                    {
                        HttpContext.Current.Cache.Insert(CacheName, doc, null, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()), TimeSpan.Zero);
                    }
                }
                if (PageMenu != null)
                {
                    PageMenu.LoadXml(doc);
                }
            }
            if (VertMenu != null)
            {
                string CacheName = string.Format("vertmenudoc_{0}_{1}_{2}", "false", Page.SkinID.ToString(), Page.ThisCustomer.LocaleSetting);
                XmlDocument doc = null;
                if (AppLogic.CachingOn)
                {
                    doc = (XmlDocument)HttpContext.Current.Cache.Get(CacheName);
                }
                if (doc == null)
                {

                    doc = new XmlDocument();

                    // get menu config file:
                    string MenuConfigFileString = CommonLogic.ReadFile(CommonLogic.SafeMapPath("skins/skin_" + Page.SkinID.ToString() + "/vertMenuData.xml"), false);

                    doc.LoadXml(MenuConfigFileString);

                    XmlNode rootNode = doc.SelectSingleNode("/SiteMap");

                    // Find Manufacturers menu top
                    XmlNode mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Manufacturers!)']");
                    AddEntityMenuXsl(doc, "Manufacturer", AppLogic.ManufacturerEntityHelper.m_TblMgr, mNode, 0, string.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Manufacturers0!)']");
                    AddEntityMenuXsl(doc, "Manufacturer", AppLogic.ManufacturerEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

                    // Find Categories menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Categories!)']");
                    AddEntityMenuXsl(doc, "Category", AppLogic.CategoryEntityHelper.m_TblMgr, mNode, 0, string.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Categories0!)']");
                    AddEntityMenuXsl(doc, "Category", AppLogic.CategoryEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

                    // Find Sections menu top
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Sections!)']");
                    AddEntityMenuXsl(doc, "Department", AppLogic.SectionEntityHelper.m_TblMgr, mNode, 0, string.Empty);
                    mNode = doc.DocumentElement.SelectSingleNode("//item[@Text='(!menu.Sections0!)']");
                    AddEntityMenuXsl(doc, "Department", AppLogic.SectionEntityHelper.m_TblMgr, mNode, 0, "VertTopItemLook");

                    Regex m_ReMatch = new Regex(@"\(!(.*?)!\)");
                    MatchEvaluator m_ResourceMatch = new MatchEvaluator(ResourceMatchEvaluatorXmlEncoded);
                    doc.InnerXml = m_ReMatch.Replace(doc.InnerXml, m_ResourceMatch);

                    if (AppLogic.CachingOn)
                    {
                        HttpContext.Current.Cache.Insert(CacheName, doc, null, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()), TimeSpan.Zero);
                    }
                }
                if (VertMenu != null)
                {
                    VertMenu.LoadXml(doc);
                }
            }
            if (PageTree != null)
            {
                // Note: Tree doc cannot be cached, as it changes every page, (as we have to open the selected tree node based on query string params)
                StringBuilder tmpS = new StringBuilder(4096);
                int curEntityID = CommonLogic.QueryStringUSInt("EntityID");
                string curEntity = CurrentEntity;
                tmpS.Append("<siteMap>");
                if (AppLogic.AppConfigBool("Tree.ShowCategories"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Category").ComponentArtTree("0", Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringCanBeDangerousContent("CategoryID")));
                }
                if (AppLogic.AppConfigBool("Tree.ShowDepartments"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Department").ComponentArtTree("0", Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringCanBeDangerousContent("DepartmentID")));
                }
                if (AppLogic.AppConfigBool("Tree.ShowManufacturers"))
                {
                    tmpS.Append(AppLogic.LookupHelper("Manufacturer").ComponentArtTree("0", Page.SkinID, Page.ThisCustomer.LocaleSetting, CommonLogic.QueryStringCanBeDangerousContent("ManufacturerID")));
                }
                if (AppLogic.AppConfigBool("Tree.ShowCustomerService"))
                {
                    string custSvcXml = "<siteMapNode Text=\"" + XmlCommon.XmlEncodeAttribute(AppLogic.GetString("menu.CustomerService", Page.SkinID, Page.ThisCustomer.LocaleSetting)) + "\" NavigateUrl=\"t-service.aspx\">";
                    custSvcXml += AppLogic.AppConfig("Tree.CustomerServiceXml");
                    if (custSvcXml.Length != 0)
                    {
                        Regex m_ReMatch = new Regex(@"\(!(.*?)!\)");
                        MatchEvaluator m_ResourceMatch = new MatchEvaluator(ResourceMatchEvaluatorXmlEncoded);
                        custSvcXml = m_ReMatch.Replace(custSvcXml, m_ResourceMatch);
                    }
                    custSvcXml += "</siteMapNode>";
                    XmlDocument x = new XmlDocument();
                    try
                    {
                        x.LoadXml(custSvcXml);
                    }
                    catch
                    {
                        custSvcXml = "<siteMapNode Text=\"Invalid XML fragment in Tree.ShowCustomerService AppConfig parameter\" NavigateUrl=\"\" />";
                    }
                    tmpS.Append(custSvcXml);
                }
                tmpS.Append("</siteMap>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(tmpS.ToString());
                PageTree.LoadXml(doc);
            }
        }

        private void AddEntityMenuXsl(XmlDocument doc, string EntityName, HierarchicalTableMgr m_TblMgr, XmlNode mnuItem, int ForParentEntityID, string NodeClass)
        {
            if (mnuItem == null) { return; }

            var tmpS = new StringWriter();
            var xForm = new XslCompiledTransform();
            string XslFile = "EntityMenuList";
            xForm.Load(CommonLogic.SafeMapPath("EntityHelper/" + XslFile + ".xslt"));
            XsltArgumentList xslArgs = new XsltArgumentList();
            xslArgs.AddParam("entity", "", EntityName);
            xslArgs.AddParam("custlocale", "", Page.ThisCustomer.LocaleSetting);
            xslArgs.AddParam("deflocale", "", Localization.WebConfigLocale);
            xslArgs.AddParam("ForParentEntityID", "", ForParentEntityID);
            xslArgs.AddParam("adminsite", "", false);
            xslArgs.AddParam("nodeclass", "", NodeClass);
            xslArgs.AddParam("suppresstoparrow", "", CommonLogic.IIF(NodeClass.Length != 0, "1", "0"));

            //let's call the extensions :)
            Customer thisCustomer = ((InterpriseSuiteEcommercePrincipal)HttpContext.Current.User).ThisCustomer;
            if (null != thisCustomer)
            {
                XSLTExtensionBase ext = new XSLTExtensionBase(thisCustomer, thisCustomer.SkinID);
                xslArgs.AddExtensionObject("urn:ise", ext);
            }

            xForm.Transform(m_TblMgr.XmlDoc, xslArgs, tmpS);
            if (AppLogic.AppConfigBool("XmlPackage.DumpTransform"))
            {
                try // don't let logging crash the site
                {
                    var sw = File.CreateText(CommonLogic.SafeMapPath(string.Format("images/{0}_{1}_{2}.xfrm.xml", XslFile, EntityName, "store")));
                    sw.WriteLine(XmlCommon.PrettyPrintXml(tmpS.ToString()));
                    sw.Close();
                }
                catch { }
            }

            if (tmpS.ToString().Length == 0) { return; }
            
            if (NodeClass.Length != 0) // this means we are adding to the ROOT level, not as children!
            {
                // Create a document fragment to contain the XML to be inserted. 
                var docFrag = doc.CreateDocumentFragment();

                    // Set the contents of the document fragment. 
                    docFrag.InnerXml = tmpS.ToString();

                    // Add the children of the document fragment to the original document. 
                    mnuItem.ParentNode.InsertAfter(docFrag, mnuItem);

                // now get rid of the parent placeholder node!
                doc.SelectSingleNode("/SiteMap").RemoveChild(mnuItem);
            }
            else
            {
                mnuItem.InnerXml = tmpS.ToString();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

    }
}
