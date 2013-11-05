// register namespace
Type.registerNamespace('ise.Pages');

ise.Pages.CheckOutPayment = {
    
    initialize : function() {
        this.paymentTermControl = null;
        this.form = null;
    },
    
    setPaymentTermControlId : function(id) {
        var del = Function.createDelegate(this, this.setPaymentTermControl);
        this.loadControl(id, ise.Controls.PaymentTermController, del);
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
    
    setForm : function(id) {
        this.form = $getElement(id);
        var thisForm = this.form;
        if(this.form) {
            var del = Function.createDelegate(this, this.validate);
            this.form.onsubmit = del;
        }
    },
    
    clearValidations : function() {
        this.paymentTermControl.clearValidationSummary();
    },
    
    validate : function() {
        if(this.paymentTermControl) {
            var isValid = false;
            var clear = false;
            
            this.clearValidations();
            
            isValid = this.paymentTermControl.validate(clear);
            
            return isValid;
        }
        else {
            return true;
        }
    }

}

ise.Pages.CheckOutPayment.initialize();
