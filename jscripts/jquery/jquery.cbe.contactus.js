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
        messages:
        {
            MESSAGE_SENDING_PROGRESS : 'Sending...',
        },
        confactFormControls:
        {
            CONTACT_NAME_ID    : 'txtContactName',
            EMAIL_ADDRESS_ID   : 'txtEmail',
            PHONE_ID           : 'txtContactNumber',
            SUBJECT_ID         : 'txtSubject',
            MESSAGE_DETAILS_ID : 'txtMessageDetails',
            CAPTCHA_ID         : 'txtCaptcha'
        },
        bubbleMessagePlaceHolderId              : 'ise-message-tips',
        bubbleMessageRequiresValidationClass    : '.requires-validation',
        requiredInputClass                      : 'required-input',
        objectOnFocusClass                      : 'current-object-on-focus'

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

            this.attachEventsListener();
            this.initializedBubbleMessage();
            this.setBubbleMessageRequiredStringResources();

        },
        initializedBubbleMessage: function (){
            
            var config = getConfig();

            var contactNameId    = config.confactFormControls.CONTACT_NAME_ID;
            var emailId          = config.confactFormControls.EMAIL_ADDRESS_ID;
            var phoneId          = config.confactFormControls.PHONE_ID;
            var subjectId        = config.confactFormControls.SUBJECT_ID;
            var messageDetailsId = config.confactFormControls.MESSAGE_DETAILS_ID;
            var captchaId        = config.confactFormControls.CAPTCHA_ID;

            $(selectorChecker(contactNameId)).ISEBubbleMessage({ "input-id": contactNameId, "label-id": "lblContactName" });
            $(selectorChecker(emailId)).ISEBubbleMessage({ "input-id": emailId, "label-id": "lblEmail", "input-mode": "email" });
            $(selectorChecker(phoneId)).ISEBubbleMessage({ "input-id": phoneId, "label-id": "lblContactNumber" });

            $(selectorChecker(subjectId)).ISEBubbleMessage({ "input-id": subjectId, "label-id": "lblSubject" });
            $(selectorChecker(messageDetailsId)).ISEBubbleMessage({ "input-id": messageDetailsId, "label-id": "lblMessageDetails" });
            
            $(selectorChecker(captchaId)).ISEBubbleMessage({ "input-id":  captchaId, "label-id": "lblCaptcha" });

        },
        attachEventsListener: function(){

            var config = getConfig();

            $(selectorChecker(config.securityCodeRefreshButtonId)).unbind('click');
            $(selectorChecker(config.securityCodeRefreshButtonId)).click(function () {
                  securityCodeCounter++;
                  $("#captcha").attr("src", "Captcha.ashx?id=" + securityCodeCounter);
            });

        },
        setBubbleMessageRequiredStringResources: function(){

              var setBubbleMessageGlobalVariable = function(){

                _IncompleteMessage          = ise.StringResource.getString("customersupport.aspx.15");
                _InvalidEmailMessage        = ise.StringResource.getString("customersupport.aspx.16");
                _InvalidCaptchaMessage      = ise.StringResource.getString("customersupport.aspx.18");
               
              }

              var keys = new Array();

              keys.push("customersupport.aspx.15");
              keys.push("customersupport.aspx.16");
              keys.push("customersupport.aspx.18");

              loadStringResource(keys, setBubbleMessageGlobalVariable);

        },
        validate: function(){
           
            var config  = getConfig();
            var formHasEmptyFields = false;
         
            var counter = 0;

            $(selectorChecker(config.bubbleMessagePlaceHolderId)).fadeOut("slow");

            $(selectorChecker(config.bubbleMessageRequiresValidationClass)).each(function () {
            
                var $this = $(this);
                if($this.val() == ""){

                    $this.removeClass(config.objectOnFocusClass);
                    $this.addClass(config.requiredInputClass);

                    if(counter == 0){

                        $this.addClass(config.objectOnFocusClass);
                        $this.focus();

                    }

                    formHasEmptyFields = true;
                    counter++;
                }

            });

            var $thisEmail = $(selectorChecker(config.confactFormControls.EMAIL_ADDRESS_ID));

            var emailIsValid = validateEmailAddress( $thisEmail.val());

            if (emailIsValid == false && formHasEmptyFields == false) $thisEmail.focus();
            if (formHasEmptyFields || emailIsValid == false) return false;

            return true;
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