<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftRegistryForm.ascx.cs" Inherits="UserControls_GiftRegistry_GiftRegistryForm" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<%@ Register Src="~/UserControls/DatePickerControl.ascx" TagName="DatePickerControl" TagPrefix="ctrl" %>
<%@ Register Src="~/UserControls/WebEditorControl.ascx" TagName="WebEditorControl" TagPrefix="ctrl" %>

<div class="gift-registry-form">
    <div class="gift-registry-form-wrapper">
        <table>
            <tr>
                <td class="form-label">
                    <span>
                        <%= AppLogic.GetString("editgiftregistry.aspx.3", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %>
                    </span>
                </td>
                <td class="form-input" valign="bottom">
                    <ctrl:DatePickerControl ID="ctrlDatePickerStartDate" runat="server" CssClass="registry-input" />
                    <span class="required-marker">*</span>
                </td>
            </tr>
            <tr>
                <td class="form-label">
                    <span>
                        <%= AppLogic.GetString("editgiftregistry.aspx.4", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %>
                    </span>
                </td>
                <td class="form-input">
                    <ctrl:DatePickerControl ID="ctrlDatePickerEndDate" runat="server" CssClass="registry-input" /><span class="required-marker">*</span>
                </td>
            </tr>
            <tr>
                <td class="form-label">
                </td>
                <td class="form-input">
                    <span class="registry-sublabel">
                        <%= AppLogic.GetString("editgiftregistry.aspx.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %>
                    </span>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr class="registry-divider" />
                </td>
            </tr>
            <tr>
                <td class="form-label">
                    <span>
                        <%= AppLogic.GetString("editgiftregistry.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %>
                    </span>
                </td>
                <td class="form-input">
                    <asp:TextBox runat="server" ID="txtTitle" CssClass="registry-input" ></asp:TextBox><span class="required-marker">*</span>
                </td>
            </tr>
            <tr>
                <td class="form-label webeditor-label">
                    <span>
                        <%= AppLogic.GetString("editgiftregistry.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %>
                    </span>
                </td>
                <td class="form-input">
                    <ctrl:WebEditorControl ID="WebEditorControl" DisplayMode="Simple" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="form-label">
                    <span>
                        <%= AppLogic.GetString("editgiftregistry.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %>
                    </span>
                </td>
                <td class="form-input">
                    <asp:FileUpload runat="server" ID="txtFileUpload" CssClass="registry-input"/> 
                </td>
            </tr>
            <tr>
                <td class="form-label">
                    <span>
                        <%= AppLogic.GetString("editgiftregistry.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %>
                    </span>
                </td>
                <td class="form-input">
                    <asp:RadioButton runat="server" ID="rdPublic" Text="Public" Checked="true" GroupName="btn" />
                    <asp:RadioButton runat="server" ID="rdPrivate" Text="Private" GroupName="btn" />
                </td>
            </tr>
            <tr>
                <td class="form-label">
                    <span>
                        <%= AppLogic.GetString("editgiftregistry.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                    </span>
                </td>
                <td class="form-input">
                    <asp:TextBox runat="server" MaxLength="5" ID="txtGuestPassword" CssClass="registry-input" Enabled="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr class="registry-divider" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <span class="registry-sublabel">
                        <%= AppLogic.GetString("editgiftregistry.aspx.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%><br />
                        <%= AppLogic.GetString("editgiftregistry.aspx.33", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                    </span>
                </td>
            </tr>
            <tr>
                <td class="form-label">
                    <span>
                        <%= AppLogic.GetString("editgiftregistry.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %>
                    </span>
                </td>
                <td class="form-input">
                    <asp:Label runat="server" id="lblprimaryurl" style="float:left; line-height:26px"></asp:Label>
                    <asp:TextBox runat="server" ID="txtCustomURL" CssClass="registry-input" ></asp:TextBox><span class="required-marker adjust-padding">*</span>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <a runat="server" target="_blank" id="lnkPreView" class="registry-form-link content" Visible="false" />
                </td>
            </tr>
        </table>
    </div>
</div>

<script type="text/javascript" >

    $(document).ready(function () {

        $('#<%=rdPublic.ClientID%>').click(function () {
            $('#<%=txtGuestPassword.ClientID%>').val('');
            $('#<%=txtGuestPassword.ClientID%>').attr("disabled", "disabled");
        })

        $('#<%=rdPrivate.ClientID%>').click(function () {
            $('#<%=txtGuestPassword.ClientID%>').removeAttr("disabled");
        })

    });
    
</script>
