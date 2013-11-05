<%@ Page Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" Title="Connected Business eCommerce Admin Site"   %>

<%@ Register src="controls/Widget.ascx" tagname="Widget" tagprefix="uc1" %>

<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>

<asp:Content ID="Content2" ContentPlaceHolderID="pnlMain" Runat="Server">

    <div class="dashboard">
        <ul class="dashboard-col">
            <li><uc1:Widget ID="WidgetStockAlert" runat="server" Title="Stock Alert" Type="StockAlert" MaxHeight="300px" /></li>
            <li><uc1:Widget ID="WidgetVisitors" runat="server" Title="Visitors" Type="Visitors" /></li>
            <li><uc1:Widget ID="WidgetStoreSettings" runat="server" Title="Store Settings" Type="StoreSettings" MaxHeight="100%" /></li>
        </ul>
        <ul class="dashboard-col">
            <li><uc1:Widget ID="WidgetSalesOverview" runat="server" Title="Sales Overview" Type="SalesOverview" MaxHeight="100%" /></li>
            <li><uc1:Widget ID="WidgetNewCustomers" runat="server" Title="New Customers" Type="NewCustomers" MaxHeight="400px" /></li>
        </ul>
        <ul class="dashboard-col">
            <li><uc1:Widget ID="WidgetRecentOrders" runat="server" Title="Recent Web Orders" Type="RecentOrders" /></li>
            <li><uc1:Widget ID="WidgetSales" runat="server" Title="Sales" Type="Sales" MaxHeight="300px" /></li>
        </ul>
    </div>
    
</asp:Content>


