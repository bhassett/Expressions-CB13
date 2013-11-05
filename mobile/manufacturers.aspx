<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.manufacturers" CodeFile="manufacturers.aspx.cs"
    EnableEventValidation="false" %>

<%@ Register TagPrefix="ise" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <ise:XmlPackage ID="Package1" PackageName="page.manufacturers.xml.config" runat="server"
        EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true"
        RuntimeParams="EntityName=Manufacturer" />
</body>
</html>
