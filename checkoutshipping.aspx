<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutshipping.aspx.cs" Inherits="InterpriseSuiteEcommerce.checkoutshipping" EnableEventValidation="false" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Register Src="UserControls/ShippingMethodControl.ascx" TagName="ShippingMethodControl" TagPrefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
</head>
 <body>
        <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />
        
        <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="center">
            <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server" BorderWidth="0">
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="87" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true" Top="0" Left="87" Bottom="54" Right="173" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx" Top="0" Left="173" Bottom="54" Right="259" />
            </asp:ImageMap>
        </asp:Panel>
        
        <form id="frmCheckOutShipping" runat="server">

        <div class="clear-both height-12"></div>
        <asp:Panel ID="pnlGetFreeShippingMsg" CssClass="FreeShippingThresholdPrompt" Visible="false" runat="server">
            <asp:Literal ID="GetFreeShippingMsg" runat="server" Mode="PassThrough"></asp:Literal>
            <div class="clear-both height-12"></div>
        </asp:Panel>

        <%-- shipping method controls section starts here  --%>
        <div class="sections-place-holder no-padding">
            <div class="section-header section-header-top"><asp:Literal ID="litPaymentDetails" runat="server">(!checkout1.aspx.30!)</asp:Literal></div>
                <div class="clear-both height-5"></div>
            <div class="section-content-wrapper">
                <asp:Panel ID="pnlShippingMethod" runat="server" HorizontalAlign="Center">
                    <asp:Label ID="lblSelectShippingMethod" Text="" runat="server" Font-Bold="true" class="content"/>
                    <br />
                    <uc:ShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                </asp:Panel>
            </div>
                <div class="clear-both height-5"></div>
        </div>
        <%-- shipping method controls section ends here  --%>
        
        <div class="clear-both height-12"></div>

        <div class="float-right">
            <asp:Panel ID="pnlCompletePurchase" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnCompletePurchase" runat="server" Text="Complete Purchase" OnClick="btnCompletePurchase_Click" CssClass="site-button content" />
            </asp:Panel>
        </div>
           
        <!-- Counpon Section { -->
            <div class="clear-both height-5"></div>
            <asp:Panel ID="panelCoupon" class="no-margin no-padding" runat="server">
                <div class="sections-place-holder no-padding">
                <div class="section-header section-header-top"><asp:Literal ID="Literal1" runat="server">(!checkoutshipping.aspx.14!)</asp:Literal></div>
                    <div id="divCouponEntered" class="section-content-wrapper"><asp:Literal ID="Literal2" runat="server">(!order.cs.12!)</asp:Literal><asp:Literal runat="server" ID="litCouponEntered"></asp:Literal></div>
                    <div class="clear-both height-5"></div>
                </div>
            </asp:Panel>
        <!-- Counpon Section } -->

        <div class="clear-both height-5"></div>
           
        <!-- Order Summary Section { -->

        <div class="sections-place-holder no-padding">
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
            </div>
            <div class="clear-both height-12"></div>
        </div>

        <!-- Order Summary Section } -->

        </form>
    </body>
</html>
