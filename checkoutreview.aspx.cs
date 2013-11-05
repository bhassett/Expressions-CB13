// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for checkoutreview.
    /// </summary>
    public partial class checkoutreview : SkinBase
    {
        InterpriseShoppingCart cart = null;
		bool _PayPalFailed = false;
        PayPalExpress pp;

		protected bool IsPayPalCheckout
		{
			get
			{
				return (Request.QueryString["PayPal"] ?? bool.FalseString) == bool.TrueString && Request.QueryString["token"] != null;
			}
		}

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + AppLogic.GetString("checkout.over13required", ThisCustomer.SkinID, ThisCustomer.LocaleSetting).ToUrlEncode());
            }

            if (ThisCustomer.IsCreditOnHold)
            {
                Response.Redirect("shoppingcart.aspx");
            }

            RequireSecurePage();

            // -----------------------------------------------------------------------------------------------
            // NOTE ON PAGE LOAD LOGIC:
            // We are checking here for required elements to allowing the customer to stay on this page.
            // Many of these checks may be redundant, and they DO add a bit of overhead in terms of db calls, but ANYTHING really
            // could have changed since the customer was on the last page. Remember, the web is completely stateless. Assume this
            // page was executed by ANYONE at ANYTIME (even someone trying to break the cart). 
            // It could have been yesterday, or 1 second ago, and other customers could have purchased limitied inventory products, 
            // coupons may no longer be valid, etc, etc, etc...
            // -----------------------------------------------------------------------------------------------
            ThisCustomer.RequireCustomerRecord();
            if (ThisCustomer.IsNotRegistered && 
                !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && 
                !AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
            {
                Response.Redirect("createaccount.aspx?checkout=true");
            }

            if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryBillingAddressID == String.Empty || ThisCustomer.PrimaryShippingAddressID == String.Empty))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("checkoutpayment.aspx.1", SkinID, ThisCustomer.LocaleSetting).ToUrlEncode());
            }

            SectionTitle = AppLogic.GetString("checkoutreview.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

            if (cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting).ToUrlEncode());
            }

            string couponCode = string.Empty;
            string couponErrorMessage = string.Empty;
       

            bool hasCoupon = cart.HasCoupon(ref couponCode);
            if (hasCoupon && cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
            {
                panelCoupon.Visible = true;
                litCouponEntered.Text = couponCode;
            }
            else
            {
                panelCoupon.Visible = false;
                if (!couponErrorMessage.IsNullOrEmptyTrimmed())
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                }
            }


            if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (cart.HasRegistryItemButParentRegistryIsRemoved() || cart.HasRegistryItemsRemovedFromRegistry())
            {
                cart.RemoveRegistryItemsHasDeletedRegistry();
                cart.RemoveRegistryItemsHasBeenDeletedInRegistry();
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("editgiftregistry.error.18", SkinID, ThisCustomer.LocaleSetting).ToUrlEncode());
            }

            if (cart.HasRegistryItemsAndOneOrMoreItemsHasZeroInNeed())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("editgiftregistry.error.15", SkinID, ThisCustomer.LocaleSetting).ToUrlEncode());
            }

            if (cart.HasRegistryItemsAndOneOrMoreItemsExceedsToTheInNeedQuantity())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("editgiftregistry.error.14", SkinID, ThisCustomer.LocaleSetting).ToUrlEncode());
            }

            if (!IsPostBack)
            {
                InitializePageContent();
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnContinueCheckout1.Click += btnContinueCheckout1_Click;
        }

        #endregion
        void btnContinueCheckout1_Click(object sender, EventArgs e)
        {
            ProcessCheckout();
        }

        private void InitializePageContent()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_5.gif");
            for (int i = 0; i < checkoutheadergraphic.HotSpots.Count; i++)
            {
                RectangleHotSpot rhs = (RectangleHotSpot)checkoutheadergraphic.HotSpots[i];
                if (rhs.NavigateUrl.IndexOf("shoppingcart") != -1) rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                if (rhs.NavigateUrl.IndexOf("account") != -1) rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.3", SkinID, ThisCustomer.LocaleSetting);
                if (rhs.NavigateUrl.IndexOf("checkoutshipping") != -1) rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.4", SkinID, ThisCustomer.LocaleSetting);
                if (rhs.NavigateUrl.IndexOf("checkoutpayment") != -1) rhs.AlternateText = AppLogic.GetString("checkoutreview.aspx.5", SkinID, ThisCustomer.LocaleSetting);
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
                    checkoutheadergraphic.HotSpots[2].NavigateUrl = CommonLogic.IIF(cart.HasMultipleShippingAddresses(), "checkoutshippingmult.aspx", "checkoutshipping.aspx");
                }
            }
            if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
            {
                checkoutheadergraphic.HotSpots[3].NavigateUrl = "checkout1.aspx";
            }

			if(IsPayPalCheckout)
			{
				checkoutheadergraphic.HotSpots[1].HotSpotMode = HotSpotMode.Inactive;
				checkoutheadergraphic.HotSpots[2].NavigateUrl += string.Format("?PayPal={0}&token={1}", bool.TrueString, Request.QueryString["token"]);
				checkoutheadergraphic.HotSpots[3].HotSpotMode = HotSpotMode.Inactive;
			}

            string XmlPackageName = AppLogic.AppConfig("XmlPackage.CheckoutReviewPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_CheckoutReviewPageHeader.Text = "<br/>" + AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
            }

            if (cart.HasMultipleShippingAddresses() || cart.HasRegistryItems())
            {
                var splittedCarts = cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();
                foreach (var splitCart in splittedCarts)
                {
                    splitCart.BuildSalesOrderDetails(litCouponEntered.Text);
                    OrderSummary.Text += splitCart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Review, litCouponEntered.Text));
                    //CartSummary.Text += splitCart.RenderHTMLLiteral(new CheckOutPaymentPageLiteralRenderer());
                }
            }
            else
            {
                //If the shopping cart contains only Electronic Downloads or Services then pass a "false" parameter for computeFreight.
                if (cart.IsNoShippingRequired())
                {
                    cart.BuildSalesOrderDetails(false, true, litCouponEntered.Text);
                }
                else
                {
                    cart.BuildSalesOrderDetails(litCouponEntered.Text);
                }

                string couponCode = string.Empty;
                if (!ThisCustomer.CouponCode.IsNullOrEmptyTrimmed()) couponCode = ThisCustomer.CouponCode;

                OrderSummary.Text = cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Review, "page.checkoutshippingordersummary.xml.config", couponCode));

            }

            if (AppLogic.AppConfigBool("ShowEditAddressLinkOnCheckOutReview"))
            {
                pnlEditBillingAddress.Visible = true;
                pnlEditShippingAddress.Visible = true;

                imgBillingRedArrow.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/redarrow.gif");
                imgShippingRedArrow.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/redarrow.gif");
            }

            litBillingAddress.Text = ThisCustomer.PrimaryBillingAddress.DisplayString(true, true, true, "<br/>");

			if(IsPayPalCheckout)
				litPaymentMethod.Text = "PayPal Express Checkout";
			else
            	litPaymentMethod.Text = GetPaymentMethod(ThisCustomer.PrimaryBillingAddress);

            if (cart.HasMultipleShippingAddresses() || (cart.HasRegistryItems() && cart.CartItems.Count > 1))
            {
                litShippingAddress.Text = "<br/>Multiple Ship Addresses";
            }
            else
            {
                Address shippingAddress = null;

                //added for PayPal ADDRESSOVERRIDE  
                if (IsPayPalCheckout && !AppLogic.AppConfigBool("PayPalCheckout.OverrideAddress"))
                {
                    if (!cart.HasShippableComponents())
                    {
                        shippingAddress = ThisCustomer.PrimaryShippingAddress;
                    }
                    else
                    {
                        pp = new PayPalExpress();
                        var GetPayPalDetails = pp.GetExpressCheckoutDetails(Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
                        shippingAddress = new Address()
                        {
                            Name = GetPayPalDetails.PayerInfo.Address.Name,
                            Address1 = GetPayPalDetails.PayerInfo.Address.Street1 + (GetPayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty) + GetPayPalDetails.PayerInfo.Address.Street2,
                            City = GetPayPalDetails.PayerInfo.Address.CityName,
                            State = GetPayPalDetails.PayerInfo.Address.StateOrProvince,
                            PostalCode = GetPayPalDetails.PayerInfo.Address.PostalCode,
                            Country = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.CountryName.ToString()),
                            CountryISOCode = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.Country.ToString()),
                            Phone = GetPayPalDetails.PayerInfo.ContactPhone
                        };
                    }
                }
                else
                {
                    if (cart.OnlyShippingAddressIsNotCustomerDefault())
                    {
                        var item =  cart.FirstItem();
                        shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, item.m_ShippingAddressID, item.GiftRegistryID);
                    }
                    else
                    {
                        shippingAddress = ThisCustomer.PrimaryShippingAddress;
                    }
                }

                litShippingAddress.Text = shippingAddress.DisplayString(true, true, true, "<br/>");
            }

            string XmlPackageName2 = AppLogic.AppConfig("XmlPackage.CheckoutReviewPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_CheckoutReviewPageFooter.Text = "<br/>" + AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
            }

            AppLogic.GetButtonDisable(btnContinueCheckout1);
            CheckoutReviewPageHeader.SetContext = this;
            CheckoutReviewPageFooter.SetContext = this;
        }

        private string GetPaymentMethod(Address BillingAddress)
        {
            var sPmtMethod = new StringBuilder();
            var paymentInfo = PaymentTermDTO.Find(ThisCustomer.PaymentTermCode);

            //  We should have a default payment method
            //  For debugging purposes, have this check here
            if (string.IsNullOrEmpty(paymentInfo.PaymentMethod))
            {
                throw new InvalidOperationException("No payment method defined!");
            }

            if (!cart.IsSalesOrderDetailBuilt)
            {
                if (cart.IsNoShippingRequired())
                {
                    cart.BuildSalesOrderDetails(false, true, litCouponEntered.Text);
                }
                else
                {
                    cart.BuildSalesOrderDetails(litCouponEntered.Text);
                }
            }

            if ((ThisCustomer.PaymentTermCode.Trim().Equals("PURCHASE ORDER", StringComparison.InvariantCultureIgnoreCase)) || 
                (ThisCustomer.PaymentTermCode.Trim().Equals("REQUEST QUOTE", StringComparison.InvariantCultureIgnoreCase)))
            {
                sPmtMethod.Append(ThisCustomer.PaymentTermCode.ToUpperInvariant());
            }
            else
            {
                switch (paymentInfo.PaymentMethod)
                {
                    case DomainConstants.PAYMENT_METHOD_CREDITCARD:

                        if ((cart.GetOrderTotal() == decimal.Zero) && (AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout")))
                        {
                            sPmtMethod.Append(AppLogic.GetString("checkoutpayment.aspx.8", SkinID, ThisCustomer.LocaleSetting));
                        }
                        else
                        {
                            sPmtMethod.AppendFormat("{0} ({1})", Security.HtmlEncode(paymentInfo.PaymentMethod), HttpUtility.HtmlEncode(ThisCustomer.PaymentTermCode));
                            sPmtMethod.Append("<br/>");
                            sPmtMethod.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                            sPmtMethod.Append("<tr><td>");
                            sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.10", SkinID, ThisCustomer.LocaleSetting));
                            sPmtMethod.Append("</td><td>");
                            sPmtMethod.Append(BillingAddress.CardName);
                            sPmtMethod.Append("</td></tr>");
                            sPmtMethod.Append("<tr><td>");
                            sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.11", SkinID, ThisCustomer.LocaleSetting));
                            sPmtMethod.Append("</td><td>");
                            DataView dt = AppLogic.GetCustomerCreditCardType("");
                            string cardTypeDescription = string.Empty;
                            try
                            {
                                cardTypeDescription = dt.Table.Select(string.Format("CreditCardType = '{0}'", BillingAddress.CardType))[0]["CreditCardTypeDescription"].ToString();
                            }
                            catch
                            {
                                cardTypeDescription = BillingAddress.CardType;
                            }
                            sPmtMethod.Append(cardTypeDescription);
                            sPmtMethod.Append("</td></tr>");
                            sPmtMethod.Append("<tr><td>");
                            sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.12", SkinID, ThisCustomer.LocaleSetting));
                            sPmtMethod.Append("</td><td>");
                            sPmtMethod.Append(BillingAddress.CardNumberMaskSafeDisplayFormat);
                            sPmtMethod.Append("</td></tr>");
                            sPmtMethod.Append("<tr><td>");
                            sPmtMethod.Append(AppLogic.GetString("checkoutreview.aspx.13", SkinID, ThisCustomer.LocaleSetting));
                            sPmtMethod.Append("</td><td>");
                            sPmtMethod.Append(BillingAddress.CardExpirationMonth.PadLeft(2, '0') + "/" + BillingAddress.CardExpirationYear);
                            sPmtMethod.Append("</td></tr>");
                            sPmtMethod.Append("</table>");
                        }
                        break;
                    case DomainConstants.PAYMENT_METHOD_CASH:
                    case DomainConstants.PAYMENT_METHOD_CHECK:
                    case DomainConstants.PAYMENT_METHOD_WEBCHECKOUT:

                        if ((cart.GetOrderTotal() == decimal.Zero) && (AppLogic.AppConfigBool("SkipPaymentEntryOnZeroDollarCheckout")))
                        {
                            sPmtMethod.Append(AppLogic.GetString("checkoutpayment.aspx.8", SkinID, ThisCustomer.LocaleSetting));
                        }
                        else
                        {
                            sPmtMethod.AppendFormat("{0} ({1})", Security.HtmlEncode(paymentInfo.PaymentMethod), HttpUtility.HtmlEncode(ThisCustomer.PaymentTermCode));
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Payment method!");
                }
            }

            return sPmtMethod.ToString();
        }

        private void ProcessCheckout()
        {
            if (!cart.IsEmpty())
            {
                var isOutOfStockAndPhaseOut = cart.CartItems.Any(item => item.Status == "P" && item.IsOutOfStock);
                if (isOutOfStockAndPhaseOut) Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            string OrderNumber = string.Empty;
            
            // ----------------------------------------------------------------
            // Process The Order:
            // ----------------------------------------------------------------
            if (ThisCustomer.PaymentTermCode.IsNullOrEmptyTrimmed())
            {
                Response.Redirect("checkoutpayment.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }
            else
            {
                string receiptCode = string.Empty;
                string status = string.Empty, multiorder = string.Empty;
                if (cart.HasMultipleShippingAddresses() || cart.HasRegistryItems())	// Paypal will never hit this
                {
                    var splittedCarts = cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();
                    bool gatewayAuthFailed = false;

                    for (int ctr = 0; ctr < splittedCarts.Count; ctr++)
                    {
                        var splitCart = splittedCarts[ctr];
                        try
                        {
                            splitCart.BuildSalesOrderDetails(litCouponEntered.Text);
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

                        var currentItem = splitCart.FirstItem();
                        var shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, currentItem.m_ShippingAddressID, currentItem.GiftRegistryID);

                        string processedSalesOrderCode = string.Empty;
                        string processedReceiptCode = string.Empty;
                        // NOTE:
                        //  3DSecure using Sagepay Gateway is not supported on multiple shipping orders
                        //  We will revert to the regular IS gateway defined on the WebStore
                        status = splitCart.PlaceOrder(null,
                                    ThisCustomer.PrimaryBillingAddress,
                                    shippingAddress,
                                    ref processedSalesOrderCode,
                                    ref processedReceiptCode,
                                    false,
                                    true,
                                    false);

                        OrderNumber = processedSalesOrderCode;
                        receiptCode = processedReceiptCode;

                        if (status == AppLogic.ro_INTERPRISE_GATEWAY_AUTHORIZATION_FAILED)
                        {
                            gatewayAuthFailed = true;

                            if (ctr == 0)
                            {
                                ThisCustomer.IncrementFailedTransactionCount();
                                if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                                {
                                    cart.ClearTransaction();
                                    ThisCustomer.ResetFailedTransactionCount();
                                    Response.Redirect("orderfailed.aspx");
                                }

                                ThisCustomer.ClearTransactions(false);

                                if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                                {
                                    Response.Redirect("checkout1.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                                }
                                else
                                {
                                    Response.Redirect("checkoutpayment.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                                }
                            }
                        }

                        // NOTE :
                        //  Should handle cases when 1 or more orders failed the payment processor 
                        //  if using a payment gateway on credit card
                        multiorder = multiorder + "," + OrderNumber;

                        if (!gatewayAuthFailed)
                        {
                            if (splitCart.HasRegistryItems())
                            {
                                DoRegistryQuantityDeduction(splitCart.CartItems);
                            }
                        }

                    }

                    if (multiorder != string.Empty) OrderNumber = multiorder.Remove(0, 1);

                    if (!gatewayAuthFailed)
                    {
                        cart.ClearTransaction();
                    }
                }
                else
                {
                    var billingAddress = ThisCustomer.PrimaryBillingAddress;
                    Address shippingAddress = null;

                    //added for PayPal ADDRESSOVERRIDE  
                    if (IsPayPalCheckout && !AppLogic.AppConfigBool("PayPalCheckout.OverrideAddress"))
                    {
                        if (!cart.HasShippableComponents())
                        {
                            shippingAddress = ThisCustomer.PrimaryShippingAddress;
                        }
                        else
                        {
                            pp = new PayPalExpress();
                            var GetPayPalDetails = pp.GetExpressCheckoutDetails(Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
                            shippingAddress = new Address()
                            {
                                Name = GetPayPalDetails.PayerInfo.Address.Name,
                                Address1 = GetPayPalDetails.PayerInfo.Address.Street1 + (GetPayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty) + GetPayPalDetails.PayerInfo.Address.Street2,
                                City = GetPayPalDetails.PayerInfo.Address.CityName,
                                State = GetPayPalDetails.PayerInfo.Address.StateOrProvince,
                                PostalCode = GetPayPalDetails.PayerInfo.Address.PostalCode,
                                Country = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.CountryName.ToString()),
                                CountryISOCode = AppLogic.ResolvePayPalAddressCode(GetPayPalDetails.PayerInfo.Address.Country.ToString()),
                                Phone = GetPayPalDetails.PayerInfo.ContactPhone ?? String.Empty
                            };
                        }
                    }
                    else
                    {
                        // Handle the scenario wherein the items in the cart
                        // does not ship to the customer's primary shipping address
                        if (cart.OnlyShippingAddressIsNotCustomerDefault())
                        {
                            var item = cart.FirstItem();
                            shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, item.m_ShippingAddressID, item.GiftRegistryID);
                        }
                        else
                        {
                            shippingAddress = ThisCustomer.PrimaryShippingAddress;
                        }
                    }

                    if (!cart.IsSalesOrderDetailBuilt)
                    {
                        cart.BuildSalesOrderDetails(litCouponEntered.Text);
                    }

                    Gateway gatewayToUse = null;

                    try
                    {
                        if (IsPayPalCheckout)
                        {
                            //Insert PayPal call here for response - For authorize and capture of order from paypal inside IS
                            pp = new PayPalExpress();

                            var paypalDetails = pp.GetExpressCheckoutDetails(Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
                            var doExpressCheckoutResp = pp.DoExpressCheckoutPayment(paypalDetails.Token, paypalDetails.PayerInfo.PayerID, OrderNumber, cart);
                            string result = String.Empty;
                            if (doExpressCheckoutResp.Errors != null && !doExpressCheckoutResp.Errors[0].ErrorCode.IsNullOrEmptyTrimmed())
                            {
                                if (AppLogic.AppConfigBool("ShowGatewayError"))
                                {
                                    result = String.Format(AppLogic.GetString("shoppingcart.aspx.27", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), doExpressCheckoutResp.Errors[0].ErrorCode, doExpressCheckoutResp.Errors[0].LongMessage);
                                }
                                else
                                {
                                    result = AppLogic.GetString("shoppingcart.aspx.28", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                                }

                                Response.Redirect("shoppingcart.aspx?ErrorMsg=" + result.ToUrlEncode(), false);
                                return;
                            }
                            else
                            {
                                var payPalResp = new GatewayResponse(String.Empty)
                                {
                                    AuthorizationCode = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID,
                                    TransactionResponse = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                                    Details = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                                    AuthorizationTransID = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID
                                };

                                status = cart.PlaceOrder(gatewayToUse, billingAddress, shippingAddress, ref OrderNumber, ref receiptCode, true, true, payPalResp, IsPayPalCheckout, false);
                            }
                        }
                        else
                        {
                            status = cart.PlaceOrder(gatewayToUse, billingAddress, shippingAddress, ref OrderNumber, ref receiptCode, true, true, null, !IsPayPalCheckout, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Unable to instantiate Default Credit Card Gateway")
                        {
                            cart.ClearLineItems();
                            Response.Redirect("pageError.aspx?Parameter=" + "An Error Occured while Authorizing your Credit Card, However your order has been Placed.");
                        }

                        Response.Redirect("pageError.aspx?Parameter=" + Server.UrlEncode(ex.Message));
                    }

                    if (status == AppLogic.ro_3DSecure)
                    { // If credit card is enrolled in a 3D Secure service (Verified by Visa, etc.)
                        Response.Redirect("secureform.aspx");
                    }
                    if (status != AppLogic.ro_OK)
                    {
                        ThisCustomer.IncrementFailedTransactionCount();
                        if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                        {
                            cart.ClearTransaction();
                            ThisCustomer.ResetFailedTransactionCount();
                            Response.Redirect("orderfailed.aspx");
                        }

                        ThisCustomer.ClearTransactions(false);

                        if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                        {
                            Response.Redirect("checkout1.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                        }
                        else
                        {
                            Response.Redirect("checkoutpayment.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + Server.UrlEncode(status));
                        }
                    }

                }
            }

            AppLogic.ClearCardNumberInSession(ThisCustomer);
            ThisCustomer.ClearTransactions(true);

			if(!_PayPalFailed)
            Response.Redirect(string.Format("orderconfirmation.aspx?ordernumber={0}", Server.UrlEncode(OrderNumber)));
        }

        protected override void OnUnload(EventArgs e)
        {
            if (cart != null)
            {
                cart.Dispose();
            }
            base.OnUnload(e);
        }

        public void DoRegistryQuantityDeduction(CartItemCollection cartItems) 
        {
            foreach (var cartItem in cartItems)
            {
                if (!cartItem.GiftRegistryID.HasValue) continue;

                decimal quatityToremove = cartItem.m_Quantity;

                //to avoid negative value upon deduction
                decimal? trueQuantity = cartItem.RegistryItemQuantity;
                if (trueQuantity < cartItem.m_Quantity) { quatityToremove = trueQuantity.Value; }

                GiftRegistryDA.DeductGiftRegistryItemQuantity(cartItem.GiftRegistryID.Value, cartItem.RegistryItemCode.Value, quatityToremove, cartItem.m_Quantity);        
            }
            
        }

    }
}
