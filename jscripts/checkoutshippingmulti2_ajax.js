Type.registerNamespace('ise.Pages');

ise.Pages._CheckOutShippingMulti2 = function() {
    this.shippingMethodControls = new Array();
    this.form = null;
    this.expectedCount = 0;
    this.loadCount = 0;
}
ise.Pages._CheckOutShippingMulti2.registerClass('ise.Pages._CheckOutShippingMulti2', ise.Pages.BasePage);
ise.Pages._CheckOutShippingMulti2.prototype = {

    init: function () {
        var thisObject = this;
        $(document).ready(function () {
            thisObject.showLoader();
            thisObject.initializeLoaderListener();
        })
    },

    registerShippingMethodControlId : function(id) {
        var control = ise.Controls.ShippingMethodController.getControl(id);
        
        if(control) {
            this.addShippingMethodControl(control);
        }
        else {
            var addControlDelegate = Function.createDelegate(this, this.addShippingMethodControl); //.bind(this);
            
            var observer =  {
            
                notify : function(registeredControl) {
                    if(registeredControl.id == id) {
                        addControlDelegate(registeredControl);
                    }                    
                }
                
            }
            
            ise.Controls.ShippingMethodController.addObserver(observer);
        }

    },

    showLoader: function () {
        $("body").data("globalLoader").show();
    },

    initializeLoaderListener: function () {

        var thisObject = this;
        $(window).load(function () {

            var checkerID = 0;

            var checker = function () {
                var shippingMethodControls = thisObject.shippingMethodControls;

                if (shippingMethodControls != null) {
                    if (shippingMethodControls[0].isShippingOnDemand == "true") {

                        if (thisObject.isShippingMethodsDoneGettingRates()) {
                            $("body").data("globalLoader").hide();
                            clearInterval(checkerID);
                        }

                    }
                    else {

                        if (thisObject.isShippingMethodsAlreadyLoaded()) {
                            $("body").data("globalLoader").hide();
                            clearInterval(checkerID);
                        }

                    }
                }
                // Added to support Free Shipping - no shipping method control
                else {
                    $("body").data("globalLoader").hide();
                    clearInterval(checkerID);
                }
            }
            checkerID = setInterval(checker, 200);
        });
    },

    isShippingMethodsDoneGettingRates: function () {

        var allDone = false;

        if (this.shippingMethodControls[0].isShippingOnDemand == 'true') {
            for (var i = 0; i < this.shippingMethodControls.length; i++) {

                if (this.shippingMethodControls[i].isAutoClickOfOption) {
                    allDone = false;
                    break;
                }
                allDone = true;
            }
        }
        else {
            allDone = true;
        }
        return allDone;
    },

    isShippingMethodsAlreadyLoaded: function () {

        var allDone = false;
        for (var i = 0; i < this.shippingMethodControls.length; i++) {
            if (!this.shippingMethodControls[i].shippingMethodLoadingComplete) {
                allDone = false;
                break;
            }
            allDone = true;
        }

        return allDone;
    },
    
    setExpectedCount : function(count) {
        this.expectedCount = count;
    },
    
    setForm : function(id) {
        this.form = $getElement(id);
        
        if(this.form) {
            this.form.onsubmit = Function.createDelegate(this, this.validate); //this.validate.bind(this);
            $disableSubmit(this.form);
        }
    },
    
    clearValidations : function() {
        for(var ctr=0; ctr<this.shippingMethodControls.length; ctr++) {
            var shippingMethodControl = this.shippingMethodControls[ctr];
            shippingMethodControl.clearValidationSummary();
        }
    },
    
    addShippingMethodControl : function(control) {
        this.shippingMethodControls.push(control);
        
        var handler = Function.createDelegate(this, this.shippingOptionsLoadedEventHandler);
        control.addShippingOptionsLoadedEventHandler(handler);
    },
    
    shippingOptionsLoadedEventHandler : function() {
        if(this.expectedCount != this.loadCount) {
            this.loadCount++;
            this.checkAllShippingOptionsAreLoaded();
        }
    },
    
    checkAllShippingOptionsAreLoaded : function() {
        if(this.expectedCount == this.loadCount) {
            $enableSubmit(this.form);
        }
    },
    
    validate : function() {
        var anyNotValid = false;
        var clear = false;
        
        this.clearValidations();
        
        for(var ctr=0; ctr<this.shippingMethodControls.length; ctr++) {
            var shippingMethodControl = this.shippingMethodControls[ctr];
            anyNotValid = !shippingMethodControl.validate(clear);
            
            if(anyNotValid) {
                break;
            }
        }
        
        return !anyNotValid
    }

}

ise.Pages.CheckOutShippingMulti2 = new ise.Pages._CheckOutShippingMulti2();
ise.Pages.CheckOutShippingMulti2.init();

