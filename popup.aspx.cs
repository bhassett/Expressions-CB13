// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for popup.
    /// </summary>
    public partial class popup : System.Web.UI.Page
    {

        protected override void OnPreInit(EventArgs e)
        {

            SkinID = CommonLogic.CookieUSInt(SkinBase.ro_SkinCookieName).ToString();

            RegisterScriptManager();
            RegisterStyles();

        }

        private void RegisterStyles()
        {
            //var css = new HtmlLink();
            //css.Href = string.Format("skins/Skin_{0}/style.css", SkinID);
            //css.Attributes["rel"] = "stylesheet";
            //css.Attributes["type"] = "text/css";
            //css.Attributes["media"] = "all";
            //hdrMain.Controls.Add(css);

            var cssUiLightness = new HtmlLink();
            cssUiLightness.Href = string.Format("skins/Skin_{0}/ui-lightness/jquery-ui-1.8.16.custom.css", SkinID);
            cssUiLightness.Attributes["rel"] = "stylesheet";
            cssUiLightness.Attributes["type"] = "text/css";
            cssUiLightness.Attributes["media"] = "all";
            hdrMain.Controls.Add(cssUiLightness);
        }

        private void RegisterScriptManager()
        {
            var ThisCustomer = Customer.Current;

            //add the scripts if rendering a topic and under edit mode
            if (ThisCustomer.IsInEditingMode() && !CommonLogic.QueryStringCanBeDangerousContent("Topic").IsNullOrEmptyTrimmed())
            {
                var jsCore = new HtmlGenericControl("script");
                jsCore.Attributes["type"] = "text/javascript";
                jsCore.Attributes["src"] = "jscripts/core.js";
                hdrMain.Controls.Add(jsCore);

                var jsJqueryMin = new HtmlGenericControl("script");
                jsJqueryMin.Attributes["type"] = "text/javascript";
                jsJqueryMin.Attributes["src"] = "jscripts/jquery/jquery.min.v1.7.2.js";
                hdrMain.Controls.Add(jsJqueryMin);

                var jsQueryUiMin = new HtmlGenericControl("script");
                jsQueryUiMin.Attributes["type"] = "text/javascript";
                jsQueryUiMin.Attributes["src"] = "jscripts/jquery/jquery-ui-1.8.16.custom.min.js";
                hdrMain.Controls.Add(jsQueryUiMin);

                var jsJQueryTemplateMin = new HtmlGenericControl("script");
                jsJQueryTemplateMin.Attributes["type"] = "text/javascript";
                jsJQueryTemplateMin.Attributes["src"] = "jscripts/jquery/jquery.tmpl.min.js";
                hdrMain.Controls.Add(jsJQueryTemplateMin);

                //var jsCmsEditor = new HtmlGenericControl("script");
                //jsCmsEditor.Attributes["type"] = "text/javascript";
                //jsCmsEditor.Attributes["src"] = "jscripts/cms_editor/cms-editor.js";
                //hdrMain.Controls.Add(jsCmsEditor);

                var jsCmsEditorPlugIn = new HtmlGenericControl("script");
                jsCmsEditorPlugIn.Attributes["type"] = "text/javascript";
                jsCmsEditorPlugIn.Attributes["src"] = "jscripts/cms_editor/jquery.cmseditor-plugin.js";
                hdrMain.Controls.Add(jsCmsEditorPlugIn);

                var jsCmsEditorPlugInTemplate = new HtmlGenericControl("script");
                jsCmsEditorPlugInTemplate.Attributes["type"] = "text/javascript";
                jsCmsEditorPlugInTemplate.Attributes["src"] = "jscripts/cms_editor/jquery.cmseditor-plugin-template.js";
                hdrMain.Controls.Add(jsCmsEditorPlugInTemplate);

                var jsTinyMce = new HtmlGenericControl("script");
                jsTinyMce.Attributes["type"] = "text/javascript";
                jsTinyMce.Attributes["src"] = "jscripts/tiny_mce/tiny_mce.js";
                hdrMain.Controls.Add(jsTinyMce);

            }
            else
            {
                //remove script manager if not editing mode and not topic
                frmForm.Controls.Remove(mgrScriptManager);
            }

        }

        private void InitTopicEditorScript()
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), DateTime.Now.Ticks.ToString(), "InitTopicEditorScript()", true);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack) return;

            var ThisCustomer = Customer.Current;

            //add the scripts if rendering a topic and under edit mode
            if (ThisCustomer.IsInEditingMode() && !CommonLogic.QueryStringCanBeDangerousContent("Topic").IsNullOrEmptyTrimmed())
            {
                InitTopicEditorScript();
            }

            base.OnLoad(e);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {

            NoCache();

            string PageTitle = CommonLogic.QueryStringCanBeDangerousContent("Title");
            if (PageTitle.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            if (PageTitle.Length == 0)
            {
                PageTitle = "Popup Window " + CommonLogic.GetRandomNumber(1, 1000000).ToString();
            }

            var ThisCustomer = Customer.Current;

            this.Title = PageTitle;

            if (CommonLogic.QueryStringCanBeDangerousContent("psrc").Length != 0)
            {
                // IMAGE POPUP:
                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" " + CommonLogic.IIF(AppLogic.AppConfigBool("OnBlurPopups"), "ONBLUR=\"self.close();\"", "") + " onClick=\"self.close();\" onLoad=\"self.focus()\">\n");
                Response.Write("<center>\n");
                string url = string.Empty;

                url += CommonLogic.QueryStringCanBeDangerousContent("psrc");

                if (url.ToLowerInvariant() == "watermark.axd?e=0")
                    url = HttpContext.Current.Request.Url.ToString().Replace("popup.aspx?psrc=", "");

                Response.Write("<img name=\"Image1\" onClick=\"javascript:self.close();\" style=\"cursor:hand;cursor:pointer;\" alt=\"" + AppLogic.GetString("popup.aspx.1", 1, Customer.Current.LocaleSetting, true) + "\" border=\"0\" src=\"" + url + "\">\n");
                Response.Write("<br/>");
            }
            else if (CommonLogic.QueryStringCanBeDangerousContent("src").Length != 0)
            {

                // IMAGE POPUP:
                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" " + CommonLogic.IIF(AppLogic.AppConfigBool("OnBlurPopups"), "ONBLUR=\"self.close();\"", "") + " onClick=\"self.close();\" onLoad=\"self.focus()\">\n");
                Response.Write("<center>\n");
                Response.Write("<img name=\"Image1\" onClick=\"javascript:self.close();\" style=\"cursor:hand;cursor:pointer;\" alt=\"" + AppLogic.GetString("popup.aspx.1", 1, Customer.Current.LocaleSetting, true) + "\" border=\"0\" src=\"" + Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("src")) + "\">\n");
                Response.Write("<br/>");

            }
            else if (CommonLogic.QueryStringCanBeDangerousContent("orderoptionid").Length != 0)
            {
                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");

                int itemCounter = CommonLogic.QueryStringUSInt("orderoptionid");

                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var reader = DB.GetRSFormat(con, "SELECT i.Counter, i.ItemName, iid.ItemDescription, iiwod.WebDescription FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryItemDescription iid with (NOLOCK) ON i.ItemCode = iid.ItemCode INNER JOIN InventoryItemWebOptionDescription iiwod with (NOLOCK) ON iiwod.ItemCode = i.ItemCode WHERE i.Counter = {0} AND iid.LanguageCode = {1} AND iiwod.LanguageCode = {1} AND iiwod.WebsiteCode = {2}", itemCounter, DB.SQuote(ThisCustomer.LanguageCode), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)))
                    {
                        if (reader.Read())
                        {
                            string itemName = DB.RSField(reader, "ItemDescription");
                            if (CommonLogic.IsStringNullOrEmpty(itemName))
                            {
                                itemName = DB.RSField(reader, "ItemName");
                            }

                            string itemDescription = DB.RSField(reader, "WebDescription");
                            if (CommonLogic.IsStringNullOrEmpty(itemDescription))
                            {
                                itemDescription = Security.HtmlEncode(DB.RSField(reader, "ItemDescription"));
                            }

                            Response.Write("<p align=\"left\"><b>" + Security.HtmlEncode(itemName) + "</b>:</p>");
                            Response.Write("<p align=\"left\">" + itemDescription + "</p>");
                        }
                        else
                        {
                            Response.Write("<p align=\"left\"><b><font color=red>" + AppLogic.GetString("popup.aspx.2", 1, ThisCustomer.LocaleSetting) + "</font></b>:</p>");
                        }
                    }
                }

            }
            else if (CommonLogic.QueryStringCanBeDangerousContent("kitgroupid").Length != 0)
            {
                // kit group info popoup:
                string kitGroupId = CommonLogic.QueryStringCanBeDangerousContent("kitgroupid");
                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");

                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var rs = DB.GetRSFormat(con, "Select * from InventoryKitOptionGroupDescription  with (NOLOCK) where GroupCode = " + DB.SQuote(kitGroupId.ToString()) + "and LanguageCode = " + DB.SQuote(ThisCustomer.LanguageCode.ToString())))
                    {
                        if (rs.Read())
                        {
                            Response.Write("<p align=\"left\"><b>" + DB.RSFieldByLocale(rs, "GroupCode", ThisCustomer.LocaleSetting) + "</b>:</p>");
                            Response.Write("<p align=\"left\">" + DB.RSFieldByLocale(rs, "Description", ThisCustomer.LocaleSetting) + "</p>");
                        }
                        else
                        {
                            Response.Write("<p align=\"left\"><b><font color=red>" + AppLogic.GetString("popup.aspx.3", 1, Customer.Current.LocaleSetting) + "</font></b>:</p>");
                        }
                    }
                }
            }
            else if (CommonLogic.QueryStringCanBeDangerousContent("KitItemID").Length != 0)
            {
                // kit group info popoup:
                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");

                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader rs3 = DB.GetRSFormat(con, "Select * from kititem  with (NOLOCK) where KitItemID=" + CommonLogic.QueryStringUSInt("KitItemID").ToString()))
                    {
                        if (rs3.Read())
                        {
                            Response.Write("<p align=\"left\"><b>" + DB.RSFieldByLocale(rs3, "Name", ThisCustomer.LocaleSetting) + "</b>:</p>");
                            Response.Write("<p align=\"left\">" + DB.RSFieldByLocale(rs3, "Description", ThisCustomer.LocaleSetting) + "</p>");
                        }
                        else
                        {
                            Response.Write("<p align=\"left\"><b><font color=red>" + AppLogic.GetString("popup.aspx.4", 1, Customer.Current.LocaleSetting) + "</font></b>:</p>");
                        }
                    }
                }
            }
            else
            {

                RenderTopic();
                return;
            }

            Response.Write("</body>\n");
            Response.Write("</html>\n");
        }

        private void RenderTopic()
        {
            // CONTENT POPUP:

            var thisCustomer = Customer.Current;

            var thisTopic = new Topic(CommonLogic.QueryStringCanBeDangerousContent("Topic"), thisCustomer.LocaleSetting, thisCustomer.SkinID, null);
            var thisParser = new Parser(AppLogic.GetCurrentSkinID(), thisCustomer);

            if (thisTopic.Contents.Length == 0)
            {
                Response.Write("<br/><br/>\n");
                Response.Write("<p align=\"center\"><font class=\"big\"><b>" + AppLogic.GetString("popup.aspx.5", thisCustomer.SkinID, Customer.Current.LocaleSetting) + "</b></font></p>");
            }
            else
            {
                string contents = thisTopic.Contents.Replace("(!SKINID!)", thisCustomer.SkinID.ToString());
                string parsing = ((thisTopic.FromDB) ? "DB" : "FILE: " + thisTopic.FN);
                litTopicContent.Text = thisParser.ReplaceTokens(contents);

            }

        }

        private void NoCache()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
        }

        public string SkinID
        {
            get;
            set;
        }
    }

}
