using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Tool;
using System.Text;

namespace InterpriseSuiteEcommerce
{
    public partial class sharegiftregistry : SkinBase
    {
        #region Initializer

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            if (ThisCustomer.IsNotRegistered)
            {
                string requestedPage = Security.UrlEncode(Request.Url.PathAndQuery);
                Response.Redirect("signin.aspx?returnurl=" + requestedPage);
            }

            if (!AppLogic.AppConfigBool("GiftRegistry.Enabled"))
            {
                CurrentContext.GoPageNotFound();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            ctrlGiftRegistryShareForm.ThisCustomer = ThisCustomer;

            var giftRegistry = this.GiftRegistryFromQueryString;
            if (giftRegistry != null)
            {

                //block the access if registry is not owned by the current user
                if (!ThisCustomer.IsRegistryOwnedByCustomer(giftRegistry.RegistryID))
                {
                    Response.Redirect("giftregistry.aspx");
                }

                ctrlGiftRegistryShareForm.LoadGiftRegistry(giftRegistry);

                string defaultMessage = string.Format(AppLogic.GetString("editgiftregistry.aspx.44", SkinID, ThisCustomer.LocaleSetting), giftRegistry.URLForViewing);
                ctrlGiftRegistryShareForm.HtmlMessage = defaultMessage;

                StartDate = giftRegistry.StartDate;
                EndDate = giftRegistry.EndDate;
                Title = giftRegistry.Title;
                CustomURL = giftRegistry.CustomURLPostfix;
            }
            else
            {
                //registry not found
                Response.Redirect("giftregistry.aspx");
            }

            btnSend.Click += (sender, ex) => SendNow();
            btnSend.Text = AppLogic.GetString("editgiftregistry.aspx.40", SkinID, ThisCustomer.LocaleSetting);
        }

        #endregion

        #region Methods

        private void SendNow()
        {
            bool isSendCopy = ctrlGiftRegistryShareForm.IsSendMeCopy;
            var emails = ctrlGiftRegistryShareForm.GetEmailAddresses();
            string subject = ctrlGiftRegistryShareForm.Subject;

            if (emails.Count() == 0) 
            {
                DisplayError(new List<string>() { AppLogic.GetString("editgiftregistry.error.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)});
                return;
            }

            //decode since we will not going to save it to the database.
            string htmlMessage = ctrlGiftRegistryShareForm.HtmlMessage.ToHtmlDecode();

            var param = new XElement(DomainConstants.XML_ROOT_NAME);
            param.Add(new XElement("MAIL_SUBJECT", subject));
            param.Add(new XElement("MAIL_BODY", htmlMessage));

            //param.Add(new XElement("REGISTRY_LINK", ));
            var package = new XmlPackage2("notification.emailgiftregistry.xml.config", param);
            string html = package.TransformString();

            string[] emailacctinfo = InterpriseHelper.GetStoreEmailAccountInfo();

            try
            {
                foreach (var email in emails)
                {
                    AppLogic.SendMailRequest(subject, htmlMessage, true, emailacctinfo[0], emailacctinfo[1], email, email, string.Empty);
                }

                //MailSerder.SendMail(subject, "jaysword1@gmail.com", html, SkinID);
                if (isSendCopy)
                {
                    AppLogic.SendMailRequest(subject, htmlMessage, true, emailacctinfo[0], emailacctinfo[1], ThisCustomer.EMail, ThisCustomer.FullName, string.Empty);
                    //MailSerder.SendMail(subject, ThisCustomer.EMail, html, SkinID);
                }

                ctrlGiftRegistryShareForm.ClearTextBox();

                DisplayError(new List<string>() { AppLogic.GetString("editgiftregistry.aspx.45", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) });
            }
            catch (Exception)
            {
                DisplayError(new List<string>() { AppLogic.GetString("editgiftregistry.error.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) } );
            }
        }

        private void DisplayError(IEnumerable<string> errorMessages)
        {
            pnlErrorMessage.Visible = false;
            var htlm = new StringBuilder();
            if (errorMessages.Count() > 0)
            {
                htlm.Append("<ul class='error-layout'>");
                foreach (var error in errorMessages)
                {
                    htlm.AppendFormat("<li>{0}</li>", error);
                }
                htlm.Append("</ul>");
            }
            htlm.ToString();

            var lit = new Literal();
            lit.Text = htlm.ToString();
            pnlErrorMessage.Controls.Add(lit);
            pnlErrorMessage.Visible = true;
        }

        #endregion

        #region Properties

        private GiftRegistry GiftRegistryFromQueryString
        {
            get
            {
                GiftRegistry giftregistry = null;
                Guid? registryId = DomainConstants.GIFTREGISTRYPARAMCHAR.ToQueryStringDecode().TryParseGuid();
                if (registryId.HasValue)
                {
                    giftregistry = GiftRegistryDA.GetGiftRegistryByRegistryID(registryId.Value, InterpriseHelper.ConfigInstance.WebSiteCode);
                }
                return giftregistry;
            }
        }

        public DateTime? StartDate
        {
            get
            {
                DateTime? dt = null;
                if (ViewState["StartDate"] != null)
                {
                    dt = (DateTime)ViewState["StartDate"];
                }
                return dt;
            }
            set
            {
                ViewState["StartDate"] = value;
            }
        }

        public DateTime? EndDate
        {
            get
            {
                DateTime? dt = null;
                if (ViewState["EndDate"] != null)
                {
                    dt = (DateTime)ViewState["EndDate"];
                }
                return dt;
            }
            set
            {
                ViewState["EndDate"] = value;
            }
        }

        public string Title
        {
            get
            {
                string title = string.Empty;
                if (ViewState["Title"] != null)
                {
                    title = ViewState["Title"].ToString();
                }
                return title;
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        public string CustomURL
        {
            get
            {
                string title = string.Empty;
                if (ViewState["CustomURL"] != null)
                {
                    title = ViewState["CustomURL"].ToString();
                }
                return title;
            }
            set
            {
                ViewState["CustomURL"] = value;
            }
        }

        #endregion
    }

}