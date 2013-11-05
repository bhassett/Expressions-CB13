// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    public class SkinBase : System.Web.UI.Page
    {
        //Per defect #86; Made public so that we can can get the cookie name outside of this class. 
        //Also updated to get it's value from the AppLogic class so that the setting is centeralized.
        public static readonly string ro_SkinCookieName = AppLogic.ro_SkinCookieName;

        private string m_ErrorMsg = string.Empty;
        private bool m_Editing = false;
        private bool m_DataUpdated = false;
        private bool m_DesignMode = false;
        private Customer m_ThisCustomer;
        private string m_SectionTitle = string.Empty;
        private TemplateBase m_Template = null;
        private string m_TemplateName = "template.ascx";
        private int m_SkinID = 1;
        private string m_SETitle = string.Empty;
        private string m_SEDescription = string.Empty;
        private string m_SEKeywords = string.Empty;
        private string m_SENoScript = string.Empty;
        private string m_TemplateFN = string.Empty;
        private bool m_DisableContents = false;
        private int m_DefaultSkinID = 1;
        private string m_GraphicsColor = string.Empty;
        private string m_ContentsBGColor = string.Empty;
        private string m_PageBGColor = string.Empty;
        private string m_IGD = string.Empty; // impersonation customer guid for admin phone order entry display of product pages, etc...

        private System.Collections.Generic.Dictionary<string, EntityHelper> m_EntityHelpers = new System.Collections.Generic.Dictionary<string, EntityHelper>();
        private Parser m_Parser;
        private HtmlForm _actualForm = null;

        public SkinBase(string TemplateName)
        {
            if (AppLogic.UseSSL() && AppLogic.OnLiveServer() && AppLogic.AppConfigBool("AlwaysGoSecure"))
            {
                AppLogic.RequireSecurePage();
            }

            m_TemplateName = TemplateName;
            if (TemplateName.Length == 0)
            {
                m_TemplateName = "template.ascx";
            }
            m_DesignMode = (HttpContext.Current == null);
            m_DefaultSkinID = AppLogic.DefaultSkinID();
            if (!m_DesignMode)
            {
                m_EntityHelpers.Add("Category", AppLogic.CategoryEntityHelper);
                m_EntityHelpers.Add("Department", AppLogic.SectionEntityHelper);
                m_EntityHelpers.Add("Manufacturer", AppLogic.ManufacturerEntityHelper);
                m_EntityHelpers.Add("Attribute", AppLogic.AttributeEntityHelper);
            }
        }

        public SkinBase() : this(GetTemplateName()) { }

        private static string GetTemplateName()
        {
            string templateName = CommonLogic.QueryStringCanBeDangerousContent(DomainConstants.DEFAULT_TEMPLATE_NAME);
            AppLogic.CheckForScriptTag(templateName);

            if (string.IsNullOrEmpty(templateName))
            {
                string pageName = CommonLogic.GetThisPageName(false);
                if (pageName.Equals("default.aspx", StringComparison.OrdinalIgnoreCase))
                {
                    templateName = AppLogic.AppConfig("HomeTemplate");
                }
                else 
                {
                    //do nothing if from mobile browsing - not supported as of now
                    if (!CurrentContext.IsRequestingFromMobileMode(Customer.Current))
                    {
                        //Template Switch
                        if (AppLogic.AppConfigBool("TemplateSwitching.Enabled"))
                        {
                            templateName = DoSwitchingReturnTemplate(pageName);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(templateName))
            {
                templateName = DomainConstants.DEFAULT_TEMPLATE_NAME;
            }

            if (templateName.Length > 0 && !templateName.EndsWith(DomainConstants.DEFAULT_TEMPLATE_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
            {
                templateName += "." + DomainConstants.DEFAULT_TEMPLATE_EXTENSION;
            }

            return templateName;
        }

        //Template Switching
        private static string DoSwitchingReturnTemplate(string pageName)
        {
            var fileNameWithoutExtArray = pageName.Split('.');

            string pageNameWithoutExtension = string.Empty;
            if (fileNameWithoutExtArray == null || fileNameWithoutExtArray.Length == 0) return pageNameWithoutExtension;

            pageNameWithoutExtension = fileNameWithoutExtArray[0];

            //If requesting from driver - change the page name to topic for user config issue.
            if (pageNameWithoutExtension == DomainConstants.DRIVER_PAGE_NAME)
            {
                pageNameWithoutExtension = DomainConstants.TOPIC_NAME; 
            }

            string templateConfigValue = "{0}.{1}".FormatWith(DomainConstants.TEMPLATE_SWITCHING_PREFIX, pageNameWithoutExtension);

            string templateName = AppLogic.AppConfig(templateConfigValue);
            if (templateName.IsNullOrEmptyTrimmed()) return string.Empty;

            templateName = templateName.ToLowerInvariant();
            //append .ascx if not set from Application Configuration
            if (!templateName.EndsWith(DomainConstants.DEFAULT_TEMPLATE_EXTENSION)) 
            { 
                templateName = "{0}.{1}".FormatWith(templateName, DomainConstants.DEFAULT_TEMPLATE_EXTENSION); 
            }

            return templateName;
        }

        private void FindLocaleStrings(Control c)
        {
            try
            {
                System.Web.UI.WebControls.Image i = c as System.Web.UI.WebControls.Image;
                if (i != null)
                {
                    if (i.ImageUrl.IndexOf("(!") >= 0)
                    {
                        i.ImageUrl = AppLogic.LocateImageURL(i.ImageUrl.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", string.Empty).Replace("!)", string.Empty), ThisCustomer.LocaleSetting);
                    }
                    if (i.AlternateText.IndexOf("(!") >= 0)
                    {
                        i.AlternateText = AppLogic.GetString(i.AlternateText, SkinID, ThisCustomer.LocaleSetting);
                    }
                }
                else
                {
                    System.Web.UI.WebControls.ImageButton b = c as System.Web.UI.WebControls.ImageButton;
                    if (b != null)
                    {
                        if (b.ImageUrl.IndexOf("(!") >= 0)
                        {
                            b.ImageUrl = AppLogic.LocateImageURL(b.ImageUrl.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", string.Empty).Replace("!)", string.Empty), ThisCustomer.LocaleSetting);
                        }
                    }
                    else
                    {
                        IEditableTextControl e = c as IEditableTextControl;
                        if (e != null)
                        {
                            if (!(e is ListControl))
                            {
                                e.Text = AppLogic.GetString(e.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                        }
                        else
                        {
                            IValidator v = c as IValidator;
                            if (v != null)
                            {
                                v.ErrorMessage = AppLogic.GetString(v.ErrorMessage.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            ITextControl t = c as ITextControl;
                            if (t != null)
                            {
                                t.Text = AppLogic.GetString(t.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            Button b2 = c as Button;
                            if (b2 != null)
                            {
                                b2.Text = AppLogic.GetString(b2.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            LinkButton l = c as LinkButton;
                            if (l != null)
                            {
                                l.Text = AppLogic.GetString(l.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            HyperLink h = c as HyperLink;
                            if (h != null)
                            {
                                h.Text = AppLogic.GetString(h.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                            RadioButton r = c as RadioButton;
                            if (r != null)
                            {
                                r.Text = AppLogic.GetString(r.Text.Replace("(!SKINID!)", SkinID.ToString()).Replace("(!", "").Replace("!)", ""), SkinID, ThisCustomer.LocaleSetting);
                            }
                        }
                    }
                }
                if (c.HasControls())
                {
                    foreach (Control cx in c.Controls)
                    {
                        FindLocaleStrings(cx);
                    }
                }
            }
            catch { } // for admin site, a hack really, will fix with master pages
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                m_ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;

                if (AppLogic.AppConfigBool("GoogleCheckout.ShowOnCartPage"))
                {
                    string s = CachingFactory.ApplicationCachingEngineInstance.GetItem<string>(DomainConstants.GCCallbackLoadCheck);
                    if (s != null)
                    {
                        string notused = CommonLogic.AspHTTP(AppLogic.GetStoreHTTPLocation(false) + "gccallback.aspx?loadcheck=1", 10);
                        CachingFactory.ApplicationCachingEngineInstance.AddItem(DomainConstants.GCCallbackLoadCheck, "true", 5);
                    }
                }

                if (!CurrentContext.IsInAdminRoot() && 
                    (AppLogic.AppConfigBool("SiteDisclaimerRequired") && 
                    CommonLogic.CookieCanBeDangerousContent("SiteDisclaimerAccepted", true).IsNullOrEmptyTrimmed()))
                {
                    string ThisPageURL = CommonLogic.GetThisPageName(true) + "?" + CommonLogic.ServerVariables("QUERY_STRING");
                    Response.Redirect("disclaimer.aspx?returnURL=" + Server.UrlEncode(ThisPageURL));
                }

                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ThisCustomer.LocaleSetting);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ThisCustomer.LocaleSetting);
                LoadSkinTemplate();
                m_Parser = new Parser(m_EntityHelpers, m_SkinID, m_ThisCustomer);
                m_Parser.RenderHeader += this.OnRenderHeader;

                if (this.HasControls())
                {
                    foreach (Control c in this.Controls)
                    {
                        FindLocaleStrings(c);
                    }

                    Control ctl;
                    int i = 1;
                    int limitLoop = 1000;
                    if (m_Template != null && m_Template.Content != null)
                    {
                        while (this.Controls.Count > 0 && i <= limitLoop)
                        {
                            bool FilterItOut = false;
                            ctl = this.Controls[0];
                            LiteralControl l = ctl as LiteralControl;
                            if (l != null)
                            {
                                string txtVal = l.Text;
                                if (txtVal.IndexOf("<html", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                                    txtVal.IndexOf("</html", StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    FilterItOut = true; // remove outer html/body crap, as we're going to be moving the page controls INSIDE The skin
                                }
                            }
                            if (!FilterItOut)
                            {
                                // reparent the page control to be moved inside the skin template user control
                                m_Template.Content.Controls.Add(ctl);
                            }
                            else
                            {
                                this.Controls.RemoveAt(0);
                            }
                            i++;
                        }
                    }

                    // clear the controls (they were now all moved inside the template user control:
                    this.Controls.Clear();
                    // set the template user control to be owned by this page:
                    this.Controls.Add(m_Template);

                    //register the ScriptManager before loading controls or the ComponentArt menu won't work with AJAX pages
                    CheckIfRequireScriptManager();

                    // Now move the template child controls up to the page level so the ViewState will load 
                    while (m_Template.Controls.Count > 0)
                    {
                        this.Controls.Add(m_Template.Controls[0]);
                    }
                }

                if (AppLogic.IsCBNMode() && m_ThisCustomer != null)
                {
                    var cart = new ShoppingCart(m_ThisCustomer.SkinID, m_ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false);
                    if (!cart.IsEmpty())
                    {
                        //empty shopping cart
                        cart.ClearContents();
                    }
                }

                string bingAdsTrackingScript = AppLogic.GetBingAdsTrackingScript();

                if (!bingAdsTrackingScript.IsNullOrEmptyTrimmed())
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), DB.GetNewGUID(), bingAdsTrackingScript, false);
                }


            }

            base.OnPreInit(e);
        }

        protected virtual void OnRenderHeader(object sender, TextWriter writer) { }

        #region FindForm
        /// <summary>
        /// Finds the instance of the HtmlForm in the page
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private HtmlForm FindForm(Control ctrl)
        {
            if (ctrl is HtmlForm)
            {
                return ctrl as HtmlForm;
            }

            var pagePlaceHolder = ctrl.FindControl("PageContent");
            if (pagePlaceHolder != null)
            {
                var htmlForms = pagePlaceHolder.Controls.OfType<HtmlForm>()
                                                        .AsParallel();
                if (htmlForms.Count() > 0)
                {
                    return htmlForms.First();
                }
            }

            foreach (Control childControl in ctrl.Controls)
            {
                HtmlForm frm = FindForm(childControl);
                if (null != frm)
                {
                    return frm;
                }
            }

            return null;
        }
        #endregion

        #region GetHTML
        /// <summary>
        /// Gets the rendered HTML of a user control
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static string GetHTML(Control ctrl)
        {
            var output = new StringBuilder();
            using (var sw = new StringWriter(output))
            {
                using (var hw = new HtmlTextWriter(sw))
                {
                    ctrl.RenderControl(hw);
                }
            }

            return output.ToString();
        }
        #endregion

        #region RequireScriptManager
        /// <summary>
        /// When overridden in derived pages and return true, will register the asp.net ajax core javascript
        /// </summary>
        protected virtual bool RequireScriptManager
        {
            get { return true; }
        }

        #endregion

        #region EnablePartialRendering
        /// <summary>
        /// When overridden in derived pages and return true, will register the asp.net ajax Sys.WebForms namespace
        /// </summary>
        protected virtual bool EnablePartialRendering
        {
            get { return false; }
        }
        #endregion

        #region EnableScriptGlobalization
        /// <summary>
        /// When overridden in derived pages and return true, will register the asp.net ajax localized javascript
        /// </summary>
        protected virtual bool EnableScriptGlobalization
        {
            get { return false; }
        }
        #endregion

        #region RegisterScriptsAndServices
        /// <summary>
        /// When overridden in derived pages, will provide means to register web service for asp.net ajax
        /// </summary>
        /// <param name="manager"></param>
        protected virtual void RegisterScriptsAndServices(ScriptManager manager) { }

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                //register the ScriptManager before loading controls or the ComponentArt menu won't work with AJAX pages
                CheckIfRequireScriptManager();

                if (!this.HasControls()) //Probably a web form so use the control management technique
                {
                    if (m_Template.Content != null)
                    {
                        //No controls so html must come from RenderContents. Create a literal to contain RenderContents
                        m_Template.Content.Controls.Add(ParseControl(CreateContent()));
                    }
                    this.Controls.Clear();
                    this.Controls.Add(m_Template);
                    // Now move the template child controls up to the page level so the ViewState will load 
                    while (m_Template.Controls.Count > 0)
                    {
                        this.Controls.Add(m_Template.Controls[0]);
                    }
                }

                SetupMenu();
            }

            base.OnPreRender(e);
        }

        #region CheckIfRequireScriptManager
        /// <summary>
        /// Checks if the inheriting page requires a script manager declared and creates the necessary scripts
        /// </summary>
        private void CheckIfRequireScriptManager()
        {

            if (!this.RequireScriptManager) return;
            
            HtmlForm frm = ThisForm;
            if (frm != null && frm.FindControl("ScriptManager") == null)
            {
                var pnlScriptManager = new Panel { ID = "pnlScriptManager" };
                var manager = new ScriptManager
                {
                    ID = "ScriptManager",
                    EnablePartialRendering = this.EnablePartialRendering,
                    EnableScriptGlobalization = this.EnableScriptGlobalization,
                    LoadScriptsBeforeUI = false,
                    ScriptMode = ScriptMode.Release
                };

                if (CurrentContext.IsRequestingFromMobileMode(ThisCustomer))
                {
                    manager.Scripts.Add(new ScriptReference("~/mobile/js/base_ajax.js"));
                    manager.Scripts.Add(new ScriptReference("~/mobile/js/system/config-loader.js"));
                }
                else
                {
                    manager.Scripts.Add(new ScriptReference("~/jscripts/base_ajax.js"));
                    manager.Scripts.Add(new ScriptReference("~/jscripts/system/config-loader.js"));

                    //load here the tiny mce when editing mode for the topic
                    //since it cannot be loaded using getscript of jquery

                    if (ThisCustomer.IsInEditingMode() && Security.IsAdminCurrentlyLoggedIn())
                    {
                        manager.Scripts.Add(new ScriptReference("~/jscripts/tiny_mce/tiny_mce.js"));
                    }

                }

                frm.Controls.AddAt(0, manager);

                // allow page to register scripts and web services
                RegisterScriptsAndServices(manager);
            }

        }

        #endregion

        #region SetupMenu
        /// <summary>
        /// Set's up the Asp.net default Menu control for display inside our skinning engine
        /// </summary>
        private void SetupMenu()
        {
            //Disregard the menu when using mobile browser.
            if (CurrentContext.IsRequestingFromMobileMode(ThisCustomer)) return;

            var menu_container = this.FindControl("menu_container") as HtmlContainerControl;
            if (menu_container == null) return;

            menu_container.InnerHtml = new InterpriseSuiteEcommerceCommon.Tool.MenuManager(
                                            ThisCustomer.SkinID,
                                            ThisCustomer.LocaleSetting,
                                            InterpriseHelper.ConfigInstance.WebSiteCode,
                                            AppLogic.AppConfigBool("CacheMenus"),
                                            CachingOption.CacheOnHTTPRuntime).GenerateMenu();
        }
        #endregion

        #region FindParentIndex
        /// <summary>
        /// Gets the index of this control on it's parent's childcontrols
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private int FindParentIndex(Control ctrl)
        {
            Control parent = ctrl.Parent;
            if (parent != null)
            {
                for (int index = 0; index < parent.Controls.Count; index++)
                {
                    Control current = parent.Controls[index];
                    if (current == ctrl)
                    {
                        return index;
                    }
                }
            }

            return 0;
        }
        #endregion

        #region ThisForm
        /// <summary>
        /// Gets the HTMLForm for this page
        /// </summary>
        public HtmlForm ThisForm
        {
            get
            {
                EnsureForm();
                return _actualForm;
            }
        }
        #endregion

        #region EnsureForm
        /// <summary>
        /// Makes sure that we have an HTMLForm instance on this page
        /// </summary>
        private void EnsureForm()
        {
            if (null == _actualForm)
            {
                // Ensure the form is existing...
                _actualForm = FindForm(this);
                if (null == _actualForm)
                {
                    var pnlForm = FindControl("pnlForm") as Panel;
                    if (null == pnlForm)
                    {
                        pnlForm = m_Template.FindControl("pnlForm") as Panel;
                    }

                    if (null != pnlForm)
                    {
                        _actualForm = new HtmlForm();
                        _actualForm.ID = "Form";
                        pnlForm.Controls.Add(_actualForm);
                        pnlForm.Visible = true;
                        pnlForm.Style["display"] = "none";
                    }
                }
            }
        }
        #endregion

        #region FindMenu
        /// <summary>
        /// Finds all the instances of ComponentArt Menu control
        /// </summary>
        /// <param name="context">The control context</param>
        /// <param name="foundMenus">The collection to populate found controls</param>
        private void FindMenu(Control context, List<WebControl> foundMenus)
        {
            foreach (Control ctrl in context.Controls)
            {
                if (ctrl is System.Web.UI.WebControls.Menu) // || ctrl is ComponentArt.Web.UI.Menu)
                {
                    foundMenus.Add(ctrl as WebControl);
                }
                else
                {
                    // recurse   
                    FindMenu(ctrl, foundMenus);
                }
            }
        }
        #endregion

        public string CreateContent()
        {
            var tmpS = new StringBuilder(25000);
            var sw = new StringWriter();
            var htw = this.CreateHtmlTextWriter(sw);

            RenderContents(htw);
            htw.Flush();
            tmpS.Append(sw.ToString());

            htw.Close();
            sw.Dispose();

            return tmpS.ToString();
        }

        // replace any localization strings in the controls:
        private void IterateControls(ControlCollection controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                var c = controls[i];
                if (c.ID == "PageContent") continue;
                
                ProcessControl(c, true);
            }
        }

        private void ProcessControl(Control ctl, bool includeChildren)
        {
            var e = ctl as IEditableTextControl;

            if (e != null) { e.Text = ReplaceTokens(e.Text); }
            else
            {
                var t = ctl as ITextControl;
                if (t != null) t.Text = ReplaceTokens(t.Text);
            }

            var v = ctl as IValidator;
            if (v != null)
            {
                v.ErrorMessage = ReplaceTokens(v.ErrorMessage);
            }

            if (includeChildren && ctl.HasControls())
            {
                IterateControls(ctl.Controls);
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            if (m_SETitle.Length == 0)
            {
                m_SETitle = AppLogic.AppConfig("SE_MetaTitle");
            }
            if (m_SEDescription.Length == 0)
            {
                m_SEDescription = AppLogic.AppConfig("SE_MetaDescription");
            }
            if (m_SEKeywords.Length == 0)
            {
                m_SEKeywords = AppLogic.AppConfig("SE_MetaKeywords");
            }
            if (m_SENoScript.Length == 0)
            {
                m_SENoScript = AppLogic.AppConfig("SE_MetaNoScript");
            }
            IterateControls(Controls);
            base.Render(writer);
        }

        public void SetTemplate(string TemplateName)
        {
            if (TemplateName.Trim().Length == 0) { return; }
            
            if (TemplateName.EndsWith(".ascx", StringComparison.InvariantCultureIgnoreCase))
            {
                m_TemplateName = TemplateName;
            }
            else
            {
                throw new ArgumentException("Skin template files (" + TemplateName + ") must end with .ascx for InterpriseSuiteEcommerce versions v5.x+!");
            }
        }

        public void LoadSkinTemplate()
        {
            SkinID = 1;
            if (m_IGD.Length != 0)
            {
                m_TemplateName = "empty.ascx"; // force override for admin phone order pages
            }
            if (m_TemplateName.Length == 0)
            {
                m_TemplateName = "template.ascx";
            }
            m_TemplateFN = string.Empty;
            if (m_TemplateName.Length != 0)
            {
                SkinID = CommonLogic.QueryStringUSInt("SkinID");

                if (SkinID == 0 && CommonLogic.QueryStringCanBeDangerousContent("AffiliateID").Length != 0)
                {
                    DataSet ds = DB.GetDS("Select DefaultSkinID from CustomerSalesRep with (NOLOCK) where SalesRepGroupCode=" + DB.SQuote(CommonLogic.QueryStringCanBeDangerousContent("AffiliateID")), AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        SkinID = DB.RowFieldInt(ds.Tables[0].Rows[0], "DefaultSkinID");
                    }
                    ds.Dispose();
                }
                if (SkinID == 0)
                {
                    SkinID = m_DefaultSkinID;
                }

                if (SkinID == 0)
                {
                    SkinID = CommonLogic.CookieUSInt(ro_SkinCookieName);
                }

                if (SkinID == 0)
                {
                    SkinID = 1;
                }

                AppLogic.SetCookie(ro_SkinCookieName, SkinID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                m_ThisCustomer.SkinID = SkinID;


                string LocaleTemplateURLCacheName = string.Format("template_{0}_{1}_{1}", m_TemplateName, SkinID.ToString(), ThisCustomer.LocaleSetting);
                string WebLocaleTemplateURLCacheName = string.Format("template_{0}_{1}_{1}", m_TemplateName, SkinID.ToString(), Localization.WebConfigLocale);
                string TemplateURLCacheName = string.Format("template_{0}_{1}_{1}", m_TemplateName, SkinID.ToString(), "");

                // try customer locale:

                string webLocale = Localization.WebConfigLocale;
                string localeSetting = ThisCustomer.LocaleSetting;
                DateTime mins = System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes());

                string _url = Path.Combine(SkinRoot, m_TemplateName.Replace(".ascx", "." + localeSetting + ".ascx"));
                m_TemplateFN = CommonLogic.SafeMapPath(_url);

                if (!CommonLogic.FileExists(m_TemplateFN))
                {
                    // try default store locale path:
                    _url = Path.Combine(SkinRoot, m_TemplateName.Replace(".ascx", "." + webLocale + ".ascx"));
                    m_TemplateFN = CommonLogic.SafeMapPath(_url);
                }

                if (!CommonLogic.FileExists(m_TemplateFN))
                {
                    _url = Path.Combine(SkinRoot, m_TemplateName);
                    m_TemplateFN = CommonLogic.SafeMapPath(_url);
                }

                if (AppLogic.CachingOn)
                {
                    HttpContext.Current.Cache.Insert(TemplateURLCacheName, _url, null, mins, TimeSpan.Zero);
                }

                if (_url != null && _url != string.Empty && !CommonLogic.FileExists(_url))
                {
                    SkinID = 1;

                    AppLogic.SetCookie(ro_SkinCookieName, SkinID.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                    m_ThisCustomer.SkinID = SkinID;

                    // try customer locale:
                    _url = Path.Combine(SkinRoot, m_TemplateName.Replace(".ascx", "." + localeSetting + ".ascx"));

                    m_TemplateFN = _url.ToMapPath();
                    if (!CommonLogic.FileExists(m_TemplateFN))
                    {
                        // try default store locale path:
                        _url = Path.Combine(SkinRoot, m_TemplateName.Replace(".ascx", "." + webLocale + ".ascx"));
                        m_TemplateFN = _url.ToMapPath();
                    }
                    if (!CommonLogic.FileExists(m_TemplateFN))
                    {
                        // try base (NULL) locale path:
                        _url = Path.Combine(SkinRoot, m_TemplateName);
                        m_TemplateFN = _url.ToMapPath();
                    }
                    if (AppLogic.CachingOn)
                    {
                        HttpContext.Current.Cache.Insert(TemplateURLCacheName, _url, null, mins, TimeSpan.Zero);
                    }
                }
                try
                {
                    m_Template = (TemplateBase)LoadControl(_url);
                }
                catch { } // if defined template not FOUND...

            }
            if (m_Template != null)
            {
                m_Template.AppRelativeTemplateSourceDirectory = "~/"; // move it from skins/skin_N to root relative, so all links/image refs are from root of site
            }
        }

        private string ReplaceTokens(string s)
        {
            if (s.IndexOf("(!") == -1)
            {
                return s;
            }
            string tmpS = string.Empty;
            // process SKIN specific tokens here only:
            s = s.Replace("(!SECTION_TITLE!)", SectionTitle);
            if (SectionTitle.Length != 0)
            {
                s = s.Replace("(!SUPERSECTIONTITLE!)", "<div align=\"left\"><span class=\"SectionTitleText\">" + SectionTitle + "</span><br/><small>&nbsp;</small></div>");
            }
            else
            {
                s = s.Replace("(!SUPERSECTIONTITLE!)", "");
            }

            s = s.Replace("(!METATITLE!)", CommonLogic.IIF(CommonLogic.StringIsAlreadyHTMLEncoded(m_SETitle), m_SETitle, m_SETitle.ToHtmlEncode()));
            s = s.Replace("(!METADESCRIPTION!)", CommonLogic.IIF(CommonLogic.StringIsAlreadyHTMLEncoded(m_SEDescription), m_SEDescription, m_SEDescription.ToHtmlEncode()));
            s = s.Replace("(!METAKEYWORDS!)", CommonLogic.IIF(CommonLogic.StringIsAlreadyHTMLEncoded(m_SEKeywords), m_SEKeywords, m_SEKeywords.ToHtmlEncode()));
            s = s.Replace("(!SENOSCRIPT!)", m_SENoScript);
            
            s = GetParser.ReplaceTokens(s);
            return s;
        }

        protected virtual void RenderContents(System.Web.UI.HtmlTextWriter writer) { }

        public Customer ThisCustomer
        {
            get
            {
                return m_ThisCustomer;
            }
        }

        public Parser GetParser
        {
            get
            {
                return m_Parser;
            }
        }

        public string SectionTitle
        {
            get
            {
                return m_SectionTitle;
            }
            set
            {
                m_SectionTitle = value;
            }
        }

        public string ErrorMsg
        {
            get
            {
                return m_ErrorMsg;
            }
            set
            {
                m_ErrorMsg = value;
            }
        }

        public string SETitle
        {
            get
            {
                return m_SETitle;
            }
            set
            {
                m_SETitle = value;
            }
        }

        public string IGD
        {
            get
            {
                return m_IGD;
            }
        }

        public string SEKeywords
        {
            get
            {
                return m_SEKeywords;
            }
            set
            {
                m_SEKeywords = value;
            }
        }

        public string SEDescription
        {
            get
            {
                return m_SEDescription;
            }
            set
            {
                m_SEDescription = value;
            }
        }

        public string SENoScript
        {
            get
            {
                return m_SENoScript;
            }
            set
            {
                m_SENoScript = value;
            }
        }

        public bool Editing
        {
            get
            {
                return m_Editing;
            }
            set
            {
                m_Editing = value;
            }
        }

        public bool DisableContents
        {
            get
            {
                return m_DisableContents;
            }
            set
            {
                m_DisableContents = value;
            }
        }

        public bool DataUpdated
        {
            get
            {
                return m_DataUpdated;
            }
            set
            {
                m_DataUpdated = value;
            }
        }

        new public int SkinID
        {
            get
            {
                return m_SkinID;
            }
            set
            {
                m_SkinID = value;
            }
        }

        public string SkinRoot
        {
            get
            {
                return string.Format("skins/skin_{0}/", this.SkinID);
            }
        }

        public Dictionary<string, EntityHelper> EntityHelpers
        {
            get
            {
                return m_EntityHelpers;
            }
        }

        public string SkinImages
        {
            get
            {
                return string.Format("skins/skin_{0}/images/", this.SkinID);
            }
        }

        public static void RequireSecurePage()
        {
            AppLogic.RequireSecurePage();
        }

        public static void GoNonSecureAgain()
        {
            if (!AppLogic.UseSSL()) return;

            if (AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") == "1")
            {
                string nonSecureUrl = String.Empty;
                if (HttpContext.Current.Request.RawUrl.IndexOf("/") != -1)
                {
                    nonSecureUrl = AppLogic.GetStoreHTTPLocation(false) + HttpContext.Current.Request.RawUrl.Substring(HttpContext.Current.Request.RawUrl.LastIndexOf("/") + 1);
                }
                else
                {
                    nonSecureUrl = AppLogic.GetStoreHTTPLocation(false) + HttpContext.Current.Request.RawUrl;
                }
                HttpContext.Current.Response.Redirect(nonSecureUrl);
            }
        }

        public void RequireCustomerRecord()
        {
            if (!m_ThisCustomer.HasCustomerRecord)
            {
                m_ThisCustomer.RequireCustomerRecord();
            }
        }

        public void RequiresLogin(string ReturnURL)
        {
            if (!m_ThisCustomer.IsRegistered)
            {
                Response.Redirect("signin.aspx?returnurl=" + Server.UrlEncode(ReturnURL));
            }
        }

        public void SetMetaTags(string SETitle, string SEKeywords, string SEDescription, string SENoScript)
        {
            m_SETitle = SETitle;
            m_SEDescription = SEKeywords;
            m_SEKeywords = SEDescription;
            m_SENoScript = SENoScript;
        }

        public void PageNoCache()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
        }

    }
}
