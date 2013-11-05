<%@ Page Language="C#" CodeFile="giftregistry.aspx.cs" Inherits="InterpriseSuiteEcommerce.giftregistry" %>

<%@ Register Src="UserControls/GiftRegistry/GiftRegistryList.ascx" TagName="GiftRegistryList"
    TagPrefix="ctrl" %>
<html>
<head>
    <title></title>
</head>
<body>
    <asp:Panel ID="pnlMain" runat="server" CssClass="pnlmain">

        <div class="sections-place-holder">
            <div class="page-sections-head"><asp:Literal runat="server" ID="litRegistryHeader"></asp:Literal></div>
            <div>
                <ctrl:GiftRegistryList ID="GiftRegistryList1" runat="server" />
            </div>
            <br />
        </div>
        <div style="text-align:right; margin-top:15px;">
            <!-- Create Registry Button -->
            <a href="editgiftregistry.aspx">
                <span class="site-button content" style="padding:7px;">
                    <asp:Literal runat="server" ID="litCreateRegistry"></asp:Literal>
                </span>
            </a>
            &nbsp;
            <!-- Find Registry Button -->
            <a href="findgiftregistry.aspx">
                <span class="site-button content" style="padding:7px;">
                    <asp:Literal runat="server" ID="litFindRegistry"></asp:Literal>
                </span>
            </a>
        </div>
        
    </asp:Panel>
</body>
</html>
