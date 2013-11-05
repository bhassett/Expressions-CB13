<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.checkoutreview" CodeFile="checkoutreview.aspx.cs" %>

<%@ Register TagPrefix="ise" Namespace="InterpriseSuiteEcommerceControls" Assembly="InterpriseSuiteEcommerceControls" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<div class="imageStepLayout">
    <asp:imagemap id="checkoutheadergraphic" hotspotmode="PostBack" runat="server" borderwidth="0">
        <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="50" />
        <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="account.aspx?checkout=true" Top="0" Left="51" Bottom="54" Right="100" />
        <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="checkoutshipping.aspx" Top="0" Left="101" Bottom="54" Right="150" />
        <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="checkoutpayment.aspx" Top="0" Left="151" Bottom="54" Right="200" />
    </asp:imagemap>
</div>
<ise:Topic runat="server" ID="CheckoutReviewPageHeader" TopicName="CheckoutReviewPageHeader" />
<asp:literal id="XmlPackage_CheckoutReviewPageHeader" runat="server" mode="PassThrough"></asp:literal>
<form runat="server">

<div class="signin_main gotextcenter removeMarginBottom">
    <div class="signin_info">
        <div class="tableHeaderArea">
            <asp:literal id="checkoutreviewaspx6" mode="PassThrough" runat="server" text="(!checkoutreview.aspx.6!)"></asp:literal>
        </div>
        <div class="signin_info_body">
            <div class="button_layout overideButtonLayout">
                <uc1:ISEMobileButton ID="btnContinueCheckout1" runat="server" />
            </div>
        </div>    
    </div>
</div>

<div class="signin_main gotextcenter removeMarginBottom">
    <div class="signin_info">
        <div class="tableHeaderArea">
            <asp:literal id="Literal1" mode="PassThrough" runat="server" text="(!mobile.checkoutreview.aspx.14!)"></asp:literal>
        </div>
         <div class="signin_info_body">
             <div class="review_addresslayout">
                 <asp:label id="checkoutreviewaspx8" text="(!checkoutreview.aspx.8!)" font-bold="true"
                     runat="server"></asp:label>
                 <br />
                 <asp:panel id="pnlEditBillingAddress" runat="server" visible="false">
                    <a href="selectaddress.aspx?returnurl=shoppingcart.aspx&AddressType=Billing">
                        <asp:Literal ID="EditBillingAddress" Text="(!editaddress.aspx.1!)" runat="server"></asp:Literal>
                    </a>
                    <br />
                </asp:panel>
                 <asp:literal id="litBillingAddress" runat="server" mode="PassThrough"></asp:literal>
                 <br />
                 <br />
                 <asp:label id="checkoutreviewaspx9" text="(!checkoutreview.aspx.9!)" font-bold="true"
                     runat="server"></asp:label>
                 <br />
                 <asp:literal id="litPaymentMethod" runat="server" mode="PassThrough"></asp:literal>
             </div>
             <div class="review_shippingLayout">
                 <asp:label id="ordercs57" text="(!order.cs.19!)" font-bold="true" runat="server"></asp:label>
                 <br />
                 <asp:panel id="pnlEditShippingAddress" runat="server" visible="false">
                    <a href="selectaddress.aspx?returnurl=shoppingcart.aspx&AddressType=Shipping">
                    <asp:Literal ID="EditShippingAddress" Text="(!editaddress.aspx.1!)" runat="server"></asp:Literal>
                    </a>
                </asp:panel>
                 <asp:literal id="litShippingAddress" runat="server" mode="PassThrough"></asp:literal>
             </div>
        </div>
    </div>
</div>
<br />
<asp:literal id="CartSummary" mode="PassThrough" runat="server"></asp:literal>
<br />

<div class="button_layout overideButtonLayout">
    <uc1:ISEMobileButton ID="btnContinueCheckout2" runat="server" />
</div>

<ise:Topic runat="server" ID="CheckoutReviewPageFooter" TopicName="CheckoutReviewPageFooter" />
<asp:literal id="XmlPackage_CheckoutReviewPageFooter" runat="server" mode="PassThrough"></asp:literal>
</form>
