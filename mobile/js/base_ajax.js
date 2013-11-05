// register namespaces
Type.registerNamespace('ise');

// constants namespace
Type.registerNamespace('ise.Constants');
ise.Constants.EMPTY_STRING = '';
ise.Constants.VAT_SETTING_INCLUSIVE = 1;
ise.Constants.VAT_SETTING_EXCLUSIVE = 2;

// Utils
ise.Utils = new function(){};
ise.Utils.showRow = function(el) {
    var row = typeof(el) == 'string' ? $getElement(el) : el;
    if(null != row) {
        row.style.display = "";
    }
};

//Forces client-side events to be fired Upon
ise.Utils.fireEvent = function(element, event) {
    if (document.createEventObject) {
        // dispatch for IE
        var evt = document.createEventObject();
        return element.fireEvent('on' + event, evt)
    }
    else {
        // dispatch for firefox + others
        var evt = document.createEvent("HTMLEvents");
        evt.initEvent(event, true, true); // event type,bubbling,cancelable
        return !element.dispatchEvent(evt);
    }
};


//Disables all of the items in drop down except for the selected item.
//This mimics a "read-only" type ability, but still allows the control to post.
ise.Utils.DisableUnselectedDropdownItems = function (blnValue, control) {
    //Loop through all the options in the control.

    if (control.options != undefined && control.options != null) {

        for (lp1 = 0; lp1 < control.options.length; lp1++) {
            //Only disable the item if blnValue is true and we are not on the selected item.
            //Otherwise enable the item.
            control.options[lp1].disabled = blnValue & !control.options[lp1].selected;
        }

    }

};

ise.Utils.hideRow = function(el) {
    var row = typeof(el) == 'string' ? $getElement(el) : el;
    if(null != row) {
        row.style.display = "none";
    }
};

/********** End Utils Namespace **************/

/********** Validators Namespace **************/

Type.registerNamespace('ise.Validators');

ise.Validators.ValidationSummary = function(controlId) {
    this.focusControl = $getElement(controlId + '_Focus');
    this.summaryControl = $getElement(controlId + '_Board');
    this.errors = $getElement(controlId + '_Board_Errors');
}
ise.Validators.ValidationSummary.registerClass('ise.Validators.ValidationSummary');
ise.Validators.ValidationSummary.prototype = {

    clear : function() {
        if(this.errors) {
            this.errors.innerHTML = '';
        }
    },
    
    display : function(message) {
        if(this.errors) {
            var li = document.createElement('li');
            li.innerHTML = message;
            this.errors.appendChild(li);
        }
    },
    
    focus : function() {
        if(this.focusControl) {
            this.focusControl.focus();
        }
    }
    
}

// Validators...
ise.Validators.BaseValidator = function(){}
ise.Validators.BaseValidator.registerClass('ise.Validators.BaseValidator');
ise.Validators.BaseValidator.prototype = {
    
    setEvaluationDelegate : function(delegate) {
        this.evalDelegate = delegate;
    },
    
    shouldEvaluate : function() {
        var evaluate = true;
        
        if(undefined != this.evalDelegate && null != this.evalDelegate) {
            evaluate = this.evalDelegate(); // <- invoke
        }
        
        return evaluate;
    }
}

// Concrete validators

// ise.Validators.RequiredFieldValidator <- start
ise.Validators.RequiredFieldValidator = function(controlId, errorMessage, next) {        
        ise.Validators.RequiredFieldValidator.initializeBase(this);
        
        this.control = $getElement(controlId);
        this.errorMessage = errorMessage;
        this.next = next;
        
        this.isValid = true;
}
ise.Validators.RequiredFieldValidator.registerClass('ise.Validators.RequiredFieldValidator', ise.Validators.BaseValidator);
ise.Validators.RequiredFieldValidator.prototype = {

    validate : function() {
        this.isValid = true;
        if(this.control) {
            if(this.control.value == ise.Constants.EMPTY_STRING) {
                this.isValid = false;
            }
        }
    },
    
    toString : function() {
        return 'Required Validator:' + this.control.id;
    }
    
}



// ise.Validators.RequiredFieldValidator <- end

// ise.Validators.CompareValidator <- start
ise.Validators.CompareValidator = function(controlId, compareWithControlId, errorMessage, next) {
        ise.Validators.CompareValidator.initializeBase(this);
        
        this.control = $getElement(controlId);
        this.compareWithControl = $getElement(compareWithControlId);
        this.errorMessage = errorMessage;
        this.next = next;
        
        this.isValid = true;
}
ise.Validators.CompareValidator.registerClass('ise.Validators.CompareValidator', ise.Validators.BaseValidator);
ise.Validators.CompareValidator.prototype = {

    validate : function() {
        this.isValid = true;
        
        if(this.control && this.compareWithControl) {
            if(this.control.value != this.compareWithControl.value) {
                this.isValid = false;
            }
        }
    },
    
    toString : function() {
        return 'Compare Validator:' + this.control.id;
    }
    
}

// ise.Validators.RegExValidator <- start
ise.Validators.RegExValidator = function(controlId, expression, errorMessage) {
        ise.Validators.RegExValidator.initializeBase(this);
        
        this.control = $getElement(controlId);
        this.expression = expression;
        this.errorMessage = errorMessage;
        
        this.isValid = true;
}

ise.Validators.RegExValidator.registerClass('ise.Validators.RegExValidator', ise.Validators.BaseValidator);
ise.Validators.RegExValidator.prototype = {

    validate : function() {
        this.isValid = true;
        
        if(this.control && this.expression && this.expression != ise.Constants.EMPTY_STRING) {
            var regEx = new RegExp(this.expression);
            this.isValid = regEx.test(this.control.value);
        }
    },
    
    toString : function() {
        return 'Reg Ex Validator:' + this.control.id;
    }
    
}
// ise.Validators.RegExValidator <- end

ise.Validators.InputLengthValidator = function(controlId, minLength, maxLength, errorMessage, next) {
        ise.Validators.InputLengthValidator.initializeBase(this);        
        this.control = $getElement(controlId);
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.errorMessage = errorMessage;
        this.next = next;
        
        this.isValid = true;
}
ise.Validators.InputLengthValidator.registerClass('ise.Validators.InputLengthValidator', ise.Validators.BaseValidator);
ise.Validators.InputLengthValidator.prototype = {

    validate : function() { 
        this.isValid = true;
        
        if(this.control) {
            if(this.minLength) {
                
                if(this.control.value.length < this.minLength || this.control.value.length > this.maxLength) {
                    this.isValid = false;
                }
            }
            else {
                if(this.control.value.length > this.maxLength) {
                    this.isValid = false;
                }
            }
            
        }
    },
    
    toString : function() {
        return 'Input Length Validator:' + this.control.id;
    }    
}

ise.Validators.DropDownListValidator = function(controlId, minLength, maxLength, errorMessage, next) {
        ise.Validators.DropDownListValidator.initializeBase(this);
        
        this.control = $getElement(controlId);
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.errorMessage = errorMessage;
        this.next = next;
        
        this.isValid = true;
}
ise.Validators.DropDownListValidator.registerClass('ise.Validators.DropDownListValidator', ise.Validators.BaseValidator);
ise.Validators.DropDownListValidator.prototype = {

    validate: function () {
        this.isValid = true;
        if (this.control && this.control.selectedIndex == this.InvalidIndex) {
            this.isValid = false;
        }
    },

    toString: function () {
        return 'Input Length Validator:' + this.control.id;
    }

}


ise.Validators.ValidationController = function() {
    this.validators = new Array();
    this.validationSummary = null;
}
ise.Validators.ValidationController.registerClass('ise.Validators.ValidationController');
ise.Validators.ValidationController.prototype = {
    
    setValidationSummary : function(summary) {
        this.validationSummary = summary;
    },
    
    register : function(validator) {
        this.validators[this.validators.length] = validator;
    },
    
    clear : function() {
        this.validationSummary.clear();
    },
    
    validate : function(clear) {
        // we'll be running on the form context from this point on...
        var validators = this.validators;
        var summary = this.validationSummary;
        
        var anyNotValid = false;
        
        var validationRoutine = function(currentValidator) {
            if(null != currentValidator) {
                if(!currentValidator.shouldEvaluate()) return;
                
                currentValidator.validate();
                if(!currentValidator.isValid) {
                    anyNotValid = true;
                    summary.display(currentValidator.errorMessage);
                }
            }
        }
        
        if(clear) {
            this.clear();
        }
        
        for(var ctr=0; ctr<validators.length; ctr++) {
            var currentValidator = validators[ctr];
            validationRoutine(currentValidator);
            
            // chain
            while(null != currentValidator && currentValidator.isValid) {
                currentValidator = currentValidator.next;
                validationRoutine(currentValidator);
            }
        }
        
        if(anyNotValid) {
            summary.focus();
        }
        
        return !anyNotValid;
    }
    
}
/********** End Validators Namespace **************/

/********** Pages Namespace **************/
Type.registerNamespace('ise.Pages');

ise.Pages.BasePage = function(){}
ise.Pages.BasePage.registerClass('ise.Pages.BasePage');
ise.Pages.BasePage.prototype = {
    
    loadControl : function(id, controller, setControlDelegate) {
        var control = controller.getControl(id);
        if(control) {
            setControlDelegate(control);
        }
        else {
            var observer = { 
            
                notify : function(control) {
                    if(control) {
                        if(control.id == id) {
                            setControlDelegate(control);
                        }
                    }
                }
            }
            
            controller.addObserver(observer);
        }
    },
    
    setForm : function(id) {
        this.form = $getElement(id);
        
        if(this.onFormSet) {
            this.onFormSet();
        }
    }
    
}

/********** End Pages Namespace **************/

ise.StringResource = {
    
    initialize : function() {
        this.strings = new Array();
    },

    registerString : function(key, value) {
        this.strings[key] = value;
    },
    
    getString : function(key) {
        if(this.strings[key]) {
            return this.strings[key];
        }
        else {
            return key;
        }
    },

    loadResources: function (arrayParam, callBack) {

        if (arrayParam == undefined || arrayParam == null) {
            return;
        }

        $.ajax({
            type: "POST",
            url: "../ActionService.asmx/LoadStringResources",
            data: JSON.stringify({ keys: arrayParam }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var resources = $.parseJSON(data.d);

                for (var i = 0; i < resources.length; i++) {
                    ise.StringResource.registerString(resources[i].Key, resources[i].Value);
                }

                // Use callback when loading more resources
                if (callBack != undefined && callBack != null) callBack();
            },
            error: function () { 
            
            }
        });

    }

}

ise.StringResource.initialize();

ise.Configuration = {

    initialize: function () {
        this.strings = new Array();
    },

    registerConfig: function (key, value) {
        this.strings[key] = value;
    },

    getConfigValue: function (key) {
        if (this.strings[key]) {
            return this.strings[key];
        }
        else {
            return key;
        }
    },

    loadResources: function (arrayParam, callBack) {

        if (arrayParam == undefined || arrayParam == null) {
            return;
        }

        $.ajax({
            type: "POST",
            url: "../ActionService.asmx/LoadAppConfigs",
            data: JSON.stringify({ keys: arrayParam }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var resources = $.parseJSON(data.d);

                for (var i = 0; i < resources.length; i++) {
                    ise.Configuration.registerConfig(resources[i].Key, resources[i].Value);
                }

                // Use callback when loading more resources
                if (callBack != undefined && callBack != null) callBack();
            },
            error: function () {

            }
        });
    }

}

ise.Configuration.initialize();

function getElementsByClassName(tagName, className){
    var elements = new Array();
    var elemx = document.getElementsByTagName(tagName);
    for(var ctr=0; ctr<elemx.length; ctr++) {
        var el = elemx[ctr];
        if(el.className == className) {
            elements.push(el);
        }
    }
    
    return elements;
}

// HtmlDecode http://lab.msdn.microsoft.com/annotations/htmldecode.js
//   client side version of the useful Server.HtmlDecode method
//   takes one string (encoded) and returns another (decoded)
function HtmlDecode(s) {
    var out = "";
    if (s == null) {
        return;
    }
    var l = s.length;
    for (var i = 0; i < l; i++) {
        var ch = s.charAt(i);
        if (ch == "&") {
            var semicolonIndex = s.indexOf(";", i + 1);
            if (semicolonIndex > 0) {
                var entity = s.substring(i + 1, semicolonIndex);
                if (entity.length > 1 && entity.charAt(0) == "#") {
                    if (entity.charAt(1) == "x" ||
                        entity.charAt(1) == "X") {
                        ch = String.fromCharCode(eval("0" + entity.substring(1)));
                    } else {
                        ch = String.fromCharCode(eval(entity.substring(1)));
                    }
                } else {
                    switch (entity) {
                      case "quot":
                        ch = String.fromCharCode(34);
                        break;
                      case "amp":
                        ch = String.fromCharCode(38);
                        break;
                      case "lt":
                        ch = String.fromCharCode(60);
                        break;
                      case "gt":
                        ch = String.fromCharCode(62);
                        break;
                      case "nbsp":
                        ch = String.fromCharCode(160);
                        break;
                      case "iexcl":
                        ch = String.fromCharCode(161);
                        break;
                      case "cent":
                        ch = String.fromCharCode(162);
                        break;
                      case "pound":
                        ch = String.fromCharCode(163);
                        break;
                      case "curren":
                        ch = String.fromCharCode(164);
                        break;
                      case "yen":
                        ch = String.fromCharCode(165);
                        break;
                      case "brvbar":
                        ch = String.fromCharCode(166);
                        break;
                      case "sect":
                        ch = String.fromCharCode(167);
                        break;
                      case "uml":
                        ch = String.fromCharCode(168);
                        break;
                      case "copy":
                        ch = String.fromCharCode(169);
                        break;
                      case "ordf":
                        ch = String.fromCharCode(170);
                        break;
                      case "laquo":
                        ch = String.fromCharCode(171);
                        break;
                      case "not":
                        ch = String.fromCharCode(172);
                        break;
                      case "shy":
                        ch = String.fromCharCode(173);
                        break;
                      case "reg":
                        ch = String.fromCharCode(174);
                        break;
                      case "macr":
                        ch = String.fromCharCode(175);
                        break;
                      case "deg":
                        ch = String.fromCharCode(176);
                        break;
                      case "plusmn":
                        ch = String.fromCharCode(177);
                        break;
                      case "sup2":
                        ch = String.fromCharCode(178);
                        break;
                      case "sup3":
                        ch = String.fromCharCode(179);
                        break;
                      case "acute":
                        ch = String.fromCharCode(180);
                        break;
                      case "micro":
                        ch = String.fromCharCode(181);
                        break;
                      case "para":
                        ch = String.fromCharCode(182);
                        break;
                      case "middot":
                        ch = String.fromCharCode(183);
                        break;
                      case "cedil":
                        ch = String.fromCharCode(184);
                        break;
                      case "sup1":
                        ch = String.fromCharCode(185);
                        break;
                      case "ordm":
                        ch = String.fromCharCode(186);
                        break;
                      case "raquo":
                        ch = String.fromCharCode(187);
                        break;
                      case "frac14":
                        ch = String.fromCharCode(188);
                        break;
                      case "frac12":
                        ch = String.fromCharCode(189);
                        break;
                      case "frac34":
                        ch = String.fromCharCode(190);
                        break;
                      case "iquest":
                        ch = String.fromCharCode(191);
                        break;
                      case "Agrave":
                        ch = String.fromCharCode(192);
                        break;
                      case "Aacute":
                        ch = String.fromCharCode(193);
                        break;
                      case "Acirc":
                        ch = String.fromCharCode(194);
                        break;
                      case "Atilde":
                        ch = String.fromCharCode(195);
                        break;
                      case "Auml":
                        ch = String.fromCharCode(196);
                        break;
                      case "Aring":
                        ch = String.fromCharCode(197);
                        break;
                      case "AElig":
                        ch = String.fromCharCode(198);
                        break;
                      case "Ccedil":
                        ch = String.fromCharCode(199);
                        break;
                      case "Egrave":
                        ch = String.fromCharCode(200);
                        break;
                      case "Eacute":
                        ch = String.fromCharCode(201);
                        break;
                      case "Ecirc":
                        ch = String.fromCharCode(202);
                        break;
                      case "Euml":
                        ch = String.fromCharCode(203);
                        break;
                      case "Igrave":
                        ch = String.fromCharCode(204);
                        break;
                      case "Iacute":
                        ch = String.fromCharCode(205);
                        break;
                      case "Icirc":
                        ch = String.fromCharCode(206);
                        break;
                      case "Iuml":
                        ch = String.fromCharCode(207);
                        break;
                      case "ETH":
                        ch = String.fromCharCode(208);
                        break;
                      case "Ntilde":
                        ch = String.fromCharCode(209);
                        break;
                      case "Ograve":
                        ch = String.fromCharCode(210);
                        break;
                      case "Oacute":
                        ch = String.fromCharCode(211);
                        break;
                      case "Ocirc":
                        ch = String.fromCharCode(212);
                        break;
                      case "Otilde":
                        ch = String.fromCharCode(213);
                        break;
                      case "Ouml":
                        ch = String.fromCharCode(214);
                        break;
                      case "times":
                        ch = String.fromCharCode(215);
                        break;
                      case "Oslash":
                        ch = String.fromCharCode(216);
                        break;
                      case "Ugrave":
                        ch = String.fromCharCode(217);
                        break;
                      case "Uacute":
                        ch = String.fromCharCode(218);
                        break;
                      case "Ucirc":
                        ch = String.fromCharCode(219);
                        break;
                      case "Uuml":
                        ch = String.fromCharCode(220);
                        break;
                      case "Yacute":
                        ch = String.fromCharCode(221);
                        break;
                      case "THORN":
                        ch = String.fromCharCode(222);
                        break;
                      case "szlig":
                        ch = String.fromCharCode(223);
                        break;
                      case "agrave":
                        ch = String.fromCharCode(224);
                        break;
                      case "aacute":
                        ch = String.fromCharCode(225);
                        break;
                      case "acirc":
                        ch = String.fromCharCode(226);
                        break;
                      case "atilde":
                        ch = String.fromCharCode(227);
                        break;
                      case "auml":
                        ch = String.fromCharCode(228);
                        break;
                      case "aring":
                        ch = String.fromCharCode(229);
                        break;
                      case "aelig":
                        ch = String.fromCharCode(230);
                        break;
                      case "ccedil":
                        ch = String.fromCharCode(231);
                        break;
                      case "egrave":
                        ch = String.fromCharCode(232);
                        break;
                      case "eacute":
                        ch = String.fromCharCode(233);
                        break;
                      case "ecirc":
                        ch = String.fromCharCode(234);
                        break;
                      case "euml":
                        ch = String.fromCharCode(235);
                        break;
                      case "igrave":
                        ch = String.fromCharCode(236);
                        break;
                      case "iacute":
                        ch = String.fromCharCode(237);
                        break;
                      case "icirc":
                        ch = String.fromCharCode(238);
                        break;
                      case "iuml":
                        ch = String.fromCharCode(239);
                        break;
                      case "eth":
                        ch = String.fromCharCode(240);
                        break;
                      case "ntilde":
                        ch = String.fromCharCode(241);
                        break;
                      case "ograve":
                        ch = String.fromCharCode(242);
                        break;
                      case "oacute":
                        ch = String.fromCharCode(243);
                        break;
                      case "ocirc":
                        ch = String.fromCharCode(244);
                        break;
                      case "otilde":
                        ch = String.fromCharCode(245);
                        break;
                      case "ouml":
                        ch = String.fromCharCode(246);
                        break;
                      case "divide":
                        ch = String.fromCharCode(247);
                        break;
                      case "oslash":
                        ch = String.fromCharCode(248);
                        break;
                      case "ugrave":
                        ch = String.fromCharCode(249);
                        break;
                      case "uacute":
                        ch = String.fromCharCode(250);
                        break;
                      case "ucirc":
                        ch = String.fromCharCode(251);
                        break;
                      case "uuml":
                        ch = String.fromCharCode(252);
                        break;
                      case "yacute":
                        ch = String.fromCharCode(253);
                        break;
                      case "thorn":
                        ch = String.fromCharCode(254);
                        break;
                      case "yuml":
                        ch = String.fromCharCode(255);
                        break;
                      case "OElig":
                        ch = String.fromCharCode(338);
                        break;
                      case "oelig":
                        ch = String.fromCharCode(339);
                        break;
                      case "Scaron":
                        ch = String.fromCharCode(352);
                        break;
                      case "scaron":
                        ch = String.fromCharCode(353);
                        break;
                      case "Yuml":
                        ch = String.fromCharCode(376);
                        break;
                      case "fnof":
                        ch = String.fromCharCode(402);
                        break;
                      case "circ":
                        ch = String.fromCharCode(710);
                        break;
                      case "tilde":
                        ch = String.fromCharCode(732);
                        break;
                      case "Alpha":
                        ch = String.fromCharCode(913);
                        break;
                      case "Beta":
                        ch = String.fromCharCode(914);
                        break;
                      case "Gamma":
                        ch = String.fromCharCode(915);
                        break;
                      case "Delta":
                        ch = String.fromCharCode(916);
                        break;
                      case "Epsilon":
                        ch = String.fromCharCode(917);
                        break;
                      case "Zeta":
                        ch = String.fromCharCode(918);
                        break;
                      case "Eta":
                        ch = String.fromCharCode(919);
                        break;
                      case "Theta":
                        ch = String.fromCharCode(920);
                        break;
                      case "Iota":
                        ch = String.fromCharCode(921);
                        break;
                      case "Kappa":
                        ch = String.fromCharCode(922);
                        break;
                      case "Lambda":
                        ch = String.fromCharCode(923);
                        break;
                      case "Mu":
                        ch = String.fromCharCode(924);
                        break;
                      case "Nu":
                        ch = String.fromCharCode(925);
                        break;
                      case "Xi":
                        ch = String.fromCharCode(926);
                        break;
                      case "Omicron":
                        ch = String.fromCharCode(927);
                        break;
                      case "Pi":
                        ch = String.fromCharCode(928);
                        break;
                      case " Rho ":
                        ch = String.fromCharCode(929);
                        break;
                      case "Sigma":
                        ch = String.fromCharCode(931);
                        break;
                      case "Tau":
                        ch = String.fromCharCode(932);
                        break;
                      case "Upsilon":
                        ch = String.fromCharCode(933);
                        break;
                      case "Phi":
                        ch = String.fromCharCode(934);
                        break;
                      case "Chi":
                        ch = String.fromCharCode(935);
                        break;
                      case "Psi":
                        ch = String.fromCharCode(936);
                        break;
                      case "Omega":
                        ch = String.fromCharCode(937);
                        break;
                      case "alpha":
                        ch = String.fromCharCode(945);
                        break;
                      case "beta":
                        ch = String.fromCharCode(946);
                        break;
                      case "gamma":
                        ch = String.fromCharCode(947);
                        break;
                      case "delta":
                        ch = String.fromCharCode(948);
                        break;
                      case "epsilon":
                        ch = String.fromCharCode(949);
                        break;
                      case "zeta":
                        ch = String.fromCharCode(950);
                        break;
                      case "eta":
                        ch = String.fromCharCode(951);
                        break;
                      case "theta":
                        ch = String.fromCharCode(952);
                        break;
                      case "iota":
                        ch = String.fromCharCode(953);
                        break;
                      case "kappa":
                        ch = String.fromCharCode(954);
                        break;
                      case "lambda":
                        ch = String.fromCharCode(955);
                        break;
                      case "mu":
                        ch = String.fromCharCode(956);
                        break;
                      case "nu":
                        ch = String.fromCharCode(957);
                        break;
                      case "xi":
                        ch = String.fromCharCode(958);
                        break;
                      case "omicron":
                        ch = String.fromCharCode(959);
                        break;
                      case "pi":
                        ch = String.fromCharCode(960);
                        break;
                      case "rho":
                        ch = String.fromCharCode(961);
                        break;
                      case "sigmaf":
                        ch = String.fromCharCode(962);
                        break;
                      case "sigma":
                        ch = String.fromCharCode(963);
                        break;
                      case "tau":
                        ch = String.fromCharCode(964);
                        break;
                      case "upsilon":
                        ch = String.fromCharCode(965);
                        break;
                      case "phi":
                        ch = String.fromCharCode(966);
                        break;
                      case "chi":
                        ch = String.fromCharCode(967);
                        break;
                      case "psi":
                        ch = String.fromCharCode(968);
                        break;
                      case "omega":
                        ch = String.fromCharCode(969);
                        break;
                      case "thetasym":
                        ch = String.fromCharCode(977);
                        break;
                      case "upsih":
                        ch = String.fromCharCode(978);
                        break;
                      case "piv":
                        ch = String.fromCharCode(982);
                        break;
                      case "ensp":
                        ch = String.fromCharCode(8194);
                        break;
                      case "emsp":
                        ch = String.fromCharCode(8195);
                        break;
                      case "thinsp":
                        ch = String.fromCharCode(8201);
                        break;
                      case "zwnj":
                        ch = String.fromCharCode(8204);
                        break;
                      case "zwj":
                        ch = String.fromCharCode(8205);
                        break;
                      case "lrm":
                        ch = String.fromCharCode(8206);
                        break;
                      case "rlm":
                        ch = String.fromCharCode(8207);
                        break;
                      case "ndash":
                        ch = String.fromCharCode(8211);
                        break;
                      case "mdash":
                        ch = String.fromCharCode(8212);
                        break;
                      case "lsquo":
                        ch = String.fromCharCode(8216);
                        break;
                      case "rsquo":
                        ch = String.fromCharCode(8217);
                        break;
                      case "sbquo":
                        ch = String.fromCharCode(8218);
                        break;
                      case "ldquo":
                        ch = String.fromCharCode(8220);
                        break;
                      case "rdquo":
                        ch = String.fromCharCode(8221);
                        break;
                      case "bdquo":
                        ch = String.fromCharCode(8222);
                        break;
                      case "dagger":
                        ch = String.fromCharCode(8224);
                        break;
                      case "Dagger":
                        ch = String.fromCharCode(8225);
                        break;
                      case "bull":
                        ch = String.fromCharCode(8226);
                        break;
                      case "hellip":
                        ch = String.fromCharCode(8230);
                        break;
                      case "permil":
                        ch = String.fromCharCode(8240);
                        break;
                      case "prime":
                        ch = String.fromCharCode(8242);
                        break;
                      case "Prime":
                        ch = String.fromCharCode(8243);
                        break;
                      case "lsaquo":
                        ch = String.fromCharCode(8249);
                        break;
                      case "rsaquo":
                        ch = String.fromCharCode(8250);
                        break;
                      case "oline":
                        ch = String.fromCharCode(8254);
                        break;
                      case "frasl":
                        ch = String.fromCharCode(8260);
                        break;
                      case "euro":
                        ch = String.fromCharCode(8364);
                        break;
                      case "image":
                        ch = String.fromCharCode(8465);
                        break;
                      case "weierp":
                        ch = String.fromCharCode(8472);
                        break;
                      case "real":
                        ch = String.fromCharCode(8476);
                        break;
                      case "trade":
                        ch = String.fromCharCode(8482);
                        break;
                      case "alefsym":
                        ch = String.fromCharCode(8501);
                        break;
                      case "larr":
                        ch = String.fromCharCode(8592);
                        break;
                      case "uarr":
                        ch = String.fromCharCode(8593);
                        break;
                      case "rarr":
                        ch = String.fromCharCode(8594);
                        break;
                      case "darr":
                        ch = String.fromCharCode(8595);
                        break;
                      case "harr":
                        ch = String.fromCharCode(8596);
                        break;
                      case "crarr":
                        ch = String.fromCharCode(8629);
                        break;
                      case "lArr":
                        ch = String.fromCharCode(8656);
                        break;
                      case "uArr":
                        ch = String.fromCharCode(8657);
                        break;
                      case "rArr":
                        ch = String.fromCharCode(8658);
                        break;
                      case "dArr":
                        ch = String.fromCharCode(8659);
                        break;
                      case "hArr":
                        ch = String.fromCharCode(8660);
                        break;
                      case "forall":
                        ch = String.fromCharCode(8704);
                        break;
                      case "part":
                        ch = String.fromCharCode(8706);
                        break;
                      case "exist":
                        ch = String.fromCharCode(8707);
                        break;
                      case "empty":
                        ch = String.fromCharCode(8709);
                        break;
                      case "nabla":
                        ch = String.fromCharCode(8711);
                        break;
                      case "isin":
                        ch = String.fromCharCode(8712);
                        break;
                      case "notin":
                        ch = String.fromCharCode(8713);
                        break;
                      case "ni":
                        ch = String.fromCharCode(8715);
                        break;
                      case "prod":
                        ch = String.fromCharCode(8719);
                        break;
                      case "sum":
                        ch = String.fromCharCode(8721);
                        break;
                      case "minus":
                        ch = String.fromCharCode(8722);
                        break;
                      case "lowast":
                        ch = String.fromCharCode(8727);
                        break;
                      case "radic":
                        ch = String.fromCharCode(8730);
                        break;
                      case "prop":
                        ch = String.fromCharCode(8733);
                        break;
                      case "infin":
                        ch = String.fromCharCode(8734);
                        break;
                      case "ang":
                        ch = String.fromCharCode(8736);
                        break;
                      case "and":
                        ch = String.fromCharCode(8743);
                        break;
                      case "or":
                        ch = String.fromCharCode(8744);
                        break;
                      case "cap":
                        ch = String.fromCharCode(8745);
                        break;
                      case "cup":
                        ch = String.fromCharCode(8746);
                        break;
                      case "int":
                        ch = String.fromCharCode(8747);
                        break;
                      case "there4":
                        ch = String.fromCharCode(8756);
                        break;
                      case "sim":
                        ch = String.fromCharCode(8764);
                        break;
                      case "cong":
                        ch = String.fromCharCode(8773);
                        break;
                      case "asymp":
                        ch = String.fromCharCode(8776);
                        break;
                      case "ne":
                        ch = String.fromCharCode(8800);
                        break;
                      case "equiv":
                        ch = String.fromCharCode(8801);
                        break;
                      case "le":
                        ch = String.fromCharCode(8804);
                        break;
                      case "ge":
                        ch = String.fromCharCode(8805);
                        break;
                      case "sub":
                        ch = String.fromCharCode(8834);
                        break;
                      case "sup":
                        ch = String.fromCharCode(8835);
                        break;
                      case "nsub":
                        ch = String.fromCharCode(8836);
                        break;
                      case "sube":
                        ch = String.fromCharCode(8838);
                        break;
                      case "supe":
                        ch = String.fromCharCode(8839);
                        break;
                      case "oplus":
                        ch = String.fromCharCode(8853);
                        break;
                      case "otimes":
                        ch = String.fromCharCode(8855);
                        break;
                      case "perp":
                        ch = String.fromCharCode(8869);
                        break;
                      case "sdot":
                        ch = String.fromCharCode(8901);
                        break;
                      case "lceil":
                        ch = String.fromCharCode(8968);
                        break;
                      case "rceil":
                        ch = String.fromCharCode(8969);
                        break;
                      case "lfloor":
                        ch = String.fromCharCode(8970);
                        break;
                      case "rfloor":
                        ch = String.fromCharCode(8971);
                        break;
                      case "lang":
                        ch = String.fromCharCode(9001);
                        break;
                      case "rang":
                        ch = String.fromCharCode(9002);
                        break;
                      case "loz":
                        ch = String.fromCharCode(9674);
                        break;
                      case "spades":
                        ch = String.fromCharCode(9824);
                        break;
                      case "clubs":
                        ch = String.fromCharCode(9827);
                        break;
                      case "hearts":
                        ch = String.fromCharCode(9829);
                        break;
                      case "diams":
                        ch = String.fromCharCode(9830);
                        break;
                      default:
                        ch = "";
                        break;
                    }
                }
                i = semicolonIndex;
            }
        }
        out += ch;
    }
    return out;
}

//JSO2.js // for IE 7 and below
if (parseInt($.browser.version, 10) < 8) {
    $.getScript("js/jquery/json2.js").done(function (script, textStatus) {
    }).fail(function (jqxhr, settings, exception) { alert(exception); });
}
