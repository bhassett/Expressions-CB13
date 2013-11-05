Type.registerNamespace('ise.Pages');

ise.Pages._OnePageCheckOut = function() {
    this.billingAddressControl = null;
    this.shippingAddressControl = null;
    this.billingAddressSelectorControl = null;
    this.shippingAddressSelectorControl = null;
    this.shippingMethodControl = null;
    this.paymentTermControl = null;
    this.form = null;
    this.isSameAddress = null;
}
ise.Pages._OnePageCheckOut.registerClass('ise.Pages._OnePageCheckOut', ise.Pages.BasePage);
ise.Pages._OnePageCheckOut.prototype = {

    setBillingAddressControlId: function (id) {
        var handler = Function.createDelegate(this, this.setBillingAddressControl);
        this.loadControl(id, ise.Controls.AddressController, handler); //this.setBillingAddressControl.bind(this));
    },

    setBillingAddressSelectorControlId: function (id) {
        var handler = Function.createDelegate(this, this.setBillingAddressSelectorControl);
        this.loadControl(id, ise.Controls.AddressSelectorController, handler); //this.setBillingAddressSelectorControl.bind(this));
    },

    setShippingAddressSelectorControlId: function (id) {
        var handler = Function.createDelegate(this, this.setShippingAddressSelectorControl);
        this.loadControl(id, ise.Controls.AddressSelectorController, handler); //this.setShippingAddressSelectorControl.bind(this));
    },

    setShippingAddressControlId: function (id) {
        var handler = Function.createDelegate(this, this.setShippingAddressControl);
        this.loadControl(id, ise.Controls.AddressController, handler); //this.setShippingAddressControl.bind(this));
    },

    setShippingMethodControlId: function (id) {
        var handler = Function.createDelegate(this, this.setShippingMethodControl);
        this.loadControl(id, ise.Controls.ShippingMethodController, handler); //this.setShippingMethodControl.bind(this));
    },

    setPaymentTermControlId: function (id) {
        var handler = Function.createDelegate(this, this.setPaymentTermControl);
        this.loadControl(id, ise.Controls.PaymentTermController, handler); //this.setPaymentTermControl.bind(this));
    },

    setControlEvents: function (src_id, des_id, chk_id, shp_id) {

        var elemChkCtrls = $getElement(chk_id);
        var elemShpMethd = $getElement(shp_id + '_Refresh');
        var elemControls = new Array("_AccountName:keyup",
                                     "_Address:keyup",
                                     "_WithStateCity:keyup",
                                     "_WithStateState:change",
                                     "_WithStateState:keyup",
                                     "_WithStatePostalCode:keyup",
                                     "_WithoutStateCity:keyup",
                                     "_WithoutStatePostalCode:keyup",
                                     "_Phone:keyup");

        for (var eCtrl = 0; eCtrl < elemControls.length; eCtrl++) {
            var elemBill = $getElement(src_id + elemControls[eCtrl].split(":")[0]);
            var elemShip = $getElement(des_id + elemControls[eCtrl].split(":")[0]);
            var elemEvnt = elemControls[eCtrl].split(":")[1];

            if (null != elemBill) {
                $addHandler(elemBill, elemEvnt, function () {
                    if (elemChkCtrls.checked == true) {
                        $getElement(des_id + "_" + this.id.split("_")[1]).value = this.value;

                        var elemName = this.id.split("_")[1];
                        if (elemName != "AccountName" && elemName != "Address" && elemName != "Phone") {
                            $getElement('ctrlShippingMethod_Content').innerHTML = '<table border=0></table>';
                        }
                    }
                });
            }

            if (null != elemShip) {
                $addHandler(elemShip, elemEvnt, function () {
                    if (elemChkCtrls.checked == false) {

                        //Select the frist item in the list which should be the default blank item.
                        ise.Pages.OnePageCheckOut.shippingAddressSelectorControl.setSelectedIndex(0, false);

                        var elemName = this.id.split("_")[1];
                        if (elemName != "AccountName" && elemName != "Address" && elemName != "Phone") {
                            $getElement('ctrlShippingMethod_Content').innerHTML = '<table border=0></table>';
                        }
                    }
                });
            }
        }

        var elemBillCountry = $getElement(src_id + "_Country");
        var elemShipCountry = $getElement(des_id + "_Country");

        if (null != elemBillCountry) {
            $addHandler(elemBillCountry, "change", function () {
                if (elemChkCtrls.checked == true) {

                    elemShipCountry.value = elemBillCountry.value;
                    ise.Utils.fireEvent(elemShipCountry, 'change');
                    $getElement('ctrlShippingMethod_Content').innerHTML = '<table border=0></table>';
                }
            });
        }
        if (null != elemShipCountry) {
            $addHandler(elemShipCountry, "change", function () {
                if (elemChkCtrls.checked == false) {

                    //Select the frist item in the list which should be the default blank item.
                    ise.Pages.OnePageCheckOut.shippingAddressSelectorControl.setSelectedIndex(0, false);

                    $getElement('ctrlShippingMethod_Content').innerHTML = '<table border=0></table>';
                }
            });
        }

        this.isSameAddress = elemChkCtrls;

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

        if (this.form) {
            this.form.onsubmit = Function.createDelegate(this, this.validate);
        }
    },

    setBillingAddressControl: function (control) {
        if (control) {
            this.billingAddressControl = control;
        }
    },

    setShippingAddressControl: function (control) {
        if (control) {
            this.shippingAddressControl = control;

            this.attachShippingMethodRefreshHandler();
        }
    },

    setBillingAddressSelectorControl: function (control) {
        if (control) {
            this.billingAddressSelectorControl = control;

            var handler = Function.createDelegate(this, this.handleBillingAddressSelectorSelectedAddressChanged);
            this.billingAddressSelectorControl.setSelectedAddressChangedEventHandler(handler);
        }
    },

    setShippingAddressSelectorControl: function (control) {
        if (control) {
            this.shippingAddressSelectorControl = control;

            var handler = Function.createDelegate(this, this.handleShippingAddressSelectorSelectedAddressChanged);
            this.shippingAddressSelectorControl.setSelectedAddressChangedEventHandler(handler);
        }
    },

    setShippingMethodControl: function (control) {
        if (control) {
            this.shippingMethodControl = control;

            this.attachShippingMethodRefreshHandler();
        }
    },

    setPaymentTermControl: function (control) {
        if (control) {
            this.paymentTermControl = control;
        }
    },

    handleBillingAddressSelectorSelectedAddressChanged: function () {
        var address = this.billingAddressSelectorControl.getSelectedAddress();
        this.billingAddressControl.setValue(address);

        if (address.nameOnCard) {
            this.paymentTermControl.setNameOnCard(address.nameOnCard);
        }
        if (address.cardNumber) {
            this.paymentTermControl.setCardNumber(address.cardNumber);
        }
        if (address.cardType) {
            this.paymentTermControl.setCardType(address.cardType);
        }
        if (address.expMonth) {
            this.paymentTermControl.setExpiryMonth(address.expMonth);
        }
        if (address.expYear) {
            this.paymentTermControl.setExpiryYear(address.expYear);
        }
    },

    handleShippingAddressSelectorSelectedAddressChanged: function () {
        var address = this.shippingAddressSelectorControl.getSelectedAddress();

        //Only set the address and refresh the items if it's not the default blank item.
        if (address.id != "DEFAULT") {
            this.shippingAddressControl.setValue(address);
            //When the address is changed refresh the shipping methods.
            this.shippingMethodControl.onRefreshClicked(address.id);
        }
    },

    attachShippingMethodRefreshHandler: function () {
        if (this.shippingMethodControl && this.shippingAddressControl) {
            var handler = Function.createDelegate(this, this.handleShippingMethodRefresh);
            this.shippingMethodControl.setRefreshClickedEventHandler(handler);
        }
    },

    handleShippingMethodRefresh: function () {
        this.clearValidations();

        var allow = false;
        allow = this.shippingAddressControl.validate(true);

        if (allow) {
            this.shippingMethodControl.setAddressValue(this.shippingAddressControl.serialize());
        }

        return allow;
    },

    SaveRecentData: function () {

        var index = $("input:radio[cc=cinfo][checked=true]").attr('index');
        var recenObject = {};
        recenObject.CurrentOption = index;

        var jsonString = JSON.stringify(recenObject);
        $("#hidRecentData").val(jsonString);

    },

    clearValidations: function () {
        this.billingAddressControl.clearValidationSummary();
        this.shippingAddressControl.clearValidationSummary();
        if (this.shippingMethodControl) {
            this.shippingMethodControl.clearValidationSummary();
        }
        this.paymentTermControl.clearValidationSummary();
    },

    validate: function () {
        var isValid = false;
        var clear = false;

        this.clearValidations();
        this.SaveRecentData();

        isValid =
        this.billingAddressControl.validate(clear) &
        this.shippingAddressControl.validate(clear) &
        this.paymentTermControl.validate(clear);

        if (this.shippingMethodControl) {
            isValid = isValid && this.shippingMethodControl.validate();
        }

        //-> added script: to hide place order button if form is valid and UseFinalReviewOrderPage is false

        if (isValid == 'true' || isValid > 0) {

            var UseFinalReviewOrderPage = $('#flag-use-final-review-order-page').html();
            UseFinalReviewOrderPage = trim(UseFinalReviewOrderPage);

            if (UseFinalReviewOrderPage == 'false') {

               $('#btnCompletePurchase').css('display', 'none');
               $('#place-order-button-container').fadeIn('slow');

            }

        }

       // <--

        return isValid > 0;
    }

}

ise.Pages.OnePageCheckOut = new ise.Pages._OnePageCheckOut();

