var _DEFAULT_MESSAGE       = "";
var _DOWNLOAD_CASE_MESSAGE = "";

var _ActivityCode          = "";
var _Priority              = "";
var _AssignedTo            = "";
var _Problem               = "";
var _Solution              = "";

$(document).ready(function () {

    GetStringResources("case-history");

    _DEFAULT_MESSAGE = $("#_DEFAULT_MESSAGE").html();
    _DOWNLOAD_CASE_MESSAGE = $("#_DOWNLOAD_CASE_MESSAGE").html();

    RenderCaseHistory();

    $("#support-select-view").change(function () {

        $("#support-search-text").val("");
        RenderCaseHistory();

    });

    $("#support-search-go").click(function () {

        var activityStatus = $("#support-select-view").val();
        var period = $("#support-select-period").val();
        var searchString = $("#support-search-text").val();

        RenderCaseHistory();

    });

    $("#open-case").click(function () {

        if ($(this).hasClass("editable-content")) return false;
        parent.location = "customersupport.aspx";

    });

    $("#support-search-text").keypress(function (event) {

        if (event.which == 13) {

            var activityStatus = $("#support-select-view").val();
            var period = $("#support-select-period").val();
            var searchString = $("#support-search-text").val();

            RenderCaseHistory();

            return false;
        }

    });

    $("#support-select-period").change(function () {

        $("#support-search-text").val("");
        var activityStatus = $("#support-select-view").val();
        var period = $("#support-select-period").val();
        var searchString = $("#support-search-text").val();

        RenderCaseHistory();

    });


});

function RenderCaseHTML_OUTPUT(jsonText) {

    var _output = "";
    var _details = "";
    
    var lst = $.parseJSON(jsonText);

    var activityCode = "";
    var subject      = "";
    var status       = "";
    var priority     = "";
    var assignedTo   = "";
    var problemText  = "";
    var solutionText = "";
    var altText      = "";

    for (var i = 0; i < lst.length; i++) {

      activityCode = lst[i].ActivityCode;
      subject      = lst[i].SubjectShort;
      altText      = lst[i].SubjectFull;
      status       = lst[i].Status;
      priority     = lst[i].Priority;
      assignedTo   = lst[i].AssignedTo;
      problemText  = lst[i].ProblemText;
      solutionText = lst[i].SolutionText;

      _details  = "<div class='clear-both height-5'></div>";
      _details += "<span id='activity-code-column-place-holder' class='capitalize-text'>" + _ActivityCode + "</span> <span id='activity-code-value-place-holder'>" + activityCode + "</span>";
      _details += "<div class='clear-both'></div>";

      _details += "<span id='priority-column-place-holder' class='capitalize-text'>" + _Priority + "</span> <span id='priority-value-place-holder'>" + priority + "</span>";
      _details += "<div class='clear-both'></div>";

      _details += "<span id='assigned-to-column-place-holder' class='capitalize-text'>" + _AssignedTo + "</span> <span id='assigned-to-value-place-holder'>" + assignedTo + "</span>";
      _details += "<div class='clear-both height-12'></div>";

      _details += "<span id='problem-text-column-place-holder' class='strong-font capitalize-text content' data-contentKey='customersupport.aspx.47' data-contentValue='" + _Problem +  "' data-contentType='string resource'>" + _Problem + "</span>";
      _details += "<div class='clear-both height-12'></div>";

      _details += "<div class='p-details'>" + problemText  + "</div>";

      _details += "<div class='clear-both height-12'></div>";
      _details += "<span id='field-solution-text-place-holder' class='strong-font capitalize-text content' data-contentKey='customersupport.aspx.48' data-contentValue='" + _Solution  + "' data-contentType='string resource'>" + _Solution + "</span>";
      _details += "<div class='clear-both height-12'></div>";

      _details += "<div class='p-details'>" + solutionText + "</div>";

      _detailsText = "<div class='support-case-details-wrapper' id='" + activityCode + "-details'>" + _details + "</div>";

      _output += "<div class='support-list-row'>";

      _output += "<div class='support-fields-date-started-value-container float-left'>" + lst[i].DateCreated + "</div>";
      _output += "<div class='support-fields-subject-value-container float-left'><a href='javascript:void(1)' class='case-subject-links' id='" + activityCode + "' title='" + altText + "'>" + subject + "</a><div class='clear-both'></div>" + _detailsText  + "</div>";
      _output += "<div class='support-fields-status-value-container float-left'>" + status + "</div>";

      _output += "</div>";
      _output += "<div class='clear-both'></div>";

    }

    if (lst.length == 0) _output = "<div id='no-case-to-display'>No records found</div>";

    return _output;
}

function RenderCaseHistory() {

    var activityStatus = $("#support-select-view").val();
    var period         = $("#support-select-period").val();
    var searchString   = $("#support-search-text").val();

    var thisProcessStringResource = _DOWNLOAD_CASE_MESSAGE;

    $("#support-grid-border-bottom").removeClass("error-message");

    $("#support-grid-border-bottom").html("<div style='float:left;width:12px;'><img id='captcha-loader' src='images/ajax-loader.gif'></div> <div id='loader-container-to-right'>" + thisProcessStringResource + "</div>");
   
    var jsonText = JSON.stringify({ activityStatus: activityStatus, period: period, searchString: searchString });

    $.ajax({
        type: "POST",
        url: "ActionService.asmx/GetCaseHistory",
        data: jsonText,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {

            // _loadStringResources(result.d);



            var _html = RenderCaseHTML_OUTPUT(result.d);

            $("#support-grid-details").html(_html);
            $("#support-grid-border-bottom").html("");

            bindCaseDetailsLink();

        },
        fail: function (result) {

            $("#support-grid-border-bottom").addClass("error-message");
            $("#support-grid-border-bottom").html(result.d);

        }
    });
}

function bindCaseDetailsLink() {

    $(".case-subject-links").click(function () {

        var thisId = $(this).attr("id");

        $(".support-case-details-wrapper").fadeOut("Slow");
        $("#" + thisId + "-details").fadeIn("Slow");

    });

}
