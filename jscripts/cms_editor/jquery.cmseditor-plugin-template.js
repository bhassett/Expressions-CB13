$.template(
        "stringResourceEditorTemplateID",
        "<div>" +
            "<div title='Edit String Resource' id='ise-cms-string-resource-editor' style='display:none'>" +
            "</div>" +
        "</div>"
    );

$.template(
        "stringResourceEditorContentTemplateID",
        "<div>" +
            "<div id='saving-string-resource-error-place-holder' class='alert alert-error display-none'></div>" +
                "<div id='dialog-content-wrapper'>" +
                "<div class='clear-both height-12' ></div>" +
                "<div class='float-left' id='string-key-caption'>Key:</div>" +
                "<div class='float-left'>" +
                    "<input disabled='disabled' type='text' id='content-key' value='${ContentKey}' class='light-style-input'/>" +
                "</div>" +
                "<div class='clear-both height-12' ></div>" +
                "<div class='float-left' id='string-value-caption'>Value:</div>" +
                "<div class='float-left'><textarea style='width:250px;' id='content-value' class='light-style-input'>${ContentValue}</textarea></div>" +
                "<div class='clear-both height-12'></div>" +
                "<div id='warning-placeholder'>Warning: Do not remove defined tags on string resource value: ex. {0} {1} etc.</div>" +
                "<div class='clear-both'></div>" +
             "</div>" +
             "<div class='clear-both height-12'></div>" +
             "<div id='string-resource-save-button-place-holder'>" +
                "<div id='saving-string-resource-progress-message-holder' class='float-right'></div>" +
                "<div id='saving-button-place-holder' class='float-right'>" +
                    "<input type='button' class='btn' id='cancel-string-resource' value='Cancel'/>&nbsp;" +
                    "<input type='button' class='btn btn-primary' id='save-string-resource' value='Save'/>" +
                "</div>" +
                "<div class='clear-both'></div>" +
              "</div>" +
        "</div>"
    );

$.template(
        "topicEditorTemplateID",
        "<div>" +
            "<div title='Edit Topic' id='ise-cms-topic-editor' style='display:none'>" +
            "</div>" +
        "</div>"
    );

$.template(
        "topicEditorContentTemplateID",
        "<div>" +
            "<div id='saving-topic-error-place-holder' class='alert alert-error display-none'></div>" +
             "<div id='dialog-content-wrapper'>" +
                "<div class='clear-both height-12' ></div>" +
                "<div class='float-left' id='string-key-caption'>Key:</div>" +
                "<div class='float-left'>" +
                    "<input disabled='disabled' type='text' id='content-key' value='${ContentKey}' class='light-style-input'/>" +
                "</div>" +
                "<div class='clear-both height-12'></div>" +
                "<div class='float-left; width:595px'>" +
                    "<textarea id='txtTopicEditorInput' class='light-style-input'>" +
                    "</textarea>" +
                "</div>" +
                  "<div class='clear-both height-12'></div>" +
             "</div>" +
            "<div class='clear-both'></div>" +
                "<div id='string-resource-save-button-place-holder' style='padding-right: 7px;'>" +
                    "<div id='saving-string-resource-progress-message-holder' class='float-right'></div>" +
                    "<div id='saving-button-place-holder' class='float-right'>" +
                        "<input type='button' class='btn' id='cancel-topic' value='Cancel'/>&nbsp;" +
                        "<input type='button' class='btn btn-primary' id='btnSaveTopic' value='Save'/>" +
                    "</div>" +
                     "<div class='clear-both'></div>" +
                "</div>" +
        "</div>"
    );

$.template(
        "cmsEditorToolbar",
        "<div id='cms-user-panel'>" +
            "<div class='cms-user-panel-command'>" +
                "<input type='button' class='site-button' id='cms-user-panel-command-button' data-active='true' value='Swich to Browse Mode'" +
                "onclick='ProcessCmsPanelButton(this);' />" +
            "</div>" +
        "</div>"
    );

$.template(
        "imageEditorTemplateID",
        "<div>" +
            "<div title='Change Photo' id='ise-cms-image-editor' style='display:none'>" +
            "</div>" +
        "</div>"
    );

$.template(
        "itemDescriptionTemplateID",
        "<div>" +
            "<div title='Edit Item Description' id='ise-cms-item-description-editor' style='display:none'></div>" +
        "</div>"
    );

$.template(
        "itemDescriptionEditorContentTemplateID",
        "<div>" +
            "<div id='saving-item-description-error-place-holder' class='alert alert-error display-none'></div>" +
                "<div id='dialog-content-wrapper'>" +
                "<div class='clear-both height-12' ></div>" +
                "<div class='float-left'>" +
                    "<input disabled='disabled' type='text' id='item-description-item-code' value='${ContentKey}' class='light-style-input display-none'/>" +
                "</div>" +
                "<div class='float-left' id='string-value-caption' style='width:66px'>Value:</div>" +
                "<div class='float-left'>" +
                "<textarea style='width:249px;' id='item-description-content-value' class='light-style-input'>${ContentValue}</textarea>" +
                "</div>" +
                "<div class='clear-both'></div>" +
             "</div>" +
             "<div class='clear-both height-5'></div>" +
             "<div id='item-description-save-button-place-holder'>" +
                "<div id='saving-item-description-progress-message-holder' class='float-right'></div>" +
                "<div id='saving-button-place-holder' class='float-right'>" +
                    "<input type='button' class='btn' id='cancel-item-description' value='Cancel'/>&nbsp;" +
                    "<input type='button' class='btn btn-primary' id='save-item-description' value='Save'/>" +
                "</div>" +
                "<div class='clear-both height-5'></div>" +
              "</div>" +
        "</div>"
    );


$.template(
        "itemWebDescriptionTemplateID",
        "<div>" +
            "<div title='Edit Item Web Description' id='ise-cms-item-web-description-editor' style='display:none'></div>" +
        "</div>"
    );

$.template(
        "itemWebDescriptionEditorContentTemplateID",
        "<div>" +
            "<div id='saving-item-web-description-error-place-holder' class='alert alert-error display-none'></div>" +
                "<div id='dialog-content-wrapper'>" +
                "<div class='float-left'>" +
                    "<input disabled='disabled' type='text' id='item-web-description-item-code' value='${ContentKey}' class='light-style-input display-none'/>" +
                "</div>" +
                "<div class='float-left'>" +
                "<textarea id='item-web-description-content-value' class='light-style-input'>${ContentValue}</textarea>" +
                "</div>" +
                "<div class='clear-both'></div>" +
             "</div>" +
             "<div class='clear-both height-5'></div>" +
             "<div id='item-web-description-save-button-place-holder'>" +
                "<div id='saving-item-web-description-progress-message-holder' class='float-right'></div>" +
                "<div id='saving-button-place-holder' class='float-right'>" +
                    "<input type='button' class='btn' id='cancel-item-web-description' value='Cancel'/>&nbsp;" +
                    "<input type='button' class='btn btn-primary' id='save-item-web-description' value='Save'/>" +
                "</div>" +
                "<div class='clear-both height-5'></div>" +
              "</div>" +
        "</div>"
    );

$.template(
        "uploadMainTemplateIDNonIE",
        "<div id='MainDialog'>" +
            "<div id='image-editor-tab' style='border: solid 1px #ccc;' class='site-tab'>" + 
                "<ul class='site-tab'>" +
                    "<li><a href='#tab-panel-1' class='normal-font-12'>Large</a></li>" +
                    "<li><a href='#tab-panel-2' class='normal-font-12'>Medium</a></li>" +
                    "<li><a href='#tab-panel-3' class='normal-font-12'>Icon</a></li>" +
                    "<li><a href='#tab-panel-4' class='normal-font-12'>Mini Cart</a></li>" +
                    "<li><a href='#tab-panel-5' class='normal-font-12'>Mobile</a></li>" +
                "</ul>" +
                "<div id='tab-panel-1' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageLarge' class='upload-image' alt='' />" +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                        "<span style='heigth:40px;'>&nbsp;</span>" +
                    "</div>" +
                "</div>" +
                "<div id='tab-panel-2' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageMedium' class='upload-image' alt='' />" +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                        "<label for='togglecheck2' class='btn btn-primary'>${defaultSetImageSizeActiveText}</label>" +
                        "<input id='togglecheck2' type='checkbox' style='display:none' />" +
                    "</div>" +
                "</div>" +
                "<div id='tab-panel-3' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageIcon' class='upload-image' alt='' class='mainimage-icon' />" +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                        "<label for='togglecheck3' class='btn btn-primary'>${defaultSetImageSizeActiveText}</label>" +
                        "<input id='togglecheck3' type='checkbox' style='display:none' />" +
                    "</div>" +
                "</div>" +
                "<div id='tab-panel-4' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageMiniCart' class='upload-image' alt='' class='mainimage-minicart' /> " +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                    "</div>" +
                "</div>" +
                "<div id='tab-panel-5' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageMobile' class='upload-image' alt='' class='mainimage-mobile' />" +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'><span>&nbsp;</span>" +
                    "</div>" +
                "</div>" +
            "</div>" +
            "<div class='upload-button-panel'>" +
                "<div class='upload-page-button-wrapper'>" +
                    "<ul id='upload-page-ul'>" + 
                    "</ul>" +
                "</div>" +
                "<div class='upload-button-wrapper'>" +
                    "<input type='button' id='btnImageUploadCancel' class='btn' value='Close'/>" +
                    "<input type='button' id='btnImageUploadDelete' value='Delete' class='btn btn-primary fancy-button-leftspace' />" +
                    "<input type='button' id='btnEditImage' value='Change' class='btn btn-primary fancy-button-leftspace' />" +
                    "<input type='button' id='btnUpload' value='Add' class='btn btn-primary fancy-button-leftspace' />" +
                "</div>" +
            "</div>" +
            "<form id='ctrlUploadForm' name='ctrlUploadForm' method='post' enctype='multipart/form-data' style='position:absolute; top:-1000000px; left:-99999px;' >" +
                "<input id='ctrlFileUpload' name='ctrlFileUpload' type='file' class='btn' style='opacity: 0; filter:alpha(opacity: 0); -moz-opacity: 0.00;' />" +
                "<input type='submit' id='btnHiddenUploadSubmit' name='btnHiddenUploadSubmit' style='display:none' />" +
            "</form>" +
        "</div>"
    );

$.template(
        "uploadMainTemplateIDForIE",
        "<div id='MainDialog'>" +
            "<div id='image-editor-tab' style='border: solid 1px #ccc;' class='site-tab'>" +
                "<ul class='site-tab'>" +
                    "<li><a href='#tab-panel-1' class='normal-font-12'>Large</a></li>" +
                    "<li><a href='#tab-panel-2' class='normal-font-12'>Medium</a></li>" +
                    "<li><a href='#tab-panel-3' class='normal-font-12'>Icon</a></li>" +
                    "<li><a href='#tab-panel-4' class='normal-font-12'>Mini Cart</a></li>" +
                    "<li><a href='#tab-panel-5' class='normal-font-12'>Mobile</a></li>" +
                "</ul>" +
                "<div id='tab-panel-1' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageLarge' class='upload-image' alt='' />" +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                    "</div>" +
                "</div>" +
                "<div id='tab-panel-2' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageMedium' class='upload-image' alt='' />" +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                        "<label for='togglecheck2' class='btn btn-primary'>${defaultSetImageSizeActiveText}</label>" +
                        "<input id='togglecheck2' type='checkbox' style='display:none' />" +
                    "</div>" +
                "</div>" +
                "<div id='tab-panel-3' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageIcon' class='upload-image' alt='' class='mainimage-icon' />" +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                        "<label for='togglecheck3' class='btn btn-primary'>${defaultSetImageSizeActiveText}</label>" +
                        "<input id='togglecheck3' type='checkbox' style='display:none' />" +
                    "</div>" +
                "</div>" +
                "<div id='tab-panel-4' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageMiniCart' class='upload-image' alt='' class='mainimage-minicart' /> " +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                    "</div>" +
                "</div>" +
                "<div id='tab-panel-5' class='site-tab'>" +
                    "<div class='upload-image-wrapper site-tab'>" +
                        "<a href='' class='cloud-zoom'>" +
                            "<img id='imageMobile' class='upload-image' alt='' class='mainimage-mobile' />" +
                        "</a>" +
                        "<div class='upload-image-size-dimension-container light-style-input'>" +
                            "<label></label>" +
                        "</div>" +
                    "</div>" +
                    "<div class='upload-image-size-default-button-container'>" +
                    "</div>" +
                "</div>" +
            "</div>" +
            "<div class='upload-button-panel'>" +
                "<div class='upload-page-button-wrapper'>" +
                    "<ul id='upload-page-ul'>" +
                    "</ul>" +
                "</div>" +
                "<div class='upload-button-wrapper'>" +
                    "<form id='ctrlUploadForm' name='ctrlUploadForm' method='post' enctype='multipart/form-data' style='position:relative'>" +
                        "<input type='button' id='btnImageUploadCancel' class='btn' value='Close'/>" +
                        "<input type='button' id='btnImageUploadDelete' value='Delete' class='btn btn-primary fancy-button-leftspace' />" +

                        "<input type='button' id='btnEditImage' value='Change' class='btn btn-primary fancy-button-leftspace' />" +
                        "<label class='label-changeupload-wrapper' >" +
                            "<input id='ctrlChangeUpload' name='ctrlChangeUpload' type='file' class='btn image-file-upload' />" +
                        "</label>" +

                        "<input type='button' id='btnUpload' value='Add' class='btn btn-primary fancy-button-leftspace' />" +
                        "<label class='label-addupload-wrapper'>" +
                            "<input id='ctrlFileUpload' name='ctrlFileUpload' type='file' class='btn image-file-upload' />" +
                        "</label>" +

                        "<input type='submit' id='btnHiddenUploadSubmit' name='btnHiddenUploadSubmit' style='display:none' />" +
                    "</form>" +
                "</div>" +
            "</div>" +
        "</div>"
    );

$.template(
        "uploadPageButtonTemplateID",
        "<li><input type='button' id='btnPage_${PageNumber}' page='${PageNumber}' value='${PageNumber}' class='btn btn-primary' /></li>"
    );

$.template(
        "previousPageButtonTemplateID",
        "<li><input type='button' id='btnImageUploadPrevious' prev-page='${PageNumber}' value='|<' class='btn btn-primary' /></li>"
    );

$.template(
        "nextPageButtonTemplateID",
        "<li><input type='button' id='btnImageUploadNext' next-page='${PageNumber}' value='>|' class='btn btn-primary' /></li>"
    );

$.template(
        "backgroundLoadingTemplateID",
        "<div>" +
            "<div class='cms-plugin-progress-bar' style='display:none'>" +
                "<span>Processing Image... Please wait</span>" +
            "</div>" +
        "</div>"
    );