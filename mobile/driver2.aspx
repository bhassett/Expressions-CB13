<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.driver2" CodeFile="driver2.aspx.cs" EnableEventValidation="false"%>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<script language="C#" runat="server">
    // NOTE TO DEVELOPERS:
    // this page cannot have the html header and footer...as
    // the TOPIC HTML ITSELF, read from the db or topic file, will be providing that inside it's "html"
</script>
<ise:Topic id="Topic1" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true"/>
