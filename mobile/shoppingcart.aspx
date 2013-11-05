<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.mobile.ShoppingCartPage"
    CodeFile="ShoppingCart.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="ise" Namespace="InterpriseSuiteEcommerceControls" Assembly="InterpriseSuiteEcommerceControls" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" Src="XmlPackageControl.ascx" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<asp:literal id="ValidationScript" runat="server"></asp:literal>
<asp:literal id="JSPopupRoutines" runat="server"></asp:literal>
<form id="CartForm" onsubmit="return Cart_Validator(this)" runat="server">
<b>
    <asp:literal id="RedirectToSignInPageLiteral" runat="server"></asp:literal>
</b>
<asp:panel id="BodyPanel" runat="server">
        <div float:left >
            <ise:Topic runat="server" ID="HeaderMsg" TopicName="CartPageHeader" />
            <asp:Literal ID="XmlPackage_ShoppingCartPageHeader" runat="server"></asp:Literal>
            <%-- Top menu Section --%>

            <div class="signin_main removeMarginBottom">
                <div class="signin_info">
                    <asp:Panel ID="ShippingInformation" runat="server" CssClass="customlinksLayout">
                        <asp:Image ID="redarrow1" AlternateText="" runat="server" />
                        <a href="popup.aspx?Title='Shipping+Information'&topic=shipping" class="kitdetaillink"> 
                            <asp:Literal ID="shoppingcartaspx8" runat="server"></asp:Literal>
                        </a>
                    </asp:Panel>
                    <div class="customlinksLayout">
                        <asp:Image ID="redarrow2" AlternateText="" runat="server" />
                        <a href="popup.aspx?Title='Return+Policy+Information'&topic=returns" class="kitdetaillink">
                            <asp:Literal ID="shoppingcartaspx9" Text="(!shoppingcart.aspx.7!)" runat="server"></asp:Literal>
                        </a>
                    </div>
                    <div class="customlinksLayout">
                        <asp:Image ID="redarrow3" AlternateText="" runat="server" />
                        <a href="popup.aspx?Title='Privacy+Information'&topic=privacy" class="kitdetaillink">
                            <asp:Literal ID="shoppingcartaspx10" Text="(!shoppingcart.aspx.8!)" runat="server"></asp:Literal>
                        </a>
                    </div>
                    <asp:Panel ID="AddresBookLlink" runat="server" CssClass="customlinksLayout hidden">
                        <asp:Image ID="redarrow4" AlternateText="" runat="server" />
                        <a href="selectaddress.aspx?returnurl=shoppingcart.aspx&AddressType=Shipping">
                            <asp:Literal ID="shoppingcartaspx11" Text="(!shoppingcart.aspx.9!)" runat="server"></asp:Literal>
                        </a>
                    </asp:Panel>
                </div>

                <div class="button_layout">
                    <uc1:ISEMobileButton ID="btnContinueShoppingTop" runat="server" />
                </div>

                <div class="button_layout">
                    <uc1:ISEMobileButton ID="btnCheckOutNowTop" runat="server" />
                </div>

            </div>

            <%-- AlternativeCheckouts Section --%>
            <asp:Panel runat="server" ID="AlternativeCheckouts" Visible="false" class="signin_main">

                <div class="signin_info">

                    <div class="tableHeaderArea" >
                        <asp:Label ID="Label3" runat="server" Text="(!shoppingcart.aspx.14!)" Style="margin-right: 7px;"></asp:Label>
                    </div>

                    <div class="signin_info_body">

                        <asp:Panel runat="server" ID="GoogleCheckoutSpan" Visible="false" class="button_layout">
                            <asp:ImageButton ImageAlign="Top" runat="server" ID="btnGoogleCheckout" OnClick="btnGoogleCheckout_Click"
                                ImageUrl="http://checkout.google.com/buttons/checkout.gif?merchant_id=&w=180&h=46&style=trans&variant=text" />
                        </asp:Panel>

                        <asp:Panel runat="server" ID="PayPalExpressSpan" Visible="false" class="button_layout">
                            <asp:ImageButton ID="btnPayPalExpressCheckoutTop" ImageUrl="http://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif"
                                runat="server" OnClick="btnPayPalExpressCheckout_Click" />
                        </asp:Panel>

                    </div>

                </div>

            </asp:Panel>

            <asp:Panel ID="pnlCouponError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="CouponError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            
            <asp:Panel ID="pnlErrorMsg" runat="Server" Visible="false">
                <div class="signin_main">
                    <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="Server"></asp:Label>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlInventoryTrimmedError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="InventoryTrimmedError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMinimumQuantitiesUpdatedError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="MinimumQuantitiesUpdatedError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderAmountError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="MeetsMinimumOrderAmountError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderWeightError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="MeetsMinimumOrderWeightError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMeetsMinimumOrderQuantityError" runat="Server" Visible="false">
                <p>
                    <asp:Label ID="MeetsMinimumOrderQuantityError" CssClass="errorLg" runat="Server"></asp:Label></p>
            </asp:Panel>
            <asp:Panel ID="pnlMicropay_EnabledError" runat="Server" Visible="false">
                <asp:Literal ID="Micropay_EnabledError" runat="Server"></asp:Literal>
            </asp:Panel>
            <br />
            <%-- Shopping Cart Summary Section --%>
            <div>
                <asp:Literal ID="CartItems" runat="server"></asp:Literal>
            </div>

            <script type="text/javascript">

                function ShoppingKitShowHideDetails(id, linkId, showText, hideText) {
                    var kitDetailId = '#' + id;
                    var linkbuttonId = '#' + linkId;
                    jqueryHideShow(kitDetailId, linkbuttonId, showText, hideText);
                }

                function MultipleAddressShowHideDetails(id, linkId, showText, hideText) {
                    var kitDetailId = '#' + id;
                    var linkbuttonId = '#' + linkId;
                    jqueryHideShow(kitDetailId, linkbuttonId, showText, hideText);
                }

            </script>

            <div class="button_layout">
                <uc1:ISEMobileButton ID="btnUpdateCart1" runat="server" />
            </div>
            <br />
            <%-- order options --%>
            <asp:Panel ID="pnlOrderOptions" runat="server" Visible="false">
                <table width="100%" cellpadding="2" style="border-style: solid; border-width: 0px;
                    border-color: #444444">
                    <tr>
                        <td align="left" valign="top">
                            <table width="100%" cellpadding="4" style="border-style: solid; border-width: 1px;
                                border-color: #444444;">
                                <tr>
                                    <td align="left" valign="top">
                                        <div style="text-align: center; width: 100%;">
                                            <table width="100%">
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="shoppingcartcs27" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.5!)"
                                                            runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="shoppingcartcs121" runat="server" CssClass="OrderOptionsRowHeader"
                                                            Text="(!shoppingcart.cs.37!)">
                                                        </asp:Label>
                                                    </td>
                                                    <td align="center">
                                                        <asp:Label ID="shoppingcartcs28" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.6!)"
                                                            runat="server"></asp:Label>
                                                    </td>
                                                    <td width="25" align="center">
                                                        <asp:Label ID="shoppingcartcs29" CssClass="OrderOptionsRowHeader" Text="(!shoppingcart.cs.7!)"
                                                            runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <asp:Repeater ID="OrderOptionsList" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td align="left">
                                                                <asp:Image ID="OptionImage" runat="server" Visible="false" />
                                                                <asp:Label ID="OrderOptionName" CssClass="OrderOptionsName" runat="server"></asp:Label>
                                                                <asp:Image ID="helpcircle_gif" runat="server" AlternateText='<%# InterpriseSuiteEcommerceCommon.AppLogic.GetString("shoppingcart.cs.8",ThisCustomer.SkinID,ThisCustomer.LocaleSetting) %>'
                                                                    Style="cursor: hand; cursor: pointer;" />
                                                            </td>
                                                            <td align="left">
                                                                <asp:Label ID="lblUnitMeasureCode" runat="server" Text=""></asp:Label>
                                                                <asp:DropDownList ID="cboUnitMeasureCode" runat="server">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td align="center">
                                                                <asp:Label ID="OrderOptionPrice" CssClass="OrderOptionsPrice" runat="server"></asp:Label>
                                                            </td>
                                                            <td align="center">
                                                                <asp:HiddenField ID="hfItemCounter" runat="server" />
                                                                <ise:DataCheckBox ID="OrderOptions" runat="server" Data='<%# ((System.Xml.XmlNode)Container.DataItem)["ItemCode"].InnerText %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="middle" align="left" colspan="3">
                                                                <asp:Label ID="lblNotes" Text="Notes:" runat="server" /><br />
                                                                <asp:TextBox ID="txtOrderOptionNotes" TextMode="MultiLine" runat="server" Width="100%" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div class="button_layout">
                    <uc1:ISEMobileButton ID="btnUpdateCart2" runat="server" />
                </div>

            </asp:Panel>
            <br />
            <asp:Panel ID="pnlUpsellProducts" runat="server" Visible="false" CssClass="signin_main" >
                <asp:Literal runat="server" id="accessoriesOptions" ></asp:Literal>
            </asp:Panel>
            <asp:Panel ID="pnlCoupon" runat="server" DefaultButton="btnUpdateCart3" CssClass="signin_main removeMarginBottom" style="position: relative; overflow: auto;">
                <div class="signin_info">
                    <div class="tableHeaderArea">
                        <asp:Literal runat="server" Text="(!mobile.shoppingcart.aspx.19!)" ></asp:Literal>
                    </div>
                    <div class="signin_info_body">
                        <div class="signin_info_item">
                            <asp:Label ID="shoppingcartcs31" runat="server" Text="(!shoppingcart.cs.9!)"></asp:Label>
                        </div>
                        <div class="signin_info_item">
                            <asp:TextBox ID="CouponCode" Columns="30" MaxLength="50" runat="server" CssClass="inputTextBox"></asp:TextBox>
                        </div>
                        <div class="button_layout">
                            <uc1:ISEMobileButton ID="btnUpdateCart3" runat="server" />
                        </div>                    
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlOrderNotes" runat="server" Visible="false" DefaultButton="btnUpdateCart4" CssClass="signin_main">
                <div class="signin_info">
                    <div class="tableHeaderArea">
                        <asp:Literal runat="server" Text="(!mobile.shoppingcart.aspx.20!)" ></asp:Literal>
                    </div>
                    <div class="signin_info_body">
                        <div class="signin_info_item">
                            <asp:Label ID="lblOrderNotes" runat="server" Text="(!shoppingcart.cs.13!)"></asp:Label>
                        </div>
                        <div class="signin_info_item">
                            <asp:TextBox ID="OrderNotes" Columns="90" Rows="4" TextMode="MultiLine" Width="100%" runat="server" CssClass="inputTextBox" ></asp:TextBox>
                        </div>
                        
                        <div class="button_layout">
                            <uc1:ISEMobileButton ID="btnUpdateCart4" runat="server" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <ise:Topic ID="CartPageFooterTopic" runat="server" TopicName="CartPageFooter" />
            <asp:Literal ID="XmlPackage_ShoppingCartPageFooter" runat="server"></asp:Literal>

            <div class="button_layout">
                <uc1:ISEMobileButton ID="btnCheckOutNowBottom" runat="server" />
            </div>

        </div>
    </asp:panel>
</form>
