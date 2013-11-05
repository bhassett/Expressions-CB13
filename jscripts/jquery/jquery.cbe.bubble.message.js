(function ($) {

    var controlIdOnFocus = "";
    var labelIdOnFocus = "";

    var methods = {
        init: function (options) {

            var settings = $.extend({
                "input-id": "",
                "label-id": "",
                "input-mode": "normal",
                "address-type": "",
                "optional": false
            }, options);

            $(this).ISEBubbleMessage("initEventsListener", settings);

        },
        initEventsListener: function (settings) {

            $this = $("#" + settings["input-id"]);

            // -- IE Fix on label 
            $("#" + settings["label-id"]).unbind("click").click(function () {
                $(this).addClass("display-none");
                $("#" + settings["input-id"]).val(" ");
                $("#" + settings["input-id"]).trigger("focus");
            });
            // <--

            if ($this.val() != "") $("#" + settings["label-id"]).addClass("display-none");
            if (!settings["optional"]) $this.addClass("requires-validation");

            $this.focus(function () {

                if ($(this).hasClass("editable-content")) return false;

                var thisId = $(this).attr("id");
                if (!settings["optional"] && $(this).val() != " ") {

                    $(this).ISEBubbleMessage("checkPreviousControlOnFocus", settings);
                    $(this).ISEBubbleMessage("lookForAnErrorTag", { "input-id": thisId, "address-type": settings["address-type"] });
                }

                // -- IE Fix on label 
                if ($(this).val() == " ") {
                    $(this).val("");
                    $(this).ISEBubbleMessage("cleanInputControl", { "control-id": settings["input-id"], "input-mode": settings["input-mode"], "label-id": settings["label-id"] });
                }
                // <--

            });

            $this.blur(function () {

                if ($(this).hasClass("editable-content")) return false;

                if ($(this).val() == "") {
                    $("#" + settings["label-id"]).removeClass("display-none");
                    $("#" + settings['label-id']).css("color", labelFadeInColor);
                }

            });

            $this.keypress(function () {

                if ($(this).hasClass("editable-content")) return false;
                $(this).ISEBubbleMessage("cleanInputControl", { "control-id": settings["input-id"], "input-mode": settings["input-mode"], "label-id": settings["label-id"] });

            });


            $this.keydown(function (event) {

                if ($(this).val() == "") {
                    if ($this.hasClass("editable-content")) return false;
                    $(this).ISEBubbleMessage("cleanInputControl", { "control-id": settings["input-id"], "input-mode": settings["input-mode"], "label-id": settings["label-id"] });
                }

            });


            $this.mousedown(function (event) {

                if ($(this).val() == "") {
                    if ($this.hasClass("editable-content")) return false;
                    $(this).ISEBubbleMessage("cleanInputControl", { "control-id": settings["input-id"], "input-mode": settings["input-mode"], "label-id": settings["label-id"] });
                }

            });


        },
        checkPreviousControlOnFocus: function (settings) {

            $this = $("#" + settings["input-id"]);

            var objectOnfocus = $(".current-object-on-focus").attr("id");
            var objectType = _prevObjectType;
            _prevObjectType = settings["input-mode"];

            $(".requires-validation").each(function () { $(this).removeClass("current-object-on-focus"); });

            if (typeof (objectOnfocus) != "undefined") {

                if ($("#" + objectOnfocus).val() == "") {

                    $("#" + objectOnfocus).addClass("required-input");
                }

                var object = [objectOnfocus, objectType];
                $this.ISEBubbleMessage("validate", object);

            }

            $("#" + settings['label-id']).animate({ color: labelFadeOutColor }, 500);
            $("#" + settings["input-id"]).addClass("current-object-on-focus");

        },
        cleanInputControl: function (settings) {

            $("#" + settings["label-id"]).addClass("display-none");

            var $this = $("#" + settings["control-id"]);

            switch (settings['input-mode']) {

                case "email":

                    $this.removeClass("invalid-email");
                    $this.removeClass("email-duplicates");

                    break;
                case "billing-postal":

                    $this.removeClass("invalid-postal");
                    $this.removeClass("invalid-postal-zero");
                    $this.removeClass("invalid-postal-many");

                    break;
                case "shipping-postal":

                    $this.removeClass("invalid-postal");
                    $this.removeClass("invalid-postal-zero");
                    $this.removeClass("invalid-postal-many");

                    break;
                case "password":

                    $this.removeClass("password-not-match");
                    $this.removeClass("password-not-strong");
                    $this.removeClass("password-length-invalid");

                    break;
                case "password-confirmation":

                    $this.removeClass("password-not-match");
                    $this.removeClass("password-not-strong");

                    break;
                default:
                    break;
            }

            $this.removeClass("required-input");
            $("#ise-message-tips").fadeOut("slow");

        },
        lookForAnErrorTag: function (options) {

            var attributes = $.extend({
                "input-id": "",
                "address-type": ""
            }, options);

            var $this = $("#" + attributes["input-id"]);

            var thisClass = $this.attr("class");
            var classes = thisClass.split(" ");

            var leftPosition = $(this).offset().left;
            var leftDeduction = 17;
            var topPosition = $(this).offset().top;
            var topDeduction = 54;
            var message = "";
            var showTips = false;

            switch (classes[classes.length - 2]) {
                case "required-input":

                    message = _IncompleteMessage;
                    showTips = true;

                    break;
                case "invalid-email":

                    message = _InvalidEmailMessage;
                    showTips = true;

                    break;
                case "email-duplicates":

                    message = _DuplicateEmailMessage;
                    showTips = true;

                    break;
                case "invalid-postal":

                    leftDeduction = 17;
                    topDeduction = 72;
                    message = _InvalidPostalFormatMessage + "<div class='clear-both height-5'></div> Please click <a id='show-postal-listing-dialog-many' onClick='FindPostal(\"" + attributes["address-type"] + "\")' href='javascript:void(1);'>HERE</a> to select your postal code";
                    showTips = true;

                    break;
                case "state-not-found":

                    var addressType = attributes["address-type"];

                    switch (attributes["input-id"]) {
                        case "ShippingAddressControl_txtState":
                            addressType = "Shipping";
                            break;
                        case "BillingAddressControl_txtState":
                            addressType = "Billing";
                            break;
                        default:
                            break;
                    }

                    leftDeduction = 17;
                    topDeduction = 72;

                    message = _InvalidStateCodeMessage + "<div class='clear-both height-5'></div> Please click <a id='show-postal-listing-dialog-many' onClick='FindPostal(\"" + addressType + "\")' href='javascript:void(1);'>HERE</a> to select your postal code";
                    showTips = true;


                    break;
                case "invalid-postal-zero":

                    leftDeduction = 17;
                    topDeduction = 90;
                    message = _PostalNotFoundMessage + "<div class='clear-both height-12'></div> Please click <a id='show-postal-listing-dialog-many' onClick='FindPostal(\"" + attributes["address-type"] + "\")' href='javascript:void(1);'>HERE</a> to correct your postal code <div class='clear-both height-5'></div> or click <a href='javascript:void(1);' onClick='SkipValidationOnPostal(\"" + attributes["address-type"] + "\")'>HERE</a> to continue on using your entered postal code.";
                    showTips = true;

                    break;
                case "invalid-captcha":

                    message = _InvalidCaptchaMessage;
                    showTips = true;

                    break;
                case "password-not-match":

                    message = _PasswordNotMatch;
                    showTips = true;

                    break;
                case "password-not-strong":

                    if (_StrongPasswordMessage.length > 200) topDeduction = 88;

                    message = _StrongPasswordMessage;
                    showTips = true;

                    break;
                case "password-length-invalid":

                    message = "Password length is not valid";
                    showTips = true;

                    break;
                case "lead-duplicates":

                    message = _leadFormLeadDuplicatesMessage;
                    showTips = true;

                    break;
                default:
                    break;

            }

            if (showTips) {

                $this.ISEBubbleMessage("showTips",
                {
                    "top-position": topPosition,
                    "top-deduction": topDeduction,
                    "left-position": leftPosition,
                    "left-deduction": leftDeduction,
                    "message": message
                });

            }

        },
        showTips: function (options) {

            var attributes = $.extend({
                "top-position": 0,
                "top-deduction": 0,
                "left-position": 0,
                "left-deduction": 0,
                "message": ""
            }, options);

            $("#ise-message-tips").css("top", attributes["top-position"] - attributes["top-deduction"]);
            $("#ise-message-tips").css("left", attributes["left-position"] - attributes["left-deduction"]);

            $("#ise-message").html(attributes["message"]);
            $("#ise-message-tips").fadeIn("slow");

        },
        validate: function (object) {

            var $this = $("#" + object[0]);
            var skip = $this.hasClass("skip");

            switch (object[1]) {

                case "email":

                    var emailPattern = /(^[a-zA-Z0-9._-]+@[a-zA-Z0-9]+([.-]?[a-zA-Z0-9]+)?([\.]{1}[a-zA-Z]{2,4}){1,4}$)/;

                    if (!emailPattern.test($this.val())) {

                        $this.addClass("invalid-email");

                    } else {

                        $this.removeClass("invalid-email");
                    }

                    break;
                case "postal":

                    var country = $("#AddressControl_drpCountry").val();
                    var postal = $("#AddressControl_txtPostal").val();
                    var state = $("#AddressControl_txtState").val();

                    var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                    break;
                case "shipping-postal":

                    var country = $("#ShippingAddressControl_drpCountry").val();
                    var postal = $("#ShippingAddressControl_txtPostal").val();
                    var state = $("#ShippingAddressControl_txtState").val();

                    var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                    break;

                case "billing-postal":

                    var country = $("#BillingAddressControl_drpCountry").val();
                    var postal = $("#BillingAddressControl_txtPostal").val();
                    var state = $("#BillingAddressControl_txtState").val();

                    var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                    break;
                case "state":

                    if (!skip) {

                        var country = $("#AddressControl_drpCountry").val();
                        var postal = $("#AddressControl_txtPostal").val();
                        var state = $("#AddressControl_txtState").val();

                        var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                        if (!formatIsInvalid) {
                            $this.ISEAddressFinder("verifyStateCode",
                            {
                                "country-id": "#AddressControl_drpCountry",
                                "postal-id": "#AddressControl_txtPostal",
                                "city-id": "#AddressControl_txtCity",
                                "state-id": "#AddressControl_txtState",
                                "city-state-place-holder": ".zip-city-other-place-holder",
                                "enter-postal-label-place-holder": "#enter-postal-label-place-holder",
                                "city-states-id": "city-states"
                            });
                        }
                    }

                    break;
                case "shipping-state":

                    if (!skip) {
                        var country = $("#ShippingAddressControl_drpCountry").val();
                        var postal = $("#ShippingAddressControl_txtPostal").val();
                        var state = $("#ShippingAddressControl_txtState").val();

                        var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                        if (!formatIsInvalid) {
                            $this.ISEAddressFinder("verifyStateCode",
                            {
                                "country-id": "#ShippingAddressControl_drpCountry",
                                "postal-id": "#ShippingAddressControl_txtPostal",
                                "city-id": "#ShippingAddressControl_txtCity",
                                "state-id": "#ShippingAddressControl_txtState",
                                "city-state-place-holder": ".shipping-zip-city-other-place-holder",
                                "enter-postal-label-place-holder": "#shipping-enter-postal-label-place-holder",
                                "city-states-id": "shipping-city-states"
                            });
                        }
                    }

                    break;
                case "billing-state":

                    if (!skip) {
                        var country = $("#BillingAddressControl_drpCountry").val();
                        var postal = $("#BillingAddressControl_txtPostal").val();
                        var state = $("#BillingAddressControl_txtState").val();

                        var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });

                        if (!formatIsInvalid) {
                            $this.ISEAddressFinder("verifyStateCode", {
                                "country-id": "#BillingAddressControl_drpCountry",
                                "postal-id": "#BillingAddressControl_txtPostal",
                                "city-id": "#BillingAddressControl_txtCity",
                                "state-id": "#BillingAddressControl_txtState",
                                "city-state-place-holder": ".billing-zip-city-other-place-holder",
                                "enter-postal-label-place-holder": "#billing-enter-postal-label-place-holder",
                                "city-states-id": "billing-city-states"
                            });
                        }
                    }

                    break;

                case "password":
                    DoPasswordValidation("#ProfileControl_txtPassword");
                    break;
                case "password-confirmation":

                    if ($("#ProfileControl_txtPassword").val() != $("#ProfileControl_txtConfirmPassword").val()) {

                        $("#ProfileControl_txtPassword").addClass("password-not-match");
                        $("#ProfileControl_txtConfirmPassword").addClass("password-not-match");

                    } else {

                        $("#ProfileControl_txtPassword").removeClass("password-not-match");
                        $("#ProfileControl_txtConfirmPassword").removeClass("password-not-match");
                    }

                    break;

                default:

                    break;
            }

        }
    };

    $.fn.ISEBubbleMessage = function (method) {

        if (methods[method]) {

            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));

        } else if (typeof method === 'object' || !method) {

            return methods.init.apply(this, arguments);

        } else {

            $.error('Method ' + method + ' does not exist on jQuery.tooltip');

        }

    };
})(jQuery);

function IsEmailGood(email) {
    var emailPattern = /(^[a-zA-Z0-9._-]+@[a-zA-Z0-9]+([.-]?[a-zA-Z0-9]+)?([\.]{1}[a-zA-Z]{2,4}){1,4}$)/;
    return emailPattern.test(email);
}

function VerifyCaptcha(args) {

    /* -> This function looks into ActionService -> CreateLeadTaskController with task validate-captca 
    -> If mismatch add class to captcha control to indicates its status
    */

    var list = [args[0]];
    var jsonText = JSON.stringify({ list: list, task: "validate-captcha" });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/CreateLeadTaskController",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result.d == "match") {

                $("#support-security-code").removeClass("support-invalid-captcha");

            } else {


                $("#support-security-code").addClass("support-invalid-captcha");

            }

        },
        fail: function (result) {}
    });

}

function ShowProcessMessage(Message, errorPlaceHolderID, messageLoaderId, buttonPlaceHolderId) {

    var thisProcessStringResource = Message;
    var lMessage = "<div style='float:left;width:12px;'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>" + thisProcessStringResource + "</div>";

    $("#" + buttonPlaceHolderId).css("display", "none");

    $("#" + messageLoaderId).html("<div style='float:right' class='OPCLoadStep'>" + lMessage + "</div>");
    $("#" + messageLoaderId).fadeIn("slow");

    $("#" + errorPlaceHolderID).html("");
    $("#" + errorPlaceHolderID).fadeOut("slow");

}

function ShowFailedMessage(message, errorPlaceHolderID, messageLoaderId, buttonPlaceHolderId) {

    if (message == "") {

        $("#" + errorPlaceHolderID).removeClass("error-place-holder");

    } else {

        $("#" + errorPlaceHolderID).addClass("error-place-holder");

    }

    $("#" + errorPlaceHolderID).html(message);
    $("#" + errorPlaceHolderID).fadeIn("slow", function () {

        $("#" + messageLoaderId).fadeOut("fast", function () {

            $("#" + buttonPlaceHolderId).fadeIn("fast");

        });

    });


}


function PopulateResources(config, resources) {

    var lst = $.parseJSON(resources);

    for (var i = 0; i < lst.length; i++) {

        if (config) {

            ise.Configuration.registerConfig(lst[i].Key, lst[i].Value);

        } else {

            ise.StringResource.registerString(lst[i].Key, lst[i].Value);

        }

    }
}

function InitPageAppConfigs(page) {

    _IsCaptchaRequired                     = ise.Configuration.getConfigValue("SecurityCodeRequiredOnCreateAccount");
    _IsVatEnablead                         = ise.Configuration.getConfigValue("VAT.Enabled");
    _IsShowTaxFieldOnRegistration          = ise.Configuration.getConfigValue("VAT.ShowTaxFieldOnRegistration");
    _IsAllowShippingAddress                = ise.Configuration.getConfigValue("AllowShipToDifferentThanBillTo");
    _IsAge13Required                       = ise.Configuration.getConfigValue("RequireOver13Checked");
    _IsAllowDuplicateCustomer              = ise.Configuration.getConfigValue("AllowCustomerDuplicateEMailAddresses");
    _IsStrongPassword                      = ise.Configuration.getConfigValue("UseStrongPwd");
    _IsUPSFedExAddressVerification         = ise.Configuration.getConfigValue("UseShippingAddressVerification");
    _CustomerSPExpression                  = ise.Configuration.getConfigValue("CustomerPwdValidator");
    _IsRequireTermsAndConditionsAtCheckout = ise.Configuration.getConfigValue("RequireTermsAndConditionsAtCheckout");

    switch (page) {
        case "create-account":

            InitSignUpFormOnLoad();

            break;
        case "edit-profile":
            
            InitProfileFormOnLoad();

            break;
        default:
            break;
    }

}

function ShowBubbleTips(id, message) {

    var thisLeft = $(id).offset().left;
    var thisTop  = $(id).offset().top;

    $("#ise-message-tips").css("top", thisTop - 54);
    $("#ise-message-tips").css("left", thisLeft - 17);

    $("#ise-message").html(message);
    $("#ise-message-tips").fadeIn("slow");

    $(id).addClass();

}


function GetAppConfigs(page) {

    var jsonText = JSON.stringify({ "configsFor": page });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetAppConfigs",
        dataType: "json",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        success: function (result) {

            PopulateResources(true, result.d);
            InitPageAppConfigs(page);

        },
        error: function (result) {

            console.log(result.d);

        }
    });
}


function GetStringResources(type, includeBubbleTipsMessages) {

    var keys = new Array();

    switch (type) {
        case "lead-form":

            keys.push("leadform.aspx.24");
            keys.push("createaccount.aspx.94");
            keys.push("leadform.aspx.20");
            keys.push("createaccount.aspx.81");

            break;
        case "customer-support":

            keys.push("customersupport.aspx.25");
            keys.push("createaccount.aspx.81");
          
            break;

        case "create-account":

            keys.push("createaccount.aspx.28");
            keys.push("createaccount.aspx.52");
            keys.push("createaccount.aspx.79");
            keys.push("createaccount.aspx.80");
            keys.push("createaccount.aspx.81");
            keys.push("createaccount.aspx.82");
            keys.push("createaccount.aspx.94");
            keys.push("createaccount.aspx.120");
            keys.push("createaccount.aspx.123");
            keys.push("createaccount.aspx.125");
            keys.push("selectaddress.aspx.6");
            keys.push("selectaddress.aspx.12");
            keys.push("selectaddress.aspx.13");
           

            break;
        case "edit-profile":

            keys.push("createaccount.aspx.81");
            keys.push("createaccount.aspx.94");

            keys.push("createaccount.aspx.52");
            keys.push("createaccount.aspx.120");

            keys.push("createaccount.aspx.121");
            keys.push("createaccount.aspx.122");

            keys.push("selectaddress.aspx.6");
            keys.push("selectaddress.aspx.12");
            keys.push("selectaddress.aspx.13");

            break;
        case "address":

            keys.push("selectaddress.aspx.8");
            keys.push("selectaddress.aspx.9");

            break;
        case "one-page-checkout":

            keys.push("createaccount.aspx.94");
            keys.push("createaccount.aspx.123");

            keys.push("checkout1.aspx.9");
            keys.push("checkout1.aspx.45");
            keys.push("checkout1.aspx.46");
            keys.push("checkout1.aspx.47");
            keys.push("checkout1.aspx.48");

            keys.push("selectaddress.aspx.6");
            keys.push("checkoutpayment.aspx.5");

            keys.push("checkoutshipping.aspx.9");
            keys.push("checkoutshipping.aspx.10");
            keys.push("checkoutshipping.aspx.11");
            keys.push("checkoutshipping.aspx.12");

            break;
        case "case-history":

            keys.push("customersupport.aspx.44");
            keys.push("customersupport.aspx.45");
            keys.push("customersupport.aspx.46");
            keys.push("customersupport.aspx.47");
            keys.push("customersupport.aspx.48");

            break;
        default: break;
    }

    if (includeBubbleTipsMessages) {

        keys.push("customersupport.aspx.15");
        keys.push("customersupport.aspx.16");
        keys.push("customersupport.aspx.17");
        keys.push("customersupport.aspx.18");
        keys.push("customersupport.aspx.23");
        keys.push("customersupport.aspx.24");
        keys.push("customersupport.aspx.38");
        keys.push("customersupport.aspx.39");
        keys.push("customersupport.aspx.40");
        keys.push("customersupport.aspx.41");


        keys.push("selectaddress.aspx.7");
        keys.push("selectaddress.aspx.10");
        keys.push("selectaddress.aspx.11");

    }

    var jsonText = jsonText = JSON.stringify({ keys: keys });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetStringResources",
        dataType: "json",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        success: function (result) {

            PopulateResources(false, result.d);
            SetContsGlobalVariableValues(type, includeBubbleTipsMessages);

        },
        error: function (result) {

            console.log(result.d);

        }
    });

}

function SetContsGlobalVariableValues(type, includeBubbleTipsMessages) {

    if (includeBubbleTipsMessages) {

        _IncompleteMessage          = ise.StringResource.getString("customersupport.aspx.15");
        _InvalidEmailMessage        = ise.StringResource.getString("customersupport.aspx.16");
        _PostalNotFoundMessage      = ise.StringResource.getString("customersupport.aspx.17");
        _InvalidCaptchaMessage      = ise.StringResource.getString("customersupport.aspx.18");
        _PostalMoreMessage          = ise.StringResource.getString("customersupport.aspx.23");
        _DownloadingStatesMessage   = ise.StringResource.getString("customersupport.aspx.24");
        _InvalidPostalFormatMessage = ise.StringResource.getString("customersupport.aspx.38");
        _InvalidStateCodeMessage    = ise.StringResource.getString("customersupport.aspx.39");
        _EnterPostalForCityState    = ise.StringResource.getString("customersupport.aspx.40");
        _OtherOption                = ise.StringResource.getString("customersupport.aspx.41");

        _SearchingPostalMessage     = ise.StringResource.getString("selectaddress.aspx.7");
        _VerifyingAddressMessage    = ise.StringResource.getString("selectaddress.aspx.10");
        _GetCityStateEnterMessage   = ise.StringResource.getString("selectaddress.aspx.11");

    }

    switch (type) {
        case "lead-form":

            _leadFormLeadDuplicatesMessage = ise.StringResource.getString("leadform.aspx.20");
            _SavingLeadMessage             = ise.StringResource.getString("leadform.aspx.24");
            _DuplicateEmailMessage         = ise.StringResource.getString("createaccount.aspx.94");
            _SalutationDefaultValue        = ise.StringResource.getString("createaccount.aspx.81");

            break;
        case "customer-support":

            _SendingCaseMessage           = ise.StringResource.getString("customersupport.aspx.25");
            _SalutationDefaultValue       = ise.StringResource.getString("createaccount.aspx.81");

            break;
        case "create-account":

            _DuplicateEmailMessage       = ise.StringResource.getString("createaccount.aspx.94");
            _SalutationDefaultValue      = ise.StringResource.getString("createaccount.aspx.81");
            _PasswordNotMatch            = ise.StringResource.getString("createaccount.aspx.52");
            _WholeSale                   = ise.StringResource.getString("createaccount.aspx.79");
            _Retail                      = ise.StringResource.getString("createaccount.aspx.80");
            _ChooseBusinessType          = ise.StringResource.getString("createaccount.aspx.82");
            _PasswordLengthIsNotValid    = ise.StringResource.getString("createaccount.aspx.120");
            _Over13Message               = ise.StringResource.getString("createaccount.aspx.123");
            _SavingAccountMessage        = ise.StringResource.getString("createaccount.aspx.125");

            _addressTypeCaption          = ise.StringResource.getString("selectaddress.aspx.6");
            _SelectAddressTypeMessage    = ise.StringResource.getString("selectaddress.aspx.12");
            _SelectBusinessTypeMessage   = ise.StringResource.getString("selectaddress.aspx.13");
            _StrongPasswordMessage       = ise.StringResource.getString("createaccount.aspx.28");


            break;
        case "edit-profile":

            _SalutationDefaultValue      = ise.StringResource.getString("createaccount.aspx.81");
            _DuplicateEmailMessage       = ise.StringResource.getString("createaccount.aspx.94");
           
            _PasswordNotMatch            = ise.StringResource.getString("createaccount.aspx.52");
            _PasswordLengthIsNotValid    = ise.StringResource.getString("createaccount.aspx.120");

            _UpdatingProfileMessage      = ise.StringResource.getString("createaccount.aspx.121"); 
            _RefreshingProfileMessage    = ise.StringResource.getString("createaccount.aspx.122"); 

            _addressTypeCaption          = ise.StringResource.getString("selectaddress.aspx.6");
            _SelectAddressTypeMessage    = ise.StringResource.getString("selectaddress.aspx.12");
            _SelectBusinessTypeMessage   = ise.StringResource.getString("selectaddress.aspx.13");

            break;

        case "address":

            _SavingAddressMessage        = ise.StringResource.getString("selectaddress.aspx.8"); 
            _UpdatingAddressMessage      = ise.StringResource.getString("selectaddress.aspx.9");

            break;
        case "one-page-checkout":

            _addressTypeCaption          = ise.StringResource.getString("selectaddress.aspx.6");

            _DuplicateEmailMessage       = ise.StringResource.getString("createaccount.aspx.94");
            _Over13Message               = ise.StringResource.getString("createaccount.aspx.123");

            _NoShippingMethodSelectedMessage = ise.StringResource.getString("checkout1.aspx.9");
            _SavingShippingMethodMessage = ise.StringResource.getString("checkout1.aspx.45");
            _SavingShippingInfoMessage   = ise.StringResource.getString("checkout1.aspx.46");
            _VerifyingBillingInfoMessage = ise.StringResource.getString("checkout1.aspx.47");
            _ProcessingOrderMessage      = ise.StringResource.getString("checkout1.aspx.48");
            _TermAndConditionMessage     = ise.StringResource.getString("checkoutpayment.aspx.5");


            _SavedCreditCardAs          = ise.StringResource.getString("checkoutpayment.aspx.13");
            _SavedThisCreditCardInfo    = ise.StringResource.getString("checkoutpayment.aspx.14");

            break;
        case "case-history":

            _ActivityCode               = ise.StringResource.getString("customersupport.aspx.44");
            _Priority                   = ise.StringResource.getString("customersupport.aspx.45");
            _AssignedTo                 = ise.StringResource.getString("customersupport.aspx.46");
            _Problem                    = ise.StringResource.getString("customersupport.aspx.47");
            _Solution                   = ise.StringResource.getString("customersupport.aspx.48");
                
            break;
        default: break;
    }

}