<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PaymentTermControl.ascx.cs" Inherits="UserControls_PaymentTermControl" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>

<asp:Panel runat="server" ID="pnlNoPayment" Visible="false">
    <span>
        <asp:Literal ID="litNoPaymentRequired" runat="server"></asp:Literal>
    </span>
</asp:Panel>

<table style="width: 100%">
    <asp:Panel ID="pnlPaymentTermOptions" runat="server">
        <asp:Repeater ID="rptPaymentOptions" runat="server">
            <ItemTemplate>
                <div class="payment-option">
                    <input id="<%# this.ClientID + "_" + this.ClientID + "_" + Container.DataItemAs<PaymentTermDTO>().Counter %>" 
                            type="radio"
                            name="<%# this.ClientID %>$"
                            <%# Container.DataItemAs<PaymentTermDTO>().IsSelected ? " checked=\"checked\" " : "" %> 
                            <%# CreatePaymentMethodAttribute(Container.DataItemAs<PaymentTermDTO>()) %>
                            <%# CreatePaymentTermAttribute(Container.DataItemAs<PaymentTermDTO>()) %> />

                    <asp:Label runat="server" ID="litCreditCardImages" Visible="false" >
                        <img style="vertical-align: middle" width="30" height="20" border="0" src="<%= AppLogic.LocateImageURL("skins/skin_" + ThisCustomer.SkinID.ToString() + "/images/visa.gif") %>" alt="visa" />
                        <img style="vertical-align: middle" width="30" height="20" border="0" src="<%= AppLogic.LocateImageURL("skins/skin_" + ThisCustomer.SkinID.ToString() + "/images/discover.gif") %>" alt="discover" />
                        <img style="vertical-align: middle" width="30" height="20" border="0" src="<%= AppLogic.LocateImageURL("skins/skin_" + ThisCustomer.SkinID.ToString() + "/images/amex.gif") %>" alt="amex" />
                        <img style="vertical-align: middle" width="30" height="20" border="0" src="<%= AppLogic.LocateImageURL("skins/skin_" + ThisCustomer.SkinID.ToString() + "/images/mastercard.gif") %>" alt="mastercard" />
                    </asp:Label>
                    
                    <%# Container.DataItemAs<PaymentTermDTO>().PaymentTermCode %> - <%# Container.DataItemAs<PaymentTermDTO>().Description %>
                </div>
                <div style="clear: both"></div>
                <div id="paypalRow" runat="server" visible="false" class="payment-option">
                    <input id="<%# this.ClientID + "_" + this.ClientID + "_PaypalOption" %>"
                                       type="radio"
                                       name="<%# this.ClientID %>$"
                                       pm="<%# DomainConstants.PAYMENT_METHOD_PAYPALX %>"
                                       pr="<%# DomainConstants.PAYMENT_METHOD_PAYPALX %>" />
                          
                                <a href="#" onclick="javascript:window.open('https://www.paypal.com/us/cgi-bin/webscr?cmd=xpt/Marketing/popup/OLCWhatIsPayPal-outside','olcwhatispaypal','toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes,resizable=yes,width=400,height=350');">
                                <img style="vertical-align: middle" src="https://www.paypal.com/en_US/i/logo/PayPal_mark_37x23.gif" border="0" alt="Acceptance Mark"></a>
                                <%= AppLogic.GetString("pm.paypal.display", Customer.Current.SkinID, Customer.Current.LocaleSetting, true)%>
                                 - <%= AppLogic.GetString("pm.paypaldescription.display", Customer.Current.SkinID, Customer.Current.LocaleSetting, true) %>
                </div>
                <div style="clear: both"></div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
    
    <asp:Panel ID="pnlCreditCardInfo" runat="server">
        <tr id="<%= this.ClientID + "_"%>cardFormRow">
            <td class="payment-method-option">
                <table class="credit-card-payment-method-panel">
                    <tbody>
                        <tr>
                            <td>
                                <span>
                                    <asp:Literal ID="litNameOnCard" runat="server"></asp:Literal>
                                </span>
                            </td>
                            <td>
                                <asp:TextBox ID="nameOnCard" class="light-style-input" type="text" size="50" maxlength="100" name="NameOnCard" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>
                                    <asp:Literal ID="litCardNumber" runat="server"></asp:Literal>
                                </span>
                            </td>
                            <td>
                                <asp:TextBox ID="cardNumber" class="light-style-input" type="text" size="50" maxlength="100" name="CardNumber" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span>
                                    <asp:Literal ID="litCVV" runat="server"></asp:Literal>
                                </span>
                            </td>
                            <td>
                                <asp:TextBox ID="cvv" class="light-style-input" type="text" autocomplete="off" size="5" maxlength="10" name="CVV" runat="server"></asp:TextBox>
                                ( <asp:LinkButton ID="lnkWhatIsCvv" runat="server" href="javascript:void(0);"></asp:LinkButton> )
                            </td>
                        </tr>
                        <tr>
                            <td>
                                 <span>
                                    <asp:Literal ID="litCardType" runat="server"></asp:Literal>
                                </span>
                            </td>
                            <td>
                                <asp:DropDownList ID="cardType" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <asp:Panel ID="pnlStartDate" runat="server" Visible="false">
                            <tr>
                                <td>
                                    <span>
                                        <asp:Literal ID="litCardStartDate" runat="server"></asp:Literal>
                                    </span>
                                </td>
                                <td>
                                    <asp:DropDownList ID="startMonth"  class="light-style-input"  name="CardStartMonth" runat="server"></asp:DropDownList>
                                    / 
                                    <asp:DropDownList ID="startYear"  class="light-style-input"  name="CardStartYear" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td>
                                <span>
                                    <asp:Literal ID="litExpirationDate" runat="server"></asp:Literal>
                                </span>
                            </td>
                            <td>
                                <asp:DropDownList ID="expirationMonth"  class="light-style-input"  name="ExpirationMonth" runat="server"></asp:DropDownList>
                                / 
                                <asp:DropDownList ID="expirationYear"  class="light-style-input"  name="ExpirationYear" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <asp:Panel ID="pnlCardIssueNumber" runat="server" Visible="false">
                            <tr>
                                <td>
                                    <span>
                                        <asp:Literal ID="litCardIssueNumber" runat="server"></asp:Literal>
                                    </span>
                                </td>
                                <td>
                                    <asp:TextBox ID="cardIssueNumber" class="light-style-input" type="text" autocomplete="off" size="2" maxlength="2" name="CardIssueNumber" runat="server"></asp:TextBox>
                                    &nbsp;
                                    <span>
                                        <asp:Literal ID="litCardIssueNumberInfo" runat="server"></asp:Literal>
                                    </span>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel ID="pnlTokenization" runat="server" Visible="false">
                            <tr>
                                <td>
                                    <span>
                                        <asp:Literal ID="litSaveCardAs" runat="server"></asp:Literal>
                                    </span>
                                </td>
                                <td>
                                    <asp:TextBox ID="cardDescription" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:CheckBox ID="chkSaveCreditCardInfo" runat="server"/>
                                    <span>
                                        <asp:Literal ID="litSaveThisCreditCardInfo" runat="server"></asp:Literal>
                                    </span>
                                </td>
                            </tr>
                        </asp:Panel>
                    </tbody>
                </table>
            </td>
        </tr>
    </asp:Panel>

    <asp:Panel ID="pnlPONumberInfo" runat="server">
        <tr id="<%= this.ClientID + "_"%>poNumberRow">
        <td class="payment-method-option">
            <table class="purchase-order-payment-method-panel">
                <tr>
                    <td>
                        <span>
                            <asp:Literal ID="litPONumber" runat="server"></asp:Literal>
                        </span>
                    </td>
                    <td>
                        <asp:TextBox ID="poNumber" class="light-style-input" type="text" autocomplete="off" size="30" maxlength="30" name="ctl12" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    </asp:Panel>
   
    <asp:Panel ID="pnlPayPalRedirectInfo" runat="server">
        <tr id="<%= this.ClientID + "_"%>externalRow">
            <td class="payment-method-option">
                <table class="redirect-payment-method-panel">
                <tr>
                    <td>
                        <span class="strong-font">
                            <asp:Literal ID="litPayPalExternal" runat="server"></asp:Literal>
                        </span>
                    </td>
                </tr>
                </table>
            </td>
        </tr>
   </asp:Panel>

    <asp:Panel ID="pnlTerms" runat="server">
        <tr>
            <td class="payment-method-option">
                <div>
                    <asp:CheckBox ID="termsAndConditionsChecked" runat="server" name="TermsAndConditionsChecked"/>
                    <asp:Literal ID="litTermsAndConditionsHTML" runat="server"></asp:Literal>
                </div>
            </td>
        </tr>
    </asp:Panel>
</table>

<div id="pnlHiddenField">
    <input type="hidden" id="paymentTerm" runat="server" />
    <input type="hidden" id="paymentMethod" runat="server" />
</div>