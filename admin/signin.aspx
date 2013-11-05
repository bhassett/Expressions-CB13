<%@ Page Language="C#" AutoEventWireup="true" CodeFile="signin.aspx.cs" Inherits="admin_signin" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connected Business eCommerce Admin Site - Login</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="styles/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="skins/Skin_2/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="mainForm" runat="server">
        <div class="signin">
            <div class="logo"></div>
            <div class="content">
                <asp:Login  ID="loginCtrl" 
                            CssClass="loginform" 
                            runat="server" 
                            TitleText=""
                            DestinationPageUrl ="~/admin/default.aspx" 
                            DisplayRememberMe="False">
                    <TextBoxStyle CssClass="input" />
                    <LabelStyle CssClass="caption" />
                    <LoginButtonStyle CssClass="btn login" />
                    <ValidatorTextStyle CssClass="text-error" />
                </asp:Login>
                <asp:Label ID="lblError" runat="server" CssClass="text-error"></asp:Label>  
            </div>
        </div>
    </form>
</body>
</html>