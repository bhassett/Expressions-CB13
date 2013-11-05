<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftRegistryItemList.ascx.cs"
    Inherits="UserControls_GiftRegistry_GiftRegistryItemList" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<div class="registry-table-wrapper">
    <table width="100%">
        <asp:Repeater ID="rptGiftRegistryItemList" runat="server">
            <ItemTemplate>
                <tr class="registry-item-row_<%# Container.DataItemAs<GiftRegistryItem>().Counter %> row_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>" style="border-bottom:dashed 1px #ccc; padding:10px;">
                    <td>
                        <div class="imgContainer" title="<%# AppLogic.GetString("editgiftregistry.aspx.30", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>">
                            <a class="registry-form-link cloud-zoom" href="<%# Container.DataItemAs<GiftRegistryItem>().ProductPhotoPath %>">
                                <img class="imgContainer" alt="" src="<%# Container.DataItemAs<GiftRegistryItem>().ProductPhotoPath %>" title="" />
                            </a>
                        </div>
                    </td>
                    <td class="form-input">
                        <table>
                            <tr>
                                <td colspan="4">
                                    <a class="registry-form-link" href="<%# Container.DataItemAs<GiftRegistryItem>().ProductURL %>" target="_blank"><%# Container.DataItemAs<GiftRegistryItem>().ProductName%></a>
                                </td>
                            </tr>
                            <tr>
                                <td style="width:25%;">
                                    <span class="registry-product-price"><%# Container.DataItemAs<GiftRegistryItem>().ProductPriceFormatted%></span>
                                    <span class="registry-product-unitmeasure fancy-button-leftspace">
                                        <% if (!AppLogic.AppConfigBool("HideUnitMeasure")) { %>
                                            <%# Container.DataItemAs<GiftRegistryItem>().UnitMeasureCode %>
                                        <% } %>
                                    </span>
                                </td>
                                <td style="width:25%;">
                                    <span class="registry-item-sort to-float-left" style="font-size:9pt !important;"><%# AppLogic.GetString("editgiftregistry.aspx.46", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
                                    <input type="text" id="txtSortOrder_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>" class="registry-input " style="width: 30px; margin-right: 5px;" value="<%# Container.DataItemAs<GiftRegistryItem>().SortOrder %>" />
                                </td>
                                <td style="width:25%;">
                                    <span class="registry-item-quantity to-float-left" style="font-size:9pt !important;"><%# AppLogic.GetString("shoppingcart.cs.2", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
                                    <input type="text" id="txtQuantity_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>"class="registry-input" maxlength="6" style="width: 50px" value="<%# Localization.ParseLocaleDecimal(Container.DataItemAs<GiftRegistryItem>().Quantity, ThisCustomer.LocaleSetting) %>" />
                                </td>
                                <td style="width:25%; vertical-align:top;">
                                    <div style="background-color:#dadada;border-radius:5px; width:100px;">
                                        <span class="ordered-caption"><%# AppLogic.GetString("editgiftregistry.aspx.28", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span> : <span class="registry-product-ordered"><%# Container.DataItemAs<GiftRegistryItem>().Ordered.ToString("00")%></span>
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="4">
                                    <span style="font-size:9pt !important;"><%# AppLogic.GetString("editgiftregistry.aspx.47", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></span>
                                    <textarea cols="60" 
                                                rows="12" 
                                                id="txtComment_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>" 
                                                class="registry-input item-comment"><%# Container.DataItemAs<GiftRegistryItem>().Comment %></textarea>
                                      
                                   <input type="hidden" 
                                        id="registryItemCode_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>" 
                                        value="<%# Container.DataItemAs<GiftRegistryItem>().RegistryItemCode.ToString() %>" />

                                    <script type="text/javascript">

                                        $(document).ready(function () {

                                            $('.cloud-zoom').fancybox({
                                                'titlePosition': 'inside',
                                                'transitionIn': 'none',
                                                'transitionOut': 'fade'
                                            });

                                            IniUpdate();

                                            //hide the move button if no other registry to move to.
                                            var buttonId = '#btnMove_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>';
                                            if ($('.modal-registries-options option').length == 0) {
                                                $(buttonId).hide();
                                            }

                                            var id = '<%# Container.DataItemAs<GiftRegistryItem>().Counter %>';

                                            $(buttonId).click(function () {

                                                //#modal-registries div is located at editgiftregistry.aspx
                                                $('#modal-registries').dialog({
                                                    autoOpen: true,
                                                    modal: true,
                                                    position: "center",
                                                    resizable: false,
                                                    zIndex: 99999,
                                                    title: '<%# AppLogic.GetString("editgiftregistry.aspx.29", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>'
                                                });

                                                var itemCode = '<%# Container.DataItemAs<GiftRegistryItem>().RegistryItemCode.ToString() %>';
                                                var currentRegistry = '<%# Container.DataItemAs<GiftRegistryItem>().RegistryID.ToString() %>';

                                                $('#modal-registries').find('#hdenRegistryItemCode').val(itemCode);
                                                $('#modal-registries').find('#hdenCurrentRegistry').val(currentRegistry);
                                                $('#modal-registries').find('#hdenRowToRemove').val('.registry-item-row_' + id);


                                            });

                                            $('#btnDelete_' + id).hover(function () {
                                                $('.row_' + id).addClass("crud-control-hover");
                                            }, function () {
                                                $('.row_' + id).removeClass("crud-control-hover");
                                            });

                                            $('#btnUpdate_' + id).hover(function () {
                                                $('.row_' + id).addClass("crud-control-hover");
                                            }, function () {
                                                $('.row_' + id).removeClass("crud-control-hover");
                                            });

                                            $('#btnMove_' + id).hover(function () {
                                                $('.row_' + id).addClass("crud-control-hover");
                                            }, function () {
                                                $('.row_' + id).removeClass("crud-control-hover");
                                            });

                                            $('#btnDelete_' + id).click(function () {

                                                if (!confirm('<%# AppLogic.GetString("editgiftregistry.aspx.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>')) return;

                                                var registryItemCode = '<%# Container.DataItemAs<GiftRegistryItem>().RegistryItemCode.ToString() %>';
                                                var currentRegistry = '<%# Container.DataItemAs<GiftRegistryItem>().RegistryID.ToString() %>';

                                                AjaxCallWithSecuritySimplified(
                                                    "DeleteRegistryItem",
                                                    { "registryItemCode": registryItemCode, "giftRegistryId": currentRegistry },
                                                    function (result) {
                                                        $('.registry-item-row_' + id).hide('slow', function () {
                                                            $(this).remove();
                                                        });
                                                    }
                                                );

                                            });

                                            function IniUpdate() {

                                                var registryItemCode = '<%# Container.DataItemAs<GiftRegistryItem>().RegistryItemCode.ToString() %>';
                                                var counter = "<%# Container.DataItemAs<GiftRegistryItem>().Counter %>";
                                                var btnUpdateID = '#btnUpdate_' + counter;
                                                var txtCommentID = '#txtComment_' + counter;
                                                var txtQuantity = '#txtQuantity_' + counter;
                                                var txtSortOrder = '#txtSortOrder_' + counter;

                                                $(btnUpdateID).click(function (event) {

                                                    var quantity = $(txtQuantity).val();
                                                    if (!$.isNumeric(quantity)) {
                                                        alert('<%# AppLogic.GetString("common.cs.22", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>');
                                                        event.preventDefault();
                                                        return;
                                                    }

                                                    var sortOrder = $(txtSortOrder).val();
                                                    if (!$.isNumeric(sortOrder) && sortOrder != '') {
                                                        alert('<%# AppLogic.GetString("editgiftregistry.aspx.31", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>');
                                                        event.preventDefault();
                                                        return;
                                                    }

                                                    if (sortOrder == '') sortOrder = 0;

                                                    var comment = $.trim($(txtCommentID).val());

                                                    AjaxCallWithSecuritySimplified(
                                                        "UpdateRegistryItem",
                                                        { "registryItemCode": registryItemCode, "comment": comment, "sortOrder": sortOrder, "quantity": quantity },
                                                        function (result) {
                                                            $(btnUpdateID).hide('fast');
                                                        }
                                                    );

                                                });

                                                $(txtCommentID).keypress(function (event) {
                                                    if (event.which == 13) {
                                                        event.preventDefault();
                                                        return;
                                                    }
                                                    $(btnUpdateID).show();
                                                });

                                                $(txtQuantity).keypress(function (event) {
                                                    if (event.which == 13) {
                                                        event.preventDefault();
                                                        return;
                                                    }
                                                    $(btnUpdateID).show();
                                                });

                                                $(txtSortOrder).keypress(function (event) {
                                                    if (event.which == 13) {
                                                        event.preventDefault();
                                                        return;
                                                    }
                                                    $(btnUpdateID).show();
                                                });

                                            };

                                        });

                                    </script>

                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        
                        <div style="position: relative;">
                            <div>
                                <img src="skins/skin_<%# ThisCustomer.SkinID %>/images/giftregistry/delete-giftregistry.png"
                                    alt="" id="btnDelete_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>"
                                    title="<%# AppLogic.GetString("editgiftregistry.aspx.35", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" />
                            </div>

                            <div id="btnUpdate_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>" style="display:none">
                                <img src="skins/skin_<%# ThisCustomer.SkinID %>/images/giftregistry/save.png" alt="" 
                                    title="<%# AppLogic.GetString("editgiftregistry.aspx.30", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" />
                            </div>

                            <div id="btnMove_<%# Container.DataItemAs<GiftRegistryItem>().Counter %>" >
                                <img src="skins/skin_<%# ThisCustomer.SkinID %>/images/giftregistry/move.png" alt="" 
                                    title="<%# AppLogic.GetString("editgiftregistry.aspx.34", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" />
                            </div>
                            
                        </div>
                        
                    

                    </td>
                </tr>
            </ItemTemplate>
            <SeparatorTemplate>
            </SeparatorTemplate>
        </asp:Repeater>
    </table>
</div>
