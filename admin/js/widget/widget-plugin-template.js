//widget loader
$.template("widget-loader", "<div class='loader'>loading....</div>");

//stock alert
$.template("stockalert-body", 
            "<table class='stockalert' id='${ID}'>" +
            "<tr class='head'>" +
                "<th>${ItemHeader}</th>" +
                "<th class='col-stock'>${StockHeader}</th>" + 
            "</tr>" + 
            "</table>");
$.template("stockalert-content", 
            "<tr>" +
                "<td class='col-item'>${ItemDescription}</td>" +
                "<td class='col-stock'>${StockTotal.FreeStock}</td>" +
            "</tr>");

//new customers
$.template("newcustomers-body",
            "<table class='newcustomers' id='${ID}'>" +
            "<tr class='head'>" +
                "<th>${CustomerCodeHeader}</th>" +
                "<th>${CustomerHeader}</th>" +
                "<th class='col-registered'>${RegisteredHeader}</th>" +
            "</tr>" +
            "</table>");
$.template("newcustomers-content",
            "<tr>" +
                "<td class='col-customercode'>${CustomerCode}</td>" +
                "<td class='col-name'>${FullName}</td>" +
                "<td class='col-registered'>${formatDate(DateRegistered)}</td>" +
            "</tr>");

//recent orders
$.template("recentorders-body",
            "<table class='recentorder' id='${ID}'>" +
            "<tr class='head'>" +
                "<th>${SalesOrderCodeHeader}</th>" +
                "<th>${CustomerHeader}</th>" +
                "<th>${DateHeader}</th>" +
                "<th>${CurrencyHeader}</th>" +
                "<th class='col-total'>${TotalHeader}</th>" +
            "</tr>" +
            "</table>");
$.template("recentorders-content",
            "<tr>" +
                "<td class='col-salesorder'>${SalesOrderCode}</td>" +
                "<td class='col-customer'>${CustomerName}</td>" +
                "<td class='col-date'>${formatDate(SalesOrderDate)}</td>" +
                "<td class='col-currency'>${CurrencyCode}</td>" +
                "<td class='col-total'>${Total}</td>" +
            "</tr>");

//date
$.template("date-tool",
            "<div class='date-tool'>" +
                "<div class='col-date'>" +
                    "<div class='btn-group'>" +
                        "<a class='btn dropdown-toggle' data-toggle='dropdown' href='#'>${formatDate2(StartValue)} - ${formatDate2(EndValue)} <span class='caret'></span></a>" +
                        "<div class='dropdown-menu' id='${DateID}'>" +
                            "<table>" +
                                "<tr valign='middle'>" +
                                    "<td><span class='add-on'>Start Date</span></td>" +
                                    "<td><input type='text' id='${StartID}' value='${StartValue}' class='visitors-date'></td>" +
                                "</tr>" +
                                "<tr valign='middle'>" +
                                    "<td><span class='add-on'>End Date</span></td>" +
                                    "<td><input type='text' id='${EndID}' value='${EndValue}' class='visitors-date'></td>" +
                                "</tr>" +
                                 "<tr valign='middle'>" +
                                    "<td></td>" +
                                    "<td align='right'><input type='button' id='${ApplyID}' value='Apply' class='btn'></td>" +
                                "</tr>" +
                            "</table>" +
                        "</div>" +
                    "</div>" +
                "</div>" +
                "<div class='col-dimensions'>" +
                    "<div class='btn-group'>" +
                        "<button class='${DayClass}' id='${DayID}'>Day</button>" +
                        "<button class='${WeekClass}' id='${WeekID}'>Week</button>" +
                        "<button class='${MonthClass}' id='${MonthID}'>Month</button>" +
                        "<button class='${YearClass}' id='${YearID}'>Year</button>" +
                    "</div>" +
                "</div>" +
            "</div>");

$.template("date-tool2",
            "<div class='date-tool'>" +
                "<div class='col-date'>" +
                    
                "</div>" +
                "<div class='col-dimensions'>" +
                    "<div class='btn-group'>" +
                        "<button class='${DayClass}' id='${DayID}'>Today</button>" +
                        "<button class='${WeekClass}' id='${WeekID}'>This Week</button>" +
                        "<button class='${MonthClass}' id='${MonthID}'>This Month</button>" +
                        "<button class='${YearClass}' id='${YearID}'>This Year</button>" +
                    "</div>" +
                "</div>" +
            "</div>");

//visitors
$.template("visitors-content",
            "<div id='${ChartID}'>" +
            "</div>");

//sales
$.template("sales-content",
            "<div id='${ChartID}'>" +
            "</div>");

//salesoverview
$.template("salesoverview-content",
             "<table class='salesoverview-table'>" +
                "<tr>" + 
                    "<td class='left'>" +
                        "<div class='salesoverview-left'>" +
                            "<div class='salesoverview-header'>Orders</div>" +
                            "<div class='salesoverview-content'>${Orders}  <span class='${OrderStatusClass}'></span></div>" +
                            "<div class='${OrdersDiffClass}'>${OrdersDiff}</div>" +
                        "</div>" +
                    "</td>" + 
                    "<td>" +
                         "<div class='salesoverview'>" +
                            "<div class='salesoverview-header'>Revenue</div>" +
                            "<div class='salesoverview-content'>${Revenues} <span class='${RevenueStatusClass}'></span></div>" +
                            "<div class='${RevenuesDiffClass}'>${RevenuesDiff}</div>" +
                         "</div>" +
                    "</td>" +
                "</tr>" +
             "</table>");

//google authorization
$.template("google-authorization", "<button id='${ID}' class='btn'>${ButtonText}</button>");


//store settings
$.template("storesettings-body",
            "<table class='storesettings' id='${ID}'>" +
            "<tr class='head'>" +
                "<th>${ConfigHeader}</th>" +
                "<th class='col-value'>${ValueHeader}</th>" +
            "</tr>" +
            "</table>");
$.template("storesettings-content",
            "<tr>" +
                "<td class='col-config'>${Key}</td>" +
                "<td class='col-value'>${Value}</td>" +
            "</tr>");

//no items
$.template("no-items", "<div>${Message}</div>");


var formatDate = function (datetime) {
    var dateObj = new Date(parseInt(datetime.replace("/Date(", "").replace(")/", ""), 10));
    return dateObj.format("dd-MMM-yyyy"); //01-Jun-2001
}

var formatDate2 = function (datetime) {
    var dateObj = new Date(datetime);
    return dateObj.format("MMM dd, yyyy"); //Jun 11, 2001
}