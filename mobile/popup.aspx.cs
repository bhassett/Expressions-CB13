// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for popup.
    /// </summary>
    public partial class popup : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("psrc").Length != 0)
            {
                InitPopUpPageResponse();

                // IMAGE POPUP:
                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" " + CommonLogic.IIF(AppLogic.AppConfigBool("OnBlurPopups"), "ONBLUR=\"self.close();\"", "") + " onClick=\"self.close();\" onLoad=\"self.focus()\">\n");
                Response.Write("<center>\n");                
                string url = string.Empty;

                url += CommonLogic.QueryStringCanBeDangerousContent("psrc");

                if (url.ToLowerInvariant() == "watermark.axd?e=0")
                    url = HttpContext.Current.Request.Url.ToString().Replace("popup.aspx?psrc=", "");
                
                Response.Write("<img name=\"Image1\" onClick=\"javascript:self.close();\" style=\"cursor:hand;cursor:pointer;\" alt=\"" + AppLogic.GetString("popup.aspx.1", 1, Customer.Current.LocaleSetting) + "\" border=\"0\" src=\"" + url + "\">\n");
                Response.Write("<br/>");
            }
            else if (CommonLogic.QueryStringCanBeDangerousContent("src").Length != 0)
            {
                InitPopUpPageResponse();
                // IMAGE POPUP:
                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" " + CommonLogic.IIF(AppLogic.AppConfigBool("OnBlurPopups"), "ONBLUR=\"self.close();\"", "") + " onClick=\"self.close();\" onLoad=\"self.focus()\">\n");
                Response.Write("<center>\n");
                Response.Write("<img name=\"Image1\" onClick=\"javascript:self.close();\" style=\"cursor:hand;cursor:pointer;\" alt=\"" + AppLogic.GetString("popup.aspx.1", 1, Customer.Current.LocaleSetting) + "\" border=\"0\" src=\"" + Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("src")) + "\">\n");
                Response.Write("<br/>");
            }
            else if (CommonLogic.QueryStringCanBeDangerousContent("orderoptionid").Length != 0)
            {
                InitPopUpPageResponse();

                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");
                
                int itemCounter = CommonLogic.QueryStringUSInt("orderoptionid");


                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader reader = DB.GetRSFormat(con, "SELECT i.Counter, i.ItemName, iid.ItemDescription, iiwod.WebDescription FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryItemDescription iid with (NOLOCK) ON i.ItemCode = iid.ItemCode INNER JOIN InventoryItemWebOptionDescription iiwod with (NOLOCK) ON iiwod.ItemCode = i.ItemCode WHERE i.Counter = {0} AND iid.LanguageCode = {1} AND iiwod.LanguageCode = {1} AND iiwod.WebsiteCode = {2}", itemCounter, DB.SQuote(ThisCustomer.LanguageCode), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)))
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
                InitPopUpPageResponse();
                // kit group info popoup:
                string kitGroupId = CommonLogic.QueryStringCanBeDangerousContent("kitgroupid");
                Response.Write("<body style=\"margin: 0px;\" bottommargin=\"0\" leftmargin=\"0\" marginheight=\"0\" marginwidth=\"0\" rightmargin=\"0\" topmargin=\"0\" bgcolor=\"#FFFFFF\" onLoad=\"self.focus()\">\n");

                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader rs = DB.GetRSFormat(con, "Select * from InventoryKitOptionGroupDescription  with (NOLOCK) where GroupCode = " + DB.SQuote(kitGroupId.ToString()) + "and LanguageCode = " + DB.SQuote(ThisCustomer.LanguageCode.ToString())))
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
                InitPopUpPageResponse();
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
                RenderTopics();
            }
        }

        private void InitPopUpPageResponse()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            string PageTitle = CommonLogic.QueryStringCanBeDangerousContent("Title");
            if (PageTitle.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            if (PageTitle.Length == 0)
            {
                PageTitle = "Popup Window " + CommonLogic.GetRandomNumber(1, 1000000).ToString();
            }

            var ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;

            Response.Write("<html>\n");
            Response.Write("<head>\n");
            Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
            Response.Write("<title>" + PageTitle + "</title>\n");
            Response.Write("<link rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/style.css\" type=\"text/css\">\n");
            Response.Write("</head>\n");
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        private void RenderTopics() 
        {
            var t = new Topic(CommonLogic.QueryStringCanBeDangerousContent("Topic"), ThisCustomer.LocaleSetting, ThisCustomer.SkinID, null);
            if (t.Contents.Length == 0)
            {
                pnlNoTopic.Visible = true;
                lblNoTopicText.Text = AppLogic.GetString("popup.aspx.5", 1, Customer.Current.LocaleSetting);
            }
            else
            {
                pnlNoTopic.Visible = true;
                lblTopic.Text = t.Contents.Replace("(!SKINID!)", CommonLogic.CookieUSInt(SkinBase.ro_SkinCookieName).ToString());
            }
        }
    }
}
