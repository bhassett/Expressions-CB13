<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.account" CodeFile="account.aspx.cs" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProfileControl" Src="~/UserControls/ProfileControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<html>
<head runat="server">
    <title></title>
    <script type="text/javascript" src="jscripts/jquery/jquery.cbe.customer.account.js"></script>
    <script type="text/javascript">

        var orderHistoryPluginStringKeys = new Object();
        orderHistoryPluginStringKeys.displayText = "account.aspx.43";
        orderHistoryPluginStringKeys.ofText = "account.aspx.29";
        orderHistoryPluginStringKeys.onText = "account.aspx.19";
        orderHistoryPluginStringKeys.orderDate = "account.aspx.12";
        orderHistoryPluginStringKeys.orderNotes = "account.aspx.16";
        orderHistoryPluginStringKeys.orderNumber = "account.aspx.11";
        orderHistoryPluginStringKeys.orderTotal = "account.aspx.15";
        orderHistoryPluginStringKeys.paymentMethod = "account.aspx.17";
        orderHistoryPluginStringKeys.paymentStatus = "account.aspx.13";
        orderHistoryPluginStringKeys.reorder = "account.aspx.22";
        orderHistoryPluginStringKeys.reorderPrompt = "account.aspx.26";
        orderHistoryPluginStringKeys.resetText = "account.aspx.44";
        orderHistoryPluginStringKeys.shippedText = "account.aspx.18";
        orderHistoryPluginStringKeys.shippingStatus = "account.aspx.14";
        orderHistoryPluginStringKeys.trackingNumber = "account.aspx.32";
        orderHistoryPluginStringKeys.viewing = "account.aspx.28";

    </script>
    <script type="text/javascript" src="components/order-history/setup.js"></script>
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

    <div id="profile-error-place-holder" class="error float-left display-none"></div> 
    <div class="clear-both height-5"></div>

    <asp:Panel ID="pnlNotCheckOutButtons" runat="server" HorizontalAlign="left">
        <asp:Image ID="redarrow1" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="accountaspx4" NavigateUrl="#OrderHistory" Text="(!account.aspx.3!)"></asp:HyperLink></b>
        <div class="clear-both height-12"></div>
        <asp:Panel ID="pnlShowWishButton" runat="server"><asp:Image ID="redarrow2" AlternateText="" runat="server" />&#0160;<b><asp:HyperLink runat="server" ID="ShowWishListButton" NavigateUrl="~/wishlist.aspx" Text="(!account.aspx.23!)"></asp:HyperLink></b></asp:Panel>
    </asp:Panel>
     <div class="clear-both height-5"></div>

    <ise:Topic runat="server" ID="HeaderMsg" TopicName="AccountPageHeader" />
    
    <div class="error"><cc1:InputValidatorSummary ID="errorSummary" runat="server" Register="false"></cc1:InputValidatorSummary></div>  

    <form id="AccountForm" runat="server">
     <asp:Panel ID="pnlPageContentWrapper" runat="server">
          <div class="clear-both height-12"></div>
           
           <!-- profile section starts here !-->

           <div id="support-grid-wrapper">
                <div class="one-page-sections-head"><asp:Literal ID="Literal1" runat="server">(!createaccount.aspx.124!)</asp:Literal></div>
                <div id="shipping-details-wrapper">

                    <div class="clear-both height-5"></div>

                    <div id="profile-info-wrapper" style="padding:12px;">
                                            
                        <div id="profile-account-info-place-holder" class="float-left">

                            <span class="form-section">
                                <asp:Literal ID="litProfileInfo" runat="server">(!createaccount.aspx.108!)</asp:Literal>
                            </span>
        
                            <div class="clear-both height-12 profile-section-clears"></div>
                            <uc:ProfileControl id="ProfileControl" showEditPasswordArea=true hideAccountNameArea=true runat="server" />
                            <div class="clear-both height-12 profile-section-clears"></div>

                            <div class="clear-both height-5"></div>
                                                
                            <div id="account-section-wrapper">
                                <span class="form-section custom-font-style">
                                    <asp:Literal ID="litAdditionalInfo" runat="server">(!createaccount.aspx.112!)</asp:Literal>
                                </span>
                                <div class="clear-both height-12"></div>

                                <div class="form-controls-place-holder">
                                    <span class="form-controls-span">
                                        <input type="checkbox" id="product-updates"/> 
                                        <span class="checkbox-captions custom-font-style">
                                            <asp:Literal ID="litOver13" runat="server">(!createaccount.aspx.113!)</asp:Literal>
                                       </span>
                                    </span>
                                </div>

                                <div class="clear-both height-5"></div>

                                <div class="form-controls-place-holder">
                                    <span class="form-controls-span label-outside" id="age-13-place-holder">
                                        <input type="checkbox" id="im-over-13-age"/> <span class="checkbox-captions custom-font-style">
                                            <asp:Literal ID="Literal3" runat="server">(!checkout1.aspx.39!)</asp:Literal>
                                        </span>
                                    </span>
                                </div>
                            </div>

                            <div class="clear-both height-12"></div>
             
              <!-- Captcha Section Starts Here -->
                <div class="form-controls-place-holder captcha-section">
                    <span class="form-controls-span custom-font-style" id="captcha-label" style="padding-right:17px !important;">
                        <asp:Literal ID="LtrEnterSecurityCodeBelow_Caption" runat="server">(!customersupport.aspx.12!)</asp:Literal>:
                    </span>

                    <span class="form-controls-span">
                       <label id="lblCaptcha" class="form-field-label">
                            <asp:Literal ID="litCaptcha" runat="server">(!customersupport.aspx.13!)</asp:Literal>
                        </label>
                        <input id="txtCaptcha" class="light-style-input" type="text" /> 
                    </span>
                </div>

                <div class="clear-both height-5  captcha-section"></div>

                <div class="form-controls-place-holder  captcha-section">

                   <div id="account-captcha-wrapper" class="float-right">
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
                        <div id="divProfileHelpfulTips" class="float-left">
                            <ise:Topic ID="ProfileHelpfulTips" runat="server" TopicName="ProfileHelpfulTips" />
                        </div>
                    </div>
                </div>
                    
            <div class="clear-both height-5"></div>
            <div class="clear-both height-5"></div>
            <div class="clear-both height-12"></div>

            <div id="profile-info-button-place-holder" style="padding-right:12px;">
               <div id="save-profile-button">
                     <div id="save-profile-loader"></div>
                     <div id="save-profile-button-place-holder">
                         <input type="button" class="site-button content" id="update-profile" 
                         data-contentType="string resource"
                         data-contentKey="account.aspx.6"
                         data-contentValue="<%=AppLogic.GetString("account.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"
                         value="<%=AppLogic.GetString("account.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" />
                         <asp:Button ID="btnContinueToCheckOut" CssClass="site-button" Text="(!account.aspx.24!)" runat="server" CausesValidation="false" OnClick="btnContinueToCheckOut_Click" />
                     </div>
               </div>
            </div>
            <div class="clear-both height-5"></div>
            <div class="clear-both height-12"></div>
          </div>
       </asp:Panel>
        
       <!-- profile section ends here !-->

       <div class="clear-both height-12"></div>

       <!-- address book section starts here !-->

       <div class="support-page-wrapper">
           <div class="clear-both height-12"></div>
           
           <div style="border: 1px solid #6ab1f2">
                <div class="one-page-sections-head">
                     <asp:Literal ID="Literal2" runat="server">(!selectaddress.aspx.1!)</asp:Literal>
                </div>
                <div id="Div2">
                    <div id="Div3" style="padding:12px;">
                                            
                        <div style="float:left;width:50%">
                        <b><asp:Label ID="accountaspx30" runat="server" Text="(!account.aspx.8!)"></asp:Label>&#160;&#160;&#160;&#160;</b>
                                    <asp:HyperLink ID="lnkChangeBilling" runat="server" CssClass="hyperlinkImageStyle"></asp:HyperLink><br/>
                                    <asp:Literal ID="litBillingAddress" runat="server"></asp:Literal>
                        </div>
                        <div style="float:left;width:50%;">
                        <asp:Panel ID="pnlShipping" runat="server">
                                        <b><asp:Label ID="accountaspx32" runat="server" Text="(!account.aspx.10!)"></asp:Label>&#160;&#160;&#160;&#160;</b>
                                        <asp:HyperLink ID="lnkChangeShipping" runat="Server" CssClass="hyperlinkImageStyle"></asp:HyperLink><br/>
                                        <asp:Literal ID="litShippingAddress" runat="server"></asp:Literal>
                                    </asp:Panel>
                        </div>
                    </div>
                </div>  
                <b><asp:HyperLink ID="lnkAddShippingAddress" runat="server"></asp:HyperLink></b>
                <div class="clear-both height-5"></div>
                <div class="clear-both height-12"></div>
          </div>
       </div>

       <!-- address book section ends here !-->

       <asp:ValidationSummary DisplayMode="List" ID="ValSummary" ShowMessageBox="false" runat="server" ShowSummary="true" ValidationGroup="account" ForeColor="red" Font-Bold="true"/>
        
       <div class="support-page-wrapper">
               <div class="clear-both height-12"></div>
           
               <div style="border: 1px solid #6ab1f2">
                    <div class="one-page-sections-head"><asp:Literal ID="litOrderHistory" runat="server">(!menu.OrderHistory!)</asp:Literal></div>
                    <div id="Div1">
                        <a name="OrderHistory"></a>
                        <div id="AccountOrderHistory" style="border: none !important">
                            <div id="accountOrderHistoryLink" runat="server" >
                                <a id="lnkOrderHistory" href="javascript:void(0)"><%=AppLogic.GetString("account.aspx.30", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%></a>
                                </div>
                                <div id="pnlOrderHistory"></div>
                        </div>
                    </div>
                </div>
       </div>

     <!-- do not remove --><input type="hidden" id="load-at-page" value="edit-profile" /><!-- do not remove -->

    </form>
    
</body>
</html>
