<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sharegiftregistry.aspx.cs" ValidateRequest="false"
    Inherits="InterpriseSuiteEcommerce.sharegiftregistry" %>

<%@ OutputCache Location="None" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<%@ Register Src="UserControls/GiftRegistry/GiftRegistryShareForm.ascx" TagName="GiftRegistryShareForm"
    TagPrefix="ctrl" %>
<head>
    <title></title>
</head>
<body>

    <form id="Form1" runat="server">
    <asp:Panel ID="pnlMain" runat="server">
        
        <div class="sections-place-holder">

            <div class="page-sections-head"><%= AppLogic.GetString("editgiftregistry.aspx.37", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></div>

            <div class="title-container" style="text-align:center;margin-bottom:0px; text-align:center; border-bottom:dashed 1px #ccc; margin:0px; background-color:#efefef; ">
                <h1><%= Title %></h1>
                <span><%= StartDate.Value.ToShortDateString() %> to <%= EndDate.Value.ToShortDateString() %></span>
            </div>

            <div style="padding:10px;">
                <asp:Panel CssClass="error-message-wrapper" runat="server" ID="pnlErrorMessage" Visible="false"></asp:Panel>
                <span style="font-size:9pt;"><%= AppLogic.GetString("editgiftregistry.aspx.38", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
                 
                <ctrl:GiftRegistryShareForm ID="ctrlGiftRegistryShareForm" runat="server" />

            </div>

             <div class="button-place-holder">
                <asp:Button runat="server" ID="btnSend" Text="Send" CssClass="site-button" ValidationGroup="emailValidationGrp" />&nbsp;&nbsp;&nbsp;
                <div class="clear-both height-17"></div>
            </div>

        </div>
        
    </asp:Panel>
    </form>
</body>
