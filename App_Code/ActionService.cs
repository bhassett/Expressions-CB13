// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Interprise.Framework.ECommerce.DatasetGateway;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceCommon.Domain;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon.Domain.Model;
/// <summary>
/// Summary description for Action
/// </summary>
[WebService(Namespace = "http://www.interprisesuite.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class ActionService : System.Web.Services.WebService
{

    public ActionService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    /// <summary>
    /// Gets the list of available shipping method options
    /// </summary>
    /// <param name="overrideDefaultAddress"></param>
    /// <param name="id"></param>
    /// <param name="addressId"></param>
    /// <returns></returns>
    [WebMethod, ScriptMethod]
    public ShippingMethodDTOCollection ShippingMethod(string addresNameValuePairOverride, string id, string addressId)
    {
        var thisCustomer = Customer.Current;

        Address preferredAddress = null;

        var cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
        cart.BuildSalesOrderDetails(false, true, thisCustomer.CouponCode);

        bool overrideDefaultAddress = !CommonLogic.IsStringNullOrEmpty(addresNameValuePairOverride);

        var giftRegistryItem = cart.CartItems
                                .Where(itm => itm.GiftRegistryID.HasValue)
                                .FirstOrDefault(r => r.m_ShippingAddressID == addressId);

        Guid? giftRegistryItemID = (giftRegistryItem != null && giftRegistryItem.GiftRegistryID.HasValue) ? giftRegistryItem.GiftRegistryID : null;

        if (overrideDefaultAddress)
        {
            var addressNameValuePair = HttpUtility.ParseQueryString(addresNameValuePairOverride);
            preferredAddress = Address.FromForm(thisCustomer, AddressTypes.Shipping, addressNameValuePair);
        }
        else if (!CommonLogic.IsStringNullOrEmpty(addressId))
        {
            if (giftRegistryItemID.HasValue)
            {
                preferredAddress = Address.Get(thisCustomer, AddressTypes.Shipping, addressId, giftRegistryItemID);
            }
            else
            {
                preferredAddress = Address.Get(thisCustomer, AddressTypes.Shipping, addressId);
            }

            if (cart.HasMultipleShippingAddresses())
            {
                InterpriseShoppingCart originalCart = cart;
                cart = cart.ForAddress(preferredAddress);
                originalCart.Dispose(); // dispose the original cart object
            }
        }
        else
        {
            preferredAddress = thisCustomer.PrimaryShippingAddress;
        }

        string shippingMethodfromSC = string.Empty;

        var myCookie = new HttpCookie("selectedSM");
        myCookie = HttpContext.Current.Request.Cookies["selectedSM"];

        if (myCookie != null)
        {
            shippingMethodfromSC = myCookie.Value;
            cart.SetCartShippingMethod(shippingMethodfromSC);
        }
        else
        {
            shippingMethodfromSC = "";
        }

        var availableShippingMethods = cart.GetShippingMethods(preferredAddress, giftRegistryItemID);
        return availableShippingMethods;

    }

    /// <summary>
    /// This computation is for OnepageCheckout AJAX CALL
    /// </summary>
    [WebMethod, ScriptMethod]
    public ShippingCalculationSummary GetShippingCalculation(string shippingMethodCode, int freightCalculation, string rateID)
    {
        var thisCustomer = Customer.Current;
        var summary = new ShippingCalculationSummary();
        string customerCode = thisCustomer.CustomerCode;

        var cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
        cart.BuildSalesOrderDetails();

        if (!cart.CouponIncludesFreeShipping())
        {
            if (freightCalculation == 1 || freightCalculation == 2)
            {
                cart.SetCartShippingMethod(shippingMethodCode, string.Empty, new Guid(rateID));
            }
            else
            {
                cart.SetCartShippingMethod(shippingMethodCode);
            }
        }

        //Recreate the cart to update the Resultset based from the selected Freight/Tax
        //Totals are converted to string to automatically format the amounts
        cart.BuildSalesOrderDetails();
        summary.SubTotal = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].SubTotalRate, customerCode);
        summary.Freight = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].FreightRate, customerCode);
        summary.Tax = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].TaxRate, customerCode);
        summary.Discount = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].CouponDiscountRate, customerCode);
        summary.DueTotal = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].TotalRate, customerCode);
        summary.Balance = InterpriseHelper.FormatCurrencyForCustomer(cart.SalesOrderDataset.CustomerSalesOrderView[0].BalanceRate, customerCode);
        return summary;
    }

    [WebMethod, ScriptMethod]
    public string GetShippingMethodRates(ShippingMethodDTO shippingMethodInfo, string addressNameValuPairOverride, string addressId)
    {
        Security.AuthenticateService();
        var thisCustomer = Customer.Current;

        var preferredAddress = new Address();

        var cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);
        cart.BuildSalesOrderDetails();

        bool overrideDefaultAddress = !CommonLogic.IsStringNullOrEmpty(addressNameValuPairOverride);

        var giftRegistryItem = cart.CartItems
                                .Where(itm => itm.GiftRegistryID.HasValue)
                                .FirstOrDefault(r => r.m_ShippingAddressID == addressId);

        Guid? giftRegistryItemID = (giftRegistryItem != null && giftRegistryItem.GiftRegistryID.HasValue) ? giftRegistryItem.GiftRegistryID : null;

        if (overrideDefaultAddress)
        {
            var addressNameValuePair = HttpUtility.ParseQueryString(addressNameValuPairOverride);
            preferredAddress = Address.FromForm(thisCustomer, AddressTypes.Shipping, addressNameValuePair);
        }
        else if (!CommonLogic.IsStringNullOrEmpty(addressId))
        {
            if (giftRegistryItemID.HasValue)
            {
                preferredAddress = Address.Get(thisCustomer, AddressTypes.Shipping, addressId, giftRegistryItemID);
            }
            else
            {
                preferredAddress = Address.Get(thisCustomer, AddressTypes.Shipping, addressId);
            }

            if (cart.HasMultipleShippingAddresses())
            {
                var originalCart = cart;
                cart = cart.ForAddress(preferredAddress);
                originalCart.Dispose(); // dispose the original cart object
            }
        }
        else
        {
            preferredAddress = thisCustomer.PrimaryShippingAddress;
        }

        string rate = String.Empty;
        cart.CalculateShippingMethodRatesOnDemand(shippingMethodInfo, preferredAddress, giftRegistryItemID);

        if (shippingMethodInfo.IsError)
        {
            rate = AppLogic.GetString("checkoutshipping.aspx.15", thisCustomer.SkinID, thisCustomer.LocaleSetting);
        }
        else
        {
            rate = shippingMethodInfo.FreightDisplay;
        }

        return rate;
    }

    private const string ACTION = "action";
    private const string ACTION_GET_STATES = "getStates";
    private const string ACTION_ADD_NEW = "new";

    /// <summary>
    /// Gets the collection of states for a given country
    /// </summary>
    /// <param name="forCountry"></param>
    /// <returns></returns>
    [WebMethod, ScriptMethod]
    public List<StateDTO> GetStates(string forCountry)
    {
        forCountry = HttpUtility.UrlDecode(forCountry);

        List<StateDTO> states = new List<StateDTO>();

        CountryAddressDTO requestedCountry = CountryAddressDTO.Find(forCountry);
        if (null != requestedCountry)
        {
            states = requestedCountry.GetStates();
        }

        return states;
    }

    /// <summary>
    /// Gets the pricing level html for an item based on the current customer
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    [WebMethod, ScriptMethod]
    public string GetPricingLevel(string itemCode)
    {
        itemCode = HttpUtility.UrlDecode(itemCode);

        Customer thisCustomer = Customer.Current;
        string response = String.Empty;

        //Do not execute code if pricing level is not applicable
        if (thisCustomer.PricingLevel == String.Empty)
        {
            return response;
        }
        bool hasPricingLevel;

        response = InterpriseHelper.GetInventoryPricingLevelTable(Customer.Current, itemCode, out hasPricingLevel);

        if (!hasPricingLevel)
        {
            return string.Empty;
        }

        return response;
    }

    /// <summary>
    /// Gets the order history
    /// </summary>
    /// <param name="pages"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    [WebMethod, ScriptMethod]
    public SalesOrderHistoryCollection GetOrderHistory(int pages, int current)
    {        
        return ServiceFactory.GetInstance<IOrderService>().GetCustomerSalesOrders(pages, current);   
    }

    [WebMethod, ScriptMethod]
    public AddressDTO AddNewAddress(string addresNameValuePair)
    {
        NameValueCollection addressNameValuePair = HttpUtility.ParseQueryString(addresNameValuePair);
        if (null != addressNameValuePair)
        {
            Address newAddress = Address.FromForm(Customer.Current, AddressTypes.Shipping, addressNameValuePair);
            string shipToCode = InterpriseHelper.AddCustomerShipTo(newAddress);

            if (!CommonLogic.IsStringNullOrEmpty(shipToCode))
            {
                return newAddress.ForTransfer();
            }
        }

        return null;
    }

    [WebMethod, ScriptMethod]
    public decimal GetItemPrice(string itemCode, string itemType, string unitMeasureCode, string composition)
    {
        return AppLogic.GetKitItemPrice(itemCode, itemType, unitMeasureCode, composition);
    }

    [WebMethod, ScriptMethod]
    public string[][] GetProductCompareImageLinks(int[] productIDs, bool includejavascript, string xmlpackagename)
    {
        string[] links = InterpriseHelper.GetProductCompareImageLinks(productIDs);
        string[] package = new string[] { InterpriseHelper.GetProductCompareXmlPackage(includejavascript, xmlpackagename) };
        string[][] returnvalue = new string[][] { links, package };
        return returnvalue;
    }

    [WebMethod]
    public string InsertItemToCart(string counter, string itemCode, decimal quantity, string uom, string typeOfCart)
    {
        try
        {
            var thisCustomer = Customer.Current;
            int skinId = thisCustomer.SkinID;
            string localeSetting = thisCustomer.LocaleSetting;

            if (AppLogic.AppConfigBool("Inventory.LimitCartToQuantityOnHand"))
            {
                decimal freeStock = InterpriseHelper.GetInventoryFreeStock(itemCode, uom, thisCustomer);
               
                if (freeStock <= 0)
                {
                    return String.Format("failed::{0}{1}", AppLogic.GetString("showproduct.aspx.30", skinId, localeSetting), freeStock.ToString());
                }

                if (freeStock < quantity)
                {
                    return String.Format("failed::{0}{1}", AppLogic.GetString("showproduct.aspx.42", skinId, localeSetting), freeStock.ToString());
                }
            }

            if (!thisCustomer.IsRegistered && typeOfCart == "wish-list")
            {
                return String.Format("failed::{0}", AppLogic.GetString("showproduct.aspx.57", skinId, localeSetting));
            }

            CartTypeEnum cartType;
            switch (typeOfCart)
            {
                case "shopping-cart":
                    cartType = CartTypeEnum.ShoppingCart;
                    break;
                case "wish-list":
                    cartType = CartTypeEnum.WishCart;
                    break;
                case "gift-list":
                    cartType = CartTypeEnum.GiftRegistryCart;
                    break;
                case "recurring":
                    cartType = CartTypeEnum.RecurringCart;
                    break;
                default:
                    cartType = CartTypeEnum.ShoppingCart;
                    break;
            }

            InterpriseShoppingCart cart = new InterpriseShoppingCart(null, skinId, thisCustomer, cartType, String.Empty, false, true);
            cart.AddItem(thisCustomer, thisCustomer.PrimaryShippingAddressID, itemCode, Convert.ToInt32(counter), quantity, uom, cartType).ToString();
        }
        catch (Exception ex)
        {
           return String.Format("failed::{0}", ex.Message.ToString());
        }

        return String.Empty;
    }

    #region Minicart

    [WebMethod, ScriptMethod]
    public void RemoveMiniCartItem(string cartRecordID)
    {
        Security.AuthenticateService();

        Customer thisCustomer = Customer.Current;

        InterpriseShoppingCart cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
        cart.RemoveItem(Convert.ToInt32(cartRecordID));
    }

    [WebMethod, ScriptMethod]
    public void AddToCart(string counter)
    {
        Customer thisCustomer = Customer.Current;
        InterpriseShoppingCart cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);

        string itemCode = InterpriseHelper.GetInventoryItemCode(Convert.ToInt32(counter));
        string shippingAddressID = CommonLogic.IIF(thisCustomer.IsNotRegistered, string.Empty, thisCustomer.PrimaryShippingAddressID);
        var umInfo = InterpriseHelper.GetItemDefaultUnitMeasure(itemCode);

        cart.AddItem(thisCustomer, shippingAddressID, itemCode, Convert.ToInt32(counter), 1, umInfo.Code, CartTypeEnum.ShoppingCart);
    }

    [WebMethod, ScriptMethod]
    public string ShoppingCartNumber()
    {
        Customer thisCustomer = Customer.Current;
        string tmpS;

        tmpS = AppLogic.GetString("AppConfig.CartPrompt", thisCustomer.SkinID, thisCustomer.LocaleSetting);
        tmpS += "&nbsp;(";
        tmpS += Localization.ParseLocaleDecimal(ShoppingCart.NumItems(thisCustomer.CustomerID, CartTypeEnum.ShoppingCart, thisCustomer.ContactCode), thisCustomer.LocaleSetting);
        tmpS += ")";

        return tmpS;
    }

    [WebMethod, ScriptMethod]
    public void UpdateCartItemQuantity(string cartRecordID, string Quantity)
    {
        Customer thisCustomer = Customer.Current;

        InterpriseShoppingCart cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
        cart.SetItemQuantity(Convert.ToInt32(cartRecordID), Convert.ToDecimal(Quantity));
    }

    [WebMethod, ScriptMethod]
    public string GetAccessoryItemForMinicart(string counter)
    {
        string result = string.Empty;
        string itemCode = InterpriseHelper.GetInventoryItemCode(Convert.ToInt32(counter));
        result = InterpriseHelper.GetAccessoryProductsForMiniCart(itemCode);
        return result;
    }

    [WebMethod, ScriptMethod]
    public string RedirectToPayPalCheckoutMinicart()
    {
        string result = String.Empty;
        var thisCustomer = Customer.Current;
        var cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

        cart.BuildSalesOrderDetails(false, false);

        if (!thisCustomer.IsRegistered &&
                (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && AppLogic.AppConfigBool("PayPalCheckout.AllowAnonCheckout")))
        {
            result = "checkoutanon.aspx?checkout=true&checkouttype=pp";
        }
        else
        {
            thisCustomer.ThisCustomerSession["paypalfrom"] = "shoppingcart";
            result = PayPalExpress.CheckoutURL(cart);
        }
        return result;
    }

    [WebMethod, ScriptMethod]
    public string RedirectToGoogleCheckoutMinicart()
    {
        string result = String.Empty;
        var thisCustomer = Customer.Current;
        var cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

        cart.BuildSalesOrderDetails();

        if (!thisCustomer.IsRegistered &&
                (AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") && AppLogic.AppConfigBool("GoogleCheckout.AllowAnonCheckout")))
        {
            result = "checkoutanon.aspx?checkout=true&checkouttype=gc";
        }
        else
        {
            result = GoogleCheckout.CreateGoogleCheckout(cart);
        }
        return result;
    }

    [WebMethod, ScriptMethod]
    public string BuildMiniCart()
    {
        string result = string.Empty;
        Customer thisCustomer = Customer.Current;
        List<XmlPackageParam> runtimeParams = new List<XmlPackageParam>();
        runtimeParams.Add(new XmlPackageParam("CartType", Convert.ToString(0)));
        runtimeParams.Add(new XmlPackageParam("WarehouseCode", thisCustomer.WarehouseCode.ToString()));
        runtimeParams.Add(new XmlPackageParam("ContactCode", thisCustomer.ContactCode.ToString()));

        result = AppLogic.RunXmlPackage(
                        "page.minicart.xml.config",
                        null,
                        thisCustomer,
                        AppLogic.DefaultSkinID(),
                        String.Empty,
                        runtimeParams,
                        true,
                        true);

        return result;
    }

    [WebMethod, ScriptMethod]
    public void UpdateCart(List<string> qtyArray, List<string> chkArray)
    {
        Security.AuthenticateService();

        if (qtyArray != null)
        {
            int index;
            string cartRecordID;
            string Quantity;

            foreach (string i in qtyArray)
            {
                index = i.IndexOf(":");
                cartRecordID = i.Substring(0, index);
                Quantity = i.Substring(index + 1);
                UpdateCartItemQuantity(cartRecordID, Quantity);
            }
        }

        if (chkArray != null)
        {
            foreach (string counter in chkArray)
            {
                AddToCart(counter);
            }
        }
    }

    #endregion

    [WebMethod(EnableSession = true)]
    public string CreateLeadTaskController(List<string> list, string task)
    {
        string status = string.Empty;

        switch (task)
        {

            case AppLogic.VALIDATECAPTCHA:

                string cSecurityCode = HttpContext.Current.Session["SecurityCode"].ToString();
                string submittedCode = list[0];

                if (submittedCode != cSecurityCode)
                {

                    status = AppLogic.CAPTCHA_MISMATCH;
                }
                else
                {
                    status = AppLogic.CAPTCHA_MATCH;
                }

                break;

            case AppLogic.SAVE_LEAD:

                string email = list[4];
                string firstName = list[1];
                string middleName = string.Empty;
                string lastName = list[3];

                bool emailHasDuplicates = InterpriseHelper.IsLeadEmailDuplicate(email);
                bool leadHasDuplicates = InterpriseHelper.IsLeadDuplicate(firstName, middleName, lastName);

                if (emailHasDuplicates) status = AppLogic.EMAIL_HAS_DUPLICATES;
                if (leadHasDuplicates) status = AppLogic.LEAD_NAME_HAS_DUPLICATES;

                if (!emailHasDuplicates && !leadHasDuplicates) status = InterpriseHelper.CreateNewLead(list);

                break;
            case AppLogic.RENDER_STATES:

                string country = list[0];
                status = AppLogic.RenderStatesOptionsHTML(country);

                break;
            default:

                status = AppLogic.UNDEFINED_TASK;

                break;
        }

        return status;
    }

    [WebMethod]
    public string GetCaseHistory(string activityStatus, string period, string searchString)
    {

        var cases = new List<CustomerActivityCase>();
        cases = CustomerActivityCase.GetCustomerActivityCase(activityStatus, period, searchString);

        string jsonValue = JSONHelper.Serialize<List<CustomerActivityCase>>(cases);

        return jsonValue;
    }

    [WebMethod, ScriptMethod]
    //This will provide the ajax autocomplete for the postal code after filtering the country
    public List<SystemPostalCode> GetSystemPostalCode(string countryname, string postalcode)
    {
        return AppLogic.GetSystemPostalCode(countryname, postalcode);
    }

    [WebMethod]
    public bool SaveNotificationService(int notificationType, string itemCode, string productUrl)
    {
        string[][] ruleloaddataset = new string[][] { new string[] {"ECOMMERCENOTIFICATION", "READECOMMERCENOTIFICATION", "@ContactCode", Customer.Current.ContactCode,
                                                      "@WebsiteCode", InterpriseHelper.ConfigInstance.WebSiteCode, "@ItemCode", itemCode, "@EmailAddress", Customer.Current.EMail}};

        var ruleDatasetContainer = new EcommerceNotificationDatasetGateway();
        if (!Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.LoadDataSet(InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, ruleloaddataset, ruleDatasetContainer)) return false;

        EcommerceNotificationDatasetGateway.EcommerceNotificationRow ruleDatasetContainernewRow = null;

        if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
        {
            ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification.NewEcommerceNotificationRow();
        }
        else
        {
            ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification[0];
        }

        bool onPriceDrop = AppLogic.CheckNotification(Customer.Current.ContactCode, Customer.Current.EMail, itemCode, 1);
        bool onItemAvail = AppLogic.CheckNotification(Customer.Current.ContactCode, Customer.Current.EMail, itemCode, 0);

        if (notificationType == 1)
        {
            onPriceDrop = true;
        }
        else
        {
            onItemAvail = true;
        }


        ruleDatasetContainernewRow.BeginEdit();
        ruleDatasetContainernewRow.WebSiteCode = InterpriseHelper.ConfigInstance.WebSiteCode;
        ruleDatasetContainernewRow.ItemCode = itemCode;
        ruleDatasetContainernewRow.ContactCode = Customer.Current.ContactCode;
        ruleDatasetContainernewRow.EmailAddress = Customer.Current.EMail;
        ruleDatasetContainernewRow.NotifyOnPriceDrop = onPriceDrop;
        ruleDatasetContainernewRow.NotifyOnItemAvail = onItemAvail;
        ruleDatasetContainernewRow.ProductURL = productUrl;

        byte[] salt = InterpriseHelper.GenerateSalt();
        byte[] iv = InterpriseHelper.GenerateVector();
        string contactCodeCypher = InterpriseHelper.Encryption(Customer.Current.ContactCode, salt, iv);
        string emailAddressCypher = InterpriseHelper.Encryption(Customer.Current.EMail, salt, iv);

        ruleDatasetContainernewRow.EncryptedContactCode = string.Format("{0}|{1}|{2}", contactCodeCypher, Convert.ToBase64String(salt), Convert.ToBase64String(iv));
        ruleDatasetContainernewRow.EncryptedEmailAddress = string.Format("{0}|{1}|{2}", emailAddressCypher, Convert.ToBase64String(salt), Convert.ToBase64String(iv));
        ruleDatasetContainernewRow.EndEdit();

        if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
        {
            ruleDatasetContainer.EcommerceNotification.AddEcommerceNotificationRow(ruleDatasetContainernewRow);
        }

        string[][] rulecommandset;
        rulecommandset = new string[][] { new string[] { ruleDatasetContainer.EcommerceNotification.TableName, "CREATEECOMMERCENOTIFICATION",
                                                                    "UPDATEECOMMERCENOTIFICATION", "DELETEECOMMERCENOTIFICATION"} };

        return Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.UpdateDataset(InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, rulecommandset, ruleDatasetContainer);
    }

    #region ShippingCalculator

    [WebMethod, ScriptMethod]
    public string GetShippingMethodCalc(string country, string state, string postalCode, string addressType)
    {
        string str = string.Empty;
        string formattedFreight = string.Empty;
        decimal freight = 0;
        Customer thisCustomer = Customer.Current;

        InterpriseShoppingCart cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
        cart.BuildSalesOrderDetails();

        Address destinationAddress = new Address();
        destinationAddress.Country = country;
        destinationAddress.PostalCode = postalCode;
        destinationAddress.State = state;
        destinationAddress.ResidenceType = InterpriseHelper.ResolveResidenceType(addressType);

        ShippingMethodDTOCollection availableShippingMethods = cart.GetShippingMethodsForShippingCalc(destinationAddress, string.Empty);
        for (int ctr = 0; ctr < availableShippingMethods.Count; ctr++)
        {
            ShippingMethodDTO shippingMethod = availableShippingMethods[ctr];
            freight = shippingMethod.Freight;
            formattedFreight = "<span class=freightText>" + " " + InterpriseHelper.FormatCurrencyForCustomer(freight, thisCustomer.CurrencyCode) + "</span>";
            str += "<input type=radio name=shippingmethod value='" + shippingMethod.Code + "'>" + "</input> <span>" + shippingMethod.Description + formattedFreight + "</span>" + "<br/>";
        }

        return str;
    }

    [WebMethod]
    public string GetRegisteredCustomerShippingAddress()
    {
        bool CustomerIsRegistered = Customer.Current.IsRegistered;
        string returnValue;

        if (CustomerIsRegistered)
        {
            var dtoShippingAddress = new AddressDTO();
            var custShippingAddress = Customer.Current.ShippingAddresses;

            dtoShippingAddress.country = custShippingAddress[0].Country;
            dtoShippingAddress.state = custShippingAddress[0].State;
            dtoShippingAddress.postalCode = custShippingAddress[0].PostalCode;
            dtoShippingAddress.city = custShippingAddress[0].City;
            dtoShippingAddress.residenceType = custShippingAddress[0].ResidenceType;
            returnValue = JSONHelper.Serialize<AddressDTO>(dtoShippingAddress);
        }
        else
        {
            returnValue = string.Empty;
        }

        return returnValue;
    }

    #endregion

    [WebMethod, ScriptMethod]
    public string PopulateStates(List<string> list)
    {
        string status = string.Empty;
        string country = list[0];
        status = AppLogic.RenderStatesOptionsHTML(country);
        return status;
    }

    [WebMethod]
    public string GetGlobalConfig()
    {
        var listConfiguration = new List<GlobalConfig>();

        string key = string.Empty;
        string value;

        // ------- Add Here the global configuration ------ //

        key = "MiniCart.Enabled"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "WebSupport.Enabled"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        bool isInMobile = CurrentContext.IsRequestingFromMobileMode(Customer.Current);
        key = "IsMobile"; value = isInMobile.ToStringLower();
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "GiftRegistry.Enabled"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "Service.Token"; value = Security.GetMD5Hash(Customer.Current.CustomerCode);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "GoogleAnalytics.TrackingCode";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value));

        key = "GoogleAnalytics.PageTracking";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "GoogleAnalytics.ConversionTracking";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "ShowSocialMediaSubscribeBox";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "ItemPopup.Enabled";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "IsAdminCurrentlyLoggedIn";
        value = Security.IsAdminCurrentlyLoggedIn().ToString();
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultSkinID"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "Checkout.UseOnePageCheckout";
        value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        key = "ShippingRatesOnDemand"; value = AppLogic.AppConfig(key);
        listConfiguration.Add(new GlobalConfig(key, value.ToLower()));

        // ------- End Global Configuration --------------- //

        string jsonValue = JSONHelper.Serialize<List<GlobalConfig>>(listConfiguration);
        return jsonValue;
    }

    [WebMethod]
    public string LoadStringResources(List<string> keys)
    {
        if (keys == null || keys.Count() == 0) return String.Empty;

        var thisCustomer = Customer.Current;

        var resources = new List<StringResourceDTO>();
        keys.ForEach(key =>
        {
            string value = AppLogic.GetString(key, thisCustomer.SkinID, thisCustomer.LocaleSetting, true);
            resources.Add(new StringResourceDTO(key, value));
        });

        return resources.ToJSON();
    }

    [WebMethod]
    public string LoadAppConfigs(List<string> keys)
    {
        var appConfigs = new List<GlobalConfig>();
        string value = String.Empty;

        foreach (string key in keys)
        {
            value = AppLogic.AppConfig(key);
            appConfigs.Add(new GlobalConfig(key, value));
        }
        
        string jsonValue = ServiceFactory.GetInstance<ICryptographyService>().SerializeToJson<List<GlobalConfig>>(appConfigs);
        return jsonValue;
    }

    [WebMethod]
    public string GenerateRequestCode()
    {
        return ServiceFactory.GetInstance<ICustomerService>()
                             .GenerateRequestCodeForActiveShopper();
    }

    [WebMethod, ScriptMethod]
    public CreditCardDTO GetCreditCardInfo(string cardCode)
    {
        CreditCardDTO credit = null;

        if (cardCode != string.Empty)
        {
            credit = CreditCardDTO.Find(AppLogic.DecryptCreditCardCode(Customer.Current, cardCode));
            credit.CreditCardCode = cardCode;
        }
        return credit;
    }

    [WebMethod, ScriptMethod]
    public void ClearCreditCardInfo(string cardCode)
    {
        AppLogic.ClearCreditCardInfo(AppLogic.DecryptCreditCardCode(Customer.Current, cardCode));
    }

    private void SendEmailNotification(bool _skipRegistration, string email, string firstName, string accountName)
    {
        if (AppLogic.AppConfigBool("SendWelcomeEmail") && (!_skipRegistration))
        {

            AppLogic.SendMail(
                AppLogic.GetString("createaccount.aspx.27", Customer.Current.SkinID, Customer.Current.LocaleSetting),
                AppLogic.RunXmlPackage(AppLogic.AppConfig("XmlPackage.WelcomeEmail"), null, Customer.Current, Customer.Current.SkinID, string.Empty, AppLogic.MakeXmlPackageParamsFromString("fullname=" + accountName), false, false),
                true,
                AppLogic.AppConfig("MailMe_FromAddress"),
                AppLogic.AppConfig("MailMe_FromName"),
                email,
                CommonLogic.IIF(Customer.Current.IsRegistered, firstName, accountName),
                "",
                AppLogic.AppConfig("MailMe_Server")
            );
        }
    }

    #region GiftRegistry

    [WebMethod]
    public void MoveItemToRegistry(string sourceRegistryID, string targetRegistryID, string registryItemCode)
    {
        Security.AuthenticateService();

        Guid? sourceRegistryGiuid = sourceRegistryID.TryParseGuid();
        Guid? targetRegistryGiuid = targetRegistryID.TryParseGuid();
        Guid? itemToMoveCode = registryItemCode.TryParseGuid();

        GiftRegistryDA.MoveRegistryItem(targetRegistryGiuid.Value, itemToMoveCode.Value);
        GiftRegistryDA.MoveCompositionKitItems(sourceRegistryGiuid.Value, targetRegistryGiuid.Value, itemToMoveCode.Value);
    }

    [WebMethod]
    public void UpdateRegistryItem(string registryItemCode, string comment, int sortOrder, int quantity)
    {
        //This will only be used for web methods that requires authentication.
        Security.AuthenticateService();

        Guid? itemToUpdateCode = registryItemCode.TryParseGuid();
        string htmlEncoded = comment.ToHtmlEncode();

        var registryItem = GiftRegistryDA.GetGiftRegistryItemByRegistryItemCode(itemToUpdateCode.Value);
        if (registryItem == null) return;

        registryItem.Comment = htmlEncoded;
        registryItem.SortOrder = sortOrder;
        registryItem.Quantity = quantity;

        GiftRegistryDA.UpdateRegistryItem(registryItem);
    }

    [WebMethod]
    public void DeleteRegistryItem(string registryItemCode, string giftRegistryId)
    {
        //This will only be used for web methods that requires authentication.
        Security.AuthenticateService();

        Guid? itemDeleteCode = registryItemCode.TryParseGuid();
        Guid? giftRegistryIdGuid = giftRegistryId.TryParseGuid();

        GiftRegistryDA.DeleteRegistryItem(itemDeleteCode.Value);
        GiftRegistryDA.ClearKitItemsFromComposition(giftRegistryIdGuid.Value, itemDeleteCode.Value);
    }

    [WebMethod]
    public string FindRegistriesReturnJSON(string firstName, string lastName, string eventTitle, int currentRow)
    {
        var header = GiftRegistryDA.FindRegistries(firstName, lastName, eventTitle, currentRow, InterpriseHelper.ConfigInstance.WebSiteCode);
        var lstfinditems = header.RawItems.Select(item => new GiftRegistryFindItem()
        {
            Title = item.Title,
            PictureFileName = item.PictureFileName,
            StartDate = item.StartDate.Value.ToShortDateString(),
            EndDate = item.EndDate.Value.ToShortDateString(),
            Counter = item.Counter,
            RegistryID = item.RegistryID,
            ContactGUID = item.ContactGUID,
            URLForViewing = item.URLForViewing,
            RowNumber = item.RowNumber,
            OwnersFullName = item.OwnersFullName
        }).ToArray();

        Thread.Sleep(300);

        var dto = new GiftRegistryFindHeaderDTO();
        dto.Items = lstfinditems;
        dto.TotalRecord = header.TotalRecord;
        dto.DefaultRecordPerSet = (int)DomainConstants.DEFAULT_REGISTRY_PAGESIZE;

        double totalSet = header.TotalRecord / DomainConstants.DEFAULT_REGISTRY_PAGESIZE;
        dto.TotalSet = (totalSet <= 1) ? 1 : (int)Math.Ceiling(totalSet);

        var lastItem = lstfinditems.LastOrDefault();
        dto.CurrentRecord = (lastItem != null) ? lastItem.RowNumber : 0;

        return JSONHelper.Serialize<GiftRegistryFindHeaderDTO>(dto);
    }

    [WebMethod]
    public void DeleteGiftRegistry(string giftRegistryID)
    {
        //This will only be used for web methods that requires authentication.
        Security.AuthenticateService();

        Guid? sourceRegistryGiuid = giftRegistryID.TryParseGuid();
        if (sourceRegistryGiuid.HasValue) GiftRegistryDA.DeleteGiftRegistry(sourceRegistryGiuid.Value);
    }

    #endregion

    #region One Page Checkout

    private const string NO_ACTIVE_POSTAL = "no-active-postal";
    private const string INVALID_EMAIL = "invalid-email";
    private const string INVALID_POSTAL = "invalid-postal";
    private const string INVALID_STATE = "invalid-state";
    private const string IS_VALID = "valid";
    private const string IS_OVER13_REQUIRED = "required-over-13";
    private const string ADDRESS_IS_SAVED = "saved";
    private const string ZERO_POSTAL = "0";

    #region One Page Checkout Customer Info Loader

    [WebMethod]
    public string GetCustomerInfo(string infoType)
    {
        string resources = String.Empty;

        var thisCustomer = Customer.Current;
        var thisAddress = Address.New(thisCustomer, AddressTypes.Shipping);

        thisCustomer.RequireCustomerRecord();

        var listResources = new List<GlobalConfig>();

        string key = String.Empty;
        string value;

        var aShipping = thisAddress.ThisCustomer.PrimaryShippingAddress;
        var cart = new InterpriseShoppingCart(null, AppLogic.GetCurrentSkinID(), thisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);

        if (!aShipping.Name.IsNullOrEmptyTrimmed())
        {
            switch (infoType)
            {
                case "shipping-contact":

                    key = "im-registered";
                    value = thisCustomer.IsRegistered.ToString().ToLower();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "final-button-text";
                    value = CommonLogic.IIF(AppLogic.AppConfigBool("Checkout.UseOnePageCheckout.UseFinalReviewOrderPage"), "Review Order", "Place Order");
                    listResources.Add(new GlobalConfig(key, value));

                    key = "contact-name";
                    value = aShipping.Name;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "email";
                    value = CommonLogic.IIF(aShipping.EMail.IsNullOrEmptyTrimmed(), thisCustomer.EMail, aShipping.EMail);
                    listResources.Add(new GlobalConfig(key, value));

                    key = "phone";
                    value = aShipping.Phone.Trim();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "country";
                    value = aShipping.Country;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "postal";
                    value = CommonLogic.IIF(!aShipping.Plus4.IsNullOrEmptyTrimmed(), String.Format("{0}-{1}", aShipping.PostalCode, aShipping.Plus4), aShipping.PostalCode);
                    listResources.Add(new GlobalConfig(key, value));

                    key = "city";
                    value = aShipping.City;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "state";
                    value = aShipping.State;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "address";
                    value = aShipping.Address1;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "residence-type";
                    value = aShipping.ResidenceType.ToString();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "force-save-credit-info";
                    value = AppLogic.AppConfigBool("ForceCreditCardInfoSaving").ToString().ToLower();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "county";
                    value = aShipping.County;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "has-shippable-components";
                    value = cart.HasShippableComponents().ToString().ToLowerInvariant();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "has-coupon-free-shipping";
                    value = cart.CouponIncludesFreeShipping(thisCustomer.CouponCode).ToString().ToLowerInvariant();
                    listResources.Add(new GlobalConfig(key, value));

                    key = "is-skip-shipping";
                    value = AppLogic.AppConfigBool("SkipShippingOnCheckout").ToString().ToLowerInvariant();
                    listResources.Add(new GlobalConfig(key, value));


                    resources = JSONHelper.Serialize<List<GlobalConfig>>(listResources);
                    listResources.Clear();

                    break;
                case "shipping-method":

                    decimal subTotal = Decimal.Zero;
                    decimal tax = Decimal.Zero;
                    decimal freight = Decimal.Zero;
                    decimal freightTax = Decimal.Zero;
                    decimal total = Decimal.Zero;

                    if (cart.CartItems.Count > 0)
                    {
                        cart.BuildSalesOrderDetails();

                        subTotal = cart.SalesOrderDataset.CustomerSalesOrderView[0].SubTotalRate;
                        tax = cart.SalesOrderDataset.CustomerSalesOrderView[0].TaxRate;
                        freight = cart.SalesOrderDataset.CustomerSalesOrderView[0].FreightRate;
                        freightTax = cart.SalesOrderDataset.CustomerSalesOrderView[0].FreightTaxRate;

                        total = subTotal + tax + freight;
                    }

                    if (cart.ThisCustomer.VATSettingReconciled == VatDefaultSetting.Inclusive)
                    {
                        subTotal = subTotal + tax;
                        subTotal -= freightTax;
                        freight += freightTax;
                    }
                    else
                    {
                        tax -= freightTax;
                    }

                    value = cart.SalesOrderDataset.CustomerSalesOrderView[0].ShippingMethod;
                    listResources.Add(new GlobalConfig("opc.shipping.method", value));

                    value = (freight == Decimal.Zero) ? AppLogic.GetString("shoppingcart.aspx.13", thisCustomer.SkinID, thisCustomer.LocaleSetting) : InterpriseHelper.FormatCurrencyForCustomer(freight, thisCustomer.CurrencyCode);
                    listResources.Add(new GlobalConfig("opc.freight.rate", Server.HtmlEncode(value)));

                    value = InterpriseHelper.FormatCurrencyForCustomer(freightTax, thisCustomer.CurrencyCode);
                    listResources.Add(new GlobalConfig("opc.freight.tax", Server.HtmlEncode(value)));

                    value = InterpriseHelper.FormatCurrencyForCustomer(tax, thisCustomer.CurrencyCode);
                    listResources.Add(new GlobalConfig("opc.tax", Server.HtmlEncode(value)));

                    value = InterpriseHelper.FormatCurrencyForCustomer(subTotal, thisCustomer.CurrencyCode);
                    listResources.Add(new GlobalConfig("opc.sub.total", Server.HtmlEncode(value)));

                    value = InterpriseHelper.FormatCurrencyForCustomer(total, thisCustomer.CurrencyCode);
                    listResources.Add(new GlobalConfig("opc.grand.total", Server.HtmlEncode(value)));

                    resources = JSONHelper.Serialize<List<GlobalConfig>>(listResources);
                    listResources.Clear();

                    break;
                case "payments-info":

                    var aBilling = thisAddress.ThisCustomer.PrimaryBillingAddress;

                    key = "opc-billing-contact-name";
                    value = aBilling.Name;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-email";
                    value = aBilling.EMail;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-phone";
                    value = aBilling.Phone;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-country";
                    value = aBilling.Country;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-postal";
                    value = CommonLogic.IIF(!aBilling.Plus4.IsNullOrEmptyTrimmed(), String.Format("{0}-{1}", aBilling.PostalCode, aBilling.Plus4), aBilling.PostalCode);
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-city";
                    value = aBilling.City;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-state";
                    value = aBilling.State;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-address";
                    value = aBilling.Address1;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-payment-method";
                    value = thisCustomer.PaymentMethod;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-payment-term-code";
                    value = thisCustomer.PaymentTermCode;
                    listResources.Add(new GlobalConfig(key, value));

                    key = "opc-billing-county";
                    value = aBilling.County;
                    listResources.Add(new GlobalConfig(key, value));

                    resources = JSONHelper.Serialize<List<GlobalConfig>>(listResources);
                    listResources.Clear();

                    break;

                default:
                    break;
            }

        }

        return resources;
    }

    #endregion

    #region One Page Checkout Step 1 (Save Shipping Info)

    [WebMethod(EnableSession = true)]
    public string OnePageCheckoutStep1(List<string> profile, List<string> shippingAddress, bool validate, string addressId)
    {
        var thisCustomer = Customer.Current;
        try
        {
            string address = shippingAddress[0].Trim();
            string countryCode = shippingAddress[1].Trim();
            string postalCode = shippingAddress[2].Trim();
            string city = shippingAddress[3].Trim();
            string stateCode = shippingAddress[4].Trim();
            string county = AppLogic.AppConfigBool("Address.ShowCounty")? shippingAddress[6].Trim() : String.Empty;

            string name = profile[0].Trim();
            string email = profile[1].Trim();
            string phone = profile[2].Trim();

            if (validate)
            {
                if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses") && thisCustomer.IsNotRegistered && Customer.EmailInUse(profile[1], Customer.Current.CustomerCode))
                {
                    return INVALID_EMAIL;
                }

                if (InterpriseHelper.IsSearchablePostal(countryCode) && InterpriseHelper.IsCountryHasActivePostal(countryCode))
                {
                    var splitPostal = postalCode.Split('-');
                    if (splitPostal.Length > 0) postalCode = splitPostal[0];

                    if (!InterpriseHelper.IsCorrectAddress(countryCode, postalCode, String.Empty))
                    {
                        return INVALID_POSTAL;
                    }

                    if (InterpriseHelper.IsWithState(countryCode) && !InterpriseHelper.IsCorrectAddress(countryCode, postalCode, stateCode))
                    {
                        return INVALID_STATE;
                    }
                }

                return IS_VALID;
            }
            else
            {
                var aShippingAddress = Address.New(thisCustomer, AddressTypes.Shipping);

                aShippingAddress.AddressID = addressId.IsNullOrEmptyTrimmed() ? thisCustomer.PrimaryShippingAddressID : addressId;
                aShippingAddress.CustomerCode = thisCustomer.CustomerCode;
                aShippingAddress.Address1 = address;
                aShippingAddress.Country = countryCode;
                aShippingAddress.PostalCode = postalCode;
                aShippingAddress.City = city;
                aShippingAddress.State = stateCode;

                if (AppLogic.AppConfigBool("Address.ShowCounty"))
                {
                    aShippingAddress.County = county;
                }

                if (!email.IsNullOrEmptyTrimmed() && thisCustomer.IsNotRegistered)
                {
                    aShippingAddress.EMail = email;
                }
                else
                {
                    aShippingAddress.EMail = thisCustomer.EMail;
                }

                aShippingAddress.Name = name;
                aShippingAddress.Phone = phone;

                aShippingAddress.ResidenceType = InterpriseHelper.ResolveResidenceType(shippingAddress[5]);
                aShippingAddress.Save();

                Address.Update(thisCustomer, aShippingAddress);

                var cart = new InterpriseShoppingCart(null, AppLogic.GetCurrentSkinID(), thisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
                cart.ShipAllItemsToThisAddress(aShippingAddress);

                if (thisCustomer.IsRegistered)
                {
                    InterpriseHelper.MakeDefaultAddress(thisCustomer.ContactCode, aShippingAddress.AddressID, AddressTypes.Shipping);
                }

                AppLogic.SavePostalCode(aShippingAddress);

                return ADDRESS_IS_SAVED;

            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    #endregion

    #region One Page Checkout Step 2 (Save Payments Method)

    [WebMethod]
    public string OnePageCheckoutStep2(string shippingMethod, string freight, string freightCalculation, string realTimeRateGUID)
    {
        Security.AuthenticateService();

        var thisCustomer = Customer.Current;

        try
        {
            thisCustomer.RequireCustomerRecord();
            var cart = new InterpriseShoppingCart(null, AppLogic.GetCurrentSkinID(), thisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

            if (shippingMethod.IsNullOrEmptyTrimmed())
            {
                cart.SetCartShippingMethod(String.Empty);
                return String.Empty;
            }

            if (freightCalculation == "1" || freightCalculation == "2")
            {
                cart.SetCartShippingMethod(shippingMethod, String.Empty, new Guid(realTimeRateGUID));
                cart.SetRealTimeRateRecord(shippingMethod, freight.Trim(new char[] {'$', ' '}), realTimeRateGUID, false);
            }
            else
            {
                cart.SetCartShippingMethod(shippingMethod);
            }

        }
        catch (Exception ex)
        {

            return ex.Message;

        }

        return IS_VALID;
    }

    #endregion

    #region One Page Checkout Step 3 (Validate Billing Info)

    [WebMethod]
    public string IsBillingInfoCorrect(string countryCode, string postalCode, string stateCode, bool isWithRequiredAge)
    {
        try
        {
            #region Validate: Over 13 Requirement

            if (AppLogic.AppConfigBool("RequireOver13Checked") && Customer.Current.IsNotRegistered && !isWithRequiredAge)
            {
                return IS_OVER13_REQUIRED;
            }

            #endregion

            #region Validate: Billing Address

            if (InterpriseHelper.IsSearchablePostal(countryCode) && InterpriseHelper.IsCountryHasActivePostal(countryCode))
            {
                var splitPostal = postalCode.Split('-');
                if (splitPostal.Length > 0) postalCode = splitPostal[0];

                if (!InterpriseHelper.IsCorrectAddress(countryCode, postalCode, String.Empty))
                {
                    return INVALID_POSTAL;
                }

                if (InterpriseHelper.IsWithState(countryCode) && !InterpriseHelper.IsCorrectAddress(countryCode, postalCode, stateCode))
                {
                    return INVALID_STATE;
                }
            }

            #endregion

            return IS_VALID;
        }
        catch (Exception ex) { return ex.Message; }
    }

    #endregion

    #endregion

    #region Customer

    #region Get Registered Customer Address Info

    [WebMethod]
    public string GetCustomerProfile()
    {
        var profile = new StringBuilder();
        bool CustomerIsRegitered = Customer.Current.IsRegistered;

        if (CustomerIsRegitered)
        {

            profile.AppendFormat("{0}::", Customer.Current.Salutation);
            profile.AppendFormat("{0}::", Customer.Current.FirstName);
            profile.AppendFormat("{0}::", Customer.Current.LastName);
            profile.AppendFormat("{0}::", Customer.Current.ContactFullName);
            profile.AppendFormat("{0}::", Customer.Current.EMail);
            profile.AppendFormat("{0}::", Customer.Current.Phone);

            var address = Customer.Current.BillingAddresses;

            profile.AppendFormat("{0}::", address[0].Country);
            profile.AppendFormat("{0}::", address[0].State);
            profile.AppendFormat("{0}::", address[0].PostalCode);
            profile.AppendFormat("{0}::", address[0].City);
            profile.AppendFormat("{0}::", address[0].Address1);

            profile.AppendFormat("{0}::", address[0].ThisCustomer.OKToEMail);
            profile.AppendFormat("{0}::", address[0].ThisCustomer.IsOver13);
        }
        else
        {
            profile.Append(0);
        }

        return profile.ToString();
    }

    #endregion

    #region Update Profile

    [WebMethod(EnableSession = true)]
    public string UpdateCustomerProfile(List<string> profile, string captcha)
    {
        // salutation, firstName, lastName, email, phone, productUpdates, imOver13YearsOfAge, password, oldPassword
        try
        {
            Security.AuthenticateService();

            Customer ThisCustomer = Customer.Current;
            ThisCustomer.RequireCustomerRecord();

            if (profile[0] == AppLogic.GetString("createaccount.aspx.81", AppLogic.GetCurrentSkinID(), Customer.Current.LocaleSetting)) profile[0] = string.Empty;

            string salutation = profile[0];
            string firstName = profile[1];
            string lastName = profile[2];
            string email = profile[3];
            string phone = profile[4];
            string productUpdates = profile[5];
            string imOver13 = profile[6];
            string newPassword = profile[7];
            string oldPassword = profile[8];

            bool changePass = false;

            if (oldPassword != string.Empty && newPassword != string.Empty)
            {
                Customer customerWithValidLogin = Customer.FindByLogin(ThisCustomer.EMail, oldPassword);

                if (customerWithValidLogin == null)
                {


                    return "false::Change of password failed, verify your old password if correct.";

                }

                changePass = true;

            }

            if (!AppLogic.AppConfigBool("AllowCustomerDuplicateEMailAddresses") && ThisCustomer.EMail != email)
            {

                if (Customer.EmailInUse(email, Customer.Current.CustomerCode))
                {
                    return String.Format("false::{0}", AppLogic.GetString("createaccount.aspx.94", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
                }

            }

            if (AppLogic.AppConfigBool("SecurityCodeRequiredOnCreateAccount"))
            {
                if (Session["SecurityCode"] != null)
                {
                    string sCode = Session["SecurityCode"].ToString();
                    string fCode = captcha;
                    bool codeMatch = false;

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

                        return "false::Security code doesn't match";
                    }
                }
            }

            bool Over13Checked = false;
            if (imOver13 == "yes") Over13Checked = true;

            bool OkToEmailChecked = false;
            if (productUpdates == "yes") OkToEmailChecked = true;

            ThisCustomer.Salutation = salutation;
            ThisCustomer.EMail = email;

            if (changePass)
            {
                ThisCustomer.Password = newPassword;

            }
            else
            {
                ThisCustomer.Password = AppLogic.PasswordValuePlaceHolder;
            }

            ThisCustomer.FirstName = firstName;
            ThisCustomer.LastName = lastName;
            ThisCustomer.Phone = phone;
            ThisCustomer.IsOver13 = Over13Checked;
            ThisCustomer.IsOKToEMail = OkToEmailChecked;

            ThisCustomer.Update();

            return "true::updated";
        }
        catch (Exception ex)
        {
            return string.Format("false::{0}", ex.Message);
        }
    }

    #endregion

    [WebMethod(EnableSession = true)]
    public List<string> GetCustomerShipTo(string ShipToCode)
    {
        Security.AuthenticateService();
        return AppLogic.GetCustomerShipTo(ShipToCode);
    }

    #endregion

    #region Matrix Group Items

    [WebMethod]
    public string GetMatrixGroupItems(string itemCode, int pageSize, int pageNumber, string imageSize)
    {
        var items = new List<MatrixGroupItems>();
        items = MatrixGroupItems.GetMatrixItems(itemCode, pageSize, pageNumber, imageSize);

        string jsonValue = JSONHelper.Serialize<List<MatrixGroupItems>>(items);
        return jsonValue;
    }

    #endregion

    #region Item Popup
    [WebMethod, ScriptMethod]
    public void AddToCartEx(string counter, decimal quantity, string kitcomposition, string unitmeasure)
    {
        Customer thisCustomer = Customer.Current;
        InterpriseShoppingCart cart = new InterpriseShoppingCart(null, thisCustomer.SkinID, thisCustomer, CartTypeEnum.ShoppingCart, string.Empty, false, true);
        string itemCode = InterpriseHelper.GetInventoryItemCode(Convert.ToInt32(counter));
        string shippingAddressID = CommonLogic.IIF(thisCustomer.IsNotRegistered, string.Empty, thisCustomer.PrimaryShippingAddressID);

        var umInfo = (unitmeasure == string.Empty) ? InterpriseHelper.GetItemDefaultUnitMeasure(itemCode) : InterpriseHelper.GetItemUnitMeasure(itemCode, unitmeasure);
        var kitItemsComposition = KitComposition.FromComposition(kitcomposition, thisCustomer, CartTypeEnum.ShoppingCart, itemCode);
        cart.AddItem(thisCustomer, shippingAddressID, itemCode, Convert.ToInt32(counter), quantity, umInfo.Code, CartTypeEnum.ShoppingCart, kitItemsComposition);
    }

    [WebMethod, ScriptMethod]
    public string GetItemPopup(string itemCode)
    {
        return AppLogic.GetItemPopup(itemCode);
    }

    [WebMethod, ScriptMethod]
    public string GetMatrixItemDetails(int itemCounter, string itemCode, string matrixCombination)
    {
        var matrixItems = MatrixItemData.GetMatrixItems(itemCounter, itemCode, false);
        var selectedAttributes = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<AttributeData>>(matrixCombination);
        var itemcode = string.Empty;
        foreach(var matrixItem in matrixItems)
        {
            int attribFound = 0;
            bool exists = false;
            for(int i = 0; i < selectedAttributes.Count; i++)
            {
                exists = matrixItem.Attributes.Exists(attrib => attrib.Code == selectedAttributes[i].Code && attrib.Value == selectedAttributes[i].Value);
                if (exists) { attribFound++; }
            }
            if (attribFound == selectedAttributes.Count) {itemcode = matrixItem.ItemCode; break; }
        }

        if (itemcode != string.Empty)
        {
            var settings = ItemWebOption.GetWebOption(itemcode);
            var itemInfo = new MatrixItemInfo();
            itemInfo.HidePriceUntilCart = settings.HidePriceUntilCart;
            itemInfo.IsCallToOrder = settings.IsCallToOrder;
            itemInfo.ItemCounter = settings.ItemCounter;
            itemInfo.ItemCode = itemcode;
            itemInfo.MinimumOrderQuantity = settings.MinimumOrderQuantity;
            itemInfo.RequiresRegistration = settings.RequiresRegistration;
            itemInfo.RestrictedQuantities = settings.RestrictedQuantities;
            itemInfo.ShowBuyButton = settings.ShowBuyButton;
            itemInfo.UnitMeasures = ProductPricePerUnitMeasure.GetAll(itemcode, Customer.Current, settings.HidePriceUntilCart, true);
            return JSONHelper.Serialize<MatrixItemInfo>(itemInfo);
        }
        return "";
    }

    [WebMethod, ScriptMethod]
    public string GetItemReviews(string itemCode, int sort)
    {
        return AppLogic.GetItemReviews(itemCode, sort);
    }

    [WebMethod]
    public string GetItemImage(string itemCode)
    {
        return AppLogic.GetProductImage(itemCode);
    }

    [WebMethod]
    public void CreateUpdateItemReview(string itemCode, int rating, string comment)
    {
        comment = comment.ToHtmlEncode();
        if (AppLogic.HasItemReview(itemCode)) { AppLogic.UpdateItemReview(itemCode, rating, comment); }
        else { AppLogic.CreateItemReview(itemCode, rating, comment); }
    }

    [WebMethod]
    public void VoteItemReview(string itemCode, string voterCustomerCode, string voterContactCode, string vote, string customerCode, string contactCode)
    {
        AppLogic.VoteItemReview(itemCode, voterCustomerCode, voterContactCode, vote, customerCode, contactCode);
    }

    [WebMethod]
    public bool NotifyOnPriceDrop(string itemcode)
    {
        return AppLogic.ProductNotification(itemcode, 1);
    }

    [WebMethod]
    public bool NotifyOnAvailability(string itemcode)
    {
        return AppLogic.ProductNotification(itemcode, 0);
    }
    #endregion

    #region StoreLocator

    [WebMethod]
    public string GetWarehouseByAddress(int storeTypeCode, string longtitude, string latitude, string distance)
    {
        var selStoreType = (StoreType)storeTypeCode;
        var systeWarehouses = StoreLocatorDA.GetDealersAndWarehouses(selStoreType);

        double inputtedDisctance = double.Parse(distance);
        double radius = 6371;
        double radiance = 3.1459 / 180;
        double inMiles = 0.621371192;

        string json = systeWarehouses.Where(w =>
        {
            double lat1 = double.Parse(latitude);
            double lat2 = (double)w.Coordinate.Latitude;

            double lon1 = double.Parse(longtitude);
            double lon2 = (double)w.Coordinate.Longtitude;

            var dLat = (lat2 - lat1) * radiance;
            var dLon = (lon2 - lon1) * radiance;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * radiance) *
                       Math.Cos(lat2 * radiance) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = radius * c * inMiles;

            w.Distance = d;

            return (d <= inputtedDisctance);
        })
        .OrderBy(sw => sw.Distance)
        .ToList()
        .ToJSON();

        return json;
    }

    #endregion

    #region CMS Editor

    [WebMethod]
    public string UpdateStringResourceConfigValue(string contentKey, string contentValue)
    {
        try
        {
            Security.AuthenticateService();

            if (Security.IsAdminCurrentlyLoggedIn())
            {
                AppLogic.UpdateStringResourceConfigValue(contentKey, contentValue);

            }
            else
            {
                return AppLogic.GetString("signin.aspx.20", Customer.Current.SkinID, Customer.Current.LocaleSetting, true);
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return String.Empty;
    }

    [WebMethod]
    public bool IsPageEditMode()
    {
        return Security.IsAdminCurrentlyLoggedIn() && Customer.Current.IsInEditingMode();
    }

    [WebMethod]
    public void TogglePageEditMode(bool mode)
    {
        Customer.Current.ThisCustomerSession[DomainConstants.CMS_ENABLE_EDITMODE] = mode.ToString();

        //reset top menu to reset caching
        ApplicationCachingEngine.Reset(DomainConstants.TOP_MENU_CACHE_NAME + "_" + InterpriseHelper.ConfigInstance.WebSiteCode);
    }

    [WebMethod]
    public bool UpdateTopicFromEditor(string topicId, string htmlContent)
    {
        try
        {
            Security.AuthenticateService();

            if (!Security.IsAdminCurrentlyLoggedIn() || !Customer.Current.IsInEditingMode()) return false;

            try
            {
                return ResourcesDA.UpdateTopic(topicId, htmlContent, InterpriseHelper.ConfigInstance.WebSiteCode, Customer.Current.LanguageCode);
            }
            catch (Exception)
            {
                throw;
            }

        }
        catch
        {
            throw;
        }
    }

    [WebMethod]
    public bool UpdateItemDescriptionFromEditor(string contentKey, string contentValue, string contentType)
    {
        try
        {
            Security.AuthenticateService();

            if (!Security.IsAdminCurrentlyLoggedIn() || !Customer.Current.IsInEditingMode()) return false;

            try
            {
                return ResourcesDA.UpdateItemDescription(contentKey, contentValue, contentType, InterpriseHelper.ConfigInstance.WebSiteCode, Customer.Current.LanguageCode);
            }
            catch (Exception)
            {
                throw;
            }

        }
        catch
        {
            throw;
        }

    }

    [WebMethod]
    public string GetProductImageData(int counter, string itemCode, string itemType, int matrixGroupCounter)
    {
        string jsonValue = String.Empty;

        if (itemType == "product")
        {
            var imgData = ProductImageData.GetForImageUpload(counter, itemCode, itemType, matrixGroupCounter);
            jsonValue = imgData.Serialize(true);
            imgData = null;
        }
        else
        {
            bool exist = false;
            string imgMobileUrl = String.Empty;

            foreach (var ext in DomainConstants.GetImageSupportedExtensions())
            {
                imgMobileUrl = AppLogic.GetMobileImagePath(itemType, "mobile", true) + counter + "." + ext;
                if (!System.IO.File.Exists(imgMobileUrl)) continue;

                imgMobileUrl = AppLogic.GetMobileImagePath(itemType, "mobile", false) + counter + "." + ext;
                exist = true;
                break;
            }

            if (!exist)
            {
                imgMobileUrl = "mobile/images/nopictureicon.gif";
            }

            string imagUrlLarge = AppLogic.LocateImageUrl(itemType, counter, "large");
            string imagUrlMedium = AppLogic.LocateImageUrl(itemType, counter, "medium");
            string imagUrlIcon = AppLogic.LocateImageUrl(itemType, counter, "icon");

            var entityImageHeader = new EntityImageHeader()
            {
                ID = counter,
                Thumbnail = new EntityImageDetail()
                {
                    src = imagUrlIcon,
                    exists = !(imagUrlIcon.Contains("skins/") || imagUrlIcon.Contains("nopicture")),
                    ImgFileName = imagUrlIcon.Substring(imagUrlIcon.LastIndexOf("/") + 1)
                },
                Medium = new EntityImageDetail()
                {
                    src = imagUrlMedium,
                    exists = !(imagUrlMedium.Contains("skins/") || imagUrlMedium.Contains("nopicture")),
                    ImgFileName = imagUrlMedium.Substring(imagUrlMedium.LastIndexOf("/") + 1)
                },
                Large = new EntityImageDetail()
                {
                    src = imagUrlLarge,
                    exists = !(imagUrlLarge.Contains("skins/") || imagUrlLarge.Contains("nopicture")),
                    ImgFileName = imagUrlLarge.Substring(imagUrlLarge.LastIndexOf("/") + 1)
                },
                Mobile = new EntityImageDetail()
                {
                    src = imgMobileUrl,
                    exists = exist,
                    ImgFileName = imgMobileUrl.Substring(imgMobileUrl.LastIndexOf("/") + 1)
                }
            };

            jsonValue = entityImageHeader.ToJSON();
            entityImageHeader = null;
        }

        return jsonValue;
    }

    [WebMethod]
    public bool ImageUploadSetAsImageDefault(string itemCode, string fileName, string size)
    {
        Security.AuthenticateService();

        return ProductDA.UpdateDefaultImageSize(itemCode, fileName, size, InterpriseHelper.ConfigInstance.WebSiteCode);
    }

    #endregion

    #region Dashboard

    [WebMethod]
    public string GetNewCustomers(int displayLimit)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        return CustomerDA.GetCustomers()
                         .OrderByDescending(c => c.DateRegistered)
                         .Take(displayLimit)
                         .ToList()
                         .ToJSON();
    }

    [WebMethod]
    public string GetProductsLowInFreeStock(int threshold, int displayLimit, bool isActiveOnly)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }
        string filter = String.Empty;
        if (isActiveOnly)
        {
            filter = " (Status = 'A' OR (Status = 'P' AND FreeStock > 0)) AND  FreeStock <= {0} ".FormatWith(threshold.ToString());
        }
        else
        {
            filter = " FreeStock <= {0} ".FormatWith(threshold.ToString());
        }

        return ProductDA.GetProductsStockTotal(Customer.Current.LanguageCode, filter)
                        .OrderBy(item => item.StockTotal.FreeStock)
                        .Take(displayLimit)
                        .ToList()
                        .ToJSON();
    }

    [WebMethod]
    public string GetStoreSettings(List<string> keys)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        var configs = new List<GlobalConfig>();

        foreach (string key in keys)
        {
            string value = AppLogic.AppConfig(key);
            configs.Add(new GlobalConfig(key, value.ToLowerInvariant()));
        }

        return configs.ToList().ToJSON();
    }

    [WebMethod]
    public string GetWebRecentOrders(int displayLimit)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        return CustomerDA.GetWebSalesOrders()
                         .OrderByDescending(s => s.SalesOrderDate)
                         .Take(displayLimit)
                         .ToList()
                         .ToJSON();
    }

    [WebMethod]
    public string GetWebStats(DateRangeType rangeType)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        return InterpriseSuiteEcommerceCommon.Integration.Interprise.admin.Dashboard.GetWebStats(rangeType)
                                                                                    .ToList()
                                                                                    .ToJSON();
    }

    [WebMethod]
    public string GetWebSales(DateRangeType rangeType, string dateFrom, string dateTo)
    {
        if (!Security.IsAdminCurrentlyLoggedIn()) { return String.Empty; }

        var sales = CustomerDA.GetWebInvoice().Where(r => r.InvoiceDate.Between(Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateTo)));

        if (rangeType == DateRangeType.Date)
        {
            return sales.GroupBy(date => date.InvoiceDate)
                        .Select(invoice => new CustomerSalesParam { Total = invoice.Sum(t => t.Total), Dimension = invoice.Key.ToString("yyyyMMdd") })
                        .OrderBy(d => d.Dimension)
                        .ToList()
                        .ToJSON();
        }

        if (rangeType == DateRangeType.Week)
        {
            return sales.GroupBy(week => week.InvoiceDate.Date.DayOfYear / 7)
                        .Select(invoice => new CustomerSalesParam { Total = invoice.Sum(t => t.Total), Dimension = (invoice.Key + 1).ToString() })
                        .OrderBy(d => Convert.ToInt32(d.Dimension))
                        .ToList()
                        .ToJSON();
        }

        if (rangeType == DateRangeType.Month)
        {
            return sales.GroupBy(month => new { month.InvoiceDate.Year, month.InvoiceDate.Month })
                        .Select(invoice => new CustomerSalesParam { Total = invoice.Sum(t => t.Total), Dimension = invoice.Key.Month.ToString() })
                        .OrderBy(d => d.Dimension)
                        .ToList()
                        .ToJSON();
        }

        if (rangeType == DateRangeType.Year)
        {
            return sales.GroupBy(year => year.InvoiceDate.Year)
                        .Select(invoice => new CustomerSalesParam { Total = invoice.Sum(t => t.Total), Dimension = invoice.Key.ToString() })
                        .OrderBy(d => d.Dimension)
                        .ToList()
                        .ToJSON();
        }
        return String.Empty;
    }

    private const string DATA_FEED_URL = "https://www.google.com/analytics/feeds/data";
    private const string ACCOUNT_FEED_URL = "https://www.googleapis.com/analytics/v2.4/management/accounts";

    [WebMethod]
    public string GetWebVisitors(string dimension, string dateFrom, string dateTo)
    {
        try
        {
            string apiKey = AppLogic.AppConfig("GoogleAnalytics.APIKey");
            string webPropertyUrl = String.Empty;
            string profileFeedUrl = String.Empty;
            string profileUrl = String.Empty;

            var service = new Google.GData.Analytics.AnalyticsService("ConnectedBusiness");
            service.setUserCredentials(AppLogic.AppConfig("GoogleAnalytics.Username"), AppLogic.AppConfig("GoogleAnalytics.Password"));

            var accountsFeed = service.Query(new Google.GData.Analytics.DataQuery("{0}?key={1}".FormatWith(ACCOUNT_FEED_URL, apiKey)));
            webPropertyUrl = accountsFeed.Entries.First().Links[1].HRef.Content;

            var webPropertiesFeed = service.Query(new Google.GData.Analytics.DataQuery(webPropertyUrl));
            profileFeedUrl = webPropertiesFeed.Entries.First().Links[2].HRef.Content;

            var profileFeed = service.Query(new Google.GData.Analytics.DataQuery(profileFeedUrl));
            profileUrl = profileFeed.Entries.First().Links[0].HRef.Content;

            var profiles = profileUrl.Split('/');
            string profileID = profiles[profiles.Length - 1];
            var query = new Google.GData.Analytics.DataQuery()
            {
                Query = "{0}?key={1}".FormatWith(DATA_FEED_URL, apiKey),
                Ids = "ga:" + profileID,
                Metrics = "ga:visits",
                Dimensions = "ga:" + dimension.ToLower(),
                GAStartDate = dateFrom,
                GAEndDate = dateTo
            };

            var visitsFeed = service.Query(query);
            var result = new List<GoogleAnalytics>();
            result.AddRange(visitsFeed.Entries.OfType<Google.GData.Analytics.DataEntry>()
                                                .Select(entry => new GoogleAnalytics()
                                                {
                                                    Dimension = entry.Title.Text.Split('=')[1],
                                                    Visits = Convert.ToInt32(entry.Metrics[0].Value)
                                                }));
            return result.ToList().ToJSON();
        }
        catch
        {
            return String.Empty;
        }
    }
    #endregion

    #region UPS/FedEx Address Verification

    [WebMethod]
    public string GetAddressBestMatch(string address, string country, string postal, string city, string state, string residenceType, bool billingANDshipping)
    {
        string key = String.Empty;
        string value;

        bool isResidenceType = false;

        if (residenceType == ResidenceTypes.Residential.ToString() || residenceType == "default-type") isResidenceType = true;

        var listBestMatchAddress = new List<GlobalConfig>();

        try
        {
            if (!billingANDshipping)
            {
                List<string> bestMatch = AppLogic.GetAddressMatch(address, country, postal, city, state, isResidenceType);

                key = "match-address";
                value = bestMatch[0];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-postal";
                value = bestMatch[1];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-city";
                value = bestMatch[2];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-state";
                value = bestMatch[3];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-country";
                value = bestMatch[4];

                listBestMatchAddress.Add(new GlobalConfig(key, value));

            }
            else
            {

                var cAddress = address.Split('+');
                var cCountry = country.Split('+');
                var cPostal = postal.Split('+');
                var cCity = city.Split('+');
                var cState = state.Split('+');
                var cResidenceType = residenceType.Split('+');

                List<string> billingBestMatch = AppLogic.GetAddressMatch(cAddress[0], cCountry[0], cPostal[0], cCity[0], cState[0], isResidenceType);

                key = "match-billing-address";
                value = billingBestMatch[0];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-billing-postal";
                value = billingBestMatch[1];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-billing-city";
                value = billingBestMatch[2];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-billing-state";
                value = billingBestMatch[3];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-billing-country";
                value = billingBestMatch[4];

                listBestMatchAddress.Add(new GlobalConfig(key, value));

                List<string> shippingBestMatch = AppLogic.GetAddressMatch(cAddress[1], cCountry[1], cPostal[1], cCity[1], cState[1], isResidenceType);

                key = "match-shipping-address";
                value = shippingBestMatch[0];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-shipping-postal";
                value = shippingBestMatch[1];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-shipping-city";
                value = shippingBestMatch[2];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-shipping-state";
                value = shippingBestMatch[3];
                listBestMatchAddress.Add(new GlobalConfig(key, value));

                key = "match-shipping-country";
                value = shippingBestMatch[4];

                listBestMatchAddress.Add(new GlobalConfig(key, value));

            }

        }
        catch (Exception ex)
        {
            return String.Format("exception[error]{0}", ex.Message);
        }

        string jsonValue = JSONHelper.Serialize<List<GlobalConfig>>(listBestMatchAddress);
        return jsonValue;

    }

    #endregion

    #region Required String Resources and App Config for Address Verification

    [WebMethod]
    public string GetStringResources(List<string> keys)
    {
        var stringResources = new List<GlobalConfig>();

        string value;

        foreach (string key in keys)
        {
            value = AppLogic.GetString(key, AppLogic.GetCurrentSkinID(), Customer.Current.LocaleSetting);
            stringResources.Add(new GlobalConfig(key, value));
        }

        string jsonValue = JSONHelper.Serialize<List<GlobalConfig>>(stringResources);
        return jsonValue;
    }

    [WebMethod]
    public string GetAppConfigs(string configsFor)
    {
        var listResources = new List<GlobalConfig>();

        string key = String.Empty;
        string value;
        if (configsFor == "create-account" || configsFor == "edit-profile") configsFor = "customer-account";

        switch (configsFor)
        {
            case "customer-account":

                key = "UseStrongPwd";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "RequireOver13Checked";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "AllowShipToDifferentThanBillTo";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "AllowCustomerDuplicateEMailAddresses";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "VAT.ShowTaxFieldOnRegistration";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "VAT.Enabled";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));


                key = "CustomerPwdValidator";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "SecurityCodeRequiredOnCreateAccount";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "PasswordMinLength";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "UseShippingAddressVerification";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                break;
            case "one-page-checkout":


                key = "VAT.ShowTaxFieldOnRegistration";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "RequireOver13Checked";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "AllowShipToDifferentThanBillTo";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "AllowCustomerDuplicateEMailAddresses";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "VAT.Enabled";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "UseShippingAddressVerification";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                key = "RequireTermsAndConditionsAtCheckout";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                break;
            case "address":

                key = "UseShippingAddressVerification";
                value = AppLogic.AppConfig(key);
                listResources.Add(new GlobalConfig(key, value.ToLowerInvariant()));

                break;
            default: break;
        }

        string jsonValue = JSONHelper.Serialize<List<GlobalConfig>>(listResources);
        return jsonValue;

    }

    #endregion

    #region Address

    [WebMethod]
    public string GetAddressList(string countryCode, string postalCode, string stateCode, string searchString, bool exactMatch, int pageNumber)
    {
        if (searchString.IsNullOrEmptyTrimmed())
        {
            searchString = postalCode;
        }

        return AppLogic.RenderPostalCodeListing(exactMatch, postalCode, stateCode, countryCode, pageNumber, searchString);
    }

    [WebMethod]
    public string GetCity(string countryCode, string postalCode, string stateCode)
    {
        if (countryCode.IsNullOrEmptyTrimmed())
        {
            return String.Empty;
        }

        if (!InterpriseHelper.IsCountryHasActivePostal(countryCode))
        {
            return NO_ACTIVE_POSTAL;
        }

        if (!InterpriseHelper.IsWithState(countryCode))
        {
            stateCode = String.Empty;
        }

        return InterpriseHelper.GetCity(countryCode, postalCode, stateCode);
    }

    [WebMethod]
    public bool IsStateCodeValid(string countryCode, string postalCode, string stateCode)
    {

        if (!InterpriseHelper.IsSearchablePostal(countryCode))
        {
            return true;
        }

        return InterpriseHelper.IsCorrectAddress(countryCode, postalCode, stateCode);
    }

    [WebMethod]
    public bool IsPostalCodeValid(string countryCode, string postalCode)
    {
        if (!InterpriseHelper.IsCountryHasActivePostal(countryCode)) return true;
        return InterpriseHelper.IsCorrectAddress(countryCode, postalCode, String.Empty);
    }

    #endregion

    [WebMethod]
    public decimal GetInventoryFreeStock(string itemCode, string unitMeasureCode)
    {
        return InterpriseHelper.GetInventoryFreeStock(itemCode, unitMeasureCode, Customer.Current);
    }

    [WebMethod]
    public string Version()
    {
        return InterpriseHelper.GetWebVersionInformation();
    }

    [WebMethod]
    public string SyncImages(int totalImages, int currentImageRow, string syncType)
    {
        var syncTypeEnum = syncType.TryParseEnum<ImageSyncType>();

        //Access the ImagePerBatch to adjust the batch transferring
        //ImageSynchronizer.ImagePerBatch = 30;
        return ImageSynchronizer.SynchronizeImages(new CustomFileUploadJson() { TotalImages = totalImages, CurrentImageRow = currentImageRow }, syncTypeEnum);
    }

    [WebMethod]
    public string GetItemImages(string itemCode)
    {
        var images = ServiceFactory.GetInstance<IProductService>().GetItemImages(itemCode).OrderBy(i => i.ImageIndex).ToList();
        return ServiceFactory.GetInstance<ICryptographyService>().SerializeToJson(images);
    }
}