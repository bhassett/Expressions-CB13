<%@ Page Language="C#" AutoEventWireup="true" CodeFile="account.aspx.cs" Inherits="InterpriseSuiteEcommerce.account"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators"
    TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.mobile"
    TagPrefix="ise" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<script type="text/javascript">

    var orderHistoryPluginStringKeys = new Object();
    orderHistoryPluginStringKeys.displayText = "account.aspx.43";
    orderHistoryPluginStringKeys.ofText = "account.aspx.29";
    orderHistoryPluginStringKeys.onText = "account.aspx.19";
    orderHistoryPluginStringKeys.orderNumber = "account.aspx.11";
    orderHistoryPluginStringKeys.reorder = "account.aspx.22";
    orderHistoryPluginStringKeys.reorderPrompt = "account.aspx.26";
    orderHistoryPluginStringKeys.resetText = "account.aspx.44";
    orderHistoryPluginStringKeys.shippedText = "account.aspx.18";
    orderHistoryPluginStringKeys.shippingStatus = "account.aspx.14";
    orderHistoryPluginStringKeys.trackingNumber = "account.aspx.32";
    orderHistoryPluginStringKeys.viewing = "account.aspx.28";

</script>
<script type="text/javascript" src="components/order-history/setup.js"></script>
<div class="signin_main">
    <asp:panel id="ErrorPanel" runat="server" visible="False" horizontalalign="Left">
            <asp:Label CssClass="errorLg" ID="ErrorMsgLabel" runat="server" />
    </asp:panel>
    <asp:panel id="pnlShowWishButton" runat="server" cssclass="customlinksLayout">
        <asp:Image ID="redarrow2" AlternateText="" runat="server" ImageUrl="skins/Skin_(!SKINID!)/images/redarrow.gif" />
        <b>
            <asp:HyperLink runat="server" ID="ShowWishListButton" NavigateUrl="wishlist.aspx" Text="(!account.aspx.23!)" class="kitdetaillink" ></asp:HyperLink>
        </b>
    </asp:panel>
    <div class="error">
        <cc1:InputValidatorSummary ID="errorSummary" runat="server" Register="false" />
    </div>
    <form id="AccountForm" runat="server">
    <div class="signin_info">
        <div class="tableHeaderArea">
            <asp:literal id="Label1" runat="server" text="(!account.aspx.5!)" />
        </div>
        <div class="signin_info_body">
            <ise:BillingAddressControl CssClass="billingaddress" ID="ctrlBillingAddress" runat="server"
                ShowAddresses="False" Width="100%" />
            <div class="clear">
            </div>
        </div>
    </div>
    <div class="button_layout">
        <uc1:ISEMobileButton ID="btnUpdateAccount" runat="server" />
    </div>
    <div class="button_layout">
        <uc1:ISEMobileButton ID="btnContinueToCheckOut" runat="server" CausesValidation="false" OnClick="btnContinueToCheckOut_Click" />
    </div>
    <div class="signin_info">
        <div class="tableHeaderArea">
            <asp:literal runat="server" text="(!mobile.account.aspx.8!)"></asp:literal>
        </div>
        <div class="signin_info_body">
            <div class="address_info">
                <asp:literal id="litBillingAddress" runat="server" />
                <br />
                <asp:hyperlink id="lnkChangeBilling" runat="server" cssclass="hyperlinkImageStyle"></asp:hyperlink>
                <br />
            </div>
        </div>
    </div>
    <div class="signin_info">
        <div class="tableHeaderArea">
            <asp:literal runat="server" text="(!mobile.account.aspx.10!)"></asp:literal>
        </div>
        <div class="signin_info_body">
            <asp:panel id="pnlShipping" runat="server">
                <asp:Literal ID="litShippingAddress" runat="server" />
                <br/>
                <asp:HyperLink ID="lnkChangeShipping" runat="Server" CssClass="hyperlinkImageStyle"></asp:HyperLink>
            </asp:panel>
            <br />
            <asp:hyperlink id="lnkAddShippingAddress" runat="server" />
        </div>
    </div>
    <div class="signin_info">
        <div class="tableHeaderArea">
            <asp:literal runat="server" id="lblHeaderOrderHistory"></asp:literal>
        </div>
        <div class="signin_info_body">
            <a id="lnkOrderHistory" href="javascript:void(0)"><asp:literal runat="server" text="(!mobile.account.aspx.34!)"></asp:literal></a>
            <div id="pnlOrderHistory"></div>
        </div>
    </div>
    </form>
</div>