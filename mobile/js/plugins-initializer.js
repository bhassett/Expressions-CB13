$(document).ready(function () {

    //request code generator plugin settings

    //not supported for mobile browser

    if (ise.Configuration.getConfigValue("IsMobile") == 'true') return;

//    if (ise.Configuration.getConfigValue("WebSupport.Enabled") == 'true') {

//        $("#request-container").show();

//        // load the copy jquery plugin
//        $.getScript("~/jscripts/jquery/jquery.zclip.min.js").done(function (script, textStatus) {

//            // load the active shoppers plugin
//            $.getScript("jscripts/activeshoppers.ajax.js").done(function (script, textStatus) {
//            }).fail(function (jqxhr, settings, exception) { alert(exception); });

//        }).fail(function (jqxhr, settings, exception) { alert(exception); });

//    }

    //------------------------------------------------------------------------

    //minicart plugin
//    var str = ise.Configuration.getConfigValue("MiniCart.Enabled");

//    if (str == 'true') {

//        //load the minicart dynamically
//        $.getScript("~/jscripts/minicart.js").done(function (script, textStatus) {

//            $('#shopping-cart').attr("href", "javascript:void(1)");

//        }).fail(function (jqxhr, settings, exception) { alert(exception); });

//    } else {

//        $('#shopping-cart').attr("href", "shoppingcart.aspx");

//    }

    //------------------------------------------------------------------------

    //Gift Registry Plugin
//    var str = ise.Configuration.getConfigValue("GiftRegistry.Enabled");

//    if (str == 'true') {
//        $("#gift-registry").show();
//        $("#find-registry").show();
//    } else {
//        $("#gift-registry").hide();
//        $("#find-registry").hide();
//    }

    //JSO2.js // for IE 7 and below
    if (parseInt($.browser.version, 10) < 8) {
        $.getScript("~/jscripts/jquery/json2.js").done(function (script, textStatus) {
        }).fail(function (jqxhr, settings, exception) { alert(exception); });
    }

    //------------------------------------------------------------------------
    //do other settings below

});