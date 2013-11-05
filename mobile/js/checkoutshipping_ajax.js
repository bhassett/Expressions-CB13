// register namespace
Type.registerNamespace('ise.Pages');

ise.Pages.CheckOutShipping = {
    
    initialize : function() {
        this.shippingMethodControl = null;
        this.form = null;
        var thisObject = this;
        $(document).ready(function () {
            thisObject.showLoader();
            thisObject.initializeLoaderListener();
        })
    },
    
    setShippingMethodControlId : function(id) {
        var del = Function.createDelegate(this, this.setShippingMethodControl);
        this.loadControl(id, ise.Controls.ShippingMethodController, del);
    },
    
    setPaymentTermControl : function(control) {
        if(control) {
            this.paymentTermControl = control;
        }
    },
    
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

    showLoader: function () {
        $("body").data("globalLoader").show();
    },

    initializeLoaderListener: function () {

        var thisObject = this;
        $(window).load(function () {

            var checkerID = 0;

            var checker = function () {
                var shippingMethodControl = thisObject.shippingMethodControl;
                if (shippingMethodControl != null && shippingMethodControl != 'undefined') {
                    if (shippingMethodControl.isShippingOnDemand == 'true') {

                        if (!shippingMethodControl.isAutoClickOfOption) {
                            $("body").data("globalLoader").hide();
                            clearInterval(checkerID);
                        }
                    }
                    else {
                        if (shippingMethodControl.shippingMethodLoadingComplete) {
                            $("body").data("globalLoader").hide();
                            clearInterval(checkerID);
                        }
                    }
                }
                else {
                    $("body").data("globalLoader").hide();
                    clearInterval(checkerID);
                }
            }
            checkerID = setInterval(checker, 200);
        });
    },
    
    setForm : function(id) {
        this.form = $getElement(id);
        
        if(this.form) {
            this.form.onsubmit = Function.createDelegate(this, this.validate);           
            
            $disableSubmit(this.form);
        }
    },
    
    setShippingMethodControl : function(control) {
        if(control) {
            this.shippingMethodControl = control;
            
            var del = Function.createDelegate(this, this.shippingOptionsLoadedEventHandler);
            control.addShippingOptionsLoadedEventHandler(del);
        }
    },
    
    shippingOptionsLoadedEventHandler : function() {        
        $enableSubmit(this.form);
        
    },
    
    clearValidations : function() {
        this.shippingMethodControl.clearValidationSummary();
    },
    
    validate : function() {
        if(this.shippingMethodControl) {
            var isValid = false;
            var clear = false;
            
            this.clearValidations();
            
            isValid = this.shippingMethodControl.validate(clear);
            
            return isValid;
        }
        else {
            return true;
        }
    }

}

ise.Pages.CheckOutShipping.initialize();
