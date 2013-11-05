<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="InterpriseSuiteEcommerce._default" %>

<%@ Register TagPrefix="ise" TagName="XmlPackage" src="../XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <div class="signin_main" runat="server" >
        <ise:XmlPackage id="Package1" 
                PackageName="page.default.xml.config" 
                runat="server" 
                EnforceDisclaimer="true" 
                EnforcePassword="true" 
                EnforceSubscription="True" 
                AllowSEPropogation="True" />
    </div>
</body>
</html>