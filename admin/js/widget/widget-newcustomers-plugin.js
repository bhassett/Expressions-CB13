(function ($) {
    var defaults = { WidgetID: "", WidgetContentClass: ".content", WidgetContentMaxHeight: "100%" };
    var widgetConstants = {
        EMPTY_VALUE: '',
        DOT_VALUE: '.'
    }

    var config = {};
    var thisObject;

    $.fn.NewCustomersWidget = {

        initialize: function (options) {
            if (options) { setConfig($.extend(defaults, options)); }

            var config = getConfig();

            this.attachEvents();
            thisObject = this;

            thisObject.displayLoader();
            thisObject.displayContent();
        },

        displayLoader: function () {
            var config = getConfig();
            var content = $(selectorChecker(config.WidgetID)).children(selectorChecker(config.WidgetContentClass));
            content.html(parseTemplate("widget-loader", ""));
        },

        displayContent: function () {
            var config = getConfig();
            var content = $(selectorChecker(config.WidgetID)).children(selectorChecker(config.WidgetContentClass));

            var table = 'table' + config.WidgetID;
            var object = new Object();
            object.displayLimit = 10; //display limit for customers

            var jsonData = JSON.stringify(object);
            $.ajax({
                type: "POST",
                url: '../ActionService.asmx/GetNewCustomers',
                data: jsonData,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    var jsonResult = result.d;
                    if (jsonResult != '') {
                        var customers = $.parseJSON(jsonResult);

                        window.setTimeout(function () {
                            $(content).empty();

                            if (customers.length > 0) {
                                $(content).append(parseTemplate("newcustomers-body", { ID: table, CustomerHeader: "Customer", RegisteredHeader: "Date Registered", CustomerCodeHeader: "CustomerCode" }));
                                $(selectorChecker(table)).append(parseTemplate("newcustomers-content", customers));
                                $(content).css("max-height", config.WidgetContentMaxHeight);
                                $(selectorChecker(config.WidgetID) + " .content table tbody tr:odd").attr("class", "alternate");
                            }
                            else {
                                $(content).append(parseTemplate("no-items", { Message: "no items found" }));
                                $(selectorChecker(config.WidgetID) + " a[widget-control='minimize']").trigger("click");
                            }
                        }, config.WidgetDelay);

                    }
                },
                error: function (result, textStatus, exception) {
                    console.log(exception);
                }
            });
        },

        attachEvents: function () {
            var config = getConfig();

            //refresh widget
            $(selectorChecker(config.WidgetID) + " a[widget-control='refresh']").click(function () {
                thisObject.refresh();
            });

            //show or hide widget
            $(selectorChecker(config.WidgetID) + " a[widget-control='minimize']").click(function () {
                var config = getConfig();
                var content = $(selectorChecker(config.WidgetID)).children(selectorChecker(config.WidgetContentClass));
                var isVisible = $(content).is(":visible");

                if (isVisible) {
                    $(this).attr("class", "icon-white icon-chevron-down");
                    $(this).attr("title", "Maximize");

                    //curve all corners of header
                    $(content).siblings(".widget-header").css("border-radius", "4px");
                    $(content).siblings(".widget-header").css("-webkit-border-radius", "4px");
                    $(content).siblings(".widget-header").css("-moz-border-radius", "4px");
                }
                else {
                    $(this).attr("class", "icon-white icon-chevron-up");
                    $(this).attr("title", "Minimize");

                    //curve top corners of header
                    $(content).siblings(".widget-header").css("border-radius", "4px 4px 0px 0px");
                    $(content).siblings(".widget-header").css("-webkit-border-radius", "4px 4px 0px 0px");
                    $(content).siblings(".widget-header").css("-moz-border-radius", "4px 4px 0px 0px");
                }

                $(content).toggle();
            });
        },

        refresh: function () {
            thisObject.displayLoader();
            thisObject.displayContent();
        }
    };

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

    function selectorChecker(selector) {
        if (selector == widgetConstants.EMPTY_VALUE) return selector;

        if (selector.indexOf(widgetConstants.DOT_VALUE) == -1) {
            selector = "#" + selector;
        }
        return selector;
    }

    function parseTemplate(templateId, data) {
        return $.tmpl(templateId, data);
    }

    function parseTemplateReturnHtml(templateId, data) {
        return $(parseTemplate(templateId, data)).html();
    }

})(jQuery);