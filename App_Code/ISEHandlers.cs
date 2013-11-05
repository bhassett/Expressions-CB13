// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Xml;
using System.Web;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using com.paypal.soap.api;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

#region AddtoCart

public class AddtoCart : IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }
    public void ProcessRequest(HttpContext context)
    {
        context.Response.CacheControl = "private";
        context.Response.Expires = 0;
        context.Response.AddHeader("pragma", "no-cache");

        var ThisCustomer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;
        ThisCustomer.RequireCustomerRecord();

        string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
        if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            throw new ArgumentException("SECURITY EXCEPTION");
        }
        
        //Anonymous users should not be allowed to used WishList, they must register first.
        if (ThisCustomer.IsNotRegistered)
        {
            string ErrMsg = string.Empty;

            if (CommonLogic.FormNativeInt("IsWishList") == 1 || CommonLogic.QueryStringUSInt("IsWishList") == 1)
            {
                ErrMsg = AppLogic.GetString("signin.aspx.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                context.Response.Redirect("signin.aspx?ErrorMsg=" + ErrMsg + "&ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
        }

        string ShippingAddressID = CommonLogic.QueryStringCanBeDangerousContent("ShippingAddressID"); // only used for multi-ship
        if (ShippingAddressID.IsNullOrEmptyTrimmed())
        {
            ShippingAddressID = CommonLogic.FormCanBeDangerousContent("ShippingAddressID");
        }

        if (ShippingAddressID.IsNullOrEmptyTrimmed() && !ThisCustomer.PrimaryShippingAddressID.IsNullOrEmptyTrimmed())
        {
            ShippingAddressID = ThisCustomer.PrimaryShippingAddressID;
        }

        string ProductID = CommonLogic.QueryStringCanBeDangerousContent("ProductID");
        if (ProductID.IsNullOrEmptyTrimmed())
        {
            ProductID = CommonLogic.FormCanBeDangerousContent("ProductID");
        }

        string itemCode = CommonLogic.QueryStringCanBeDangerousContent("ItemCode");
        // check if the item being added is matrix group
        // look for the matrix item and use it as itemcode instead
        if (!string.IsNullOrEmpty(CommonLogic.FormCanBeDangerousContent("MatrixItem")))
        {
            itemCode = CommonLogic.FormCanBeDangerousContent("MatrixItem");
        }

        bool itemExisting = false;
        string defaultUnitMeasure = string.Empty;

        if (itemCode.IsNullOrEmptyTrimmed())
        {
            int itemCounter = 0;
            if (!ProductID.IsNullOrEmptyTrimmed() &&
                int.TryParse(ProductID, out itemCounter) &&
                itemCounter > 0)
            {
                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var reader = DB.GetRSFormat(con, "SELECT i.ItemCode, ium.UnitMeasureCode FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryUnitMeasure ium with (NOLOCK) ON i.ItemCode = ium.ItemCode AND IsBase = 1 WHERE i.Counter = {0}", itemCounter))
                    {
                        itemExisting = reader.Read();

                        if (itemExisting)
                        {
                            itemCode = DB.RSField(reader, "ItemCode");
                            defaultUnitMeasure = DB.RSField(reader, "UnitMeasureCode");
                        }
                    }
                }
            }
        }
        else
        {
            // verify we have a valid item code
            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, "SELECT i.ItemCode FROM InventoryItem i with (NOLOCK) WHERE i.ItemCode = {0}", DB.SQuote(itemCode)))
                {
                    itemExisting = reader.Read();

                    if (itemExisting)
                    {
                        itemCode = DB.RSField(reader, "ItemCode");
                    }
                }
            }
        }

        if (!itemExisting)
        {
            GoNextPage(context);
        }

        // get the unit measure code
        string unitMeasureCode = CommonLogic.QueryStringCanBeDangerousContent("UnitMeasureCode");
        if (unitMeasureCode.IsNullOrEmptyTrimmed())
        {
            unitMeasureCode = CommonLogic.FormCanBeDangerousContent("UnitMeasureCode");
        }

        if (unitMeasureCode.IsNullOrEmptyTrimmed())
        {
            unitMeasureCode = defaultUnitMeasure;
        }

        // check if the unit measure is default so that we won't have to check
        // if the unit measure specified is valid...
        if (false.Equals(unitMeasureCode.Equals(defaultUnitMeasure, StringComparison.InvariantCultureIgnoreCase)))
        {
            bool isValidUnitMeasureForThisItem = false;

            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, "SELECT UnitMeasureCode FROM InventoryUnitMeasure with (NOLOCK) WHERE ItemCode= {0} AND UnitMeasureCode = {1}", DB.SQuote(itemCode), DB.SQuote(unitMeasureCode)))
                {
                    isValidUnitMeasureForThisItem = reader.Read();

                    if (isValidUnitMeasureForThisItem)
                    {
                        // maybe mixed case specified, just set..
                        unitMeasureCode = DB.RSField(reader, "UnitMeasureCode");
                    }
                }
            }

            if (!isValidUnitMeasureForThisItem)
            {
                GoNextPage(context);
            }
        }
        decimal Quantity = CommonLogic.FormLocaleDecimal("Quantity",ThisCustomer.LocaleSetting);//CommonLogic.QueryStringUSDecimal("Quantity");
        
        if (Quantity == 0) { Quantity = CommonLogic.FormNativeDecimal("Quantity"); }

        if (Quantity == 0) { Quantity = 1; }

        Quantity = CommonLogic.RoundQuantity(Quantity);

        // Now let's check the shipping address if valid if specified
        if (ShippingAddressID != ThisCustomer.PrimaryShippingAddressID)
        {
            if (ThisCustomer.IsRegistered)
            {
                bool shippingAddressIsValidForThisCustomer = false;

                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var reader = DB.GetRSFormat(con, "SELECT ShipToCode FROM CustomerShipTo with (NOLOCK) WHERE CustomerCode = {0} AND IsActive = 1 AND ShipToCode = {1}", DB.SQuote(ThisCustomer.CustomerCode), DB.SQuote(ShippingAddressID)))
                    {
                        shippingAddressIsValidForThisCustomer = reader.Read();

                        if (shippingAddressIsValidForThisCustomer)
                        {
                            // maybe mixed case, just set...
                            ShippingAddressID = DB.RSField(reader, "ShipToCode");
                        }
                    }
                }

                if (!shippingAddressIsValidForThisCustomer)
                {
                    GoNextPage(context);
                }
            }
            else
            {
                ShippingAddressID = ThisCustomer.PrimaryShippingAddressID;
            }
        }

        var CartType = CartTypeEnum.ShoppingCart;
        if (CommonLogic.FormNativeInt("IsWishList") == 1 || CommonLogic.QueryStringUSInt("IsWishList") == 1)
        {
            CartType = CartTypeEnum.WishCart;
        }

        var giftRegistryItemType = GiftRegistryItemType.vItem;
        if (CommonLogic.FormNativeInt("IsAddToGiftRegistry") == 1 || CommonLogic.QueryStringUSInt("IsAddToGiftRegistry") == 1)
        {
            CartType = CartTypeEnum.GiftRegistryCart;
        }

        if (CommonLogic.FormNativeInt("IsAddToGiftRegistryOption") == 1 || CommonLogic.QueryStringUSInt("IsAddToGiftRegistryOption") == 1)
        {
            CartType = CartTypeEnum.GiftRegistryCart;
            giftRegistryItemType = GiftRegistryItemType.vOption;
        }

        ShoppingCart cart = null;
        bool itemIsARegistryItem = false;
        if (!itemCode.IsNullOrEmptyTrimmed())
        {
            #region " --GIFTREGISTRY-- "

            if (CartType == CartTypeEnum.GiftRegistryCart)
            {
                Guid? registryID = CommonLogic.FormCanBeDangerousContent("giftregistryOptions").TryParseGuid();
                if (registryID.HasValue)
                {
                    var selectedGiftRegistry = ThisCustomer.GiftRegistries.FindFromDb(registryID.Value);
                    if (selectedGiftRegistry != null)
                    {
                        bool isKit = AppLogic.IsAKit(itemCode);
                        KitComposition preferredComposition = null;
                        GiftRegistryItem registryItem = null;

                        if (isKit)
                        {
                            preferredComposition = KitComposition.FromForm(ThisCustomer, itemCode);
                            var registrytems = selectedGiftRegistry.GiftRegistryItems.Where(giftItem => giftItem.ItemCode == itemCode &&
                                                                                     giftItem.GiftRegistryItemType == giftRegistryItemType);
                            Guid? matchedRegitryItemCode = null;
                            //Do this routine to check if there are kit items
                            //matched the selected kit items from the cart in the registry items
                            foreach (var regitm in registrytems)
	                        {
                                regitm.IsKit = true;
                                var compositionItems = regitm.GetKitItemsFromComposition();

                                if (compositionItems.Count() == 0) continue;

                                var arrItemCodes = compositionItems.Select(item => item.ItemCode)
                                                                   .ToArray();
                                var preferredItemCodes = preferredComposition.Compositions.Select(kititem => kititem.ItemCode);
                                var lst = arrItemCodes.Except(preferredItemCodes);

                                //has match
                                if (lst.Count() == 0)
                                {
                                    matchedRegitryItemCode = regitm.RegistryItemCode;
                                    break;
                                }
	                        }

                            if (matchedRegitryItemCode.HasValue)
                            {
                                registryItem = selectedGiftRegistry.GiftRegistryItems.FirstOrDefault(giftItem => giftItem.RegistryItemCode == matchedRegitryItemCode);
                            }
                        }

                        //if not kit item get the item as is
                        if (registryItem == null && !isKit)
                        {
                            registryItem = selectedGiftRegistry.GiftRegistryItems.FirstOrDefault(giftItem => giftItem.ItemCode == itemCode &&
                                                                                     giftItem.GiftRegistryItemType == giftRegistryItemType);
                        }

                        if (registryItem != null)
                        {
                            registryItem.Quantity += Quantity;
                            registryItem.UnitMeasureCode = unitMeasureCode;
                            selectedGiftRegistry.GiftRegistryItems.UpdateToDb(registryItem);
                        }
                        else
                        {
                            registryItem = new GiftRegistryItem()
                            {
                                GiftRegistryItemType = giftRegistryItemType,
                                RegistryItemCode = Guid.NewGuid(),
                                ItemCode = itemCode,
                                Quantity = Quantity,
                                RegistryID = registryID.Value,
                                UnitMeasureCode = unitMeasureCode
                            };

                            selectedGiftRegistry.GiftRegistryItems.AddToDb(registryItem);
                        }

                        if (isKit && preferredComposition != null)
                        {
                            registryItem.ClearKitItemsFromComposition();
                            preferredComposition.AddToGiftRegistry(registryID.Value, registryItem.RegistryItemCode);
                        }

                        HttpContext.Current.Response.Redirect(string.Format("~/editgiftregistry.aspx?{0}={1}", DomainConstants.GIFTREGISTRYPARAMCHAR, registryID.Value));
                    }
                }

                GoNextPage(context);
            }

            #endregion

            CartRegistryParam registryCartParam = null;
            if (AppLogic.AppConfigBool("GiftRegistry.Enabled"))
            {
                registryCartParam = new CartRegistryParam ()
                {
                    RegistryID = CommonLogic.FormGuid("RegistryID"),
                    RegistryItemCode = CommonLogic.FormGuid("RegistryItemCode")
                };
            }

            if(registryCartParam != null && registryCartParam.RegistryID.HasValue && registryCartParam.RegistryItemCode.HasValue)
            {
                ShippingAddressID = GiftRegistryDA.GetPrimaryShippingAddressCodeOfOwnerByRegistryID(registryCartParam.RegistryID.Value);
                itemIsARegistryItem = true;
            }

            cart = new ShoppingCart(null, 1, ThisCustomer, CartType, string.Empty, false, true, string.Empty);
            if (Quantity > 0)
            {
                if (AppLogic.IsAKit(itemCode))
                {
                    var preferredComposition = KitComposition.FromForm(ThisCustomer, CartType, itemCode);

                    if (preferredComposition == null)
                    {
                        int itemCounter = 0;
                        int.TryParse(ProductID, out itemCounter);
                        var kitData = KitItemData.GetKitComposition(ThisCustomer, itemCounter, itemCode);

                        var kitContents = new StringBuilder();
                        foreach (var kitGroup in kitData.Groups)
                        {
                            if (kitContents.Length > 0) { kitContents.Append(","); }

                            var selectedItems = new StringBuilder();
                            int kitGroupCounter = kitGroup.Id;

                            var selectedKitItems = kitGroup.Items.Where(i => i.IsSelected == true);

                            foreach (var item in selectedKitItems)
                            {
                                if (selectedItems.Length > 0) { selectedItems.Append(","); }

                                //note: since we are adding the kit counter and kit item counter in KitItemData.GetKitComposition (stored proc. EcommerceGetKitItems)
                                //as "kit item counter", we'll reverse the process in order to get the "real kit item counter"

                                int kitItemCounter = item.Id - itemCounter; 
                                selectedItems.Append(kitGroupCounter.ToString() + DomainConstants.KITCOMPOSITION_DELIMITER + kitItemCounter.ToString());
                            }
                            kitContents.Append(selectedItems.ToString());
                        }
                        preferredComposition = KitComposition.FromComposition(kitContents.ToString(), ThisCustomer, CartType, itemCode);
                    }

                    preferredComposition.PricingType = CommonLogic.FormCanBeDangerousContent("KitPricingType");

                    if (CommonLogic.FormBool("IsEditKit") &&
                        !CommonLogic.IsStringNullOrEmpty(CommonLogic.FormCanBeDangerousContent("KitCartID")) &&
                        InterpriseHelper.IsValidGuid(CommonLogic.FormCanBeDangerousContent("KitCartID")))
                    {
                        Guid cartID = new Guid(CommonLogic.FormCanBeDangerousContent("KitCartID"));
                        preferredComposition.CartID = cartID;
                    }
                    cart.AddItem(ThisCustomer, ShippingAddressID, itemCode, int.Parse(ProductID), Quantity, unitMeasureCode, CartType, preferredComposition, registryCartParam);
                }
                else
                {
                    cart.AddItem(ThisCustomer, ShippingAddressID, itemCode, int.Parse(ProductID), Quantity, unitMeasureCode, CartType, null, registryCartParam);
                }
            }

            string RelatedProducts = CommonLogic.QueryStringCanBeDangerousContent("relatedproducts").Trim();
            string UpsellProducts = CommonLogic.FormCanBeDangerousContent("UpsellProducts").Trim();
            string combined = string.Concat(RelatedProducts, UpsellProducts); 

            if (combined.Length != 0 && CartType == CartTypeEnum.ShoppingCart)
            {
                string[] arrUpsell = combined.Split(',');
                foreach (string s in arrUpsell)
                {
                    string PID = s.Trim();
                    if (PID.Length == 0) { continue; }

                    int UpsellProductID;
                    try
                    {
                        UpsellProductID = Localization.ParseUSInt(PID);
                        if (UpsellProductID != 0)
                        {
                            string ItemCode = InterpriseHelper.GetInventoryItemCode(UpsellProductID);
                            string itemUnitMeasure = string.Empty;

                            using (var con = DB.NewSqlConnection())
                            {
                                con.Open();
                                using (var reader = DB.GetRSFormat(con, "SELECT ium.UnitMeasureCode FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryUnitMeasure ium with (NOLOCK) ON i.ItemCode = ium.ItemCode AND IsBase = 1 WHERE i.ItemCode = {0}", DB.SQuote(ItemCode)))
                                {
                                    if (reader.Read())
                                    {
                                        itemUnitMeasure = DB.RSField(reader, "UnitMeasureCode");
                                    }
                                }
                            }

                            cart.AddItem(ThisCustomer, ShippingAddressID, ItemCode, UpsellProductID, 1, itemUnitMeasure, CartType);
                        }
                    }
                    catch { }
                }
            }
        }

        GoNextPage(context, itemIsARegistryItem, CartType, ThisCustomer);
    }

    private void GoNextPage(HttpContext context, bool itemIsARegistryItem = false, CartTypeEnum cartType = CartTypeEnum.ShoppingCart, Customer ThisCustomer = null)
    {
        string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");        
        if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            throw new ArgumentException("SECURITY EXCEPTION");
        }

        CartTypeEnum CartType = CartTypeEnum.ShoppingCart;
        if (CommonLogic.FormNativeInt("IsWishList") == 1 || CommonLogic.QueryStringUSInt("IsWishList") == 1)
        {
            CartType = CartTypeEnum.WishCart;
        }

        bool isAddRegistryItem = (cartType == CartTypeEnum.ShoppingCart && itemIsARegistryItem);
        if ((isAddRegistryItem) ||
                ("STAY".Equals(AppLogic.AppConfig("AddToCartAction"), StringComparison.InvariantCultureIgnoreCase) && ReturnURL.Length != 0))
        {
            string addedParam = string.Empty;
            if (isAddRegistryItem)
            {
                addedParam = "&" + DomainConstants.NOTIFICATION_QRY_STRING_PARAM + "=" + AppLogic.GetString("editgiftregistry.aspx.48", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            context.Response.Redirect(ReturnURL + addedParam);
        }
        else
        {
            if (ReturnURL.Length == 0)
            {
                ReturnURL = string.Empty;
                if (context.Request.UrlReferrer != null)
                {
                    ReturnURL = context.Request.UrlReferrer.AbsoluteUri; // could be null
                }
                if (ReturnURL == null)
                {
                    ReturnURL = string.Empty;
                }
            }
            if (CartType == CartTypeEnum.WishCart)
            {
                context.Response.Redirect("wishlist.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
            if (CartType == CartTypeEnum.GiftRegistryCart)
            {
                context.Response.Redirect("giftregistry.aspx?ReturnUrl=" + Security.UrlEncode(ReturnURL));
            }
            context.Response.Redirect("ShoppingCart.aspx?add=true&ReturnUrl=" + Security.UrlEncode(ReturnURL));
        }
    }
}
#endregion

#region ExecXmlPackage
/// <summary>
/// Outputs the raw package results along with setting any http headers specified in the package.
/// The package transform output method needs to match the Content-Type http header or you may not get the results you expect
/// </summary>
public class ExecXmlPackage : IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        string pn = CommonLogic.QueryStringCanBeDangerousContent("xmlpackage");
        Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;
        try
        {
            using (XmlPackage2 p = new XmlPackage2(pn, ThisCustomer, ThisCustomer.SkinID, "", XmlPackageParam.FromString(""), "", true))
            {
                if (!p.AllowEngine)
                {
                    context.Response.Write("This XmlPackage is not allowed to be run from the engine.  Set the package element's allowengine attribute to true to enable this package to run.");
                }
                else
                {
                    if (p.HttpHeaders != null)
                    {
                        foreach (XmlNode xn in p.HttpHeaders)
                        {
                            string headername = xn.Attributes["headername"].InnerText;
                            string headervalue = xn.Attributes["headervalue"].InnerText;
                            context.Response.AddHeader(headername, headervalue);
                        }
                    }
                    string output = p.TransformString();
                    context.Response.AddHeader("Content-Length", output.Length.ToString());
                    context.Response.Write(output);
                }
            }
        }
        catch (Exception ex)
        {
            context.Response.Write(ex.Message + "<br/><br/>");
            Exception iex = ex.InnerException;
            while (iex != null)
            {
                context.Response.Write(ex.Message + "<br/><br/>");
                iex = iex.InnerException;
            }
        }
    }
}
#endregion

#region PaypalExpressCheckoutPostback

public class PaypalExpressCheckoutPostback : IHttpHandler
{
    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        var ThisCustomer = ((InterpriseSuiteEcommercePrincipal)context.User).ThisCustomer;

        var m_PayPalExpress = new PayPalExpress();
        //Get PayPal info
        var PayPalDetails = m_PayPalExpress.GetExpressCheckoutDetails(context.Request.QueryString["token"]).GetExpressCheckoutDetailsResponseDetails;
        var paypalShippingAddress = Address.New(ThisCustomer, AddressTypes.Shipping);

        if (PayPalDetails.PayerInfo.Address.Name.IsNullOrEmptyTrimmed() && (PayPalDetails.PayerInfo.Address.Street1.IsNullOrEmptyTrimmed() || PayPalDetails.PayerInfo.Address.Street2.IsNullOrEmptyTrimmed()) &&
            PayPalDetails.PayerInfo.Address.CityName.IsNullOrEmptyTrimmed() && PayPalDetails.PayerInfo.Address.StateOrProvince.IsNullOrEmptyTrimmed() && PayPalDetails.PayerInfo.Address.PostalCode.IsNullOrEmptyTrimmed() &&
            PayPalDetails.PayerInfo.Address.CountryName.ToString().IsNullOrEmptyTrimmed() || PayPalDetails.PayerInfo.ContactPhone.IsNullOrEmptyTrimmed())
        {
            paypalShippingAddress = ThisCustomer.PrimaryShippingAddress;
        }
        else
        {
            string streetAddress = PayPalDetails.PayerInfo.Address.Street1 + (!PayPalDetails.PayerInfo.Address.Street2.IsNullOrEmptyTrimmed() ? Environment.NewLine : String.Empty) + PayPalDetails.PayerInfo.Address.Street2;
            string sql = String.Empty;
            if (ThisCustomer.IsRegistered)
            {
                sql = String.Format("SELECT COUNT(ShipToCode) AS N FROM CustomerShipTo where Address = {0} and City = {1} and State = {2} and PostalCode = {3} and Country = {4} and ShipToName = {5} and CustomerCode = {6}",
                                streetAddress.ToDbQuote(), PayPalDetails.PayerInfo.Address.CityName.ToDbQuote(), PayPalDetails.PayerInfo.Address.StateOrProvince.ToDbQuote(), PayPalDetails.PayerInfo.Address.PostalCode.ToDbQuote(),
                                AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.CountryName).ToString().ToDbQuote(), PayPalDetails.PayerInfo.Address.Name.ToDbQuote(), ThisCustomer.CustomerCode.ToDbQuote());
            }
            else
            {
                sql = String.Format("SELECT COUNT(1) AS N FROM EcommerceAddress where ShipToAddress = {0} and ShipToCity = {1} and ShipToState = {2} and ShipToPostalCode = {3} and ShipToCountry = {4} and ShipToName = {5} and CustomerID = {6}",
                                streetAddress.ToDbQuote(), PayPalDetails.PayerInfo.Address.CityName.ToDbQuote(), PayPalDetails.PayerInfo.Address.StateOrProvince.ToDbQuote(), PayPalDetails.PayerInfo.Address.PostalCode.ToDbQuote(),
                                AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.CountryName).ToString().ToDbQuote(), PayPalDetails.PayerInfo.Address.Name.ToDbQuote(), ThisCustomer.CustomerCode.ToDbQuote());

                paypalShippingAddress.EMail = ThisCustomer.IsRegistered ? ThisCustomer.EMail : ThisCustomer.GetAnonEmail();
                paypalShippingAddress.Name = PayPalDetails.PayerInfo.Address.Name;
                paypalShippingAddress.Address1 = PayPalDetails.PayerInfo.Address.Street1 + (PayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty) + PayPalDetails.PayerInfo.Address.Street2;
                paypalShippingAddress.City = PayPalDetails.PayerInfo.Address.CityName;
                paypalShippingAddress.State = PayPalDetails.PayerInfo.Address.StateOrProvince;
                paypalShippingAddress.PostalCode = PayPalDetails.PayerInfo.Address.PostalCode;
                paypalShippingAddress.Country = AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.CountryName.ToString());
                paypalShippingAddress.Phone = PayPalDetails.PayerInfo.ContactPhone ?? String.Empty;

            }

            int isAddressExists = DB.GetSqlN(sql);

            if (AppLogic.AppConfigBool("PayPalCheckout.RequireConfirmedAddress") || isAddressExists == 0)
            {
                ServiceFactory.GetInstance<ICustomerService>().UpdateCustomerNotesWhenPaypalAddressIsUsed();
            }
        }

        ThisCustomer.PrimaryShippingAddress = paypalShippingAddress;
        paypalShippingAddress.Save();

        string redirectUrl = String.Empty;

        //Checking for redirectURL of PayPal -- Express Checkout button in Shopping Cart page or PayPal Radio Button in Payment Page
        if (Customer.Current.ThisCustomerSession["paypalfrom"] == "shoppingcart" || Customer.Current.ThisCustomerSession["paypalfrom"] == "checkoutanon")
        {
            redirectUrl = "checkoutshipping.aspx?PayPal=True&token=" + context.Request.QueryString["token"];
        }
        else
        {
            if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
            {
                if (!AppLogic.AppConfigBool("Checkout.UseOnePageCheckout.UseFinalReviewOrderPage"))
                {
                    //Insert PayPal call here for response - For authorize and capture of order from paypal inside IS
                    ThisCustomer.ThisCustomerSession["paypalfrom"] = "onepagecheckout";
                    string OrderNumber = String.Empty;
                    string status = String.Empty;
                    string receiptCode = String.Empty;
                    var billingAddress = ThisCustomer.PrimaryBillingAddress;
                    Address shippingAddress = null;
                    var cart = new InterpriseShoppingCart(null, ThisCustomer.SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
                    if (cart.IsNoShippingRequired())
                    {
                        cart.BuildSalesOrderDetails(false, true);
                    }
                    else
                    {
                        cart.BuildSalesOrderDetails();
                    }

                    if (!AppLogic.AppConfigBool("PayPalCheckout.OverrideAddress"))
                    {
                        if (!cart.HasShippableComponents())
                        {
                            shippingAddress = ThisCustomer.PrimaryShippingAddress;
                        }
                        else
                        {
                            if (ThisCustomer.IsRegistered)
                            {
                                var GetShippingAddress = new Address()
                                {
                                    Name = PayPalDetails.PayerInfo.Address.Name,
                                    Address1 = PayPalDetails.PayerInfo.Address.Street1 + (PayPalDetails.PayerInfo.Address.Street2 != String.Empty ? Environment.NewLine : String.Empty) + PayPalDetails.PayerInfo.Address.Street2,
                                    City = PayPalDetails.PayerInfo.Address.CityName,
                                    State = PayPalDetails.PayerInfo.Address.StateOrProvince,
                                    PostalCode = PayPalDetails.PayerInfo.Address.PostalCode,
                                    Country = AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.CountryName.ToString()),
                                    CountryISOCode = AppLogic.ResolvePayPalAddressCode(PayPalDetails.PayerInfo.Address.Country.ToString()),
                                    Phone = PayPalDetails.PayerInfo.ContactPhone ?? String.Empty
                                };
                                shippingAddress = GetShippingAddress;
                            }
                            else
                            {
                                shippingAddress = paypalShippingAddress;
                            }
                        }
                    }

                    var doExpressCheckoutResp = m_PayPalExpress.DoExpressCheckoutPayment(PayPalDetails.Token, PayPalDetails.PayerInfo.PayerID, OrderNumber, cart);
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

                        context.Response.Redirect("shoppingcart.aspx?ErrorMsg=" + result.ToUrlEncode(), false);
                        return;
                    }
                    else
                    {
                        Gateway gatewayToUse = null;
                        var payPalResp = new GatewayResponse(String.Empty)
                        {
                            AuthorizationCode = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID,
                            TransactionResponse = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                            Details = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString(),
                            AuthorizationTransID = doExpressCheckoutResp.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID
                        };

                        InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, DomainConstants.PAYMENT_METHOD_CREDITCARD);
                        status = cart.PlaceOrder(gatewayToUse, billingAddress, shippingAddress, ref OrderNumber, ref receiptCode, true, true, payPalResp, true, false);

                        if (status != AppLogic.ro_OK)
                        {
                            ThisCustomer.IncrementFailedTransactionCount();
                            if (ThisCustomer.FailedTransactionCount >= AppLogic.AppConfigUSInt("MaxFailedTransactionCount"))
                            {
                                cart.ClearTransaction();
                                ThisCustomer.ResetFailedTransactionCount();
                                context.Response.Redirect("orderfailed.aspx");
                            }
                            ThisCustomer.ClearTransactions(false);
                            context.Response.Redirect("checkout1.aspx?paymentterm=" + ThisCustomer.PaymentTermCode + "&errormsg=" + status.ToUrlEncode());
                        }

                        AppLogic.ClearCardNumberInSession(ThisCustomer);
                        ThisCustomer.ClearTransactions(true);

                        context.Response.Redirect(String.Format("orderconfirmation.aspx?ordernumber={0}", OrderNumber.ToUrlEncode()));
                    }
                }
                else
                {
                    InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, DomainConstants.PAYMENT_METHOD_CREDITCARD);
                    redirectUrl = "checkoutreview.aspx?PayPal=True&token=" + context.Request.QueryString["token"];
                }
            }
            else
            {
                InterpriseHelper.UpdateCustomerPaymentTerm(ThisCustomer, DomainConstants.PAYMENT_METHOD_CREDITCARD);
                redirectUrl = "checkoutreview.aspx?PayPal=True&token=" + context.Request.QueryString["token"];
            }
        }

        context.Response.Redirect(redirectUrl);
    }

}
#endregion