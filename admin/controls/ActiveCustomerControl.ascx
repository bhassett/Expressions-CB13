<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ActiveCustomerControl.ascx.cs" Inherits="admin_controls_ActiveCustomerControl" %>
<asp:GridView ID="grdList" 
    runat="server" 
    AutoGenerateColumns="False" 
    CellPadding="4"
    GridLines="None" 
    CssClass="gv" 
    Width="100%">
    <Columns>
        <asp:HyperLinkField DataTextField="CustomerCode" HeaderText="Customer Code" />
        <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" />
        <asp:BoundField DataField="CustomerAddress" HeaderText="Address" NullDisplayText="N/A" />
        <asp:BoundField DataField="CustomerCity" HeaderText="City" NullDisplayText="N/A" />
        <asp:BoundField DataField="CustomerState" HeaderText="State" NullDisplayText="N/A" />
        <asp:BoundField DataField="CustomerPostalCode" HeaderText="Postal" NullDisplayText="N/A" />
        <asp:BoundField DataField="CustomerCountry" HeaderText="Country" NullDisplayText="N/A" />
    </Columns>
   <RowStyle CssClass="gv-row gv-col"/>
    <EditRowStyle CssClass="gv-editrow" />
    <PagerStyle CssClass="gv-pager" />
    <HeaderStyle CssClass="gv-header" HorizontalAlign="Left" />
    <FooterStyle CssClass="gv-footer" />
    <AlternatingRowStyle CssClass="gv-altrow gv-col" />  
</asp:GridView>
