// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls.Validators;
using InterpriseSuiteEcommerceControls.Validators.Special;
using InterpriseSuiteEcommerceGateways;

namespace InterpriseSuiteEcommerce.mobile
{
    /// <summary>
    /// Summary description for createaccount.
    /// </summary>
    public partial class createaccount : SkinBase
    {
        #region Variable Declaration

        private bool _checkOutMode = false;
        private bool _skipRegistration = false;
        private InterpriseShoppingCart _cart = null;
        InputValidator _skipRegEmailValidator = null;
        private bool isAnonPayPal = false;

        #endregion

        #region Methods

        #region OnInit

        protected override void OnInit(EventArgs e)
        {
			//for mobile button
            btnContinueCheckout.Click += btnContinueCheckout_Click;
            btnContinueCheckout.Text = AppLogic.GetString("createaccount.aspx.25", SkinID, ThisCustomer.LocaleSetting);

            base.OnInit(e);

            RequireCustomerRecord();
            ResolveQueryStrings();
            InitializeShoppingCart();
            PerformPageAccessLogic();

            InitializeAddressControls();

            ApplySignInOptionIfNotRegistered();
            InitializeHeaderTopic();
            TogglePageMode();
            ToggleButtonMode();
        }

        #endregion

        private void ApplySignInOptionIfNotRegistered()
        {
            if (_checkOutMode)
            {
                Signin.Text = "<p align=\"left\">" + AppLogic.GetString("createaccount.aspx.2", SkinID, ThisCustomer.LocaleSetting) + " <a href=\"signin.aspx?checkout=" + CommonLogic.QueryStringBool("checkout").ToString().ToLowerInvariant() + "&returnURL=" + Server.UrlEncode(CommonLogic.IIF(_checkOutMode, "shoppingcart.aspx?checkout=true", "account.aspx")) + "\"><b>" + AppLogic.GetString("createaccount.aspx.3", SkinID, ThisCustomer.LocaleSetting) + "</b></a>.</p>";
            }
        }

        private void InitializeHeaderTopic()
        {
            CreateAccountPageHeader.SetContext = this;
        }

        #region ResolveQueryStrings

        private void ResolveQueryStrings()
        {
            _checkOutMode = CommonLogic.QueryStringBool("checkout");
            _skipRegistration = CommonLogic.QueryStringBool("skipreg");
            isAnonPayPal = CommonLogic.QueryStringBool("isAnonPayPal");
        }

        #endregion

        #region CheckWhichCountriesWeDontRequirePostalCodes

        private void CheckWhichCountriesWeDontRequirePostalCodes()
        {
            string postalCodeNotRequiredCountries = AppLogic.AppConfig("PostalCodeNotRequiredCountries");
            string[] countriesThatDontRequirePostalCodes = postalCodeNotRequiredCountries.Split(',');
            foreach (string country in countriesThatDontRequirePostalCodes)
            {
                ctrlBillingAddress.PostalCodeOptionalCountryCodes.Add(country.Trim());
                ctrlShippingAddress.PostalCodeOptionalCountryCodes.Add(country.Trim());
            }
        }

        #endregion

        #region CheckIfShouldRequireCaptcha

        private void CheckIfShouldRequireCaptcha()
        {
            if (_checkOutMode && AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccountDuringCheckout") ||
                !_checkOutMode && AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccount"))
            {
                ctrlBillingAddress.RequireCaptcha = true;
                if (!IsPostBack)
                {
                    ctrlBillingAddress.CaptchaImageUrl = "Captcha.ashx?id=1";
                }
                else
                {
                    ctrlBillingAddress.CaptchaImageUrl = "Captcha.ashx?id=2";
                }
            }
        }

        #endregion

        private void InitializeAddressControls()
        {
            LoadAllAvailableCountriesAndAssignRegistriesForAddresses();
			
			//required for mobile process
            ctrlBillingAddress.RenderStateDropDownBasedOnCurrentSelectedCountry();
            ctrlShippingAddress.RenderStateDropDownBasedOnCurrentSelectedCountry();

            LoadAvailableSalutationsForBillingAddress();
            CheckWhichCountriesWeDontRequirePostalCodes();
            AssignAddressCaptions();
            ApplyAddressStyles();
            InitializeShippingAddressDefaultDisplay();
            AssignAddressValidatorPrerequisites();
            AssignHostFormForAddresses();
            AssignAddressErrorSummary();
            SetCopyBillingInfoScript();
            CheckIfShouldRequireCaptcha();
            CheckIfShouldRequireOver13BillingOption();
            CheckIfShouldRequireStrongPassword();
            CheckIfShouldRequireNoDuplicateCustomerEmail();
            ConfigureTaxNumberDisplay();
            CheckIfShouldShowSalutation();

            //Set oktoemail default value to yes.
            ctrlBillingAddress.OkToEmailChecked = true;
            ctrlBillingAddress.ShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
            ctrlShippingAddress.ShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
        }

        private void CheckIfShouldShowSalutation()
        {
            ctrlBillingAddress.RequireSalutation = AppLogic.AppConfigBool("Address.ShowSalutation");
        }

        private void ConfigureTaxNumberDisplay()
        {
            ctrlBillingAddress.RequireBusinessType = AppLogic.AppConfigBool("VAT.Enabled") && AppLogic.AppConfigBool("VAT.ShowTaxFieldOnRegistration");

            if (ctrlBillingAddress.RequireBusinessType)
            {
                ctrlBillingAddress.ChooseBusinessTypeDisplayText = AppLogic.GetString("createaccount.aspx.82", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlBillingAddress.BusinessTypeWholeSaleDisplayText = AppLogic.GetString("createaccount.aspx.79", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlBillingAddress.BusinessTypeRetailDisplayText = AppLogic.GetString("createaccount.aspx.80", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
        }

        private void InitializeShippingAddressDefaultDisplay()
        {
            ctrlShippingAddress.Visible = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo");
                        
            if (ctrlShippingAddress.Visible)
            {
                ctrlShippingAddress.ShowFirstName = false;
                ctrlShippingAddress.ShowLastName = false;
            }
        }

        private void TogglePageMode()
        {
            if (_skipRegistration)
            {
                lblSkipRegistrationEmail.Text = AppLogic.GetString("mobile.createaccount.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                lblSkipRegistrationInfo.Text = AppLogic.GetString("createaccount.aspx.66", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

                pnlSkipRegistrationEmail.Visible = true;

                pnlCheckoutImage.Visible = true;
                CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");

                ctrlBillingAddress.RequireEmail = false;
                ctrlBillingAddress.RequireSalutation = false;
                ctrlBillingAddress.RequirePassword = false;
                ctrlBillingAddress.RequireOkToEmail = false;
                ctrlBillingAddress.RequireOver13 = AppLogic.AppConfigBool("RequireOver13Checked");
                ctrlBillingAddress.ShowFirstName = false;
                ctrlBillingAddress.ShowLastName = false;

                //we don't need the business type for Anon customers
                ctrlBillingAddress.RequireBusinessType = false;
            }
            else
            {
                pnlSkipRegistrationEmail.Visible = false;
            }
        }

        private void ToggleButtonMode()
        {
            btnContinueCheckout.Text = CommonLogic.IIF(_checkOutMode,
                                        AppLogic.GetString("createaccount.aspx.25", ThisCustomer.SkinID, ThisCustomer.LocaleSetting),
                                        AppLogic.GetString("createaccount.aspx.24", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
        }

        private void CheckIfShouldRequireStrongPassword()
        {
            if (AppLogic.AppConfigBool("UseStrongPwd"))
            {
                ctrlBillingAddress.RequireStrongPassword = true;
            }
        }

        private void CheckIfShouldRequireOver13BillingOption()
        {
            ctrlBillingAddress.RequireOver13 = AppLogic.AppConfigBool("RequireOver13Checked");

        }

        private void CheckIfShouldRequireNoDuplicateCustomerEmail()
        {
            if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses"))
            {
                ctrlBillingAddress.RequireNoDuplicateCustomerEmail = true;
            }
        }

        #region AssignAddressCaptions

        private void AssignAddressCaptions()
        {
            AssignBillingAddressCaptions();

            if (AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"))
            {
                AssignShippingAddressCaptions();
            }
        }

        #endregion

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
        }

        #region PerformPageAccessLogic

        private void PerformPageAccessLogic()
        {
            if (!_checkOutMode) return;

            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------

                if (_cart.IsEmpty())
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1");
                }

                if (_cart.InventoryTrimmed)
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting)));
                }

                string couponCode = string.Empty;
                string couponErrorMessage = string.Empty;
                if (_cart.HasCoupon(ref couponCode) && !_cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                }

                if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1");
                }

                if (!_cart.MeetsMinimumOrderWeight(AppLogic.AppConfigUSDecimal("MinOrderWeight")))
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1");
                }

                if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1");
                }

            pnlCheckoutImage.Visible = true;
            CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");
        }

        #endregion

        #region AssignBillingAddressCaptions

        private void AssignBillingAddressCaptions()
        {
            ctrlBillingAddress.AccountNameCaption = CommonLogic.IIF(_skipRegistration, 
                                                                    AppLogic.GetString("mobile.createaccount.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting),
                                                                    AppLogic.GetString("mobile.createaccount.aspx.36", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

            //Any - test if atleast 1 satisfies the condition. If there is. return true.
            bool _postalCodeOptionalCountry = ctrlShippingAddress.PostalCodeOptionalCountryCodes.Any(c => ctrlShippingAddress.CountryCode.Equals(c));
            if (_postalCodeOptionalCountry)
            {
                ctrlBillingAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.30", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlBillingAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.29", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            else
            {
                ctrlBillingAddress.WithOutStatePostalCaption = AppLogic.GetString("mobile.createaccount.aspx.77", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlBillingAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.31", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            } 

            //ctrlBillingAddress.SalutationCaption = AppLogic.GetString("createaccount.aspx.35", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.EmailCaption = AppLogic.GetString("mobile.createaccount.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.OkToEmailCaption = AppLogic.GetString("mobile.createaccount.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.Over13YearsOldCaption = AppLogic.GetString("createaccount.aspx.26", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.SubScriptionOptionCaption = AppLogic.GetString("mobile.createaccount.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.AddressCaption = AppLogic.GetString("mobile.createaccount.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.ResidenceTypeCaption = AppLogic.GetString("address.cs.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.BusinessTypeCaption = AppLogic.GetString("address.cs.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.TaxNumberCaption = AppLogic.GetString("address.cs.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.CountryCaption = AppLogic.GetString("createaccount.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.FirstNameCaption = AppLogic.GetString("mobile.createaccount.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.LastNameCaption = AppLogic.GetString("mobile.createaccount.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PhoneNumberCaption = AppLogic.GetString("mobile.createaccount.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
			
			//for mobile layout
            ctrlBillingAddress.WithCityCaption = AppLogic.GetString("mobile.createaccount.aspx.95", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.WithStateCaption = AppLogic.GetString("mobile.createaccount.aspx.96", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.WithPostalCaption = AppLogic.GetString("mobile.createaccount.aspx.77", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);          
            ctrlBillingAddress.CountyCaption = AppLogic.GetString("createaccount.aspx.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.PasswordCaption = AppLogic.GetString("mobile.createaccount.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PasswordMinLengthCaption = AppLogic.GetString("mobile.createaccount.aspx.4", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.ConfirmPasswordCaption = AppLogic.GetString("mobile.createaccount.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.CaptchaCaption = AppLogic.GetString("signin.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.OkToEmailYesOptionCaption = AppLogic.GetString("createaccount.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.OkToEmailNoOptionCaption = AppLogic.GetString("createaccount.aspx.13", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        #endregion

        #region AssignShippingAddressCaptions

        private void AssignShippingAddressCaptions()
        {
            //Any - test if atleast 1 satisfies the condition. If there is. return true.
            bool _postalCodeOptionalCountry = ctrlShippingAddress.PostalCodeOptionalCountryCodes.Any(c => ctrlShippingAddress.CountryCode.Equals(c));
            if (_postalCodeOptionalCountry)
            {
				//added for mobile resources
                ctrlShippingAddress.WithOutStatePostalCaption = AppLogic.GetString("mobile.createaccount.aspx.77", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlShippingAddress.WithCityCaption = AppLogic.GetString("mobile.createaccount.aspx.95", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlShippingAddress.WithStateCaption = AppLogic.GetString("mobile.createaccount.aspx.96", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlShippingAddress.WithPostalCaption = AppLogic.GetString("mobile.createaccount.aspx.77", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            else
            {
				//added for mobile resources
                ctrlShippingAddress.WithOutStatePostalCaption = AppLogic.GetString("mobile.createaccount.aspx.77", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlShippingAddress.WithCityCaption = AppLogic.GetString("mobile.createaccount.aspx.95", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlShippingAddress.WithStateCaption = AppLogic.GetString("mobile.createaccount.aspx.96", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlShippingAddress.WithPostalCaption = AppLogic.GetString("mobile.createaccount.aspx.77", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }

            ctrlShippingAddress.AccountNameCaption = AppLogic.GetString("mobile.createaccount.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.AddressCaption = AppLogic.GetString("mobile.createaccount.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.ResidenceTypeCaption = AppLogic.GetString("address.cs.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.CountryCaption = AppLogic.GetString("createaccount.aspx.23", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.PhoneNumberCaption = AppLogic.GetString("mobile.createaccount.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.WithOutStateCityCaption = AppLogic.GetString("createaccount.aspx.33", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.CountyCaption = AppLogic.GetString("createaccount.aspx.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        #endregion

        #region AssignAddressValidatorPrerequisites

        private void AssignAddressValidatorPrerequisites()
        {
            AssignBillingAddressValidatorPrerequisites();

            if (AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"))
            {
                AssignShippingAddressValidatorPrerequisites();
            }
        }

        private void AssignBillingAddressValidatorPrerequisites()
        {
            ctrlBillingAddress.FirstNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.37", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.LastNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.38", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.AccountNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.39", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.AddressRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.40", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
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

            ctrlBillingAddress.CityRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.69", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.CityMaximumCharacterLength = 50;
            ctrlBillingAddress.CityMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.73", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.PostalCodeRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.70", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.PostalCodeMaximumCharacterLength = 10;
            ctrlBillingAddress.PostalCodeMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.74", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlBillingAddress.RequireNoDuplicateCustomerEmailErrorMessage = AppLogic.GetString("createaccount.aspx.94", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.CaptchaRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.54", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.CaptchaInvalidErrorMessage = AppLogic.GetString("createaccount.aspx.55", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.StrongPasswordValidationRegularExpression = AppLogic.AppConfig("CustomerPwdValidator");
            ctrlBillingAddress.StrongPasswordValidationErrorMessage = AppLogic.GetString("createaccount.aspx.28", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.RequireBusinessTypeSelectErrorMessage = AppLogic.GetString("createaccount.aspx.78", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlBillingAddress.POBoxAddressNotAllowedErrorMessage = AppLogic.GetString("address.cs.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

        }

        private void AssignShippingAddressValidatorPrerequisites()
        {
            ctrlShippingAddress.FirstNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.56", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.LastNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.57", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.AccountNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.58", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.AddressRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.59", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.PhoneRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.60", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlShippingAddress.FirstNameMaximumCharacterLength = 50;
            ctrlShippingAddress.FirstNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.61", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.LastNameMaximumCharacterLength = 50;
            ctrlShippingAddress.LastNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.62", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.AccountNameMaximumCharacterLength = 100;
            ctrlShippingAddress.AccountNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.63", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.AddressMaximumCharacterLength = 200;
            ctrlShippingAddress.AddressMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.64", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.PhoneMaximumCharacterLength = 50;
            ctrlShippingAddress.PhoneMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.65", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlShippingAddress.CityRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.71", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.CityMaximumCharacterLength = 50;
            ctrlShippingAddress.CityMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.75", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlShippingAddress.PostalCodeRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.72", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlShippingAddress.PostalCodeMaximumCharacterLength = 10;
            ctrlShippingAddress.PostalCodeMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.76", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlShippingAddress.POBoxAddressNotAllowedErrorMessage = AppLogic.GetString("address.cs.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

        }

        #endregion

        #region AssignErrorSummaryDisplayControlForAddresses

        private void AssignErrorSummaryDisplayControlForAddresses()
        {
            ctrlBillingAddress.ErrorSummaryControl = this.InputValidatorySummary1;
            ctrlShippingAddress.ErrorSummaryControl = this.InputValidatorySummary1;
        }
        #endregion

        private void AssignHostFormForAddresses()
        {
            ctrlBillingAddress.HostForm = frmCreateAccount;
            ctrlShippingAddress.HostForm = frmCreateAccount;
        }

        #region SetCopyBillingInfoScript

        private void SetCopyBillingInfoScript()
        {
            chkCopyBillingInfo.Attributes.Add(
                "onclick",
                string.Format(
                        "if(this.checked){{" +
                            "ise.Controls.AddressController.getControl('{0}').setValue(ise.Controls.AddressController.getControl('{1}').getValue());" +
                            "ise.Controls.AddressController.getControl('{0}').setDisabled(true);" +
                        "}} else{{ " + 
                            "ise.Controls.AddressController.getControl('{0}').setDisabled(false);" +
                            "$('#{0}').val('')".FormatWith(hdenStateValue.ClientID) +
                        "}}", ctrlShippingAddress.ClientID, ctrlBillingAddress.ClientID));
        }

        #endregion

        #region LoadAvailableSalutationsForBillingAddress

        private void LoadAvailableSalutationsForBillingAddress()
        {
            var salutations = new List<KeyValuePair<string, string>>();
            salutations.Add(new KeyValuePair<string, string>(AppLogic.GetString("mobile.createaccount.aspx.81", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), string.Empty));

			//for optimized code
            var lstSalutations = AppLogic.GetSystemSalutationsBillingAddress();
            salutations.AddRange(lstSalutations.Select(s => new KeyValuePair<string, string>(s.SalutationDescription, s.SalutationDescription)));
            ctrlBillingAddress.SetSalutations(salutations);
        }

        #endregion

        #region LoadAllAvailableCountriesAndAssignRegistriesForAddresses

        private void LoadAllAvailableCountriesAndAssignRegistriesForAddresses()
        {
            var countries = CountryAddressDTO.GetAllCountries();

            ctrlBillingAddress.Countries = countries;
            ctrlBillingAddress.RegisterCountries = true;

            ctrlShippingAddress.Countries = countries;
            ctrlShippingAddress.RegisterCountries = false;
        }

        #endregion

        #region ApplyAddressStyles

		//not applicable for mobile design
        private void ApplyAddressStyles()
        {
            if (AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"))
            {
                //not applicable for mobile design
                //tblShippingInfo.Attributes["style"] = AppLogic.AppConfig("BoxFrameStyle");
            }
            else
            {
                //pnlShippingInfo.Visible = false;
            }
        }

        private void AssignAddressErrorSummary()
        {
            ctrlBillingAddress.ErrorSummaryControl = this.InputValidatorySummary1;
            ctrlShippingAddress.ErrorSummaryControl = this.InputValidatorySummary1;
        }

        #endregion

        private void SendEmailNotificationAndRedirect()
        {
            SendEmailNotification();
            RedirectToSucceedingPage();
        }

        private void SendEmailNotification()
        {
            if (AppLogic.AppConfigBool("SendWelcomeEmail") && (!_skipRegistration))
            {

                AppLogic.SendMail(
                    AppLogic.GetString("createaccount.aspx.27", ThisCustomer.SkinID, ThisCustomer.LocaleSetting),
                    AppLogic.RunXmlPackage(AppLogic.AppConfig("XmlPackage.WelcomeEmail"), null, ThisCustomer, this.SkinID, string.Empty, AppLogic.MakeXmlPackageParamsFromString("fullname=" + ThisCustomer.Name), false, false),
                    true,
                    AppLogic.AppConfig("MailMe_FromAddress"),
                    AppLogic.AppConfig("MailMe_FromName"),
                    ThisCustomer.EMail,
                    CommonLogic.IIF(ThisCustomer.IsRegistered, ThisCustomer.FirstName, ctrlBillingAddress.AccountName),
                    "",
                    AppLogic.AppConfig("MailMe_Server")
                );
            }
        }

        private void RedirectToSucceedingPage()
        {
            if (_checkOutMode)
            {
                string redirectUrl;

                if (isAnonPayPal)
                {
                    _cart.BuildSalesOrderDetails(false, false);
                    Customer.Current.ThisCustomerSession["paypalfrom"] = "shoppingcart";
                    redirectUrl = PayPalExpress.CheckoutURL(_cart);
                }
                else
                {
                    redirectUrl = "checkoutshipping.aspx";
                }

                Response.Redirect(redirectUrl);
            }
            else
            {
                Response.Redirect("account.aspx");
            }
        }

        private void AssignEmailValidatorOnSkipRegistration()
        {
            if (!_skipRegistration) {return;}

            if (null != _skipRegEmailValidator) {return;}
            
            _skipRegEmailValidator = new RequiredInputValidator(
                                            txtSkipRegistrationEmail,
                                            AppLogic.GetString("createaccount.aspx.67", ThisCustomer.SkinID, ThisCustomer.LocaleSetting),
                                            new RegularExpressionInputValidator(
                                                txtSkipRegistrationEmail,
                                                @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
                                                AppLogic.GetString("createaccount.aspx.68", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)
                                            )
            );

                    _skipRegEmailValidator.Error += InputValidatorySummary1.HandleValidationErrorEvent;

            var validateNoDuplicateEmail = new NoDuplicateCustomerEmailValidator(
                                            txtSkipRegistrationEmail, 
                                            AppLogic.GetString("createaccount.aspx.94", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

                    validateNoDuplicateEmail.Error += InputValidatorySummary1.HandleValidationErrorEvent;

                    Controls.Add(_skipRegEmailValidator);
                    Controls.Add(validateNoDuplicateEmail);
        }

        #endregion

        #region Event Handlers

        #region Page_Load

        protected override void OnLoad(EventArgs e)
        {
            AssignEmailValidatorOnSkipRegistration();

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            if (null != _skipRegEmailValidator)
            {
                Page.Validators.Remove(_skipRegEmailValidator);
            }

            base.OnUnload(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();
            if (_skipRegistration && _checkOutMode)
            {
                Label1.Text = AppLogic.GetString("createaccount.aspx.15", SkinID, ThisCustomer.LocaleSetting);
            }
            SectionTitle = AppLogic.GetString("createaccount.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            StringBuilder script = new StringBuilder();

            script.Append("<script type=\"text/javascript\" >\n");
            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.Pages.CreateAccount.setBillingAddressControlId('{0}');\n", this.ctrlBillingAddress.ClientID);
            script.AppendFormat("   ise.Pages.CreateAccount.setShippingAddressControlId('{0}');\n", this.ctrlShippingAddress.ClientID);
            script.AppendFormat("   ise.Pages.CreateAccount.setForm('{0}');\n", this.frmCreateAccount.ClientID);
            script.AppendFormat("   ise.Pages.CreateAccount.setChkSameAsBillingControlId('{0}');\n", this.chkCopyBillingInfo.ClientID);
            script.AppendFormat("   ise.Controls.AddressController.registerControl('{0}');\n", this.ctrlShippingAddress.ClientID);
            script.AppendFormat("   ise.Controls.AddressController.registerControl('{0}');\n", this.ctrlBillingAddress.ClientID);
            if (AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"))
            {
                script.AppendFormat("   ise.Controls.AddressController.getControl('{0}').setDisabled({1});\n", this.ctrlShippingAddress.ClientID, this.chkCopyBillingInfo.Checked.ToString().ToLower());
            }

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());

            ctrlShippingAddress.SameWithBillingAddress = chkCopyBillingInfo.Checked;

            // If Shipping Address Information is the same with Billing Information
            if (!chkCopyBillingInfo.Checked) {return;}
                
            // Checking if BillingAddress Information is Not Empty
            if (!string.IsNullOrEmpty(ctrlBillingAddress.Address))
                ctrlShippingAddress.BillingAddress = ctrlBillingAddress.Address;

            // Checking if BillingAddress City Information is Not Empty
            if (!string.IsNullOrEmpty(ctrlBillingAddress.City))
                ctrlShippingAddress.BillingCity = ctrlBillingAddress.City;
            else
                ctrlShippingAddress.BillingCity = ctrlShippingAddress.City;                  // get previous value from control

            // Checking if BillingAddress CountryCode Information is Not Empty
            if (!string.IsNullOrEmpty(ctrlBillingAddress.CountryCode))
                ctrlShippingAddress.BillingCountryCode = ctrlBillingAddress.CountryCode;
            else
                ctrlShippingAddress.BillingCountryCode = ctrlShippingAddress.CountryCode;    // get previous value from control  

            // Checking if BillingAddress Phone Information is Not Empty
            if (!string.IsNullOrEmpty(ctrlBillingAddress.PhoneNumber))
                ctrlShippingAddress.BillingPhone = ctrlBillingAddress.PhoneNumber;
            else
                ctrlShippingAddress.BillingPhone = ctrlShippingAddress.PhoneNumber;          // get previous value from control

            // Checking if BillingAddress PostalCode Information is Not Empty
            if (!string.IsNullOrEmpty(ctrlBillingAddress.PostalCode))
                ctrlShippingAddress.BillingPostalCode = ctrlBillingAddress.PostalCode;
            else
                ctrlShippingAddress.BillingPostalCode = ctrlShippingAddress.PostalCode;      // get previous value from control

            // Checking if BillingAddress StateCode Information is Not Empty
            if (!string.IsNullOrEmpty(ctrlBillingAddress.State))
                ctrlShippingAddress.BillingStateCode = ctrlBillingAddress.State;
            else
                ctrlShippingAddress.BillingStateCode = ctrlShippingAddress.State;            // get previous value from control

            // Checking if BillingAddress AccountName Information is Not Empty
            if (!string.IsNullOrEmpty(ctrlBillingAddress.AccountName))
                ctrlShippingAddress.BillingName = ctrlBillingAddress.AccountName;
            else
                ctrlShippingAddress.BillingName = ctrlShippingAddress.AccountName;           // get previous value from control
        }

        #endregion

        #region RegisterScriptsAndServices

		//for mobile layout postal code ajax dropdown
        protected override void OnRenderHeader(object sender, System.IO.TextWriter writer)
        {
            // this is a prerequisite as we can't be sure of the ordering of jscripts called, this will be rendered on the <head> section
            writer.WriteLine("<script type=\"text/javascript\" src=\"js/jquery/jquery-ui.min.js\" ></script>");
            writer.WriteLine("<link  type=\"text/css\" rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/jquery-ui.css\" />");
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/address_ajax.js"));
            manager.Scripts.Add(new ScriptReference("js/createaccount_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx")); 
        }

        #endregion

        #region btnContinueCheckout_Click

        protected void btnContinueCheckout_Click(object sender, EventArgs e)
        {
            this.hidBillCtrl.Value = string.Concat(ctrlBillingAddress.FindControl("WithStateCity").ClientID.ToString(), "*", ctrlBillingAddress.FindControl("WithStatePostalCode").ClientID.ToString(), "*ctrlBillingAddress_WithStateState", "*ctrlBillingAddress_WithoutStateCity", "*ctrlBillingAddress_WithoutStatePostalCode");
            
            if (ctrlShippingAddress.Visible)
            {
                this.hidShipCtrl.Value = string.Concat(ctrlShippingAddress.FindControl("WithStateCity").ClientID.ToString(), "*", ctrlShippingAddress.FindControl("WithStatePostalCode").ClientID.ToString(), "*ctrlShippingAddress_WithStateState", "*ctrlShippingAddress_WithoutStateCity", "*ctrlShippingAddress_WithoutStatePostalCode");
            }

            var billingAddress = ctrlBillingAddress.ExtractAddress(ThisCustomer);
            Address shippingAddress = null;

            if (AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"))
            {
                //if the addresses are allowed to be different, get the address from the shipping control
                shippingAddress = ctrlShippingAddress.ExtractAddress(ThisCustomer, AddressTypes.Shipping);
            }
            else
            {
                //if the addresses are not allowed to be different, the shipping control won't be visible or populated
                //so instead get the address from the billing control.
                shippingAddress = ctrlBillingAddress.ExtractAddress(ThisCustomer, AddressTypes.Shipping);
            }

            var billCountry = CountryAddressDTO.Find(billingAddress.Country);
            var shipCountry = CountryAddressDTO.Find(shippingAddress.Country);

            this.hidBlnWithState.Value = billCountry.withState.ToString();
            this.hidShpWithState.Value = shipCountry.withState.ToString();

            hidCheck.Value = chkCopyBillingInfo.Checked.ToString();
            
            if (this.IsValid)
            {            
                //Checked to see Billing Address Info is the same with Shipping Address Info
                if (!this.chkCopyBillingInfo.Checked)
                {
                    this.hidBillCheck.Value = CommonLogic.CheckIfAddressIsCorrect(billingAddress, billCountry);

                    if (ctrlShippingAddress.Visible)
                    {
                        this.hidShipCheck.Value = CommonLogic.CheckIfAddressIsCorrect(shippingAddress, shipCountry);
                    }      
                }
                else
                {
                    this.hidBillCheck.Value = CommonLogic.CheckIfAddressIsCorrect(billingAddress, billCountry);
                    this.hidShipCheck.Value = string.Empty;
                }

                DisplayErrorIfAny(AddressTypes.Billing);
                DisplayErrorIfAny(AddressTypes.Shipping);

                // Checked if either Billing Address or Shipping Address is Correct
                if (!string.IsNullOrEmpty(this.hidBillCheck.Value) || !string.IsNullOrEmpty(this.hidShipCheck.Value))
                {
                    // Set the PopUp window to Show Up
                    hidValid.Value = "false";

                    hidSkinID.Value = ThisCustomer.SkinID.ToString();
                    hidLocale.Value = ThisCustomer.LocaleSetting.ToString();

                    // Collections of Caption/Labels/Headers, having ':' as its separator which be parsed at the receving end.
                    this.hidBillTitle.Value = string.Concat(AppLogic.GetString("createaccount.aspx.87", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                    this.hidShipTitle.Value = string.Concat(AppLogic.GetString("createaccount.aspx.88", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

                    this.hidBlnState.Value = billingAddress.State;
                    this.hidBlnPostalCode.Value = billingAddress.PostalCode;
                    this.hidBlnCountry.Value = billingAddress.Country;
                    this.hidBlnCity.Value = billingAddress.City;

                    this.hidShpState.Value = shippingAddress.State;
                    this.hidShpPostalCode.Value = shippingAddress.PostalCode;
                    this.hidShpCountry.Value = shippingAddress.Country;
                    this.hidShpCity.Value = shippingAddress.City;

                }
                else
                {
                    if (chkCopyBillingInfo.Checked)
                    {
                        shippingAddress.CopyAddressDetails(billingAddress);
                    }

                    if (_skipRegistration)
                    {
                        SkipRegistrationAndCheckout(billingAddress, shippingAddress);
                    }
                    else
                    {
                        CreateCustomerAccount(billingAddress, shippingAddress);
                    }
                }

            }
            else
            {
                // just reset the password
                ctrlBillingAddress.Password = ctrlBillingAddress.Password;
                ctrlBillingAddress.ConfirmedPassword = ctrlBillingAddress.ConfirmedPassword;
                if (AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccount"))
                {
                    ThisCustomer.ThisCustomerSession["SecurityCode"] = CommonLogic.GenerateRandomCode(6);
                }
            }
        }

        void DisplayErrorIfAny(AddressTypes typeToValidate)
        {
            WebControl cityTextBoxControl = null; 
            WebControl postalTextBoxControl = null;

            if (typeToValidate == AddressTypes.Billing)
            {
                cityTextBoxControl = (ctrlBillingAddress.FindControl("WithStateCity") as WebControl);
                postalTextBoxControl = (ctrlBillingAddress.FindControl("WithStatePostalCode") as WebControl);
            }
            else
            {
                cityTextBoxControl = (ctrlShippingAddress.FindControl("WithStateCity") as WebControl);
                postalTextBoxControl = (ctrlShippingAddress.FindControl("WithStatePostalCode") as WebControl);
            }

            if (this.hidBillCheck.Value.IsNullOrEmptyTrimmed() && this.hidShipCheck.Value.IsNullOrEmptyTrimmed()) return;

            cityTextBoxControl.CssClass = cityTextBoxControl.CssClass.Replace(" mobile-text-error", String.Empty);
            postalTextBoxControl.CssClass = postalTextBoxControl.CssClass.Replace(" mobile-text-error", String.Empty);

            if (typeToValidate == AddressTypes.Billing)
            {
                switch (this.hidBillCheck.Value)
                {
                    case "IsInvalidCityOnly":
                        cityTextBoxControl.CssClass = cityTextBoxControl.CssClass + " mobile-text-error";
                        break;
                    case "IsInvalidPostalOnly":
                        postalTextBoxControl.CssClass = postalTextBoxControl.CssClass + " mobile-text-error";
                        break;
                    case "IsInvalidPostalAndCity":
                        cityTextBoxControl.CssClass = cityTextBoxControl.CssClass + " mobile-text-error";
                        postalTextBoxControl.CssClass = postalTextBoxControl.CssClass + " mobile-text-error";
                        break;
                }

            }
            else
            {
                switch (this.hidShipCheck.Value)
                {
                    case "IsInvalidCityOnly":
                        cityTextBoxControl.CssClass = cityTextBoxControl.CssClass + " mobile-text-error";
                        break;
                    case "IsInvalidPostalOnly":
                        postalTextBoxControl.CssClass = postalTextBoxControl.CssClass + " mobile-text-error";
                        break;
                    case "IsInvalidPostalAndCity":
                        cityTextBoxControl.CssClass = cityTextBoxControl.CssClass + " mobile-text-error";
                        postalTextBoxControl.CssClass = postalTextBoxControl.CssClass + " mobile-text-error";
                        break;
                }
            }
        }

        #endregion

        #region CreateCustomerAccount

        private void CreateCustomerAccount(Address withBillingAddress, Address withShippingAddress)
        {
            bool allowRedirect = true;

            try
            {
                ThisCustomer.Register(withBillingAddress, withShippingAddress, _checkOutMode);
                _cart.ShipAllItemsToThisAddress(ThisCustomer.PrimaryShippingAddress);
            }
            catch (Customer.CustomerRegistrationException regEx)
            {
                allowRedirect = false;
                pnlErrorMsg.Controls.Add(new LiteralControl(Security.HtmlEncode(regEx.Message)));
            }

            if (allowRedirect)
            {
                SendEmailNotificationAndRedirect();
            }
        }

        #endregion

        #region SkipRegistrationAndCheckout

        private void SkipRegistrationAndCheckout(Address _billingAddress, Address _shippingAddress)
        {
            ThisCustomer.EMail = txtSkipRegistrationEmail.Text;
            _billingAddress.EMail = txtSkipRegistrationEmail.Text;
            _billingAddress.Name = _billingAddress.Company;
            _shippingAddress.EMail = txtSkipRegistrationEmail.Text;
            _shippingAddress.Name = _shippingAddress.Company;

            if (AppLogic.AppConfigBool("RequireOver13Checked") && ThisCustomer.IsNotRegistered)
            {
                ThisCustomer.IsOver13 = ctrlBillingAddress.Over13Checked;
                int isover13 = CommonLogic.IIF(ThisCustomer.IsOver13, 1, 0);
                int isupdated = 1;
                string updateAnonRecordIfIsover13checked = string.Format("UPDATE EcommerceCustomer SET IsOver13 = {0} , IsUpdated = {1} WHERE CustomerID = {2}", DB.SQuote(isover13.ToString()), DB.SQuote(isupdated.ToString()), DB.SQuote(ThisCustomer.CustomerID.ToString()));
                DB.ExecuteSQL(updateAnonRecordIfIsover13checked);
            }

            ThisCustomer.PrimaryBillingAddress = _billingAddress;
            ThisCustomer.PrimaryShippingAddress = _shippingAddress;

            _billingAddress.Save();
            _shippingAddress.Save();

            _cart.ShipAllItemsToThisAddress(_shippingAddress);

            string orderNote = DB.GetSqlS(String.Format("SELECT Notes AS S from Customer WHERE CustomerCode = {0}", ThisCustomer.CustomerCode.ToDbQuote()));
            if (!orderNote.IsNullOrEmptyTrimmed())
            {
                orderNote += Environment.NewLine + ThisCustomer.EMail;
                if (orderNote.Length >= DomainConstants.ORDER_NOTE_MAX_LENGTH)
                {
                    orderNote = orderNote.Substring(0, DomainConstants.ORDER_NOTE_MAX_LENGTH);
                }
            }
            else
            {
                if (ThisCustomer.EMail.Length > DomainConstants.ORDER_NOTE_MAX_LENGTH)
                {
                    orderNote = ThisCustomer.EMail.Substring(0, DomainConstants.ORDER_NOTE_MAX_LENGTH);
                }
                else
                {
                    orderNote = String.Format("Anonymous Customer: {0}", ThisCustomer.EMail);
                }
            }

            DB.ExecuteSQL(
                String.Format("UPDATE Customer SET Notes = {0} WHERE CustomerCode = {1}",
                orderNote.ToDbQuote(),
                ThisCustomer.AnonymousCustomerCode.ToDbQuote())
            );

            SendEmailNotificationAndRedirect();
        }

        #endregion

        #endregion

    }
}



