var _passwordMinLength = 0;
$(document).ready(function () {

    var loadAtPage = $("#load-at-page").val();

    switch (loadAtPage) {
        case "create-account":

            GetStringResources(loadAtPage, true);    // --> see jquery.cbe.bubble.message.js for definition
            GetAppConfigs(loadAtPage);  // --> see jquery.cbe.bubble.message.js for definition

            CreateAccount_InitControls();
            CreateAccount_EventsListener();

            break;
        case "edit-profile":

            GetStringResources(loadAtPage, true);
            GetAppConfigs(loadAtPage);

            Profile_EventHandlers();
            RenderProfileDetails();

            break;
        case "select-address":

            GetStringResources("address", true);
            GetAppConfigs("address");

            var process = getQueryString()["add"];

            if (process == "true") {

                AddressBook_EventHandlers(true);
                AddressBook_InitControls();
            }

            break;
        case "edit-address":

            GetStringResources("address", true);
            GetAppConfigs("address");

            AddressBook_EventHandlers(false);
            AddressBook_InitControls();

            break;
        default:
            break;
    }

});


/* Create Account starts here --> 
  
List of active function used on Create Account:

1. InitSignUpFormOnLoad
2. CreateAccount_EventsListener
3. CopyBillingInformation
4. DisableShippingControl
5. StartSaveProcess
6. VerifyBillingAndShippingAddress
7. CreateAccount_InitControls
8. TriggerCreateAccountSubmitButton
9. GetSelectedCityState

*/

function InitSignUpFormOnLoad() { // <-- this function is active on jquery.cbe.bubble.message.js: see InitCreateAccountAppConfigs function

    _passwordMinLength = ise.Configuration.getConfigValue("PasswordMinLength");
  
    if (_IsCaptchaRequired == "true") {
     
        $(".captcha-section").fadeIn("slow");
        $("#txtCaptcha").ISEBubbleMessage({ "input-id": "txtCaptcha", "label-id": "lblCaptcha" });
    }

    if (_IsAllowShippingAddress == "false") {

        $(".shipping-section-clears").css("display", "none");
        $("#shipping-section-head-place-holder").html("");
        $("#shipping-info-place-holder").html("");

    };

    $("#copyBillingInfo").click(function () {

        if ($(this).attr("checked") == "checked") {

            CopyBillingInformation(true);

        } else {

            CopyBillingInformation(false);
        }
    });

    $("#chkOver13").click(function () {

        if ($(this).attr("checked") == "checked") $("#ise-message-tips").fadeOut("slow"); ;

    });

    if ($("#BillingAddressControl_drpBusinessType").val() == _WholeSale) {
        
        $("#tax-number-place-holder").removeClass("display-none");
        $("#tax-number-place-holder").fadeIn("slow");
    }
}

function CreateAccount_EventsListener() {

    $("#create-customer-account").click(function () { StartSaveProcess(); });

    $("#captcha-refresh-button").click(function () {

        captchaCounter++;
        $("#captcha").attr("src", "Captcha.ashx?id=" + captchaCounter);

    });

    $("#ProfileControl_txtPassword").keyup(function () {

        if ($(this).val() == "") {

            $("#ProfileControl_txtConfirmPassword").val("");

            $("#lblConfirmPassword").removeClass("display-none");

            $("#ProfileControl_txtConfirmPassword").removeClass("password-not-match");
            $("#ProfileControl_txtPassword").removeClass("password-not-strong");
            $("#ProfileControl_txtConfirmPassword").removeClass("required-input")
        }

    });

    $("#ProfileControl_txtConfirmPassword").keyup(function () {

        if ($(this).val() == "") {

            $("#ProfileControl_txtPassword").removeClass("password-not-match");
            $("#ProfileControl_txtPassword").removeClass("password-not-strong");
            $("#ProfileControl_txtPassword").removeClass("password-length-invalid");
            $("#ProfileControl_txtPassword").removeClass("required-input")
        }

    });

 
    $("#ShippingAddressControl_drpType").change(function () {

        $(this).removeClass("required-input");
        $("#ise-message-tips").fadeOut("slow");

    });


    $("#BillingAddressControl_drpBusinessType").change(function () {

        $(this).removeClass("required-input");
        $("#ise-message-tips").fadeOut("slow");

        if ($(this).val() == _WholeSale) {

            $("#tax-number-place-holder").removeClass("display-none");
            $("#tax-number-place-holder").fadeIn("slow");

        } else {

            $("#tax-number-place-holder").fadeOut("slow");
        }

    });

    $("#ProfileControl_txtFirstName").blur(function () {

        var fistName = $(this).val();
        var lastName = $("#ProfileControl_txtLastName").val();

        if (fistName != "" && lastName != "") {

            $("#lblAccountName").addClass("display-none");
            $("#ProfileControl_txtAccountName").val(fistName + " " + lastName);
            $("#ProfileControl_txtAccountName").removeClass("required-input");
        }

    });

    $("#ProfileControl_txtLastName").blur(function () {

        var fistName = $("#ProfileControl_txtFirstName").val();
        var lastName = $(this).val();

        if (fistName != "" && lastName != "") {
            $("#lblAccountName").addClass("display-none");
            $("#ProfileControl_txtAccountName").val(fistName + " " + lastName);
            $("#ProfileControl_txtAccountName").removeClass("required-input");
        }

    });

    disableSubmitOnENTER("create-account");
}

function disableSubmitOnENTER(page) {

    switch (page) {
        case "create-account":

               $("#ProfileControl_txtEmail").keypress(function (event) { return enterEvent(event); });
               $("#ProfileControl_txtFirstName").keypress(function (event) { return enterEvent(event); });
               $("#ProfileControl_txtLastName").keypress(function (event) { return enterEvent(event); });
               $("#ProfileControl_txtEmail").keypress(function (event) { return enterEvent(event); });
               $("#ProfileControl_txtContactNumber").keypress(function (event) { return enterEvent(event); });
               $("#ProfileControl_txtAccountName").keypress(function (event) { return enterEvent(event); });
               $("#ProfileControl_txtPassword").keypress(function (event) { return enterEvent(event); });
               $("#ProfileControl_txtConfirmPassword").keypress(function (event) { return enterEvent(event); });

            break;
        default:
            break;
    }
}

function enterEvent(event) { if (event.which == 13) return false; }

function ValidatePassword(controlId) {
    if (_CustomerSPExpression != "" && _IsStrongPassword == "true" && !$(controlId).val().match(_CustomerSPExpression)) return false;
    return true;
}

function DoPasswordValidation(thisObject) {

    if (typeof ($(thisObject).val()) == "undefined") return true;

    var strongPassword = true;
    strongPassword = ValidatePassword(thisObject);

    if ($(thisObject).val().length < _passwordMinLength) {

        $(thisObject).addClass("password-length-invalid");
        return false;

    }

    if (!strongPassword) {

        $(thisObject).addClass("password-not-strong");
        return false;

    } else {

        $(thisObject).removeClass("password-not-strong");

        var myPassword      = $("#ProfileControl_txtPassword").val();
        var confirmPassword = $("#ProfileControl_txtConfirmPassword").val();

        if (myPassword != "" && confirmPassword != "") {

            if (myPassword != confirmPassword) {

                $(thisObject).addClass("password-not-match");

                return false;

            } else {

                $(thisObject).removeClass("password-not-match");

                $("#ProfileControl_txtPassword").removeClass("password-not-match");
                $("#ProfileControl_txtConfirmPassword").removeClass("password-not-match");

                return true;
            }

        }

    }

    return true;
}

function CopyBillingInformation(copy) {

    $("#ShippingAddressControl_txtStreet").removeClass("required-input");
    $("#ShippingAddressControl_txtPostal").removeClass("required-input");
    $("#ShippingAddressControl_txtPostal").removeClass("invalid-postal");
    $("#ShippingAddressControl_txtState").removeClass("required-input");
    $("#ShippingAddressControl_txtState").removeClass("state-not-found");
    $("#ShippingAddressControl_txtCity").removeClass("required-input");
    $("#ShippingAddressControl_txtCounty").removeClass("required-input");

    if (copy) {

        var country          = $("#BillingAddressControl_drpCountry").val();
        var billingCityState = $("#billing-city-states").val();
        var city             = "";
        var state            = "";

        if (typeof (billingCityState) == "undefined" || billingCityState == "other") {

            city  = $("#BillingAddressControl_txtCity").val();
            state = $("#BillingAddressControl_txtState").val();

        } else {

            var bc = billingCityState.split(", ");

            if (bc.length > 1) {

                city = bc[1];
                state = bc[0];

            } else {

                city = bc[0];
                state = "";
            }

        }

        var street = $("#BillingAddressControl_txtStreet").val();
        var postal = $("#BillingAddressControl_txtPostal").val();
        var county = $("#BillingAddressControl_txtCounty").val();

        $("#ShippingAddressControl_txtStreet").val(street);
        $("#ShippingAddressControl_txtCity").val(city);
        $("#ShippingAddressControl_txtPostal").val(postal);
        $("#ShippingAddressControl_txtState").val(state);
        $("#ShippingAddressControl_drpCountry").val(country);

        if (typeof (county) != "undefined") $("#ShippingAddressControl_txtCounty").val(county);

        $("#ise-message-tips").fadeOut("slow");

        DisableShippingControl(true);

    } else {

        DisableShippingControl(false);

    }

    if (IsCountryWithStates("#ShippingAddressControl_drpCountry") == "false") {

        $("#ShippingAddressControl_txtState").fadeOut("slow", function () {

            $("#ShippingAddressControl_lblState").addClass("display-none");
            $("#ShippingAddressControl_txtCity").addClass("city-width-if-no-state");

        });

    } else {

        $("#ShippingAddressControl_txtCity").removeClass("city-width-if-no-state");
        $("#ShippingAddressControl_txtState").fadeIn("slow");

    }
}

function DisableShippingControl(disable) {

    if (disable) {

        $("#ShippingAddressControl_lblStreet").addClass("display-none");
        $("#ShippingAddressControl_lblPostal").addClass("display-none");
        $("#ShippingAddressControl_lblCity").addClass("display-none");
        $("#ShippingAddressControl_lblState").addClass("display-none");
        $("#ShippingAddressControl_lblCounty").addClass("display-none");

        $("#ShippingAddressControl_txtStreet").addClass("control-disabled");
        $("#ShippingAddressControl_txtStreet").attr("disabled", "disabled");

        $("#ShippingAddressControl_txtPostal").addClass("control-disabled");
        $("#ShippingAddressControl_txtPostal").attr("disabled", "disabled");

        $("#ShippingAddressControl_txtCity").addClass("control-disabled");
        $("#ShippingAddressControl_txtCity").attr("disabled", "disabled");

        $("#ShippingAddressControl_txtState").addClass("control-disabled");
        $("#ShippingAddressControl_txtState").attr("disabled", "disabled");

        $("#ShippingAddressControl_txtCounty").addClass("control-disabled");
        $("#ShippingAddressControl_txtCounty").attr("disabled", "disabled");

        $("#ShippingAddressControl_drpCountry").css("background", "#ccc");
        $("#ShippingAddressControl_drpCountry").attr("disabled", "disabled");

        $("#shipping-enter-postal-label-place-holder").html("<input type='hidden' id='shipping-city-states' value='other'/>");
        $("#shipping-city-states").fadeOut("Slow", function () { $(".shipping-zip-city-other-place-holder").fadeIn("Slow"); });

    } else {

        $("#ShippingAddressControl_txtStreet").removeClass("control-disabled");
        $("#ShippingAddressControl_txtStreet").removeAttr("disabled");

        $("#ShippingAddressControl_txtPostal").removeClass("control-disabled");
        $("#ShippingAddressControl_txtPostal").removeAttr("disabled");

        $("#ShippingAddressControl_txtCity").removeClass("control-disabled");
        $("#ShippingAddressControl_txtCity").removeAttr("disabled", "disabled");

        $("#ShippingAddressControl_drpCountry").css("background", "#fff");
        $("#ShippingAddressControl_drpCountry").removeAttr("disabled");

        $("#ShippingAddressControl_txtState").removeClass("control-disabled");
        $("#ShippingAddressControl_txtState").removeAttr("disabled");

        $("#ShippingAddressControl_txtCounty").removeClass("control-disabled");
        $("#ShippingAddressControl_txtCounty").removeAttr("disabled");
    }
}

function StartSaveProcess() {

    

    /*           
        -> This function revalidates required informations and email address format 
        -> If all information is good then calls function DoSubmissionOfCaseFormAction()
    */

    $("#ise-message-tips").fadeOut("slow");

    var goodForm = true;

    var counter = 0;
    var formHasEmptyFields = false;

    var thisObjectId = "";
    var skip = false;
    var skipStateValidation = false;

    $(".requires-validation").each(function () {   //-> scan all html controls with class .apply-behind-caption-effects

        if ($(this).val() == "") {

            skip = false;

            var object = this;
            var thisObjectId = "#" + $(object).attr("id");

            var cssDisplay = $(".billing-zip-city-other-place-holder").css("display");
            var objectValue = $(this).val()

            /*  If city control is on display and empty: validate */

            if (thisObjectId == "#BillingAddressControl_txtCity") {

                if (cssDisplay == "none") {
                    skip = true;
                } else {
                    skip = true;
                    if (objectValue == "") skip = false;
                }
            }

            if (thisObjectId == "#BillingAddressControl_txtState") {

                if (cssDisplay == "none") {
                    skip = true;
                } else {
                    skip = true;
                    if (objectValue == "") skip = false;
                }
            }


            //-> validation (IsEmpty) for shipping city input

            var sameAsBillingInfo = $("#copyBillingInfo").attr('checked');

            if ((thisObjectId == "#ShippingAddressControl_txtPostal" || thisObjectId == "#ShippingAddressControl_txtStreet") && sameAsBillingInfo == "checked") skip = true;

            if (thisObjectId == "#ShippingAddressControl_txtCity") {

                cssDisplay = $(".shipping-zip-city-other-place-holder").css("display");

                if (cssDisplay == "none" || sameAsBillingInfo == "checked") {

                    skip = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            /* state control --> 

            If state control is on display 
                
            -> It must be validated on submit 
            -> Skip EMPTY validation if state control has assigned value

            otherwise:

            skip state validation and skp EMPTY validation of state control

            */

            // -> validation (IsEmpty) for billing states input:

            if (thisObjectId == "#BillingAddressControl_txtState") {
               
                cssDisplay = $(".billing-zip-city-other-place-holder").css("display");
                var status = IsCountryWithStates("#BillingAddressControl_drpCountry");

                if (cssDisplay == "none" || status == "false" || sameAsBillingInfo == "checked") {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            // -> validation (IsEmpty) for shipping states input:

            if (thisObjectId == "#ShippingAddressControl_txtState" && _IsAllowShippingAddress == "true") {

                cssDisplay = $(".shipping-zip-city-other-place-holder").css("display");
                var status = IsCountryWithStates("#ShippingAddressControl_drpCountry");

                if (cssDisplay == "none" || status == "false" || sameAsBillingInfo == "checked") {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            // state control <--

            if (skip == false) {

                $(this).removeClass("current-object-on-focus");
                $(this).addClass("required-input");

                /* Points mouse cursor on the first input with no value to render bubble message */

                if (counter == 0) {

                    thisObjectId = "#" + $(this).attr("id");
                    $(this).addClass("current-object-on-focus");
                    $(this).focus();

                }

                formHasEmptyFields = true;
                counter++;
            }
        }

    });

    if (formHasEmptyFields) {

        $(thisObjectId).focus();
        return false;

    }

    var emailAddress = $("#ProfileControl_txtEmail").val();
    var emailIsValid = IsEmailGood(emailAddress);

    if (emailIsValid == false && formHasEmptyFields == false) {

        $("#ProfileControl_txtEmail").focus();
        return false;
    }

    if (formHasEmptyFields || emailIsValid == false)   return false;

    if (!DoPasswordValidation("#ProfileControl_txtPassword") && $("#ProfileControl_txtPassword").val() != "") {

        $("#ProfileControl_txtPassword").focus();
        return false;

    }

    if (_IsShowTaxFieldOnRegistration == "true" && _IsVatEnablead == "true") {

        if ($("#BillingAddressControl_drpBusinessType").val().toLowerCase() == _ChooseBusinessType.toLowerCase()) {

            $("#BillingAddressControl_drpBusinessType").addClass("required-input");

            var thisLeft = $("#BillingAddressControl_drpBusinessType").offset().left;
            var thisTop = $("#BillingAddressControl_drpBusinessType").offset().top;

            $("#ise-message-tips").css("top", thisTop - 54);
            $("#ise-message-tips").css("left", thisLeft - 17);

            $("#ise-message").html(_SelectBusinessTypeMessage);
            $("#ise-message-tips").fadeIn("slow");

            return false;
        }
    } 

    if (_IsAllowShippingAddress == "true") {

        if ($("#ShippingAddressControl_drpType").val().toLowerCase() == _addressTypeCaption.toLowerCase()) {

            $("#ShippingAddressControl_drpType").addClass("required-input");

            var thisLeft = $("#ShippingAddressControl_drpType").offset().left;
            var thisTop  = $("#ShippingAddressControl_drpType").offset().top;

            $("#ise-message-tips").css("top", thisTop - 54);
            $("#ise-message-tips").css("left", thisLeft - 17);

            $("#ise-message").html(_SelectAddressTypeMessage);
            $("#ise-message-tips").fadeIn("slow");

            return false;
        }

    }

    var b_cityStates = $("#billing-city-states").val();
    if (typeof (b_cityStates) == "undefined") {

        $("#BillingAddressControl_txtPostal").addClass("invalid-postal-zero");

        var thisLeft = $("#BillingAddressControl_txtPostal").offset().left;
        var thisTop = $("#BillingAddressControl_txtPostal").offset().top;

        $("#ise-message-tips").css("top", thisTop - 54);
        $("#ise-message-tips").css("left", thisLeft - 17);

        $("#ise-message").html(_GetCityStateEnterMessage);
        $("#ise-message-tips").fadeIn("slow");

        return false;

    } else {

        $("#BillingAddressControl_txtPostal").removeClass("invalid-postal-zero");
        $("#ise-message-tips").fadeOut("slow");
    }

    var sameAsBillingInfo = $("#copyBillingInfo").attr('checked');

    if (sameAsBillingInfo == "checked")  CopyBillingInformation(true);

    if (_IsAllowShippingAddress == "true" && sameAsBillingInfo != "checked") {

        var s_cityStates = $("#shipping-city-states").val();

        if (typeof (s_cityStates) == "undefined") {

            $("#ShippingAddressControl_txtPostal").addClass("invalid-postal-zero");

            var thisLeft = $("#ShippingAddressControl_txtPostal").offset().left;
            var thisTop = $("#ShippingAddressControl_txtPostal").offset().top;

            $("#ise-message-tips").css("top", thisTop - 54);
            $("#ise-message-tips").css("left", thisLeft - 17);

            $("#ise-message").html(_GetCityStateEnterMessage);
            $("#ise-message-tips").fadeIn("slow");


            return false;

        } else {

            $("#ShippingAddressControl_txtPostal").removeClass("invalid-postal-zero");
            $("#ise-message-tips").fadeOut("slow");
        }
    }

    VerifyBillingAndShippingAddress();
}

function VerifyBillingAndShippingAddress() {

    // -> Billing Address

    var b_addressControlId      = "#BillingAddressControl_drpCountry";
    var b_cityStatesId          = "#billing-city-states";
    var b_postalCodeId          = "#BillingAddressControl_txtPostal";
    var b_statesId              = "#BillingAddressControl_txtState";
    var b_cityStateClassName    = ".billing-zip-city-other-place-holder";
    var b_enterPlaceHolderId    = "#billing-enter-postal-label-place-holder";
    var b_stateCodeId           = "#BillingAddressControl_txtState";

    var b_citystateIsVisible    = $(b_cityStateClassName).css("display");
    var b_citystateIsVisible    = b_citystateIsVisible.toLowerCase();
    var b_country               = $(b_addressControlId).val();
    var b_postalCode            = $(b_postalCodeId).val();
    var b_searchable            = IsCountrySearchable(b_addressControlId);
    var b_withStates            = IsCountryWithStates(b_addressControlId);
    var postalIsGood            = true;

    // check format

    var b_postalHasInvalidFormat = false;
    var s_postalHasInvalidFormat = false;

    if (b_searchable == "true") {

        b_postalHasInvalidFormat = IsPostalFormatInvalid(b_country, b_postalCode);

        if (b_postalHasInvalidFormat) {

            $(b_postalCodeId).addClass("invalid-postal");
            if (b_citystateIsVisible == "none") $(b_enterPlaceHolderId).html(_EnterPostalForCityState);

            postalIsGood = false;
        }

    }

    // -> Shipping Address

    var sameAsBillingInfo = $("#copyBillingInfo").attr('checked');

    if (_IsAllowShippingAddress == "true" && sameAsBillingInfo != "checked") {

        var s_addressControlId   = "#ShippingAddressControl_drpCountry";
        var s_cityStatesId       = "#shipping-city-states";
        var s_postalCodeId       = "#ShippingAddressControl_txtPostal";
        var s_statesId           = "#ShippingAddressControl_txtState"

        var s_cityStateClassName = ".shipping-zip-city-other-place-holder";
        var s_stateCodeId        = "#ShippingAddressControl_txtState";
        var s_enterPlaceHolderId = "#shipping-enter-postal-label-place-holder";

        var s_citystateIsVisible = $(s_cityStateClassName).css("display");
        var s_citystateIsVisible = s_citystateIsVisible.toLowerCase();

        var s_country = $(s_addressControlId).val();
        var s_postalCode = $(s_postalCodeId).val();

        var s_searchable = IsCountrySearchable(s_addressControlId);
        var s_withStates = IsCountryWithStates(s_addressControlId);

        if (s_searchable == "true") {

            s_postalHasInvalidFormat = IsPostalFormatInvalid(s_country, s_postalCode);

            if (s_postalHasInvalidFormat) {

                $(s_postalCodeId).addClass("invalid-postal");
                if (s_citystateIsVisible == "none") $(s_enterPlaceHolderId).html(_EnterPostalForCityState);

                postalIsGood = false;
            }
        }

    }

    var imOver13Flag = $("#chkOver13").attr('checked');

    if (_IsAge13Required == "true"  && imOver13Flag != "checked") {

        $("#age-13-place-holder").addClass("required-input");

        var thisLeft = $("#age-13-place-holder").offset().left;
        var thisTop  = $("#age-13-place-holder").offset().top;

        $("#ise-message-tips").css("top", thisTop - 54);
        $("#ise-message-tips").css("left", thisLeft - 17);

        $("#ise-message").html(_Over13Message);
        $("#ise-message-tips").fadeIn("slow");

        return false;

    } else {  $("#age-13-place-holder").removeClass("support-required-input"); }

    /*
    
    All is Good:
    
    -> Profile Info
    -> Billing Info
    -> Shipping Info [if app config AllowShipToDifferentThanBillTo is set to TRUE]
    -> Email Format, 
    -> Postal Pormat, 
    -> Password Matched( and strong [if app config UseStrongPwd is set to TRUE] 
    -> Checked Over13 [if app config RequireOver13Checked is set to TRUE] 
    -> Business Type [if app config VAT.Enabled && VAT.ShowTaxFieldOnRegistration is set to TRUE]

    Do revalidation of POSTAL (billing and shipping[if app config AllowShipToDifferentThanBillTo is set to TRUE])
    Do create account if ALL is ok

    */

    if (postalIsGood) {

        _CreateAccountFormIsGood = true;
        var progress = ["errorSummary", "save-account-loader", "save-account-button-place-holder"];
        ISEAddressVerification(true, "create-account", "", true, progress);

    }

}

function CreateAccount_InitControls() {

    $("#BillingAddressControl_txtPostal").ISEAddressFinder();

    $("#BillingAddressControl_txtStreet").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtStreet", "label-id": "BillingAddressControl_lblStreet" });
    $("#BillingAddressControl_txtPostal").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtPostal", "label-id": "BillingAddressControl_lblPostal", "input-mode": "billing-postal", "address-type": 'Billing' });
    $("#BillingAddressControl_txtCity").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtCity", "label-id": "BillingAddressControl_lblCity" });
    $("#BillingAddressControl_txtState").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtState", "label-id": "BillingAddressControl_lblState", "input-mode": "billing-state" });
    $("#BillingAddressControl_txtTaxNumber").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtTaxNumber", "label-id": "BillingAddressControl_lblTaxNumber", "optional": true });
   
    if (typeof ($("#BillingAddressControl_txtCounty").val()) != "undefined") {

        $("#BillingAddressControl_txtCounty").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtCounty", "label-id": "BillingAddressControl_lblCounty", "optional": true });

    }

    $("#ProfileControl_txtFirstName").ISEBubbleMessage({ "input-id": "ProfileControl_txtFirstName", "label-id": "lblFirstName" });
    $("#ProfileControl_txtLastName").ISEBubbleMessage({ "input-id": "ProfileControl_txtLastName", "label-id": "lblLastName" });
    $("#ProfileControl_txtEmail").ISEBubbleMessage({ "input-id": "ProfileControl_txtEmail", "label-id": "lblEmail", "input-mode": "email" });
    $("#ProfileControl_txtContactNumber").ISEBubbleMessage({ "input-id": "ProfileControl_txtContactNumber", "label-id": "lblContactNumber" });

    $("#ProfileControl_txtAccountName").ISEBubbleMessage({ "input-id": "ProfileControl_txtAccountName", "label-id": "lblAccountName" });
    $("#ProfileControl_txtPassword").ISEBubbleMessage({ "input-id": "ProfileControl_txtPassword", "label-id": "lblPassword", "input-mode": "password" });
    $("#ProfileControl_txtConfirmPassword").ISEBubbleMessage({ "input-id": "ProfileControl_txtConfirmPassword", "label-id": "lblConfirmPassword", "input-mode": "password-confirmation" });

    $("#ShippingAddressControl_txtPostal").ISEAddressFinder({
        'country-id': '#ShippingAddressControl_drpCountry',
        'postal-id': '#ShippingAddressControl_txtPostal',
        'city-id': '#ShippingAddressControl_txtCity',
        'state-id': '#ShippingAddressControl_txtState',
        'city-state-place-holder': '.shipping-zip-city-other-place-holder',
        'enter-postal-label-place-holder': '#shipping-enter-postal-label-place-holder',
        'city-states-id': 'shipping-city-states'
    });

    $("#ShippingAddressControl_txtStreet").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtStreet", "label-id": "ShippingAddressControl_lblStreet" });
    $("#ShippingAddressControl_txtPostal").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtPostal", "label-id": "ShippingAddressControl_lblPostal", "input-mode": "shipping-postal", "address-type": 'Shipping' });
    $("#ShippingAddressControl_txtCity").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtCity", "label-id": "ShippingAddressControl_lblCity" });
    $("#ShippingAddressControl_txtState").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtState", "label-id": "ShippingAddressControl_lblState", "input-mode": "shipping-state" });
 
    if (typeof ($("#ShippingAddressControl_txtCounty").val()) != "undefined") {

        $("#ShippingAddressControl_txtCounty").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtCounty", "label-id": "ShippingAddressControl_lblCounty", "optional": true });

    }

    if ($("#copyBillingInfo").attr("checked") == "checked") CopyBillingInformation(true);
    GetSelectedCityState();
}

function TriggerCreateAccountSubmitButton() {

    var _bCityStates = $("#billing-city-states").val();

    if (_bCityStates != "other") {

        $("#billingTxtCityStates").val(_bCityStates);

    } else {

        $("#billingTxtCityStates").val($("#BillingAddressControl_txtState").val() + ", " + $("#BillingAddressControl_txtCity").val());
    }

    if (_IsAllowShippingAddress == "true") {

        var _sCityStates = $("#shipping-city-states").val();

        if (_sCityStates != "other") {

            $("#shippingTxtCityStates").val(_sCityStates);

        } else {

            $("#shippingTxtCityStates").val($("#ShippingAddressControl_txtState").val() + ", " + $("#ShippingAddressControl_txtCity").val());
        }

    }

   ShowProcessMessage(_SavingAccountMessage, "error-summary", "save-account-loader", "save-account-button-place-holder");
   $("#btnCreateAccount").click();

}

function GetSelectedCityState() {
    
    if ($("#billingTxtCityStates").val() != "") {

        $(".billing-zip-city-other-place-holder").fadeIn("Slow");
        $("#billing-enter-postal-label-place-holder").html("<input type='hidden' id='billing-city-states' value='other'/>");

        var _bCityState = $("#billingTxtCityStates").val().split(",");

        if (_bCityState.length > 1) {

            if (_bCityState[0] != "") $("#BillingAddressControl_lblState").addClass("display-none");
            if (_bCityState[1] != "") $("#BillingAddressControl_lblCity").addClass("display-none");

            $("#BillingAddressControl_txtState").val(_bCityState[0]);
            $("#BillingAddressControl_txtCity").val(_bCityState[1]);
          

        } else {

            if (_bCityState[0] != "") $("#BillingAddressControl_lblCity").addClass("display-none");
            $("#BillingAddressControl_txtCity").val(_bCityState[0]);
        }

    }

    if ($("#shippingTxtCityStates").val() != "") {

        $(".shipping-zip-city-other-place-holder").fadeIn("Slow");
        $("#shipping-enter-postal-label-place-holder").html("<input type='hidden' id='shipping-city-states' value='other'/>");

        var _sCityState = $("#shippingTxtCityStates").val().split(",");

        if (_sCityState.length > 1) {

            if (_sCityState[0] != "") $("#ShippingAddressControl_lblState").addClass("display-none");
            if (_sCityState[1] != "") $("#ShippingAddressControl_lblCity").addClass("display-none");

            $("#ShippingAddressControl_txtState").val($.trim(_sCityState[0]));
            $("#ShippingAddressControl_txtCity").val($.trim(_sCityState[1]));
           
        } else {

            if (_sCityState[0] != "") $("#ShippingAddressControl_lblCity").addClass("display-none");
            $("#ShippingAddressControl_txtCity").val(_sCityState[0]);
            
        }

    }

    if(IsCountryWithStates("#BillingAddressControl_drpCountry")=="false"){

        $("#BillingAddressControl_lblState").removeClass("display-none")
        $("#BillingAddressControl_txtState").val("");
        $("#BillingAddressControl_txtState").addClass("control-disabled");

        $("#BillingAddressControl_txtState").fadeOut("slow", function () {

            $("#BillingAddressControl_lblState").addClass("display-none");
            $("#BillingAddressControl_txtCity").addClass("city-width-if-no-state");
        
        });

    }

    if (IsCountryWithStates("#ShippingAddressControl_drpCountry") == "false") {

        $("#ShippingAddressControl_lblState").removeClass("display-none")
        $("#ShippingAddressControl_txtState").val("");
        $("#ShippingAddressControl_txtState").addClass("control-disabled");

        $("#ShippingAddressControl_txtState").fadeOut("slow", function () {

            $("#ShippingAddressControl_lblState").addClass("display-none");
            $("#ShippingAddressControl_txtCity").addClass("city-width-if-no-state");

        });

    }
}

// Create Account ends here <--


/* Edit Profile starts here --> 
  
List of active function used on Edit Account:

1. InitProfileFormOnLoad
2. Profile_EventHandlers
3. UpdateCustomerProfile
4. Profile_EditPassword
5. GetCustomerProfile
6. RenderProfileDetails

*/

function InitProfileFormOnLoad() { // <-- this function is active on jquery.cbe.bubble.message.js: see InitCreateAccountAppConfigs function

    _passwordMinLength = ise.Configuration.getConfigValue("PasswordMinLength");

    if (_IsCaptchaRequired == "true") {

        $("#txtCaptcha").val("");
        $(".captcha-section").fadeIn("slow");
        $("#txtCaptcha").ISEBubbleMessage({ "input-id": "txtCaptcha", "label-id": "lblCaptcha" });

    }

    $("#ProfileControl_txtFirstName").ISEBubbleMessage({ "input-id": "ProfileControl_txtFirstName", "label-id": "lblFirstName" });
    $("#ProfileControl_txtLastName").ISEBubbleMessage({ "input-id": "ProfileControl_txtLastName", "label-id": "lblLastName" });
    $("#ProfileControl_txtEmail").ISEBubbleMessage({ "input-id": "ProfileControl_txtEmail", "label-id": "lblEmail", "input-mode": "email" });
    $("#ProfileControl_txtContactNumber").ISEBubbleMessage({ "input-id": "ProfileControl_txtContactNumber", "label-id": "lblContactNumber" });

    $("#ProfileControl_txtPassword").ISEBubbleMessage({ "input-id": "ProfileControl_txtPassword", "label-id": "lblPassword", "input-mode": "password" });
    $("#ProfileControl_txtConfirmPassword").ISEBubbleMessage({ "input-id": "ProfileControl_txtConfirmPassword", "label-id": "lblConfirmPassword", "input-mode": "password-confirmation" });
    $("#old-password-input").ISEBubbleMessage({ "input-id": "old-password-input", "label-id": "old-password-input-label" });

}

function Profile_EventHandlers() {

    Profile_EditPassword(false);

    $("#edit-password").removeAttr("checked");

    $("#edit-password").click(function () {

        var checked = $(this).attr("checked");

        if (checked == "checked") {

            Profile_EditPassword(true);

        } else {

            Profile_EditPassword(false);

        }

    });

    $("#captcha-refresh-button").click(function () {

        _CaptchaCounter += 1;
        _CaptchaCounter++;

        $("#captcha").attr("src", "Captcha.ashx?id=" + _CaptchaCounter);

    });

    $("#support-security-code").css("width", "193px");
    $("#captcha-label").css("padding-right", "31px");

    $("#update-profile").click(function () {

        if ($(this).hasClass("editable-content")) return false;

        var emailAddress = $.trim($("#ProfileControl_txtEmail").val());

        var RequiredInputsAllGood = IsRequiredInformationAllGood();

        if (!RequiredInputsAllGood) return false;

        var EmailIsValid = IsEmailGood(emailAddress);

        if (!EmailIsValid) {

            $("#ProfileControl_txtEmail").focus();
            return false;
        }

        if ($("#edit-password").attr("checked") == "checked") {

            if (!DoPasswordValidation("#ProfileControl_txtPassword")) {

                $("#ProfileControl_txtPassword").focus();
                return false;

            }

            if (!DoPasswordValidation("#ProfileControl_txtConfirmPassword")) {

                $("#ProfileControl_txtConfirmPassword").focus();
                return false;

            }

            UpdateCustomerProfile(true);

        } else {

            UpdateCustomerProfile(false);

        }


    });

}

function UpdateCustomerProfile(updatePassword) {

    var salutation = $("#ProfileControl_drpLstSalutation").val();

    if (typeof (salutation) != "undefined") {

        if (salutation.toLowerCase() == _SalutationDefaultValue) salutation = "";

    } else {

        salutation = "";
    }

    var firstName   = $.trim($("#ProfileControl_txtFirstName").val());
    var lastName    = $.trim($("#ProfileControl_txtLastName").val());
    var email       = $.trim($("#ProfileControl_txtEmail").val());
    var password    = "";
    var oldPassword = "";

    if (updatePassword) {

        password    = $("#ProfileControl_txtPassword").val();
        oldPassword = $("#old-password-input").val();

    }

    var productUpdatesSubscription = false;
    var ageOver13YearOld = false;

    var contactNumber = $.trim($("#ProfileControl_txtContactNumber").val());

    var productUpdates = $("#product-updates").attr('checked');

    if (productUpdates == "checked") {

        productUpdates = "yes";

    } else {

        productUpdates = "no";

    }

    var imOver13YearsOfAge = $("#im-over-13-age").attr('checked');

    if (imOver13YearsOfAge == "checked") {

        imOver13YearsOfAge = "yes";

    } else {

        imOver13YearsOfAge = "no";

    }

    var captcha = $.trim($("#txtCaptcha").val());

    var profile = [salutation, firstName, lastName, email, contactNumber, productUpdates, imOver13YearsOfAge, password, oldPassword];

    ShowProcessMessage(_UpdatingProfileMessage, "profile-error-place-holder", "save-profile-loader", "save-profile-button-place-holder");

    AjaxCallWithSecuritySimplified(
        "UpdateCustomerProfile",
        { "profile": profile, "captcha": captcha },
        function (result) {

            var _output = result.d;

            if (_output != "") {

                _output = _output.split("::");

                switch (_output[0]) {
                    case "false":
                        ShowFailedMessage(_output[1], "profile-error-place-holder", "save-profile-loader", "save-profile-button-place-holder");
                        $("#profile-error-place-holder").removeClass("display-none");
                        break;
                    case "true":
                        $("#profile-error-place-holder").addClass("display-none");
                        ShowProcessMessage(_RefreshingProfileMessage, "profile-error-place-holder", "save-profile-loader", "save-profile-button-place-holder");
                        parent.location = "account.aspx";
                    default:
                        break;
                }

            }

        },
        function (result) {
            ShowFailedMessage(result.d, "profile-error-place-holder", "save-profile-loader", "save-profile-button-place-holder");
            $("#profile-error-place-holder").removeClass("display-none");
        }
    );

}

function Profile_EditPassword(edit) {

    $("#old-password-input").removeClass("required-input");
    $("#old-password-input").removeClass("current-object-on-focus");

    $("#ProfileControl_txtPassword").removeClass("required-input");
    $("#ProfileControl_txtPassword").removeClass("current-object-on-focus");

    $("#ProfileControl_txtConfirmPassword").removeClass("required-input");
    $("#ProfileControl_txtConfirmPassword").removeClass("current-object-on-focus");

    $("#ProfileControl_txtPassword").removeClass("password-not-match");
    $("#ProfileControl_txtPassword").removeClass("password-not-strong");

    $("#ProfileControl_txtConfirmPassword").removeClass("password-not-match");

    $("#old-password-input-label").css("color", "#8E8E8E");
    $("#lblPassword").css("color", "#8E8E8E");
    $("#lblConfirmPassword").css("color", "#8E8E8E");

    $("#ise-message-tips").fadeOut("slow");

    if (edit) {

        $("#old-password-input").val("");
        $("#ProfileControl_txtPassword").val("");
        $("#ProfileControl_txtConfirmPassword").val("");

        $("#old-password-input").removeClass("control-disabled");

        $("#old-password-input-label").removeClass("display-none");
        $("#old-password-input").removeAttr("disabled");

        $("#ProfileControl_txtPassword").removeClass("control-disabled");
        $("#ProfileControl_txtPassword").removeAttr("disabled");

        $("#ProfileControl_lblPassword").removeClass("display-none");

        $("#ProfileControl_txtConfirmPassword").removeClass("control-disabled");
        $("#ProfileControl_lblConfirmPassword").removeClass("display-none");

        $("#ProfileControl_txtConfirmPassword").removeAttr("disabled");

        $("#password-caption").removeClass("control-caption-disabled");
        $("#old-password-label-place-holder").removeClass("control-caption-disabled");

    } else {

        $("#old-password-label-place-holder").addClass("control-caption-disabled");

        $("#old-password-input").addClass("control-disabled");
        $("#old-password-input").attr("disabled", "disabled");

        $("#ProfileControl_txtPassword").addClass("control-disabled");
        $("#ProfileControl_txtPassword").attr("disabled", "disabled");

        $("#ProfileControl_txtConfirmPassword").addClass("control-disabled");
        $("#ProfileControl_txtConfirmPassword").attr("disabled", "disabled");

        $("#password-caption").addClass("control-caption-disabled");

    }

}

function GetCustomerProfile(list) {

    /* 
    Profile details assignments:

    1. Assigns profile details to input box
    2. Hides input box label if field value is not empty 
    3. Shows input box label if field value is empty

    -->

    */

    $("#ProfileControl_drpLstSalutation").val(list[0]);

    if (list[1] != "") {

        $("#ProfileControl_txtFirstName").val(list[1]);
        $("#lblFirstName").addClass("display-none");

    } else {

        $("#lblFirstName").removeClass("display-none");

    }

    if (list[2] != "") {

        $("#ProfileControl_txtLastName").val(list[2]);
        $("#lblLastName").addClass("display-none");

    } else {

        $("#lblLastName").removeClass("display-none");

    }

    if (list[4] != "") {
        $("#ProfileControl_txtEmail").val(list[4]);
        $("#lblEmail").addClass("display-none");
    } else {
        $("#lblEmail").removeClass("display-none");
    }


    if (list[5] != "") {
        $("#ProfileControl_txtContactNumber").val(list[5]);
        $("#lblContactNumber").addClass("display-none");
    } else {
        $("#lblContactNumber").removeClass("display-none");
    }

    if (list[11] == "True") {

        $("#product-updates").attr("checked", "checked");

    } else {

        $("#product-updates").removeAttr("checked");
    }

    if (list[12] == "True") {

        $("#im-over-13-age").attr("checked", "checked");

    } else {

        $("#im-over-13-age").removeAttr("checked");
    }

}

function RenderProfileDetails() {

    var buttonsHTML = "";
    var thisProcessStringResource = _ProfileCheckingMessage;

    $("#support-form-ajax-process-message").removeClass("error-message");

    $("#support-form-ajax-process-message").html("<div style='float:left;width:12px;'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>" + thisProcessStringResource + "</div>");
    $("#support-form-ajax-process-message").fadeIn("slow");

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetCustomerProfile",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            var result = result.d;

            if (result != "0") {
                var list = result.split("::");
                GetCustomerProfile(list);
            }

            $("#support-form-ajax-process-message").html(_DefaultMessage);
        },
        fail: function (result) {

            $("#support-form-ajax-process-message").addClass("error-message");
            $("#support-form-ajax-process-message").html(result.d);
        }
    });
}


// Edit profile active functions ends here <--

// Edit Address / Add Address

function AddressBook_InitControls() {

    $("#AddressControl_txtPostal").ISEAddressFinder({
        'country-id'                : '#AddressControl_drpCountry',
        'postal-id'                 : '#AddressControl_txtPostal',
        'city-id'                   : '#AddressControl_txtCity',
        'state-id'                  : '#AddressControl_txtState',
        'city-state-place-holder'   : '.zip-city-other-place-holder',
        'enter-postal-label-place-holder'    : '#enter-postal-label-place-holder',
        'city-states-id'            : 'city-states'
    });

    $("#txtContactName").ISEBubbleMessage({ "input-id": "txtContactName", "label-id": "lblContactName" });
    $("#txtContactNumber").ISEBubbleMessage({ "input-id": "txtContactNumber", "label-id": "lblContactNumber" });

    $("#AddressControl_txtStreet").ISEBubbleMessage({ "input-id": "AddressControl_txtStreet", "label-id": "AddressControl_lblStreet" });
    $("#AddressControl_txtPostal").ISEBubbleMessage({ "input-id": "AddressControl_txtPostal", "label-id": "AddressControl_lblPostal", "input-mode": "postal" });
    $("#AddressControl_txtCity").ISEBubbleMessage({ "input-id": "AddressControl_txtCity", "label-id": "AddressControl_lblCity" });
    $("#AddressControl_txtState").ISEBubbleMessage({ "input-id": "AddressControl_txtState", "label-id": "AddressControl_lblState", "input-mode": "state" });

    if (typeof ($("#AddressControl_txtCounty").val()) != "undefined") {

        $("#AddressControl_txtCounty").ISEBubbleMessage({ "input-id": "AddressControl_txtCounty", "label-id": "AddressControl_lblCounty", "optional": true });

    }

    $("#txtContactName").addClass("edit-address-contact-name");

}

function AddressBook_EventHandlers(addnew) {

    if (!addnew) {
        $(".zip-city-other-place-holder").fadeIn("Slow");
        $("#enter-postal-label-place-holder").html("<input type='hidden' id='city-states' value='other'/>");
    }

    $("#save-address").click(function () {

        if ($(this).hasClass("editable-content")) return true;
        SaveAddress(addnew);

    });

}

function SaveAddress(addnew) {

    $("#ise-message-tips").fadeOut("slow");

    var goodForm = true;

    var counter = 0;
    var formHasEmptyFields = false;

    var thisObjectId = "";
    var skip = false;
    var skipStateValidation = false;

    $(".requires-validation").each(function () {   //-> scan all html controls with class .apply-behind-caption-effects

        if ($(this).val() == "") {

            skip = false;

            var object = this;
            var thisObjectId = "#" + $(object).attr("id");

            var cssDisplay = $(".zip-city-other-place-holder").css("display");
            var objectValue = $(this).val()

            /* city control -->  
            
            If city control is on display and empty: validate
                
            city control <-- */

            if (thisObjectId == "#AddressControl_txtCity") {

                if (cssDisplay == "none") {

                    skip = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            /* state control --> 

            If state control is on display 
                
            -> It must be validated on submit 
            -> Skip EMPTY validation if state control has assigned value

            otherwise:

            skip state validation and skp EMPTY validation of state control

            */

            if (thisObjectId == "#AddressControl_txtState") {

                var status = IsCountryWithStates("#AddressControl_drpCountry");

                if (cssDisplay == "none" || status == "false") {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            // state control <--


            if (skip == false) {

                $(this).removeClass("current-object-on-focus");
                $(this).addClass("required-input");


                /* Points mouse cursor on the first input with no value to render bubble message */

                if (counter == 0) {

                    thisObjectId = "#" + $(this).attr("id");
                    $(this).addClass("current-object-on-focus");
                    $(this).focus();

                }

                formHasEmptyFields = true;
                counter++;
            }


        }


    });

  
    if (formHasEmptyFields) {

        $(thisObjectId).focus();
        return false;
    }

    if (goodForm) {

        // --> verify if CityStates dropdown is initialized

        var cityStates = $("#city-states").val();

        if (typeof (cityStates) == "undefined") {

            $("#AddressControl_txtPostal").addClass("invalid-postal-zero");

            var thisLeft = $("#AddressControl_txtPostal").offset().left;
            var thisTop  = $("#AddressControl_txtPostal").offset().top;

            $("#ise-message-tips").css("top", thisTop - 54);
            $("#ise-message-tips").css("left", thisLeft - 17);


            $("#ise-message").html(_GetCityStateEnterMessage);
            $("#ise-message-tips").fadeIn("slow");

            return false;

        } else {

            $("#AddressControl_txtPostal").removeClass("undefined-city-states").removeClass("invalid-postal-zero");
            $("#ise-message-tips").fadeOut("slow");
        }

        // <--

        var transact = "add-address";
        if (!addnew) transact = "update-address";


        ValidateAddressDetails(true, true, true, "", transact);  // --> see usercontrol.address.control.js 

    }

}

function TriggerAddressBookSubmitButton(addnew) {

    var _bCityStates = $("#city-states").val();

    if (_bCityStates != "other") {

        $("#txtCityStates").val(_bCityStates);

    } else {

        $("#txtCityStates").val($("#AddressControl_txtState").val() + ", " + $("#AddressControl_txtCity").val());
    }

    if (addnew) {
    
        ShowProcessMessage(_SavingAddressMessage, "error-summary", "save-address-loader", "save-address-button-place-holder");
        $("#btnNewAddress").click();
    
    } else {

        ShowProcessMessage(_UpdatingAddressMessage, "error-summary", "update-address-loader", "update-address-button-place-holder");
        $("#btnUpdateAddress").click();
    }
}