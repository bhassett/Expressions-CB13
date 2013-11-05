var maxitemtocompare = 5;
var comparepanelname = '#minicompare_panel';
var CompareNowButtonid = '#CompareNowButton';
var ImgLinkid = '#ImgLink_';
var Imgid = '#Imgid_';
var removelinkid = '#removelinkid_';
var xmlpackage ='productcompare.xml.config'
// reload minicompare upon load.
$(document).ready(function () {
    rebuildProductCompareContainer();
    setItemTabPageDetails();
});

function setCompareDefaults(max, pCompareNowButtonid, pImgLinkid, pImgid, premovelinkid, pxmlpackage) {

    if (max < 2) { max = 2; }
    if (max > 5) { max = 5; }
    
    xmlpackage = pxmlpackage;

    maxitemtocompare = max;
    CompareNowButtonid = '#'+pCompareNowButtonid;
    ImgLinkid = '#' + pImgLinkid;
    Imgid = '#' + pImgid;
    removelinkid = '#' + premovelinkid;
}

function findIndexOfElementInArray(elem, arraylist) {
    var elemindex = -1;

    for (var i = 0; i < arraylist.length; i++) {
        if (arraylist[i] == elem) {elemindex = i;}
    }

    return elemindex;
}

// method for add to compare checkbox
function showDifference(elem) {
    if (!elem.value) { return; }
    var iSChecked = elem;

    $('.CompareAttributeRow').children().each(
       function () {
           var s = $(this).html().toLowerCase();

           if (iSChecked.checked) {
               if (s.indexOf('equalattributevalue') != -1) {
                   $(this).parent().attr('bgcolor', '#F9F69D');
               }
               else {
                   $(this).parent().attr('bgcolor', 'White');
               }
           }


           else {
               $(this).parent().attr('bgcolor', 'White');
           }

       });
    
 }

function addRemoveCompare(elem) {
    var selectedItemStr = "";
    var arrSelected = new Array();
    selectedItemStr = readCookie("cia");

    if (selectedItemStr.length > 0) {arrSelected = selectedItemStr.split('%');}

    if (!elem.value) { return; }

    var iSChecked = elem;
    var found = findIndexOfElementInArray(elem.value, arrSelected);

    if (iSChecked.checked && found == -1) {
        if (maxitemtocompare > arrSelected.length) {
            selectedItemStr = addToCompare(elem.value);
        }
        else { updateCompareCheckedValue(false, elem.value); }
    }

    if (!iSChecked.checked && found >= 0) {selectedItemStr = removeToCompare(elem.value);}

    return selectedItemStr;
}

// method for add to compare button
function addToCompare(itemID) {

    var selectedItemStr = "";
    var arrSelected = new Array();

    selectedItemStr = readCookie("cia");

    if (selectedItemStr.length > 0) {arrSelected = selectedItemStr.split('%');}

    var itemIDToAdd = itemID;

    if (!itemIDToAdd) { return; }

    var found = findIndexOfElementInArray(itemIDToAdd.toString(), arrSelected);

    if (found == -1) { arrSelected.push(itemIDToAdd.toString()); }

    selectedItemStr = "";
    for (var i = 0; i < arrSelected.length; i++) {
    selectedItemStr = selectedItemStr + "%" + arrSelected[i];
    }

    selectedItemStr = selectedItemStr.slice(1, selectedItemStr.length);

    upadateCookie("cia", selectedItemStr);

    rebuildProductCompareContainer();

    return selectedItemStr;
}

// method for remove to compare button
function removeToCompare(itemID) {

    var selectedItemStr = "";
    var arrSelected = new Array();

    selectedItemStr = readCookie("cia");

    if (selectedItemStr.length > 0) {arrSelected = selectedItemStr.split('%');}

    var itemIDToAdd = itemID;

    if (!itemIDToAdd) { return; }

    var found = findIndexOfElementInArray(itemIDToAdd.toString(), arrSelected);

    if (found >= 0) {
        arrSelected.splice(found, 1);
        updateCompareCheckedValue(false, itemID);
    }

    selectedItemStr = "";
    for (var i = 0; i < arrSelected.length; i++) {
        selectedItemStr = selectedItemStr + "%" + arrSelected[i];
    }

    selectedItemStr = selectedItemStr.slice(1, selectedItemStr.length);

    upadateCookie("cia", selectedItemStr);

    rebuildProductCompareContainer();

    return selectedItemStr;
}


// methof for Cookie update
function upadateCookie(name, value) {
    eraseCookie(name); createCookie(name, value); return readCookie(name)
}


function createCookie(name, value) {
    document.cookie = name + "=" + value + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return "";
}

function eraseCookie(name) {
    createCookie(name, "");
}

// this will rebuild the compare container
//var arrCompareImageUrl = new Array();

function rebuildProductCompareContainer() {

    var selectedItemStr = "";
    var arrSelected = new Array();
    var arrSelectedInt = new Array();


    selectedItemStr = readCookie("cia");

    if (selectedItemStr.length > -1) {
        arrSelected = selectedItemStr.split('%');

        var itemIDToAdd = "";
        arrCompareImageUrl = new Array();

        for (var i = 0; i < arrSelected.length; i++) {
            itemIDToAdd = arrSelected[i];
            if (!itemIDToAdd) { continue; }
            arrSelectedInt.push(parseInt(itemIDToAdd));
        }

        var jsonText = JSON.stringify({ productIDs: arrSelectedInt, includejavascript: false, xmlpackagename: xmlpackage});

        $.ajax({
            type: "POST",
            url: "ActionService.asmx/GetProductCompareImageLinks",
            contentType: "application/json; charset=utf-8",
            data: jsonText,
            dataType: "json",
            success: GetProductCompareImageLinksSucceeded,
            error: GetProductCompareImageLinksFailed
        });
    }
    else { $(comparepanelname).children().remove(); }
}

function GetProductCompareImageLinksSucceeded(result) {
    var returnlist = result.d;
//    setCompareDefaults();
    createMiniCompareTable(returnlist);
}

function GetProductCompareImageLinksFailed(result) {
    //alert(result.status + ' ' + result.statusText);
}

function createMiniCompareTable(linksAndPackage) {

    var selectedItemStr = "";
    var arrSelected = new Array();

    selectedItemStr = readCookie("cia");

    if (selectedItemStr.length > 0) {
        arrSelected = selectedItemStr.split('%');
    }

    var arrCompareImageUrl = new Array();
    var packagearray = new Array();
    var splitimageUrls = new Array();
    var package;

    if (linksAndPackage.length > 0)
    {arrCompareImageUrl = linksAndPackage[0]; }

    if (linksAndPackage.length > 1) {
        packagearray = linksAndPackage[1];
        package = packagearray[0];
    }

    $(comparepanelname).children().remove();
    if (package.length > 0) {
        
        $(comparepanelname).html(package);

        //get image links
        for (var i = 0; i < arrCompareImageUrl.length; i++) {

            var imageUrl = arrCompareImageUrl[i];
            if (!imageUrl) { continue; }

            splitimageUrls = imageUrl.split("\n");
            if (splitimageUrls.length == 0) { continue; }

            //get imageids
            var id = arrSelected[i];
            var imghreflinkid = ImgLinkid + id.toString();
            var imgboxid = Imgid + id.toString();
           
            //get image details
            var imghref = '""';
            var imgsrc = '""';
            var imgalt = '""';
            var imgtitle = '""';
            var j = 0;
            for (j = 0; j < splitimageUrls.length; j++) {
                if (j == 0) { imghref = splitimageUrls[j].split("=")[1]; }
                if (j == 1) { imgsrc = splitimageUrls[j].split("=")[1]; }
                if (j == 2) { imgalt = splitimageUrls[j].split("=")[1]; }
                if (j == 3) { imgtitle = splitimageUrls[j].split("=")[1]; }   
            }

            //assign details
            if (imghref) { $(imghreflinkid).attr('href', imghref.replace("\r", "")); }
            if (imgsrc) { $(imgboxid).attr('src', imgsrc.replace("\r", "")); }
            if (imgalt) { $(imgboxid).attr('alt', imgalt.replace("\r", "")); }
            if (imgtitle) { $(imgboxid).attr('title', imgtitle.replace("\r", "")); }
        }

        //update checkbox
        if (arrSelected.length == 0) { return}
        for (var x = 0; x < arrSelected.length; x++) {
            updateCompareCheckedValue(true,arrSelected[x])
        }
    }

    if (arrSelected.length > 1) { $(CompareNowButtonid).removeAttr('disabled'); }
    else { $(CompareNowButtonid).attr('disabled', 'disabled'); }
}

function updateCompareCheckedValue(checked, id) {
    var elemName = '#chkcom_' + id.toString();

    var elem = $(elemName);
    if (!elem) { return; }

    if (checked) {
        elem.attr('checked', 'checked');
    }
    else {
        elem.removeAttr('checked');
    }
}



// for tab page events
var itemTabDetails = new Array(6);
function setItemTabPageDetails() {

    var panelctr;
    for (var i = 0; i < itemTabDetails.length; i++) {
        panelctr = i + 1;
        itemTabDetails[i] = $("#pagetabcontent_panel_" + panelctr.toString()).html();
        if (!$("#pagetabcontent_panel_" + panelctr.toString()).text()) { itemTabDetails[i] = ""; }
//        $("#pagetabcontent_panel_" + panelctr.toString()).remove();
    }

    $("#pagetabcontent_panel").children().remove();
    hideTabPageHeaders();
}

function hideTabPageHeaders() {
    var hastabheader = false;
    var hasselected = false;
    var selectedtabpageindex = -1;
    for (var k = 0; k < itemTabDetails.length; k++) {
        if (!itemTabDetails[k]) { $("#tabpageheader_" + (k+1).toString()).remove(); continue; }
        else {
            if (!hasselected) { hasselected = true;selectedtabpageindex = k; }
         }
        hastabheader = true;

    }
    if (!hastabheader) { $("itemdetailtabcontorl").hide(); }
    if (selectedtabpageindex > -1) {SelectTabPageHeader(selectedtabpageindex+1); }
}

function SelectTabPageHeader(tabindex) {
    if ($("#pagetabcontent_panel").html()==null) { return; }
    $("#pagetabcontent_panel").children().remove();
    
    var j = 1;
    for (j = 1; j < itemTabDetails.length + 1; j++) {
        SetTabPageHeaderStyle(j, (j == tabindex));
        if (j == tabindex) {
            $("#pagetabcontent_panel").html(itemTabDetails[j - 1]) 
            
            if(tabindex == 6 && $("#tabpageheader_" + tabindex.toString()).html().toLowerCase() == "comments") {
                ShowSocialMediaComments();
            }
        }
    }
}

function SetTabPageHeaderStyle(tabindex, isselected) {
    var selectedStyle = 'border-color: #000000; border-width: 1px; border-style: solid solid none solid; width:160px'
    var unselectedStyle = 'border-color: #000000; border-width: 1px; border-style: solid; width:160px; background-color: Silver;'
    var endlineStyle = 'border-color: #000000; border-width: 1px; border-style: none none solid none; width:'

    if (!$("#tabpageheader_" + tabindex.toString()).html()) { return; }

    if (isselected) {
        $("#tabpageheader_" + tabindex.toString()).attr('style', selectedStyle);
       
    }
    else {
        $("#tabpageheader_" + tabindex.toString()).attr('style', unselectedStyle);

    }

    if (!$("#tabpageheader_endline").html()) { return; }

    var tabpagecount = 0;
    for (var k = 0; k < itemTabDetails.length; k++) {
        if (!itemTabDetails[k]) { tabpagecount = tabpagecount + 1; }
    }
    endlineStyle = endlineStyle + (60 + (60 * tabpagecount)).toString() + 'px';

    $("#tabpageheader_endline").attr('style', endlineStyle);

}

function ShowSocialMediaComments() {
    var d = document;
    var s = "script";
    var id = "facebook-jssdk";

    $("#" + id).remove();
    var js, fjs = d.getElementsByTagName(s)[0];

    if (d.getElementById(id)) return;

    js = d.createElement(s); js.id = id;

    js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";

    fjs.parentNode.insertBefore(js, fjs);
}