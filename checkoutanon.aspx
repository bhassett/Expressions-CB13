<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.checkoutanon" CodeFile="checkoutanon.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<html>
<head>
</head>
<body>
    <form runat="Server">
    <div align="center">
        <asp:Panel ID="HeaderPanel" runat="server" Width="90%">
            <div align="center" style="text-align: center">
                <asp:Panel ID="CheckoutPanel" runat="server">
                    <div id="CheckoutSequence" align="center">
                        <asp:ImageMap ID="CheckoutMap" runat="server">
                            <asp:RectangleHotSpot Bottom="54" HotSpotMode="Navigate" NavigateUrl="shoppingcart.aspx?resetlinkback=1&amp;checkout=true"
                                Right="87" />
                        </asp:ImageMap><br />
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>
        <asp:Panel ID="ErrorPanel" runat="server" Visible="False" HorizontalAlign="center">
            <asp:Label ID="ErrorMsgLabel" CssClass="errorLg" runat="server"></asp:Label><br />
            <br />
        </asp:Panel>
        <asp:Panel ID="FormPanel" runat="server" Width="90%">
            <table cellspacing="0" cellpadding="1" width="550" border="0" id="table1">
                <tr>
                    <td valign="top" width="300">
                        <asp:Label ID="Label6" runat="server" Text="(!checkoutanon.aspx.11!)" Font-Bold="true"></asp:Label><br />
                        <br />
                        <asp:Label ID="Label1" runat="server" Text="(!checkoutanon.aspx.3!)" /><br />
                        <br />
                        <table cellspacing="0" cellpadding="0" width="100%" border="0" id="table2">
                            <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server" Text="(!checkoutanon.aspx.4!)" Font-Bold="true" />
                                </td>
                            </tr>
                            <tr>
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
                                    <img src="images/spacer.gif" alt="" width="100%" height="10" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label3" runat="server" Text="(!checkoutanon.aspx.5!)" Font-Bold="true" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="Password" runat="server" ValidationGroup="Group1" MaxLength="50"
                                        CausesValidation="True" TextMode="Password" Width="155px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="Password"
                                        runat="server" ErrorMessage="!!" ValidationGroup="Group1" Display="Dynamic" Font-Bold="True"
                                        SetFocusOnError="True"></asp:RequiredFieldValidator><br />
                                    <br />
                                </td>
                            </tr>
                            <tr id="trSecurityCodeText" runat="server" visible="false">
                                <td>
                                    <table>
                                        <tr>
                                            <td valign="top" align="left">
                                                <font class="LightCellText">
                                                    <asp:Label ID="Label4" runat="server" Text="(!signin.aspx.18!)" Visible="false"></asp:Label></font>
                                            </td>
                                            <td valign="top" align="left">
                                                <asp:TextBox ID="SecurityCode" runat="server" Visible="false" ValidationGroup="Group1"
                                                    CausesValidation="True" Width="73px" EnableViewState="False"></asp:TextBox>
                                                <br />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="SecurityCode"
                                                    ErrorMessage="(!signin.aspx.17!)" ValidationGroup="Group1" Enabled="False">
                                                </asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="trSecurityCodeImage" runat="server" visible="false">
                                <td valign="top" align="left">
                                    <asp:Image ID="SecurityImage" runat="server" Visible="false"></asp:Image>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td valign="middle">    
                                     <asp:Button ID="btnSignInAndCheckout" CssClass="site-button content" Text="(!checkoutanon.aspx.12!)" 
                                     runat="server" ValidationGroup="Group1" CausesValidation="true" OnClick="btnSignInAndCheckout_Click" />
                                    <br />
                                    <br />
                                    <asp:Label ID="Label5" runat="server" Text="(!checkoutanon.aspx.6!)" Font-Bold="true" /><br />
                                    <br />
                                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="signin.aspx?checkout=true">(!checkoutanon.aspx.7!)</asp:HyperLink>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" width="20" rowspan="2">
                    </td>
                    <td valign="top" width="240">
                        <asp:Label ID="Label7" runat="server" Text="(!checkoutanon.aspx.8!)" Font-Bold="true" /><br />
                        <br />
                        <asp:Button ID="RegisterAndCheckoutButton" CssClass="site-button content" Text="(!checkoutanon.aspx.13!)"
                                runat="server" CausesValidation="false" OnClick="RegisterAndCheckoutButton_Click" />
                        <br />
                        <br />
                        <ise:Topic runat="server" ID="Teaser" TopicName="CheckoutAnonTeaser" />
                        <asp:Panel runat="Server" ID="PasswordOptionalPanel" Visible="false">
                        <br />
                        <br />
                        <asp:Label ID="Label8" runat="server" Text="(!checkoutanon.aspx.9!)" Font-Bold="true" /><br />
                        <br />
                        <asp:Label ID="Label9" runat="server" Text="(!checkoutanon.aspx.10!)" /><br />
                        <br />
                        <asp:Button ID="Skipregistration" CssClass="site-button content" Text="(!checkoutanon.aspx.14!)"
                                runat="server" CausesValidation="false" OnClick="Skipregistration_Click" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="ExecutePanel" runat="server" Width="90%" HorizontalAlign="center" Visible="false">
            <img src="images/spacer.gif" alt="" width="100%" height="40" />
            <asp:Label ID="SignInExecuteLabel" runat="server" Font-Bold="true"></asp:Label>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
