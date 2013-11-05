<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="createaccount.aspx.cs" Inherits="InterpriseSuiteEcommerce.createaccount" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="ise" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ShippingAddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="BillingAddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProfileControl" Src="~/UserControls/ProfileControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="jscripts/jquery/jquery.cbe.customer.account.js"></script>
</head>
<body>
 <ise:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="False" />
 <form id="createAccount" runat="server">
  <div class="clear-both"></div>
    <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
        <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
            <asp:RectangleHotSpot Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate"
                NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
        </asp:ImageMap>
       <div class="clear-both height-5"></div>
   </asp:Panel>
   <asp:Panel ID="pnlPageContentWrapper" runat="server">
        
        <asp:Panel ID="pnlPageHeaderPlaceHolder" runat="server">
            <div id="divPageTitle">
                <h3><asp:Literal ID="litCreateAccountPageTitle" runat="server">(!createaccount.aspx.105!)</asp:Literal></h3>
            </div>
            <div>
               <p><asp:Literal ID="litCreateAccountPageTips" runat="server">(!createaccount.aspx.106!)</asp:Literal></p>
                 <div class="clear-both height-12"></div>
               <p><asp:Literal ID="litIfYouHaveAlreadyAnAccount" runat="server">(!createaccount.aspx.107!)</asp:Literal>&nbsp;
               <a href="signin.aspx"><asp:Literal ID="litSignIn" runat="server">(!signin.aspx.16!)</asp:Literal></a></p>
            </div>
            <div class="clear-both height-12"></div>
        </asp:Panel>
        <div id="divFormWrapper">
        <div id="divFormContainer">
           <%-- create account form left content starts here --%>
            <div id="divFormLeft" class="float-left">
                <span class="form-section">
                    <asp:Literal ID="LitYourProfileInto" runat="server">(!createaccount.aspx.108!)</asp:Literal>
                </span>
               <!-- Profile Info Section Starts Here -->
               <div class="clear-both height-12 profile-section-clears"></div>
                    <uc:ProfileControl id="ProfileControl"  runat="server" />
               <div class="clear-both height-12 profile-section-clears"></div>
               <!-- Profile Info Section Ends Here -->

               <!-- Billing Info Section Starts Here -->

               <div class="clear-both height-12"></div>

               <span class="form-section">
                    <asp:Literal ID="litBillingInfo" runat="server">(!createaccount.aspx.109!)</asp:Literal>
               </span>
               <div class="clear-both height-12"></div>
              
                <uc:BillingAddressControl id="BillingAddressControl" IdPrefix="billing-" runat="server" />
               
               <div class="clear-both height-12"></div>

               <!-- Billing Info Section Ends Here -->

                <div class="clear-both height-12 shipping-section-clears"></div>

                <!-- Shipping Info Section Starts Here -->

               <span class="form-section" id="shipping-section-head-place-holder">
                    <span class="float-left" id="lit-shipping-info"  class="custom-font-style"><asp:Literal ID="litShippingInfo" runat="server">(!createaccount.aspx.110!)</asp:Literal></span>
                    <span class="float-right" id="copy-billing-info-place-holder">
                        <asp:CheckBox ID="copyBillingInfo" runat="server"/>
                        <span class="checkbox-captions custom-font-style"><asp:Literal ID="litSameAsBillingInfo" runat="server">(!createaccount.aspx.111!)</asp:Literal></span>
                    </span>
               </span>
               <div class="clear-both height-12 shipping-section-clears"></div>
               <div id="shipping-info-place-holder">
                    <uc:ShippingAddressControl id="ShippingAddressControl" IdPrefix="shipping-" showResidenceTypes=true showBusinessTypes=false hideStreetAddress=false runat="server" />
               </div>
                
               <div class="clear-both height-12 shipping-section-clears"></div>
               <!-- Shipping Info Section Ends Here -->

               <!-- Account Info Section Starts Here -->

               <div class="clear-both height-12 account-info-sections-clears"></div>
               <div id="account-section-wrapper">
                 <span class="form-section">
                        <asp:Literal ID="litAdditionaInfo" runat="server">(!createaccount.aspx.112!)</asp:Literal>
                  </span>
                  <div class="clear-both height-12"></div>

                  <asp:Panel ID="pnlProductUpdates" runat="server">
                      <div class="form-controls-place-holder">
                          <span class="form-controls-span label-outside">
                            <asp:CheckBox ID="chkProductUpdates" runat="server" Checked="true"/> <span class="checkbox-captions custom-font-style">
                               <asp:Literal ID="litProductUpdates" runat="server">(!createaccount.aspx.113!)</asp:Literal>
                            </span>
                         </span>
                      </div>
                  </asp:Panel>

                  <div class="clear-both height-5"></div>

                  <div class="form-controls-place-holder">
                     <span class="form-controls-span label-outside" id="age-13-place-holder">
                        <asp:CheckBox ID="chkOver13" runat="server"/> <span class="checkbox-captions custom-font-style"><asp:Literal ID="litImOver13" runat="server">(!checkout1.aspx.39!)</asp:Literal></span>
                     </span>
                  </div>
               </div>

                <div class="clear-both height-12 account-info-sections-clears"></div>

               <!-- Account Info Section Ends Here -->

                <div class="clear-both height-5"></div>

                <!-- Captcha Section Starts Here -->

                <div class="form-controls-place-holder captcha-section">

                    <span class="form-controls-span custom-font-style capitalize-text" id="create-account-captcha-label">
                        <asp:Literal ID="LitEnterSecurityCodeBelow" runat="server">(!customersupport.aspx.12!)</asp:Literal>
                    </span>

                    <span class="form-controls-span">
                       <label id="lblCaptcha" class="form-field-label">
                            <asp:Literal ID="litCaptcha" runat="server">(!customersupport.aspx.13!)</asp:Literal>
                        </label>
                        <asp:TextBox ID="txtCaptcha" runat="server" class="light-style-input"></asp:TextBox>
                    </span>

                </div>

                <div class="clear-both height-5  captcha-section"></div>

                <div class="form-controls-place-holder  captcha-section">
                   <div id="create-account-captcha-wrapper" class="float-right">
                        <div id="captcha-image">
                            <img alt="captcha" src="Captcha.ashx?id=1" id="captcha"/>
                         </div>
                         <div id="captcha-refresh">
                            <a href="javascript:void(1);" id="captcha-refresh-button" alt="Refresh Captcha" title="Click to change the security code"></a>
                         </div>
                    </div>
                </div>

                <div class="clear-both height-5  captcha-section"></div>
              
                <!-- Captcha Section Ends Here -->

            </div>
            <%-- case form left content ends here --%>
            <%-- case form  right content starts here --%>

            <div id="divFormRight" class="float-right">
                 <ise:Topic ID="CreateAccountHelpfulTips" runat="server" TopicName="CreateAccountHelpfulTips" />
            </div>
           
            <%-- case form  right content ends here --%>
         </div>
   
         <div class="clear-both height-5"></div>
         <div class="clear-both height-12"></div>
 
         <div id="account-form-button-place-holder" class="button-place-holder">
             <div id="save-account-button">
                <div id="save-account-loader"></div>
                    <div id="save-account-button-place-holder">
                        <input type="button"  id="create-customer-account" 
                        class="site-button content" 
                        data-contentKey="<%= CommonLogic.IIF(CommonLogic.QueryStringBool("checkout"),  "createaccount.aspx.25",  "createaccount.aspx.24")%>" 
                        data-contentType="string resource"
                        data-contentValue="<%= CommonLogic.IIF(CommonLogic.QueryStringBool("checkout"),  AppLogic.GetString("createaccount.aspx.25", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true),  
                                            AppLogic.GetString("createaccount.aspx.24", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true))%>"
                        value="<%= CommonLogic.IIF(CommonLogic.QueryStringBool("checkout"),  AppLogic.GetString("createaccount.aspx.25", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true),  
                                            AppLogic.GetString("createaccount.aspx.24", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true))%>"/> 
                    </div>
               </div>
         </div>
          <div class="display-none">
                <asp:Button ID="btnCreateAccount" runat="server" Text="" OnClick="btnCreateAccount_Click" />
                <asp:TextBox ID="billingTxtCityStates" runat="server"></asp:TextBox>     
                <asp:TextBox ID="shippingTxtCityStates" runat="server"></asp:TextBox>             
            </div>
         </div>
     </asp:Panel>

    <%-- 
        do not touch the following html script, the following elements are used in overriding postal listing dialog assignment of values to city and zip
        see jscripts/jquery.cbe.address.verification.js updateAddressInputValues function
        
    --%>

    <div id="submit-case-caption" style="display:none;"><asp:Literal ID="LtrCreateAccount_Caption" runat="server"></asp:Literal></div>
        <div style="display:none;margin:auto" title="Address Verification" id="ise-address-verification-for-create-account">
        <div class='address-verification-dialog-wrapper'>
            <div class="clear-both height-5"></div>
            <asp:Literal ID="litAddressVerificationDialogText" runat="server">(!createaccount.aspx.114!)</asp:Literal>
            <div class="clear-both height-5"></div>

            <asp:Literal ID="litAddressVerificationDialogSubText" runat="server">(!createaccount.aspx.115!)</asp:Literal>
            <div class="clear-both height-12"></div>
         
            <div id="divEnteredAddress">
                <ul class="ul-list-no-style">
                    <li style="font-weight:bold">
                        <input type="radio" id="use-my-billing" name="billing-address"/>
                        <asp:Literal ID="litUseBillingIProvided" runat="server">(!createaccount.aspx.116!)</asp:Literal>
                    </li>
                    <li><div class="clear-both height-5"></div></li>
                    <li style="padding-left:21px;" id="c-billing-address"></li>
                    <li style="padding-left:21px;" id="c-billing-city-state-postal"></li>
                </ul>
                 <ul class="ul-list-no-style">
                    <li style="font-weight:bold">
                        <input type="radio" id="use-my-shipping" name="shipping-address"/>
                        <asp:Literal ID="litUseShippingIProvided" runat="server">(!createaccount.aspx.117!)</asp:Literal>
                    </li>
                    <li><div class="clear-both height-5"></div></li>
                    <li style="padding-left:21px;" id="c-shipping-address"></li>
                    <li style="padding-left:21px;" id="c-shipping-city-state-postal"></li>
                 </ul>
                <div class="clear-both height-12"></div>
            </div>
            
            <div id="divSuggestedAddress">
                <ul  class="ul-list-no-style">
                    <li style="font-weight:bold">
                        <input type="radio" id="use-suggested-billing" checked name="billing-address"/>
                        <asp:Literal ID="litUseSuggestedBilling" runat="server">(!createaccount.aspx.118!)</asp:Literal>
                    </li>
                    <li><div class="clear-both height-5"></div></li>
                    <li style="padding-left:21px;" id="n-billing-address"></li>
                    <li style="padding-left:21px;" id="n-billing-city-state-postal"></li>
                </ul>

                 <ul  class="ul-list-no-style">
                    <li style="font-weight:bold">
                        <input type="radio" id="use-suggested-shipping" checked name="shipping-address"/>
                        <asp:Literal ID="litUseSuggestedShipping" runat="server">(!createaccount.aspx.119!)</asp:Literal>
                    </li>
                    <li><div class="clear-both height-5"></div></li>
                    <li style="padding-left:21px;" id="n-shipping-address"></li>
                    <li style="padding-left:21px;" id="n-shipping-city-state-postal"></li>
                </ul>
                <div class="clear-both height-12"></div>
            </div>
            <div class="clear-both height-5"></div>
        </div>
        <div id="submit-suggested-button-place-holder">
               <div id="saving-button-place-holder" class='float-right' style="padding-right:4px">
                    <input type="button" class="btn btn-primary" id="submit-suggested-address" value="Continue"/>
               </div>
                <div class="clear-both"></div>
          </div>
    </div>
    <input type="hidden" id="load-at-page" value="create-account" />
    <%-- do not touch <-- --%>

    </form>
</body>
</html>