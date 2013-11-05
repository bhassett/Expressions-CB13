var captchaIsGood       = false;
var counter             = 1;

$("document").ready(function () {

    var str = $("#country-not-required-postal").html();
    var str = str.trim();
    str = str.split(",");

    var countriesNotRequiresPostal = new Array();
    var country = "";

    for (i = 0; i < str.length; i++) {

        country = str[i];
        countriesNotRequiresPostal.push(country.trim());

    }

    var defaultCountry = $("#LF_Country").val();

    if (defaultCountry != "") {

        var cityId = "txtCityLF";
        var postalId = "txtPostalLF";

        var thisClass = $("#LF_Country").attr("class");
        var classes = thisClass.split(" ");
        var countryStatesFlag = classes[0];
        var withStates = countryStatesFlag.split("-");
        var flag = withStates[0];

        flag = flag.split("::");

        var status = flag[0];

        if (status == "True") {

            PopulateStates(defaultCountry);
            $("#state-options-container").fadeIn("slow");

            $("#" + postalId).attr("id", "LF_WithStatePostalCode");
            $("#" + cityId).attr("id", "LF_WithStateCity");

        } else {

            $("#state-options-container").fadeOut("slow");

            $("#" + postalId).attr("id", "LF_WithoutStatePostalCode");
            $("#" + cityId).attr("id", "LF_WithoutStateCity");

        }

        var inArray = $.inArray(defaultCountry, countriesNotRequiresPostal);

        if (inArray > -1) {

            $("#postal-asterisk").html("");

        } else {

            $("#postal-asterisk").html("*");

        }

    }

    $("#txtAreaCodeLF").format({ type: "phone-number", autofix: true });


    $("#LF_WithStateState").change(function () {

        resetPostalCity();

    });

    $("#LF_Country").change(function () {

        resetPostalCity();

        var selectedIndex = $('option:selected', this).index();

        var selected = $("#LF_Country").val();

        var thisClass = $(this).attr("class");
        var classes = thisClass.split(" ");
        var countryStatesFlag = classes[0];
        var withStates = countryStatesFlag.split("-");
        var flag = withStates[selectedIndex];

        flag = flag.split("::");

        var status = flag[0];

        if (status == "True") {

            PopulateStates(selected);
            $("#state-options-container").fadeIn("slow");

            $("").attr("id", postalId + "_WithStatePostalCode");

            $("#LF_WithoutStatePostalCode").attr("id", "LF_WithStatePostalCode");
            $("#LF_WithoutStateCity").attr("id", "LF_WithStateCity");

        } else {

            $("#state-options-container").fadeOut("slow");
            $("#LF_WithStateState").html("<option></option>");

            $("#LF_WithStatePostalCode").attr("id", "LF_WithoutStatePostalCode");
            $("#LF_WithStateCity").attr("id", "LF_WithoutStateCity");


        }

        var inArray = $.inArray(selected, countriesNotRequiresPostal);

        if (inArray > -1) {

            $("#postal-asterisk").html("");

        } else {

            $("#postal-asterisk").html("*");

        }

    });

    $("#btnSubmitLF").click(function () {

        var error = "";

        var requiredFieldsAllGood = true;
        var emailIsGood = true;

        // get values of lead form input controls

        var optionNoDefault = $("#option-no-default-value").html(); //<-- no default value assigned to dropdown/select box is rendered from string resource: createaccount.aspx.81
        optionNoDefault = $.trim(optionNoDefault);

        var salutation = $("#drpLstSalutation").val();

        if (salutation == optionNoDefault) salutation = "";

        var firstName = $("#txtFirstNameLF").val();
        var middleName = $("#txtMiddleNameLF").val();
        var lastName = $("#txtLastNameLF").val();

        var suffix = $("#drpLstSuffix").val();

        if (suffix == optionNoDefault) suffix = "";

        var email = $("#txtEmailLF").val();
        var country = $("#LF_Country").val();
        var states = $("#LF_WithStateState").val();
        var areaCode = $("#txtAreaCodeLF").val();

        var selectedIndex = $('option:selected', "#LF_Country").index();

        var thisClass = $("#LF_Country").attr("class");
        var classes = thisClass.split(" ");
        var countryStatesFlag = classes[0];
        var withStates = countryStatesFlag.split("-");
        var flag = withStates[selectedIndex];

        flag = flag.split("::");

        var thisStatus = flag[0];

        if (thisStatus == "True") {

            thisCityId = "#LF_WithStateCity";
            thisPostalId = "#LF_WithStatePostalCode";

        } else {

            thisCityId = "#LF_WithoutStateCity";
            thisPostalId = "#LF_WithoutStatePostalCode";

        }

        var city = $(thisCityId).val();
        var postal = $(thisPostalId).val();

        var message = $("#txtMessageLF").val();
        var captcha = $("#txtCaptcha").val();

        var skinId = $("#skin-id").html();
        skinId = $.trim(skinId);

        var localeSettiings = $("#local-settings").html();
        localeSettiings = $.trim(localeSettiings);

        // --> check for empty field(s) that is/are required

        if (isEmpty(firstName)) requiredFieldsAllGood = false;
        if (isEmpty(middleName)) requiredFieldsAllGood = false;
        if (isEmpty(lastName)) requiredFieldsAllGood = false;
        if (isEmpty(email)) requiredFieldsAllGood = false;
        if (isEmpty(areaCode)) requiredFieldsAllGood = false;
        if (isEmpty(message)) requiredFieldsAllGood = false;
        if (isEmpty(city)) requiredFieldsAllGood = false;
        if (isEmpty(country)) requiredFieldsAllGood = false;

        // <--

        if (isEmpty(states) && thisStatus == "True") requiredFieldsAllGood = false;  // --> only require states if WITHSTATE status is TRUE

        var inArray = $.inArray(country, countriesNotRequiresPostal);

        var postalIsRequired = false;
        if (inArray == -1) postalIsRequired = true;

        if (isEmpty(postal) && inArray == -1) {

            requiredFieldsAllGood = false;   // --> only require postal if country selected is not in App Config: PostalCodeNotRequiredCountries

        }

        if (requiredFieldsAllGood) {

            resetLFNotice();

            emailIsGood = validateEmail(email);

            if (!emailIsGood) {

                error = $("#email-error").html();
                error = $.trim(error);

                $("#lead-form-tips").removeClass("success");

                $("#lead-form-tips").addClass("bad-form");
                $("#lead-form-tips").html(error);


            } else {

                resetLFNotice();

            }


        } else {

            error = $("#required-error").html();
            error = $.trim(error);

            $("#lead-form-tips").removeClass("success");

            $("#lead-form-tips").addClass("bad-form");
            $("#lead-form-tips").html(error);

        }

        var invalidAddress = revalidatePostalCity(thisPostalId, postalIsRequired);

        if (requiredFieldsAllGood && emailIsGood && invalidAddress == 0) {

            var args = new Array();

            args[0] = captcha;
            args[1] = salutation;
            args[2] = firstName;
            args[3] = middleName;
            args[4] = lastName;
            args[5] = suffix;
            args[6] = email;
            args[7] = country;
            args[8] = states;
            args[9] = city;
            args[10] = areaCode;
            args[11] = message;
            args[12] = skinId;
            args[13] = localeSettiings;
            args[14] = postal;

            DoCreateLead(args);

        }

    });


    $("#captcha-refresh-button").click(function () {

        counter++;
        $("#captcha").attr("src", "Captcha.ashx?id=" + counter);

    });


});

function resetLFNotice() {

    var thisProcessStringResource = $("#reset-lead-form-notice").html();

    $("#lead-form-tips").removeClass("bad-form");
    $("#lead-form-tips").removeClass("success");
    $("#lead-form-tips").html(thisProcessStringResource);


}

function isEmpty(val) {


    var isEmpty = false;

    if (val == "" || val == null) {

        isEmpty = true;

    }

    return isEmpty;
}


function validateEmail(email) {

    var emailPattern = /(^[a-zA-Z0-9._-]+@[a-zA-Z0-9]+([.-]?[a-zA-Z0-9]+)?([\.]{1}[a-zA-Z]{2,4}){1,4}$)/;

    return emailPattern.test(email);
}


function DoCreateLead(args) {

    var thisProcessStringResource = $("#do-create-request-status").html();

    $("#lead-form-tips").removeClass("bad-form");
    $("#lead-form-tips").html("<div style='float:left'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>" + thisProcessStringResource + "</div>");

     var list     = [args[0]];
     var jsonText = JSON.stringify({ list: list, task: "validate-captcha" });

     $.ajax({
         type: "POST",
         url: "ActionService.asmx/CreateLeadTaskController",
         data: jsonText,
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: function (result) {

             if (result.d == "match") {

                saveLead(args);

             } else {

                 var error = $("#captcha-error").html();
                 error = $.trim(error);

                 $("#lead-form-tips").addClass("bad-form");
                 $("#lead-form-tips").html(error);

             }

         },
         fail: function (result) {


             $("#lead-form-tips").addClass("bad-form");
             $("#lead-form-tips").html(result.d);

         }
     });


}

function saveLead(details) {

    var thisProcessStringResource = $("#save-lead-request-status").html();

    var salutation    = details[1];
    var firstName     = details[2];
    var middleName    = details[3];
    var lastName      = details[4];
    var suffix        = details[5];
    var email         = details[6];
    var country       = details[7];
    var states        = details[8];
    var city          = details[9];
    var phone         = details[10];
    var message       = details[11];
    var skinId        = details[12];
    var localSettings = details[13];
    var postalCode    = details[14];

    $("#lead-form-tips").removeClass("bad-form");
    $("#lead-form-tips").html("<div style='float:left'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>" + thisProcessStringResource + "</div>");

    var list = [salutation, firstName, middleName, lastName, suffix, email, country, states, city, phone, message, skinId, localSettings, postalCode];

    var jsonText = JSON.stringify({ list: list, task: "save-lead" });


    $.ajax({
        type: "POST",
        url: "ActionService.asmx/CreateLeadTaskController",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            switch (result.d) {
                case "True":

                    var success = $("#new-lead-created").html()

                    $("#lead-form-tips").removeClass("bad-form");
                    $("#lead-form-tips").addClass("success");

                    $("#lead-form-tips").html(success);


                    $("#lead-form-container").fadeOut("slow");

                    $("#hidden-div").css("display", "block");
                    $("#lead-form-thankyou").fadeIn("slow");

                    clearLeadForm();

                    break;
                case "EmailHasDuplicates":

                    error = $("#email-duplicate-error").html();
                    error = $.trim(error);

                    $("#lead-form-tips").addClass("bad-form");
                    $("#lead-form-tips").html(error);

                    break;
                case "LeadHasDuplicates":

                    error = $("#lead-duplicate-error").html();
                    error = $.trim(error);

                    $("#lead-form-tips").addClass("bad-form");
                    $("#lead-form-tips").html(error);

                    break;
                default:

                    $("#lead-form-tips").addClass("bad-form");
                    $("#lead-form-tips").html(result.d);

                    break;
            }


        },
        fail: function (result) {

            $("#lead-form-tips").addClass("bad-form");
            $("#lead-form-tips").html(result.d);

        }
    });
}


function PopulateStates(country) {

    var thisProcessStringResource = $("#populate-states-request-status").html();

    var list = [country];
    var jsonText = JSON.stringify({ list: list, task: "render-states" });

    $("#lead-form-tips").removeClass("bad-form");
    $("#lead-form-tips").html("<div style='float:left;width:12px;'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>" + thisProcessStringResource + "</div>");


    $.ajax({
        type: "POST",
        url: "ActionService.asmx/CreateLeadTaskController",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            $("#LF_WithStateState").html(result.d);

            resetLFNotice()

        },
        fail: function (result) {

            $("#LF_WithStateState").html(result.d);

            $("#lead-form-tips").addClass("bad-form");
            $("#lead-form-tips").html(result.d);

        }
    });


}

function revalidatePostalCity(postalId, required) {

    var invalid = 0;

    if (required == true) {

        $("#postal-tips").html("");

        var idBeingHandled = $(postalId).attr("id");

        recheckPostalAfterSubmit(idBeingHandled, "", "#postal-tips");
        invalid = $(".invalid-address-field").length;

    } else {

    }

    return invalid;
}

function clearLeadForm() {

    $("#drpLstSalutation").val("");
    $("#txtFirstNameLF").val("");
    $("#txtMiddleNameLF").val("");
    $("#txtLastNameLF").val("");
    $("#drpLstSuffix").val("");
    $("#txtEmailLF").val("");
    $("#LF_Country").val("");
    $("#LF_WithStateState").val("");
    $("#txtAreaCodeLF").val("");
    $("#txtCityLF").val("");
    $("#txtMessageLF").val("");
    $("#txtCaptcha").val("");
    $("#LF_WithStateState").html("");

}

function resetPostalCity() {

    $("#LF_WithoutStatePostalCode").removeClass("invalid-address-field");
    $("#LF_WithoutStateCity").removeClass("invalid-address-field");

    $("#LF_WithoutStatePostalCode").removeClass("current-control-being-verified");
    $("#LF_WithoutStateCity").removeClass("current-control-being-verified");

    $("#LF_WithStatePostalCode").removeClass("invalid-address-field");
    $("#LF_WithStateCity").removeClass("invalid-address-field");

    $("#LF_WithStatePostalCode").removeClass("current-control-being-verified");
    $("#LF_WithStateCity").removeClass("current-control-being-verified");

    $("#LF_WithStatePostalCode").val("");
    $("#LF_WithStateCity").val("");


    $("#LF_WithoutStatePostalCode").val("");
    $("#LF_WithoutStateCity").val("");

    $("#postal-tips").html("");
}