<%@ Page Title="" Language="C#" ValidateRequest="false" MasterPageFile="~/admin/default.master" AutoEventWireup="true" CodeFile="AppConfigs.aspx.cs" Inherits="admin_AppConfigs" %>
<%@ Import Namespace="System.Data" %>

<asp:Content ID="cntMain" ContentPlaceHolderID="pnlMain" Runat="Server">

    <asp:UpdatePanel ID="upInsertUpdatePanel" runat="server" RenderMode="Block" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            
            <!-- Add Form -->
            <asp:Panel ID="pnlAddNew" runat="server" Visible="false" CssClass="createform" DefaultButton="btnSave">
                <div class="form-header">Add Application Configuration</div>
                <br />
                <div class="form-horizontal">
                    <div class="control-group">
                        <asp:Label ID="lblName" runat="server" Text="Name" CssClass="control-label"></asp:Label>
                        <div class="controls"><asp:TextBox ID="txtName" runat="server" MaxLength="100" Width="396px"></asp:TextBox></div>
                    </div>
                    <div class="control-group">
                        <asp:Label ID="lblGroupName" runat="server" Text="Group Name" CssClass="control-label"></asp:Label>
                        <div class="controls"><asp:TextBox ID="txtGroupName" runat="server" MaxLength="100" Width="396px"></asp:TextBox></div>
                    </div>
                    <div class="control-group">
                        <asp:Label ID="lblConfigValue" runat="server" Text="Value" CssClass="control-label"></asp:Label>
                        <div class="controls"><asp:TextBox ID="txtConfigValue" runat="server" TextMode="MultiLine" MaxLength="1000" Width="396px" Rows="5"></asp:TextBox></div>
                    </div>
                    <div class="control-group">
                        <asp:Label ID="lblDescription" runat="server" Text="Description" CssClass="control-label"></asp:Label>
                        <div class="controls"><asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" MaxLength="1073741823" Width="396px" Rows="5"></asp:TextBox></div>
                    </div>
                </div>
                <div class="form-footer">
                    <asp:Button ID="btnSave" runat="server" Text="Save New" OnClick="btnSave_Click" CssClass="btn" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn" />
                </div>
            </asp:Panel>

            <!-- Main Header -->
            <div class="content-header">
                <div class="title">Application Configuration</div>
                <div class="tools">
                    <asp:Panel ID="pnlCommands" runat="server" CssClass="commands" DefaultButton="btnSearch">
                        <div class="input-append menu">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="search"></asp:TextBox>
                            <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="Search" OnClick="btnSearch_Click" />
                        </div>
                        <div class="menu">
                            <asp:Button ID="btnInsert" runat="server" CssClass="btn" Text="Add New" OnClick="btnInsert_OnClick"></asp:Button>
                        </div>
                    </asp:Panel>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="upMainUpdatePanel" runat="server" RenderMode="Inline" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:GridView ID="gvAppConfig" runat="server" 
                DataKeyNames="AppConfigGUID" 
                AutoGenerateColumns="false"
                EmptyDataText="There were no records found." 
                CssClass="gv" 
                AllowSorting="true"
                AutoGenerateEditButton="true" 
                PageSize="20" 
                AllowPaging="true" 
                GridLines="None"
                OnPageIndexChanging="gvAppConfig_PageIndexChanging"
                OnSorting="gvAppConfig_Sorting" 
                OnRowEditing="gvAppConfig_RowEditing"
                OnRowUpdating="gvAppConfig_RowUpdating" 
                OnRowCancelingEdit="gvAppConfig_RowCancelingEdit" Width="100%">
                <Columns>
                    <%--Name Column--%>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server"
                                Text='<%# HttpUtility.HtmlEncode(((DataRowView)Container.DataItem)["Name"].ToString()) %>'
                                ToolTip='<%# ((DataRowView)Container.DataItem)["Name"].ToString() %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server" MaxLength="100" ReadOnly="true"
                                Text='<%# ((DataRowView)Container.DataItem)["Name"].ToString() %>'>
                            </asp:TextBox>
                        </EditItemTemplate>
                        <ItemStyle CssClass="gridNameCell" />
                    </asp:TemplateField>
                    <%--Group Name Column--%>
                    <asp:TemplateField HeaderText="Group Name" SortExpression="GroupName">
                        <ItemTemplate>
                            <asp:Label ID="lblGroupName" runat="server"
                                Text='<%# HttpUtility.HtmlEncode(((DataRowView)Container.DataItem)["GroupName"].ToString()) %>'
                                ToolTip='<%# ((DataRowView)Container.DataItem)["GroupName"].ToString() %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtGroupName" runat="server" MaxLength="100" ReadOnly="true"
                                Text='<%# ((DataRowView)Container.DataItem)["GroupName"].ToString() %>'>
                            </asp:TextBox>
                        </EditItemTemplate>
                        <ItemStyle CssClass="gridGroupNameCell" />
                    </asp:TemplateField>                    
                    <%--Config Value Column--%>
                    <asp:TemplateField HeaderText="Value" SortExpression="ConfigValue">
                        <ItemTemplate>
                            <asp:Label ID="lblConfigValue" runat="server"
                                Text='<%# HttpUtility.HtmlEncode(((DataRowView)Container.DataItem)["ConfigValue"].ToString()) %>'
                                ToolTip='<%# ((DataRowView)Container.DataItem)["ConfigValue"].ToString() %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtConfigValue" runat="server" TextMode="MultiLine" 
                                MaxLength="1000" Rows="4" 
                                Text='<%# ((DataRowView)Container.DataItem)["ConfigValue"].ToString() %>'>
                            </asp:TextBox>
                        </EditItemTemplate>
                        <ItemStyle CssClass="gridConfigValueCell" />
                    </asp:TemplateField>
                    <%--Description Column--%>
                    <asp:TemplateField HeaderText="Description" SortExpression="Description">
                        <ItemTemplate>
                            <asp:Label ID="lblDescription" runat="server"
                                Text='<%# HttpUtility.HtmlEncode(((DataRowView)Container.DataItem)["Description"].ToString()) %>'
                                ToolTip='<%# ((DataRowView)Container.DataItem)["Description"].ToString() %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" ReadOnly="true"
                                MaxLength="1073741823" Width="98%" Rows="4"
                                Text='<%# ((DataRowView)Container.DataItem)["Description"].ToString() %>'>
                            </asp:TextBox>
                        </EditItemTemplate>
                        <ItemStyle CssClass="gridDescriptionCell" />
                    </asp:TemplateField>
                </Columns>
                <RowStyle CssClass="gv-row gv-col"/>
                <EditRowStyle CssClass="gv-editrow" />
                <PagerStyle CssClass="gv-pager" />
                <HeaderStyle CssClass="gv-header" HorizontalAlign="Left" />
                <FooterStyle CssClass="gv-footer" />
                <AlternatingRowStyle CssClass="gv-altrow gv-col" />  
                <PagerSettings Mode="NumericFirstLast" Position="Bottom" />
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSave" />
            <asp:AsyncPostBackTrigger ControlID="btnSearch" />
        </Triggers>
    </asp:UpdatePanel>        
    
    <asp:UpdatePanel ID="upAlerts" runat="server" RenderMode="Inline" UpdateMode="Always">
        <ContentTemplate>
            <asp:HiddenField ID="hfAlert" runat="server" Visible="false" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

