using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    public partial class contactus : SkinBase
    {
        protected override void OnInit(EventArgs e)
        {
            btnSendMessage.Click += (sender, ex) => SendMessage();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitPageContent();
        }


        protected void SendMessage()
        {
            try
            {
                if (!IsSecurityCodeGood(txtCaptcha.Text))
                {
                    errorSummary.DisplayErrorMessage(AppLogic.GetString("createaccount.aspx.126", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
                    return;
                }

                if (AppLogic.OnLiveServer() && (Request.UrlReferrer == null || Request.UrlReferrer.Authority != Request.Url.Authority))
                {
                    Response.Redirect("default.aspx", true);
                    return;
                }

                var content = new StringBuilder();

                string subject = txtSubject.Text.Trim();
                string senderName = txtContactName.Text.Trim();
                string senderEmail = txtEmail.Text.Trim();

                string toName = CommonLogic.IIF(AppLogic.AppConfig("ContactUsNameTo").IsNullOrEmptyTrimmed(), AppLogic.AppConfig("GotOrderEMailToName"), AppLogic.AppConfig("ContactUsNameTo"));
                toName = toName.Trim();

                string toEmail = CommonLogic.IIF(AppLogic.AppConfig("ContactUsEmailTo").IsNullOrEmptyTrimmed(), AppLogic.AppConfig("GotOrderEMailTo"), AppLogic.AppConfig("ContactUsEmailTo"));
                toEmail = toEmail.Trim();

                string message = txtMessageDetails.Text.Trim();

                content.Append(message);
                content.Append("<br/><br/>");

                content.AppendFormat("Contact Name: {0}", senderName);
                content.Append("<br/>");
                content.AppendFormat("Phone: {0}", txtContactNumber.Text.Trim());
                content.Append("<br/>");
                content.AppendFormat("Email Address: {0}", senderEmail);

                AppLogic.SendMailRequest(subject, content.ToString(), true, senderEmail, senderEmail, toEmail, toName, string.Empty, true);
                Response.Redirect("t-ContactUsFormThankYouPage.aspx");
            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }


        protected bool IsSecurityCodeGood(string code)
        {
        
            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnContactUs")) return true;

            if (Session["SecurityCode"] != null)
            {

                string sCode = Session["SecurityCode"].ToString();
                string fCode = code;

                if (AppLogic.AppConfigBool("Captcha.CaseSensitive"))
                {
                    if (fCode.Equals(sCode)) return true;
                }
                else
                {
                    if (fCode.Equals(sCode, StringComparison.InvariantCultureIgnoreCase)) return true;
                }

                return false;
            }

            return true;

        }

        protected void InitPageContent()
        {
            SectionTitle = AppLogic.GetString("contactus.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            btnSendMessage.Text = AppLogic.GetString("contactus.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);

            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnContactUs")) pnlSecurityCode.Visible = false;

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnSendMessage, "contactus.aspx.11");
            }

            ContactUsFormHelpFulTipsTopic.SetContext = this;

        }

    }
}