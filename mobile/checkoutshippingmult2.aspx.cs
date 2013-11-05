// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for checkoutshippingmult2.
    /// </summary>
    public partial class checkoutshippingmult2 : SkinBase
    {
        #region Variable Declaration

        private InterpriseShoppingCart _cart = null;
        private int _shippingMethodCount = 0;
        public InterpriseShoppingCart cart;

        #endregion

        #region Properties

        public int ShippingGroupCounter { get; set; }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetCacheability();
            RequireSecurePage();
            RequireCustomerRecord();
            InitializeShoppingCart();
            PerformPageAccessLogic();
            InitializeCartRepeaterControl();
            DisplayCheckOutStepsImage();

			//mobile new resources
            btnCompletePurchase.Text = AppLogic.GetString("checkoutshippingmult.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            litHeaderText.Text = AppLogic.GetString("checkoutshipping.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            btnCompletePurchase.Click +=btnCompletePurchase_Click;
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

            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkout.over13required", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)));
            }

            if (ThisCustomer.IsNotRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                Response.Redirect("createaccount.aspx?checkout=true");
            }
            if (ThisCustomer.PrimaryBillingAddressID == String.Empty || ThisCustomer.PrimaryShippingAddressID == String.Empty)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutpayment.aspx.1", SkinID, ThisCustomer.LocaleSetting)));
            }

            SectionTitle = AppLogic.GetString("checkoutshippingmult.aspx.1", SkinID, ThisCustomer.LocaleSetting);

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

            if (_cart.IsNoShippingRequired() || !Shipping.MultiShipEnabled() || (_cart.NumItems() > AppLogic.MultiShipMaxNumItemsAllowed()) || _cart.NumItems() == 1)
            {
                // not allowed then:
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult.aspx.3", SkinID, ThisCustomer.LocaleSetting)));
            }

            if (ThisCustomer.PrimaryShippingAddress == null ||
                CommonLogic.IsStringNullOrEmpty(ThisCustomer.PrimaryShippingAddress.AddressID))
            {
                // not allowed here anymore!
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult.aspx.2", SkinID, ThisCustomer.LocaleSetting)));
            }
            
        }

        private void InitializeShoppingCart()
        {
            _cart = new InterpriseShoppingCart(base.EntityHelpers, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
            _cart.BuildSalesOrderDetails();
        }

        private void DisplayCheckOutStepsImage()
        {
            checkoutheadergraphic.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_3.gif");
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.3", SkinID, ThisCustomer.LocaleSetting);
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.4", SkinID, ThisCustomer.LocaleSetting);
        }

        private void InitializeCartRepeaterControl()
        {
            rptCartItems.ItemDataBound += rptCartItems_ItemDataBound;
            InitializeDataSource();
        }

        private void InitializeDataSource()
        {
            rptCartItems.DataSource = GetDataSource();
            rptCartItems.DataBind();
        }

        private List<InterpriseShoppingCart> GetDataSource()
        {
            return _cart.SplitIntoMultipleOrdersByDifferentShipToAddresses();
        }

		//modified for mobile layout (div)
        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem is InterpriseShoppingCart)
                {
                    var cart = e.Item.DataItem as InterpriseShoppingCart;
                    cart.BuildSalesOrderDetails();
                    foreach (CartItem item in cart.CartItems)
                    {
                        var itemContainer = e.Item.FindByParse<Panel>("pnlItemContainer");
                        itemContainer.Controls.Add(new Label() { Text = item.DisplayName, CssClass = "product_description_min" });
                        itemContainer.Controls.Add(new Literal() { Text = "<br />" });
                        itemContainer.Controls.Add(new Label(){ Text = AppLogic.GetString("shoppingcart.cs.25", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) });
                        itemContainer.Controls.Add(new Literal() { Text = " : " });
                        itemContainer.Controls.Add(new Label() { Text = Localization.ParseLocaleDecimal(item.m_Quantity, ThisCustomer.LocaleSetting) });
                        itemContainer.Controls.Add(new Literal() { Text = "<br />" });
                    }

                    var mainShipMethodContainer = e.Item.FindByParse<Panel>("divShippingInfo");
                    var lblShipmethodHeader = e.Item.FindByParse<Label>("lblShipmethodHeader");

                    if (cart.HasShippableComponents())
                    {
                        var shippingAddress = Address.Get(ThisCustomer, AddressTypes.Shipping, cart.FirstItem().m_ShippingAddressID);
                        var lblShippingAddressString = e.Item.FindByParse<Label>("lblShippingAddressString");
                        lblShippingAddressString.Text = shippingAddress.DisplayString(true, true, true, "<br/>");

                        var ctrlShippingMethod = e.Item.FindByParse<UserControls_MobileShippingMethodControl>("ctrlShippingMethod");
                        ctrlShippingMethod.ShippingAddressID = shippingAddress.AddressID;
                        ctrlShippingMethod.ErrorSummaryControl = this.errorSummary;
                        ctrlShippingMethod.ShippingMethodRequiredErrorMessage = AppLogic.GetString("checkout1.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        mainShipMethodContainer.Visible = true;
                        lblShipmethodHeader.Text = AppLogic.GetString("shoppingcart.cs.30", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        
                        var script = new StringBuilder();
                        script.Append("<script type='text/javascript'>\n");
                        script.Append("$(document).ready( function() { \n");
                        script.AppendFormat("ise.Pages.CheckOutShippingMulti2.registerShippingMethodControlId('{0}');\n", ctrlShippingMethod.ClientID);
                        script.Append("});\n");
                        script.Append("</script>\n");

                        Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
                        _shippingMethodCount++;
                    }
                    else
                    {
                        lblShipmethodHeader.Text = AppLogic.GetString("checkoutshippingmult.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        lblShipmethodHeader.CssClass = "notificationtext";
                        mainShipMethodContainer.Visible = false;
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var script = new StringBuilder();

            script.Append("<script type=\"text/javascript\">\n");
            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.9", AppLogic.GetString("checkoutshipping.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.10", AppLogic.GetString("checkoutshipping.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.11", AppLogic.GetString("checkoutshipping.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            script.AppendFormat("   ise.StringResource.registerString('{0}', '{1}');\n", "checkoutshipping.aspx.12", AppLogic.GetString("checkoutshipping.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

            script.AppendFormat("   ise.Pages.CheckOutShippingMulti2.setForm('{0}');\n", this.frmCheckOutMultiShipping2.ClientID);
            script.AppendFormat("   ise.Pages.CheckOutShippingMulti2.setExpectedCount({0});\n", _shippingMethodCount);

            script.Append(" }\n");
            script.Append(");\n");
            script.Append("</script>\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
        }

        protected override void OnRenderHeader(object sender, System.IO.TextWriter writer)
        {
			//required for mobile design
            writer.WriteLine("<script type=\"text/javascript\" src=\"js/jquery/jquery-ui.min.js\" ></script>");
            writer.WriteLine("<link  type=\"text/css\" rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/jquery-ui.css\" />");
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/jquery-template/shipping-method-template.js"));
            manager.Scripts.Add(new ScriptReference("js/jquery-template/shipping-method-oversized-template.js"));
            manager.Scripts.Add(new ScriptReference("js/shippingmethod_ajax.js"));
            manager.Scripts.Add(new ScriptReference("js/checkoutshippingmulti2_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        protected void btnCompletePurchase_Click(object sender, EventArgs e)
        {
            if (!this.IsValid) return;

            //Clear EcommerceRealTimeRate record first
            DB.ExecuteSQL(String.Format("DELETE FROM EcommerceRealTimeRate WHERE ContactCode = {0}", ThisCustomer.ContactCode.ToDbQuote()));

            var carts = rptCartItems.DataSource as List<InterpriseShoppingCart>;
            for (int ctr = 0; ctr < carts.Count; ctr++)
            {
                // the items should be at sync with the cart datasource..
                var cart = carts[ctr];
                if (cart.HasShippableComponents())
                {
                    var ctrlShippingMethod = rptCartItems.Items[ctr].FindControl("ctrlShippingMethod") as UserControls_MobileShippingMethodControl;

                    if (ctrlShippingMethod.FreightCalculation == "1" || ctrlShippingMethod.FreightCalculation == "2")
                    {
                        cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod, ctrlShippingMethod.ShippingAddressID, ctrlShippingMethod.RealTimeRateGUID);

                        string freight = ctrlShippingMethod.Freight.Trim(new char[] { ' ', '$' });
                        _cart.SetRealTimeRateRecord(ctrlShippingMethod.ShippingMethod, freight, ctrlShippingMethod.RealTimeRateGUID.ToString(), true);                        
                    }
                    else
                    {
                        cart.SetCartShippingMethod(ctrlShippingMethod.ShippingMethod, ctrlShippingMethod.ShippingAddressID);
                    }
                }
                else
                {
                    cart.MakeShippingNotRequired(false);
                }
            }

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
    }
}
