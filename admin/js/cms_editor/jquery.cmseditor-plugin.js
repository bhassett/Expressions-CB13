/* Plugin Requirements

1. Must only be triggered if admin is currently logged in and site is on edit mode
2. Elements good for edit should have the following attributes:

A. data-contentKey
B. data-contentValue
C. data-contentType (string resource, topic, or image)
D. data-contentHasTag (optional)
E. must contain a class: content
   
3. All updates via ajax must use AjaxCallWithSecuritySimplified method
   
*/

(function ($) {

    var thisCMSEditorPlugin;

    var config = {};

    var global = {
        selected: '',
        selector: ''
    };

    var currentSelectedControl = null;

    var imageUploadCurrentActiveTabIndex = 1;
    var loadedImages = null;
    var ajaxFormUploadOption = null;
    var hasUploadImageModification = false;
    var editorImageControl = null;
    var currentImageSource = null;

    var defaultImageUploadPageCount = 4;
    var totalImageUploadPages = 0;
    var currentImageUploadPage = 1;

    var editorConstants = {
        EMPTY_VALUE: '',
        DOT_VALUE: '.'
    };

    var tabEnum = {
        large: 0,
        medium: 1,
        icon: 2,
        minicart: 3,
        mobile: 4
    };

    var uploadType = {
        upload: 0,
        edit: 1,
        remove: 2,
        statusChange: 3
    };

    var pageDirectionEnum = {
        previous: 0,
        next: 1
    };

    //the item information once user click the image (item code, item type)
    var currentlySelectedImageItem = null;

    var defaults = {
        contentTag: '.content',
        contentKeyId: "content-key",
        contentValueId: "content-value",

        // string resource editor:

        stringResourceEditorDialogId: 'ise-cms-string-resource-editor',
        stringResourceSavingButtonPlaceHolderId: "saving-button-place-holder",
        stringResourceProgressMessagePlaceHolderId: "saving-string-resource-progress-message-holder",
        stringResourceErrorPlaceHolderId: "saving-string-resource-error-place-holder",
        stringResourceButtonId: "save-string-resource",
        stringResourceCloseDialogId: "cancel-string-resource",

        // topic editor :
        topicEditorDialogId: 'ise-cms-topic-editor',
        topicEditorInputId: 'txtTopicEditorInput',
        topicEditorSaveButtonId: 'btnSaveTopic',
        topicEditorCloseDialogId: "cancel-topic",
        topicEditorDialogWidth: 763,
        topicEditorDialogHeight: 250,

        // image editor ;

        imageEditorDialogId: 'ise-cms-image-editor',
        imageEditorContentImageId: 'source-image-content-holder',
        imageEditorCancel: 'btnCancelImageUploader',

        // item description editor :

        itemDescriptionItemCode: "item-description-item-code",
        itemDescriptionContentValue: "item-description-content-value",
        itemDescriptionEditorDialogId: "ise-cms-item-description-editor",
        itemDescriptionEditorButtonId: "save-item-description",
        itemDescriptionSavingButtonPlaceHolderId: "saving-item-description-button-holder",
        itemDescriptionProgressMessagePlaceHolderId: "saving-item-description-progress-message-holder",
        itemDescriptionErrorPlaceHolderId: "saving-item-description-error-place-holder",
        itemDescriptionEditorCancel: "cancel-item-description",

        // item web description editor :

        itemWebDescriptionItemCode: "item-web-description-item-code",
        itemWebDescriptionContentValue: "item-web-description-content-value",
        itemWebDescriptionEditorDialogId: "ise-cms-item-web-description-editor",
        itemWebDescriptionEditorButtonId: "save-item-web-description",
        itemWebDescriptionSavingButtonPlaceHolderId: "saving-item-web-description-button-holder",
        itemWebDescriptionProgressMessagePlaceHolderId: "saving-item-web-description-progress-message-holder",
        itemWebDescriptionErrorPlaceHolderId: "saving-item-web-description-error-place-holder",
        itemWebDescriptionEditorCancel: "cancel-item-web-description",
        itemWebDescriptionUIDialogTitleId: "ui-dialog-title-ise-cms-item-web-description-editor",

        thirdPartyAttributeForConflictID: "cms-3rdparty-attr",
        imageEditorTabId: "image-editor-tab",
        tabPrefixName: "tab-panel",
        jqueryFormPluginPath: "jscripts/jquery/jquery.form.js",
        cmsEditorCssPath: "jscripts/cms_editor/css/index.css",
        defaultSetImageSizeActiveText: 'Set as default',
        defaultSetImageSizeInActiveText: 'Default',

        messages:
        {
            MESSAGE_SAVING_PROGRESS: 'Saving...',
            MESSAGE_SAVING_TOPIC_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.',
            MESSAGE_OPEN_TOPIC_NOT_SUPPORTED_ERROR: 'Sorry, topic editing is currently not supported in mobile browser.',
            MESSAGE_SAVING_IMAGE_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.'
        }

    };

    var KeyValue = function (key, value) {
        this.ContentKey = key;
        this.ContentValue = value;
    };

    var KeyValueEntity = function (key, value, entity) {
        this.ContentKey = key;
        this.ContentValue = value;
        this.ContentEntity = entity;
    };

    var init = $.prototype.init;
    $.prototype.init = function (selector, context) {
        var r = init.apply(this, arguments);
        if (selector && selector.selector) {
            r.context = selector.context, r.selector = selector.selector;
        }
        if (typeof selector == 'string') {
            r.context = context || document, r.selector = selector;
            global.selector = r.selector;
        }
        global.selected = r;
        return r;
    }

    $.prototype.init.prototype = $.prototype;

    $.fn.CmsEditor = {

        Initialize: function (options) {

            if (options) { setConfig($.extend(defaults, options)); }

            thisCMSEditorPlugin = this;

            this.lookForEditableContents();
            this.turnOfEventHandlers();

            this.attachEventsListener();
            this.initializeEditorDialogs();

            this.handleConflicts();

        },

        lookForEditableContents: function () {

            /* Summary:

            lookForEditableContents() does the following:

            1. Scan all html elements with class .content
            2. Set read only attributes for text and textarea
            3. Add an attribute class: editable content to all elements tagged as editable
            4. Handles case of string resource being used as behind caption of textbox or textarea

            */

            var config = getConfig();
            $(config.contentTag).each(function () {

                var $this = $(this);
                var contentType = $this.attr("data-contentType");

                if ($this.attr("type") == "text") $this.attr("readonly", true);

                if ($this.attr("type") == "button" || $this.attr("type") == "submit") {

                    thisCMSEditorPlugin.handleInputButtonStringResource($this);
                }

                if (typeof (contentType) != "undefined") {

                    switch (contentType) {
                        case "string resource":

                            thisCMSEditorPlugin.handleStringResourceInsideALabel($this);

                            if (typeof ($this.parent("a").parent("li").html()) == "undefined") {
                                $this.parent("a").replaceWith($this.parent("a").html() + "&nbsp;");
                            }


                            break;
                        case "topic":



                            break;
                        case "image":
                            $this.addClass("editable-content-image");
                            $this.parent("a").replaceWith($this.parent("a").contents());


                            break;
                        default:
                            break;
                    }
                }
            });
        },

        turnOfEventHandlers: function () {

            /* Summary:

            TurnOfEventHandlers() does the following:

            1. Override click event of all anchor elements
            2. Remove cloud-zoom class from all anchor elements on product image
            3. Disable Select Control
          
            */

            //Commented to activate the close button of cloud zoom inside image upload
            //$('a').unbind('click');

            //$('a').click(function (e) { e.preventDefault(); }).removeClass("cloud-zoom").removeAttr("title");
            $('a').removeAttr("onclick");

            //  $('select').attr("disabled", "disabled").addClass("control-disabled").css("color", "#ccc"); has cache problem

            $(".cart-delete-custom").click(function (e) { e.preventDefault(); }); // <- a work around for delete button on shopping cart

            $("a#shopping-cart").addClass("page-is-on-edit-mode");

        },

        attachEventsListener: function () {

            /* Summary:

            attachEventsListener() does the following:

            1. Initialize click event of all elements with attribute class: content and string resource save button
            2. Onclick event of an element with class attribute:content calls this plugin EditContent function
            3. Onclick event of string resource save button with an id of: save-string-resource calls this plugin UpdateStringResource function

            */

            var config = getConfig();

            //unbind any click event so it will not append
            $(this.selectorChecker(config.contentTag)).unbind('click');
            $(this.selectorChecker(config.contentTag)).click(function () {
                thisCMSEditorPlugin.editContent($(this));
            });

            //unbind any click event so it will not append
            $(this.selectorChecker(config.stringResourceButtonId)).unbind('click');
            $(this.selectorChecker(config.stringResourceButtonId)).click(function () {
                thisCMSEditorPlugin.updateStringResource();
            });

            //unbind any click event so it will not append
            $(this.selectorChecker(config.topicEditorSaveButtonId)).unbind('click');
            $(this.selectorChecker(config.topicEditorSaveButtonId)).click(function () {
                thisCMSEditorPlugin.updateTopic();
            });

            //unbind any click event so it will not append
            $(this.selectorChecker(config.itemDescriptionEditorButtonId)).unbind('click');
            $(this.selectorChecker(config.itemDescriptionEditorButtonId)).click(function () {
                thisCMSEditorPlugin.updateItemDescription();
            });


            //unbind any click event so it will not append
            $(this.selectorChecker(config.itemWebDescriptionEditorButtonId)).unbind('click');
            $(this.selectorChecker(config.itemWebDescriptionEditorButtonId)).click(function () {
                thisCMSEditorPlugin.updateItemWebDescription();
            });


            //unbind any click event so it will not append
            $(this.selectorChecker(config.stringResourceCloseDialogId)).unbind('click');
            $(this.selectorChecker(config.stringResourceCloseDialogId)).click(function () {
                $(thisCMSEditorPlugin.selectorChecker(config.stringResourceEditorDialogId)).dialog("close");
            });

            //unbind any click event so it will not append
            $(this.selectorChecker(config.topicEditorCloseDialogId)).unbind('click');
            $(this.selectorChecker(config.topicEditorCloseDialogId)).click(function () {
                $(thisCMSEditorPlugin.selectorChecker(config.topicEditorDialogId)).dialog("close");
            });

            $(this.selectorChecker(config.imageEditorCancel)).unbind("click");
            $(this.selectorChecker(config.imageEditorCancel)).click(function () {
                $(thisCMSEditorPlugin.selectorChecker(config.imageEditorDialogId)).dialog("close");
            });


            $(this.selectorChecker(config.itemDescriptionEditorCancel)).unbind("click");
            $(this.selectorChecker(config.itemDescriptionEditorCancel)).click(function () {
                $(thisCMSEditorPlugin.selectorChecker(config.itemDescriptionEditorDialogId)).dialog("close");
            });

            $(this.selectorChecker(config.itemWebDescriptionEditorCancel)).unbind("click");
            $(this.selectorChecker(config.itemWebDescriptionEditorCancel)).click(function () {
                $(thisCMSEditorPlugin.selectorChecker(config.itemWebDescriptionEditorDialogId)).dialog("close");
            });

        },

        editContent: function (control) {

            /* Summary:

            editContent() does the following:

            1. Get content type, key and value from the attributes attached to an element with content being edited
            2. Calls this pluging ShowEditorDialogs function

            */

            var contentType = control.attr("data-contentType");
            var contentKey = control.attr("data-contentKey");
            var contentValue = control.attr("data-contentValue");
            var contentHasTag = control.attr("data-contentHasTag");

            if (contentType == "item-description" || contentType == "item-webdescription" || contentType == "item-summary" || contentType == "item-warranty") contentKey = control.attr("data-itemCode");

            // handles string resource with tag like {0} {1} 

            if (typeof (contentHasTag) != "undefined" && contentHasTag.toLowerCase() == "true") {

                var callBack = function () {

                    contentValue = ise.StringResource.getString(contentKey);
                    thisCMSEditorPlugin.showEditorDialogs(contentType, contentKey, contentValue);
                }

                this.loadStringResource(contentKey, callBack);

            } else {

                currentSelectedControl = control;
                this.showEditorDialogs(contentType, contentKey, contentValue);

            }
        },

        handleTinyMceNotSupportedForMobile: function () {
            var config = getConfig();
            if (tinyMCE.activeEditor == null) { alert(config.messages.MESSAGE_OPEN_TOPIC_NOT_SUPPORTED_ERROR); }
        },

        initializeEditorDialogs: function () {

            //loads the jquery tempplate and
            //attach the string resource editor dialog container to the body
            var template = this.parseTemplateReturnHtml("stringResourceEditorTemplateID", null);
            $(template).appendTo("body");

            template = this.parseTemplateReturnHtml("backgroundLoadingTemplateID", null);
            $(template).appendTo("body");

            //loads the jquery tempplate and
            //attach the string resource editor dialog container to the body
            template = this.parseTemplateReturnHtml("topicEditorTemplateID", null);
            $(template).appendTo("body");

            //loads the jquery tempplate and
            //attach the image editor dialog container to the body
            template = this.parseTemplateReturnHtml("imageEditorTemplateID", null);
            $(template).appendTo("body");

            template = this.parseTemplateReturnHtml("itemDescriptionTemplateID", null);
            $(template).appendTo("body");

            template = this.parseTemplateReturnHtml("itemWebDescriptionTemplateID", null);
            $(template).appendTo("body");
        },

        showEditorDialogs: function (type, key, value) {

            /* Summary:

            showEditorDialogs() does the following:

            1. Open editor modal dialog for specific types of content being edited
            2. Gets the requirements (attribute values or key) needed to get or save of contents details

            */

            //return immediately when satisfied
            if (type == undefined) return;

            var config = getConfig();

            var dialogId = '';
            var template = null;
            var option = null;

            switch (type.toLowerCase()) {

                case "string resource":

                    //set the dialog for the string resource
                    dialogId = this.selectorChecker(config.stringResourceEditorDialogId);

                    //create the content using the keyvalue pair
                    template = this.parseTemplateReturnHtml("stringResourceEditorContentTemplateID", new KeyValue(key, value));

                    //clear the dialog content holder
                    $(dialogId).html('');

                    //replace with new data from template
                    $(template).appendTo($(dialogId));

                    //set the dialog option layout
                    option = {
                        autoOpen: true,
                        width: 350,
                        modal: true,
                        resize: false,
                        resizable: false,
                        position: "center"
                    }

                    this.attachEventsListener();

                    break;
                case "topic":

                    //set the dialog for the string resource
                    dialogId = this.selectorChecker(config.topicEditorDialogId);

                    //create the content using the keyvalue pair
                    template = this.parseTemplateReturnHtml("topicEditorContentTemplateID", new KeyValue(key, null));

                    //clear the dialog content holder
                    $(dialogId).html('');

                    //replace with new data from template
                    $(template).appendTo($(dialogId));

                    //set the dialog option layout
                    option = {
                        autoOpen: false,
                        width: 763,
                        modal: true,
                        resizable: false,
                        create: function () {

                            var initSuccessfulCallBack = function () {
                                thisCMSEditorPlugin.attachEventsListener();
                                //auto focus dialog when window is resized or scrolled
                                thisCMSEditorPlugin.attachAutoScrollOnDialog(dialogId);
                                $(dialogId).dialog("open");
                            }

                            thisCMSEditorPlugin.initTinyMceForControl(initSuccessfulCallBack, config.topicEditorInputId, true);

                        },
                        close: function (event, ui) {
                            tinyMCE.execCommand('mceRemoveControl', true, config.topicEditorInputId);
                            $(dialogId).dialog("destroy");
                        }
                    }

                    break;
                case "image":

                    currentImageSource = currentSelectedControl.attr("src");

                    var entitytype = currentSelectedControl.attr("data-contentEntityType");
                    var counter = currentSelectedControl.attr("data-contentCounter");
                    var index = currentSelectedControl.attr("data-contentindex");

                    //set the dialog for the image
                    dialogId = this.selectorChecker(config.imageEditorDialogId);

                    var textParam = new Object();
                    textParam.defaultSetImageSizeActiveText = config.defaultSetImageSizeActiveText;
                    textParam.defaultSetImageSizeInActiveText = config.defaultSetImageSizeInActiveText;

                    if (!$.browser.msie) {
                        template = this.parseTemplateReturnHtml("uploadMainTemplateIDNonIE", textParam);
                    } else {
                        template = this.parseTemplateReturnHtml("uploadMainTemplateIDForIE", textParam);
                    }

                    //clear the dialog content holder
                    $(dialogId).html('');

                    //get current image to editor control
                    editorImageControl = this.selectorChecker(config.imageEditorContentImageId);

                    //replace with new data from template
                    $(template).appendTo($(dialogId));

                    this.resetUploadUptions();
                    this.initializeTabPanel(entitytype);

                    var selectedImageItem = new Object();
                    selectedImageItem.counter = counter;
                    selectedImageItem.itemCode = key;
                    selectedImageItem.itemType = entitytype;
                    selectedImageItem.matrixGroupCounter = 0;
                    this.setSelectedImageItem(selectedImageItem);

                    var dialogWidth = 650;
                    if ($.browser.msie && $.browser.version == '7.0') {
                        dialogWidth = 660;
                    }

                    option = {
                        autoOpen: true,
                        width: dialogWidth,
                        modal: true,
                        resize: false,
                        resizable: false,
                        open: function () {
                            //support if jquery.form.js plugin is already downloaded ouside this plugin. Just execute the image loading
                            if (config.jqueryFormPluginPath != editorConstants.EMPTY_VALUE && config.jqueryFormPluginPath != null) {
                                //passed the plugin path and the success callback
                                thisCMSEditorPlugin.downloadCss(config.cmsEditorCssPath, function () {
                                    thisCMSEditorPlugin.downloadPlugin(config.jqueryFormPluginPath, function () {
                                        thisCMSEditorPlugin.initializeImageDataLoading(key, counter, entitytype, index);
                                    });
                                });
                            } else {
                                thisCMSEditorPlugin.initializeImageDataLoading(key, counter, entitytype);
                            }
                        },
                        close: function () {
                            //thisCMSEditorPlugin.imageUploadCleanup();
                        }
                    };

                    this.initUploadButtonsEvents();
                    this.attachEventsListener();

                    break;
                case "item-name":


                    break;
                case "item-description":

                    //set the dialog for the product name
                    dialogId = this.selectorChecker(config.itemDescriptionEditorDialogId);

                    //create the content using the keyvalue pair
                    value = $.trim($("[data-itemCode='" + key + "']").children("div.string-value").html());
                    template = this.parseTemplateReturnHtml("itemDescriptionEditorContentTemplateID", new KeyValue(key, value));

                    //clear the dialog content holder
                    $(dialogId).html('');

                    //replace with new data from template
                    $(template).appendTo($(dialogId));

                    //set the dialog option layout
                    option = {
                        autoOpen: true,
                        width: 350,
                        modal: true,
                        resize: false,
                        resizable: false,
                        position: "center"
                    }

                    this.attachEventsListener();

                    break;
                case "item-webdescription":

                    //set the dialog for the product name
                    dialogId = this.selectorChecker(config.itemWebDescriptionEditorDialogId);

                    //create the content using the keyvalue pair
                    value = $.trim($("[data-itemCode='" + key + "']").children("div.item-web-description-value").html());
                    template = this.parseTemplateReturnHtml("itemWebDescriptionEditorContentTemplateID", new KeyValue(key, value));

                    //clear the dialog content holder
                    $(dialogId).html('');

                    //replace with new data from template
                    $(template).appendTo($(dialogId));

                    //set the dialog option layout
                    option = {
                        autoOpen: false,
                        width: 763,
                        modal: true,
                        resizable: false,
                        create: function () {

                            var initSuccessfulCallBack = function () {
                                thisCMSEditorPlugin.attachEventsListener();
                                //auto focus dialog when window is resized or scrolled
                                thisCMSEditorPlugin.attachAutoScrollOnDialog(dialogId);
                                $(dialogId).dialog("open");
                            }

                            thisCMSEditorPlugin.initTinyMceForControl(initSuccessfulCallBack, config.itemWebDescriptionContentValue, false);

                        },
                        close: function (event, ui) {
                            tinyMCE.execCommand('mceRemoveControl', true, config.itemWebDescriptionContentValue);
                            $(dialogId).dialog("destroy");
                        }
                    }

                    break;
                case "item-summary":

                    //set the dialog for the product name
                    dialogId = this.selectorChecker(config.itemWebDescriptionEditorDialogId);

                    //create the content using the keyvalue pair
                    value = $.trim($("[data-itemCode='" + key + "']").children("div.item-web-description-value").html());
                    template = this.parseTemplateReturnHtml("itemWebDescriptionEditorContentTemplateID", new KeyValue(key, value));

                    //clear the dialog content holder
                    $(dialogId).html('');

                    //replace with new data from template
                    $(template).appendTo($(dialogId));

                    //set the dialog option layout
                    option = {
                        autoOpen: false,
                        width: 763,
                        modal: true,
                        resizable: false,
                        create: function () {

                            var initSuccessfulCallBack = function () {
                                thisCMSEditorPlugin.attachEventsListener();
                                //auto focus dialog when window is resized or scrolled
                                thisCMSEditorPlugin.attachAutoScrollOnDialog(dialogId);
                                $(dialogId).dialog("open");
                            }

                            thisCMSEditorPlugin.initTinyMceForControl(initSuccessfulCallBack, config.itemWebDescriptionContentValue, false);
                            $(thisCMSEditorPlugin.selectorChecker(config.itemWebDescriptionUIDialogTitleId)).html("Edit Summary");
                        },
                        close: function (event, ui) {
                            tinyMCE.execCommand('mceRemoveControl', true, config.itemWebDescriptionContentValue);
                            $(dialogId).dialog("destroy");
                        }
                    }


                    break;
                case "item-warranty":

                    //set the dialog for the product name
                    dialogId = this.selectorChecker(config.itemWebDescriptionEditorDialogId);

                    //create the content using the keyvalue pair
                    value = $.trim($("[data-itemCode='" + key + "']").children("div.item-web-description-value").html());
                    template = this.parseTemplateReturnHtml("itemWebDescriptionEditorContentTemplateID", new KeyValue(key, value));

                    //clear the dialog content holder
                    $(dialogId).html('');

                    //replace with new data from template
                    $(template).appendTo($(dialogId));

                    //set the dialog option layout
                    option = {
                        autoOpen: false,
                        width: 763,
                        modal: true,
                        resizable: false,
                        create: function () {

                            var initSuccessfulCallBack = function () {
                                thisCMSEditorPlugin.attachEventsListener();
                                //auto focus dialog when window is resized or scrolled
                                thisCMSEditorPlugin.attachAutoScrollOnDialog(dialogId);
                                $(dialogId).dialog("open");
                            }

                            thisCMSEditorPlugin.initTinyMceForControl(initSuccessfulCallBack, config.itemWebDescriptionContentValue, false);
                            $(thisCMSEditorPlugin.selectorChecker(config.itemWebDescriptionUIDialogTitleId)).html("Edit Warranty");
                        },
                        close: function (event, ui) {
                            tinyMCE.execCommand('mceRemoveControl', true, config.itemWebDescriptionContentValue);
                            $(dialogId).dialog("destroy");
                        }
                    }

                    break;
                default:
                    break;
            }

            $(dialogId).dialog(option);

        },

        imageUploadCleanup: function () {

            var config = getConfig();

            //clean up
            this.setImageUploadCurrentActiveTabIndex(1);
            this.setImageUploadLoadedImages(null);
            this.setSelectedImageItem(null);
            this.setAjaxFormUploadOption(null);

            //inherited method from core.js - jqueryBasePlugin
            this.removeCssReference(config.cmsEditorCssPath);

            //check if user modifies the images. If has modifications then reload the page to see changes
            //if (this.getHasUploadImageModification()) {
            //document.location.reload();
            //}
        },

        initializeTabPanel: function (entityType, previousTab) {

            var config = getConfig();
            var defaultSelectedTab = tabEnum.large;

            if (typeof previousTab != 'undefined') {
                defaultSelectedTab = previousTab;
            }

            var uploadOption = this.getAjaxFormUploadOption();
            uploadOption.size = this.getSizeDescriptionByEnumSizes(defaultSelectedTab);
            uploadOption.sizeValue = defaultSelectedTab;

            if (entityType == "product") {
                $(this.selectorChecker(config.imageEditorTabId)).tabs({
                    selected: defaultSelectedTab,
                    select: function (event, ui) {

                        var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
                        uploadOption.size = thisCMSEditorPlugin.getSizeDescriptionByEnumSizes(ui.index);
                        uploadOption.sizeValue = ui.index;

                        //thisCMSEditorPlugin.setCurrentPage(1);
                        thisCMSEditorPlugin.setImageUploadCurrentActiveTabIndex(ui.index + 1);

                        var recentIndex = thisCMSEditorPlugin.getAjaxFormUploadOption().imgIndex;
                        thisCMSEditorPlugin.initImagePaging(recentIndex);
                        thisCMSEditorPlugin.setDefaultImageToShow(recentIndex);
                        //thisCMSEditorPlugin.setButtonDefaultSizingFunctionality(null, ui.index + 1);
                    }
                });
            } else {
                $(this.selectorChecker(config.imageEditorTabId)).tabs({
                    selected: defaultSelectedTab,
                    select: function (event, ui) {

                        var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
                        uploadOption.size = thisCMSEditorPlugin.getSizeDescriptionByEnumSizes(ui.index);
                        uploadOption.sizeValue = ui.index;

                        thisCMSEditorPlugin.setImageUploadCurrentActiveTabIndex(ui.index + 1);
                        thisCMSEditorPlugin.setDefaultImageToShow();

                        thisCMSEditorPlugin.tryDeleteButtonEnableDisableForEntity();
                    },
                    disabled: [tabEnum.minicart] // disable minicart since it's not applicable on entities
                });
            }

        },

        setButtonDefaultSizingFunctionality: function (imageData, currentActiveTabIndex) {

            var config = getConfig();
            var isInMedium = (tabEnum.medium + 1 == currentActiveTabIndex);
            var isInIcon = (tabEnum.icon + 1 == currentActiveTabIndex);

            var chkButtonID = this.selectorChecker(config.tabPrefixName) + "-" + currentActiveTabIndex + " input[id='togglecheck" + currentActiveTabIndex + "']";
            var labelId = this.selectorChecker(config.tabPrefixName) + "-" + currentActiveTabIndex + " label[for='togglecheck" + currentActiveTabIndex + "']";

            //clear jquery ui css
            $(chkButtonID).button("css", null);
            $(chkButtonID).hide();

            (typeof (imageData) != 'undefined' && imageData.exists) ? $(labelId).show() : $(labelId).hide();

            if (isInMedium && typeof (imageData) != 'undefined') {

                $(labelId).unbind("click");

                if (imageData.IsDefaultMedium != null && imageData.IsDefaultMedium) {
                    $(labelId).attr("class", "btn");
                    $(labelId).text(config.defaultSetImageSizeInActiveText);
                    $(labelId).attr('disabled', true);
                    $(chkButtonID).attr('checked', 'checked');
                } else {
                    $(labelId).attr("class", "btn btn-primary");
                    $(labelId).text(config.defaultSetImageSizeActiveText);
                    $(labelId).removeAttr('disabled');
                    $(chkButtonID).removeAttr('checked');

                    $(labelId).click(function () {

                        var callBack = function () {
                            thisCMSEditorPlugin.reloadImageData();
                            thisCMSEditorPlugin.setHasUploadImageModification(true);
                            thisCMSEditorPlugin.hideProgressbar();
                        };

                        thisCMSEditorPlugin.showProgressbar();
                        thisCMSEditorPlugin.updateImageDefaultSizeToDB(callBack);
                    });
                }
            }

            if (isInIcon && typeof (imageData) != 'undefined') {

                $(labelId).unbind("click");

                if (imageData.IsDefaultIcon != null && imageData.IsDefaultIcon) {
                    $(labelId).attr("class", "btn");
                    $(labelId).text(config.defaultSetImageSizeInActiveText);
                    $(labelId).attr('disabled', true);
                    $(chkButtonID).attr('checked', 'checked');
                } else {
                    $(labelId).attr("class", "btn btn-primary");
                    $(labelId).text(config.defaultSetImageSizeActiveText);
                    $(labelId).removeAttr('disabled');
                    $(chkButtonID).removeAttr('checked');

                    $(labelId).click(function () {
                        var callBack = function () {
                            thisCMSEditorPlugin.reloadImageData();
                            thisCMSEditorPlugin.setHasUploadImageModification(true);
                            thisCMSEditorPlugin.hideProgressbar();
                        };

                        thisCMSEditorPlugin.showProgressbar();
                        thisCMSEditorPlugin.updateImageDefaultSizeToDB(callBack);
                    });

                }
            }

            $(chkButtonID).change(function () {

                if ($(this).is(':checked')) {
                    $(labelId).attr("class", "btn");
                    $(labelId).text(config.defaultSetImageSizeInActiveText);
                    $(labelId).attr('disabled', true);
                } else {
                    if (typeof $(labelId).attr('disabled') != undefined) {
                        $(this).attr('checked', 'checked');
                    }
                }

            });

        },

        reloadImageData: function () {

            var uploadOption = this.getAjaxFormUploadOption();
            var selectedItem = this.getSelectedImageItem();

            if (uploadOption.uploadType == uploadType.remove ||
                uploadOption.uploadType == uploadType.upload) {

                this.setCurrentPage(1);
                uploadOption.imgIndex = 0;
            }

            //Since size is string. we need to convert it to enumeration
            this.initializeTabPanel(selectedItem.itemType, this.getEnumSizeByDescription(uploadOption.size));
            this.initializeImageDataLoading(selectedItem.itemCode, selectedItem.counter, selectedItem.itemType);

        },

        imagePathModifier: function (images) {
            $.each(images, function (key, value) {

                if (key.toLowerCase() == "largeimages" ||
                    key.toLowerCase() == "mediumimages" ||
                    key.toLowerCase() == "iconimages" ||
                    key.toLowerCase() == "minicartimages" ||
                    key.toLowerCase() == "mobileimages"
                    ) {
                    $.each(value, function (index, img) {
                        img.src = "../" + img.src;
                    });
                }
            });
            return images;
        },

        initializeImageDataLoading: function (itemCode, counter, entityType, index) {
            //set the upload options
            var uploadOption = this.getAjaxFormUploadOption();
            uploadOption.entityCode = itemCode;
            uploadOption.entityType = entityType;
            uploadOption.counter = counter;

            AjaxCallCommon(
                "../ActionService.asmx/GetProductImageData",
                this.getSelectedImageItem(),
                function (result) {

                    if (result.d) {
                        //save the downloaded images
                        var images = thisCMSEditorPlugin.imagePathModifier($.parseJSON(result.d));
                        thisCMSEditorPlugin.setImageUploadLoadedImages(images);

                        if (entityType == "product") {
                            //thisCMSEditorPlugin.loadProductImages();
                            thisCMSEditorPlugin.setupDefaultSizingButtonsForTabs();

                            if (index > -1) {
                                thisCMSEditorPlugin.initImagePaging(index);
                            }
                            else {
                                thisCMSEditorPlugin.initImagePaging(thisCMSEditorPlugin.getAjaxFormUploadOption().imgIndex);
                            }
                        } else {
                            thisCMSEditorPlugin.loadEntityImages();
                            thisCMSEditorPlugin.hideDefaultImageSizeButtonForIconAndMedium();
                        }

                        if (index > -1) {
                            thisCMSEditorPlugin.setDefaultImageToShow(index);
                        }
                        else {
                            thisCMSEditorPlugin.setDefaultImageToShow(thisCMSEditorPlugin.getAjaxFormUploadOption().imgIndex);
                        }
                        thisCMSEditorPlugin.tryChangeButtonEnableDisable();
                        thisCMSEditorPlugin.tryDeleteButtonEnableDisableForProduct();
                        thisCMSEditorPlugin.tryDeleteButtonEnableDisableForEntity();
                        thisCMSEditorPlugin.tryAddButtonEnableDisable();

                        //setup image zoom
                        $('.cloud-zoom').fancybox({
                            'titlePosition': 'inside',
                            'transitionIn': 'none',
                            'transitionOut': 'fade'
                        });

                    }
                },
                function (result) {
                    var errorObject = $.parseJSON(result.responseText);
                    var errorMessage = 'error in method GetProductImageData \n \n' +
                                        'Error Details: ' + errorObject.Message;
                    alert(errorMessage);
                }
            );

        },

        loadEntityImages: function () {
            var imageData = this.getImageUploadLoadedImages();
            this.setSelectedImageByIndex(imageData.Large, tabEnum.large + 1);
            this.setSelectedImageByIndex(imageData.Medium, tabEnum.medium + 1);
            this.setSelectedImageByIndex(imageData.Thumbnail, tabEnum.icon + 1);
            this.setSelectedImageByIndex(imageData.Mobile, tabEnum.mobile + 1);
        },

        loadProductImages: function () {
            var imageData = this.getImageUploadLoadedImages();
            this.setSelectedImageByIndex(imageData.large, tabEnum.large + 1);
            this.setSelectedImageByIndex(imageData.medium, tabEnum.medium + 1);
            this.setSelectedImageByIndex(imageData.icon, tabEnum.icon + 1);
            this.setSelectedImageByIndex(imageData.Minicart, tabEnum.minicart + 1);
            this.setSelectedImageByIndex(imageData.Mobile, tabEnum.mobile + 1);
        },

        setupDefaultSizingButtonsForTabs: function () {
            var imageData = this.getImageUploadLoadedImages();
            this.setButtonDefaultSizingFunctionality(imageData.medium, tabEnum.medium + 1);
            this.setButtonDefaultSizingFunctionality(imageData.icon, tabEnum.icon + 1);
        },

        setSelectedImageByIndex: function (imageData, currentActiveTabIndex) {

            var stamp = this.getTimeStamp();

            var img = $(this.selectorChecker(config.tabPrefixName) + "-" + currentActiveTabIndex + " img");

            this.showProgressbar();
            $(img).unbind('load');
            $(img).load(function () {
                thisCMSEditorPlugin.hideProgressbar();
            });
            img.attr('src', imageData.src + stamp);

            this.showImageDimension(imageData.src, currentActiveTabIndex);

            $(this.selectorChecker(config.tabPrefixName) + "-" + currentActiveTabIndex + " img").attr('alt', imageData.Alt);
            $(this.selectorChecker(config.tabPrefixName) + "-" + currentActiveTabIndex + " img").attr('title', imageData.Alt);

            //image zoom popup requirements
            $(this.selectorChecker(config.tabPrefixName) + "-" + currentActiveTabIndex + " a").attr('href', imageData.src + stamp);

        },

        initUploadButtonsEvents: function () {

            if (!$.browser.msie) {

                $('#btnUpload').unbind('click');
                $('#btnUpload').click(function () {

                    var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
                    uploadOption.uploadType = uploadType.upload;

                    //safari and opera
                    if ($.browser.webkit) {

                        var form = $('#ctrlUploadForm');
                        $('input[type="file"]', form).trigger("click");

                    } else { //firefox, chrome
                        $('#ctrlFileUpload').click();
                    }

                });

                $('#btnEditImage').unbind('click');
                $('#btnEditImage').click(function () {
                    var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
                    uploadOption.uploadType = uploadType.edit;
                    $('#ctrlFileUpload').click();
                });

            }

            //if ie template creates a new change upload element
            if ($.browser.msie) {
                $('#ctrlChangeUpload').unbind('change');
                $('#ctrlChangeUpload').change(function () {
                    var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
                    uploadOption.uploadType = uploadType.edit;
                    thisCMSEditorPlugin.startFileUpload();
                });
            }

            $('#ctrlFileUpload').unbind('change');
            $('#ctrlFileUpload').change(function () {
                var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
                if ($.browser.msie) {
                    uploadOption.uploadType = uploadType.upload;
                }

                //Show upload message only for product
                if (uploadOption.size == "medium" && uploadOption.entityType == 'product') {
                    thisCMSEditorPlugin.showApplyMediumImageUpload();
                }

                thisCMSEditorPlugin.startFileUpload();
            });

            $("#btnImageUploadDelete").unbind('click');
            $("#btnImageUploadDelete").click(function () {

                var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
                uploadOption.uploadType = uploadType.remove;
                uploadOption.isApplyToAll = false;

                if (thisCMSEditorPlugin.getImagesByTabIndex().length == 0 && !uploadOption.isImageExist) {
                    thisCMSEditorPlugin.showUnableToDeleteMessageIfImageDoesNotExist();
                    return;
                }

                var isDelete = thisCMSEditorPlugin.showDeleteConfirmation();
                if (isDelete) {
                    //ask question only for product
                    if (uploadOption.entityType == 'product') {
                        var isDeleteAll = thisCMSEditorPlugin.showImageDeletionConfirmationMessage();
                        uploadOption.deleteAll = isDeleteAll;

                        if (!uploadOption.deleteAll && !uploadOption.isImageExist) {
                            thisCMSEditorPlugin.showUnableToDeleteMessageIfImageDoesNotExist();
                            return;
                        }
                    }
                    thisCMSEditorPlugin.showProgressbar();
                    thisCMSEditorPlugin.executeServerUpload();
                }
            });

            $("#btnImageUploadCancel").unbind('click');
            $("#btnImageUploadCancel").click(function () {
                //var config = getConfig();
                //var dialogId = thisCMSEditorPlugin.selectorChecker(config.imageEditorDialogId);
                $(".ui-dialog-content").dialog().dialog("close");
                //$(dialogId).dialog("close",
                //function () {
                //thisCMSEditorPlugin.imageUploadCleanup();
                //});
            });

        },

        startFileUpload: function () {

            var newUploadOption = this.getAjaxFormUploadOption();
            if (newUploadOption.size == 'large') {

                //ask question only for product
                if (newUploadOption.entityType == 'product') {
                    var yesno = this.showApplyAllConfirmation();
                    newUploadOption.isApplyToAll = yesno;
                }

            } else {
                newUploadOption.isApplyToAll = false;
            }

            this.executeServerUpload();

            if ($.browser.msie) {
                $('#ctrlChangeUpload').show();
                $('#ctrlFileUpload').show();

                $('#ctrlChangeUpload').val('');
                $('#ctrlFileUpload').val('');
            }

        },

        showProgressbar: function () {

            $(".cms-plugin-progress-bar").dialog({
                autoOpen: true,
                width: 300,
                height: 120,
                modal: true,
                resize: false,
                resizable: false
            });

            $(".cms-plugin-progress-bar")
                    .siblings(".ui-dialog-titlebar")
                    .remove();

        },

        hideProgressbar: function () {
            $(".cms-plugin-progress-bar").dialog("close");
        },

        executeServerUpload: function () {

            $('#ctrlUploadForm').unbind("submit");
            $('#ctrlUploadForm').on("submit", function (e) {

                e.preventDefault();

                thisCMSEditorPlugin.showProgressbar();
                var option = thisCMSEditorPlugin.buildAjaxFormOption();

                $('#ctrlUploadForm').ajaxSubmit(option);

            });

            //manually submit the form using hidden submit button
            $('#btnHiddenUploadSubmit').submit();
        },

        showDeleteConfirmation: function () {
            return confirm("Are you sure you want to delete this image?");
        },

        showApplyAllConfirmation: function () {
            return confirm("You want to apply this image to all sizes?");
        },

        showUnableToDeleteMessageIfImageDoesNotExist: function () {
            this.showImageUploadMessage('Unable to delete, Either no images has been uploaded or Image does not exist');
        },

        showImageUploadMessage: function (message) {
            alert(message);
        },

        showApplyMediumImageUpload: function () {
            this.showImageUploadMessage("Note: Uploading image from Medium size will also create image for micro, minicart and mobile.");
        },

        showImageDeletionConfirmationMessage: function () {
            return confirm("Delete Options \n\n Select [OK] To delete all sizes \n\n Select [CANCEL] to delete only the selected size");
        },

        initImagePaging: function (recentIndex) {

            //clear pages
            $("#upload-page-ul").html(null);

            var pagerParam = this.getImagesForPagesByCurrentSelectedPage();

            if (pagerParam == null || pagerParam.length == 0) return;

            //init previous button
            $(this.parseTemplate("previousPageButtonTemplateID", null)).appendTo($("#upload-page-ul"));

            //generate paging of images
            $(this.parseTemplate("uploadPageButtonTemplateID", pagerParam).appendTo($("#upload-page-ul")));

            //init previous button
            $(this.parseTemplate("nextPageButtonTemplateID", null)).appendTo($("#upload-page-ul"));

            this.tryNavigationPageButtonHideShow();

            //attach events for each pager button
            this.attachEventsOnPagers(pagerParam);

            var index = pagerParam[0].Index;
            if (typeof recentIndex != 'undefined') {
                index = recentIndex;
            }

            var uploadOptions = this.getAjaxFormUploadOption();
            uploadOptions.imgIndex = index;

            this.setHighlightedPagerBasedFromSeletedIndex();

        },

        setHighlightedPagerBasedFromSeletedIndex: function () {

            //clear the highlights
            //$("input[type='button'][page]").addClass('btn-primary');

            var uploadOptions = this.getAjaxFormUploadOption();
            var index = uploadOptions.imgIndex + 1;
            var obj = $('#upload-page-ul').find("#btnPage_" + index);
            $(obj).removeClass('btn-primary');

        },

        updateImageDefaultSizeToDB: function (callBack) {

            var uploadOptions = this.getAjaxFormUploadOption();
            uploadOptions.uploadType = uploadType.statusChange;

            var param = new Object();
            param.itemCode = uploadOptions.entityCode;
            param.fileName = uploadOptions.fileName;
            param.size = uploadOptions.size;

            AjaxCallWithSecuritySimplified(
                "ImageUploadSetAsImageDefault",
                param,
                function (result) {

                    if (result.d) {

                        if (typeof callBack != 'undefined') callBack();

                    } else {
                        alert('failed update image default size.');
                    }

                },
                function (result) {
                    alert('error');
                }
            );

        },

        getImagesForPagesByCurrentSelectedPage: function () {

            var images = this.getImagesByTabIndex();

            if (images.length == 0) return null;

            var pagerParam = this.getImagesPagesParamByImageType(images);

            //Calculate total pages
            var total = Math.ceil(pagerParam.length / defaultImageUploadPageCount);
            this.setTotalPages(total);

            var curPage = this.getCurrentPage();

            var startIndex = 0;
            var end = defaultImageUploadPageCount;

            if (curPage > 1) {
                startIndex = ((curPage * defaultImageUploadPageCount) - defaultImageUploadPageCount);
                end = (curPage * defaultImageUploadPageCount);
            }

            return pagerParam.slice(startIndex, end);
        },

        attachEventsOnPagers: function (pagerParam) {

            var imagesCopy = this.getImagesByTabIndex();
            var currentActiveTabIndex = this.getImageUploadCurrentActiveTabIndex();

            var currentPage = this.getCurrentPage();
            var totalPages = this.getTotalPages();

            for (var i = 0; i < pagerParam.length; i++) {

                var id = '#btnPage_' + pagerParam[i].PageNumber;

                //if image is only one hide the page button
                if (imagesCopy.length == 1) {
                    $(id).hide();
                    break;
                    return;
                }

                $(id).unbind("click");
                $(id).click(function () {

                    $("input[type='button'][page]").addClass('btn-primary');

                    var imageIndex = $(this).attr('page');
                    thisCMSEditorPlugin.setSelectedImageByIndex(imagesCopy[imageIndex - 1], currentActiveTabIndex);
                    thisCMSEditorPlugin.setButtonDefaultSizingFunctionality(imagesCopy[imageIndex - 1], currentActiveTabIndex);

                    var actualIndex = imageIndex - 1;

                    var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
                    uploadOption.fileName = imagesCopy[actualIndex].ImgFileName;
                    uploadOption.imageSource = imagesCopy[actualIndex].src;

                    var isImageExist = imagesCopy[actualIndex].exists;
                    uploadOption.isImageExist = isImageExist;

                    thisCMSEditorPlugin.getAjaxFormUploadOption().imgIndex = actualIndex;

                    $(this).removeClass('btn-primary');
                });

            }

            $('#btnImageUploadPrevious').unbind('click');
            $('#btnImageUploadPrevious').click(function () {
                thisCMSEditorPlugin.movePage(pageDirectionEnum.previous);
            });

            $('#btnImageUploadNext').unbind('click');
            $('#btnImageUploadNext').click(function () {
                thisCMSEditorPlugin.movePage(pageDirectionEnum.next);
            });

        },

        getTotalPages: function () {
            return totalImageUploadPages;
        },

        setTotalPages: function (value) {
            totalImageUploadPages = value; ;
        },

        getCurrentPage: function () {
            return currentImageUploadPage;
        },

        setCurrentPage: function (value) {
            currentImageUploadPage = value;
        },

        showImageDimension: function (src, currentActiveTabIndex) {

            var imageCopy = new Image();
            $(imageCopy).load(function () {

                var config = getConfig();
                var width = imageCopy.width;
                var height = imageCopy.height;

                $(thisCMSEditorPlugin.selectorChecker(config.tabPrefixName) + "-" + currentActiveTabIndex + ' .upload-image-size-dimension-container label').html('Width: ' + width + ' - Height: ' + height);
                imageCopy = null;
            })
            imageCopy.src = src;

        },

        movePage: function (direction) {

            var currentPage = this.getCurrentPage();
            var total = this.getTotalPages();

            if (direction == pageDirectionEnum.previous) {
                this.setCurrentPage(currentPage - 1);
            } else {
                this.setCurrentPage(currentPage + 1);
            }

            this.initImagePaging();
            this.setDefaultImageToShow(this.getAjaxFormUploadOption().imgIndex);

        },

        getImagesPagesParamByImageType: function (images) {

            var pages = new Array();
            var param = null;
            for (var i = 0; i < images.length; i++) {
                param = new Object();
                param.PageNumber = i + 1;
                param.Index = i;
                pages.push(param);
            }

            return pages;
        },

        getImageUploadCurrentActiveTabIndex: function () {
            return imageUploadCurrentActiveTabIndex;
        },

        setImageUploadCurrentActiveTabIndex: function (value) {
            imageUploadCurrentActiveTabIndex = value;
        },

        setAjaxFormUploadOption: function (value) {
            ajaxFormUploadOption = value;
        },

        getAjaxFormUploadOption: function () {
            return ajaxFormUploadOption;
        },

        resetUploadUptions: function () {
            this.setAjaxFormUploadOption({
                entityCode: '',
                entityType: '',
                size: '',
                sizeValue: '',
                counter: '',
                fileName: '',
                uploadType: '',
                isApplyToAll: false,
                isImageExist: false,
                imageSource: '',
                imgIndex: 0,
                deleteAll: false
            });
        },

        getSelectedImageItem: function () {
            return currentlySelectedImageItem;
        },

        setSelectedImageItem: function (value) {
            currentlySelectedImageItem = value;
        },

        buildAjaxFormOption: function () {

            var uploadOptions = this.getAjaxFormUploadOption();
            var option = {
                url: '../FileUploadHandler.ashx?ec=' + uploadOptions.entityCode +
                                            '&pk=' + uploadOptions.counter +
                                            '&et=' + uploadOptions.entityType +
                                            '&fName=' + uploadOptions.fileName +
                                            '&s=' + uploadOptions.size +
                                            '&sv=' + uploadOptions.sizeValue +
                                            '&ut=' + uploadOptions.uploadType +
                                            '&aa=' + uploadOptions.isApplyToAll +
                                            '&exist=' + uploadOptions.isImageExist +
                                            '&src=' + uploadOptions.imageSource +
                                            '&da=' + uploadOptions.deleteAll,

                type: "post",
                dataType: 'json',
                resetForm: true,
                error: thisCMSEditorPlugin.uploadError,
                success: thisCMSEditorPlugin.uploadResult
            }

            return option;
        },

        uploadError: function () {
            thisCMSEditorPlugin.hideProgressbar();
        },

        uploadResult: function (data, status, xhr) {
            //tag the plugin if requires reload
            thisCMSEditorPlugin.setHasUploadImageModification(true);

            var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();

            if (uploadOption.uploadType != uploadType.edit ||
                (uploadOption.uploadType == uploadType.edit && uploadOption.isApplyToAll) ||
                (uploadOption.uploadType == uploadType.edit && !uploadOption.isImageExist)) {

                //refresh the images
                thisCMSEditorPlugin.reloadImageData();

            } else {

                thisCMSEditorPlugin.setDefaultImageToShow(
                    thisCMSEditorPlugin.getAjaxFormUploadOption().imgIndex);

            }

            //finally hide the progress bar
            thisCMSEditorPlugin.hideProgressbar();

            //refresh item container
            setItemImages(uploadOption.entityCode, uploadOption.counter, true);
        },

        setDefaultImageToShow: function (imageIndexToShow) {

            var img = null;
            var images = this.getImagesByTabIndex();
            if (typeof (images) != 'undefined' && images.length == 0) { return; }

            if (typeof imageIndexToShow != 'undefined') {
                img = images[imageIndexToShow];
            } else {
                img = images[0];
            }

            var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
            uploadOption.fileName = img.ImgFileName;
            uploadOption.imageSource = img.src;
            uploadOption.isImageExist = img.exists;

            this.setSelectedImageByIndex(img, this.getImageUploadCurrentActiveTabIndex());

            if (uploadOption.entityType == 'product') {
                this.setButtonDefaultSizingFunctionality(img, this.getImageUploadCurrentActiveTabIndex());
            }
        },

        tryChangeButtonEnableDisable: function () {
            var images = thisCMSEditorPlugin.getImagesByTabIndex();
            if (typeof (images) != 'undefined' && images.length == 0) {
                $('#btnEditImage').attr('disabled', true);
            } else {
                $('#btnEditImage').removeAttr('disabled');
            }

            if ($.browser.msie) {
                if (typeof (images) != 'undefined' && images.length == 0) {
                    $('.label-changeupload-wrapper').hide();
                } else {
                    $('.label-changeupload-wrapper').show();
                }
            }

        },

        tryAddButtonEnableDisable: function () {

            var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
            if (uploadOption.entityType == 'product') {
                $('#btnUpload').removeAttr('disabled');
            } else {
                $('#btnUpload').attr('disabled', true);
            }

            if ($.browser.msie) {
                if (uploadOption.entityType == 'product') {
                    $('.label-addupload-wrapper').show();
                } else {
                    $('.label-addupload-wrapper').hide();
                }
            }

        },

        tryDeleteButtonEnableDisableForProduct: function () {

            var currentImgIndex = this.getAjaxFormUploadOption().imgIndex;
            if ((typeof (images) != 'undefined' && images.length == 0)) {
                $('#btnImageUploadDelete').attr('disabled', true);
            } else {
                $('#btnImageUploadDelete').removeAttr('disabled');
            }

        },

        tryDeleteButtonEnableDisableForEntity: function () {

            var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();
            if (uploadOption.entityType == 'product') return;

            //Disable delete button if image from entities does not exist
            if (uploadOption.isImageExist) {
                $('#btnImageUploadDelete').removeAttr('disabled');
            } else {
                $('#btnImageUploadDelete').attr('disabled', true);
            }
        },

        hideDefaultImageSizeButtonForIconAndMedium: function () {
            var mediumTabIndex = tabEnum.medium + 1;
            var iconTabIndex = tabEnum.icon + 1;

            $(this.selectorChecker(config.tabPrefixName) + "-" + mediumTabIndex + " input[id='togglecheck" + mediumTabIndex + "']").hide();
            $(this.selectorChecker(config.tabPrefixName) + "-" + mediumTabIndex + " label[for='togglecheck" + mediumTabIndex + "']").hide();

            $(this.selectorChecker(config.tabPrefixName) + "-" + iconTabIndex + " input[id='togglecheck" + iconTabIndex + "']").hide();
            $(this.selectorChecker(config.tabPrefixName) + "-" + iconTabIndex + " label[for='togglecheck" + iconTabIndex + "']").hide();
        },

        tryNavigationPageButtonHideShow: function () {
            var images = thisCMSEditorPlugin.getImagesByTabIndex();

            if (typeof (images) != 'undefined' && (images.length == 0 || images.length <= 4)) {
                $('#btnImageUploadPrevious').hide();
                $('#btnImageUploadNext').hide();
            }

            var curPage = this.getCurrentPage();
            var totPage = this.getTotalPages();

            if (curPage == 1) {
                $('#btnImageUploadPrevious').attr('disabled', true);
            } else {
                $('#btnImageUploadPrevious').removeAttr('disabled');
            }

            if (totPage == curPage) {
                $('#btnImageUploadNext').attr('disabled', true);
            } else {
                $('#btnImageUploadNext').removeAttr('disabled');
            }

        },

        getHasUploadImageModification: function () {
            return hasUploadImageModification;
        },

        setHasUploadImageModification: function (value) {
            hasUploadImageModification = value;
        },

        getImageUploadLoadedImages: function () {
            return loadedImages;
        },

        setImageUploadLoadedImages: function (value) {
            loadedImages = value;
        },

        getEnumSizeByDescription: function (size) {

            var value = '';
            switch (size) {
                case 'large':
                    value = tabEnum.largevalue;
                    break;
                case 'medium':
                    value = tabEnum.medium;
                    break;
                case 'icon':
                    value = tabEnum.icon;
                    break;
                case 'minicart':
                    value = tabEnum.minicart;
                    break;
                case 'mobile':
                    value = tabEnum.mobile;
                    break;
            }

            return value;
        },

        getSizeDescriptionByEnumSizes: function (enumSize) {

            var value = '';
            switch (enumSize) {
                case tabEnum.large:
                    value = 'large';
                    break;
                case tabEnum.medium:
                    value = 'medium';
                    break;
                case tabEnum.icon:
                    value = 'icon';
                    break;
                case tabEnum.minicart:
                    value = 'minicart';
                    break;
                case tabEnum.mobile:
                    value = 'mobile';
                    break;
            }

            return value;
        },

        getImagesByTabIndex: function () {

            var imageReturn = null;
            var uploadOption = thisCMSEditorPlugin.getAjaxFormUploadOption();

            var imageData = this.getImageUploadLoadedImages();
            var currentActiveTabIndex = this.getImageUploadCurrentActiveTabIndex();

            if (uploadOption.entityType == 'product') {

                switch (currentActiveTabIndex) {
                    case tabEnum.large + 1:
                        imageReturn = imageData.largeImages;
                        break;

                    case tabEnum.medium + 1:
                        imageReturn = imageData.mediumImages;
                        break;

                    case tabEnum.icon + 1:
                        imageReturn = imageData.IconImages;
                        break;

                    case tabEnum.minicart + 1:
                        imageReturn = imageData.MinicartImages;
                        break;

                    case tabEnum.mobile + 1:
                        imageReturn = imageData.MobileImages;
                        break;
                }

            } else {

                var arrayReturn = new Array();
                switch (currentActiveTabIndex) {
                    case tabEnum.large + 1:

                        arrayReturn.push(imageData.Large);
                        imageReturn = arrayReturn;
                        break;

                    case tabEnum.medium + 1:
                        arrayReturn.push(imageData.Medium);
                        imageReturn = arrayReturn;
                        break;

                    case tabEnum.icon + 1:
                        arrayReturn.push(imageData.Thumbnail);
                        imageReturn = arrayReturn;
                        break;

                    case tabEnum.mobile + 1:
                        arrayReturn.push(imageData.Mobile);
                        imageReturn = arrayReturn;
                        break;
                }

            }

            return imageReturn;

        },

        updateTopic: function () {

            var config = getConfig();

            tinyMCE.triggerSave(true, true)
            var editedValue = tinyMCE.activeEditor.getContent();

            //get the topic id from the selected control
            var topicId = currentSelectedControl.attr("data-contentKey");
            var param = new Object();
            param.topicId = topicId;
            param.htmlContent = editedValue;

            AjaxCallWithSecuritySimplified(
                "UpdateTopicFromEditor",
                param,
                function (result) {

                    if (result.d) {

                        $(currentSelectedControl).html(editedValue);
                        $(thisCMSEditorPlugin.selectorChecker(config.topicEditorDialogId)).dialog("close");

                    } else {
                        alert(config.messages.MESSAGE_SAVING_TOPIC_ADMIN_NOTLOGGEDIN_ERROR);
                    }

                },
                function (result) {
                    alert('error');
                }
            );


        },

        initTinyMceForControl: function (initCallBack, editorInputId, isTopic) {

            if (!isTopic) currentSelectedControl = $(currentSelectedControl).children("div.item-web-description-value");

            var htmlContent = $(currentSelectedControl).html();

            if (!isTopic) htmlContent = $.trim(currentSelectedControl.html());

            $(this.selectorChecker(editorInputId)).val(htmlContent);

            //this assume that the tiny mce plugin 
            //is already loaded outside the plugin

            tinyMCE.init({
                // General options
                mode: "exact",
                theme: "advanced",
                elements: editorInputId,
                width: "755",
                height: "500",
                force_br_newlines: true,
                force_p_newlines: false,
                forced_root_block: false,
                plugins: "autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",
                // Theme options
                theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,styleselect,formatselect,fontselect,fontsizeselect",
                theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,code,|,insertdate,inserttime,preview,|,forecolor,backcolor",
                theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,ltr,rtl,|,fullscreen",
                theme_advanced_buttons4: "insertlayer,moveforward,movebackward,absolute,|,styleprops,spellchecker,|,cite,abbr,acronym,del,ins,|,visualchars,nonbreaking,template,blockquote,pagebreak,|,insertfile,insertimage",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: false,
                valid_children: "+body[style]",
                inline_styles: true,
                cleanup: false,
                //extended_valid_elements: "a[href|target|class]",
                // Drop lists for link/image/media/template dialogs
                template_external_list_url: "lists/template_list.js",
                external_link_list_url: "lists/link_list.js",
                external_image_list_url: "lists/image_list.js",
                media_external_list_url: "lists/media_list.js",
                init_instance_callback: function () {
                    if (typeof initCallBack == 'function') {
                        initCallBack();
                    }
                }
            });

        },

        handleConflicts: function () {
            this.handleEmptyAnchorsBeforeEditing();
            this.handle3rdPartyControlsBeforeEditing();
        },

        handle3rdPartyControlsBeforeEditing: function () {
            var config = getConfig();
            $("[" + config.thirdPartyAttributeForConflictID + "]").hide();
        },

        handleEmptyAnchorsBeforeEditing: function () {

            if (currentSelectedControl == undefined || currentSelectedControl == null) return;

            //Note: Tinymce has problems rendering empty text anchors(a)
            //To fix: Add &nbsp as default text so tinymce will be able to render it
            var anchors = $(currentSelectedControl).find('a');
            anchors.each(function (index, anchor) {
                if ($.trim($(anchor).html()) == editorConstants.EMPTY_VALUE) $(anchor).html("&nbsp");
            });

        },

        updateStringResource: function () {

            /* Summary:

            updateStringResource() does the following:

            1. On call of this method show saving progress message
            2. Get string resource key and value 
            3. Using AjaxCallWithSecuritySimplified (secured) call action service method: UpdateContent
            4. If requested method(UpdateContent) was successfully called hide the saving process message
            5. If UpdateContent method returns and empty string calls this plugin function UpdateElementWithAttributeOf then close the dialog
            else call this plugin ShowSavingFailedMessage function
            6. If request of method UpdateContent fails hide saving progress message and call this pluging ShowSavingFailedMessage function 

            */
            var config = getConfig();

            this.hideSavingFailedMessage(this.selectorChecker(config.stringResourceErrorPlaceHolderId));
            this.showSavingProgressIndicator(config.messages.MESSAGE_SAVING_PROGRESS);

            var param = this.getEditorKeyValueParam();

            if (param.contentValue == editorConstants.EMPTY_VALUE) {

                thisCMSEditorPlugin.showSavingFailedMessage(this.selectorChecker(config.stringResourceErrorPlaceHolderId), "Config Value is required");
                thisCMSEditorPlugin.hideSavingProgressIndicator(this.selectorChecker(config.stringResourceSavingButtonPlaceHolderId), this.selectorChecker(config.stringResourceProgressMessagePlaceHolderId));
                return false;
            }

            AjaxCallWithSecuritySimplified(
                "UpdateStringResourceConfigValue",
                param,
                function (result) {

                    thisCMSEditorPlugin.hideSavingProgressIndicator(thisCMSEditorPlugin.selectorChecker(config.stringResourceSavingButtonPlaceHolderId), thisCMSEditorPlugin.selectorChecker(config.stringResourceProgressMessagePlaceHolderId));

                    if (result.d != editorConstants.EMPTY_VALUE) {

                        thisCMSEditorPlugin.showSavingFailedMessage(result.d);

                    } else {

                        thisCMSEditorPlugin.updateElementWithAttributeOf(param.contentKey, param.contentValue);

                        $(thisCMSEditorPlugin.selectorChecker(config.stringResourceEditorDialogId)).dialog("close");
                    }

                },
                function (result) {

                    thisCMSEditorPlugin.hideSavingProgressIndicator(thisCMSEditorPlugin.selectorChecker(config.stringResourceSavingButtonPlaceHolderId), thisCMSEditorPlugin.selectorChecker(config.stringResourceProgressMessagePlaceHolderId));
                    thisCMSEditorPlugin.showSavingFailedMessage(thisCMSEditorPlugin.selectorChecker(config.stringResourceErrorPlaceHolderId), result.d);

                }
            );

        },

        updateItemDescription: function () {

            var config = getConfig();

            var errorPlaceHolderId = this.selectorChecker(config.itemDescriptionErrorPlaceHolderId);
            var savingButtonPlaceHolderId = this.selectorChecker(config.itemDescriptionSavingButtonPlaceHolderId);
            var progressMessagePlaceHolderId = this.selectorChecker(config.itemDescriptionProgressMessagePlaceHolderId);

            this.hideSavingFailedMessage(errorPlaceHolderId);
            this.showSavingProgressIndicator(config.messages.MESSAGE_SAVING_PROGRESS);

            var contentKey = $.trim($(this.selectorChecker(config.itemDescriptionItemCode)).val());
            var contentValue = $.trim($(this.selectorChecker(config.itemDescriptionContentValue)).val());
            var contentType = "item-description";

            if (contentKey == editorConstants.EMPTY_VALUE) {

                thisCMSEditorPlugin.showSavingFailedMessage(errorPlaceHolderId, "Item code is required");
                thisCMSEditorPlugin.hideSavingProgressIndicator(savingButtonPlaceHolderId, progressMessagePlaceHolderId);
                return false;
            }

            if (contentValue == editorConstants.EMPTY_VALUE) {

                thisCMSEditorPlugin.showSavingFailedMessage(errorPlaceHolderId, "Item description is required");
                thisCMSEditorPlugin.hideSavingProgressIndicator(savingButtonPlaceHolderId, progressMessagePlaceHolderId);

                return false;
            }

            var param = new Object();

            param.contentKey = contentKey;
            param.contentValue = contentValue;
            param.contentType = contentType;

            AjaxCallWithSecuritySimplified(
                "UpdateItemDescriptionFromEditor",
                param,
                function (result) {

                    thisCMSEditorPlugin.hideSavingProgressIndicator(savingButtonPlaceHolderId, progressMessagePlaceHolderId);

                    if (result.d == false) {

                        thisCMSEditorPlugin.showSavingFailedMessage(errorPlaceHolderId, config.messages.MESSAGE_SAVING_TOPIC_ADMIN_NOTLOGGEDIN_ERROR);

                    } else {

                        $(thisCMSEditorPlugin.selectorChecker(config.itemDescriptionEditorDialogId)).dialog("close");
                        $("[data-itemCode='" + contentKey + "']").each(function () {

                            if ($.trim($(this).attr("data-contentType")) == contentType) {
                                $(this).children("div.string-value").html($.trim(contentValue));
                            }

                        });

                    }
                },
                function (result) {

                    thisCMSEditorPlugin.hideSavingProgressIndicator(savingButtonPlaceHolderId, progressMessagePlaceHolderId);
                    thisCMSEditorPlugin.showSavingFailedMessage(errorPlaceHolderId, result.d);

                }
            );

        },
        updateItemWebDescription: function () {

            var config = getConfig();

            var errorPlaceHolderId = this.selectorChecker(config.itemWebDescriptionErrorPlaceHolderId);
            var savingButtonPlaceHolderId = this.selectorChecker(config.itemWebDescriptionSavingButtonPlaceHolderId);
            var progressMessagePlaceHolderId = this.selectorChecker(config.itemWebDescriptionProgressMessagePlaceHolderId);

            this.hideSavingFailedMessage(errorPlaceHolderId);
            this.showSavingProgressIndicator(config.messages.MESSAGE_SAVING_PROGRESS);

            var param = this.getEditorKeyValueParam();
            if (param.itemWebDescriptionItemCode == editorConstants.EMPTY_VALUE) {

                thisCMSEditorPlugin.showSavingFailedMessage(errorPlaceHolderId, "Item code is required");
                thisCMSEditorPlugin.hideSavingProgressIndicator(savingButtonPlaceHolderId, progressMessagePlaceHolderId);
                return false;
            }

            if (tinyMCE.activeEditor.getContent() == editorConstants.EMPTY_VALUE) {

                thisCMSEditorPlugin.showSavingFailedMessage(errorPlaceHolderId, "Item description is required");
                thisCMSEditorPlugin.hideSavingProgressIndicator(savingButtonPlaceHolderId, progressMessagePlaceHolderId);

                return false;
            }

            var contentKey = $.trim($(this.selectorChecker(config.itemWebDescriptionItemCode)).val());
            var contentValue = $.trim($(this.selectorChecker(config.itemWebDescriptionContentValue)).val());

            var contentType = $.trim(currentSelectedControl.parent("div").attr("data-contenttype").toLowerCase());

            param = new Object();
            param.contentKey = contentKey;
            param.contentValue = tinyMCE.activeEditor.getContent();
            param.contentType = contentType;


            AjaxCallWithSecuritySimplified(
                "UpdateItemDescriptionFromEditor",
                param,
                function (result) {

                    thisCMSEditorPlugin.hideSavingProgressIndicator(savingButtonPlaceHolderId, progressMessagePlaceHolderId);

                    if (result.d == false) {
                        thisCMSEditorPlugin.showSavingFailedMessage(errorPlaceHolderId, config.messages.MESSAGE_SAVING_TOPIC_ADMIN_NOTLOGGEDIN_ERROR);
                    } else {
                        location.reload();
                    }
                },
                function (result) {

                    thisCMSEditorPlugin.hideSavingProgressIndicator(savingButtonPlaceHolderId, progressMessagePlaceHolderId);
                    thisCMSEditorPlugin.showSavingFailedMessage(errorPlaceHolderId, result.d);

                }
            );

        },
        getEditorKeyValueParam: function () {

            var config = getConfig();

            var contentKey = $.trim($(this.selectorChecker(config.contentKeyId)).val());
            var contentValue = $.trim($(this.selectorChecker(config.contentValueId)).val());

            var param = new Object();
            param.contentKey = contentKey;
            param.contentValue = contentValue;

            return param;
        },

        updatePhoto: function () {

        },

        updateElementWithAttributeOf: function (attributeValue, newValue) {

            /* Summary:

            updateElementWithAttributeOf() does the following:

            1. Upon successful saving of string resource scan all elements with data-contentKey value equals to the 
            content key modified string resource
            2. Checks if element type is button - if true update its current value by the new value of the string resource then return true
            3. If #2 is false checks if type is undefined update its current html with the new value of the string resource
            4. If type is not undefined (case for textbox or textarea) get the id of the label control associated with the input text or textarea then 
            update its html content
    
            */

            $("[data-contentkey='" + attributeValue + "']").each(function () {

                var type = $(this).attr("type");
                $(this).attr("data-contentValue", newValue);

                if (type == "button" || type == "submit") {

                    $(this).val(newValue);
                    return true;

                }

                if (typeof (type) == "undefined") {

                    $(this).children("div.string-value").html(newValue);

                } else {

                    var id = $(this).parent("span").children()[0].id;
                    $("#" + id).html(newValue);

                }

            });
        },

        handleStringResourceInsideALabel: function (thisStringResource) {

            /* Summary (special case):

            This function will handle string resource with span and rendered inside a label control and used a behind caption of an input box (text or textarea)

            handleStringResourceInsideALabel() does the following:

            1. On LookForEditableContents methods: recieved the string resource element (place holder) as object
            2. Checks if element has a parent with class of: form-field-label
            3. If #2 is true get the id and type of the control (text, textarea, or password) 
            4. Append class: editable-content content to the control (text, textarea, or password)
            5. Append attributes: data-contentType, data-contentKey, and data-contentValue to  the control (text, textarea, or password)
            6. Remove the <span> from the string resource value by updating the html content of its parent element (label)

            */

            if (thisStringResource.parent().hasClass("form-field-label")) {

                var inputId = thisStringResource.parent().parent().children()[1].id;
                var inputType = thisStringResource.parent().parent().children()[1].type;

                if (inputType == "text" || inputType == "textarea" || inputType == "password") {

                    var tempStr = "<div class='content editable-content' data-contentKey='" + thisStringResource.attr("data-contentKey") + "' data-contentValue='" + thisStringResource.attr("data-contentValue") + "' data-contentType='string resource'>";
                    tempStr += "<div class='edit-pencil'></div>";
                    tempStr += "<div class='string-value' style='color:#fff !important'>" + $.trim(thisStringResource.attr("data-contentValue")) + "</div>";
                    tempStr += "<div class='clear-both'></div>";
                    tempStr += "</div>";

                    $("#" + inputId).remove();

                    thisStringResource.parent().parent().html(tempStr);

                }

            }
        },

        handleInputButtonStringResource: function (thisButton) {

            var contentKey = thisButton.attr("data-contentKey");
            var contentValue = thisButton.attr("data-contentValue");

            var tempStr = "<div class='content editable-content' data-contentKey='" + contentKey + "' data-contentValue='" + contentValue + "' data-contentType='string resource'>";
            tempStr += "<div class='edit-pencil'></div>";
            tempStr += "<div class='string-value' style='color:#fff !important'>" + $.trim(thisButton.val()) + "</div>";
            tempStr += "<div class='clear-both'></div>";
            tempStr += "</div>";

            thisButton.parent().append(tempStr);
            thisButton.remove();

        },

        showSavingProgressIndicator: function (message) {

            var config = getConfig();

            $(this.selectorChecker(config.stringResourceSavingButtonPlaceHolderId)).addClass("display-none");
            $(this.selectorChecker(config.stringResourceProgressMessagePlaceHolderId)).html(message);

        },

        hideSavingProgressIndicator: function (placeholderId, buttonPlaceHolderId) {

            $(this.selectorChecker(config.stringResourceSavingButtonPlaceHolderId)).removeClass("display-none");
            $(this.selectorChecker(config.stringResourceProgressMessagePlaceHolderId)).hide();

        },

        showSavingFailedMessage: function (placeholderId, message) {
            var config = getConfig();
            $(placeholderId).html(message);
            $(placeholderId).removeClass("display-none");

        },
        hideSavingFailedMessage: function (placeholderId) {

            $(placeholderId).html("");
            $(placeholderId).addClass("display-none");

        },
        attachAutoScrollOnDialog: function (dialogId) {

            $(window).resize(function () {
                $(dialogId).dialog("option", "position", "center");
            });

            $(window).scroll(function () {
                $(dialogId).dialog("option", "position", "center");
            });

        }
    }

    //jqueryBasePlugin is located inside core.js
    $.extend($.fn.CmsEditor, new jqueryBasePlugin());

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

})(jQuery);