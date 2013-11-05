<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OpenSalesOrderControl.ascx.cs" Inherits="admin_controls_OpenSalesOrderControl" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<asp:GridView ID="grdList" 
    runat="server" 
    AutoGenerateColumns="False" 
    CellPadding="4"
    GridLines="None" 
    Width="100%" 
    EmptyDataText="There was no record found" 
    OnRowDataBound="Grid_RowDataBound" CssClass="gv">
    <Columns>
        <asp:HyperLinkField DataNavigateUrlFields="SalesOrderCode" DataNavigateUrlFormatString="~/admin/order.aspx?order={0}"
            DataTextField="SalesOrderCode" HeaderText="Sales Order Code" Text="Sales Order Code" />
        <asp:BoundField DataField="CustomerCode" HeaderText="Customer Code" />
        <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" />
        <asp:BoundField DataField="Date" DataFormatString="{0:dd-MM-yy}" HeaderText="Date" />
        <asp:BoundField DataField="Total" HeaderText="Total" />
        <asp:BoundField DataField="Currency" HeaderText="Currency" />
    </Columns>
    <RowStyle CssClass="gv-row gv-col"/>
    <EditRowStyle CssClass="gv-editrow" />
    <PagerStyle CssClass="gv-pager" />
    <HeaderStyle CssClass="gv-header" HorizontalAlign="Left" />
    <FooterStyle CssClass="gv-footer" />
    <AlternatingRowStyle CssClass="gv-altrow gv-col" />  
</asp:GridView>
