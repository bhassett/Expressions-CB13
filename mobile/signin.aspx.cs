using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceControls.Validators;

namespace InterpriseSuiteEcommerce.mobile
{
    public partial class signin : SkinBase
    {
        InputValidator _EmailValidator = null;
        private const string REMEMBERME_COOKIE_NAME = "ISERememberMeCookie";

        #region Events

        protected void Page_Init(object sender, EventArgs e)
        {
            LoginButton.Text = AppLogic.GetString("signin.aspx.16", SkinID, ThisCustomer.LocaleSetting);
            RequestPassword.Text = AppLogic.GetString("signin.aspx.15", SkinID, ThisCustomer.LocaleSetting);


            EMail.Attributes.Add("placeholder", AppLogic.GetString("mobile.signin.aspx.9", SkinID, ThisCustomer.LocaleSetting));
            Password.Attributes.Add("placeholder", AppLogic.GetString("mobile.signin.aspx.10", SkinID, ThisCustomer.LocaleSetting));
            SecurityCode.Attributes.Add("placeholder", AppLogic.GetString("signin.aspx.18", SkinID, ThisCustomer.LocaleSetting));
            ForgotEMail.Attributes.Add("placeholder", AppLogic.GetString("mobile.signin.aspx.9", SkinID, ThisCustomer.LocaleSetting));

        }

        protected override void OnInit(EventArgs e)
        {
            LoginButton.Click += LoginButton_Click;
            RequestPassword.Click += RequestPassword_Click;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ReturnURL.Text = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            if (ReturnURL.Text.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            string errorMsg = CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg");
            if (errorMsg.Trim().Length != 0)
            {
                ErrorMsgLabel.Text = errorMsg;
                ErrorPanel.Visible = true;
            }

            RequireSecurePage();

            if (!Page.IsPostBack)
            {
                DoingCheckout.Checked = CommonLogic.QueryStringBool("checkout");
                if (ReturnURL.Text.Length == 0)
                {
                    ReturnURL.Text = CommonLogic.QueryStringBool("checkout") ? "shoppingcart.aspx?checkout=true" : "default.aspx";
                }

                try
                {
                    string cookieValue = CookieTool.GetValue(REMEMBERME_COOKIE_NAME);
                    if (!string.IsNullOrEmpty(cookieValue) && CommonLogic.IsValidGuid(cookieValue))
                    {
                        var customerGuid = new Guid(cookieValue);
                        var rememberMeCustomer = Customer.Find(customerGuid);
                        EMail.Text = rememberMeCustomer.EMail;
                        this.Password.Attributes.Add("value", rememberMeCustomer.GetPassword());
                        this.PersistLogin.Checked = true;
                    }
                }
                catch
                {
                    EMail.Text = string.Empty;
                    Password.Text = string.Empty;
                }

                SignUpLink.NavigateUrl = "createaccount.aspx?checkout=" + DoingCheckout.Checked.ToString();
            }

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                // Create a random code and store it in the Session object.
                SecurityCodePanel.Visible = true;
                SecurityImagePanel.Visible = true;
                SecurityImage.ImageUrl = "Captcha.ashx?id=1";
            }

            HeaderMsg.SetContext = this;
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string EMailField = EMail.Text.ToLower();
            string PasswordField = Password.Text;

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
                        ErrorMsgLabel.Text = string.Format(AppLogic.GetString("signin.aspx.22", SkinID, ThisCustomer.LocaleSetting), string.Empty, string.Empty);
                        ErrorPanel.Visible = true;
                        SecurityCode.Text = string.Empty;
                        SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                        return;
                    }
                }
                else
                {
                    ErrorMsgLabel.Text = string.Format(AppLogic.GetString("signin.aspx.22", SkinID, ThisCustomer.LocaleSetting), string.Empty, string.Empty);
                    ErrorPanel.Visible = true;
                    SecurityCode.Text = String.Empty;
                    SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                    return;
                }
            }

            if (string.IsNullOrEmpty(EMailField) ||
                string.IsNullOrEmpty(EMailField.Trim()) ||
                string.IsNullOrEmpty(PasswordField) ||
                string.IsNullOrEmpty(PasswordField.Trim()))
            {
                DisplayInvalidLogin();
                return;
            }

            if (!CheckValidEmail()) return;

            var customerWithValidLogin = Customer.FindByLogin(EMail.Text, PasswordField);

            if (customerWithValidLogin == null)
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

            //check if remember me
            if (PersistLogin.Checked)
            {
                CookieTool.Add(REMEMBERME_COOKIE_NAME, customerWithValidLogin.ContactGUID.ToString(), DateTime.Now.AddDays(30));
            }
            else
            {
                CookieTool.Add(REMEMBERME_COOKIE_NAME, string.Empty, DateTime.Now.AddYears(-10));
            }

            //save the last record of fullmode to the loggedin user to maintain the view mode
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

            string cookieUserName = customerWithValidLogin.ContactGUID.ToString();
            bool createPersistentCookie = PersistLogin.Checked;

            //To handle multiple domain ie bug
            if (Request.Browser.Browser == "IE" &&
                    Request.Cookies.Keys.OfType<string>()
                                        .Where(k => k.ToUpper() == FormsAuthentication.FormsCookieName)
                                        .Count() > 1)
            {
                var autCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (autCookie != null)
                {
                    string encryptedData = FormsAuthentication.Encrypt(
                                                new FormsAuthenticationTicket(1, cookieUserName, DateTime.Now, DateTime.Now.AddMinutes(30),
                                                    createPersistentCookie, string.Empty, FormsAuthentication.FormsCookiePath));
                    autCookie.Value = encryptedData;
                    Request.Cookies.Set(autCookie);
                    Response.Cookies.Set(autCookie);
                }
            }
            else
            {
                FormsAuthentication.SetAuthCookie(cookieUserName, createPersistentCookie);
            }

            string sReturnURL = DoingCheckout.Checked ? "shoppingcart.aspx" : "default.aspx";
            if (sReturnURL.Contains("default.aspx"))
            {
                sReturnURL = "account.aspx";
            }

            Response.AddHeader("REFRESH", "1; URL=" + sReturnURL.ToUrlDecode());
        }

        protected void RequestPassword_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = true; // that is where the status msg goes, in all cases in this routine

            //FireFox does not validate RequiredFieldValidator1.
            //This code will double check forgotemail has value.
            if (ForgotEMail.Text.Trim() == "")
            {
                ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                return;
            }

            //Decrypt connectionstring using salt & vector scheme implemented by Interprise.
            Interprise.Licensing.Base.Services.CryptoServiceProvider tmpCrypto = new Interprise.Licensing.Base.Services.CryptoServiceProvider();

            ErrorMsgLabel.Text = string.Empty;
            string PWD = string.Empty;
            bool passwordValid = true;
            bool isNewPassword = false;
            string customerCode = string.Empty;
            string contactCode = string.Empty;
            bool exists = false;
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
                            PWD = tmpCrypto.Decrypt(Convert.FromBase64String(pwdCypher)
                                                , Convert.FromBase64String(salt)
                                             , Convert.FromBase64String(iv));
                        }
                        catch
                        {
                            passwordValid = false;
                        }
                    }
                    else
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", SkinID, ThisCustomer.LocaleSetting);
                        return;
                    }
                }
            }

            tmpCrypto = null;
            if (exists && !passwordValid)
            {
                byte[] salt = InterpriseHelper.GenerateSalt();
                byte[] iv = InterpriseHelper.GenerateVector();

                string newPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
                string newPasswordCypher = InterpriseHelper.Encryption(newPassword, salt, iv);

                string saltBase64 = Convert.ToBase64String(salt);
                string ivBase64 = Convert.ToBase64String(iv);

                DB.ExecuteSQL("UPDATE CRMContact SET Password = {0}, PasswordSalt = {1}, PasswordIV = {2} WHERE EntityCode = {3} AND ContactCode = {4}", DB.SQuote(newPasswordCypher), DB.SQuote(saltBase64), DB.SQuote(ivBase64), DB.SQuote(customerCode), DB.SQuote(contactCode));

                PWD = newPassword;
                isNewPassword = true;
            }

            if (PWD.Length != 0)
            {
                string FromEMail = AppLogic.AppConfig("MailMe_FromAddress");
                string EMail = ForgotEMail.Text;
                bool SendWasOk = false;
                try
                {
                    string WhoisRequestingThePassword = "\r\n" + ThisCustomer.LastIPAddress + "\r\n" + DateTime.Now.ToString();
                    string MsgBody = string.Empty;

                    MsgBody = InterpriseHelper.GetPasswordEmailTemplate(EMail);
                    MsgBody = InterpriseHelper.GetPasswordEmailTemplate(EMail);
                    if (MsgBody.Length > 0)
                    {
                        AppLogic.SendMail(AppLogic.AppConfig("StoreName") + " " + AppLogic.GetString("lostpassword.aspx.5", SkinID, ThisCustomer.LocaleSetting), MsgBody, true, FromEMail, FromEMail, EMail, EMail, "", AppLogic.AppConfig("MailMe_Server"));
                        SendWasOk = true;
                    }
                    else
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", SkinID, ThisCustomer.LocaleSetting);
                    }
                }
                catch { }
                if (SendWasOk)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                }
                else
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                }
            }
            else
            {
                ErrorMsgLabel.Text = AppLogic.GetString("lostpassword.aspx.4", SkinID, ThisCustomer.LocaleSetting);
            }
        }

        #endregion

        #region Methods
        private void DisplayInvalidLogin()
        {
            ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.20", SkinID, ThisCustomer.LocaleSetting);
            ErrorPanel.Visible = true;
        }

        private bool CheckValidEmail()
        {
            ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.21", SkinID, ThisCustomer.LocaleSetting);
            ErrorPanel.Visible = true;

            _EmailValidator = new RegularExpressionInputValidator(EMail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMsgLabel.Text);
            _EmailValidator.Validate();

            bool valid = _EmailValidator.IsValid;
            return valid;
        }

        #endregion
    }
}