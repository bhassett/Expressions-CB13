$(document).ready(function () {
    InitButtonValue();
});

function ProcessCmsPanelButton(callerObject) {

    var active = $(callerObject).attr('data-active');
    var isTrue = parseBool(active);

    var param = new Object();
    param.mode = isTrue;

    AjaxCallCommon("ActionService.asmx/TogglePageEditMode", param,
        function () {
            var arr = location.href.split("?");

            if (arr.length > 1) {

                var qryString = arr[1];

                //CMS_ENABLE_EDITMODE query string passed to the page if intended to switch to edit mode

                if (qryString.indexOf("CMS_ENABLE_EDITMODE") != -1) {
                    location.href = arr[0];
                }
                else {
                    location.reload();
                }

            }
            else {
                location.reload();
            }

        },
        function (error) {
        }
    );

}

//function InitCmsPanel() {

//    var keys = new Array();
//    keys.push('cms.browsemode');
//    keys.push('cms.designmode');

//    //to ensure that keys already loaded
//    var callBack = function () {
//        $("#cms-user-panel").show('slow', function () {
//            InitButtonValue();
//        });
//    }

//    ise.StringResource.loadResources(keys, callBack);
//}

function InitButtonValue() {

    AjaxCallCommon("ActionService.asmx/IsPageEditMode", null,

        function (data) {

            var isEditMode = data.d;

            var browseModeText = 'Swich to Browse Mode' //ise.StringResource.getString('cms.browsemode');
            var designModeText = 'Swich to Design Mode' //ise.StringResource.getString('cms.designmode');

            $.getScript("jscripts/cms_editor/jquery.cmseditor-plugin-template.js").done(function (script, textStatus) {

                //Tempate "cmsEditorToolbar" is inside jquery.cmseditor-plugin-template.js
                $.tmpl("cmsEditorToolbar", null).prependTo('body');

                $("#cms-user-panel").show('slow', function () {
                    $("#cms-user-panel-command-button").attr('data-active', !isEditMode);

                    if (!isEditMode) {
                        $("#cms-user-panel-command-button").val(designModeText);
                    }
                    else {
                        $("#cms-user-panel-command-button").val(browseModeText);
                    }

                });

                if (isEditMode) {

                    $.getScript("jscripts/cms_editor/jquery.cmseditor-plugin.js").done(function (script, textStatus) {

                        //setup the width/height of the topic popup based on the current document width/height
                        var curentDocWidth = ($(document).width() - $(document).width() * .58);
                        var curentDocHeight = ($(document).height() - $(document).height() * .48);

                        if (curentDocHeight > 700) { curentDocHeight = 700; }
                        if (curentDocWidth > 800) { curentDocWidth = 800; }

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
                            imageEditorCancel: 'btnCancelImageUploader',
                            thirdPartyAttributeForConflictID: 'cms-3rdparty-attr',
                            jqueryFormPluginPath: "jscripts/jquery/jquery.form.js", // set this to null or empty if jquery.form.js plugin is already loaded somewhere

                            //set CMS editor css path if you want to apply the css per skin or from other directory.
                            //If Not supplied, it will get the default style under jscripts/cms_editor/css/index.css
                            // comment it like this //cmsEditorCssPath if you want to get the default style
                            cmsEditorCssPath: "skins/Skin_" + ise.Configuration.getConfigValue('DefaultSkinID') + "/plugin/cms_editor/index.css",
                            messages:
                            {
                                MESSAGE_SAVING_PROGRESS: 'Saving...',
                                MESSAGE_SAVING_TOPIC_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.',
                                MESSAGE_OPEN_TOPIC_NOT_SUPPORTED_ERROR: 'Sorry, topic editing is currently not supported in mobile browser.',
                                MESSAGE_SAVING_IMAGE_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.'
                            }
                        });

                    }).fail(function (jqxhr, settings, exception) { alert(exception); });

                }
                else {

                    $("#cms-user-panel-command-button").val(designModeText);

                }


            }).fail(function (jqxhr, settings, exception) { alert(exception); });

        },
        function (error) {

        }

    );

}