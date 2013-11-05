<%@ Page Language="C#" AutoEventWireup="true" CodeFile="download.aspx.cs" Inherits="download" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Download</title>
    <link href="skins/Skin_(!SKINID!)/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="divSignInPrompt" runat="server">
        <center>
            <asp:Label ID="lblSignInCaption" runat="server" Text=""></asp:Label>
        </center>
    </div>
    <div id="divDownload" runat="server">
        <center>
            <table>
                <tr>
                    <td colspan="3">
                        <asp:Label CssClass="errorLg" ID="lblError" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblCaption" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Image ID="imgCaptcha" runat="server" />
                    </td>
                    <td align="right" valign="top">
                        <asp:TextBox ID="txtCaptcha" runat="server" OnTextChanged="txtCaptcha_TextChanged"></asp:TextBox>
                    </td>
                    <td align="left" valign="top">
                        <asp:Button ID="btnDownload" runat="server" Text="Download" OnClick="btnDownload_Click"
                            ValidationGroup="Group1" />
                    </td>
                </tr>
            </table>
        </center>
    </div>
    </form>
</body>
</html>
