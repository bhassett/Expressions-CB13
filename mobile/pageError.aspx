<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pageError.aspx.cs" Inherits="InterpriseSuiteEcommerce.pageError" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>
<html>
<head>
</head>
<body>
    <div runat="server" class="signin_main removeMarginTop">

        <div runat="server" class="signin_info">

            <div runat="server" class="errorLg tableHeaderArea">
                <asp:Literal id="ErrorMessage" runat="server"></asp:Literal>
            </div>

            <div class="signin_info_body">

                <ise:XmlPackage id="Package1" PackageName="page.Error.xml.config" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="True" AllowSEPropogation="True"/>

            </div>

        </div>

    </div>
</body>
</html>