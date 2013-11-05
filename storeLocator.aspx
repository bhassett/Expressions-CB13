<%@ Page Language="C#" AutoEventWireup="true" CodeFile="storeLocator.aspx.cs"
    Inherits="InterpriseSuiteEcommerce.storeLocator" %>

<%@ Register Src="UserControls/StoreLocator/StoreLocatorControl.ascx" TagName="StoreLocatorControl" TagPrefix="ctrl" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<html>
<head>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ctrl:StoreLocatorControl ID="ctrlStoreLocator" runat="server" />
    </div>
    </form>
</body>
</html>
