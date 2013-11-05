<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.editaddress" CodeFile="editaddress.aspx.cs"%>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="uc"  TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc"  TagName="ProfileControl" Src="~/UserControls/ProfileControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title> 
    <script type="text/javascript" src="jscripts/jquery/jquery.cbe.customer.account.js"></script>
</head>
<body>
    <form id="frmEditAddress" runat="server">
    <asp:Literal ID="litswitchformat" runat="server" Mode="PassThrough"></asp:Literal> 
        <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
            <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
                <asp:RectangleHotSpot AlternateText="Back To Step 1: Shopping Cart" Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
            </asp:ImageMap>
            <br /><br />
        </asp:Panel>
        <cc1:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="false" style="width:660px;"></cc1:InputValidatorSummary>
        <div class="clear-both"></div>                            
        <asp:Panel ID="pnlAddress" runat="server" Visible="true">
        <table cellpadding="0" id="tblAccountInfo" runat="server" cellspacing="0" width="100%">
            <tr>
                <td>
                    <div class="page-sections-head"><asp:Literal ID="litEditAddress" runat="server">(!selectaddress.aspx.5!)</asp:Literal></div>

                    <table cellpadding="2" id="tblAccountInfoBox" runat="server" cellspacing="0" width="100%">
                        <tr>
                            <td>
                                                  
                                   <div class="clear-both height-12"></div>
                                   <div class="float-left" style="width: 60%; padding-left: 12px;">
                                        <span class="support-labels custom-font-style"><asp:Literal ID="litAddressTitle" runat="server">(!selectaddress.aspx.4!)</asp:Literal></span>
                                        <div class="clear-both height-5"></div>
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
                                                <asp:TextBox ID="txtContactNumber" runat="server" MaxLength="100" class="light-style-input"></asp:TextBox>
                                            </span>
                                        </div>
                                        <div class="clear-both height-5"></div>
            
                                        <div class="clear-both height-12 profile-section-clears"></div>
                                            <uc:AddressControl id="AddressControl"  runat="server" />
                                        </div>
                                        <div class="float-left" style="width:35%">
                                            <ise:Topic ID="AddressBookHelpfulTips" runat="server" TopicName="AddressBookHelpfulTips" />
                                        </div>

                                        <div class="clear-both height-12"></div> 
                                        <div class="clear-both height-12"></div> 

                            </td>
                        </tr>
                    </table>

                </td>
            </tr>
            <tr>
                <td>
                    
                     <div style="padding-right:12px;" class="button-place-holder">
                     <div id="return-address-button">
                          <div id="return-address-button-place-holder">

                                <div class="display-none">
                                    <asp:Button ID="btnUpdateAddress" runat="server" CssClass="site-button" OnClick="btnUpdateAddress_Click" />
                                    <asp:TextBox ID="txtCityStates" runat="server"></asp:TextBox>   
                                </div>

                                <div class="float-right">
                                    <asp:Panel ID="pnlUpdasteAddress" CssClass="float-left" style="padding-right:5px;" runat="server">
                                         <div id="update-address-button">
                                            <div id="update-address-loader"></div>
                                            <div id="update-address-button-place-holder">
                                            <input type="button" id="save-address" class="site-button content" 
                                              data-contentKey="editaddress.aspx.3"
                                              data-contentValue="<%=AppLogic.GetString("editaddress.aspx.3", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"
                                              data-contentType="string resource"
                                              value="<%=AppLogic.GetString("editaddress.aspx.3", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"/> 
                                            </div>
                                         </div>
                                    </asp:Panel>
                                    <asp:Panel ID="Panel1" runat="server" CssClass="float-left">
                                        <asp:Button ID="btnReturn" runat="server" CssClass="site-button content" />
                                        <asp:Button ID="btnCheckOut" runat="server" Visible="false" CssClass="site-button" />
                                    </asp:Panel>
                                </div>
                            </div>

                        </div>
                     <div class="clear-both height-12"></div>
                    </div>

                </td>    
            </tr>
        </table>    
        </asp:Panel>

        <input type="hidden" id="hidSkinID" runat="server" />
        <input type="hidden" id="hidLocale" runat="server" />    
        <input type="hidden" id="txtPhone" runat="server" />
        <input type="hidden" id="load-at-page" value="edit-address" />
    </form>
</body>
</html>
