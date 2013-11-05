// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Linq;
using System.Collections;
using System.Globalization;
using System.Web.Security;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for checkoutanon.
    /// </summary>
    public partial class checkoutanon : SkinBase
    {

        String PaymentMethod = String.Empty;
        InterpriseShoppingCart cart;
        private string _checkoutType = string.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            _checkoutType = CommonLogic.QueryStringCanBeDangerousContent("checkoutType");

            RequireSecurePage();

            SectionTitle = AppLogic.GetString("checkoutanon.aspx.1", SkinID, ThisCustomer.LocaleSetting, true);

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

            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

            if (cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting, true)));
            }

            if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!cart.MeetsMinimumOrderWeight(AppLogic.AppConfigUSDecimal("MinOrderWeight")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            CheckoutMap.HotSpots[0].AlternateText = AppLogic.GetString("checkoutanon.aspx.2", SkinID, ThisCustomer.LocaleSetting, true);

            Teaser.SetContext = this;

            if (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                PasswordOptionalPanel.Visible = true;
            }

            ErrorMsgLabel.Text = "";
            if (!IsPostBack)
            {
                InitializePageContent();
            }

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
            {
                // Create a random code and store it in the Session object.
                SecurityImage.Visible = true;
                SecurityCode.Visible = true;

                trSecurityCodeText.Visible = true;
                trSecurityCodeImage.Visible = true;
                
                Label4.Visible = true;                      
                if (!IsPostBack)
                    SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                else
                    SecurityImage.ImageUrl = "Captcha.ashx?id=2";
            }
        }

        private void InitializePageContent()
        {
            CheckoutMap.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");
          
            btnSignInAndCheckout.Text = AppLogic.GetString("checkoutanon.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            RegisterAndCheckoutButton.Text =  AppLogic.GetString("checkoutanon.aspx.13", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
            Skipregistration.Text = AppLogic.GetString("checkoutanon.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnSignInAndCheckout, "checkoutanon.aspx.12");
                AppLogic.EnableButtonCaptionEditing(RegisterAndCheckoutButton, "checkoutanon.aspx.13");
                AppLogic.EnableButtonCaptionEditing(Skipregistration, "checkoutanon.aspx.14");
            }
        }

        protected void btnSignInAndCheckout_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {

                String EMailField = EMail.Text.ToLower();
                String PasswordField = Password.Text;

                if (AppLogic.AppConfigBool("SecurityCodeRequiredOnStoreLogin"))
                {
                    String sCode = Session["SecurityCode"].ToString();
                    String fCode = SecurityCode.Text;

                    Boolean codeMatch = false;

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
                        SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                        return;
                    }
                }         

                Customer customerWithValidLogin = Customer.FindByLogin(EMail.Text, PasswordField);
                if (null == customerWithValidLogin)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.20", SkinID, ThisCustomer.LocaleSetting, true);
                    ErrorPanel.Visible = true;
                    return;
                }

                ThisCustomer.ThisCustomerSession["CustomerID"] = customerWithValidLogin.ContactGUID.ToString();

                AppLogic.ExecuteSigninLogic(ThisCustomer.CustomerCode, ThisCustomer.ContactCode, customerWithValidLogin.CustomerCode, customerWithValidLogin.PrimaryShippingAddressID, customerWithValidLogin.ContactCode);

                string cookieUserName = customerWithValidLogin.ContactGUID.ToString();

                Security.SignOutCrossDomainCookie();
                Security.CreateLoginCookie(cookieUserName, true);

                if (_checkoutType == "pp")
                {
                    var customer = Customer.Find(customerWithValidLogin.ContactGUID);
                    Security.OverrideThisCustomer(customer);

                    if (!cart.IsSalesOrderDetailBuilt)
                    {
                        cart.BuildSalesOrderDetails();
                    }
                    Customer.Current.ThisCustomerSession["paypalFrom"] = "checkoutanon";
                    Response.Redirect(PayPalExpress.CheckoutURL(cart));
                }
                else
                {
                    string sReturnURL = "shoppingcart.aspx";

                    FormPanel.Visible = false;
                    HeaderPanel.Visible = false;
                    ExecutePanel.Visible = true;
                    SignInExecuteLabel.Text = AppLogic.GetString("signin.aspx.2", SkinID, ThisCustomer.LocaleSetting, true);

                    Response.AddHeader("REFRESH", "1; URL=" + Server.UrlDecode(sReturnURL));
                }
            }
        }
        protected void RegisterAndCheckoutButton_Click(object sender, EventArgs e)
        {
            if (_checkoutType == "pp")
            {
                Response.Redirect("createaccount.aspx?checkout=true&isAnonPayPal=true");
            }
            else
            {
                Response.Redirect("createaccount.aspx?checkout=true");
            }
        }
        protected void Skipregistration_Click(object sender, EventArgs e)
        {
		    if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout") && AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                Response.Redirect("checkout1.aspx");
            }
            else
            {
                if (_checkoutType == "pp")
                {
                    Response.Redirect("createaccount.aspx?checkout=true&skipreg=true&isAnonPayPal=true");
                }
                else if (_checkoutType == "gc")
                {
                    Response.Redirect("createaccount.aspx?checkout=true&skipreg=true&isAnonGoogleCheckout=true");
                }
                else
                {
                    Response.Redirect("createaccount.aspx?checkout=true&skipreg=true");
                }
            }
        }
    }
}
