using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
{
    public partial class checkout1 : SkinBase
    {
        private InterpriseShoppingCart _cart = null;
        private bool _cartHasCouponAndIncludesFreeShipping = false;
        private bool isUsingInterpriseGatewayv2 = false;
        private IEnumerable<PaymentTermDTO> paymentTermOptions = null;
        private bool _isRequirePayment = true;
        private bool _skipCreditCardValidation = false;
        public const string PAYMENT_METHOD_CREDITCARD = DomainConstants.PAYMENT_METHOD_CREDITCARD;

        bool _IsPayPal = false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Initialize(); 
        }

        protected override void OnLoad(EventArgs e)
        {
            DoPassingOfValueWhenTokenizedCreditCard();
            base.OnLoad(e);
        }

        public override void Validate()
        {
            base.Validate();
        }

        private void Initialize()
        {
            ctrlShippingMethod.ThisCustomer = ThisCustomer;
            ctrlPaymentTerm.ThisCustomer = ThisCustomer;

            RequireSecurePage();
            RequireCustomerRecord();

            _IsPayPal = (CommonLogic.QueryStringCanBeDangerousContent("PayPal") == bool.TrueString && CommonLogic.QueryStringCanBeDangerousContent("token") != null);

            if (ThisCustomer.IsInEditingMode())
            {
                PageNoCache();
            }
            else
            {
                SetCacheability();
            }

            isUsingInterpriseGatewayv2 = AppLogic.IsUsingInterpriseGatewayv2();
            OnePageCheckoutHelpfulTips.SetContext = this;

            if (ThisCustomer.IsNotRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                Response.Redirect("createaccount1.aspx?checkout=true");
            }

            var script = new StringBuilder();

            script.Append("<script type=\"text/javascript\" language=\"Javascript\" >\n");
            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.Pages.CheckOutPayment.setPaymentTermControlId('{0}');\n", this.ctrlPaymentTerm.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutPayment.setForm('{0}');\n", this.OnePageCheckout.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
            SectionTitle = AppLogic.GetString("checkoutpayment.aspx.9", SkinID, ThisCustomer.LocaleSetting);

            InitializeShoppingCart();
            PerformPageAccessLogic();
            CheckWhetherToRequireShipping();
            InitializeShippingMethodControl();
            InitializePaymentTermControl(IsCreditCardTokenizationEnabled);

            if (IsCreditCardTokenizationEnabled)
            {
                LitTokenizationFlag.Text = "true";
                CreditCardOptionsRenderer();
            }
            else
            {
                LitTokenizationFlag.Text = "false";
            }

            if (ThisCustomer.IsRegistered)
            {
                ShippingAddressGridRenderer();
            }

            if (_cart.CartItems.Count > 0)
            {
                CheckIfWeShouldRequirePayment();
            }

            DisplayErrorMessageIfAny();

            string couponCode = string.Empty;
            if (!ThisCustomer.CouponCode.IsNullOrEmptyTrimmed()) couponCode = ThisCustomer.CouponCode;

            OrderSummary.Text = _cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Shipping, "page.checkout1ordersummary.xml.config", couponCode));

        }

        private void SetCacheability()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-template.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-oversized-template.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/shippingmethod_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));

            manager.Scripts.Add(new ScriptReference("~/jscripts/tooltip.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/creditcard.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/paymentterm_ajax.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/checkoutpayment_ajax.js"));

            var service = new ServiceReference("~/actionservice.asmx");
            service.InlineScript = false;
            manager.Services.Add(service);
        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
            bool computeVat = AppLogic.AppConfigBool("Vat.Enabled");

            if (_cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (_cart.CartItems.Count > 0)
            {
                try
                {
                    _cart.BuildSalesOrderDetails();
                    _cartHasCouponAndIncludesFreeShipping = _cart.CouponIncludesFreeShipping(ThisCustomer.CouponCode);
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message == AppLogic.GetString("shoppingcart.cs.35", 1, ThisCustomer.LocaleSetting))
                    {
                        Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                    }
                    else { throw ex; }
                }
                catch (Exception ex) { throw ex; }
            }
            else { Response.Redirect("shoppingcart.aspx"); }



            if (_cart.HasRegistryItems()) Response.Redirect("checkoutpayment.aspx");

            string couponCode = string.Empty;
            string error = string.Empty;

            bool hasCoupon = _cart.HasCoupon(ref couponCode);
            if (hasCoupon && _cart.IsCouponValid(ThisCustomer, couponCode, ref error))
            {
                pnlCoupon.Visible = true;
                litCouponEntered.Text = couponCode;
            }
            else
            {
                pnlCoupon.Visible = false;
                if (!error.IsNullOrEmptyTrimmed())
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                }
            }

        }

        private void InitializeShippingMethodControl()
        {
            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || !_cart.HasShippableComponents())
            {
                ctrlShippingMethod.SkipShipping = true;
                pnlShippingMethod.Visible = false;
            }
            else
            {
                if (_cartHasCouponAndIncludesFreeShipping)
                {
                    ctrlShippingMethod.Visible = false;
                }
                else
                {
                    InitializeShippingMethodControlValues();
                    InitializeShippingMethodCaptions();
                }

                InitializeShippingMethodCaptions();
            }

        }

        private void InitializeShippingMethodControlValues()
        {
            ctrlShippingMethod.ShippingAddressID = (_cart.OnlyShippingAddressIsNotCustomerDefault()) ? _cart.FirstItem().m_ShippingAddressID : ThisCustomer.PrimaryShippingAddressID;
            ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
            ctrlShippingMethod.ShippingMethodRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
        }

        private void InitializeShippingMethodCaptions()
        {
            if (!_cart.CartAllowsShippingMethodSelection) return;

            if (_cartHasCouponAndIncludesFreeShipping)
            {
                lblSelectShippingMethod.Text = AppLogic.GetString("checkoutshipping.aspx.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            else
            {
                if (ThisCustomer.IsRegistered && Shipping.MultiShipEnabled() && _cart.TotalQuantity() > 1)
                {
                    lblSelectShippingMethod.Text = string.Format(AppLogic.GetString("checkoutshipping.aspx.7", SkinID, ThisCustomer.LocaleSetting), "checkoutshippingmult.aspx");
                }
                else
                {
                    lblSelectShippingMethod.Text = AppLogic.GetString("checkout1.aspx.4", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                }
            }
        }

        private void InitializePaymentTermControl(bool IsTokenization)
        {
            bool hidePaypalOptionIfMultiShipAndHasGiftRegistry = !(_cart.HasMultipleShippingAddresses() || _cart.HasRegistryItems());
            ctrlPaymentTerm.ShowPaypalPaymentOption = hidePaypalOptionIfMultiShipAndHasGiftRegistry;
            paymentTermOptions = PaymentTermDTO.GetAllForGroup(ThisCustomer.ContactCode, ThisCustomer.PrimaryShippingAddress); //availableTerms;

            ctrlPaymentTerm.PaymentTermOptions = paymentTermOptions; ;
            ctrlPaymentTerm.ShowCardStarDate = AppLogic.AppConfigBool("ShowCardStartDateFields");

            if (IsTokenization)
            {
                ctrlPaymentTerm.IsTokenization = true;
                ctrlPaymentTerm.IsInOnePageCheckOut = true;
            }

            AssignPaymentTermDatasources();
            InitializePaymentTermControlValues();
            AssignPaymentTermCaptions();
            AssignPaymentTermErrorSummary();
            AssignPaymentTermValidationPrerequisites();
            InitializeTermsAndConditions();
        }

        private void AssignPaymentTermDatasources()
        {
            int currentYear = DateTime.Now.Year;
            var startYears = new List<string>();
            var expirationYears = new List<string>();
            DataView cardTypeViewDataSource = AppLogic.GetCustomerCreditCardType(AppLogic.GetString("address.cs.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));

            startYears.Add(AppLogic.GetString("address.cs.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
            expirationYears.Add(AppLogic.GetString("address.cs.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));

            for (int offsetYear = 0; offsetYear <= 10; offsetYear++)
            {
                startYears.Add((currentYear - offsetYear).ToString());
                expirationYears.Add((currentYear + offsetYear).ToString());
            }

            var months = new List<string>();
            months.Add(AppLogic.GetString("address.cs.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
            for (int month = 1; month <= 12; month++)
            {
                months.Add(month.ToString().PadLeft(2, '0'));
            }

            ctrlPaymentTerm.CardTypeViewDataSource = cardTypeViewDataSource;
            ctrlPaymentTerm.StartYearDataSource = startYears;
            ctrlPaymentTerm.ExpiryYearDataSource = expirationYears;
            ctrlPaymentTerm.StartMonthDataSource = months;
            ctrlPaymentTerm.ExpiryMonthDataSource = months;
        }

        private void InitializePaymentTermControlValues()
        {
            if (!ThisCustomer.IsRegistered) return;

            ctrlPaymentTerm.NameOnCard = ThisCustomer.PrimaryBillingAddress.CardName;
        }

        private void AssignPaymentTermCaptions()
        {
            int customerSkinID = Customer.Current.SkinID;
            string customerLocaleSetting = Customer.Current.LocaleSetting;

            var resource = ResourceProvider.GetPaymentTermControlDefaultResources();
            resource.NameOnCardCaption = AppLogic.GetString("checkout1.aspx.50", customerSkinID, customerLocaleSetting);
            resource.NoPaymentRequiredCaption = AppLogic.GetString("checkout1.aspx.58", customerSkinID, customerLocaleSetting);
            resource.CardNumberCaption = AppLogic.GetString("checkout1.aspx.51", customerSkinID, customerLocaleSetting);
            resource.CVVCaption = AppLogic.GetString("checkout1.aspx.52", customerSkinID, customerLocaleSetting);
            resource.WhatIsCaption = AppLogic.GetString("checkout1.aspx.55", customerSkinID, customerLocaleSetting);
            resource.CardTypeCaption = AppLogic.GetString("checkout1.aspx.53", customerSkinID, customerLocaleSetting);
            resource.CardStartDateCaption = AppLogic.GetString("checkout1.aspx.22", customerSkinID, customerLocaleSetting);
            resource.ExpirationDateCaption = AppLogic.GetString("checkout1.aspx.54", customerSkinID, customerLocaleSetting);
            resource.CardIssueNumberCaption = AppLogic.GetString("checkout1.aspx.23", customerSkinID, customerLocaleSetting);
            resource.CardIssueNumberInfoCaption = AppLogic.GetString("checkout1.aspx.24", customerSkinID, customerLocaleSetting);
            resource.SaveCardAsCaption = AppLogic.GetString("checkout1.aspx.56", customerSkinID, customerLocaleSetting);
            resource.SaveThisCreditCardInfoCaption = AppLogic.GetString("checkout1.aspx.57", customerSkinID, customerLocaleSetting);
            resource.PONumberCaption = AppLogic.GetString("checkout1.aspx.59", customerSkinID, customerLocaleSetting);
            resource.PaypalExternalCaption = AppLogic.GetString("checkout1.aspx.60", customerSkinID, customerLocaleSetting);
            ctrlPaymentTerm.LoadStringResources(resource);

        }

        private void AssignPaymentTermErrorSummary()
        {
            ctrlPaymentTerm.ErrorSummaryControl = this.errorSummary;
        }

        private void AssignPaymentTermValidationPrerequisites()
        {
            ctrlPaymentTerm.PaymentTermRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.NameOnCardRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.CardNumberRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.CVVRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.13", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.CardTypeInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.StartMonthInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.25", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.StartYearInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.26", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.ExpirationYearInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.UnknownCardTypeErrorMessage = AppLogic.GetString("checkout1.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.NoCardNumberProvidedErrorMessage = AppLogic.GetString("checkout1.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.CardNumberInvalidFormatErrorMessage = AppLogic.GetString("checkout1.aspx.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.CardNumberInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.CardNumberInAppropriateNumberOfDigitsErrorMessage = AppLogic.GetString("checkout1.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlPaymentTerm.StoredCardNumberInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.27", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
        }

        private void InitializeTermsAndConditions()
        {
            if (AppLogic.AppConfigBool("RequireTermsAndConditionsAtCheckout"))
            {
                Topic t = new Topic("checkouttermsandconditions", ThisCustomer.LocaleSetting, ThisCustomer.SkinID);
                string resouce1 = AppLogic.GetString("checkoutpayment.aspx.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

                ctrlPaymentTerm.RequireTermsAndConditions = true;
                ctrlPaymentTerm.RequireTermsAndConditionsPrompt = resouce1;
                ctrlPaymentTerm.TermsAndConditionsHTML = t.Contents;
            }
            else
            {
                ctrlPaymentTerm.RequireTermsAndConditions = false;
            }
        }

        private void CheckIfWeShouldRequirePayment()
        {

            if (_cart.GetOrderBalance() == System.Decimal.Zero && AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
            {
                ctrlPaymentTerm.NoPaymentRequired = true;
                _cart.MakePaymentTermNotRequired();
                _isRequirePayment = false;
            }
            else
            {
                ctrlPaymentTerm.NoPaymentRequired = false;
                _isRequirePayment = true;
            }

        }

        private void CreditCardOptionsRenderer()
        {

            if (ThisCustomer.IsRegistered)
            {

                if (!this.IsPostBack)
                {
                    AppLogic.GenerateCreditCardCodeSaltIV(ThisCustomer);
                }

                var creditCards = CreditCardDTO.GetCreditCards(ThisCustomer.CustomerCode);

                StringBuilder _CreditOptionsHTML = new StringBuilder();
                string _default = "---";

                _CreditOptionsHTML.Append("<span class='strong-font'>Saved Credit Card Info</span>");
                _CreditOptionsHTML.Append("<div class='clear-both height-12'></div>");


                _CreditOptionsHTML.Append("<div id='credit-card-options-wrapper'>");

                _CreditOptionsHTML.Append("<div id='credit-card-options-header-wrapper'>");

                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left custom-font-style'>{0}</div>", string.Empty);
                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-name-place-holder float-left custom-font-style'>{0}</div>", AppLogic.GetString("checkoutpayment.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left custom-font-style'>{0}</div>", AppLogic.GetString("address.cs.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left custom-font-style'>{0}</div>", AppLogic.GetString("checkoutpayment.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left custom-font-style'>{0}</div>", string.Empty);

                _CreditOptionsHTML.Append("</div>");

                _CreditOptionsHTML.Append("<div class='clear-both'></div>");

                int counter = 1;
                string thisOption = string.Empty;
                string isPrimaryAddress = String.Empty;

                foreach (CreditCardDTO credit in creditCards)
                {

                    if (credit.CreditCardCode == ThisCustomer.PrimaryBillingAddress.AddressID)
                    {
                        thisOption = "checked";
                        isPrimaryAddress = "class='is-primary-address'";
                    }
                    else
                    {
                        thisOption = String.Empty;
                        isPrimaryAddress = String.Empty;
                    }

                    string creditCardCode = AppLogic.EncryptCreditCardCode(ThisCustomer, credit.CreditCardCode);
                    string creditOption = string.Format("<input type='radio' id='{2}' {1} name='credit-card-code' value = '{0}'/>", creditCardCode, thisOption, counter);

                    string description = string.Empty;

                    if (credit.RefNo > 0)
                    {
                        description = credit.Description;
                    }

                    _CreditOptionsHTML.Append("<div class='opc-credit-card-options-row'>");

                    _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left'>{0}</div>", creditOption);
                    _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-name-place-holder float-left'>{0}{1}</div>", credit.NameOnCard, string.Format("<div class='clr5'></div><div id='{1}-credit-card-description'>{0}</div>", description, counter));

                    if (credit.RefNo > 0)
                    {

                        string lastFour = string.Empty;

                        if (credit.CardNumber.Length > 0)
                        {
                            lastFour = credit.CardNumber.Substring(credit.CardNumber.Length - 4);
                            lastFour = string.Format("&nbsp;<span class=\"MaskNumber\">ending in {0}</span>", lastFour);
                        }

                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left' id='{2}-credit-card-type'>{0} {1}</div>", credit.CardType, lastFour, counter);
                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left'  id='{1}-credit-card-expiry'>{0}</div>", string.Format("{0}/{1}", credit.ExpMonth, credit.ExpYear), counter);
                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left' id='{1}-credit-card-clear'>{0}</div>", string.Format("<a class='opc-clearcard' id='opc::{0}::{1}' href='javascript:void(1);'>Clear</a>", creditCardCode, counter), counter);

                    }
                    else
                    {

                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-type-place-holder float-left'>{0}</div>", _default);
                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-expiry-place-holder float-left'>{0}</div>", _default);
                        _CreditOptionsHTML.AppendFormat("<div class='opc-options-credit-card-clear-link-place-holder float-left'>{0}</div>", _default);
                    }


                    _CreditOptionsHTML.Append("</div>");

                    _CreditOptionsHTML.Append("<div class='clear-both'></div>");

                    counter++;

                }

                _CreditOptionsHTML.Append("</div>");
                _CreditOptionsHTML.Append("<div class='clear-both height-12'></div>");

                LtrCreditCardOptionsRenderer.Text = _CreditOptionsHTML.ToString();

            }
        }

        private void ShippingAddressGridRenderer()
        {

            SqlConnection con = DB.NewSqlConnection();

            try
            {
                StringBuilder gridLayout = new StringBuilder();
                con.Open();

                int counter = 1;
                string thisOption = string.Empty;

                string sql = string.Format("exec EcommerceGetAddressList @CustomerCode = {0}, @AddressType = {1}, @ContactCode = {2} ", DB.SQuote(ThisCustomer.CustomerCode), 2, DB.SQuote(ThisCustomer.ContactCode));

                IDataReader reader = DB.GetRSFormat(con, sql);

                gridLayout.Append("<div id='billing-address-options-wrapper'>");

                gridLayout.Append("<div id='credit-card-options-header-wrapper'>");

                gridLayout.AppendFormat("<div class='opc-options-credit-card-code-place-holder float-left custom-font-style'>{0}</div>", string.Empty);
                gridLayout.AppendFormat("<div class='option-billing-account-name-place-holder float-left custom-font-style'>{0}</div>", AppLogic.GetString("address.cs.25", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                gridLayout.AppendFormat("<div class='option-billing-country-place-holder float-left custom-font-style'>{0}</div>", AppLogic.GetString("createaccount.aspx.97", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                gridLayout.AppendFormat("<div class='option-billing-address-place-holder float-left custom-font-style'>{0}</div>", AppLogic.GetString("customersupport.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

                gridLayout.Append("</div>");

                gridLayout.Append("<div class='clear-both'></div>");

                while (reader.Read())
                {

                    thisOption = CommonLogic.IIF((int)reader["PrimaryAddress"] == 1, "checked", String.Empty);

                    string creditOption = String.Empty;
                    string contactName = String.Empty;
                    string address = String.Empty;

                    creditOption = String.Format("<input type='radio' id='{2}' {1} name='multiple-shipping-address' value = '{0}'/>", reader["AddressID"], thisOption, counter);

                    gridLayout.Append("<div class='multiple-address-options-row'>");

                    gridLayout.AppendFormat("<div class='multiple-address-options-control-column float-left'>{0}</div>", creditOption);
                    gridLayout.AppendFormat("<div class='multiple-address-options-account-name-column float-left custom-font-style'>{0}</div>", reader["Name"].ToString());
                    gridLayout.AppendFormat("<div class='multiple-address-options-country-column float-left custom-font-style'>{0}</div>", reader["Country"].ToString());
                    gridLayout.AppendFormat("<div class='multiple-address-options-street-colum float-left custom-font-style'>{0}</div>", reader["CityStateZip"].ToString());

                    gridLayout.Append("</div>");

                    gridLayout.Append("<div class='clear-both'></div>");

                    counter++;
                }

                gridLayout.Append("</div>");
                gridLayout.Append("<div class='clear-both height-12'></div>");
                litShippingAddressGrid.Text = gridLayout.ToString();

                if ((counter - 1) > 1)
                {
                    litShippingAddressGrid.Visible = true;
                }
                else
                {
                    litShippingAddressGrid.Visible = false;
                }

            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
            finally
            {
                con.Close();
                con.Dispose();

            }

        }

        protected bool IsCreditCardTokenizationEnabled
        {
            get { return isUsingInterpriseGatewayv2 && ThisCustomer.IsRegistered && AppLogic.AppConfigBool("AllowCreditCardInfoSaving"); }
        }

        private void DisplayErrorMessageIfAny()
        {
            string errorMessage = CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg", true);
            DisplayErrorMessageIfAny(errorMessage);
        }

        private void DisplayErrorMessageIfAny(string errorMessage)
        {
            if (CommonLogic.IsStringNullOrEmpty(errorMessage)) return;

            if (errorMessage.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            if (errorMessage == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
            {
                if (AppLogic.AppConfigBool("ShowGatewayError"))
                {
                    errorMessage = ThisCustomer.ThisCustomerSession["LastGatewayErrorMessage"];
                }
                else
                {
                    errorMessage = AppLogic.GetString("checkoutpayment.aspx.cs.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                }
            }

            errorSummary.DisplayErrorMessage(errorMessage);
        }

        protected void btnDoProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessPayment();
            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_cart != null)
            {
                _cart.Dispose();
            }

            base.OnUnload(e);
        }

        protected void ProcessPayment()
        {
            if (!_cart.IsEmpty())
            {
                var isOutOfStockAndPhaseOut = _cart.CartItems.Any(item => item.Status == "P" && item.IsOutOfStock);
                if (isOutOfStockAndPhaseOut) Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }
            if (!_isRequirePayment) { Response.Redirect("checkoutreview.aspx"); }

            bool isCustomerRegistered = Customer.Current.IsRegistered;
            bool isCreditCardTokenizationEnabled = IsCreditCardTokenizationEnabled;

            string paymentMethodFromInput = ctrlPaymentTerm.PaymentMethod;
            string paymentTermCodeFromInput = ctrlPaymentTerm.PaymentTerm;

            #region Save Billing Address

            var aBillingAddress = Address.New(ThisCustomer, AddressTypes.Billing);
            var aShippingAddress = ThisCustomer.PrimaryShippingAddress;

            string email = ThisCustomer.EMail.IsNullOrEmptyTrimmed() ? aShippingAddress.EMail : ThisCustomer.EMail;

            ThisCustomer.EMail = email;
            aBillingAddress.EMail = email;

            string PAYMENT_METHOD_PAYPALX = DomainConstants.PAYMENT_METHOD_PAYPALX;

            if (!copyShippingInfo.Checked)
            {
                aBillingAddress.Address1 = BillingAddressControl.street;
                aBillingAddress.Country = BillingAddressControl.country;
                aBillingAddress.PostalCode = BillingAddressControl.postal;

                string bCityStates = txtCityStates.Text;
                string city = String.Empty;
                string state = String.Empty;

                var cityStateArray = GetCityStateArray();
                aBillingAddress.State = cityStateArray[0];
                aBillingAddress.City = cityStateArray[1];

                aBillingAddress.ResidenceType = aShippingAddress.ThisCustomer.PrimaryShippingAddress.ResidenceType;
                aBillingAddress.Name = txtBillingContactName.Text;
                aBillingAddress.Phone = txtBillingContactNumber.Text;
           
                if (paymentMethodFromInput == PAYMENT_METHOD_CREDITCARD) aBillingAddress.CardName = ctrlPaymentTerm.NameOnCard;
                if (AppLogic.AppConfigBool("Address.ShowCounty")) { aBillingAddress.County = BillingAddressControl.county; }
            }
            else
            {
                aBillingAddress.Address1 = aShippingAddress.Address1;
                aBillingAddress.Country = aShippingAddress.Country;
                aBillingAddress.PostalCode = aShippingAddress.PostalCode;
                aBillingAddress.City = aShippingAddress.City;
                aBillingAddress.State = aShippingAddress.State;
                aBillingAddress.ResidenceType = aShippingAddress.ResidenceType;
                aBillingAddress.Name = aShippingAddress.Name;
                aBillingAddress.Phone = aShippingAddress.Phone;

                if (paymentMethodFromInput == PAYMENT_METHOD_CREDITCARD) aBillingAddress.CardName = aShippingAddress.Name;

                if (AppLogic.AppConfigBool("Address.ShowCounty")) { aBillingAddress.County = aShippingAddress.County; }
            }

            AppLogic.SavePostalCode(aBillingAddress);
            UpdateAnonForAge13();

            #endregion

            //Save Anonymous Customer Email Address in Sales Order Note
            ServiceFactory.GetInstance<ICustomerService>().UpdateAnonymousCustomerNotes();

            #region Payments

            if (_cart.GetOrderBalance() == System.Decimal.Zero && AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
            {
                _cart.MakePaymentTermNotRequired();
            }
            if (paymentTermCodeFromInput.ToString().Trim().Equals("PURCHASE ORDER", StringComparison.InvariantCultureIgnoreCase))
            {
                ThisCustomer.ThisCustomerSession.SetVal("PONumber", ctrlPaymentTerm.PONumber);
            }
            else if (paymentTermCodeFromInput.ToString().Trim().Equals("REQUEST QUOTE", StringComparison.InvariantCultureIgnoreCase))
            {
            }
            else if (paymentMethodFromInput == PAYMENT_METHOD_PAYPALX)
            {
                if (ThisCustomer.IsNotRegistered)
                {
                    //for the address information
                    aBillingAddress.Save();
                    ThisCustomer.PrimaryBillingAddress = aBillingAddress;
                }

                ThisCustomer.ThisCustomerSession["paypalfrom"] = "checkoutpayment";
                Response.Redirect(PayPalExpress.CheckoutURL(_cart));
            }
            else if (paymentMethodFromInput == PAYMENT_METHOD_CREDITCARD)
            {
                //Validate first the inputs
                //triggers the input registered validators.

                if (!IsValid) return;

                if (!_skipCreditCardValidation)
                {
                    //credit card validation
                    if (!IsValidCreditCardInfo()) return;
                }

                #region Posted Data (Credit Card Information)

                string nameOnCard = ctrlPaymentTerm.NameOnCard;
                string cardNumberFromInput = ctrlPaymentTerm.CardNumber;
                string cardTypeFromInput = ctrlPaymentTerm.CardType;
                string cardExpiryYearFromInput = ctrlPaymentTerm.CardExpiryYear;
                string cardExpiryMonthFromInput = ctrlPaymentTerm.CardExpiryMonth;
                string cVVFromInput = ctrlPaymentTerm.CVV;
                string saveCreditCardAsFromInput = ctrlPaymentTerm.CardDescription;

                string cardStartMonth = string.Empty;
                string cardStartYear = string.Empty;
                string cardIssueNumber = string.Empty;

                if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                {
                    cardStartMonth = ctrlPaymentTerm.CardStartMonth;
                    cardStartYear = ctrlPaymentTerm.CardStartYear;
                    cardIssueNumber = ctrlPaymentTerm.CardIssueNumber;
                }

                #endregion

                #region Credit Card Info

                string maskedCardNumber = string.Empty;

                //set the default value of creditCardCode to primary billing address

                string creditCardCode = ThisCustomer.PrimaryBillingAddress.AddressID;

                if (isCustomerRegistered)
                {

                    if (isCreditCardTokenizationEnabled && !txtCode.Text.IsNullOrEmptyTrimmed())
                    {
                        //txtCode.Text - Customer CreditCard code
                        //Override the credit card code if tokenization
                        //decrypt the credit card code from the rendered hidden text box since it is encrypted.

                        creditCardCode = AppLogic.DecryptCreditCardCode(ThisCustomer, txtCode.Text);
                        maskedCardNumber = AppLogic.GetCustomerCreditCardMaskedCardNumber(creditCardCode);
                    }

                    if (maskedCardNumber.StartsWith("X"))
                    {
                        CreditCardDTO credit = null;

                        if (!creditCardCode.IsNullOrEmptyTrimmed())
                        {
                            //set the credit card info using the creditcard code

                            credit = CreditCardDTO.Find(creditCardCode);
                        }

                        //test if the credit card info has been tokenized and saved by the client
                        //if refno > 0 means the credit card has been authorized

                        if (credit.RefNo > 0)
                        {
                            cardNumberFromInput = credit.CardNumber;
                            nameOnCard = credit.NameOnCard;
                            cardTypeFromInput = credit.CardType;
                            cardExpiryMonthFromInput = credit.ExpMonth;
                            cardExpiryYearFromInput = credit.ExpYear;

                            if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                            {
                                cardStartMonth = credit.StartMonth;
                                cardStartYear = credit.StartYear;
                            }

                        }
                    }

                    if (AppLogic.AppConfigBool("Address.ShowCounty")) { aBillingAddress.County = BillingAddressControl.county; }

                }

                if (isCreditCardTokenizationEnabled)
                {
                    bool saveCreditCardInfo = (AppLogic.AppConfigBool("ForceCreditCardInfoSaving") || ctrlPaymentTerm.SaveCreditCreditCardInfo);
                    ThisCustomer.ThisCustomerSession["SaveCreditCardChecked"] = saveCreditCardInfo.ToString();
                    if (saveCreditCardInfo) aBillingAddress.CardDescription = saveCreditCardAsFromInput;
                }


                //Credit card code has default value of primary billing addressid
                //This will be overridden when tokenization

                aBillingAddress.AddressID = creditCardCode;
                aBillingAddress.CardNumber = cardNumberFromInput;

                aBillingAddress.CardType = cardTypeFromInput;
                aBillingAddress.CardExpirationMonth = cardExpiryMonthFromInput;
                aBillingAddress.CardExpirationYear = cardExpiryYearFromInput;
                aBillingAddress.CustomerCode = ThisCustomer.CustomerCode;

                aBillingAddress.Save();
                Address.Update(ThisCustomer, aBillingAddress);

                #endregion

                if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                {
                    //-> Some CCs do not have StartDate, so here we should provide Default if none was supplied.

                    string defaultCardStartMonth = DateTime.Now.Month.ToString();
                    string defaultCardStartYear = DateTime.Now.Year.ToString();

                    aBillingAddress.CardStartMonth = (cardStartMonth != "MONTH") ? cardStartMonth : defaultCardStartMonth;
                    aBillingAddress.CardStartYear = (cardStartYear != "YEAR") ? cardStartYear : defaultCardStartYear;
                    aBillingAddress.CardIssueNumber = cardIssueNumber;

                }

                //-> Capture the credit card number from the payment page and encrypt it so that the gateway can capture from that credit card

                if (!cardNumberFromInput.StartsWith("X"))
                {
                    string salt = String.Empty;
                    string iv = String.Empty;
                    string cardNumberEnc = AppLogic.EncryptCardNumber(cardNumberFromInput, ref salt, ref iv);
                    AppLogic.StoreCardNumberInSession(ThisCustomer, cardNumberEnc, salt, iv);
                }

                if (isCreditCardTokenizationEnabled)
                {

                    InterpriseHelper.MakeDefaultAddress(ThisCustomer.ContactCode, creditCardCode, AddressTypes.Billing);

                    #region "Update Address w/ CreditCardInfo"

                    string thisCardNumber = Interprise.Framework.Base.Shared.Common.MaskCardNumber(aBillingAddress.CardNumber);

                    if (!maskedCardNumber.IsNullOrEmptyTrimmed())
                    {
                        thisCardNumber = maskedCardNumber;
                    }

                    #region Postal Code Handler

                    var parsedPostalCode = InterpriseHelper.ParsePostalCode(aBillingAddress.Country, aBillingAddress.PostalCode);
                    string postal = parsedPostalCode.PostalCode;
                    int plus4 = parsedPostalCode.Plus4;

                    #endregion

                    var sql = new StringBuilder();

                    sql.Append(" UPDATE CustomerCreditCard ");
                    sql.AppendFormat(" SET CreditCardDescription = {0}, MaskedCardNumber = {1}, NameOnCard = {2}, ", saveCreditCardAsFromInput.ToDbQuote(), thisCardNumber.ToDbQuote(), nameOnCard.ToDbQuote());
                    sql.AppendFormat(" Address = {0}, City = {1}, State={2}, ", aBillingAddress.Address1.ToDbQuote(), aBillingAddress.City.ToDbQuote(), aBillingAddress.State.ToDbQuote());

                    if (plus4 == 0)
                    {
                        sql.AppendFormat(" PostalCode = {0}, Country = {1}, Plus4=NULL, ", postal.ToDbQuote(), aBillingAddress.Country.ToDbQuote());
                    }
                    else
                    {
                        sql.AppendFormat(" PostalCode = {0}, Country = {1}, Plus4={2}, ", postal.ToDbQuote(), aBillingAddress.Country.ToDbQuote(), plus4);
                    }

                    sql.AppendFormat(" ExpMonth={0}, ExpYear={1}, Telephone={2}, ", InterpriseHelper.ToInterpriseExpMonth(aBillingAddress.CardExpirationMonth).ToDbQuote(), aBillingAddress.CardExpirationYear.ToDbQuote(), aBillingAddress.Phone.ToDbQuote());
                    sql.AppendFormat(" CreditCardType = {0}, DateModified=getdate() ", aBillingAddress.CardType.ToDbQuote());

                    sql.AppendFormat(" WHERE CreditCardCode={0} ", creditCardCode.ToDbQuote());

                    DB.ExecuteSQL(sql.ToString());
                    sql.Clear();

                    #endregion

                    DB.ExecuteSQL(@"UPDATE Customer SET Creditcardcode={0} WHERE CustomerCode={1}", DB.SQuote(creditCardCode), DB.SQuote(ThisCustomer.CustomerCode));

                    AppLogic.ClearCreditCardCodeInSession(ThisCustomer);

                }

                AppLogic.StoreCardExtraCodeInSession(ThisCustomer, cVVFromInput);
            }
            else
            {
                //handling of non credit card, Paypal, REQUEST QUOTE, PONumber when tokenization is enabled

                if (isCreditCardTokenizationEnabled && !txtCode.Text.IsNullOrEmptyTrimmed())
                {
                    //txtCode.Text - Customer CreditCard code
                    //Override the credit card code if tokenization
                    //decrypt the credit card code from the rendered hidden text box since it is encrypted.

                    var primariBillingAddress = ThisCustomer.PrimaryBillingAddress;
                    primariBillingAddress.Address1 = BillingAddressControl.street;
                    primariBillingAddress.Country = BillingAddressControl.country;
                    primariBillingAddress.PostalCode = BillingAddressControl.postal;

                    var cityStateArray = GetCityStateArray();
                    primariBillingAddress.State = cityStateArray[0];
                    primariBillingAddress.City = cityStateArray[1];
                    primariBillingAddress.Phone = txtBillingContactNumber.Text;
                    primariBillingAddress.Name = txtBillingContactName.Text.Trim();
                    primariBillingAddress.CardName = txtBillingContactName.Text.Trim();

                    if (AppLogic.AppConfigBool("Address.ShowCounty")) { primariBillingAddress.County = BillingAddressControl.county; }

                    Address.Update(ThisCustomer, primariBillingAddress);
                }
                else
                {
                    aBillingAddress.Save();
                    Address.Update(ThisCustomer, aBillingAddress);
                }

            }

            #endregion

            #region Redirect to Confirmation Page or Do Place Order

            RedirectToConfirmationPage(paymentTermCodeFromInput, aBillingAddress, aShippingAddress);

            #endregion
        }

        private void PerformPageAccessLogic()
        {
            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------

            if (ThisCustomer.IsCreditOnHold)
            {
                Response.Redirect("shoppingcart.aspx");
            }

            //added to handle when anonymous checkout and onepagecheckout is on
            if (ThisCustomer.IsRegistered && AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkout.over13required", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)));
            }

            if (ThisCustomer.IsNotRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                Response.Redirect("createaccount.aspx?checkout=true");
            }

            //If current user came from IS, chances are it has no Primary Billing Info! then tried to checkout
            if (ThisCustomer.IsRegistered && ThisCustomer.PrimaryBillingAddressID == string.Empty)
            {
                Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Billing&returnURL=account.aspx");
            }

            SectionTitle = AppLogic.GetString("checkoutshipping.aspx.1", SkinID, ThisCustomer.LocaleSetting, true);

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

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!_cart.IsNoShippingRequired() && (_cart.HasMultipleShippingAddresses()) && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.NumItems() > 1)
            {
                Response.Redirect("checkoutshippingmult.aspx");
            }

        }

        protected void RedirectToConfirmationPage(string paymentTerm, Address billing, Address shipping)
        {

            InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, paymentTerm);
            if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout.UseFinalReviewOrderPage")) Response.Redirect("checkoutreview.aspx");

            string salesOrderCode = string.Empty;
            string receiptCode = string.Empty;
            Gateway gatewayToUse = null;

            string status = _cart.PlaceOrder(gatewayToUse, billing, shipping, ref salesOrderCode, ref receiptCode, true, true, false);

            if (status == AppLogic.ro_3DSecure)
            {
                // If credit card is enrolled in a 3D Secure service (Verified by Visa, etc.)
                Response.Redirect("secureform.aspx");

            }
            if (status == AppLogic.ro_OK)
            {
                ThisCustomer.ClearTransactions(true);
                Response.Redirect(string.Format("orderconfirmation.aspx?ordernumber={0}", Server.UrlEncode(salesOrderCode)));
            }
            else
            {
                ThisCustomer.IncrementFailedTransactionCount();
                if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                {

                    _cart.ClearTransaction();
                    ThisCustomer.ResetFailedTransactionCount();

                    Response.Redirect("orderfailed.aspx");

                }

                if (status == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
                {
                    Response.Redirect("checkout1.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                }

                ThisCustomer.ClearTransactions(false);
                errorSummary.DisplayErrorMessage(status);

            }

        }

        private string[] GetCityStateArray()
        {
            var arrCityState = new string[2];

            if (!txtCityStates.Text.IsNullOrEmptyTrimmed())
            {
                var cityState = txtCityStates.Text.Split(',');
                if (cityState.Length > 1)
                {
                    arrCityState[0] = cityState[0].Trim();
                    arrCityState[1] = cityState[1].Trim();
                }
                else
                {
                    arrCityState[0] = String.Empty;
                    arrCityState[1] = cityState[0].Trim();
                }
            }
            else
            {
                arrCityState[0] = BillingAddressControl.state;
                arrCityState[1] = BillingAddressControl.city;
            }

            return arrCityState;

        }

        private void DoPassingOfValueWhenTokenizedCreditCard()
        {
            if (!ThisCustomer.IsRegistered || !IsCreditCardTokenizationEnabled) return;

            if (txtCode.Text.IsNullOrEmptyTrimmed()) return;

            var defaultBillingAddress = ThisCustomer.PrimaryBillingAddress;

            string addressid = AppLogic.DecryptCreditCardCode(ThisCustomer, txtCode.Text);

            if (addressid.IsNullOrEmptyTrimmed()) return;

            defaultBillingAddress = ThisCustomer.BillingAddresses.FirstOrDefault(a => a.AddressID == addressid);

            var addressDto = CreditCardDTO.Find(addressid);

            if (addressDto.RefNo == 0) return;

            ctrlPaymentTerm.CardExpiryMonth = defaultBillingAddress.CardExpirationMonth;
            ctrlPaymentTerm.CardExpiryYear = defaultBillingAddress.CardExpirationYear;
            ctrlPaymentTerm.CardType = defaultBillingAddress.CardType;
            ctrlPaymentTerm.CardStartMonth = defaultBillingAddress.CardStartMonth;
            ctrlPaymentTerm.CardStartYear = defaultBillingAddress.CardStartYear;

            // set the skip registration for validation

            _skipCreditCardValidation = true;

            // set the credit card control to readonly to bypass validation
            ctrlPaymentTerm.CardNumberControl.ReadOnly = true;
            ctrlPaymentTerm.CCVCControl.ReadOnly = true;
        }

        private void UpdateAnonForAge13()
        {
            if (ThisCustomer.IsRegistered) return;

            int isupdated = 1;
            string updateAnonRecordIfIsover13checked = string.Format("UPDATE EcommerceCustomer SET IsOver13 = 1, IsUpdated = {0} WHERE CustomerID = {1}", DB.SQuote(isupdated.ToString()), DB.SQuote(ThisCustomer.CustomerID.ToString()));
            DB.ExecuteSQL(updateAnonRecordIfIsover13checked);
            ThisCustomer.Update();
        }

        private bool IsValidCreditCardInfo()
        {
            var ccValidator = new CreditCardValidator();

            //-> Validate Expiration Date
            if (!ccValidator.IsValidExpirationDate(string.Concat(ctrlPaymentTerm.CardExpiryYear, ctrlPaymentTerm.CardExpiryMonth)))
            {
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage);
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.ExpirationYearInvalidErrorMessage);
                return false;
            }

            ccValidator.AcceptedCardTypes = ctrlPaymentTerm.CardType;

            if (ccValidator.AcceptedCardTypes.Contains("0"))
            {
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.CardTypeInvalidErrorMessage);
                return false;
            }

            string cardNumber = ctrlPaymentTerm.CardNumber;

            if (((!ccValidator.IsValidCardType(cardNumber) || !ccValidator.ValidateCardNumber(cardNumber))))
            {
                errorSummary.DisplayErrorMessage(ctrlPaymentTerm.CardNumberInvalidErrorMessage);
                return false;
            }

            return true;
        }

        private void CheckWhetherToRequireShipping()
        {
            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || !_cart.HasShippableComponents() || _cart.CouponIncludesFreeShipping(litCouponEntered.Text))
            {
                _cart.MakeShippingNotRequired();

                if (_IsPayPal)
                {
                    InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, PAYMENT_METHOD_CREDITCARD);
                    Response.Redirect("checkoutreview.aspx?PayPal=True&token=" + Request.QueryString["token"]);
                }
            }
        }

    }

}
