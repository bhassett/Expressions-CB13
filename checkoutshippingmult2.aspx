<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.checkoutshippingmult2" CodeFile="checkoutshippingmult2.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register Src="~/UserControls/ShippingMethodControl.ascx" TagName="ShippingMethodControl" TagPrefix="uc" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
 <body>
        <asp:Panel ID="pnlHeaderGraphic" runat="server" HorizontalAlign="center">
            <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server">
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="87" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="~/account.aspx?checkout=true" Top="0" Left="87" Bottom="54" Right="173" />
                <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="~/checkoutshipping.aspx" Top="0" Left="173" Bottom="54" Right="259" />
            </asp:ImageMap>
        </asp:Panel>
         
        <ise:InputValidatorSummary ID="errorSummary" runat="server" ForeColor="Red" Register="False" />
        
        <form id="frmCheckOutMultiShipping2" runat="server">
        
            <asp:Repeater ID="rptCartItems" runat="server">
                 <HeaderTemplate>
                    <table width="100%" cellpadding="0" cellspacing="0" >
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="background-color:#DDDDDD;">
                        <td style="height:15px;">
                            <span>
                                <b>
                                    <%=String.Format(AppLogic.GetString("shoppingcart.cs.29", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), ++ShippingGroupCounter)%>
                                </b>
                            </span>
                        </td>
                        <td style="height:15px;">
                            <span>
                                <b>
                                    <%=AppLogic.GetString("shoppingcart.cs.28", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>
                                </b>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding: 2px;">
                            <br />
                                <asp:Panel runat="server" ID="pnlItemContainer"></asp:Panel>
                            <br />
                        </td>
                        <td valign="top">
                            <br />
                            <b>
                                <asp:Label runat="server" ID="lblShipmethodHeader"></asp:Label>
                            </b>
                            <asp:Panel runat="server" ID="divShippingInfo">
                                <asp:Label runat="server" ID="lblShippingAddressString"></asp:Label>
                                <br />
                                <span>
                                    <b><%# AppLogic.GetString("shoppingcart.cs.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %></b>
                                </span>
                                <uc:ShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                            </asp:Panel>
                        </td>
                    </tr>
                </ItemTemplate>
                 <FooterTemplate>
                    </table>
                 </FooterTemplate>
             </asp:Repeater>
            
            <hr />
            
            <asp:Panel ID="pnlCompletePurchase" runat="server" HorizontalAlign="Center">
                <asp:Button ID="btnCompletePurchase" CssClass="site-button" runat="server" Text="(!checkoutshippingmult.aspx.6!)" OnClick="btnCompletePurchase_Click" />
            </asp:Panel>
        
        </form>
    </body>
</html>
