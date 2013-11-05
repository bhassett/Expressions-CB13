
$.fn.miniCartCarousel = function (options) {

    var settings = $.extend({
        'id': 'test'
    }, options);

    return this.each(function () {

        var $wrapper3 = $('> div', this).css('overflow', 'hidden');
        $slider = $wrapper3.find('> ul');
        $items = $slider.find('> li');
        $single = $items.filter(':first');

        var singleWidth = $single.outerWidth();
        var visible = Math.ceil($wrapper3.innerWidth() / singleWidth);
        var currentPage = 1;
        var pages = Math.ceil($items.length / visible);

        $wrapper3.scrollLeft(visible);

        function gotoPage(page) {
            var id = settings.id;
            //alert(pages);
            if (page <= pages && page >= 1) {
                var dir = page < currentPage ? -1 : 1,
                n = Math.abs(currentPage - page),
                left = singleWidth * dir * visible * n;

                $wrapper3.filter(':not(:animated)').animate({
                    scrollLeft: '+=' + left
                }, 500, function () {
                    if (page == 1) {
                        $('a#b' + id).addClass('back-disabled');
                        $('a#f' + id).removeClass('forward-disabled');
                    }
                    else if (page == 2 && page == pages) {
                        $('a#b' + id).removeClass('back-disabled');
                        $('a#f' + id).addClass('forward-disabled');
                    }
                    else if (page == pages) {
                        $('a#f' + id).addClass('forward-disabled');
                    }
                    else {

                        $('a#f' + id).removeClass('forward-disabled');
                        $('a#b' + id).removeClass('back-disabled');
                    }
                    if (page == 0) {
                        $wrapper3.scrollLeft(singleWidth * visible * pages);
                        page = pages;
                    }
                    currentPage = page;
                });
            }
            return false;
        }

        $wrapper3.after('<div style="float:left;margin-top:0px;position:relative;width:30px;"><a class=\"arrow back\" id=\"b' + settings.id + '\"></a></div><div style="float:right;margin-top:0px;position:relative;width:30px;"><a class=\"arrow forward\" id=\"f' + settings.id + '\"></a></div>');
        // 5. Bind to the forward and back buttons
        $('a#b' + settings.id, this).click(function () {
            return gotoPage(currentPage - 1);
        });

        $('a#f' + settings.id, this).click(function () {
            return gotoPage(currentPage + 1);
        });
        // create a public interface to move to a specific page
        $(this).bind('goto', function (event, page) {

            gotoPage(page);
        });
    });
};

$.fn.carousel = function (options) {
    var settings = $.extend({
        'id': 'test'
    }, options);

    return this.each(function () {

        var $wrapper3 = $('> div', this).css('overflow', 'hidden');

        $slider = $wrapper3.find('> ul');
        $items = $slider.find('> li');
        $single = $items.filter(':first');

        var singleWidth = $single.outerWidth();
        var visible = Math.ceil($wrapper3.innerWidth() / singleWidth);
        var currentPage = 1;
        var pages = Math.ceil($items.length / visible);
        //var page = 1;

        // alert("." + settings.id + "-pagination");
        var noofitems = $("." + settings.id + "-item").length;
        // alert("No of Items: " + noofitems);
        var noofpages = Math.ceil(noofitems / 4);
        var paging = "<ul id='" + settings.id + "-pagination-ul'>";
        //alert("Pages: " + noofpages);
        if (noofpages > 1) {
            for (var i = 1; i <= noofpages; i++) {
                paging += "<li class='test-me' id='" + settings.id + "-page-" + i + "' ><a href='javascript:void(1)' id='test' class= 'paging-" + settings.id + "'>" + i + "</a></li>";
            }
            paging += "</ul>";

            $("." + settings.id + "-pagination").html(paging);
        }
        $wrapper3.scrollLeft(visible);

        function gotoPage(page) {

            var id = settings.id;
            if (page <= pages && page >= 1) {

                var dir = page < currentPage ? -1 : 1,
                n = Math.abs(currentPage - page),
                left = singleWidth * dir * visible * n;

                $("#" + settings.id + "-page-" + currentPage).removeClass('currentitem');
                $("#" + id + "-page-" + page).attr("class", "currentitem");

                $wrapper3.filter(':not(:animated)').animate({
                    scrollLeft: '+=' + left

                }, 500, function () {

                    if (page == 1) {

                        $('a#b' + id).addClass('back-disabled');
                        $('a#f' + id).removeClass('forward-disabled');
                    }
                    else if (page == 2 && page == pages) {

                        $('a#b' + id).removeClass('back-disabled');
                        $('a#f' + id).addClass('forward-disabled');
                    }
                    else if (page == pages) {

                        $('a#f' + id).addClass('forward-disabled');
                        $('a#b' + id).removeClass('back-disabled');
                    }
                    else {

                        $('a#f' + id).removeClass('forward-disabled');
                        $('a#b' + id).removeClass('back-disabled');
                    }
                    if (page == 0) {

                        $wrapper3.scrollLeft(singleWidth * visible * pages);
                        page = pages;
                    }
                    currentPage = page;
                });
            }
            return false;
        }


        $wrapper3.after('<a class=\"arrow back\" id=\"b' + settings.id + '\"></a><a class=\"arrow forward\" id=\"f' + settings.id + '\"></a>');

        // 5. Bind to the forward and back buttons
        $('a#b' + settings.id, this).click(function () {
            var page = parseInt(currentPage) - 1;
            return gotoPage(page);
        });

        $('a#f' + settings.id, this).click(function () {
            var page = parseInt(currentPage) + 1;
            return gotoPage(page);
        });


        $('.paging-' + settings.id).click(function () {
            $("#" + settings.id + "-page-" + currentPage).removeClass('currentitem');
            var pageNumber = $(this).html();
            return gotoPage(pageNumber);
        });
        // create a public interface to move to a specific page
        $(this).bind('goto', function (event, page) {
            gotoPage(page);
        });
    });
};



function NewUpsellClick(item) {

    if (!item.value) { return; }

    var itemid = item.id;
    var currentaction = "";
    if ($('.AddToCartclass').html()) {
        currentaction = $('.AddToCartclass').attr("action");
    }

    var arrUpsellItems = new Array();

    if (currentaction.length > 0) { arrUpsellItems = currentaction.split('&'); }

    var isSelected = false;
    var upsellids = "";
    for (var i = 0; i < arrUpsellItems.length; i++) {
        if (arrUpsellItems[i].toLowerCase().replace(' ', '').indexOf('relatedproducts=') > -1) {  //selected = 1,2,3
            isSelected = true;
            if (arrUpsellItems[i].split('=').length > 1) {
                upsellids = arrUpsellItems[i].split('=')[1]; //1,2,3
                //remove selected word in action
                currentaction = currentaction.replace('&' + arrUpsellItems[i], '');
            }
        }
    }

    var isChecked = item;
    //var found = upsellids.indexOf(',' + isChecked.id + ',')

    if (isChecked.checked) {
        //found == -1
        //add to selected
        upsellids = upsellids + isChecked.id + ','
    }

    else {
        upsellids = upsellids.replace(isChecked.id + ',', '');
    }

    currentaction = currentaction + "&relatedproducts=" + upsellids;
    $('.AddToCartclass').attr("action", currentaction);
}


$(document).ready(function () {

    $('#also-Purchased').carousel({ id: "also-Purchased" });
    $('#also-Viewed').carousel({ id: "also-Viewed" });
    $('#mini-Accessory').miniCartCarousel({ id: "mini-Accessory" });

    $('.back').addClass('back-disabled');

    var litems = $('.l-acc').length;
    var vitems = $('.also-Viewed-item').length;
    var page = 1;
    var vpages = Math.ceil(vitems / 4);
    var lpages = Math.ceil(litems / 3);
    var pitems = $('.also-Purchased-item').length;
    var ppages = Math.ceil(pitems / 4);

    if (litems == 0) {
        $('#mini').hide();
    }

    if (lpages == 1) {
        $('#fmini-Accessory').addClass('forward-disabled');
    }

    if (ppages == 1) {
        $('#falso-Purchased').addClass('forward-disabled');
        $('#balso-Purchased').addClass('back-disabled');
    }

    if (vpages == 1) {
        $('#falso-Viewed').addClass('forward-disabled');
        $('#balso-Viewed').addClass('back-disabled');
    }

    $("#also-Purchased-page-1").attr("class", "currentitem");
    $("#also-Viewed-page-1").attr("class", "currentitem");

});
