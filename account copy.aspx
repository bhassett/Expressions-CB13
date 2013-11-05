<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.account" CodeFile="account.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
    <title></title>
</head>
<body>
    <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
        <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
            <asp:RectangleHotSpot Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
        </asp:ImageMap>
    </asp:Panel>
    
    <asp:Label ID="unknownerrormsg" runat="server" style="color:#FF0000;"></asp:Label>
    <asp:Label ID="ErrorMsgLabel" runat="server" style="color:#FF0000;"></asp:Label>
    
    <asp:Panel ID="pnlAccountUpdated" runat="server" HorizontalAlign="left">
        <asp:Label ID="lblAcctUpdateMsg" runat="server" style="font-weight:bold;color:#FF0000;"></asp:Label><br/><br/>
    </asp:Panel>
    
    <asp:Panel ID="pnlNotCheckOutButtons" runat="server" HorizontalAlign="left">
        <asp:Image ID="redarrow1" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="accountaspx4" NavigateUrl="#OrderHistory" Text="(!account.aspx.3!)"></asp:HyperLink></b>
        <asp:Panel ID="pnlShowWishButton" runat="server"><asp:Image ID="redarrow2" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="ShowWishListButton" NavigateUrl="~/wishlist.aspx" Text="(!account.aspx.23!)"></asp:HyperLink></b></asp:Panel>
    </asp:Panel>
    
    <ise:Topic runat="server" ID="HeaderMsg" TopicName="AccountPageHeader" />
    
    <br />
    <div class="error">
        <cc1:InputValidatorSummary ID="errorSummary" runat="server" Register="false"></cc1:InputValidatorSummary>  
    </div>  
    <br />

    <form id="AccountForm" runat="server">
    
        <table width="100%" cellpadding="2" cellspacing="0" border="0" style="border-style: solid; border-width: 0px; border-color: #444444">
            <tr>
              <td align="left" valign="top">
                <asp:Image AlternateText="" ID="imgAccountinfo" runat="server"/>

                <table width="100%" cellpadding="4" cellspacing="0" border="0" style="border-style: solid; border-width: 1px; border-color: #444444;">
                    <tr>
                        <td>
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                  <td width="100%" colspan="2" style="height: 19px"><b><asp:Label ID="Label1" runat="server" Text="(!account.aspx.5!)"></asp:Label></b></td>
                                </tr>

                                <tr>
                                  <td width="100%" colspan="2">
                                    <hr />
                                  </td>
                                </tr>
                                
                                <tr>
                                    <td>
                                        <ise:BillingAddressControl ID="ctrlBillingAddress" runat="server" ShowAddresses="False" Width="100%" />
                                    </td>
                                </tr>
                            </table>
                            
                        </td>
                    </tr>                  
                </table>
              </td>
            </tr>
        </table>

        <center>
            <p>
                <asp:Button ID="btnUpdateAccount" CssClass="UpdateAccountButton" Text="(!account.aspx.6!)" runat="server"  OnClick="btnUpdateAccount_Click"  />
                <asp:Button ID="btnContinueToCheckOut" CssClass="AccountPageContinueCheckoutButton" Text="(!account.aspx.24!)" runat="server" CausesValidation="false" OnClick="btnContinueToCheckOut_Click" />
            </p>
        </center>
        <asp:ValidationSummary DisplayMode="List" ID="ValSummary" ShowMessageBox="false" runat="server" ShowSummary="true" ValidationGroup="account" ForeColor="red" Font-Bold="true"/>
        
        
        <br/>
    <table width="100%" cellpadding="2" cellspacing="0" border="0" style="border-style: solid; border-width: 0px; border-color: #444444">
      <tr>
        <td align="left" valign="top">
          <asp:Image AlternateText="" ID="imgAddressbook"  runat="server" />

          <table width="100%" cellpadding="4" cellspacing="0" border="0" style="border-style: solid; border-width: 1px; border-color: #444444;">
            <tr>
              <td align="left" valign="top">
                <table width="100%" border="0">
                  <tr>
                    <td colspan="3">
                      <b><asp:Label ID="accountaspx29" runat="server" Text="(!account.aspx.7!)" ></asp:Label></b><br />
                      <hr />
                    </td>
                  </tr>

                  <tr>
                    <td valign="top" width="50%">
                        <b><asp:Label ID="accountaspx30" runat="server" Text="(!account.aspx.8!)"></asp:Label>&#160;&#160;&#160;&#160;</b>
                        <asp:HyperLink ID="lnkChangeBilling" runat="server" CssClass="hyperlinkImageStyle"></asp:HyperLink><br/>
                        <asp:Literal ID="litBillingAddress" runat="server"></asp:Literal>
                    </td>

                    <td valign="top">
                        <asp:Panel ID="pnlShipping" runat="server">
                            <b><asp:Label ID="accountaspx32" runat="server" Text="(!account.aspx.10!)"></asp:Label>&#160;&#160;&#160;&#160;</b>
                            <asp:HyperLink ID="lnkChangeShipping" runat="Server" CssClass="hyperlinkImageStyle"></asp:HyperLink><br/>
                            <asp:Literal ID="litShippingAddress" runat="server"></asp:Literal>
                        </asp:Panel>
                    </td>
                  </tr>

                  <tr>
                    <td valign="top" width="50%"><br /></td>
                    <td valign="top"><br /><b><asp:HyperLink ID="lnkAddShippingAddress" runat="server"></asp:HyperLink></b></td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
            
        </td>
      </tr>
    </table>     
    
    <br />
    <a name="OrderHistory"></a>
    <asp:ImageButton ID="btnOrderHistory" runat="server" OnClick="btnOrderHistory_Click" CausesValidation ="false" />
    <div id="AccountOrderHistory" >
        <div id="AccountOrderHistoryLink" runat="server" >
            <asp:LinkButton ID="lnkOrderHistory" runat="server" OnClick="btnOrderHistory_Click" Text="(!account.aspx.30!)" CausesValidation ="false"/>
        </div>
        <asp:PlaceHolder ID="pnlOrderHistory2" runat="server" />
    </div>
    
    </form>
    
</body>
</html>
