<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProfileControl.ascx.cs" Inherits="UserControls_ProfileControl" %>
<div id="profile-section-wrapper">
    
    <% if (!UseFullnameTextbox)
        { %>
        <div class="form-controls-place-holder ">
            
            <% if (showSalutation)
               { %>
            <span class="form-controls-span" id="salutation-place-holder">
                <asp:DropDownList ID="drpLstSalutation"  class="light-style-input" runat="server"></asp:DropDownList>
            </span>
            <% } %>

            <span class="form-controls-span">
                <label id="lblFirstName" class="form-field-label">
                    <asp:Literal ID="litFirstName" runat="server">(!leadform.aspx.4!)</asp:Literal>
                </label>
                <asp:TextBox ID="txtFirstName"  CssClass="light-style-input" runat="server"></asp:TextBox>
            </span>
        
            <span class="form-controls-span">
                <label id="lblLastName" class="form-field-label">
                    <asp:Literal ID="litLastName" runat="server">(!leadform.aspx.6!)</asp:Literal>
                </label>
                <asp:TextBox ID="txtLastName" CssClass="light-style-input" runat="server"></asp:TextBox>
            </span>
        </div>
        <div class="clear-both height-5"></div>

        <div class="form-controls-place-holder">
            <span class="form-controls-span">
                <label  id="lblEmail" class="form-field-label" maxlength="50">
                    <asp:Literal ID="litEmail" runat="server">(!customersupport.aspx.5!)</asp:Literal>
                </label>
                    <asp:TextBox ID="txtEmail" CssClass="light-style-input" runat="server"></asp:TextBox>
            </span>
            <span class="form-controls-span">
                <label  id="lblContactNumber" class="form-field-label">
                <asp:Literal ID="litContactNumber" runat="server">(!customersupport.aspx.7!)</asp:Literal>
                </label>
                <asp:TextBox ID="txtContactNumber" CssClass="light-style-input" Maxlength="100" runat="server"></asp:TextBox>
            </span>
        </div>

    <% } else { %>

    <div class="form-controls-place-holder">
        <span class="form-controls-span">
            <label id="lblShippingContactName" class="form-field-label">
                <asp:Literal ID="litShippingContactName" runat="server">(!customersupport.aspx.4!)</asp:Literal>
            </label>
            <asp:TextBox ID="txtShippingContactName" CssClass="light-style-input" runat="server"></asp:TextBox>
        </span>

       <span class="form-controls-span">
                <label  id="lblShippingContactNumber" class="form-field-label">
                <asp:Literal ID="litShippingContactNumber" runat="server">(!customersupport.aspx.7!)</asp:Literal>
                </label>
                <asp:TextBox ID="txtShippingContactNumber" MaxLength="100" CssClass="light-style-input" runat="server"></asp:TextBox>
       </span>


    </div>
    <div class="clear-both height-5"></div>

     <div class="form-controls-place-holder">
             <span class="form-controls-span" id="spanShippingSectionEmail">
            <label  id="lblShippingEmail" class="form-field-label" maxlength="50">
                <asp:Literal ID="litShippingEmail" runat="server">(!customersupport.aspx.5!)</asp:Literal>
            </label>
            <asp:TextBox ID="txtShippingEmail" CssClass="light-style-input" runat="server"></asp:TextBox>
        </span>
    </div>
                 
    <% } %>

    <% if (showEditPasswordArea){%>

    <div class="clear-both height-12 accounts-clear"></div>
    <div class="clear-both height-5 accounts-clear"></div>

    <div class="form-controls-place-holder">
        <span class="form-controls-span label-outside">
            <input type="checkbox" id="edit-password"/> <span class="checkbox-captions custom-font-style">
            
            <asp:Literal ID="Literal6" runat="server">(!account.aspx.34!)</asp:Literal>
            
            </span>
        </span>
    </div>
    <div class="clear-both height-12 accounts-clear"></div>
    <div id="Div1" class="form-controls-place-holder">
            <span class="form-controls-span custom-font-style" id="old-password-label-place-holder">
                 <asp:Literal ID="Literal5" runat="server">(!account.aspx.35!)</asp:Literal>
            </span>
            <span class="form-controls-span">
                <label id="old-password-input-label" class="form-field-label custom-font-style">
                    <asp:Literal ID="litCurrentPassword" runat="server">(!account.aspx.36!)</asp:Literal>
                </label>
                <input id="old-password-input" maxlength="12" class="light-style-input " type="password" char="." /> 
            </span>
    </div>
    <% } %>

    <% if (!exCludeAccountName) { %>

    <div class="clear-both height-5 accounts-clear"></div>
    
    <% if (!hideAccountNameArea) { %>

    <div class="clear-both height-12 accounts-clear"></div>

    <div id ="account-name-wrapper" class="form-controls-place-holder">
             <span class="form-controls-span custom-font-style" id="enter-account-name-place-holder">
                <asp:Literal ID="Literal4" runat="server">(!account.aspx.37!)</asp:Literal>
            </span>
            <span class="form-controls-span">
                <label id="lblAccountName" class="form-field-label">
                    <asp:Literal ID="LitAccountName" runat="server">(!account.aspx.38!)</asp:Literal>
                </label>
                <asp:TextBox ID="txtAccountName" runat="server" class="light-style-input" MaxLength=50></asp:TextBox>
            </span>
    </div>
    <div class="clear-both height-5 accounts-clear"></div>

    <% }  %>

    <div id="passwords-wrapper" class="form-controls-place-holder">
            
           <% if (!hideAccountNameArea) { %>

             <span class="form-controls-span custom-font-style" id="password-caption">
               <asp:Literal ID="Literal3" runat="server">(!account.aspx.39!)</asp:Literal>
            </span>

           <% } else {  %>

             <span class="form-controls-span custom-font-style" id="new-password-caption">
                <asp:Literal ID="litAccountSecurity" runat="server">(!account.aspx.41!)</asp:Literal>
            </span>

           <% } %>
            <span class="form-controls-span">
                <label id="lblPassword" class="form-field-label custom-font-style">
                    <asp:Literal ID="litPassword" runat="server">(!lostpassword.aspx.5!)</asp:Literal>
                </label>
                <asp:TextBox ID="txtPassword" MaxLength="12" class="light-style-input" TextMode="Password"  runat="server" AutoCompleteType="Disabled"></asp:TextBox>
            </span>
    
            <span class="form-controls-span">
                <label id="lblConfirmPassword" class="form-field-label custom-font-style">
                    <asp:Literal ID="litConfirmPassword" runat="server">account.aspx.40</asp:Literal>
                </label>
               <asp:TextBox ID="txtConfirmPassword" MaxLength="12" class="light-style-input" TextMode="Password" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
            </span>

    </div>
     <div class="clear-both height-5"></div>

     <% } %>
</div>
<script>
    $(document).ready(function () {

        var salutation = $("#ProfileControl_drpLstSalutation").val();
        if (typeof (salutation) == "undefined") {
            $("#ProfileControl_txtFirstName").addClass("new-first-name-width");
            $("#ProfileControl_txtLastName").addClass("new-last-name-width");
        } else {
            $("#ProfileControl_txtFirstName").removeClass("new-first-name-width");
            $("#ProfileControl_txtLastName").removeClass("new-last-name-width");
        }

    });
</script>