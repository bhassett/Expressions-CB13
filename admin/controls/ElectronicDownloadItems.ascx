<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ElectronicDownloadItems.ascx.cs" Inherits="admin_controls_ElectronicDownloadItems" %>
<asp:GridView ID="grdList" 
    runat="server" 
    AutoGenerateColumns="False" 
    CellPadding="4" 
    OnRowDataBound="Grid_RowDataBound"
    GridLines="None" ShowFooter="true"
    Width="100%" 
    CssClass="gv">
    <Columns>
        <asp:BoundField DataField="FileName" HeaderText="File Name" FooterText="&nbsp;" />
        <asp:HyperLinkField DataTextField="ItemCode" HeaderText="Item Code" FooterText="&nbsp;" />
        <asp:BoundField DataField="ItemName" HeaderText="Item Name" FooterText="&nbsp;" />
        <asp:BoundField DataField="ItemDescription" HeaderText="Description" NullDisplayText="N/A" FooterText="&nbsp;" />
        <asp:BoundField HeaderText="Download Id" FooterText="&nbsp;" />
        <asp:BoundField DataField="UnitsInStock" HeaderText="In Stock" NullDisplayText="N/A" FooterText="&nbsp;" />
        <asp:BoundField DataField="IsActive" HeaderText="Status" FooterText="&nbsp;" />
    </Columns><PagerTemplate>test </PagerTemplate>
    <RowStyle CssClass="gv-row gv-col"/>
    <EditRowStyle CssClass="gv-editrow" />
    <PagerStyle CssClass="gv-pager" />
    <HeaderStyle CssClass="gv-header" HorizontalAlign="Left" />
    <FooterStyle CssClass="gv-footer" />
    <AlternatingRowStyle CssClass="gv-altrow gv-col" />
</asp:GridView>
