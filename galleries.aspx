<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.galleries" CodeFile="galleries.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <ise:XmlPackage ID="XmlPackage1" PackageName="page.galleries.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="True" EnforceSubscription="true" AllowSEPropogation="True" />
</body>
</html>
