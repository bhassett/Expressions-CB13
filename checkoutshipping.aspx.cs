// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using System.Linq;

namespace InterpriseSuiteEcommerce
{
    public partial class checkoutshipping : SkinBase
    {
        #region Variables

        private InterpriseShoppingCart _cart = null;
        private bool _cartHasCouponAndIncludesFreeShipping = false;
        public const string PAYMENT_METHOD_CREDITCARD = "Credit Card";

        bool _isFromPayPal = false;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Check for paypal
            _isFromPayPal = (CommonLogic.QueryStringCanBeDangerousContent("PayPal") == bool.TrueString && CommonLogic.QueryStringCanBeDangerousContent("token") != null);
            ctrlShippingMethod.ThisCustomer = ThisCustomer;

            if (ThisCustomer.IsInEditingMode())
            {
                PageNoCache();
            }
            else
            {
                SetCacheability();
            }

            RequireSecurePage();
            RequireCustomerRecord();

            InitializeShoppingCart();
            PerformPageAccessLogic();
            CheckWhetherToRequireShipping();
            DisplayCheckOutStepsImage(); 
            InitializeShippingMethodCaptions();
            InitializeShippingMethodControlValues();
            DisplayOrderSummary();
            ShowFreeshippingInfo();
            AssignCheckOutButtonCaption();

            RegisterPageScript();

        }

        #endregion

        #region Methods

        private void RegisterPageScript()
        {
            if (_cartHasCouponAndIncludesFreeShipping) return;

            var script = new StringBuilder();
            script.Append("<script type=\"text/javascript\" >\n");
            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.9", AppLogic.GetString("checkoutshipping.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.10", AppLogic.GetString("checkoutshipping.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.11", AppLogic.GetString("checkoutshipping.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.12", AppLogic.GetString("checkoutshipping.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));

            script.AppendFormat("   ise.Pages.CheckOutShipping.setShippingMethodControlId('{0}');\n", this.ctrlShippingMethod.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutShipping.setForm('{0}');\n", this.frmCheckOutShipping.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-template.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/jquery-template/shipping-method-oversized-template.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/shippingmethod_ajax.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/checkoutshipping_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);

            string couponCode = string.Empty;
            bool hasCoupon = _cart.HasCoupon(ref couponCode);

            if (hasCoupon)
            {
                panelCoupon.Visible = true;
                litCouponEntered.Text = couponCode;
            }
            else
            {
                panelCoupon.Visible = false;
            }

            try
            {
                // Always compute the vat since we need to display the vat even if the the vat enabled = true
                _cart.BuildSalesOrderDetails(false, true, couponCode);
                _cartHasCouponAndIncludesFreeShipping = _cart.CouponIncludesFreeShipping(couponCode);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == AppLogic.GetString("shoppingcart.cs.35", 1, ThisCustomer.LocaleSetting, true))
                {
                    Response.Redirect("shoppingcart.aspx?resetlinkback=1&discountvalid=false");
                }
                else { throw ex; }
            }
            catch (Exception ex) { throw ex; }
        }

        private void SetCacheability()
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
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

        private void CheckWhetherToRequireShipping()
        {
            if (AppLogic.AppConfigBool("SkipShippingOnCheckout") ||
                !_cart.HasShippableComponents() ||
                _cart.CouponIncludesFreeShipping(litCouponEntered.Text))
            {
                _cart.MakeShippingNotRequired();

                if (!_isFromPayPal)
                {
                    Response.Redirect("checkoutpayment.aspx");
                }
                else
                {
                    InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, PAYMENT_METHOD_CREDITCARD);
                    Response.Redirect("checkoutreview.aspx?PayPal=True&token=" + Request.QueryString["token"]);
                }
            }
        }

        private void DisplayCheckOutStepsImage()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.3", SkinID, ThisCustomer.LocaleSetting, true);
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.4", SkinID, ThisCustomer.LocaleSetting, true);

            if (_isFromPayPal)
                checkoutheadergraphic.HotSpots[1].HotSpotMode = HotSpotMode.Inactive;
        }

        private void InitializeShippingMethodControl()
        {
            InitializeShippingMethodControlValues();
            AssignShippingMethodErrorSummary();
            AssignShippingMethodValidationPrerequisites();
            InitializeShippingMethodCaptions();
        }

        private void InitializeShippingMethodControlValues()
        {
            string shippingAddressID = String.Empty;

            if (_cart.OnlyShippingAddressIsNotCustomerDefault())
            {
                var shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, _cart.FirstItem().m_ShippingAddressID);
                shippingAddressID = shippingAddress.AddressID;
            }
            else
            {
                shippingAddressID = ThisCustomer.PrimaryShippingAddress.AddressID;
            }

            ctrlShippingMethod.ShippingAddressID = shippingAddressID;

            ctrlShippingMethod.ShippingMethodRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
        }

        private void AssignShippingMethodErrorSummary()
        {
            ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
        }

        private void AssignShippingMethodValidationPrerequisites()
        {
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
                if (ThisCustomer.IsRegistered && Shipping.MultiShipEnabled() && _cart.TotalQuantity() > 1 && !_isFromPayPal)
                {
                    lblSelectShippingMethod.Text = String.Format(AppLogic.GetString("checkoutshipping.aspx.7", SkinID, ThisCustomer.LocaleSetting, true), "checkoutshippingmult.aspx");
                    if (ThisCustomer.IsInEditingMode())
                    {
                        lblSelectShippingMethod.Attributes.Add("data-contentKey", "checkoutshipping.aspx.7");
                        lblSelectShippingMethod.Attributes.Add("data-contentValue", AppLogic.GetString("checkoutshipping.aspx.7", SkinID, ThisCustomer.LocaleSetting, true));
                        lblSelectShippingMethod.Attributes.Add("data-contentType", "string resource");
                    }
                }
                else
                {
                    lblSelectShippingMethod.Text = AppLogic.GetString("checkout1.aspx.4", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                }
            }
        }

        private void DisplayOrderSummary()
        {
            // OrderSummary.Text = _cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Shipping, litCouponEntered.Text));
            //OrderSummary.Text = _cart.RenderHTMLLiteral(new CheckOutShippingPageLiteralRenderer());

            OrderSummary.Text = _cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.Shipping, "page.checkoutshippingordersummary.xml.config", litCouponEntered.Text));
        }

        private void AssignCheckOutButtonCaption()
        {
            string contentKey = String.Empty;

            if (_cartHasCouponAndIncludesFreeShipping)
            {
                btnCompletePurchase.Text = AppLogic.GetString("checkoutshipping.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
                contentKey = "checkoutshipping.aspx.8";
            }
            else
            {
                btnCompletePurchase.Text = AppLogic.GetString("checkoutshipping.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
                contentKey = "checkoutshipping.aspx.6";
            }

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnCompletePurchase, contentKey);
            }
        }

        /// <summary>
        /// Compute Sub total needed to avail free shipping. FreeShippingThreshold and ShippingMethodCodeIfFreeShippingIsOn appconfig MUST be setup
        /// properly for this feature to work.
        /// </summary>
        private void ShowFreeshippingInfo()
        {
            decimal threshHold = AppLogic.AppConfigUSDecimal("FreeShippingThreshold");
            string currencyCode = _cart.ThisCustomer.CurrencyCode;
            decimal subTotal = _cart.GetCartSubTotalExcludeOversized();
            string shippingMethods = AppLogic.AppConfig("ShippingMethodCodeIfFreeShippingIsOn");
            string total;

            if (threshHold > decimal.Zero && threshHold > subTotal)
            {
                pnlGetFreeShippingMsg.Visible = true;
                total = InterpriseHelper.FormatCurrencyForCustomer(threshHold, currencyCode);
                GetFreeShippingMsg.Text = string.Format(AppLogic.GetString("checkoutshipping.aspx.2", SkinID, ThisCustomer.LocaleSetting), total, shippingMethods);
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_cartHasCouponAndIncludesFreeShipping) return;

            var script = new StringBuilder();
            script.Append("<script type=\"text/javascript\" >\n");
            script.Append("$(document).ready(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.Pages.CheckOutShipping.setShippingMethodControlId('{0}');\n", this.ctrlShippingMethod.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutShipping.setForm('{0}');\n", this.frmCheckOutShipping.ClientID);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected void btnCompletePurchase_Click(object sender, EventArgs e)
        {
            if (!_cart.IsEmpty())
            {
                var isOutOfStockAndPhaseOut = _cart.CartItems.Any(item => item.Status == "P" && item.IsOutOfStock);
                if (isOutOfStockAndPhaseOut) Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!_cartHasCouponAndIncludesFreeShipping)
            {
                if (ctrlShippingMethod.FreightCalculation == "1" || ctrlShippingMethod.FreightCalculation == "2")
                {
                    _cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod, String.Empty, ctrlShippingMethod.RealTimeRateGUID);

                    string freight = ctrlShippingMethod.Freight.Trim(new char[] { ' ', '$' });
                    _cart.SetRealTimeRateRecord(ctrlShippingMethod.ShippingMethod, freight, ctrlShippingMethod.RealTimeRateGUID.ToString(), false);
                }
                else
                {
                    _cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod);
                }

            }

            if (Request.QueryString["PayPal"] == bool.TrueString && Request.QueryString["token"] != null)
            {
                InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, PAYMENT_METHOD_CREDITCARD);
                Response.Redirect("checkoutreview.aspx?PayPal=True&token=" + Request.QueryString["token"]);
            }
            else
                Response.Redirect("checkoutpayment.aspx");

        }

        protected override void OnUnload(EventArgs e)
        {
            if (_cart != null)
            {
                _cart.Dispose();
            }
            base.OnUnload(e);
        }

        #endregion

    }

}



