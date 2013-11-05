// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceControls;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for checkoutshippingmult.
    /// </summary>
    public partial class checkoutshippingmult : SkinBase
    {
        #region Variable Declaration

        private InterpriseShoppingCart _cart = null;
        private List<CountryAddressDTO> _countries = null;
        private bool shouldRegisterAddressCountries = true;

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
            InitControlText();
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

            if (ThisCustomer.IsCreditOnHold) 
            {
                Response.Redirect("shoppingcart.aspx");
            }

            if (AppLogic.AppConfigBool("RequireOver13Checked") && !ThisCustomer.IsOver13)
            {
                Response.Redirect("shoppingcart.aspx?errormsg=" + AppLogic.GetString("checkout.over13required", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true).ToUrlEncode());
            }

            if (ThisCustomer.IsNotRegistered && !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout"))
            {
                Response.Redirect("createaccount.aspx?checkout=true");
            }

            if ((ThisCustomer.PrimaryBillingAddress == null || ThisCustomer.PrimaryShippingAddress == null) &&
                (ThisCustomer.PrimaryBillingAddressID.IsNullOrEmptyTrimmed() || ThisCustomer.PrimaryShippingAddressID.IsNullOrEmptyTrimmed()))
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("checkoutpayment.aspx.1", SkinID, ThisCustomer.LocaleSetting, true).ToUrlEncode());
            }

            SectionTitle = AppLogic.GetString("checkoutshippingmult.aspx.1", SkinID, ThisCustomer.LocaleSetting, true);

            if (_cart.IsEmpty())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1");
            }

            if (_cart.InventoryTrimmed)
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("shoppingcart.aspx.1", SkinID, ThisCustomer.LocaleSetting, true).ToUrlEncode());
            }

            if (_cart.HasRegistryItemButParentRegistryIsRemoved() || _cart.HasRegistryItemsRemovedFromRegistry())
            {
                _cart.RemoveRegistryItemsHasDeletedRegistry();
                _cart.RemoveRegistryItemsHasBeenDeletedInRegistry();
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("editgiftregistry.error.18", SkinID, ThisCustomer.LocaleSetting, true).ToUrlEncode());
            }

            if (_cart.HasRegistryItemsAndOneOrMoreItemsHasZeroInNeed())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("editgiftregistry.error.15", SkinID, ThisCustomer.LocaleSetting, true).ToUrlEncode());
            }

            if (_cart.HasRegistryItemsAndOneOrMoreItemsExceedsToTheInNeedQuantity())
            {
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + AppLogic.GetString("editgiftregistry.error.14", SkinID, ThisCustomer.LocaleSetting, true).ToUrlEncode());
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

            if ((_cart.IsNoShippingRequired() || !Shipping.MultiShipEnabled() || _cart.NumItems() == 1 || _cart.NumItems() > AppLogic.MultiShipMaxNumItemsAllowed()) && !_cart.HasRegistryItems())
            {
                // not allowed then:
                Response.Redirect("shoppingcart.aspx?resetlinkback=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult.aspx.3", SkinID, ThisCustomer.LocaleSetting, true)));
            }

            if (ThisCustomer.PrimaryShippingAddress == null ||
                CommonLogic.IsStringNullOrEmpty(ThisCustomer.PrimaryShippingAddress.AddressID))
            {
                // not allowed here anymore!
                Response.Redirect("shoppingcart.aspx?errormsg=" + Server.UrlEncode(AppLogic.GetString("checkoutshippingmult.aspx.2", SkinID, ThisCustomer.LocaleSetting, true)));
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
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[0]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.3", SkinID, ThisCustomer.LocaleSetting, true);
            ((RectangleHotSpot)checkoutheadergraphic.HotSpots[1]).AlternateText = AppLogic.GetString("checkoutshipping.aspx.4", SkinID, ThisCustomer.LocaleSetting, true);
        }

        private void InitializeCartRepeaterControl()
        {
            rptCartItems.ItemDataBound += new RepeaterItemEventHandler(rptCartItems_ItemDataBound);
            InitializeDataSource();
        }

        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                bool isCartItem = (e.Item.DataItem is CartItem);
                if(!isCartItem) return;

                var item = e.Item.DataItemAs<CartItem>();

                var trHeader = new TableRow();
                var converter = new WebColorConverter();
                trHeader.BackColor = (Color)converter.ConvertFrom("#" + AppLogic.AppConfig("LightCellColor"));
                // headers..

                var tdItemNameHeader = new TableCell();
                tdItemNameHeader.Width = Unit.Percentage(30);
                var lblItemNameHeader = new Label();
                lblItemNameHeader.Text = string.Format("<b>{0}</b>", AppLogic.GetString("shoppingcart.cs.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                tdItemNameHeader.Controls.Add(lblItemNameHeader);

                trHeader.Cells.Add(tdItemNameHeader);

                var tdShipHeader = new TableCell();
                tdShipHeader.Width = Unit.Percentage(70);
                var lblShipHeader = new Label();
                lblShipHeader.Text = string.Format("<b>{0}</b>", AppLogic.GetString("shoppingcart.cs.24", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                tdShipHeader.Controls.Add(lblShipHeader);

                trHeader.Cells.Add(tdShipHeader);
                e.Item.Controls.Add(trHeader);

                // details
                var trDetail = new TableRow();
                var tdDetailCaption = new TableCell()
                {
                    Width = Unit.Percentage(30),
                    VerticalAlign = VerticalAlign.Top,
                };
                trDetail.Cells.Add(tdDetailCaption);
    
                var lblItemName = new Label()
                {
                    Text = item.DisplayName
                };
                tdDetailCaption.Controls.Add(lblItemName);

        
                var tdDetailAddNew = new TableCell()
                {
                    Width = Unit.Percentage(70),
                    VerticalAlign = VerticalAlign.Top
                };
                trDetail.Cells.Add(tdDetailAddNew);

                e.Item.Controls.Add(trDetail);
       
                if (item.IsDownload || item.IsService)
                {
                    var lblNoShippingRequired = new Label() { Text = AppLogic.GetString("checkoutshippingmult.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) };
                    tdDetailAddNew.Controls.Add(lblNoShippingRequired);
                    tdDetailAddNew.Controls.Add(new LiteralControl("<br />"));
                    tdDetailAddNew.Controls.Add(new LiteralControl("<br />"));
                }
                else
                {
                    var ctrlAddressSelector = new AddressSelectorControl() { ID = "ctrlAddressSelector" };

                    var availableAddresses = new List<Address>();
                    availableAddresses.AddRange(ThisCustomer.ShippingAddresses);

                    bool shouldNotContainingTheSameAddressId = !ThisCustomer.ShippingAddresses.Any(addressItem => addressItem.AddressID == item.m_ShippingAddressID && !item.GiftRegistryID.HasValue);
                    if (item.GiftRegistryID.HasValue && shouldNotContainingTheSameAddressId)
                    {
                        var registryBillingAddress = ThisCustomer.GetRegistryItemShippingAddress(item.m_ShippingAddressID, item.GiftRegistryID);
                        availableAddresses.Add(registryBillingAddress);
                        availableAddresses.Reverse();
                    }

                    ctrlAddressSelector.AddressesDataSource = availableAddresses;
                    ctrlAddressSelector.SelectedAddressID = item.m_ShippingAddressID;
                    tdDetailAddNew.Controls.Add(ctrlAddressSelector);

                    var script = new StringBuilder();

                    script.Append("<script type=\"text/javascript\" language=\"Javascript\" >\n");
                    script.Append("$add_windowLoad(\n");                
                    script.Append(" function() { \n");

                    script.AppendFormat("   var row = new ise.Controls.CheckOutShippingMultiItemRowControl();\n");
                    script.AppendFormat("   row.setAddressSelectorcontrolId('{0}');\n", ctrlAddressSelector.ClientID);

                    script.Append(" }\n");
                    script.Append(");\n");
                    script.Append("</script>\n");

                    Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
                }
            }
        }

        private void InitializeDataSource()
        {
            rptCartItems.DataSource = GetCartItemsDataSource();
            rptCartItems.DataBind();
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/address_ajax.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/checkoutshippingmulti_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));

            manager.Scripts.Add(new ScriptReference("~/jscripts/address.verification.js"));
            manager.Scripts.Add(new ScriptReference("~/jscripts/usercontrol.address.control.js"));
        }

        private IEnumerable<CartItem> GetCartItemsDataSource()
        {
            List<CartItem> individualItems = new List<CartItem>();

            foreach (CartItem item in _cart.CartItems)
            {
                if (item.IsDownload || item.IsService)
                {
                    individualItems.Add(item);
                }
                else
                {
                    for (int quantity = 1; quantity <= item.m_Quantity; quantity++)
                    {
                        individualItems.Add(item);
                    }
                }
            }

            return individualItems;
        }

        private void ReloadCartAndItemAddresses()
        {
            InitializeShoppingCart();
            shouldRegisterAddressCountries = true;
            InitializeDataSource();            
        }

        private void ProcessCartItemAddresses()
        {
            var itemsPerAddress = new Dictionary<string, List<CartItem>>();
            var individualItems = rptCartItems.DataSource as List<CartItem>;

            for (int ctr = 0; ctr < individualItems.Count; ctr++)
            {
                var item = individualItems[ctr].Clone<CartItem>();
                if (item.IsDownload || item.IsService)
                {
                    // ship this to primary if not yet set..
                    _cart.SetItemAddress(item.m_ShoppingCartRecordID, ThisCustomer.PrimaryShippingAddress.AddressID);
                }
                else
                {
                    var ctrlAddressSelector = rptCartItems.Items[ctr].FindControl("ctrlAddressSelector") as AddressSelectorControl;
                    string preferredAddress = ctrlAddressSelector.SelectedAddress.AddressID;

                    if (item.m_ShippingAddressID != ctrlAddressSelector.SelectedAddress.AddressID)
                    {
                        if (!itemsPerAddress.ContainsKey(preferredAddress))
                        {
                            itemsPerAddress.Add(preferredAddress, new List<CartItem>());
                        }

                        var itemsInThisAddress = itemsPerAddress[preferredAddress];

                        // check if we have dups for this item
                        if (!itemsInThisAddress.Any(itemPerAddress => itemPerAddress.ItemCode == item.ItemCode))
                        {
                            itemsInThisAddress.Add(item);
                            item.MoveableQuantity = 1;                            
                        }
                        else
                        {
                            var savedCartItem = itemsInThisAddress.First(i => i.ItemCode == item.ItemCode);
                            savedCartItem.MoveableQuantity = savedCartItem.MoveableQuantity + 1;
                        }
                    }
                }
            }

            var lstRecAndTotalItems = _cart.CartItems.Select(i => new EcommerceCartRecordPerQuantity()
            {
                CartRecId = i.m_ShoppingCartRecordID,
                Total = i.m_Quantity
            }).ToList();

            foreach (string preferredAddress in itemsPerAddress.Keys)
            {
                foreach (var item in itemsPerAddress[preferredAddress])
                {
                    if (item.ItemType == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_KIT)
                    {
                        var composition = KitComposition.FromCart(ThisCustomer, CartTypeEnum.ShoppingCart, item.ItemCode, item.Id);

                        var cartRecord = lstRecAndTotalItems.Single(ri => ri.CartRecId == item.m_ShoppingCartRecordID);
                        cartRecord.Total = cartRecord.Total - item.MoveableQuantity;

                        _cart.SetItemQuantity(cartRecord.CartRecId, cartRecord.Total);
                        _cart.AddItem(ThisCustomer,preferredAddress,item.ItemCode,item.ItemCounter,item.MoveableQuantity,item.UnitMeasureCode,CartTypeEnum.ShoppingCart,composition);
                        InitializeShoppingCart();
                    }
                    else
                    {
                        var cartRecord = lstRecAndTotalItems.Single(ri => ri.CartRecId == item.m_ShoppingCartRecordID);
                        cartRecord.Total = cartRecord.Total - item.MoveableQuantity;

                        _cart.SetItemQuantity(cartRecord.CartRecId, cartRecord.Total);
                        _cart.AddItem(ThisCustomer,preferredAddress,item.ItemCode,item.ItemCounter,item.MoveableQuantity,item.UnitMeasureCode,CartTypeEnum.ShoppingCart);
                        InitializeShoppingCart();
                    }
                }
            }
        }

        protected void btnCompletePurchase_Click(object sender, EventArgs e)
        {
            ProcessCartItemAddresses();
            Response.Redirect("checkoutshippingmult2.aspx");
        }

        protected void lnkShipAllItemsToPrimaryShippingAddress_Click(object sender, EventArgs e)
        {
            ShipAllCartITemsToPrimaryShippingAddress();
        }

        private void ShipAllCartITemsToPrimaryShippingAddress()
        {
            string primaryShippingAddress = ThisCustomer.PrimaryShippingAddress.AddressID;

            foreach (CartItem item in _cart.CartItems)
            {
                _cart.SetItemAddress(item.m_ShoppingCartRecordID, primaryShippingAddress);
            }

            ReloadCartAndItemAddresses();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (_cart != null)
            {
                _cart.Dispose();
            }
            base.OnUnload(e);
        }

        private void InitControlText(){

            btnCompletePurchase.Text = AppLogic.GetString("checkoutshippingmult.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnCompletePurchase, "checkoutshippingmult.aspx.6");
            }
        }
    }
}
