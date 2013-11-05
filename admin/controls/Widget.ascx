<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Widget.ascx.cs" Inherits="admin_controls_Widget" %>

<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>

<script src="js/jquery.tmpl.min.js" type="text/javascript"></script>
<script src="js/widget/widget-plugin-template.js" type="text/javascript"></script>
<script src="//apis.google.com/js/client.js" type="text/javascript"></script>

<div id="<%= ID %>" class="widget">
    <!-- header -->
    <div class="widget-header">
        <div class="title"><%= Title %></div>
        <div class="control">
            <span class="dropdown" style="display:none" widget-control="settings">
                  <a class="icon-white icon-wrench" data-toggle="dropdown" href="#" title="Settings"></a>
                    <ul class="dropdown-menu" style="text-align:left; left:auto; right:0"  role="menu" aria-labelledby="dLabel">
                        <li><a class= "selected" tabindex="-1" href="#" widget-control="activeOnly">Active Only</a></li>
                        <li><a tabindex="-1" href="#" widget-control="showAll">Show All</a></li>
                    </ul>
            </span>
            <a class="icon-white icon-refresh" href="#" title="Refresh" widget-control="refresh"></a>
            <a class="icon-white icon-chevron-up" href="#" title="Minimize" widget-control="minimize"></a>
        </div>
    </div>
    <!-- content -->
    <div class="content"></div>
</div>

<script type="text/javascript">

    var WidgetTypes =
    {
        Visitors: "visitors",
        SalesOverview: "salesoverview",
        RecentOrders: "recentorders",
        StockAlert: "stockalert",
        NewCustomers: "newcustomers",
        Sales: "sales",
        StoreSettings: "storesettings"
    };

    $(window).load(function () {
        widgetID = "<%= ID %>";
        widgetType = "<%= Type %>".toLowerCase();
        widgetMaxHeight = "<%= MaxHeight %>";

        LoadPlugin(widgetType, widgetID, widgetMaxHeight);

        if (widgetType == "stockalert") {
            $("#" + widgetID + " span[widget-control='settings']").show();
        }
    });

    //loads the specific widget's plugin script
    function LoadPlugin(type, id, maxheight) {

        var options = {
            WidgetID: id,
            WidgetContentClass: ".content",
            WidgetContentMaxHeight: maxheight,
            WidgetDelay: 300
        };

        var dimensions = {
            Day: "date",
            Week: "week",
            Month: "month",
            Year: "year"
        };

        switch (type) {

            case WidgetTypes.Visitors:
                $.getScript("js/widget/widget-visitors-plugin.js").done(function () {

                    //include google analytics config
                    options = $.extend(
                    {
                        ClientID: '<%= AppLogic.AppConfig("GoogleAnalytics.ClientID") %>',
                        APIKey: '<%= AppLogic.AppConfig("GoogleAnalytics.APIKey") %>',
                        TrackingCode: '<%= AppLogic.AppConfig("GoogleAnalytics.TrackingCode") %>',
                        Scopes: 'https://www.googleapis.com/auth/analytics.readonly',
                        Dimensions: dimensions.Day,
                        StartDate: '',
                        EndDate: ''
                    }, options);

                    $(id).VisitorsWidget.initialize(options);
                });
                break;

            case WidgetTypes.SalesOverview:
                options = $.extend(
                {
                    ClientID: '<%= AppLogic.AppConfig("GoogleAnalytics.ClientID") %>',
                    APIKey: '<%= AppLogic.AppConfig("GoogleAnalytics.APIKey") %>',
                    TrackingCode: '<%= AppLogic.AppConfig("GoogleAnalytics.TrackingCode") %>',
                    Scopes: 'https://www.googleapis.com/auth/analytics.readonly',
                    Dimensions: dimensions.Day,
                    StartDate: '',
                    EndDate: '',
                    CurrencySymbol: '<%= Currency.GetSymbol(Localization.GetPrimaryCurrency()) %>'
                }, options);

                $.getScript("js/widget/widget-salesoverview-plugin.js").done(function () {
                    $(id).SalesOverViewWidget.initialize(options);
                });
                break;

            case WidgetTypes.RecentOrders:
                $.getScript("js/widget/widget-recentorders-plugin.js").done(function () {
                    $(id).RecentOrdersWidget.initialize(options);
                });
                break;

            case WidgetTypes.StockAlert:
                options = $.extend(
                {
                    Threshold: '<%= AppLogic.AppConfig("Dashboard.StockAlertThreshold") %>'
                }, options);

                $.getScript("js/widget/widget-stockalert-plugin.js").done(function () {
                    $(id).StockAlertWidget.initialize(options);
                });
                break;

            case WidgetTypes.NewCustomers:
                $.getScript("js/widget/widget-newcustomers-plugin.js").done(function () {
                    $(id).NewCustomersWidget.initialize(options);
                });
                break;

            case WidgetTypes.Sales:
                options = $.extend(
                {
                    Dimensions: dimensions.Day,
                    StartDate: '',
                    EndDate: ''
                }, options);

                $.getScript("js/widget/widget-sales-plugin.js").done(function () {
                    $(id).SalesWidget.initialize(options);
                });
                break;

            case WidgetTypes.StoreSettings:
                var keys = [];
                keys.push("CacheMenus");
                keys.push("TransactionMode");
                keys.push("UseSSL");
                keys.push("AlwaysGoSecure");
                keys.push("ErrorNotification");
                keys.push("AllowCreditHold");
                keys.push("StoreVersion");
                //add appconfigs here

                options = $.extend(
                {
                    Keys: keys ,
                    StoreVersion: '<%= CommonLogic.GetVersionNumber() %>'
                }, options);

                $.getScript("js/widget/widget-storesettings-plugin.js").done(function () {
                    $(id).StoreSettings.initialize(options);
                });
                break;

            default:
                break;
        }
    }

</script>
