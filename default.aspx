<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce._default" CodeFile="default.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <ise:XmlPackage id="Package1" PackageName="page.default.xml.config" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="True" AllowSEPropogation="True"/>
</body>
</html>
