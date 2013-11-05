$(window).load(function () {

    if (ise.Configuration.getConfigValue("IsAdminCurrentlyLoggedIn") == "true") return false;
    
    var process = getQueryString()["process"];

    if (process != "" && typeof (process) != "undefined") {

        $("#support-form-wrapper").addClass("display-none");
        $("#lead-form-thank-you").fadeIn("slow");

    } else {

        InitControls();

        GetStringResources("lead-form", true);
        LeadFormControlEventsListener();

    }

});

function LeadFormControlEventsListener() {

  $("#btnSubmitLF").click(function () { FinalizeLeadForm(); });

   $("#captcha-refresh-button").click(function () {

          captchaCounter++;
          $("#captcha").attr("src", "Captcha.ashx?id=" + captchaCounter);

   });


   $("#user-control-profile-first-name-input").keypress(function(){
       
        $(this).removeClass("lead-duplicates");
        $("#user-control-profile-last-name-input").removeClass("lead-duplicates");

   });

   $("#user-control-profile-last-name-input").keypress(function(){
       
        $(this).removeClass("lead-duplicates");
        $("#user-control-profile-first-name-input").removeClass("lead-duplicates");

   });

}

function FinalizeLeadForm() {

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

                // see for IsCountryWithStates definition
                var status = IsCountryWithStates("#AddressControl_drpCountry");

                if (cssDisplay == "none" || status == "false") {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

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

    var emailAddress = $("#ProfileControl_txtEmail").val();
    var emailIsValid = IsEmailGood(emailAddress);

    if (emailIsValid == false && formHasEmptyFields == false)  $("#ProfileControl_txtEmail").focus();

    if (formHasEmptyFields || emailIsValid == false) goodForm = false;

    if (goodForm) {

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

            $("#AddressControl_txtPostal").removeClass("undefined-city-states");
            $("#ise-message-tips").fadeOut("slow");
        }

        ValidateAddressDetails(true, true, true, "", "lead-form");
    }
}

function TriggerLeadFormSubmitButton() {

    var cityStates = $("#city-states").val();
    if (cityStates != "other") $("#txtCityStates").val(cityStates);

    ShowProcessMessage(_SavingLeadMessage, "error-summary", "save-lead-loader", "save-lead-button-place-holder");
    $("#btnSaveLead").trigger("click");
}

function InitControls() {

    $("#AddressControl_txtPostal").ISEAddressFinder({
        'country-id': '#AddressControl_drpCountry',
        'postal-id': '#AddressControl_txtPostal',
        'city-id': '#AddressControl_txtCity',
        'state-id': '#AddressControl_txtState',
        'city-state-place-holder': '.zip-city-other-place-holder',
        'enter-postal-label-place-holder': '#enter-postal-label-place-holder',
        'city-states-id': 'city-states'
    });

    $("#AddressControl_txtStreet").ISEBubbleMessage({ "input-id": "AddressControl_txtStreet", "label-id": "AddressControl_lblStreet" });
    $("#AddressControl_txtPostal").ISEBubbleMessage({ "input-id": "AddressControl_txtPostal", "label-id": "AddressControl_lblPostal", "input-mode": "postal"});
    $("#AddressControl_txtCity").ISEBubbleMessage({ "input-id": "AddressControl_txtCity", "label-id": "AddressControl_lblCity" });
    $("#AddressControl_txtState").ISEBubbleMessage({ "input-id": "AddressControl_txtState", "label-id": "AddressControl_lblState", "input-mode": "state" });
  
    if (typeof ($("#AddressControl_txtCounty").val()) != "undefined") {

        $("#AddressControl_txtCounty").ISEBubbleMessage({ "input-id": "AddressControl_txtCounty", "label-id": "AddressControl_lblCounty", "optional": true });

    }

    $("#ProfileControl_txtFirstName").ISEBubbleMessage({ "input-id": "ProfileControl_txtFirstName", "label-id": "lblFirstName" });
    $("#ProfileControl_txtLastName").ISEBubbleMessage({ "input-id": "ProfileControl_txtLastName", "label-id": "lblLastName" });

    $("#ProfileControl_txtEmail").ISEBubbleMessage({ "input-id": "ProfileControl_txtEmail", "label-id": "lblEmail", "input-mode": "email" });
    $("#ProfileControl_txtContactNumber").ISEBubbleMessage({ "input-id": "ProfileControl_txtContactNumber", "label-id": "lblContactNumber" });
    $("#txtMessage").ISEBubbleMessage({ "input-id": "txtMessage", "label-id": "lblMessage" });
    $("#txtCaptcha").ISEBubbleMessage({ "input-id": "txtCaptcha", "label-id": "lblCaptcha" });

    $("#account-name-wrapper").html("");
    $("#passwords-wrapper").html("");
}