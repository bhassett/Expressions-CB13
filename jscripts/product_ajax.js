// Ensure root namespaces are existing
Type.registerNamespace('ise.Products');

ise.Products.ProductController = {
    
    initialize : function() {
        this.products = new Array();
        this.observers = new Array();
    },
    
    registerProduct : function(product) {
        if(product) {
            this.products[product.getId()] = product;
            this.notifyObservers(product);
            
            return product;
        }
    },
    
    getProduct : function(id) {
        return this.products[id];
    },
    
    addObserver : function(observer) {
        if(observer) {
            this.observers[this.observers.length] = observer;
        }
    },
    
    notifyObservers : function(product) {
        for(var ctr=0; ctr< this.observers.length; ctr++) {
            this.observers[ctr].notify(product);
        }
    }
}

ise.Products.ProductController.initialize();

ise.Products.ImagePreLoader = function(id) {
    this.id = id;
    this.images = new Array();
    this.preLoadingCompleteEventHandlers = new Array();
}
ise.Products.ImagePreLoader.registerClass('ise.Products.ImagePreLoader');
ise.Products.ImagePreLoader.prototype = {

    add : function(img) {
        this.images.push(img);
    },
    
    addPreloadingCompleteEventHandler : function(handler) {
        this.preLoadingCompleteEventHandlers.push(handler);
    },
    
    process : function() {
        var lambda = Function.createDelegate(this, this.doProcess);
        this.doProcess(lambda);
    },
    
    doProcess : function(lambda) {
        if (this.images.length > 0) 
        {
            var recurse = function () { if (lambda) { lambda(lambda) }; }
            var current = this.images.pop();
            if (current && current.src) 
            {
            // check status 200 or 404
                var transport = jQuery.ajax({ type: "get", url: current.src})

                    .done(function (data, textStatus, xhr) {
                        current.exists = (200 == xhr.status);
                        recurse();
                    })

                    .fail(function (xhr, textStatus, errorThrown) {
                        current.exists = false;
                        recurse();
                    });

            }
            else 
            {
                recurse();
            }
        }
        else 
        {
            this.onPreloadingComplete();
        }
        
    },
    
    onPreloadingComplete : function() {
        for(var ctr=0;ctr<this.preLoadingCompleteEventHandlers.length; ctr++) {
            var handler = this.preLoadingCompleteEventHandlers[ctr];
            handler(this);
        }
    }

}


ise.Products.Product = function (id, itemCode, itemType) {
    this.id = id;
    this.itemCode = itemCode;
    this.itemType = itemType;

    this.currentUnitMeasure = null;
    this.price = null;
    this.imageData = null;

    this.priceChangedEventHandlers = new Array();
    this.attributeChangedEventHandlers = new Array();
    this.unitMeasures = new Array();
    this.unitMeasureChangedEventHandlers = new Array();

    this.restrictedQuantities = null;
    this.minQuantity = 0;
    this.hidePriceUntilCart = false;
    this.kitDiscount = 0;

    this.imagesLoadedEventHandlers = new Array();

    this.hasVat = false;
    this.vatSetting = ise.Constants.VAT_SETTING_EXCLUSIVE;

    this.showBuyButton = true;

    this.QuantityRegEx = null;
    this.QuantityDecimalSeparator = null;
    this.QuantityDecimalZero = null;

    this.imageZoomOption = null;
}
ise.Products.Product.registerClass('ise.Products.Product');
ise.Products.Product.prototype = {

    setUnitMeasureIntrinsics: function (unitMeasures) {
        this.unitMeasures = unitMeasures;

        if (this.unitMeasures.length > 0) {
            this.setUnitMeasure(this.unitMeasures[0].code);
            this.currentUnitMeasure.freestock = '';
        }

        this.onAttributeChanged();
    },

    getUnitMeasures: function () {
        return this.unitMeasures;
    },

    getId: function () {
        return this.id;
    },

    getItemCode: function () {
        return this.itemCode;
    },

    getItemType: function () {
        return this.itemType;
    },

    getUnitMeasure: function () {
        return this.currentUnitMeasure.code;
    },

    getUnitMeasureQuantity: function () {
        return this.currentUnitMeasure.unitMeasureQuantity;
    },

    getPrice: function () {
        return this.currentUnitMeasure.price;
    },

    getPriceFormatted: function () {
        return this.currentUnitMeasure.priceFormatted;
    },

    getPromotionalPrice: function () {
        return this.currentUnitMeasure.promotionalPrice;
    },

    getPromotionalPriceFormatted: function () {
        return this.currentUnitMeasure.promotionalPriceFormatted;
    },

    hasPromotionalPrice: function () {
        return this.currentUnitMeasure.hasPromotionalPrice;
    },

    getHasDiscount: function () {
        return this.currentUnitMeasure.hasDiscount;
    },

    getOriginalSalePrice: function () {
        return this.currentUnitMeasure.originalSalePrice;
    },

    getOriginalSalePriceFormatted: function () {
        return this.currentUnitMeasure.originalSalePriceFormatted;
    },

    getHasVat: function () {
        return this.hasVat;
    },

    setHasVat: function (hasVat) {
        this.hasVat = hasVat;
    },

    getVatSetting: function () {
        return this.vatSetting;
    },

    setVatSetting: function (vatSetting) {
        this.vatSetting = vatSetting;
    },

    getVat: function () {
        return this.currentUnitMeasure.tax;
    },

    getFreeStock: function () {
        var param = new Object();
        param.itemCode = this.getItemCode();
        param.unitMeasureCode = this.getUnitMeasure();
        var thisObject = this;

        if (thisObject.currentUnitMeasure.freestock == '') {
            $.ajax({
                type: "POST",
                url: "ActionService.asmx/GetInventoryFreeStock",
                data: JSON.stringify(param),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (result) {
                    thisObject.currentUnitMeasure.freestock = result.d;
                },
                fail: function (result) {
                    thisObject.currentUnitMeasure.freestock = 0;
                }
            });
        }

        return thisObject.currentUnitMeasure.freestock;
    },

    hasAvailableStock: function () {
        return this.getFreeStock() > 0;
    },

    hasRestrictedQuantities: function () {
        return null != this.restrictedQuantities && this.restrictedQuantities.length > 0;
    },

    getRestrictedQuantities: function () {
        return this.restrictedQuantities;
    },

    setRestrictedQuantities: function (quantities) {
        this.restrictedQuantities = quantities;
    },

    setMinimumOrderQuantity: function (qty) {
        this.minQuantity = qty;
    },

    getMinimumOrderQuantity: function () {
        return this.minQuantity;
    },

    setKitDiscount: function (kitdiscount) {
        this.kitdiscount = kitdiscount;
    },

    getKitDiscount: function () {
        return this.kitdiscount;
    },

    setHidePriceUntilCart: function (hide) {
        this.hidePriceUntilCart = hide;
    },

    getHidePriceUntilCart: function () {
        return this.hidePriceUntilCart;
    },

    setShowBuyButton: function (show) {
        this.showBuyButton = show;
    },

    getShowBuyButton: function () {
        return this.showBuyButton;
    },

    setImageZoomOption: function (option) {
        this.imageZoomOption = option;
    },

    getImageZoomOption: function () {
        return this.imageZoomOption;
    },

    setUnitMeasure: function (unitMeasure) {
        for (var ctr = 0; ctr < this.unitMeasures.length; ctr++) {
            var currentUnitMeasure = this.unitMeasures[ctr];
            if (currentUnitMeasure.code == unitMeasure) {
                this._setCurrentUnitMeasure(currentUnitMeasure);
            }
        }

        this.onUnitMeasureChanged();
    },

    addUnitMeasureChangedEventHandler: function (handler) {
        this.unitMeasureChangedEventHandlers[this.unitMeasureChangedEventHandlers.length] = handler;
    },

    onUnitMeasureChanged: function () {
        for (var ctr = 0; ctr < this.unitMeasureChangedEventHandlers.length; ctr++) {
            var handler = this.unitMeasureChangedEventHandlers[ctr];
            handler(this);
        }
    },

    _setCurrentUnitMeasure: function (unitMeasure) {
        this.currentUnitMeasure = unitMeasure;

        this.onPriceChanged();
    },
    addPriceChangedEventHandler: function (handler) {
        this.priceChangedEventHandlers[this.priceChangedEventHandlers.length] = handler;
    },

    addAttributeChangedEventHandler: function (handler) {
        this.attributeChangedEventHandlers[this.attributeChangedEventHandlers.length] = handler;
    },

    onPriceChanged: function () {
        for (var ctr = 0; ctr < this.priceChangedEventHandlers.length; ctr++) {
            var handler = this.priceChangedEventHandlers[ctr];
            handler(this);
        }
    },

    onAttributeChanged: function () {
        for (var ctr = 0; ctr < this.attributeChangedEventHandlers.length; ctr++) {
            var handler = this.attributeChangedEventHandlers[ctr];
            handler(this);
        }
    },

    getImageData: function () {
        return this.imageData;
    },


    areImagesLoaded: function () {
        return this.imagesAreLoaded;
    },


    addImagesLoadedEventHandler: function (handler) {
        this.imagesLoadedEventHandlers.push(handler);
    },

    onImagesLoaded: function () {
        for (var ctr = 0; ctr < this.imagesLoadedEventHandlers.length; ctr++) {
            var handler = this.imagesLoadedEventHandlers[ctr];
            handler(this);
        }
    },

    setImageData: function (data) {
        this.imageData = data;

        if (data.isRemote) {
            this.imagesAreLoaded = false;
            this.preLoadImages();

        }
        else {
            this.imagesAreLoaded = true;
        }
    },

    preLoadImages: function () {
        if (this.imageData) {
            var loader = new ise.Products.ImagePreLoader(this.id);
            loader.add(this.imageData.medium);
            loader.add(this.imageData.large);
            loader.add(this.imageData.swatch);
            for (var ctr = 0; ctr < 10; ctr++) {
                loader.add(this.imageData.mediumImages[ctr]);
                loader.add(this.imageData.largeImages[ctr]);
                loader.add(this.imageData.microImages[ctr]);
            }

            var handler = Function.createDelegate(this, this.onPreloadingImagesCompleteEventHandler);
            loader.addPreloadingCompleteEventHandler(handler);
            loader.process();
        }
    },

    onPreloadingImagesCompleteEventHandler: function () {
        this.imagesAreLoaded = true;
        this.onImagesLoaded();
    },

    getMediumImage: function (index) {
        if (arguments.length == 0) {
            return this.imageData.medium;
        }
        else {
            return this.imageData.mediumImages[index];
        }
    },

    getLargeImage: function (index) {
        if (arguments.length == 0) {
            return this.imageData.large;
        }
        else {
            return this.imageData.largeImages[index];
        }
    },

    getMicroImage: function (index) {
        return this.imageData.microImages[index];
    },

    serializeToForm: function () {

    },

    toString: function () {
        return 'ise.Products.Product';
    },

    setQuantityRegEx: function (regExComparer, qtyDecSep, qtyDecZero) {
        this.QuantityRegEx = regExComparer;
        this.QuantityDecimalSeparator = qtyDecSep;
        this.QuantityDecimalZero = qtyDecZero;
    },

    getQuantityRegEx: function () {
        return this.QuantityRegEx;
    },

    getQuantityDecimalSeparator: function () {
        return this.QuantityDecimalSeparator;
    },

    getQuantityDecimalZero: function () {
        return this.QuantityDecimalZero;
    },

    getNumberOfDigitsAfterDecimal: function () {
        return this.currentUnitMeasure.numberOfDigitsAfterDecimal;
    }

}



ise.Products.MatrixAttribute = function(code, value){
    this.code = code;
    this.value = value;    
}
ise.Products.MatrixAttribute.registerClass('ise.Products.MatrixAttribute');
ise.Products.MatrixAttribute.prototype = {
    
    matches : function(other) {
        return this.code == other.code && this.value == other.value;
    }
}

ise.Products.MatrixProduct = function(id, itemCode, itemType) {
    ise.Products.MatrixProduct.initializeBase(this, [id, itemCode, itemType]);
    
    this.attributes = new Array();
}
ise.Products.MatrixProduct.registerClass('ise.Products.MatrixProduct', ise.Products.Product);
ise.Products.MatrixProduct.prototype = {

    setAttributes : function(attributes) {
        this.extractAttributes(attributes);
    },
    
    extractAttributes : function(attributesAsArray) {
        for(var ctr=0; ctr<attributesAsArray.length; ctr++) {
            var definedAttribute = attributesAsArray[ctr];
            
            var attribute = new ise.Products.MatrixAttribute(definedAttribute.Code, definedAttribute.Value);
            this.attributes.push(attribute);
        }  
    },
    
    getAttributes : function() {
        return this.attributes;
    },
    
    hasMatch : function(otherAttribute) {
        var ownAttributes = this.attributes; 
        for(var ctr=0; ctr<ownAttributes.length; ctr++) {
            var ownAttribute = ownAttributes[ctr];
            if(ownAttribute.matches(otherAttribute)) {
                return true;
            }
        }
        return false;
    },
    
    matches : function(selectedAttributes) {
        var matchCount = 0;
        
        var ownAttributes = this.attributes;
        for(var ctr=0; ctr<selectedAttributes.length; ctr++) {
            var selectedAttribute = selectedAttributes[ctr];
            if(this.hasMatch(selectedAttribute)) {
                matchCount++;
            }
        }
        
        return selectedAttributes.length == ownAttributes.length && matchCount == ownAttributes.length;
    },
    
    toString : function() {
        return 'ise.Products.MatrixProduct';
    }
}

ise.Products.MatrixGroupProduct = function(id, itemCode, itemType) {
    ise.Products.MatrixGroupProduct.initializeBase(this, [id, itemCode, itemType]);
    
    this.matrixProducts = new Array();
    this.interChangeEventHandlers = new Array();
    
    this.attributes = null;
    this.selectedMatrixProduct = null;
    this.matrixProductsWithImagesPreloaded = 0;        
    this.onImagesLoaded = this.onMatrixImagesLoaded;
}
ise.Products.MatrixGroupProduct.registerClass('ise.Products.MatrixGroupProduct', ise.Products.Product);
ise.Products.MatrixGroupProduct.prototype = {

    onMatrixImagesLoaded : function() {
        if(this.matrixProductsWithImagesPreloaded == this.matrixProducts.length) {
            for(var ctr=0; ctr<this.imagesLoadedEventHandlers.length; ctr++) {
                var handler = this.imagesLoadedEventHandlers[ctr];
                handler(this);
            }
        }
    },
    
    registerMatrixProduct : function(product) {
        this.matrixProducts[this.matrixProducts.length] = product;
        var handler = Function.createDelegate(this, this.onMatrixItemImageLoaded);
        product.addImagesLoadedEventHandler(handler);
    },
    
    onMatrixItemImageLoaded : function() {
        this.matrixProductsWithImagesPreloaded++;
        this.onMatrixImagesLoaded();
    },
    
    chooseMatrixItem : function(itemCode) {
        var matchFound = false;
        
        for(var ctr=0; ctr<this.matrixProducts.length; ctr++) {
            var currentMatrixProduct = this.matrixProducts[ctr];
            if(currentMatrixProduct.getItemCode() == itemCode) {
                this.setSelectedMatrixProduct(currentMatrixProduct);
                matchFound = true;
            }
        }
        
        if(!matchFound) {
            this.selectedMatrixProduct = null;
        }
    },
    
    chooseAttributes : function(attributes) {
        var matchFound = false;
        
        for(var ctr=0; ctr<this.matrixProducts.length; ctr++) {
            var currentMatrixProduct = this.matrixProducts[ctr];
            if(currentMatrixProduct.matches(attributes)) {
                this.setSelectedMatrixProduct(currentMatrixProduct);
                matchFound = true;
            }
        }
        
        if(!matchFound) {
            this.selectedMatrixProduct = null;
        }
    },
    
    getSelectedMatrixProduct : function() {
        return this.selectedMatrixProduct;
    },
    
    hasSelectedMatrixProduct :function() {
        return null != this.selectedMatrixProduct;
    },
    
    setSelectedMatrixProduct : function(selectedMatrixProduct) {
        this.selectedMatrixProduct = selectedMatrixProduct;        
        this.ownAsSelf(selectedMatrixProduct);
    },
    
    ownAsSelf : function(other) {
        this.id = other.getId();
        this.itemCode = other.getItemCode();
        this.setRestrictedQuantities(other.getRestrictedQuantities());
        this.setHidePriceUntilCart(other.getHidePriceUntilCart());
        this.setShowBuyButton(other.getShowBuyButton());
        this.setImageZoomOption(other.getImageZoomOption());
        this.setMinimumOrderQuantity(other.getMinimumOrderQuantity());
        this.setUnitMeasureIntrinsics(other.getUnitMeasures());
        this.setImageData(other.getImageData());
        this.setAttributes(other.getAttributes());
        
        this.onInterChange();
    },
    
    getAttributes : function() {
        return this.attributes;
    },
    
    setAttributes : function(attributes) {
        this.attributes = attributes;
    },
    
    addInterChangeEventHandler : function(handler) {
        this.interChangeEventHandlers[this.interChangeEventHandlers.length] = handler;
    },
    
    onInterChange : function() {
        for(var ctr=0; ctr<this.interChangeEventHandlers.length; ctr++) {
            var handler = this.interChangeEventHandlers[ctr];
            handler(this);
        }
    },
    
    getSwatchImage : function(id) {
        for(var ctr=0; ctr<this.matrixProducts.length; ctr++) {
            var matrixProduct = this.matrixProducts[ctr];
            if(matrixProduct.getId() == id) {
                return matrixProduct.imageData.swatch;
            }
        }
    },
    
    toString : function() {
        return 'ise.Products.MatrixGroupProduct';
    }
    
}

ise.Products.PriceControl = function(id, clientId) {
    this.id = id;
    this.ctrl = $getElement(clientId);
}
ise.Products.PriceControl.registerClass('ise.Products.PriceControl');

ise.Products.PriceControl.prototype = {
    setProduct: function (product) {
        this.setProductWithLabel(product, 'true');
    },

    setProductWithLabel: function (product, showLabel) {
        if (product) {
            this.product = product;
            this.attachPriceChangedEventHandler();

            this.buildDisplayWithLabel(showLabel);
            //this.buildDisplay();
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },

    notify: function (product) {
        if (product.getId() == this.id) {
            this.setProduct(product);
        }
    },

    attachPriceChangedEventHandler: function () {
        if (this.product) {
            var handler = Function.createDelegate(this, this.onProductPriceChanged);
            this.product.addPriceChangedEventHandler(handler);
        }
    },

    clearDisplay: function () {
        this.ctrl.innerHTML = "";
    },

    buildDisplay: function () {
        this.buildDisplayWithLabel('true');
    },

    buildDisplayWithLabel: function (displayLabel) {
        if (this.product) {
            this.clearDisplay();

            if (!this.product.getHidePriceUntilCart()) {
                var pnlPrice = document.createElement('DIV');
                pnlPrice.id = 'pnlPrice_' + this.id;
                this.ctrl.appendChild(pnlPrice);

                var lblPriceCaption = document.createElement('SPAN');
                lblPriceCaption.id = 'lblPriceCaption_' + this.id;

                if (displayLabel == 'true') {
                    lblPriceCaption.innerHTML = ise.StringResource.getString('showproduct.aspx.33');
                }

                lblPriceCaption.innerHTML = lblPriceCaption.innerHTML + '&nbsp;';

                pnlPrice.appendChild(lblPriceCaption);

                var lblPrice = document.createElement('SPAN');
                lblPrice.id = 'lblPrice_' + this.id;

                if (this.product.getHasDiscount() && !this.product.hasPromotionalPrice()) {
                    lblPrice.innerHTML = this.product.getOriginalSalePriceFormatted() + '&nbsp';
                } 
                else {
                    lblPrice.innerHTML = this.product.getPriceFormatted() + '&nbsp';
                }
                
                pnlPrice.appendChild(lblPrice);
                // getting html components involved.
                var lblStockHint = $getElement('lblStockHint_' + this.id);
                var imgStockHint = $getElement('imgStockHint_' + this.id);

                // displays the Stock Hint if lblStockHint element is present.
                if (lblStockHint != null) {

                    // checked if FreeStock is greater than zero
                    if (this.product.getFreeStock() > 0) {
                        var nFreeStock = this.product.getFreeStock();
                        var nFreeTemp = Math.floor(nFreeStock);

                        nFreeTemp += '';
                        nFreeArray = nFreeTemp.split('.');
                        nFreeItem = nFreeArray[0];
                        nFreeActual = nFreeArray.length > 1 ? '.' + nFreeArray[1] : '';

                        var nRegExp = /(\d+)(\d{3})/;
                        while (nRegExp.test(nFreeItem)) {
                            nFreeItem = nFreeItem.replace(nRegExp, '$1' + ',' + '$2');
                        }
                        nFreeStock = nFreeItem + nFreeActual;
                        lblStockHint.innerHTML = nFreeStock + ' ' + ise.StringResource.getString('showproduct.aspx.47');
                    }
                    else {
                        if (imgStockHint != null) {
                            imgStockHint.style.visibility = 'visible';
                        }
                    }
                }
                var lblPrice_VAT = document.createElement('SPAN');
                lblPrice_VAT.id = 'lblPrice_VAT';
                if (this.product.getHasVat()) {
                    if (this.product.getVatSetting() == ise.Constants.VAT_SETTING_INCLUSIVE) {
                        lblPrice_VAT.innerHTML = ise.StringResource.getString('showproduct.aspx.38');
                    }
                    else {
                        lblPrice_VAT.innerHTML = ise.StringResource.getString('showproduct.aspx.37');
                    }
                }

                pnlPrice.appendChild(lblPrice_VAT);
                lblPrice_VAT.className = 'VATLabel';
                pnlPrice.className = "SalesPrice";

                if (this.product.hasPromotionalPrice()) {
                    var pnlPromotionalPrice = document.createElement('DIV');
                    pnlPromotionalPrice.id = 'pnlPromotionalPrice_' + this.id;
                    this.ctrl.appendChild(pnlPromotionalPrice);

                    var lblPromotionalPriceCaption = document.createElement('SPAN');
                    lblPromotionalPriceCaption.id = 'lblPromotionalPriceCaption_' + this.id
                    lblPromotionalPriceCaption.innerHTML = ise.StringResource.getString('showproduct.aspx.34') + '&nbsp;';
                    pnlPromotionalPrice.appendChild(lblPromotionalPriceCaption);

                    var lblPromotionalPrice = document.createElement('SPAN');
                    lblPromotionalPrice.id = 'lblPromotionalPrice_' + this.id
                    lblPromotionalPrice.innerHTML = this.product.getPromotionalPriceFormatted() + '&nbsp';
                    pnlPromotionalPrice.appendChild(lblPromotionalPrice);

                    var lblPromotionalPrice_VAT = document.createElement('SPAN');
                    lblPromotionalPrice_VAT.id = 'lblPromotionalPrice_VAT'
                    if (this.product.getHasVat()) {
                        if (this.product.getVatSetting() == ise.Constants.VAT_SETTING_INCLUSIVE) {
                            lblPromotionalPrice_VAT.innerHTML = ise.StringResource.getString('showproduct.aspx.38');
                        }
                        else {
                            lblPromotionalPrice_VAT.innerHTML = ise.StringResource.getString('showproduct.aspx.37');
                        }
                    }
                    pnlPromotionalPrice.appendChild(lblPromotionalPrice_VAT);

                    pnlPromotionalPrice.className = 'PromotionalPrice';
                    pnlPrice.className = "SalesPriceOverridden";
                    lblPromotionalPrice_VAT.className = 'VATLabel';
                }

                if (this.product.getHasDiscount() && !this.product.hasPromotionalPrice()) {
                    var pnlDiscountedPrice = document.createElement('DIV');
                    pnlDiscountedPrice.id = 'pnlDiscountedPrice_' + this.id;
                    this.ctrl.appendChild(pnlDiscountedPrice);

                    var lblDsicountedPriceCaption = document.createElement('SPAN');
                    lblDsicountedPriceCaption.id = 'lblDsicountedPriceCaption_' + this.id
                    lblDsicountedPriceCaption.innerHTML = ise.StringResource.getString('showproduct.aspx.63') + '&nbsp;';
                    pnlDiscountedPrice.appendChild(lblDsicountedPriceCaption);

                    var lblDsicountePrice = document.createElement('SPAN');
                    lblDsicountePrice.id = 'lblDsicountePrice_' + this.id;
                    lblDsicountePrice.innerHTML = this.product.getPriceFormatted() + '&nbsp';
                    pnlDiscountedPrice.appendChild(lblDsicountePrice);

                    var lblDsicountedPrice_VAT = document.createElement('SPAN');
                    lblDsicountedPrice_VAT.id = 'lblDsicountedPrice_VAT'
                    if (this.product.getHasVat()) {
                        if (this.product.getVatSetting() == ise.Constants.VAT_SETTING_INCLUSIVE) {
                            lblDsicountedPrice_VAT.innerHTML = ise.StringResource.getString('showproduct.aspx.38');
                        }
                        else {
                            lblDsicountedPrice_VAT.innerHTML = ise.StringResource.getString('showproduct.aspx.37');
                        }
                    }
                    pnlDiscountedPrice.appendChild(lblDsicountedPrice_VAT);

                    pnlDiscountedPrice.className = 'DiscountedPrice';
                    pnlPrice.className = "SalesPriceOverridden";
                    lblDsicountedPrice_VAT.className = 'VATLabel';
                }

            }
        }
    },

    onProductPriceChanged: function (product) {
        this.buildDisplay();
    }

}

ise.Products.UnitMeasureControl = function(id, clientId) {
    this.id = id;
    this.ctrl = $getElement(clientId);
    this.product = null;
}
ise.Products.UnitMeasureControl.registerClass('ise.Products.UnitMeasureControl');
ise.Products.UnitMeasureControl.prototype = {

    setProduct : function(product) {
        if(product) {
            this.product = product;
            this.buildDisplay();
            
            if(this.product.getItemType() == 'Matrix Group') {
                var handler = Function.createDelegate(this, this.onProductInterChanged);
                this.product.addInterChangeEventHandler(handler);
            }
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },
    
    notify : function(product) {
        if(product.getId() == this.id) {
            this.setProduct(product);
        }
    },
    
    onUnitMeasureChanged : function(sender) {
        if(this.product) {
            var selectedUnitMeasure = sender.options[sender.selectedIndex].value;
            
            this.product.setUnitMeasure(selectedUnitMeasure);
        }
    },
    
    onProductInterChanged : function() {
        this.buildDisplay();
    },
    
    buildDisplay : function() {
        this.clearDisplay();
        
        var umCode = this.product.getUnitMeasure();
        var idx = 0;
        
        var unitMeasures = this.product.getUnitMeasures();
        if(unitMeasures.length > 1) {
            var span = document.createElement('SPAN');
            span.id = 'lblUnitMeasure_' + this.id;
            span.innerHTML = ise.StringResource.getString('showproduct.aspx.32') + '&nbsp;';
            this.ctrl.appendChild(span);
            
            var select = document.createElement('SELECT');
            select.id = 'UnitMeasureCode_' + this.id;
            select.name = 'UnitMeasureCode';
            
            for(var ctr=0; ctr<unitMeasures.length; ctr++) {
                var current = unitMeasures[ctr];
                select.options.add(new Option(current.description, current.code));
                
                if(umCode == current.code) {
                    idx = ctr;
                }
            }
            
            this.ctrl.appendChild(select);
            select.selectedIndex = idx;
            var handler = Function.createDelegate(this, this.onUnitMeasureChanged);
            // we need to call our handler that accepts the drop down list as parameter
            // we therefore use anonymous method instead
            var onChangeHandler = function() {
                // pass the combobox as parameter
                handler(select);
            }
            $addHandler(select, 'change', onChangeHandler);
        }
        else {
            var input = document.createElement('INPUT');
            input.type = 'hidden';
            input.id = 'UnitMeasureCode_' + this.id;
            input.name = 'UnitMeasureCode';            
            this.ctrl.appendChild(input);
            
            var span = document.createElement('SPAN');
            span.id = 'lblUnitMeasure_' + this.id;
            span.innerHTML = ise.StringResource.getString('showproduct.aspx.32') + '&nbsp;' + unitMeasures[0].description;
            this.ctrl.appendChild(span);
        }
    },
    
    clearDisplay : function() {
        this.ctrl.innerHTML = '';
    }
    
}


ise.Products.MatrixAttributeControl = function(id, attribute) {
    this.ctrl = $getElement(id);
    this.attribute = attribute;
    
    this.attributeChangedEventHandlers = new Array();
    
    if(this.ctrl) {
        this._attachAttributeChangedEventHandler();
    }
}
ise.Products.MatrixAttributeControl.registerClass('ise.Products.MatrixAttributeControl');
ise.Products.MatrixAttributeControl.prototype = {

    _attachAttributeChangedEventHandler : function() {
        var handler = Function.createDelegate(this, this.onAttributeChanged);
        $addHandler(this.ctrl, 'change', handler);
    },
    
    onAttributeChanged : function() {
        var value = this.ctrl.options[this.ctrl.selectedIndex].value;
        
        for(var ctr=0; ctr< this.attributeChangedEventHandlers.length; ctr++) {
            var handler = this.attributeChangedEventHandlers[ctr];
            handler(value);
        }
    },
    
    addAttributeChangedEventHandler : function(handler) {        
        this.attributeChangedEventHandlers[this.attributeChangedEventHandlers.length] = handler;
    },
    
    getAttribute : function() {
        return this.attribute;
    },
    
    getValue : function() {
        if(this.hasAttributeSelected()) {
            var value = this.ctrl.options[this.ctrl.selectedIndex].value;
            return value;
        }
        
        return ise.Constants.EMPTY_STRING;
    },
    
    setValue : function(value) {
        var idx = 0;
        
        for(var ctr=0;ctr<this.ctrl.options.length; ctr++) {
            var option = this.ctrl.options[ctr];
            if(option.value == value) {
                idx = ctr;
                break;
            }
        }
        
        this.ctrl.selectedIndex = idx;
    },
    
    hasAttributeSelected : function() {
        return this.ctrl.selectedIndex > 0;
    }

}

ise.Products.MatrixAttributeGroupControl = function(id) {
    this.id = id;
    
    this.attributeControls = new Array();
    this.product = null;
    
    this.pnlError = $getElement('pnlMatrixAttribute_Error_' + id);
    this.pnlSelectCaption = $getElement('pnlMatrixAttribute_SelectCaption_' + id);
    this.pnlStockHint = $getElement('pnlStockHint_' + id);

    this.attachFormHandler();
}

ise.Products.MatrixAttributeGroupControl.registerClass('ise.Products.MatrixAttributeGroupControl');
ise.Products.MatrixAttributeGroupControl.prototype = {

    setProduct: function (product) {
        if (product) {
            this.product = product;

            if (this.product.getItemType() == 'Matrix Group') {
                var handler = Function.createDelegate(this, this.onProductInterChanged);
                this.product.addInterChangeEventHandler(handler);
            }
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },

    notify: function (product) {
        if (product.getId() == this.id) {
            this.setProduct(product);
        }
    },

    attachFormHandler: function () {
        var addToCartForm = ise.Products.AddToCartFormController.getForm(this.id);

        var handler = Function.createDelegate(this, this.onAddToCartFormValidating);
        var addToCartFormValidatingEventHandler = handler;
        if (addToCartForm) {
            addToCartForm.addAddToCartValidatingEventHandler(addToCartFormValidatingEventHandler);
        }
        else {
            var id = this.id;

            var observer = {

                notify: function (form) {
                    if (form.getId() == id) {
                        form.addAddToCartValidatingEventHandler(addToCartFormValidatingEventHandler);
                    }
                }

            }

            ise.Products.AddToCartFormController.addObserver(observer);
        }
    },

    onAddToCartFormValidating: function (e) {
        if (!this.ensureAllAttributesAreSelected()) {
            e.cancel = true;

            var appendComma = false;

            if (this.product.hasSelectedMatrixProduct()) {
                e.cancel = false;
                return;
            }
            else {
                this.pnlError.innerHTML = ise.StringResource.getString('showproduct.aspx.40') + ' ';
                this.pnlSelectCaption.style.display = 'none';
            }

            for (var ctr = 0; ctr < this.attributeControls.length; ctr++) {
                var ctrl = this.attributeControls[ctr];
                if (!ctrl.hasAttributeSelected()) {
                    this.pnlError.innerHTML += (appendComma ? ', ' : '') + ctrl.getAttribute();
                    appendComma = true;
                }
            }
        }
        else {
            if (!this.product.hasSelectedMatrixProduct()) {
                e.cancel = true;
                this.pnlError.innerHTML = ise.StringResource.getString('showproduct.aspx.39');
            }
        }
    },

    onProductInterChanged: function () {
        this.clearError();

        var attributes = this.product.getAttributes();
        var controls = this.attributeControls;

        for (var ctr = 0; ctr < attributes.length; ctr++) {
            var attribute = attributes[ctr];
            for (var ictr = 0; ictr < controls.length; ictr++) {
                var currentControl = controls[ictr];
                if (currentControl.getAttribute() == attribute.code) {
                    currentControl.setValue(attribute.value);
                }
            }
        }
    },

    clearError: function () {
        this.pnlError.innerHTML = '';
    },

    registerAttributeControl: function (control) {
        this.attributeControls[this.attributeControls.length] = control;

        var handler = Function.createDelegate(this, this.onAttributeChangedEventHandler);
        control.addAttributeChangedEventHandler(handler);
    },

    onAttributeChangedEventHandler: function (selectedValue) {
        this.clearError();

        if (this.hasAtleastOneSelectedAttribute()) {
            var allAttributes = new Array();

            for (var ctr = 0; ctr < this.attributeControls.length; ctr++) {
                var ctrl = this.attributeControls[ctr];

                if (this.attributeControls[ctr].hasAttributeSelected()) {
                    var attribute = new ise.Products.MatrixAttribute(ctrl.getAttribute(), ctrl.getValue());
                    allAttributes.push(attribute);
                }
            }

            this.product.chooseAttributes(allAttributes);

            if (!this.product.hasSelectedMatrixProduct() && allAttributes.length == this.attributeControls.length) {

                this.pnlError.innerHTML = ise.StringResource.getString('showproduct.aspx.39');
                this.pnlSelectCaption.style.display = 'none';
                if (this.pnlStockHint) {
                    this.pnlStockHint.style.display = 'none';
                }

            }

        }
        else {
            this.pnlSelectCaption.style.display = '';
            if (this.pnlStockHint) {
                this.pnlStockHint.style.display = 'none';
            }
        }
    },

    hasAtleastOneSelectedAttribute: function () {
        for (var ctr = 0; ctr < this.attributeControls.length; ctr++) {
            var ctrl = this.attributeControls[ctr];
            if (ctrl.hasAttributeSelected()) return true;
        }
        return false;
    },

    selectedAttributeCount: function () {
        var selecteAttribute = 0;

        for (var ctr = 0; ctr < this.attributeControls.length; ctr++) {
            if (this.attributeControls[ctr].hasAttributeSelected()) {
                selecteAttribute = selecteAttribute + 1;
            }
        }

        return selecteAttribute;
    },

    ensureAllAttributesAreSelected: function () {
        for (var ctr = 0; ctr < this.attributeControls.length; ctr++) {
            var ctrl = this.attributeControls[ctr];

            if (!ctrl.hasAttributeSelected()) {
                return false;
            }
        }

        return true;
    }

}


ise.Products.ImageMultipleControl = function(id, index, src) {
    this.id = id;
    this.index = index;
    
    this.ctrl = $getElement(id);
    this.ctrl.src = src;
    
    this.attachClickEventHandler();
    this.attachHoverEventHandler();
    
    this.selectEventHandlers = new Array();
    this.hoverEventHandlers = new Array();
}
ise.Products.ImageMultipleControl.registerClass('ise.Products.ImageMultipleControl');
ise.Products.ImageMultipleControl.prototype = {

    getIndex : function() {
        return this.index;
    },
    
    attachClickEventHandler : function() {
        var handler = Function.createDelegate(this, this.onClickEventHandler);
        $addHandler(this.ctrl, 'click', handler);
    },
    
    addSelectEventHandler : function(handler) {
        this.selectEventHandlers[this.selectEventHandlers.length] = handler;
    },
    
    attachHoverEventHandler : function() {
        var handler = Function.createDelegate(this, this.onHoverEventHandler);
        $addHandler(this.ctrl, 'mouseover', handler);
    },
    
    onClickEventHandler : function() {
        for(var ctr=0; ctr<this.selectEventHandlers.length; ctr++) {
            var handler = this.selectEventHandlers[ctr];
            handler(this);
        }
    },
    
    addHoverEventHandler : function(handler) {
        this.hoverEventHandlers[this.hoverEventHandlers.length] = handler;
    },
    
    onHoverEventHandler : function() {
        for(var ctr=0; ctr<this.hoverEventHandlers.length; ctr++) {
            var handler = this.hoverEventHandlers[ctr];
            handler(this);
        }
    },
    
    setImage : function(src) {
        this.ctrl.src = src;
    },
    
    show : function() {
        this.ctrl.style.display = "";
    },
    
    hide : function() {
        this.ctrl.style.display = "none";
    }
}

ise.Products.ImageSwatchControl = function(id, clientId, itemCode){
    this.id = id;
    this.itemCode = itemCode;
    
    this.ctrl = $getElement(clientId);
    
    this.attachClickEventHandler();
    this.selectEventHandlers = new Array();
}
ise.Products.ImageSwatchControl.registerClass('ise.Products.ImageSwatchControl');
ise.Products.ImageSwatchControl.prototype = {

    getId : function() {
        return this.id;
    },
    
    getItemCode : function() {
        return this.itemCode;
    },
    
    attachClickEventHandler : function() {
        var handler = Function.createDelegate(this, this.onClickEventHandler);
        $addHandler(this.ctrl, 'click', handler);
    },
    
    
    addSelectEventHandler : function(handler) {
        this.selectEventHandlers[this.selectEventHandlers.length] = handler;
    },
    
    onClickEventHandler : function() {
        for(var ctr=0; ctr<this.selectEventHandlers.length; ctr++) {
            var handler = this.selectEventHandlers[ctr];
            handler(this);
        }
    },
    
    setImage : function(src) {
        this.ctrl.src = src;
    },
    
    show : function() {
        this.ctrl.style.display = "";
    },
    
    hide : function() {
        this.ctrl.style.display = "none";
    }
    
}

ise.Products.LargeImageLinkControl = function(id) {
    this.id = id;
    this.ctrl = $getElement(id);
    
    this.attachClickEventHandler();
    this.selectEventHandlers = new Array();
}
ise.Products.LargeImageLinkControl.registerClass('ise.Products.LargeImageLinkControl');
ise.Products.LargeImageLinkControl.prototype = {

    attachClickEventHandler : function() {
        var handler = Function.createDelegate(this, this.onClickEventHandler);
        $addHandler(this.ctrl, 'click', handler);
    },
    
    
    addSelectEventHandler : function(handler) {
        this.selectEventHandlers[this.selectEventHandlers.length] = handler;
    },
    
    onClickEventHandler : function() {
        for(var ctr=0; ctr<this.selectEventHandlers.length; ctr++) {
            var handler = this.selectEventHandlers[ctr];
            handler(this);
        }
    },
    
    show : function() {
        this.ctrl.style.display = "";
    },
    
    hide : function() {
        this.ctrl.style.display = "none";
    }
}

ise.Products.ImageControl = function(id, clientId) {
    this.id = id;
    this.ctrl = $getElement(clientId);
    
    this.lnkLargeImage = null;
    this.multipleControls = new Array();
    this.swatchControls = new Array();
    this.product = null;
    
    this.multipleImageIndex = -1;
    
    this.useMicroImages = false;
    this.handleHover = false;
}
ise.Products.ImageControl.registerClass('ise.Products.ImageControl');
ise.Products.ImageControl.prototype = {

    setProduct: function (product) {
        if (product) {
            this.product = product;

            if (this.product.areImagesLoaded()) {
                this.arrangeDisplay();
            }
            else {
                var handler = Function.createDelegate(this, this.onProductImagesLoaded);
                this.product.addImagesLoadedEventHandler(handler);
            }

            if (this.product.getItemType() == 'Matrix Group') {
                this.attachInterChangeEventHandler();
            }
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },

    notify: function (product) {
        if (product.getId() == this.id) {
            this.setProduct(product);
        }
    },

    arrangeDisplay: function () {
        if (this.product) {
            var src = this.product.getMediumImage().src;
            this.setImage(src);

            this.toggleLargeImageVisibility();
            this.arrangeMultipleControls();

            if (this.product.getItemType() == 'Matrix Group') {
                this.arrangeSwatchControls();
            }
        }
    },

    onProductImagesLoaded: function () {
        this.arrangeDisplay();
    },

    setLargeImageControl: function (control) {
        if (control) {
            //this.lnkLargeImage = control;
            //var handler = Function.createDelegate(this, this.onLargeImageLinkSelected);
            //this.lnkLargeImage.addSelectEventHandler(handler);
            //this.toggleLargeImageVisibility();
            this.imageZoom(this.id, 0);
            this.multipleImageHover = false;
        }
    },

    setUseMicroImages: function (useMicro) {
        this.useMicroImages = useMicro;
    },

    setHandleHover: function (handle) {
        this.handleHover = handle;
    },

    attachInterChangeEventHandler: function () {
        var handler = Function.createDelegate(this, this.onProductInterChanged);
        this.product.addInterChangeEventHandler(handler);
    },

    refreshImage: function () {
        var src = this.product.getMediumImage().src;
        this.setImage(src);

        this.multipleImageIndex = -1;

        this.toggleLargeImageVisibility();
        this.arrangeMultipleControls();
    },

    onProductInterChanged: function () {
        this.refreshImage();
    },

    registerMultipleControl: function (control) {
        this.multipleControls[this.multipleControls.length] = control;

        var handler = Function.createDelegate(this, this.onMultipleImageControlSelected);
        control.addSelectEventHandler(handler);

        if (this.handleHover) {
            control.addHoverEventHandler(handler);
        }

        this.arrangeMultipleControls();
    },

    toggleLargeImageVisibility: function () {
        if (this.lnkLargeImage && this.product) {

            var img = null;
            if (this.multipleImageIndex == -1) {
                img = this.product.getLargeImage();
            }
            else {
                img = this.product.getLargeImage(this.multipleImageIndex);
            }

            if (img && img.exists) {
                this.lnkLargeImage.show();

                var handler = Function.createDelegate(this, this.showLargeImage);
                this.ctrl.onclick = handler;

                this.ctrl.className = 'product_image';
            }
            else {
                this.lnkLargeImage.hide();
            }
        }
    },

    onLargeImageLinkSelected: function () {
        this.showLargeImage();
    },

    showLargeImage: function () {
        var img = null;
        if (this.multipleImageIndex == -1) {
            img = this.product.getLargeImage();
        }
        else {
            img = this.product.getLargeImage(this.multipleImageIndex);
        }

        var url = 'popup.aspx?psrc=' + encodeURIComponent(img.src);
        var name = 'LargeImage';
        var resizable = img.resizable ? 'yes' : 'no';
        var params = 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=' + resizable + ',resizable=' + resizable + ',copyhistory=no,width=' + img.size.width + ',height=' + img.size.height + 'left=0,top=0';
        window.open(url, name, params);
    },

    arrangeMultipleControls: function () {
        if (this.product) {
            for (var ctr = 0; ctr < this.multipleControls.length; ctr++) {
                var control = this.multipleControls[ctr];
                var img = this.product.getMediumImage(ctr);
                if (img && img.exists) {
                    // control has show function
                    control.show();

                    if (this.useMicroImages) {
                        var micro = this.product.getMicroImage(ctr);
                        control.setImage(micro.src);
                    }
                }
                else {
                    // control has hide function
                    control.hide();
                }
            }
        }
    },

    onMultipleImageControlSelected: function (control) {
        var index = control.getIndex();
        this.multipleImageIndex = index;

        this.imageZoom(this.id, index);

        var src = this.product.getMediumImage(index).src;

        var altText = this.product.getMediumImage(index).Alt;
        var titleText = this.product.getMediumImage(index).Title;

        if (null == altText) { altText = ''; }
        if (null == titleText) { titleText = ''; }

        this.setImage(src);
        this.setImageAlt(altText);
        this.setImageTitle(titleText);

        this.toggleLargeImageVisibility();
    },

    imageZoom: function (id, index) {

        if (this.product == null) {
            return;
        }

        var zoomOption = this.product.getImageZoomOption();

        //check if zoomoption will not be used
        if (zoomOption == null) {
            return;
        }

        var link = $("#lnkLarge_" + id);

        //assign title to link with image title
        var imgTitle = window["imgTitleArray" + id];
        $("#imgProduct_" + id).attr("title", imgTitle[index])
        link.attr("title", imgTitle[index]);


        //check if product large image is not available
        if (this.product.getLargeImage(index) == null) {
            link.attr("class", "");
            return;
        }


        //asign href to link with large image path
        link.attr("href", this.product.getLargeImage(index).src);

        if (link.attr("href") == undefined) {
            return;
        } else {

            //check if large image path contains the following key: {nopicture}
            if (link.attr("href").indexOf("nopicture") > -1) {
                link.removeClass();
                link.attr("href", "");
                link.css("cursor", "default");
                link.click(function () { return false; });
                return;
            }
        }

        switch (zoomOption.toLowerCase()) {
            case 'lens zoom': //lens zoom
                link.attr("rel", "adjustX: 10, adjustY:-4" + this.getImageZoomLensSize());
                this.imageZoomCloudStyle();
                break;
            case 'lens blur': //lens blur
                link.attr("rel", "tint: '#FF9933',tintOpacity:0.5 ,smoothMove:5, adjustY:-4, adjustX:10" + this.getImageZoomLensSize());
                this.imageZoomCloudStyle();
                break;
            case 'inner zoom': //inner zoomOption
                link.attr("rel", "position: 'inside' , showTitle: false, adjustX:-4, adjustY:-4");
                this.imageZoomCloudStyle();
                break;
            case 'blur focus': //blur focus
                link.attr("rel", "softFocus: true, smoothMove:2, adjustX: 10, adjustY:-4" + this.getImageZoomLensSize());
                this.imageZoomCloudStyle();
                break;
            case 'zoom out': //zoom out
                this.imageZoomFancyStyle({
                    'overlayShow': false,
                    'transitionIn': 'elastic',
                    'transitionOut': 'elastic'
                });
                break;
            case 'popup': //popout
                this.imageZoomFancyStyle({
                    'titlePosition': 'inside',
                    'transitionIn': 'none',
                    'transitionOut': 'fade'
                    });
                break;
            case 'gallery': //gallery
                //get the no. of multiple images
                var imgctrl = $("#pnlImageMultiples_" + id + " > img:visible").length;


                if (this.multipleImageIndex > -1) { //image is hovered
                    for (var ctr = 0; ctr < imgctrl; ctr++) {
                        $("#dummyImage_" + ctr).remove();
                    }

                    for (var ctr = 0; ctr < imgctrl; ctr++) {
                        if (ctr <= index) {
                            if (index != ctr) {
                                $("<a rel='example_group' href='" + this.product.getLargeImage(ctr).src + "' id='dummyImage_" + ctr + "' title='" + imgTitle[index] + "'>" +
                                                            "<img style='display:none;' src='" + this.product.getMediumImage(ctr).src + "'/>" + "</a>").insertBefore('#lnkLarge_' + id);
                            }
                        }
                        else {
                            $('#pnlImage_' + id).append("<a rel='example_group' href='" + this.product.getLargeImage(ctr).src + "' id='dummyImage_" + ctr + "' title='" + imgTitle[index] + "'>" +
                                                            "<img style='display:none;' src='" + this.product.getMediumImage(ctr).src + "'/>" + "</a>");
                        }
                    }
                }
                else { //image is set
                    for (var ctr = 0; ctr < imgctrl; ctr++) {
                        if (ctr != 0) {
                            $('#pnlImage_' + id).append("<a rel='example_group' href='" + this.product.getLargeImage(ctr).src + "' id='dummyImage_" + ctr + "' title='" + imgTitle[index] + "'>" +
                                                            "<img style='display:none;' src='" + this.product.getMediumImage(ctr).src + "'/>" + "</a>");
                        }
                    }
                }

                link.attr("rel", "example_group");
                $("a[rel=example_group]").fancybox({
                    'transitionIn': 'none',
                    'transitionOut': 'none',
                    'titlePosition': 'over',
                    'titleFormat': function (title, currentArray, currentIndex, currentOpts) {
                        return '<span id="fancybox-title-over">Image ' + (currentIndex + 1) + ' / ' + currentArray.length + '</span>';
                    }
                });
                break;
            default:
                this.imageZoomFancyStyle({
                    'titlePosition': 'inside',
                    'transitionIn': 'none',
                    'transitionOut': 'fade'
                });
                break;
        }
    },

    imageZoomCloudStyle: function () {
        $('.mousetrap').remove();
        $('.cloud-zoom').CloudZoom();
    },

    imageZoomFancyStyle: function (option) {
        $(".cloud-zoom").fancybox(option);
    },

    getImageZoomLensSize: function () {
        var imageZoomLensWidth = ise.Configuration.getConfigValue("ImageZoomLensWidth");
        var imageZoomLensHeight = ise.Configuration.getConfigValue("ImageZoomLensHeight");
        var lensZoomSize = '';

        if (imageZoomLensHeight > 0) {
            lensZoomSize += ",zoomHeight:" + imageZoomLensHeight;
        }

        if (imageZoomLensWidth > 0) {
            lensZoomSize += ",zoomWidth:" + imageZoomLensWidth;
        }
        
        return lensZoomSize;
    },

    registerSwatchControl: function (control) {
        this.swatchControls[this.swatchControls.length] = control;

        var handler = Function.createDelegate(this, this.onSwatchImageControlSelected);
        control.addSelectEventHandler(handler);
    },

    arrangeSwatchControls: function () {
        for (var ctr = 0; ctr < this.swatchControls.length; ctr++) {
            var swatchControl = this.swatchControls[ctr];
            var id = swatchControl.getId();
            var swatch = this.product.getSwatchImage(id);
            if (swatch && swatch.exists) {
                swatchControl.setImage(swatch.src);
                // SwatchControl has a show function
                swatchControl.show();
            }
            else {
                // SwatchControl has a hide function
                swatchControl.hide();
            }
        }
    },

    onSwatchImageControlSelected: function (control) {
        var itemCode = control.getItemCode();
        this.product.chooseMatrixItem(itemCode);
    },

    setImage: function (src) {
        this.ctrl.src = src;
    },

        setImageAlt: function (altText) {
        this.ctrl.alt = altText;
    },

    setImageTitle: function (titleText) {
        this.ctrl.title = titleText;
    }

}

var pnlShippingDate;
ise.Products.StockHintControl = function(id, clientId, inStockSrc, outOfStockSrc) {
    this.id = id;
    this.ctrl = $getElement(clientId);
    this.inStockSrc = inStockSrc;
    this.outOfStockSrc = outOfStockSrc;
            
    this.attributeControls = new Array();
    
    this.pnlSelectCaption = $getElement('pnlMatrixAttribute_SelectCaption_' + id);
    this.pnlStockHint = $getElement('pnlStockHint_' + id);
    this.pnlShippingDate = $getElement('pnlDisplayExpShipDate_' + id);
}
ise.Products.StockHintControl.registerClass('ise.Products.StockHintControl');
ise.Products.StockHintControl.prototype = {

    setProduct: function (product) {
        if (product) {
            this.product = product;
            this.arrangeDisplay();

            if (this.product.getItemType() == 'Matrix Group') {
                var handler = Function.createDelegate(this, this.onProductInterChanged);
                this.product.addInterChangeEventHandler(handler);
            }
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },

    notify: function (product) {
        if (product.getId() == this.id) {
            this.setProduct(product);
        }
    },

    onProductInterChanged: function () {
        this.arrangeDisplay();
    },

    arrangeDisplay: function () {
        if (this.product) {
            if (this.product.getItemType() == 'Matrix Group') {
                if (this.product.hasSelectedMatrixProduct()) {
                    this.pnlSelectCaption.style.display = 'none';
                    this.pnlStockHint.style.display = '';
                }
                else {
                    this.pnlSelectCaption.innerHTML = ise.StringResource.getString('showproduct.aspx.46') + '<br/><br/>';
                    this.pnlSelectCaption.style.display = '';
                    this.pnlStockHint.style.display = 'none';
                }
            }

            if (this.product.hasAvailableStock()) {
                this.showInStock();
                if (this.pnlShippingDate != undefined) {
                    this.pnlShippingDate.style.visibility = 'hidden';
                }
            }
            else {
                this.showOutOfStock();
                if (this.pnlShippingDate != undefined) {
                    this.pnlShippingDate.style.visibility = 'show';
                }
            }
        }
    },

    showInStock: function () {
        this.ctrl.src = this.inStockSrc;
    },

    showOutOfStock: function () {
        this.ctrl.src = this.outOfStockSrc;
    },

    show: function () {
        this.style.display = "";
    },

    hide: function () {
        this.style.display = "none";
    }

}

ise.Products.PricingLevelControl = function(id, clientId) {
    this.id = id;
    this.ctrl = $getElement('pnlPricingLevel_' + id);
    this.pnlInline = $getElement('pnlPricingLevelInline_' + id);
    this.pnlPopUp = $getElement('pnlPricingLevelPopUp_' + id);
    this.lnkPopUp = $getElement('lnkPricingLevelPopUp_' + id);
    
    this.product = null;
    this.showInline = true;
    this.backColor = '';
}
ise.Products.PricingLevelControl.registerClass('ise.Products.PricingLevelControl');
ise.Products.PricingLevelControl.prototype = {

    setProduct : function(product) {
        if(product) {
            this.product = product;
            this.buildDisplay();
            
            if(this.product.getItemType() == 'Matrix Group') {
                var handler = Function.createDelegate(this, this.onProductInterChanged);
                this.product.addInterChangeEventHandler(handler);
            }
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },
    
    notify : function(product) {
        if(product.getId() == this.id) {
            this.setProduct(product);
        }
    },
    
    onProductInterChanged : function() {
        this.buildDisplay();
    },
    
    setShowInline : function(inline) {
        this.showInline = inline;
    },
    
    setBackColor : function(color) {
        this.backColor = color;
    },
    
    clearDisplay : function() {
        this.pnlInline.style.display = 'none';
        this.pnlPopUp.style.display = 'none';
    },
    
    buildDisplay : function() {
        this.clearDisplay();
        if(this.product && !this.product.hasPromotionalPrice()) {
        
            var handler = Function.createDelegate(this, this.arrangeDisplay);            
            
            var itemCode = encodeURIComponent(this.product.getItemCode());
            var service = new ActionService();
            service.GetPricingLevel(itemCode, handler);
        }
    },
    
    arrangeDisplay : function(html) {
        if(html == '') {
            if(this.pnlInline) this.pnlInline.style.display = 'none';
            if(this.pnlPopUp) this.pnlPopUp.style.display = 'none';
        }
        else {
            if(this.showInline) {
                this.pnlInline.innerHTML = '<p><b>' + ise.StringResource.getString('showproduct.aspx.7') + '<br/>' + html + '<br/> </b></p>';
                this.pnlInline.style.display = '';
            }
            else {
                // tooltip.js
                new ToolTip(this.lnkPopUp.id, 'pricingLevel_ToolTip', html);
                this.pnlPopUp.style.display = '';
            }
        }
    }
}

ise.Products.QuantityControl = function(id, clientId, initialQuantity) {
    this.id = $getElement(id);
    this.ctrl = $getElement(clientId);
    this.initialQuantity = initialQuantity;
    
    this.product = null;
    this.getValueDelegate = null
}
ise.Products.QuantityControl.registerClass('ise.Products.QuantityControl');
ise.Products.QuantityControl.prototype = {

    getValue : function() {
        if(this.getValueDelegate) {
            return this.getValueDelegate();
        }
        
        return 0;
    },
    
    setProduct : function(product) {
        if(product) {
            this.product = product;
            this.buildDisplay();
            
            if(this.product.getItemType() == 'Matrix Group') {
                var handler = Function.createDelegate(this, this.onProductInterChanged);
                this.product.addInterChangeEventHandler(handler);
            }
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },
    
    notify : function(product) {
        if(product.getId() == this.id) {
            this.setProduct(product);
        }
    },
    
    onProductInterChanged : function() {
        this.buildDisplay();
    },
    
    buildDisplay : function() {
        this.clearDisplay();
        
        var span = document.createElement('SPAN');
        span.id = 'lblQuantity_' + this.id;
        span.innerHTML = ise.StringResource.getString('showproduct.aspx.31') + '&nbsp;';
        this.ctrl.appendChild(span);
            
        if(this.product.hasRestrictedQuantities()) {
            var restrictedQuantities = this.product.getRestrictedQuantities();
            var select = document.createElement('SELECT');
            select.id = 'Quantity_' + this.id;
            select.name = 'Quantity';
            
            for(var ctr=0; ctr<restrictedQuantities.length; ctr++) {
                var quantity = restrictedQuantities[ctr];
                select.options.add(new Option(quantity, quantity));
            }
            
            this.ctrl.appendChild(select);
            
            this.getValueDelegate = function(){ return select.options[select.selectedIndex].value; };
        }
        else {
            var input = document.createElement('INPUT');
            input.type = 'text';
            input.id = 'Quantity' + this.id;
            input.name = 'Quantity';
            input.size = 3;
            input.maxLength = 14;
            input.value = this.initialQuantity;
            this.ctrl.appendChild(input);
            
            this.getValueDelegate = function(){ return input.value; };
        }
    },
    
    clearDisplay : function() {
        this.ctrl.innerHTML = '';
    }
        
}


ise.Products.AddToCartForm = function (id) {
    this.id = $getElement(id);
    this.pnlAddToCart = $getElement('pnlAddToCartForm_' + id);

    this.form = $getElement('AddToCartForm_' + id);
    var submitHandler = Function.createDelegate(this, this.onAddToCartClick);
    this.form.onsubmit = submitHandler;

    this.btnAddToCart = $getElement('AddToCart_' + id);
    var addToCartClickHandler = Function.createDelegate(this, this.onAddToCartClick);
    this.btnAddToCart.onclick = addToCartClickHandler;

    this.btnAddToWishList = $getElement('AddToWishList_' + id);
    if (this.btnAddToWishList) {
        var addToWishHandler = Function.createDelegate(this, this.onAddToWishListClick);
        this.btnAddToWishList.onclick = addToWishHandler;
    }

    //gift registry feature
    var str = ise.Configuration.getConfigValue("GiftRegistry.Enabled");
    if (str == 'true') {

        this.btnAddToRegistry = $getElement('addRegistryButton_' + id);
        this.btnAddToRegistryOption = $getElement('addOptionRegistryButton_' + id);

        if (this.btnAddToRegistry != undefined && this.btnAddToRegistryOption != undefined) {
            this.btnAddToRegistry.onclick = Function.createDelegate(this, this.onAddToGiftRegistryClick);
            this.btnAddToRegistryOption.onclick = Function.createDelegate(this, this.onAddToGiftRegistryOptionClick);
        }

    }

    this.ctrlGiftRegistryDropdown = null;
    this.ctrlQuantity = null;
    this.checkForFreeStock = false;
    this.product = null;

    this.addToCartValidatingEventHandlers = new Array();
}
ise.Products.AddToCartForm.registerClass('ise.Products.AddToCartForm');
ise.Products.AddToCartForm.prototype = {

    getId: function () {
        return this.id;
    },

    setProduct: function (product) {
        if (product) {
            this.product = product;
            this.toggleVisibility();

            if (this.product.getItemType() == 'Matrix Group') {
                var handler = Function.createDelegate(this, this.toggleVisibility);
                this.product.addInterChangeEventHandler(handler);
            }
        }
        else {
            ise.Products.ProductController.addObserver(this);
        }
    },

    toggleVisibility: function () {
        if (this.product.getShowBuyButton()) {
            this.pnlAddToCart.style.display = '';
        }
        else {
            this.pnlAddToCart.style.display = 'none';
        }
    },

    notify: function (product) {
        if (product.getId() == this.id) {
            this.setProduct(product);
        }
    },

    addAddToCartValidatingEventHandler: function (handler) {
        this.addToCartValidatingEventHandlers.push(handler);
    },

    onAddToCartValidating: function (cancelEventArgs) {

        for (var ctr = 0; ctr < this.addToCartValidatingEventHandlers.length; ctr++) {
            var cancelEventHandler = this.addToCartValidatingEventHandlers[ctr];
            cancelEventHandler(cancelEventArgs);
            if (cancelEventArgs.cancel) {
                return true;
            }
        }

        return false;
    },

    setCheckForFreeStock: function (check) {
        this.checkForFreeStock = check;
    },

    onAddToCartClick: function () {
        if (this.validate()) {
            $getElement('IsWishList_' + this.id).value = 0;
            return this.doSubmit();
        }
        return false;
    },

    onAddToWishListClick: function () {
        if (this.validate()) {
            $getElement('IsWishList_' + this.id).value = 1;
            return this.doSubmit();
        } 
        return false;
    },

    onAddToGiftRegistryClick: function () {
        $('#IsAddToGiftRegistry_' + this.id).val(1);
        if (this.validate()) {
            return this.doSubmit();
        }
        return false;
    },

    onAddToGiftRegistryOptionClick: function () {
        $('#IsAddToGiftRegistryOption_' + this.id).val(1);
        if (this.validate()) {
            return this.doSubmit();
        }
        return false;
    },

    setQuantityControl: function (control) {
        this.ctrlQuantity = control;
    },

    setGiftRegistryDropdownControl: function (control) {
        this.ctrlGiftRegistryDropdown = control;
    },

    validate: function () {
        if (null == this.product) {
            alert(ise.StringResource.getString('showproduct.aspx.38'));
            return false;
        }

        // check for other handlers
        var cancelEventArgs = { cancel: false }
        this.onAddToCartValidating(cancelEventArgs);
        if (cancelEventArgs.cancel) {
            return false;
        }

        if (this.product.getItemType() == 'Matrix Group' && !this.product.hasSelectedMatrixProduct()) {
            alert(ise.StringResource.getString('showproduct.aspx.29'));
            return false;
        }

        var quantity = this.ctrlQuantity.getValue();
        if (quantity.length > 0 && quantity.charAt(0) == this.product.getQuantityDecimalSeparator()) {
            quantity = this.product.getQuantityDecimalZero() + quantity;
        }

        var quantityRegEx = this.product.getQuantityRegEx();

        if (!quantity.match(quantityRegEx)) {
            alert(ise.StringResource.getString('common.cs.22'));
            return false;
        }

        if (quantity == 0) {

            if ($('#IsAddToGiftRegistry_' + this.id).val() == 1) {
                alert(ise.StringResource.getString('editgiftregistry.error.6'));
                return false;
            }

            if ($('#IsAddToGiftRegistryOption_' + this.id).val() == 1) {
                alert(ise.StringResource.getString('editgiftregistry.error.6'));
                return false;
            }

            alert(ise.StringResource.getString('common.cs.24'));
            return false;
        }

        if (this.product.getItemType() == "Kit") {
            this.product.persistComposition();
        }

        var str = ise.Configuration.getConfigValue("GiftRegistry.Enabled");
        if (str == 'true') {
            if ($('#IsAddToGiftRegistry_' + this.id).val() == 1 || $('#IsAddToGiftRegistryOption_' + this.id).val() == 1) {
                if (this.ctrlGiftRegistryDropdown.value == 0) {
                    alert(ise.StringResource.getString('editgiftregistry.error.5'));
                    $('#IsAddToGiftRegistryOption_' + this.id).val(0);
                    $('#IsAddToGiftRegistry_' + this.id).val(0);
                    return false;
                }
                return true;
            }
        }

        if (quantity < this.product.getMinimumOrderQuantity()) {
            alert(ise.StringResource.getString('showproduct.aspx.36') + this.product.getMinimumOrderQuantity());
            return false;
        }

        if (this.checkForFreeStock) {

            if (this.product.getItemType() == 'Stock' ||
                this.product.getItemType() == 'Matrix Group' ||
                this.product.getItemType() == 'Matrix Item' ||
                this.product.getItemType() == 'Assembly') {

                if (this.product.getFreeStock() <= 0) {
                    alert(ise.StringResource.getString('showproduct.aspx.30'));
                    return false;
                }

                if (this.product.getFreeStock() < quantity) {
                    alert(ise.StringResource.getString('showproduct.aspx.42') + '   ' + this.product.getFreeStock());
                    return false;
                }
            }
            else if (this.product.getItemType() == "Kit") {
                var allHasStock = true;

                for (var ctr = 0; ctr < this.product.groups.length; ctr++) {
                    if (allHasStock) {
                        var group = this.product.groups[ctr];
                        var items = group.getSelectedItems();

                        for (var ictr = 0; ictr < items.length; ictr++) {
                            var item = items[ictr];

                            if (item &&
                                (item.getItemType() == 'Stock' || item.getItemType() == 'Matrix Item' || item.getItemType() == 'Assembly')) {
                                if (item.getFreeStock() < quantity) {
                                    allHasStock = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!allHasStock) {
                    alert(ise.StringResource.getString('showproduct.aspx.43'));
                    return false;
                }
            }

        }

        return true;
    },

    doSubmit: function () {
        if (this.product.getItemType() == 'Matrix Group' && this.product.hasSelectedMatrixProduct()) {
            $getElement('ProductID_' + this.id).value = this.product.getId();
        }
        this.form.submit();
        return false;
    }

}

ise.Products.AddToCartFormController = {
    
    initialize : function() {
        this.forms = new Array();
        this.observers = new Array();
    },
    
    registerForm : function(form) {
        this.forms[form.getId()] = form;
        this.notifyObservers(form);
    },
    
    getForm : function(id) {
        return this.forms[id];
    },
    
    addObserver : function(observer) {
        this.observers.push(observer);
    },
    
    notifyObservers : function(form) {
        for(var ctr=0; ctr<this.observers.length; ctr++) {
            var observer = this.observers[ctr];
            observer.notify(form);
        }
    }

}

ise.Products.AddToCartFormController.initialize();

function ShowProductNotifyPriceDropPopUp(notificationCode, itemcode, productUrl, priceDropString) {
    SavePop(notificationCode, itemcode, productUrl);
    $getElement('NotifyOnPriceDrop').disabled = 'disabled';
    $getElement('NotifyOnPriceDrop').value = priceDropString;
}

function ShowProductOnItemAvailPopUp(notificationCode, itemcode, productUrl, notifyAvailMessagePrompt) {
    SavePop(notificationCode, itemcode, productUrl);
    $getElement('NotifyOnItemAvail').disabled = 'disabled';
    $getElement('NotifyOnItemAvail').value = notifyAvailMessagePrompt;
}

function SavePop(notificationCode, itemcode, productUrl) {
    window.open('savenotification.aspx?NotificationType=' + notificationCode + 
                '&itemCode=' + itemcode +
                '&ProductURL=' + productUrl, 
                '', 'scrollbars=no,menubar=no,height=1,width=1,resizable=yes,toolbar=no,location=no,status=no');
}

function InitAddToCartFormScript(jSONAddToCartParamDTO) {

    var paramObject = $.parseJSON(jSONAddToCartParamDTO);

    ise.StringResource.registerString('showproduct.aspx.29', paramObject.AttributeNotAvailableText);
    ise.StringResource.registerString('showproduct.aspx.30', paramObject.ProductNoEnuoughStockText);
    ise.StringResource.registerString('showproduct.aspx.31', paramObject.QuantityText);
    ise.StringResource.registerString('showproduct.aspx.32', paramObject.UnitMeasureText);
    ise.StringResource.registerString('showproduct.aspx.36', paramObject.OrderLessThanText);
    ise.StringResource.registerString('showproduct.aspx.42', paramObject.NoEnoughStockText);
    ise.StringResource.registerString('showproduct.aspx.43', paramObject.SelectedItemNoEnoughStockText);
    ise.StringResource.registerString('showproduct.aspx.46', paramObject.NotificationAvailabilityText);
    ise.StringResource.registerString('common.cs.22', paramObject.EnterNoQuantityText);
    ise.StringResource.registerString('common.cs.24', paramObject.SpecifyQuantityText);

    var product = ise.Products.ProductController.getProduct(paramObject.ItemCounter);
    var frm = new ise.Products.AddToCartForm(paramObject.ItemCounter);
    frm.setProduct(product);
    ise.Products.AddToCartFormController.registerForm(frm);
    frm.setCheckForFreeStock(paramObject.IgnoreStockLevel);

    if (paramObject.ItemTypeIsKitAndWeAreInEditMode) {
        // make ready our delegate method
        var delResetUnitMeasure = function (p) {
            p.setUnitMeasure(paramObject.UnitMeasureCode);
            p.onCompositionChanged();
        };

        if (product) {
            delResetUnitMeasure(product);
        } else {

            var umObserver = {
                notify: function (product) {
                    if (product.getId() == paramObject.ItemCounter) {
                        delResetUnitMeasure(product);
                    }
                }
            }

            ise.Products.ProductController.addObserver(umObserver);
        }
    }

    var qControl = new ise.Products.QuantityControl(paramObject.ItemCounter, 'ctrlQuantity_' + paramObject.ItemCounter, paramObject.InitialQuantity);
    qControl.setProduct(product);
    frm.setQuantityControl(qControl);

    frm.setGiftRegistryDropdownControl($getElement('giftregistryOptions_' + paramObject.ItemCounter));

    if (!paramObject.HideUnitMeasure) {
        var umControl = new ise.Products.UnitMeasureControl(paramObject.ItemCounter, 'ctrlUnitMeasure_' + paramObject.ItemCounter);
        umControl.setProduct(product);
    }

}