<%@ Page Title="" Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true" CodeFile="Tools.aspx.cs" Inherits="admin_Tools" %>

<asp:Content ID="Content1" ContentPlaceHolderID="pnlMain" Runat="Server">
<asp:UpdatePanel ID="updatePanel" runat="server" RenderMode="Block" UpdateMode="Conditional" ChildrenAsTriggers="true">
<ContentTemplate>

    <div class="createform">
        <div class="form-header">Facebook API Link Generator</div>
         <div class="form-horizontal">
            <%--<div class="alert alert-block">Note: Any images under mobile folder will be replaced.</div>
            <div class="alert alert-error" id="divError" style="display:none;">Problem Encountered. Please contact administrator. <br /></div>--%>

            <div class="alert alert-success" id="divFacebookFeedboxResult" runat="server" visible="false">
                Copy this link: <asp:Label runat="server" ID="lblFacebookFeedboxAPI" Text=""></asp:Label>
            </div>
            <br />                
            <div class="control-group">
                <span class="control-label">Facebook Page URL</span>
                <div class="controls">
                    <asp:TextBox ID="txtFacebookURL" runat="server" Width="300px"></asp:TextBox>
                    <asp:HyperLink ID="lnkFacebookURL" runat="server" NavigateUrl="#" ForeColor="Orange" Font-Underline="false" Font-Bold="true" ToolTip="The facebook page url where the latest posts will be taken">?</asp:HyperLink>
                    <asp:RequiredFieldValidator ID="rfvFacebookURL" runat="server" ControlToValidate="txtFacebookURL" ErrorMessage="Facebook Page URL is required" ForeColor="Red"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="control-group">
                <span class="control-label">Post Limit</span>
                <div class="controls">
                    <asp:TextBox ID="txtPostLimit" runat="server" Width="50px" Text="10"></asp:TextBox>
                    <asp:HyperLink ID="lnkPostLimit" runat="server" NavigateUrl="#" ForeColor="Orange" Font-Underline="false" Font-Bold="true" ToolTip="Determines the no. of latest posts to be displayed in the feedbox">?</asp:HyperLink>
                    <asp:RequiredFieldValidator ID="rfvPostLimit" runat="server" ControlToValidate="txtPostLimit" ErrorMessage="Post limit is required"  ForeColor="Red"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="control-group">
                <span class="control-label">Access Token</span>
                <div class="controls">
                    <asp:TextBox ID="txtAccessToken" runat="server" Width="300px"></asp:TextBox> 
                    <asp:HyperLink ID="lnkAccessToken" runat="server" Target="_blank" NavigateUrl="http://blog.awpny.com/kaiawpny/2011/how-to-facebook-access-token.html" ForeColor="Orange" Font-Underline="false" Font-Bold="true" ToolTip="How to get access token">?</asp:HyperLink>
                    <asp:RequiredFieldValidator ID="rfvAccessToken" runat="server" ControlToValidate="txtAccessToken" ErrorMessage="Access token is required"  ForeColor="Red"></asp:RequiredFieldValidator>
                </div>
            </div>
            
        </div>
        <div class="form-footer">
            <asp:Button ID="btnGenerateFacebookFeedboxAPI" runat="server" Text="Generate" CssClass="btn" />
        </div>    
    </div>


</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>

