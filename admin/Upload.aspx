<%@ Page Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true" CodeFile="Upload.aspx.cs" Inherits="admin_Upload" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="pnlMain" Runat="Server">
    <asp:Panel ID="pnlUpload" runat="server" CssClass="createform" >
        <div class="form-header">Upload File</div>
            <div class="form-horizontal">
                <div class="control-group">
                    <div class="alert alert-info"><asp:Label ID="lblAllowableSize" runat="server" Text=""></asp:Label></div>
                </div>
                <div class="control-group">
                    <span class="control-label">Item Code</span>
                    <div class="controls"><asp:DropDownList ID="cboItemCode" runat="server"></asp:DropDownList></div>
                </div>
                <div class="control-group">
                        <span class="control-label">File to Upload</span>
                    <div class="controls"><asp:FileUpload ID="fileUpload" runat="server" CssClass="btn-file" /></div>
                </div>
                 <asp:Panel ID="pnlInfo" runat="server" Visible="false" >
                    <div class="control-group">
                    <div class="alert alert-success">
                        <asp:Label ID="lblDownloadIdCaption" runat="server" Text=""></asp:Label>
                        <br />
                        <asp:Label ID="lblFileNameCaption" runat="server" Text=""></asp:Label>
                        <br />
                        <asp:Label ID="lblContentType" runat="server" Text=""></asp:Label> 
                        <br />
                        <asp:Label ID="lblDownloadSize" runat="server" Text=""></asp:Label>
                        <br />
                        <asp:Label ID="lblDownloadCaption" runat="server" Text=""></asp:Label>
                        <br />
                        <asp:Label ID="lblActiveCaption" runat="server" Text=""></asp:Label>
                    </div>
                    </div>
                </asp:Panel>
            </div>
            <div class="form-footer">
                <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" CssClass="btn" />
            </div>
    </asp:Panel>
    
    
    
</asp:Content>

