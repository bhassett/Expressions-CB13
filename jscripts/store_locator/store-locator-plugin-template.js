$.template(
        "headerTextTemplate",
        "<h1>${OrderCount} ${Text} ${OtherText}</h1>"
    );

$.template(
        "storeMenuTemplate",
        "<li><a href='javascript:void(0);'>" +
            "<div class='store-address-info'>" +
                "<span class='store-address-info-address'>${Title}</span>" +
                "<br />" +
                "<span class='store-address-info-address-detail'>${Address.address}</span>" +
                "<br />" +
                "<span class='store-address-info-address-detail'>${Address.city}, ${Address.state} ${Address.postal}</span>" +
                "<br />" +
                "<span class='store-address-info-address-detail'>${Address.country}</span>" +
            "</div>" +
            "</a>" +
        "</li>"
    );

$.template(
        "infoWindowTemplate",
        "<div>" +
            "<div class='store-infowindow'>" +
                "<span class='store-address-info-address'>${Title}</span>" +
                "<br />" +
                "<span class='store-address-info-address-detail'>${Address.address}</span>" +
                "<br />" +
                "<span class='store-address-info-address-detail'>${Address.city}, ${Address.state} ${Address.postal}</span>" +
                "<br />" +
                "<span class='store-address-info-address-detail'>${Address.country}</span>" +
                "<br />" +
                "<div class='store-address-direction-main'>" +
                    "<a href='javascript:void(0);' id='${Id}' class='store-infowindow-direction-link'>${Captions.MESSAGE_DEFAULT_DIRECTION_LINK_TEXT}</a>" +
                "</div>" +
            "</div>" +
        "</div>"
    );

$.template(
        "addressInfoWindowTemplate",
        "<div class='store-address-info'>" +
            "<span class='store-address-info-address'>${Title}</span>" +
        "</div>"
    );