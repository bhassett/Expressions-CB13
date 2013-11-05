
<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="contactus.aspx.cs" Inherits="InterpriseSuiteEcommerce.contactus"%>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title></title>
<script type="text/javascript" src="jscripts/jquery/jquery.cbe.contactus.js"></script>
</head>
<body>
<ise:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="False" />
<div class="clear-both height-5">&nbsp;</div>
<form id="frmContactUs" runat="server" action=>
<asp:Panel ID="pnlPageContentWrapper" runat="server">
    <div id="divPageTitle"><h3><asp:Literal ID="LitPageWelcomeHeader_Caption" runat="server">(!contactus.aspx.1!)</asp:Literal></h3></div>
    <div><p><asp:Literal ID="litWeWantToHearFromYou" runat="server">(!contactus.aspx.2!)</asp:Literal></p></div>
    <div class="clear-both height-5"></div>
    
    <div id="divFormWrapper">
        <div id="divFormContainer">

        <!-- Form Left Section Starts Here -->
        <div id="divFormLeft" class="float-left">
            <span class="form-section"><asp:Literal ID="litYourContactDetails" runat="server">(!contactus.aspx.3!)</asp:Literal></span>
            <div class="clear-both height-12"></div>

            <%--Contact Name and ContactNumber Input Box Section Starts Here--%> 
            <div class="form-controls-place-holder">

                <span class="form-controls-span">
                    <label id="lblContactName" class="form-field-label">
                        <asp:Literal ID="litContactName" runat="server">(!contactus.aspx.4!)</asp:Literal>
                    </label>
                    <asp:Textbox ID="txtContactName" class="light-style-input" runat="server"></asp:Textbox>
                </span>

                 <span class="form-controls-span">
                    <label id="lblContactNumber" class="form-field-label">
                        <asp:Literal ID="litContactNumber" runat="server">(!contactus.aspx.7!)</asp:Literal>
                    </label>
                    <asp:Textbox ID="txtContactNumber" class="light-style-input" runat="server"></asp:Textbox>
                </span>
              
            </div>
            <%--Contact Name and Email Input Box Section Ends Here --%>

            <div class="clear-both height-5"></div>

           <%-- Email Input Box Section Starts Here--%>
             <span class="form-controls-span">
                    <label  id="lblEmail" class="form-field-label" maxlength="50">
                        <asp:Literal ID="litEmail" runat="server">(!contactus.aspx.5!)</asp:Literal>
                    </label>
                    <asp:Textbox ID="txtEmail" runat="server" class="light-style-input"></asp:Textbox>
              </span>
            <%--Area Code and Primary Phone Input Box Section Ends Here--%>

            <div class="clear-both height-12"></div>
            <span class="form-section"><asp:Literal ID="litWhatsOnYourMind" runat="server">(!contactus.aspx.8!)</asp:Literal></span>
            <div class="clear-both height-12"></div>

            <%-- Subject and Message Input Box Starts Here --%>
            <div class="form-controls-place-holder">
                <span class="form-controls-span">
                    <label id="lblSubject" class="form-field-label">
                        <asp:Literal ID="litSubject" runat="server">(!contactus.aspx.9!)</asp:Literal>
                    </label>
                    <asp:TextBox runat="server" ID="txtSubject"  class="light-style-input" MaxLength="100" Columns="58" runat="server"></asp:TextBox>
                </span>
            </div>
            <%-- Subject and Message Input Box Ends Here --%>

            <div class="clear-both height-5"></div>

            <div class="form-controls-place-holder">
                <span class="form-controls-span">
                    <label id="lblMessageDetails"  class="form-field-label">
                            <asp:Literal ID="litMessageDetails" runat="server">(!contactus.aspx.10!)</asp:Literal>
                    </label>
                    <asp:TextBox  ID="txtMessageDetails" rows="12" TextMode="MultiLine" runat="server" class="light-style-input" Columns="55"></asp:TextBox>
                </span>
            </div>

            <%-- Security Code / Captcha Section Starts Here --%>
            <asp:Panel runat="server" ID="pnlSecurityCode">
                <div class="clear-both height-5"></div>
                <div class="form-controls-place-holder">
                    <span class="form-controls-span custom-font-style capitalize-text" id="support-captcha-label">
                        <asp:Literal ID="LitEnterSecurityCodeBelow" runat="server">(!customersupport.aspx.12!)</asp:Literal>
                    </span>
                    <span class="form-controls-span">
                        <label id="lblCaptcha" class="form-field-label">
                            <asp:Literal ID="litCaptcha" runat="server">(!customersupport.aspx.13!)</asp:Literal>
                        </label>
                        <asp:TextBox ID="txtCaptcha" runat="server" class="light-style-input"></asp:TextBox>
                    </span>
                </div>
                <div class="clear-both height-5"></div>
                <div class="form-controls-place-holder">
                    <div id="support-captcha-wrapper">
                        <div id="captcha-image"><img alt="captcha" src="Captcha.ashx?id=1" id="captcha"/></div>
                        <div id="captcha-refresh"><a href="javascript:void(1);" id="captcha-refresh-button" alt="Refresh Captcha" title="Click to change the security code"></a></div>
                    </div>
                </div>
            </asp:Panel>
            <%-- Security Code / Captcha Section Ends Here --%>

           <div class="clear-both height-5"></div>
        </div>
        <%--Form Left Section Ends Here--%>

       <%-- Form Right Section Starts Here --%>
       <div id="divFormRight"  class="float-right"><ise:Topic ID="ContactUsFormHelpFulTipsTopic" runat="server" TopicName="ContactUsFormHelpFulTips" /></div>
       <%--Form Right Section Ends Here--%> 

       <div class="clear-both height-5"></div>
       <div class="clear-both height-12"></div>

       <%-- Form Lower Section : Button Container Starts Here --%>

        <div id="contact-form-button-place-holder" class="button-place-holder">
            <div id="send-message-button">
                <div id="send-message-loader"></div>
                <div id="save-case-button-place-holder">
                        <asp:Button ID="btnSendMessage" runat="server" OnClientClick="if (!formInfoIsGood()) {return false;};" Text="" UseSubmitBehavior="False" CssClass="site-button content"/> 
                </div>
            </div>
        </div>
       <%-- Form Lower Section : Button Container Ends Here --%>
    </div>
</div>
</asp:Panel>
</form>
</body>
</html>
