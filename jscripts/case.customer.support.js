$(document).ready(function () {

    var process = getQueryString()["process"];
   
    if (process != "" && typeof(process) != "undefined") {

        $("#support-form-wrapper").addClass("display-none");
        $("#case-form-thank-you").fadeIn("fast");

    } else {

        InitControls();
        EventsListener();

        GetStringResources("customer-support", true);
    }

    CheckIfUserIsLoggedIn();

});

function EventsListener() {

    $("#submit-case").click(function () {

        if ($(this).hasClass("editable-content")) return false;
        SubmitCaseForm();

    });

    $("#captcha-refresh-button").click(function () {

        captchaCounter++;
        $("#captcha").attr("src", "Captcha.ashx?id=" + captchaCounter);

    });

}

function SubmitCaseForm() {

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

            var cssDisplay  = $(".zip-city-other-place-holder").css("display");
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


    if (formHasEmptyFields) $(thisObjectId).focus();

    var emailAddress = $("#txtEmail").val();
    var emailIsValid = IsEmailGood(emailAddress);

    if (emailIsValid == false && formHasEmptyFields == false) $("#txtEmail").focus();
    if (formHasEmptyFields || emailIsValid == false) goodForm = false;

    if (goodForm) {

            // --> verify if CityStates dropdown is initialized

            var cityStates = $("#city-states").val();

            if (typeof (cityStates) == "undefined") {

                var thisLeft = $("#AddressControl_txtPostal").offset().left;
                var thisTop  = $("#AddressControl_txtPostal").offset().top;

                $("#ise-message-tips").css("top", thisTop - 54);
                $("#ise-message-tips").css("left", thisLeft - 17);


                $("#ise-message").html(_GetCityStateEnterMessage);
                $("#ise-message-tips").fadeIn("slow");

                return false;

            } else {

                $("#AddressControl_txtPostal").removeClass("invalid-postal-zero");
                $("#ise-message-tips").fadeOut("slow");
            }

        //<--

         ValidateAddressDetails(true, true, true, "", "case-form");

    }
}

function CheckIfUserIsLoggedIn() {

    var buttonsHTML = "";
    var thisProcessStringResource = _ProfileCheckingMessage;

    $("#onload-process-place-holder").removeClass("error-message");

    $("#onload-process-place-holder").html("<div style='float:left;width:12px;'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container'>" + thisProcessStringResource + "</div>");
    $("#onload-process-place-holder").fadeIn("slow");

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetCustomerProfile",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            var result = result.d;

            if (result != "0") {

                var list = result.split("::");
                RenderProfileDetails(list);

                $("#case-history-link").css("display", "");

            } else {

                $("#case-history-link").addClass("display-none");

            }

            $("#onload-process-place-holder").html(_DefaultMessage);
        },
        fail: function (result) {

            ShowFailedMessage(result.d, "errorSummary", "save-case-loader", "save-case-button-place-holder");
            $("#case-history-link").addClass("display-none");

        }
    });
}

function RenderProfileDetails(list) {

    var fullName = list[3];
    var email    = list[4];
    var phone    = list[5];
    var country  = list[6];
    var state    = list[7];
    var postal   = list[8];
    var city     = list[9];
    var address  = list[10];

    /* 
      Profile details assignments:

      1. Assigns profile details to input box
      2. Hides input box label if field value is not empty 
      3. Shows input box label if field value is empty

      -->
    */

    if (fullName != "") {

        $("#txtContactName").val(fullName);
        $("#lblContactName").addClass("display-none");
    
    } else {

        $("#lblContactName").removeClass("display-none");

    }

    if (email != "") {

       $("#txtEmail").val(email);
       $("#lblEmail").addClass("display-none");
    
    } else {

       $("#lblEmail").removeClass("display-none");

    }

    if (phone != "") {
        $("#txtContactNumber").val(phone);
        $("#lblContactNumber").addClass("display-none");
    } else {
        $("#lblContactNumber").removeClass("display-none");
    }

    if (country != "") {

        $("#AddressControl_drpCountry").val(country);
    }

    if (postal != "") {

        $("#AddressControl_txtPostal").val(postal);
        $("#AddressControl_lblPostal").addClass("display-none");

    } else {

        $("#AddressControl_lblPostal").removeClass("display-none");

    }

    if (city != "") {

        $("#AddressControl_txtCity").val(city);
        $("#AddressControl_lblCity").addClass("display-none");

    } else {

        $("#AddressControl_lblCity").removeClass("display-none");

    }

    if (state != "") {

        $("#AddressControl_txtState").val(state);
        $("#AddressControl_lblState").addClass("display-none");

    } else {

        $("#AddressControl_lblState").removeClass("display-none");

    }

    if (address != "") {

        $("#AddressControl_txtStreet").val(address);
        $("#AddressControl_lblStreet").addClass("display-none");

    } else {

        $("#AddressControl_lblStreet").removeClass("display-none");

    }

   // <-- Profile details assignments

   // -> display city and states, remove "Enter Postal for City and State" caption

    $(".zip-city-other-place-holder").fadeIn("Slow");
    $("#enter-postal-label-place-holder").html("<input type='hidden' value='other' id='city-states'>");

    // <--

    HideStateInputBoxForCountryWithState("AddressControl");
 }

function TriggerCaseFormSubmitButton() {

    var cityStates = $("#city-states").val();
    if (cityStates != "other") $("#txtCityStates").val(cityStates);

    ShowProcessMessage(_SendingCaseMessage, "error-summary", "save-case-loader", "save-case-button-place-holder");
    $("#btnSendCaseForm").trigger("click");

}

function InitControls() {

    $("#AddressControl_txtPostal").ISEAddressFinder({
        'country-id'                     : '#AddressControl_drpCountry',
        'postal-id'                      : '#AddressControl_txtPostal',
        'city-id'                        : '#AddressControl_txtCity',
        'state-id'                       : '#AddressControl_txtState',
        'city-state-place-holder'        : '.zip-city-other-place-holder',
        'enter-postal-label-place-holder': '#enter-postal-label-place-holder',
        'city-states-id'                 : 'city-states'
    });

    $("#AddressControl_txtStreet").ISEBubbleMessage({ "input-id": "AddressControl_txtStreet", "label-id": "AddressControl_lblStreet" });
    $("#AddressControl_txtPostal").ISEBubbleMessage({ "input-id": "AddressControl_txtPostal", "label-id": "AddressControl_lblPostal", "input-mode": "postal"});
    $("#AddressControl_txtCity").ISEBubbleMessage({ "input-id": "AddressControl_txtCity", "label-id": "AddressControl_lblCity" });
    $("#AddressControl_txtState").ISEBubbleMessage({ "input-id": "AddressControl_txtState", "label-id": "AddressControl_lblState", "input-mode": "state" });
    $("#AddressControl_txtCounty").ISEBubbleMessage({ "input-id": "AddressControl_txtCounty", "label-id": "AddressControl_lblCounty", "optional": true });

    $("#txtContactName").ISEBubbleMessage({ "input-id": "txtContactName", "label-id": "lblContactName" });
    $("#txtEmail").ISEBubbleMessage({ "input-id": "txtEmail", "label-id": "lblEmail", "input-mode": "email" });
    $("#txtContactNumber").ISEBubbleMessage({ "input-id": "txtContactNumber", "label-id": "lblContactNumber" });

    $("#txtCaseDetails").ISEBubbleMessage({ "input-id": "txtCaseDetails", "label-id": "lblCaseDetails" });
    $("#txtSubject").ISEBubbleMessage({ "input-id": "txtSubject", "label-id": "lblSubject" });

    $("#txtCaptcha").ISEBubbleMessage({ "input-id": "txtCaptcha", "label-id": "lblCaptcha" });

}
