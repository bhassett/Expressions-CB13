<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.mobile.wishlist" CodeFile="wishlist.aspx.cs" %>

<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<form runat="server" style="float: left; width:100%" onsubmit="return true;">
<b>
    <asp:literal id="RedirectToSignInPageLiteral" runat="server"></asp:literal>
</b>
<ise:Topic runat="server" ID="TopicWishListPageHeader" TopicName="WishListPageHeader" />
<asp:literal id="XmlPackage_WishListPageHeader" runat="server" mode="PassThrough"></asp:literal>
<asp:literal id="XmlPackage_WishListPageTopControlLines" runat="server" mode="PassThrough"
    visible="false"></asp:literal>

<asp:panel id="pnlTopControlLines" runat="server" visible="false">
    <div class="button_layout">
        <uc1:ISEMobileButton ID="btnContinueShopping1" runat="server" />

        <uc1:ISEMobileButton ID="btnUpateWishList1" runat="server" />

    </div>
</asp:panel>

<asp:panel id="Panel1" runat="server" defaultbutton="btnUpateWishList1">
    <asp:Literal ID="CartItems" runat="server" Mode="PassThrough"></asp:Literal>
</asp:panel>

<asp:literal id="Xml_WishListPageBottomControlLines" runat="server" mode="PassThrough" visible="false"></asp:literal>

<asp:panel id="pnlBottomControlLines" runat="server">
    <div class="button_layout">
        <uc1:ISEMobileButton ID="btnUpateWishList2" runat="server" />
        <uc1:ISEMobileButton ID="btnContinueShopping2" runat="server" />
    </div>
</asp:panel>

<br />
<ise:Topic runat="server" ID="TopicWishListPageFooter" TopicName="WishListPageFooter" />
<asp:literal id="Xml_WishListPageFooter" runat="server" mode="PassThrough"></asp:literal>
<script type="text/javascript">
    function AttachClickOnBuyButton(qtyInputId) {
        var qty = $getElement('Quantity_' + qtyInputId).value;
        self.location = 'wishlist.aspx?movetocartid=' + qtyInputId + '&movetocartqty=' + qty;
        return true;
    }

    function KitShowHideDetails(id, linkId, showText, hideText) {
        //var kitDetailId = '.kit_details';
        var kitDetailId = '#' + id;
        var linkbuttonId = '#' + linkId;
        jqueryHideShow(kitDetailId, linkbuttonId, showText, hideText);
    }
</script>
</form>
