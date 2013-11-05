<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.checkoutanon" CodeFile="checkoutanon.aspx.cs" %>

<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<html>
<head>
</head>
<body>
    <asp:Panel ID="HeaderPanel" runat="server" Width="90%">
        <div align="center" style="text-align: center">
            <asp:Panel ID="CheckoutPanel" runat="server">
                <div id="CheckoutSequence" align="center">
                    <asp:ImageMap ID="CheckoutMap" runat="server">
                        <asp:RectangleHotSpot HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx?resetlinkback=1&amp;checkout=true" Bottom="54" Right="50" />
                    </asp:ImageMap>
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>
    <div class="signin_main" runat="Server">
        <form runat="Server">

        <asp:Panel ID="ErrorPanel" runat="server" Visible="False" HorizontalAlign="center">
            <br />
            <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="server"></asp:Label>
            <br />
        </asp:Panel>
    
        <div id="FormPanel" runat="server" class="signin_info">
            <div class="tableHeaderArea">
                <asp:Label ID="Label6" runat="server" Text="(!checkoutanon.aspx.11!)"></asp:Label>
            </div>
            <div class="signin_info_body">
                <div class="singin_info_midheader">
                    <asp:Label ID="Label1" runat="server" Text="(!checkoutanon.aspx.3!)" />
                </div>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="(!checkoutanon.aspx.4!)" />
                        </td>
                        <td>
                            <asp:TextBox ID="EMail" runat="server" ValidationGroup="Group1" MaxLength="100" CausesValidation="True"
                                AutoCompleteType="Email" Width="157px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="EMail"
                                runat="server" ValidationGroup="Group1" ErrorMessage="!!" Display="Dynamic" Font-Bold="True"
                                SetFocusOnError="True"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="(!checkoutanon.aspx.5!)" />
                        </td>
                        <td>
                            <asp:TextBox ID="Password" runat="server" ValidationGroup="Group1" MaxLength="50" CausesValidation="True" TextMode="Password" Width="155px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="Password" runat="server" ErrorMessage="!!" ValidationGroup="Group1" Display="Dynamic" Font-Bold="True" SetFocusOnError="True"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <br />  
                            <asp:Label ID="Label5" runat="server" Text="(!checkoutanon.aspx.6!)" Font-Bold="true" />
                            <asp:HyperLink ID="HyperLink1" runat="server" CssClass="kitdetaillink" NavigateUrl="signin.aspx?checkout=true">(!checkoutanon.aspx.7!)</asp:HyperLink>                            
                        </td>
                    </tr>
                    <tr id="trSecurityCodeText" runat="server" visible="false">
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="(!signin.aspx.18!)" Visible="false"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="SecurityCode" runat="server" Visible="false" ValidationGroup="Group1"
                                CausesValidation="True" Width="73px" EnableViewState="False"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="SecurityCode"
                                ErrorMessage="(!signin.aspx.17!)" ValidationGroup="Group1" Enabled="False">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr id="trSecurityCodeImage" runat="server" visible="false">
                        <td colspan="2">
                            <asp:Image ID="SecurityImage" runat="server" Visible="false"></asp:Image>
                        </td>
                    </tr>
                </table>
                <br />
                <div class="button_layout">
                    <uc1:ISEMobileButton ID="btnSignInAndCheckout" runat="server" CausesValidation="true" ValidationGroup="Group1" />
                </div>
            </div>
        </div>

        <div class="signin_info">
            <div class="tableHeaderArea">
                <asp:Label ID="Label7" runat="server" Text="(!checkoutanon.aspx.8!)" />
            </div>
            <div class="signin_info_body">
                <div class="button_layout">
                    <uc1:ISEMobileButton ID="RegisterAndCheckoutButton" runat="server" CausesValidation="false" />
                </div>
                <br />
                <ise:Topic runat="server" ID="Teaser" TopicName="CheckoutAnonTeaser" />
            </div>
        </div>

        <div runat="Server" id="PasswordOptionalPanel" visible="false" class="signin_info">
            <div class="tableHeaderArea">
                <asp:Label ID="Label8" runat="server" Text="(!checkoutanon.aspx.9!)" />
            </div>
            <div class="signin_info_body">
                
                <div class="singin_info_midheader">
                    <asp:Label ID="Label9" runat="server" Text="(!checkoutanon.aspx.10!)" />
                </div>
                
                <div class="button_layout">
                    <uc1:ISEMobileButton ID="Skipregistration" runat="server" CausesValidation="false" />
                </div>
            </div>
        </div>

        <asp:Panel ID="ExecutePanel" runat="server" Width="90%" HorizontalAlign="center" Visible="false">
            <img src="images/spacer.gif" alt="" width="100%" height="40" />
            <asp:Label ID="SignInExecuteLabel" runat="server" Font-Bold="true"></asp:Label>
        </asp:Panel>

        </form>
    </div>
</body>
</html>
