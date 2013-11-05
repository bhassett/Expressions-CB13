// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceControls.Validators;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for signin.
    /// </summary>
    public partial class signin : SkinBase
    {
        InputValidator _EmailValidator = null;
        private const string REMEMBERME_COOKIE_NAME = "ISERememberMeCookie";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            LoginButton.Text = AppLogic.GetString("signin.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            RequestPassword.Text = AppLogic.GetString("signin.aspx.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(LoginButton, "signin.aspx.16");
                AppLogic.EnableButtonCaptionEditing(RequestPassword, "signin.aspx.15");
            }

            ReturnURL.Text = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");

            //
            if (ReturnURL.Text.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            if (HttpContext.Current.Request.Browser.Browser.Equals("Firefox", StringComparison.InvariantCultureIgnoreCase))
            {
                ErrorMsgLabel.Text = string.Empty;
            }

            string errorMsg = CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg");
            if (errorMsg.Trim().Length != 0)
            {
                ErrorMsgLabel.Text = errorMsg;
                ErrorPanel.Visible = true;
            }

            RequireSecurePage();
            SectionTitle = AppLogic.GetString("signin.aspx.1", SkinID, ThisCustomer.LocaleSetting, true);
            if (!Page.IsPostBack)
            {
                DoingCheckout.Checked = CommonLogic.QueryStringBool("checkout");
                if (ReturnURL.Text.Length == 0)
                {
                    if (CommonLogic.QueryStringBool("checkout"))
                    {
                        ReturnURL.Text = "shoppingcart.aspx?checkout=true";
                    }
                    else
                    {
                        ReturnURL.Text = "default.aspx";
                    }
                }
                
                if (Request.Cookies[REMEMBERME_COOKIE_NAME] != null)
                {
                    try
                    {
                        HttpCookie cookie = Request.Cookies[REMEMBERME_COOKIE_NAME];
                        if (cookie != null)
                        {
                            if (CommonLogic.IsValidGuid(cookie.Value))
                            {
                                Guid customerGuid = new Guid(cookie.Value);
                                Customer rememberMeCustomer = Customer.Find(customerGuid);
                                EMail.Text = rememberMeCustomer.EMail;
                                this.Password.Attributes.Add("value", rememberMeCustomer.GetPassword());

                                this.PersistLogin.Checked = true;
                            }
                        }
                    }
                    catch
                    {
                        EMail.Text = string.Empty;
                        Password.Text = string.Empty;
                    }
                }
                SignUpLink.NavigateUrl = "createaccount.aspx?checkout=" + DoingCheckout.Checked.ToString();
                CheckoutPanel.Visible = DoingCheckout.Checked;
                CheckoutMap.HotSpots[0].AlternateText = AppLogic.GetString("checkoutanon.aspx.2", SkinID, ThisCustomer.LocaleSetting, true);
            }

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                // Create a random code and store it in the Session object.
                SecurityImage.Visible = true;
                SecurityCode.Visible = true;
                RequiredFieldValidator4.Enabled = true;
                Label1.Visible = true;
                SecurityImage.ImageUrl = "Captcha.ashx?id=1";
            }
            HeaderMsg.SetContext = this;
        }

        private void DisplayInvalidLogin()
        {
            ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.20", SkinID, ThisCustomer.LocaleSetting, true);
            ErrorPanel.Visible = true;
        }

        private bool CheckValidEmail()
        {
            ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.21", SkinID, ThisCustomer.LocaleSetting, true);
            ErrorPanel.Visible = true;

            _EmailValidator = new RegularExpressionInputValidator(EMail, DomainConstants.EmailRegExValidator, ErrorMsgLabel.Text.ToString());
            _EmailValidator.Validate();
            return (_EmailValidator.IsValid);
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string EMailField = EMail.Text.ToLower();
            string PasswordField = Password.Text;
            string NewCustomerID = string.Empty;

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                if (Session["SecurityCode"] != null)
                {
                    string sCode = Session["SecurityCode"].ToString();
                    string fCode = SecurityCode.Text;
                    bool codeMatch = false;

                    if (AppLogic.AppConfigBool("Captcha.CaseSensitive"))
                    {
                        if (fCode.Equals(sCode))
                            codeMatch = true;
                    }
                    else
                    {
                        if (fCode.Equals(sCode, StringComparison.InvariantCultureIgnoreCase))
                            codeMatch = true;
                    }

                    if (!codeMatch)
                    {
                        ErrorMsgLabel.Text = string.Format(AppLogic.GetString("signin.aspx.22", SkinID, ThisCustomer.LocaleSetting, true), string.Empty, string.Empty);
                        ErrorPanel.Visible = true;
                        SecurityCode.Text = string.Empty;
                        SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                        return;
                    }
                }
                else
                {
                    ErrorMsgLabel.Text = string.Format(AppLogic.GetString("signin.aspx.22", SkinID, ThisCustomer.LocaleSetting, true), string.Empty, string.Empty);
                    ErrorPanel.Visible = true;
                    SecurityCode.Text = string.Empty;
                    SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                    return;
                }
            }
            
            if( string.IsNullOrEmpty(EMailField) || 
                string.IsNullOrEmpty(EMailField.Trim()) || 
                string.IsNullOrEmpty(PasswordField) ||
                string.IsNullOrEmpty(PasswordField.Trim()) )
            {
                DisplayInvalidLogin();
                return;
            }

            if (CheckValidEmail())
            {
                Customer customerWithValidLogin = Customer.FindByLogin(EMail.Text, PasswordField);
                
                if (null == customerWithValidLogin)
                {
                    DisplayInvalidLogin();
                    return;
                }

                bool isAllowed = InterpriseHelper.ValidateContactSubscription(customerWithValidLogin);
                if (!isAllowed)
                {
                    DisplayInvalidLogin();
                    return;
                }

                var rememberMeCookie = new HttpCookie(REMEMBERME_COOKIE_NAME);
                Response.Cookies.Remove(REMEMBERME_COOKIE_NAME);
                //check if remember me
                if (PersistLogin.Checked == true)
                {
                    rememberMeCookie.Value = customerWithValidLogin.ContactGUID.ToString();
                    rememberMeCookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(rememberMeCookie);
                }
                else
                {
                    rememberMeCookie.Expires = DateTime.Now.AddYears(-10);
                }

                //save the last record of fullmode
                customerWithValidLogin.FullModeInMobile = ThisCustomer.FullModeInMobile;

                // dis-associate the session information if any..
                ThisCustomer.ThisCustomerSession.Clear();

                // we've got a good login...
                AppLogic.ExecuteSigninLogic(ThisCustomer.CustomerCode, ThisCustomer.ContactCode, customerWithValidLogin.CustomerCode, string.Empty, customerWithValidLogin.ContactCode);
                
                // we've got a good login:
                FormPanel.Visible = false;
                ExecutePanel.Visible = true;

                ThisCustomer.ThisCustomerSession["ContactID"] = customerWithValidLogin.ContactGUID.ToString();
                SignInExecuteLabel.Text = AppLogic.GetString("signin.aspx.2", SkinID, ThisCustomer.LocaleSetting);

                InterpriseHelper.CreateContactSiteLog(customerWithValidLogin, "Login");

                string cookieUserName       = customerWithValidLogin.ContactGUID.ToString();
                bool createPersistentCookie = PersistLogin.Checked;

                //support cross domain login
                Security.SignOutCrossDomainCookie();
                Security.CreateLoginCookie(cookieUserName, createPersistentCookie);

                string sReturnURL = FormsAuthentication.GetRedirectUrl(cookieUserName, createPersistentCookie);
                if (sReturnURL.Length == 0)
                {
                    sReturnURL = ReturnURL.Text;                 
                }
                if (sReturnURL.Length == 0)
                {
                    if (DoingCheckout.Checked)
                    {
                        sReturnURL = "shoppingcart.aspx";
                    }
                    else
                    {
                        sReturnURL = "default.aspx";
                    }
                }
                if (sReturnURL.Contains("default.aspx"))
                {
                    sReturnURL = sReturnURL.Replace("default", "account");
                }

                if (sReturnURL.Contains("download.aspx"))
                {
                    sReturnURL = sReturnURL + "&sid=" + CommonLogic.QueryStringCanBeDangerousContent("sid");
                }
                
                Response.AddHeader("REFRESH", "1; URL=" + Security.UrlDecode(sReturnURL));
            }

        }

        protected void RequestPassword_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = true; // that is where the status msg goes, in all cases in this routine

            //FireFox does not validate RequiredFieldValidator1.
            //This code will double check forgotemail has value.
            if (ForgotEMail.Text.Trim() == string.Empty)
            {
                ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.3", SkinID, ThisCustomer.LocaleSetting, true);
                return;
            }

            //Decrypt connectionstring using salt & vector scheme implemented by Interprise.
            ErrorMsgLabel.Text  = string.Empty;
            string PWD          = string.Empty;
            bool passwordValid = true;
            string customerCode = string.Empty;
            string contactCode  = string.Empty;
            bool exists         = false;

            string sql = string.Format("SELECT EntityCode, cc.ContactCode, Password,PasswordSalt,PasswordIV FROM CRMContact cc WITH (NOLOCK) INNER JOIN EcommerceCustomerActiveSites ecas ON cc.ContactCode = ecas.ContactCode WHERE IsAllowWebAccess=1 AND UserName= {0} AND ecas.WebSiteCode = {1} AND ecas.IsEnabled = 1", DB.SQuote(ForgotEMail.Text.ToLower()), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode));
            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var rs = DB.GetRSFormat(con, sql))
                {
                    exists = rs.Read();
                    if (exists)
                    {
                        string pwdCypher = DB.RSField(rs, "Password");
                        string salt = DB.RSField(rs, "PasswordSalt");
                        string iv = DB.RSField(rs, "PasswordIV");
                        customerCode = DB.RSField(rs, "EntityCode");
                        contactCode = DB.RSField(rs, "ContactCode");

                        try
                        {
                            var tmpCrypto = new Interprise.Licensing.Base.Services.CryptoServiceProvider();
                            PWD = tmpCrypto.Decrypt(Convert.FromBase64String(pwdCypher), 
                                                    Convert.FromBase64String(salt), 
                                                    Convert.FromBase64String(iv));
                        }
                        catch
                        {
                            passwordValid = false;
                        }
                    }
                    else
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", SkinID, ThisCustomer.LocaleSetting, true);
                        return;
                    }
                }
            }

            if (exists && !passwordValid)
            {
                byte[] salt = InterpriseHelper.GenerateSalt();
                byte[] iv = InterpriseHelper.GenerateVector();
                
                string newPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
                string newPasswordCypher = InterpriseHelper.Encryption(newPassword, salt, iv);

                string saltBase64=  Convert.ToBase64String(salt);
                string ivBase64 = Convert.ToBase64String(iv);

                DB.ExecuteSQL("UPDATE CRMContact SET Password = {0}, PasswordSalt = {1}, PasswordIV = {2} WHERE EntityCode = {3} AND ContactCode = {4}", DB.SQuote(newPasswordCypher), DB.SQuote(saltBase64), DB.SQuote(ivBase64), DB.SQuote(customerCode), DB.SQuote(contactCode));

                PWD = newPassword;
            }

            if (PWD.Length != 0)
            {
                string FromEMail = AppLogic.AppConfig("MailMe_FromAddress");
                string EMail     = ForgotEMail.Text;
                bool SendWasOk   = false;
                try
                {
                    string WhoisRequestingThePassword = "\r\n" + ThisCustomer.LastIPAddress + "\r\n" + DateTime.Now.ToString();
                    string MsgBody                    = string.Empty;

                    MsgBody = InterpriseHelper.GetPasswordEmailTemplate(EMail);
                    if (MsgBody.Length > 0)
                    {
                        AppLogic.SendMail(AppLogic.AppConfig("StoreName") + " " + AppLogic.GetString("lostpassword.aspx.5", SkinID, ThisCustomer.LocaleSetting, true), MsgBody, true, FromEMail, FromEMail, EMail, EMail, "", AppLogic.AppConfig("MailMe_Server"));
                        SendWasOk = true;
                    }
                    else 
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", SkinID, ThisCustomer.LocaleSetting, true);
                    }
                }
                catch { }
                if (SendWasOk)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.2", SkinID, ThisCustomer.LocaleSetting, true);
                }
                else
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.3", SkinID, ThisCustomer.LocaleSetting, true);
                }
            }
            else
            {
                ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", SkinID, ThisCustomer.LocaleSetting, true);
            }
        }
    }
}
