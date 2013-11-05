<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.engine" CodeFile="engine.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head></head>
<body>
<ise:XmlPackage id="Package1" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true"/>
</body>
</html>


