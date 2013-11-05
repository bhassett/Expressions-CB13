// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce.mobile
{

    public partial class TopicControl : System.Web.UI.UserControl
    {
        private string m_TackageName = string.Empty;
        private string m_RuntimeParams = string.Empty;
        private Customer m_ThisCustomer = null;
        private Topic m_T = null;
        private bool m_DesignMode = false;

        // these are set to false, to make "most" page control invocations shorter to create (as "most" instances don't care about these!)
        private bool m_EnforcePassword = false;
        private bool m_EnforceSubscription = false;
        private bool m_EnforceDisclaimer = false;
        private bool m_AllowSEPropogation = false;

        private int m_SkinID = 1;
        private string m_LocaleSetting = string.Empty;

        private SkinBase m_SkinBase = null; // if not null, this control will set the page metatags to the results from the Topic, IF those Topic results are not "empty strings"

        protected void Page_Load(object sender, EventArgs e)
        {
            m_DesignMode = (HttpContext.Current == null);
            
            if (m_DesignMode)
            {
                if (TopicName.Length != 0)
                {
                    Contents.Text = "Topic: " + TopicName;
                }
                else
                {
                    Contents.Text = "Topic";
                }
            }
            else
            {
                try
                {
                    if (Page != null)
                    {
                        m_T = new Topic(TopicName.Replace("-"," "), ThisCustomer.LocaleSetting, ThisCustomer.SkinID, Page.GetParser);
                        m_SkinID = ThisCustomer.SkinID;
                        m_LocaleSetting = ThisCustomer.LocaleSetting;
                    }
                    else
                    {
                        m_LocaleSetting = Localization.WebConfigLocale;
                        m_T = new Topic(TopicName.Replace("-", " "), m_LocaleSetting, m_SkinID, null);
                    }

                    if (m_T.ShowOnWeb == false && (Request.Url.LocalPath.Contains("driver.aspx") || Request.Url.LocalPath.Contains("driver2.aspx")))
                    {
                        //If topic does not exists redirect to 404 error page.
                        HttpContext.Current.Response.Redirect("~/t-error404.aspx");
                    }

                    StringBuilder tmpS = new StringBuilder(4096);
                    string password = string.Empty;
                    if (m_T.Password.Length != 0)
                    {
                        password = InterpriseHelper.TopicPassword(m_T.TopicID, m_LocaleSetting);
                    }

                    string xpdd = m_SkinBase.ThisCustomer.ThisCustomerSession["Topic" + XmlCommon.GetLocaleEntry(m_T.TopicName, m_SkinBase.ThisCustomer.LocaleSetting, true)];
                    if (EnforcePassword && m_T.Password.Length != 0 && xpdd != password)
                    {
                        string Url = string.Empty;

                        bool isDriverEquals = "driver.aspx".Equals(CommonLogic.GetThisPageName(false), StringComparison.InvariantCultureIgnoreCase);
                        Url = CommonLogic.IIF(isDriverEquals , SE.MakeDriverLink(m_T.TopicName), SE.MakeDriver2Link(m_T.TopicName));

                        tmpS.Append("<form method=\"POST\" action=\"" + Url + "\">\n");
                        tmpS.Append("<p><b>");
                        tmpS.Append(AppLogic.GetString("driver.aspx.1", m_SkinID, m_LocaleSetting));
                        tmpS.Append("</b></p>\n");
                        tmpS.Append("<p>");
                        tmpS.Append(AppLogic.GetString("driver.aspx.2", m_SkinID, m_LocaleSetting));
                        tmpS.Append(" <input type=\"password\" name=\"Password\" size=\"20\" maxlength=\"100\" TextMode=\"Password\"><input type=\"submit\" value=\"");
                        tmpS.Append(AppLogic.GetString("driver.aspx.4", m_SkinID, m_LocaleSetting));
                        tmpS.Append("\" name=\"B1\"></p>\n");
                        tmpS.Append("</form>\n");
                    }
                    else
                    {
                        if (EnforceDisclaimer && m_T.RequiresDisclaimer && CommonLogic.CookieCanBeDangerousContent("SiteDisclaimerAccepted", true).Length == 0)
                        {
                            string ThisPageURL = CommonLogic.GetThisPageName(true) + "?" + CommonLogic.ServerVariables("QUERY_STRING");
                            Response.Redirect("disclaimer.aspx?returnURL=" + Server.UrlEncode(ThisPageURL));
                        }

                        if (EnforceSubscription && m_T.RequiresSubscription && ThisCustomer.SubscriptionExpiresOn < System.DateTime.Now)
                        {
                            tmpS.Append("<p><b>" + AppLogic.GetString("driver.aspx.3", m_SkinID, m_LocaleSetting) + "</b></p>");
                        }
                        else
                        {
                            tmpS.Append("<!-- READ FROM ");
                            tmpS.Append(CommonLogic.IIF(m_T.FromDB, "DB", "FILE: " + m_T.FN));
                            tmpS.Append(" -->");
                            tmpS.Append(m_T.Contents);
                            tmpS.Append("<!-- END OF ");
                            tmpS.Append(CommonLogic.IIF(m_T.FromDB, "DB", "FILE: " + m_T.FN));
                            tmpS.Append(" -->");
                        }
                    }
                    Contents.Text = tmpS.ToString();
                }
                catch (Exception ex)
                {
                    Contents.Text = CommonLogic.GetExceptionDetail(ex, "<br/>");
                }
                if (Page != null && m_AllowSEPropogation)
                {
                    if (m_T.SectionTitle.Length != 0)
                    {
                        Page.SectionTitle = m_T.SectionTitle;
                    }
                    if (m_T.SETitle.Length != 0)
                    {
                        Page.SETitle = m_T.SETitle;
                    }
                    if (m_T.SEKeywords.Length != 0)
                    {
                        Page.SEKeywords = m_T.SEKeywords;
                    }
                    if (m_T.SEDescription.Length != 0)
                    {
                        Page.SEDescription = m_T.SEDescription;
                    }
                    if (m_T.SENoScript.Length != 0)
                    {
                        Page.SENoScript = m_T.SENoScript;
                    }
                }
            }
        }

        public SkinBase SetContext
        {
            set
            {
                m_SkinBase = value;
                m_ThisCustomer = m_SkinBase.ThisCustomer;
            }
        }

        public new SkinBase Page
        {
            get
            {
                return m_SkinBase;
            }
            set
            {
                m_SkinBase = value;
            }
        }

        public string TopicName
        {
            get
            {
                return m_TackageName;
            }
            set
            {
                m_TackageName = value;
            }
        }

        public string RuntimeParams
        {
            get
            {
                return m_RuntimeParams;
            }
            set
            {
                m_RuntimeParams = value;
            }
        }

        public Customer ThisCustomer
        {
            get
            {
                return m_ThisCustomer;
            }
            set
            {
                m_ThisCustomer = value;
            }
        }

        public string SectionTitle
        {
            get
            {
                return m_T.SectionTitle;
            }
        }

        public string Password
        {
            get
            {
                return m_T.Password;
            }
        }

        public bool RequiresSubscription
        {
            get
            {
                return m_T.RequiresSubscription;
            }
        }

        public bool RequiresDisclaimer
        {
            get
            {
                return m_T.RequiresDisclaimer;
            }
        }

        public string SETitle
        {
            get
            {
                return m_T.SETitle;
            }
        }

        public string SEKeywords
        {
            get
            {
                return m_T.SEKeywords;
            }
        }

        public string SEDescription
        {
            get
            {
                return m_T.SEDescription;
            }
        }

        public string SENoScript
        {
            get
            {
                return m_T.SENoScript;
            }
        }

        public bool EnforcePassword
        {
            get
            {
                return m_EnforcePassword;
            }
            set
            {
                m_EnforcePassword = value;
            }
        }

        public bool EnforceSubscription
        {
            get
            {
                return m_EnforceSubscription;
            }
            set
            {
                m_EnforceSubscription = value;
            }
        }

        public bool EnforceDisclaimer
        {
            get
            {
                return m_EnforceDisclaimer;
            }
            set
            {
                m_EnforceDisclaimer = value;
            }
        }

        public bool AllowSEPropogation
        {
            get
            {
                return m_AllowSEPropogation;
            }
            set
            {
                m_AllowSEPropogation = value;
            }
        }
    }
}