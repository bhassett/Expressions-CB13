(function ($) {
    var defaults = { WidgetID: "", WidgetContentClass: ".content", WidgetContentMaxHeight: "100%" };
    var widgetConstants = {
        EMPTY_VALUE: '',
        DOT_VALUE: '.',
        DASH_VALUE: '-'
    }

    var dimensions = {
        Day: 'date',
        Week: 'week',
        Month: 'month',
        Year: 'year'
    }

    var prefix = {
        BUTTON: 'btn',
        TEXTBOX: 'txt'
    }

    var config = {};
    var thisObject;

    $.fn.VisitorsWidget = {

        initialize: function (options) {
            if (options) { setConfig($.extend(defaults, options)); }

            var config = getConfig();

            this.attachEvents();
            thisObject = this;

            if(config.StartDate == '' || config.EndDate == '') {
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
            //thisObject.gaClientLoad();
            //console.log(config);

            var object = { dimension: config.Dimensions, dateFrom: config.StartDate, dateTo: config.EndDate  };
            var jsonData = JSON.stringify(object);
            $.ajax({
                type: "POST",
                url: '../ActionService.asmx/GetWebVisitors',
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
                            $(content).append(parseTemplate("visitors-content", { ChartID: chartID }));
                            buildChart2({ ChartID: chartID, Delay: config.WidgetDelay, Dimensions: config.Dimensions }, items);
                            $(content).css("max-height", config.WidgetContentMaxHeight);
                        }, config.WidgetDelay);
                    }
                    else{
                        $(content).empty();
                        $(content).append(parseTemplateReturnHtml("no-items", { Message: "Please configure the following appconfigs:" }));
                        $(content).append("<br>");
                        $(content).append(parseTemplateReturnHtml("no-items", { Message: "1. GoogleAnalytics.Username" }));
                        $(content).append("<br>");
                        $(content).append(parseTemplateReturnHtml("no-items", { Message: "2. GoogleAnalytics.Password" }));
                        $(content).append("<br>");
                        $(content).append(parseTemplateReturnHtml("no-items", { Message: "3. GoogleAnalytics.APIKey" }));
                    }
                },
                error: function (result, textStatus, exception) {
                    $(content).empty();
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
        },

        //handles google api client load
        gaClientLoad: function () {
            var config = getConfig();

            //set google api key
            gapi.client.setApiKey(config.APIKey);

            //authenticate
            window.setTimeout(thisObject.gaCheckAuth, 1);
        },

        //check authorization
        gaCheckAuth: function () {
            var config = getConfig();

            console.log("Authorizing...");
            gapi.auth.authorize({ client_id: config.ClientID, scope: config.Scopes, immediate: true }, thisObject.gaAuthResult);
        },

        //handles authorization result
        gaAuthResult: function (authResult) {
            if (authResult) {
                console.log("Authorization successful");
                thisObject.gaAnalyticsClient();
            }
            else {
                console.log("Authorization failed");
                thisObject.gaUnauthorized();
            }
        },

        //handles unauthorized user
        gaUnauthorized: function () {
            var config = getConfig();
            var content = $(selectorChecker(config.WidgetID)).children(selectorChecker(config.WidgetContentClass));
            var btnAuth = prefix.BUTTON + config.WidgetID + "Authorize";

            $(content).html(parseTemplate("google-authorization", { ID: btnAuth, ButtonText: "Google Sign In" }));

            $(selectorChecker(btnAuth)).click(function (event) { thisObject.gaAuthClick(event); });
        },

        //handles authorize button click
        gaAuthClick: function (event) {
            event.preventDefault();

            var config = getConfig();
            gapi.auth.authorize({ client_id: config.ClientID, scope: config.Scopes, immediate: false }, thisObject.gaAuthResult);

            return false;
        },

        //load analytics client
        gaAnalyticsClient: function () {
            gapi.client.load('analytics', 'v3', thisObject.gaAuthorized);
        },

        //handles authorized client
        gaAuthorized: function () {
            thisObject.displayLoader();

            //get list of google analytics accounts for this user
            thisObject.gaQueryAccounts();
        },

        //get list of all google analytics accounts for this user
        gaQueryAccounts: function () {
            console.log("Querying Accounts...");
            gapi.client.analytics.management.accounts.list().execute(thisObject.gaAccounts);
        },

        //handle accounts
        gaAccounts: function (results) {
            if (!results.code) {
                if (results && results.items && results.items.length) {

                    console.log("Account(s) found: " + results.items.length);

                    var accountID = results.items[0].id;
                    var defaultAccountID = getAccountID(config.TrackingCode);

                    for (var i = 0; i < results.items.length; i++) {
                        if (defaultAccountID == results.items[i].id) {
                            accountID = defaultAccountID;
                        }
                    }

                    console.log("AccountID: " + accountID);

                    // query for web properties
                    thisObject.gaQueryWebProperties(accountID);
                } else {
                    console.log('No accounts found for this user.')
                }
            }
            else {
                console.log('There was an error querying accounts: ' + results.message);
            }
        },

        //query web properties  
        gaQueryWebProperties: function (accountID) {
            console.log('Querying Web Properties...');

            // get a list of all the web properties for the account
            gapi.client.analytics.management.webproperties.list({ 'accountId': accountID }).execute(thisObject.gaWebProperites);
        },

        //handles web properties
        gaWebProperites: function (results) {
            if (!results.code) {
                if (results && results.items && results.items.length) {
                    //get the first google analytics account
                    var accountID = results.items[0].accountId;

                    //get the first web property id
                    var webPropertyID = results.items[0].id;

                    console.log("PropertyID: " + webPropertyID);
                    thisObject.gaQueryProfiles(accountID, webPropertyID);
                }
                else {
                    console.log("No web properties found for this user");
                }
            }
            else {
                console.log('There was an error querying webproperties: ' + results.message);
            }
        },

        //query profiles
        gaQueryProfiles: function (accountID, webPropertyID) {
            console.log("Querying Profiles...");
            gapi.client.analytics.management.profiles.list({ 'accountId': accountID, 'webPropertyId': webPropertyID }).execute(thisObject.gaProfiles);
        },

        //handles profiles
        gaProfiles: function (results) {
            if (!results.code) {
                if (results && results.items && results.items.length) {

                    //get the first Profile ID
                    var profileID = results.items[0].id;

                    //query the Core Reporting API
                    console.log("ProfileID: " + profileID);
                    thisObject.gaQueryCoreReportingAPI(profileID);

                } else {
                    console.log('No profiles found for this user.');
                }
            } else {
                console.log('There was an error querying profiles: ' + results.message);
            }
        },

        //query core reporting api
        gaQueryCoreReportingAPI: function (profileID) {
            console.log("Querying Core Reporting...");
            // Use the Analytics Service Object to query the Core Reporting API
            //todo:
            gapi.client.analytics.data.ga.get({
                'ids': 'ga:' + profileID,
                'start-date': config.StartDate,
                'end-date': config.EndDate,
                'metrics': 'ga:visits',
                'dimensions': 'ga:' + config.Dimensions,
            }).execute(thisObject.gaCoreReportingResults);
        },

        //handles core reporting results
        gaCoreReportingResults: function (results) {
            var config = getConfig();
            var content = $(selectorChecker(config.WidgetID)).children(selectorChecker(config.WidgetContentClass));
            $(content).empty();

            thisObject.displayHeader();

            if (results.error) {
                $(content).append(results.message);
                console.log('There was an error querying core reporting API: ' + results.message);
            }
            else {
                thisObject.gaPrintResults(results);
            }
        },

        //print results
        gaPrintResults: function (results) {
            var config = getConfig();
            var content = $(selectorChecker(config.WidgetID)).children(selectorChecker(config.WidgetContentClass));
            
            if (results.rows && results.rows.length) {
                //console.log(results);
                thisObject.gaGetChart(results);    
            } else {
                console.log('No results found');
            }
        },

        //dimension click
        gaDimensionClick: function(event, dimensions) {
            event.preventDefault();
            config.Dimensions = dimensions;
            thisObject.refresh();
        },

        //date click
        gaApplyDateClick: function(event, date) {
            event.preventDefault();
            config.StartDate = date.StartDate;
            config.EndDate = date.EndDate;
            thisObject.refresh();
        },

        displayHeader: function() {
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
            $(selectorChecker(btnApply)).click(function (event) { thisObject.gaApplyDateClick(event,
                {
                    StartDate: $(selectorChecker(txtStart)).val(),
                    EndDate: $(selectorChecker(txtEnd)).val()
                }); 
            });

            $(selectorChecker(btnDate)).click(function(event) { event.stopPropagation(); });

            //attach date picker
            $(selectorChecker(txtStart)).datepicker({ dateFormat: 'yy-mm-dd' });
            $(selectorChecker(txtEnd)).datepicker({ dateFormat: 'yy-mm-dd' });

            //stop date picker from closing the date dropdown menu
            $(selectorChecker("ui-datepicker-div")).click(function(event) { event.stopPropagation() });
        },

        gaGetChart: function(data){
            var config = getConfig();
            var content = $(selectorChecker(config.WidgetID)).children(selectorChecker(config.WidgetContentClass));

            //var url = buildChartQuery({Width: 600, Height: 150}, convertColumnsToArray(data,1), convertColumnsToArray(data,0));
            var chartID = config.WidgetID + "Chart";
            $(content).append(parseTemplate("visitors-content", { ChartID: chartID }));
            buildChart({ ChartID: chartID, Delay: config.WidgetDelay, Dimensions: config.Dimensions }, data);
        }
    };
    function buildChart(settings, data)
    {
        $.getScript("//www.google.com/jsapi").done(function () {
            setTimeout(function(){
                google.load('visualization', '1', {'callback':function(){ 
                   var dt = new google.visualization.DataTable();
                
                   //add columns
                   dt.addColumn('string', settings.Dimensions); //yaxis
                   dt.addColumn('number', 'Visits'); //axis

                   //add empty rows
                   dt.addRows(data.rows.length);

                   for (var i = 0; i < data.rows.length; i++) {
                        var ydata = formatDate(settings.Dimensions, data.rows[i][0].toString());
                        dt.setCell(i, 0, ydata); //yaxis
                        dt.setCell(i, 1, Number(data.rows[i][1])); //xaxis
                   }
                    

                   var options = { 
                        series: {0:{color: '#FF6600'}}
                    };
                   var chart = new google.visualization.AreaChart(document.getElementById(settings.ChartID));
                   chart.draw(dt, options);
                   console.log("Done");
                }, 'packages':['corechart']})
            }, 1);
        });
    }

     function buildChart2(settings, data) {
        $.getScript("//www.google.com/jsapi").done(function () {
            setTimeout(function () {
                google.load('visualization', '1', { 'callback': function () {
                    var dt = new google.visualization.DataTable();
                    //add columns
                    dt.addColumn('string', settings.Dimensions); //yaxis
                    dt.addColumn('number', 'Visitors'); //axis

                    //add empty rows
                    dt.addRows(data.length);

                    for (var i = 0; i < data.length; i++) {
                        var ydata = formatDate(settings.Dimensions, data[i].Dimension.toString());
                        dt.setCell(i, 0, ydata); //yaxis
                        dt.setCell(i, 1, Number(data[i].Visits)); //xaxis
                    }

                    var options = {
                        series: { 0: { color: '#FF6600'} },
                        legend : { position: 'top' }
                    };
                    var chart = new google.visualization.AreaChart(document.getElementById(settings.ChartID));
                    chart.draw(dt, options);
                }, 'packages': ['corechart']
                })
            }, 1);
        });
    }

    function formatDate(dimension, data)
    {
        var ret = '';
        switch(dimension)
        {
            case dimensions.Day:
                var date = new Date(Number(data.substr(0,4)), Number(data.substr(4,2)) - 1, Number(data.substr(6,2)), 0, 1, 1);
                ret = date.format("MMM dd, yyyy");
                break;
            case dimensions.Month:
                var date = new Date("2012", data - 1, "1");
                ret = date.format("MMM");
                break;
            default:
                ret = data;
                break;
        }
        return ret;
    }

    function getDefaultDate()
    {
        var ret = { StartDate: '', EndDate: '' };
        var date = new Date();
        ret.StartDate = (new Date(date.getFullYear(), date.getMonth(), 1)).format("yyyy-MM-dd");
        ret.EndDate = (new Date(date.getFullYear(), date.getMonth() + 1, 0, 23,59,59)).format("yyyy-MM-dd");
        return ret;
    }

     //deprecated 
    function buildChartQuery(settings, xdata, ydata) {

        //see documentation here 
        //https://developers.google.com/analytics/solutions/articles/gdataAnalyticsCharts
        var chartParams = {
            'chs': '',  //image dimensions
            'chxt': '', //axes
            'chts': '', //title style
            'cht': '',  //chart type
            'chco': '', //colors
            'chbh': '', //width and spacing
            'chm': '',  //markers
            'chtt': '', //title
            'chdl': '', //legend
            'chd': '',  //chart data
            'chxl': '', //axis labels
            'chds': '', //scaling
            'chxr': ''  //axis scaling
        };
        
        chartParams.chs = settings.Width + "x" + settings.Height;
        chartParams.chxt = "x,y";
        chartParams.chts = "000000,15";
        chartParams.cht = "lc";
        chartParams.chco = "a3d5f7,389ced";
        chartParams.chbh = "a,5,20";
        //chartParams.chm = "N,FF0000,-1,,12";
        chartParams.chd = "t:" + xdata.join();
        chartParams.chxl = "0:|" + ydata.join('|');
        chartParams.chds = "a";

        var url =  "//chart.googleapis.com/chart?";
        $.each(chartParams, function(name, val){
            if(val == '') { return; }

            if(name != "chs") {
                url += "&";
            }   
            url += name + "=" + val;
        });
        return url;
    }

    function convertColumnsToArray(data, columnNo)
    {
        var ret = new Array();

        for(var i=0; i < data.length; i++) {
            ret.push(data[i][columnNo]);
        }

        return ret;
    }

    function getCurrentDimension(id) {
        var dimension = { height: 0, width: 0};
        dimension.height = $(id).height();
        dimension.width = $(id).width();
        return dimension;
    }

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

    function getAccountID(trackingCode) {
        return trackingCode.split(widgetConstants.DASH_VALUE)[1];
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

