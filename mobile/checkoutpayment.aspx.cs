// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls;
using InterpriseSuiteEcommerceGateways;

namespace InterpriseSuiteEcommerce
{
    public partial class checkoutpayment : SkinBase
    {
        InterpriseShoppingCart _cart = null;
        private bool _weShouldRequirePayment = true;

        protected override void OnInit(EventArgs e)
        {
            ctrlPaymentTerm.ThisCustomer = ThisCustomer;
			//for mobile button user control
            btnCompletePurchase.Click += btnCompletePurchase_Click;
            btnCompletePurchase2.Click += btnCompletePurchase_Click;

            base.OnInit(e);

            this.PageNoCache();
            RequireSecurePage();
            RequireCustomerRecord();

            InitializeShoppingCart();
            PerformPageAccessLogic();
            DisplayCheckOutStepsImage();
            DisplayErrorMessageIfAny();
            InitializePaymentTermControl();
            DisplayOrderSummary();
            AssignCheckOutButtonCaption();
            CheckIfWeShouldRequirePayment();
            
        }

        private void DisplayErrorMessageIfAny()
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").Length == 0) return;

            string errorMessage = CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg", true);
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

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
            _cart.BuildSalesOrderDetails();
        }

        private void DisplayOrderSummary()
        {
            if (_cart.HasMultipleShippingAddresses())
            {
                var splittedCarts = _cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();
                splittedCarts.ForEach(splitCart => 
                {
                    splitCart.BuildSalesOrderDetails();
                    OrderSummary.Text += splitCart.RenderHTMLLiteral(new MobileCheckOutPaymentPageLiteralRenderer());
                });
            }
            else
            {
                OrderSummary.Text = _cart.RenderHTMLLiteral(new MobileCheckOutPaymentPageLiteralRenderer());
            }
        }

        private void DisplayCheckOutStepsImage()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_4.gif");
            for (int i = 0; i < checkoutheadergraphic.HotSpots.Count; i++)
            {
                var rhs = checkoutheadergraphic.HotSpots[i] as RectangleHotSpot;
                if (rhs.NavigateUrl.IndexOf("shoppingcart") != -1) rhs.AlternateText = AppLogic.GetString("checkoutpayment.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                if (rhs.NavigateUrl.IndexOf("account") != -1) rhs.AlternateText = AppLogic.GetString("checkoutpayment.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                if (rhs.NavigateUrl.IndexOf("checkoutshipping") != -1) rhs.AlternateText = AppLogic.GetString("checkoutpayment.aspx.4", SkinID, ThisCustomer.LocaleSetting);
            }

            if (!AppLogic.AppConfigBool("SkipShippingOnCheckout"))
            {
                checkoutheadergraphic.HotSpots[2].HotSpotMode = HotSpotMode.Navigate;
                if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                {
                    checkoutheadergraphic.HotSpots[2].NavigateUrl = "checkout1.aspx";
                }
                else
                {
                    checkoutheadergraphic.HotSpots[2].NavigateUrl = CommonLogic.IIF(_cart.HasMultipleShippingAddresses(), "checkoutshippingmult.aspx", "checkoutshipping.aspx");
                }
            }
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
            
            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkout.over13required", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }

            if (ThisCustomer.IsNotRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                Response.Redirect("createaccount.aspx?checkout=true");
            }
            if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryBillingAddressID == String.Empty || ThisCustomer.PrimaryShippingAddressID == String.Empty))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.1", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (_cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (_cart.HasRegistryItems())
            {
                Response.Redirect("shoppingcart.aspx");
            }

            if (_cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting)));
            }
            if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }
        }

        private void AssignCheckOutButtonCaption()
        {
            string value = AppLogic.GetString("checkoutpayment.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            btnCompletePurchase.Text = value;
            btnCompletePurchase2.Text = value;
        }

        private void InitializePaymentTermControl()
        {
            string baseTermOnThisCustomer = ThisCustomer.ContactCode;

            if (ThisCustomer.IsNotRegistered)
            {
                baseTermOnThisCustomer = ThisCustomer.AnonymousCustomerCode;
            }

            bool hidePaypalOptionIfMultiShipAndHasGiftRegistry = !(_cart.HasMultipleShippingAddresses() || _cart.HasRegistryItems());
            ctrlPaymentTerm.ShowPaypalPaymentOption = hidePaypalOptionIfMultiShipAndHasGiftRegistry;
            ctrlPaymentTerm.PaymentTermOptions = PaymentTermDTO.GetAllForGroup(baseTermOnThisCustomer, ThisCustomer.PrimaryShippingAddress); //availableTerms;
            ctrlPaymentTerm.ShowCardStarDate = AppLogic.AppConfigBool("ShowCardStartDateFields");
			//added header text instead of image
            lblCheckOutPaymentHeaderText.Text = AppLogic.GetString("mobile.checkoutpayment.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            AssignPaymentTermDatasources();
            InitializePaymentTermControlValues();
            AssignPaymentTermCaptions();
            AssignPaymentTermErrorSummary();
            AssignPaymentTermValidationPrerequisites();
            InitializeTermsAndConditions();
        }

        private void InitializeTermsAndConditions()
        {
            if(AppLogic.AppConfigBool("RequireTermsAndConditionsAtCheckout"))
            {
                ctrlPaymentTerm.RequireTermsAndConditions = true;
                ctrlPaymentTerm.RequireTermsAndConditionsPrompt = AppLogic.GetString("checkoutpayment.aspx.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

                var t = new Topic("checkouttermsandconditions", ThisCustomer.LocaleSetting, ThisCustomer.SkinID);
                ctrlPaymentTerm.TermsAndConditionsHTML = t.Contents;
            }
            else
            {
                ctrlPaymentTerm.RequireTermsAndConditions = false;
            }            
        }

        private void InitializePaymentTermControlValues()
        {
            if (!ThisCustomer.IsRegistered) return;
            
            ctrlPaymentTerm.NameOnCard = ThisCustomer.PrimaryBillingAddress.CardName;
        }

        private void AssignPaymentTermDatasources()
        {
            var cardTypes = new List<string>();
            cardTypes.Add(AppLogic.GetString("address.cs.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, "SELECT CreditCardType FROM CustomerCreditCardType with (NOLOCK) WHERE IsActive = 1"))
                {
                    while (reader.Read())
                    {
                        cardTypes.Add(DB.RSField(reader, "CreditCardType"));
                    }
                }
            }

            ctrlPaymentTerm.CardTypeDataSource = cardTypes;

            ////---------------------------------
            int currentYear = DateTime.Now.Year;

            var startYears = new List<string>();
            var expirationYears = new List<string>();

            startYears.Add(AppLogic.GetString("address.cs.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            expirationYears.Add(AppLogic.GetString("address.cs.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            for (int offsetYear = 0; offsetYear <= 10; offsetYear++)
            {
                startYears.Add((currentYear - offsetYear).ToString());
                expirationYears.Add((currentYear + offsetYear).ToString());
            }

            ctrlPaymentTerm.StartYearDataSource = startYears;
            ctrlPaymentTerm.ExpiryYearDataSource = expirationYears;

            var months = new List<string>();
            months.Add(AppLogic.GetString("address.cs.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            for (int month = 1; month <= 12; month++)
            {
                months.Add(month.ToString().PadLeft(2, '0'));
            }
            ctrlPaymentTerm.StartMonthDataSource = months;
            ctrlPaymentTerm.ExpiryMonthDataSource = months;
        }

        private void AssignPaymentTermValidationPrerequisites()
        {
            ctrlPaymentTerm.PaymentTermRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.NameOnCardRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.CardNumberRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.CVVRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.13", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.CardTypeInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.StartMonthInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.25", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.StartYearInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.26", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.ExpirationYearInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.UnknownCardTypeErrorMessage = AppLogic.GetString("checkout1.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.NoCardNumberProvidedErrorMessage = AppLogic.GetString("checkout1.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.CardNumberInvalidFormatErrorMessage = AppLogic.GetString("checkout1.aspx.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.CardNumberInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.CardNumberInAppropriateNumberOfDigitsErrorMessage = AppLogic.GetString("checkout1.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlPaymentTerm.StoredCardNumberInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.27", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        private void CheckIfWeShouldRequirePayment()
        {
            if (_cart.GetOrderBalance() == System.Decimal.Zero &&
                AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout"))
            {
                ctrlPaymentTerm.NoPaymentRequired = true;
                _weShouldRequirePayment = false;
                _cart.MakePaymentTermNotRequired();
            }
            else
            {
                ctrlPaymentTerm.NoPaymentRequired = false;
                _weShouldRequirePayment = true;
            }
        }

        private void AssignPaymentTermErrorSummary()
        {
            ctrlPaymentTerm.ErrorSummaryControl = this.errorSummary;
        }

        private void AssignPaymentTermCaptions()
        {
            int customerSkinID = Customer.Current.SkinID;
            string customerLocaleSetting = Customer.Current.LocaleSetting;

            var resource = ResourceProvider.GetMobilePaymentTermControlDefaultResources();
            resource.NameOnCardCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.2", customerSkinID, customerLocaleSetting);
            resource.NoPaymentRequiredCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.15", customerSkinID, customerLocaleSetting);
            resource.CardNumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.3", customerSkinID, customerLocaleSetting);
            resource.CVVCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.4", customerSkinID, customerLocaleSetting);
            resource.WhatIsCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.11", customerSkinID, customerLocaleSetting);
            resource.CardTypeCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.5", customerSkinID, customerLocaleSetting);
            resource.CardStartDateCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.6", customerSkinID, customerLocaleSetting);
            resource.ExpirationDateCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.7", customerSkinID, customerLocaleSetting);
            resource.CardIssueNumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.8", customerSkinID, customerLocaleSetting);
            resource.CardIssueNumberInfoCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.9", customerSkinID, customerLocaleSetting);
            resource.SaveCardAsCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.10", customerSkinID, customerLocaleSetting);
            resource.SaveThisCreditCardInfoCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.12", customerSkinID, customerLocaleSetting);
            resource.PONumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.13", customerSkinID, customerLocaleSetting);
            resource.PaypalExternalCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.14", customerSkinID, customerLocaleSetting);
            ctrlPaymentTerm.LoadStringResources(resource);
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/tooltip.js"));
            manager.Scripts.Add(new ScriptReference("js/creditcard.js"));
            manager.Scripts.Add(new ScriptReference("js/paymentterm_ajax.js"));
            manager.Scripts.Add(new ScriptReference("js/checkoutpayment_ajax.js"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var script = new StringBuilder();

            script.Append("<script type='text/javascript' > \n");
            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.Pages.CheckOutPayment.setPaymentTermControlId('{0}');\n", this.ctrlPaymentTerm.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutPayment.setForm('{0}');\n", this.frmCheckOutPayment.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            SectionTitle = AppLogic.GetString("checkoutpayment.aspx.9", SkinID, ThisCustomer.LocaleSetting);
            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected void btnCompletePurchase_Click(object sender, EventArgs e)
        {
            if (!this.IsValid) return;

            if (_weShouldRequirePayment)
            {
                if (ctrlPaymentTerm.PaymentTerm.ToString().Trim().Equals("PURCHASE ORDER", StringComparison.InvariantCultureIgnoreCase))
                {
                    ThisCustomer.ThisCustomerSession.SetVal("PONumber", ctrlPaymentTerm.PONumber);
                }
                else if (ctrlPaymentTerm.PaymentTerm.ToString().Trim().Equals("REQUEST QUOTE", StringComparison.InvariantCultureIgnoreCase))
                {
                }
                else if (ctrlPaymentTerm.PaymentMethod == DomainConstants.PAYMENT_METHOD_PAYPALX)
                {
                    ThisCustomer.ThisCustomerSession["paypalfrom"] = "checkoutpayment";
                    Response.Redirect(PayPalExpress.CheckoutURL(_cart));
                }
                else if (ctrlPaymentTerm.PaymentMethod == DomainConstants.PAYMENT_METHOD_CREDITCARD)
                {
                    //Validate Card Number
                    bool blnCcInvalid = false;
                    string cardNumber;
                    string cardNumberInvalidErrorMessage;

                    var ccValidator = new CreditCardValidator();
                    ccValidator.AcceptedCardTypes = ctrlPaymentTerm.CardType;
                    if (ccValidator.AcceptedCardTypes.Contains("0"))
                    {
                        ctrlPaymentTerm.CardTypeInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        errorSummary.DisplayErrorMessage(ctrlPaymentTerm.CardTypeInvalidErrorMessage);
                        return;
                    }

                        //See if we should use the card number on file.
                        //We also want to see if the card number starts with an *.
                        //If it doesn't it probably means the user entered a new number.
                        if (ctrlPaymentTerm.CardNumber.StartsWith("*"))
                        {
                            //Get the stored card number.
                            cardNumber = ThisCustomer.PrimaryBillingAddress.CardNumber;
                            cardNumberInvalidErrorMessage = ctrlPaymentTerm.StoredCardNumberInvalidErrorMessage;
                        }
                        else
                        {
                            //Get the card number the user entered.
                            cardNumber = ctrlPaymentTerm.CardNumber;
                            cardNumberInvalidErrorMessage = ctrlPaymentTerm.CardNumberInvalidErrorMessage;
                        }

                    if (!ccValidator.IsValidCardType(cardNumber) || !ccValidator.ValidateCardNumber(cardNumber))
                    {
                        errorSummary.DisplayErrorMessage(cardNumberInvalidErrorMessage);
                        blnCcInvalid = true;
                    }

                    //Validate Expiration Date
                    if (!ccValidator.IsValidExpirationDate(string.Concat(ctrlPaymentTerm.CardExpiryYear, ctrlPaymentTerm.CardExpiryMonth)))
                    {
                        ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        ctrlPaymentTerm.ExpirationYearInvalidErrorMessage = AppLogic.GetString("checkout1.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        errorSummary.DisplayErrorMessage(ctrlPaymentTerm.ExpirationMonthInvalidErrorMessage);
                        errorSummary.DisplayErrorMessage(ctrlPaymentTerm.ExpirationYearInvalidErrorMessage);
                        blnCcInvalid = true;
                    }

                    //If an error was found display them
                    if (blnCcInvalid)
                    {
                        return;
                    }

                    var billingAddress = ThisCustomer.PrimaryBillingAddress;
                    billingAddress.CardNumber = cardNumber;

                    billingAddress.CardName = ctrlPaymentTerm.NameOnCard;
                    billingAddress.CardType = ctrlPaymentTerm.CardType;
                    billingAddress.CardExpirationMonth = ctrlPaymentTerm.CardExpiryMonth;
                    billingAddress.CardExpirationYear = ctrlPaymentTerm.CardExpiryYear;

                    if (AppLogic.AppConfigBool("ShowCardStartDateFields"))
                    {
                        //Some CCs do not have StartDate, so here we should provide Default if none was supplied.
                        string defaultCardStartMonth = DateTime.Now.Month.ToString();
                        string defaultCardStartYear = DateTime.Now.Year.ToString();

                        billingAddress.CardStartMonth = CommonLogic.IIF(ctrlPaymentTerm.CardStartMonth != "MONTH", ctrlPaymentTerm.CardStartMonth, defaultCardStartMonth);
                        billingAddress.CardStartYear = CommonLogic.IIF(ctrlPaymentTerm.CardStartYear != "YEAR", ctrlPaymentTerm.CardStartYear, defaultCardStartYear);

                        billingAddress.CardIssueNumber = ctrlPaymentTerm.CardIssueNumber;
                    }

                    AppLogic.StoreCardExtraCodeInSession(ThisCustomer, ctrlPaymentTerm.CVV);

                    //Capture the credit card number from the payment page and encrypt it so that the gateway can capture from that credit card
                    string salt = null;
                    string iv = null;
                    string cardNumberEnc = AppLogic.EncryptCardNumber(cardNumber, ref salt, ref iv);
                    AppLogic.StoreCardNumberInSession(ThisCustomer, cardNumberEnc, salt, iv);

                    Address.Update(ThisCustomer, billingAddress);
                }

                InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, ctrlPaymentTerm.PaymentTerm);

            }
            Response.Redirect("checkoutreview.aspx");
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_cart != null)
            {
                _cart.Dispose();
            }
            base.OnUnload(e);
        }

    }   

}
