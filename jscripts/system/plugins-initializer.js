$(document).ready(function () {

    //request code generator plugin settings
    if (ise.Configuration.getConfigValue("WebSupport.Enabled") == 'true') {

        $("#request-container").show();

        // load the copy jquery plugin
        $.getScript("jscripts/jquery/jquery.zclip.min.js").done(function (script, textStatus) {

            // load the active shoppers plugin
            $.getScript("jscripts/activeshoppers.ajax.js").done(function (script, textStatus) {
            }).fail(function (jqxhr, settings, exception) {
                if (exception == "Not Found") {
                    alert("jscripts/activeshoppers.ajax.js - " + exception);
                }
            });

        }).fail(function (jqxhr, settings, exception) {
            if (exception == "Not Found") {
                alert("jscripts/jquery/jquery.zclip.min.js - " + exception);
            }
        });

    } else {

        $("#request-container").hide();

    }

    //------------------------------------------------------------------------

    //minicart plugin
    var str = ise.Configuration.getConfigValue("MiniCart.Enabled");

    if (str == 'true') {

        //load the minicart dynamically
        $.getScript("jscripts/minicart.js").done(function (script, textStatus) {
            $('#shopping-cart').attr("href", "javascript:void(1)");
        }).fail(function (jqxhr, settings, exception) {
            if (exception == "Not Found") {
                alert("jscripts/minicart.js - " + exception);
            }
        });

    } else {

        $('#shopping-cart').attr("href", "shoppingcart.aspx");

    }

    //------------------------------------------------------------------------

    //Gift Registry Plugin
    str = ise.Configuration.getConfigValue("GiftRegistry.Enabled");

    if (str == 'true') {
        $("#gift-registry").show();
        $("#find-registry").show();
    } else {
        $("#gift-registry").hide();
        $("#find-registry").hide();
    }

    //------------------------------------------------------------------------

    //social media subscribe box plugin
    str = ise.Configuration.getConfigValue("ShowSocialMediaSubscribeBox");

    if (str == 'true') {
        $("#socialmedia-subscribebox").show();
    } else {
        $("#socialmedia-subscribebox").hide();
    }

    //------------------------------------------------------------------------

    //item popup plugin
    str = ise.Configuration.getConfigValue("ItemPopup.Enabled");

    if (str == 'true') {
        $("body").before("<div id='itempopup-loader'><img src='images/ajax-loader.gif' /><br/>loading...</div>");
        $("body").before("<div id='itempopup-container'></div>"); //create popup container
        $("body").before("<div id='itempopup-mask'></div>"); //create mask
        $("#itempopup-container").append("<a class='close'></a>"); //create close button

        //load the item popup plugin
        $.getScript("jscripts/itempopup.js").done(function (script, textStatus) {
        }).fail(function (jqxhr, settings, exception) {
            if (exception == "Not Found") {
                alert("jscripts/itempopup.js - " + exception);
            }
        });
    }

    //Cms editor
    str = ise.Configuration.getConfigValue("IsAdminCurrentlyLoggedIn");

    if (str == 'true') {
        //load the cms editor script
        $.getScript("jscripts/cms_editor/cms-editor.js").done(function (script, textStatus) {
        }).fail(function (jqxhr, settings, exception) {
            if (exception == "Not Found") {
                alert("jscripts/cms_editor/cms-editor.js - " + exception);
            }
        });
    }

    //------------------------------------------------------------------------
    //do other settings below

});