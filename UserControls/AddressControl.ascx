
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddressControl.ascx.cs" Inherits="UserControls_AddressControl" %>
<script type="text/javascript" src="jscripts/usercontrol.address.control.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery.cbe.address.verification.js"></script>
<% if (!hideStreetAddress)
 { %>

<div class="form-controls-place-holder">
    <span class="form-controls-span">
        <label id="lblStreet" class="form-field-label" runat="server">
            <asp:Literal ID="litStreet" runat="server">(!customersupport.aspx.8!)</asp:Literal>
        </label>
        <asp:TextBox ID="txtStreet"  TextMode="multiline" runat="server" maxlength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>
</div>
<div class="clear-both height-5"></div>
       
<% } %>
                           
<div class="form-controls-place-holder">
    <span class="form-controls-span">
        <asp:DropDownList ID="drpCountry" runat="server"></asp:DropDownList>
    </span>
</div>

<div class="clear-both height-5"></div>

<div class="<%=IdPrefix%>zip-city-other-place-holder">               
    <span class="form-controls-span">
        <label id="lblCity"  class="form-field-label" runat="server">
            <asp:Literal ID="litCity" runat="server">(!leadform.aspx.11!)</asp:Literal>
        </label>
        <asp:TextBox ID="txtCity" runat="server" maxlength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>

    <span class="form-controls-span">
        <label  id="lblState" class="form-field-label" runat="server">
            <asp:Literal ID="litState" runat="server">(!leadform.aspx.10!)</asp:Literal>
        </label>
        <asp:TextBox ID="txtState" runat="server" maxlength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>
</div>   

<div class="postal-place-holder">
    <span class="form-controls-span">
        <label id="lblPostal" class="form-field-label" runat="server" maxlength="10">
                <asp:Literal ID="litPostal" runat="server">(!leadform.aspx.28!)</asp:Literal>
        </label>
        <asp:TextBox ID="txtPostal" runat="server" maxlength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>

    <span class="form-controls-span custom-font-style capitalize-text" id="<%=IdPrefix%>enter-postal-label-place-holder">
        <asp:Literal ID="litEnterPostal" runat="server">(!customersupport.aspx.40!)</asp:Literal>
    </span>
</div>

<asp:Panel ID="pnlCounty" runat="server" Visible="false">
<div class="clear-both height-5"></div>
 <div class="float-left">
    <span class="form-controls-span">
        <label id="lblCounty" class="form-field-label" runat="server" maxlength="10">
                <asp:Literal ID="litCounty" runat="server">County (optional)</asp:Literal>
        </label>
        <asp:TextBox ID="txtCounty" runat="server" maxlength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>
 </div>
 </asp:Panel>

<% if (showBusinessTypes)
   { %>
<div class="clear-both height-5"></div>
<div class="form-controls-place-holder">
    <span class="form-controls-span" id="business-type-place-holder">
         <asp:DropDownList ID="drpBusinessType"  runat="server" CssClass="light-style-input"></asp:DropDownList>
    </span>
</div>
<div id="tax-number-place-holder" class="display-none">
    <div class="clear-both height-5"></div>
    <div class="form-controls-place-holder">
      <span class="form-controls-span">
            <label  id="lblTaxNumber" class="form-field-label" runat="server">
                <asp:Literal ID="litTaxtNumber" runat="server">Tax Number</asp:Literal>
            </label>
            <asp:TextBox ID="txtTaxNumber" runat="server" maxlength = "32" class="light-style-input" type="text" ></asp:TextBox>
    </span>
    </div>
</div>
<% } %>

<% if (showResidenceTypes)
{ %>
<div class="clear-both height-5"></div>
<div class="form-controls-place-holder">
    <span class="form-controls-span">
        <asp:DropDownList ID="drpType"  runat="server" CssClass="light-style-input"></asp:DropDownList>
    </span>
</div>
<% } %>

<div id="adjust-country-width-if-other-option-is-selected" class="display-none"><%=countryWidthAdjustment%></div>