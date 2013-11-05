<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.manufacturers" CodeFile="manufacturers.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head></head>
<body>
<ise:XmlPackage id="Package1" PackageName="entity.manufacturers.xml.config" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true" RuntimeParams="EntityName=Manufacturer"/>
</body>
</html>


