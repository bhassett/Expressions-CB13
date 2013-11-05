// Ensure Root Namespaces are existing..
Type.registerNamespace('ise.Controls');

ise.Controls.AddressControl = function (id) {
    this.id = id;
    this.validationController = new ise.Validators.ValidationController();

    this.lblWithCityCaption = $getElement(this.id + '_WithCityCaption');
    this.lblWithStateCaption = $getElement(this.id + '_WithStateCaption');
    this.lblPostalCaption = $getElement(this.id + '_WithPostalCaption');

    this.lblWithStateCityStatePostalCaption = $getElement(this.id + '_WithStateCityStatePostalCaption');
    this.lblWithoutStatePostalCaption = $getElement(this.id + '_WithoutStatePostalCaption');

    var elemBusinessType = $getElement(id + "_BusinessType");
    if (null != elemBusinessType) {
        var del = Function.createDelegate(this, this.switchBusinessType);
        $addHandler(elemBusinessType, 'change', del);
    }

    var elemCountry = $getElement(id + "_Country");
    if (null != elemCountry) {
        var del = Function.createDelegate(this, this.switchCountry);
        $addHandler(elemCountry, 'change', del);
    }

    // now check for the auto input value of text for account name
    var elemFirstName = $getElement(this.id + "_FirstName");
    var elemLastName = $getElement(this.id + "_LastName");
    var elemAccountName = $getElement(this.id + "_AccountName");

    if (null != elemFirstName &&
        null != elemLastName &&
        null != elemAccountName) {

        var updateAccountName = function () {
            var currentValue = elemAccountName.value;
            if (currentValue == '') {
                currentValue = elemFirstName.value + ' ' + elemLastName.value;
                elemAccountName.value = currentValue;
            }
        }

        $addHandler(elemLastName, 'blur', updateAccountName);
    }

    this.postalCodeOptionalCountries = new Array();
}
ise.Controls.AddressControl.registerClass('ise.Controls.AddressControl');
ise.Controls.AddressControl.prototype = {

    switchBusinessType: function () {
        var elemBusinessType = $getElement(this.id + "_BusinessType");

        var taxRowId = $getElement(this.id + '_TaxNumberRow');

        if (elemBusinessType.value == "WholeSale") {
            ise.Utils.showRow(taxRowId);
        }
        else {
            ise.Utils.hideRow(taxRowId);
        }
    },

    clearStates: function () {
        var elemState = $getElement(this.id + "_StateDropDown");
        if (elemState) {
            elemState.options.length = 0;
        }
    },

    switchCountry: function () {
        var elemCountry = $getElement(this.id + "_Country");
        var elemState = $getElement(this.id + "_StateDropDown");

        if (null != elemCountry) {
            var country = ise.Controls.CountryRepository.findCountry(elemCountry.value);

            if (country.withState) {
                //ise.Utils.showRow(this.id + '_WithStateCityStatePostalRow');

                ise.Utils.showRow(this.id + '_row_WithCityCaption');
                ise.Utils.showRow(this.id + '_row_WithStateCaption');
                ise.Utils.showRow(this.id + '_row_WithStateCity');
                ise.Utils.showRow(this.id + '_row_StateDropDown');
                ise.Utils.showRow(this.id + '_row_WithCityCaption');
                ise.Utils.showRow(this.id + '_row_WithStateState');
                ise.Utils.showRow(this.id + '_row_WithPostalCaption');
                ise.Utils.showRow(this.id + '_row_WithStatePostalCode');

                ise.Utils.hideRow(this.id + '_row_WithoutStateCityCaption');
                ise.Utils.hideRow(this.id + '_row_WithoutStateCity');
                ise.Utils.hideRow(this.id + '_row_WithoutStatePostalCaption');
                ise.Utils.hideRow(this.id + '_row_WithoutStatePostalCode');

                //                ise.Utils.hideRow(this.id + '_row_WithoutStateCityRow');
                //                ise.Utils.hideRow(this.id + '_WithoutStateCountyPostalRow');

                var populateState = function (states) {
                    if (states.length == 0) {
                        elemState.options[0] = new Option('None', '');
                    }
                    else {
                        for (var ctr = 0; ctr < states.length; ctr++) {
                            var state = states[ctr];
                            var display = state.code + " - " + state.description;
                            elemState.options[ctr] = new Option(display, state.code);
                        }
                    }

                    elemState.disabled = false;
                };

                // clear the values then repopulate
                this.clearStates();
                elemState.options[0] = new Option('Fetching', '');
                elemState.disabled = true;

                if (null != country.states) {
                    populateState(country.states);
                }
                else {
                    var onGetStateComplete = function (states) {
                        country.states = states;
                        populateState(states);
                    }

                    var service = new ActionService();
                    service.GetStates(encodeURIComponent(country.code), onGetStateComplete);
                }
            }
            else {
                this.clearStates();

                //ise.Utils.hideRow(this.id + '_WithStateCityStatePostalRow');
                ise.Utils.hideRow(this.id + '_row_WithCityCaption');
                ise.Utils.hideRow(this.id + '_row_WithStateCaption');
                ise.Utils.hideRow(this.id + '_row_WithStateCity');
                ise.Utils.hideRow(this.id + '_row_WithCityCaption');
                ise.Utils.hideRow(this.id + '_row_WithStateState');
                ise.Utils.hideRow(this.id + '_row_StateDropDown');
                ise.Utils.hideRow(this.id + '_row_WithPostalCaption');
                ise.Utils.hideRow(this.id + '_row_WithStatePostalCode');

                ise.Utils.showRow(this.id + '_row_WithoutStateCityCaption');
                ise.Utils.showRow(this.id + '_row_WithoutStateCity');
                ise.Utils.showRow(this.id + '_row_WithoutStatePostalCaption');
                ise.Utils.showRow(this.id + '_row_WithoutStatePostalCode');

                //                ise.Utils.showRow(this.id + '_row_WithoutStateCityRow');
                //                ise.Utils.showRow(this.id + '_WithoutStateCountyPostalRow');
            }

            if (this.getIsPostalCodeOptional()) {
                this.lblWithCityCaption.innerHTML = ise.StringResource.getString('createaccount.aspx.95');
                this.lblWithStateCaption.innerHTML = ise.StringResource.getString('createaccount.aspx.96');
                this.lblPostalCaption.innerHTML = ise.StringResource.getString('createaccount.aspx.30');
                this.lblWithoutStatePostalCaption.innerHTML = ise.StringResource.getString('createaccount.aspx.30');
            }
            else {
                this.lblWithCityCaption.innerHTML = ise.StringResource.getString('createaccount.aspx.95');
                this.lblWithStateCaption.innerHTML = ise.StringResource.getString('createaccount.aspx.96');
                this.lblPostalCaption.innerHTML = ise.StringResource.getString('createaccount.aspx.30');
                this.lblWithoutStatePostalCaption.innerHTML = ise.StringResource.getString('createaccount.aspx.77');
            }
        }
    },

    getFirstName: function () {
        var elem = $getElement(this.id + '_FirstName');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getLastName: function () {
        var elem = $getElement(this.id + '_LastName');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getAccountName: function () {
        var elem = $getElement(this.id + '_AccountName');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getCountry: function () {
        var elem = $getElement(this.id + '_Country');
        if (elem) {
            return elem.options[elem.selectedIndex].value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getAddress: function () {
        var elem = $getElement(this.id + '_Address');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getResidenceType: function () {
        var elem = $getElement(this.id + '_ResidenceType');
        if (elem) {
            return elem.options[elem.selectedIndex].value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getBusinessType: function () {
        var elem = $getElement(this.id + '_BusinessType');
        if (elem) {
            return elem.options[elem.selectedIndex].value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getTaxNumber: function () {
        var elem = $getElement(this.id + '_TaxNumber');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getWithStateCity: function () {
        var elem = $getElement(this.id + '_WithStateCity');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getWithStateState: function () {
        var elem = $getElement(this.id + '_StateDropDown');
        if (elem) {
            return elem.options[elem.selectedIndex].value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getWithStatePostalCode: function () {
        var elem = $getElement(this.id + '_WithStatePostalCode');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getWithoutStateCity: function () {
        var elem = $getElement(this.id + '_WithoutStateCity');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getWithoutStatePostalCode: function () {
        var elem = $getElement(this.id + '_WithoutStatePostalCode');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getCounty: function () {
        var elem = $getElement(this.id + '_County');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    getPhone: function () {
        var elem = $getElement(this.id + '_Phone');
        if (elem) {
            return elem.value;
        }

        return ise.Constants.EMPTY_STRING;
    },

    setFirstName: function (value) {
        var elem = $getElement(this.id + '_FirstName');
        if (elem) {
            elem.value = value;
        }
    },

    setLastName: function (value) {
        var elem = $getElement(this.id + '_LastName');
        if (elem) {
            elem.value = value;
        }
    },

    setAccountName: function (value) {
        var elem = $getElement(this.id + '_AccountName');
        if (elem) {
            elem.value = value;
        }
    },

    setCountry: function (value) {
        var elem = $getElement(this.id + '_Country');
        if (elem) {
            // check if we should need to
            if (this.getCountry() != value) {
                var idx = 0;
                for (var ctr = 0; ctr < elem.options.length; ctr++) {
                    var option = elem.options[ctr];
                    if (option.value == value) {
                        idx = ctr;
                        break;
                    }
                }

                elem.selectedIndex = idx;
                this.switchCountry();
            }
        }
    },

    setAddress: function (value) {
        var elem = $getElement(this.id + '_Address');
        if (elem) {
            elem.value = value;
        }
    },

    setResidenceType: function (value) {
        var elem = $getElement(this.id + '_ResidenceType');
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

    setBusinessType: function (value) {
        var elem = $getElement(this.id + '_Businesstype');
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

            this.switchBusinessType();
        }
    },

    setTaxNumber: function (value) {
        var elem = $getElement(this.id + '_TaxNumber');
        if (elem) {
            elem.value = value;
        }
    },

    setWithStateCity: function (value) {
        var elem = $getElement(this.id + '_WithStateCity');
        if (elem) {
            elem.value = value;
        }
    },

    setWithStateState: function (value) {
        var elem = $getElement(this.id + '_StateDropDown');
        if (elem) {
            var idx = 0;
            for (var ctr = 0; ctr < elem.options.length; ctr++) {
                var option = elem.options[ctr];
                if (option.value == value) {
                    idx = ctr;
                    break;
                }
            }

            if ($('#hdenStateValue')) {
                $('#hdenStateValue').val(value);
            }

            elem.options[idx].selected = "selected";
            elem.options[idx].value = value;
            elem.selectedIndex = idx;
        }
    },

    setWithStatePostalCode: function (value) {
        var elem = $getElement(this.id + '_WithStatePostalCode');
        if (elem) {
            elem.value = value;
        }
    },

    setWithoutStateCity: function (value) {
        var elem = $getElement(this.id + '_WithoutStateCity');
        if (elem) {
            elem.value = value;
        }
    },

    setWithoutStatePostalCode: function (value) {
        var elem = $getElement(this.id + '_WithoutStatePostalCode');
        if (elem) {
            elem.value = value;
        }
    },

    setCounty: function (value) {
        var elem = $getElement(this.id + '_County');
        if (elem) {
            elem.value = value;
        }
    },

    setPhone: function (value) {
        var elem = $getElement(this.id + '_Phone');
        if (elem) {
            elem.value = value;
        }
    },

    getValue: function () {
        var country = this.getCountry();
        var countryInfo = ise.Controls.CountryRepository.findCountry(country);
        var city = ise.Constants.EMPTY_STRING;
        var state = ise.Constants.EMPTY_STRING;
        var postalCode = ise.Constants.EMPTY_STRING;
        var county = ise.Constants.EMPTY_STRING;

        if (countryInfo.withState) {
            city = this.getWithStateCity();
            state = this.getWithStateState();
            postalCode = this.getWithStatePostalCode();
        }
        else {
            city = this.getWithoutStateCity();
            postalCode = this.getWithoutStatePostalCode();
        }

        var address = {
            firstName: this.getFirstName(),
            lastName: this.getLastName(),
            accountName: this.getAccountName(),
            address: this.getAddress(),
            country: country,
            city: city,
            state: state,
            withState: countryInfo.withState,
            postalCode: postalCode,
            county: this.getCounty(),
            residenceType: this.getResidenceType(),
            phone: this.getPhone()
        }

        return address;
    },

    setValue: function (address) {
        if (address) {
            this.setFirstName(address.firstName);
            this.setLastName(address.lastName);
            this.setAccountName(address.accountName);
            this.setCountry(address.country);
            this.setAddress(address.address);
            this.setResidenceType(address.residenceType);
            this.setWithStateCity(address.city);
            if (address.withState) {
                this.setWithStateState(address.state);
            }
            else {
                this.clearStates();
            }
            this.setWithStatePostalCode(address.postalCode);
            this.setWithoutStateCity(address.city);
            this.setWithoutStatePostalCode(address.postalCode);
            this.setCounty(address.county);
            this.setPhone(address.phone);
        }
    },

    //Made changes per defect #82
    //Created the DisableUnselectedDropdownItems in the base_ajax.js file.
    //Called this method for the drop downs that were being disabled.
    setDisabled: function (blnValue) {

        if (blnValue == true || blnValue == false) {

            $getElement(this.id + '_AccountName').readOnly = blnValue;
            ise.Utils.DisableUnselectedDropdownItems(blnValue, $getElement(this.id + '_Country'));
            $getElement(this.id + '_Address').readOnly = blnValue;
            ise.Utils.DisableUnselectedDropdownItems(blnValue, $getElement(this.id + '_ResidenceType'));
            $getElement(this.id + '_WithStateCity').readOnly = blnValue;
            $getElement(this.id + '_WithoutStateCity').readOnly = blnValue;
            $getElement(this.id + '_WithStatePostalCode').readOnly = blnValue;
            $getElement(this.id + '_WithoutStatePostalCode').readOnly = blnValue;
            //ise.Utils.DisableUnselectedDropdownItems(blnValue, $getElement(this.id + '_StateDropDown'));
            $getElement(this.id + '_Phone').readOnly = blnValue;



            // For hide/show of the Shipping Address Control.
            // ---

            // --
            // Shipping Method Content Panel
            // --
            if (null != $getElement('ctrlShippingMethod_Content')) {
                $getElement('ctrlShippingMethod_Content').innerHTML = '<table border=0></table>';
            }
            // --
            // --
            // Shipping Contreol Panel
            // --
            if (blnValue == true) {
                $getElement(this.id).style.display = 'none';

                if (null != $getElement('pnlSelectShippingAddress')) {
                    $getElement('pnlSelectShippingAddress').style.display = 'none';
                }
            }
            else {
                $getElement(this.id).style.display = 'inline-block';
                if (null != $getElement('pnlSelectShippingAddress')) {
                    $getElement('pnlSelectShippingAddress').style.display = 'block';
                }
            }
            // --


            // ---

        }
    },

    serialize: function () {
        var info = this.getValue();

        var serialized =
        "FirstName=" + encodeURIComponent(info.firstName) +
        "&LastName=" + encodeURIComponent(info.lastName) +
        "&AccountName=" + encodeURIComponent(info.accountName) +
        "&Address=" + encodeURIComponent(info.address) +
        "&Country=" + encodeURIComponent(info.country) +
        "&City=" + encodeURIComponent(info.city) +
        "&State=" + encodeURIComponent(info.state) +
        "&PostalCode=" + encodeURIComponent(info.postalCode) +
        "&County=" + encodeURIComponent(info.county) +
        "&ResidenceType=" + encodeURIComponent(info.residenceType) +
        "&Phone=" + encodeURIComponent(info.phone);

        return serialized;
    },

    setValidationSummary: function (summary) {
        this.validationController.setValidationSummary(summary);
    },

    clearValidationSummary: function () {
        this.validationController.clear();
    },

    registerValidator: function (validator) {
        this.validationController.register(validator);
    },

    validate: function (clear) {
        return this.validationController.validate(clear);
    },

    clear: function () {
        this.setFirstName("");
        this.setLastName("");
        this.setAccountName("");
        this.setCountry("");
        this.setAddress("");
        this.setResidenceType("");
        this.setWithStateCity("");
        this.setWithStatePostalCode("");
        this.setWithoutStateCity("");
        this.setWithoutStatePostalCode("");
        this.setCounty("");
        this.setPhone("");
    },

    setPostalCodeOptionalCountries: function (countries) {
        this.postalCodeOptionalCountries = countries;
    },

    getIsPostalCodeOptional: function () {
        var country = this.getCountry();
        if (country && this.postalCodeOptionalCountries.length > 0) {
            for (var ctr = 0; ctr < this.postalCodeOptionalCountries.length; ctr++) {
                var postalCodeOptionalCountry = this.postalCodeOptionalCountries[ctr];
                if (postalCodeOptionalCountry == country) {
                    return true;
                }
            }
        }

        return false;
    }
}

ise.Controls.CountryRepository = {
    
    initialize : function() {
        this.countries = new Array();
        this.countriesArray = null;
    },
    
    setCountries : function(countriesDTO) {
        
        var countries = this.countries;
        
        this.countriesArray = countriesDTO;
        
        for(var ctr=0; ctr<countriesDTO.length; ctr++) {
            var country = countriesDTO[ctr];
            countries[country.code] = country;
        }
    },
    
    findCountry : function(code) {
        return this.countries[code];
    }
    
}
ise.Controls.CountryRepository.initialize();

ise.Controls.AddressController = {
    
    initialize : function() {
        this.addresses = new Array();
        this.observers = new Array();
    },
    
    registerControl : function(addressId) {
        var addr = new ise.Controls.AddressControl(addressId);
        
        this.addresses[addressId] = addr;
        
        this.notifyObservers(addr);
        
        return addr;
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
        var ctrl = this.addresses[id];
        return ctrl;
    }
        
}
ise.Controls.AddressController.initialize();

ise.Controls.AddressSelectorControl = function(id, addresses) {
    this.id = id;
    this.control = $getElement(id);
    this.addresses = null; 
    
    if(this.control) {
        var del = Function.createDelegate(this, this.onSelectedAddressChanged);
        $addHandler(this.control, 'change', del);
    }
    
    this.selectedAddresChangedEventHandler = null;
}
ise.Controls.AddressSelectorControl.registerClass('ise.Controls.AddressSelectorControl');
ise.Controls.AddressSelectorControl.prototype = {

    setAddresses: function(addresses) {
        this.addresses = addresses;
    },

    getSelectedAddress: function() {
        var idx = this.control.selectedIndex;
        if (this.addresses && this.addresses.length >= idx) {
            return this.addresses[idx];
        }

        return null;
    },

    setSelectedIndex: function(index, fireChangedEvent) {
        if (index > -1 && index <= this.control.options.length - 1) {
            this.control.selectedIndex = index;
            if (fireChangedEvent) {
                this.onSelectedAddressChanged();
            }
        }
    },

    onSelectedAddressChanged: function() {

        if (this.selectedAddresChangedEventHandler) {
            this.selectedAddresChangedEventHandler();
        }
    },

    setSelectedAddressChangedEventHandler: function(handler) {
        this.selectedAddresChangedEventHandler = handler;
    },

    addAddress: function(newAddress, setSelected) {
        this.control.options[this.control.options.length] = new Option(newAddress.full, newAddress.id);

        if (setSelected) {
            this.control.selectedIndex = (this.control.options.length - 1);
        }
    }

}

ise.Controls.AddressSelectorController = {

    initialize : function() {
        this.selectors = new Array();
        this.observers = new Array();
    },
    
    registerControl : function(id) {
        var control = new ise.Controls.AddressSelectorControl(id);
        
        this.selectors.push(control);
        
        this.notifyObservers(control);
        
        return control;
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
        for(var ctr=0; ctr<this.selectors.length; ctr++) {
            var selector = this.selectors[ctr];
            if(selector.id == id) {
                return selector;
            }
        }
        
        return null;
    },
    
    getControls : function() {
        return this.selectors;
    }
    
}
ise.Controls.AddressSelectorController.initialize();

ise.Constants.ADDRESS_STATE_HIDDEN = 0;
ise.Constants.ADDRESS_STATE_VISIBLE = 1;
ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_MOUSEOVER = 0;
ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_MOUSEOUT = 1;
ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_POPUP = 2;
ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_POPUPCLOSED = 3;

ise.Controls.AddNewAddressControl = function(id) {
    this.id = id;
    this.control = $getElement(id);
    this.pnlAddNew = $getElement(this.id + '_pnlAddNew');
    this.addNewLink = $getElement(this.id + '_AddNew');
    this.saveLink = $getElement(this.id + '_Save');
    this.cancelLink = $getElement(this.id + '_Cancel');
    this.addressContainer = $getElement(this.id + '_Content');
    
    this.pnlCommand = $getElement(this.id + '_pnlCommand');
    
    var delSaveAddress = Function.createDelegate(this, this.saveAddress);        
    this.saveLink.onclick = delSaveAddress;
    
    var delToggleVisibility = Function.createDelegate(this, this.toggleVisibility);
    this.pnlAddNew.onclick = delToggleVisibility;
    
    var delMouseOver = Function.createDelegate(this, this.addNewLinkMouseOverEventHandler);        
    $addHandler(this.pnlAddNew, 'mouseover', delMouseOver);
    
    var delMouseOut = Function.createDelegate(this, this.addNewLinkMouseOutEventHandler);
    $addHandler(this.pnlAddNew, 'mouseout', delMouseOut);
    
    var delToggleVisibility = Function.createDelegate(this, this.toggleVisibility);
    this.cancelLink.onclick = delToggleVisibility;
    
    this.state = ise.Constants.ADDRESS_STATE_HIDDEN;
    
    this.addressControlId = '';
    this.addressControl = null;
    
    this.addressAddedEventHandlers = new Array();
}
ise.Controls.AddNewAddressControl.registerClass('ise.Controls.AddNewAddressControl');
ise.Controls.AddNewAddressControl.prototype = {
    
    addNewLinkMouseOverEventHandler : function(withPopUpShown) {
        if(this.state != ise.Constants.ADDRESS_STATE_VISIBLE) {
            this.setAddNewLinkPanelAppearance(ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_MOUSEOVER);
        }
    },
    
    addNewLinkMouseOutEventHandler : function() {
        if(this.state != ise.Constants.ADDRESS_STATE_VISIBLE) {
            this.setAddNewLinkPanelAppearance(ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_MOUSEOUT);
        }
    },
    
    setAddNewLinkPanelAppearance : function(action) {
        switch(action) {
            case ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_MOUSEOVER:
                this.pnlAddNew.className = "AddNewAddressLinkHover";
                break;
            case ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_MOUSEOUT:
                this.pnlAddNew.className = "AddNewAddressLink";
                break;
            case ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_POPUP:
                this.pnlAddNew.className = "AddNewAddressLinkWithPopUp";
                break;
            case ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_POPUPCLOSED:
                this.pnlAddNew.className = "AddNewAddressLink";
                break;
        }
    },

    addAddressAddedEventHandler : function(handler) {
        this.addressAddedEventHandlers.push(handler);
    },
    
    onAddressAdded : function(newAddress) {
        for(var ctr=0; ctr<this.addressAddedEventHandlers.length;ctr++) {
            var handler = this.addressAddedEventHandlers[ctr];
            handler(newAddress);
        }
        
        this.enableCommands();
        
        if(this.addressControl) {
            this.addressControl.clear();
            this.toggleVisibility();
        }
    },
    
    enableCommands : function() {
        //this.pnlCommand.className = "AddNewAddressCommand";
        var del = Function.createDelegate(this, this.saveAddress);
        this.saveLink.onclick = del; 
    },
    
    disableCommands : function() {
        //this.pnlCommand.className = "AddNewAddressCommandDisabled";
        this.saveLink.onclick = null;
    },

    setAddressControlId : function(id) {
        this.addressControlId = id;
        
        var control = ise.Controls.AddressController.getControl(id);
        if(control) {
            this.setAddressControl(control);
        }
        else {
            ise.Controls.AddressController.addObserver(this);
        }
    },
    
    notify : function(control) {
        if(control.id == this.addressControlId) {
            this.setAddressControl(control);
        }
    },
    
    setAddressControl : function(control) {
        this.addressControl = control;
    },
    
    saveAddress : function() {
        if(this.addressControl) {
            if(this.addressControl.validate(true)) {            
                var value = this.addressControl.serialize();                                
                var del = Function.createDelegate(this, this.onAddressAdded);
                var onAddressAddedDelegate = del; 
                
                this.disableCommands();
                
                var service = new ActionService();
                service.AddNewAddress(value, del);
            }
        }
    },
    
    toggleVisibility : function(forcedState) {
    
        var state = this.state;
        if(forcedState) {
            state = this.state;
        }
        switch(state) {
            case ise.Constants.ADDRESS_STATE_HIDDEN:
                this.showAddress(); 
                this.state = ise.Constants.ADDRESS_STATE_VISIBLE;
                this.setAddNewLinkPanelAppearance(ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_POPUP);
                break;                
                
            case ise.Constants.ADDRESS_STATE_VISIBLE:
                this.hideAddress(); 
                this.state = ise.Constants.ADDRESS_STATE_HIDDEN;
                this.setAddNewLinkPanelAppearance(ise.Constants.ADDRESS_ADD_NEW_LINK_ACTION_POPUPCLOSED);
                break;
                
        }
    },
    
    handleOnBodyClick : function() {
        if(this.isVisible()) {
            this.hideAddress();
        }
    },
    
    isVisible : function() {
        return this.state == ise.Constants.ADDRESS_STATE_VISIBLE;
    },
    
    showAddress : function() {
        this.addressContainer.style.display = '';
        this.addressContainer.style.zIndex = 999999;
    },
    
    hideAddress : function() {
        this.addressContainer.style.display = 'none';
    }
}


ise.Controls.AddNewAddressController = {

    initialize : function() {
        this.controls = new Array();
        this.observers = new Array();
    },
    
    registerControl : function(id) {
        var control = new ise.Controls.AddNewAddressControl(id);
        
        this.controls[id] = control;
        
        this.notifyObservers(control);
        
        return control;
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
ise.Controls.AddNewAddressController.initialize();

ise.Validators.DisallowShippingToPOBoxesValidator = function(controlId, errorMessage, next) {
    ise.Validators.DisallowShippingToPOBoxesValidator.initializeBase(this);
    this.control = $getElement(controlId);
    this.errorMessage = errorMessage;
    this.next = next;
    
    this.isValid = true;
}
ise.Validators.DisallowShippingToPOBoxesValidator.registerClass('ise.Validators.DisallowShippingToPOBoxesValidator');
ise.Validators.DisallowShippingToPOBoxesValidator.prototype = {

    shouldEvaluate: function() {
        return true;
    },

    validate: function() {
        this.isValid = true;

        if (this.control) {
            var value = this.control.value.toLowerCase().replace(/\./g, "").replace(/ /g, "");

            //Contains
            if ((value.indexOf('postoffice') != -1) || (value.indexOf('pobox') != -1) || (value.indexOf('postbox') != -1)) {
                this.isValid = false;   
                return;
            }
            //Starts With
            if ((value.substr(0, value.length) == 'po') || (value.substr(0, value.length) == 'box')) {
                this.isValid = false;
                return;
            }

        }
    },

    toString: function() {
        return 'POBox Address Validator';
    }

}
