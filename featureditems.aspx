<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.featureditems" CodeFile="featureditems.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <ise:XmlPackage ID="XmlPackage1" PackageName="page.featureditems.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="True" EnforceSubscription="true" AllowSEPropogation="True" />
</body>
</html>