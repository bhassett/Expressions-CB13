/// <reference path="core.js" />

Type.registerNamespace('ise.Pages');

ise.Controls.CheckOutShippingMultiItemRowControl = function() {
    this.addNewAddressControl = null;
    this.addressSelectorControl = null;
}
ise.Controls.CheckOutShippingMultiItemRowControl.registerClass('ise.Controls.CheckOutShippingMultiItemRowControl');
ise.Controls.CheckOutShippingMultiItemRowControl.prototype = {

    setAddressSelectorcontrolId : function(id) {
        var control = ise.Controls.AddressSelectorController.getControl(id);
        if(control) {
            this.setAddressSelectorControl(control);
        }
        else {
            var setControlDelegate = Function.createDelegate(this, this.setAddressSelectorControl);
            
            var observer = { 
            
                notify : function(control) {
                    if(control.id == id) {
                        setControlDelegate(control);
                    }
                }
                
            }
            
            ise.Controls.AddressSelectorController.addObserver(observer);
        }
    },
    
    setAddressSelectorControl : function(selectorControl) {
        this.addressSelectorControl = selectorControl;
    },
    
    setAddNewAddressControlId : function(id) {
        var control = ise.Controls.AddNewAddressController.getControl(id);
        if(control) {
            this.setAddNewAddressControl(control);
        }
        else {
            var setControlDelegate = Function.createDelegate(this, this.setAddNewAddressControl); 
            
            var observer = { 
            
                notify : function(control) {
                    if(control.id == id) {
                        setControlDelegate(control);
                    }
                }
                
            }
            
            ise.Controls.AddNewAddressController.addObserver(observer);
        }        
        
    },
    
    setAddNewAddressControl : function(addNewAddressControl) {
        this.addNewAddressControl = addNewAddressControl;
        
        var handler = Function.createDelegate(this, this.handleAddressAdded);
        addNewAddressControl.addAddressAddedEventHandler(handler); 
    },
    
    handleAddressAdded : function(newAddress) {
        var selectorControls = ise.Controls.AddressSelectorController.getControls();
        
        for(var ctr=0; ctr<selectorControls.length; ctr++) {
            var control = selectorControls[ctr];
            
            var makeDefault = (this.addressSelectorControl && this.addressSelectorControl == control);
            
            control.addAddress(newAddress, makeDefault);
        }
    }

}

//checkoutshippingmult.aspx
function IniAddAddressUi(textPostalDynamicName, dynamicComboBoxId) {

//    $(ToJQueryID(lnkAddAddId)).click(function () {
//        jqueryHideShow(ToJQueryID(addressContainerId));
//    });

//    $(ToJQueryID(lnkCancelId)).click(function () {
//        jqueryHideShow(ToJQueryID(addressContainerId));    
//    });
    RegisterAutoCompletePostalCode(ToJQueryClass(textPostalDynamicName), dynamicComboBoxId, true);
}