<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.driver" CodeFile="driver.aspx.cs" EnableEventValidation="false" %>

<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>
    <div runat="server" class="signin_main" id="test">
        <div class="signin_info">
            <div class="signin_info_body">
                <ise:Topic ID="Topic1" runat="server" EnforceDisclaimer="true" EnforcePassword="true"
                    EnforceSubscription="true" AllowSEPropogation="true" />
            </div>
        </div>
    </div>
</body>
</html>
