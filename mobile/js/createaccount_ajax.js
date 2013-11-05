Type.registerNamespace('ise.Pages');

ise.Pages._CreateAccount = function() {
    ise.Pages._CreateAccount.initializeBase(this);

    this.chkSameAsBillingControl = null;
    this.billingAddressControl = null;
    this.shippingAddressControl = null;
    
    this.onFormSet = this.onFormSetHandler;
}
ise.Pages._CreateAccount.registerClass('ise.Pages._CreateAccount', ise.Pages.BasePage);
ise.Pages._CreateAccount.prototype = {
    setBillingAddressControlId: function (id) {
        var del = Function.createDelegate(this, this.setBillingAddressControl);
        this.loadControl(id, ise.Controls.AddressController, del);
    },

    setShippingAddressControlId: function (id) {
        var del = Function.createDelegate(this, this.setShippingAddressControl);
        this.loadControl(id, ise.Controls.AddressController, del);
    },

    setBillingAddressControl: function (control) {
        if (control) {
            this.billingAddressControl = control;
        }
    },

    setChkSameAsBillingControl: function (control) {
        if (control) {
            this.chkSameAsBillingControl = control;
        }
    },

    setChkSameAsBillingControlId: function (id) {
        this.setChkSameAsBillingControl($getElement(id));
    },

    setShippingAddressControl: function (control) {
        if (control) {
            this.shippingAddressControl = control;
        }
    },

    clearValidations: function () {
        this.billingAddressControl.clearValidationSummary();
        if (this.shippingAddressControl) {
            this.shippingAddressControl.clearValidationSummary();
        }
    },

    onFormSetHandler: function () {
        if (this.form) {
            var del = Function.createDelegate(this, this.validate);
            this.form.onsubmit = del;
        }
    },

    tryCopyBillingToShipping: function () {

        if (!this.chkSameAsBillingControl.checked) return;

        this.shippingAddressControl.setValue(this.billingAddressControl.getValue());
    },

    validate: function () {
        var isValid = false;
        var clear = false;

        this.clearValidations();

        this.tryCopyBillingToShipping();

        isValid = this.billingAddressControl.validate(clear);
        if (this.shippingAddressControl) {
            isValid = isValid && this.shippingAddressControl.validate(clear);
        }

        return isValid > 0;
    }
}
ise.Pages.CreateAccount = new ise.Pages._CreateAccount();