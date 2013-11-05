<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.checkoutshippingmult" CodeFile="checkoutshippingmult.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
 <body>
        <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="center">
            <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server">
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="87" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true" Top="0" Left="87" Bottom="54" Right="173" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx" Top="0" Left="173" Bottom="54" Right="259" />
            </asp:ImageMap>
        </asp:Panel>
        
        <p>
            <asp:Label ID="lblHeader1" runat="server" Text="(!checkoutshippingmult.aspx.5!)" Font-Bold="true"></asp:Label>
        </p>
        
        <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />
        
        <form id="frmCheckOutMultiShipping" runat="server">
        <div class="clear-both heigh-5"></div>
            <p> 
                <asp:Label ID="lblHeader2" runat="server" Text="To add or edit shipping address" ></asp:Label>
 
                <asp:HyperLink ID="lnkEditAddress" runat="server" NavigateUrl="selectaddress.aspx?checkout=true&AddressType=shipping&returnURL=checkoutshippingmult.aspx">click here</asp:HyperLink>
                <div class="clear-both heigh-5"></div>
                
                <asp:Label ID="lblHeader3" runat="server" Text="To ship all items to your primary shipping address " >
                </asp:Label>
                <asp:LinkButton ID="lnkShipAllItemsToPrimaryShippingAddress" runat="server" Text="click here" OnClick="lnkShipAllItemsToPrimaryShippingAddress_Click"  ></asp:LinkButton>    
                
            </p>   
               
            <asp:Repeater ID="rptCartItems" runat="server">
                 <HeaderTemplate>
                    <table width="100%" cellpadding="0" cellspacing="0"  id="divMultiShippingItems">
                </HeaderTemplate>
                 <FooterTemplate>
                    </table>
                 </FooterTemplate>
             </asp:Repeater>
             
             <hr />
             
            <asp:Panel ID="pnlCompletePurchase" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnCompletePurchase" runat="server" OnClick="btnCompletePurchase_Click" CssClass="site-button content" />
            </asp:Panel>
        
        </form>
    </body>
</html>
