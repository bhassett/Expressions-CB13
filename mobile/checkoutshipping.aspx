<%@ Page Language="C#" AutoEventWireup="true" CodeFile="checkoutshipping.aspx.cs" Inherits="InterpriseSuiteEcommerce.checkoutshipping" EnableEventValidation="false" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register src="UserControls/ISEMobileButton.ascx" tagname="ISEMobileButton" tagprefix="uc1" %>
<%@ Register Src="UserControls/MobileShippingMethodControl.ascx" TagName="MobileShippingMethodControl" TagPrefix="uc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
 <body>
        <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="center">
            <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server" BorderWidth="0">
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="50" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="account.aspx?checkout=true" Top="0" Left="51" Bottom="54" Right="100" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="checkoutshipping.aspx" Top="0" Left="101" Bottom="54" Right="317" />
            </asp:ImageMap>
        </asp:Panel>
        
        <asp:Panel ID="pnlGetFreeShippingMsg" Visible="false" runat="server" CssClass="signin_info" >
            <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />
            <div class="signin_info_item" >
                <asp:Literal ID="GetFreeShippingMsg" runat="server" Mode="PassThrough"></asp:Literal>
            </div>
        </asp:Panel>
        
        <form id="frmCheckOutShipping" runat="server">
            <asp:Panel ID="pnlShippingMethod" runat="server" CssClass="signin_main removeMarginTop" >
                <div class="signin_info" >
                    <div class="tableHeaderArea">
                        <asp:Label ID="lblSelectShippingMethod" runat="server" />
                        <span >
                            <a href="checkoutshippingmult.aspx" class="kitdetaillink">
                                <asp:Label ID="lblClickHere" runat="server" />
                            </a>
                        </span>
                    </div>
                    <div class="signin_info_body">
                        <uc:MobileShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                    </div>
                </div>
            </asp:Panel>
            <br />

            <asp:Panel ID="pnlCompletePurchase" runat="server" class="button_layout">
                <uc1:ISEMobileButton ID="btnCompletePurchase" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlOrderSummary" runat="server" >
                <asp:Literal ID="OrderSummary" Mode="PassThrough" runat="server"></asp:Literal>            
            </asp:Panel>

            <div class="button_layout" >
                <uc1:ISEMobileButton ID="btnEditShoppingCart" runat="server" />                                            
            </div>

            <asp:Panel ID="Panel1" runat="server" class="button_layout">
                <uc1:ISEMobileButton ID="btnCompletePurchase2" runat="server" />
            </asp:Panel>

        </form>
    </body>
</html>
