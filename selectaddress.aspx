<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.selectaddress" CodeFile="selectaddress.aspx.cs" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<%@ Register TagPrefix="uc" TagName="AddressControl" Src="~/UserControls/AddressControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="ProfileControl" Src="~/UserControls/ProfileControl.ascx" %>
<%@ Register TagPrefix="ise" TagName="Topic" src="TopicControl.ascx" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript" src="jscripts/jquery/jquery.cbe.customer.account.js"></script>
    <style>#txtContactName { width: 363px; }</style>
</head>
<body>
    <form id="frmAddAddress" runat="server">
        <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
            <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
                <asp:RectangleHotSpot AlternateText="Back To Step 1: Shopping Cart" Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
            </asp:ImageMap>
            <br />
        </asp:Panel>
                                    
        <asp:Panel ID="pnlAddressList" runat="server" Visible="false">
            <cc1:InputValidatorSummary ID="errorSummary" CssClass="error float-left normal-font-style" runat="server" Register="false" style="width:660px;"></cc1:InputValidatorSummary>
            <div class="clear-both"></div>
            <asp:Table ID="tblAddressList" CellSpacing="0" CellPadding="0" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                    <div class="page-sections-head"><asp:Literal ID="litYourAddressBook" runat="server">(!selectaddress.aspx.1!)</asp:Literal></div>
                        <asp:Table ID="tblAddressListBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                <asp:Panel ID="pnlNewAddress" runat="server" Visible="false">
                                  
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
                                       <div class="clear-both height-12 profile-section-clears"></div>
                                       <uc:AddressControl id="AddressControl"  runat="server" />
                                   </div>
                                   <div class="float-left" style="width:35%">
                                         <ise:Topic ID="AddressBookHelpfulTips" runat="server" TopicName="AddressBookHelpfulTips" />
                                   </div>

                                   <div class="clear-both height-12"></div> 
                                   <div class="clear-both height-12"></div> 

                                 </asp:Panel>
                                    <asp:Panel ID="pnlAddressListMain" runat="server" Visible="true">
                                        <br />
                                        <ol>
                                        <asp:Repeater ID="AddressList" runat="server">
                                            <ItemTemplate>
                                                <li>
                                                    <%#  InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "PrimaryAddress").ToString() == "1", "<b>", "")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Name").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Name").ToString()) %>   
                                                            &nbsp;&nbsp;
                                                            <asp:ImageButton ID="btnMakePrimary" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' style="vertical-align: middle;" CommandName="makeprimary" runat="server" />
                                                            &nbsp;&nbsp;
                                                            <asp:ImageButton ID="btnEdit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' style="vertical-align: middle;" CommandName="edit" runat="server" />
                                                            <br />                                                 
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Address").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Address") + "<br />")%>                                                    
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "CityStateZip").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "CityStateZip") + "<br />")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Country").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Country") + "<br />")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Telephone").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Telephone") + "<br />")%>
                                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "County").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "County") + "<br />")%>
                                                        <%#  InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "PrimaryAddress").ToString() == "1", "</b>", "")%>
                                                    <br />
                                                 </li>
                                            </ItemTemplate>
                                        </asp:Repeater>                                  
                                        <li>
                                            <asp:Panel ID="liAdd" runat="server" Visible="true">
                                                <asp:HyperLink ID="lnkAddAddress" runat="server"></asp:HyperLink>
                                            </asp:Panel>
                                        </li>
                                        </ol>
                                    </asp:Panel>
                              </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                   
                      <div style="padding-right:12px;" class="button-place-holder">
                       <div id="return-address-button">
                            <div id="return-address-button-place-holder">

                                <div class="display-none">
                                    <asp:Button ID="btnNewAddress" runat="server" CssClass="site-button" OnClick="btnNewAddress_Click" />
                                    <asp:TextBox ID="txtCityStates" runat="server"></asp:TextBox>   
                                </div>
                              
                                <div class="float-right">
                                    <asp:Panel ID="pnlSaveAddress" CssClass="float-left" style="padding-right:5px;" runat="server" Visible="false">
                                         <div id="save-address-button">
                                            <div id="save-address-loader"></div>
                                            <div id="save-address-button-place-holder">

                                                <input type="button" class="site-button content" id="save-address" 
                                                data-contentKey="selectaddress.aspx.16"
                                                data-contentValue="<%=AppLogic.GetString("selectaddress.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"
                                                data-contentType="string resource"
                                                Value="<%=AppLogic.GetString("selectaddress.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" />

                                            </div>
                                         </div>
                                     </asp:Panel>
                                    <asp:Panel runat="server" CssClass="float-left">
                                        <asp:Button ID="btnReturn" runat="server" CssClass="site-button content" />
                                        <asp:Button ID="btnCheckOut" runat="server" Visible="false" CssClass="site-button content" />
                                
                                </asp:Panel>
                                </div>
                            </div>
                       </div>
                       <div class="clear-both height-12"></div>
                    </div>

                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlNoAddresses" runat="server" Visible="false"><asp:Literal ID="litNoAddresses" runat="server" Mode="PassThrough"></asp:Literal></asp:Panel>

        <input type="hidden" id="hidSkinID" runat="server" />
        <input type="hidden" id="hidLocale" runat="server" />      
        <input type="hidden" id="load-at-page" value="select-address" />
    </form>
</body>
</html>