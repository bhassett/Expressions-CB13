$(document).ready(LoadGlobalConfig());

function LoadGlobalConfig() {
    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetGlobalConfig",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        cache: false,
        success: function (result) {
            PopulateConfiguration(result.d);
            RegisterPlugins();
        },
        error: function (result, textStatus, errorThrown) {
            return;
        }
    });
}

function PopulateConfiguration(configurations) {
    var lst = $.parseJSON(configurations);
    for (var i = 0; i < lst.length; i++) {
        ise.Configuration.registerConfig(lst[i].Key, lst[i].Value);
    }
}

function RegisterPlugins() {
    $.getScript("jscripts/system/plugins-initializer.js").done(function (script, textStatus) {
    }).fail(function (jqxhr, settings, exception) {
        if (exception == "Not Found") {
            alert("jscripts/system/plugins-initializer.js - " + exception);
        }
    });
}

function DisplayErrorFromAjaxCall(result) {
    alert(result.InnerHTML);
}