<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.wishlist" CodeFile="wishlist.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>

    <form runat="server" onsubmit="return FormValidator(this)">
        <b><asp:Literal ID="RedirectToSignInPageLiteral" runat="server"></asp:Literal></b>
        <ise:Topic runat="server" ID="TopicWishListPageHeader" TopicName="WishListPageHeader" />
        &nbsp;
        <asp:Literal ID="XmlPackage_WishListPageHeader" runat="server" Mode="PassThrough"></asp:Literal>

        <asp:Literal ID="XmlPackage_WishListPageTopControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
        
        <asp:Panel ID="pnlTopControlLines" runat="server" Visible="false">
            <table cellspacing="3" cellpadding="0" width="100%" border="0" id="table1">
                <tr>
                    <td valign="bottom" align="right">
                        <asp:Button ID="btnContinueShopping1" runat="server" CssClass="site-button content" OnClick="btnContinueShopping1_Click" />
                        <asp:Button ID="btnUpateWishList1" runat="server" CssClass="site-button content btn btn-success" OnClick="btnUpateWishList1_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>       
        
        <br />
        <asp:Panel ID="Panel1" runat="server" DefaultButton="btnUpateWishList1" class="wishlist-wrapper">
            <asp:Table ID="tblWishList" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                        <asp:Image ID="wishlist_gif" runat="server" AlternateText="" /><br />

                        <asp:Table ID="tblWishListBox" CellSpacing="0" CellPadding="4" Width="100%" runat="server" CssClass="wishlist">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                    <asp:Literal ID="CartItems" runat="server" Mode="PassThrough"></asp:Literal>                                
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        
        <asp:Literal ID="Xml_WishListPageBottomControlLines" runat="server" Mode="PassThrough" Visible="false"></asp:Literal>
        
        <asp:Panel ID="pnlBottomControlLines" runat="server">
            <table cellspacing="3" cellpadding="0" width="100%" border="0" id="table2">
                <tr>
                    <td valign="bottom" align="right">
                        <br />
                        <asp:Button ID="btnContinueShopping2" runat="server" CssClass="site-button content" OnClick="btnContinueShopping2_Click"/>
                        <asp:Button ID="btnUpateWishList2" runat="server" CssClass="site-button content" OnClick="btnUpateWishList2_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        
        <ise:Topic runat="server" ID="TopicWishListPageFooter" TopicName="WishListPageFooter" />
        <asp:Literal ID="Xml_WishListPageFooter" runat="server" Mode="PassThrough"></asp:Literal>
    </form>
</body>
</html>
