deleteCookie('cartisopen');
$('#shopping-cart').click(function () {

    if ($(this).hasClass("page-is-on-edit-mode")) return false;

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/BuildMiniCart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (getCookie('cartisopen') == null || getCookie('cartisopen') == "") {
                $("#mini-cart").html(result.d);
                $("#mini-cart").slideDown("fast");
                minicartScroller();

                if ($("#vatEnabled").val() == 'true') {
                    if ($("#vatSetting").val() == 2) {
                        var divHeight = ($("#SubTotal").height());
                        $("#SubTotal").css("height", divHeight + 10 + "px");
                    }
                }

                $('.minicartImage').bind("click", function () {
                    GetAccessoryItemForMinicart(this.id);
                });

                setCookie('cartisopen', 'open');
            }
            else {
                $("#mini-cart").slideUp("fast");
                deleteCookie('cartisopen');
            }

        },
        fail: function (result) {

        }
    });
});

function GetAccessoryItemForMinicart(id) {

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetAccessoryItemForMinicart",
        data: '{"counter":' + id + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            $("#minicart-accessory-panel").html(result.d);
        },
        fail: function (result) {

        }
    });
}

function updateCart() {

    var itemsarray = [];
    var accitemsarray = [];

    $('.qtyLineItem').each(function (index) {
        var qty = $('#Quantity_' + this.id).val();
        itemsarray.push(this.id + ':' + qty);
    });

    $('.chkAccItem').each(function (index) {
        if ($('#chkcom_' + this.id).attr('checked')) {
            accitemsarray.push(this.id);
        }
    });

    AjaxCallWithSecuritySimplified(
        "UpdateCart",
        { "qtyArray":  itemsarray, "chkArray": accitemsarray },
        function (result) {
            reloadMinicart();
        }
    );
};

function removeItem(itemCounter) {

    AjaxCallWithSecuritySimplified(
        "RemoveMiniCartItem",
        { "cartRecordID": itemCounter },
        function (result) {
            reloadMinicart();
        }
    );

};

function goToPayPalCheckout() {

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/RedirectToPayPalCheckoutMinicart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            window.location = result.d;
        }
    });
}

function goToGoogleCheckout() {

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/RedirectToGoogleCheckoutMinicart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            window.location = result.d;
        }
    });
}

function reloadMinicart() {

    $('#mini-cart').children().remove();

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/BuildMiniCart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.d.length != 0) {
                $("#mini-cart").html(result.d);

                $('.minicartImage').bind("click", function () {
                    GetAccessoryItemForMinicart(this.id);
                });

                reloadShoppingCartNumber();
                minicartScroller();
            }
            else {
                reloadShoppingCartNumber();
                $("#mini-cart").hide();
            }
        }
    });
};

function reloadShoppingCartNumber() {
    $('#shopping-cart').children().remove();

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/ShoppingCartNumber",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            $("#shopping-cart").html(result.d);
        }
    });
};

function minicartScroller() {
    var listLimit = $("#NumMiniCartItems").val();
    var listCounts = $("ul#minicartItems > li").length;

    var rowHeight = 0;
    
    if ($("#vatEnabled").val() == 'true') {
        if ($("#vatSetting").val() == 2) {
            rowHeight = 120;
        }
        else {
            rowHeight = 99;
        }
    }
    else {
        rowHeight = 99;
    }

    var d = 0;
    var ulTopMargin = 0;

    var divHeight = listLimit * rowHeight;

    var x = listCounts - listLimit;

    if (listCounts > listLimit) {
        $("#mbTm").css("display", "block");
        $("#miniCartLineItem").css("height", divHeight + "px");
    }

    $("#mbTm").bind("click", function () {

        d = d + 1;
        ulTopMargin = ulTopMargin - rowHeight;

        $("#minicartItems").animate({
            marginTop: ulTopMargin
        }, 500);

        if (d == x) {

            $("#mbTm").css("display", "none");

        }

        $("#mbTop").css("display", "block");

    });

    $("#mbTop").bind("click", function () {

        d = d - 1;
        ulTopMargin = ulTopMargin + rowHeight

        $("#minicartItems").animate({
            marginTop: ulTopMargin
        }, 500);

        if (d == 0) {

            $("#mbTop").css("display", "none");

        }

        $("#mbTm").css("display", "block");

        return false;
    });
}

function setCookie(name, value) {
    document.cookie = name + "=" + value + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function deleteCookie(name) {
    setCookie(name, "");
}
