var _mItemCode       = 0;
var _mItemsOnDisplay = 0;
var _mItemsTotal     = 0;
var _mPageSize       = 0;
var _mImageSize      = "";
var _mQuantityRegExp = "";

$(document).ready(function (){ 
    InitMatrixItemsListing();
    EventsListener();
});

function RenderMatrixItemHTML(jsonText, onload) {

    var lst = $.parseJSON(jsonText);
    var rowNum = 0;

    var _html = "";
    var _nav = "";

    var images = "";
    var imgsource = "";
    var itemCounter = 0;
    var itemCode = "";
    var itemDescription = "";
    var itemHTMLDescription = "";
    var itemName = "";
    var itemStock = "";
    var itemUOM = "";
    var tmpUOM = "";

    var zoomOption = "";
    var stockImage = "";

    var skip = false;

    var _HideOutOfStockProducts = "false";
    var _ShowActualInventory = "false";

    var listing = [];

    for (var i = 0; i < lst.length; i++) {
        
        itemCounter         = lst[i].itemCounter;
        itemCode            = lst[i].itemCode;
        itemDescription     = lst[i].itemDescription;
        itemHTMLDescription = lst[i].itemHTMLDescription;
        itemName            = lst[i].itemName;
        itemStock           = lst[i].itemStock;
        itemUOM             = lst[i].itemUOM;
        itemPrice           = lst[i].itemPrice;
        rowNum              = lst[i].totalNumberOfItems;
        images              = lst[i].images;
        zoomOption          = lst[i].zoomOption;
        _mQuantityRegExp    = lst[i].quantityRegExp;

        /* for default image and item with multiple images --> */

        imgsource = "";
        _nav = "";

        images = images.split("::");
         
        if (images.length > 1 && images[1] != "") {

            _nav = "<div style='multiple-images-nav-place-holder'>";

            for (var j = 1; j <= (images.length - 1); j++) {

                if (images[j - 1] != "") {

                    var source       = images[j - 1].split(" ")[0];
                    var defaultImage = images[j - 1].split(" ")[1].toLowerCase();
                    var selected     = "";

                    if (defaultImage == "true") {

                        imgsource = source;
                        selected = " multiple-image-nav-selected";
                    }

                    var useRollOver = $("#use-roll-over-for-multi-nav").html().toLowerCase();
                    if ($("#use-image-for-multi-nav").html().toLowerCase() == "true") {

                        var img = source.split("/");

                        img = img[img.length - 1];

                        if (useRollOver == "true") {

                            _nav += '<a href="javascript:void(1)\" style="background-image:url(images/product/micro/' + img + ')" onMouseOver="ChangePhotoSource(' + itemCounter + ', \'' + itemCode + '\', \'' + source + '\', \'' + j + '\')" id="img-' + itemCode + '-nav-' + j + '" class="multiple-image-nav' + selected + ' matrix-micro-image"></a>';

                        } else {

                            _nav += '<a href="javascript:void(1)\" style="background-image:url(images/product/micro/' + img + ')" onClick="ChangePhotoSource(' + itemCounter + ', \'' + itemCode + '\', \'' + source + '\', \'' + j + '\')" id="img-' + itemCode + '-nav-' + j + '" class="multiple-image-nav' + selected + ' matrix-micro-image"></a>';

                        }

                    } else {

                        if (useRollOver == "true") {

                            _nav += '<a href="javascript:void(1)\" onMouseOver="ChangePhotoSource(' + itemCounter + ', \'' + itemCode + '\', \'' + source + '\', \'' + j + '\')" id="img-' + itemCode + '-nav-' + j + '" class="multiple-image-nav' + selected + '">' + (j) + '</a>';

                        } else {

                            _nav += '<a href="javascript:void(1)\" onClick="ChangePhotoSource(' + itemCounter + ', \'' + itemCode + '\', \'' + source + '\', \'' + j + '\')" id="img-' + itemCode + '-nav-' + j + '" class="multiple-image-nav' + selected + '">' + (j) + '</a>';

                        }
                    }

                }
            }
            _nav += "</div>";

        } else {
            imgsource = images[0].split(" ")[0];
        }

        /* for default image and item with multiple images <-- */

        if (itemHTMLDescription != "") itemDescription =  itemHTMLDescription;

        skip = false;
        if (_HideOutOfStockProducts == "true" && itemStock <= 0) skip = true;

        if (!skip) { //<-- out of stock items is skipped if HideOutOfStockProducts app config is set to true


           var largeImage = imgsource.split("/");
               largeImage = largeImage[largeImage.length - 1];


           var _itemPhoto  = "<div id='item-photo-" + itemCounter + "-wrapper' style='margin:auto'>";
                    _itemPhoto  += "<a id='item-photo-" + itemCounter + "' data-ZoomOption='" + zoomOption + "' class='cloud-zoom' title='" + itemName + "' href='images/product/large/" + largeImage + "'>";
                    _itemPhoto += "<img data-contentKey='" + itemCode  + "' data-contentType='image' id='image-" + itemCode + "' class='matrix-images content' src='" + imgsource + "'/>";
                    _itemPhoto  += "</a>";
               _itemPhoto   += "</div>";

               listing.push({ MatrixItemCounter                    : itemCounter,
                              MatrixItemCode                       : itemCode,
                              MatrixItemName                       : itemName,
                              MatrixItemDescription                : itemDescription,
                              MatrixItemPrice                      : itemPrice,
                              htmlMatrixItemStock                  : RenderStockImage(_ShowActualInventory, itemStock),
                              htmlUnitOfMeasurement                : RenderUnitMeasuresSelector(itemCode, itemUOM),
                              htmlMultipleImagesLink               : _nav,
                              htmlMatrixItemDefaultPhoto           : _itemPhoto,
                              htmlAddToCartReturnMessagePlaceHolder: '<div class="clear-both" id="' + itemCounter + '-' +  itemCode + '-added-place-holder"></div>'
                           });
        }

        if (!onload) _mItemsOnDisplay++;
    }

    _mItemsTotal = rowNum;

    if (_mItemsOnDisplay > rowNum) _mItemsOnDisplay = rowNum;

    if (rowNum != 0) {

        $("#page-items-place-holder").html("Showing " + _mItemsOnDisplay + " of " + rowNum);
        $("#page-size-all").val(rowNum);
    }

    if(rowNum <= _mPageSize) $("#matrix-items-bottom-controls-wrapper").fadeOut("slow");

    _html = $("#matrixItemsTemplate").tmpl(listing);
    return _html;
}

function splitClass(id, separator) {
    return $("#" + id).attr("class").split(separator);
}

function InitMatrixItemsListing() {

    var params = splitClass("matrix-items-wrapper", "::");

    _mPageSize       = params[0];
    _mItemsOnDisplay = params[0];
    _mItemCode       = params[1];
    _mImageSize      = params[2];

    GetMatrixItems(_mPageSize, 0, true);
}

function EventsListener() {

    $("#view-more").click(function () {

        $("#ise-message-tips").fadeOut("slow");

        var params = splitClass("view-more", "::");
        GetMatrixItems(params[1], params[2], false);
    });

    $("#page-size").change(function () {

        $("#ise-message-tips").fadeOut("slow");

        var selected = $(this).val();

        if (selected > _mItemsTotal && _mItemsTotal != 0) selected = _mItemsTotal;
        _mItemsOnDisplay = selected;

        GetMatrixItems(selected, 0, true);

        var thisClass = $("#view-more").attr("class");

        $("#view-more").removeClass(thisClass);
        $("#view-more").addClass(_mItemCode + "::" + selected + "::1");
    });
}


function AddMatrixItemToCart(itemCounter, itemCode) {
    var qty = $("#qty-" + itemCode).val();
    var uom = $("#uom-" + itemCode).val();

    if (!qty.match(_mQuantityRegExp)) {

        ShowBubbleTips("#qty-" + itemCode, $("#quantity-format").html());
        $("#qty-" + itemCode).addClass("invalid-quantity");

        return false;
    } else {

        $("#ise-message-tips").fadeOut("slow");
        $("#qty-" + itemCode).removeClass("invalid-quantity");
    }

    if (qty <= 0 || qty == "") {

        ShowBubbleTips("#qty-" + itemCode, $("#quantity-zero").html());
        $("#qty-" + itemCode).addClass("invalid-quantity");

        return false;
    } else {

        $("#ise-message-tips").fadeOut("slow");
        $("#qty-" + itemCode).removeClass("invalid-quantity");
    }

    var jsonText = JSON.stringify({ "counter": itemCounter, "itemCode": itemCode, "quantity": qty, "uom": uom, "typeOfCart": "shoppingcart" });

    var stay = $("#stay-on-page-after-add-to-cart").html();
    stay = $.trim(stay.toLowerCase());

    if (stay == "stay") {

        var loading = '<div id="message-loader" style="width:200px;"><div style="float:left; padding-right:5px;"><img src="images/ajax-loader.gif" style="width:24px;height:24px"></div><div style="float:left;font-size:11px;font-weight:bold;position:relative;top:-2px;">Adding <br/>this item...</div></div>';
        $("#" + itemCounter + "-" + itemCode + "-button-place-holder").html(loading);

    }

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/InsertItemToCart",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            var _msg = result.d;
            _msg = _msg.split("::");

            if (_msg[0] == "failed") {

                var message = _msg[1].replace("\\n", "\n");

                ShowBubbleTips("#qty-" + itemCode, message);
                $("#qty-" + itemCode).addClass("invalid-quantity");

            } else {

                if (stay == "stay") {

                    var added  = '<div class="float-right"><div style="padding-right:5px;" class="float-right"><img src="images/check-box.png" id="item-added-check-image"></div><div id="item-added-message" class="float-left"><a href="shoppingcart.aspx">Item added successfully</a></div></div>';
                    var button = '<input type="button" onclick="AddMatrixItemToCart(' + itemCounter + ', \'' + itemCode + '\')" class="site-button" value="Add To Cart">';

                    $("#" + itemCounter + "-" + itemCode + "-button-place-holder").html(button).delay();
                    $("#" + itemCounter + "-" + itemCode + "-added-place-holder").html(added).delay();

                } else {

                    window.location = "shoppingcart.aspx";
                }
            }
        },
        fail: function (result) {
            $("#errorSummary").html("<div id='errorSummary_Board'><ul id='errorSummary_Board_Errors'><li>" + result.d + "</li></ul></div>");
            $("#errorSummary").fadeIn("slow");
            $("#error-summary-clear").css("display", "");
        }
    });
}

function GetMatrixItems(pageSize, currentPage, onload) {

    currentPage++;

    var currentClass = _mItemCode + "::" + pageSize + "::" + (currentPage - 1);
    var newClass     = _mItemCode + "::" + pageSize + "::" + currentPage;

    var jsonText = JSON.stringify({ "itemCode": _mItemCode, "pageSize": pageSize, "pageNumber": currentPage, "imageSize": _mImageSize });

    $("#matrix-page-loading-indicator").html("<div id='message-loader' style='padding-left:30px;'><div class='clear-both height-12'></div><div style='float:left; padding-right:12px;'><img src='images/ajax-loader.gif'></div><div style='float:left;font-size:14px;font-weight:bold;position:relative;top:7px;'>Loading Matrix Items...</div></div>");
    $("#matrix-page-loading-indicator").slideDown("slow");

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetMatrixGroupItems",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            _html = RenderMatrixItemHTML(result.d, onload);

            if (onload) {

                $("#matrix-page-loading-indicator").fadeOut("slow", function () {

                    $("#matrix-items-wrapper").html(_html);
                    $("#matrix-items-wrapper").fadeIn("slow", function(){
                        InitImageZoom();
                     });

                });

            } else {

                $("#matrix-page-loading-indicator").slideUp("slow", function () { 

                    $("#matrix-items-wrapper").append(_html);
                    InitImageZoom(); 
                });

                $("#view-more").removeClass(currentClass);
                $("#view-more").addClass(newClass);
            }
        },
        fail: function (result) {
            $("#errorSummary").html("<div id='errorSummary_Board'><ul id='errorSummary_Board_Errors'><li>" + result.d + "</li></ul></div>");
            $("#errorSummary").fadeIn("slow");
            $("#error-summary-clear").css("display", "");
        }
    });

}

function RenderUnitMeasuresSelector(itemCode, itemUOM) {

    var selector = "<SELECT id='uom-" + itemCode + "' class='light-style-input'>";
    var unitMeasurements = itemUOM.split("::");

    for (var i = 0; i < unitMeasurements.length; i++) {

        if (unitMeasurements[i] != "") {
            var uom = unitMeasurements[i]
            uom = uom.split(":-:");
            selector += "<option value='" + uom[0] + "'>"  + uom[1] + "</option>";
        }
    }

    selector += "</SELECT>";
    return selector;
}

function RenderStockImage(_ShowActualInventory, itemStock) {

    var stockImage = "<img src= 'images/outofstock.png'/>";

    if (_ShowActualInventory == "true") {
        stockImage = itemStock;
    } else {
        if (itemStock > 0) stockImage = "<img  src= 'images/instock.png'/>";
    }

    return stockImage;
}

function RemoveInvalidQuantityMessage(id) {

    $("#ise-message-tips").fadeOut("slow");
    $("#qty-" + id).removeClass("invalid-quantity");
}

function InitImageZoom() {

    $(".cloud-zoom").each(function () {
        VerifyPhoto($(this).attr("href"), $(this).attr("id"),  false, $(this).attr("data-ZoomOption"), "");
    });
}

function ChangePhotoSource(itemCounter, itemCode, source, counter) {

   var id         = "item-photo-" + itemCounter;
   var zoomOption = $("#" + id).attr("data-ZoomOption");
   var itemName   = $("#" + id).attr("title");

   $("#image-" + itemCode).attr("src", source);

   $(".multiple-image-nav").removeClass("multiple-image-nav-selected");
   $("#img-" + itemCode + "-nav-" + counter).addClass("multiple-image-nav-selected");

   var img = source.split("/");
   img = img[img.length - 1];

   $("#" + id).attr("href", "images/product/large/" + img);

   var  _itemPhoto = "<a id='item-photo-" + itemCounter + "' data-ZoomOption='" + zoomOption + "' class='cloud-zoom' title='" + itemName + "' href='images/product/large/" + img + "'>";
                 _itemPhoto += "<img id='image-" + itemCode + "' class='matrix-images' src='" + source + "'/>";
          _itemPhoto += "</a>";

   VerifyPhoto("images/product/large/" + img, id, true, zoomOption, _itemPhoto);

}

function VerifyPhoto(src, id, onClick, zoomOption, _itemPhoto) {

    var img = new Image();

    img.onload = function () {
        setZoomOptions(id, zoomOption);
    };
    img.onerror = function () {

        if (onClick) $("#" + id + "-wrapper").html(_itemPhoto);
        setZoomOptions(id, "popup");

    };

    img.src = src; // fires off loading of image
}

function setZoomOptions(id, zoomOption){
    
    if (zoomOption == null)  return;

    var link = $("#" + id);

    switch (zoomOption.toLowerCase()) {
        case 'lens zoom': //lens zoom

            link.attr("rel", "adjustX: 10, adjustY:-4" + getImageZoomLensSize());

            $('.mousetrap').remove();
            $(".cloud-zoom").CloudZoom();

            break;
        case 'lens blur': //lens blur
             
            link.attr("rel", "tint: '#FF9933',tintOpacity:0.5 ,smoothMove:5, adjustY:-4, adjustX:10" + getImageZoomLensSize());

            $('.mousetrap').remove();
            $(".cloud-zoom").CloudZoom();


            break;
        case 'inner zoom': //inner zoomOption

            link.attr("rel", "position: 'inside' , showTitle: false, adjustX:-4, adjustY:-4");

            $('.mousetrap').remove();
            $(".cloud-zoom").CloudZoom();

            break;
        case 'blur focus': //blur focus

            link.attr("rel", "softFocus: true, smoothMove:2, adjustX: 10, adjustY:-4" + getImageZoomLensSize());
           
            $('.mousetrap').remove();
            $(".cloud-zoom").CloudZoom();

            break;
        case 'zoom out': //zoom out
                
            link.fancybox({
                'overlayShow': false,
                'transitionIn': 'elastic',
                'transitionOut': 'elastic'
                });

            break;
        case 'popup': //popout

            link.fancybox({
                'titlePosition': 'inside',
                'transitionIn': 'none',
                'transitionOut': 'fade'
                });

            break;
        default:

            link.fancybox({
                'titlePosition': 'inside',
                'transitionIn': 'none',
                'transitionOut': 'fade'
                });

            break;
    }
}

function getImageZoomLensSize() {

    var imageZoomLensWidth  = ise.Configuration.getConfigValue("ImageZoomLensWidth");
    var imageZoomLensHeight = ise.Configuration.getConfigValue("ImageZoomLensHeight");
    var lensZoomSize = '';

    if (imageZoomLensHeight > 0) {
        lensZoomSize += ",zoomHeight:" + imageZoomLensHeight;
    }

    if (imageZoomLensWidth > 0) {
        lensZoomSize += ",zoomWidth:" + imageZoomLensWidth;
    }
        
    return lensZoomSize;
 }
