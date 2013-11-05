<%@ Page Language="C#" AutoEventWireup="true" CodeFile="viewregistry.aspx.cs" Inherits="InterpriseSuiteEcommerce.viewregistry" %>

<%@ OutputCache Location="None" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<%@ Register Src="UserControls/GiftRegistry/GiftRegistryViewForm.ascx" TagName="GiftRegistryViewForm"
    TagPrefix="ctrl" %>
<%@ Register Src="UserControls/GiftRegistry/GiftRegistryViewItemList.ascx" TagName="GiftRegistryViewItemList"
    TagPrefix="ctrl" %>
<head>
    <title></title>
</head>
<body>
    <script type="text/javascript" src="jscripts/imagezoom.js">
    </script>
    <asp:Panel ID="pnlMain" runat="server" CssClass="pnlmain">
        <!-- Registry Info -->
        <div class="sections-place-holder">
            <div class="page-sections-head"><%= AppLogic.GetString("viewregistry.aspx.1", SkinID, ThisCustomer.LocaleSetting)%></div>
            <asp:Panel CssClass="error-message-wrapper" runat="server" ID="pnlErrorMessage" Visible="false"></asp:Panel>
            <ctrl:GiftRegistryViewForm ID="ctrlGiftRegistryViewForm" runat="server" />
        </div>
        <br />

        <!-- Registry Items -->
        <div class="sections-place-holder"  runat="server" id="pnlItems">
            <div class="page-sections-head"><%= AppLogic.GetString("editgiftregistry.aspx.20", SkinID, ThisCustomer.LocaleSetting)%></div>
            <ctrl:GiftRegistryViewItemList ID="ctrGiftRegistryViewItemList" runat="server" />
            <br />
        </div>
        <br />
            
        <!-- Registry Options -->
        <div class="sections-place-holder" runat="server" id="pnlOptionItems">
            <div class="page-sections-head"> <%= AppLogic.GetString("editgiftregistry.aspx.36", SkinID, ThisCustomer.LocaleSetting)%></div>
            <ctrl:GiftRegistryViewItemList ID="GiftRegistryViewItemOptionList" runat="server" />
            <br />
        </div>
    </asp:Panel>

    <form runat="server" id="frmAuthenticate">
    <asp:Panel runat="server" ID="pnlPassword" Visible="false">

    <div class="sections-place-holder">
        <div class="page-sections-head"><%= AppLogic.GetString("viewregistry.aspx.5", SkinID, ThisCustomer.LocaleSetting)%></div>
         <div style="padding:10px;">
            <span style="font-size:8pt;"><%= AppLogic.GetString("viewregistry.aspx.4", SkinID, ThisCustomer.LocaleSetting)%></span>&nbsp;
            <asp:TextBox TextMode="Password" CssClass="light-style-input" ID="txtPassword" runat="server" Width="200px"></asp:TextBox>
            <br />
            <asp:Label runat="server" ID="lblError" CssClass="error" Visible="false"></asp:Label>
         </div>
         <div class="button-place-holder">
            <asp:Button runat="server" ID="btnAuthenticate" CssClass="site-button content" Text="Continue" />&nbsp;&nbsp;&nbsp;
            <div class="clear-both height-17"></div>
         </div>
    </div>
    </asp:Panel>
    </form>
</body>