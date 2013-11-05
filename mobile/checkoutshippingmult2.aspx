<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.checkoutshippingmult2" CodeFile="checkoutshippingmult2.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<%@ Register Src="UserControls/MobileShippingMethodControl.ascx" TagName="MobileShippingMethodControl" TagPrefix="uc" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="System" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<body>
    <asp:Panel ID="pnlHeaderGraphic" runat="server" CssClass="imageStepLayout">
        <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server">
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx"
                Top="0" Left="0" Bottom="54" Right="50" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="account.aspx?checkout=true"
                Top="0" Left="51" Bottom="54" Right="100" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="checkoutshipping.aspx"
                Top="0" Left="101" Bottom="54" Right="317" />
        </asp:ImageMap>
    </asp:Panel>
    <div class="signin_main">
        <ise:InputValidatorSummary ID="errorSummary" runat="server" CssClass="errorLg" Register="False" />
        <div class="signin_info removeMarginTop">
            <div class="tableheaderarea">
                <asp:Literal ID="litHeaderText" runat="server"></asp:Literal>
            </div>
            <div class="signin_info_body">
                <form id="frmCheckOutMultiShipping2" runat="server">
                <asp:Repeater ID="rptCartItems" runat="server">
                    <HeaderTemplate>
                        <table style="width:100%">
                    </HeaderTemplate>
                    <ItemTemplate >
                        <tr>
                            <td colspan="2">
                                <span class="shippingGroupHeaderLayout">
                                    <b><%# String.Format("{0} {1}", AppLogic.GetString("mobile.shoppingcart.cs.29", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), ++ShippingGroupCounter)%></b>
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding: 2px;">
                                <br />
                                    <asp:Panel runat="server" ID="pnlItemContainer"></asp:Panel>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="signin_info" runat="server">
                                    <div class="tableheaderarea">
                                        <asp:Label runat="server" ID="lblShipmethodHeader"></asp:Label>
                                    </div>
                                    <asp:Panel runat="server" CssClass="signin_info_body" ID="divShippingInfo">
                                        <br />
                                        <b>
                                            <asp:Label runat="server" ID="lblShippingAddressString"></asp:Label>
                                        </b>
                                        <br />
                                        <br />
                                        <span>
                                            <b><%# AppLogic.GetString("shoppingcart.cs.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %></b>
                                        </span>
                                        <uc:MobileShippingMethodControl ID="ctrlShippingMethod" runat="server" />
                                        </asp:Panel>
                                    </asp:Panel>
                                </div>                                                                
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <hr />
                <asp:Panel ID="pnlCompletePurchase" runat="server" CssClass="button_layout">
                    <uc1:ISEMobileButton ID="btnCompletePurchase" runat="server" />
                </asp:Panel>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
