<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.bestsellers" CodeFile="bestsellers.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <ise:XmlPackage ID="Package1" PackageName="page.bestsellers.xml.config" runat="server" EnforceDisclaimer="True" EnforcePassword="True" EnforceSubscription="True" AllowSEPropogation="true"/>
</body>
</html>
