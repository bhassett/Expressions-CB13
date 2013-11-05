
$('document').ready(function () {

    // -> Add to cart button click event handler

    $('#add-to-cart').click(function () {

        var itemCode = $('input[name=product-option]:checked').val();
        var quantity = $("#quantity").val();
        var uom = $("#uom-options").val();

        if (uom == null || typeof uom == undefined) {
            uom = $("#uom-container-code").html();
        }

        var message = matrixFormIsGood(quantity, itemCode);

        if (message == true) {
            var str = itemCode.split("::");
            var code = $.trim(str[0]);
            var counter = $.trim(str[1]);
            insertItemToCart(counter, code, quantity, uom, "shopping-cart");
        } else {
            alert(message);
        }

    });

    // <- Add to cart


    // -> Add to wish list button click event handler

    $('#add-to-wish-list').click(function () {

        var itemCode = $('input[name=product-option]:checked').val();
        var quantity = $("#quantity").val();
        var uom = $("#uom-options").val();

        if (uom == null || typeof uom == undefined) {
            uom = $("#uom-container-code").html();
        }

        var message = matrixFormIsGood(quantity, itemCode);

        if (message == true) {
            var str = itemCode.split("::");
            var code = $.trim(str[0]);
            var counter = $.trim(str[1]);
            insertItemToCart(counter, code, quantity, uom, "wish-list");
        } else {
            alert(message);
        }

    });

    // <- Add to wish list

});

function matrixFormIsGood(quantity, selection) {

    var quantityFormat = $("#quantity-format").html();
    var quantityZero   = $("#quantity-zero").html();
    var selectedNone = $("#selected-none").html();

    if (selection == null) {
        return selectedNone;
    }

    if (quantity <= 0 || quantity == "") {
        return quantityZero;
    }

    var quantityRegExp = $("#quantity-reg-ex").html();
    quantityRegExp = $.trim(quantityRegExp);

    if (!quantity.match(quantityRegExp)) {
        return quantityFormat;
    }

    return true;
}

function getProductOptionImage(item) {
    
    var param = item;
    param = param.split("::");

    $("#option-" + param[0]).attr('checked', true);
    $(".product-image-for-matrix-options").attr("src", param[1]);

    var unitOfMeasurements = param[2];
    unitOfMeasurements = unitOfMeasurements.split(",");
    var uomLength = unitOfMeasurements.length - 1;
    var uomHTML = "";

    var uom = "";

    if (uomLength == 1) {

        uom = unitOfMeasurements[0].split("+");
        uomHTML = uom[1];
        uomHTML += "<div id='uom-container-code' style='display:none'>" + uom[0] + "</div>";

    } else {

        uomHTML  = "<SELECT id='uom-options'>";
        
        for (var i = 0; i < uomLength; i++) {
            uom = unitOfMeasurements[i].split("+");
            uomHTML += "<option value = '" + uom[0] + "' >" + uom[1] + "</option>";
        }

        uomHTML += "</SELECT>";

    }

    $("#uom-container").html(uomHTML);

}

function insertItemToCart(counter, itemCode, quantity, unitOfMeasurement, typeOfCart) {

    var jsonText = JSON.stringify({ "counter": counter, "itemCode": itemCode, "quantity": quantity, "uom": unitOfMeasurement, "typeOfCart": typeOfCart });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/InsertItemToCart",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            var message = result.d.split("::");
            if (message[0] == "failed") {
                alert(message[1].replace("\\n", "\n"));
            } else {

                if (typeOfCart == "shopping-cart") {
                    window.location = "shoppingcart.aspx";
                } else {
                    window.location = "wishlist.aspx";
                }
            }
        },
        fail: function (result) {
            alert(result.Message);
        }
    });

}
