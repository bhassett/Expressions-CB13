(function ($) {
    var thisOrderHistory;
    var config = {};

    var global = {
        selected: "",
        selector: ""
    };

    var constants = {
        DOT_VALUE: ".",
        EMPTY_VALUE: ""
    };

    var defaults = {
        accountOrderHistoryLinkId: "accountOrderHistoryLink",
        orderHistoryContentId: "orderHistoryContent",
        orderHistoryDisplayRangeId: "orderHistoryDisplayRange",
        orderHistoryLinkId: "lnkOrderHistory",
        orderHistoryPagerId: "pager",
        orderHistoryPlaceHolderId: "pnlOrderHistory",
        pagingHeaderId: "pagingHeader",
        resetLinkId: "reset",
        orderHistoryCssPath: "./components/order-history/css/index.css",
        stringResourceKeys: ""
    };

    var constantClassName = {
        LINK_REORDER: ".lnkReOrder",
        PAGE_NUMBER: ".order-history-page-number"
    };

    var inputControlsId = {
        DISPLAY_PAGE_TEXT: "txtPages"
    };

    var serviceMethods = {
        GET_STRING_RESOURCES: "GetStringResources"
    };

    var appConfigs = {
        IS_SHOW_CUSTOMER_SERVICE_NOTES_IN_RECEIPTS: true,
        ORDER_HISTORY_DEFAULT_PAGE_COUNT: 0
    };

    var stringResource = {
        DISPLAY_TEXT: constants.EMPTY_VALUE,
        OF_TEXT: constants.EMPTY_VALUE,
        ON_TEXT: constants.EMPTY_VALUE,
        ORDER_NUMBER: constants.EMPTY_VALUE,
        REORDER: constants.EMPTY_VALUE,
        REORDER_PROMPT: constants.EMPTY_VALUE,
        RESET_TEXT: constants.EMPTY_VALUE,
        SHIPPED_TEXT: constants.EMPTY_VALUE,
        SHIPPING_STATUS: constants.EMPTY_VALUE,
        TRACKING_NUMBER: constants.EMPTY_VALUE,
        VIEWING: constants.EMPTY_VALUE
    };

    var constantsTemplateID = {
        ORDER_HISTORY_CONTENT_TEMPLATE: "orderHistoryContentTemplate",
        ORDER_HISTORY_PAGER_TEMPLATE: "orderHistoryPager",
        ORDER_HISTORY_PAGINATION_TEMPLATE: "orderHistoryPaginationTemplate",
        ORDER_HISTORY_TEMPLATE: "orderHistoryTemplate",
        ORDER_HISTORY_RANGE_TEMPLATE: "orderHistoryRangeTemplate"
    };

    var init = $.prototype.init;
    $.prototype.init = function (selector, context) {
        var r = init.apply(this, arguments);
        if (selector && selector.selector) {
            r.context = selector.context, r.selector = selector.selector;
        }
        if (typeof selector == "string") {
            r.context = context || document, r.selector = selector;
            global.selector = r.selector;
        }
        global.selected = r;
        return r;
    }

    $.prototype.init.prototype = $.prototype;

    $.fn.OrderHistory = {
        setup: function (options) {
            setConfig($.extend(defaults, options));
            thisOrderHistory = this;

            thisOrderHistory.downloadCss(config.orderHistoryCssPath, function () { });

            var callback = function () {
                thisOrderHistory.setStringResourcesValue(config.stringResourceKeys);
                thisOrderHistory.setAppConfigsValue();
                thisOrderHistory.attachEventsListener();
            }

            thisOrderHistory.downloadAppConfigs(thisOrderHistory.getRequiredAppConfigKeys(), callback);
            thisOrderHistory.downloadStringResources(thisOrderHistory.convertObjectToArray(config.stringResourceKeys), callback);

        },

        getRequiredAppConfigKeys: function () {

            var keys = new Array();

            keys.push("ShowCustomerServiceNotesInReceipts");
            keys.push("OrderHistoryDefaultPageCount");

            return keys;
        },

        setStringResourcesValue: function (stringResourceKeys) {
            stringResource.DISPLAY_TEXT = thisOrderHistory.getString(stringResourceKeys.displayText);
            stringResource.OF_TEXT = thisOrderHistory.getString(stringResourceKeys.ofText);
            stringResource.ON_TEXT = thisOrderHistory.getString(stringResourceKeys.onText);
            stringResource.ORDER_NUMBER = thisOrderHistory.getString(stringResourceKeys.orderNumber);
            stringResource.REORDER = thisOrderHistory.getString(stringResourceKeys.reorder);
            stringResource.REORDER_PROMPT = thisOrderHistory.getString(stringResourceKeys.reorderPrompt);
            stringResource.RESET_TEXT = thisOrderHistory.getString(stringResourceKeys.resetText);
            stringResource.SHIPPED_TEXT = thisOrderHistory.getString(stringResourceKeys.shippedText);
            stringResource.SHIPPING_STATUS = thisOrderHistory.getString(stringResourceKeys.shippingStatus);
            stringResource.TRACKING_NUMBER = thisOrderHistory.getString(stringResourceKeys.trackingNumber);
            stringResource.VIEWING = thisOrderHistory.getString(stringResourceKeys.viewing);
        },

        setAppConfigsValue: function () {
            appConfigs.IS_SHOW_CUSTOMER_SERVICE_NOTES_IN_RECEIPTS = thisOrderHistory.toBoolean(ise.Configuration.getConfigValue("ShowCustomerServiceNotesInReceipts"));
            appConfigs.ORDER_HISTORY_DEFAULT_PAGE_COUNT = ise.Configuration.getConfigValue("OrderHistoryDefaultPageCount");
        },

        renderOrderHistoryHeader: function () {
            var resources = new Object();

            resources.orderNumber = stringResource.ORDER_NUMBER;
            resources.shippingStatus = stringResource.SHIPPING_STATUS;

            var content = new Object();
            content.stringResource = resources;

            var orderHistoryHeadersContent = thisOrderHistory.parseJqueryTemplate(constantsTemplateID.ORDER_HISTORY_TEMPLATE, content);

            thisOrderHistory.setElementHTMLContent(defaults.orderHistoryPlaceHolderId, orderHistoryHeadersContent);

            resources = null;
            content = null;
        },

        attachEventsListener: function () {

            var config = getConfig();
            var thisObject = this;

            $(this.selectorChecker(config.orderHistoryLinkId)).unbind("click").click(function () {
                thisOrderHistory.renderOrderHistoryHeader();

                var pageCount = appConfigs.ORDER_HISTORY_DEFAULT_PAGE_COUNT;
                thisOrderHistory.renderOrderHistoryContent(pageCount, 1);

            });
        },

        renderOrderHistoryContent: function (pageCount, currentpage) {
            var callBack = function (result) {

                var ordersData = result.d;

                var orderHistoryDetails = new Object();
                orderHistoryDetails.salesOrder = ordersData.orders;

                var displayText = new Object();
                displayText.reorderCaption = stringResource.REORDER;
                displayText.paymentMethodCaption = stringResource.PAYMENT_METHOD;
                displayText.shippedCaption = stringResource.SHIPPED_TEXT;
                displayText.onCaption = stringResource.ON_TEXT;
                displayText.trackingNumberCaption = stringResource.TRACKING_NUMBER;

                orderHistoryDetails.stringResource = displayText;
                reOrderString = null;

                var orderHistoryContent = thisOrderHistory.parseJqueryTemplate(constantsTemplateID.ORDER_HISTORY_CONTENT_TEMPLATE, orderHistoryDetails);

                thisOrderHistory.setElementHTMLContent(config.orderHistoryContentId, $(orderHistoryContent).html());
                thisOrderHistory.hideAccountOrderHistoryLink();

                thisOrderHistory.displayOrderHistoryRange(ordersData.rows, ordersData.start, ordersData.end, ordersData.current);

                thisOrderHistory.displayOrderHistoryPager(ordersData.rows, ordersData.pages, ordersData.current);

                $(thisOrderHistory.selectorChecker(config.resetLinkId)).unbind("click").click(function () {

                    var rowToDisplay = $(thisOrderHistory.selectorChecker(inputControlsId.DISPLAY_PAGE_TEXT)).val();

                    callback = function () {
                        var txtPageCountDisplay = new Object();
                        txtPageCountDisplay.count = rowToDisplay;

                        var pagesContent = thisOrderHistory.parseJqueryTemplate(constantsTemplateID.ORDER_HISTORY_PAGER_TEMPLATE, pageDisplayContent);
                        thisOrderHistory.setElementHTMLContent(config.orderHistoryPagerId, pagesContent);
                    }

                    thisOrderHistory.getWebOrder(rowToDisplay, 1, callBack);
                });

                $(thisOrderHistory.selectorChecker(constantClassName.LINK_REORDER)).unbind("click").click(function () {

                    thisOrderHistory.displayPrompt(this);
                });
            }
            thisOrderHistory.getWebOrder(pageCount, 1, callBack);
        },

        displayPrompt: function (link) {
            var prompt = stringResource.REORDER_PROMPT;
            if (confirm(prompt)) {
                var code = $(link).attr("data-orderNumber");
                $(link).attr("href", 'reorder.aspx?so=' + code);
            }
        },

        displayOrderHistoryRange: function (rows, start, end, current) {

            var displayRangeContent = new Object();
            displayRangeContent.rows = rows;
            displayRangeContent.start = start;
            displayRangeContent.end = end;
            displayRangeContent.current = current;

            var displayRangeCaptions = new Object();
            displayRangeCaptions.viewing = stringResource.VIEWING;
            displayRangeCaptions.ofText = stringResource.OF_TEXT;

            displayRangeContent.stringResource = displayRangeCaptions;
            displayRangeCaptions = null;

            var orderRangeContent = thisOrderHistory.parseJqueryTemplate(constantsTemplateID.ORDER_HISTORY_RANGE_TEMPLATE, displayRangeContent);
            thisOrderHistory.setElementHTMLContent(config.orderHistoryDisplayRangeId, orderRangeContent);
        },

        displayOrderHistoryPager: function (allPages, rowsPerPage, current) {

            var pageDisplayContent = new Object()
            pageDisplayContent.count = rowsPerPage;

            var pageCountCaption = new Object();
            pageCountCaption.display = stringResource.DISPLAY_TEXT;
            pageCountCaption.reset = stringResource.RESET_TEXT;

            pageDisplayContent.stringResource = pageCountCaption;
            pageCountCaption = null;

            var pagesContent = thisOrderHistory.parseJqueryTemplate(constantsTemplateID.ORDER_HISTORY_PAGER_TEMPLATE, pageDisplayContent);
            thisOrderHistory.setElementHTMLContent(config.orderHistoryPagerId, pagesContent);
            thisOrderHistory.pagination(allPages, rowsPerPage, current);

            $(thisOrderHistory.selectorChecker(constantClassName.PAGE_NUMBER)).unbind("click").click(function () {
                var current = $(this).attr("data-pagenumber");
                var allPages = $(this).attr("data-allPages");

                var callBack = function (result) {

                    var ordersData = result.d;

                    var orderHistoryDetails = new Object();
                    orderHistoryDetails.salesOrder = ordersData.orders;

                    var displayText = new Object();
                    displayText.reorderCaption = stringResource.REORDER;
                    displayText.paymentMethodCaption = stringResource.PAYMENT_METHOD;
                    displayText.shippedCaption = stringResource.SHIPPED_TEXT;
                    displayText.onCaption = stringResource.ON_TEXT;
                    displayText.trackingNumberCaption = stringResource.TRACKING_NUMBER;

                    orderHistoryDetails.stringResource = displayText;
                    reOrderString = null;

                    var orderHistoryContent = thisOrderHistory.parseJqueryTemplate(constantsTemplateID.ORDER_HISTORY_CONTENT_TEMPLATE, orderHistoryDetails);
                    thisOrderHistory.setElementHTMLContent(config.orderHistoryContentId, $(orderHistoryContent).html());
                    thisOrderHistory.displayOrderHistoryRange(ordersData.rows, ordersData.start, ordersData.end, ordersData.current);
                }

                thisOrderHistory.getWebOrder(allPages, current, callBack);
            });
        },

        pagination: function (allPages, rowsPerPage, current) {

            var pageNumbers = Math.ceil(allPages / rowsPerPage);
            if (allPages < rowsPerPage) {
                pageNumbers = 1;
            }

            var pagingContainer = new Array();
            var content = new Object();

            for (var i = 1; i <= pageNumbers; i++) {
                var paging = new Object();

                paging.current = i;
                pagingContainer.push(paging);

                paging = null;
            }

            content.paging = pagingContainer;
            content.allPages = rowsPerPage;

            var pagingContent = thisOrderHistory.parseJqueryTemplate(constantsTemplateID.ORDER_HISTORY_PAGINATION_TEMPLATE, content);
            thisOrderHistory.setElementHTMLContent(config.pagingHeaderId, pagingContent);
        },

        getWebOrder: function (pageCount, current, callBack) {

            var jsonText = { pages: pageCount, current: current }

            var successCallback = function (result) {
                if (result.d) {
                    if (typeof callBack != 'undefined') callBack(result);
                } else {
                    console.log(result.d);
                }
            }

            var failedCallback = function (result) {
                console.log(result.d);
            }

            thisOrderHistory.ajaxRequest("../ActionService.asmx/GetOrderHistory", jsonText, successCallback, failedCallback);
        },

        hideAccountOrderHistoryLink: function () {
            var config = getConfig();
            $(this.selectorChecker(config.orderHistoryLinkId)).hide();
        }
    }

    $.extend($.fn.OrderHistory, new jqueryBasePlugin());

    function setConfig(value) { config = value; }
    function getConfig() { return config; }

})(jQuery);
