<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.signin" CodeFile="signin.aspx.cs" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<html>
<head>
</head>
<body>
    <form runat="Server" method="POST" id="SigninForm" name="SigninForm">
        <div align="center">
            <asp:Panel ID="FormPanel" runat="server" Width="90%">
                <div align="center" style="text-align: center">
                    <asp:Panel ID="CheckoutPanel" runat="server">
                        <div id="CheckoutSequence" align="center">
                            <asp:ImageMap ID="CheckoutMap" runat="server" ImageUrl="(!skins/skin_(!SKINID!)/images/step_2.gif!)">
                                <asp:RectangleHotSpot Bottom="54" HotSpotMode="Navigate"
                                    NavigateUrl="shoppingcart.aspx?resetlinkback=1&amp;checkout=true" Right="87" />
                            </asp:ImageMap>
                            </div>
                    </asp:Panel>
                    <asp:Panel ID="ErrorPanel" runat="server" Visible="False" HorizontalAlign="Left">
                                <asp:Label CssClass="errorLg" ID="ErrorMsgLabel" runat="server"></asp:Label>
                    </asp:Panel>
                    <ise:Topic runat="server" ID="HeaderMsg" TopicName="SigninPageHeader" />
                    <p align="left">
                        <b>
                            <asp:Label ID="Label11" runat="server" Text="(!signin.aspx.5!)"></asp:Label>&nbsp;&nbsp;(<asp:HyperLink
                                ID="SignUpLink" runat="server">(!signin.aspx.6!)</asp:HyperLink>)</b></p>
                    <table width="100%">
                        <tbody>
                            <tr valign="top">
                                <td class="MediumCell" align="left" width="90%">
                                    &nbsp;<b><asp:Label ID="Label5" runat="server" Text="(!signin.aspx.7!)"></asp:Label></b>
                                </td>
                            </tr>
                            <tr class="MediumCell" valign="top">
                                <td class="LightCell" align="left" width="90%">
                                    <table cellspacing="5" cellpadding="0" width="100%" border="0">
                                        <tbody>
                                            <tr valign="baseline">
                                                <td colspan="2">
                                                    <b><font class="LightCellText">
                                                        <asp:Label ID="Label4" runat="server" Text="(!signin.aspx.8!)"></asp:Label>
                                                    </font></b>
                                                </td>
                                            </tr>
                                            <tr valign="baseline">
                                                <td valign="middle" align="right">
                                                    <font class="LightCellText"><asp:Label ID="Label3" runat="server" Text="(!signin.aspx.9!)"></asp:Label></font>
                                                </td>
                                                <td valign="middle" align="left">
                                                    <asp:TextBox ID="EMail" runat="server" ValidationGroup="Group1" Columns="30" MaxLength="100" CausesValidation="True"
                                                        AutoCompleteType="Email"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="Group1"
                                                        ErrorMessage="(!signin.aspx.3!)" ControlToValidate="EMail"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="middle" align="right">
                                                    <font class="LightCellText"><asp:Label ID="Label2" runat="server" Text="(!signin.aspx.10!)"></asp:Label></font>
                                                </td>
                                                <td valign="middle" align="left">
                                                    <asp:TextBox ID="Password" runat="server" ValidationGroup="Group1" Columns="30" MaxLength="50"
                                                        CausesValidation="True" TextMode="Password" AutoCompleteType="Disabled"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="Group1"
                                                        ErrorMessage="(!signin.aspx.4!)" ControlToValidate="Password"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr valign="baseline">
                                                <td valign="middle" align="right">
                                                    <font class="LightCellText">
                                                        <asp:Label ID="Label1" runat="server" Text="(!signin.aspx.18!)" Visible="False"></asp:Label></font></td>
                                                <td valign="middle" align="left">
                                                    <asp:TextBox ID="SecurityCode" runat="server" Visible="False" ValidationGroup="Group1"
                                                        CausesValidation="True" Width="73px" EnableViewState="False"></asp:TextBox><asp:RequiredFieldValidator
                                                            ID="RequiredFieldValidator4" runat="server" ControlToValidate="SecurityCode"
                                                            ErrorMessage="(!signin.aspx.17!)" ValidationGroup="Group1" Enabled="False"></asp:RequiredFieldValidator></td>
                                            </tr>
                                            <tr valign="baseline">
                                                <td valign="middle" align="center" colspan="2">
                                                    <asp:Image ID="SecurityImage" runat="server" Visible="False"></asp:Image>
                                                </td>
                                            </tr>
                                            <tr valign="baseline">
                                                <td valign="middle" align="right">
                                                    <font class="LightCellText"><asp:Label ID="Label7" runat="server" Text="(!signin.aspx.11!)"></asp:Label></font>
                                                </td>
                                                <td valign="middle" align="left">
                                                    <asp:CheckBox ID="PersistLogin" runat="server"></asp:CheckBox>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td align="left" width="90%">
                                    <p align="right">
                                        <asp:Button ID="LoginButton" OnClick="LoginButton_Click" runat="server" CssClass="site-button content" ValidationGroup="Group1">
                                        </asp:Button>
                                    </p>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <p align="left">
                        <b>
                            <asp:Label ID="Label6" runat="server" Text="(!signin.aspx.12!)"></asp:Label>
                        </b>&nbsp;</p>
                    <p align="left">
                        <asp:Label ID="Label8" runat="server" Text="(!signin.aspx.13!)"></asp:Label>
                    </p>
                    <table width="100%">
                        <tbody>
                            <tr class="MediumCell" valign="top">
                                <td align="left" width="100%">
                                    &nbsp;<font class="MediumCellText"><b><asp:Label ID="Label9" runat="server" Text="(!signin.aspx.14!)"></asp:Label></b></font></td>
                            </tr>
                            <tr class="MediumCell" valign="top">
                                <td class="LightCell" align="left" width="100%">
                                    <table cellspacing="5" cellpadding="0" border="0" width="100%">
                                        <tbody>
                                            <tr valign="baseline">
                                                <td align="right" style="height: 24px">
                                                    <font class="LightCellText">&nbsp;<asp:Label ID="Label12" runat="server" Text="(!signin.aspx.9!)"></asp:Label>
                                                    </font>
                                                </td>
                                                <td style="height: 24px">
                                                    <asp:TextBox ID="ForgotEMail" runat="server" ValidationGroup="Group2" CausesValidation="True"
                                                        AutoCompleteType="Email" Columns="30"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ForgotEMail" ErrorMessage="(!signin.aspx.3!)" ValidationGroup="Group2"></asp:RequiredFieldValidator>                                                                                                    
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td align="left" width="100%">
                                    <p align="right">
                                        <asp:Button ID="RequestPassword" OnClick="RequestPassword_Click" runat="server" CssClass="site-button content" ValidationGroup="Group2"></asp:Button></p>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </asp:Panel>
            <asp:Panel ID="ExecutePanel" runat="server" Width="90%">
                <div align="center">
                    <img src="images/spacer.gif" alt="" width="100%" height="40" />
                    <b>
                        <asp:Literal ID="SignInExecuteLabel" runat="server"></asp:Literal></b></div>
            </asp:Panel>
            <asp:CheckBox ID="DoingCheckout" runat="server" Visible="False" />
            <asp:Label ID="ReturnURL" runat="server" Text="default.aspx" Visible="False" />
        </div>
    </form>
</body>
</html>
