function InitImageEditorScript() {

    $(window).load(function () {

        //adjust the body height if body is lessthan 250px
        if ($("body").height() < 250) { $('body').height(500); }

        var curentDocWidth = ($(document).width() - $(document).width() * .20);
        var curentDocHeight = ($(document).height() - $(document).height() * .20);

        $(this).CmsEditor.Initialize({
            contentTag: '.content',
            contentKeyId: "content-key",
            contentValueId: "content-value",
            stringResourceEditorDialogId: 'ise-cms-string-resource-editor',
            stringResourceSavingButtonPlaceHolderId: "saving-button-place-holder",
            stringResourceProgressMessagePlaceHolderId: "saving-string-resource-progress-message-holder",
            stringResourceErrorPlaceHolderId: "saving-string-resource-error-place-holder",
            stringResourceButtonId: "save-string-resource",
            topicEditorDialogId: 'ise-cms-topic-editor',
            topicEditorInputId: 'txtTopicEditorInput',
            topicEditorSaveButtonId: 'btnSaveTopic',
            topicEditorDialogWidth: curentDocWidth,
            topicEditorDialogHeight: curentDocHeight,
            imageEditorDialogId: 'ise-cms-image-editor',
            imageEditorContentImageId: 'source-image-content-holder',
            messages:
                            {
                                MESSAGE_SAVING_PROGRESS: 'Saving...',
                                MESSAGE_SAVING_TOPIC_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.',
                                MESSAGE_OPEN_TOPIC_NOT_SUPPORTED_ERROR: 'Sorry, topic editing is currently not supported in mobile browser.',
                                MESSAGE_SAVING_IMAGE_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.'
                            }
        });

    });


};



var batchcount = 3;
var lastindex;
var maxindex;

function NavigateToFilename(filename) {

    var files = $('#hidselectedFilenameIndex').val();

    if (files.length == 0) { $('#btnNavigator').hide(); return; }

    var arrayOfStrings = files.split('|');
    var count = arrayOfStrings.length;

    if (count <= 1) { $('#btnNavigator').hide(); return; }

    var imageindex = jQuery.inArray(filename, arrayOfStrings) + 1;

    if (imageindex <= 0) { $('#btnNavigator').hide(); return; }

    lastindex = batchcount;
    if (maxindex < batchcount) { lastindex = maxindex; }
    while ($('#btnNavigator' + imageindex.toString()).is(':visible') == false) {
        NavigateBatchImage('>');
    }

};

function NavigateBatchImage(index) {
    var imageindex = index;
    if ((imageindex == '>') && (lastindex < maxindex)) {
        lastindex = lastindex + batchcount;
        for (var i = maxindex; i > 0; i--) {
            $('#btnNavigator' + i.toString()).hide();
        }

        for (var x = lastindex; x > Math.abs(lastindex - batchcount); x--) {
            $('#btnNavigator' + x.toString()).show();
        }
        return index;
    }

    if ((imageindex == '<') && (lastindex > batchcount)) {
        lastindex = lastindex - batchcount;
        for (var i = maxindex; i > 0; i--) {
            $('#btnNavigator' + i.toString()).hide();
        }

        for (var x = lastindex; x > Math.abs(lastindex - batchcount); x--) {
            $('#btnNavigator' + x.toString()).show();
        }
        return index;
    }

    return index;
};

function NavigateImage(index) {
    var imageindex = index;
    if ((imageindex == '>') && (lastindex < maxindex)) {
        lastindex = lastindex + 1;
        for (var i = maxindex; i > 0; i--) {
            $('#btnNavigator' + i.toString()).hide();
        }

        for (var x = lastindex; x > Math.abs(lastindex - batchcount); x--) {
            $('#btnNavigator' + x.toString()).show();
        }
        return index;
    }

    if ((imageindex == '<') && (lastindex > batchcount)) {
        lastindex = lastindex - 1;
        for (var i = maxindex; i > 0; i--) {
            $('#btnNavigator' + i.toString()).hide();
        }

        for (var x = lastindex; x > Math.abs(lastindex - batchcount); x--) {
            $('#btnNavigator' + x.toString()).show();
        }
        return index;
    }

    return index;
};

function DisplayEntityImage(elem) {

    var index = elem.value;

    NavigateBatchImage(index);

    if (Math.floor(index) != index) {
        return index;
    }

    if (index < 0) { return index; }

    var files = $('#hidselectedFilenameIndex').val();
    var arrayOfStrings = files.split('|');
    var count = arrayOfStrings.length;

    if (count < 0 || count < index) { return index; }
    $('#hidFilename').val(arrayOfStrings[index - 1]);

    $('#btnDisplay').click();

    return index;
};

function RefreshImageNavigator() {

    var elem = $('#btnNavigator').clone();
    $('#divNavigator').empty();
    elem.appendTo('#divNavigator');
    AddButton();

    //NavigateToFilename($('#hidFilename').val());

};

function AddButton() {

    var files = $('#hidselectedFilenameIndex').val();

    if (files.length == 0) { $('#btnNavigator').hide(); return; }

    var arrayOfStrings = files.split('|');
    var count = arrayOfStrings.length;

    if (count <= 1) { $('#btnNavigator').hide(); return; }

    //create prev button
    if (count > batchcount) {
        var elem = $('#btnNavigator').clone();
        elem.attr('id', 'btnNavigatorPrev');
        elem.val('<');
        elem.appendTo('#divNavigator');
    }

    //create nav button
    for (var i = 1; i < (count + 1); i++) {
        var elem = $('#btnNavigator').clone();
        elem.attr('id', 'btnNavigator' + i.toString());
        elem.val(i);
        elem.appendTo('#divNavigator');
        if (i > batchcount) {
            elem.hide();
            maxindex = i;
        }
        else { lastindex = i; }
    }

    //create next button
    if (count > batchcount) {
        var elem = $('#btnNavigator').clone();
        elem.attr('id', 'btnNavigatorNext');
        elem.val('>');
        elem.appendTo('#divNavigator');
    }
    $('#btnNavigator').hide();
};

function triggerFileUpload(islarge) {
    var resizeImage = ($("#hidResizeImage").val().toString() == 'true');
    $('#btnUploadTheFile2').hide();
    $('#btnUploadTheFile').show();
    isShowResizeDialog = (resizeImage && islarge);
    $("#hidUploadMode").val('false');

    if ($.browser.msie) {
        DisableContent(true);
    } else {
        $("#FileUploader").click();
    }


    return false;
};

function triggerFileUploadFromButton() {
    $("#hidUploadMode").val('true');
    $('#btnUploadTheFile').hide();
    $('#btnUploadTheFile2').show();
    var resizeImage = ($("#hidResizeImage").val().toString() == 'true');
    isShowResizeDialog = resizeImage;
    if ($.browser.msie) {
        DisableContent(true);
    } else {
        $("#FileUploader").click();
    }

    return false;
};

function FileUpload_OnChange(sender) {

    var isUpload = ($("#hidUploadMode").val().toString() == 'true');

    var showSameFilename = false;
    var arrayOfStrings = $("#FileUploader").val().toString().toLowerCase().split('\\');
    var count = arrayOfStrings.length;
    var filename;
    var files;
    if (count > 0) {
        filename = arrayOfStrings[count - 1];
        filename = FixFilename(filename);
        files = $('#hidselectedFilenameIndex').val().toLowerCase();

        if (filename.length > 0) {
            var isValidFileType = ValidateFileType(filename);
            if (isValidFileType == false) {
                showInvalidFileTypeDialog(filename);
                $("#FileUploader").val('');
                return false;
            }
        }
    }

    if (isUpload) {
        if (count > 0) {
            if (files.length > 0 && filename.length > 0) {
                var arrayOfStrings = files.split('|');
                var count = arrayOfStrings.length;

                if (count >= 1) {
                    var imageindex = jQuery.inArray(filename, arrayOfStrings) + 1;
                    if (imageindex >= 1) {
                        showSameFilename = true;
                    }
                }
            }

        }
    }

    if (showSameFilename) {
        showSameFilenameDialog();
        return false;
    }


    if (isShowResizeDialog) {
        showResizeDialog();
    }
    else {
        DialogResizeCallBack();
    }
    return false;
};

function SaveChanges() {
    $("#Uploader").click();
};

function DialogResizeCallBack() {

    if ($.browser.msie) {
        $('#hidResize').val($("#hidResizeImage").val());
        DisableContent(true);
    }
    else {
        SaveChanges();
        RefreshImageNavigator();
    }

};

function DisableContent(disable) {
    if (disable) {
        $('#Uploader').val('');
        $("#UploadDialog").show();
        $('#MainDialog').find('a, ul, input, textarea, button, select').attr('disabled', 'disabled');
    }
    else {
        $("#UploadDialog").hide();
        $('#MainDialog').find('a, ul, input, textarea, button, select').removeAttr('disabled');
    }

};

function DialogDeleteCallBack() {
    $("#btnDeleter").click();
    RefreshImageNavigator();
};

function showResizeDialog() {

    // call uplaod method here.
    $("#dialog-confirm-message").text('Do you want to use the same image for all sizes?');

    $("#dialog-confirm").dialog({
        resizable: false,
        height: 200,
        modal: true,
        buttons: {
            "Yes": function () {
                $(this).dialog("close");
                $("#hidResizeImage").val('true');
                DialogResizeCallBack();
            },
            "No": function () {
                $(this).dialog("close");
                $("#hidResizeImage").val('false');
                DialogResizeCallBack();

            },
            Cancel: function () {
                $(this).dialog("close");
                if ($.browser.msie) { DisableContent(true); }
            }
        },
        open: function () {
            $("#dialog-confirm").css("visibility", "visible");
            $("#dialog-confirm-message").text('Do you want to use the same image for all sizes?');
        }
    });

};


function showDeleteDialog() {

    // call uplaod method here.
    $("#dialog-confirm-message").text('Are you sure you want to delete this image?');

    $("#dialog-confirm").dialog({
        resizable: false,
        height: 200,
        modal: true,
        buttons: {
            "Yes": function () {
                $(this).dialog("close");
                DialogDeleteCallBack();
            },
            "No": function () {
                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");

            }
        },
        open: function () {
            $("#dialog-confirm-message").text('Are you sure you want to delete this image?');
            $("#dialog-confirm").css("visibility", "visible");
        }
    });

    return false;
};


function DialogSameFilenameCallBack() {
    if ($.browser.msie) {
        DisableContent(false);
        $("#FileUploader").val('');
    }
    // RefreshImageNavigator();
};

function showSameFilenameDialog() {

    var filename = $("#FileUploader").val().toString().split(/[, ]+/).pop();
    var arrayOfStrings = $("#FileUploader").val().toString().split('\\');
    var count = arrayOfStrings.length;

    if (count > 0) {
        filename = arrayOfStrings[count - 1];
        filename = FixFilename(filename);
    }

    var message = 'The filename: "' + filename + '" is already in use for this item';
    $("#dialog-confirm-message").text(message);

    $("#dialog-confirm").dialog({
        resizable: false,
        height: 200,
        modal: true,
        buttons: {
            "OK": function () {
                $(this).dialog("close");
                DialogSameFilenameCallBack();
            }
        },
        open: function () {
            $("#dialog-confirm-message").text(message);
            $("#dialog-confirm").css("visibility", "visible");
        }
    });

    return false;
};

function FixFilename(filename) {
    return filename.replace(/([^A-Za-z0-9._()!-])/g, '');
};

function ValidateFileType(filename) {
    var isValid = false;

    var ext = filename.substring(filename.lastIndexOf('.') + 1);
    if (ext == undefined) { return false; }
    //".jpg", ".gif", ".png" 
    if (ext == "gif" || ext == "GIF" || ext == "JPEG" || ext == "jpeg" || ext == "jpg" || ext == "JPG" || ext == "png" || ext == "PNG") {
        isValid = true;
    }
    else {
        isValid = false;
    }

    return isValid;
};


function showInvalidFileTypeDialog(filename) {

    var message = 'The filename: "' + filename + '" is NOT a valid image file.';
    $("#dialog-confirm-message").text(message);

    $("#dialog-confirm").dialog({
        resizable: false,
        height: 200,
        modal: true,
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        },
        open: function () {
            $("#dialog-confirm-message").text(message);
            $("#dialog-confirm").css("visibility", "visible");
        }
    });

    return false;
};