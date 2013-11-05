<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.sitemap" CodeFile="sitemap.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <asp:Panel runat="server" ID="PackagePanel" Visible="True">
        <ise:xmlpackage id="XmlPackage1" runat="server" enforcedisclaimer="true" enforcepassword="True" enforcesubscription="true" allowsepropogation="True" />
    </asp:Panel>
    <asp:Panel runat="server" ID="EntityPanel" Visible="False">
        <asp:Literal runat="Server" ID="Literal1" />
    </asp:Panel>
</body>
</html>
