<%@ Page  Language="C#" AutoEventWireup="true" CodeFile="customersupport.aspx.cs" Inherits="InterpriseSuiteEcommerce.CustomerSupport" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <title></title>
     <script type="text/javascript" src="jscripts/jquery/jquery.format.1.05.js"></script>
     <script type="text/javascript" src="jscripts/case.customer.support.js"></script>
</head>
<body>
    <ise:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="False" />
    <form id="form1" runat="server">
    <div class="clear-both"></div>
    <asp:Panel ID="pnlPageContentWrapper" runat="server">
        <div id="divPageTitle"><h3><asp:Literal ID="LtrPageWelcomeHeader_Caption" runat="server">(!customersupport.aspx.19!)</asp:Literal></h3></div>
        <div>
             <p>
                <asp:Literal ID="Literal1" runat="server">(!customersupport.aspx.2!)</asp:Literal>&nbsp;<a href="contactus.aspx"><u><asp:Literal ID="ltrMenuContact" runat="server">(!menu.Contact!)</asp:Literal></u> <a href="casehistory.aspx" id="case-history-link" class="float-right"><u><asp:Literal ID="Literal31" runat="server">(!menu.CaseHistory!)</asp:Literal></u></a>
             </p>
        </div>
        <div id="divFormWrapper">
        <div id="divFormContainer">
     
           <%-- case form left content starts here --%>
            <div id="onload-process-place-holder">
               <asp:Literal ID="Literal11" runat="server">(!customersupport.aspx.3!)</asp:Literal>
            </div>
            <div class="clear-both height-5"></div>
            <div id="divFormLeft" class="float-left">
                <span class="form-section">
                    <asp:Literal ID="LtrYourProfileSectionHeader_Caption" runat="server">(!customersupport.aspx.1!)</asp:Literal>
                </span>

                <div class="clear-both height-12"></div>

                <div class="form-controls-place-holder">

                    <span class="form-controls-span">
                        <label id="lblContactName" class="form-field-label">
                            <asp:Literal ID="litContactName" runat="server">(!customersupport.aspx.4!)</asp:Literal>
                        </label>
                        <asp:Textbox ID="txtContactName" class="light-style-input" runat="server"></asp:Textbox>
                    </span>

                     <span class="form-controls-span">
                       <label  id="lblContactNumber" class="form-field-label">
                            <asp:Literal ID="litContactNumber" runat="server">(!customersupport.aspx.7!)</asp:Literal>
                       </label>
                        <asp:TextBox ID="txtContactNumber" runat="server" class="light-style-input"></asp:TextBox>
                    </span>

                </div>
                <div class="clear-both height-5"></div>

                <div class="form-controls-place-holder">
                      <span class="form-controls-span">
                       <label  id="lblEmail" class="form-field-label" maxlength="50">
                            <asp:Literal ID="litEmail" runat="server">(!customersupport.aspx.5!)</asp:Literal>
                       </label>
                        <asp:Textbox ID="txtEmail" runat="server" class="light-style-input"></asp:Textbox>
                    </span>
                </div>

               <div class="clear-both height-12"></div>

               <!-- Your Profile Section Ends Here -->

               <!-- Your Address Section Starts Here -->

               <span class="form-section">
                    <asp:Literal ID="LtrYourAddressHeader_Caption" runat="server">(!customersupport.aspx.43!)</asp:Literal>
               </span>
               <div class="clear-both height-12"></div>
              
               <uc:AddressControl id="AddressControl"  runat="server" />
               
               <div class="clear-both height-12"></div>

               <!-- Your Address Section Ends Here -->

               <!-- Your Case Section Starts Here -->

               <span class="form-section">
                     <asp:Literal ID="LtrYourCaseHeader_Caption" runat="server">(!customersupport.aspx.9!)</asp:Literal>
               </span>
               <div class="clear-both height-12"></div>

                <div class="form-controls-place-holder">

                    <span class="form-controls-span">
                        <label id="lblSubject" class="form-field-label">
                            <asp:Literal ID="litSubject" runat="server">(!customersupport.aspx.10!)</asp:Literal>
                        </label>
                        <asp:TextBox runat="server" ID="txtSubject"  class="light-style-input" MaxLength="100" Columns="58" runat="server"></asp:TextBox>
                    </span>

                </div>
                <div class="clear-both height-5"></div>
               <div class="form-controls-place-holder">

                    <span class="form-controls-span">
                       <label id="lblCaseDetails"  class="form-field-label">
                             <asp:Literal ID="litCaseDetails" runat="server">(!customersupport.aspx.11!)</asp:Literal>
                       </label>
                       <asp:TextBox  ID="txtCaseDetails" rows="12" TextMode="MultiLine" runat="server" class="light-style-input" Columns="55"></asp:TextBox>
                    </span>

                </div>
                
               <!-- Your Case Section Ends Here -->
                
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
                            <div id="captcha-image">
                                <img alt="captcha" src="Captcha.ashx?id=1" id="captcha"/>
                             </div>
                             <div id="captcha-refresh">
                                <a href="javascript:void(1);" id="captcha-refresh-button" alt="Refresh Captcha" title="Click to change the security code"></a>
                             </div>
                        </div>
                    </div>
                    <div class="clear-both height-5"></div>
                </asp:Panel>

            </div>

            <%-- case form left content ends here --%>

            <%-- case form  right content starts here --%>

            <div id="divFormRight" class="float-right">
                 <ise:Topic ID="CaseFormHelpFulTipsTopic" runat="server" TopicName="CaseFormHelpFulTips" />
            </div>
           
            <%-- case form  right content ends here --%>
        </div>
    
         <div class="clear-both height-5"></div>
         <div class="clear-both height-12"></div>

         <div id="case-form-button-place-holder" class="button-place-holder">
             <div id="save-case-button">
                <div id="save-case-loader"></div>
                    <div id="save-case-button-place-holder">
                        <input type="button"  id="submit-case" class="site-button content" 
                          data-contentKey="customersupport.aspx.14"
                          data-contentValue="<%=AppLogic.GetString("customersupport.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"
                          data-contentType="string resource"
                          value="<%=AppLogic.GetString("customersupport.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"/> 
                    </div>
               </div>
         </div>


         </div>
    </asp:Panel>

    <%-- 
        do not touch the following html script, the following elements are used in overriding postal listing dialog assignment of values to city and zip
        see jscripts/jquery.cbe.address.verification.js updateAddressInputValues function
    --%>
    <div style="display:none">
         <asp:Button ID="btnSendCaseForm" runat="server" Text="" OnClick="btnSendCaseForm_Click" />
         <asp:TextBox ID="txtCityStates" runat="server"></asp:TextBox>             
    </div>
     <%-- do not touch --%>

    </form>
</body>
</html>