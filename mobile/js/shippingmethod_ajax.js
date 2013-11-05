// register namespace
Type.registerNamespace('ise.Controls');

ise.Controls.ShippingMethodOption = function (ctrl, shippingMethod, freightCalculation, freight, rateID, current) {
    this.id = ctrl.id;
    this.ctrl = ctrl;
    this.shippingMethod = shippingMethod;
    this.freightCalculation = freightCalculation;
    this.freight = freight;
    this.rateID = rateID;
    this.shippingInfo = current;

    var del = Function.createDelegate(this, this.handleOptionClick);
    $addHandler(this.ctrl, 'click', del);
    this.selectedEventHandler = null;
}
ise.Controls.ShippingMethodOption.registerClass('ise.Controls.ShippingMethodOption');
ise.Controls.ShippingMethodOption.prototype = {
    
    handleOptionClick : function(option) {
        this.onSelected(option);
    },
    
    onSelected : function(option) {
        if(option) {
            if(this.selectedEventHandler) {
                this.selectedEventHandler(this);
                //hdenOnpage - hidden field indicator that where in the checkout1.aspx
                //if undefined where in the checkoutshipping.aspx
                if($getElement('hdenOnpage') != undefined) {
                    this.updateSummary(this);
                }
            }
        }
    },
    
    getIsChecked : function() {
        if(this.ctrl) {
            return this.ctrl.checked;
        }
        
        return false;
    },
    onCalculationSummaryComplete: function (summary) {
        
        if (summary == undefined) return;

        $getElement('subtotal_elem').innerHTML = summary.SubTotal;
        $getElement('fr_elem').innerHTML = summary.Freight;
        $getElement('tax_elem').innerHTML = summary.Tax;
        $getElement('total_elem').innerHTML = summary.DueTotal;

        var main = this.id.split('_')[0];
        var loadingRow = $getElement(main + '_LoadingRow');
        if (null != loadingRow) { ise.Utils.hideRow(loadingRow); }

        var container = $getElement(main + '_Content');
        if (null != container) { ise.Utils.showRow(container); }

    },

    updateSummary: function (seloption) {
        var main = this.id.split('_')[0];
        var loadingRow = $getElement(main + '_LoadingRow');

        if (null != loadingRow) {
            ise.Utils.showRow(loadingRow);
        }

        var container = $getElement(main + '_Content');
        if (null != container) {
            ise.Utils.hideRow(container);
        }

        var service = new ActionService();
        var calculationSummary = service.GetShippingCalculation(seloption.shippingMethod, seloption.freightCalculation, seloption.rateID, Function.createDelegate(this, this.onCalculationSummaryComplete));
    },
    setSelectedEventHandler : function(handler) {
        return this.selectedEventHandler = handler;
    },
    
    getShippingMethod : function() {
        return this.shippingMethod;
    },

    getFreightCalculation: function () {
        return this.freightCalculation;
    },

    getFreight: function () {
        return this.freight;
    },

    getRateID: function () {
        return this.rateID;
    }
    
}

// Constructor
ise.Controls.ShippingMethodControl = function (id) {
    this.id = id;
    this.ctrl = $getElement(id);
    this.addressControlId = null;
    this.addressControl = null;
    this.shippingMethodID = null;
    this.skipShipping = false;
    this.options = new Array();

    this.currentOption = null;

    this.validationController = new ise.Validators.ValidationController();

    if (this.ctrl) {
        this.hookUpRefreshLink();
    }

    // delegate
    this.refreshClickedEventHandler = null;

    this.shippingOptionsLoadedEventHandlers = new Array();

    this.addressValue = null;

    this.shippingMethodLoadingComplete = false;

    this.isShippingOnDemand = ise.Configuration.getConfigValue("ShippingRatesOnDemand");
    this.isAutoClickOfOption = (ise.Configuration.getConfigValue("ShippingRatesOnDemand") == 'true');
}
ise.Controls.ShippingMethodControl.registerClass('ise.Controls.ShippingMethodControl');
ise.Controls.ShippingMethodControl.prototype = {

    getIsSkipShipping: function () {
        return this.skipShipping;
    },

    setIsSkipShipping: function (skip) {
        this.skipShipping = skip;
    },

    hookUpRefreshLink: function () {
        var link = $getElement(this.id + '_Refresh');

        if (link) {
            link.onclick = Function.createDelegate(this, this.handleRefreshClick);
        }
    },

    handleRefreshClick: function () {
        this.onRefreshClicked();
    },

    onRefreshClicked: function (adressid) {
        var shippingInfoValid = false;

        if (this.refreshClickedEventHandler) {
            shippingInfoValid = this.refreshClickedEventHandler(this);
        }

        if (shippingInfoValid) {
            this.requestShippingMethod(adressid);
        }
    },

    setRefreshClickedEventHandler: function (handler) {
        this.refreshClickedEventHandler = handler;
    },

    clearOptions: function () {
        this.options.length = 0;

        this.setShippingMethod(ise.Constants.EMPTY_STRING);
    },

    getCurrentOption: function () {
        return this.currentOption;
    },

    setCurrentOption: function (option, callback) {

        this.currentOption = option;
        this.setShippingMethod(option.getShippingMethod());
        this.setFreightCalculation(option.getFreightCalculation());
        this.setFreight(option.getFreight());
        this.setRateID(option.getRateID());

        var thisObject = this;

        if (this.isShippingOnDemand == "true" && option.shippingInfo.FreightDisplay != "FREE") {

            var freight = $("#" + option.id).parent("div").children("span").children("span:last")

            if (freight.hasClass("shipping-rate")) {
                var rate = $("#" + option.id).parent("div").children("span").children("span:last").html();
                thisObject.setFreight(rate);
                return false;
            }

            if (!this.isAutoClickOfOption) {
                $("body").data("globalLoader").show();
            }

            AjaxCallWithSecuritySimplified(
                "GetShippingMethodRates",
                { "shippingMethodInfo": option.shippingInfo, "addressNameValuPairOverride": this.addressValue ? this.addressValue : '', "addressId": this.shippingMethodID },
                function (result) {
                    thisObject.onLoadShippingRate(result.d);
                    thisObject.setFreight(result.d);
                    if (typeof callback != 'undefined') callback();

                },
                null, this.serviceToken
            );
        }
        else {
            if (typeof callback != 'undefined') callback();
            $("body").data("globalLoader").hide();
        }
    },

    onLoadShippingRate: function (rate) {
        $("#" + this.currentOption.id).parent("div").children("span").children("span:last").html(rate).addClass("shipping-rate");

        if (!this.isAutoClickOfOption) {
            $("body").data("globalLoader").hide();
        }

        if (rate == "(Not Applicable)") {

            $("#" + this.currentOption.id).parent("div").children("span").children("span:last").addClass("shipping-rate-not-applicable");
            $("#" + this.currentOption.id).attr("checked", false);
            $("#" + this.currentOption.id).attr("disabled", "disabled");
        }

    },

    registerOption: function (option) {
        this.options[this.options.length] = option;

        if (option.getIsChecked()) {
            var thisObject = this;
            var callback = function () {
                thisObject.isAutoClickOfOption = false;
            }
            this.setCurrentOption(option, callback);
        }

        var del = Function.createDelegate(this, this.handleOptionSelected);
        option.setSelectedEventHandler(del);
    },

    handleOptionSelected: function (option) {
        this.setCurrentOption(option);
    },

    setAddressControlId: function (addressId) {
        this.addressControlId = addressId;

        if (ise.Controls.AddressController) {
            var control = ise.Controls.AddressController.getControl(addressId);
            if (null != control) {
                this.addressControl = control;
                this.requestShippingMethod(true);
            }
            else {
                ise.Controls.AddressController.addObserver(this);
            }
        }
    },

    notify: function (ctrl) {
        if (null != this.addressControlId && ctrl) {
            if (ctrl.id == this.addressControlId) {
                this.addressControl = ctrl;
                this.requestShippingMethod(true);
            }
        }
    },

    setAddressValue: function (value) {
        this.addressValue = value;
    },

    addShippingOptionsLoadedEventHandler: function (handler) {
        this.shippingOptionsLoadedEventHandlers.push(handler);
    },

    onShippingOptionsLoaded: function () {
        for (var ctr = 0; ctr < this.shippingOptionsLoadedEventHandlers.length; ctr++) {
            var handler = this.shippingOptionsLoadedEventHandlers[ctr];
            handler(this);
        }
    },

    onLoadShippingMethodComplete: function (shippingMethods) {
        this.clearOptions();

        this.shippingMethodLoadingComplete = true;

        var contentArea = $getElement(this.id + '_content');
        contentArea.className = 'content';

        contentArea.innerHTML = '';

        var tbl = document.createElement("div");
        contentArea.appendChild(tbl);

        var anyShippingMethodValid = false;

        try {
            for (var ctr = 0; ctr < shippingMethods.length; ctr++) {
                var current = shippingMethods[ctr];

                if (current.ForOversizedItem)
                    continue;
                var td = document.createElement("div");

                td.className = 'shipping-option';
                $(tbl).append(td);

                var shippingMethodObject = new Object();
                var html = null;

                if (current.IsError) {
                    shippingMethodObject.disabled = 'disabled=disabled';
                    shippingMethodObject.description = current.Description;
                    html = $.tmpl("shippingMethodOptionTemplateID", shippingMethodObject);
                    $(td).append(html);
                }
                else {
                    shippingMethodObject.id = this.id + "_shippingMethod_" + ctr;
                    shippingMethodObject.name = this.id + ":shippingMethod";
                    shippingMethodObject.description = current.Description;
                    shippingMethodObject.isDefault = (current.IsDefault) ? 'checked=checked' : "";
                    shippingMethodObject.forOversizedItem = current.ForOversizedItem;
                    if (this.isShippingOnDemand == "true") {
                        if (current.FreightDisplay == "FREE") {
                            shippingMethodObject.freight = current.FreightDisplay;
                        }
                        else {
                            shippingMethodObject.freight = "";
                        }
                    }
                    else {
                        shippingMethodObject.freight = current.FreightDisplay;
                    }

                    html = $.tmpl("shippingMethodOptionTemplateID", shippingMethodObject);
                    $(td).append(html);

                    var opt = $("#" + this.id + "_shippingMethod_" + ctr)[0];

                    if (opt) {
                        // register
                        if (this.isShippingOnDemand == "true") {
                            var option = new ise.Controls.ShippingMethodOption(opt, current.Code, current.FreightCalculation, current.Freight, current.RateID, current);
                        }
                        else {
                            var option = new ise.Controls.ShippingMethodOption(opt, current.Code, current.FreightCalculation, current.Freight, current.RateID, null);
                        }

                        this.registerOption(option);
                    }

                    anyShippingMethodValid = true;
                }
                $(tbl).append(td);
            }

            $(".shipping-description").unbind("click").click(function () {

                var id = $(this).attr("id").split("$");

                if (id.length > 0) {
                    id = id[1];
                } else {
                    id = id[0];
                }

                $("#" + id).trigger("click");

            });

            var shippingMethodOversizedInfo = new Array();

            for (var ctr = 0; ctr < shippingMethods.length; ctr++) {
                var current = shippingMethods[ctr];
                if (!current.ForOversizedItem)
                    continue;

                var shippingMethodOversizedObject = new Object();
                shippingMethodOversizedObject.ItemDescription = current.OversizedItemName;
                shippingMethodOversizedObject.ShippingDescription = current.Description;
                shippingMethodOversizedObject.Freight = current.FreightDisplay;
                shippingMethodOversizedInfo.push(shippingMethodOversizedObject);

                shippingMethodOversizedObject = null;

            }

            if (shippingMethodOversizedInfo.length > 0) {

                var stringResource = new Object();
                stringResource.oversizedMessage = ise.StringResource.getString('checkoutshipping.aspx.9');
                stringResource.productHeader = ise.StringResource.getString('checkoutshipping.aspx.10');
                stringResource.shippingMethodHeader = ise.StringResource.getString('checkoutshipping.aspx.11');
                stringResource.freightHeader = ise.StringResource.getString('checkoutshipping.aspx.12');

                var shippingMethodOversizedContent = new Object();
                shippingMethodOversizedContent.shippingMethodInfo = shippingMethodOversizedInfo;
                shippingMethodOversizedContent.stringResource = stringResource;

                html = $.tmpl("shippingMethodOversizedTemplateID", shippingMethodOversizedContent);
                $(tbl).append(html);
            }

        }
        catch (e) {
            alert(e);
            contentArea.innerHTML = e; //'Could not load shipping methods';
        }

        if (anyShippingMethodValid) {
            this.onShippingOptionsLoaded();
        }
    },

    onComplete: function (result) {
        alert(result);
    },

    requestShippingMethod: function (addressId) {
        if (addressId != null) {
            this.shippingMethodID = addressId;
        }
        var onCompleteDelegate = Function.createDelegate(this, this.onLoadShippingMethodComplete);
        var service = new ActionService();
        service.ShippingMethod(this.addressValue ? this.addressValue : '', this.id, this.shippingMethodID, onCompleteDelegate);
    },

    getShippingMethod: function () {
        var elemShippingMethod = $getElement(this.id + '_shippingMethod');
        if (elemShippingMethod) {
            return elemShippingMethod.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    setFreight: function (value) {
        var elemFreight = $getElement(this.id + '_freight');
        if (elemFreight) {
            elemFreight.value = value;
        }
    },

    setShippingMethod: function (value) {
        var elemShippingMethod = $getElement(this.id + '_shippingMethod');
        if (elemShippingMethod) {
            elemShippingMethod.value = value;
        }
    },

    getFreightCalculation: function () {
        var elemFreightCalculation = $getElement(this.id + '_freightCalculation');
        if (elemFreightCalculation) {
            return elemFreightCalculation.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    setFreightCalculation: function (value) {
        var elemFreightCalculation = $getElement(this.id + '_freightCalculation');
        if (elemFreightCalculation) {
            elemFreightCalculation.value = value;
        }
    },

    getRateID: function () {
        var elemRateID = $getElement(this.id + '_realTimeRateGUID');
        if (elemRateID) {
            return elemRateID.value;
        }
        return ise.Constants.EMPTY_STRING;

    },

    setRateID: function (value) {
        var elemRateID = $getElement(this.id + '_realTimeRateGUID');
        if (elemRateID) {
            elemRateID.value = value;
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
        if (this.skipShipping) return true;

        return this.validationController.validate(clear);
    }

}

ise.Controls.ShippingMethodController = {

    initialize : function() {
        this.controls = new Array();
        this.observers = new Array();
    },
    
    registerControl : function(id) {
        var ctrl = new ise.Controls.ShippingMethodControl(id);
        this.controls[id] = ctrl;
        
        this.notifyObservers(ctrl);
        
        return ctrl;
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
        var ctrl = this.controls[id];
        return ctrl;
    }
    
}

ise.Controls.ShippingMethodController.initialize();
