<%@ Page Language="C#" CodeFile="editgiftregistry.aspx.cs" Inherits="InterpriseSuiteEcommerce.editgiftregistry"
    ValidateRequest="false" EnableEventValidation="true" %>

<%@ OutputCache Location="None" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<%@ Register Src="~/UserControls/GiftRegistry/GiftRegistryForm.ascx" TagName="GiftRegistryForm" TagPrefix="ctrl" %>
<%@ Register Src="~/UserControls/GiftRegistry/GiftRegistryItemList.ascx" TagName="GiftRegistryItemList" TagPrefix="ctrl" %>
<html>
<head>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <script type="text/javascript" src="jscripts/imagezoom.js"></script>
    <asp:Panel ID="pnlMain" runat="server" CssClass="pnlmain">

        <!-- Registry Form -->
        <div class="sections-place-holder">
            <div class="page-sections-head">
                <asp:Literal runat="server" ID="litRegistryHeader"></asp:Literal>
                <a href="javascript:void(0);" class="gift-registry-expander" style="text-align:right; float:right;">
                    <span style="color:#fff !important; font-size:9pt !important; margin-right:5px;"><%=AppLogic.GetString("editgiftregistry.aspx.51", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true) %></span>
                </a>
            </div>
            
            <div class="formcollapse1">
                <asp:Panel CssClass="error-message-wrapper" runat="server" ID="pnlErrorMessage" Visible="false"></asp:Panel>
                <ctrl:GiftRegistryForm ID="ctrlGiftRegistryForm" runat="server" />
                <div class="button-place-holder">
                    <asp:Button runat="server" ID="btnShare" Text="Share" CssClass="site-button content" Visible="false" />
                    <asp:Button runat="server" ID="btnSave" Text="Save" CssClass="site-button content" />&nbsp;&nbsp;&nbsp;
                    <div class="clear-both height-17"></div>
                </div>
            </div>
        </div>

    </asp:Panel>

    <asp:Panel ID="pnlSubMain" runat="server" CssClass="pnlmain">
        
        <!-- Registry Items -->
        <div class="gift-registry-form" runat="server" id="pnlRegItems" visible="false">
            <div class="sections-place-holder">
                <div class="page-sections-head">
                    <%= AppLogic.GetString("editgiftregistry.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                    <a href="javascript:void(0);" class="gift-registry-expander1" style="text-align:right; float:right;">
                        <span style="color:#fff !important; font-size:9pt !important; margin-right:5px;"><%=AppLogic.GetString("editgiftregistry.aspx.51", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true) %></span>
                    </a>
                </div>
                <div class="formcollapse2">
                    <ctrl:GiftRegistryItemList ID="ctrlGiftRegistryItemList" runat="server" />
                    <br />
                </div>
            </div>
        </div>

        <!-- Registry Item Options -->
        <div class="gift-registry-form" runat="server" id="pnlRegItemOptions" visible="false">
            <div class="sections-place-holder">
                <div class="page-sections-head">
                    <%= AppLogic.GetString("editgiftregistry.aspx.26", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                     <a href="javascript:void(0);" class="gift-registry-expander2" style="text-align:right; float:right;">
                        <span style="color:#fff !important; font-size:9pt !important; margin-right:5px;"><%=AppLogic.GetString("editgiftregistry.aspx.51", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true) %></span>
                    </a>
                </div>
                <div class="formcollapse3">
                    <ctrl:GiftRegistryItemList ID="ctrlGiftRegistryItemListOptions" runat="server" />
                    <br />
                </div>
            </div>
        </div>

        <!-- Back to Registries Button -->
        <div class="button-layout">
            <asp:Button runat="server" ID="btnShowAllRegistry" Text="Back To Gift Registry" CssClass="site-button content" />    
        </div>
        <br />
        <br />
        <div runat="server" id="topicContainer" visible="false" class="gift-registry-form"
            style="padding-top: 20px">
            <div class="gift-registry-form-wrapper">
                <asp:Literal runat="server" ID="litTopic"></asp:Literal>
            </div>
        </div>
        <div id="modal-registries" style="display: none;">
            <asp:DropDownList ID="ddlModalRegistries" runat="server" CssClass="modal-registries-options">
            </asp:DropDownList>
            <div style="margin-top: 10px; text-align: center">
                <input type="button" id="btnMoveItem" value="Move item to this registry" />
            </div>
            <input type="hidden" id="hdenRegistryItemCode" value="" />
            <input type="hidden" id="hdenCurrentRegistry" value="" />
            <input type="hidden" id="hdenRowToRemove" value="" />
        </div>

        <script type="text/javascript">

            $(document).ready(function () {

                var show = '<%=AppLogic.GetString("editgiftregistry.aspx.50", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true) %>';
                var hide = '<%=AppLogic.GetString("editgiftregistry.aspx.51", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true) %>';

                $('.gift-registry-expander').click(function () {
                    if ($(this).hasClass("editable-content")) return false;
                    jqueryHideShow('.formcollapse1', '.gift-registry-expander', show, hide);
                })

                $('.gift-registry-expander1').click(function () {
                    jqueryHideShow('.formcollapse2', '.gift-registry-expander1', show, hide);
                })

                $('.gift-registry-expander2').click(function () {
                    jqueryHideShow('.formcollapse3', '.gift-registry-expander2', show, hide);
                })

                $('#btnMoveItem').click(function () {

                    var sourceRegistryID = $("#hdenCurrentRegistry").val();
                    var seletedRegistrCode = $('.modal-registries-options option:selected').val();
                    var registryItemCode = $("#hdenRegistryItemCode").val();
                    var trToRemove = $("#hdenRowToRemove").val();

                    AjaxCallWithSecuritySimplified(
                        "MoveItemToRegistry",
                        { "sourceRegistryID": sourceRegistryID, "targetRegistryID": seletedRegistrCode, "registryItemCode": registryItemCode },
                        function (result) {
                            $(trToRemove).hide('fast');
                            $('#modal-registries').dialog('close');
                        }
                    );

                });

            });
    
        </script>

    </asp:Panel>
    </form>
</body>
</html>