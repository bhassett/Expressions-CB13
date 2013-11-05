// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for sendform.
    /// </summary>
    public partial class sendform : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            SectionTitle = AppLogic.GetString("sendform.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            // DOS attack prevention:
            if (AppLogic.OnLiveServer() && (Request.UrlReferrer == null || Request.UrlReferrer.Authority != Request.Url.Authority))
            {
                Response.Redirect("default.aspx", true);
                return;
            }
                string FormContents = string.Empty;
                if (CommonLogic.FormCanBeDangerousContent("AsXml").Length == 0)
                {
                    FormContents = CommonLogic.GetFormInput(true, "<br/>");
                    FormContents = FormContents + AppLogic.AppConfig("MailFooter");
                }
                else
                {
                    FormContents = CommonLogic.GetFormInputAsXml(true, "root");
                }
                string Subject = CommonLogic.FormCanBeDangerousContent("Subject");
                if (Subject.Length == 0)
                {
                    Subject = AppLogic.GetString("sendform.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                }
                string SendTo = CommonLogic.FormCanBeDangerousContent("SendTo");
                if (SendTo.Length == 0)
                {
                    SendTo = AppLogic.AppConfig("GotOrderEMailTo");
                }
                else
                {
                    SendTo += "," + AppLogic.AppConfig("GotOrderEMailTo");
                }
                foreach (string s in SendTo.Split(','))
                {
                    AppLogic.SendMail(Subject, FormContents, true, AppLogic.AppConfig("GotOrderEMailFrom"), AppLogic.AppConfig("GotOrderEMailFromName"), s, s, "", AppLogic.AppConfig("MailMe_Server"));
                }
                Label1.Text = AppLogic.GetString("sendform.aspx.3", SkinID, ThisCustomer.LocaleSetting);
        }
		
    }
}
