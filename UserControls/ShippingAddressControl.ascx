<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShippingAddressControl.ascx.cs" Inherits="UserControls_ShippingAddressControl" %>

<div class="form-controls-place-holder">
    <span class="form-controls-span">
        <label id="shipping-address-input-label"  class="support-control-label">
            <asp:Literal ID="LtrStreetAddress_Caption" runat="server">(!customersupport.aspx.8!)</asp:Literal>
        </label>
        <textarea id="shipping-address-input" class="light-style-input apply-behind-caption-effect"></textarea> 
    </span>
</div>
<div class="clr5"></div>
                              
<div class="form-controls-place-holder">

    <span class="form-controls-span">
        <asp:DropDownList ID="shipping_country_input_select"  runat="server"></asp:DropDownList>
    </span>

</div>

<div class="clr5"></div>
<div class="shipping-zip-city-other-place-holder">               
    <span class="form-controls-span">
        <label id="shipping-city-input-label"  class="support-control-label">
                <asp:Literal ID="LtrCity_Caption" runat="server">(!leadform.aspx.11!)</asp:Literal>
        </label>
        <input id="shipping-city-input"  maxlength = "32" class="light-style-input apply-behind-caption-effect" type="text" /> 
    </span>

    <span class="form-controls-span">
        <label  id="shipping-states-input-label" class="support-control-label">
            <asp:Literal ID="LtrState_Caption" runat="server">(!leadform.aspx.10!)</asp:Literal>
        </label>
        <input id="shipping-states-input" maxlength = "2" class="light-style-input apply-behind-caption-effect" type="text" /> 
    </span>
</div>   
<div class="shipping-postal-place-holder">
<span class="form-controls-span">
    <label id="shipping-postal-code-input-label" class="support-control-label" maxlength="10">
            <asp:Literal ID="LtrPostal_Caption" runat="server">(!leadform.aspx.28!)</asp:Literal>
    </label>
    <input id="shipping-postal-code-input" class="light-style-input apply-behind-caption-effect" type="text" /> 
</span>

<span class="form-controls-span label-outside" id="shipping-enter-postal-label-place-holder">
    <asp:Literal ID="LtrEnterPostal" runat="server">(!customersupport.aspx.40!)</asp:Literal>
</span>
</div>

<div class="clr5"></div>

<div class="form-controls-place-holder">
    <span class="form-controls-span">
        <asp:DropDownList ID="shipping_address_type"  runat="server" CssClass="light-style-input"></asp:DropDownList>
    </span>

</div>
