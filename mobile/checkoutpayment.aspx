<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutpayment.aspx.cs"
    Inherits="InterpriseSuiteEcommerce.checkoutpayment" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators"
    TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.mobile" TagPrefix="ise" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<%@ Register Src="UserControls/MobilePaymentTermControl.ascx" TagName="MobilePaymentTermControl" TagPrefix="uc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<body>
    <asp:Panel ID="pnlHeaderGraphic" runat="server" class="imageStepLayout">
        <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server" BorderWidth="0">
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx"
                Top="0" Left="0" Bottom="54" Right="50" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="account.aspx?checkout=true"
                Top="0" Left="51" Bottom="54" Right="100" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="checkoutshipping.aspx"
                Top="0" Left="101" Bottom="54" Right="150" />
        </asp:ImageMap>
    </asp:Panel>
    <ise:InputValidatorSummary ID="errorSummary" CssClass="error" runat="server" Register="False" />
    <form id="frmCheckOutPayment" runat="server">
    <asp:Panel ID="pnlPaymentTerm" runat="server" CssClass="signin_main">
        <div class="signin_info">
            <div class="tableHeaderArea">
                <asp:Label runat="server" ID="lblCheckOutPaymentHeaderText"></asp:Label>
            </div>
             <uc:MobilePaymentTermControl ID="ctrlPaymentTerm" runat="server" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlCompletePurchase" runat="server" CssClass="button_layout">
        <uc1:ISEMobileButton ID="btnCompletePurchase" runat="server" />
    </asp:Panel>

    <asp:Panel ID="pnlOrderSummary" runat="server">
        <asp:Literal ID="OrderSummary" Mode="PassThrough" runat="server"></asp:Literal>
    </asp:Panel>

    <asp:Panel ID="pnlCompletePurchase2" runat="server" CssClass="button_layout">
        <uc1:ISEMobileButton ID="btnCompletePurchase2" runat="server" />
    </asp:Panel>

    </form>
</body>
</html>
