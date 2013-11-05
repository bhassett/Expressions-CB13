var _imRegistered = false;
var _finalButtonText = "";

$(document).ready(function () {
    /* .ready() 

    -> execute the following functions when the DOM is fully loaded
        
    1. InitilizeOnePageInfo()
    2. EventsListener()
    3. CreditCardPanelWorkAround()
    4. ClearHiddentFieldsValue()
    5. DisplayErrorMessagePlaceHolder()
        
    note: each of the functions has attached definition, read it.

    */

    InitPlugIn();
    InitOnePageCheckout();
  

});

$(window).load(function () {

    $("#ctrlPaymentTerm_chkSaveCreditCardInfo").click(function () {

        var isChecked = $(this).attr("checked");

        if ($("#ctrlPaymentTerm_chkSaveCreditCardInfo")) {

            if (isChecked && isChecked != undefined) {
                $("#ctrlPaymentTerm_chkSaveCreditCardInfo").attr('checked', 'checked');
            }
            else {
                $("#ctrlPaymentTerm_chkSaveCreditCardInfo").removeAttr('checked');
            }
        }

    });

    initializeLoaderListener();

})

function initializeLoaderListener() {
    var checkerID = 0;

    var checker = function () {

        var shippingMethodControl = ise.Controls.ShippingMethodController.getControl('ctrlShippingMethod');
        if (shippingMethodControl != null && shippingMethodControl != 'undefined') {
            if (shippingMethodControl.isShippingOnDemand == 'true') {

                if (!shippingMethodControl.isAutoClickOfOption) {
                    $("body").data("globalLoader").hide();
                    clearInterval(checkerID);
                }
            }
            else {
                if (shippingMethodControl.shippingMethodLoadingComplete) {
                    $("body").data("globalLoader").hide();
                    clearInterval(checkerID);
                }
            }
        }
        else {
            $("body").data("globalLoader").hide();
            clearInterval(checkerID);
        }
    }
    checkerID = setInterval(checker, 200);
}


function InitOnePageCheckout() {

    /* Definition:
        
    1. This function is called on $("document").ready
    2. Initialized / Set up all the need contents for shipping contact section, shipping method, and payments section
    3. Load string resources
    4. Load list of items (cart)

    */

    $("body").data("globalLoader").show();
    GetStringResources("one-page-checkout", true);  // --> see jquery.cbe.bubble.message.js for definition
    GetAppConfigs("one-page-checkout"); // --> see jquery.cbe.bubble.message.js for definition

    GetCustomerInfo(false, "shipping-contact", "");
    GetCustomerInfo(false, "payments-info", "");

    EventsListener();
    CreditCardPanelStylesWorkAround();
    ClearHiddenFieldsValue();
}

function EventsListener() {

    /* Definition:
       
    This function initalize event handler of the following controls

    1. opc-submit-step-1 (html button) click event
    2. opc-submit-step-2 (html button) click event
    3. opc-submit-step-3 (html button) click event
    4. edit-shipping (html anchor link) click event
    5. edit-shipping-method (html anchor link) click event
    6. edit-payments-method (html anchor link) click event

    */

    $("#opc-submit-step-1").click(function () {

        /* Definition:
        
        On click event of this control it calls the function OnePageCheckOutStep1() 
        
        */
        OnePageCheckOutStep1();

    });

    $("#opc-submit-step-2").click(function () {

        /* Definition:
        
        On click event of this control it executes the following:
         
        1. Hide any error message (bubble tips)
        2. Hide step1 and step3 error place holder\
        3. Store the value of the selected shipping method on variable: shippingMethod
        4. Store the value of the selected shipping method rate on variable: rateId
        5. Calls the function OnePageCheckoutStep2()

        */

        $("#ise-message-tips").fadeOut("fast");

        $("#step-3-error-place-holder").fadeOut("fast");
        $("#step-1-error-place-holder").fadeOut("fast");

        var shippingMethod = $("#ctrlShippingMethod_shippingMethod").val();
        var freight = $("#ctrlShippingMethod_freight").val();
        var freightCalculation = $("#ctrlShippingMethod_freightCalculation").val();
        var rateID = $("#ctrlShippingMethod_realTimeRateGUID").val();

        OnePageCheckoutStep2(shippingMethod, freight, freightCalculation, rateID);

    });

    $("#opc-submit-step-3").click(function () {

        /* Definition:
        
        On click event of this control it executes the following:
            
        1. Assign the value of of credit card description on a hidden server control: txtDescription (this is only working if tokenization is ON)
        2. Assign empty string on a hidden server control: txtDescription if the value of variable cdesc is undefined
        3. Calls the function OnePageCheckoutStep3()

        */

        if (_IsRequireTermsAndConditionsAtCheckout == "true") {

            if ($("#ctrlPaymentTerm_termsAndConditionsChecked").is(':checked')) {

                OnePageCheckOutStep3();

            } else {

                ShowTermsAndConditionsMessage();
            }

        } else {

            OnePageCheckOutStep3();
        }


    });


    $("#edit-shipping").click(function () {

        /* Definition:
        
        On click event of this control it calls the function OpenShippingSection 
        note: edit-shipping is an html anchor (link)
        */

        OpenShippingSection();

    });

    $("#edit-shipping-method").click(function () {

        /* Definition:
        
        On click event of this control it calls the function OpenShippingSection 
        note: edit-shipping-method is an html anchor (link)
          
        1. Hide any error message (bubble tips)
        2. Hide step1 and step3 error place holder
        3. Verify if the variable _HasShippingMethod is not an empty string
        4. Calls ShowShippingMethodsSection function if #3 condition is TRUE

        */

        $("#ise-message-tips").fadeOut("fast");

        $("#step-1-error-place-holder").fadeOut("fast");
        $("#step-3-error-place-holder").fadeOut("fast");

        var _HasShippingMethod = $("#selected-shipping-method").html();
        if (_HasShippingMethod != "") ShowShippingMethodsSection("", false, false);


    });

    $("#ShippingAddressControl_drpType").change(function () {

        $("#ise-message-tips").fadeOut("slow");
        $("#ShippingAddressControl_drpType").removeClass("required-input");

    });
    
    $("#im-over-13-years-of-age").click(function () {

        $("#ise-message-tips").fadeOut("slow");
        $(this).removeClass("required-input");

    });

    $("#ise-message-tips").click(function () {
        $("#ise-message-tips").fadeOut("slow");
    });

    $("#ctrlPaymentTerm_termsAndConditionsChecked").click(function () {
        $("#ise-message-tips").fadeOut("slow");
    });

    $("#copyShippingInfo").click(function () {

        var adjustCityWidth = true;
        if ($("#li-billing-contact").html() == "") adjustCityWidth = false;

        if ($(this).attr('checked') == "checked") {

            UpdateBillingInfoFormValues(false, "", adjustCityWidth, true);
            disabledBillingInfos(true);

        } else {

            UpdateBillingInfoFormValues(false, "", adjustCityWidth, false);
            disabledBillingInfos(false);

        }

    });

}

function disabledBillingInfos(disabled) {

    if (disabled) {
   
        $("#BillingAddressControl_drpCountry").addClass("control-disabled");
        $("#txtBillingContactName").addClass("control-disabled");
        $("#txtBillingContactNumber").addClass("control-disabled");

        $("#BillingAddressControl_txtStreet").addClass("control-disabled");
        $("#BillingAddressControl_txtPostal").addClass("control-disabled");
        $("#BillingAddressControl_txtCity").addClass("control-disabled");
        $("#BillingAddressControl_txtState").addClass("control-disabled");  
        $("#BillingAddressControl_txtCounty").addClass("control-disabled");

        $("#BillingAddressControl_drpCountry").attr("disabled", "disabled");
        $("#txtBillingContactName").attr("disabled", "disabled");
        $("#txtBillingContactNumber").attr("disabled", "disabled");

        $("#BillingAddressControl_txtStreet").attr("disabled", "disabled");
        $("#BillingAddressControl_txtPostal").attr("disabled", "disabled");
        $("#BillingAddressControl_txtCity").attr("disabled", "disabled");
        $("#BillingAddressControl_txtState").attr("disabled", "disabled");
        $("#BillingAddressControl_txtCounty").attr("disabled", "disabled");

    } else {
        
        $("#BillingAddressControl_drpCountry").removeClass("control-disabled");
        $("#txtBillingContactName").removeClass("control-disabled");
        $("#txtBillingContactNumber").removeClass("control-disabled");

        $("#BillingAddressControl_txtStreet").removeClass("control-disabled");
        $("#BillingAddressControl_txtPostal").removeClass("control-disabled");
        $("#BillingAddressControl_txtCity").removeClass("control-disabled");
        $("#BillingAddressControl_txtState").removeClass("control-disabled");
        $("#BillingAddressControl_txtCounty").removeClass("control-disabled");

        $("#BillingAddressControl_drpCountry").removeAttr("disabled", "disabled");
        $("#txtBillingContactName").removeAttr("disabled", "disabled");
        $("#txtBillingContactNumber").removeAttr("disabled", "disabled");

        $("#BillingAddressControl_txtStreet").removeAttr("disabled", "disabled");
        $("#BillingAddressControl_txtPostal").removeAttr("disabled", "disabled");
        $("#BillingAddressControl_txtCity").removeAttr("disabled", "disabled");
        $("#BillingAddressControl_txtState").removeAttr("disabled", "disabled");
        $("#BillingAddressControl_txtCounty").removeAttr("disabled", "disabled");

    }

}

function OpenPaymentsSection() {

    /* Definition:
        
    On click event of this control it calls the function OpenShippingSection 
    note: edit-payments-method is an html anchor (link)
          
    1. Hide any error message (bubble tips)
    2. Hide step1 and step3 error place holder
    3. Slide up billing information (readonly mode) place holder and Slide down billing / payments information form
    4. Calls the function closedOpenSections (see definition)
    5. Update billing information form control values
    6. Get the selected / default options on save credit card listing 
    7. if #6 return values if not undefined calls the function _GetCreditCardInfo
			
    */

    $("#ise-message-tips").fadeOut("fast");

    $("#step-1-error-place-holder").fadeOut("fast");
    $("#step-3-error-place-holder").fadeOut("fast");


    $("#billing-details-wrapper-hidden").slideUp("slow", function () {

        $("#billing-details-wrapper").slideDown("slow", function () {

            closedOpenSections("available-shipping-methods", "selected-shipping-method", "edit-shipping-method");
            closedOpenSections("shipping-details-wrapper", "shipping-details-wrapper-hidden", "");

            var adjustCityWidth = true;
            if ($("#li-billing-contact").html() == "") adjustCityWidth = false;

            UpdateBillingInfoFormValues(false, "", adjustCityWidth, false);
            HideStateInputBoxForCountryWithState("BillingAddressControl");

        });

    });

    var cardCode = $("input[name='credit-card-code']:checked").val();
    if (typeof (cardCode) != "undefined") _GetCreditCardInfo(cardCode, true);

    var defaultBillingCode = $("input[name='multiple-billing-address']:checked").val();
    if (typeof (defaultBillingCode) != "undefined") _GetCreditCardInfo(defaultBillingCode, false);
}

function OpenShippingSection() {

    /* Definition:
       
    This function  executes the following:

    1. Hide any error message (bubble tips)
    2. Calls the function ShowStepButton
    3. Slide up the shipping details place holder (readonly mode)
    4. Slide down the shipping details form (edit mode)
    5. Updates shipping details form values (get the current values from thisCustomer shipping address)
    6. Calls the function closedOpenSections (see definition)
    7. Hides step2 and step3 error place holder

    */

    $("#ise-message-tips").fadeOut("fast");

    ShowStepButton("step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");

    $("#shipping-details-wrapper-hidden").slideUp("slow", function () {

        UpdateShippingInfoFormValues("", false);

        $("#shipping-details-wrapper").slideDown("slow", function () {

            HideStateInputBoxForCountryWithState("ShippingAddressControl");

        });

        closedOpenSections("available-shipping-methods", "selected-shipping-method", "edit-shipping-method");
        closedOpenSections("billing-details-wrapper", "billing-details-wrapper-hidden", "");

    });

    $("#step-2-error-place-holder").fadeOut("fast");
    $("#step-3-error-place-holder").fadeOut("fast");

}

function ClearCreditCardInfo(cardCode, counter) {

    /* definition:
        
    1. This function is action on opc-clearcard (anchor html control / link) click event
    2. Calls the action service ClearCreditCardInfo
    3. After process is successfully execute clear selected save cc info row

    */

    var jsonText = JSON.stringify({ cardCode: cardCode });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/ClearCreditCardInfo",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            $("#" + counter + "-credit-card-type").html("---");
            $("#" + counter + "-credit-card-expiry").html("---");
            $("#" + counter + "-credit-card-clear").html("---");
            $("#" + counter + "-credit-ctrlPaymentTerm_cardDescription").html("");

            DisableControlPayment(false);

        },
        fail: function (result) { }
    });

}

function CreditCardPanelStylesWorkAround() {

    /*
    This is a workaround i did to manipulate the style of PaymentTermControl

    Handles the following event:

    1. ctrlPayment radio button click Event
    2. saved cc info radio button click Event

    On input[name='ctrlPaymentTerm$'] click event:

    1. Checks if id = ctrlPaymentTerm_ctrlPaymentTerm_1
    2. If #1 condition is TRUE: append ctrlPaymentTerm_cardDescription html input (text) control on save-as-credit-place-holder (div)
    3. Slide down credit card options
    4. if #1 condition is FALSE: slide up credit-card-options place holder (div)
    5. Store the attribute (pm) of the selected payment control on the variable: selectedPayment

    6. if variable selectedPayment value is Credit Card and Credit Card description place holder is not empty
    -> fade in save-as-credit-place-holder (div) and override CreditCardPaymentMethodPanel border style
    -> slide down credit card options

    On input[name='credit-card-code'] click event:

    1. Assign the value of selected option on variable: cardCode
    2. Calls web service _GetCreditCardInfo (required cardCode parameter)

    On .opc-clearcard click event:

    1. Get the attribute (id) of the selected option
    2. Splits the id using "::" as separator
    3. Assign element with an index of 1 to variable cardCode
    4. Assign element wit and index of 2 to variable counter
    5. Calls ClearCreditCardInfo function (requires params :cardCode and counter)
       
    */

    $(".RedirectPaymentMethodPanel > tbody > tr > td").css("border", "0");
    $(".RedirectPaymentMethodPanel > tbody > tr > td > span").addClass("strong-font");

    $("#ctrlPaymentTerm_nameOnCard").addClass("light-style-input");
    $("#ctrlPaymentTerm_cardNumber").addClass("light-style-input");
    $("#ctrlPaymentTerm_cvv").addClass("light-style-input");
    $("#ctrlPaymentTerm_cardType").addClass("light-style-input");
    $("#ctrlPaymentTerm_expirationMonth").addClass("light-style-input");
    $("#ctrlPaymentTerm_expirationYear").addClass("light-style-input");

    if (typeof ($("#ctrlPaymentTerm_startMonth")) != "undefined") {

        $("#ctrlPaymentTerm_startMonth").addClass("light-style-input");
    }

    if (typeof ($("#ctrlPaymentTerm_startYear")) != "undefined") {

        $("#ctrlPaymentTerm_startYear").addClass("light-style-input");

    }

    $("input[name='ctrlPaymentTerm$']").click(function () {

        if ($(this).attr("id") == "ctrlPaymentTerm_ctrlPaymentTerm_1") {

            $("#save-as-credit-place-holder").fadeIn("slow", function () {});
            $("#credit-card-options").slideDown("slow");

        } else {

            $("#save-as-credit-place-holder").fadeOut("fast");
            $("#credit-card-options").slideUp("slow");
        }

    });

    var selectedPayment = $("input[name='ctrlPaymentTerm$']:checked").attr("pm");

    if (selectedPayment == "Credit Card" && $("#save-as-credit-place-holder").html != "") {

        $("#save-as-credit-place-holder").fadeIn("slow");
        $("#credit-card-options").slideDown("slow");

    } else {

        $("#save-as-credit-place-holder").fadeOut("fast");
        $("#credit-card-options").slideUp("slow");
    }

    $("input[name='credit-card-code']").click(function () {
        _GetCreditCardInfo( $(this).val(), true);
    });

    $("input[name='multiple-billing-address']").click(function () {
        _GetCreditCardInfo($(this).val(), false);
    });


    $("input[name='multiple-shipping-address']").click(function () {

        GetCustomerShipTo($(this).val());
   
    });

    $(".opc-clearcard").click(function () {

        var thisId = $(this).attr("id");
        var str = thisId.split("::");
        var cardCode = str[1];
        var counter = str[2];

        ClearCreditCardInfo(cardCode, counter);

    });
}

function closedOpenSections(idWrapper, idWrapperHidden, linkToDisabled) {

    idWrapper = "#" + idWrapper;
    idWrapperHidden = "#" + idWrapperHidden;
    linkToDisabled = "#" + linkToDisabled;

    if (linkToDisabled != "#") {

        $(linkToDisabled).removeClass("disabled-shipping-method");

    }

    var shippingInfoEditMode = $(idWrapper).css("display");

    if (shippingInfoEditMode != "none") {

        $(idWrapper).slideUp("slow", function () {

            $(idWrapperHidden).slideDown("slow", function () { });

        });
    }

}

function OnePageCheckOutStep1() {
 
    /*  Definition:
    
    This function is active on  opc-submit-step-1 (html button) click event 
        
    On opc-submit-step-1 click event:

    1. Hide any error message (bubble tips)
    2. Checks if Shipping information is good: see IsShippingInfoGood
    3. Checks if shipping address is good: see IsShippingAddressGood
    4. if #2 or #3 conditions is FALSE skip process by returning FALSE
    5. if #2 and #3 conditions is TRUE does  the following

    -> Store the shipping info form values to the following variables

    # contact
    # email
    # phone
    # shippingState
    # shippingStreeAddress
    # shippingCountry
    # shippingPostal
    # shippingCityState
    # shippingAddressType

    -> If no residence type selected skip process by returning FALSE
    -> A: Checks if the [city, states] control box is undefined or with value OTHERS
    -> if A: condition is TRUE store control:ShippingAddressControl_txtCity value into variable: shippingCity and control: ShippingAddressControl_txtState value into variable shippingState
    -> if A: condition is FALSE splut [city, states] using comma as separator, and store it into variable: thisCityState
    -> B: Checks if thisCItyState(array) length is greater than zero
    -> if B: condition is TRUE store thisCityState[0] value into shippingCity variable and thisCityState[1] value into shippingState variable
    -> if B: condition is FALSE store thisCityState[0] valye into shippingCity variable, and assign an empty string into shippingState variable

    -> Calls function Step1ProcessValidator()

    */

    $("#ise-message-tips").fadeOut("fast");
    $("#step-1-error-place-holder").fadeOut("fast");

    if (!IsShippingInfoGood()) return false;
    if (!IsShippingAddressGood()) return false;

    var contact              = $("#ProfileControl_txtShippingContactName").val();
    var email                = $("#ProfileControl_txtShippingEmail").val();
    var phone                = $("#ProfileControl_txtShippingContactNumber").val();

    var shippingState        = "";
    var shippingStreeAddress = $("#ShippingAddressControl_txtStreet").val();
    var shippingCountry      = $("#ShippingAddressControl_drpCountry").val();
    var shippingPostal       = $("#ShippingAddressControl_txtPostal").val();
    var shippingCityState    = $("#shipping-city-states").val();
    var shippingAddressType  = $("#ShippingAddressControl_drpType").val();


    if (shippingAddressType.toLowerCase() == _addressTypeCaption.toLowerCase()) {   // _addressTypeCaption value is SET on SetContsGlobalVariableValues function (see jquery.cbe.bubble.message.js)

        var thisLeft = $("#ShippingAddressControl_drpType").offset().left;
        var thisTop  = $("#ShippingAddressControl_drpType").offset().top;

        $("#ise-message-tips").css("top", thisTop - 54);
        $("#ise-message-tips").css("left", thisLeft - 17);

        $("#ise-message").html(_SelectYourResidenceTypeMessage);
        $("#ise-message-tips").fadeIn("slow");
        
        $("#ShippingAddressControl_drpType").addClass("required-input");

        return false;

    } else {

        $("#ise-message-tips").fadeOut("slow");
        $("#ShippingAddressControl_drpType").removeClass("required-input");
    }

    if (typeof (shippingCityState) == "undefined" || shippingCityState == "other") {

        shippingCity = $("#ShippingAddressControl_txtCity").val();
        shippingState = $("#ShippingAddressControl_txtState").val();

    } else {

        var thisCityState = shippingCityState.split(", ");

        if (thisCityState.length > 1) {

            shippingCity = thisCityState[1];
            shippingState = thisCityState[0];

        } else {

            shippingCity = thisCityState[0];
            shippingState = "";
        }

    }

    var profile = [contact, email, phone];
    var shippingAddress = [shippingStreeAddress, shippingCountry, shippingPostal, shippingCity, shippingState, shippingAddressType];


   Step1ProcessValidator(profile, shippingAddress);

}

function Step1ProcessValidator(profile, shippingAddress) {

    /* Definition:
      
    This function is active on OnePageCheckOutStep1 function

    1. Display process message
    2. Calls web service method OnePageCheckoutStep1
    3. Checks $.ajar returns value (result.d) if not empty
    4. if #3 condition is TRUE split the returned value using :: as separator
    5. Check if returnValue[0] is "true"
    6. if #5 condition is TRUE hide process message (see ShowFailedMessage function), and calls ISEAddressVerification (defined on usercontrol.address.control.js) function
    7. if #5 condition is FALSE switch on the following cases

    -> duplicate-email
    -> false-postal
    -> false-state-code
    -> default (for messages not defined / exeptions)
    */

    var jsonText = JSON.stringify({ "profile": profile, "shippingAddress": shippingAddress, "validate": true, "addressId" : "" });

    ShowProcessMessage(_SavingShippingInfoMessage, "step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/OnePageCheckoutStep1",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            var $thisPostal = $("#ShippingAddressControl_txtPostal");

            if ($thisPostal.hasClass("skip")) {
                var progress = ["step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder"];
                ISEAddressVerification(true, "one-page-checkout-step-1", "Shipping", false, progress);
                return false;
            }

            if (result.d == "valid") {

                var progress = ["step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder"];
                ISEAddressVerification(true, "one-page-checkout-step-1", "Shipping", false, progress);

            } else {

                switch (result.d) {
                    case 'invalid-email':

                        $("#ProfileControl_txtShippingEmail").addClass("email-duplicates");
                        $("#ProfileControl_txtShippingEmail").focus();

                        ShowFailedMessage("", "step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");
                        break;
                    case 'invalid-postal':

                        $("#ShippingAddressControl_txtPostal").addClass("invalid-postal-zero");
                        $("#ShippingAddressControl_txtPostal").focus();

                        ShowFailedMessage("", "step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");
                        break;
                    case 'invalid-state':

                        $("#ShippingAddressControl_txtState").addClass("state-not-found");
                        $("#ShippingAddressControl_txtState").focus();

                        ShowFailedMessage("", "step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");
                        break;
                    default:
                        var message = result.d;
                        ShowFailedMessage(message, "step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");
                        break;
                }
            }
        },
        fail: function (result) {

            var message = result.d;
            ShowFailedMessage(message, "step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");

        }
    });

}

function Step1SaveShippingContact() {

    /* Definition:
    
    This function is active on usercontrol.address.control.js : see ISEAddressVerificationTaskSelector function 
    
    1. Store the shipping info form values to the following variables

    # contact
    # email
    # phone
    # shippingState
    # shippingStreeAddress
    # shippingCountry
    # shippingPostal
    # shippingCityState
    # shippingAddressType

    -> A: Checks if the [city, states] control box is undefined or with value OTHERS
    -> if A: condition is TRUE store control:ShippingAddressControl_txtCity value into variable: shippingCity and control: ShippingAddressControl_txtState value into variable shippingState
    -> if A: condition is FALSE splut [city, states] using comma as separator, and store it into variable: thisCityState
    -> B: Checks if thisCItyState(array) length is greater than zero
    -> if B: condition is TRUE store thisCityState[0] value into shippingCity variable and thisCityState[1] value into shippingState variable
    -> if B: condition is FALSE store thisCityState[0] valye into shippingCity variable, and assign an empty string into shippingState variable
    
    2. Calls web service method: OnePageCheckoutStep1
    3. If web service process is successful calls GetCustomerInfo function

    */

    var contact              = $("#ProfileControl_txtShippingContactName").val();
    var email                = $("#ProfileControl_txtShippingEmail").val();
    var phone                = $("#ProfileControl_txtShippingContactNumber").val();
    var shippingState        = "";
    var shippingStreeAddress = $("#ShippingAddressControl_txtStreet").val();
    var shippingCountry      = $("#ShippingAddressControl_drpCountry").val();
    var shippingPostal       = $("#ShippingAddressControl_txtPostal").val();
    var shippingCityState    = $("#shipping-city-states").val();
    var shippingAddressType  = $("#ShippingAddressControl_drpType").val();
    var shippingCounty       = $("#ShippingAddressControl_txtCounty").val();
    if (typeof (shippingCounty) == "undefined") shippingCounty = "";

    if (typeof (shippingCityState) == "undefined" || shippingCityState == "other") {

        shippingCity  = $("#ShippingAddressControl_txtCity").val();
        shippingState = $("#ShippingAddressControl_txtState").val();

    } else {

        var thisCityState = shippingCityState.split(", ");

        if (thisCityState.length > 1) {

            shippingCity  = thisCityState[1];
            shippingState = thisCityState[0];

        } else {

            shippingCity  = thisCityState[0];
            shippingState = "";
        }

    }

    var addressId = $("input[name='multiple-shipping-address']:checked").val();
    if (typeof (addressId) == "undefined") addressId = "";

    var profile         = [contact, email, phone];
    var shippingAddress = [shippingStreeAddress, shippingCountry, shippingPostal, shippingCity, shippingState, shippingAddressType, shippingCounty];
    var jsonText        = JSON.stringify({ "profile": profile, "shippingAddress": shippingAddress, "validate": false, "addressId" : addressId });

  
    ShowProcessMessage(_SavingShippingInfoMessage, "step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/OnePageCheckoutStep1",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result.d == "saved") { GetCustomerInfo(true, "shipping-contact", ""); }

        },
        fail: function (result) {

            var message = result.d;
            ShowFailedMessage(message, "step-1-error-place-holder", "save-shipping-loader", "save-shipping-button-place-holder");

        }
    });

}

function OnePageCheckoutStep2(shippingMethod, freight, freightCalculation, rateID) {

    ShowProcessMessage(_SavingShippingMethodMessage, "step-2-error-place-holder", "save-shipping-method-loader", "save-shipping-method-button");

    AjaxCallWithSecuritySimplified(
        "OnePageCheckoutStep2",
        { "shippingMethod": shippingMethod, "freight": freight, "freightCalculation": freightCalculation, "realTimeRateGUID": rateID },
        function (result) {
            if (result.d == "valid") {
                var shippingMethodSelected = GetCustomerInfo(true, "shipping-method", "");
            } else {
                ShowFailedMessage("Please select a shipping method", "step-2-error-place-holder", "save-shipping-method-loader", "save-shipping-method-button");
            }
        },
        function (result) {
            ShowFailedMessage(result.d, "step-2-error-place-holder", "save-shipping-method-loader", "save-shipping-method-button");
        }
    );
}

function OnePageCheckOutStep3() {

    var cdesc = $("#ctrlPaymentTerm_cardDescription").val();
    if (typeof (cdesc) == "undefined") cdesc = "";

    $("#txtDescription").val(cdesc);
    $("#ise-message-tips").fadeOut("fast");

    $("#step-1-error-place-holder").fadeOut("fast");
    $("#step-3-error-place-holder").fadeOut("fast");

    if (!IsBillingInfoGood()) return false;
    if (!IsBillingAddressGood()) return false;

    var billingState         = "";
    var billingStreetAddress = $("#BillingAddressControl_txtStreet").val();
    var billingCountry       = $("#BillingAddressControl_drpCountry").val();
    var billingPostal        = $("#BillingAddressControl_txtPostal").val();
    var billingCityState     = $("#billing-city-states").val();

    if (typeof (billingCityState) == "undefined" || billingCityState == "other") {

        billingCity  = $("#BillingAddressControl_txtCity").val();
        billingState = $("#BillingAddressControl_txtState").val(); 

    } else {

        var bc = billingCityState.split(", ");

        if (bc.length > 1) {

            billingCity = bc[1];
            billingState = bc[0];

        } else {

            billingCity = bc[0];
            billingState = "";
        }

    }

    var isWithRequiredAge = ($("#im-over-13-years-of-age").attr('checked') == "checked");
    IsBillingInfoCorrect(billingCountry, billingPostal, billingState, isWithRequiredAge);
}

function IsBillingInfoCorrect(countryCode, postalCode, stateCode, isWithRequiredAge) {

    ShowProcessMessage(_VerifyingBillingInfoMessage, "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button");

    var successFunction = function (result) {

        if ($("#BillingAddressControl_txtPostal").hasClass("skip")) {
            ISEAddressVerification(true, "one-page-checkout-step-3", "Billing", false, progress);
            return true;
        }

        if (result.d == "valid") {

            var progress = ["step-3-error-place-holder", "save-billing-method-loader", "billing-method-button"];
            ISEAddressVerification(true, "one-page-checkout-step-3", "Billing", false, progress);

        } else {

            switch (result.d) {
                case 'invalid-postal':

                    $("#BillingAddressControl_txtPostal").addClass("invalid-postal-zero");
                    $("#BillingAddressControl_txtPostal").focus();

                    ShowFailedMessage("", "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button");

                    break;
                case 'invalid-state':

                    $("#BillingAddressControl_txtState").addClass("state-not-found");
                    $("#BillingAddressControl_txtState").focus();

                    ShowFailedMessage("", "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button");

                    break;
                case 'required-over-13':

                    var hasRequired13CheckBox = $("#im-over-13-years-of-age").attr("name");
                    if (typeof (hasRequired13CheckBox) != "undefined") {

                    $("#im-over-13-years-of-age").addClass("required-input");

                        var thisLeft = $("#im-over-13-years-of-age").offset().left;
                        var thisTop = $("#im-over-13-years-of-age").offset().top;

                        $("#ise-message-tips").css("top", thisTop - 54);
                        $("#ise-message-tips").css("left", thisLeft - 17);

                        $("#ise-message").html(_Over13Message);
                        $("#ise-message-tips").fadeIn("slow");

                        ShowFailedMessage("", "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button");

                    } else {

                        $("#btnDoProcessPayment").trigger("click");

                    }

                    break;
                default:

                    ShowFailedMessage(result.d, "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button");

                    break;
            }
        }
    }

    var errorFunction = function () {
        ShowFailedMessage(result.d, "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button"); 
    }

    var data = new Object();
    data.countryCode = countryCode;
    data.postalCode = postalCode;
    data.stateCode = stateCode;
    data.isWithRequiredAge = isWithRequiredAge;

    AjaxCallCommon("ActionService.asmx/IsBillingInfoCorrect", data, successFunction, errorFunction);

}

function Step3SavePaymentsInfo() {

    ShowProcessMessage(_ProcessingOrderMessage, "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button");

    var b_CityStates = $("#billing-city-states").val();

    if (b_CityStates != "other") $("#txtCityStates").val(b_CityStates);

    $("#btnDoProcessPayment").trigger("click");

    //workaround if credit card empty or cvv is empty 
    if ($("#errorSummary_Board_Errors").children("li").length > 0) ShowFailedMessage("", "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button");

}

function GetCustomerInfo(onSubmit, infoType, _output) {

    /* apply loading message here if not onSubmit !important */

    var jsonText = JSON.stringify({ infoType: infoType });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetCustomerInfo",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result.d != "") {

                switch (infoType) {
                    case "shipping-contact":

                        ShowShippingContactSection(result.d, onSubmit);

                        break;
                    case "shipping-method":

                        ShowShippingMethodsSection(result.d, onSubmit, true);

                        break;
                    case "payments-info":

                        var url = ""
                        var _result = _output.split("::");

                        if (_result.length > 0) {

                            if (_result[0] == "bad") {

                                ShowFailedMessage(_result[1], "step-3-error-place-holder", "save-billing-method-loader", "billing-method-button");

                            } else {

                                url = _result[1];
                                ShowPaymentsSection(result.d, onSubmit, true, url);

                            }

                            if ($("#errorSummary_Board_Errors").children("li").length > 0) OpenPaymentsSection();
                        }

                        break;
                    default:
                        break;
                }
            } else {

                if (infoType == "shipping-method") {

                     $("#edit-shipping-method").addClass("disabled-shipping-method");
                     var _freight = ise.StringResource.getString("opc.freight.rate");
                     $("#freight-value").html(_freight);
                }
            }
        },
        fail: function (result) { console.log(result.d) }
    });

}

function LoadAvailableShippingMethod(isRegistered) {

    var selectedShipToCode = $("input[name='multiple-shipping-address']:checked").val();
    if (typeof (selectedShipToCode) != "undefined") {

        OPC_GetShippingMethod(selectedShipToCode);

        $("#selected-shipping-method").fadeOut("slow", function () {
            $("#available-shipping-methods").slideDown("slow");
            $("#edit-shipping-method").removeClass("disabled-shipping-method");
        });

    } else {

        location.reload();

    }
}

function ShowShippingContactSection(details, onSubmit) {

    var data = $.parseJSON(details);

    if (data.length > 0) {

        var _imRegistered = data[0].Value;
        var _finalButtonText = data[1].Value;

        if (_imRegistered == "true") {

            $("#ProfileControl_txtShippingEmail").attr("disabled", "disabled");
            $("#ProfileControl_txtShippingEmail").addClass("disabled-input");
            $("#ProfileControl_txtShippingEmail").addClass("display-none");
            $("#lblShippingEmail").addClass("display-none");
            $("#age-13-place-holder").css("display", "none");

            if (data[11].Value == "true") {

                $("#ctrlPaymentTerm_chkSaveCreditCardInfo").attr("checked", "checked");
                $("#ctrlPaymentTerm_chkSaveCreditCardInfo").attr("disabled", "disabled");
            }

        } else {

            $("#ProfileControl_txtShippingEmail").removeAttr("disabled");
            $("#ProfileControl_txtShippingEmail").removeClass("disabled-input");

            $("#age-13-place-holder").css("display", "");
        }

        $("#opc-submit-step-3").val(_finalButtonText);

        var contact = data[2].Value;
        var email = data[3].Value;
        var phone = data[4].Value;
        var country = data[5].Value;
        var postal = data[6].Value;
        var city = data[7].Value;
        var state = data[8].Value;
        var address = data[9].Value;
        var residenceType = data[10].Value;
        var county = data[12].Value;
      
        $("#li-contact").html(contact);
        $("#li-phone").html(phone);
        $("#li-email").html(email);

        $("#li-address").html(address);

        $("#li-span-city").html(city);
        $("#li-span-state").html(state);
        $("#li-span-postal").html(postal);

        $("#li-country").html(country);
        $("#li-residence").html(residenceType);
        $("#li-residence").html(residenceType);

        if (county != "") {
            $("#li-county").removeClass("display-none");
            $("#li-county").html(county);
        }

        $("#shipping-details-wrapper").slideUp("slow", function () {
            $("#shipping-details-wrapper-hidden").slideDown("slow", function () {
                var _HasShippingMethod = $("#selected-shipping-method").html();
                $("#edit-shipping-method").addClass("disabled-shipping-method");

                if (onSubmit) {
                    location.reload();
                } else {
                    $("#selected-shipping-method").fadeOut("slow", function () {
                        $("#available-shipping-methods").slideDown("slow");
                        $("#edit-shipping-method").removeClass("disabled-shipping-method");
                    });
                }
            });

        });

        CheckWhetherToRequireShipping(data[13].Value, data[14].Value, data[15].Value);
    }
}

function ShowPaymentsSection(details, onSubmit, slideUp, url) {

    var _billingInfo = $.parseJSON(details);

    if (_billingInfo.length > 0) {

        var contact          = _billingInfo[0].Value;
        var phone            = _billingInfo[2].Value;
        var country          = _billingInfo[3].Value;
        var postal           = _billingInfo[4].Value;
        var city             = _billingInfo[5].Value;
        var state            = _billingInfo[6].Value;
        var address          = _billingInfo[7].Value;
        var paymentsMethod   = _billingInfo[8].Value;
        var paymentsTermCode = _billingInfo[9].Value;
        var county           = _billingInfo[10].Value;

        $("#li-billing-contact").html(contact);
        $("#li-billing-phone").html(phone);

        $("#li-billing-address").html(address);

        $("#li-billing-city").html(city);
        $("#li-billing-state").html(state);
        $("#li-billing-postal").html(postal);

        $("#li-billing-country").html(country);
        $("#selected-payments-method").html(paymentsMethod + "(" + paymentsTermCode + ")");

        if (county != "") {

            $("#li-billing-county").removeClass("display-none");
            $("#li-billing-county").html(county);
        }

        if (!onSubmit) {

            $("#billing-details-wrapper").slideUp("slow", function () {

                $("#billing-details-wrapper-hidden").slideDown("slow", function () { });
            });

        } else {


            $("#step-3-error-place-holder").fadeOut("fast");

            $("#billing-details-wrapper").slideUp("slow", function () {

                $("#billing-details-wrapper-hidden").slideDown("slow", function () {

                    parent.location = url;

                });
            });

        }
    }
}


function CheckWhetherToRequireShipping(hasShippableComponents, couponIncludeFreeShipping, skipShipping) {

    if (hasShippableComponents == "false" || couponIncludeFreeShipping == "true" || skipShipping == "true") {
        $("#shipping-methods-wrapper").fadeOut("slow", function () {
            $("#edit-payment-method").addClass("disabled-shipping-method");
            $("#edit-shipping-method").html("");
            OpenPaymentsSection(); 
            GetCustomerInfo(false, "shipping-method", "");
        });
    }

    if (couponIncludeFreeShipping == "true") {
        $("#coupon-free-shipping-text").html($.trim($("#lblSelectShippingMethod").html()));
    }

  
}

function ShowShippingMethodsSection(details, onSubmit, slideUp) {

    if (slideUp) {

        RegisterThisCustomerShippingMethod(details);

        var couponIncludeFreeShipping = ise.StringResource.getString("has-coupon-free-shipping");
        var skipShipping =ise.StringResource.getString("is-skip-shipping");

        if (ise.StringResource.getString("has-shippable-components") == "false" || couponIncludeFreeShipping == "true" || skipShipping == "true") {
            
            OpenPaymentsSection();
            
            $("#edit-payment-method").addClass("disabled-shipping-method");
            $("#shipping-methods-wrapper").fadeOut("slow");

            if(couponIncludeFreeShipping=="true")  $("#coupon-free-shipping-text").html($.trim($("#lblSelectShippingMethod").html()));

            return false;
        }

        $("#shipping-methods-wrapper").fadeIn("slow");
        $("#available-shipping-methods").slideUp("slow", function () {

            var shippingMethod = ise.StringResource.getString("opc.shipping.method");

            var freightRate = ise.StringResource.getString("opc.freight.rate");
            var freightTax = ise.StringResource.getString("opc.freight.tax");

            var subTotal = ise.StringResource.getString("opc.sub.total");
            var grandTotal = ise.StringResource.getString("opc.grand.total");

            var tax = ise.StringResource.getString("opc.tax");

            $("#selected-shipping-method").html(shippingMethod + " " + freightRate);
            $("#selected-shipping-method").fadeIn("slow");
          
            $("#opc-freight-rate").html(freightRate);
            $("#opc-freight-tax").html(freightTax);

            $("#opc-sub-total").html(subTotal);
            $("#opc-grand-total").html(grandTotal);

            $("#tax-place-holder").html(tax);

            if (onSubmit) {
                OpenPaymentsSection();
                $("#edit-payment-method").addClass("disabled-shipping-method")
            }

            $("#save-shipping-method-loader").fadeOut("slow", function () {

                $(this).html("");
                $("#save-shipping-method-button").css("display", "block");
                UpdateShippingInfoFormValues("", false);

            });

            $("#edit-shipping-method").removeClass("disabled-shipping-method");

        });

    } else {

        $("#selected-shipping-method").fadeOut("slow", function () {

            $("#available-shipping-methods").slideDown("slow", function () {

                $("#edit-shipping-method").addClass("disabled-shipping-method");

                closedOpenSections("billing-details-wrapper", "billing-details-wrapper-hidden", "");
                closedOpenSections("shipping-details-wrapper", "shipping-details-wrapper-hidden", "");

            });

        });

    }
}

function RegisterThisCustomerShippingMethod(resources) {

    var lst = $.parseJSON(resources);

    for (var i = 0; i < lst.length; i++) {
        ise.StringResource.registerString(lst[i].Key, lst[i].Value);
    }
}

function UpdateShippingInfoFormValues(info, isCustomerShipTo) {

    var contact       = $("#li-contact").html();
    var phone         = $("#li-phone").html();
    var address       = $("#li-address").html();
    var city          = $("#li-span-city").html();
    var state         = $("#li-span-state").html();
    var postal        = $("#li-span-postal").html();
    var country       = $("#li-country").html();
    var residenceType = $("#li-residence").html();
    var county        = $("#li-county").html();
    var email         = $("#li-email").html();

    if (isCustomerShipTo) {
        contact = info[0];
        address = info[1];
        city = info[2];
        state = info[3];
        postal = info[4];
        country = info[5];
        county = info[6];
        phone = info[7];
        email = info[8];
        residenceType = info[9];
    }

    $("#ShippingAddressControl_drpCountry").val(country);
    $("#ShippingAddressControl_drpType").val(residenceType);

    if (email != "") {
        $("#ProfileControl_txtShippingEmail").val(email);
        $("#lblShippingEmail").addClass("display-none");
    } else {
        $("#ProfileControl_txtShippingEmail").val("");
        $("#lblShippingEmail").removeClass("display-none");
    } 

    if (contact != "") {

        $("#ProfileControl_txtShippingContactName").val(contact);
        $("#lblShippingContactName").addClass("display-none");

    } else {

        $("#ProfileControl_txtShippingContactName").val("");
        $("#lblShippingContactName").removeClass("display-none");

    }

    if (phone != "") {

        $("#ProfileControl_txtShippingContactNumber").val(phone);
        $("#lblShippingContactNumber").addClass("display-none");

    } else {

        $("#ProfileControl_txtShippingContactNumber").val("");
        $("#lblShippingContactNumber").removeClass("display-none");

    }

    if (address != "") {

        $("#ShippingAddressControl_txtStreet").val(address);
        $("#ShippingAddressControl_lblStreet").addClass("display-none");

    } else {

        $("#ShippingAddressControl_txtStreet").val("");
        $("#ShippingAddressControl_lblStreet").removeClass("display-none");
    }

    if (postal != "") {

        $("#ShippingAddressControl_txtPostal").val(postal);
        $("#ShippingAddressControl_lblPostal").addClass("display-none");
    
    } else {

        $("#ShippingAddressControl_txtPostal").val("");
        $("#ShippingAddressControl_lblPostal").removeClass("display-none");

    }

    if (city != "") {

        $("#ShippingAddressControl_txtCity").val(city);
        $("#ShippingAddressControl_lblCity").addClass("display-none");

    } else {
        $("#ShippingAddressControl_txtCity").val("");
        $("#ShippingAddressControl_lblCity").removeClass("display-none");

    }

    if (state != "") {

        $("#ShippingAddressControl_txtState").val(state);
        $("#ShippingAddressControl_lblState").addClass("display-none");

    } else {
        $("#ShippingAddressControl_txtState").val("");
        $("#ShippingAddressControl_lblState").removeClass("display-none");

    }

    if (typeof ($("#ShippingAddressControl_txtCounty").val()) != "undefined") {

        if (county != "") {

            $("#ShippingAddressControl_lblCounty").addClass("display-none");
            $("#ShippingAddressControl_txtCounty").val(county);

        } else {
            $("#ShippingAddressControl_txtCounty").val("");
            $("#ShippingAddressControl_lblCounty").removeClass("display-none");

        }

    }

    $(".shipping-zip-city-other-place-holder").fadeIn("Slow");
    $("#shipping-enter-postal-label-place-holder").html("<input type='hidden' value='other' id='shipping-city-states'>");

    if (contact == "" || phone == "" || address == "" || postal == "") return false;

}

function UpdateBillingInfoFormValues(cc, credit, adjustCityWidth, copyShippingInfo) {

    var contact = "";
    var email   = "";
    var phone   = "";
    var address = "";
    var city    = "";
    var state   = "";
    var postal  = "";

    var country = "";
    var county  = "";

    if (copyShippingInfo) {

        contact = $("#li-contact").html();
        phone   = $("#li-phone").html();

        address = $("#li-address").html();

        city    = $("#li-span-city").html();
        state   = $("#li-span-state").html();
        postal  = $("#li-span-postal").html();

        country = $("#li-country").html();
        county  = $("#li-county").html();

    } else {


        if (cc) {

            contact = credit.NameOnCard;
            phone   = credit.PhoneNumber;

            address = credit.Address;

            city    = credit.City;
            state   = credit.State;
            postal  = credit.Postal;

            country = credit.Country;
            county  = credit.County;

        } else {

            contact = $("#li-billing-contact").html();
            phone   = $("#li-billing-phone").html();

            address = $("#li-billing-address").html();

            city    = $("#li-billing-city").html();
            state   = $("#li-billing-state").html();
            postal  = $("#li-billing-postal").html();

            country = $("#li-billing-country").html();
            county  = $("#li-billing-county").html();
        }

    }

   

    $("#BillingAddressControl_drpCountry").val(country);

    if (contact != "") {

        $("#txtBillingContactName").val(contact);
        $("#lblBillingContactName").addClass("display-none");
    }

    if (phone != "") {

        $("#txtBillingContactNumber").val(phone);
        $("#lblBillingContactNumber").addClass("display-none");
    }


    if (address != "") {

        $("#BillingAddressControl_txtStreet").val(address);
        $("#BillingAddressControl_lblStreet").addClass("display-none");

    }

    if (postal != "") {

        $("#BillingAddressControl_txtPostal").val(postal);
        $("#BillingAddressControl_lblPostal").addClass("display-none");
    }

    if ($("#shipping-city-states").val() == "other") { } else { }


    if (city != "") {

        $("#BillingAddressControl_txtCity").val(city);
        $("#BillingAddressControl_lblCity").addClass("display-none");

    }

    if (state != "") {

        $("#BillingAddressControl_txtState").val(state);
        $("#BillingAddressControl_txtState").removeClass("display-none");
        $("#BillingAddressControl_lblState").addClass("display-none");

        $("#BillingAddressControl_txtCity").removeClass("city-width-if-no-state");

    } else {

        if (adjustCityWidth) {
            
            $("#BillingAddressControl_lblState").addClass("display-none")
            $("#BillingAddressControl_txtState").addClass("display-none");
            $("#BillingAddressControl_txtCity").addClass("city-width-if-no-state");

        }

    }

    if( typeof ($("#BillingAddressControl_txtCounty").val()) != "undefined" ){

        if (county != "") {

            $("#BillingAddressControl_lblCounty").addClass("display-none");
            $("#BillingAddressControl_txtCounty").val(county);

        } else {

            $("#BillingAddressControl_txtCounty").val(county);
            $("#BillingAddressControl_lblCounty").removeClass("display-none");
            
        }

    }

   
    $(".billing-zip-city-other-place-holder").fadeIn("Slow");
    $("#billing-enter-postal-label-place-holder").html("<input type='hidden' value='other' id='billing-city-states'>");

    if (contact == "" || phone == "" || address == "" || postal == "") return false;

}

function IsShippingInfoGood() {

    $("#ise-message-tips").fadeOut("slow");

    var goodForm = true;

    var counter = 0;
    var formHasEmptyFields = false;

    var thisObjectId = "";
    var skip = false;
    var skipStateValidation = false;

    var ids = new Array();

    ids.push("#spare");
    ids.push("#ProfileControl_txtShippingContactName");
    ids.push("#ProfileControl_txtShippingContactNumber");
    ids.push("#ShippingAddressControl_txtStreet");
    ids.push("#ShippingAddressControl_txtPostal");
    ids.push("#ShippingAddressControl_txtCity");
    ids.push("#ShippingAddressControl_txtState");
    if ($("#ProfileControl_txtShippingEmail").css("display") != "none") {
        ids.push("#ProfileControl_txtShippingEmail");
    }


    $(".requires-validation").each(function () {   //-> scan all html controls with class .apply-behind-caption-effects


        var object = this;
        var thisObjectId = "#" + $(object).attr("id");
        var validateMe = jQuery.inArray(thisObjectId, ids);

        if ($(this).val() == "" && validateMe > 0) {

            skip = false;

            var cssDisplay = $(".shipping-zip-city-other-place-holder").css("display");
            var objectValue = $(this).val()

            /* city control -->  
            
            If city control is on display and empty: validate
                
            city control <-- */

            if (thisObjectId == "#ShippingAddressControl_txtCity") {

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

            // -> validation (IsEmpty) for Shipping states input:

            if (thisObjectId == "#ShippingAddressControl_txtState") {

                cssDisplay = $(".shipping-zip-city-other-place-holder").css("display");
                var status = IsCountryWithStates("#ShippingAddressControl_drpCountry");

                if (cssDisplay == "none" || status == "false") {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            // state control <--

            if (skip == false && validateMe > 0) {

                $(this).removeClass("current-object-on-focus");
                $(this).addClass("required-input");

                $(thisObjectId + "-label").removeClass("display-none");
                $(thisObjectId + "-label").css("color", "#999999");

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
    
    var emailIsValid = true;

    if ($("#ProfileControl_txtShippingEmail").css("display") != "none") {

        var emailAddress = $("#ProfileControl_txtShippingEmail").val();
        emailIsValid = IsEmailGood(emailAddress);
    }

    if (emailIsValid == false && formHasEmptyFields == false) $("#ProfileControl_txtShippingEmail").focus();

    if (formHasEmptyFields || emailIsValid == false) return false;

    if (goodForm) {

        var s_cityStates = $("#shipping-city-states").val();

        if (typeof (s_cityStates) == "undefined") {
            $("#ShippingAddressControl_txtPostal").focus();
            return false;
        } 
        
    }
    return true;
}

function IsShippingAddressGood() {

    /* Definition:
        
    1. Check if selected shipping country is searchable
    2. if #1 is true, it validates the postal code format (see function IsPostalFormatInvalid)
    3. returns false if format is not valid else returns true

    */

    var s_citystateIsVisible     = $(".shipping-zip-city-other-place-holder").css("display");
    var s_citystateIsVisible     = s_citystateIsVisible.toLowerCase();
    var s_country                = $("#ShippingAddressControl_drpCountry").val();
    var s_postalCode             = $("#ShippingAddressControl_txtPostal").val();
    var s_searchable             = IsCountrySearchable("#ShippingAddressControl_drpCountry");
    var s_postalHasInvalidFormat = false;
    var s_postalHasInvalidFormat = false;

    if (s_searchable == "true") {

        s_postalHasInvalidFormat = IsPostalFormatInvalid(s_country, s_postalCode);

        if (s_postalHasInvalidFormat) {

            $("#ShippingAddressControl_txtPostal").addClass("invalid-postal");
            if (s_citystateIsVisible == "none") $("#shipping-enter-postal-label-place-holder").html(_EnterPostalForCityState);
            $("#ShippingAddressControl_txtPostal").focus();

            return false;
        }
    }

    return true;
}

function IsBillingInfoGood() {

    $("#ise-message-tips").fadeOut("slow");

    var counter             = 0;
    var goodForm            = true;
    var formHasEmptyFields  = false;
    var skip                = false;
    var skipStateValidation = false;
    var thisObjectId        = "";
    var ids                 = new Array();

    ids.push("#spare");
    ids.push("#txtBillingContactName");
    ids.push("#txtBillingContactNumber");
    ids.push("#BillingAddressControl_txtStreet");
    ids.push("#BillingAddressControl_txtCity");
    ids.push("#BillingAddressControl_txtState");
    ids.push("#BillingAddressControl_txtPostal");

    $(".requires-validation").each(function () {   //-> scan all html controls with class .apply-behind-caption-effects

        var object = this;
        var thisObjectId = "#" + $(object).attr("id");
        var validateMe = jQuery.inArray(thisObjectId, ids);

        if ($(this).val() == "" && validateMe > 0) {

            skip = false;

            var cssDisplay = $(".billing-zip-city-other-place-holder").css("display");
            var objectValue = $(this).val()

            /* city control -->  
            
            If city control is on display and empty: validate
                
            city control <-- */

            if (thisObjectId == "#BillingAddressControl_txtCity") {

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

            // -> validation (IsEmpty) for billing states input:

            if (thisObjectId == "#BillingAddressControl_txtState") {

                cssDisplay = $(".billing-zip-city-other-place-holder").css("display");
                var status = IsCountryWithStates("#BillingAddressControl_drpCountry");

                if (cssDisplay == "none" || status == "false") {

                    skip = true;
                    skipStateValidation = true;

                } else {

                    skip = true;
                    if (objectValue == "") skip = false;

                }

            }

            // state control <--

            if (skip == false && validateMe > 0) {

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

        var b_cityStates = $("#billing-city-states").val();

        if (typeof (b_cityStates) == "undefined") {
            $("#BillingAddressControl_txtPostal").focus();
            return false;
        } 
    }
    return true;
}

function IsBillingAddressGood() {

    /* Definition:
        
      1. Check if selected billing country is searchable
      2. if #1 is true, it validates the postal code format (see function IsPostalFormatInvalid)
      3. returns false if format is not valid else returns true

    */

    var b_citystateIsVisible     = $(".billing-zip-city-other-place-holder").css("display");
    var b_citystateIsVisible     = b_citystateIsVisible.toLowerCase();
    var b_country                = $("#BillingAddressControl_drpCountry").val();
    var b_postalCode             = $("#BillingAddressControl_txtPostal").val();
    var b_searchable             = IsCountrySearchable("#BillingAddressControl_drpCountry");
    var postalIsGood             = true;
    var b_postalHasInvalidFormat = false;

    if (b_searchable == "true") {

        b_postalHasInvalidFormat = IsPostalFormatInvalid(b_country, b_postalCode);

        if (b_postalHasInvalidFormat) {

            $("#BillingAddressControl_txtPostal").addClass("invalid-postal");

            if (b_citystateIsVisible == "none") $("#billing-enter-postal-label-place-holder").html(_EnterPostalForCityState);

            $("#BillingAddressControl_txtPostal").focus();
            postalIsGood = false;
        }

        if (!postalIsGood) return false;
     }

    return true;
}

function _GetCreditCardInfo(cardCode, updateCreditCardInfo) {
    var jsonText = JSON.stringify({ cardCode: cardCode });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetCreditCardInfo",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result.d != "") {

            AssignValueToControlPayment(result.d);
            UpdateBillingInfoFormValues(true, result.d, true, false);

                if (result.d.CardNumber != "" && result.d.RefNo != 0) {

                    DisableControlPayment(true);

                } else {

                    DisableControlPayment(false);
                    ClearHiddenFieldsValue();
                }
            }

        },
        fail: function (result) { }
    });
}

function GetCustomerShipTo(ShipToCode) {

    AjaxCallWithSecuritySimplified(
        "GetCustomerShipTo",
        { "ShipToCode": ShipToCode },
        function (result) {
            UpdateShippingInfoFormValues(result.d, true);
            HideStateInputBoxForCountryWithState("ShippingAddressControl");
        },
        function (result) {
            console.log(result.d);
        }
    );
   
}

function OPC_GetShippingMethod(addressId) {

    var reg = ise.Controls.ShippingMethodController.registerControl('ctrlShippingMethod');
    if (reg) {
        reg.registerValidator(new ise.Validators.RequiredFieldValidator('ctrlShippingMethod_ShippingMethod', 'Please select a shipping method', null));
        reg.requestShippingMethod(addressId);
    } 


}

function ShowStepButton(errorPlaceHolderID, messageLoaderId, buttonPlaceHolderId) {

    /* Definition:

    1. Clears any text or loading (process) message on a specifid message place holder control
    2. Hides error and message place holder
    3. Removed display none attribute of button place holder
       
    */

    $("#" + errorPlaceHolderID).html("");
    $("#" + errorPlaceHolderID).css("display", "none");
    $("#" + messageLoaderId).css("display", "none");
    $("#" + buttonPlaceHolderId).css("display", "");
}

function AssignValueToControlPayment(creditCard) {

    /* This function is active on _GetCreditCardInfo and .ready() functions */

    $("#txtCode").val(creditCard.CreditCardCode);

    $("#ctrlPaymentTerm_nameOnCard").val(creditCard.NameOnCard);
    $("#ctrlPaymentTerm_cardNumber").val(creditCard.CardNumber);
    $("#ctrlPaymentTerm_cardType").val(creditCard.CardType);
    $("#ctrlPaymentTerm_expirationMonth").val(creditCard.ExpMonth);
    $("#ctrlPaymentTerm_expirationYear").val(creditCard.ExpYear);
    $("#ctrlPaymentTerm_cardDescription").val(creditCard.Description);

    $("#credit-card-masked").html("cardNumber");

    if (typeof ($("#ctrlPaymentTerm_startMonth")) != "undefined") {

        $("#ctrlPaymentTerm_startMonth").val(creditCard.StartMonth);
    }

    if (typeof ($("#ctrlPaymentTerm_startYear")) != "undefined") {

        $("#ctrlPaymentTerm_startYear").val(creditCard.StartYear);

    }

    $("#hidMaskCardNumber").val(creditCard.CardNumber);
    $("#hidCreditCardCode").val(creditCard.CreditCardCode);

}

function ClearHiddenFieldsValue() {

    /* This function is active on _GetCreditCardInfo and .ready() functions
    -> clear the values of the billing section address controls (hidden)
    */

    $("#txtBAddress").val("");
    $("#txtBCountry").val("");
    $("#txtBPostalCode").val("");
    $("#txtBCity").val("");
    $("#txtBState").val("");
    $("#txtBName").val("");
    $("#txtBPhone").val("");
}

function DisableControlPayment(disable) {

    /* This function is active on _GetCreditCardInfo function
        
    1. Accepts a boolean param: disable
    2. If param: disable is true add disable attribute and class control-disabled to the following controls:
    ELSE: remove disabled attribute and class control-disabled / and clear the values of the controls

    -> ctrlPaymentTerm_sameOnCard
    -> ctrlPaymentTerm_cardNumber
    -> ctrlPaymentTerm_cardType
    -> ctrlPaymentTerm_expirationMonth
    -> ctrlPaymentTerm_expirationYear
    -> ctrlPaymentTerm_cvv
   
    */

    if (disable) {

        $("#ctrlPaymentTerm_nameOnCard").attr("readonly", "true");
        $("#ctrlPaymentTerm_nameOnCard").addClass("control-disabled");

        $("#ctrlPaymentTerm_cardNumber").attr("readonly", "true");
        $("#ctrlPaymentTerm_cardNumber").addClass("control-disabled");

        $("#ctrlPaymentTerm_cardType").attr("disabled", "true");
        $("#ctrlPaymentTerm_cardType").addClass("control-disabled");

        $("#ctrlPaymentTerm_expirationMonth").attr("disabled", "true");
        $("#ctrlPaymentTerm_expirationMonth").addClass("control-disabled");

        $("#ctrlPaymentTerm_expirationYear").attr("disabled", "true");
        $("#ctrlPaymentTerm_expirationYear").addClass("control-disabled");

        $("#ctrlPaymentTerm_cvv").attr("readonly", "true");
        $("#ctrlPaymentTerm_cvv").addClass("control-disabled");

        if (typeof ($("#ctrlPaymentTerm_startMonth")) != "undefined") {

            $("#ctrlPaymentTerm_startMonth").attr("disabled", "true");
            $("#ctrlPaymentTerm_startMonth").addClass("control-disabled");
        }

        if (typeof ($("#ctrlPaymentTerm_startYear")) != "undefined") {

            $("#ctrlPaymentTerm_startYear").attr("disabled", "true");
            $("#ctrlPaymentTerm_startYear").addClass("control-disabled");

        }

        $("#ctrlPaymentTerm_cvv").parent("td").parent("tr").addClass("display-none");

    } else {

        $("#ctrlPaymentTerm_cardNumber").val("");
        $("#ctrlPaymentTerm_cardType").val("");
        $("#ctrlPaymentTerm_expirationMonth").val("");
        $("#ctrlPaymentTerm_expirationYear").val("");
        $("#ctrlPaymentTerm_cardDescription").val("");

        $("#ctrlPaymentTerm_nameOnCard").removeAttr("readonly");
        $("#ctrlPaymentTerm_nameOnCard").removeClass("control-disabled");

        $("#ctrlPaymentTerm_cardNumber").removeAttr("readonly");
        $("#ctrlPaymentTerm_cardNumber").removeClass("control-disabled");

        $("#ctrlPaymentTerm_cardType").removeAttr("disabled");
        $("#ctrlPaymentTerm_cardType").removeClass("control-disabled");

        $("#ctrlPaymentTerm_expirationMonth").removeAttr("disabled");
        $("#ctrlPaymentTerm_expirationMonth").removeClass("control-disabled");

        $("#ctrlPaymentTerm_expirationYear").removeAttr("disabled");
        $("#ctrlPaymentTerm_expirationYear").removeClass("control-disabled");

        $("#ctrlPaymentTerm_cvv").removeAttr("readonly");
        $("#ctrlPaymentTerm_cvv").removeClass("control-disabled");

        $("#credit-card-masked").html("");


        if (typeof ($("#ctrlPaymentTerm_startMonth")) != "undefined") {

            $("#ctrlPaymentTerm_startMonth").removeAttr("disabled");
            $("#ctrlPaymentTerm_startMonth").removeClass("control-disabled");
        }

        if (typeof ($("#ctrlPaymentTerm_startYear")) != "disabled") {

            $("#ctrlPaymentTerm_startYear").removeAttr("readonly");
            $("#ctrlPaymentTerm_startYear").removeClass("control-disabled");

        }

        $("#ctrlPaymentTerm_cvv").parent("td").parent("tr").removeClass("display-none");
    }
}

function InitPlugIn() {

    /* 
      This function append ISEAddressFinder (see jquery.cbe.address.verification.js) plugin to the followings on (checkout1.aspx):

      1. input text: ShippingAddressControl_txtPostal
      2. input text: BillingAddressControl_txtPostal


      This function appends ISEBubbleMessage (see jquery.cbe.bubble.message.js) plugin to the following controls (checkout1.aspx):

       1. ShippingAddressControl_txtStreet
       2. ShippingAddressControl_txtPostal
       3. ShippingAddressControl_txtCity
       4. ShippingAddressControl_txtState
       5. ProfileControl_txtShippingContactName
       6. ProfileControl_txtShippingEmail
       7. ProfileControl_txtShippingContactNumber
       8. BillingAddressControl_txtStreet
       9. BillingAddressControl_txtPostal
      10. BillingAddressControl_txtCity
      11. BillingAddressControl_txtState
      12. txtBillingContactName
      13. txtBillingContactNumber

    */

    $("#ShippingAddressControl_txtPostal").ISEAddressFinder({
        'country-id': '#ShippingAddressControl_drpCountry',
        'postal-id': '#ShippingAddressControl_txtPostal',
        'city-id': '#ShippingAddressControl_txtCity',
        'state-id': '#ShippingAddressControl_txtState',
        'city-state-place-holder': '.shipping-zip-city-other-place-holder',
        'enter-postal-label-place-holder': '#shipping-enter-postal-label-place-holder',
        'city-states-id': 'shipping-city-states'
    });

    $("#BillingAddressControl_txtPostal").ISEAddressFinder();

    $("#ShippingAddressControl_txtStreet").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtStreet", "label-id": "ShippingAddressControl_lblStreet" });
    $("#ShippingAddressControl_txtPostal").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtPostal", "label-id": "ShippingAddressControl_lblPostal", "input-mode": "shipping-postal", "address-type": "Shipping"});
    $("#ShippingAddressControl_txtCity").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtCity", "label-id": "ShippingAddressControl_lblCity" });
    $("#ShippingAddressControl_txtState").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtState", "label-id": "ShippingAddressControl_lblState", "input-mode": "shipping-state" });

    if (typeof ($("#ShippingAddressControl_txtCounty").val()) != "undefined") {

        $("#ShippingAddressControl_txtCounty").ISEBubbleMessage({ "input-id": "ShippingAddressControl_txtCounty", "label-id": "ShippingAddressControl_lblCounty", "optional": true });

    }

    $("#ProfileControl_txtShippingContactName").ISEBubbleMessage({ "input-id": "ProfileControl_txtShippingContactName", "label-id": "lblShippingContactName" });
    $("#ProfileControl_txtShippingEmail").ISEBubbleMessage({ "input-id": "ProfileControl_txtShippingEmail", "label-id": "lblShippingEmail", "input-mode": "email" });
    $("#ProfileControl_txtShippingContactNumber").ISEBubbleMessage({ "input-id": "ProfileControl_txtShippingContactNumber", "label-id": "lblShippingContactNumber" });

    $("#BillingAddressControl_txtStreet").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtStreet", "label-id": "BillingAddressControl_lblStreet" });
    $("#BillingAddressControl_txtPostal").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtPostal", "label-id": "BillingAddressControl_lblPostal", "input-mode": "billing-postal", "address-type": "Billing" });
    $("#BillingAddressControl_txtCity").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtCity", "label-id": "BillingAddressControl_lblCity" });
    $("#BillingAddressControl_txtState").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtState", "label-id": "BillingAddressControl_lblState", "input-mode": "billing-state" });

    if (typeof ($("#BillingAddressControl_txtCounty").val()) != "undefined") {

        $("#BillingAddressControl_txtCounty").ISEBubbleMessage({ "input-id": "BillingAddressControl_txtCounty", "label-id": "BillingAddressControl_lblCounty", "optional": true });

    }

    $("#txtBillingContactName").ISEBubbleMessage({ "input-id": "txtBillingContactName", "label-id": "lblBillingContactName" });
    $("#txtBillingContactNumber").ISEBubbleMessage({ "input-id": "txtBillingContactNumber", "label-id": "lblBillingContactNumber" });
}


function ShowTermsAndConditionsMessage() {

    $("#ctrlPaymentTerm_termsAndConditionsChecked").addClass("required-input");

    var thisLeft = $("#ctrlPaymentTerm_termsAndConditionsChecked").offset().left;
    var thisTop = $("#ctrlPaymentTerm_termsAndConditionsChecked").offset().top;

    $("#ise-message-tips").css("top", thisTop - 54);
    $("#ise-message-tips").css("left", thisLeft - 57);

    $("#ise-message").html(_TermAndConditionMessage);
    $("#ise-message-tips").fadeIn("slow");

}





