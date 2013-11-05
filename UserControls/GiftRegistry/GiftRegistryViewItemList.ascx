<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftRegistryViewItemList.ascx.cs"
    Inherits="UserControls_GiftRegistry_GiftRegistryViewItemList" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<div class="registry-table-wrapper">
    <table>
        <asp:Repeater ID="rptGiftRegistryItemList" runat="server">
            <ItemTemplate>
                <tr class="registry-item-row_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" style="border-bottom:dashed 1px #ccc; padding:10px;">
                    <td style="width: 10%;">
                        <div class="imgContainer">
                            <a class="cloud-zoom" href="<%# Container.DataItemAs<GiftRegistryItem>().ProductPhotoPath %>">
                                <img class="imgContainer" alt="" title="" src="<%# Container.DataItemAs<GiftRegistryItem>().ProductPhotoPath %>" />
                            </a>
                        </div>
                    </td>
                    <td class="form-input" style="width: 90%;">
                        <form id="AddToCartForm_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter%>"
                        action="addtocart.aspx?returnurl=viewregistry.aspx?g=<%#CustomUrl%>" method="post">
                        <table class="registry-item-inner">
                            <tr>
                                <td colspan="3"><a class="registry-form-link" target="_blank" href="<%# Container.DataItemAs<GiftRegistryItem>().ProductURL %>"><%# Container.DataItemAs<GiftRegistryItem>().ProductName%></a></td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="registry-product-price"><%# Container.DataItemAs<GiftRegistryItem>().ProductPriceFormatted%></span>
                                    <span class="registry-product-unitmeasure fancy-button-leftspace">
                                        <% if (!AppLogic.AppConfigBool("HideUnitMeasure")) { %>
                                            <%# Container.DataItemAs<GiftRegistryItem>().UnitMeasureCode %>
                                        <% } %>
                                    </span>
                                </td>
                                <td align="center" style="width:40px;"> 
                                    <input type="text" name="Quantity" id="txtQuantity_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" class="registry-input" maxlength="6" style="width: 50px" value="1" /></td>
                                <td align="left" style="width:100px;">
                                 <script type="text/javascript">

                                        $(document).ready(function () {

                                            $('#btnAddToCart_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>').click(function (e) {
                                                
                                                var stillInNeed = <%# Container.DataItemAs<GiftRegistryItem>().Quantity %>;
                                                var inputQuantity = $('#txtQuantity_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>').val();

                                                if (!$.isNumeric(inputQuantity) || inputQuantity > stillInNeed || inputQuantity < 1)
                                                {
                                                    alert('<%# AppLogic.GetString("editgiftregistry.error.13", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>');
                                                    e.preventDefault();
                                                    return;
                                                }

//                                                $.ajax({
//                                                    type: "POST",
//                                                    url: "addtocart.aspx?returnurl=shoppingcart.aspx",
//                                                    dataType: "html",
//                                                    success: function (result) {
//                                                        alert('Success add to cart.');
//                                                    },
//                                                    error: function (result, textStatus, errorThrown) {
//                                                        alert(errorThrown)
//                                                    }
//                                                });

                                            });

                                            $(".cloud-zoom").fancybox({
                                                'titlePosition': 'inside',
                                                'transitionIn': 'none',
                                                'transitionOut': 'fade'
                                            });

                                            var comment = $.trim('<%# Container.DataItemAs<GiftRegistryItem>().Comment %>');
                                            if (comment != '') {
                                                var id = <%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>;
                                                var commentObjId = $('#view-comment_' + id);
                                                $(commentObjId).show();
                                                $(commentObjId).click(function () {
                                                    $('#lblComment_' + id).dialog({
                                                        autoOpen: true,
                                                        modal: true,
                                                        position: "center",
                                                        resizable: false,
                                                        zIndex: 99999
                                                    });
                                                });
                                            } else {
                                                $('#view-comment_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>').hide();
                                            }

                                        });

                                    </script>

                                    <input type="submit" class="move-attr site-button" id="btnAddToCart_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>"
                                        data-contentKey="AppConfig.CartButtonPrompt"
                                        data-contentValue="<%= AppLogic.GetString("AppConfig.CartButtonPrompt", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"
                                        data-contentType="string resource"
                                        value="<%# AppLogic.GetString("AppConfig.CartButtonPrompt", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" 
                                        />
                                    <%-- <input type="button" class="move-attr site-button" id="btnAddToCart_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" value="<%# AppLogic.GetString("AppConfig.CartButtonPrompt", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>" />--%>
                                    <input type="hidden" id="ItemCode_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" value="<%# Container.DataItemAs<GiftRegistryItem>().ItemCode %>" />
                                    <input type="hidden" name="ProductID" id="AddToCart_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" value="<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" />
                                    <input type="hidden" id="UnitMeasureCode_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" name="UnitMeasureCode" value="<%# Container.DataItemAs<GiftRegistryItem>().UnitMeasureCode %>">
                                    <input type="hidden" id="hdenRegistryID_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" name="RegistryID" value="<%# Container.DataItemAs<GiftRegistryItem>().RegistryID.ToString() %>">
                                    <input type="hidden" id="Hidden1" name="RegistryItemCode" value="<%# Container.DataItemAs<GiftRegistryItem>().RegistryItemCode %>">
                                    <input type="hidden" id="KitItems" name="KitItems" value="<%# Container.DataItemAs<GiftRegistryItem>().KitComposition %>">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <div class="registry-ordered-wrapper">
                                        <span class="ordered-caption">
                                            <%# AppLogic.GetString("viewregistry.aspx.2", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                                        </span>: <span class="registry-product-ordered">
                                            <%# ((int)Container.DataItemAs<GiftRegistryItem>().Quantity).ToString("00") %>
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <a href="javascript:void(0);" class="registry-form-link view-comment-link" 
                                        id="view-comment_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>">
                                        <%# AppLogic.GetString("viewregistry.aspx.3", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                                    </a>
                                    <div id="lblComment_<%# Container.DataItemAs<GiftRegistryItem>().ProductCounter %>" style="display: none;">
                                        <pre><code><%# Container.DataItemAs<GiftRegistryItem>().Comment%></code></pre>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        </form>
                    </td>
                </tr>
            </ItemTemplate>
            <SeparatorTemplate>
            </SeparatorTemplate>
        </asp:Repeater>
    </table>
</div>
