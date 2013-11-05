function $add_windowLoad(handler) {
    if (window.addEventListener) {
        window.addEventListener('load', handler, false);
    }
    else if (document.addEventListener) {
        document.addEventListener('load', handler, false);
    }
    else if (window.attachEvent) {
        window.attachEvent('onload', handler);
    }
    else {
        if (typeof window.onload == 'function') {
            var oldload = window.onload;
            window.onload = function () {
                oldload();
                handler();
            }
        }
        else { window.onload = init; }
    }
}

function $getElement(el) {
    return typeof (el) == 'string' ? $get(el) : el;
}

function $enableSubmit(frm) {
    __doEnableSubmit(frm, true);
}

function $disableSubmit(frm) {
    __doEnableSubmit(frm, false);
}

function __doEnableSubmit(frm, enable) {
    var form = $getElement(frm);
    if (form) {
        for (var ctr = 0; ctr < form.length; ctr++) {
            var inpt = form[ctr];
            if (inpt.type.toLowerCase() == 'submit' ||
               inpt.type.toLowerCase() == 'reset') {
                inpt.disabled = !enable;
            }
        }
    }
}

function attachValidators(formId, searchTextId, searchTextRequiredMessage, searchTextMinLen, searchTextMinLengthErrorMessage) {

    var requiredMsg = searchTextRequiredMessage;
    var minLength = searchTextMinLen;
    var minLengthMsg = searchTextMinLengthErrorMessage;

    var funcTrim = function (s) { return s.replace(/^\s+/, '').replace(/\s+$/, ''); };
    var form = $getElement(formId);

    if (form) {
        form.onsubmit = function () {
            var txt = $getElement(searchTextId);
            var val = funcTrim(txt.value);

            if (val == '') {
                alert(requiredMsg);
                txt.focus();
                return false;
            }

            if (val.length < minLength) {
                alert(minLengthMsg);
                txt.focus();
                return false;
            }

            form.submit();
            return true;
        };
    }
}

CURRENTWIDTH = 0;
function checkOrientation() {
    if (window.innerWidth != CURRENTWIDTH) {
        CURRENTWIDTH = window.innerWidth;
        document.body.setAttribute('orient', (CURRENTWIDTH == 320) ? 'portrait' : 'landscape');
        setTimeout(scrollTo, 100, 0, 1);
    }
}

function loadHandlers() {
    checkOrientation();
    setInterval(checkOrientation, 100);
}

function jqueryHideShow(id, linkId, showText, hideText) {
    if ($(id).css("display") == 'none') {
        $(id).show('fast', function () {
            if (hideText != 'undefined' && hideText != '') {
                $(linkId).find('span').text(hideText)
            }
        });
    } else {
        $(id).hide('fast', function () {
            if (showText != 'undefined' && showText != '') {
                $(linkId).find('span').text(showText);
            }
        })
    }
}

function ToJQueryClass(className) {
    return '.' + className;
}

function ToJQueryID(id) {
    return '#' + id;
}


function JSONSerialize(object) {
    var _SPECIAL_CHARS = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, _CHARS = {
        '\b': '\\b',
        '\t': '\\t',
        '\n': '\\n',
        '\f': '\\f',
        '\r': '\\r',
        '"': '\\"',
        '\\': '\\\\'
    }, EMPTY = '', OPEN_O = '{', CLOSE_O = '}', OPEN_A = '[', CLOSE_A = ']', COMMA = ',', COMMA_CR = ",\n", CR = "\n", COLON = ':', space = "", COLON_SP = ': ', stack = [], QUOTE = '"';
    function _char(c) {
        if (!_CHARS[c]) {
            _CHARS[c] = '\\u' + ('0000' + (+(c.charCodeAt(0))).toString(16))
                        .slice(-4);
        }
        return _CHARS[c];
    }
    function _string(s) {
        return QUOTE + s.replace(_SPECIAL_CHARS, _char) + QUOTE;
        // return str.replace('\"','').replace('\"','');
    }

    function serialize(h, key) {
        var value = h[key], a = [], colon = ":", arr, i, keys, t, k, v;
        arr = value instanceof Array;
        stack.push(value);
        keys = value;
        i = 0;
        t = typeof value;
        switch (t) {
            case "object":
                if (value == null) {
                    return null;
                }
                break;
            case "string":
                return _string(value);
            case "number":
                return isFinite(value) ? value + EMPTY : NULL;
            case "boolean":
                return value + EMPTY;
            case "null":
                return null;
            default:
                return undefined;
        }
        arr = value.length === undefined ? false : true;

        if (arr) { // Array
            for (i = value.length - 1; i >= 0; --i) {
                a[i] = serialize(value, i) || NULL;
            }
        }
        else { // Object
            i = 0;
            for (k in keys) {
                if (keys.hasOwnProperty(k)) {
                    v = serialize(value, k);
                    if (v) {
                        a[i++] = _string(k) + colon + v;
                    }
                }
            }
        }

        stack.pop();
        if (space && a.length) {

            return arr
                        ? "[" + _indent(a.join(COMMA_CR), space) + "\n]"
                        : "{\n" + _indent(a.join(COMMA_CR), space) + "\n}";
        }
        else {
            return arr ? "[" + a.join(COMMA) + "]" : "{" + a.join(COMMA)
                        + "}";
        }
    }
    return serialize({
        "": object
    }, "");
}

//Note: Autocomplete postal code user input
//Component: ISEService.asmx/GetSystemPostalCode
//Get Parameter from the dropdown country to filter the coutry
//Get the top 10 postal code from the country
function RegisterAutoCompletePostalCode(inputClassName, classOrId, useClass) {

    //handle old codes
    if (useClass == undefined) {
        useClass = false;
    }

    if (!useClass) {

        $(inputClassName).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../ActionService.asmx/GetSystemPostalCode",
                    data: "{ 'countryname': '" + $('#' + classOrId).val() + "', 'postalcode': '" + request.term + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function (data) { return data; },
                    success: function (data) {
                        response($.map(data.d, function (item) {
                            return {
                                value: item.PostalCode
                            }
                        }))
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Error Status:" + textStatus + "- Error Thrown:" + errorThrown);
                    }
                });
            },
            minLength: 1
        });

    }
    else {

        $(inputClassName).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../ActionService.asmx/GetSystemPostalCode",
                    data: "{ 'countryname': '" + $('.' + classOrId).val() + "', 'postalcode': '" + request.term + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function (data) { return data; },
                    success: function (data) {
                        response($.map(data.d, function (item) {
                            return {
                                value: item.PostalCode
                            }
                        }))
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Error Status:" + textStatus + "- Error Thrown:" + errorThrown);
                    }
                });
            },
            minLength: 1
        });

    }

};


function getQueryString() {

    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');

    for (var i = 0; i < hashes.length; i++) {

        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];

    }

    return vars;
}

function AjaxCallWithSecuritySimplified(methodName, data, successFunction, errorFunction) {
    AjaxCallWithSecurity('', '', methodName, '', '', data, successFunction, errorFunction);
}

function AjaxCallCommon(methodName, data, successFunction, errorFunction) {

    var param = (data != null) ? JSON.stringify(data) : null;

    $.ajax({
        type: "POST",
        url: methodName,
        data: param,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: successFunction,
        error: errorFunction
    });

}

function AjaxCallWithSecurity(url, type, methodName, dataType, contentType, data, successFunction, errorFunction) {

    if (type == '' || type == undefined) { type = 'POST'; }

    if (dataType == '' || dataType == undefined) { dataType = 'json'; }

    if (url == '' || url == undefined) {
        url = '../ActionService.asmx/' + methodName;
    }

    if (contentType == '' || contentType == undefined) {
        contentType = "application/json; charset=utf-8";
    }

    var token = ise.Configuration.getConfigValue("Service.Token");
    if (token == '' || token == undefined) {
        alert('Error in security token: ' + 'Empty');
        return;
    }

    if (errorFunction == null || errorFunction == undefined) {
        errorFunction = ErrorHandler;
    }

    var ajaxOptions = {
        type: type,
        url: url,
        dataType: dataType,
        contentType: contentType,
        data: JSON.stringify(data),
        beforeSend: function (req) {
            req.setRequestHeader("TOKEN", token);
        },
        success: successFunction,
        error: errorFunction
    };

    $.ajax(ajaxOptions);
}

function ErrorHandler(result, textStatus, errorThrown) {
    var errorObject = $.parseJSON(result.responseText);
    //alert(errorObject.Message);
}

var jqueryBasePlugin = function () {

    this.selectorChecker = function (selector) {
        if (selector == '') return selector;
        if (selector.indexOf(".") == -1) {
            selector = "#" + selector;
        }
        return selector;
    },

    this.getTimeStamp = function () {
        return "?timestamp=" + new Date().getTime();
    },

    this.downloadCss = function (cssPath, callbackkSucess) {
        if ($('head').find("link[href*='" + cssPath + "']").length == 0) {
            $('head').append("<link href='" + cssPath + this.getTimeStamp() + "' rel='stylesheet' type='text/css' />");
        }
        if (typeof callbackkSucess != 'undefined') callbackkSucess();
    },

    this.removeCssReference = function (cssPath, callbackkSucess) {
        $('head').find("link[href*='" + cssPath + "']").remove();
        if (typeof callbackkSucess != 'undefined') callbackkSucess();
    },

    this.downloadPlugin = function (pluginPath, callbackkSucess) {
        $.getScript(pluginPath)
            .done(function (script, textStatus) { //execute callback after the download done
                if (typeof callbackkSucess != 'undefined') callbackkSucess();
            }).fail(function (jqxhr, settings, exception) { //execute callback after the download failed 
                alert('unable to load the plugin please check the source');
            });
    },

    this.parseTemplate = function (templateId, data) {
        return $.tmpl(templateId, data);
    },

    this.parseTemplateReturnHtml = function (templateId, data) {
        return $(this.parseTemplate(templateId, data)).html();
    },

    this.loadStringResource = function (key, callBack) {
        var keys = new Array();
        keys.push(key);
        ise.StringResource.loadResources(keys, callBack);
    },

     this.parseTemplateReturnHtml = function (templateId, data) {
         return $(this.parseTemplate(templateId, data)).html();
     },

    this.parseJqueryTemplate = function (templateId, data) {
        return $.trim($(this.selectorChecker(templateId)).tmpl(data).html());
    },

    this.downloadStringResources = function (keys, callBack) {
        ise.StringResource.loadResources(keys, callBack);
    },

    this.downloadAppConfigs = function (keys, callBack) {
        ise.Configuration.loadResources(keys, callBack);
    },

    this.getString = function (key) {
        return ise.StringResource.getString(key);
    },

    this.getAppConfig = function (key) {
        return ise.Configuration.getConfigValue(key);
    },

    this.getAppConfigBool = function (key) {
        return this.toBoolean(ise.Configuration.getConfigValue(key));
    },

    this.toBoolean = function (value) {
        return (value.toLowerCase() == "true");
    },

    this.ajaxRequest = function (methodName, data, callBack, failedCallBack) {

        AjaxCallCommon(methodName, data, callBack, failedCallBack);

    },

    this.ajaxSecureRequest = function (methodName, data, callBack, failedCallBack) {

        AjaxCallWithSecuritySimplified(methodName, data, callBack, failedCallBack);

    },
    this.isEmpty = function (value) {

        if (typeof (value) == "undefined" || value == null || value == "") return true;

        if (value.length > 0) {
            if ($.inArray("", value) < 0) {
                return false;
            } else {
                return true;
            }
        }

        return false;
    },

    this.isInputControlUndefined = function (selector) {
        var value = typeof ($(this.selectorChecker(selector)).val());
        if (value == "undefined" || value == undefined) return true;
        return false;
    },

    this.clearInputControlValue = function (selector) {
        $(this.selectorChecker(selector)).val("");
    },

     this.setInputControlValue = function (selector, value) {
         $(this.selectorChecker(selector)).val($.trim(value));
     },

     this.getInputControlValue = function (selector) {
         return $.trim($(this.selectorChecker(selector)).val());
     },

     this.setElementHTMLContent = function (selector, content) {
         $(this.selectorChecker(selector)).html($.trim(content));
     },

     this.hideElement = function (selector) {
         $(this.selectorChecker(selector)).addClass("display-none");
     },

     this.removeClass = function (selector, className) {
         $(this.selectorChecker(selector)).removeClass(className);
     },

     this.addClass = function (selector, className) {
         $(this.selectorChecker(selector)).addClass(className);
     },

    this.convertObjectToArray = function (object) {
        return $.map(object, function (k, v) { return [k]; });
    }
}