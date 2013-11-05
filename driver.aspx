<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.driver" CodeFile="driver.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head></head>
<body>
<ise:Topic id="Topic1" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true"/>
</body>
</html>
