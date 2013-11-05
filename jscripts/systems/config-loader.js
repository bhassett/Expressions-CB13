var iseConfiguration = function () {

    this.initialize = function () {
        this.strings = new Array();
    },

    this.registerConfig = function (key, value) {
        this.strings[key] = value;
    },

    this.getConfigValue = function (key) {

        if (this.strings[key]) {
            return this.strings[key];
        }
        else {
            return key;
        }
    }
}

var iseAppConfig = new iseConfiguration();
iseAppConfig.initialize();

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
        },
        error: function (result, textStatus, errorThrown) {
            return;
        }
    });
}

function PopulateConfiguration(configurations) {
    var lst = $.parseJSON(configurations);
    for (var i = 0; i < lst.length; i++) {
        iseAppConfig.registerConfig(lst[i].Key, lst[i].Value);
    }
}

