$(document).ready(function(){
     $(this).ContactUs.Initialize();
});

(function ($) {
    
    var thisContactUsPlugIn;
    var securityCodeCounter = 1;

    var config = {};

    var global = {
        selected: '',
        selector: ''
    };

    var currentSelectedControl = null;

    var editorConstants = {
        EMPTY_VALUE: '',
        DOT_VALUE: '.'
    }

    var defaults = {

        sendMessageButtonId              : 'send-message',
        sendMessageButtonPlaceHolderId   : "contact-form-button-place-holder",
        sendMessageProgressPlaceHolderId : "sending-message-progress-place-holder",
        sendMessageErrorPlaceHolderId    : "sending-message-error-place-holder",
        securityCodeRefreshButtonId      : "captcha-refresh-button",
        summaryBoardErrorsID             : "errorSummary_Board_Errors",
        messages:
        {
            MESSAGE_SENDING_PROGRESS : 'Sending your message, please wait this may take a moment',
            MESSAGE_INCOMPLETE_FORM  : 'Please complete this required information',
            MESSAGE_INVALID_EMAIL    : 'Invalid email address format'

        },
        confactFormControls:
        {
            CONTACT_NAME_ID     : 'txtContactName',
            EMAIL_ADDRESS_ID    : 'txtEmail',
            AREA_CODE_ID        : 'txtAreaCode',
            PRIMARY_PHONE_ID    : 'txtPrimaryPhone',
            SUBJECT_ID          : 'txtSubject',
            MESSAGE_DETAILS_ID  : 'txtMessageDetails',
            CAPTCHA_ID          : 'txtCaptcha',
            REQUIRED_INPUT      : '.required-input',
            ERROR_PLACEHOLDER_ID: 'page-top-links-place-holder'
        }
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

    $.fn.ContactUs = {

        Initialize: function (options) {

            setConfig($.extend(defaults, options)); 

            thisContactUsPlugIn = this;
            thisContactUsPlugIn.attachEventsListener();
            thisContactUsPlugIn.checkforErrorsOnLoad();

        },
        attachEventsListener: function(){

            var config = getConfig();

            $(selectorChecker(config.securityCodeRefreshButtonId)).unbind('click');
            $(selectorChecker(config.securityCodeRefreshButtonId)).click(function () {
                  securityCodeCounter++;
                  $("#captcha").attr("src", "Captcha.ashx?id=" + securityCodeCounter);
            });

        },
        validate: function(){
            var config  = getConfig();
            var errorPlaceHolder = $(selectorChecker(config.confactFormControls.ERROR_PLACEHOLDER_ID));
            
            if(thisContactUsPlugIn.requiredInputIsEmpty()){

                errorPlaceHolder.html(config.messages.MESSAGE_INCOMPLETE_FORM);
                errorPlaceHolder.addClass("bad-form");

                return false;
            }
         
            var $thisEmail = $(selectorChecker(config.confactFormControls.EMAIL_ADDRESS_ID));

            if (validateEmailAddress($thisEmail.val())== false){
            
                errorPlaceHolder.html(config.messages.MESSAGE_INVALID_EMAIL);
                errorPlaceHolder.addClass("bad-form");

                $thisEmail.focus();

                return false;
            }

            errorPlaceHolder.html(config.messages.MESSAGE_SENDING_PROGRESS);
            errorPlaceHolder.removeClass("bad-form");

            return true;
        },
        requiredInputIsEmpty: function(){

            var config  = getConfig();
            var hasEmpty = false;

            $(selectorChecker(config.confactFormControls.REQUIRED_INPUT)).each(function(){
                if($(this).val()==editorConstants.EMPTY_VALUE) hasEmpty = true;
            });

            return hasEmpty;
        },
        checkforErrorsOnLoad: function(){

            var config  = getConfig();
            var errorBoard = $(selectorChecker(config.summaryBoardErrorsID));
            var errorPlaceHolder = $(selectorChecker(config.confactFormControls.ERROR_PLACEHOLDER_ID));

            if( errorBoard.children("li").length>0){
                errorPlaceHolder.html(errorBoard.children("li:first").html());
                errorPlaceHolder.addClass("bad-form");
                errorBoard.addClass("hidden");
            }else{
                 errorPlaceHolder.removeClass("bad-form");
            }

        }
    }
    
    

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

    function selectorChecker(selector) {

        if (selector == editorConstants.EMPTY_VALUE) return selector;

        if (selector.indexOf(editorConstants.DOT_VALUE) == -1) {
            selector = "#" + selector;
        }
        return selector;
    }

    function loadStringResource(keys, callBack) {

        ise.StringResource.loadResources(keys, callBack);
    }

    function validateEmailAddress(email){
        var emailPattern = /(^[a-zA-Z0-9._-]+@[a-zA-Z0-9]+([.-]?[a-zA-Z0-9]+)?([\.]{1}[a-zA-Z]{2,4}){1,4}$)/;
        return emailPattern.test(email);
    }

  })(jQuery);

  function formInfoIsGood(){  
       return $(this).ContactUs.validate();
  }

