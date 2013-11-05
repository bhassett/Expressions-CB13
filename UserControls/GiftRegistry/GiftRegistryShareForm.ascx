<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftRegistryShareForm.ascx.cs"
    Inherits="UserControls_GiftRegistry_GiftRegistryShareForm" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<%@ Register Src="~/UserControls/WebEditorControl.ascx" TagName="WebEditorControl"
    TagPrefix="ctrl" %>
<table>
    <tr>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="txtEmail1" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="regexValidator1" ControlToValidate="txtEmail1"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error">
            </asp:RegularExpressionValidator>
        </td>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox1" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="TextBox1"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error">
            </asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox2" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="TextBox2"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error">
            </asp:RegularExpressionValidator>
        </td>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox3" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" ControlToValidate="TextBox3"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error">
            </asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox4" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" ControlToValidate="TextBox4"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error"></asp:RegularExpressionValidator>
        </td>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox5" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" ControlToValidate="TextBox5"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox6" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" ControlToValidate="TextBox6"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error"></asp:RegularExpressionValidator>
        </td>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox7" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator7" ControlToValidate="TextBox7"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox8" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator8" ControlToValidate="TextBox8"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error"></asp:RegularExpressionValidator>
        </td>
        <td class="form-label">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
        </td>
        <td class="form-input">
            <asp:TextBox runat="server" ID="TextBox9" CssClass="registry-input email"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator9" ControlToValidate="TextBox9"
                Display="Dynamic" ValidationGroup="emailValidationGrp" CssClass="error"></asp:RegularExpressionValidator>
        </td>
    </tr>
</table>
<hr class="registry-divider" />
<table style="margin-bottom:0px;">
    <tr>
        <td class="form-label" style="text-align: left; width:11%">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.43", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
            </span>
        </td>
        <td class="form-input" style="width:80%; text-align: left;">
            <asp:TextBox runat="server" ID="txtSubject" CssClass="registry-input" Width="98%"></asp:TextBox>
        </td>
    </tr>
</table>
<table width="100%">
    <tr>
        <td valign="top" class="form-label" style="text-align: left">
            <span>
                <%= AppLogic.GetString("editgiftregistry.aspx.39", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
            </span>
            <div style="float:right; font-size:9pt;">
                <b><asp:CheckBox runat="server" ID="chkSendCopy" /></b>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <ctrl:WebEditorControl ID="ctrlWebEditorControl" runat="server" DisplayMode="Advanced" />
        </td>
    </tr>
</table>
