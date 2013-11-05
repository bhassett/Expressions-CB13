(function ($) {
    var defaults = { WidgetID: "", WidgetContentClass: ".content", WidgetContentMaxHeight: "100%" };
    var widgetConstants = {
        EMPTY_VALUE: '',
        DOT_VALUE: '.'
    }

    var config = {};
    var thisObject;

    var prefix = {
        BUTTON: 'btn',
        TEXTBOX: 'txt'
    }

    var dimensions = {
        Day: 'date',
        Week: 'week',
        Month: 'month',
        Year: 'year'
    }

    $.fn.SalesWidget = {

        initialize: function (options) {
            if (options) { setConfig($.extend(defaults, options)); }

            var config = getConfig();

            this.attachEvents();
            thisObject = this;

            if (config.StartDate == '' || config.EndDate == '') {
                config.StartDate = getDefaultDate().StartDate;
                config.EndDate = getDefaultDate().EndDate;
            }

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
            object.rangeType = config.Dimensions;
            object.dateFrom = config.StartDate;
            object.dateTo = config.EndDate;

            var jsonData = JSON.stringify(object);
            $.ajax({
                type: "POST",
                url: '../ActionService.asmx/GetWebSales',
                data: jsonData,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    var jsonResult = result.d;
                    if (jsonResult != '') {
                        var items = $.parseJSON(jsonResult);

                        window.setTimeout(function () {
                            $(content).empty();
                            thisObject.displayHeader();
                            var chartID = config.WidgetID + "Chart";
                            $(content).append(parseTemplate("sales-content", { ChartID: chartID }));
                            buildChart({ ChartID: chartID, Delay: config.WidgetDelay, Dimensions: config.Dimensions }, items);
                            $(content).css("max-height", config.WidgetContentMaxHeight);
                        }, config.WidgetDelay);

                    }
                },
                error: function (result, textStatus, exception) {
                    console.log(exception);
                }
            });
        },

        displayHeader: function () {
            var config = getConfig();
            var content = $(selectorChecker(config.WidgetID)).children(selectorChecker(config.WidgetContentClass));

            var txtStart = prefix.TEXTBOX + config.WidgetID + 'StartDate';
            var txtEnd = prefix.TEXTBOX + config.WidgetID + 'EndDate';
            var txtStartVal = config.StartDate;
            var txtEndVal = config.EndDate;
            var btnDay = prefix.BUTTON + config.WidgetID + 'Day';
            var btnWeek = prefix.BUTTON + config.WidgetID + 'Week';
            var btnMonth = prefix.BUTTON + config.WidgetID + 'Month';
            var btnYear = prefix.BUTTON + config.WidgetID + 'Year';
            var btnApply = prefix.BUTTON + config.WidgetID + "Apply";
            var btnDate = prefix.BUTTON + config.WidgetID + "Date";

            //display header
            $(content).append(parseTemplate("date-tool", {
                StartID: txtStart,
                StartValue: txtStartVal,
                EndID: txtEnd,
                EndValue: txtEndVal,
                DayID: btnDay,
                WeekID: btnWeek,
                MonthID: btnMonth,
                YearID: btnYear,
                ApplyID: btnApply,
                DateID: btnDate,
                DayClass: (config.Dimensions == "date") ? "btn active" : "btn",
                WeekClass: (config.Dimensions == "week") ? "btn active" : "btn",
                MonthClass: (config.Dimensions == "month") ? "btn active" : "btn",
                YearClass: (config.Dimensions == "year") ? "btn active" : "btn"
            }));

            $(selectorChecker(btnDay)).click(function (event) { thisObject.gaDimensionClick(event, dimensions.Day); });
            $(selectorChecker(btnWeek)).click(function (event) { thisObject.gaDimensionClick(event, dimensions.Week); });
            $(selectorChecker(btnMonth)).click(function (event) { thisObject.gaDimensionClick(event, dimensions.Month); });
            $(selectorChecker(btnYear)).click(function (event) { thisObject.gaDimensionClick(event, dimensions.Year); });
            $(selectorChecker(btnApply)).click(function (event) {
                thisObject.gaApplyDateClick(event,
                {
                    StartDate: $(selectorChecker(txtStart)).val(),
                    EndDate: $(selectorChecker(txtEnd)).val()
                });
            });

            $(selectorChecker(btnDate)).click(function (event) { event.stopPropagation(); });

            //attach date picker
            $(selectorChecker(txtStart)).datepicker({ dateFormat: 'yy-mm-dd' });
            $(selectorChecker(txtEnd)).datepicker({ dateFormat: 'yy-mm-dd' });

            //stop date picker from closing the date dropdown menu
            $(selectorChecker("ui-datepicker-div")).click(function (event) { event.stopPropagation() });
        },

        //dimension click
        gaDimensionClick: function (event, dimensions) {
            event.preventDefault();
            config.Dimensions = dimensions;
            thisObject.refresh();
        },

        //date click
        gaApplyDateClick: function (event, date) {
            event.preventDefault();
            config.StartDate = date.StartDate;
            config.EndDate = date.EndDate;
            thisObject.refresh();
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

    function buildChart(settings, data) {
        $.getScript("//www.google.com/jsapi").done(function () {
            setTimeout(function () {
                google.load('visualization', '1', { 'callback': function () {
                    var dt = new google.visualization.DataTable();

                    //add columns
                    dt.addColumn('string', settings.Dimensions); //yaxis
                    dt.addColumn('number', 'Sales'); //axis

                    //add empty rows
                    dt.addRows(data.length);

                    for (var i = 0; i < data.length; i++) {
                        var ydata = formatDate(settings.Dimensions, data[i].Dimension.toString());
                        dt.setCell(i, 0, ydata); //yaxis
                        dt.setCell(i, 1, Number(data[i].Total)); //xaxis
                    }

                    var options = {
                        series: { 0: { color: '#FF6600'} }
                    };
                    var chart = new google.visualization.ColumnChart(document.getElementById(settings.ChartID));
                    chart.draw(dt, options);
                }, 'packages': ['corechart']
                })
            }, 1);
        });
    }

    function formatDate(dimension, data) {
        var ret = '';
        switch (dimension) {
            case dimensions.Day:
                var date = new Date(Number(data.substr(0, 4)), Number(data.substr(4, 2)) - 1, Number(data.substr(6, 2)), 0, 1, 1);
                ret = date.format("MMM dd, yyyy");
                break;
            case dimensions.Month:
                var date = new Date("2012", data - 1, "1");
                ret = date.format("MMM");
                break;
            case dimensions.Week:
                ret = "Week " + data;
                break;
            default:
                ret = data;
                break;
        }
        return ret;
    }

    function getDefaultDate() {
        var ret = { StartDate: '', EndDate: '' };
        var date = new Date();
        ret.StartDate = (new Date(date.getFullYear(), date.getMonth(), 1)).format("yyyy-MM-dd");
        ret.EndDate = (new Date(date.getFullYear(), date.getMonth() + 1, 0, 23, 59, 59)).format("yyyy-MM-dd");
        return ret;
    }

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