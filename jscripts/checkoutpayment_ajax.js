// register namespace
Type.registerNamespace('ise.Pages');

ise.Pages.CheckOutPayment = {

    initialize: function () {
        this.paymentTermControl = null;
        this.addressControl = null;
        this.form = null;
    },

    setPaymentTermControlId: function (id) {
        var del = Function.createDelegate(this, this.setPaymentTermControl);
        this.loadControl(id, ise.Controls.PaymentTermController, del);
    },

    setAddressControlId: function (id) {
        var del = Function.createDelegate(this, this.setAddressControl);
        this.loadControl(id, ise.Controls.AddressController, del);
    },

    setPaymentTermControl: function (control) {
        if (control) {
            this.paymentTermControl = control;
        }
    },

    setAddressControl: function (control) {
        if (control) {
            this.addressControl = control;
        }
    },

    loadControl: function (id, controller, setControlDelegate) {
        var control = controller.getControl(id);
        if (control) {
            setControlDelegate(control);
        }
        else {
            var observer = {

                notify: function (control) {
                    if (control) {
                        if (control.id == id) {
                            setControlDelegate(control);
                        }
                    }
                }
            }
            controller.addObserver(observer);
        }
    },

    setForm: function (id) {
        this.form = $getElement(id);
        var thisForm = this.form;
        if (this.form) {
            var del = Function.createDelegate(this, this.validate);
            this.form.onsubmit = del;
        }
    },

    clearValidations: function () {
        this.paymentTermControl.clearValidationSummary();
    },

    validate: function () {
        if (this.paymentTermControl) {
            var isValid = false;
            var clear = false;

            this.clearValidations();

            //validate address if tokenization
            var isvalid2 = true;
            if (this.paymentTermControl.getPaymentMethod() == "Credit Card" &&
                    this.paymentTermControl.IsTokenization &&
                        this.addressControl) {

                isvalid2 = this.addressControl.validate(clear);

            }

            isValid = this.paymentTermControl.validate(clear);
            return isValid && isvalid2;

        }
        else {
            return true;
        }
    }

}

ise.Pages.CheckOutPayment.initialize();
