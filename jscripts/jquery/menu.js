function RegisterMultiColumnMenu() {
    $("#nav ul ul").css({ display: "none" });
    $(".nav li.ismulticolumn").hover(function () {
        $(this).find('ul:first').css({ visibility: "visible", display: "none" }).show(200);
        $(this).find('a:first').addClass("highlighttopmenu");
    }, function () {
        $(this).find('a:first').removeClass("highlighttopmenu");
        $(this).find('ul:first').css({ visibility: "hidden" });
    });

    $(".main-category-detail").hover(function () {
        $(this).addClass("squareHighlight");
    }, function () {
        $(this).removeClass("squareHighlight");
    });

    $('.main-category-container ul li').hover(function () {
        $(this).find('ul.subitem:first').css({ visibility: "visible", display: "none" }).show(200);
    }, function () {
        $(this).find('ul.subitem:first').css({ visibility: "hidden" });
    });
}

function RegisterSimpleMenu() {
    $("#nav ul").css({ display: "none" });
    $("#nav li").hover(function () {
        $(this).find('ul:first').css({ visibility: "visible", display: "none" }).show(300);
    }, function () {
        $(this).find('ul:first').css({ visibility: "hidden" })
    })
}

$(document).ready(function () {
    $('div.collapsable').click(function () { window.location.href = $(this).parent().find('a.leftmenu').attr('href'); })
    $('div.expandable').click(function () { window.location.href = $(this).parent().find('a.leftmenu').attr('href'); });
});