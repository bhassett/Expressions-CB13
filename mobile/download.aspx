<%@ Page Language="C#" AutoEventWireup="true" CodeFile="download.aspx.cs" Inherits="download" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Download</title>
    <link href="skins/Skin_1/portrait.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="signin_main">
        <div class="signin_info">
            <div class="tableHeaderArea">
                <span>Download</span>            
            </div>
            <div class="signin_info_body">
                <div id="divSignInPrompt" align="center" runat="server">
                    <asp:Label ID="lblSignInCaption" runat="server" Text=""></asp:Label>
                    <br />
                    <br />
                </div>
                <br />
                <div runat="server" id="divDownload" align="center">
                    <b>
                        <asp:Label ID="lblCaption" runat="server" Text=""></asp:Label></b>
                    <br />
                    <br />
                    <asp:Image ID="imgCaptcha" runat="server" />
                    <br />
                    <br />
                    <asp:Label CssClass="errorLg" ID="lblError" runat="server" Text=""></asp:Label>
                    <asp:TextBox ID="txtCaptcha" runat="server" CssClass="inputTextBox" ></asp:TextBox>
                    <br />
                    <br />
                    <uc1:ISEMobileButton ID="btnDownload" runat="server" ValidationGroup="Group1" />
                </div>
            </div>
        </div>
    </form>
    </div>
</body>
</html>
