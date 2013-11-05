<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterControl2.ascx.cs" Inherits="admin_controls_FilterControl2"  EnableViewState="false" %>
<asp:Panel ID="pnlFilterPopup"  CssClass="createform" runat="server" >

        
                <div class="form-header">General</div>
                <br />
                <div class="form-horizontal">
                    <div class="control-group">
                        <span class="control-label">Top Count</span>
                        <div class="controls"> <asp:TextBox ID="txtTopCount" runat="server" ></asp:TextBox></div>
                    </div>
                    <div class="control-group">
                        <span class="control-label">Order By</span>
                        <div class="controls">
                            <asp:DropDownList ID="cboOrderBy" runat="server" ></asp:DropDownList>
                            <asp:DropDownList ID="cboOrderByDirection" runat="server" >
                                <asp:ListItem Enabled="True" Selected ="True" Text="Ascending" Value="Ascending"></asp:ListItem>
                                <asp:ListItem Enabled="True" Selected ="False" Text="Descending" Value="Descending"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="control-group">
                        <span class="control-label">Top Count</span>
                        <div class="controls"> <asp:TextBox ID="TextBox1" runat="server" ></asp:TextBox></div>
                    </div>
                     <asp:Panel ID="pnlFilterByDate" runat="server">
                    <div class="control-group">
                        <span class="control-label">Filter by <asp:Label ID="lblDateFilterColumn" runat="server" Text="Label"></asp:Label></span>
                        <div class="controls"></div>
                    </div>
                    <div class="control-group">
                        <span class="control-label">From</span>
                        <div class="controls"><asp:TextBox ID="txtDateFrom" runat="server"></asp:TextBox> &nbsp;<span style="font-style:italic;font-size:smaller;">(MM-dd-yyyy) format</div>
                    </div>
                        <div class="control-group">
                        <span class="control-label">To</span>
                        <div class="controls"> <asp:TextBox ID="txtDateTo" runat="server"></asp:TextBox> &nbsp;<span style="font-style:italic;font-size:smaller;">(MM-dd-yyyy) format</div>
                    </div>
                    </asp:Panel>
    
                </div>
                <div class="form-footer">
                    
                </div>

</asp:Panel>



