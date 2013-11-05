(function ($) {

    $.fn.cartcontrol = function (options, unitMeasureCtrl, priceCtrl, promoPriceCtrl, quantityCtrl, stockHintCtrl, matrixOptionCtrl, kitOptionCtrl, messageBoardCtrl, addToCtrl) {
        //console.log(options);

        // INITIALIZE
        var base = this; //cache base elements
        var defaults = {}
        var options = $.extend(defaults, options);
        var captions = {
            UnitMeasure: "",
            Price: "",
            PromoPrice: "",
            Quantity: "",
            StockHint: "",
            KitDetails: "",
            AddToCart: "",
            CallToOrder: ""
        }
        var messages = {
            MatrixItemNotConfigured: "",
            SelectOption: "",
            MinOrder: "",
            InvalidQuantity: "",
            SpecifyQuantity: "",
            PleaseSelect: "",
            ItemAdded: ""
        }
        var url = { ShoppingCart: "shoppingcart.aspx" }
        var currentitem = {
            ItemCode: null,
            ItemCounter: null,
            ItemType: null,
            ShowStockHint: null,
            ShowActualStock: null,
            InstockImg: options.imgInStock,
            OutOfStockImg: options.imgOutOfStock,
            UnitMeasures: [],
            RestrictedQuantity: [],
            HidePrice: null,
            AddToCartAction: null,
            callToOrder: null,
            minOrder: 0,
            ShowBuyButton: null,
            HideUnitMeasure: null
        }

        // PUBLIC FUNCTIONS
        this.initCartControl = function () {
            currentitem.ItemCode = options.itemCode;
            currentitem.ItemCounter = options.itemCounter;
            currentitem.ItemType = options.itemType;
            currentitem.ShowStockHint = options.showStockHint;
            currentitem.ShowActualStock = options.showActualStock;
            currentitem.UnitMeasures = options.unitMeasures;
            currentitem.RestrictedQuantity = options.restrictedQty;
            currentitem.HidePrice = options.hidePrice;
            currentitem.AddToCartAction = options.addToCartAction;
            currentitem.callToOrder = options.callToOrder;
            currentitem.minOrder = options.minOrder;
            currentitem.ShowBuyButton = options.showBuyButton;
            currentitem.HideUnitMeasure = options.hideUnitMeasure;

            var callBack = function () {
                //assign string resources
                messages.InvalidQuantity = ise.StringResource.getString("common.cs.22");
                messages.SpecifyQuantity = ise.StringResource.getString("common.cs.24");
                messages.PleaseSelect = ise.StringResource.getString("showproduct.aspx.40");
                messages.MinOrder = ise.StringResource.getString("showproduct.aspx.36");
                messages.SelectOption = ise.StringResource.getString("showproduct.aspx.46");
                messages.MatrixItemNotConfigured = ise.StringResource.getString("showproduct.aspx.39");
                messages.ItemAdded = ise.StringResource.getString("itempopup.aspx.7");

                captions.UnitMeasure = ise.StringResource.getString("showproduct.aspx.32");
                captions.Price = ise.StringResource.getString("showproduct.aspx.33");
                captions.PromoPrice = ise.StringResource.getString("showproduct.aspx.34");
                captions.Quantity = ise.StringResource.getString("showproduct.aspx.31");
                captions.StockHint = ise.StringResource.getString("showproduct.aspx.47");
                captions.AddToCart = ise.StringResource.getString("showproduct.aspx.58");
                captions.CallToOrder = ise.StringResource.getString("common.cs.20");

                //dislay controls
                base.DisplayUnitMeasureControl();
                base.DisplayPriceControl();
                base.DisplayPromoPriceControl();
                base.DisplayQuantityControl();
                base.DisplayStockHintControl();
                base.DisplayAddToControl();

                switch (currentitem.ItemType.toLowerCase()) {
                    case "matrix group":
                        base.DisplayMatrixOptionControl();
                        stockHintCtrl.hide();
                        break;
                    case "kit":
                        UpdateKitComposition();
                        break;
                    default:
                }

                if (currentitem.callToOrder) { ShowCallToOrder(); }

                //attach events
                base.EventsListener();
            }
            base.LoadStringResource(callBack);
        }
        this.LoadStringResource = function (callback) {
            var keys = new Array();
            keys.push("common.cs.22"); //invalid quantity
            keys.push("common.cs.24"); //specify quantity
            keys.push("showproduct.aspx.40"); //please select
            keys.push("showproduct.aspx.36"); //min order
            keys.push("showproduct.aspx.46"); //select option
            keys.push("showproduct.aspx.39"); //matrix item not configured

            keys.push("showproduct.aspx.32"); //unit measure
            keys.push("showproduct.aspx.33"); //price
            keys.push("showproduct.aspx.34"); //promo price
            keys.push("showproduct.aspx.31"); //quantity
            keys.push("showproduct.aspx.47"); //stock hint
            keys.push("showproduct.aspx.58"); //add to cart
            keys.push("common.cs.20"); //call to orde
            keys.push("itempopup.aspx.7"); //item added

            ise.StringResource.loadResources(keys, callback);
        }
        this.EventsListener = function () {

            $(addToCtrl.children("input[name=AddToCart]")).click(function () {
                var itemcounter = $(this).attr("itemcounter");
                var composition = $(this).attr("kitcomposition");
                var qty = GetQuantity();
                var umcode = GetUnitMeasure();

                //for matrix item
                var addToCartID = $(this).attr("id").replace("AddToCart_", "");
                var matrixOptions = $("#MatrixOptionControlID_" + addToCartID + " select[name=MatrixOpt]");
                if (matrixOptions.length > 0) {
                    var requiredOptions = new Array();
                    $.each(matrixOptions, function () {
                        if ($(this).prop("selectedIndex") == 0) {

                            var selectID = $(this).attr("id");
                            var defaultOption = $("#" + selectID + " option:first").val();
                            requiredOptions.push(defaultOption);
                        }
                    });

                    if (requiredOptions.length > 0) {
                        var msg = messages.PleaseSelect;
                        for (var i = 0; i < requiredOptions.length; i++) {
                            msg += " " + requiredOptions[i] + ",";
                        }
                        alert(msg.slice(0, -1));
                        return;
                    }
                }

                if (itemcounter == "" || itemcounter == undefined) {
                    //do nothing
                    return;
                }
                if (qty < currentitem.minOrder) {
                    alert(messages.MinOrder + " " + currentitem.minOrder);
                    return;
                }

                if (qty == 0 || qty == "") {
                    alert(messages.SpecifyQuantity);
                    return;
                }

                var regex = new RegExp($("#quantityRegex").val());
                if (!qty.match(regex)) {
                    alert(messages.InvalidQuantity);
                    return;
                }

                qty = qty.replace(',', '')

                var params = { counter: itemcounter, quantity: qty, kitcomposition: composition, unitmeasure: umcode }
                var result = ActionService("AddToCartEx", params, false);
                if (currentitem.AddToCartAction == "STAY") { alert(messages.ItemAdded); }
                else { window.location = url.ShoppingCart; }

            });

            //kit item div select and unselect event
            $(kitOptionCtrl.children(".kitgroup").children(".kitgroup-content").children("div")).click(function () {
                var control = $(this).children(".itemcontrol")[0];
                var checked = $(control).attr("checked");
                var type = $(control).attr("type");

                if (checked && type == "checkbox") { $(control).attr("checked", false); }
                else { $(control).attr("checked", true); }

                KitItemSelectedEventHandler(control);
            });

            //kit item control select and unselect event
            $(".itemcontrol").click(function (event) {
                //stop firing the parent div onclick event
                event.stopPropagation();

                KitItemSelectedEventHandler($(this)[0]);
            });

            //kit option collapsible panel event
            $(kitOptionCtrl.children(".kitgroup").children(".kitgroup-header")).click(function () {
                var maxHeight = kitOptionCtrl.css('max-height').replace("px", ""); ;
                var itemMaxHeight = maxHeight / (kitOptionCtrl.children(".kitgroup").length);
                var headerHeight = 25;
                var currentKitHidden = $(this).next().is(':hidden');

                if (currentKitHidden) {
                    $(this).children(".icon").html("-"); //todo: use icon
                }
                else {
                    $(this).children(".icon").html("+"); //todo: use icon
                }

                $(this).next().css('max-height', Math.floor(itemMaxHeight) - headerHeight);
                $(this).next().stop().toggle('blind', 'fast');
                $(this).next().children("input:checked").focus();
                return false;
            }).next().hide();

            //unit measure select change event
            $(unitMeasureCtrl.children().children()).change(function () {
                var index = $(this).prop("selectedIndex");
                SetPriceDisplay(currentitem.UnitMeasures[index].priceFormatted, currentitem.HidePrice);
                SetPromoPriceDisplay(currentitem.UnitMeasures[index].hasPromotionalPrice, currentitem.UnitMeasures[index].promotionalPriceFormatted, currentitem.HidePrice);
                SetStockHintDisplay(currentitem.ShowStockHint, currentitem.ShowActualStock, currentitem.UnitMeasures[index].freeStock, currentitem.InstockImg, currentitem.OutOfStockImg);
            });

            //matrix option select change event
            $(matrixOptionCtrl.children("select[name='MatrixOpt']")).change(function () {
                var matrixItemCombination = [];

                $.each(matrixOptionCtrl.children("select[name='MatrixOpt']"), function () {
                    var attribute = { code: $(this).children("option:eq(0)").val(), value: $(this).val() }

                    //get selected value of every matrix option dropdown
                    if (attribute.code != attribute.value) {
                        matrixItemCombination.push(attribute);
                    }
                });

                //validate if selected matrixitem combination is valid or existing
                GetMatrixItemDetails(matrixItemCombination, currentitem.ItemCode, currentitem.ItemCounter);
            });
        }
        //displays the addto control
        this.DisplayAddToControl = function () {
            if (addToCtrl.length > 0) {
                addToCtrl.empty();
                if (currentitem.ShowBuyButton == true) {
                    var itemcounter = "";
                    if (currentitem.ItemType.toLowerCase() != "matrix group") {
                        itemcounter = currentitem.ItemCounter
                    }
                    addToCtrl.append(StringFormat("<input type='submit' class='addto' id='{0}' name='{1}' itemcounter='{2}' kitcomposition='' value='{3}'  />",
                    "AddToCart_" + currentitem.ItemCounter,
                    "AddToCart",
                    itemcounter,
                    captions.AddToCart));
                    addToCtrl.show();
                }
            }
            else {
                console.log("addto display control is undefined");
            }
        }
        //displays the matrix option control
        this.DisplayMatrixOptionControl = function () {
            if (matrixOptionCtrl.length > 0) {
                //future use: change matrix control background
            }
            else {
                console.log("matrixoption display control is undefined");
            }
        }
        //displays the stock hint control
        this.DisplayStockHintControl = function () {
            if (stockHintCtrl.length > 0) {
                stockHintCtrl.empty();
                stockHintCtrl.append("<span class='caption'>" + captions.StockHint + "</span>");
                stockHintCtrl.append("<span class='stockhint'></span>");
                SetStockHintDisplay(currentitem.ShowStockHint, currentitem.ShowActualStock, currentitem.UnitMeasures[0].freeStock, currentitem.InstockImg, currentitem.OutOfStockImg);
            }
            else {
                console.log("stockhint display control is undefined");
            }
        }
        //displays the quantity control
        this.DisplayQuantityControl = function () {
            if (quantityCtrl.length > 0) {
                quantityCtrl.empty();
                quantityCtrl.append("<span class='caption'>" + captions.Quantity + "</span>");
                quantityCtrl.append("<span class='quantity'></span>");

                SetQuantityDisplay(currentitem.RestrictedQuantity);

                //set the default value
                quantityCtrl.children(".quantity").children("").val(1);
            }
            else {
                console.log("quantity display control is not defined");
            }
        }
        //displays the promo price control (e.g. Promo Price: $ 6.00)
        this.DisplayPromoPriceControl = function () {
            if (promoPriceCtrl.length > 0) {
                //display promo price with label
                promoPriceCtrl.empty();
                promoPriceCtrl.append("<span class='caption'>" + captions.PromoPrice + "</span>");
                promoPriceCtrl.append("<span class='price'></span>");

                //set the promo price
                SetPromoPriceDisplay(currentitem.UnitMeasures[0].hasPromotionalPrice, currentitem.UnitMeasures[0].promotionalPriceFormatted, currentitem.HidePrice);
            }
            else {
                console.log("promo price display control is not defined");
            }
        }
        //displays the price control (e.g. Price: $ 10.00)
        this.DisplayPriceControl = function () {
            if (priceCtrl.length > 0) {
                //display price with label
                priceCtrl.empty();
                priceCtrl.append("<span class='caption'>" + captions.Price + "</span>");
                priceCtrl.append("<span class='price'></span>");

                //set the price
                SetPriceDisplay(currentitem.UnitMeasures[0].priceFormatted, currentitem.HidePrice);
            }
            else {
                console.log("price display control is not defined");
            }
        }
        //displays the unit measure control (e.g Unit Measure: Each)
        this.DisplayUnitMeasureControl = function () {
            if (unitMeasureCtrl.length > 0) {
                unitMeasureCtrl.empty();
                unitMeasureCtrl.append("<span class='caption'>" + captions.UnitMeasure + "</span>");
                unitMeasureCtrl.append("<span class='unitmeasure'></span>");

                SetUnitMeasuresDisplay(currentitem.UnitMeasures, currentitem.HideUnitMeasure);
            }
            else {
                console.log("unit measure display control is not defined");
            }
        }

        // PRIVATE FUNCTIONS
        function KitItemSelectedEventHandler(control) {
            var type = $(control).attr("type");
            var checked = $(control).attr("checked");
            var parent = $(control).parent();
            var style = "normal";

            switch (type) {
                case "checkbox":
                    if (checked) { style = "selected"; }
                    break;
                case "radio":
                    $(parent).siblings().attr("class", style);
                    style = "selected";
                    break;
                default:
            }

            $(parent).attr("class", style);
            UpdateKitComposition();
        }

        function GetUnitMeasure() {
            var um = null;

            //get umcode from dropdown
            var ctrl = $(unitMeasureCtrl).children(".unitmeasure").children("select")[0];
            if (ctrl != undefined) {
                um = $(ctrl).val();
                return um;
            }

            //get quantity from textblock
            ctrl = $(unitMeasureCtrl).children(".unitmeasure");
            if (ctrl != undefined) {
                um = $(ctrl).text();
                return um;
            }
            return um;
        }

        function GetQuantity() {
            var quantity = 1;

            //get quantity from textbox
            var ctrl = $(quantityCtrl).children(".quantity").children("input")[0];
            if (ctrl != undefined) {
                quantity = $(ctrl).val();
                return quantity;
            }

            //get quantity from dropdown
            ctrl = $(quantityCtrl).children(".quantity").children("select")[0];
            if (ctrl != undefined) {
                quantity = $(ctrl).val();
                return quantity;
            }
            return quantity;
        }


        //todo: utilize the action service getitemprice
        function UpdateKitComposition() {
            var selectedKit = $(kitOptionCtrl).children(".kitgroup").children(".kitgroup-content").children("div.selected");
            var symbol = $(kitOptionCtrl).children("input[name=currencysymbol]").val();
            var amount = parseFloat(0);
            var kitcomposition = "";

            for (var i = 0; i < selectedKit.length; i++) {
                var price = Number($(selectedKit[i]).children("input.itemprice").val());
                amount = (amount * 100 + price * 100) / 100;

                var kititemid = $(selectedKit[i]).children("input.itemprice").attr("itemid");
                var groupid = $(selectedKit[i]).children("input.itemprice").attr("groupid");
                if (kititemid != null) {
                    kitcomposition += groupid + "+" + (kititemid - currentitem.ItemCounter);
                    if (i != selectedKit.length - 1) {
                        kitcomposition += ",";
                    }
                }
            }
            SetPriceDisplay(symbol + ' ' + amount, currentitem.HidePrice, true);

            $(addToCtrl).children("input[name=AddToCart]").attr("kitcomposition", kitcomposition);

            //console.log(kitcomposition);
        }
        function StringFormat() {
            var s = arguments[0];
            for (var i = 0; i < arguments.length - 1; i++) {
                var reg = new RegExp("\\{" + i + "\\}", "gm");
                s = s.replace(reg, arguments[i + 1]);
            }
            return s;
        }
        function SetQuantityDisplay(restrictQty) {
            var quantity;
            var strictQuantity = restrictQty;

            if (strictQuantity.length > 0) {
                //dropdown control
                quantity = "<select>";
                for (var i = 0; i < strictQuantity.length; i++) {
                    quantity += StringFormat("<option value='{0}'>{0}</option>", strictQuantity[i]);
                }
                quantity += "</select>";
            }
            else {
                //textbox control
                quantity = "<input type='text' name='txtquantity' size='3' maxlength='14' />";
            }
            quantityCtrl.children(".quantity").html(quantity);
            quantityCtrl.show();
            quantityCtrl.children(".quantity").children("").val(1);
        }
        function SetUnitMeasuresDisplay(unitmeasures, hideUnitMeasure) {

            if (hideUnitMeasure) { unitMeasureCtrl.hide(); }
            else {
                var unitMeasure;
                if (unitmeasures.length > 1) {
                    unitMeasure = "<select>";
                    for (var i = 0; i < unitmeasures.length; i++) {
                        unitMeasure += StringFormat("<option value='{0}'>{1}</option>", unitmeasures[i].code, unitmeasures[i].description);
                    }
                    unitMeasure += "</select>";
                }
                else { unitMeasure = currentitem.UnitMeasures[0].description; }
                //console.log(unitMeasureCtrl.children(".unitmeasure").children("select"));
                unitMeasureCtrl.children(".unitmeasure").html(unitMeasure);
                unitMeasureCtrl.show();
            }
        }
        function SetPriceDisplay(price, hideprice, animate) {
            if (hideprice) {
                priceCtrl.hide();
            }
            else {
                if (animate) {
                    $(priceCtrl).fadeIn('slow', function () {
                        priceCtrl.children(".price").html(price);
                    });
                }
                else {
                    priceCtrl.children(".price").html(price);
                    priceCtrl.show();
                }
            }
        }
        function SetPromoPriceDisplay(haspromo, promoprice, hideprice) {
            if (haspromo && !hideprice) {
                //update promo price display
                promoPriceCtrl.children(".price").html(promoprice);
                promoPriceCtrl.show();
                priceCtrl.css("text-decoration", "line-through");
            }
            else {
                promoPriceCtrl.hide();
                priceCtrl.css("text-decoration", "none");
            }
        }
        function SetStockHintDisplay(showstockhint, showactualstock, freestock, instockimg, outofstockimg) {
            freestock = Math.floor(freestock);
            if (showstockhint) {
                if (showactualstock) { stockHintCtrl.children(".stockhint").html(freestock); }
                else {
                    if (freestock > 0) { stockHintCtrl.html(StringFormat("<img src={0} />", instockimg)); }
                    else { stockHintCtrl.html(StringFormat("<img src={0} />", outofstockimg)); ; }
                }
                stockHintCtrl.show();
            }
            else { stockHintCtrl.hide(); }
        }
        //todo: use caching
        function GetMatrixItemDetails(combination, code, counter) {
            var params = { itemCounter: counter, itemCode: code, matrixCombination: JSON.stringify(combination) }
            var result = ActionService("GetMatrixItemDetails", params, false);

            if (result != null && result != "") {
                stockHintCtrl.show();
                priceCtrl.show();
                promoPriceCtrl.show();
                unitMeasureCtrl.show();

                var settings = $.parseJSON(result);
                //console.log(settings);
                currentitem.UnitMeasures = settings.UnitMeasures;
                currentitem.HidePrice = settings.HidePriceUntilCart;
                currentitem.UnitMeasures = settings.UnitMeasures;
                currentitem.RestrictedQuantity = settings.RestrictedQuantities;
                currentitem.callToOrder = settings.IsCallToOrder;
                currentitem.minOrder = settings.MinimumOrderQuantity;
                currentitem.ShowBuyButton = settings.ShowBuyButton;


                SetPriceDisplay(currentitem.UnitMeasures[0].priceFormatted, currentitem.HidePrice);
                SetPromoPriceDisplay(currentitem.UnitMeasures[0].hasPromotionalPrice, currentitem.UnitMeasures[0].promotionalPriceFormatted, currentitem.HidePrice);
                SetStockHintDisplay(currentitem.ShowStockHint, currentitem.ShowActualStock, currentitem.UnitMeasures[0].freeStock, currentitem.InstockImg, currentitem.OutOfStockImg);
                SetUnitMeasuresDisplay(currentitem.UnitMeasures, currentitem.HideUnitMeasure);
                SetQuantityDisplay(currentitem.RestrictedQuantity);

                $(unitMeasureCtrl.children().children()).change(function () {
                    var index = $(this).prop("selectedIndex");
                    SetPriceDisplay(currentitem.UnitMeasures[index].priceFormatted, currentitem.HidePrice);
                    SetPromoPriceDisplay(currentitem.UnitMeasures[index].hasPromotionalPrice, currentitem.UnitMeasures[index].promotionalPriceFormatted, currentitem.HidePrice);
                    SetStockHintDisplay(currentitem.ShowStockHint, currentitem.ShowActualStock, currentitem.UnitMeasures[index].freeStock, currentitem.InstockImg, currentitem.OutOfStockImg);
                });

                messageBoardCtrl.hide();

                //update addtocart itemcode
                $(addToCtrl).children("input[name=AddToCart]").attr("itemcounter", settings.ItemCounter);

                //switch images of matrix item
                var matrixItemImages = ActionService("GetItemImage", { itemCode: settings.ItemCode }, false);
                var photogallery = $(".photo-gallery[itemcode=" + code + "]");
                $(photogallery).hide(); //hide the main image

                if ($(photogallery).parent().children().length == 1) { $(photogallery).parent().append(matrixItemImages); }
                else { $(photogallery).next().replaceWith(matrixItemImages); }

                //show buy button
                if (currentitem.ShowBuyButton == true) { addToCtrl.show(); } else { addToCtrl.hide(); }

                //call to order
                if (currentitem.callToOrder == true) { ShowCallToOrder(); }
                else { HideCallToOrder(); }
            }
            else {
                stockHintCtrl.hide();
                priceCtrl.hide();
                promoPriceCtrl.hide();
                unitMeasureCtrl.hide();

                var showMessage = true;
                $.each(matrixOptionCtrl.children("select[name='MatrixOpt']"), function () {
                    var index = $(this).prop('selectedIndex');
                    if (index == 0) { showMessage = false; } //hide error message if alteast one of the matrix option is not yet selected
                });
                if (showMessage) {
                    //matrix item not configured
                    messageBoardCtrl.html(messages.MatrixItemNotConfigured);
                    messageBoardCtrl.show();
                }
                else {
                    messageBoardCtrl.html(messages.SelectOption);
                    messageBoardCtrl.show();
                }

                //update addtocart itemcode
                $(addToCtrl).children("input[name=AddToCart]").attr("itemcounter", "");
            }
        }
        function ShowCallToOrder() {
            addToCtrl.hide();
            quantityCtrl.hide();
            unitMeasureCtrl.hide();
            $(addToCtrl).parent().append("<div class='calltoorder'>" + captions.CallToOrder + "</div>");
        }
        function HideCallToOrder() {
            addToCtrl.show();
            quantityCtrl.show();
            unitMeasureCtrl.show();
            $(addToCtrl).next(".calltoorder").remove();
        }
        function ActionService(methodname, params, asyncmode) {
            var result;
            $.ajax({
                type: "POST",
                url: "ActionService.asmx/" + methodname,
                data: JSON.stringify(params),
                dataType: "json",
                async: asyncmode,
                contentType: "application/json; charset=utf-8",
                success: function (message) { result = message.d; },
                error: function (message, textStatus, exception) { return ""; }
            });
            return result;
        }

        this.each(function () {
            base.initCartControl();
        });
        return this;
    }
})(jQuery);

