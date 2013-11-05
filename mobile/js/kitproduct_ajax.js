ise.Products.KitProductItem = function(id, itemCode, itemType, name, selected) {
    ise.Products.KitProductItem.initializeBase(this, [id, itemCode, itemType]);
        
    this.name = name;
    
    this.deltaPrice = 0;
    this.selected = selected;
    
    this.selectedChangedEventHandlers = new Array();
    this.deltaPriceChangedEventHandlers = new Array();
}
ise.Products.KitProductItem.registerClass('ise.Products.KitProductItem', ise.Products.Product);
ise.Products.KitProductItem.prototype = {

    getName : function() {
        return this.name;
    },
    
    compareWith : function(other) {
        var delta = 0;
        if(other == this) {
            delta = 0;
        }
        else {
            delta = this.getPrice() - other.getPrice();
        }
        
        this.setDeltaPrice(delta);
    },
    
    computeDelta : function() {
        var delta = this.getPrice();
        this.setDeltaPrice(delta);
    },
    
    setDeltaPrice : function(delta) {
        this.deltaPrice = delta;
        this.onDeltaPriceChanged();
    },
    
    getDeltaPrice : function() {
        return this.deltaPrice;
    },
    
    getIsSelected : function() {
        return this.selected;
        this.onSelectedChanged();
    },
    
    setIsSelected : function(selected, invoker) {
        this.selected = selected;
        
        this.onSelectedChanged(invoker);
        
        return this.selected;
    },
    
    addSelectedChangedEventHandler : function(handler) {
        this.selectedChangedEventHandlers.push(handler);
    },
    
    addDeltaPriceChangedEventHandler : function(handler) {
        this.deltaPriceChangedEventHandlers.push(handler);
    },
    
    onSelectedChanged : function(invoker) {
        for(var ctr=0; ctr<this.selectedChangedEventHandlers.length; ctr++) {
            var handler = this.selectedChangedEventHandlers[ctr];
            handler(this, invoker);
        }
    },
    
    onDeltaPriceChanged : function() {
        for(var ctr=0; ctr<this.deltaPriceChangedEventHandlers.length; ctr++) {
            var handler = this.deltaPriceChangedEventHandlers[ctr];
            handler(this);
        }
    }

}

ise.Products.KitProductItemGroup = function(id, groupCode) {
    this.id = id;
    this.code = groupCode;
    this.items = new Array();
    
    this.selectionChangedEventHandlers = new Array()
    
    // virtual methods
    this.afterKitItemRegisteredDelegate = null;
    this.chooseItemsDelegate = null;
    this.inspectDelegate = this.baseInspect;
}
ise.Products.KitProductItemGroup.registerClass('ise.Products.KitProductItemGroup');
ise.Products.KitProductItemGroup.prototype = {

    getId : function() {
        return this.id;
    },
    
    getGroupCode : function() {
        return this.code;
    },
    
    getSelectedItems : function() {
        var selectedItems = new Array();
        
        for(var ctr=0; ctr<this.items.length; ctr++) {
            var current = this.items[ctr];
            if(current.getIsSelected()) {
                selectedItems.push(current);
            }
        }
        
        return selectedItems;
    },
    
    registerKitItem : function(kitItem) {
        if(kitItem) {
            this.items.push(kitItem);
            var handler = Function.createDelegate(this, this.kitItemSelectedChangedEventHandler);
            kitItem.addSelectedChangedEventHandler(handler);
            
            if(this.afterKitItemRegisteredDelegate) {
                this.afterKitItemRegisteredDelegate(kitItem);
            }
        }
    },
    
    inspect : function() {
        if(this.inspectDelegate) {
            this.inspectDelegate();
        }
    },
    
    baseInspect : function() {
        // meant to be overridden
    },
    
    kitItemSelectedChangedEventHandler : function(item, invoker) {
        if(invoker && this == invoker) {
            return;
        }
        
        if(this.chooseItemsDelegate) {
            this.chooseItemsDelegate(item);
        }
        
        this.onSelectionChanged();
    },
    
    addSelectionChangedEventHandler : function(handler) {
        this.selectionChangedEventHandlers.push(handler);
    },
    
    onSelectionChanged : function() {
        for(var ctr=0; ctr<this.selectionChangedEventHandlers.length; ctr++) {
            var handler = this.selectionChangedEventHandlers[ctr];
            handler(this);
        }
    }
    
}

ise.Products.KitProductItemRequiredGroup = function(id, groupCode) {
    ise.Products.KitProductItemRequiredGroup.initializeBase(this, [id, groupCode]);
    
    var handler = Function.createDelegate(this, this.chooseItems);
    this.chooseItemsDelegate = handler;
    this.inspectDelegate = handler; 
}
ise.Products.KitProductItemRequiredGroup.registerClass('ise.Products.KitProductItemRequiredGroup', ise.Products.KitProductItemGroup);
ise.Products.KitProductItemRequiredGroup.prototype = {

    chooseItems : function(selectedItem) {
        if(arguments.length == 0) {
            selectedItem = this.getSelectedItems()[0];
        }
        
        for(var ctr=0; ctr<this.items.length; ctr++) {
            var current = this.items[ctr];
            if(current != selectedItem) {
                current.setIsSelected(false, this);
            }
            
            current.compareWith(selectedItem);
        }
    }
}

ise.Products.KitProductItemMultiSelectGroup = function(id, groupCode) {
    ise.Products.KitProductItemMultiSelectGroup.initializeBase(this, [id, groupCode]);    
    
    this.chooseItemsDelegate = Function.createDelegate(this, this.chooseItems); 
    this.inspectDelegate = Function.createDelegate(this, this.displayDefault); 
}
ise.Products.KitProductItemMultiSelectGroup.registerClass('ise.Products.KitProductItemMultiSelectGroup', ise.Products.KitProductItemGroup);
ise.Products.KitProductItemMultiSelectGroup.prototype = {

    displayDefault : function() {
        for(var ctr=0; ctr<this.items.length; ctr++) {
            var current = this.items[ctr];
            current.computeDelta();
        }
    },
    
    chooseItems : function(selectedItem) {
        if(arguments.length == 0) {
            selectedItem = this.getSelectedItems()[0];
        }
        
        if(!selectedItem.getIsSelected()) {
            var hasOtherSelected = false;
            
            for(var ctr=0; ctr<this.items.length; ctr++) {
                var current = this.items[ctr];
                if(current != selectedItem) {
                    if(current.getIsSelected()) {
                        hasOtherSelected = true;
                        break;
                    }
                }
            }
            
            if(!hasOtherSelected) {
                selectedItem.setIsSelected(true, this);
            }
        }
        
        selectedItem.computeDelta();
    }
        
}

ise.Products.KitProductItemOptionalGroup = function(id, groupCode) {
    ise.Products.KitProductItemOptionalGroup.initializeBase(this, [id, groupCode]);
    
    this.chooseItemsDelegate = Function.createDelegate(this, this.chooseItems);
    this.inspectDelegate = Function.createDelegate(this, this.chooseItems); 
}
ise.Products.KitProductItemOptionalGroup.registerClass('ise.Products.KitProductItemOptionalGroup', ise.Products.KitProductItemGroup);
ise.Products.KitProductItemOptionalGroup.prototype = {

    unSelectAll : function() {
        for(var ctr=0; ctr<this.items.length; ctr++) {
            var current = this.items[ctr];
            current.setIsSelected(false, this);
        }
        
        this.chooseItems(null);
        
        this.onSelectionChanged();
    },
    
    chooseItems : function(selectedItem) {
        if(arguments.length == 0) {
            var selectedItems = this.getSelectedItems();
            if(selectedItems.length > 0) {
                selectedItem = selectedItems[0];
            }
            else {
                selectedItem = null;
            }
        }
        
        for(var ctr=0; ctr<this.items.length; ctr++) {
            var current = this.items[ctr];
            if(selectedItem) {
                if(current != selectedItem) {
                    current.setIsSelected(false, this);
                }
                
                current.compareWith(selectedItem);
            }
            else {
                current.computeDelta();
            }
        }
    }
    
}

ise.Products.KitProduct = function(id, itemCode, itemType) {
    ise.Products.KitProduct.initializeBase(this, [id, itemCode, itemType]);
    
    this.items = new Array();
    this.compositionChangedEventHandlers = new Array();
    this.groups = new Array();
    
    var handler = Function.createDelegate(this, this.onCompositionChanged);
    this.addUnitMeasureChangedEventHandler(handler); 
}
ise.Products.KitProduct.registerClass('ise.Products.KitProduct', ise.Products.Product);
ise.Products.KitProduct.prototype = {

    getGroup : function(id) {
        for(var ctr=0; ctr<this.groups.length; ctr++) {
            var group = this.groups[ctr];
            if(group.getId() == id) {
                return group;
            }
        }
        
        return null;
    },
    
    getGroups : function() {
        return this.groups;
    },
    
    getKitPrice : function() {
        var kitPrice = 0;
        
        for(var ctr=0; ctr<this.groups.length; ctr++) {
            var group = this.groups[ctr];
            var items = group.getSelectedItems();
            for(var ictr=0; ictr<items.length; ictr++) {
                var item = items[ictr];
                if(item) {
                    kitPrice += item.getPrice();
                }
            }
        }
        kitPrice = kitPrice - (this.getKitDiscount() * kitPrice);
        return kitPrice * this.getUnitMeasureQuantity();
    },
    
    getComposition : function() {
        var compositions = new Array();
        for(var gctr=0; gctr<this.groups.length; gctr++) {
            var group = this.groups[gctr];
            var items = group.getSelectedItems();
            for(var ictr=0; ictr<items.length; ictr++) {
                var item = items[ictr];
                var itemId = Math.abs((this.id - item.getId()));
                compositions.push(group.getId()+'+'+itemId);
            }
        }
        
        return compositions;
    },
    
    persistComposition : function() {
        var compositions = this.getComposition();        
        
        var items = getElementsByClassName('input', 'KitItems');
        for(var ctr=0; ctr<items.length; ctr++) {
            var elem = items[ctr];
            elem.value = compositions;
        }
    },
    
    registerGroup : function(group) {
        if(group) {
            this.groups.push(group);
            var handler = Function.createDelegate(this, this.groupSelectionChangedEventHandler);
            group.addSelectionChangedEventHandler(handler); 
        }
    },
    
    groupSelectionChangedEventHandler : function() {
        this.onCompositionChanged();
    },
    
    addCompositionChangedEventHandler : function(handler) {
        this.compositionChangedEventHandlers.push(handler);
    },
    
    onCompositionChanged : function() {
        for(var ctr=0; ctr<this.compositionChangedEventHandlers.length; ctr++) {
            var handler = this.compositionChangedEventHandlers[ctr];
            handler(ctr);
        }
    }
    
}

ise.Products.PriceDeltaControl = function(id, clientId) {
    this.id = id;
    this.ctrl = $getElement(clientId);
},
ise.Products.PriceDeltaControl.registerClass('ise.Products.PriceDeltaControl');
ise.Products.PriceDeltaControl.prototype = {

    setProduct : function(product) {
        if(product) {
            this.product = product;
            var handler = Function.createDelegate(this, this.controlDeltaPriceChangedEventHandler);
            this.product.addDeltaPriceChangedEventHandler(handler);
            this.buildDisplay();
        }
    },
    
    buildDisplay : function() {
        if(this.product.getIsSelected()) {
            this.ctrl.innerHTML = ise.StringResource.getString('showproduct.aspx.100');
        }
        else {
            var delta = this.product.getDeltaPrice();
            var display = '';
            
            if(delta>=0) {
                display = ise.StringResource.getString('showproduct.aspx.10') + ' ' + delta.localeFormat('c'); 
            }
            else {
                display = ise.StringResource.getString('showproduct.aspx.11') + ' ' + Math.abs(delta).localeFormat('c'); 
            }
            
            if(this.product.getHasVat()) {
                display += ' <span class="VATLabel">' + ise.StringResource.getString('showproduct.aspx.37') + '</span>';
            }
            this.ctrl.innerHTML = display;
        }
    },
    
    controlDeltaPriceChangedEventHandler : function(product) {
        this.buildDisplay();
    }
    
}

ise.Products.KitItemRadioControl = function(id, clientId) {
    this.id = id;
    
    this.ctrl = $getElement(clientId);
    this.ctrl.onclick = Function.createDelegate(this, this.onControlSelected); 
    
    this.product = null;
}
ise.Products.KitItemRadioControl.registerClass('ise.Products.KitItemRadioControl');
ise.Products.KitItemRadioControl.prototype = {

    getId : function() {
        return this.id;
    },
    
    setProduct : function(product) {
        if(product) {
            this.product = product;
            if(this.product.addSelectedChangedEventHandler) {
                var handler = Function.createDelegate(this, this.onProductSelectedChanged);
                this.product.addSelectedChangedEventHandler(handler); 
            }
        }
    },

    getProduct : function() {
        return this.product;
    },
    
    onProductSelectedChanged : function() {
        this.ctrl.checked = this.product.getIsSelected();
    },
    
    onControlSelected : function() {
        if(this.product) {
            this.product.setIsSelected(this.ctrl.checked);
        }
    }
    
}

ise.Products.KitItemCheckBoxControl = function(id, clientId) {
    this.id = id;
    
    this.ctrl = $getElement(clientId);
    this.ctrl.onclick = Function.createDelegate(this, this.onControlSelected); 
    
    this.product = null;
}
ise.Products.KitItemCheckBoxControl.registerClass('ise.Products.KitItemCheckBoxControl');
ise.Products.KitItemCheckBoxControl.prototype = {

    getId : function() {
        return this.id;
    },
    
    setProduct : function(product) {
        if(product) {
            this.product = product;
            var handler = Function.createDelegate(this, this.onProductSelectedChanged);
            this.product.addSelectedChangedEventHandler(handler); 
        }
    },

    getProduct : function() {
        return this.product;
    },
    
    onProductSelectedChanged : function() {
        this.ctrl.checked = this.product.getIsSelected();
    },
    
    onControlSelected : function() {
        if(this.product) {
            this.product.setIsSelected(this.ctrl.checked);
        }
    }

}

ise.Products.KitDropDownOptionControl = function(id) {
    this.id = id;
    
    this.product = null;
    this.shouldDisplayPriceDelta = true;
}
ise.Products.KitDropDownOptionControl.registerClass('ise.Products.KitDropDownOptionControl');
ise.Products.KitDropDownOptionControl.prototype = {

    getId : function() {
        return this.id;
    },
    
    setProduct : function(product) {
        if(product) {
            this.product = product;
        }
    },
    
    compareWith : function(other) {
        this.product.compareWith(other.product);
    },
    
    getIsSelected : function() {
        return this.product && this.product.getIsSelected();
    },
    
    setIsSelected : function(included) {
        this.product.setIsSelected(included);
    },
    
    setShouldDisplayPriceDelta : function(shouldDisplay) {
        this.shouldDisplayPriceDelta = shouldDisplay;
    },
    
    getDisplay : function() {
        if(this.shouldDisplayPriceDelta && this.product) {
            if(this.product.getIsSelected()) {
                return this.product.getName();
            }
            else {
                var display = '';
                
                if(this.product.getDeltaPrice) {
                    var delta = this.product.getDeltaPrice();
                    
                    if(delta>=0) {
                        display = this.product.getName() +  ', ' + ise.StringResource.getString('showproduct.aspx.10') + ' ' + delta.localeFormat('c');
                    }
                    else {
                        display = this.product.getName() +  ', ' + ise.StringResource.getString('showproduct.aspx.11') + ' ' + Math.abs(delta).localeFormat('c');
                    }
                    
                    if(this.product.getHasVat()) {
                        display += ' ' + ise.StringResource.getString('showproduct.aspx.37');
                    }
                }
                else {
                    display = this.product.getName();
                }
                
                return display;
            }
        }
        else {
            return this.product.getName();
        }
    }
    
}

ise.Products.KitDropDownGroupControl = function(id, clientId) {
    this.id = id;
    this.ctrl = $getElement(clientId);
    this.ctrl.onchange = Function.createDelegate(this, this.onOptionChanged); 
    
    this.controls = new Array();
}
ise.Products.KitDropDownGroupControl.registerClass('ise.Products.KitDropDownGroupControl');
ise.Products.KitDropDownGroupControl.prototype = {

    getId : function() {
        return this.id;
    },
    
    registerControl : function(control) {
        if(control) {
            this.controls.push(control);
            if(control.getIsSelected()) {
                this.selectedControl = control;
            }
        }
    },
    
    onOptionChanged : function() {
        var index = this.ctrl.selectedIndex;
        if(index < this.controls.length) {
            this.selectedControl = this.controls[index];
            this.selectedControl.setIsSelected(true);
        }
        this.buildDisplay();
    },
    
    buildDisplay : function() {
        if(this.controls.length == 0) return;
        
        if(this.selectedControl) {
            this.ctrl.options.length = 0;
                        
            var index = 0;
            
            for(var ctr=0; ctr<this.controls.length; ctr++) {
                var currentControl = this.controls[ctr];
                if(currentControl == this.selectedControl) {
                    index = ctr;
                }
                this.ctrl.options[ctr] = new Option(currentControl.getDisplay(), currentControl.getId());
            }
            
            this.ctrl.selectedIndex = index;
        }
        else {
            this.selectedControl = this.controls[0];
        }
    }

}


ise.Products.KitProductNoneItem = function(selected) {
    this.selected = selected;
    this.group = null;
}
ise.Products.KitProductNoneItem.registerClass('ise.Products.KitProductNoneItem');
ise.Products.KitProductNoneItem.prototype = {

    setGroup : function(group) {
        this.group = group;
    },
    
    getName : function() {
        return "None";
    },
    
    setIsSelected : function(selected) {
        if(selected) {
            this.group.unSelectAll();
        }
        this.selected = selected;
    },
    
    getIsSelected : function() {
        return this.selected;
    }
    
}

ise.Products.KitPriceControl = function(id, clientId) {
    this.id = id;
    this.ctrl = $getElement(clientId);
    this.kitProduct = null;
    this.lblPrice = $getElement(clientId + '_Price');
}
ise.Products.KitPriceControl.registerClass('ise.Products.KitPriceControl');
ise.Products.KitPriceControl.prototype = {

    setKitProduct : function(kitProduct) {
        if(kitProduct) {
            this.kitProduct = kitProduct;
            var handler = Function.createDelegate(this, this.kitCompositionChangedEventHandler);
            this.kitProduct.addCompositionChangedEventHandler(handler); 
            this.buildDisplay();
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },
    
    notify : function(product) {
        if(product.getId() == this.id) {
            this.setKitProduct(product);
        }
    },
    
    kitCompositionChangedEventHandler : function() {
        this.buildDisplay();
    },
    
    buildDisplay : function() {
        this.lblPrice.innerHTML = '';
        
        if( this.kitProduct.getHasVat() &&
            this.kitProduct.getVatSetting() == ise.Constants.VAT_SETTING_INCLUSIVE ) {
            
            var lblPriceLabel = document.createElement('SPAN');
            lblPriceLabel.innerHTML = ise.StringResource.getString('showproduct.aspx.28');
            this.lblPrice.appendChild(lblPriceLabel);
            
            var lblPriceValue = document.createElement('SPAN');
            lblPriceValue.innerHTML = '<i>' + ise.StringResource.getString('showproduct.aspx.45') + '<i>';
            this.lblPrice.appendChild(lblPriceValue);
            
            var params = "ItemCode=" + this.kitProduct.getItemCode() + "&ItemType=" + this.kitProduct.getItemType() + "&UMCode=" + this.kitProduct.getUnitMeasure() + "&KitItems=" + this.kitProduct.getComposition();
            
            var delUpdatePrice = function(price){
                lblPriceValue.innerHTML = price.localeFormat('c') + ' ' + ise.StringResource.getString('showproduct.aspx.38');
            };
            var service = new ActionService();
            service.GetItemPrice(this.kitProduct.getItemCode(), 
                            this.kitProduct.getItemType(),
                            this.kitProduct.getUnitMeasure(),
                            this.kitProduct.getComposition().toString(),
                            delUpdatePrice);
        }
        else {
            var kitPrice = this.kitProduct.getKitPrice();
            
            var lblPriceValue = document.createElement('SPAN');
            lblPriceValue.innerHTML = ise.StringResource.getString('showproduct.aspx.28') + ' ' + kitPrice.localeFormat('c');
            this.lblPrice.appendChild(lblPriceValue);
            
            if(this.kitProduct.getHasVat()) {
                var lblVat = document.createElement('SPAN');
                lblVat.innerHTML = '&nbsp;' + ise.StringResource.getString('showproduct.aspx.37');
                lblVat.className = 'VATLabel';
                this.lblPrice.appendChild(lblVat);
            }
        }
    }
    
}

ise.Products.KitDetailsControl = function(id) {
    this.id = id;
    
//    this.lnkHeader = $getElement('lnkKitDetailHeader_' + id);
//    this.lnkHeader.onclick = Function.createDelegate(this, this.onHeaderLinkClicked);
    
    this.pnlDetails = $getElement('pnlKitDetails_' + id);
    
    this.kitProduct = null;
}
ise.Products.KitDetailsControl.registerClass('ise.Products.KitDetailsControl');
ise.Products.KitDetailsControl.prototype = {

    setKitProduct : function(kitProduct) {
        this.kitProduct = kitProduct;
        var handler = Function.createDelegate(this, this.kitCompositionChangedEventHandler);
        this.kitProduct.addCompositionChangedEventHandler(handler);
        this.buildDisplay();
    },
    
    onHeaderLinkClicked : function() {
        if(this.pnlDetails.style.display == "") {
            this.lnkHeader.innerHTML = ise.StringResource.getString('showproduct.aspx.26');
            this.pnlDetails.style.display = "none";
        }
        else {
            this.lnkHeader.innerHTML = ise.StringResource.getString('showproduct.aspx.27');
            this.pnlDetails.style.display = "";
        }
    },
    
    kitCompositionChangedEventHandler : function() {
        this.buildDisplay();
    },
    
    buildDisplay : function() {
        this.pnlDetails.innerHTML = '';
        
        var ul = document.createElement('ul');
        this.pnlDetails.appendChild(ul);
        
        var kitGroups = this.kitProduct.getGroups();
        for(var gctr=0; gctr<kitGroups.length; gctr++) {
            var currentGroup = kitGroups[gctr];                
            var items = currentGroup.getSelectedItems();
            for(var ictr=0; ictr<items.length; ictr++) {
                var item = items[ictr];
                var li = document.createElement('li');
                li.innerHTML = item.getName();
                
                ul.appendChild(li);
            }
        }
    }

}

ise.Products.KitSideBarControl = function() {
    this.placeHolder = $getElement('KitPlaceHolder');
    this.sideBar = $getElement('KitSideBar');    
    
    this.vOffset = 10;
    this.hOffset = 10;
}

function floatKitSideBar() {
    var placeHolder = $getElement('KitPlaceHolder');
    var sideBar = $getElement('KitSideBar');
    
    var vOffset = 10;
    var hOffset = 10;

    var py = 0;
    var px = 0;
    var mv = 0;
    
    var pageY = 0;
    pageY = (document.body.scrollTop > 0) ? document.body.scrollTop : pageY;
    pageY = (document.documentElement.scrollTop > 0) ? document.documentElement.scrollTop : pageY;     

    var phY = GetElementPosition(placeHolder).y;
    
    if(pageY < phY) {
        py = phY;
    }
    else {
        py = pageY + vOffset;
        mv = ( py + sideBar.offsetTop) / 2;            
    }
    
    mv = Math.abs(py - sideBar.offsetTop) / 2;
    mv = mv > 1 ? mv : 0;
    
    sideBar.style.top = (py + mv) + px;
    
    /*********************************/
    var mm = $getElement('KitPlaceHolder');
    if($getElement('KitPlaceHolder') == null) {
        return;
    }
    
    var my = 0;
    my = (document.body.scrollTop > 0) ? document.body.scrollTop : my;
    my = (document.documentElement.scrollTop > 0) ? document.documentElement.scrollTop : my;
    var mmy = GetElementPosition(mm).y;

    var sideBar = $getElement("KitSideBar");
    
    if (my < mmy) {
        my = mmy;
    }      
    else {
        my = my + 10;
        mv = (my + sideBar.offsetTop) / 2;
    }
    
    var mv = Math.abs(my - sideBar.offsetTop) / 2;
    mv = mv > 1 ? mv : 0;
    
    sideBar.style.top = (my + mv) + "px";

    var dw = 0;
    dw = (document.body.clientWidth > 0) ? document.body.clientWidth : dw;
    dw = (document.documentElement.clientWidth > 0) ? document.documentElement.clientWidth : dw;

    var hx = GetElementPosition(mm).x;
    var sw = GetElementPosition(sideBar).width;
    var bx =  (dw - (sw + 10));
    
    if(bx > hx) {
        sideBar.style.left = hx + "px";
    }
    else {
        sideBar.style.left = bx + "px";
    }        

    setTimeout("floatKitSideBar()", 50);
}

function GetElementPosition(element)
{
       var result = new Object();
       result.x = 0;
       result.y = 0;
       result.width = 0;
       result.height = 0;
       result.x = element.offsetLeft;
       result.y = element.offsetTop;
       var parent = element.offsetParent;
       while (parent) {
               result.x += parent.offsetLeft;
               result.y += parent.offsetTop;
               parent = parent.offsetParent;
       }
       result.width = element.offsetWidth;
       result.height = element.offsetHeight;
       return result;
}

