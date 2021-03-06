﻿// Ensure Root Namespaces are existing..
Type.registerNamespace('ise.Controls');
Type.registerNamespace('ise.Constants');
Type.registerNamespace('ise.Validators');

ise.Constants.PAYMENT_METHOD_CHECK = "Check/Cheque";
ise.Constants.PAYMENT_METHOD_CREDITCARD = "Credit Card";
ise.Constants.PAYMENT_METHOD_CASH = "Cash/Other";
ise.Constants.PAYMENT_METHOD_REDIRECT = "PayPal";

ise.Validators.CardNumberValidator = function(cardNumberId, cardTypeId, errorMessages, next) {
    ise.Validators.CardNumberValidator.initializeBase(this);
    
    this.cardNumberControl = $getElement(cardNumberId);
    this.cardTypeControl = $getElement(cardTypeId);
    this.errorMessages = errorMessages;
    this.errorMessage = ''; // would be determined by what type of error returned by our third party verifier
    this.next = next;
    
    this.isValid = true;
}
ise.Validators.CardNumberValidator.registerClass('ise.Validators.CardNumberValidator', ise.Validators.BaseValidator);
ise.Validators.CardNumberValidator.prototype = {

    validate : function() {
        this.isValid = true;
        
        if(this.cardNumberControl && this.cardTypeControl) {
            var cardNumber = this.cardNumberControl.value;
            
            if(cardNumber.indexOf('*') == 0) {
                this.isValid = true;
                return;
            }
            
            var cardType = this.cardTypeControl.options[this.cardTypeControl.selectedIndex].value;
            
            this.isValid = checkCreditCard(cardNumber, cardType);
            
            if(!this.isValid) {
                if(ccErrorNo == 0) {
                    this.isValid = true;
                }
            }
            this.errorMessage = this.errorMessages[ccErrorNo];
        }
    },
    
    toString : function() {
        return 'Card Number Validator:' + this.cardNumberControl.id;
    }
}


ise.Controls.PaymentTermOption = function(id, term, method) {
    this.id = id;
    this.ctrl = $getElement(id);
    this.term = term;
    this.paymentMethod = method;
    
    if(this.ctrl) {
        var del = Function.createDelegate(this, this.handleOptionClick);
        $addHandler(this.ctrl, 'click', del);
    }
    
    this.selectedEventHandler = null;
}

ise.Controls.PaymentTermOption.registerClass('ise.Controls.PaymentTermOption');
ise.Controls.PaymentTermOption.prototype = {

    handleOptionClick: function (option) {
        this.onSelected(option);
    },

    onSelected: function (option) {
        if (option) {
            if (this.selectedEventHandler) {
                this.selectedEventHandler(this);
            }

            //For RC2 Optimization

            var firstCreditCardCode = $(".is-primary-address");
            if (firstCreditCardCode && typeof (firstCreditCardCode) != undefined) {

                //During single billing address and user already updated the address
                //clicking the Payment term options address should not return the address to original values.

                if ($("input[name='credit-card-code']").length > 1) {
                    $(firstCreditCardCode).trigger("click");
                }

            }

        }
    },

    getIsChecked: function () {
        if (this.ctrl) {
            return this.ctrl.checked;
        }

        return false;
    },

    setSelectedEventHandler: function (handler) {
        return this.selectedEventHandler = handler;
    },

    getPaymentTerm: function () {
        return this.term;
    },

    getPaymentMethod: function () {
        return this.paymentMethod;
    }

}

ise.Controls.PaymentTermControl = function(id) {
    this.id = id;
    this.ctrl = $getElement(id);
    
    this.options = new Array();
    
    this.currentOption = null;
    
    this.validationController = new ise.Validators.ValidationController();
    
    this.noPaymentRequired = false;
    this.requireTermsAndConditions = false;
    this.IsTokenization = false;
    this.IsInOnePageCheckout = false;
    this.CvvErrorMessage = '';
}

ise.Controls.PaymentTermControl.registerClass('ise.Controls.PaymentTermControl');
ise.Controls.PaymentTermControl.prototype = {

    setRequireTermsAndConditions: function (requireTermsAndConditions) {
        this.requireTermsAndConditions = requireTermsAndConditions;
    },

    getNoPaymentRequired: function () {
        return this.noPaymentRequired;
    },

    setNoPaymentRequired: function (noPaymentRequired) {
        this.noPaymentRequired = noPaymentRequired;
    },

    getTermsAndConditionsChecked: function () {
        var elemTermsAndConditionsChecked = $getElement(this.id + '_termsAndConditionsChecked');
        if (elemTermsAndConditionsChecked) {
            return elemTermsAndConditionsChecked.checked;
        }

        return true;
    },

    getPaymentTerm: function () {
        var elemTerm = $getElement(this.id + '_paymentTerm');
        if (elemTerm) {
            return elemTerm.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    setPaymentTerm: function (term) {
        var elemTerm = $getElement(this.id + '_paymentTerm');
        if (elemTerm) {
            elemTerm.value = term;
        }
    },

    getPaymentMethod: function () {
        var elemPM = $getElement(this.id + '_paymentMethod');
        if (elemPM) {
            return elemPM.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    setPaymentMethod: function (term) {
        var elemPM = $getElement(this.id + '_paymentMethod');
        if (elemPM) {
            elemPM.value = term;
        }
    },

    getNameOnCard: function () {
        var elem = $getElement(this.id + '_nameOnCard');
        if (elem) {
            return elem.value;
        }
    },

    getCardNumber: function () {
        var elem = $getElement(this.id + '_cardNumber');
        if (elem) {
            return elem.value;
        }
    },

    getCardType: function () {
        var elem = $getElement(this.id + '_cardType');
        if (elem) {
            return elem.options[elem.selectedIndex].value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getExpirationMonth: function () {
        var elem = $getElement(this.id + '_expirationMonth');
        if (elem) {
            return elem.options[elem.selectedIndex].value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getExpirationYear: function () {
        var elem = $getElement(this.id + '_expirationYear');
        if (elem) {
            return elem.options[elem.selectedIndex].value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    setNameOnCard: function (value) {
        var elem = $getElement(this.id + '_nameOnCard');
        if (elem) {
            elem.value = value;
        }
    },

    setCardNumber: function (value) {
        var elem = $getElement(this.id + '_cardNumber');
        if (elem) {
            elem.value = value;
        }
    },

    setCardType: function (value) {
        var elem = $getElement(this.id + '_cardType');
        if (elem) {
            var idx = 0;
            for (var ctr = 0; ctr < elem.options.length; ctr++) {
                var option = elem.options[ctr];
                if (option.value == value) {
                    idx = ctr;
                    break;
                }
            }

            elem.selectedIndex = idx;
        }
    },

    setExpirationMonth: function (value) {
        var elem = $getElement(this.id + '_expirationMonth');
        if (elem) {
            var idx = 0;
            for (var ctr = 0; ctr < elem.options.length; ctr++) {
                var option = elem.options[ctr];
                if (option.value == value) {
                    idx = ctr;
                    break;
                }
            }

            elem.selectedIndex = idx;
        }
    },

    setExpirationYear: function (value) {
        var elem = $getElement(this.id + '_expirationYear');
        if (elem) {
            var idx = 0;
            for (var ctr = 0; ctr < elem.options.length; ctr++) {
                var option = elem.options[ctr];
                if (option.value == value) {
                    idx = ctr;
                    break;
                }
            }

            elem.selectedIndex = idx;
        }
    },

    getCurrentOption: function () {
        return this.currentOption;
    },

    setCurrentOption: function (option) {
        this.currentOption = option;
        this.setPaymentTerm(option.getPaymentTerm());
        this.setPaymentMethod(option.getPaymentMethod());

        this.switchView();
    },

    registerOption: function (option) {
        this.options[this.options.length] = option;

        if (option.getIsChecked()) {
            this.setCurrentOption(option);
        }

        var del = Function.createDelegate(this, this.handleOptionSelected);
        option.setSelectedEventHandler(del);
    },

    handleOptionSelected: function (option) {
        this.setCurrentOption(option);
    },

    switchView: function () {
        var current = this.currentOption;
        if (current) {
            this.hidePONumberPanel();
            this.hideCreditCardPanel();
            this.hideRedirectPanel();

            if (this.validationController.validationSummary) {
                this.validationController.validationSummary.HideSummary();
            }

            if (current.getPaymentTerm().toUpperCase() == 'PURCHASE ORDER') {
                this.showPONumberPanel();
            }
            else if (current.getPaymentTerm().toUpperCase() == 'REQUEST QUOTE') {
            }
            else if (current.getPaymentMethod() == ise.Constants.PAYMENT_METHOD_REDIRECT) {
                this.showRedirectPanel();
            }
            else {
                if (current.getPaymentMethod() == ise.Constants.PAYMENT_METHOD_CREDITCARD) {
                    this.showCreditCardPanel();
                }
            }
        }
    },

    showCreditCardPanel: function () {
        var ccRow = $getElement(this.id + '_cardFormRow');
        if (ccRow) {
            ise.Utils.showRow(ccRow);
        }
    },

    hideCreditCardPanel: function () {
        var ccRow = $getElement(this.id + '_cardFormRow');
        if (ccRow) {
            ise.Utils.hideRow(ccRow);
        }
    },

    showPONumberPanel: function () {
        var poRow = $getElement(this.id + '_poNumberRow');
        if (poRow) {
            ise.Utils.showRow(poRow);
        }
    },

    hidePONumberPanel: function () {
        var poRow = $getElement(this.id + '_poNumberRow');
        if (poRow) {
            ise.Utils.hideRow(poRow);
        }
    },

    showRedirectPanel: function () {
        var exRow = $getElement(this.id + '_externalRow');
        if (exRow) {
            ise.Utils.showRow(exRow);
        }
    },

    hideRedirectPanel: function () {
        var exRow = $getElement(this.id + '_externalRow');
        if (exRow) {
            ise.Utils.hideRow(exRow);
        }
    },

    setValidationSummary: function (summary) {
        this.validationController.setValidationSummary(summary);
    },

    registerValidator: function (validator) {
        this.validationController.register(validator);
    },

    clearValidationSummary: function () {
        this.validationController.clear();
    },

    validate: function (clear) {
        if (this.noPaymentRequired) { return true; }

        //removal of cvv validation when card is tokenized since controls are not editable anymore
        if (this.validationController.validators.length > 0 && this.IsTokenization) {
            this.DoRemovalOfCCVValidation();
        }

        var valid = this.validationController.validate(clear);

        if (valid && this.requireTermsAndConditions && !this.getTermsAndConditionsChecked()) {
            alert(ise.StringResource.getString('checkoutpayment.aspx.5'));
            valid = false;
        }

        return valid;
    },

    DoRemovalOfCCVValidation: function () {

        var isTokenization = this.IsTokenization;
        var cvvErrorMessage = this.CvvErrorMessage;

        var currentCardMask = $("#hidMaskCardNumber").val();
        var currentCardCode = $("#hidCreditCardCode").val();

        //remove CVV validator when tokenization
        if (this.IsTokenization &&
        currentCardMask != ise.Constants.EMPTY_STRING &&
        currentCardCode != ise.Constants.EMPTY_STRING) {
            var lstValidators = this.validationController.validators;
            lstValidators = $.grep(lstValidators, function (n) {
                if (n.errorMessage != null && n.errorMessage != undefined) {
                    return n.errorMessage.indexOf(cvvErrorMessage) == -1;
                }
                return true;
            });
            this.validationController.validators = lstValidators;
        }
    }

}

ise.Controls.PaymentTermController = {
    
    initialize : function() {
        this.terms = new Array();
        this.observers = new Array();
    },
    
    registerControl : function(id) {
        var term = new ise.Controls.PaymentTermControl(id);
        
        this.terms[id] = term;
        
        this.notifyObservers(term);
        
        return term;
    },
    
    addObserver : function(observer) {
        if(observer) {
            this.observers[this.observers.length] = observer;
        }
    },
    
    notifyObservers : function(control) {
        for(var ctr=0; ctr< this.observers.length; ctr++) {
            this.observers[ctr].notify(control);
        }
    },
    
    getControl : function(id) {
        var ctrl = this.terms[id];
        return ctrl;
    }
        
}

ise.Controls.PaymentTermController.initialize();

