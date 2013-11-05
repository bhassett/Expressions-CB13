<%@ Page ClientTarget="UpLevel" Language="c#" Inherits="InterpriseSuiteEcommerce.mobile.createaccount"
    CodeFile="createaccount.aspx.cs" %>

<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.Validators"
    TagPrefix="cc1" %>
<%@ Register Assembly="InterpriseSuiteEcommerceControls" Namespace="InterpriseSuiteEcommerceControls.mobile"
    TagPrefix="ise" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<asp:panel id="pnlCheckoutImage" runat="server" horizontalalign="Center" visible="false">
        <asp:ImageMap ID="CheckoutImage" HotSpotMode="Navigate" runat="server">
            <asp:RectangleHotSpot Top="0" Left="0" Right="87" Bottom="54" HotSpotMode="Navigate" NavigateUrl="~/shoppingcart.aspx?resetlinkback=1" />
        </asp:ImageMap>
        <br />
</asp:panel>
<ise:Topic runat="server" ID="CreateAccountPageHeader" TopicName="CreateAccountPageHeader" />
<div class="signin_main">
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
            if (hidBlnWithState.value == "True") {
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
            if (hidShpWithState.value == "True") {
                var citShipCtrl = document.getElementById(arrShipCtrl[0]);
                var posShipCtrl = document.getElementById(arrShipCtrl[1]);
            }
            else {
                var citShipCtrl = document.getElementById(arrShipCtrl[3]);
                var posShipCtrl = document.getElementById(arrShipCtrl[4]);
            }

            var varShipStateCtrl = document.getElementById(arrShipCtrl[2]);
            if (chkAddrInfo.value == "True" && varBillStateCtrl != null) {
                varShipStateCtrl.value = varBillStateCtrl.value;
                citShipCtrl.value = citBillCtrl.value;
                posShipCtrl.value = posBillCtrl.value;
            }

            var strBillAddr = '';
            var strShipAddr = '';
            var returnValue = '';
            var popModalFlag = document.getElementById('hidValid');

            if (popModalFlag != null && popModalFlag.value == "false") {

                //            // Check if return flag for Billing Address has a value 
                //            if (strBChck.value != '') {

                //                var strQueryString = "?SkinId=" + hidSkinID.value + "&Locale=" + hidLocale.value + "&Status=" +
                //                        hidBillCheck.value + "&Title=" + hidBillTitle.value + "&PostalCode=" + encodeURIComponent(hidBlnPostalCode.value) + "&City=" +
                //                        encodeURIComponent(hidBlnCity.value) + "&State=" + hidBlnState.value + "&Country=" + hidBlnCountry.value + "&WithState=" + hidBlnWithState.value;

                //                returnValue = window.showModalDialog("addresschecker.aspx" + strQueryString, "", "dialogWidth:585px; dialogHeight:400px; status:no; center:yes; edge:raised; scroll:off");

                //                if (returnValue != '' && returnValue != null) {
                //                    var retBillValue = returnValue;
                //                    var arrBillValue = retBillValue.split("*");

                //                    posBillCtrl.value = arrBillValue[0];
                //                    citBillCtrl.value = arrBillValue[1];
                //                    varBillStateCtrl.value = arrBillValue[2];

                //                    // Check to see if Billing and Shipping Address Info are set to be the Same
                //                    if (strSChck.value == '' && chkAddrInfo.value == "True") {
                //                        citShipCtrl.value = arrBillValue[1];
                //                        posShipCtrl.value = arrBillValue[0];
                //                        varShipStateCtrl.value = arrBillValue[2];
                //                    }
                //                }
                //            }

                //            // Check if return flag for Shipping Address has a value which is different to that of Billing Address
                //            if (strSChck.value != '') {
                //                var strQueryString = "?SkinId=" + hidSkinID.value + "&Locale=" + hidLocale.value + "&Status=" +
                //                    hidShipCheck.value + "&Title=" + hidShipTitle.value + "&PostalCode=" + encodeURIComponent(hidShpPostalCode.value) + "&City=" +
                //                    encodeURIComponent(hidShpCity.value) + "&State=" + hidShpState.value + "&Country=" + hidShpCountry.value + "&WithState=" + hidShpWithState.value;

                //                returnValue = window.showModalDialog("addresschecker.aspx" + strQueryString, "", "dialogWidth:585px; dialogHeight:400px; status:no; center:yes; edge:raised; scroll:off");

                //                if (returnValue != '' && returnValue != null) {
                //                    var retShipValue = returnValue;
                //                    var arrShipValue = retShipValue.split("*");
                //                    citShipCtrl.value = arrShipValue[1];
                //                    posShipCtrl.value = arrShipValue[0];
                //                }
                //            }

            }
        }
    </script>
    <asp:panel id="pnlErrorMsg" runat="Server" class="errorMessageContainer"></asp:panel>
    <cc1:InputValidatorSummary ID="InputValidatorySummary1" CssClass="errorLg" runat="server"
        Register="false">
    </cc1:InputValidatorSummary>
    <asp:literal id="Signin" runat="server" mode="PassThrough"></asp:literal>
    <form id="frmCreateAccount" runat="server">
    <asp:panel id="pnlSkipRegistrationEmail" runat="server" horizontalalign="left" class="signin_info">
        <div class="signin_info_body" >
            <table>
            <tr>
                <td>
                    <asp:Label ID="lblSkipRegistrationEmail" runat="server" Text="Enter your email"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtSkipRegistrationEmail" runat="server" Columns="30" MaxLength="50"></asp:TextBox>
                    <br /><br />
                    <center>
                    <small>
                        <asp:Label ID="lblSkipRegistrationInfo" runat="server" Text="Please enter your email so we can email your receipt" /></small>
                        </center>
                </td>
            </tr>
        </table>    
        </div>
    </asp:panel>
    <div class="signin_info">
        <div class="tableHeaderArea">
            <asp:label id="Label1" text="(!createaccount.aspx.5!)" runat="server"></asp:label>
        </div>
        <div class="signin_info_body">
            <ise:BillingAddressControl CssClass="billingaddress" ID="ctrlBillingAddress" runat="server"
                Width="100%" WithStateCountyCaption="County" RegisterCountries="True" ShowResidenceType="false"
                TextBoxWithStatePostalCodeCssClass="inputTextBox ajax_textbox1" EnableViewState="true" />
        </div>
    </div>
    <br />
    <div class="signin_info">
        <div class="tableHeaderArea">
            <asp:literal id="Literal1" mode="PassThrough" text="(!createaccount.aspx.19!)" runat="server"></asp:literal>
            <asp:checkbox id="chkCopyBillingInfo" runat="server" />
            <asp:literal id="Literal2" mode="PassThrough" text="(!createaccount.aspx.20!)" runat="server"></asp:literal>
        </div>
        <div class="signin_info_body">
            <asp:hiddenfield runat="server" id="hdenStateValue"></asp:hiddenfield>
            <ise:AddressControl2 Width="100%" CssClass="billingaddress" ID="ctrlShippingAddress"
                runat="server" RegisterCountries="False" ShowFirstName="False" ShowLastName="False"
                ShowTaxNumber="True" TextBoxWithStatePostalCodeCssClass="inputTextBox ajax_textbox1"
                EnableViewState="True" />
        </div>
    </div>
    <div class="button_layout">
        <uc1:ISEMobileButton ID="btnContinueCheckout" runat="server" />
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            //Function Location: mobile/js/core.js
            RegisterAutoCompletePostalCode('.ajax_textbox1', 'ctrlBillingAddress_Country');
            RegisterAutoCompletePostalCode('.ajax_textbox2', 'ctrlShippingAddress_Country');
        });
    </script>
    </form>
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
</div>
