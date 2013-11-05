<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.ShoppingCartPage" CodeFile="ShoppingCart.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="ise" Namespace="InterpriseSuiteEcommerceControls" Assembly="InterpriseSuiteEcommerceControls" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script type="text/javascript" src="jscripts/shippingcalculator.js"></script>
</head>
<body>
    <asp:Literal ID="ValidationScript" runat="server"></asp:Literal>
    <asp:Literal ID="JSPopupRoutines" runat="server"></asp:Literal>
    <form id="CartForm" onsubmit="return Cart_Validator(this)" runat="server">
    <b>
        <asp:Literal ID="RedirectToSignInPageLiteral" runat="server"></asp:Literal></b>
    <asp:Panel ID="BodyPanel" runat="server">
        <div style="width: 100%; height: 100%">
            <ise:Topic runat="server" ID="HeaderMsg" TopicName="CartPageHeader" />
            <asp:Literal ID="XmlPackage_ShoppingCartPageHeader" runat="server"></asp:Literal>
            <table cellspacing="3" cellpadding="0" border="0" style="width: 100%">
                <tr>
                    <td>
                        <asp:Panel ID="ShippingInformation" runat="server">
                            <asp:Image ID="redarrow1" AlternateText="" runat="server" />&#0160;<a onclick="popuptopicwh('Shipping+Information','shipping',650,550,'yes')"
                                href="javascript:void(0);"><asp:Literal ID="shoppingcartaspx8" runat="server"></asp:Literal></a><br />
                        </asp:Panel>
                        <asp:Image ID="redarrow2" AlternateText="" runat="server" />&#0160;<a onclick="popuptopicwh('Return+Policy+Information','returns',650,550,'yes')"
                            href="javascript:void(0);"><asp:Literal ID="shoppingcartaspx9" Text="(!shoppingcart.aspx.7!)"
                                runat="server"></asp:Literal></a><br />
                        <asp:Image ID="redarrow3" AlternateText="" runat="server" />&#0160;<a onclick="popuptopicwh('Privacy+Information','privacy',650,550,'yes')"
                            href="javascript:void(0);"><asp:Literal ID="shoppingcartaspx10" Text="(!shoppingcart.aspx.8!)"
                                runat="server"></asp:Literal></a><br />
                        <asp:Panel ID="AddresBookLlink" runat="server">
                            <asp:Image ID="redarrow4" AlternateText="" runat="server" />&#0160;<a href="selectaddress.aspx?returnurl=shoppingcart.aspx&AddressType=Shipping"><asp:Literal
                                ID="shoppingcartaspx11" Text="(!shoppingcart.aspx.9!)" runat="server"></asp:Literal></a><br />
                        </asp:Panel>
                        &#160;<br />
                    </td>
                    <td valign="middle" align="right">
                        <asp:Button ID="btnContinueShoppingTop" Text="(!shoppingcart.cs.12!)" CssClass="site-button content"
                            runat="server" />&#160;
                        <asp:Button ID="btnCheckOutNowTop" Text="(!shoppingcart.cs.34!)" runat="server" CssClass="site-button CheckoutNowButton content" /><br />
                    </td>
                </tr>
                <tr runat="server" id="AlternativeCheckoutsTop">
                    <td colspan="2" align="right" style="height: 61px">
                        <table border="0">
                            <tr>
                                <td align="right" colspan="2">
                                    <asp:Label ID="Label3" runat="server" Text="(!shoppingcart.aspx.14!)" Style="margin-right: 7px;"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span runat="server" id="GoogleCheckoutSpanTop" visible="false">
                                        <asp:ImageButton ImageAlign="Top" runat="server" ID="btnGoogleCheckoutTop" OnClick="btnGoogleCheckout_Click"/>
                                    </span>
                                </td>
                                <td align="right">
                                    <span runat="server" id="PayPalExpressSpanTop" visible="false">
                                            <asp:ImageButton ID="btnPayPalExpressCheckoutTop" cms-3rdparty-attr runat="server" OnClick="btnPayPalExpressCheckout_Click" />
                                    </span>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlCouponError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="CouponError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlErrorMsg" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlRemovePhasedOutItemWithNoStockError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="RemovePhasedOutItemWithNoStockError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlInventoryTrimmedError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="InventoryTrimmedError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMinimumQuantitiesUpdatedError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="MinimumQuantitiesUpdatedError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderAmountError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="MeetsMinimumOrderAmountError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderWeightError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="MeetsMinimumOrderWeightError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderQuantityError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="MeetsMinimumOrderQuantityError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMicropay_EnabledError" runat="Server" Visible="false">
                <asp:Literal ID="Micropay_EnabledError" runat="Server"></asp:Literal></asp:Panel>
            <div style="clear: both">
            </div>
            <div class="hidden errorLg" id="required-error">
                <asp:Literal ID="lRequiredError" runat="server" Visible="True" Text="(!leadform.aspx.16!)"></asp:Literal>
            </div>
            <br />
            <asp:Panel ID="pnlCartSummary" runat="server" HorizontalAlign="right" DefaultButton="btnUpdateCart1">
                <asp:Literal ID="CartItems" runat="server"></asp:Literal>
                <br />
                <asp:Panel ID="pnlCartSummarySubTotals" runat="server">
                    <asp:Literal ID="CartSubTotal" runat="server"></asp:Literal>
                </asp:Panel>
                <div class="update-cart-layout">
                    <asp:Button ID="btnUpdateCart1" CssClass="site-button content" Text="(!shoppingcart.cs.33!)" runat="server" />
                </div>
            </asp:Panel>
            <br />
            <div class="clr">
            </div>
            <asp:Panel ID="pnlShippingCalculator" runat="Server" Visible="false">
                <div class="cart-header-wrapper">
                    <span class="cart-header-text">
                        <%= AppLogic.GetString("shoppingcart.aspx.20", SkinID, ThisCustomer.LocaleSetting)%>
                    </span>
                </div>
                <div id="pnlShippingCalculatorcontainer">
                    <div class="shipping-calculator-wrapper">
                        <span class="shipping-calculator-label">
                            <%= AppLogic.GetString("shoppingcart.aspx.21", SkinID, ThisCustomer.LocaleSetting)%>
                        </span>
                        <br />
                        <uc:AddressControl ID="AddressControl" hideStreetAddress="true" runat="server" showResidenceTypes="true" hideCounty="true" />
                        <div class="clr">
                        </div>
                        <div class="shipping-calculator-horizontal-line">
                        </div>
                        <div class="shipping-calculator-controls">
                            <asp:Button runat="server" class="site-button content" ID="btnCalcShip" UseSubmitBehavior="false"></asp:Button>
                        </div>
                    </div>
                    <div id="pnlShippingMethods" class="calculator-shipping-methods rButton" >
                        <div id="imgLoading" style="display: none">
                            <asp:Image ImageUrl="~/skins/Skin_1/images/loadingshipping.gif" runat="server" />
                        </div>
                        <div id="shippingMethodOpt">
                        </div>
                    </div>
                    <div class="clr">
                    </div>
                </div>
                <div class="clr">
                </div>
            </asp:Panel>
            <br />
            <asp:Panel ID="pnlOrderOptions" runat="server" Visible="false">
                <table width="100%" cellpadding="2" cellspacing="0" border="0" style="border-style: solid;
                    border-width: 0px; border-color: #444444">
                    <tr>
                        <td align="left" valign="top">
                            <asp:Image ID="ShoppingCartorderoptions_gif" runat="server" /><br />
                            <table width="100%" cellpadding="4" cellspacing="0" border="0" style="border-style: solid;
                                border-width: 1px; border-color: #444444;">
                                <tr>
                                    <td align="left" valign="top">
                                        <div style="text-align: center; width: 100%;">
                                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="shoppingcartcs27" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.5!)"
                                                            runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="shoppingcartcs121" runat="server" CssClass="OrderOptionsRowHeader"
                                                            Text="(!shoppingcart.cs.37!)">
                                                        </asp:Label>
                                                    </td>
                                                    <td align="center">
                                                        <asp:Label ID="shoppingcartcs28" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.6!)"
                                                            runat="server"></asp:Label>
                                                    </td>
                                                    <td width="25" align="center">
                                                        <asp:Label ID="shoppingcartcs29" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.7!)"
                                                            runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <asp:Repeater ID="OrderOptionsList" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td align="left">
                                                                <asp:Image ID="OptionImage" runat="server" Visible="false" />
                                                                <asp:Label ID="OrderOptionName" CssClass="OrderOptionsName" runat="server"></asp:Label>
                                                                <asp:Image ID="helpcircle_gif" runat="server" AlternateText='<%# InterpriseSuiteEcommerceCommon.AppLogic.GetString("shoppingcart.cs.8",ThisCustomer.SkinID,ThisCustomer.LocaleSetting) %>'
                                                                    Style="cursor: hand; cursor: pointer;" />
                                                            </td>
                                                            <td align="left">
                                                                <asp:Label ID="lblUnitMeasureCode" runat="server" Text=""></asp:Label>
                                                                <asp:DropDownList ID="cboUnitMeasureCode" runat="server">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td align="center">
                                                                <asp:Label ID="OrderOptionPrice" CssClass="OrderOptionsPrice" runat="server"></asp:Label>
                                                            </td>
                                                            <td align="center">
                                                                <asp:HiddenField ID="hfItemCounter" runat="server" />
                                                                <ise:DataCheckBox ID="OrderOptions" runat="server" Data='<%# ((System.Xml.XmlNode)Container.DataItem)["ItemCode"].InnerText %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="middle" align="left" colspan="3">
                                                                <asp:Label ID="lblNotes" Text="Notes:" runat="server" /><br />
                                                                <asp:TextBox ID="txtOrderOptionNotes" TextMode="MultiLine" runat="server" Width="100%" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div style="text-align: right;">
                    <asp:Button ID="btnUpdateCart2" runat="server" Text="(!shoppingcart.cs.33!)" CssClass="site-button content" /></div>
            </asp:Panel>
            <br />
            <asp:Panel ID="pnlUpsellProducts" runat="server" Visible="false">
                <asp:Literal ID="UpsellProducts" runat="server"></asp:Literal>
                <div style="text-align: right;">
                    <asp:Button ID="btnUpdateCart5" runat="server" Text="(!shoppingcart.cs.33!)" CssClass="site-button content"
                        Visible="false" /></div>
            </asp:Panel>
            <asp:Panel ID="pnlCoupon" runat="server" Visible="false" DefaultButton="btnUpdateCart3">
                <table width="100%" cellpadding="2" cellspacing="0" border="0" style="border-style: solid;
                    border-width: 0px; border-color: #444444">
                    <tr>
                        <td align="left" valign="top">
                            <asp:Image ID="ShoppingCartCoupon_gif" runat="server" /><br />
                            <table width="100%" cellpadding="4" cellspacing="0" border="0" style="border-style: solid;
                                border-width: 1px; border-color: #444444;">
                                <tr>
                                    <td align="left" valign="top">
                                        <asp:Label ID="shoppingcartcs31" runat="server" Text="(!shoppingcart.cs.9!)"></asp:Label>&#160;
                                        <asp:TextBox ID="CouponCode" Columns="30" MaxLength="50" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div style="text-align: right">
                    <asp:Button ID="btnUpdateCart3" runat="server" Text="(!shoppingcart.cs.33!)" CssClass="site-button content" />
                </div>
            </asp:Panel>
            <br />
            <asp:Panel ID="pnlOrderNotes" runat="server" Visible="false" DefaultButton="btnUpdateCart4">
                <table width="100%" cellpadding="2" cellspacing="0" border="0" style="border-style: solid;
                    border-width: 0px; border-color: #444444">
                    <tr>
                        <td align="left" valign="top">
                            <asp:Image ID="ShoppingCartNotes_gif" runat="server" /><br />
                            <table width="100%" cellpadding="4" cellspacing="0" border="0" style="border-style: solid;
                                border-width: 1px; border-color: #444444;">
                                <tr>
                                    <td align="left" valign="top">
                                        <asp:Label ID="lblOrderNotes" runat="server" Text="(!shoppingcart.cs.13!)"></asp:Label><br />
                                        <br />
                                        <asp:TextBox ID="OrderNotes" Columns="90" Rows="4" TextMode="MultiLine" Width="99%"
                                            runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div style="text-align: right">
                    <asp:Button ID="btnUpdateCart4" runat="server" Text="(!shoppingcart.cs.33!)" CssClass="site-button content" />
                </div>
            </asp:Panel>
            <br />
            <table cellspacing="3" cellpadding="0" width="100%" border="0">
                <tr>
                    <td>
                        &#160;
                    </td>
                    <td valign="bottom" align="right">
                        <asp:Button ID="btnContinueShoppingBottom" Text="(!shoppingcart.cs.12!)" CssClass="site-button content"
                            runat="server" />&#160;
                        <asp:Button ID="btnCheckOutNowBottom" Text="(!shoppingcart.cs.34!)" runat="server"
                            CssClass="site-button CheckoutNowButton content" />
                        <br />
                        <br />
                    </td>
                </tr>
                <tr runat="server" id="AlternativeCheckoutsBottom">
                    <td colspan="2" align="right" style="height: 61px">
                        <table border="0">
                            <tr>
                                <td colspan="2" align="right">
                                    <asp:Label ID="Label1" runat="server" Text="(!shoppingcart.aspx.14!)" Style="margin-right: 7px;"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span runat="server" id="GoogleCheckoutSpanBottom" visible="false">
                                        <asp:ImageButton ImageAlign="Top" runat="server" ID="btnGoogleCheckoutBottom" OnClick="btnGoogleCheckout_Click" />
                                    </span>
                                </td>
                                <td align="right">
                                    <span runat="server" id="PayPalExpressSpanBottom" visible="false">
                                        <asp:ImageButton ID="btnPayPalExpressCheckoutBottom" cms-3rdparty-attr runat="server" OnClick="btnPayPalExpressCheckout_Click" />
                                    </span>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <ise:Topic ID="CartPageFooterTopic" runat="server" TopicName="CartPageFooter" />
            <asp:Literal ID="XmlPackage_ShoppingCartPageFooter" runat="server"></asp:Literal>
        </div>
    </asp:Panel>
    </form>
</body>
</html>
