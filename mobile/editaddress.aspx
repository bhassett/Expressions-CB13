<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.editaddress" CodeFile="editaddress.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators" TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.mobile" TagPrefix="ise" %>
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

            if (popModalFlag != null && popModalFlag.value == "false") {

                //                var skinID = document.getElementById('hidSkinID');
                //                var localeSetting = document.getElementById('hidLocale');

                //                // Check if return flag for Billing Address has a value
                //                if (strBChck.value != '') {

                //                    var postalCode = document.getElementById('hidBlnPostalCode');
                //                    var state = document.getElementById('hidBlnState');
                //                    var country = document.getElementById('hidBlnCountry');
                //                    var city = document.getElementById('hidBlnCity');
                //                    var withState = varBlnWithState;

                //                    var strQueryString = "?SkinId=" + skinID.value + "&Locale=" + localeSetting.value + "&Status=" +
                //                        strBChck.value + "&Title=" + strBTitl.value + "&PostalCode=" + encodeURIComponent(postalCode.value) + "&City=" +
                //                        encodeURIComponent(city.value) + "&State=" + state.value + "&Country=" + country.value + "&WithState=" + withState.value;

                //                    returnValue = window.showModalDialog("addresschecker.aspx" + strQueryString, "", "dialogWidth:585px; dialogHeight:400px; status:no; center:yes; edge:raised; scroll:off");

                //                    if (returnValue != '' && returnValue != null) {

                //                        var retBillValue = returnValue;
                //                        var arrBillValue = retBillValue.split("*");

                //                        posBillCtrl.value = arrBillValue[0];
                //                        citBillCtrl.value = arrBillValue[1];
                //                    }
                //                }

                //                // Check if return flag for Shipping Address has a value
                //                if (strSChck.value != '') {

                //                    var postalCode = document.getElementById('hidShpPostalCode');
                //                    var state = document.getElementById('hidShpState');
                //                    var country = document.getElementById('hidShpCountry');
                //                    var city = document.getElementById('hidShpCity');
                //                    var withState = varShpWithState;

                //                    var strQueryString = "?SkinId=" + skinID.value + "&Locale=" + localeSetting.value + "&Status=" +
                //                    strSChck.value + "&Title=" + strSTitl.value + "&PostalCode=" + encodeURIComponent(postalCode.value) + "&City=" +
                //                    encodeURIComponent(city.value) + "&State=" + state.value + "&Country=" + country.value + "&WithState=" + withState.value;

                //                    returnValue = window.showModalDialog("addresschecker.aspx" + strQueryString, "", "dialogWidth:585px; dialogHeight:400px; status:no; center:yes; edge:raised; scroll:off");

                //                    if (returnValue != '' && returnValue != null) {

                //                        var retShipValue = returnValue;
                //                        var arrShipValue = retShipValue.split("*");

                //                        citShipCtrl.value = arrShipValue[1];
                //                        posShipCtrl.value = arrShipValue[0];

                //                    }

                //                }

            }

        }    
        
    </script>
</head>
<body>
    <div class="signin_main">
        <div class="signin_info">
            <div class="tableheaderarea">
                <span>
                    <asp:Literal ID="litAddressPrompt" runat="Server" Mode="PassThrough"></asp:Literal>
                </span>
            </div>
            <div>
                <form id="frmEditAddress" runat="server">
                <asp:Literal ID="litswitchformat" runat="server" Mode="PassThrough"></asp:Literal>
                <asp:Panel ID="pnlCheckoutImage" runat="server" HorizontalAlign="Center" Visible="false">
                    <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
                        <asp:RectangleHotSpot AlternateText="Back To Step 1: Shopping Cart" Top="0" Left="0"
                            Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
                    </asp:ImageMap>
                    <br />
                    <br />
                </asp:Panel>
                <div class="error">
                    <cc1:InputValidatorSummary ID="InputValidatorySummary1" runat="server" Register="false">
                    </cc1:InputValidatorSummary>
                </div>
                <asp:Panel ID="pnlAddress" runat="server" Visible="true">

                    <ise:BillingAddressControl 
                        ID="ctrlAddress" 
                        runat="server" 
                        Width="100%" 
                        CaptionWidth="30%" 
                        InputWidth="70%" 
                        WithStateCountyCaption="County" 
                        RegisterCountries="True"
                        TextBoxWithStatePostalCodeCssClass="inputTextBox ajax_textbox1"
                        EnableViewState="true" />

                        <script type="text/javascript">
                            $(document).ready(function () {
                                //Function Location: mobile/js/createaccount_ajax.js
                                RegisterAutoCompletePostalCode('.ajax_textbox1', 'ctrlAddress_Country');
                            });
                        </script>

                        <div class="button_layout" >
                            <uc1:ISEMobileButton ID="btnSaveAddress" runat="server" OnClick="btnSaveAddress_Click" />
                            <uc1:ISEMobileButton ID="btnReturn" runat="server" />
                        </div>

                </asp:Panel>

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
