// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using System.Text.RegularExpressions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

namespace InterpriseSuiteEcommerce
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

            RedirectToSignInPageLiteral.Text = AppLogic.GetString("shoppingcart.cs.1011", SkinID, ThisCustomer.LocaleSetting, true);

            // perform redirect
            Response.AddHeader("REFRESH", string.Format("1; URL={0}", Server.UrlDecode("signin.aspx")));
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.RequireCustomerRecord();

            SectionTitle = AppLogic.GetString("wishlist.aspx.1", SkinID, ThisCustomer.LocaleSetting, true);

            if (CommonLogic.QueryStringUSInt("MoveToCartID") != 0)
            {
                int cartId = CommonLogic.QueryStringUSInt("MoveToCartID");
                decimal quantity = CommonLogic.QueryStringUSDecimal("MoveToCartQty");

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
                    cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);

                    if (itemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_KIT)
                    {
                        cart.AddItem(ThisCustomer,
                            shippingAddressID,
                            itemCode,
                            counter,
                            quantity,
                            unitMeasureCode,
                            CartTypeEnum.ShoppingCart,
                            kitCartWishListComposition, null, CartTypeEnum.WishCart);
                    }
                    else
                    {
                        cart.AddItem(ThisCustomer,
                            shippingAddressID,
                            itemCode,
                            counter,
                            quantity,
                            unitMeasureCode,
                            CartTypeEnum.ShoppingCart);
                    }

                    ServiceFactory.GetInstance<IShoppingCartService>()
                                  .ClearLineItemsAndKitComposition(new String[] { cartGuid.ToString() });
                }
                Response.Redirect("ShoppingCart.aspx");
            }

            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, string.Empty, false, true);

            string[] formkeys = Request.Form.AllKeys;
            foreach (string s in formkeys)
            {
                if (s == "bt_Delete")
                {
                    UpdateWishList();
                    InitializePageContent();
                }
            }

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

        public void btnUpateWishList1_Click(object sender, EventArgs e)
        {
            UpdateWishList();
            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, string.Empty, false, true);
            InitializePageContent();
        }

        public void btnUpateWishList2_Click(object sender, EventArgs e)
        {
            UpdateWishList();
            cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, string.Empty, false, true);
            InitializePageContent();
        }

        protected void btnContinueShopping1_Click(object sender, EventArgs e)
        {
            if (ViewState["returnurl"].ToString() == "")
                Response.Redirect(AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting));
            else
                Response.Redirect(ViewState["returnurl"].ToString());
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
                cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.WishCart, string.Empty, false, true);
            }

            string XmlPackageName = AppLogic.AppConfig("XmlPackage.WishListPageHeader");
            if (XmlPackageName.Length != 0)
            {
                throw new NotImplementedException("Not yet ported");
            }

            string CartTopControlLinesXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageTopControlLines");
            if (CartTopControlLinesXmlPackage.Length != 0)
            {
                XmlPackage_WishListPageTopControlLines.Text = AppLogic.RunXmlPackage(CartTopControlLinesXmlPackage, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
                XmlPackage_WishListPageTopControlLines.Visible = true;
            }
            else
            {
                pnlTopControlLines.Visible = true;
                btnContinueShopping1.Text = AppLogic.GetString("shoppingcart.cs.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
                btnContinueShopping1.Attributes.Add("onclick", "self.location='" + BACKURL + "'");
                if (!cart.IsEmpty())
                {
                    btnUpateWishList1.Text = AppLogic.GetString("shoppingcart.cs.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
                }
                else
                {
                    btnUpateWishList1.Visible = false;
                }
            }

           
            tblWishList.Attributes.Add("style", "border-style: solid; border-width: 0px; border-color: #" + AppLogic.AppConfig("HeaderBGColor"));
            tblWishListBox.Attributes.Add("style", AppLogic.AppConfig("BoxFrameStyle"));
            wishlist_gif.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/wishlist.gif");

            string CartItemsXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageItems");
            if (CartItemsXmlPackage.Length != 0)
            {
                CartItems.Text = AppLogic.RunXmlPackage(CartItemsXmlPackage, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }
            else
            {
                CartItems.Text = cart.RenderHTMLLiteral(new WishListPageLiteralRenderer());
            }

            string CartBottomControlLinesXmlPackage = AppLogic.AppConfig("XmlPackage.WishListPageBottomControlLines");
            if (CartBottomControlLinesXmlPackage.Length != 0)
            {
                Xml_WishListPageBottomControlLines.Text = AppLogic.RunXmlPackage(CartBottomControlLinesXmlPackage, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
                Xml_WishListPageBottomControlLines.Visible = true;
            }
            else
            {
                pnlBottomControlLines.Visible = true;
                btnContinueShopping2.Text = AppLogic.GetString("shoppingcart.cs.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
                btnContinueShopping2.Attributes.Add("onclick", "self.location='" + BACKURL + "'");
                if (!cart.IsEmpty())
                {
                    btnUpateWishList2.Text = AppLogic.GetString("shoppingcart.cs.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);
                }
                else
                {
                    btnUpateWishList2.Visible = false;
                }
            }

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnContinueShopping1, "shoppingcart.cs.12");
                AppLogic.EnableButtonCaptionEditing(btnContinueShopping2, "shoppingcart.cs.12");
                AppLogic.EnableButtonCaptionEditing(btnUpateWishList1, "shoppingcart.cs.32");
                AppLogic.EnableButtonCaptionEditing(btnUpateWishList2, "shoppingcart.cs.32");

            }

            string XmlPackageName2 = AppLogic.AppConfig("XmlPackage.WishListPageFooter");
            if (XmlPackageName2.Length != 0)
            {
                Xml_WishListPageFooter.Text = AppLogic.RunXmlPackage(XmlPackageName2, base.GetParser, ThisCustomer, SkinID, string.Empty, null, true, true);
            }


            GetJSFunctions();
        }

        private void UpdateWishList()
        {
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
                        decimal iquan = Convert.ToDecimal(quantity);//Localization.ParseUSDecimal(quantity);
                        if (iquan < 0)
                        {
                            iquan = 0;
                        }
                        cart.SetItemQuantity(recID, iquan);
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

        protected void btnContinueShopping2_Click(object sender, EventArgs e)
        {
            if (ViewState["returnurl"].ToString() == "")
                Response.Redirect(AppLogic.GetCartContinueShoppingURL(SkinID, ThisCustomer.LocaleSetting));
            else
                Response.Redirect(ViewState["returnurl"].ToString());
        }
    }
}






