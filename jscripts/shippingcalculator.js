$(document).ready(function () {

    if (typeof ( $("#AddressControl_txtPostal").val() ) != "undefined") {

        GetStringResources("", true);

        checkIfUserIsLoggedIn();
        ShippingCalculatorControlEventsListener();
    }

})

function ShippingCalculatorControlEventsListener() {

    $("#btnCalcShip").click(function () {
        $("#shippingMethodOpt").html("");
        FinalizeShippingAddressForm();
    });

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
    $("#AddressControl_txtPostal").ISEBubbleMessage({ "input-id": "AddressControl_txtPostal", "label-id": "AddressControl_lblPostal", "input-mode": "postal" });
    $("#AddressControl_txtCity").ISEBubbleMessage({ "input-id": "AddressControl_txtCity", "label-id": "AddressControl_lblCity" });
    $("#AddressControl_txtState").ISEBubbleMessage({ "input-id": "AddressControl_txtState", "label-id": "AddressControl_lblState", "input-mode": "state" });

    $("#AddressControl_txtCity").removeClass("requires-validation");

}

function FinalizeShippingAddressForm() {
    /*  
    -> This function revalidates required informations and email address format 
    -> If all information is good then calls function DoSubmissionOfCaseFormAction()
    */
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
            If city control is on display and empty: validate city control <-- */

            if (thisObjectId == "#AddressControl_txtCity") {
                if (cssDisplay == "none") {
                    skip = true;
                } else {
                    skip = true;
                    if (objectValue == "") skip = false;
                }
            }

            /* state control
            If state control is on display 
            -> It must be validated on submit 
            -> Skip EMPTY validation if state control has assigned value
            otherwise:
            skip state validation and skp EMPTY validation of state control*/

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
    if (formHasEmptyFields) goodForm = false;

    if (goodForm) {

        /* verify if CityStates dropdown is initialized */

        var cityStates = $("#city-states").val();

        if (typeof (cityStates) == "undefined") {

            $("#AddressControl_txtPostal").addClass("invalid-postal");

            var thisLeft = $("#AddressControl_txtPostal").offset().left;
            var thisTop = $("#AddressControl_txtPostal").offset().top;

            $("#ise-message-tips").css("top", thisTop - 54);
            $("#ise-message-tips").css("left", thisLeft - 17);


            $("#ise-message").html(_GetCityStateEnterMessage);
            $("#ise-message-tips").fadeIn("slow");

            $("#AddressControl_txtPostal").focus();

            return false;

        } else {

            $("#AddressControl_txtPostal").removeClass("undefined-city-states");
            $("#ise-message-tips").fadeOut("slow");
        }

        ValidateAddressDetails(true, true, skipStateValidation, "", "shipping-calculator");
   
    }
 }

 function DoSubmissionOfShippingCalculatorAction() {

     /* note: this function is active on usercontrol.address.control.js PageSubmitProcess() 
     function definition: */

     var country = $("#AddressControl_drpCountry").val();
     var postal = $("#AddressControl_txtPostal").val();
     var city = "";
     var states = "";
     var addressType = $("#AddressControl_drpType").val();

     var billingCityState = $("#city-states").val();
     if (typeof (billingCityState) == undefined || billingCityState == "other") {
        
         city = $("#AddressControl_txtCity").val();
         states = $("#AddressControl_txtState").val();

     } else {

         var bc = billingCityState.split(", ");
         if (bc.length > 1) {

             city = bc[1];
             states = bc[0];

         } else {

             city = bc[0];
             states = "";
         }
     }

     $("#imgLoading").css("display", "block");
     getShippingCalculation(country, states, postal, addressType);

 }

 function getShippingCalculation(selectedCountry, selectedState, postalCode, addressType) {

     var jsonText = JSON.stringify({ country: selectedCountry, state: selectedState, postalCode: postalCode, addressType: addressType });

     $.ajax({
         type: "POST",
         url: "ActionService.asmx/GetShippingMethodCalc",
         data: jsonText,
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: function (result) {

             $("#imgLoading").css("display", "none");
             $("#shippingMethodOpt").html(result.d);

             $('input:radio[name=shippingmethod]').bind("click", function () {
                 var selectedShippingMethod = $('input:radio[name=shippingmethod]:checked').val();
                 setCookie('selectedSM', selectedShippingMethod);                
             });

         }
     });
 }

 function checkIfUserIsLoggedIn() {
     $.ajax({
         type: "POST",
         url: "ActionService.asmx/GetRegisteredCustomerShippingAddress",
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: function (result) {

            var custShipAddress = $.parseJSON(result.d);
            loadCustomerShipToAddress(custShipAddress); 

         },
         fail: function (result) {
         }
     });
 }

 function loadCustomerShipToAddress(list) {

     if (typeof (list) == "undefined" || list == null || list == "") return false;

     var country = list.country;
     var state   = list.state;
     var postal  = list.postalCode;
     var city    = list.city;
     var residenceType = list.residenceType; 

     /* Profile details assignments:
         1. Assigns profile details to input box
         2. Hides input box label if field value is not empty 
         3. Shows input box label if field value is empty */

     if (country != "") {
         $("#AddressControl_drpCountry").val(country);

     }

     if (postal != "") {

         $("#AddressControl_txtPostal").val(postal);
         $("#AddressControl_lblPostal").addClass("display-none");
         $("#AddressControl_txtPostal").addClass("postal-is-corrected");

     } else {
         $("#AddressControl_lblPostal").removeClass("display-none");
     }

     if (city != "") {
         $("#AddressControl_txtCity").val(city);
         $("#AddressControl_lblCity").addClass("display-none");
         $('#AddressControl_txtCity').attr('disabled', true);
     } else {
         $("#AddressControl_lblCity").removeClass("display-none");
     }


     if (state != "") {
         $("#AddressControl_txtState").val(state);
         $("#AddressControl_lblState").addClass("display-none");
     } else {
         $("#AddressControl_lblState").removeClass("display-none");
     }

     if (residenceType != "") {
         switch (residenceType) {
             case 0:
                 residenceType = "Unknown";
                 break;
             case 1:
                 residenceType = "Residential";
                 break;
             default:
                 residenceType = "Commercial";
         }
         $("#AddressControl_drpType").val(residenceType);
     }

     // <-- Profile details assignments

     //display city and states, remove "Enter Postal for City and State" caption
     $(".zip-city-other-place-holder").fadeIn("Slow");
     $("#enter-postal-label-place-holder").html("<input type='hidden' value='other' id='city-states'>");

     HideStateInputBoxForCountryWithState("AddressControl");
 }

 function setCookie(name, value) {
    document.cookie = name + "=" + value + "; path=/";
}