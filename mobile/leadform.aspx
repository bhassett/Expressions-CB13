<%@ Page Language="C#" ClientTarget="UpLevel" CodeFile="leadform.aspx.cs" Inherits="InterpriseSuiteEcommerce.leadform" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Interprise Leads</title>
    <script type="text/javascript" src="js/jquery/jquery.format.1.05.js"></script>
    <script type="text/javascript" src="js/leadform.js"></script>
</head>
<body>
    <div class="signin_main">
        <div id="lead-form-logo">
        </div>
        <div class="signin_info">
            <div class="tableHeaderArea gotextcenter">
                <ise:Topic ID="LeadFormThankYouPageTopic" runat="server" TopicName="MobileLeadFormThankYouPage" />
            </div>
        </div>
        <div class="signin_info">
            <div class="tableHeaderArea" id="lead-form-tips">
                <asp:Label ID="lblFormTips" runat="server">(!leadform.aspx.2!)</asp:Label>    
            </div>

            <div class="signin_info_body">
                
                <asp:Panel ID="pnlMain" runat="server" HorizontalAlign="Center" Visible="true">
                    <ise:Topic runat="server" ID="LeadFormPageHeader" TopicName="LeadFormPageHeader" />
                </asp:Panel>

                <asp:Panel ID="pnlErrorMsg" runat="Server" Font-Italic="true" ForeColor="red" HorizontalAlign="Center" Style="margin-left: 20px;">
                </asp:Panel>

                <form id="frmLeadForm" runat="server">
                <div id="lead-form-field-wrapper">
                    <div id="lead-form-caption" style="display:none">
                        <asp:Literal ID="Literal1" runat="server" Visible="True" Text="(!leadform.aspx.22!)"></asp:Literal>
                    </div>
                    <div id="lead-form-container">
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <asp:Label ID="Label11" runat="server">(!leadform.aspx.3!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <asp:DropDownList ID="drpLstSalutation" runat="server">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="LblFullName" runat="server">(!leadform.aspx.4!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <input type="text" id="txtFirstNameLF" />
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label8" runat="server">(!leadform.aspx.5!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <input type="text" id="txtMiddleNameLF" />
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label9" runat="server">(!leadform.aspx.6!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <input type="text" id="txtLastNameLF" />
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <asp:Label ID="Label10" runat="server">(!leadform.aspx.7!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <asp:DropDownList ID="drpLstSuffix" runat="server">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label2" runat="server">(!leadform.aspx.8!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <input type="text" id="txtEmailLF" />
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label3" runat="server">(!leadform.aspx.9!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <asp:DropDownList ID="drpLstCountriesLF" runat="server">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label4" runat="server">(!leadform.aspx.10!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <select id="drpLstStatesLF">
                                </select>
                                <div id="state-tips">
                                </div>
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label5" runat="server">(!leadform.aspx.11!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <input type="text" id="txtCityLF" />
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label6" runat="server">(!leadform.aspx.12!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <input type="text" maxlength="20" id="txtAreaCodeLF" />
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label7" runat="server">(!leadform.aspx.13!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <textarea id="txtMessageLF" rows="" ></textarea>
                            </div>
                        </div>
                        <div class="lead-form-field-wrapper">
                            <div class="lead-form-field-label">
                                <span class="required">*</span>
                                <asp:Label ID="Label1" runat="server">(!leadform.aspx.14!)</asp:Label>
                            </div>
                            <div class="lead-form-field-control">
                                <input type="text" id="txtCaptcha" />
                                <div id="captcha-wrapper">
                                    <div id="captcha-image">
                                        <img alt="captcha" src="Captcha.ashx?id=1" id="captcha" />
                                    </div>
                                    <div id="captcha-refresh">
                                        <a href="javascript:void(1);" id="captcha-refresh-button" alt="Refresh Captcha" title="Click to change the security code">
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="button_layout">
                            <uc1:ISEMobileButton ID="btnSubmitLF" runat="server" UseSubmitBehavior="false" />
                        </div>
                        <div class="hidden" id="required-error">
                            <asp:Literal ID="lRequiredError" runat="server" Visible="True" Text="(!leadform.aspx.16!)"></asp:Literal>
                        </div>
                        <div class="hidden" id="email-error">
                            <asp:Literal ID="lEmailError" runat="server" Visible="True" Text="(!leadform.aspx.17!)"></asp:Literal>
                        </div>
                        <div class="hidden" id="captcha-error">
                            <asp:Literal ID="lCaptchaError" runat="server" Visible="True" Text="(!leadform.aspx.18!)"></asp:Literal>
                        </div>
                        <div class="hidden" id="email-duplicate-error">
                            <asp:Literal ID="lEmailDuplicateError" runat="server" Visible="True" Text="(!leadform.aspx.19!)"></asp:Literal>
                        </div>
                        <div class="hidden" id="lead-duplicate-error">
                            <asp:Literal ID="lLeadDuplicateError" runat="server" Visible="True" Text="(!leadform.aspx.20!)"></asp:Literal>
                        </div>
                        <div class="hidden" id="local-settings">
                            <asp:Literal ID="lLocalSettings" runat="server" Visible="True"></asp:Literal>
                        </div>
                        <div class="hidden" id="skin-id">
                            <asp:Literal ID="lSkinId" runat="server" Visible="True"></asp:Literal>
                        </div>
                    </div>
                </div>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
