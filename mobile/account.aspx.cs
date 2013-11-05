using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interprise.Facade.Customer;
using Interprise.Framework.Customer.DatasetGateway;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceControls.mobile;

namespace InterpriseSuiteEcommerce
{
    public partial class account : SkinBase
    {
        bool AccountUpdated = false;
        bool Checkout = false;
        public CultureInfo SqlServerCulture = new System.Globalization.CultureInfo(CommonLogic.Application("DBSQLServerLocaleSetting")); // qualification needed for vb.net (not sure why)
        public string m_StoreLoc = AppLogic.GetStoreHTTPLocation(true);
        string SkinImagePath = string.Empty;

        protected override void OnInit(EventArgs e)
        {
			//required for mobile buttons
            BindControls();

            base.OnInit(e);
            RequireCustomerRecord();
            PerformPageAccessLogic();
            InitializeAccountControl();
            SetDefaultPaswordPlaceHolderValues();
            CheckIfShouldRequireCaptcha();
        }
		
		//for mobile user control settings
        private void BindControls()
        {
            btnUpdateAccount.Click += btnUpdateAccount_Click;
            btnUpdateAccount.Text = AppLogic.GetString("account.aspx.6", SkinID, ThisCustomer.LocaleSetting);
            lblHeaderOrderHistory.Text = AppLogic.GetString("mobile.account.aspx.34", SkinID, ThisCustomer.LocaleSetting);
            btnContinueToCheckOut.Text = AppLogic.GetString("account.aspx.24", SkinID, ThisCustomer.LocaleSetting);
        }

        private void PerformPageAccessLogic()
        {
            RequireSecurePage();
            RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));

            //If current user came from IS, chances are it has no Primary Billing Info!
            if (ThisCustomer.PrimaryBillingAddressID == String.Empty)
            {

                // IS Facade implementation
                string[][] commandSet;
                string[][] parameters;

                var customerGateway = new CustomerDetailDatasetGateway();
                var iseCustomer = new CustomerDetailFacade(customerGateway);

                // Specific view and stored procedure
                commandSet = new string[][] { new string[] { "CustomerView", "ReadCustomer" } };
                parameters = new string[][] { new string[] { "@CustomerCode", null } };

                // Load it at defined Dataset
                iseCustomer.LoadDataSet(commandSet, parameters, Interprise.Framework.Base.Shared.Enum.ClearType.Specific, Interprise.Framework.Base.Shared.Enum.ConnectionStringType.Online);

                // Retrieve data information, store it at DataRow array
                // Filter the data information based on the Customer Code
                DataRow[] rowsCustomer = customerGateway.Tables["CustomerView"].Select(string.Concat("CustomerCode = '", ThisCustomer.CustomerCode, "'"));

                // It should return 1 record from filtered dataset.
                if (rowsCustomer.Length > 0)
                {
                    // By Default 
                    string ResidenceType = "Residential";

                    // Address Class to store address information 
                    // retrieve via DataRow array
                    var thisAddress = new Address
                    {
                        CustomerCode = ThisCustomer.CustomerCode,
                        Name = rowsCustomer[0]["CustomerName"].ToString(),
                        Address1 = rowsCustomer[0]["Address"].ToString(),
                        City = rowsCustomer[0]["City"].ToString(),
                        State = rowsCustomer[0]["State"].ToString(),
                        PostalCode = rowsCustomer[0]["PostalCode"].ToString(),
                        Country = rowsCustomer[0]["Country"].ToString(),
                        Phone = rowsCustomer[0]["Telephone"].ToString(),
                        County = rowsCustomer[0]["County"].ToString()
                    };

                    // set default if null
                    if (!string.IsNullOrEmpty(rowsCustomer[0]["ResidenceType"].ToString()))
                    {
                        ResidenceType = rowsCustomer[0]["ResidenceType"].ToString();
                    }

                    thisAddress.ResidenceType = (ResidenceTypes)Enum.Parse(typeof(ResidenceTypes), ResidenceType);

                    // Adding of Customer Billing Address Information
                    InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, true);
                }
                else
                {
                    // If no Record is found
                    // Redirects to address information entry
                    Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Billing&returnURL=account.aspx");
                }

            }

            ThisCustomer.ValidatePrimaryAddresses();
            SkinImagePath = "skins/skin_" + SkinID.ToString() + "/images/";
        }

        private void InitializeAccountControl()
        {
            AttachDuplicateOtherCustomerEmailEvaluationEventHandler();
            LoadAvailableSalutationsForBillingAddress();
            AssignAddressCaptions();
            AssignAddressValidationPrerequisites();
            AssignAddressErrorSummary();
            InitializeOnlyDisplayedFieldsAndDisplayAccountInformation();
            CheckIfShouldRequireOver13BillingOption();
            CheckIfShouldRequireStrongPassword();
            CheckIfShouldDisplaySaveCCDetailsOption();
            CheckIfShouldShowSalutation();
        }

        private void CheckIfShouldDisplaySaveCCDetailsOption()
        {
            ctrlBillingAddress.RequireSaveCCDetails = false;
            ctrlBillingAddress.SaveCCDetailsCaption = "Save my Credit Card Info";
            ctrlBillingAddress.SaveCCDetails = false;
        }

        private void AttachDuplicateOtherCustomerEmailEvaluationEventHandler()
        {
            ctrlBillingAddress.EvaluateNoDuplicateCustomerEmail += BillingAddress_EvaluateNoDuplicateCustomerEmail;
        }

        private void BillingAddress_EvaluateNoDuplicateCustomerEmail(object sender, CancelEventArgs e)
        {
            e.Cancel = ValidateCanCustomerChangeToOtherEmail();

            // revert...
            if (!e.Cancel)
            {
                ctrlBillingAddress.Email = ThisCustomer.EMail;
            }
        }

        private bool ValidateCanCustomerChangeToOtherEmail()
        {
            string proposedEmail = ctrlBillingAddress.Email;

            return ThisCustomer.EMail.Equals(proposedEmail) || ThisCustomer.CanChangeEmailTo(proposedEmail);
        }

        private void LoadAvailableSalutationsForBillingAddress()
        {
            var salutations = new List<KeyValuePair<string, string>>();
            salutations.Add(new KeyValuePair<string, string>(AppLogic.GetString("createaccount.aspx.81", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), string.Empty));

			//Chage for optimization
            var lstSalutations = AppLogic.GetSystemSalutationsBillingAddress();
            salutations.AddRange(lstSalutations.Select(s => new KeyValuePair<string, string>(s.SalutationDescription, s.SalutationDescription)));
            ctrlBillingAddress.SetSalutations(salutations);
        }

        private void AssignAddressCaptions()
        {
            ctrlBillingAddress.AccountNameCaption = AppLogic.GetString("createaccount.aspx.36", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.SalutationCaption = AppLogic.GetString("createaccount.aspx.35", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.EmailCaption = AppLogic.GetString("createaccount.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.OkToEmailCaption = AppLogic.GetString("createaccount.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.Over13YearsOldCaption = AppLogic.GetString("createaccount.aspx.26", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.SubScriptionOptionCaption = AppLogic.GetString("createaccount.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.FirstNameCaption = AppLogic.GetString("createaccount.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.LastNameCaption = AppLogic.GetString("createaccount.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PhoneNumberCaption = AppLogic.GetString("createaccount.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PasswordCaption = AppLogic.GetString("createaccount.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PasswordMinLengthCaption = AppLogic.GetString("mobile.createaccount.aspx.4", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.ConfirmPasswordCaption = AppLogic.GetString("createaccount.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.CaptchaCaption = AppLogic.GetString("signin.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.OkToEmailYesOptionCaption = AppLogic.GetString("createaccount.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.OkToEmailNoOptionCaption = AppLogic.GetString("createaccount.aspx.13", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
		}

        private void AssignAddressValidationPrerequisites()
        {
            ctrlBillingAddress.FirstNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.37", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.LastNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.38", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.AccountNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.39", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PhoneRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.41", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.FirstNameMaximumCharacterLength = 50;
            ctrlBillingAddress.FirstNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.LastNameMaximumCharacterLength = 50;
            ctrlBillingAddress.LastNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.43", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.AccountNameMaximumCharacterLength = 100;
            ctrlBillingAddress.AccountNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.44", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.AddressMaximumCharacterLength = 200;
            ctrlBillingAddress.AddressMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.45", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PhoneMaximumCharacterLength = 50;
            ctrlBillingAddress.PhoneMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.46", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.EmailRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.47", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PasswordRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.48", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.ConfirmPasswordRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.49", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.EmailMaximumCharacterLength = 50;
            ctrlBillingAddress.EmailMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.50", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PasswordMinimumCharacterLength = 4;
            ctrlBillingAddress.PasswordMaximumCharacterLength = 50;
            ctrlBillingAddress.PasswordMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.51", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.CompareConfirmPasswordErrorMessage = AppLogic.GetString("createaccount.aspx.52", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.EmailValidationRegularExpression = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            ctrlBillingAddress.EmailValidationRegularExpressionErrorMessage = AppLogic.GetString("createaccount.aspx.53", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.RequireNoDuplicateCustomerEmailErrorMessage = AppLogic.GetString("createaccount.aspx.94", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.CaptchaRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.54", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.CaptchaInvalidErrorMessage = AppLogic.GetString("createaccount.aspx.55", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.StrongPasswordValidationRegularExpression = AppLogic.AppConfig("CustomerPwdValidator");
            ctrlBillingAddress.StrongPasswordValidationErrorMessage = AppLogic.GetString("createaccount.aspx.28", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        private void InitializeOnlyDisplayedFieldsAndDisplayAccountInformation()
        {
            ChooseOnlyFieldsToDisplayAndDefaultSettings();
            if (!IsPostBack)
            {
                DisplayCustomerAccountInformation();
            }
        }

        private void AssignAddressErrorSummary()
        {
            ctrlBillingAddress.ErrorSummaryControl = this.errorSummary;
        }

        private void ChooseOnlyFieldsToDisplayAndDefaultSettings()
        {
            // these can be set on the designer or this one
            ctrlBillingAddress.ShowAddresses = false;
            ctrlBillingAddress.ShowAccountName = false;
            ctrlBillingAddress.RequireNoDuplicateCustomerEmail = true;
        }

        private void DisplayCustomerAccountInformation()
        {
            ctrlBillingAddress.Salutation = ThisCustomer.Salutation;
            ctrlBillingAddress.FirstName = ThisCustomer.FirstName;
            ctrlBillingAddress.LastName = ThisCustomer.LastName;
            ctrlBillingAddress.Email = ThisCustomer.EMail;
            ctrlBillingAddress.PhoneNumber = ThisCustomer.Phone;
            ctrlBillingAddress.Password = AppLogic.PasswordValuePlaceHolder;
            ctrlBillingAddress.ConfirmedPassword = AppLogic.PasswordValuePlaceHolder;
            ctrlBillingAddress.OkToEmailChecked = ThisCustomer.OKToEMail;
            ctrlBillingAddress.Over13Checked = ThisCustomer.IsOver13;
        }

        #region CheckIfShouldRequireStrongPassword
        private void CheckIfShouldRequireStrongPassword()
        {
            if (AppLogic.AppConfigBool("UseStrongPwd"))
            {
                ctrlBillingAddress.RequireStrongPassword = true;
            }
        }
        #endregion

        #region CheckIfShouldRequireOver13BillingOption
        private void CheckIfShouldRequireOver13BillingOption()
        {
            ctrlBillingAddress.RequireOver13 = AppLogic.AppConfigBool("RequireOver13Checked");
        }
        #endregion

        #region CheckIfShouldRequireCaptcha

        private void CheckIfShouldRequireCaptcha()
        {
            if (!Checkout && AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccount"))
            {
                ctrlBillingAddress.RequireCaptcha = true;
                ctrlBillingAddress.CaptchaImageUrl = "Captcha.ashx?id=1";
            }
        }

        #endregion

        private void CheckIfShouldShowSalutation()
        {
            ctrlBillingAddress.RequireSalutation = AppLogic.AppConfigBool("Address.ShowSalutation");
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/address_ajax.js"));
            manager.Scripts.Add(new ScriptReference("js/account_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshPage();

            var script = new StringBuilder();
            script.Append("<script type=\"text/javascript\" >\n");
            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "account.aspx.26", AppLogic.GetString("account.aspx.26", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            script.AppendFormat("   ise.Pages.Account.setBillingAddressControlId('{0}');\n", this.ctrlBillingAddress.ClientID);
            script.AppendFormat("   ise.Pages.Account.setForm('{0}');\n", this.AccountForm.ClientID);
            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        public void btnContinueToCheckOut_Click(object sender, EventArgs e)
        {
            Response.Redirect("checkoutshipping.aspx");
        }

        public void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            if (!this.IsValid) { return; }

            ThisCustomer.EMail = ctrlBillingAddress.Email;
            ThisCustomer.Password = ctrlBillingAddress.Password;
            ThisCustomer.FirstName = ctrlBillingAddress.FirstName;
            ThisCustomer.LastName = ctrlBillingAddress.LastName;
            ThisCustomer.Phone = ctrlBillingAddress.PhoneNumber;
            ThisCustomer.IsOver13 = ctrlBillingAddress.Over13Checked;
            ThisCustomer.IsOKToEMail = ctrlBillingAddress.OkToEmailChecked;
            ThisCustomer.Salutation = ctrlBillingAddress.Salutation;

            ThisCustomer.Update();
            AccountUpdated = true;
            RefreshPage();
        }

       private void SetDefaultPaswordPlaceHolderValues()
        {
            ctrlBillingAddress.Password = AppLogic.PasswordValuePlaceHolder;
            ctrlBillingAddress.ConfirmedPassword = AppLogic.PasswordValuePlaceHolder;
        }
		//converted to private since it should now be called outside
        private void RefreshPage()
        {
            Address BillingAddress = ThisCustomer.PrimaryBillingAddress;
            Address ShippingAddress = ThisCustomer.PrimaryShippingAddress;

            Checkout = CommonLogic.QueryStringBool("checkout");

            if (Checkout)
            {
                //pnlCheckoutImage.Visible = true;
                //CheckoutImage.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "step_2.gif");
                if (ThisCustomer.PrimaryBillingAddressID == String.Empty || ThisCustomer.PrimaryShippingAddressID == String.Empty || !ThisCustomer.HasAtLeastOneAddress())
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("account.aspx.33", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "&nbsp;&nbsp;";
                }
            }

            //string XRI = AppLogic.LocateImageURL(SkinImagePath + "redarrow.gif");
            //redarrow1.ImageUrl = XRI;
            //redarrow2.ImageUrl = XRI;
            //pnlCheckoutImage.Visible = Checkout;
			//remove unknownerrormsg.Text use ErrorMsgLabel for messages
            ErrorMsgLabel.Text = Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("unknownerror")) + "<br />";
            ErrorMsgLabel.Text += Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("errormsg"));

            //Optimize the code - remove lblAcctUpdateMsg and use the ErrorMsgLabel
            ErrorMsgLabel.Text = AppLogic.GetString(AccountUpdated ? "account.aspx.1" : "account.aspx.2", SkinID, ThisCustomer.LocaleSetting);

            //pnlNotCheckOutButtons.Visible = !Checkout;
            pnlShowWishButton.Visible = AppLogic.AppConfigBool("ShowWishListButton");
            //imgAccountinfo.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "accountinfo.gif");
            btnContinueToCheckOut.Visible = Checkout;

            lnkChangeBilling.ImageUrl = AppLogic.LocateImageURL(string.Format("skins/Skin_{0}/images/change.gif", ThisCustomer.SkinID.ToString()));
            lnkChangeBilling.NavigateUrl = "javascript:self.location='selectaddress.aspx?Checkout=" + Checkout.ToString() + "&AddressType=billing&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString()) + "'";
            //imgAddressbook.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "addressbook.gif");
            litBillingAddress.Text = BillingAddress.DisplayHTML(Checkout);
            bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo") && !AppLogic.AppConfigBool("SkipShippingOnCheckout");

            if (!AllowShipToDifferentThanBillTo)
            {
                pnlShipping.Visible = false;
            }
            else
            {
                lnkChangeShipping.ImageUrl = AppLogic.LocateImageURL(string.Format("skins/Skin_{0}/images/change.gif", this.ThisCustomer.SkinID));
                lnkChangeShipping.NavigateUrl = "javascript:self.location='selectaddress.aspx?Checkout=" + Checkout.ToString() + "&AddressType=shipping&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString()) + "'";
                lnkAddShippingAddress.NavigateUrl = "selectaddress.aspx?add=true&addressType=Shipping&Checkout=" + Checkout.ToString() + "&returnURL=" + Server.UrlEncode("account.aspx?checkout=" + Checkout.ToString());
                litShippingAddress.Text = ThisCustomer.PrimaryShippingAddress.DisplayHTML(Checkout);
            }

            if (ThisCustomer.PrimaryBillingAddress.PaymentMethod.Length != 0) return;

            litBillingAddress.Text += "<b>" + AppLogic.GetString("account.aspx.9", SkinID, ThisCustomer.LocaleSetting) + "</b><br/>";
            litBillingAddress.Text += ThisCustomer.PrimaryBillingAddress.DisplayPaymentMethod(ThisCustomer);
            //btnOrderHistory.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "orderhistory.gif");
			//change the link to a button- and it's always visible
            //AccountOrderHistoryLink.Visible = true;
        }
    }
}