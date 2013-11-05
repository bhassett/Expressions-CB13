// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Linq;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceControls;
using InterpriseSuiteEcommerceGateways;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for ShoppingCartPage.
    /// </summary>
    public partial class ShoppingCartPage : SkinBase
    {
        #region Private Members

        string _skinImagePath = string.Empty;
        InterpriseShoppingCart _cart = null;

        #endregion

        #region Events

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.RequireCustomerRecord();

            RequireSecurePage();

            ClearErrors();

            SectionTitle = AppLogic.GetString("AppConfig.CartPrompt", SkinID, ThisCustomer.LocaleSetting, true);

            if (!this.IsPostBack)
            {
                string returnurl = CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");
                if (returnurl.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    throw new ArgumentException("SECURITY EXCEPTION");
                }
                ViewState["returnurl"] = returnurl;
            }

            //for optimization
            string[] formkeys = Request.Form.AllKeys;
            if (formkeys.Any(k => k.Contains("bt_Delete")))
            {
                ProcessCart(false);

                //refresh the option items
                RenderOrderOptions();
            }

            InitializePageContent();

            HeaderMsg.SetContext = this;
            CartPageFooterTopic.SetContext = this;
        }

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            RenderOrderOptions();
            base.OnInit(e);
        }

        void OrderOptionsList_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            var orderOptionNode = e.Item.DataItem as XmlNode;
            int counter = 0;
            if (orderOptionNode != null &&
                int.TryParse(orderOptionNode["Counter"].InnerText, out counter))
            {
                string itemCode = orderOptionNode["ItemCode"].InnerText;
                string itemName = orderOptionNode["ItemName"].InnerText;
                string itemDescription = orderOptionNode["ItemDescription"].InnerText;
                string popupTitle = string.Empty;

                var lblDisplayName = e.Item.FindControl("OrderOptionName") as Label;
                if (!CommonLogic.IsStringNullOrEmpty(itemDescription))
                {
                    lblDisplayName.Text = Security.HtmlEncode(itemDescription);
                    popupTitle = CommonLogic.Left(Security.UrlEncode(SE.MungeName(itemDescription)), 90);
                }
                else
                {
                    lblDisplayName.Text = Security.HtmlEncode(itemName);
                    popupTitle = CommonLogic.Left(Security.UrlEncode(SE.MungeName(itemName)), 90);
                }

                if (AppLogic.AppConfigBool("ShowPicsInCart"))
                {
                    string ImgUrl = InterpriseHelper.LookUpImageByItemCode(itemCode, "icon", SkinID, ThisCustomer.LocaleSetting);
                    if (!string.IsNullOrEmpty(ImgUrl) && ImgUrl.IndexOf("nopicture") == -1)
                    {
                        Image imgControl = (Image)e.Item.FindControl("OptionImage");
                        imgControl.ImageUrl = ImgUrl;
                        imgControl.Visible = true;
                    }
                }

                var helpCircle = e.Item.FindControl("helpcircle_gif") as Image;
                helpCircle.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/" + "helpcircle.gif");
                helpCircle.Attributes.Add("onclick", "popuporderoptionwh('Order Option " + popupTitle + "', " + counter.ToString() + ",650,550,'yes');");

                // 2 Control choices for drop down list
                var cboUnitMeasureCode = e.Item.FindControl("cboUnitMeasureCode") as DropDownList;
                var lblUnitMeasureCode = e.Item.FindControl("lblUnitMeasureCode") as Label;
                var availableUnitMeasures = ProductDA.GetProductUnitMeasureAvailability(ThisCustomer.CustomerCode, itemCode,
                                                                                        AppLogic.AppConfigBool("ShowInventoryFromAllWarehouses"),
                                                                                        ThisCustomer.IsNotRegistered);
                if (availableUnitMeasures.Count() > 1)
                {
                    lblUnitMeasureCode.Visible = false;
                    foreach (string unitMeasureCode in availableUnitMeasures)
                    {
                        cboUnitMeasureCode.Items.Add(new ListItem(unitMeasureCode.ToHtmlEncode(), unitMeasureCode.ToHtmlEncode()));
                    }
                }
                else
                {
                    // The only unit measure the item is configured for is the default
                    // which we are guaranteed to be in the first index..
                    cboUnitMeasureCode.Visible = false;
                    lblUnitMeasureCode.Text = availableUnitMeasures.First().ToHtmlEncode();
                }

                bool withVat = AppLogic.AppConfigBool("VAT.Enabled") && ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive;
                var um = UnitMeasureInfo.ForItem(itemCode, UnitMeasureInfo.ITEM_DEFAULT);

                decimal promotionalPrice = decimal.Zero;
                decimal price = InterpriseHelper.GetSalesPriceAndTax(ThisCustomer.CustomerCode, itemCode, ThisCustomer.CurrencyCode, decimal.One, um.Code, withVat, ref promotionalPrice);
                if (promotionalPrice != decimal.Zero)
                {
                    price = promotionalPrice;
                }

                string vatDisplay = String.Empty;
                if (AppLogic.AppConfigBool("VAT.Enabled"))
                {
                    vatDisplay = (ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive) ?
                        " <span class=\"VATLabel\">" + AppLogic.GetString("showproduct.aspx.38", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "</span>\n" :
                        " <span class=\"VATLabel\">" + AppLogic.GetString("showproduct.aspx.37", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "</span>\n";
                }

                var lblPrice = e.Item.FindControl("OrderOptionPrice") as Label;
                lblPrice.Text = InterpriseHelper.FormatCurrencyForCustomer(price, ThisCustomer.CurrencyCode) + vatDisplay;

                var hfCounter = e.Item.FindControl("hfItemCounter") as HiddenField;
                hfCounter.Value = counter.ToString();

                var cbk = e.Item.FindControl("OrderOptions") as DataCheckBox;
                cbk.Checked = false;

                bool shouldBeAbleToEnterNotes = orderOptionNode["CheckOutOptionAddMessage"].InnerText.TryParseBool().Value;
                var lblNotes = e.Item.FindControl("lblNotes") as Label;
                var txtNotes = e.Item.FindControl("txtOrderOptionNotes") as TextBox;
                lblNotes.Visible = txtNotes.Visible = shouldBeAbleToEnterNotes;
                txtNotes.Attributes.Add("onkeyup", "return imposeMaxLength(this, 1000);");
            }
        }

        void btnContinueShoppingTop_Click(object sender, EventArgs e)
        {
            ContinueShopping();
        }

        void btnContinueShoppingBottom_Click(object sender, EventArgs e)
        {
            ContinueShopping();
        }

        void btnCheckOutNowTop_Click(object sender, EventArgs e)
        {
            DeleteOutofStockPhasedOutItem();
            ProcessCart(true);
        }

        void btnCheckOutNowBottom_Click(object sender, EventArgs e)
        {
            DeleteOutofStockPhasedOutItem();
            ProcessCart(true);
        }

        void btnUpdateCart1_Click(object sender, EventArgs e)
        {
            ProcessCart(false);
            InitializePageContent();
        }

        void btnUpdateCart2_Click(object sender, EventArgs e)
        {
            ProcessCart(false);
            InitializePageContent();
        }

        void btnUpdateCart3_Click(object sender, EventArgs e)
        {
            ProcessCart(false);
            InitializePageContent();
        }

        void btnUpdateCart4_Click(object sender, EventArgs e)
        {
            ProcessCart(false);
            InitializePageContent();
        }

        void btnUpdateCart5_Click(object sender, EventArgs e)
        {
            ProcessCart(false);
            InitializePageContent();
        }

        protected void btnGoogleCheckout_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!ThisCustomer.IsRegistered &&
                (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && AppLogic.AppConfigBool("GoogleCheckout.AllowAnonCheckout")))
            {
                Response.Redirect("checkoutanon.aspx?checkout=true&checkouttype=gc");
            }
            else
            {
                ProcessCart(false);

                if (!_cart.IsSalesOrderDetailBuilt)
                {
                    _cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
                    _cart.BuildSalesOrderDetails(CouponCode.Text);
                }

                Response.Redirect(GoogleCheckout.CreateGoogleCheckout(_cart));
            }
        }

        protected void btnPayPalExpressCheckout_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!ThisCustomer.IsRegistered &&
                (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && AppLogic.AppConfigBool("PayPalCheckout.AllowAnonCheckout")))
            {
                Response.Redirect("checkoutanon.aspx?checkout=true&checkouttype=pp");
            }
            else
            {
                ProcessCart(false);

                if (!_cart.IsSalesOrderDetailBuilt)
                {
                    _cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
                    _cart.BuildSalesOrderDetails(false, false, CouponCode.Text);
                }

                ThisCustomer.ThisCustomerSession["paypalfrom"] = "shoppingcart";
                Response.Redirect(PayPalExpress.CheckoutURL(_cart));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            OrderOptionsList.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(OrderOptionsList_ItemDataBound);
            btnContinueShoppingTop.Click += new EventHandler(btnContinueShoppingTop_Click);
            btnContinueShoppingBottom.Click += new EventHandler(btnContinueShoppingBottom_Click);
            btnCheckOutNowTop.Click += new EventHandler(btnCheckOutNowTop_Click);
            btnCheckOutNowBottom.Click += new EventHandler(btnCheckOutNowBottom_Click);
            btnUpdateCart1.Click += new EventHandler(btnUpdateCart1_Click);
            btnUpdateCart2.Click += new EventHandler(btnUpdateCart2_Click);
            btnUpdateCart3.Click += new EventHandler(btnUpdateCart3_Click);
            btnUpdateCart4.Click += new EventHandler(btnUpdateCart4_Click);
            btnUpdateCart5.Click += new EventHandler(btnUpdateCart5_Click);
        }

        private void RedirectToSignInPage()
        {
            // disable all buttons
            btnCheckOutNowBottom.Enabled = false;
            btnCheckOutNowTop.Enabled = false;
            btnContinueShoppingBottom.Enabled = false;
            btnContinueShoppingTop.Enabled = false;
            btnUpdateCart1.Enabled = false;
            btnUpdateCart2.Enabled = false;
            btnUpdateCart3.Enabled = false;
            btnUpdateCart4.Enabled = false;
            btnUpdateCart5.Enabled = false;

            BodyPanel.Visible = false;

            RedirectToSignInPageLiteral.Text = AppLogic.GetString("shoppingcart.cs.1011", SkinID, ThisCustomer.LocaleSetting, true);

            // perform redirect
            Response.AddHeader("REFRESH", string.Format("1; URL={0}", Server.UrlDecode("signin.aspx")));
        }

        private void InitializePageContent()
        {
            _skinImagePath = "skins/skin_" + SkinID.ToString() + "/images/";
            int AgeCartDays = AppLogic.AppConfigUSInt("AgeCartDays");
            if (AgeCartDays == 0)
            {
                AgeCartDays = 7;
            }

            string localeSetting = ThisCustomer.LocaleSetting;

            ShoppingCart.Age(ThisCustomer.CustomerID, AgeCartDays, CartTypeEnum.ShoppingCart);
            shoppingcartaspx8.Text = AppLogic.GetString("shoppingcart.aspx.6", SkinID, localeSetting);
            shoppingcartaspx10.Text = AppLogic.GetString("shoppingcart.aspx.8", SkinID, localeSetting);
            shoppingcartaspx11.Text = AppLogic.GetString("shoppingcart.aspx.9", SkinID, localeSetting);
            shoppingcartaspx9.Text = AppLogic.GetString("shoppingcart.aspx.7", SkinID, localeSetting);
            shoppingcartcs27.Text = AppLogic.GetString("shoppingcart.cs.5", SkinID, localeSetting);
            shoppingcartcs28.Text = AppLogic.GetString("shoppingcart.cs.6", SkinID, localeSetting);
            shoppingcartcs29.Text = AppLogic.GetString("shoppingcart.cs.7", SkinID, localeSetting);
            shoppingcartcs31.Text = AppLogic.GetString("shoppingcart.cs.9", SkinID, localeSetting);

            string updateCartKey = "shoppingcart.cs.33";
            string updateCart = AppLogic.GetString(updateCartKey, SkinID, localeSetting, true);
            btnUpdateCart1.Text = updateCart;
            btnUpdateCart2.Text = updateCart;
            btnUpdateCart3.Text = updateCart;
            btnUpdateCart4.Text = updateCart;
            btnUpdateCart5.Text = updateCart;

            lblOrderNotes.Text = AppLogic.GetString("shoppingcart.cs.13", SkinID, localeSetting);

            string continueShopping = AppLogic.GetString("shoppingcart.cs.12", SkinID, localeSetting, true);
            btnContinueShoppingTop.Text = continueShopping;
            btnContinueShoppingBottom.Text = continueShopping;

            string checkoutnow = AppLogic.GetString("shoppingcart.cs.34", SkinID, localeSetting, true);
            btnCheckOutNowTop.Text = checkoutnow;
            btnCheckOutNowBottom.Text = checkoutnow;

            btnCalcShip.Text = AppLogic.GetString("shoppingcart.aspx.26", SkinID, localeSetting, true);

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart1, updateCartKey);
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart2, updateCartKey);
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart3, updateCartKey);
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart4, updateCartKey);
                AppLogic.EnableButtonCaptionEditing(btnUpdateCart5, updateCartKey);

                AppLogic.EnableButtonCaptionEditing(btnContinueShoppingTop, "shoppingcart.cs.12");
                AppLogic.EnableButtonCaptionEditing(btnContinueShoppingBottom, "shoppingcart.cs.12");
                AppLogic.EnableButtonCaptionEditing(btnCheckOutNowTop, "shoppingcart.cs.34");
                AppLogic.EnableButtonCaptionEditing(btnCheckOutNowBottom, "shoppingcart.cs.34");
                AppLogic.EnableButtonCaptionEditing(btnCalcShip, "shoppingcart.aspx.26");
            }
            else
            {

                btnContinueShoppingBottom.OnClientClick = "self.location='" + AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting) + "'";

            }

            OrderNotes.Attributes.Add("onkeyup", "return imposeMaxLength(this, 255);");

            if (_cart == null)
            {
                _cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);

                if (!Page.IsPostBack)
                {
                    string couponCode = string.Empty;
                    string couponErrorMessage = string.Empty;

                    if (_cart.HasCoupon(ref couponCode) &&
                        _cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                    {
                        CouponCode.Text = couponCode;
                    }
                    else
                    {
                        ErrorMsgLabel.Text = couponErrorMessage;
                        _cart.ClearCoupon();
                    }

                    //check customer IsCreditHold
                    if (ThisCustomer.IsCreditOnHold && _cart != null)
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("shoppingcart.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
                        _cart.ClearCoupon();
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(CouponCode.Text)) _cart.ClearCoupon();
                }
            }

            if (_cart != null && !_cart.IsEmpty())
            {
                try
                {
                    _cart.BuildSalesOrderDetails(false, true, CouponCode.Text);
                }
                catch (InvalidOperationException ex)
                {
                    ErrorMsgLabel.Text = ex.Message;
                    return;
                }
                catch (Exception ex) { throw ex; }
            }

            //Refresh page with errors since some registry items has been removed
            int totalRemoved = _cart.RemoveRegistryItemsHasDeletedRegistry();
            int totalItemsremoved = _cart.RemoveRegistryItemsHasBeenDeletedInRegistry();
            if (totalRemoved > 0 || totalItemsremoved > 0)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("editgiftregistry.error.18", SkinID, ThisCustomer.LocaleSetting, true).ToUrlEncode());
            }

            if (_cart.IsEmpty())
            {
                btnUpdateCart1.Visible = false;
                AlternativeCheckoutsTop.Visible = false;
                AlternativeCheckoutsBottom.Visible = false;
            }

            RenderValidationScript();

            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();

            string XmlPackageName = AppLogic.AppConfig("XmlPackage.ShoppingCartPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_ShoppingCartPageHeader.Text = AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }

            string XRI = AppLogic.LocateImageURL(_skinImagePath + "redarrow.gif");
            redarrow1.ImageUrl = XRI;
            redarrow2.ImageUrl = XRI;
            redarrow3.ImageUrl = XRI;
            redarrow4.ImageUrl = XRI;

            ShippingInformation.Visible = (!AppLogic.AppConfigBool("SkipShippingOnCheckout"));
            AddresBookLlink.Visible = (ThisCustomer.IsRegistered);

            bool isNotCartEmpty = !_cart.IsEmpty();
            btnCheckOutNowTop.Visible = isNotCartEmpty;

            if (!IsPostBack)
            {
                if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").Length != 0 || ErrorMsgLabel.Text.Length > 0)
                {
                    if (CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg").IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        throw new ArgumentException("SECURITY EXCEPTION");
                    }
                    pnlErrorMsg.Visible = true;
                    ErrorMsgLabel.Text += Server.HtmlEncode(CommonLogic.QueryStringCanBeDangerousContent("ErrorMsg"));
                }
            }

            if (_cart.HasNoStockPhasedOutItem)
            {
                pnlRemovePhasedOutItemWithNoStockError.Visible = true;
                var errormessage = new Topic("RemoveNoStockPhasedOutItemText", _cart.ThisCustomer.LocaleSetting, _cart.SkinID, null);
                RemovePhasedOutItemWithNoStockError.Text = errormessage.Contents;
            }

            if (_cart.InventoryTrimmed)
            {
                pnlInventoryTrimmedError.Visible = true;
                InventoryTrimmedError.Text = AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting, true);
            }

            if (_cart.MinimumQuantitiesUpdated)
            {
                pnlMinimumQuantitiesUpdatedError.Visible = true;
                MinimumQuantitiesUpdatedError.Text = AppLogic.GetString("shoppingcart.aspx.5", SkinID, ThisCustomer.LocaleSetting, true);
            }

            decimal MinOrderAmount = AppLogic.AppConfigUSDecimal("CartMinOrderAmount");
            if (!_cart.MeetsMinimumOrderAmount(MinOrderAmount))
            {
                pnlMeetsMinimumOrderAmountError.Visible = true;
                string amountFormatted = InterpriseHelper.FormatCurrencyForCustomer(MinOrderAmount, ThisCustomer.CurrencyCode);
                MeetsMinimumOrderAmountError.Text = string.Format(AppLogic.GetString("shoppingcart.aspx.2", SkinID, ThisCustomer.LocaleSetting, true), amountFormatted);
            }

            int quantityDecimalPlaces = InterpriseHelper.GetInventoryDecimalPlacesPreference();
            var formatter = (new CultureInfo(ThisCustomer.LocaleSetting)).NumberFormat;

            // setup the formatter
            formatter.NumberDecimalDigits = quantityDecimalPlaces;
            formatter.PercentDecimalDigits = quantityDecimalPlaces;

            MeetsMinimumOrderQuantityError.Text = string.Empty;
            decimal MinQuantity = AppLogic.AppConfigUSDecimal("MinCartItemsBeforeCheckout");
            if (!_cart.MeetsMinimumOrderQuantity(MinQuantity))
            {
                pnlMeetsMinimumOrderQuantityError.Visible = true;
                MeetsMinimumOrderQuantityError.Text = string.Format(AppLogic.GetString("shoppingcart.aspx.16", SkinID, ThisCustomer.LocaleSetting, true), MinQuantity.ToString(), MinQuantity.ToString());
            }

            CartItems.Text = _cart.RenderHTMLLiteral(new DefaultShoppingCartPageLiteralRenderer(RenderType.ShoppingCart, CouponCode.Text));
            //CartItems.Text = cart.RenderHTMLLiteral(new ShoppingCartPageLiteralRenderer());
            // CartSubTotal.Text = cart.RenderHTMLLiteral(new ShoppingCartPageSummaryLiteralRenderer());

            if (!_cart.IsEmpty())
            {
                ShoppingCartorderoptions_gif.ImageUrl = AppLogic.LocateImageURL(_skinImagePath + "ShoppingCartorderoptions.gif");
                string strXml = string.Empty;
                pnlErrorMsg.Visible = true;

                if (AppLogic.AppConfigBool("RequireOver13Checked") && ThisCustomer.IsRegistered && !ThisCustomer.IsOver13)
                {
                    btnCheckOutNowTop.Enabled = false;
                    btnCheckOutNowBottom.Enabled = false;
                    ErrorMsgLabel.Text = AppLogic.GetString("over13oncheckout", SkinID, ThisCustomer.LocaleSetting, true);
                    return;
                }

                btnCheckOutNowBottom.Enabled = btnCheckOutNowTop.Enabled;

                string upsellproductlist = GetUpsellProducts(_cart);
                if (upsellproductlist.Length > 0)
                {
                    UpsellProducts.Text = upsellproductlist;
                    btnUpdateCart5.Visible = true;
                    pnlUpsellProducts.Visible = true;
                }
                else
                {
                    UpsellProducts.Text = string.Empty;
                    pnlUpsellProducts.Visible = false;
                }

                if (_cart.CouponsAllowed)
                {
                    ShoppingCartCoupon_gif.ImageUrl = AppLogic.LocateImageURL(_skinImagePath + "ShoppingCartCoupon.gif");
                    pnlCoupon.Visible = true;
                }
                else
                {
                    pnlCoupon.Visible = false;
                }

                ShoppingCartNotes_gif.ImageUrl = AppLogic.LocateImageURL(_skinImagePath + "ShoppingCartNotes.gif");

                DoOrderNotesChecking();

                btnCheckOutNowBottom.Visible = true;

                if (ThisCustomer.IsNotRegistered)
                {
                    //Remove this line of code to enable the "Coupon Support for Anonymous Customers" feature
                    //---------------------------------------------------------------------------------------
                    pnlCoupon.Visible = false;
                    //---------------------------------------------------------------------------------------
                    pnlOrderNotes.Visible = false;
                }
                
                if (!_cart.CartItems.All(item => item.IsOverSized))
                {
                    if (AppLogic.AppConfigBool("ShippingCalculator.Enabled"))
                    {
                        pnlShippingCalculator.Visible = true;
                        this.btnCalcShip.Attributes["onClick"] = "return false";
                    }
                }
                else
                {
                    pnlShippingCalculator.Visible = false;
                }

                //Check if alternate checkout methods are supported (PayPal and GoogleCheckout)
                if (AppLogic.IsSupportedAlternateCheckout)
                {
                    //Set the image url for the google button.
                    if (AppLogic.AppConfigBool("GoogleCheckout.UseSandbox"))
                    {
                        //Multishipping of gift registry not supported in google checkout
                        if (_cart != null && !_cart.HasRegistryItems())
                        {
                            if (AppLogic.UseSSL() && AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") == "1")
                            {
                                btnGoogleCheckoutTop.ImageUrl = string.Format("https://sandbox.google.com/checkout/buttons/checkout.gif?merchant_id={0}&w=180&h=46&style=trans&variant=text", AppLogic.AppConfig("GoogleCheckout.SandboxMerchantId"));
                                btnGoogleCheckoutBottom.ImageUrl = string.Format("https://sandbox.google.com/checkout/buttons/checkout.gif?merchant_id={0}&w=180&h=46&style=trans&variant=text", AppLogic.AppConfig("GoogleCheckout.SandboxMerchantId"));
                            }
                            else
                            {
                                btnGoogleCheckoutTop.ImageUrl = string.Format(AppLogic.AppConfig("GoogleCheckout.SandBoxCheckoutButton"), AppLogic.AppConfig("GoogleCheckout.SandboxMerchantId"));
                                btnGoogleCheckoutBottom.ImageUrl = string.Format(AppLogic.AppConfig("GoogleCheckout.SandBoxCheckoutButton"), AppLogic.AppConfig("GoogleCheckout.SandboxMerchantId"));
                            }
                        }
                    }
                    else
                    {
                        //Multishipping of gift registry not supported in google checkout
                        if (_cart != null && !_cart.HasRegistryItems())
                        {
                            if (AppLogic.UseSSL() && AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") == "1")
                            {
                                btnGoogleCheckoutTop.ImageUrl = string.Format("https://checkout.google.com/buttons/checkout.gif?merchant_id={0}&w=180&h=46&style=trans&variant=text", AppLogic.AppConfig("GoogleCheckout.MerchantId"));
                                btnGoogleCheckoutBottom.ImageUrl = string.Format("https://checkout.google.com/buttons/checkout.gif?merchant_id={0}&w=180&h=46&style=trans&variant=text", AppLogic.AppConfig("GoogleCheckout.MerchantId"));
                            }
                            else
                            {
                                btnGoogleCheckoutTop.ImageUrl = string.Format(AppLogic.AppConfig("GoogleCheckout.LiveCheckoutButton"), AppLogic.AppConfig("GoogleCheckout.MerchantId"));
                                btnGoogleCheckoutBottom.ImageUrl = string.Format(AppLogic.AppConfig("GoogleCheckout.LiveCheckoutButton"), AppLogic.AppConfig("GoogleCheckout.MerchantId"));
                            }
                        }
                    }

                    bool hidePaypalOptionIfMultiShipAndHasGiftRegistry = !(_cart.HasMultipleShippingAddresses() || _cart.HasRegistryItems());

                    if (AppLogic.AppConfigBool("PayPalCheckout.ShowOnCartPage") && (ThisCustomer.IsRegistered || !AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                        && (_cart != null && !_cart.IsEmpty() && _cart.SalesOrderDataset.CustomerSalesOrderView[0].SubTotalRate > Decimal.Zero && hidePaypalOptionIfMultiShipAndHasGiftRegistry))
                    {
                        if (AppLogic.UseSSL() && AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") == "1")
                        {
                            btnPayPalExpressCheckoutTop.ImageUrl = "https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif";
                            btnPayPalExpressCheckoutBottom.ImageUrl = "https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif";
                        }
                        else
                        {
                            btnPayPalExpressCheckoutTop.ImageUrl = "http://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif";
                            btnPayPalExpressCheckoutBottom.ImageUrl = "http://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif";
                        }
                        AlternativeCheckoutsTop.Visible = true;
                        AlternativeCheckoutsBottom.Visible = true;
                        PayPalExpressSpanTop.Visible = true;
                        PayPalExpressSpanBottom.Visible = true;
                    }
                    else
                    {
                        AlternativeCheckoutsTop.Visible = false;
                        AlternativeCheckoutsBottom.Visible = false;
                        PayPalExpressSpanTop.Visible = false;
                        PayPalExpressSpanBottom.Visible = false;
                    }

                    if (AppLogic.AppConfigBool("GoogleCheckout.ShowOnCartPage"))
                    {
                        if (InterpriseShoppingCart.IsWebCheckOutIncluded("Google"))
                        {
                            //Multishipping of gift registry not supported in google checkout
                            if (_cart != null && !_cart.HasRegistryItems())
                            {
                                AlternativeCheckoutsTop.Visible = true;
                                AlternativeCheckoutsBottom.Visible = true;
                                GoogleCheckoutSpanTop.Visible = true;
                                GoogleCheckoutSpanBottom.Visible = true;
                            }
                        }
                    }

                    if (_cart != null && _cart.IsShipSeparatelyCount() > 0)
                    {
                        //Multishipping of gift registry not supported in google checkout
                        if (!_cart.HasRegistryItems())
                        {
                            AlternativeCheckoutsTop.Visible = false;
                            AlternativeCheckoutsBottom.Visible = false;
                            GoogleCheckoutSpanTop.Visible = false;
                            GoogleCheckoutSpanBottom.Visible = false;
                        }
                    }
                }

                //if no alternative methods are visible, hide the whole row
                
                if (!AppLogic.IsSupportedAlternateCheckout && AlternativeCheckoutsTop.Visible == true && AlternativeCheckoutsBottom.Visible == true)
                {
                    ErrorMsgLabel.Text = PayPalExpress.ErrorMsg;
                    AlternativeCheckoutsTop.Visible = false;
                    AlternativeCheckoutsBottom.Visible = false;
                }

            }
            else
            {
                pnlOrderOptions.Visible = false;
                pnlUpsellProducts.Visible = false;
                pnlCoupon.Visible = false;
                pnlOrderNotes.Visible = false;
                btnCheckOutNowBottom.Visible = false;
                pnlShippingCalculator.Visible = false;
            }

            btnContinueShoppingBottom.OnClientClick = "self.location='" + AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting) + "'";
            CartPageFooterTopic.SetContext = this;

            string XmlPackageName2 = AppLogic.AppConfig("XmlPackage.ShoppingCartPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_ShoppingCartPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }
        }

        private void RenderOrderOptions()
        {
            string strXml = String.Empty;
            var optionsCount = ServiceFactory.GetInstance<IProductService>().GetCheckOutOptionCount(ref strXml);

            pnlOrderOptions.Visible = (optionsCount > 0);
            if (optionsCount > 0)
            {
                var xDoc = new XmlDocument();
                xDoc.LoadXml(strXml);

                var xslDoc = new XmlDocument();
                xslDoc.LoadXml("<?xml version=\"1.0\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:param name=\"locale\" /><xsl:template match=\"/\"><xsl:for-each select=\"*\"><xsl:copy><xsl:for-each select=\"*\"><xsl:copy><xsl:for-each select=\"*\"><xsl:copy><xsl:choose><xsl:when test=\"ml\"><xsl:value-of select=\"ml/locale[@name=$locale]\"/></xsl:when><xsl:otherwise><xsl:value-of select=\".\"/></xsl:otherwise></xsl:choose></xsl:copy></xsl:for-each></xsl:copy></xsl:for-each></xsl:copy></xsl:for-each></xsl:template></xsl:stylesheet>");

                var xsl = new XslCompiledTransform();
                xsl.Load(xslDoc);

                var tw = new StringWriter();
                var xslArgs = new XsltArgumentList();
                xslArgs.AddParam("locale", "", ThisCustomer.LocaleSetting);
                xsl.Transform(xDoc, xslArgs, tw);

                var xResults = new XmlDocument();
                xResults.LoadXml(tw.ToString());

                var nodeList = xResults.SelectNodes("//orderoption");

                OrderOptionsList.DataSource = nodeList;
                OrderOptionsList.DataBind();
            }
        }

        private void RenderValidationScript()
        {
            var html = new StringBuilder();
            html.Append("<script type=\"text/javascript\" Language=\"JavaScript\">\n");
            html.Append("function Cart_Validator(theForm)\n");
            html.Append("{\n");
            string cartJS = CommonLogic.ReadFile("jscripts/shoppingcart.js", true);
            foreach (var c in _cart.CartItems)
            {
                string itemJS = string.Empty;

                itemJS = cartJS.Replace("%MAX_QUANTITY_INPUT%", AppLogic.MAX_QUANTITY_INPUT_NoDec).Replace("%ALLOWED_QUANTITY_INPUT%", AppLogic.GetQuantityRegularExpression(c.ItemType, true));
                itemJS = itemJS.Replace("%DECIMAL_SEPARATOR%", Localization.GetNumberDecimalSeparatorLocaleString(ThisCustomer.LocaleSetting)).Replace("%LOCALE_ZERO%", Localization.GetNumberZeroLocaleString(ThisCustomer.LocaleSetting));
                itemJS = itemJS.Replace("%DECIMAL_PLACE%", AppLogic.InventoryDecimalPlacesPreference.ToString());
                html.Append(itemJS.Replace("%SKU%", c.m_ShoppingCartRecordID.ToString()));
            }
            html.Append("return(true);\n");
            html.Append("}\n");
            html.Append("function imposeMaxLength(theControl, maxLength)\n");
            html.Append("{\n");
            html.Append("theControl.value = theControl.value.substring(0, maxLength);\n");
            html.Append("}\n");
            html.Append("</script>\n");

            string x = ThisCustomer.LocaleSetting;
            ValidationScript.Text = html.ToString();
        }

        public string GetUpsellProducts(ShoppingCart cart)
        {
            StringBuilder UpsellProductList = new StringBuilder(1024);
            StringBuilder results = new StringBuilder("");

            // ----------------------------------------------------------------------------------------
            // WRITE OUT UPSELL PRODUCTS:
            // ----------------------------------------------------------------------------------------
            if (AppLogic.AppConfigBool("ShowAccessoryProductsOnCartPage"))
            {
                string S = string.Empty;
                try
                {
                    int upsellProductLimit = AppLogic.AppConfigUSInt("AccessoryProductsLimitNumberOnCart");
                    if (upsellProductLimit == 0)
                    {
                        upsellProductLimit = 10;
                    }
                    S = InterpriseHelper.ShowInventoryAccessoryOptions(string.Empty, true, upsellProductLimit, string.Empty, ThisCustomer, false, false, InterpriseHelper.ViewingPage.ShoppingCart);
                }
                catch { }
                if (S.Length != 0)
                {
                    results.Append("<br/>");
                    results.Append("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" style=\"border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor") + "\">\n");
                    results.Append("<tr><td align=\"left\" valign=\"top\">\n");
                    results.Append("<span class=\"UpsellSectionLabel\"> " + AppLogic.GetString("shoppingcart.aspx.19", SkinID, ThisCustomer.LocaleSetting) + " </span>");
                    results.Append("<table width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\" style=\"" + AppLogic.AppConfig("BoxFrameStyle") + "\">\n");
                    results.Append("<tr><td align=\"left\" valign=\"top\">\n");

                    results.Append(S);

                    results.Append("</td></tr>\n");
                    results.Append("</table>\n");
                    results.Append("</td></tr>\n");
                    results.Append("</table>\n");
                }

            }
            return results.ToString();
        }

        private void DeleteOutofStockPhasedOutItem()
        {
            if (_cart.IsEmpty()) return;
            var lst = _cart.CartItems.Where(item => item.Status == "P" && item.IsOutOfStock);
            foreach (CartItem item in lst)
            {
                var shoppingCartService = ServiceFactory.GetInstance<IShoppingCartService>();
                shoppingCartService.ClearLineItemsAndKitComposition(new String[] {item.Id.ToString()});
            }
        }

        private void ProcessCart(bool DoingFullCheckout)
        {
            this.PageNoCache();

            ThisCustomer.RequireCustomerRecord();
            CartTypeEnum cte = CartTypeEnum.ShoppingCart;

            if (CommonLogic.QueryStringCanBeDangerousContent("CartType").Length != 0)
            {
                cte = (CartTypeEnum)CommonLogic.QueryStringUSInt("CartType");
            }

            _cart = new InterpriseShoppingCart(null, 1, ThisCustomer, cte, string.Empty, false, true);

            if (!Page.IsPostBack)
            {
                string couponCode = string.Empty;
                if (_cart.HasCoupon(ref couponCode))
                {
                    CouponCode.Text = couponCode;
                }

            }
            else
            {
                if (CouponCode.Text.IsNullOrEmptyTrimmed())
                {
                    _cart.ClearCoupon();
                }
            }

            // check if credit on hold
            if (ThisCustomer.IsCreditOnHold) { Response.Redirect("shoppingcart.aspx"); }

            if (_cart.IsEmpty())
            {
                // can't have this at this point:
                switch (cte)
                {
                    case CartTypeEnum.ShoppingCart:
                        Response.Redirect("shoppingcart.aspx");
                        break;
                    case CartTypeEnum.WishCart:
                        Response.Redirect("wishlist.aspx");
                        break;
                    case CartTypeEnum.GiftRegistryCart:
                        Response.Redirect("giftregistry.aspx");
                        break;
                    default:
                        Response.Redirect("shoppingcart.aspx");
                        break;
                }
            }
            

            // update cart quantities:
            for (int i = 0; i <= Request.Form.Count - 1; i++)
            {
                string fld = Request.Form.Keys[i];
                string fldval = Request.Form[Request.Form.Keys[i]];
                int recID;
                string quantity;
                if (fld.StartsWith("Quantity"))
                {
                    if (fldval.StartsWith(Localization.GetNumberDecimalSeparatorLocaleString(_cart.ThisCustomer.LocaleSetting)))
                    {
                        fldval = fldval.Insert(0, Localization.GetNumberZeroLocaleString(_cart.ThisCustomer.LocaleSetting));
                    }
                    if (Regex.IsMatch(fldval, AppLogic.AllowedQuantityWithDecimalRegEx(_cart.ThisCustomer.LocaleSetting), RegexOptions.Compiled))
                    {
                        recID = Localization.ParseUSInt(fld.Substring("Quantity".Length + 1));
                        quantity = fldval;
                        decimal iquan = Convert.ToDecimal(quantity);

                        //check if gift registry item exceeds the maximum No.# of IN-NEED quantity
                        decimal? regItemQty = GiftRegistryDA.GetGiftRegistryItemQuantityByCartRecID(recID);
                        if ((regItemQty.HasValue && regItemQty > 0) && iquan > regItemQty)
                        {
                            ErrorMsgLabel.Text += AppLogic.GetString("editgiftregistry.error.14", SkinID, ThisCustomer.LocaleSetting, true);
                        }
                        else if (regItemQty.HasValue && regItemQty == 0) // blocks the user from
                        {
                            ErrorMsgLabel.Text += AppLogic.GetString("editgiftregistry.error.15", SkinID, ThisCustomer.LocaleSetting, true);
                        }
                        else
                        {
                            if (iquan < 0) { iquan = 0; }
                            _cart.SetItemQuantity(recID, iquan);
                        }
                    }
                    else
                    {
                        ErrorMsgLabel.Text += "The item quantity must have a valid input.";
                    }
                }

                if (fld.StartsWith("UnitMeasureCode"))
                {
                    if (!fldval.IsNullOrEmptyTrimmed())
                    {
                        recID = Localization.ParseUSInt(fld.Substring("UnitMeasureCode".Length + 1));
                        string unitMeasureCode = fldval.ToHtmlDecode();
                        _cart.UpdateUnitMeasureForItem(recID, unitMeasureCode, (_cart.HasMultipleShippingAddresses() || (_cart.HasRegistryItems() && _cart.CartItems.Count > 0)), _cart.HasRegistryItems());
                    }
                }

            }

            // save coupon code, no need to reload cart object
            // will update customer record also:
            if (cte == CartTypeEnum.ShoppingCart)
            {
                if (!string.IsNullOrEmpty(CouponCode.Text))
                {
                    string errorMessage = string.Empty;
                    if (_cart.IsCouponValid(ThisCustomer, CouponCode.Text, ref errorMessage))
                    {
                        _cart.ApplyCoupon(CouponCode.Text);
                    }
                    else
                    {
                        // NULL out the coupon for this cusotmer...
                        string customerCode = CommonLogic.IIF(ThisCustomer.IsRegistered, ThisCustomer.CustomerCode, ThisCustomer.CustomerID);
                        InterpriseHelper.ClearCustomerCoupon(customerCode, ThisCustomer.IsRegistered);

                        ErrorMsgLabel.Text = errorMessage;
                        CouponCode.Text = string.Empty;

                        //rebiuld the shopping for the renderer computation
                        //if not rebuild cart.SalesOrderDataset.TransactionItemTaxDetailView will be null.
                        _cart.BuildSalesOrderDetails(false, true, CouponCode.Text);
                        return;
                    }
                }

                // check for upsell products
                if (CommonLogic.FormCanBeDangerousContent("Upsell").Length != 0)
                {
                    foreach (string s in CommonLogic.FormCanBeDangerousContent("Upsell").Split(','))
                    {
                        int ProductID = Localization.ParseUSInt(s);
                        if (ProductID == 0) { continue; }

                        string itemCode = InterpriseHelper.GetInventoryItemCode(ProductID);
                        string shippingAddressID;

                        shippingAddressID = CommonLogic.IIF(ThisCustomer.IsNotRegistered, string.Empty, ThisCustomer.PrimaryShippingAddressID);

                        var umInfo = InterpriseHelper.GetItemDefaultUnitMeasure(itemCode);
                        _cart.AddItem(ThisCustomer, shippingAddressID, itemCode, ProductID, 1, umInfo.Code, CartTypeEnum.ShoppingCart);
                    }
                }

                bool hasCheckedOptions = false;

                if (pnlOrderOptions.Visible)
                {
                    // Process the Order Options
                    foreach (RepeaterItem ri in OrderOptionsList.Items)
                    {
                        var cbk = (DataCheckBox)ri.FindControl("OrderOptions");
                        if (cbk.Checked)
                        {
                            hasCheckedOptions = true;
                            string itemCode = (string)cbk.Data;
                            var hfCounter = ri.FindControl("hfItemCounter") as HiddenField;
                            var txtNotes = ri.FindControl("txtOrderOptionNotes") as TextBox;

                            string strNotes = txtNotes.Text.ToHtmlEncode();
                            string notes = CommonLogic.IIF((strNotes != null), CommonLogic.CleanLevelOne(strNotes), string.Empty);

                            //check the length of order option notes
                            //should not exceed 1000 characters including spaces
                            int maxLen = 1000;
                            if (notes.Length > maxLen)
                            {
                                notes = notes.Substring(0, maxLen);
                            }

                            string unitMeasureCode = string.Empty;

                            // check if the item has only 1 unit measure
                            // hence it's rendered as a label
                            // else it would be rendered as a drop down list
                            var lblUnitMeasureCode = ri.FindControl("lblUnitMeasureCode") as Label;
                            if (null != lblUnitMeasureCode && lblUnitMeasureCode.Visible)
                            {
                                unitMeasureCode = lblUnitMeasureCode.Text;
                            }
                            else
                            {
                                // it's rendered as combobox because the item has multiple unit measures configured
                                var cboUnitMeasureCode = ri.FindControl("cboUnitMeasureCode") as DropDownList;
                                if (null != cboUnitMeasureCode && cboUnitMeasureCode.Visible)
                                {
                                    unitMeasureCode = cboUnitMeasureCode.SelectedValue;
                                }
                            }

                            if (CommonLogic.IsStringNullOrEmpty(unitMeasureCode))
                            {
                                throw new ArgumentException("Unit Measure not specified!!!");
                            }

                            //check if this Order Option has Restricted Quantity and Minimum Order Qty set.
                            decimal itemQuantity = 1;

                            using (var con = DB.NewSqlConnection())
                            {
                                con.Open();
                                using (var reader = DB.GetRSFormat(con, "SELECT iw.RestrictedQuantity, iw.MinOrderQuantity FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryItemWebOption iw with (NOLOCK) ON i.ItemCode = iw.ItemCode AND iw.WebsiteCode = {0} WHERE i.ItemCode = {1}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode), DB.SQuote(itemCode)))
                                {
                                    if (reader.Read())
                                    {
                                        string restrictedQuantitiesValue = DB.RSField(reader, "RestrictedQuantity");
                                        decimal minimumOrderQuantity = Convert.ToDecimal(DB.RSFieldDecimal(reader, "MinOrderQuantity"));
                                        if (!CommonLogic.IsStringNullOrEmpty(restrictedQuantitiesValue))
                                        {
                                            string[] quantityValues = restrictedQuantitiesValue.Split(',');
                                            if (quantityValues.Length > 0)
                                            {
                                                int ctr = 0;
                                                bool loop = true;
                                                while (loop)
                                                {
                                                    int quantity = 0;
                                                    string quantityValue = quantityValues[ctr];
                                                    if (int.TryParse(quantityValue, out quantity))
                                                    {
                                                        if (quantity >= minimumOrderQuantity)
                                                        {
                                                            itemQuantity = quantity;
                                                            loop = false;
                                                        }
                                                    }
                                                    ctr++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (minimumOrderQuantity > 0)
                                            {
                                                itemQuantity = minimumOrderQuantity;
                                            }
                                        }
                                    }
                                }
                            }
                            // Add the selected Order Option....
                            Guid cartItemId = Guid.Empty;
                            cartItemId = _cart.AddItem(ThisCustomer, ThisCustomer.PrimaryShippingAddressID, itemCode, int.Parse(hfCounter.Value), itemQuantity, unitMeasureCode, CartTypeEnum.ShoppingCart);
                            _cart.SetItemNotes(cartItemId, notes);
                        }
                    }
                }

                if (hasCheckedOptions)
                {
                    //refresh the option items
                    RenderOrderOptions();
                }

                if (ThisCustomer.IsRegistered)
                {
                    string sOrderNotes = CommonLogic.CleanLevelOne(OrderNotes.Text);
                    //check the length of order notes
                    //should not exceed 255 characters including spaces
                    if (sOrderNotes.Length > DomainConstants.ORDER_NOTE_MAX_LENGTH)
                    {
                        sOrderNotes = sOrderNotes.Substring(0, DomainConstants.ORDER_NOTE_MAX_LENGTH);
                    }

                    DB.ExecuteSQL(
                        String.Format("UPDATE Customer SET Notes = {0} WHERE CustomerCode = {1}",
                        sOrderNotes.ToDbQuote(),
                        ThisCustomer.CustomerCode.ToDbQuote())
                    );
                }

            }
            bool validated = true;
            if (_cart.InventoryTrimmed)
            {
                // inventory got adjusted, send them back to the cart page to confirm the new values!
                ErrorMsgLabel.Text += AppLogic.GetString("shoppingcart.cs.43", SkinID, ThisCustomer.LocaleSetting, true).ToUrlDecode();
                validated = false;
            }

            _cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);

            if (AppLogic.AppConfigBool("ShowShipDateInCart") && AppLogic.AppConfigBool("ShowStockHints"))
            {
                _cart.BuildSalesOrderDetails(false, true, CouponCode.Text);
            }

            if (cte == CartTypeEnum.WishCart)
            {
                Response.Redirect("wishlist.aspx");
            }
            if (cte == CartTypeEnum.GiftRegistryCart)
            {
                Response.Redirect("giftregistry.aspx");
            }

            if (DoingFullCheckout)
            {

                if (!_cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
                {
                    validated = false;
                }

                if (!_cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
                {
                    validated = false;
                }

                string couponCode = string.Empty;
                string couponErrorMessage = string.Empty;
                if (_cart.HasCoupon(ref couponCode) && !_cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                {
                    validated = false;
                }

                if (ThisCustomer.IsRegistered && AppLogic.AppConfigBool("Checkout.UseOnePageCheckout") &&
                    !_cart.HasMultipleShippingAddresses() && !_cart.HasRegistryItems())
                {
                    Response.Redirect("checkout1.aspx");
                }

                if (validated)
                {
                    if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryBillingAddressID == string.Empty)) // || !ThisCustomer.HasAtLeastOneAddress()
                    {
                        Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=true&addressType=Billing");
                    }

                    if (ThisCustomer.IsRegistered && (ThisCustomer.PrimaryShippingAddressID == string.Empty)) //  || !ThisCustomer.HasAtLeastOneAddress()
                    {
                        Response.Redirect("selectaddress.aspx?add=true&setPrimary=true&checkout=False&addressType=Shipping");
                    }

                    if (ThisCustomer.IsNotRegistered || ThisCustomer.PrimaryBillingAddressID == string.Empty || ThisCustomer.PrimaryShippingAddressID == string.Empty || !ThisCustomer.HasAtLeastOneAddress())
                    {
                        Response.Redirect("checkoutanon.aspx?checkout=true");
                    }
                    else
                    {
                        if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || !_cart.HasShippableComponents())
                        {
                            _cart.MakeShippingNotRequired();
                            Response.Redirect("checkoutpayment.aspx");
                        }
                        if ((_cart.HasMultipleShippingAddresses() && _cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && _cart.CartAllowsShippingMethodSelection) || _cart.HasRegistryItems())
                        {
                            Response.Redirect("checkoutshippingmult.aspx");
                        }
                        else
                        {
                            Response.Redirect("checkoutshipping.aspx");
                        }
                    }
                }
                InitializePageContent();
            }
        }

        private void ClearErrors()
        {
            CouponError.Text = string.Empty;
            ErrorMsgLabel.Text = string.Empty;
            RemovePhasedOutItemWithNoStockError.Text = string.Empty;
            InventoryTrimmedError.Text = string.Empty;
            MinimumQuantitiesUpdatedError.Text = string.Empty;
            MeetsMinimumOrderAmountError.Text = string.Empty;
            MeetsMinimumOrderQuantityError.Text = string.Empty;
            Micropay_EnabledError.Text = string.Empty;
        }

        private void ContinueShopping()
        {
            if (AppLogic.AppConfig("ContinueShoppingURL") == "")
            {
                if (ViewState["ReturnURL"] == null || ViewState["ReturnURL"].ToString() == "")
                {
                    Response.Redirect("default.aspx");
                }
                else
                {
                    Response.Redirect(ViewState["ReturnURL"].ToString());
                }
            }
            else
            {
                Response.Redirect(AppLogic.AppConfig("ContinueShoppingURL"));
            }
        }

        private void DoOrderNotesChecking()
        {
            if (IsPostBack) { return; }
            if (!AppLogic.AppConfigBool("DisallowOrderNotes"))
            {
                OrderNotes.Text = _cart.OrderNotes;
                pnlOrderNotes.Visible = true;
            }
            else
            {
                pnlOrderNotes.Visible = false;
            }
        }

        #endregion
    }
}



