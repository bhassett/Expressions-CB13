$(document).ready(function () {

    //if link w/ attrib itempopup is clicked
    $("a[itempopup]").on("click", function (event) {
        //cancel link behaviour
        event.preventDefault();

        var loader = $("#itempopup-loader"); //get the item popup loader
        var destination = $(this).attr("href"); //get the link url
        var itemCode = $(this).attr("itempopup"); //get the itemcode
        var itemPopupSection = $('div[itempopup-section=' + itemCode + ']'); //get the item popup section

        //get the window height and width
        var winH = $(window).height();
        var winW = $(window).width();

        //show loader
        loader.show();

        //hide all itempopup section
        $('div[itempopup-section]').hide();

        //if itempopup section is already populated
        if ($(itemPopupSection).length) {
            itemPopupSection.show(); //show the selected item popup section
            loader.hide();
        }
        else {
            $.ajax({
                type: "POST",
                url: "ActionService.asmx/GetItemPopup",
                data: JSON.stringify({ itemCode: itemCode }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result.d == "") {
                        document.location.href = destination; //redirect to the original destination
                    }
                    else {
                        $("#itempopup-container").append(result.d);
                    }
                    loader.hide();
                },
                error: function (result, textStatus, exception) {
                    console.log(exception);
                }
            });
        }

        var popup = $("#itempopup-container");
        var mask = $("#itempopup-mask");
        var maskHeight = $(document).height();
        var maskWidth = $(window).width();

        //set height and width of mask to fill up the whole screen
        mask.css({ 'width': maskWidth, 'height': maskHeight });

        //mask transition effect     
        mask.fadeIn(1);
        mask.fadeTo("slow", 0.2);

        //set the popup container to center
        $(popup).css('top', winH / 2 - $(popup).height() / 2);
        $(popup).css('left', winW / 2 - $(popup).width() / 2);

        //popup transition effect 
        $(popup).fadeIn(1);
    });

    //if close button is clicked
    $('#itempopup-container .close').click(function (e) {
        //cancel link behavior
        e.preventDefault();
        $('#itempopup-mask').hide();
        $('#itempopup-container').hide();
        $('#itempopup-loader').hide();
    });

    //if mask is clicked
    $('#itempopup-mask').click(function () {
        $(this).hide();
        $('#itempopup-container').hide();
        $('#itempopup-loader').hide();
    });

    //re-position modal popup on window resize
    $(window).resize(function () {
        var popup = $('#itempopup-container');

        //get the screen height and width
        var maskHeight = $(document).height();
        var maskWidth = $(window).width();

        //set height and width to mask to fill up the whole screen
        $('#itempopup-mask').css({ 'width': maskWidth, 'height': maskHeight });

        //get the window height and width
        var winH = $(window).height();
        var winW = $(window).width();

        //set the popup window to center
        popup.css('top', winH / 2 - popup.height() / 2);
        popup.css('left', winW / 2 - popup.width() / 2);
    });
});

$(document).ready(function () {
    var itemcode = getQueryString()["itempopup"];
    if (itemcode != null) {
        getItemPopup(null, itemcode, null);
    }
}); 

function getItemPopup(currentitem, nextitem, event) {
    //cancel link behaviour
    if (event != null) {
        event.preventDefault();
    }

    var loader = $("#itempopup-loader"); //get the item popup loader
    var itemPopupSection = $('div[itempopup-section=' + currentitem + ']'); //get the current item popup section
    
    //hide current itempopup
    $(itemPopupSection).hide();
    
    var destination = $(this).attr("href"); //get the link url
    var itemCode = $(this).attr("itempopup"); //get the itemcode
    var itemPopupSection = $('div[itempopup-section=' + nextitem + ']'); //get the item popup section

    //show loader
    loader.show();

    //get the window height and width
    var winH = $(window).height();
    var winW = $(window).width();

    //hide all itempopup section
    $('div[itempopup-section]').hide();

    //check if itempopup section is already populated
    if ($(itemPopupSection).length) {
        itemPopupSection.show(); //show the selected item popup section
        loader.hide();
    }
    else {
        $.ajax({
            type: "POST",
            url: "ActionService.asmx/GetItemPopup",
            data: JSON.stringify({ itemCode: nextitem }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.d == "not-supported") {
                    document.location.href = destination; //redirect if item type is not yet supported
                }
                else {
                    $("#itempopup-container").append(result.d);
                }
                loader.hide();
            },
            error: function (result, textStatus, exception) {
                console.log(exception);
            }
        });
    }

    var popup = $("#itempopup-container");
    var mask = $("#itempopup-mask");
    var maskHeight = $(document).height();
    var maskWidth = $(window).width();

    //set height and width of mask to fill up the whole screen
    mask.css({ 'width': maskWidth, 'height': maskHeight });

    //mask transition effect     
    mask.fadeIn(1);
    mask.fadeTo("slow", 0.2);

    //set the popup container to center
    $(popup).css('top', winH / 2 - $(popup).height() / 2);
    $(popup).css('left', winW / 2 - $(popup).width() / 2);

    //popup transition effect 
    $(popup).fadeIn(1);

    switchTabPage(1, nextitem);
}

function switchTabPage(index, itemCode) {
    //hide all tabs of the active itempopup and update the class attrib
    $('div[itempopup-section=' + itemCode + '] .itempopup-tab div[tabIndex]').hide();
    $('div[itempopup-section=' + itemCode + '] .itempopup-tab-header a[tabIndex]').attr("class", "tab-menu");

    //show selected tab of the active itempopup and update the class attrib 
    $('div[itempopup-section=' + itemCode + '] .itempopup-tab div[tabIndex=' + index + ']').show();
    $('div[itempopup-section=' + itemCode + '] .itempopup-tab-header a[tabIndex=' + index + ']').attr("class", "tab-menu-selected");
    return false;
}

