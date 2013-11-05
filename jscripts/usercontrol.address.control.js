var _AddressBestMatchFound = "";

$(document).ready(function () {

    RealTimAddressVerificationEventHandler();

    HideStateInputBoxForCountryWithState("AddressControl");
    HideStateInputBoxForCountryWithState("ShippingAddressControl");
    HideStateInputBoxForCountryWithState("BillingAddressControl");

    var param = new Object();
    param.countryCode = "";
    param.postalCode = "";
    param.stateCode = "";

    AjaxCallCommon("ActionService.asmx/GetCity", param, function () { }, function (error) { });

});

function RealTimAddressVerificationEventHandler() {

    $("#select-entered-address").click(function () {

        var values = _AddressBestMatchFound.split("::");

        var task = values[4];
        var onSubmit = values[5];
        var idPrefix = values[6];

        if (onSubmit) ISEAddressVerificationTaskSelector(task);

    });

    $("#select-matching-address").click(function () {

        var values = _AddressBestMatchFound.split("::");

        var nAddress = values[0];
        var nPostal = values[1];
        var nCity = values[2];
        var nState = values[3];
        var task = values[4];
        var onSubmit = values[5];

        var idPrefix = values[6];

        if (nAddress && nPostal && nCity && nState) {
            $("#" + idPrefix + "AddressControl_txtStreet").val(nAddress);
            $("#" + idPrefix + "AddressControl_txtPostal").val(nPostal);
            $("#" + idPrefix + "AddressControl_txtCity").val(nCity);
            $("#" + idPrefix + "AddressControl_txtState").val(nState);
        }

        if (onSubmit) ISEAddressVerificationTaskSelector(task);

    });
}

function RenderCityStates(excludeStateCode, focusOnControl, idPrefix) {

    var addressControlId   = "#" + idPrefix + "AddressControl_drpCountry";
    var postalCodeId       = "#" + idPrefix + "AddressControl_txtPostal";
    var stateCodeId        = "#" + idPrefix + "AddressControl_txtState";

    var cityStatesId       = "#city-states";
    var cityStateClassName = ".zip-city-other-place-holder";
    var enterPlaceHolderId = "#enter-postal-label-place-holder";

    if (idPrefix != "") {

        cityStatesId       = "#" + idPrefix.toLowerCase() + "-city-states";
        cityStateClassName = "." + idPrefix.toLowerCase() + "-zip-city-other-place-holder";
        enterPlaceHolderId = "#" + idPrefix.toLowerCase() + "-enter-postal-label-place-holder";
    }

    var citystateIsVisible = $(cityStateClassName).css("display");
    citystateIsVisible = citystateIsVisible.toLowerCase();

    if ($(postalCodeId).val() == "" && (citystateIsVisible != "none" || citystateIsVisible != "")) {

        $(cityStateClassName).fadeOut("Slow", function () {

            $(enterPlaceHolderId).html(_EnterPostalForCityState);
            $("#ise-message-tips").fadeOut("fast");

        });

        return false;
    }

    if (citystateIsVisible != "none" || citystateIsVisible == "") return false;

    var country = $(addressControlId).val()
    var postalCode = $(postalCodeId).val();
    var stateCode = $(stateCodeId).val();

    if (excludeStateCode) stateCode = "";

    if (IsPostalFormatInvalid(country, postalCode)) {

        $(postalCodeId).addClass('support-invalid-postal');
        $(enterPlaceHolderId).html(_EnterPostalForCityState);

        if (focusOnControl) $(postalCodeId).focus();

    } else {

        $(postalCodeId).removeClass('support-invalid-postal');
        SearchForCityAndState(country, _PostalCode, stateCode, focusOnControl, idPrefix);

    }

}

function SearchForCityAndState(country, postalCode, stateCode, focusOnControl, idPrefix) {

    var successFunction = function (result) {

        var addressControlId = "#" + idPrefix + "AddressControl_drpCountry";
        var postalCodeId = "#" + idPrefix + "AddressControl_txtPostal";
        var stateCodeId = "#" + idPrefix + "AddressControl_txtState";

        var cityStatesId = "city-states";
        var cityStateClassName = ".zip-city-other-place-holder";
        var enterPlaceHolderId = "#enter-postal-label-place-holder";

        if (idPrefix != "") {
            cityStatesId = idPrefix.toLowerCase() + "-city-states";
            cityStateClassName = "." + idPrefix.toLowerCase() + "-zip-city-other-place-holder";
            enterPlaceHolderId = "#" + idPrefix.toLowerCase() + "-enter-postal-label-place-holder";
        }

        if (result.d != "") {

            var renderHTML = "<select id='" + cityStatesId + "' class='light-style-input'>";
            renderHTML += result.d;
            renderHTML += "<option value='other'>" + _OtherOption + "</option>";
            renderHTML += "</select>";

            if (result.d.length > 0) {

                $(cityStateClassName).fadeOut("Slow", function () {

                    $(enterPlaceHolderId).html(renderHTML);

                });

            } else {

                $(enterPlaceHolderId).html(_EnterPostalForCityState);
                if (focusOnControl) $(postalCodeId).focus();
            }

            $("#" + cityStatesId + "").change(function () {

                if ($(this).val() == "other") {

                    $(this).fadeOut("Slow", function () {

                        $(cityStateClassName).fadeIn("Slow")

                    });

                    HideStateInputBoxForCountryWithState(idPrefix + "AddressControl");

                }
            });

            $(postalCodeId).removeClass("invalid-postal-zero");

        } else {

            $(enterPlaceHolderId).html(_EnterPostalForCityState);
            $(postalCodeId).addClass("invalid-postal-zero");

            if (focusOnControl) $(postalCodeId).focus();
        }
    };

    var errorFunction = function () { };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = stateCode;

    AjaxCallCommon("ActionService.asmx/GetCity", data, successFunction, errorFunction);
}

function IsCountryWithStates(addressControlId) {

    var thisClass = $(addressControlId).attr("class");
    var status = "";

    if (typeof (thisClass) != "undefined") {

        var classes = thisClass.split(" ");
        var countryStatesFlag = classes[0];
        var withStates = countryStatesFlag.split("-");
        var flag = withStates[$(addressControlId).prop("selectedIndex")];
        flag = flag.split("::");

        status = flag[0];
    }

    return status.toLowerCase();

}

function IsCountrySearchable(addressControlId) {

    var thisClass = $(addressControlId).attr("class");

    var classes = thisClass.split(" ");
    var countryStatesFlag = classes[0];
    var withStates = countryStatesFlag.split("-");
    var flag = withStates[$(addressControlId).prop("selectedIndex")];
    flag = flag.split("::");

    var status = flag[1];

    return status.toLowerCase();

}

function VerifyPostalCode(country, postalCode, focusOnControl, skipStateValidation, doSumbissionIfGood, postalCodeId, page) {

    if ($(postalCodeId).hasClass("skip")) {
        $(postalCodeId).removeClass("invalid-postal-zero");
        PageSubmitProcess(page);
        return true;
    }

    var successFunction = function (result) {
        if (result.d != false) {

            $(postalCodeId).removeClass("invalid-postal-zero");
            if (doSumbissionIfGood == true && IsAddressConrolBad() == 0) {
                PageSubmitProcess(page);
            }

        } else {

            $(postalCodeId).addClass("invalid-postal-zero");
            if (focusOnControl) $(postalCodeId).focus();

        }
    };

    var errorFunction = function () { };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = "";

    AjaxCallCommon("ActionService.asmx/IsPostalCodeValid", data, successFunction, errorFunction);

}

function VerifyStateCode(country, postalCode, stateCode, doSumbissionIfGood, page, stateCodeId) {

    var successFunction = function (result) {

        if (result.d == false) {

            $(stateCodeId).addClass("state-not-found");
            if (doSumbissionIfGood == true) {
                $(stateCodeId).addClass("current-object-on-focus");
                $(stateCodeId).focus();
            }

        } else {

            $(stateCodeId).removeClass("state-not-found");
            if (doSumbissionIfGood == true) PageSubmitProcess(page);
        }

        return result.d;
    };

    var errorFunction = function () { return 0 };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = stateCode;

    AjaxCallCommon("ActionService.asmx/IsStateCodeValid", data, successFunction, errorFunction);

}

function IsShippingStateCodeIsGood(country, postalCode, stateCode) {

    var successFunction = function (result) {
        if (result.d == "0") {
            $("#shipping-states-input").addClass("support-state-not-found");
            return false;
        } else {
            $("#shipping-states-input").removeClass("support-state-not-found");
            return true;
        }
    };

    var errorFunction = function () { return false; };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = stateCode;

    AjaxCallCommon("ActionService.asmx/GetCity", data, successFunction, errorFunction)

}

function GetCountryPostalFormats(country, postalCode) {

    var names = new Array();
    names.push("united states of america");

    var digits = new Array();
    digits.push("12345-6789");

    var index = 0;

    if (country.toLowerCase() != "united states of america") {
        return "free-form";
    }

    return digits[index];

}

function IsPostalFormatInvalid(country, postalCode) {

    var postalFormat = GetCountryPostalFormats(country, postalCode);
    var formats = postalFormat;

    formats = formats.split("-")

    if (formats.length > 0 && postalFormat != "free-form") {

        var postal = postalCode.split("-");
        _PostalCode = postal[0];

        if (postal.length > 1) {

            /*
                           
            Check if the user postal input number of elements separated by hypen(-) is the 
            same as your defined number of digits
                           
            */

            if (postal.length != formats.length) {

                return true;

            } else {

                /* 
                loops through your postal elements separated  by hypen(-)

                -> verify if each element in user postal has the same length with each element in your postal format
                              
                */

                for (var i = 0; i < postal.length; i++) {

                    var userPostalLength = postal[i].length;
                    var yourPostalLength = formats[i].length;

                    if ((userPostalLength != yourPostalLength) || userPostalLength == 0) {

                        return true;
                        break;

                    }

                }

            }


        } else {

            _PostalCode = postalCode;
            if (postalCode.length == 0 || postalCode.length != formats[0].length) return true;

        }


    } else {

        _PostalCode = postalCode;

    }

    return false;
}


function ValidateAddressDetails(focusOnControl, formSubmission, skipStateValidation, idPrefix, page) {

    /*
    Function definition:

    1. Validates address (billing or shipping <if IsUsedInShippingControl is set to TRUE>)
    2. If formSubmission is SET TO true does PageSubmitProcess after validating the Address
    3. Checks if Country is SEARCHABLE (if true do address validation else: jump to PageSubmitProcess())

    4. If postal is good and county is SEARCHABLE function does the following

    -> checks if cityState is equals to "other" and if skipStateValidation is set to TRUE: call VerifyPostalCode()
    -> if skipStateValidation is set to FALSE do VerifyStateCode
    -> if citystate not equals to "other" and formSubmission is set false calls RenderCityStates()
       
    */

    var addressControlId   = "#" + idPrefix + "AddressControl_drpCountry";
    var postalCodeId       = "#" + idPrefix + "AddressControl_txtPostal";
    var statesId           = "#" + idPrefix + "AddressControl_txtState";
    var stateCodeId        = "#" + idPrefix + "AddressControl_txtState";

    var county             = "#" + idPrefix + "AddressControl_txtCounty";

    var cityStatesId       = "#city-states";
    var cityStateClassName = ".zip-city-other-place-holder";
    var enterPlaceHolderId = "#enter-postal-label-place-holder";

    if (idPrefix != "") {

        cityStatesId       = "#" + idPrefix.toLowerCase() + "-city-states";
        cityStateClassName = "." + idPrefix.toLowerCase() + "-zip-city-other-place-holder";
        enterPlaceHolderId = "#" + idPrefix.toLowerCase() + "-enter-postal-label-place-holder";
    }

    var citystateIsVisible = $(cityStateClassName).css("display");
    citystateIsVisible = citystateIsVisible.toLowerCase();

    var country = $(addressControlId).val();
    var postalCode = $(postalCodeId).val();

    var postalIsGood = true;

    // check format
    if (IsCountrySearchable(addressControlId) == "true") {

        if (IsPostalFormatInvalid(country, postalCode)) {

            $(postalCodeId).addClass('invalid-postal'); 

            // hide citystates dropdown only if city and state inputs is hidden

            if (citystateIsVisible == "none") {
                $(enterPlaceHolderId).html(_EnterPostalForCityState);
            }

            if (focusOnControl) $(postalCodeId).focus();
            return false;
        }

    } else {

        PageSubmitProcess(page);
        return true;
    }

   
 
    if (postalIsGood) {

        /* 

        -> if CityState dropdown box is not on display and City and State inputs are hidden 
        -> select city state based on user postal inputs
                            
        otherwise:

        */

        _prevPostal = _currentPostal;
        _currentPostal = postalCode;

        var cityState = $(cityStatesId).val();

        if (cityState == "other") {

            if (skipStateValidation) {
               
                VerifyPostalCode(country, _PostalCode, focusOnControl, skipStateValidation, formSubmission, postalCodeId, page);

            } else {

                var stateCode = $(statesId).val();
                VerifyStateCode(country, _PostalCode, stateCode, true, page, stateCodeId);
            }


        } else {

            if ((citystateIsVisible == "none" && formSubmission == false) && (typeof (cityState) == "undefined" || (_prevPostal != _currentPostal))) {

                RenderCityStates(true, focusOnControl, idPrefix);

            }

            if (formSubmission == true) {

                VerifyPostalCode(country, _PostalCode, focusOnControl, skipStateValidation, formSubmission, postalCodeId, page);

            }

        }

    }

}

function IsAddressConrolBad() {

    var postalZero    = $(".invalid-postal-zero").length;
    var postalInvalid = $(".invalid-postal").length;
    var stateInvalid  = $(".state-not-found").length;
    var emptyFields   = $(".required-input").length;

    return postalZero + postalInvalid + stateInvalid + emptyFields;
}

function PageSubmitProcess(page) {

    switch (page) {
        case "case-form":

            var progress = ["errorSummary", "save-case-loader", "save-case-button-place-holder"];
            ISEAddressVerification(true, "case-form", "", false, progress);

            break;
        case "lead-form":

            var progress = ["errorSummary", "save-lead-loader", "save-lead-button-place-holder"];
            ISEAddressVerification(true, "lead-form", "", false, progress);

            break;
        case "add-address":

            var progress = ["errorSummary", "save-address-loader", "save-address-button-place-holder"];
            ISEAddressVerification(true, "add-address", "", false, progress);

            break;
        case "update-address":

            var progress = ["errorSummary", "update-address-loader", "update-address-button-place-holder"];
            ISEAddressVerification(true, "update-address", "", false, progress);

            break;
        case "shipping-calculator":
            
            DoSubmissionOfShippingCalculatorAction();

            break;
        default:
            break;
    }

}

function GetAddressVerificationGatewayError(result) {
    var test = result.d.split("[error]");
    if (test.length > 1) return $.trim(test[1]);
    return "";
}


function GetAddressBestMatch(address, country, postal, city, state, residenceType, onSubmit, task, idPrefix, billingANDshipping, progress) {

    if (_IsUPSFedExAddressVerification == "true") {

        if (residenceType == "" || typeof (residenceType) == "undefined") residenceType = "default-type";
        if (typeof (city) == "undefined") city = "";

        var jsonText = JSON.stringify({ address: address, country: country, postal: postal, city: city, state: state, residenceType: residenceType, billingANDshipping: billingANDshipping });

        ShowProcessMessage(_VerifyingAddressMessage, progress[0], progress[1], progress[2]);

        $.ajax({
            type: "POST",
            url: "ActionService.asmx/GetAddressBestMatch",
            data: jsonText,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {

                var gatewayResponseErrorMessage = GetAddressVerificationGatewayError(result);
                var isGatewayResponseError = false;

                if (gatewayResponseErrorMessage != "") {
                    isGatewayResponseError = true;
                }

                if (task == "create-account") {

                    // c = Current, cb = Current Billing; nb = New Billing; cs = Current Shipping; ns = New Shipping

                    var found_nbAddress = false;
                    var found_nsAddress = false;

                    var cAddress = address.split("+");
                    var cPostal = postal.split("+");
                    var cCity = city.split("+");
                    var cState = state.split("+");

                    // entered billing address

                    var cbAddress = $.trim(cAddress[0]);
                    var cbCity = $.trim(cCity[0]);
                    var cbState = $.trim(cState[0]);
                    var cbPostal = $.trim(cPostal[0]);

                    // entered shipping address

                    var csAddress = $.trim(cAddress[1]);
                    var csCity = $.trim(cCity[1]);
                    var csState = $.trim(cState[1]);
                    var csPostal = $.trim(cPostal[1]);

                    // suggested billing address 
                    var nbAddress = "";
                    var nbPostal = "";
                    var nbCity = "";
                    var nbState = "";

                    //suggested shipping address
                    var nsAddress = "";
                    var nsPostal = "";
                    var nsCity = "";
                    var nsState = "";

                    if (!isGatewayResponseError) {

                        var data = $.parseJSON(result.d);

                        // suggested billing address 
                        nbAddress = $.trim(data[0].Value);
                        nbPostal = $.trim(data[1].Value);
                        nbCity = $.trim(data[2].Value);
                        nbState = $.trim(data[3].Value);

                        //suggested shipping address
                        nsAddress = $.trim(data[5].Value);
                        nsPostal = $.trim(data[6].Value);
                        nsCity = $.trim(data[7].Value);
                        nsState = $.trim(data[8].Value);

                    }

                    if (cbAddress != nbAddress || cbCity != nbCity || cbState != nbState || cbPostal != nbPostal) found_nbAddress = true;
                    if (nbAddress == "" || nbCity == "" || nbState == "" || nbPostal == "") found_nbAddress = false;

                    if (csAddress != nsAddress || csCity != nsCity || csState != nsState || csPostal != nsPostal) found_nsAddress = true;
                    if (nsAddress == "" || nsCity == "" || nsState == "" || nsPostal == "") found_nsAddress = false;

                    if (!found_nbAddress && !found_nsAddress && !isGatewayResponseError) {

                        if (_CreateAccountFormIsGood) ISEAddressVerificationTaskSelector("create-account");

                    } else {

                        if (found_nbAddress == false) {

                            $("#n-billing-address").addClass("control-caption-disabled");
                            $("#n-billing-city-state-postal").addClass("control-caption-disabled");

                            $("#billing-address-suggested").addClass("control-caption-disabled");
                            $("#billing-address-suggested").attr("disabled", "disabled");

                        } else {

                            $("#n-billing-address").removeClass("control-caption-disabled");
                            $("#n-billing-city-state-postal").removeClass("control-caption-disabled");

                            $("#billing-address-suggested").removeClass("control-caption-disabled");
                            $("#billing-address-suggested").removeAttr("disabled");
                        }


                        $("#c-billing-address").html(cbAddress);
                        $("#c-billing-city-state-postal").html(cbCity + " " + cbState + " " + cbPostal);

                        $("#c-shipping-address").html(csAddress);
                        $("#c-shipping-city-state-postal").html(csCity + " " + csState + " " + csPostal);

                        var nBillingAddress = nbAddress;
                        var nBillingCityStatePostal = nbState + " " + nbCity + " " + nbPostal;

                        var nShippingAddress = nsAddress;
                        var nShippingCityStatePostal = nsCity + " " + nsState + " " + nsPostal;

                        $("#n-billing-city-state-postal").html(nBillingCityStatePostal);
                        $("#n-shipping-city-state-postal").html(nShippingCityStatePostal);

                        if (isGatewayResponseError) {

                            $("#n-billing-address").html(gatewayResponseErrorMessage).addClass("error-place-holder");
                            $("#n-shipping-address").html(gatewayResponseErrorMessage).addClass("error-place-holder");

                            $("#use-suggested-billing").addClass("display-none");
                            $("#use-suggested-shipping").addClass("display-none");

                            $("#use-my-billing").attr("checked", "checked");
                            $("#use-my-shipping").attr("checked", "checked");

                        } else {

                            $("#n-billing-address").html(nBillingAddress).removeClass("error-place-holder");
                            $("#n-shipping-address").html(nShippingAddress).removeClass("error-place-holder");

                            $("#use-suggested-billing").parent("li").removeClass("display-none");
                            $("#use-suggested-shipping").parent("li").removeClass("display-none");
                        }

                        if (found_nsAddress == false) {

                            $("#n-shipping-address").addClass("control-caption-disabled");
                            $("#n-shipping-city-state-postal").addClass("control-caption-disabled");

                            $("#shipping-address-suggested").addClass("control-caption-disabled");
                            $("#shipping-address-suggested").attr("disabled", "disabled");

                        } else {

                            $("#n-shipping-address").removeClass("control-caption-disabled");
                            $("#n-shipping-city-state-postal").removeClass("control-caption-disabled");

                            $("#shipping-address-suggested").removeClass("control-caption-disabled");
                            $("#shipping-address-suggested").removeAttr("disabled");
                        }


                        var copySuggestedAddress = function () {

                            var useSuggestedBillingAddress = false;
                            var useSuggestedShippingAddreess = false;

                            if ($("input[name='billing-address']:checked").attr("id") == "use-suggested-billing") useSuggestedBillingAddress = true;
                            if ($("input[name='shipping-address']:checked").attr("id") == "use-suggested-shipping") useSuggestedShippingAddreess = true;

                            if (useSuggestedBillingAddress && !isGatewayResponseError) {
                                UpdateAddressInfo("Billing", nbAddress, nbCity, nbState, nbPostal);
                            } else {
                                UpdateAddressInfo("Billing", cbAddress, cbCity, cbState, cbPostal);
                            }

                            if (useSuggestedShippingAddreess && !isGatewayResponseError) {
                                UpdateAddressInfo("Shipping", nsAddress, nsCity, nsState, nsPostal);
                            } else {
                                UpdateAddressInfo("Shipping", csAddress, csCity, csState, csPostal);
                            }

                            $("#billing-enter-postal-label-place-holder").html("<input type='hidden' id='billing-city-states' value='other'/>");
                            $("#billing-city-states").fadeOut("Slow", function () {
                                $(".billing-zip-city-other-place-holder").fadeIn("Slow");
                            });

                            $("#shipping-enter-postal-label-place-holder").html("<input type='hidden' id='shipping-city-states' value='other'/>");
                            $("#shipping-city-states").fadeOut("Slow", function () {
                                $(".shipping-zip-city-other-place-holder").fadeIn("Slow");
                            });

                            if (_CreateAccountFormIsGood) {
                                $("#ise-address-verification-for-create-account").dialog("destroy");
                                ISEAddressVerificationTaskSelector("create-account");
                            }

                        };

                        $("#submit-suggested-address").unbind("click");
                        $("#submit-suggested-address").click(function () {
                            copySuggestedAddress();
                        });

                        $("#ise-address-verification-for-create-account").dialog({
                            autoOpen: false,
                            width: 700,
                            modal: true,
                            resize: false,
                            closeOnEscape: false,
                            dialogClass: "no-close"
                        });

                        $("#ise-address-verification-for-create-account").dialog("open");

                    }

                } else {

                    var cAddress = $.trim(address);
                    var cPostal = $.trim(postal);
                    var cCity = $.trim(city);
                    var cState = $.trim(state);

                    var nAddress = "";
                    var nPostal = "";
                    var nCity = "";
                    var nState = "";

                    if (!isGatewayResponseError) {

                        var data = $.parseJSON(result.d);

                        nAddress = $.trim(data[0].Value);
                        nPostal = $.trim(data[1].Value);
                        nCity = $.trim(data[2].Value);
                        nState = $.trim(data[3].Value);
                    }

                    var found_MatchAddress = false;

                    if (cAddress != nAddress || cCity != nCity || cState != nState || cPostal != nPostal) found_MatchAddress = true;
                    if (nAddress == "" || nCity == "" || nState == "" || nPostal == "") found_MatchAddress = false;

                    if (found_MatchAddress || isGatewayResponseError) {

                        $("#c-address").html(address);
                        $("#c-city-state-postal").html(city + " " + state + " " + postal);

                        if (isGatewayResponseError) {
                            $("#n-address").html(gatewayResponseErrorMessage).addClass("error-place-holder");
                            $("#select-matching-address").addClass("display-none");
                        } else {
                            $("#n-address").html(nAddress).removeClass("error-place-holder");
                            $("#select-matching-address").removeClass("display-none");
                        }

                        $("#n-city-state-postal").html(nCity + " " + nState + " " + nPostal);

                        $("#ise-address-verification").dialog({
                            autoOpen: false,
                            width: 623,
                            modal: true,
                            resize: false,
                            closeOnEscape: false,
                            dialogClass: "no-close"
                        });

                        $("#ise-address-verification").dialog("open");

                    } else {

                        if (onSubmit) ISEAddressVerificationTaskSelector(task);
                    }

                    _AddressBestMatchFound = nAddress + "::" + nPostal + "::" + nCity + "::" + nState + "::" + task + "::" + onSubmit + "::" + idPrefix;

                }
            },
            fail: function (result) {

                ShowFailedMessage(result.d, progress[0], progress[1], progress[2]);
                return false;
            }
        });

    } else {

        ISEAddressVerificationTaskSelector(task);
    }
}

function ISEAddressVerification(onSubmit, task, idPrefix, billingANDshipping, progress) {

    _AddressBestMatchFound = "";

    if (!billingANDshipping) {

        var residenceTypeId    = "#ShippingAddressControl_drpType";
        var countryId          = "#" + idPrefix + "AddressControl_drpCountry";

        var cityStatesId       = "#city-states";
        var enterPlaceHolderId = "#enter-postal-label-place-holder";
        var cityStateClassName = ".zip-city-other-place-holder";

        if (idPrefix != "") {

            cityStatesId       = "#" + idPrefix.toLowerCase() + "-city-states";
            enterPlaceHolderId = "#" + idPrefix.toLowerCase() + "-enter-postal-label-place-holder";
            cityStateClassName = "." + idPrefix.toLowerCase() + "-zip-city-other-place-holder";

        }

        var addressId          = "#" + idPrefix + "AddressControl_txtStreet";
        var postalCodeId       = "#" + idPrefix + "AddressControl_txtPostal";
        var stateCodeId        = "#" + idPrefix + "AddressControl_txtState";
        var cityId             = "#" + idPrefix + "AddressControl_txtCity";
       
      
        var city               = "";
        var state              = "";

        var country = $(countryId).val();
        var address = $(addressId).val();
        var postal  = $(postalCodeId).val();

        var residenceType = "";

        if (residenceTypeId != "") residenceType = $(residenceTypeId).val();

        var cityState = $(cityStatesId).val();

        if (cityState == "other") {

            city = $(cityId).val();
            state = $(stateCodeId).val();

        } else {

            var str = cityState;
            str = str.split(",");

            if (str.length > 0) {

                state = str[0];
                city = str[1];

            } else {

                city = str[0];
                state = "";
            }
        }

        GetAddressBestMatch(address, country, postal, city, state, residenceType, onSubmit, task, idPrefix, false, progress);

    } else {

        var bCity = "";
        var bState = "";

        var bCountry = $("#BillingAddressControl_drpCountry").val();
        var bAddress = $("#BillingAddressControl_txtStreet").val();
        var bPostal  = $("#BillingAddressControl_txtPostal").val();

        var bCityState = $("#billing-city-states").val();

        if (bCityState == "other") {

            bCity  = $("#BillingAddressControl_txtCity").val();
            bState = $("#BillingAddressControl_txtState").val();

        } else {

            var str = bCityState;
            str = str.split(",");

            if (str.length > 0) {

                bState = str[0];
                bCity = str[1];

            } else {

                bCity = str[0];
                bState = "";
            }
        }

        var sCity      = "";
        var sState     = "";
        var sCountry   = "";
        var sAddress   = "";
        var sPostal    = "";

        if (typeof ($("#ShippingAddressControl_drpCountry").val()) != "undefined") {

            sCountry = $("#ShippingAddressControl_drpCountry").val();
            sAddress = $("#ShippingAddressControl_txtStreet").val();
            sPostal  = $("#ShippingAddressControl_txtPostal").val();

            var sCityState = $("#shipping-city-states").val();
            if (sCityState == "other") {

                sCity = $("#ShippingAddressControl_txtCity").val();
                sState = $("#ShippingAddressControl_txtState").val();

            } else {

                var str = sCityState;
                str = str.split(",");

                if (str.length > 0) {

                    sState = str[0];
                    sCity = str[1];

                } else {

                    sCity = str[0];
                    sState = "";
                }
            }
        }

        var address = bAddress + "+" + sAddress;
        var country = bCountry + "+" + sCountry;
        var postal  = bPostal + "+" + sPostal;
        var city    = bCity + "+" + sCity;
        var state   = bState + "+" + sState;

        GetAddressBestMatch(address, country, postal, city, state, residenceType, onSubmit, task, idPrefix, true, progress);

    }
}

function ISEAddressVerificationTaskSelector(task) {
    $("#ise-address-verification").dialog("destroy");
    switch (task) {
        case "one-page-checkout-step-1":
            Step1SaveShippingContact();       // <-- defined on jquery.onepage.checkout.js
            break;
        case "one-page-checkout-step-3":
            Step3SavePaymentsInfo();          // <-- defined on jquery.onepage.checkout.js
            break;
        case "case-form":
            TriggerCaseFormSubmitButton(); // <-- defined on case.customer.support.js
            break;
        case "lead-form":
            TriggerLeadFormSubmitButton(); // <-- defined on lead.form.js 
            break;
        case "create-account":
            TriggerCreateAccountSubmitButton();  // <-- defined on create.account.js
            break;
        case "add-address":
            TriggerAddressBookSubmitButton(true);
            break;
        case "update-address":
            TriggerAddressBookSubmitButton(false);
            break;
        case "submit-payment-form":
            SubmitPaymentForm(); // <-- defined on jquery.normal.checkout.js
            break;
        default:
            break;
    }
}

function ClearAddressControl() {

    $("#shipping-address-input").val("");
    $("#shipping-postal-code-input").val("");
    $("#shipping-city-input").val("");
    $("#shipping-states-input").val("");

    $("#shipping-address-input-label").removeClass("display-none");
    $("#shipping-postal-code-input-label").removeClass("display-none");
    $("#shipping-city-input-label").removeClass("display-none");
    $("#shipping-states-input-label").removeClass("display-none");

    $("#shipping-address-input-label").css("color", labelFadeInColor);
    $("#shipping-postal-code-input-label").css("color", labelFadeInColor);
    $("#shipping-city-input-label").css("color", labelFadeInColor);
    $("#shipping-states-input-label").css("color", labelFadeInColor);

    $("#shipping-address-input").removeClass("support-required-input");
    $("#shipping-city-input").removeClass("support-required-input");
    $("#shipping-states-input").removeClass("support-required-input");
    $("#shipping-postal-code-input").removeClass("support-required-input");

    $("#shipping-postal-code-input").removeClass("support-invalid-postal");
    $("#shipping-postal-code-input").removeClass("support-current-input-on-focus");

}

function UpdateAddressInfo(idPrefix, address, city, state, postal) {

    $("#ise-message-tips").fadeOut("slow");

    var addressId = "#" + idPrefix + "AddressControl_txtStreet";
    var cityId    = "#" + idPrefix + "AddressControl_txtCity";
    var stateId   = "#" + idPrefix + "AddressControl_txtState";
    var postalId  = "#" + idPrefix + "AddressControl_txtPostal";

    $(addressId).removeClass("required-input");
    $(cityId).removeClass("required-input");
    $(stateId).removeClass("required-input");

    $(postalId).removeClass("required-input");
    $(postalId).removeClass("invalid-postal");
    $(postalId).removeClass("current-input-on-focus");

    if (address != "") {

        $("#" + idPrefix + "AddressControl_lblStreet").addClass("display-none");
        $(addressId).val(address);
    }

    if (city != "") {

        $("#" + idPrefix + "AddressControl_lblCity").addClass("display-none");
        $(cityId).val(city);

    }

    if (state != "") {

        $("#" + idPrefix + "AddressControl_lblState").addClass("display-none");
        $(stateId).val(state);

    }

    if (postal != "") {

        $("#" + idPrefix + "AddressControl_lblPostal").addClass("display-none");
        $(postalId).val(postal);

    }

}

function _SplitAreaCodeAndPhone(areaCodeId, primaryPhoneId, phone) {

    var details = phone.split(")");
    var areaCode = "";
    var primary = "";

    if (details.length > 1) {

        areaCode = details[0];

        areaCode = areaCode.replace(")", "");
        areaCode = areaCode.replace("(", "");
        areaCode = areaCode.replace("+", "");

        primary = details[1];

    } else {

        var i = 1;

        details = phone.replace("+", "");

        if (phone.length >= 10) {

            primary = details.substring(3, phone.length);
            areaCode = details.substring(0, 3);

        } else {

            primary = details;
            $("#" + primaryPhoneId + "-label").removeClass("display-none");
        }
    }

    if (areaCode != "") {

        $("#" + areaCodeId[0]).val($.trim(areaCode));
        $("#" + areaCodeId[1]).addClass("display-none");

    }

    if (primary != "") {

        $("#" + primaryPhoneId[0]).val($.trim(primary));
        $("#" + primaryPhoneId[1]).addClass("display-none");

    }
}

function HideStateInputBoxForCountryWithState(controlId) {
    
    var withState = IsCountryWithStates("#" + controlId  + "_drpCountry");

    if (typeof (withState) != "undefined" && withState == "false") {

        $("#" + controlId + "_txtState").addClass("display-none");
        $("#" + controlId + "_lblState").addClass("display-none");

        $("#" + controlId + "_txtCity").addClass("city-width-if-no-state");

    } else {

        $("#" + controlId + "_txtState").removeClass("display-none");
        $("#" + controlId + "_txtCity").removeClass("city-width-if-no-state");
    }

}

