var EmptyString = "";

// used in overriding value assignment to your address control //

var overrideThis = EmptyString;
var overrideIds = EmptyString;
var overrider = EmptyString;

var _postalIndex = 0;
var _cityIndex = 1;
var _statesIndex = 2;
var _countryIndex = 3;

var _postalId = EmptyString;
var _cityId = EmptyString;
var _statesId = EmptyString;
var _countryId = EmptyString;

var _IsAddressVerificationAtShippingPostal = false;

// <--

var copyBillingToShipping = false;

$("document").ready(function () {

    //--> Not related to address verification, added for checkoutreview.aspx to hide place order button on click event

    $('#btnContinueCheckout1').click(function () {

        $(this).css('display', 'none');
        $('#place-order-button-container').fadeIn('slow');

    });


    //<--

    //--> Address verification control events

    /*
    The following values are being used if address verification js can't suit your address controls id 
     
    */

    overrideThis = $("#overides-verification-dialog-values-assignment").html();

    if (overrideThis == 'true') {

        overrideIds = $("#overides-custom-ids").html();
        overrider = $("overides-page-name").html();

        var ids = overrideIds.split(",");

        _postalId = ids[_postalIndex];
        _cityId = ids[_cityIndex];
        _statesId = ids[_statesIndex];
        _countryId = ids[_countryIndex];

    }
    /* -- */

    controlEventsInit();

    //<--

});

function controlEventsInit() {

    var postalCode = "";
    var state = "";
    var country = "";
    var city = "";

    $(".requires-address-validation").focus(function () {

        id = $(this).attr("id");
        $("#" + id).removeClass("invalid-address-field");

    });

    $(".requires-address-validation").blur(function () {

        doAddressVerification(this);

    });

    $(".address-control-country").change(function () {

        clearControls(this);

    });


    $("#ctrlBillingAddress_WithStateState").change(function () {

        $("#ctrlBillingAddress_WithStatePostalCode").val("");
        $("#ctrlBillingAddress_WithStateCity").val("");

    });


    $("#ctrlShippingAddress_WithStateState").change(function () {

        $("#ctrlShippingAddress_WithStatePostalCode").val("");
        $("#ctrlShippingAddress_WithStateCity").val("");

    });


}

function addressDialogControlsInit() {

    $('#postal-search-text').click(function () {

        var text = $(this).val();

        if (text == "Search Address") {

            $(this).val("");
            $(this).css("font-style", "normal");
            $(this).css("color", "#000000");

        }

    });

    $("#postal-search-text").blur(function () {

        var text = $(this).val();

        if (text == "") {

            $(this).val("Search Address");

            $(this).css("font-style", "italic");
            $(this).css("color", "#999999");

        } else {

            $(this).css("font-style", "normal");
            $(this).css("color", "#000000");

        }

    });


    $("#postal-search-go").click(function () {

        var searchText = $("#postal-search-text").val();

        var page = "1";
        var currentIndex = 1;
        var status = "";

        var idOfControlBeingHandled = EmptyString;

        if (overrideThis != "true") {

            var idOfControlBeingHandled = $(".current-control-being-verified").attr("id");

        }

        searchPostalCode(idOfControlBeingHandled, page, currentIndex, searchText);

    });


    $("#postal-search-viewl-all").click(function () {

        $("#postal-search-text").val(EmptyString);

        var searchText = EmptyString;

        var page = "1";
        var currentIndex = 1;
        var status = EmptyString;

        var idOfControlBeingHandled = EmptyString;

        if (overrideThis != "true") {

            var idOfControlBeingHandled = $(".current-control-being-verified").attr("id");

        }


        searchPostalCode(idOfControlBeingHandled, page, currentIndex, searchText);

        $("#postal-search-text").val("Search Address");

        $("#postal-search-text").css("font-style", "italic");
        $("#postal-search-text").css("color", "#999999");

    });


    $("#postal-search-text").keypress(function (event) {

        $(this).css("font-style", "normal");
        $(this).css("color", "#000000");

        if (event.which == 13) {

            var searchText = $("#postal-search-text").val();

            var page = "1";
            var currentIndex = 1;

            var section = $("#address-section").val();

            searchText = cleanSearchString(searchText);
            $(this).val(searchText);

            var idOfControlBeingHandled = EmptyString;

            if (overrideThis != "true") {

                var idOfControlBeingHandled = $(".current-control-being-verified").attr("id");

            }

            searchPostalCode(idOfControlBeingHandled, page, currentIndex, searchText);
        }

    });

}

function searchPostalCode(idOfControlBeingHandled, page, currentIndex, searchText) {

    var control = EmptyString;
    var idSuffix = EmptyString;

    var postalCode = EmptyString;
    var city = EmptyString;
    var state = EmptyString;
    var country = EmptyString;

    if (overrideThis == "true") {

        /* 

        for old address control

        state      = $("#" + _statesId).val();
        country    = $("#" + _countryId).val();
        postalCode = $("#" + _postalId).val();
        city       = $("#" + _cityId).val();
     
        */
        country = $("#AddressControl_CountryDropDownList").val();
        if (_IsAddressVerificationAtShippingPostal) country = $("#ShippingAddressControl_shipping_country_input_select").val();

        postalCode = "";
        state = "";
        city = "";

    } else {

        control = idOfControlBeingHandled.split("_");
        idSuffix = getIdSuffix(control);

        state = $("#" + idSuffix + "WithStateState").val();
        country = $("#" + idSuffix + "Country").val();

        if (state != null && state != "" && typeof (state) != undefined) {

            postalCode = $("#" + idSuffix + "WithStatePostalCode").val();
            city = $("#" + idSuffix + "WithStateCity").val();

        } else {

            state = "";
            postalCode = $("#" + idSuffix + "WithoutStatePostalCode").val();
            city = $("#" + idSuffix + "WithoutStateCity").val();
        }

    }

    var list = [postalCode, state, country, searchText, page];
    var jsonText = JSON.stringify({ list: list, task: "search-postal-code" });

    $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>Searching, this may take a while please wait...</div></div>");

    jQuery.ajax({
        type: "POST",
        url: "ActionService.asmx/AddressTaskSelector",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            $("#search-results").html(msg.d);
            initSearchEngineRowEvents(state);

            var searchParam = postalCode + "::" + state + "::" + country + "::" + searchText + "::" + page;

            renderGridPagination(searchParam, currentIndex);

            checkIfResultPanelHasScrollbar();
        },
        fail: function (result) {

            $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>An error has been encountered, while doing a search. Please try again.</div></div>");

        }

    });

}


function renderGridPagination(searchParam, currentIndex) {

    var items = $("#results-found").html();
    $("#records-found").html(items + " " + $("#records-found-label").html());

    var spRowsLimit = 100;
    var pages = items / spRowsLimit;
    var remainder = pages % 1;

    if (remainder > 0) {

        pages = Math.floor(pages) + 1;
    }


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

    $("#search-result-pagination").html(pagination);

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

    var list = searchParams.split("::");

    var postalCode = list[0];
    var state = list[1];
    var country = list[2];
    var searchText = list[3];
    var page = currentPage;

    var list = [postalCode, state, country, searchText, page];
    var jsonText = JSON.stringify({ list: list, task: "search-postal-code" });

    $("#search-pages").html("Downloading Page " + page + " of " + pages + ", this may take a while please wait...");

    jQuery.ajax({
        type: "POST",
        url: "ActionService.asmx/AddressTaskSelector",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            $("#search-results").html(msg.d);
            $("#search-pages").html("Page " + page + " of " + pages);

            initSearchEngineRowEvents(state);

            checkIfResultPanelHasScrollbar();
        },
        fail: function (result) {

            $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>An error has been encountered, while doing a search. Please try again.</div></div>");

        }

    });

    renderPagesURL(pages, pageUrlMax, currentPage, currentIndex, searchParams);
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

    $(".list-row").click(function () {

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


        var copyToShipping = EmptyString;
        var idOfControlBeingHandled = EmptyString;
        var control = EmptyString;
        var idSuffix = EmptyString;

        if (overrideThis != "true") {

            copyToShipping = $("#chkCopyBillingInfo").is(':checked') || $("#chkSameAsBilling").is(':checked');
            idOfControlBeingHandled = $(".current-control-being-verified").attr("id");
            control = idOfControlBeingHandled.split("_");
            idSuffix = "#" + getIdSuffix(control);
        }

        updateAddressInputValues(idSuffix, withStates, postalCode, city, copyToShipping, false);

    });

    $(".list-row").dblclick(function () {

        $("#postal-search-engine").dialog("destroy");

    });

}

function showPostalSearchEngineDialog(postalCode, state, country, page) {

    $("#postal-search-engine").dialog({
        autoOpen: false,
        width: 651,
        modal: true,
        resize: false,
        buttons: {
            "Done": function () {
                $(this).dialog("close");
            }
        },
        close: function () {
        }
    });

    $("#postal-search-engine").dialog("open");

    $("#postal-search-text").val("");
    $("#postal-search-text").focus();

    $(".ui-dialog-buttonpane").append("<div id='search-result-pagination'></div>");

    var list = [postalCode, state, country, page];
    var jsonText = JSON.stringify({ list: list, task: "get-postal-code-listing" });

    $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>Searching match, this may take a while please wait...</div></div>");

    jQuery.ajax({
        type: "POST",
        url: "ActionService.asmx/AddressTaskSelector",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            $("#search-results").html(msg.d);
            initSearchEngineRowEvents(state);

            var searchText = postalCode;
            var searchParam = postalCode + "::" + state + "::" + country + "::" + searchText + "::" + page;

            renderGridPagination(searchParam, 1);

            var stateCodeCountry = $("#results-state-country").html();
            $("#state-country").html(stateCodeCountry);

            checkIfResultPanelHasScrollbar();
        },
        fail: function (result) {


            $("#search-results").html("<div id='searching-panel'><div id='search-icon'><img src='images/ajax-loader.gif' style='width:25px;'></div><div id='search-loading-text'>An error has been encountered, while doing a search. Please try again.</div></div>");

        }

    });

}

function updateAddressInputValues(idSuffix, withStates, postalCode, city, copyToShipping, IsUsedInShippingControl, IsUsedInShippingControl) {

    if (overrideThis == "true") {

        if (_IsAddressVerificationAtShippingPostal) {

            _cityId = "shipping-city-input";
            _postalId = "shipping-postal-code-input";

            IsUsedInShippingControl = true;

        } else {


            _cityId = "city-input";
            _postalId = "postal-code-input";

            IsUsedInShippingControl = false;
        }

        $("#" + _cityId).val(city);

        $("#" + _cityId + "-label").addClass("support-hide");
        $("#" + _cityId).removeClass("support-invalid-input");
        $("#" + _cityId).removeClass("support-required-input");

        $("#" + _postalId).val(postalCode);

        $("#" + _postalId + "-label").addClass("support-hide");

        $("#" + _postalId).removeClass("support-invalid-postal");
        $("#" + _postalId).removeClass("support-required-input");
        $("#" + _postalId).removeClass("support-invalid-postal-many");
        $("#" + _postalId).removeClass("support-invalid-postal-zero");

        $("#" + _postalId).addClass("postal-is-corrected");

        $("#ise-message-tips").fadeOut("slow");


        if (IsCountrySearchable(IsUsedInShippingControl) == "true") {

            var focusOnControl = false;
            var formSubmission = false;
            var skipStateValidation = true;

            ValidateAddressDetails(focusOnControl, formSubmission, skipStateValidation, IsUsedInShippingControl, "");
        }

    } else {

        if (withStates != null && withStates != "") {

            $(idSuffix + "WithStateCity").val(city);
            $(idSuffix + "WithStatePostalCode").val(postalCode);

            $(idSuffix + "WithStateCity").removeClass("invalid-address-field");
            $(idSuffix + "WithStatePostalCode").removeClass("invalid-address-field");


            if (copyToShipping == true) {

                $("#ctrlShippingAddress_WithStateCity").val(city);
                $("#ctrlShippingAddress_WithStatePostalCode").val(postalCode);

                $("#ctrlShippingAddress_WithStateCity").removeClass("invalid-address-field");
                $("#ctrlShippingAddress_WithStatePostalCode").removeClass("invalid-address-field");

            }

        } else {

            $(idSuffix + "WithoutStateCity").val(city);
            $(idSuffix + "WithoutStatePostalCode").val(postalCode);

            $(idSuffix + "WithoutStateCity").removeClass("invalid-address-field");
            $(idSuffix + "WithoutStatePostalCode").removeClass("invalid-address-field");

            if (copyToShipping == true) {

                $("#ctrlShippingAddress_WithoutStateCity").val(city);
                $("#ctrlShippingAddress_WithoutStatePostalCode").val(postalCode);

                $("#ctrlShippingAddress_WithoutStateCity").removeClass("invalid-address-field");
                $("#ctrlShippingAddress_WithoutStatePostalCode").removeClass("invalid-address-field");

            }
        }
    }

}

function checkIfResultPanelHasScrollbar() {

    var scrollHeight = $("#search-results").get(0).scrollHeight;
    var listingHeight = $("#search-results").get(0).clientHeight;


    var count = $("#results-found").html();

    for (var i = 0; i <= count; i++) {

        if (scrollHeight <= listingHeight) {

            $(".item-" + i + "-state-code").css("width", "260px");

        }

    }

}


function verifyAddress(idOfControlBeingHandled) {

    var valid = true;

    var control = idOfControlBeingHandled.split("_");
    var idSuffix = "#" + getIdSuffix(control);

    var postalCode = "";
    var city = "";
    var state = $(idSuffix + "WithStateState").val();
    var country = $(idSuffix + "Country").val();

    if (state != null && state != "" && typeof (state) != undefined) {

        postalCode = $(idSuffix + "WithStatePostalCode").val();
        city = $(idSuffix + "WithStateCity").val();

    } else {

        state = "";

        postalCode = $(idSuffix + "WithoutStatePostalCode").val();
        city = $(idSuffix + "WithoutStateCity").val();
    }


    var list = [postalCode, state, country, city];
    var jsonText = JSON.stringify({ list: list, task: "verify-postal-code" });

    jQuery.ajax({
        type: "POST",
        url: "ActionService.asmx/AddressTaskSelector",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            $(".requires-address-validation").removeClass("current-control-being-verified");

            var result = result.d;
            result = result.split(",");

            var count = result[0];
            city = result[1];

            var status = false;
            var page = 1;

            switch (count) {

                case "1":

                    var idSuffix = "#" + getIdSuffix(control);

                    var copyToShipping = $("#chkCopyBillingInfo").is(':checked') || $("#chkSameAsBilling").is(':checked');

                    updateAddressInputValues(idSuffix, state, postalCode, city, copyToShipping, false);

                    $("#" + idOfControlBeingHandled).removeClass("invalid-address-field");
                    $("#" + idOfControlBeingHandled).addClass("current-control-being-verified");

                    break;
                case "0":

                    showPostalSearchEngineDialog("", state, country, page);

                    if (idOfControlBeingHandled != "" && idOfControlBeingHandled != 0 && idOfControlBeingHandled != null) {

                        $("#" + idOfControlBeingHandled).addClass("invalid-address-field");
                    }

                    $("#" + idOfControlBeingHandled).addClass("current-control-being-verified");

                    addressDialogControlsInit();

                    break;
                default:

                    showPostalSearchEngineDialog(postalCode, state, country, page);

                    if (idOfControlBeingHandled != "" && idOfControlBeingHandled != 0 && idOfControlBeingHandled != null) {

                        $("#" + idOfControlBeingHandled).addClass("invalid-address-field");
                    }

                    $("#" + idOfControlBeingHandled).addClass("current-control-being-verified");

                    addressDialogControlsInit();

                    break;
            }


        },
        fail: function (result) { }
    });

    return valid;
}

function cleanSearchString(text) {

    var str = $.trim(text);

    str = str.replace(/[\+\-\s,]/g, " ");
    str = str.replace(/\s+/g, " ");

    str = str.replace(/[\^\$\'\"\|\{\}\=\)\(\.*\!\@\%\&\;\:\#\.\?]/g, "");

    return str;

}

function recheckPostalAfterSubmit(idOfControlBeingHandled, thisObject, rowId) {

    // this function is utilized to double check postal and city after user submits the create account form
    // the function highlights both controls(city and postal) in billing and section
    // the function display an address book icon to provide option to the user to correct the city/postal values
    // there are 3 return values from SP being processed by this function: IsInvalidPostalAndCity, IsInvalidPostalOnly, and IsInvalidCityOnly

    var control = idOfControlBeingHandled.split("_");
    var idSuffix = getIdSuffix(control);

    var postalCode = "";
    var city = "";
    var state = $("#" + idSuffix + "WithStateState").val();
    var country = $("#" + idSuffix + "Country").val();

    if (state != null && state != "" && typeof (state) != undefined) {

        postalCode = $("#" + idSuffix + "WithStatePostalCode").val();
        city = $("#" + idSuffix + "WithStateCity").val();

    } else {

        state = "";

        postalCode = $("#" + idSuffix + "WithoutStatePostalCode").val();
        city = $("#" + idSuffix + "WithoutStateCity").val();
    }


    var list = [postalCode, state, country, city];
    var jsonText = JSON.stringify({ list: list, task: "verify-postal-and-city" });

    jQuery.ajax({
        type: "POST",
        url: "ActionService.asmx/AddressTaskSelector",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result.d != "" || result.d != null) $("#pnlErrorMsg").html($("#address-string-10").html());

            var thisId = "";
            var includeHideShippingScript = true;

            findAddressIcon = "<div id='" + idSuffix + "find-address' class='address-verifcation-icon-book'></div>";

            var badAddress = false;

            switch (result.d) {

                case "IsInvalidPostalAndCity":

                    if (state != "") {

                        $("#" + idSuffix + "WithStatePostalCode").addClass("invalid-address-field");
                        $("#" + idSuffix + "WithStateCity").addClass("invalid-address-field");

                    } else {

                        $("#" + idSuffix + "WithoutStateCity").addClass("invalid-address-field");
                        $("#" + idSuffix + "WithoutStatePostalCode").addClass("invalid-address-field");

                    }


                    badAddress = true;

                    break;
                case "IsInvalidPostalOnly":

                    if (state != "") {

                        $("#" + idSuffix + "WithStatePostalCode").addClass("invalid-address-field");
                        $("#" + idSuffix + "WithStateCity").removeClass("invalid-address-field");


                    } else {

                        $("#" + idSuffix + "WithoutStatePostalCode").addClass("invalid-address-field");
                        $("#" + idSuffix + "WithoutStateCity").removeClass("invalid-address-field");

                    }

                    badAddress = true;

                    break;

                case "IsInvalidCityOnly":


                    if (state != "") {

                        $("#" + idSuffix + "WithStateCity").addClass("invalid-address-field");
                        $("#" + idSuffix + "WithStatePostalCode").removeClass("invalid-address-field");


                    } else {


                        $("#" + idSuffix + "WithoutStateCity").addClass("invalid-address-field");
                        $("#" + idSuffix + "WithoutStatePostalCode").removeClass("invalid-address-field");

                    }

                    badAddress = true;

                    break;

                default:

                    if (state != "") {

                        $("#" + idSuffix + "WithStateCity").removeClass("invalid-address-field");
                        $("#" + idSuffix + "WithStatePostalCode").removeClass("invalid-address-field");

                    } else {

                        $("#" + idSuffix + "WithoutStateCity").removeClass("invalid-address-field");
                        $("#" + idSuffix + "WithoutStatePostalCode").removeClass("invalid-address-field");

                    }

                    if (thisObject != null && thisObject != "") {

                        /*
                        -> this code segments was moved from address_ajax.js
                        -> see line 967 or find function recheckPostalAfterSubmit
                        -> applicable only in multi shipping address validation
                        -> reason of moving: adding of new address to require that address control validation is true and after successful request of ActionService.asmx/AddressTaskSelector
                        */

                        var value = thisObject.addressControl.serialize();
                        var del = Function.createDelegate(thisObject, thisObject.onAddressAdded);
                        var onAddressAddedDelegate = del;

                        thisObject.disableCommands();

                        var service = new ActionService();
                        service.AddNewAddress(value, del);

                    }

                    badAddress = false;

                    break;
            }

            if (state != "" && badAddress == true) {

                if (rowId == null || rowId == "") {

                    $("#" + idSuffix + "find-address").remove();
                    $("#" + idSuffix + "WithStatePostalRow td:nth-child(2)").append(findAddressIcon);

                } else {

                    $(rowId).html("");
                    $(rowId).html(findAddressIcon);

                }

            }

            if (state == "" && badAddress == true) {

                if (rowId == null || rowId == "") {

                    $("#" + idSuffix + "find-address").remove();
                    $("#" + idSuffix + "WithoutStatePostalRow td:nth-child(2)").append(findAddressIcon);

                } else {

                    $(rowId).html("");
                    $(rowId).html(findAddressIcon);

                }

            }

            if (badAddress == true) {

                initializedAddressBooks(idSuffix, idOfControlBeingHandled);

            }

        },
        fail: function (result) {

            $("#pnlErrorMsg").html(result.d);

        }

    });

}

function doAddressVerification(thisObject) {

    // -> get the id of the current object being handled and split it by "_"
    // -> assign section 

    var idOfControlBeingHandled = $(thisObject).attr("id");

    /*
    -> verify if current object being handled is not having empty string
    -> clean string removing extra spaces, comma, plus sign see function cleanSearhString
    -> verify address see function verifyAddress
    */

    if ($(thisObject).val() != "") {

        var str = cleanSearchString($(thisObject).val());
        $(thisObject).val(str);

        verifyAddress(idOfControlBeingHandled);

    } else {

        $(thisObject).addClass("invalid-address-field");

    }

    //<---

}

function getIdSuffix(control) {

    /*
    -> get maximum index of the split string id : now as control variable array
    -> loop through the array and append its index value to string idSuffix (excluding the last array element value)
    -> see i < (maxIndex - 1 ) condition
    -> id will be used in detecting what particular section of address control is currently being validated
    */

    var maxIndex = control.length;
    var thisControlId = "";

    for (i = 0; i < (maxIndex - 1); i++) {

        thisControlId += control[i] + "_";
    }

    return thisControlId;

}

function getControlIdsToCheckAfterSubmit() {

    var controlIds = new Array();
    var section = new Array();

    $(".requires-address-validation").each(function () {

        var id = $(this).attr("id");
        var control = id.split("_");

        var inArray = 0;

        switch (control[0]) {

            case "ctrlBillingAddress":

                inArray = $.inArray("Billing", section);

                if (inArray == -1) {

                    section.push("Billing");
                    controlIds.push(id);
                }

                break;
            case "ctrlShippingAddress":

                inArray = $.inArray("Shipping", section);

                if (inArray == -1) {

                    section.push("Shipping");
                    controlIds.push(id);
                }

                break;
            case "ctrlAddress":

                inArray = $.inArray("Address", section);

                if (inArray == -1) {

                    section.push("Address");
                    controlIds.push(id);
                }

                break;
            default:
                break;

        }

    });

    return controlIds;
}

function clearControls(thisObject) {

    var control = $(thisObject).attr("id");
    var idSuffix = getIdSuffix(control);

    $("#" + idSuffix + "WithStatePostalCode").val("");
    $("#" + idSuffix + "WithStateCity").val("");

    $("#" + idSuffix + "WithoutStatePostalCode").val("");
    $("#" + idSuffix + "WithoutStateCity").val("");

    $("#" + idSuffix + "WithStatePostalCode").removeClass("invalid-address-field");
    $("#" + idSuffix + "WithStateCity").removeClass("invalid-address-field");

    $("#" + idSuffix + "WithoutStatePostalCode").removeClass("invalid-address-field");
    $("#" + idSuffix + "WithoutStateCity").removeClass("invalid-address-field");

}


function initializedAddressBooks(idSuffix, idOfControlBeingHandled) {

    // --> Event handlers for Billing-find-address and Shipping-find-address icon after successful request via ajax

    $("#" + idSuffix + "find-address").click(function () {

        state = $("#" + idSuffix + "WithStateState").val();
        country = $("#" + idSuffix + "Country").val();

        if (state == null) state = "";

        if (state != "") {

            $("#" + idSuffix + "WithStatePostalCode").val();
            $("#" + idSuffix + "WithStateCity").val();

        } else {

            $("#" + idSuffix + "WithoutStatePostalCode").val();
            $("#" + idSuffix + "WithoutStateCity").val();

        }

        verifyAddress(idOfControlBeingHandled);

    });

    // <--
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


var countrySelectedIndex = 0;
var captchaCounter = 0;

$("document").ready(function () {

    SetFormDefaultSettings();

});

function SetFormDefaultSettings() {


    $(".apply-behind-caption-effect").each(function () {

        $(this).val("");

    });


    $("#support-contact-form-left").dblclick(function () {

        $("#ise-message-tips").fadeOut("slow");

    });
}

function FindPostal(shipping_address) {

    var country = $("#AddressControl_CountryDropDownList").val();
    var state = "";
    var postalCode = "";

    if (shipping_address) {

        _IsAddressVerificationAtShippingPostal = true;
        country = $("#ShippingAddressControl_shipping_country_input_select").val();

    } else {

        _IsAddressVerificationAtShippingPostal = false;

    }


    showPostalSearchEngineDialog(postalCode, state, country, 1);
    addressDialogControlsInit();

}

// scripts for new address and form validation  <--