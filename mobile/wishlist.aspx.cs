// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce.mobile
{
    /// <summary>
    /// Summary description for wishlist.
    /// </summary>
    public partial class wishlist : SkinBase
    {
        InterpriseShoppingCart cart;
        string BACKURL = string.Empty;

        private void RedirectToSignInPage()
        {
            pnlTopControlLines.Visible = false;
            Panel1.Visible = false;
            Xml_WishListPageBottomControlLines.Visible = false;
            pnlBottomControlLines.Visible = false;
            Xml_WishListPageFooter.Visible = false;
            RedirectToSignInPageLiteral.Text = AppLogic.GetString("shoppingcart.cs.1011", SkinID, ThisCustomer.LocaleSetting);
            Response.AddHeader("REFRESH", string.Format("1; URL={0}", Server.UrlDecode("signin.aspx")));
        }

        protected void Page_Init(object sender, System.EventArgs e)
        {
            btnContinueShopping1.Click += btnContinueShopping1_Click;
            btnContinueShopping2.Click += btnContinueShopping1_Click;

            //btnContinueShopping2.Text = AppLogic.GetString("shoppingcart.cs.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            //btnContinueShopping1.Text = AppLogic.GetString("shoppingcart.cs.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            btnUpateWishList1.Click += btnUpateWishList1_Click;
            btnUpateWishList2.Click += btnUpateWishList1_Click;
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.RequireCustomerRecord();

            SectionTitle = AppLogic.GetString("wishlist.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            int? moveToCartId = "MoveToCartID".ToQueryString().TryParseIntUsLocalization();
            if (moveToCartId.HasValue)
            {
                int cartId = moveToCartId.Value;
                decimal? quantity = "MoveToCartQty".ToQueryString().TryParseDecimalUsLocalization();

                bool cartItemExisting = false;
                string itemCode = string.Empty;
                string itemType = string.Empty;
                string unitMeasureCode = string.Empty;
                string shippingAddressID = string.Empty;
                Guid cartGuid = Guid.Empty;
                int counter = 0;
                // NOTE : 
                // Move this logic on the Shopping Cart Form

                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var reader = DB.GetRSFormat(con, "SELECT wsc.ShoppingCartRecGuid, i.Counter, i.ItemCode, i.ItemType, wsc.UnitMeasureCode, wsc.ShippingAddressID FROM EcommerceShoppingCart wsc with (NOLOCK) INNER JOIN InventoryItem i with (NOLOCK) ON i.ItemCode = wsc.ItemCode WHERE wsc.ShoppingCartRecID = {0}", cartId))
                    {
                        cartItemExisting = reader.Read();
                        if (cartItemExisting)
                        {
                            cartGuid = DB.RSFieldGUID2(reader, "ShoppingCartRecGuid");
                            counter = DB.RSFieldInt(reader, "Counter");
                            itemCode = DB.RSField(reader, "ItemCode");
                            itemType = DB.RSField(reader, "ItemType");
                            unitMeasureCode = DB.RSField(reader, "UnitMeasureCode");
                            shippingAddressID = DB.RSField(reader, "ShippingAddressID");
                        }
                    }
                }

                if (cartItemExisting)
                {
                    var kitCartWishListComposition = KitComposition.FromCart(ThisCustomer, CartTypeEnum.WishCart, itemCode, cartGuid);
                    cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

                    if (itemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_KIT)
                    {
                        cart.AddItem(ThisCustomer,
                            shippingAddressID,
                            itemCode,
                            counter,
                            quantity.Value,
                            unitMeasureCode,
                            CartTypeEnum.ShoppingCart,
                            kitCartWishListComposition);
                    }
                    else
                    {
                        cart.AddItem(ThisCustomer,
                            shippingAddressID,
                            itemCode,
                            counter,
                            quantity.Value,
                            unitMeasureCode,
                            CartTypeEnum.ShoppingCart);
                    }

                    ServiceFactory.GetInstance<IShoppingCartService>()
                                  .ClearLineItemsAndKitComposition(new String[] { cartGuid.ToString() });
                }
                Response.Redirect("ShoppingCart.aspx");
            }

            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, String.Empty, false, true);

            ProcessDelete();

            if (!IsPostBack)
            {
                string returnurl = CommonLogic.QueryStringCanBeDangerousContent("ReturnUrl");

                if (returnurl.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    throw new ArgumentException("SECURITY EXCEPTION");
                }

                ViewState["returnurl"] = returnurl;
                InitializePageContent();
            }
            TopicWishListPageHeader.SetContext = this;
            TopicWishListPageFooter.SetContext = this;
        }

        private void ProcessDelete()
        {
            string[] formkeys = Request.Form.AllKeys;
            var btnKeyDelete = formkeys.FirstOrDefault(k => k.Contains("bt_Delete"));
            if (!string.IsNullOrEmpty(btnKeyDelete))
            {
                int recId = btnKeyDelete.Substring("Quantity".Length + 2).TryParseIntUsLocalization().Value;
                cart.SetItemQuantity(recId, 0);
                InitializePageContent();
            }
        }

        private void InitializePageContent()
        {
            int AgeWishListDays = AppLogic.AppConfigUSInt("AgeWishListDays");
            if (AgeWishListDays == 0)
            {
                AgeWishListDays = 7;
            }

            ShoppingCart.Age(ThisCustomer.CustomerID, AgeWishListDays, CartTypeEnum.WishCart);

            if (cart == null)
            {
                cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, String.Empty, false, true);
            }

            string XmlPackageName = AppLogic.AppConfig("XmlPackage.WishListPageHeader");
            if (XmlPackageName.Length != 0)
            {
                throw new NotImplementedException("Not yet ported");
            }

            string CartTopControlLinesXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageTopControlLines");
            if (CartTopControlLinesXmlPackage.Length != 0)
            {
                XmlPackage_WishListPageTopControlLines.Text = AppLogic.RunXmlPackage(CartTopControlLinesXmlPackage, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
                XmlPackage_WishListPageTopControlLines.Visible = true;
            }
            else
            {
                pnlTopControlLines.Visible = true;
                btnContinueShopping1.Text = AppLogic.GetString("shoppingcart.cs.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                btnContinueShopping1.Attributes.Add("onclick", "self.location='" + BACKURL + "';");
                if (!cart.IsEmpty())
                {
                    btnUpateWishList1.Text = AppLogic.GetString("shoppingcart.cs.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                }
                else
                {
                    btnUpateWishList1.Visible = false;
                }
            }

            //tblWishList.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            //tblWishListBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
            //wishlist_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/wishlist.gif");

            string CartItemsXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageItems");
            if (CartItemsXmlPackage.Length != 0)
            {
                CartItems.Text = AppLogic.RunXmlPackage(CartItemsXmlPackage, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
            }
            else
            {
                CartItems.Text = cart.RenderHTMLLiteral(new MobileWishListPageLiteralRenderer());
            }

            string CartBottomControlLinesXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageBottomControlLines");
            if (CartBottomControlLinesXmlPackage.Length != 0)
            {
                Xml_WishListPageBottomControlLines.Text = AppLogic.RunXmlPackage(CartBottomControlLinesXmlPackage, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
                Xml_WishListPageBottomControlLines.Visible = true;
            }
            else
            {
                pnlBottomControlLines.Visible = true;
                btnContinueShopping2.Text = AppLogic.GetString("shoppingcart.cs.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                btnContinueShopping2.Attributes.Add("onclick", "self.location='" + BACKURL + "'");
                if (!cart.IsEmpty())
                {
                    btnUpateWishList2.Text = AppLogic.GetString("shoppingcart.cs.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                }
                else
                {
                    btnUpateWishList2.Visible = false;
                }
            }

            string XmlPackageName2 = AppLogic.AppConfig("XmlPackage.WishListPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                Xml_WishListPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, String.Empty, null, true, true);
            }

            GetJSFunctions();
        }

        private void UpdateWishList()
        {
            var keys = Request.Form.AllKeys;

            //Annonimous objects
            var quantityElementAndIndex = keys
                        .Where(k => k.Contains("Quantity"))
                        .Select((n,i) => new { Value = Request.Form[n], Id = n.Substring(n.IndexOf('_') + 1) });

            foreach (var item in quantityElementAndIndex)
            {
                int itemId = item.Id.TryParseIntUsLocalization().Value;
                decimal? value = item.Value.TryParseDecimalUsLocalization();
                if (!value.HasValue || value.Value < 0) value = 0;
                cart.SetItemQuantity(itemId, value.Value);
            }

            var unitElementAndIndex = keys
                        .Where(k => k.Contains("UnitMeasureCode"))
                        .Select((n, i) => new { Value = Request.Form[n], Id = n.Substring(n.IndexOf('_') + 1) });

            foreach (var item in unitElementAndIndex)
            {
                int itemId = item.Id.TryParseIntUsLocalization().Value;
                string value = item.Value;
                if (string.IsNullOrEmpty(value)) continue;
                cart.UpdateUnitMeasureForItem(itemId, value.ToHtmlDecode(), cart.HasMultipleShippingAddresses());
            }
        }

        private void GetJSFunctions()
        {
            var s = new StringBuilder("<script type=\"text/javascript\" Language=\"JavaScript\">");
            s.Append("function " + "FormValidator(theForm){\n");
            string cartJS = CommonLogic.ReadFile("jscripts/shoppingcart.js", true);
            decimal TotalQ = 0;
            foreach (var c in cart.CartItems)
            {
                string itemJS = string.Empty;

                itemJS = cartJS.Replace("%MAX_QUANTITY_INPUT%", AppLogic.MAX_QUANTITY_INPUT_NoDec).Replace("%ALLOWED_QUANTITY_INPUT%", AppLogic.GetQuantityRegularExpression(c.ItemType, true));
                itemJS = itemJS.Replace("%DECIMAL_SEPARATOR%", Localization.GetNumberDecimalSeparatorLocaleString(ThisCustomer.LocaleSetting)).Replace("%LOCALE_ZERO%", Localization.GetNumberZeroLocaleString(ThisCustomer.LocaleSetting));
                s.Append(itemJS.Replace("%SKU%", c.m_ShoppingCartRecordID.ToString()));
                TotalQ += c.m_Quantity;
            }

            s.Append("return(true);\n");
            s.Append("}\n");
            s.Append("</script>\n");
            ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), s.ToString());
        }

        public void btnUpateWishList1_Click(object sender, EventArgs e)
        {
            UpdateWishList();
            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, String.Empty, false, true);
            InitializePageContent();
        }

        protected void btnContinueShopping1_Click(object sender, EventArgs e)
        {
            if (ViewState["returnurl"].ToString() == "")
                Response.Redirect(AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting));
            else
                Response.Redirect(ViewState["returnurl"].ToString());
        }

    }
}