var captchaIsGood       = false;
var counter             = 1;

$("document").ready(function () {

    var defaultCountry = $("#drpLstCountriesLF").val();

    if (defaultCountry != "") {

        PopulateStates(defaultCountry);

    }

    $("#txtAreaCodeLF").format({ type: "phone-number", autofix: true });


    // DrpLstCountriesLF (server control without autopostback) OnChange Event

    $("#drpLstCountriesLF").change(function () {

        var selected = $("#drpLstCountriesLF").val();
        PopulateStates(selected);

    });


    // Submit Button (html control) OnClick Event

    $("#btnSubmitLF_lnkMobileButton").click(function () {
        var error = "";

        var requiredFieldsAllGood = true;
        var emailIsGood = true;


        // get values of lead form input controls

        var salutation = $("#drpLstSalutation").val();
        if (salutation == "Select One") salutation = "";

        var firstName = $("#txtFirstNameLF").val();
        var middleName = $("#txtMiddleNameLF").val();
        var lastName = $("#txtLastNameLF").val();

        var suffix = $("#drpLstSuffix").val();
        if (suffix == "Select One") suffix = "";

        var email = $("#txtEmailLF").val();
        var country = $("#drpLstCountriesLF").val();
        var states = $("#drpLstStatesLF").val();
        var areaCode = $("#txtAreaCodeLF").val();
        var city = $("#txtCityLF").val();
        var message = $("#txtMessageLF").val();
        var captcha = $("#txtCaptcha").val();

        var skinId = $("#skin-id").html();
        skinId = $.trim(skinId);

        var localeSettiings = $("#local-settings").html();
        localeSettiings = $.trim(localeSettiings);

        // check for empty required information

        if (isEmpty(firstName)) requiredFieldsAllGood = false;
        if (isEmpty(middleName)) requiredFieldsAllGood = false;
        if (isEmpty(lastName)) requiredFieldsAllGood = false;
        if (isEmpty(email)) requiredFieldsAllGood = false;
        if (isEmpty(areaCode)) requiredFieldsAllGood = false;
        if (isEmpty(message)) requiredFieldsAllGood = false;
        if (isEmpty(city)) requiredFieldsAllGood = false;
        if (isEmpty(country)) requiredFieldsAllGood = false;
        if (isEmpty(states)) requiredFieldsAllGood = false;


        if (requiredFieldsAllGood) {

            resetLFNotice();

            emailIsGood = validateEmail(email);

            if (!emailIsGood) {

                error = $("#email-error").html();
                $("#lead-form-tips").addClass("notificationText");

                $("#lead-form-tips").html(error);

            } else {
                resetLFNotice();
            }


        } else {

            error = $("#required-error").html();

            $("#lead-form-tips").addClass("notificationText");
            $("#lead-form-tips").html(error);

        }


        // Create lead if form(via ajax) is good -->

        if (requiredFieldsAllGood && emailIsGood) {


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

            DoCreateLead(args);


        }

    });


    $("#captcha-refresh-button").click(function () {

        counter++;
        $("#captcha").attr("src", "Captcha.ashx?id=" + counter);

    });


});

function resetLFNotice() {
    $("#lead-form-tips").removeClass("notificationText");
    $("#lead-form-tips").html("Please complete the following form and we will contact you soon.");
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

    $("#lead-form-tips").removeClass("notificationText");
    $("#lead-form-tips").html("<div style='float:left'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>Checking security code, Please wait this may take a while(<i>if it's taking too long please refresh and resubmit the form</i>).</div>");

     var list     = [args[0]];
     var jsonText = JSON.stringify({ list: list, task: "validate-captcha" });

     $.ajax({
         type: "POST",
         url: "../leadformAjaxController.asmx/TaskSelector",
         data: jsonText,
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: function (result) {

             if (result.d == "match") {

                 saveLead(args);

             } else {

                 var error = $("#captcha-error").html();

                 $("#lead-form-tips").addClass("notificationText");
                 $("#lead-form-tips").html(error);

             }

         },
         fail: function (result) {


             $("#lead-form-tips").addClass("notificationText");
             $("#lead-form-tips").html(result.d);

         }
     });


}

function saveLead(details) {
    
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

    $("#lead-form-tips").removeClass("notificationText");
    $("#lead-form-tips").html("<div style='float:left'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>Saving details, Please wait this may take a while(<i>if it's taking too long please refresh and resubmit the form</i>).</div>");

    var list = [salutation, firstName, middleName, lastName, suffix, email, country, states, city, phone, message, skinId, localSettings];

    var jsonText = JSON.stringify({ list: list, task: "save-lead" });


    $.ajax({
        type: "POST",
        url: "../leadformAjaxController.asmx/TaskSelector",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            switch (result.d) {
                case "True":

                    $("#lead-form-tips").removeClass("notificationText");
                    $("#lead-form-tips").addClass("success");

                    $("#lead-form-tips").html("New lead has been successfully added!");


                    $("#lead-form-container").fadeOut("slow");

                    $("#hidden-div").css("display", "block");
                    $("#lead-form-thankyou").fadeIn("slow");

                    clearLeadForm();

                    break;
                case "EmailHasDuplicates":

                    error = $("#email-duplicate-error").html();

                    $("#lead-form-tips").addClass("bad-form");
                    $("#lead-form-tips").html(error);

                    break;
                case "LeadHasDuplicates":

                    error = $("#lead-duplicate-error").html();

                    $("#lead-form-tips").removeClass("notificationText");
                    $("#lead-form-tips").html(error);

                    break;
                default:

                    $("#lead-form-tips").removeClass("notificationText");
                    $("#lead-form-tips").html(result.d);

                    //$("#lead-form-tips").html("Undefined Result, Please refresh your browser and try again.");

                    break;
            }


        },
        fail: function (result) {

            $("#lead-form-tips").removeClass("notificationText");
            $("#lead-form-tips").html(result.d);

        }
    });
}


function PopulateStates(country) {

    var list = [country];
    var jsonText = JSON.stringify({ list: list, task: "render-states" });

    $("#lead-form-tips").removeClass("notificationText");
    $("#lead-form-tips").html("<div style='float:left;width:12px;'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>Getting country states, Please wait this may take a while(<i>if it's taking too long please refresh and reselect your country</i>).</div>");


    $.ajax({
        type: "POST",
        url: "../leadformAjaxController.asmx/TaskSelector",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (result) {

            $("#drpLstStatesLF").html(result.d);

            resetLFNotice()

        },
        fail: function (result) {

            $("#drpLstStatesLF").html(result.d);

            $("#lead-form-tips").addClass("bad-form"); 
            $("#lead-form-tips").html(result.d);

        }
    });


}


function clearLeadForm() {

    $("#drpLstSalutation").val("");
    $("#txtFirstNameLF").val("");
    $("#txtMiddleNameLF").val("");
    $("#txtLastNameLF").val("");
    $("#drpLstSuffix").val("");
    $("#txtEmailLF").val("");
    $("#drpLstCountriesLF").val("");
    $("#drpLstStatesLF").val("");
    $("#txtAreaCodeLF").val("");
    $("#txtCityLF").val("");
    $("#txtMessageLF").val("");
    $("#txtCaptcha").val("");
    $("#drpLstStatesLF").html("");


}

// function validateReCaptcha() {
//    
//     var list = [Recaptcha.get_challenge(),  Recaptcha.get_response()];
//     var jsonText = JSON.stringify({ list: list, task: "validate-captcha" });

//     alert(jsonText);

//    $.ajax({
//        type: "POST",
//        url: "leadformAjaxController.asmx/TaskSelector",
//        data: jsonText,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {

//            alert(msg.d);

//        },
//        fail: function (msg) {

//            alert(msg.d);

//        }
//    });

//}