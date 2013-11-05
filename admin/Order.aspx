<%@ Page Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true"  CodeFile="Order.aspx.cs" Inherits="Order" Title="Untitled Page" %>

<asp:Content ID="Content2" ContentPlaceHolderID="pnlMain" Runat="Server">
    &nbsp;<dxwc:ReportToolbar ID="ReportToolbar1" runat="server" ShowDefaultButtons="False" ReportViewer="<%# rptVyuOrder %>">
        <Items>
            <dxwc:ReportToolbarButton ItemKind='Search' ToolTip='Display the search window' />
            <dxwc:ReportToolbarSeparator />
            <dxwc:ReportToolbarButton ItemKind='PrintReport' ToolTip='Print the report' />
            <dxwc:ReportToolbarButton ItemKind='PrintPage' ToolTip='Print the current page' />
            <dxwc:ReportToolbarSeparator />
            <dxwc:ReportToolbarButton Enabled='False' ItemKind='FirstPage' ToolTip='First Page' />
            <dxwc:ReportToolbarButton Enabled='False' ItemKind='PreviousPage' ToolTip='Previous Page' />
            <dxwc:ReportToolbarLabel Text='Page' />
            <dxwc:ReportToolbarComboBox ItemKind='PageNumber'>
            </dxwc:ReportToolbarComboBox>
            <dxwc:ReportToolbarLabel Text='of' />
            <dxwc:ReportToolbarTextBox IsReadOnly='True' ItemKind='PageCount' />
            <dxwc:ReportToolbarButton ItemKind='NextPage' ToolTip='Next Page' />
            <dxwc:ReportToolbarButton ItemKind='LastPage' ToolTip='Last Page' />
            <dxwc:ReportToolbarSeparator />
            <dxwc:ReportToolbarButton ItemKind='SaveToDisk' ToolTip='Export a report and save it to the disk' />
            <dxwc:ReportToolbarButton ItemKind='SaveToWindow' ToolTip='Export a report and show it in a new window' />
            <dxwc:ReportToolbarComboBox ItemKind='SaveFormat'>
                <Elements>
                    <dxwc:ListElement Text='Pdf' Value='pdf' />
                    <dxwc:ListElement Text='Xls' Value='xls' />
                    <dxwc:ListElement Text='Rtf' Value='rtf' />
                    <dxwc:ListElement Text='Mht' Value='mht' />
                    <dxwc:ListElement Text='Text' Value='txt' />
                    <dxwc:ListElement Text='Csv' Value='csv' />
                    <dxwc:ListElement Text='Image' Value='png' />
                </Elements>
            </dxwc:ReportToolbarComboBox>
        </Items>
        <Styles>
            <LabelStyle>
                <Margins MarginLeft='3px' MarginRight='3px' />
            </LabelStyle>
        </Styles>
    </dxwc:ReportToolbar>
    <br />
    <dxwc:reportviewer id="rptVyuOrder" runat="server"></dxwc:reportviewer>
    <br />
    <br />
</asp:Content>

