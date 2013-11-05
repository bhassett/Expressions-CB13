$.template(
        "shippingMethodOptionTemplateID",
        "<div>" +
            "<input id='${id}' type='radio' name='${name}' value='${description}' ${isDefault} ${disabled}>" +
            "<span>" +
                "<span id='shipping-description$" + "${id}' class='shipping-description' style='cursor: pointer;'> ${description} </span>" +
                "<span> ${freight} </span>" +
            "</span>" +
        "</div>"
    );