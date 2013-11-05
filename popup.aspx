<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.popup" CodeFile="popup.aspx.cs" %>

<html>

<head runat="server" id="hdrMain">
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <style type="text/css">

        TD, SPAN, DIV { color: #363636; font-family: Georgia,Arial,Helvetica,sans-serif; font-size: 12px; font-style: italic; }
        
        .site-button { height:26px !important; font-weight:bold !important; font-size:11px !important; border:1px solid #ccc !important; 
                       border-bottom-color:#aaa !important; -moz-border-radius:3px !important; -webkit-border-radius:3px !important; border-radius:3px !important; 
                       -moz-box-shadow:inset 0 0 1px #fff !important; -ms-box-shadow:inset 0 0 1px #fff !important; -webkit-box-shadow:inset 0 0 1px #fff !important; 
                       -moz-border-bottom-colors: none !important; }
                       
        .site-button { background-color: #6891E7 !important; background-image: linear-gradient(to bottom, #61BFF2 0px, #3B7EB8 100%) !important; 
                       border-color: #304EA6 #304EA6 #000000 !important; color: #FFFFFF !important; cursor: pointer !important;
                       text-shadow: 0 1px 0 rgba(0, 0, 0, 0.41) !important; }
                       
        .float-left { float:left; }
        .float-right { float:right; }
        .clear-both { clear:both; }
        .height-12 { height: 12px; }
        .topic-content:hover { float:left; background-color:yellow; cursor: text; }
        .topic-content { float:left; }
        .editor-button-topic-top { float:left; margin-left:10px; }
    
    </style>

</head>

<body runat="server">

    <script type="text/javascript">

        function InitTopicEditorScript() {

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
                    topicEditorSaveButtonTopId: 'btnSaveTopicTop',
                    topicEditorDialogWidth: curentDocWidth,
                    topicEditorDialogHeight: curentDocHeight,
                    messages:
                            {
                                MESSAGE_SAVING_PROGRESS: 'Saving...',
                                MESSAGE_SAVING_TOPIC_ADMIN_NOTLOGGEDIN_ERROR: 'Unable to process, Please check if admin is still logged in.'
                            }
                });

            });
            

        }

    </script>
    
    <form runat="server" id="frmForm">
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="false" ID="mgrScriptManager" ScriptMode="Release" >
        <Scripts>
            <asp:ScriptReference Path="~/jscripts/base_ajax.js" />
            <asp:ScriptReference Path="~/jscripts/system/custom-config-loader.js" />
        </Scripts>
    </asp:ScriptManager>
    <asp:Literal runat="server" ID="litTopicContent" ></asp:Literal>
    </form>



</body>
</html>
