using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceGateways;

namespace InterpriseSuiteEcommerce
{
    public partial class createaccount : SkinBase
    {

        #region Variable Declaration

        private bool _checkOutMode = false;
        private bool _skipRegistration = false;
        private InterpriseShoppingCart _cart = null;
        private bool isAnonPayPal = false;
        private bool isAnonGoogleCheckout = false;

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PageNoCache();
            RequireSecurePage();
            InitPageContent();
        }

        private void InitPageContent()
        {
        
            SectionTitle = AppLogic.GetString("createaccount.aspx.127", SkinID, ThisCustomer.LocaleSetting, true);

            _checkOutMode = CommonLogic.QueryStringBool("checkout");
            _skipRegistration = CommonLogic.QueryStringBool("skipreg");

            isAnonPayPal = CommonLogic.QueryStringBool("isAnonPayPal");
            isAnonGoogleCheckout = CommonLogic.QueryStringBool("isAnonGoogleCheckout");

            RequireCustomerRecord();
            InitializeShoppingCart();

            CreateAccountHelpfulTips.SetContext = this;
            PerformPageAccessLogic();

            if (AppLogic.AppConfigBool("VAT.Enabled") && AppLogic.AppConfigBool("VAT.ShowTaxFieldOnRegistration"))
            {
                BillingAddressControl.showBusinessTypes = true;
            }


            if (_checkOutMode && _skipRegistration)
            {
                pnlPageHeaderPlaceHolder.Visible = false;
                pnlProductUpdates.Visible = false;
              
                ProfileControl.exCludeAccountName = true;
                ProfileControl.isAnonymousCheckout = true;

                CreateAccountHelpfulTips.TopicName = "AnonymousCheckoutHelpfulTips";

                if (CommonLogic.QueryStringBool("editaddress"))
                {
                   GetAnonymousCustomerProfile();
                }
            }
           
        }

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
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting, true)));
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

        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsSecurityCodeGood(txtCaptcha.Text))
                {
                    errorSummary.DisplayErrorMessage(AppLogic.GetString("createaccount.aspx.126", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
                }
                else
                {
                    RegisterCustomer();

                }

            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }

        private void RegisterCustomer()
        {
            string salutation = ProfileControl.salutation;
            string email = ProfileControl.email;

            bool infoIsAllGood = true;

            if (salutation == AppLogic.GetString("createaccount.aspx.81", AppLogic.GetCurrentSkinID(), Customer.Current.LocaleSetting, true)) salutation = String.Empty;

            if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses") && Customer.EmailInUse(email, ThisCustomer.CustomerCode))
            {
                errorSummary.DisplayErrorMessage("Email is already used by another customer");
                infoIsAllGood = false;
            }

            if (infoIsAllGood)
            {

                #region Customer Profile 
                string contactNumber = ProfileControl.contactNumber;
                ThisCustomer.Salutation = salutation;
                ThisCustomer.FirstName = ProfileControl.firstName;
                ThisCustomer.LastName = ProfileControl.lastName;
                ThisCustomer.EMail = email;
                ThisCustomer.Password = ProfileControl.password;
                ThisCustomer.Phone = contactNumber;

                ThisCustomer.IsOKToEMail = chkProductUpdates.Checked;
                ThisCustomer.IsOver13 = chkOver13.Checked;

                #endregion

                #region Customer Business Type

                if (AppLogic.AppConfigBool("VAT.Enabled") && AppLogic.AppConfigBool("VAT.ShowTaxFieldOnRegistration"))
                {
                    if (BillingAddressControl.businessType.ToLowerInvariant() == Interprise.Framework.Base.Shared.Const.BUSINESS_TYPE_WHOLESALE.ToLower()) ThisCustomer.BusinessType = Customer.BusinessTypes.WholeSale;
                    if (BillingAddressControl.businessType.ToLowerInvariant() == Interprise.Framework.Base.Shared.Const.BUSINESS_TYPE_RETAIL.ToLower()) ThisCustomer.BusinessType = Customer.BusinessTypes.Retail;
                   
                    ThisCustomer.TaxNumber = BillingAddressControl.taxNumber;
                }

                #endregion

                #region Customer Billing Address

                var aBillingAddress = Address.New(ThisCustomer, AddressTypes.Billing);

                var parsedBillingCityText = InterpriseHelper.ParseCityText(billingTxtCityStates.Text, BillingAddressControl.state, BillingAddressControl.city);
                aBillingAddress.State = parsedBillingCityText[0];
                aBillingAddress.City = parsedBillingCityText[1];

                aBillingAddress.Address1 = BillingAddressControl.street;
                aBillingAddress.Country = BillingAddressControl.country;
                aBillingAddress.PostalCode = BillingAddressControl.postal;

                if (!BillingAddressControl.county.IsNullOrEmptyTrimmed()) aBillingAddress.County = BillingAddressControl.county;

                aBillingAddress.FirstName = ProfileControl.firstName;
                aBillingAddress.LastName = ProfileControl.lastName;
                aBillingAddress.EMail = email;
                aBillingAddress.Name = ProfileControl.accountName;
                aBillingAddress.Company = ProfileControl.accountName;
                aBillingAddress.ResidenceType = ResidenceTypes.Residential;
                aBillingAddress.Phone = contactNumber;

                #endregion

                #region Customer Shipping Address 

                var aShippingAddress = Address.New(ThisCustomer, AddressTypes.Shipping);

                if (AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo"))
                {
                    if (copyBillingInfo.Checked)
                    {
                        aShippingAddress.State = parsedBillingCityText[0];
                        aShippingAddress.City = parsedBillingCityText[1];
                        aShippingAddress.Address1 = BillingAddressControl.street;
                        aShippingAddress.Country = BillingAddressControl.country;
                        aShippingAddress.PostalCode = BillingAddressControl.postal;
                        if (!BillingAddressControl.county.IsNullOrEmptyTrimmed()) aShippingAddress.County = BillingAddressControl.county;
                    }
                    else
                    {
                        var parsedShippingCityText = InterpriseHelper.ParseCityText(shippingTxtCityStates.Text, ShippingAddressControl.state, ShippingAddressControl.city);
                        aShippingAddress.State = parsedShippingCityText[0];
                        aShippingAddress.City = parsedShippingCityText[1];
                        aShippingAddress.Address1 = ShippingAddressControl.street;
                        aShippingAddress.Country = ShippingAddressControl.country;
                        aShippingAddress.PostalCode = ShippingAddressControl.postal;
                        if (!ShippingAddressControl.county.IsNullOrEmptyTrimmed()) aShippingAddress.County = ShippingAddressControl.county;
                    }

                    aShippingAddress.FirstName = ProfileControl.firstName;
                    aShippingAddress.LastName = ProfileControl.lastName;
                    aShippingAddress.EMail = email;
                    aShippingAddress.Name = ProfileControl.accountName;
                    aShippingAddress.Company = ProfileControl.accountName;
                    aShippingAddress.ResidenceType = InterpriseHelper.ResolveResidenceType(ShippingAddressControl.residenceType);
                    aShippingAddress.Phone = contactNumber;

                }
                else
                {
                    aShippingAddress = aBillingAddress;
                }

                #endregion

                #region Register Or Checkout 

                if (_checkOutMode && _skipRegistration)
                {
                    SkipRegistrationAndCheckout(aBillingAddress, aShippingAddress);
                }
                else
                {
                    ThisCustomer.Register(aBillingAddress, aShippingAddress, _checkOutMode);
                }

                #endregion

                #region Assign Items Shipping / Mail Notification / Redirection 

                _cart.ShipAllItemsToThisAddress(ThisCustomer.PrimaryShippingAddress);
               
                SendEmailNotification(false, email, ProfileControl.firstName, ProfileControl.accountName);
                RedirectToSucceedingPage();

                #endregion

            }

        }

        private void SendEmailNotification(bool _skipRegistration, string email, string firstName, string accountName)
        {
            if (AppLogic.AppConfigBool("SendWelcomeEmail") && (!_skipRegistration))
            {
				AppLogic.SendMail(
                AppLogic.GetString("createaccount.aspx.27", Customer.Current.SkinID, Customer.Current.LocaleSetting, true),
                AppLogic.RunXmlPackage(AppLogic.AppConfig("XmlPackage.WelcomeEmail"), null, Customer.Current, Customer.Current.SkinID, string.Empty, AppLogic.MakeXmlPackageParamsFromString("fullname=" + accountName), false, false),
                true,
                AppLogic.AppConfig("MailMe_FromAddress"),
                AppLogic.AppConfig("MailMe_FromName"),
                email,
                CommonLogic.IIF(Customer.Current.IsRegistered, firstName, accountName),
                "",
                AppLogic.AppConfig("MailMe_Server")
				);
			}
        }

        protected bool IsSecurityCodeGood(string code)
        {

            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccount")) return true;

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
                else if (isAnonGoogleCheckout)
                {
                    _cart.BuildSalesOrderDetails(false, false);
                    redirectUrl = GoogleCheckout.CreateGoogleCheckout(_cart);
                }
                else if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                {
                    redirectUrl = "checkout1.aspx";
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

        #region SkipRegistrationAndCheckout

        private void SkipRegistrationAndCheckout(Address _billingAddress, Address _shippingAddress)
        {
            string name = ProfileControl.accountName;
            if (string.IsNullOrEmpty(name)) name = string.Format("{0} {1}", ProfileControl.firstName, ProfileControl.lastName);

            ThisCustomer.EMail = ProfileControl.email;

            _billingAddress.Name = name;
            _billingAddress.EMail = ProfileControl.email;
          
            _shippingAddress.EMail = ProfileControl.email;
            _shippingAddress.Name = name;

            if (AppLogic.AppConfigBool("RequireOver13Checked") && ThisCustomer.IsNotRegistered)
            {
                ThisCustomer.IsOver13 = chkOver13.Checked;
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

            ServiceFactory.GetInstance<ICustomerService>().UpdateAnonymousCustomerNotes();

            SendEmailNotificationAndRedirect();
        }


        private void SendEmailNotificationAndRedirect()
        {
            SendEmailNotification(_skipRegistration, ProfileControl.email, ProfileControl.firstName, ProfileControl.lastName);
            RedirectToSucceedingPage();
        }

        #endregion

        #region Get Anonymous Customer Profile

        private void GetAnonymousCustomerProfile()
        {

            if (IsPostBack) return;

            // profile

            string customerName = ThisCustomer.PrimaryBillingAddress.Name.Trim();
            string firstName = String.Empty;
            string lastName = String.Empty;

            if (!customerName.IsNullOrEmptyTrimmed())
            {
                var name = customerName.Split(' ');
                firstName = CommonLogic.IIF(name.Length > 1, name[0], customerName);
                lastName = customerName.Substring(firstName.Length, customerName.Length - firstName.Length).Trim();
            }


            string contactNumber = ThisCustomer.PrimaryBillingAddress.Phone;

            ProfileControl.firstName = firstName;
            ProfileControl.lastName = lastName;
            ProfileControl.email = ThisCustomer.PrimaryBillingAddress.EMail;
            ProfileControl.contactNumber = contactNumber;

            // billing info

            string billingState = ThisCustomer.PrimaryBillingAddress.State;
            string billingCity = ThisCustomer.PrimaryBillingAddress.City;

            BillingAddressControl.postal = ThisCustomer.PrimaryBillingAddress.PostalCode;
            BillingAddressControl.street = ThisCustomer.PrimaryBillingAddress.Address1;
            BillingAddressControl.state = billingState;
            BillingAddressControl.city = billingCity;

            BillingAddressControl.countrySelected = ThisCustomer.PrimaryBillingAddress.Country;
            billingTxtCityStates.Text = String.Format("{0},{1}", billingState, billingCity);

            // shipping info

            string shippingState = ThisCustomer.PrimaryShippingAddress.State;
            string shippingCity = ThisCustomer.PrimaryShippingAddress.City;

            ShippingAddressControl.postal = ThisCustomer.PrimaryShippingAddress.PostalCode;
            ShippingAddressControl.street = ThisCustomer.PrimaryShippingAddress.Address1;
            ShippingAddressControl.state = shippingState;
            ShippingAddressControl.city = shippingCity;

            ShippingAddressControl.countrySelected = ThisCustomer.PrimaryShippingAddress.Country;
            shippingTxtCityStates.Text = String.Format("{0},{1}", shippingState, shippingCity);

            ShippingAddressControl.residenceTypeSelected = ThisCustomer.PrimaryShippingAddress.ResidenceType.ToString();

        }

        #endregion

    }
}
