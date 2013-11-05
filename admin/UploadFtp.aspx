<%@ Page Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true" CodeFile="UploadFtp.aspx.cs" Inherits="admin_UploadFtp" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="pnlMain" Runat="Server">
    <div class="createform">
        <div class="form-header">Files Uploaded</div>
        <div class="form-horizontal">
            <asp:Panel ID="pnlUnMapped" runat="server">
                <asp:GridView ID="grdUnMapped" 
                    runat="server" 
                    AutoGenerateColumns="False" 
                    CellPadding="4" 
                    GridLines="None" 
                    OnRowDataBound="GridUnmapped_RowDataBound" 
                    Width="100%">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <Columns>
                        <asp:BoundField DataField="FileName" HeaderText="File Name" />
                        <asp:BoundField DataField="ContentType" HeaderText="Content Type" />
                        <asp:BoundField DataField="ContentLength" DataFormatString="{0} kb" HeaderText="Size" />
                        <asp:TemplateField HeaderText="Map to this Item">
                            <ItemTemplate>
                                <asp:HiddenField ID="map" runat="server" />
                                <asp:DropDownList ID="cboItemCode" runat="server"></asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="gv-row gv-col"/>
                    <EditRowStyle CssClass="gv-editrow" />
                    <PagerStyle CssClass="gv-pager" />
                    <HeaderStyle CssClass="gv-header" HorizontalAlign="Left" />
                    <FooterStyle CssClass="gv-footer" />
                    <AlternatingRowStyle CssClass="gv-altrow gv-col" />  
                </asp:GridView>
                
            </asp:Panel>
            <asp:Panel ID="pnlMapped" runat="server">
                <asp:GridView ID="grdMapped" 
                    runat="server" 
                    AutoGenerateColumns="False" 
                    CellPadding="4"
                    GridLines="None" 
                    OnRowDataBound="GridMapped_RowDataBound" 
                    Width="100%">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <Columns>
                        <asp:BoundField DataField="FileName" HeaderText="File Name" />
                        <asp:HyperLinkField DataTextField="ItemCode" HeaderText="Item Code" />
                        <asp:BoundField HeaderText="Download Id" />
                    </Columns>
                    <RowStyle CssClass="gv-row gv-col"/>
                    <EditRowStyle CssClass="gv-editrow" />
                    <PagerStyle CssClass="gv-pager" />
                    <HeaderStyle CssClass="gv-header" HorizontalAlign="Left" />
                    <FooterStyle CssClass="gv-footer" />
                    <AlternatingRowStyle CssClass="gv-altrow gv-col" />  
                </asp:GridView>
            </asp:Panel>
            <asp:Label ID="lblError" runat="server" CssClass="alert-error" Text=""></asp:Label>
            <div class="form-footer">
                <asp:Button ID="btnScan" runat="server" Text="Scan" OnClick="btnScan_Click" CssClass="btn" />
                <asp:Button ID="btnMap" runat="server" Text="Assign" OnClick="btnMap_Click" Visible="False" CssClass="btn" />
            </div>
        </div>
    </div>
</asp:Content>

