
/* Global Loader */
(function ($) {

    $.globalLoader = function (element, options) {

        var defaults = {
            delay: 1000,
            background: '#FFFFFF',
            image: 'ajax-loader.gif',
            zIndex: 99,
            autoHide: true,
            opacity: 0.3,
            text: 'loading...'
        };

        var plugin = this;

        plugin.settings = {}

        var $element = $(element),
             element = element,
             globalMaskID = '',
             loaderID = '';

        var constants = {
            EMPTY_VALUE: '',
            DOT_VALUE: '.'
        };

        // INITIALIZE
        plugin.init = function () {
            plugin.settings = $.extend({}, defaults, options);

            //set ids
            globalMaskID = 'divGlobalMask';
            loaderID = 'divGlobalLoader';

            var globalMask = $("<div />", { id: globalMaskID });
            var loader = $("<div />", { id: loaderID }); 
            $('body').append(globalMask);
            $('body').append(loader);

            $(selectChecker(loaderID)).append("<img src='" + plugin.settings.image + "' />");
            $(selectChecker(loaderID)).append("<span>" + plugin.settings.text + "</span>");

            //hide popup loader
            $(selectChecker(loaderID)).hide();
        }

        // PUBLIC METHODS
        plugin.show = function () {
            displayMask();
            displayPopup();
            autoHideChecker();
        }
        plugin.hide = function () {
            //hide the mask and popup loader
            $(selectChecker(loaderID)).fadeOut();
            $(selectChecker(globalMaskID)).fadeOut();
        }

        // PRIVATE METHODS
        var displayMask = function () {
            var height = $(document).height();
            var width = $(window).width();

            //set mask css
            $(selectChecker(globalMaskID)).css({
                width: width,
                height: height,
                background: plugin.settings.background,
                position: 'absolute',
                left: 0,
                top: 0,
                display: 'inline-block',
                'z-index': plugin.settings.zIndex
            }).fadeTo("fast", plugin.settings.opacity);
        }

        var displayPopup = function () {
            var height = $(window).height();
            var width = $(window).width();
            var popup = $(selectChecker(loaderID));
            var posTop = (height / 2 - popup.height() / 2);
            var posLeft = (width / 2 - popup.width() / 2);

            $(selectChecker(loaderID)).css({
                top: posTop,
                left: posLeft,
                position: 'fixed',
                'z-index': 100
            }).show();
        }

        var autoHideChecker = function () {
            // check if auto-hide
            if (plugin.settings.autoHide) {
                window.setTimeout(function () {
                    plugin.hide();
                }, plugin.settings.delay);
            }
        }

        var selectChecker = function (selector) {
            if (selector == constants.EMPTY_VALUE) return selector;

            if (selector.indexOf(constants.DOT_VALUE) == -1) {
                selector = "#" + selector;
            }
            return selector;
        }

        plugin.init();
    }

    $.fn.globalLoader = function (options) {
        return this.each(function () {
            if (undefined == $(this).data('globalLoader')) {
                var plugin = new $.globalLoader(this, options);
                $(this).data('globalLoader', plugin);
            }
        });
    }

})(jQuery);


/* Content Loader */
(function ($) {

    $.contentLoader = function (element, options) {

        var defaults = {
            delay: 1000,
            background: '#FFFFFF',
            image: 'ajax-loader.gif',
            zIndex: 99,
            autoHide: true,
            opacity: 0.3, 
            text: 'loading...'
        };

        var plugin = this;

        plugin.settings = {}

        var $element = $(element),
             element = element,
             globalMaskID = '',
             loaderID = '';

        var constants = {
            EMPTY_VALUE: '',
            DOT_VALUE: '.'
        };

        // INITIALIZE
        plugin.init = function () {
            plugin.settings = $.extend({}, defaults, options);

            globalMaskID = 'divGlobalMask';
            loaderID = 'divContentLoader';

            var globalMask = $('<div />', { id: globalMaskID });
            var loader = $('<div />', { id: loaderID }); //todo: change to image

            //todo: add checker if global mask and loader is already populated
            $('body').append(globalMask);
            $('body').append(loader);

            $(selectChecker(loaderID)).append("<img src='" + plugin.settings.image + "' />");
            $(selectChecker(loaderID)).append("<span>" + plugin.settings.text + "</span>");
        }

        // PUBLIC METHODS
        plugin.show = function () {
            var height = $(element).height();
            var width = $(element).width();
            var position = $(element).position();

            $(selectChecker(globalMaskID)).css({
                width: width,
                height: height,
                background: plugin.settings.background,
                position: 'absolute',
                left: position.left,
                top: position.top,
                display: 'inline-block',
                'z-index': plugin.settings.zIndex
            }).fadeTo("fast", plugin.settings.opacity);

            centerToBox($(selectChecker(loaderID)), $(element));

            $(selectChecker(loaderID)).show();

            // check if auto-hide
            if (plugin.settings.autoHide) {
                window.setTimeout(function () {
                    plugin.hide();
                }, plugin.settings.delay);
            }
        }
        plugin.hide = function () {
            $(selectChecker(loaderID)).fadeOut();
            $(selectChecker(globalMaskID)).fadeOut();
        }

        // PRIVATE METHODS
        var selectChecker = function (selector) {
            if (selector == constants.EMPTY_VALUE) return selector;

            if (selector.indexOf(constants.DOT_VALUE) == -1) {
                selector = "#" + selector;
            }
            return selector;
        }
        var centerToBox = function (cur, box) {
            var position = $(box).position();
            var topOffset = ($(box).height()) / 2;
            var leftOffset = ($(box).width()) / 2;
            position.left = position.left + leftOffset;
            position.top = position.top + ( topOffset - $(cur).height() / 2 );
            
            $(cur).css({
                position: 'absolute',
                left: position.left,
                top: position.top,
                'z-index': 100
            });
        }


        plugin.init();
    }

    $.fn.contentLoader = function (options) {
        return this.each(function () {
            if (undefined == $(this).data('contentLoader')) {
                var plugin = new $.contentLoader(this, options);
                $(this).data('contentLoader', plugin);
            }
        });
    }

})(jQuery);
