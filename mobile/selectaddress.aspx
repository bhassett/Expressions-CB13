<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.selectaddress" CodeFile="selectaddress.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators"
    TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.mobile"
    TagPrefix="ise" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script type="text/javascript">

        window.onload = function () {

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

        }    
        
    </script>
</head>
<body>
    <div class="signin_main">
        <div class="signin_info">
            <div class="tableheaderarea">
                <span>
                    <asp:Literal runat="server" ID="litHeaderText"></asp:Literal>
                </span>
            </div>
            <div class="signin_info_body">
                <form id="frmAddAddress" runat="server" style="margin-left:20px;">
                <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
                    <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
                        <asp:RectangleHotSpot AlternateText="Back To Step 1: Shopping Cart" Top="0" Left="0"
                            Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
                    </asp:ImageMap>
                    <br />
                </asp:Panel>
                <asp:Panel ID="pnlAddressList" runat="server" Visible="false">
                    <div class="error">
                        <cc1:InputValidatorSummary ID="InputValidatorySummary1" runat="server" ForeColor="Red" Register="false">
                        </cc1:InputValidatorSummary>
                    </div>
                    <asp:Panel ID="pnlNewAddress" runat="server" Visible="false">
                        <ise:BillingAddressControl ID="ctrlAddress" runat="server" Width="100%" CaptionWidth="30%"
                            InputWidth="70%" WithStateCountyCaption="County" TextBoxWithStatePostalCodeCssClass="inputTextBox ajax_textbox1"
                            RegisterCountries="True" Visible="False" EnableViewState="true" />
                        <script type="text/javascript">
                            $(document).ready(function () {
                                //Function Location: mobile/js/createaccount_ajax.js
                                RegisterAutoCompletePostalCode('.ajax_textbox1', 'ctrlAddress_Country');
                            });
                        </script>
                        <br />
                        <br />
                        <div class="button_layout">
                            <uc1:ISEMobileButton ID="btnNewAddress" runat="server" OnClick="btnNewAddress_Click" />
                        </div>
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlAddressListMain" runat="server" Visible="true">
                        <br />
                        <ol>
                            <asp:Repeater ID="AddressList" runat="server">
                                <ItemTemplate>
                                    <li class="borderBottom">
                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "PrimaryAddress").ToString() == "1", "<b>", "")%>
                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Name").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Name").ToString()) %>
                                        <br />
                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Address").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Address") + "<br />")%>
                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "CityStateZip").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "CityStateZip") + "<br />")%>
                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Country").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Country") + "<br />")%>
                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "Telephone").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "Telephone") + "<br />")%>
                                        <%# InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "County").ToString().Trim() == "", "", DataBinder.Eval(Container.DataItem, "County") + "<br />")%>
                                        <%#  InterpriseSuiteEcommerceCommon.CommonLogic.IIF(DataBinder.Eval(Container.DataItem, "PrimaryAddress").ToString() == "1", "</b>", "")%>
                                        <br />

                                        <div>
                                            <uc1:ISEMobileButton ID="btnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' CommandName="edit" />
                                            <uc1:ISEMobileButton ID="btnMakePrimary" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AddressID") %>' CommandName="makeprimary" />
                                        </div>
                                    </li>
                                    
                                </ItemTemplate>
                            </asp:Repeater>
                        </ol>
                    </asp:Panel>
                </asp:Panel>
                <asp:Panel ID="pnlNoAddresses" runat="server" Visible="false">
                    <asp:Literal ID="litNoAddresses" runat="server" Mode="PassThrough">
                    </asp:Literal>
                </asp:Panel>
                <br />

                <div style="text-align:left" >
                    <asp:HyperLink ID="lnkAddAddress" runat="server" CssClass="kitdetaillink"></asp:HyperLink>
                </div>
                <br />
                <div class="button_layout" >
                    <uc1:ISEMobileButton ID="btnReturn" runat="server" />
                    <uc1:ISEMobileButton ID="btnCheckOut" runat="server" Visible="false" />
                </div>

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
            </div>
        </div>
    </div>
</body>
</html>
