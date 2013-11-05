// used in overriding value assignment to your address control //

var _postalId    = "";
var _cityId      = "";
var _statesId    = "";
var _countryId   = "";

var _AddressType = "";

// <--

var labelFadeInColor  = '#999999';
var labelFadeOutColor = '#cccccc';

/* The following variable are GLOBAL and values are set on InitPageAppConfigs and SetContsGlobalVariableValues: see jquery.cbe.bubble.message.js*/

var _addressTypeCaption          = "";
var _ChooseBusinessType          = "";
var _CaptchaCheckingMessage      = "";
var _DownloadingStatesMessage    = "";
var _DefaultMessage              = "";
var _DuplicateEmailMessage       = "";
var _EnterPostalForCityState     = "";
var _GetCityStateEnterMessage    = "";
var _IncompleteMessage           = "";
var _InvalidEmailMessage         = "";
var _InvalidCaptchaMessage       = "";
var _InvalidStateCodeMessage     = "";
var _InvalidPostalFormatMessage  = "";
var _Over13Message               = "";
var _TermAndConditionMessage     = "";

var _PostalNotFoundMessage       = "";
var _ProfileCheckingMessage      = "";
var _PostalMoreMessage           = "";
var _PasswordLengthIsNotValid    = "";
var _PasswordNotMatch            = "";
var _ProcessingOrderMessage      = "";
var _RefreshingProfileMessage    = "";
var _SalutationDefaultValue      = "";
var _SavingAccountMessage        = "";
var _SavingAddressMessage        = "";
var _SavingLeadMessage           = "";
var _SavingShippingInfoMessage   = "";
var _SavingShippingMethodMessage = "";
var _SearchingPostalMessage      = "";
var _SelectAddressTypeMessage    = "";
var _SelectBusinessTypeMessage   = "";
var _SendingCaseMessage          = "";
var _SentMessage                 = "";
var _StrongPasswordMessage       = "";
var _UpdatingAddressMessage      = "";
var _UpdatingProfileMessage      = "";
var _VerifyingBillingInfoMessage = "";
var _VerifyingAddressMessage     = "";
var _NoShippingMethodSelectedMessage = "";

var _SelectYourResidenceTypeMessage = "Select your Residence Type";
var _OtherOption    = "Other";

var _WholeSale      = "";
var _Retail         = "";
var _PostalCode     = "";
var _prevPostal     = "";
var _currentPostal  = "";
var _prevObjectType = "";

var _leadFormHeadSubText = "";
var _leadFormLeadDuplicatesMessage = "";

var _SavedCreditCardAs = "";
var _SavedThisCreditCardInfo = "";

// App configs 

var _IsCaptchaRequired             = false;
var _IsVatEnablead                 = false;
var _IsShowTaxFieldOnRegistration  = false;
var _IsAllowShippingAddress        = false;
var _IsAge13Required               = false;
var _IsAllowDuplicateCustomer      = false;
var _IsUPSFedExAddressVerification = false;
var _IsStrongPassword              = false;
var _IsRequireTermsAndConditionsAtCheckout = false;

var _CustomerSPExpression = "";
var _CaptchaCounter       = 1;

var countrySelectedIndex = 0;
var captchaCounter       = 0;

$(document).ready(function () {

    DisplayErrorMessagePlaceHolder();

    if (typeof ($('#btnContinueCheckout1')) != "undefined") {

        $('#btnContinueCheckout1').click(function () {

            $(this).css('display', 'none');
            $('#place-order-button-container').fadeIn('slow');

        });
    }

});

function addressDialogControlsInit() {

    $("#postal-search-go").unbind("click").click(function () {

        var searchText = $("#postal-search-text").val();

        var page = "1";
        var currentIndex = 1;
        var status = "";

        searchPostalCode(page, currentIndex, searchText);

    });

    $("#postal-search-viewl-all").unbind("click").click(function () {

        var searchText = "";

        var page = "1";
        var currentIndex = 1;
        var status = "";
        searchPostalCode(page, currentIndex, searchText);

        $("#postal-search-text").val(searchText);
        $("#postal-search-text").css("font-style", "italic");
        $("#postal-search-text").css("color", "#999999");

    });

    $("#postal-search-text").unbind("keypress").keypress(function (event) {

        $(this).css("font-style", "normal");
        $(this).css("color", "#000000");

        if (event.which == 13) {

            var searchText = $("#postal-search-text").val();

            var page = "1";
            var currentIndex = 1;

            var section = $("#address-section").val();

            searchText = cleanSearchString(searchText);
            $(this).val(searchText);
            searchPostalCode(page, currentIndex, searchText);
        }

    });

}

function searchPostalCode(page, currentIndex, searchText) {

    $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>" + _SearchingPostalMessage + "</div></div>");

    var country = $("#" + _AddressType + "AddressControl_drpCountry").val();
    var postalCode = "";
    var stateCode = "";

    var successFunction = function (result) {
        $("#search-results").html(result.d);
        initSearchEngineRowEvents("");

        var searchParam = postalCode + "::" + "" + "::" + country + "::" + searchText + "::" + page;
        renderGridPagination(searchParam, currentIndex);
        checkIfResultPanelHasScrollbar();
    };

    var errorFunction = function (result) {
        $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>" + result.d + "</div></div>");
     };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = stateCode;
    data.searchString = searchText;
    data.exactMatch = $("#address-verifcation-search-exact-word").is(":checked");
    data.pageNumber = page;

    AjaxCallCommon("ActionService.asmx/GetAddressList", data, successFunction, errorFunction);

}


function renderGridPagination(searchParam, currentIndex) {

    var items = $("#results-found").html();
    $("#records-found").html(items + " " + $("#records-found-label").html());

    var spRowsLimit = 100;
    var pages = items / spRowsLimit;
    var remainder = pages % 1;

    if (remainder > 0)  pages = Math.floor(pages) + 1;
    $("#search-pages").html("Page 1 of " + pages);

    var currentPage = 1;
    pageUrlMax = 10;
    renderPagesURL(pages, pageUrlMax, currentPage, currentIndex, searchParam);

}

function renderPagesURL(pages, pageUrlMax, currentPage, currentIndex, searchParam) {

    x = currentPage / pageUrlMax;
    x = Math.ceil(x);
    pager = (pageUrlMax * x) - pageUrlMax;

    var pagination = "<ul id='pagination-ul'>";

    if (currentIndex > 1 || pager > 1) {

        pagination += "<li onClick='downloadPage(\"forward\", " + pages + "," + pageUrlMax + ", 0, 0, \"" + searchParam + "\")'  title='Show page 1 of " + pages + "' id='first-page' >|<</li>";
        pagination += "<li onClick='downloadPage(\"backward\", " + pages + "," + pageUrlMax + "," + (currentIndex - 1) + "," + (currentPage - 1) + ", \"" + searchParam + "\")' title='Show page " + (currentPage - 1) + " of " + pages + "' id='prev-page'><<</li>";

    } else {

        pagination += "<li class='pages-url-disabled'>|<</li>";
        pagination += "<li class='pages-url-disabled'><<</li>";

    }

    for (var i = 1; i <= pageUrlMax; i++) {

        var pageText = i + pager;

        if (i > pages || pages == 1 || pageText > pages) {


            pagination += "<li class='pages-url-disabled'>" + pageText + "</li>";

        } else {


            if (i == currentIndex) {

                pagination += "<li id='page-" + i + "'  class='selected-page'>" + pageText + "</li>";

            } else {

                var thisPage = pageText - 1;
                var thisIndex = i - 1;

                pagination += "<li onClick='downloadPage(\"forward\", " + pages + "," + pageUrlMax + "," + thisIndex + "," + thisPage + ", \"" + searchParam + "\")' id='page-" + i + "'  class='pages-url'>" + pageText + "</li>";

            }

        }
    }


    if (pages <= pageUrlMax || pageText >= pages) {

        pagination += "<li class='pages-url-disabled'>>></li>";
        pagination += "<li class='pages-url-disabled'>>|</li>";

    } else {

        var lastPage = pages - 1;
        var lastIndex = lastPage % pageUrlMax;

        pagination += "<li onClick='downloadPage(\"forward\", " + pages + "," + pageUrlMax + "," + currentIndex + "," + currentPage + ", \"" + searchParam + "\")' title='Show page " + (currentPage + 1) + " of " + pages + "'  id='next-page'>>></li>";
        pagination += "<li onClick='downloadPage(\"forward\", " + pages + "," + pageUrlMax + ", " + lastIndex + ", " + lastPage + ", \"" + searchParam + "\")' title='Show page " + pages + " of " + pages + "'  id='last-page'>>|</li>";

    }

    pagination += "</ul>";

    $("#search-result-pagination").html("<div>" + pagination + "</div><div class='float-right'><input type='button' value='Done' class='btn btn-primary address-dialog-done-button' onClick='closeModalDialog(\"postal-search-engine\")'></div><div class='clear-both'></div>");

}


function downloadPage(type, pages, pageUrlMax, currentIndex, currentPage, searchParams) {

    var x = 0;
    var p = 0;
    var newP = false;

    if (currentIndex >= pageUrlMax) {
        currentIndex = 0;
    }

    if (type == "forward") {
        currentIndex++;
        currentPage++;
    } else {
        if (currentIndex <= 0) {
            currentIndex = pageUrlMax;
        }
    }

    $("#search-pages").html("Downloading Page " + currentPage + " of " + pages + ", this may take a while please wait...");

    var successFunction = function (result) {
        $("#search-results").html(result.d);
        $("#search-pages").html("Page " + currentPage + " of " + pages);

        initSearchEngineRowEvents("");
        checkIfResultPanelHasScrollbar();

        renderPagesURL(pages, pageUrlMax, currentPage, currentIndex, searchParams);

    };

    var errorFunction = function (result) {
        $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>" + result.d + "</div></div>");
    };

    var data = new Object();
    var list = searchParams.split("::");

    data.countryCode = list[2];
    data.postalCode =  list[0];
    data.stateCode =  list[1];
    data.searchString = list[3];
    data.exactMatch = $("#address-verifcation-search-exact-word").is(":checked");
    data.pageNumber = currentPage;


    AjaxCallCommon("ActionService.asmx/GetAddressList", data, successFunction, errorFunction);
}


function initSearchEngineRowEvents(withStates) {

    $(".list-row").hover(

        function () {

            var index = $(this).index();

            $(".item-" + index + "-postal-code").addClass("hoverStyle");
            $(".item-" + index + "-city").addClass("hoverStyle");
            $(".item-" + index + "-country-code").addClass("hoverStyle");
            $(".item-" + index + "-county").addClass("hoverStyle");
            $(".item-" + index + "-state-code").addClass("hoverStyle");

        },
        function () {

            var index = $(this).index();

            $(".item-" + index + "-postal-code").removeClass("hoverStyle");
            $(".item-" + index + "-city").removeClass("hoverStyle");
            $(".item-" + index + "-country-code").removeClass("hoverStyle");
            $(".item-" + index + "-county").removeClass("hoverStyle");
            $(".item-" + index + "-state-code").removeClass("hoverStyle");

        }

    );

    $(".list-row").unbind("click").click(function () {

        var index = $(this).index();

        $(".selected-row").removeClass("selected-row");

        $(".item-" + index + "-postal-code").removeClass("hoverStyle");
        $(".item-" + index + "-city").removeClass("hoverStyle");
        $(".item-" + index + "-country-code").removeClass("hoverStyle");
        $(".item-" + index + "-county").removeClass("hoverStyle");
        $(".item-" + index + "-state-code").removeClass("hoverStyle");


        $(".item-" + index + "-postal-code").addClass("selected-row");
        $(".item-" + index + "-city").addClass("selected-row");
        $(".item-" + index + "-country-code").addClass("selected-row");
        $(".item-" + index + "-county").addClass("selected-row");
        $(".item-" + index + "-state-code").addClass("selected-row");

        var postalCode = $(".item-" + index + "-postal-code").html();
        var city = $(".item-" + index + "-city").html();

        var copyToShipping          = "";
        var idOfControlBeingHandled = "";
        var control                 = "";
        var idSuffix                = "";
        updateAddressInputValues(idSuffix, withStates, postalCode, city);

    });

    $(".list-row").unbind("dblclick").dblclick(function () {

        $("#postal-search-engine").dialog("destroy");

    });

}

function closeModalDialog(dialogId) {
    $("#" + dialogId).dialog("close");
}

function showPostalSearchEngineDialog(postalCode, state, country, page) {

    $("#postal-search-engine").dialog({
        autoOpen: false,
        width: 651,
        modal: true,
        resize: false
    });

    $("#postal-search-engine").dialog("open");

    $("#postal-search-text").val("");
    $("#postal-search-text").focus();

    $(".ui-dialog-buttonpane").append("<div id='search-result-pagination'></div>");
    $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>" + _SearchingPostalMessage + "</div></div>");

    var successFunction = function (result) {

        $("#search-results").html(result.d);
        initSearchEngineRowEvents(state);

        var searchText = postalCode;
        var searchParam = postalCode + "::" + state + "::" + country + "::" + searchText + "::" + page;

        renderGridPagination(searchParam, 1);

        var stateCodeCountry = $("#results-state-country").html();
        $("#state-country").html(stateCodeCountry);

        checkIfResultPanelHasScrollbar();

    };

    var errorFunction = function (result) {
        $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>" + result.d + "</div></div>");
    };

    var data = new Object();
    data.countryCode = country;
    data.postalCode = postalCode;
    data.stateCode = state;
    data.searchString = "";
    data.exactMatch = false;
    data.pageNumber = page;

    AjaxCallCommon("ActionService.asmx/GetAddressList", data, successFunction, errorFunction);

}

function updateAddressInputValues(idSuffix, withStates, postalCode, city) {


    _cityId   = _AddressType + "AddressControl_txtCity";
    _postalId = _AddressType + "AddressControl_txtPostal";
    _stateId  = _AddressType + "AddressControl_txtState";

    $("#" + _cityId).val(city);
    $("#" + _cityId).removeClass("required-input");

    $("#" + _AddressType + "AddressControl_lblCity").addClass("display-none");

    $("#" + _postalId).val(postalCode);
    $("#" + _postalId).removeClass("invalid-postal");
    $("#" + _postalId).removeClass("required-input");
    $("#" + _postalId).removeClass("invalid-postal-many");
    $("#" + _postalId).removeClass("invalid-postal-zero");
    $("#" + _postalId).addClass("postal-is-corrected");
    $("#" + _postalId).removeClass("undefined-city-states");
      
    $("#" + _AddressType + "AddressControl_lblPostal").addClass("display-none");

    if (IsCountrySearchable("#" + _AddressType + "AddressControl_drpCountry") == "true") {

        var focusOnControl = false;
        var formSubmission = false;
        var skipStateValidation = true;

        ValidateAddressDetails(focusOnControl, false, true, _AddressType, ""); // --> see usercontrol.address.control.js 
    }


    var hasStateInputBox = $("#" + _stateId).val();
    var stateCode        = $(".selected-row").parent("div").attr("data-stateCode");

      
    if ( typeof (hasStateInputBox) != "undefined") {

        $("#" + _stateId).removeClass("state-not-found");
        $("#" + _stateId).removeClass("required-input");
        $("#" + _stateId).val($.trim(""));

        if (stateCode != "") {
            $("#" + _AddressType + "AddressControl_lblState").addClass("display-none");
            $("#" + _stateId).val($.trim(stateCode));
        } else {
            $("#" + _AddressType + "AddressControl_lblState").removeClass("display-none");
        }
    } 

    $("#ise-message-tips").fadeOut("slow");

}

function checkIfResultPanelHasScrollbar() {

    var scrollHeight = $("#search-results").get(0).scrollHeight;
    var listingHeight = $("#search-results").get(0).clientHeight;

    var count = $("#results-found").html();

    for (var i = 0; i <= count; i++) {
       if (scrollHeight <= listingHeight) $(".item-" + i + "-state-code").css("width", "260px");
    }
}

function cleanSearchString(text) {

    var str = $.trim(text);

    str = str.replace(/[\+\-\s,]/g, " ");
    str = str.replace(/\s+/g, " ");
    str = str.replace(/[\^\$\'\"\|\{\}\=\)\(\.*\!\@\%\&\;\:\#\.\?]/g, "");

    return str;
}

function trim(str, chars) {
    return ltrim(rtrim(str, chars), chars);
}

function ltrim(str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
}

function rtrim(str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
}


// -> old jscript code (made minor changes)
// -> used in createaccount.aspx

function doAddressVerificationAfterSubmit() {

    var billingStates = $("#ctrlBillingAddress_WithStateState").val();
    var shippingStates = $("#ctrlShippingAddress_WithStateState").val();

    if (billingStates == null || billingStates == "") {

        $("#ctrlBillingAddress_WithStateState").css("display", "none");

    }

    if (shippingStates == null || shippingStates == "") {

        $("#ctrlShippingAddress_WithStateState").css("display", "none");

    }

    var chkAddrInfo = $("#hidCheck").val();

    var strBTitl = $("#hidBillTitle").val();
    var strSTitl = $("#hidShipTitle").val();

    var strBChck = $("#hidBillCheck").val();
    var strSChck = $("#hidShipCheck").val();

    var strBAddr = $("#hidBilling").val();
    var strSAddr = $("#hidShipping").val();

    // NOTE: '*' character is used for seperation of information
    //       which will be parsed at the receiving end.

    var strBillCtrl = $("#hidBillCtrl").val();
    var arrBillCtrl = strBillCtrl.split("*");


    // Get WithState or WithOutState Billing Address Control id's

    var _hidBlnWithState = $("#hidBlnWithState").val();

    if (_hidBlnWithState == "True") {

        var citBillCtrl = document.getElementById(arrBillCtrl[0]);
        var posBillCtrl = document.getElementById(arrBillCtrl[1]);


    }
    else {

        var citBillCtrl = document.getElementById(arrBillCtrl[3]);
        var posBillCtrl = document.getElementById(arrBillCtrl[4]);

    }

    var varBillStateCtrl = document.getElementById(arrBillCtrl[2]);

    var strShipCtrl = document.getElementById('hidShipCtrl');
    var arrShipCtrl = strShipCtrl.value.split("*");

    // Get WithState or WithOutState Shipping Address Control id's

    var _hidShpWithState = $("#hidShpWithState").val();

    if (_hidShpWithState == "True") {

        var citShipCtrl = document.getElementById(arrShipCtrl[0]);
        var posShipCtrl = document.getElementById(arrShipCtrl[1]);

    }
    else {

        var citShipCtrl = document.getElementById(arrShipCtrl[3]);
        var posShipCtrl = document.getElementById(arrShipCtrl[4]);
    }

    var varShipStateCtrl = document.getElementById(arrShipCtrl[2]);

    var _chkAddrInfo = $("#chkAddrInfo").val();

    if (_chkAddrInfo == "True" && varBillStateCtrl != null) {

        varShipStateCtrl.value = varBillStateCtrl.value;
        citShipCtrl.value = citBillCtrl.value;
        posShipCtrl.value = posBillCtrl.value;

    }

    var popModalFlag = $('#hidValid').val();

    if (popModalFlag != null && popModalFlag == "false") {

        var ids = getControlIdsToCheckAfterSubmit();

        for (var i = 0; i < ids.length; i++) {

            recheckPostalAfterSubmit(ids[i], "", "");

        }

    }
}

// old jscript code (made minor changes) <--


// scripts for new address and form validation -->


function FindPostal(addressType) {
    
    _AddressType = addressType;

    var country = "";
    var state = "";
    var postalCode = "";

    switch (addressType) {
        case "Shipping":

            country = $("#ShippingAddressControl_drpCountry").val();

            break;
        case "Billing":

            country = $("#BillingAddressControl_drpCountry").val();

            break;
        default:

            country = $("#AddressControl_drpCountry").val();

            break;
    }

    showPostalSearchEngineDialog(postalCode, state, country, 1);
    addressDialogControlsInit();

}


/*

    The following functions is for create account and edit account (profile.aspx) 
    note: this is not related to address verification dialog

*/


function DisplayErrorMessagePlaceHolder() {

    /* This function is action on .ready() 
    
    1 Checks if errorSummary_Board_Error li controls is greater than zero
    2. If condition #1 is true slide down errorSummary (div html control)

    */

    if ($("#errorSummary_Board_Errors").children("li").length > 0) {

        $("#errorSummary").slideDown("slow", function () { });
        $("#error-summary-clear").fadeIn("fast");
    }

}

function IsRequiredInformationAllGood() {

    var counter = 0;
    var IsGood = true;
    var skip = false;

    $(".requires-validation").each(function () {   //-> scan all html controls with class .apply-behind-caption-effects

        skip = false;

        if ($(this).attr("disabled") == "disabled") skip = true;

        if ($(this).val() == "" && skip == false) {


            var object = this;
            var thisObjectId = "#" + $(object).attr("id");

            $(this).removeClass("current-object-on-focus");
            $(this).addClass("required-input");
            /* Points mouse cursor on the first input with no value to render bubble message */

            if (counter == 0) {

                thisObjectId = "#" + $(this).attr("id");
                $(this).addClass("current-object-on-focus");
                $(this).focus();

            }

            counter++;
            IsGood = false;
        }
    });

    return IsGood;
}

function SkipValidationOnPostal(addressType) {
    var $CityId = $("#" + addressType + "AddressControl_txtCity");
    var $PostalId = $("#" + addressType + "AddressControl_txtPostal");
    var $StateId = $("#" + addressType + "AddressControl_txtState");


    $PostalId.addClass("skip");
    $PostalId.removeClass("invalid-postal").removeClass("invalid-postal-zero").removeClass("required-input").removeClass("invalid-postal-many").removeClass("invalid-postal-zero").removeClass("undefined-city-states");

    var city = $.trim($CityId.val());
    var stateCode = $.trim($StateId.val());
    var postalCode = $.trim($PostalId.val());

    if (city == "") {
        $CityId.val("").removeClass("required-input");
        $("#" + addressType + "AddressControl_lblCity").removeClass("display-none");
    }

    if (stateCode == "") {
        $StateId.removeClass("required-input").val("");
        $("#" + addressType + "AddressControl_lblState").removeClass("display-none");
    }

    if (postalCode == "") {
        $PostalId.removeClass("required-input").val("");
        $("#" + addressType + "AddressControl_lblState").removeClass("display-none");
    }

    $StateId.removeClass("state-not-found").addClass("skip").removeAttr("disabled").removeClass("display-none").removeClass("control-disabled");
    $CityId.removeClass("city-width-if-no-state");


    HideStateInputBoxForCountryWithState(addressType + "AddressControl");

    if (addressType != "") addressType += "-";

    $("#" + addressType.toLowerCase() + "enter-postal-label-place-holder").html("<input type='hidden' id='" + addressType.toLowerCase() + "city-states' value='other'/>");
    $("." + addressType.toLowerCase() + "zip-city-other-place-holder").fadeIn("Slow");

    $("#ise-message-tips").fadeOut("slow");
    $CityId.focus();
}