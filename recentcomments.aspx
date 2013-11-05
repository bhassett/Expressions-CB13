<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.recentcomments" CodeFile="recentcomments.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <ise:XmlPackage ID="XmlPackage1" PackageName="page.recentcomments.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="True" EnforceSubscription="true" AllowSEPropogation="True" />
</body>
</html>