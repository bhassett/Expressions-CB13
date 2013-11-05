<%@ Page Language="C#" AutoEventWireup="true" CodeFile="signin.aspx.cs" Inherits="InterpriseSuiteEcommerce.mobile.signin" %>

<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls"
    TagPrefix="cc1" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="ccMobile" %>
<div class="signin_main">
    <form id="form1" runat="server">
    <asp:panel id="FormPanel" runat="server">
            <%-- Error message --%>
            <div class="signin_toplayout">
                <asp:Panel ID="ErrorPanel" runat="server" Visible="False" HorizontalAlign="Left">
                    <asp:Label CssClass="errorLg" ID="ErrorMsgLabel" runat="server" />
                </asp:Panel>
                <%-- Header topic --%>
                <ise:Topic runat="server" ID="HeaderMsg" TopicName="SigninPageHeaderMobile" />
                <%-- Signin info --%>
                
            </div>
            <%-- Login Information --%>
            <div class="signin_info">
                <div class="tableHeaderArea" colspan="2">
                    <b>
                        <%--<asp:Literal ID="Label5" runat="server" Text="(!signin.aspx.7!)"></asp:Literal>--%>
                        <asp:Literal ID="Label1" runat="server" Text="(!mobile.signin.aspx.5!)"></asp:Literal>
                        </b>
                    <asp:HyperLink ID="SignUpLink" class="kitdetaillink" runat="server" style="float:right">(!mobile.signin.aspx.6!)</asp:HyperLink>
                </div>
                <div class="signin_info_body">
                    <table>
                    <tbody>
                        <tr>
                            <td>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="Group1" Display="Dynamic"
                                    CssClass="errorLg" ErrorMessage="(!signin.aspx.3!)" ControlToValidate="EMail"></asp:RequiredFieldValidator>
                                <asp:TextBox ID="EMail" CssClass="inputTextBox" runat="server" ValidationGroup="Group1" Columns="44" MaxLength="100" CausesValidation="True" AutoCompleteType="Email" >
                                </asp:TextBox>
                            </td>
                        </tr>
                        <%-- Password --%>
                        <tr>
                            <td>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="Group1" Display="Dynamic"
                                    CssClass="errorLg" ErrorMessage="(!signin.aspx.4!)" ControlToValidate="Password"></asp:RequiredFieldValidator>
                                <asp:TextBox ID="Password" CssClass="inputTextBox" runat="server" ValidationGroup="Group1"
                                    Columns="44" MaxLength="50" CausesValidation="True" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <%-- Capcha --%>
                        <tr>
                            <td>
                                <asp:Panel ID="SecurityCodePanel" runat="server" Visible="false">
                                    <asp:TextBox ID="SecurityCode" CssClass="inputTextBox" runat="server" ValidationGroup="Group1"
                                        CausesValidation="True" EnableViewState="False"></asp:TextBox>
                                    <br />
                                    <asp:RequiredFieldValidator ID="SecurityCodeValidator" runat="server" ControlToValidate="SecurityCode" Display="Dynamic"
                                        CssClass="errorLg" ErrorMessage="(!signin.aspx.17!)" ValidationGroup="Group1"></asp:RequiredFieldValidator>
                                </asp:Panel>
                            </td>
                        </tr>
                        <%-- Capcha Image --%>
                        <tr>
                            <td>
                                <div style="text-align: center;">
                                    <asp:Panel ID="SecurityImagePanel" runat="server" Visible="false">
                                        <asp:Image ID="SecurityImage" runat="server"></asp:Image>&nbsp;&nbsp;
                                    </asp:Panel>
                                </div>
                            </td>
                        </tr>
                        <%-- Remember Me --%>
                        <tr>
                            <td>
                                <asp:Literal ID="Label7" runat="server" Text="(!signin.aspx.11!)"></asp:Literal>
                                <asp:CheckBox ID="PersistLogin" runat="server"></asp:CheckBox>
                            </td>
                        </tr>
                        <%-- Login button --%>
                        <tr>
                            <td>
                                <div class="button_layout">
                                    <ccMobile:ISEMobileButton ID="LoginButton" runat="server" ValidationGroup="Group1" />
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
                </div>
            </div>

            <div class="signin_info">
                
                <div class="tableHeaderArea">
                    <asp:Literal ID="Label9" runat="server" Text="(!signin.aspx.14!)"></asp:Literal>
                </div>

                <div class="signin_info_body">

                    <div class="singin_info_midheader" style="text-align:left">
                        <asp:Literal ID="Label6" runat="server" Text="(!signin.aspx.12!)"></asp:Literal>
                        <br />
                        <br />
                        <asp:Literal ID="Label8" runat="server" Text="(!signin.aspx.13!)"></asp:Literal>
                        <br />
                    </div>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ForgotEMail" Display="Dynamic" CssClass="errorLg" ErrorMessage="(!signin.aspx.3!)" ValidationGroup="Group2"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="ForgotEMail" CssClass="inputTextBox" runat="server" ValidationGroup="Group2" CausesValidation="True" AutoCompleteType="Email" Columns="44"></asp:TextBox>

                    <div class="button_layout">
                        <ccMobile:ISEMobileButton ID="RequestPassword" runat="server" ValidationGroup="Group2" />
                    </div>

                </div>

            </div>
        </asp:panel>
    <%-- Good login --%>
    <asp:panel id="ExecutePanel" runat="server">
            <div align="center">
                <b>
                    <asp:Literal ID="SignInExecuteLabel" runat="server"></asp:Literal></b></div>
        </asp:panel>
    <asp:checkbox id="DoingCheckout" runat="server" visible="False" />
    <asp:literal id="ReturnURL" runat="server" text="default.aspx" visible="False" />
    </form>
</div>