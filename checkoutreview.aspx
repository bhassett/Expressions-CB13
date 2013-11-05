<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.checkoutreview" CodeFile="checkoutreview.aspx.cs" %>

<%@ Register TagPrefix="ise" Namespace="InterpriseSuiteEcommerceControls" Assembly="InterpriseSuiteEcommerceControls" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<html>
<head>
</head>
<body>
    <asp:Panel runat="server">
        <div style="text-align: center;">
            <div style="text-align: center;">
                <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server" BorderWidth="0">
                    <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx"
                        Top="0" Left="0" Bottom="54" Right="87" />
                    <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true"
                        Top="0" Left="87" Bottom="54" Right="173" />
                    <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx"
                        Top="0" Left="173" Bottom="54" Right="259" />
                    <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/checkoutpayment.aspx"
                        Top="0" Left="259" Bottom="54" Right="345" />
                </asp:ImageMap>
            </div>
            <ise:Topic runat="server" ID="CheckoutReviewPageHeader" TopicName="CheckoutReviewPageHeader" />
            <asp:Literal ID="XmlPackage_CheckoutReviewPageHeader" runat="server" Mode="PassThrough"></asp:Literal>
            <form runat="server">
            <div style="clear: both; height: 12px;">
            </div>
            <asp:Literal ID="checkoutreviewaspx6" Mode="PassThrough" runat="server" Text="(!checkoutreview.aspx.6!)"></asp:Literal>
            <div style="clear: both; height: 12px;">
            </div>
            <div id='place-order-button-container'>
                <div>
                    <div id="place-order-message">
                        <img id="ajax-loader" src="images/ajax-loader.gif">
                        <strong>
                            <asp:Literal ID="checkoutreviewaspx14" Mode="PassThrough" runat="server" Text="(!checkoutreview.aspx.14!)"></asp:Literal></strong></div>
                </div>
            </div>
            <asp:Panel ID="pnlButtonPlaceHolder" runat="server">
            <%if (Customer.Current.IsInEditingMode() && Security.IsAdminCurrentlyLoggedIn())
              { %>
                 <input type="button"  id="create-customer-account" 
                        class="site-button content" 
                        data-contentKey="checkoutreview.aspx.7" 
                        data-contentType="string resource"
                        data-contentValue="<%= AppLogic.GetString("checkoutreview.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"
                        value="<%= AppLogic.GetString("checkoutreview.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"/> 
            <% }
              else
              { %>
            <asp:Button ID="btnContinueCheckout1" Text="(!checkoutreview.aspx.7!)" CssClass="site-button" runat="server" />
            <% } %>
            </asp:Panel>

            <div style="clear: both; height: 12px">
            </div>
            <table width="100%">
                <tr>
                    <td width="50%" align="left" valign="top">
                        <asp:Label ID="checkoutreviewaspx8" Text="(!checkoutreview.aspx.8!)" Font-Bold="true"
                            runat="server"></asp:Label>
                        <br />
                        <asp:Panel ID="pnlEditBillingAddress" runat="server" Visible="false">
                            <img src="images/white.gif" alt="" /><br />
                            <asp:Image ID="imgBillingRedArrow" AlternateText="" runat="server" />&#0160;<a href="selectaddress.aspx?AddressType=Billing&editaddress=true&checkout=true"><asp:Literal
                                ID="EditBillingAddress" Text="(!editaddress.aspx.1!)" runat="server"></asp:Literal></a><br />
                            <img src="images/white.gif" height="3" alt="" />
                        </asp:Panel>
                        <asp:Literal ID="litBillingAddress" runat="server" Mode="PassThrough"></asp:Literal>
                        <br />
                        <br />
                        <asp:Label ID="checkoutreviewaspx9" Text="(!checkoutreview.aspx.9!)" Font-Bold="true"
                            runat="server"></asp:Label>
                        <br />
                        <asp:Literal ID="litPaymentMethod" runat="server" Mode="PassThrough"></asp:Literal>
                    </td>
                    <td width="50%" align="left" valign="top">
                        <asp:Label ID="ordercs57" Text="(!order.cs.19!)" Font-Bold="true" runat="server"></asp:Label>
                        <br />
                        <asp:Panel ID="pnlEditShippingAddress" runat="server" Visible="false">
                            <img src="images/white.gif" alt="" /><br />
                            <asp:Image ID="imgShippingRedArrow" AlternateText="" runat="server" />&#0160;<a href="selectaddress.aspx?AddressType=Shipping&editaddress=true&checkout=true"><asp:Literal
                                ID="EditShippingAddress" Text="(!editaddress.aspx.1!)" runat="server"></asp:Literal></a><br />
                            <img src="images/white.gif" height="3" alt="" />
                        </asp:Panel>
                        <asp:Literal ID="litShippingAddress" runat="server" Mode="PassThrough"></asp:Literal>
                    </td>
                </tr>
            </table>
            <br />

        <!-- Counpon Section { -->
    <div class="clear-both height-5"></div>
    <asp:Panel ID="panelCoupon" class="no-margin no-padding" runat="server">
        <div class="sections-place-holder no-padding">
            <div class="section-header section-header-top"><asp:Literal ID="Literal1" runat="server">(!checkoutshipping.aspx.14!)</asp:Literal></div>
            <div id="divCouponEntered"><asp:Literal ID="Literal2" runat="server">(!order.cs.12!)</asp:Literal><asp:Literal runat="server" ID="litCouponEntered"></asp:Literal></div>
            </div>
    </asp:Panel>
    <!-- Counpon Section } -->
    <div class="clear-both height-12"></div>
    <div class="clear-both height-5"></div>

    <div class="sections-place-holder no-padding">
        <!-- Order Summary Section { -->

        <div class="sections-place-holder">
            <div class="section-header section-header-top"><asp:Literal ID="litItemsToBeShipped" runat="server">(!checkout1.aspx.42!)</asp:Literal></div>
              
            <div class="section-content-wrapper">
            <div id="order-summary-head-text" style="padding-left: 23px;padding-right:12px">
                <div class="clear-both height-12"></div>
                <span class="strong-font  custom-font-style">
                <asp:Literal ID="litOrderSummary" runat="server">(!checkout1.aspx.43!)</asp:Literal>
                </span> 
                <span class="one-page-link-right normal-font-style  float-right">
                <a href="shoppingcart.aspx" class="custom-font-style"><asp:Literal ID="litEditCart" runat="server">(!checkout1.aspx.44!)</asp:Literal></a></span>
            </div>

            <div class="clear-both height-12"></div>

            <div id="items-to-be-shipped-place-holder-1">
            <asp:Literal ID="OrderSummary" runat="server"></asp:Literal>
            </div>
           
           
            <div class="clear-both height-12"></div>
            <div id='items-to-be-shipped-footer'>
            <asp:Literal runat="server" ID="litOrderSummaryFooter"></asp:Literal>
            </div>
            <div class="clear-both height-12"></div>
            </div>
        </div>

        <!-- Order Summary Section } -->
    </div>


            <ise:Topic runat="server" ID="CheckoutReviewPageFooter" TopicName="CheckoutReviewPageFooter" />
            <asp:Literal ID="XmlPackage_CheckoutReviewPageFooter" runat="server" Mode="PassThrough"></asp:Literal>
            </form>
        </div>
    </asp:Panel>
</body>
</html>
