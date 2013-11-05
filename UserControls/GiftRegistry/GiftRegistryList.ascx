<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftRegistryList.ascx.cs" Inherits="UserControls_GiftRegistry_GiftRegistryList" %>

<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<asp:Label runat="server" ID="lblNoRecord" CssClass="registry-norecord-found" ></asp:Label>

<div class="registry-table">
    <table>
        <asp:Repeater ID="rptGiftRegistries" runat="server">
            <ItemTemplate>
                <tr id="row_<%# Container.DataItemAs<GiftRegistry>().Counter%>" style="border-bottom:dashed 1px #ccc; padding:10px;">
                    <td></td>
                    <td style="width: 85%; padding:5px;">
                        <a class="registry-link" href="editgiftregistry.aspx?<%# DomainConstants.GIFTREGISTRYPARAMCHAR %>=<%# Container.DataItemAs<GiftRegistry>().RegistryID.ToString().ToLower() %>">
                                <%# Container.DataItemAs<GiftRegistry>().Title%></a>
                        <br />
                        <span class="registry-date">
                            <%# AppLogic.GetString("giftregistry.aspx.4", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%> : 
                        </span>
                        <span class="registry-date">
                            <strong>
                                <%# Container.DataItemAs<GiftRegistry>().StartDate.Value.ToLongDateString()%>
                            </strong>
                        </span>
                    </td>
                    <td>
                        <a class="" href="editgiftregistry.aspx?<%# DomainConstants.GIFTREGISTRYPARAMCHAR %>=<%# Container.DataItemAs<GiftRegistry>().RegistryID.ToString().ToLower() %>">
                            <%= AppLogic.GetString("giftregistry.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                        </a>
                    </td>
                    <%--<td>
                        <a href="#">Disable</a>
                    </td>--%>
                    <td>
                        <a href="javascript:void(0);" id="ctrDelete_<%# Container.DataItemAs<GiftRegistry>().Counter%>">
                            <%= AppLogic.GetString("giftregistry.aspx.8", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                        </a>

                        <img style="display:none" id="ctrLoader_<%# Container.DataItemAs<GiftRegistry>().Counter%>" 
                                src="skins/skin_<%= ThisCustomer.SkinID %>/images/spinner.gif" alt="" />

                        <script type="text/javascript" >

                            $(document).ready(function () {

                                    var lnkDeleteId = '#ctrDelete_<%# Container.DataItemAs<GiftRegistry>().Counter%>';
                                    var currentRegistry = '<%# Container.DataItemAs<GiftRegistry>().RegistryID %>'
                                    $(lnkDeleteId).click(function () {
                                    
                                    if(confirm("<%= AppLogic.GetString("giftregistry.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>")) {
                                        
                                         $('#ctrLoader_<%# Container.DataItemAs<GiftRegistry>().Counter%>').show();
                                         $(lnkDeleteId).hide();

                                         AjaxCallWithSecuritySimplified(
                                            "DeleteGiftRegistry",
                                            { "giftRegistryID": currentRegistry },
                                            function (result) {
                                                $('#row_<%# Container.DataItemAs<GiftRegistry>().Counter%>').hide("fast");
                                            }
                                        );

                                    }
                                                                         
                                });

                            });
                            
                        </script>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>
