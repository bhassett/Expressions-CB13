// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Globalization;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceControls;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;
using System.Collections.Generic;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for ShoppingCartPage.
    /// </summary>
    public partial class ShoppingCartPage : SkinBase
    {
        string SkinImagePath = string.Empty;
        InterpriseShoppingCart cart = null;

        public bool HasCouponAndCouponValid { get; set; }

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

            RedirectToSignInPageLiteral.Text = AppLogic.GetString("shoppingcart.cs.1011", SkinID, ThisCustomer.LocaleSetting);

            // perform redirect
            Response.AddHeader("REFRESH", string.Format("1; URL={0}", Server.UrlDecode("signin.aspx")));
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            SkinImagePath = "skins/skin_" + SkinID.ToString() + "/images/";
            this.RequireCustomerRecord();
            RequireSecurePage();
            SectionTitle = AppLogic.GetString("AppConfig.CartPrompt", SkinID, ThisCustomer.LocaleSetting);
            ClearErrors();

            if (!this.IsPostBack)
            {
                string returnurl = CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");
                if (returnurl.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    throw new ArgumentException("SECURITY EXCEPTION");
                }
                ViewState["returnurl"] = returnurl;
                InitializePageContent();
            }

            string[] formkeys = Request.Form.AllKeys;
            foreach (string s in formkeys)
            {
                if (s != "bt_Delete") { continue; }
                ProcessCart(false);
                InitializePageContent();
            }

            HeaderMsg.SetContext = this;
            CartPageFooterTopic.SetContext = this;
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

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

        #endregion
        
        void OrderOptionsList_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            XmlNode orderOptionNode = e.Item.DataItem as XmlNode;
            int counter = 0;
            if (orderOptionNode != null &&
                int.TryParse(orderOptionNode["Counter"].InnerText, out counter))
            {
                string itemCode = orderOptionNode["ItemCode"].InnerText;
                string itemName = orderOptionNode["ItemName"].InnerText;
                string itemDescription = orderOptionNode["ItemDescription"].InnerText;
                string popupTitle = string.Empty;

                Label lblDisplayName = e.Item.FindControl("OrderOptionName") as Label;
                if(!CommonLogic.IsStringNullOrEmpty(itemDescription))
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
                helpCircle.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "helpcircle.gif");
                helpCircle.Attributes.Add("onclick", "popuporderoptionwh('Order Option " + popupTitle + "', " + counter.ToString() + ",650,550,'yes');");

                // 2 Control choices for drop down list
                var cboUnitMeasureCode = e.Item.FindControl("cboUnitMeasureCode") as DropDownList;
                var lblUnitMeasureCode = e.Item.FindControl("lblUnitMeasureCode") as Label;
                var availableUnitMeasures = new List<string>();
                
                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var reader = DB.GetRSFormat(con, "exec eCommerceGetProductUnitMeasureAvailability @CustomerCode = {0}, @ItemCode = {1}, @IncludeAllWarehouses = {2}, @Anon = {3}", DB.SQuote(cart.ThisCustomer.CustomerCode), DB.SQuote(itemCode), AppLogic.AppConfigBool("ShowInventoryFromAllWarehouses"), cart.ThisCustomer.IsNotRegistered))
                    {
                        while (reader.Read())
                        {
                            availableUnitMeasures.Add(DB.RSField(reader, "UnitMeasureCode"));
                        }
                    }
                }

                if (availableUnitMeasures.Count > 1)
                {
                    // render as drop down list
                    lblUnitMeasureCode.Visible = false;

                    foreach (string unitMeasureCode in availableUnitMeasures)
                    {
                        cboUnitMeasureCode.Items.Add(new ListItem(HttpUtility.HtmlEncode(unitMeasureCode), HttpUtility.HtmlEncode(unitMeasureCode)));
                    }
                }
                else
                {
                    // The only unit measure the item is configured for is the default
                    // which we are guaranteed to be in the first index..
                    cboUnitMeasureCode.Visible = false;
                    lblUnitMeasureCode.Text = HttpUtility.HtmlEncode(availableUnitMeasures[0]);
                }

                bool withVat = AppLogic.AppConfigBool("VAT.Enabled") && ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive;
                var um = UnitMeasureInfo.ForItem(itemCode, UnitMeasureInfo.ITEM_DEFAULT );

                decimal promotionalPrice = decimal.Zero;
                decimal price =
                InterpriseHelper.GetSalesPriceAndTax(ThisCustomer.CustomerCode, 
                    itemCode,
                    ThisCustomer.CurrencyCode,
                    decimal.One,
                    um.Code, withVat, 
                    ref promotionalPrice);

                if (promotionalPrice != decimal.Zero)
                {
                    price = promotionalPrice;
                }

                string vatDisplay = string.Empty;
                if (AppLogic.AppConfigBool("VAT.Enabled"))
                {
                    vatDisplay = CommonLogic.IIF(ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive,
                                        " <span class=\"VATLabel\">" + AppLogic.GetString("showproduct.aspx.38", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "</span>\n",
                                        " <span class=\"VATLabel\">" + AppLogic.GetString("showproduct.aspx.37", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) + "</span>\n");
                }

                Label lblPrice = e.Item.FindControl("OrderOptionPrice") as Label;
                lblPrice.Text = InterpriseHelper.FormatCurrencyForCustomer(price, ThisCustomer.CurrencyCode) + vatDisplay;

                HiddenField hfCounter = e.Item.FindControl("hfItemCounter") as HiddenField;
                hfCounter.Value = counter.ToString();

                var cbk = e.Item.FindControl("OrderOptions") as DataCheckBox;
                cbk.Checked = false;

                bool shouldBeAbleToEnterNotes = Convert.ToBoolean(Convert.ToInt16(orderOptionNode["CheckOutOptionAddMessage"].InnerText));

                Label lblNotes = e.Item.FindControl("lblNotes") as Label;
                TextBox txtNotes = e.Item.FindControl("txtOrderOptionNotes") as TextBox;
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
            ProcessCart(true);
        }
        void btnCheckOutNowBottom_Click(object sender, EventArgs e)
        {
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

        public void InitializePageContent()
        {
            int AgeCartDays = AppLogic.AppConfigUSInt("AgeCartDays");
            if (AgeCartDays == 0)
            {
                AgeCartDays = 7;
            }

            ShoppingCart.Age(ThisCustomer.CustomerID, AgeCartDays, CartTypeEnum.ShoppingCart);
            shoppingcartaspx8.Text = AppLogic.GetString("shoppingcart.aspx.6", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx10.Text = AppLogic.GetString("shoppingcart.aspx.8", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx11.Text = AppLogic.GetString("shoppingcart.aspx.9", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartaspx9.Text = AppLogic.GetString("shoppingcart.aspx.7", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs27.Text = AppLogic.GetString("shoppingcart.cs.5", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs28.Text = AppLogic.GetString("shoppingcart.cs.6", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs29.Text = AppLogic.GetString("shoppingcart.cs.7", SkinID, ThisCustomer.LocaleSetting);
            shoppingcartcs31.Text = AppLogic.GetString("shoppingcart.cs.9", SkinID, ThisCustomer.LocaleSetting);

            string caption = AppLogic.GetString("shoppingcart.cs.33", SkinID, ThisCustomer.LocaleSetting);

            btnUpdateCart1.Text = caption;
            btnUpdateCart2.Text = caption;
            btnUpdateCart3.Text = caption;
            btnUpdateCart4.Text = caption;
            btnUpdateCart5.Text = caption;

            lblOrderNotes.Text = AppLogic.GetString("shoppingcart.cs.13", SkinID, ThisCustomer.LocaleSetting);
            btnContinueShoppingTop.Text = AppLogic.GetString("shoppingcart.cs.12", SkinID, ThisCustomer.LocaleSetting);
            btnContinueShoppingBottom.Text = AppLogic.GetString("shoppingcart.cs.12", SkinID, ThisCustomer.LocaleSetting);
            btnCheckOutNowTop.Text = AppLogic.GetString("shoppingcart.cs.34", SkinID, ThisCustomer.LocaleSetting);
            btnCheckOutNowBottom.Text = AppLogic.GetString("shoppingcart.cs.34", SkinID, ThisCustomer.LocaleSetting);
            OrderNotes.Attributes.Add("onkeyup", "return imposeMaxLength(this, 255);");

            if (cart == null)
            {
                cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);

                string couponCode = string.Empty;
                string couponErrorMessage = string.Empty;
                if (cart.HasCoupon(ref couponCode) &&
                    cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                {
                    CouponCode.Text = couponCode;
                }
                else
                {
                    ErrorMsgLabel.Text = couponErrorMessage;
                    cart.ClearCoupon();
                }

                if (!Page.IsPostBack)
                {
                    if (ThisCustomer.IsCreditOnHold && cart != null)
                    {
                        ErrorMsgLabel.Text = AppLogic.GetString("shoppingcart.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                        cart.ClearCoupon();
                    }
                }
                else
                {
                    if (CouponCode.Text.IsNullOrEmptyTrimmed()) { cart.ClearCoupon(); }
                }
            }

            if (cart.IsEmpty())
            {
                btnUpdateCart1.Visible = false;
                AlternativeCheckouts.Visible = false;
            }

             if (cart != null && !cart.IsEmpty())
            {
                cart.BuildSalesOrderDetails(false, true, CouponCode.Text);
            }

            string BACKURL = AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting);

            StringBuilder html = new StringBuilder("");
            html.Append("<script type=\"text/javascript\" Language=\"JavaScript\">\n");
            html.Append("function Cart_Validator(theForm)\n");
            html.Append("{\n");
            string cartJS = CommonLogic.ReadFile("jscripts/shoppingcart.js", true);
            foreach (var c in cart.CartItems)
            {
                string itemJS = string.Empty;                
                if (c.ItemType.ToUpper() == "NON-STOCK" || c.ItemType.ToUpper() == "SERVICE" || c.ItemType.ToUpper() == "ELECTRONIC DOWNLOAD" || AppLogic.IsAllowFractional)
                {
                    itemJS = cartJS.Replace("%MAX_QUANTITY_INPUT%", AppLogic.MAX_QUANTITY_INPUT_Dec).Replace("%ALLOWED_QUANTITY_INPUT%", AppLogic.AllowedQuantityWithDecimalRegEx(ThisCustomer.LocaleSetting));
                }                
                else
                {
                    itemJS = cartJS.Replace("%MAX_QUANTITY_INPUT%", AppLogic.MAX_QUANTITY_INPUT_NoDec).Replace("%ALLOWED_QUANTITY_INPUT%", AppLogic.AllowedQuantityWithNoDecimalRegEx(ThisCustomer.LocaleSetting));
                }
                itemJS = itemJS.Replace("%DECIMAL_SEPARATOR%", Localization.GetNumberDecimalSeparatorLocaleString(ThisCustomer.LocaleSetting)).Replace("%LOCALE_ZERO%", Localization.GetNumberZeroLocaleString(ThisCustomer.LocaleSetting));
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

            JSPopupRoutines.Text = AppLogic.GetJSPopupRoutines();
            
            string XmlPackageName = AppLogic.AppConfig("XmlPackage.ShoppingCartPageHeader");
            if (XmlPackageName.Length != 0)
            {
                XmlPackage_ShoppingCartPageHeader.Text = AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }

            string XRI = AppLogic.LocateImageURL(SkinImagePath + "redarrow.gif");
            redarrow1.ImageUrl = XRI;
            redarrow2.ImageUrl = XRI;
            redarrow3.ImageUrl = XRI;
            redarrow4.ImageUrl = XRI;

            ShippingInformation.Visible = (!AppLogic.AppConfigBool("SkipShippingOnCheckout"));
            AddresBookLlink.Visible = (ThisCustomer.IsRegistered);

            btnCheckOutNowTop.Visible = (!cart.IsEmpty());

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

            if (cart.InventoryTrimmed)
            {
                pnlInventoryTrimmedError.Visible = true;
                InventoryTrimmedError.Text = AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting);
            }

            if (cart.MinimumQuantitiesUpdated)
            {
                pnlMinimumQuantitiesUpdatedError.Visible = true;
                MinimumQuantitiesUpdatedError.Text = AppLogic.GetString("shoppingcart.aspx.5", SkinID, ThisCustomer.LocaleSetting);
            }

            decimal MinOrderAmount = AppLogic.AppConfigUSDecimal("CartMinOrderAmount");
            if (!cart.MeetsMinimumOrderAmount(MinOrderAmount))
            {
                pnlMeetsMinimumOrderAmountError.Visible = true;
                string amountFormatted = InterpriseHelper.FormatCurrencyForCustomer(MinOrderAmount, ThisCustomer.CurrencyCode);
                MeetsMinimumOrderAmountError.Text = string.Format(AppLogic.GetString("shoppingcart.aspx.2", SkinID, ThisCustomer.LocaleSetting), amountFormatted);
            }

            int quantityDecimalPlaces = InterpriseHelper.GetInventoryDecimalPlacesPreference();

            var formatter = (new CultureInfo(ThisCustomer.LocaleSetting)).NumberFormat;

            // setup the formatter
            formatter.NumberDecimalDigits = quantityDecimalPlaces;
            formatter.PercentDecimalDigits = quantityDecimalPlaces;

            decimal MinQuantity = AppLogic.AppConfigUSDecimal("MinCartItemsBeforeCheckout");
            if (!cart.MeetsMinimumOrderQuantity(MinQuantity))
            {
                pnlMeetsMinimumOrderQuantityError.Visible = true;
                MeetsMinimumOrderQuantityError.Text = string.Format(AppLogic.GetString("shoppingcart.aspx.16", SkinID, ThisCustomer.LocaleSetting), MinQuantity.ToString(), MinQuantity.ToString());
            }
            
            ShoppingCartGif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCart.gif");

            CartItems.Text = cart.RenderHTMLLiteral(new ShoppingCartPageLiteralRenderer());
            CartSubTotal.Text = cart.RenderHTMLLiteral(new ShoppingCartPageSummaryLiteralRenderer());

            if (!cart.IsEmpty())
            {
                ShoppingCartorderoptions_gif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCartorderoptions.gif");
                string strXml = string.Empty;
                pnlErrorMsg.Visible = true;

                if (AppLogic.AppConfigBool("RequireOver13Checked") && ThisCustomer.IsRegistered && !ThisCustomer.IsOver13)
                {
                    btnCheckOutNowTop.Enabled = false;
                    btnCheckOutNowBottom.Enabled = false;
                    ErrorMsgLabel.Text = AppLogic.GetString("over13oncheckout", SkinID, ThisCustomer.LocaleSetting);
                    return;
                }

                btnCheckOutNowBottom.Enabled = btnCheckOutNowTop.Enabled;

                string orderOptionItemsQuery = "SELECT i.Counter, i.ItemCode, i.ItemName, iid.ItemDescription, io.CheckOutOptionAddMessage FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryItemWebOption io with (NOLOCK) ON i.ItemCode = io.ItemCode INNER JOIN InventoryItemDescription iid with (NOLOCK) ON iid.ItemCode = i.ItemCode ";
                if (ThisCustomer.ProductFilterID != string.Empty)
                {
                    orderOptionItemsQuery += string.Format("INNER JOIN (SELECT DISTINCT(ItemCode),TemplateID FROM InventoryProductFilterTemplateItem) pfi ON pfi.ItemCode = i.ItemCode AND pfi.TemplateID = {0}", DB.SQuote(ThisCustomer.ProductFilterID));
                }
                orderOptionItemsQuery += string.Format("LEFT OUTER JOIN EcommerceShoppingCart wsc with (NOLOCK) ON wsc.CustomerCode = {0} AND wsc.ItemCode = i.ItemCode WHERE io.CheckOutOption = 1 AND wsc.ItemCode IS NULL AND i.ItemType IN ({1}, {2}, {3}, {4}) AND io.WebsiteCode = {5} AND iid.LanguageCode = {6} AND {7} between isnull(io.StartDate, '1/1/1900') AND isnull(io.EndDate, '12/31/9999')",
                DB.SQuote(ThisCustomer.CustomerCode),
                DB.SQuote(Interprise.Framework.Base.Shared.Const.ITEM_TYPE_STOCK),
                DB.SQuote(Interprise.Framework.Base.Shared.Const.ITEM_TYPE_NON_STOCK),
                DB.SQuote(Interprise.Framework.Base.Shared.Const.ITEM_TYPE_SERVICE),
                DB.SQuote(Interprise.Framework.Base.Shared.Const.ITEM_TYPE_ASSEMBLY),
                DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode),
                DB.SQuote(ThisCustomer.LanguageCode),
                DB.SQuote(Localization.DateTimeStringForDB(DateTime.Now)));
                
                int optionsCount =
                DB.GetXml(orderOptionItemsQuery,
                    "options",
                    "orderoption",
                    ref strXml
                );

                if (optionsCount > 0)
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(strXml);
                    XmlDocument XslDoc = new XmlDocument();
                    XslDoc.LoadXml("<?xml version=\"1.0\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:param name=\"locale\" /><xsl:template match=\"/\"><xsl:for-each select=\"*\"><xsl:copy><xsl:for-each select=\"*\"><xsl:copy><xsl:for-each select=\"*\"><xsl:copy><xsl:choose><xsl:when test=\"ml\"><xsl:value-of select=\"ml/locale[@name=$locale]\"/></xsl:when><xsl:otherwise><xsl:value-of select=\".\"/></xsl:otherwise></xsl:choose></xsl:copy></xsl:for-each></xsl:copy></xsl:for-each></xsl:copy></xsl:for-each></xsl:template></xsl:stylesheet>");
                    XslCompiledTransform xsl = new XslCompiledTransform();
                    xsl.Load(XslDoc);
                    TextWriter tw = new StringWriter();
                    XsltArgumentList XslArgs = new XsltArgumentList();
                    XslArgs.AddParam("locale", "", ThisCustomer.LocaleSetting);
                    xsl.Transform(xdoc, XslArgs, tw);
                    XmlDocument xresults = new XmlDocument();
                    xresults.LoadXml(tw.ToString());
                    XmlNodeList nodelist = xresults.SelectNodes("//orderoption");


                    OrderOptionsList.DataSource = nodelist;
                    OrderOptionsList.DataBind();
                    pnlOrderOptions.Visible = true;
                }
                else
                {
                    pnlOrderOptions.Visible = false;
                }


                string upsellproductlist = GetUpsellProducts(cart);
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

                if (cart.CouponsAllowed)
                {
                    ShoppingCartCoupon_gif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCartCoupon.gif");
                    pnlCoupon.Visible = true;
                }
                else
                {
                    pnlCoupon.Visible = false;
                }

                ShoppingCartNotes_gif.ImageUrl = AppLogic.LocateImageURL(SkinImagePath + "ShoppingCartNotes.gif");
                if (!AppLogic.AppConfigBool("DisallowOrderNotes"))
                {
                    OrderNotes.Text = cart.OrderNotes;
                    pnlOrderNotes.Visible = true;
                }
                else
                {
                    pnlOrderNotes.Visible = false;
                }

                btnCheckOutNowBottom.Visible = true;

                if (ThisCustomer.IsNotRegistered)
                {
                    //Remove this line of code to enable the "Coupon Support for Anonymous Customers" feature
                    //---------------------------------------------------------------------------------------
                    pnlCoupon.Visible = false;
                    //---------------------------------------------------------------------------------------
                    pnlOrderNotes.Visible = false;
                }

                //Check if alternate checkout methods are supported (PayPal and GoogleCheckout)
                if (AppLogic.IsSupportedAlternateCheckout)
                {
                    //Set the image url for the google button.

                    if (AppLogic.AppConfigBool("GoogleCheckout.UseSandbox"))
                    {
                        btnGoogleCheckout.ImageUrl = string.Format(AppLogic.AppConfig("GoogleCheckout.SandBoxCheckoutButton"),
                            AppLogic.AppConfig("GoogleCheckout.SandboxMerchantId"));
                    }
                    else
                    {
                        btnGoogleCheckout.ImageUrl = string.Format(AppLogic.AppConfig("GoogleCheckout.LiveCheckoutButton"),
                            AppLogic.AppConfig("GoogleCheckout.MerchantId"));
                    }

                    if (AppLogic.AppConfigBool("GoogleCheckout.ShowOnCartPage"))
                    {
                        AlternativeCheckouts.Visible = InterpriseShoppingCart.IsWebCheckOutIncluded("Google");
                        GoogleCheckoutSpan.Visible = InterpriseShoppingCart.IsWebCheckOutIncluded("Google");
                    }

                    if (cart != null && cart.IsShipSeparatelyCount() > 0)
                    {
                        GoogleCheckoutSpan.Visible = false;
                    }

                    if (AppLogic.AppConfigBool("PayPalCheckout.ShowOnCartPage") && (ThisCustomer.IsRegistered || !AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                        && (cart != null && !cart.IsEmpty() && cart.SalesOrderDataset.CustomerSalesOrderView[0].SubTotalRate > 0))
                    {
                        AlternativeCheckouts.Visible = true;
                        PayPalExpressSpan.Visible = true;
                    }
                    else
                    {
                        AlternativeCheckouts.Visible = false;
                        PayPalExpressSpan.Visible = false;
                    }
                }

                //if no alternative methods are visible, hide the whole row
                if (!AppLogic.IsSupportedAlternateCheckout && AlternativeCheckouts.Visible == true)
                {
                    ErrorMsgLabel.Text = PayPalExpress.ErrorMsg;
                    AlternativeCheckouts.Visible = false;
                }
            }
            else
            {
                pnlOrderOptions.Visible = false;
                pnlUpsellProducts.Visible = false;
                pnlCoupon.Visible = false;
                pnlOrderNotes.Visible = false;
                btnCheckOutNowBottom.Visible = false;
                AlternativeCheckouts.Visible = false;
                PayPalExpressSpan.Visible = false;
                GoogleCheckoutSpan.Visible = false;
            }
            btnContinueShoppingBottom.OnClientClick = "self.location='" + BACKURL + "'";
            CartPageFooterTopic.SetContext = this;
            string XmlPackageName2 = AppLogic.AppConfig("XmlPackage.ShoppingCartPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                XmlPackage_ShoppingCartPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }            
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
                    int upsellProductLimit = AppLogic.AppConfigUSInt("AccesoryProductsLimitNumberOnCart");
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
                    results.Append("<img src=\"" + AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/ShoppingCartUpsell.gif") + "\" border=\"0\"><br/>");
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
        public void ProcessCart(bool DoingFullCheckout)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            ThisCustomer.RequireCustomerRecord();
            CartTypeEnum cte = CartTypeEnum.ShoppingCart;
            if (CommonLogic.QueryStringCanBeDangerousContent("CartType").Length != 0)
            {
                cte = (CartTypeEnum)CommonLogic.QueryStringUSInt("CartType");
            }
            cart = new InterpriseShoppingCart(null, 1, ThisCustomer, cte, string.Empty, false, true);

            if (!Page.IsPostBack)
            {
                string couponCode = string.Empty;
                if (cart.HasCoupon(ref couponCode))
                {
                    CouponCode.Text = couponCode;
                }
            }
            else
            {
                if (CouponCode.Text.IsNullOrEmptyTrimmed())
                {
                    cart.ClearCoupon();
                }
            }

            // check if credit on hold
            if (ThisCustomer.IsCreditOnHold) { Response.Redirect("shoppingcart.aspx"); }

            if (cart.IsEmpty())
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
                    if (fldval.StartsWith(Localization.GetNumberDecimalSeparatorLocaleString(cart.ThisCustomer.LocaleSetting)))
                    {
                        fldval = fldval.Insert(0, Localization.GetNumberZeroLocaleString(cart.ThisCustomer.LocaleSetting));
                    }
                    if (Regex.IsMatch(fldval, AppLogic.AllowedQuantityWithDecimalRegEx(cart.ThisCustomer.LocaleSetting), RegexOptions.Compiled))
                    {
                        recID = Localization.ParseUSInt(fld.Substring("Quantity".Length + 1));
                        quantity = fldval;
                        decimal iquan = Convert.ToDecimal(quantity);
                        if (iquan < 0)
                        {
                            iquan = 0;
                        }
                        cart.SetItemQuantity(recID, iquan);
                    }
                    else
                    {
                        ErrorMsgLabel.Text += "The item quantity must have a valid input.";
                    }
                }
                if (fld.StartsWith("UnitMeasureCode"))
                {
                    if (!string.IsNullOrEmpty(fldval))
                    {
                        recID = Localization.ParseUSInt(fld.Substring("UnitMeasureCode".Length + 1));
                        string unitMeasureCode = HttpUtility.HtmlDecode(fldval);
                        cart.UpdateUnitMeasureForItem(recID, unitMeasureCode, cart.HasMultipleShippingAddresses());
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
                    if (cart.IsCouponValid(ThisCustomer, CouponCode.Text, ref errorMessage))
                    {
                        cart.ApplyCoupon(CouponCode.Text);
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
                        cart.BuildSalesOrderDetails(false, true, CouponCode.Text);
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
                        cart.AddItem(ThisCustomer, shippingAddressID, itemCode, ProductID, 1, umInfo.Code, CartTypeEnum.ShoppingCart);
                    }
                }

                if (pnlOrderOptions.Visible)
                {
                    // Process the Order Options
                    foreach (RepeaterItem ri in OrderOptionsList.Items)
                    {
                        DataCheckBox cbk = (DataCheckBox)ri.FindControl("OrderOptions");
                        if (cbk.Checked)
                        {
                            string itemCode = (string)cbk.Data;
                            HiddenField hfCounter = ri.FindControl("hfItemCounter") as HiddenField;
                            TextBox txtNotes = ri.FindControl("txtOrderOptionNotes") as TextBox;
                            
                            string strNotes = HttpUtility.HtmlEncode(txtNotes.Text);
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
                            Label lblUnitMeasureCode = ri.FindControl("lblUnitMeasureCode") as Label;
                            if (null != lblUnitMeasureCode && lblUnitMeasureCode.Visible)
                            {
                                unitMeasureCode = lblUnitMeasureCode.Text;
                            }
                            else
                            {
                                // it's rendered as combobox because the item has multiple unit measures configured
                                DropDownList cboUnitMeasureCode = ri.FindControl("cboUnitMeasureCode") as DropDownList;
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
                            cart.AddItem(ThisCustomer,ThisCustomer.PrimaryShippingAddressID,itemCode,int.Parse(hfCounter.Value),itemQuantity,unitMeasureCode,CartTypeEnum.ShoppingCart);
                        }
                    }
                }

                if (ThisCustomer.IsRegistered)
                {
                    string sOrderNotes = CommonLogic.CleanLevelOne(OrderNotes.Text);
                    //check the length of order notes
                    //should not exceed 255 characters including spaces
                    int maxLen = 255;
                    if (sOrderNotes.Length > maxLen)
                    {
                        sOrderNotes = sOrderNotes.Substring(0, maxLen);
                    }

                    DB.ExecuteSQL(
                        string.Format("UPDATE Customer SET Notes = {0} WHERE CustomerCode = {1}",
                        DB.SQuote(sOrderNotes),
                        DB.SQuote(ThisCustomer.CustomerCode))
                    );
                }

            }
            bool validated = true;
            if (cart.InventoryTrimmed)
            {
                // inventory got adjusted, send them back to the cart page to confirm the new values!
                ErrorMsgLabel.Text += Server.UrlDecode(AppLogic.GetString("shoppingcart.cs.43", SkinID, ThisCustomer.LocaleSetting));
                validated = false;
            }

            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
            cart.BuildSalesOrderDetails(false, true, CouponCode.Text);

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
                
                if (!cart.MeetsMinimumOrderAmount(AppLogic.AppConfigUSDecimal("CartMinOrderAmount")))
                {
                    validated = false;
                }

                if (!cart.MeetsMinimumOrderQuantity(AppLogic.AppConfigUSInt("MinCartItemsBeforeCheckout")))
                {
                    validated = false;
                }

                string couponCode = string.Empty;
                string couponErrorMessage = string.Empty;
                if (cart.HasCoupon(ref couponCode) && !cart.IsCouponValid(ThisCustomer, couponCode, ref couponErrorMessage))
                {
                    validated = false;
                }

                if (ThisCustomer.IsRegistered && AppLogic.AppConfigBool("Checkout.UseOnePageCheckout") && 
                    !cart.HasMultipleShippingAddresses())
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
                        if (AppLogic.AppConfigBool("SkipShippingOnCheckout") || 
                            !cart.HasShippableComponents())
                        {
                            cart.MakeShippingNotRequired();
                            Response.Redirect("checkoutpayment.aspx");
                        }

                        if ((cart.HasMultipleShippingAddresses() && cart.NumItems() <= AppLogic.MultiShipMaxNumItemsAllowed() && cart.CartAllowsShippingMethodSelection))
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

        private bool DeleteButtonExists(string s)
        {
            return s == "bt_Delete";
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

                if (!cart.IsSalesOrderDetailBuilt)
                {
                    cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
                    cart.BuildSalesOrderDetails(CouponCode.Text);
                }

                Response.Redirect(GoogleCheckout.CreateGoogleCheckout(cart));
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
                // Get IS Cart ready
                ProcessCart(false);

                if (!cart.IsSalesOrderDetailBuilt)
                {
                    cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
                    cart.BuildSalesOrderDetails();
                }

                ThisCustomer.ThisCustomerSession["flag"] = "Shoppingcart";
                Response.Redirect(PayPalExpress.CheckoutURL(cart));
            }
        }
    }
}



