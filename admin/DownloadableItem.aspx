<%@ Page Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true" CodeFile="DownloadableItem.aspx.cs" Inherits="admin_DownloadableItem" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="pnlMain" Runat="Server">
  <asp:Panel ID="pnlUpload" runat="server" >
        <table>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label ID="lblAllowableSize" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Item Code</td>
                <td>
                    <asp:Label ID="lblItemCode" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Item Name</td>
                <td>
                    <asp:Label ID="lblItemName" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Upload File</td>
                <td>
                <asp:FileUpload ID="fileUpload" runat="server" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    
    <asp:Panel ID="pnlInfo" runat="server" >
        <asp:Label ID="lblDownloadIdCaption" runat="server" Text=""></asp:Label>
        <br />
        <asp:Label ID="lblFileNameCaption" runat="server" Text=""></asp:Label> <asp:Image ID="imgFileType" runat="server" />
        <br />
        <asp:Label ID="lblContentType" runat="server" Text=""></asp:Label> 
        <br />
        <asp:Label ID="lblDownloadSize" runat="server" Text=""></asp:Label>
        <br />
        <asp:Label ID="lblDownloadCaption" runat="server" Text=""></asp:Label>
        <br />
        <asp:Label ID="lblActiveCaption" runat="server" Text=""></asp:Label>
        <br />
    </asp:Panel>
</asp:Content>

