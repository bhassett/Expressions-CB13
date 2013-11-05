// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for tableorder_process.
    /// </summary>
    public partial class tableorder_process : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;
            ThisCustomer.RequireCustomerRecord();

            InterpriseShoppingCart cart = new InterpriseShoppingCart(null, 1, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false,true);

            bool redirectToWishList = false;

            foreach (string key in Request.Form.AllKeys)
            {
                try
                {
                    if (!key.StartsWith("ProductID")) { continue; }

                    // retrieve the item counter
                    // This may look obvious 4 but we want to make it expressive
                    string itemCounterValue = Request.Form[key];
                    string quantityOrderedValue = Request.Form["Quantity"];

                        if (string.IsNullOrEmpty(quantityOrderedValue))
                        {
                            quantityOrderedValue = Request.Form["Quantity_" + itemCounterValue];

                            if (!string.IsNullOrEmpty(quantityOrderedValue))
                            {
                                quantityOrderedValue = quantityOrderedValue.Split(',')[0];
                            }
                        }

                        int counter = 0;
                        int quantityOrdered = 0;
                        if (!string.IsNullOrEmpty(itemCounterValue) && 
                            int.TryParse(itemCounterValue, out counter) && 
                            !string.IsNullOrEmpty(quantityOrderedValue) && 
                            int.TryParse(quantityOrderedValue, out quantityOrdered) && 
                            quantityOrdered > 0)
                        {
                            string unitMeasureFieldKey = "UnitMeasureCode_" + counter.ToString();
                            bool useDefaultUnitMeasure = string.IsNullOrEmpty(Request.Form[unitMeasureFieldKey]);

                            string isWishListFieldKey = "IsWishList_" + counter.ToString();
                            bool isWishList = !string.IsNullOrEmpty(Request.Form[isWishListFieldKey]);
                            redirectToWishList = isWishList;

                            // we've got a valid counter
                            string itemCode = string.Empty;

                            using (var con = DB.NewSqlConnection())
                            {
                                con.Open();
                                using (var reader = DB.GetRSFormat(con, "SELECT ItemCode FROM InventoryItem with (NOLOCK) WHERE Counter = {0}", counter))
                                {
                                    if (reader.Read())
                                    {
                                        itemCode = DB.RSField(reader, "ItemCode");
                                    }
                                }
                            }

                            if(!string.IsNullOrEmpty(itemCode))
                            {
                                UnitMeasureInfo? umInfo = null;
                            
                                if(!useDefaultUnitMeasure)
                                {
                                    umInfo = InterpriseHelper.GetItemUnitMeasure(itemCode, Request.Form[unitMeasureFieldKey]);
                                }

                                if(null == umInfo && useDefaultUnitMeasure)
                                {
                                    umInfo = InterpriseHelper.GetItemDefaultUnitMeasure(itemCode);
                                }

                            if (null != umInfo && umInfo.HasValue)
                            {
                                if (isWishList)
                                {
                                    cart.CartType = CartTypeEnum.WishCart;
                                }
                                cart.AddItem(ThisCustomer, ThisCustomer.PrimaryShippingAddressID, itemCode, counter, quantityOrdered, umInfo.Value.Code, CartTypeEnum.ShoppingCart); //, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, CartTypeEnum.ShoppingCart, false, false, string.Empty, decimal.Zero);
                            }
                        }
                    }
                }
                catch
                {
                    // do nothing, add the items that we can
                }
            }

           if (redirectToWishList)
            {
                Response.Redirect("WishList.aspx");
            }
            else
            {
                Response.Redirect("ShoppingCart.aspx?add=true");
            }
          }
    }
}

