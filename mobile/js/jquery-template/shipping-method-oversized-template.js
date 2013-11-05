$.template("shippingMethodOversizedTemplateID",
            "<br/>" +
            "<div  class='oversized-container'>" +
                "<div class='oversized-message border oversized-row no-padding-margin group'>" +
                    "<p class='oversized-row-margin'> ${stringResource.oversizedMessage} </p> " +
                "</div>" +

                "<div class='oversized-header-container oversized-row no-padding-margin group'>" +
                        "<div class='col'> <p class='oversized-row-header'> ${stringResource.productHeader} </p> </div>" +
                        "<div class='col'> <p class='oversized-row-header'> ${stringResource.shippingMethodHeader} </p> </div>" +
                        "<div class='col'> <p class='oversized-row-header'> ${stringResource.freightHeader} </p> </div>" +
                    "</div>" +

                "{{each shippingMethodInfo}}" +
                    "<div class='oversized-row no-padding-margin group'>" +
                        "<div class='col'> <p class='oversized-row-margin'> ${$value.ItemDescription} </p> </div>" +
                        "<div class='col'> <p class='oversized-row-margin'> ${$value.ShippingDescription} </p> </div>" +
                        "<div class='col'> <p class='oversized-row-margin oversized-row-freight-alignment'> ${$value.Freight} </p> </div>" +
                    "</div>" +
                    "<div class='clear-both no-padding-margin'></div>" +
                "{{/each}}" +
            "<div class='clear-both no-padding-margin'></div>" +
            "</div>"
);
