<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.news" CodeFile="news.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <ise:XmlPackage ID="XmlPackage1" PackageName="page.news.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="True" EnforceSubscription="true" AllowSEPropogation="True" />
</body>
</html>
