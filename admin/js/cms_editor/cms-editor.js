$(document).ready(function () {
    CMSImageEditorInit();
});

function CMSImageEditorInit() {
    $.getScript("js/cms_editor/jquery.cmseditor-plugin-template.js").done(function (script, textStatus) {
        $.getScript("js/cms_editor/jquery.cmseditor-plugin.js").done(function (script, textStatus) {
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
                jqueryFormPluginPath: "../jscripts/jquery/jquery.form.js", // set this to null or empty if jquery.form.js plugin is already loaded somewhere

                //set CMS editor css path if you want to apply the css per skin or from other directory.
                //If Not supplied, it will get the default style under jscripts/cms_editor/css/index.css
                // comment it like this //cmsEditorCssPath if you want to get the default style
                cmsEditorCssPath: "js/cms_editor/css/index.css",
                messages:
                {
                    MESSAGE_SAVING_PROGRESS: 'Saving...',
                    MESSAGE_SAVING_TOPIC_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.',
                    MESSAGE_OPEN_TOPIC_NOT_SUPPORTED_ERROR: 'Sorry, topic editing is currently not supported in mobile browser.',
                    MESSAGE_SAVING_IMAGE_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.'
                }
            });

        }).fail(function (jqxhr, settings, exception) { alert(exception); });
    }).fail(function (jqxhr, settings, exception) { alert(exception); });
}