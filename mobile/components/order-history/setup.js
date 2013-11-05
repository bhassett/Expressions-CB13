$(window).load(function () {

    // load all required templates
    $.get("components/order-history/templates/jquery.orderhistory.tmpl.html", function (data, textStatus, XMLHttpRequest) {
        $("body").append(data);


        // load order history plugin cs and call the set up function
        $.getScript("components/order-history/jquery.orderhistory.plugin.js").done(function (script, textStatus) {
            $(this).OrderHistory.setup({ "stringResourceKeys": orderHistoryPluginStringKeys });
        }).fail(function (jqxhr, settings, exception) { alert(exception); });
    });
});