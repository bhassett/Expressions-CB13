<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.recentadditions" CodeFile="recentadditions.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <ise:XmlPackage ID="XmlPackage1" PackageName="page.recentadditions.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="True" EnforceSubscription="true" AllowSEPropogation="True" />
</body>
</html>