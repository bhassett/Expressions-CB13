<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.checkoutshippingmult" CodeFile="checkoutshippingmult.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators"
    TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.mobile"
    TagPrefix="ise" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<body>
    <asp:Panel ID="pnlHeaderGraphic" runat="server" CssClass="imageStepLayout" >
        <asp:ImageMap ID="checkoutheadergraphic" HotSpotMode="PostBack" runat="server">
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx" Top="0" Left="0" Bottom="54" Right="50" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Navigate" NavigateUrl="account.aspx?checkout=true" Top="0" Left="51" Bottom="54" Right="100" />
            <asp:RectangleHotSpot AlternateText="" HotSpotMode="Inactive" NavigateUrl="checkoutshipping.aspx" Top="0" Left="101" Bottom="54" Right="317" />
        </asp:ImageMap>
    </asp:Panel>
    <div class="signin_main">
        <div class="signin_info">
            <div class="tableHeaderArea">
                <asp:Label ID="lblHeader1" runat="server"></asp:Label>
            </div>
            <div class="signin_info_body">
                <ise:InputValidatorSummary ID="errorSummary" runat="server" CssClass="errorLg" Register="False" />
                <form id="frmCheckOutMultiShipping" runat="server">
                <p>
                    <a href="account.aspx?checkout=true" class="kitdetaillink"><b>
                        <asp:Literal runat="server" ID="litClickHere"></asp:Literal></b> </a>
                    <asp:Label ID="lblHeader2" runat="server"></asp:Label>.
                    <br />
                    <br />
                    <b>
                        <asp:LinkButton ID="lnkShipAllItemsToPrimaryShippingAddress" CssClass="kitdetaillink"
                            runat="server" OnClick="lnkShipAllItemsToPrimaryShippingAddress_Click"></asp:LinkButton>
                    </b>
                    <asp:Label ID="lblHeader3" runat="server"></asp:Label>
                    <br />
                    <br />
                </p>
                <asp:Repeater ID="rptCartItems" runat="server">
                    <HeaderTemplate>
                        <table width="100%">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="multipleAddressItem_layout">
                            <td style="width: 40%;">
                                <b>
                                    <%# AppLogic.GetString("shoppingcart.cs.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %></b>
                            </td>
                            <td style="width: 60%;">
                                <b>
                                    <%# AppLogic.GetString("shoppingcart.cs.24", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %></b>
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align: top;">
                                <b>
                                    <%# Container.DataItemAs<CartItem>().DisplayName%></b>
                            </td>
                            <td style="vertical-align: top;">
                                <asp:Label runat="server" ID="lblDownloadText" Visible="false" CssClass="notificationText"></asp:Label>
                                <asp:Panel runat="server" ID="pnlAddressSelector">
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Panel runat="server" ID="pnlTrueAddressContainer">
                                </asp:Panel>
                                <br />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <hr />
                <asp:Panel ID="pnlCompletePurchase" runat="server" CssClass="button_layout">
                    <uc1:ISEMobileButton ID="btnCompletePurchase" runat="server" OnClick="btnCompletePurchase_Click" />
                </asp:Panel>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
