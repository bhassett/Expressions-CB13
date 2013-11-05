// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;

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
		
		//for mobile user control setup
        protected override void OnInit(EventArgs e)
        {
            btnSignInAndCheckout.Click += btnSignInAndCheckout_Click;
            btnSignInAndCheckout.Text = AppLogic.GetString("checkoutanon.aspx.12", SkinID, ThisCustomer.LocaleSetting);

            RegisterAndCheckoutButton.Click += RegisterAndCheckoutButton_Click;
            RegisterAndCheckoutButton.Text = AppLogic.GetString("checkoutanon.aspx.13", SkinID, ThisCustomer.LocaleSetting);

            Skipregistration.Click += Skipregistration_Click;
            Skipregistration.Text = AppLogic.GetString("checkoutanon.aspx.14", SkinID, ThisCustomer.LocaleSetting);
            
            Password.Attributes.Add("placeholder", AppLogic.GetString("checkoutanon.aspx.5", SkinID, ThisCustomer.LocaleSetting));
            EMail.Attributes.Add("placeholder", AppLogic.GetString("checkoutanon.aspx.4", SkinID, ThisCustomer.LocaleSetting));
            SecurityCode.Attributes.Add("placeholder", AppLogic.GetString("signin.aspx.18", SkinID, ThisCustomer.LocaleSetting));

            base.OnInit(e);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            _checkoutType = CommonLogic.QueryStringCanBeDangerousContent("checkoutType");

            RequireSecurePage();

            SectionTitle = AppLogic.GetString("checkoutanon.aspx.1", SkinID, ThisCustomer.LocaleSetting);

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

            if (cart.HasRegistryItems())
            {
                Response.Redirect("shoppingcart.aspx");
            }

            if (cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting)));
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

            CheckoutMap.HotSpots[0].AlternateText = AppLogic.GetString("checkoutanon.aspx.2", SkinID, ThisCustomer.LocaleSetting);

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
                
                if (!IsPostBack)
                    SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                else
                    SecurityImage.ImageUrl = "Captcha.ashx?id=2";
            }
        }

        private void InitializePageContent()
        {
            CheckoutMap.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");
        }

        protected void btnSignInAndCheckout_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {

                string EMailField = EMail.Text.ToLower();
                string PasswordField = Password.Text;

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
                        ErrorMsgLabel.Text = string.Format(AppLogic.GetString("signin.aspx.22", SkinID, ThisCustomer.LocaleSetting), string.Empty, string.Empty);
                        ErrorPanel.Visible = true;                        
                        SecurityImage.ImageUrl = "Captcha.ashx?id=1";
                        return;
                    }
                }         

                Customer customerWithValidLogin = Customer.FindByLogin(EMail.Text, PasswordField);
                if (null == customerWithValidLogin)
                {
                    ErrorMsgLabel.Text = AppLogic.GetString("signin.aspx.20", SkinID, ThisCustomer.LocaleSetting);
                    ErrorPanel.Visible = true;
                    return;
                }

                ThisCustomer.ThisCustomerSession["CustomerID"] = customerWithValidLogin.ContactGUID.ToString();

                //save the last record of fullmode
                bool value = ThisCustomer.FullModeInMobile;

                AppLogic.ExecuteSigninLogic(ThisCustomer.CustomerCode, ThisCustomer.ContactCode, customerWithValidLogin.CustomerCode, customerWithValidLogin.PrimaryShippingAddressID, customerWithValidLogin.ContactCode);

                //set again the fullmode value to the loggedin customer
                ThisCustomer.FullModeInMobile = value;

                string cookieUserName = customerWithValidLogin.ContactGUID.ToString();
                
                FormsAuthentication.SetAuthCookie(cookieUserName, true);
                HttpCookie authCookie = Response.Cookies[FormsAuthentication.FormsCookieName];
                string sReturnURL = "shoppingcart.aspx";

                FormPanel.Visible = false;
                HeaderPanel.Visible = false;
                ExecutePanel.Visible = true;
                SignInExecuteLabel.Text = AppLogic.GetString("signin.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                
                Response.AddHeader("REFRESH", "1; URL=" + Server.UrlDecode(sReturnURL));
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
            Response.Redirect("createaccount.aspx?checkout=true&skipreg=true");
        }

    }
}
