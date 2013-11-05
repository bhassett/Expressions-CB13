var global_postal = "";

(function ($) {
    
    var methods = {
        init: function (options) {

            /* on initialization of this plugin it set default values into the options parameters 
               and calls the initEventsListener function of this plugin */

            var settings = $.extend({
                'country-id'                       :'#BillingAddressControl_drpCountry',
                'postal-id'                        :'#BillingAddressControl_txtPostal',
                'city-id'                          :'#BillingAddressControl_txtCity',
                'state-id'                         :'#BillingAddressControl_txtState',
                'city-state-place-holder'          :'.billing-zip-city-other-place-holder',
                'enter-postal-label-place-holder'  :'#billing-enter-postal-label-place-holder',
                'city-states-id'                   : 'billing-city-states'
            }, options);

            $(this).ISEAddressFinder("initEventsListener", settings);

        },
        initEventsListener: function (settings) {

            var $this = $(this);

            $this.keypress(function (event) {

                if (event.which == 13) {
                    
                    /* note: if you have a new label id just add a new case and set your labelId on the  switch statement below: 
                       
                       On ENTER event of postal input box the following code segments does the following:
                       
                       1. Clears the value os state input box 
                       2. Remove error tag (class) state-not-found and required-input
                       3. Shows the State label for state input box
                       4. Class renderCityState function of this plugin

                    */

                    $(settings["state-id"]).val("");
                    $(settings["state-id"]).removeClass("state-not-found");
                    $(settings["state-id"]).removeClass("required-input");

                    var labelId = "";
                    switch(settings["state-id"]){
                        case "#BillingAddressControl_txtState":
                            labelId = "#BillingAddressControl_lblState";
                            break;
                        case "#ShippingAddressControl_txtState":
                            labelId = "#ShippingAddressControl_lblState";
                            break;
                        default:
                            labelId = "#AddressControl_lblState";
                            break;      
                    }

                    $(labelId).removeClass("display-none");
                    $(labelId).css("color", "#8E8E8E");

                    $this.ISEAddressFinder("renderCityState", settings);
           
                    return false;
                }

            });

            $(settings['postal-id']).blur(function () {
                
                var cityStateOnDisplay = $(settings["city-state-place-holder"]).css("display");
               
                if(cityStateOnDisplay=="none" || typeof(cityStateOnDisplay) == "undefined"){

                    $(settings["state-id"]).val("");
                    $(settings["state-id"]).removeClass("state-not-found");
                    $(settings["state-id"]).removeClass("required-input");

                    var labelId = "";

                    switch(settings["state-id"]){
                        case "#BillingAddressControl_txtState":
                            labelId = "#BillingAddressControl_lblState";
                            break;
                        case "#ShippingAddressControl_txtState":
                            labelId = "#ShippingAddressControl_lblState";
                            break;
                        default:
                            labelId = "#AddressControl_lblState";
                            break;      
                    }

                    $(labelId).removeClass("display-none");
                    $(labelId).css("color", "#8E8E8E");

                }

                $this.ISEAddressFinder("renderCityState", settings);

               

            });

            $this.keyup(function () {
                
                /* On KEYUP event of postal code input box and this control value is not empty, the following code segments does:
                   1. Verifies if city state SELECT control is on display
                   2. Clears the value fo State and City input box
                   3. Remove error tags (class) state-not-found and required-input from state input box 
                   4. Remove error tag (class) required-input from city input box */
                   
                if ($(this).val() == "") {

                   var country = $(settings['country-id']).val();
                   var postal  = $(settings['postal-id']).val();

                   var params = [postal, settings['city-state-place-holder'], settings['enter-postal-label-place-holder'],  settings['postal-id']];
                   $this.ISEAddressFinder("isCityStatesVisible", params);

                   $(settings["state-id"]).val("");
                   $(settings["city-id"]).val("");
                   
                   $(settings["state-id"]).removeClass("state-not-found");
                   $(settings["state-id"]).removeClass("required-input");
                   $(settings["city-id"]).removeClass("required-input");

                   var labelId = "";

                   switch(settings["city-id"]){
                     case "#BillingAddressControl_txtCity":
                        labelId = "#BillingAddressControl_lblCity";    
                        break;   
                     case "#ShippingAddressControl_txtCity":
                        labelId = "#ShippingAddressControl_lblCity";       
                        break;
                     default:
                        labelId = "#AddressControl_lblCity";       
                        break;
                   }

                   $(labelId).removeClass("display-none");
                   $(labelId).css("color", "#8E8E8E");
                }

            });

            $(settings['state-id']).keypress(function (event) {
                if (event.which == 13) {return false;}
            });

            $(settings['country-id']).change(function () {
                
                /* On Change event of Country SELECT control calls removeErrorTagsOnAddressControl function of this plugin,
                   clears postal code input box value, and trigger keyup event of postal input box.
                 */
               $this.ISEAddressFinder("removeErrorTagsOnAddressControl", settings);

               $(settings['postal-id']).val("");
               $(settings['postal-id']).trigger("keyup");
    
                return false;
            });

        },
        isCityStatesVisible: function (params) {
            
            /* This function verifies if city-state place SELECT control is undefined, 
               if true then place the _EnterPostalForCityState string on the place holder
               of city-state SELECT control.*/

            var $this = $(this);

            var citystateIsVisible = $(params[1]).css("display");
            var citystateIsVisible = citystateIsVisible.toLowerCase();

            if (params[0] == "" && (citystateIsVisible != "none" || citystateIsVisible != "")) {

                $(params[1]).fadeOut("Slow", function () {
        
                    $(params[2]).html($.trim(_EnterPostalForCityState));
                    $(params[2]).addClass("enter-postal-message-width");
                    $(params[3]).removeClass("skip");

                    $("#ise-message-tips").fadeOut("fast");
                });

                return false;
            }else{
             $(params[2]).removeClass("enter-postal-message-width");
            }


            if (citystateIsVisible != "none" || citystateIsVisible == "") return false;

            return true;
        },
        isPostalFormatInvalid: function (options) {

            var params = $.extend({
                'country': '',
                'postal' : ''
            }, options);

            var $this = $(this);
  
            var postalFormat = $this.ISEAddressFinder("getCountryPostalFormats", params['country']);
            var postalCode = params['postal'];

            var formats = postalFormat;

            formats = formats.split("-")

            if (formats.length > 0 && postalFormat != "free-form") {

                var postal    = postalCode.split("-");
                global_postal = postal[0];
 
                if (postal.length > 1) {

                    /*Check if the user postal input number of elements separated by hypen(-) is the 
                      same as your defined number of digits. */

                    if (postal.length != formats.length) {

                        return true;

                    } else {

                        /* loops through your postal elements separated  by hypen(-)
                        -> and verify if each element in user postal has the same length with each element in your postal format.*/

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

                    global_postal = postalCode;
                    if (postalCode.length == 0 || postalCode.length != formats[0].length) return true;

                }

            } else {

                global_postal = postalCode;

            }

            return false;

        },
        isCountrySearchable: function (options) {

            var params = $.extend({
                'country-id'    : '',
                'selected-index': ''
            }, options);

            var thisClass = $(params['country-id']).attr("class");

            var classes = thisClass.split(" ");
            var countryStatesFlag = classes[0];
            var withStates = countryStatesFlag.split("-");

            var flag = withStates[params['selected-index']];
            flag = flag.split("::");

            var status = flag[1];

            if (status.toLowerCase() == "false") return false;

            return true;

        },
        isCountryHasState: function (options) {

            var params = $.extend({
                'country-id'    : '',
                'selected-index': ''
            }, options);

            var thisClass = $(params['country-id']).attr("class");

            var classes = thisClass.split(" ");
            var countryStatesFlag = classes[0];
            var withStates = countryStatesFlag.split("-");

            var flag = withStates[params['selected-index']];
            flag = flag.split("::");

            var status = flag[0];

            if (status.toLowerCase() == "false") return false;

            return true;

        },
        getCountryPostalFormats: function (country) {
            
            // static - temporary solution for country postal format

            var names = new Array();
            names.push("united states of america");

            var digits = new Array();
            digits.push("12345-6789");
            
            var index = 0;

            if (country.toLowerCase() != "united states of america") {
                return "free-form";
            }

            return digits[index];

        },
        highlightsError: function (options) {

            var params = $.extend({
                'control-id'                   : '',
                'error-type'                   : '',
                'focus'                        : false,
                'enter-caption-place-holder-id': ''
            }, options);

            switch (params['error-type']) {
                case "invalid-postal-format":

                    $(params['control-id']).addClass('invalid-postal');
                    $(params['enter-caption-place-holder-id']).html(_EnterPostalForCityState);

                    break;
                default:
                    break;
            }


        },
        bindCityStateOnChange: function (options) {

            var params = $.extend({
                'city-states-id'         : 'billing-city-states',
                'city-state-place-holder': '.zip-city-other-place-holder'
            }, options);

            $("#" + params['city-states-id']).change(function () {
                var $this = $(this);

                if ($this.val().toLowerCase() == "other") {
                    $this.fadeOut("Slow", function () {
                        $(params['city-state-place-holder']).fadeIn("Slow");

                        var selectedState = $("#" + params["city-states-id"] + " option:first").val().split(",");
                        
                        if(selectedState.length > 1){
                            selectedState = $.trim(selectedState[0]);
                        }else{
                            selectedState = "";
                        }

                        $this.ISEAddressFinder("displayCityStateInputBox", params["city-states-id"], selectedState);

                    });

                }
            });

        },
        displayCityStateInputBox: function (id, selectedState){

            var cityInputId  = "#AddressControl_txtCity";
            var stateInputId = "#AddressControl_txtState";
            var stateLabelId = "#AddressControl_lblState";
            var controlType  = "AddressControl";
             
            if( id == "billing-city-states"){
                            
                cityInputId  = "#BillingAddressControl_txtCity";
                stateInputId = "#BillingAddressControl_txtState";
                stateLabelId = "#BillingAddressControl_lblState";
                controlType  = "BillingAddressControl";
            }

            if( id == "shipping-city-states" ){

                cityInputId  = "#ShippingAddressControl_txtCity";
                stateInputId = "#ShippingAddressControl_txtState";
                stateLabelId = "#ShippingAddressControl_lblState";
                controlType  = "ShippingAddressControl";
            }

            $(cityInputId).removeClass("city-width-if-no-state");
            $(cityInputId).removeClass("required-input");

            $(stateInputId).removeAttr("disabled");
            $(stateInputId).removeClass("display-none");
            $(stateInputId).removeClass("control-disabled");

            if(selectedState != ""){
               $(stateInputId).val(selectedState);
               $(stateLabelId).addClass("display-none");
            }else{
               $(stateInputId).val("");
               $(stateLabelId).removeClass("display-none");
            }

            $(stateInputId).addClass("skip");
            HideStateInputBoxForCountryWithState(controlType);

        },
        renderCityState: function (settings) {

            var $this = $(this);

            var country = $(settings['country-id']).val();
            var postal  = $(settings['postal-id']).val();

            var params  = [postal, settings['city-state-place-holder'], settings['enter-postal-label-place-holder'], settings['postal-id']];

            if ($this.ISEAddressFinder("isCityStatesVisible", params)) {

                var formatIsInvalid = $this.ISEAddressFinder("isPostalFormatInvalid", { 'country': country, 'postal': postal });
    
                if (!formatIsInvalid) {
                    $(settings['postal-id']).removeClass('invalid-postal');
                    $this.ISEAddressFinder("searchForCityAndState", settings);

                } else {
                    $this.ISEAddressFinder("highlightsError", { "control-id": settings['postal-id'], "error-type": "invalid-postal-format", "focus": true, "enter-caption-place-holder-id": settings['enter-postal-label-place-holder'] });
                }
            }

        },
        searchForCityAndState: function (settings) {

            var successFunction = function (result) {

                $(settings["postal-id"]).removeClass("current-object-on-focus");
                if (result.d != "" && result.d != "no-active-postal") {
                        
                    var renderHTML = "<select id='" + settings['city-states-id'] + "' class='light-style-input'>";

                    renderHTML += result.d;
                    renderHTML += "<option value='other'>" + _OtherOption + "</option>";
                    renderHTML += "</select>";

                    $(settings['city-state-place-holder']).fadeOut("Slow", function () {
                        $(settings['enter-postal-label-place-holder']).html(renderHTML);
                    });
           
                    $(settings['postal-id']).removeClass("invalid-postal-zero");
                    $(settings['postal-id']).removeClass("undefined-city-states");
                    $("#ise-message-tips").fadeOut("slow");

                    $(this).ISEAddressFinder("bindCityStateOnChange", { "city-states-id": settings['city-states-id'], "city-state-place-holder": settings['city-state-place-holder'] });

                } else {
                        
                    /* if postal is not found the following code segments does:
                    1. verify if country is searchable: 
                    2. if #1 is true: highlights postal inputs
                    3. if #1 is false: show hidden controls: city and state */

                    var _SelectedIndex = $(settings['country-id']).prop("selectedIndex");
                         
                    if ($this.ISEAddressFinder("isCountrySearchable", { "country-id": settings['country-id'], "selected-index": _SelectedIndex }) && result.d != "no-active-postal" && !$(settings["postal-id"]).hasClass("skip")) { 
                       
                        $(settings['enter-postal-label-place-holder']).html(_EnterPostalForCityState); 
                        $(settings['postal-id']).addClass("invalid-postal-zero");
                    
                     }else{
                            
                        var citystates = $(settings["city-states-id"]).val();

                        if (typeof (citystates) == "undefined") {

                            $(settings["city-state-place-holder"]).fadeIn("Slow");
                            var cityStatesId = "";
                            var stateLabelId = "";

                            switch(settings["state-id"]){
                                case "#ShippingAddressControl_txtState":
                                    citystatesId = "shipping-city-states";
                                    stateLabelId = "ShippingAddressControl_lblState";
                                    break;
                                case "#BillingAddressControl_txtState":
                                    citystatesId = "billing-city-states";
                                    stateLabelId = "BillingAddressControl_lblState";
                                    break;
                                default:
                                    citystatesId = "city-states";
                                    stateLabelId = "AddressControl_lblState";
                                    break;
                                }

                            $(settings["enter-postal-label-place-holder"]).html("<input type='hidden' id='" + citystatesId + "' value='other'/>");

                            if($this.ISEAddressFinder("isCountryHasState", { "country-id": settings['country-id'], "selected-index": _SelectedIndex })){
                                   
                                $(settings["state-id"]).removeClass("display-none");
                                $(settings["city-id"]).removeClass("city-width-if-no-state");
                                    
                            }else{
                                    
                                $("#" + stateLabelId).addClass("display-none");

                                $(settings["state-id"]).val("");
                                $(settings["state-id"]).addClass("display-none");  
                                
                                $(settings["city-id"]).addClass("city-width-if-no-state");
                                $(settings["city-id"]).focus();
                            }

                        } else {

                            $(settings["city-states-id"]).fadeOut("Slow", function () { $(settings["city-state-place-holder"]).fadeIn("Slow"); });
                    
                        }              
                }
             }
            };

            var errorFunction = function (result) {  console.log(result.d); };

            var data = new Object();
            data.countryCode = $(settings['country-id']).val();
            data.postalCode = global_postal;
            data.stateCode = $(settings['state-id']).val();

            AjaxCallCommon("ActionService.asmx/GetCity", data, successFunction, errorFunction);

        },
        verifyStateCode: function (settings) {

            var successFunction = function (result) {

                if (result.d == false) {
                    $(settings['state-id']).addClass("state-not-found");
                } else {
                    $(settings['state-id']).removeClass("state-not-found");
                }

                return result.d;
            };

            var errorFunction = function () { return 0 };

            var data = new Object();
            data.countryCode =  $(settings['country-id']).val();
            data.postalCode = $(settings['postal-id']).val();
            data.stateCode = $(settings['state-id']).val();

            AjaxCallCommon("ActionService.asmx/IsStateCodeValid", data, successFunction, errorFunction);

        },
        lookForCorrectPostal: function ( options ){
            
              var params = $.extend({
                'country'   : '',
                'state'     : '',
                'postal'    : '',
                'city-id'   : '',
                'postal-id' : '',
                'state-id'  : ''
            }, options);

             _IsAddressVerificationAtShippingPostal = true;
     
             showPostalSearchEngineDialog(postalCode, state, country, 1);
             addressDialogControlsInit();

        }, 
        removeErrorTagsOnAddressControl: function (settings) {

            $(settings["city-id"]).removeClass("required-input");

            $(settings["state-id"]).removeClass("required-input");
            $(settings["state-id"]).removeClass("state-not-found");

            $(settings["postal-id"]).removeClass("required-input");
            $(settings["postal-id"]).removeClass("invalid-postal");
            $(settings["postal-id"]).removeClass("current-object-on-focus");

            $(settings["postal-id"]).removeClass("state-not-found");
            $(settings["postal-id"]).removeClass("invalid-postal-zero");

            $(settings["postal-id"]).removeClass("state-not-found");
            $(settings["postal-id"]).removeClass("undefined-city-states");

            $("#ise-message-tips").fadeOut("slow");

        }
    };


    $.fn.ISEAddressFinder = function (method) {

        if (methods[method]) {

            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));

        } else if (typeof method === 'object' || !method) {

            return methods.init.apply(this, arguments);

        } else {

            $.error('Method ' + method + ' does not exist on jQuery.tooltip');

        }

    };

})(jQuery);