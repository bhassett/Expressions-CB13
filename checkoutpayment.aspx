<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutpayment.aspx.cs"  Inherits="InterpriseSuiteEcommerce.checkoutpayment" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators"
    TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls"
    TagPrefix="ise" %>

<%@ Register TagPrefix="uc" TagName="BillingAddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentTermControl" Src="~/UserControls/PaymentTermControl.ascx" %>

<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ OutputCache Location="None" NoStore="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="jscripts/jquery/jquery.cbe.normal.checkout.js"></script>
    <style type="text/css">
        #save-as-credit-place-holder{display:none;}
        #errorSummary{display:none;}
        .CreditCardPaymentMethodPanel tbody tr td{text-align:left;}
    </style>
</head>
<body>
    <ise:InputValidatorSummary ID="errorSummary" CssClass="error" runat="server" Register="False" />

    <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="center">
        <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server" BorderWidth="0">
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx"
                Top="0" Left="0" Bottom="54" Right="87" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true"
                Top="0" Left="87" Bottom="54" Right="173" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx"
                Top="0" Left="173" Bottom="54" Right="259" />
        </asp:ImageMap>
    </asp:Panel>

    <div class="clear-both height-12"></div>
    
    <form id="frmCheckOutPayment" runat="server">
        
    <asp:Panel ID="pnlPageWrapper" runat="server">

    <!-- Billing Address and Payment Control Wrapper } -->   
      <div class="sections-place-holder no-padding">
        <div class="section-header section-header-top"><asp:Literal ID="litPaymentDetails" runat="server">(!checkout1.aspx.36!)</asp:Literal></div>
        <!-- Wrapper Padding { -->
        <div class="section-content-wrapper"> 
            <!-- Billing Address Section { -->
            <div id="payment-form-error-container" class="error-place-holder float-left normal-font-style font-size-12"></div>     
            
           <asp:Panel ID="pnlBillingAddressGrid" runat="server">
               <span class="strong-font  custom-font-style">
                   <asp:Literal ID="Literal3">&nbsp;</asp:Literal></span>
               <div id="billing-address-grid">
                   <asp:Literal ID="litBillingAddressGrid" runat="server"></asp:Literal>
               </div>
           </asp:Panel> 
                
           <div id="credit-card-options">
                <asp:Literal ID="LtrCreditCardOptionsRenderer" runat="server"></asp:Literal>
           </div>

            <div  id="billing-details-place-holder" class="normal-font-style font-size-12 float-left">
                <span class="strong-font  custom-font-style"><asp:Literal ID="litBillingContact" runat="server">(!checkout1.aspx.37!)</asp:Literal></span> 
                <div class="clear-both height-5"></div>

                    <div class="form-controls-place-holder">

                    <span class="form-controls-span">
                        <label id="lblBillingContactName" class="form-field-label">
                            <asp:Literal ID="litBillingContactName" runat="server">(!customersupport.aspx.4!)</asp:Literal>
                        </label>
                        <asp:TextBox ID="txtBillingContactName" runat="server" CssClass="light-style-input"></asp:TextBox>
                    </span>

                     <span class="form-controls-span">
                        <label  id="lblBillingContactNumber" class="form-field-label">
                            <asp:Literal ID="litContactNumber" runat="server" >(!customersupport.aspx.7!)</asp:Literal>
                        </label>
                        <asp:TextBox ID="txtBillingContactNumber" runat="server" CssClass="light-style-input" MaxLength="100"></asp:TextBox>
                     </span>

                    </div>

                    <div class="clear-both height-12"></div>

                    <span class="strong-font  custom-font-style"><asp:Literal ID="litBillingAddress" runat="server">(!checkout1.aspx.38!)</asp:Literal></span>
                    <div class="clear-both height-5"></div>
                    <uc:BillingAddressControl id="BillingAddressControl" IdPrefix="billing-" runat="server" />
                <div class="clear-both height-12"></div>
             </div>

             <!-- Billing Address Section } -->
              <div class="clear-both height-12"></div>
             <div id="move-credit-card-down-if-width-less-or-1000"></div>

             <!-- Payment Control { -->

             <div id="credit-card-details-place-holder-checkout-payment" class="custom-font-style ">      
               
                <span class="strong-font  custom-font-style"><asp:Literal ID="litPaymentsMethod" runat="server">(!checkout1.aspx.40!)</asp:Literal></span>
                <div class="clear-both height-12"></div>

                <asp:Panel ID="pnlPaymentTerm" runat="server" HorizontalAlign="Center">
                    <uc:PaymentTermControl ID="ctrlPaymentTerm" runat="server"></uc:PaymentTermControl>
                </asp:Panel> 

          
            </div>
            <div class="clear-both height-5"></div>
            <div class="clear-both height-12"></div>
               
            <!-- Payment Control { -->
        </div>
        <!-- Wrapper Padding } -->
    </div>
    <!-- Billing Address and Payment Control Wrapper } -->

    <div class="clear-both height-12"></div>

    <!-- Checkout Button Section { -->

    <div id="billing-method-button-place-holder">
        <div id="billing-method-button">

             <input type="button" value="<%=AppLogic.GetString("checkoutpayment.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" id="checkoutpayment-submit-button" class="site-button float-right content" 
            data-contentKey="checkoutpayment.aspx.6" 
            data-contentType="string resource"
            data-contentValue="<%=AppLogic.GetString("checkoutpayment.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"/>
        </div>
        <div id="save-billing-method-loader"></div>
    </div>


    <!-- Checkout Button Section } -->
  
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

    <!-- Hidden Fields { -->

    <asp:HiddenField ID="hidRecentData" runat="server" EnableViewState="true" />

    <div class="display-none">
        <asp:Button ID="btnDoProcessPayment" runat="server" Text="Complete Purchase" CssClass="site-button" />
        <asp:TextBox ID="txtCityStates" runat="server"></asp:TextBox>
        <asp:TextBox id="txtCode" runat="server"></asp:TextBox>
        <asp:TextBox ID="hidMaskCardNumber" runat="server"></asp:TextBox>
        <asp:TextBox ID="hidCreditCardCode" runat="server"></asp:TextBox>
    </div>

    <div id="c-ref-no" class="display-none"></div>
    <div id="isTokenization" class="display-none"><asp:Literal ID="litTokenizationFlag" runat="server"></asp:Literal></div>
    <div id="isRegistered" class="display-none"><asp:Literal ID="litIsRegistered" runat="server"></asp:Literal></div
    <!-- Hidden Fields } -->
     </asp:Panel>
    </form>
</body>
</html>