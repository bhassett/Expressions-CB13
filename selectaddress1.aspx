<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.selectaddress1" CodeFile="selectaddress1.aspx.cs" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls" TagPrefix="ise" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    
    <script language="javascript" type="text/javascript">

        $("document").ready(function () {

            var chkAddrInfo = document.getElementById('hidCheck');

            var strBTitl = document.getElementById('hidBillTitle');
            var strSTitl = document.getElementById('hidShipTitle');

            var strBChck = document.getElementById('hidBillCheck');
            var strSChck = document.getElementById('hidShipCheck');

            var strBAddr = document.getElementById('hidBilling');
            var strSAddr = document.getElementById('hidShipping');

            // NOTE: '*' character is used for seperation of information
            //       which will be parsed at the receiving end.

            var strBillCtrl = document.getElementById('hidBillCtrl');
            var arrBillCtrl = strBillCtrl.value.split("*");


            // Get WithState or WithOutState Billing Address Control id's
            var varBlnWithState = document.getElementById('hidBlnWithState');
            if (varBlnWithState.value == "True") {
                var citBillCtrl = document.getElementById(arrBillCtrl[0]);
                var posBillCtrl = document.getElementById(arrBillCtrl[1]);
            }
            else {
                var citBillCtrl = document.getElementById(arrBillCtrl[3]);
                var posBillCtrl = document.getElementById(arrBillCtrl[4]);
            }

            var varBillStateCtrl = document.getElementById(arrBillCtrl[2]);

            var strShipCtrl = document.getElementById('hidShipCtrl');
            var arrShipCtrl = strShipCtrl.value.split("*");

            // Get WithState or WithOutState Shipping Address Control id's
            var varShpWithState = document.getElementById('hidShpWithState');
            if (varShpWithState.value == "True") {
                var citShipCtrl = document.getElementById(arrShipCtrl[0]);
                var posShipCtrl = document.getElementById(arrShipCtrl[1]);
            }
            else {
                var citShipCtrl = document.getElementById(arrShipCtrl[3]);
                var posShipCtrl = document.getElementById(arrShipCtrl[4]);
            }

            var varShipStateCtrl = document.getElementById(arrShipCtrl[2]);

            var strBillAddr = '';
            var strShipAddr = '';

            var returnValue = '';

            var popModalFlag = document.getElementById('hidValid');

            if (popModalFlag != null && popModalFlag.value == "false") {

                var skinID = document.getElementById('hidSkinID');
                var localeSetting = document.getElementById('hidLocale');

                var ids = getControlIdsToCheckAfterSubmit();

                for (var i = 0; i < ids.length; i++) {
                    recheckPostalAfterSubmit(ids[i], "", "");
                }

            }

            var addressStates = $("#ctrlAddress_WithStateState").find("option").length;
            if (addressStates == 0) $("#ctrlAddress_WithStateState").css("display", "none");

        });    
        
    </script>    
    
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
            <div class="error">
                <cc1:InputValidatorSummary ID="InputValidatorySummary1" runat="server" ForeColor="Red"  Register="false"></cc1:InputValidatorSummary>
            </div>
            <asp:Table ID="tblAddressList" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                        <asp:Image runat="server" ID="addressbook_gif" /><br />
                        <asp:Table ID="tblAddressListBox" CellSpacing="0" CellPadding="2" Width="100%" runat="server">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="left" VerticalAlign="top">
                                <asp:Panel ID="pnlNewAddress" runat="server" Visible="false"  HorizontalAlign="Center">
                                <br />
                                <br />
                                        <ise:BillingAddressControl ID="ctrlAddress" runat="server" 
                                                Width="100%"
                                                CaptionWidth="30%" 
                                                InputWidth="70%"  
                                                WithStateCountyCaption="County" 
                                                RegisterCountries="True"
                                                Visible="False"
                                            />
                                        <br />
                                        <br /> 
                                        <asp:Button ID="btnNewAddress" runat="server" CssClass="site-button"  OnClick="btnNewAddress_Click" />
                                        <br />
                                        <br /> 
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
                                            <asp:Panel ID="liAdd" runat="server" Visible="true"><asp:HyperLink ID="lnkAddAddress" runat="server"></asp:HyperLink></asp:Panel>
                                        </li>
                                        </ol>
                                    </asp:Panel>
                              </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        <asp:Panel ID="pnlNoAddresses" runat="server" Visible="false"><asp:Literal ID="litNoAddresses" runat="server" Mode="PassThrough"></asp:Literal></asp:Panel>
        <p align="center">
        <asp:Button ID="btnReturn" runat="server" CssClass="site-button" />
        <asp:Button ID="btnCheckOut" runat="server" Visible="false" CssClass="site-button" /></p>
        <input type="hidden" id="hidCheck" value="true" runat="server" />
        <input type="hidden" id="hidValid" value="true" runat="server" />
        
        <input type="hidden" id="hidShipTitle" runat="server" />
        <input type="hidden" id="hidBillTitle" runat="server" />
        
        <input type="hidden" id="hidShipCheck" runat="server" />
        <input type="hidden" id="hidBillCheck" runat="server" />
        
        <input type="hidden" id="hidShipping" runat="server" />
        <input type="hidden" id="hidBilling" runat="server" />
        
        <input type="hidden" id="hidShipCtrl" runat="server" />
        <input type="hidden" id="hidBillCtrl" runat="server" />       
        
        <input type="hidden" id="hidShpState" runat="server" />
        <input type="hidden" id="hidShpCountry" runat="server" />
        <input type="hidden" id="hidShpPostalCode" runat="server" />
        <input type="hidden" id="hidShpCity" runat="server" />
        <input type="hidden" id="hidShpWithState" runat="server" />           

        <input type="hidden" id="hidBlnState" runat="server" />
        <input type="hidden" id="hidBlnCountry" runat="server" />
        <input type="hidden" id="hidBlnPostalCode" runat="server" />
        <input type="hidden" id="hidBlnCity" runat="server" />
        <input type="hidden" id="hidBlnWithState" runat="server" />   

        <input type="hidden" id="hidSkinID" runat="server" />
        <input type="hidden" id="hidLocale" runat="server" />      
    </form>
</body>
</html>