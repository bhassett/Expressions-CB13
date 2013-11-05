<%@ Page Title="" Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true" CodeFile="Items.aspx.cs" Inherits="admin_Items" %>
<%@ MasterType VirtualPath="~/admin/default.master" %>

<%@ Register Src="controls/OpenSalesOrderControl.ascx" TagName="OpenSalesOrderControl"
    TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="pnlMain" Runat="Server">

 <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div style="display:none;">
    <!-- Sales Order -->
    <span id="spanOpenSalesOrderFilter"></span>
    <div class="content-header">
                <div class="title">Open Sales Order</div>
                <div class="tools">
                    <input onclick="LoadManager.loadFilter(OpenSalesOrderRequest);" type="button" value="Apply Filter" class="btn" />
                </div>
    </div>
    <div id="divOpenSalesOrderModel"></div>

    <br />

    <!-- Customers -->
    <span id="spanActiveCustomerFilter"></span>
    <div class="content-header">
                <div class="title">Active Customers</div>
                <div class="tools">
                    <input onclick="LoadManager.loadFilter(ActiveCustomerRequest);" type="button" value="Apply Filter" class="btn" />
                </div>
    </div>
    <div id="divActiveCustomerModel"></div>
    
    <br />
    </div>
    <!-- Electronic Download Items -->
    <span id="spanElectronicDownloadItemFilter"></span>
    <div class="content-header">
                <div class="title">Electronic Download Items</div>
                <div class="tools">
                    <input onclick="LoadManager.loadFilter(ElectronicDownloadItemRequest);" type="button" value="Apply Filter" class="btn" />
                </div>
    </div>
    <div id="divElectronicDownloadItemModel"></div>
    

</asp:Content>

