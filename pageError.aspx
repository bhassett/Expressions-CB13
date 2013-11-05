<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pageError.aspx.cs" Inherits="InterpriseSuiteEcommerce.pageError" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <div>
        <div runat="server" class="errorLg"><asp:Literal id="ErrorMessage" runat="server"></asp:Literal></div>
        <hr />
        <ise:XmlPackage id="Package1" PackageName="page.Error.xml.config" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="True" AllowSEPropogation="True"/>
    </div>
</body>
</html>