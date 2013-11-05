(function ($) {

    var map;
    var config = {};
    var markers = [];
    var foundAddress;

    var thisObject;

    var locatorConstants = {
        EMPTY_VALUE: '',
        DOT_VALUE: '.'
    }

    var Address = function (address, city, state, postal, country) {
        this.address = address;
        this.city = city;
        this.state = state;
        this.postal = postal;
        this.country = country;

        this.getCompleteAddress = function () {
            return (this.address + ', ' + this.city + ', ' + this.state + ', ' + this.postal + ' ' + this.country);
        }
    }

    var Store = function () {
        this.Id = '';
        this.Title = '';
        this.Address = null;
        this.Latitude = 0;
        this.Longtitude = 0;
        this.IsUpdated = false;
        this.Captions = {
            MESSAGE_DEFAULT_DIRECTION_LINK_TEXT: ''
        }
        this.UpdateStoreLocationFromGoogle = function () {

            var storeOjbCopy = this;
            var updateStoreLatlLngCallBack = function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    storeOjbCopy.Latitude = results[0].geometry.location.lat();
                    storeOjbCopy.Longtitude = results[0].geometry.location.lng();
                    storeOjbCopy.IsUpdated = true;
                    thisObject.addStore(storeOjbCopy, true);
                } else {
                    storeOjbCopy.UpdateStoreLocationFromGoogle();
                }
            }

            var completeAddress = thisObject.formatStoresAddress(this);

            //Warning: google MAP geocoder limits the request. 
            //so we need to set an interval before the next request.
            var timerId = 0;
            var updateGeoAddress = function () {
                var geocoder = new google.maps.Geocoder();
                geocoder.geocode({ address: completeAddress }, updateStoreLatlLngCallBack);
                clearTimeout(timerId);
            }

            timerId = setTimeout(updateGeoAddress, 1000);
        }
    }

    var distanceInputTypes = {
        dropdown: 0,
        textbox: 1
    }

    var totalFoundStores = 0;

    var selectedStoreId;

    var infoWindow = new google.maps.InfoWindow({});
    var directionsService = new google.maps.DirectionsService();
    var directionsDisplay = new google.maps.DirectionsRenderer();

    var defaults = {
        zoom: 3,
        zoomControlOptions: { style: google.maps.ZoomControlStyle.SMALL },
        center: new google.maps.LatLng(0, 0),
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControlOptions:
        {
            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
            poistion: google.maps.ControlPosition.TOP_RIGHT,
            mapTypeIds: [google.maps.MapTypeId.ROADMAP,
              google.maps.MapTypeId.TERRAIN,
              google.maps.MapTypeId.HYBRID,
              google.maps.MapTypeId.SATELLITE]
        },
        navigationControl: true,
        navigationControlOptions: { style: google.maps.NavigationControlStyle.ZOOM_PAN },
        availableStores: [],
        searchInputId: '',
        searchInputButtonId: '',
        distanceInputId: '',
        distanceInputType: distanceInputTypes.dropdown,
        storeMenuId: 'marker-menu',
        storeMenuTemplate: '',
        menuHighlightClass: 'selectors-body-selected',
        headerContainerClass: '',
        infoWindowTemplate: '',
        addressInfoWindowTemplate: '',
        directionInputContainerId: '',
        directionInputId: '',
        directionlinkClass: '',
        getDirectionButtonId: '',
        storeTypeDropdownId: '',
        storesIcon: '',
        addressIcon: '',
        wideScreenButtonId: '',
        expandCollapseButtonId: '',
        mainLocatorContainerClass: '.locator-main-container',
        searchContainerClass: '.locator-search-detail',
        mapContainerClass: '.map-wrapper',
        messages:
        {
            MESSAGE_VALIDATION_SEARCH_INPUT: 'Please enter a valid search input',
            MESSAGE_VALIDATION_SEARCH_NOT_FOUND: 'Address not found.',
            MESSAGE_OUTPUT_SEARCH_FOUND: '{0} stores/customers found.',
            MESSAGE_DEFAULT_SEARCH_HEADER_TEXT: 'Search output',
            MESSAGE_DEFAULT_DIRECTION_HEADER_TEXT: 'Directions',
            MESSAGE_DEFAULT_DIRECTION_LINK_TEXT: 'Input Direction',
            MESSAGE_DEFAULT_WIDESCREEN_BUTTON_TITLE: 'Wide Screen',
            MESSAGE_DEFAULT_COLLAPSE_TITLE: 'Hide',
            MESSAGE_DEFAULT_EXPAND_TITLE: 'Show'
        }
    };

    var global = {
        selected: '',
        selector: ''
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

    $.fn.StoreLocator = {

        initialize: function (optionalConfig) {

            thisObject = this;

            if (optionalConfig) setConfig($.extend(defaults, optionalConfig));

            var config = getConfig();

            //selected[0] is the DOM element
            map = new google.maps.Map(global.selected[0], config);

            //ensure that the map is already loaded before all the events are attached
            google.maps.event.addListenerOnce(map, 'idle', function () {

                // do something only the first time the map is loaded

                thisObject.initializeCaptions();
                thisObject.showWideScreenButton();
                thisObject.showSearchButton();

                thisObject.attachEvents();
                thisObject.clear();

            });
        },

        showWideScreenButton: function () {
            var config = getConfig();
            $(this.selectorChecker(config.wideScreenButtonId)).show();
        },

        showSearchButton: function () {
            var config = getConfig();
            $(this.selectorChecker(config.searchInputButtonId)).show();
        },

        initializeCaptions: function () {
            var config = getConfig();
            $(this.selectorChecker(config.wideScreenButtonId)).attr('title', config.messages.MESSAGE_DEFAULT_WIDESCREEN_BUTTON_TITLE);
            $(this.selectorChecker(config.expandCollapseButtonId)).attr('title', config.messages.MESSAGE_DEFAULT_COLLAPSE_TITLE);
        },

        //display the stores to the map
        showStores: function () {
            var config = getConfig();
            for (var i = 0; i < config.availableStores.length; i++) {
                var curMarker = config.availableStores[i];
                curMarker.setMap(map);
            }
        },

        addStores: function (stores) {

            for (var i = 0; i < stores.length; i++) {

                var store = new Store();
                store.Id = stores[i].Address.id;
                store.Title = stores[i].Description;
                store.Latitude = stores[i].Coordinate.Latitude;
                store.Longtitude = stores[i].Coordinate.Longtitude;

                var state = (stores[i].Address.state != locatorConstants.EMPTY_VALUE) ? stores[i].Address.state : "," + locatorConstants.EMPTY_VALUE;
                store.Address = new Address(stores[i].Address.address, stores[i].Address.city, state, stores[i].Address.postalCode, stores[i].Address.country);

                //trigger the update per store.
                store.UpdateStoreLocationFromGoogle();

            }

        },

        formatStoresAddress: function (store) {

            if (!store) return locatorConstants.EMPTY_VALUE;

            var state = (store.Address.state != locatorConstants.EMPTY_VALUE) ? store.Address.state : "," + locatorConstants.EMPTY_VALUE;
            var country = (store.Address.country != locatorConstants.EMPTY_VALUE) ? store.Address.country : "," + locatorConstants.EMPTY_VALUE;

            return (store.Address.address + ', ' + store.Address.city + ', ' + state + ' ' + store.Address.postal + ', ' + country);
        },

        addStore: function (store, includeClick) {

            if (!store) return;

            var config = getConfig();

            var storeMarker = new google.maps.Marker({ position: new google.maps.LatLng(store.Latitude, store.Longtitude), title: store.Title, map: map });
            storeMarker.setIcon(config.storesIcon);

            //refence both
            storeMarker.Store = store;
            store.StoreMarker = storeMarker;

            config.availableStores.push(storeMarker);

            if (includeClick) this.addGoogleClick(storeMarker);
        },

        addDirectionClick: function () {

            var config = getConfig();
            var currentInfowindow = getCurrentInfoWindow();

            google.maps.event.addListener(currentInfowindow, 'domready', function () {
                $(".store-infowindow-direction-link").click(function (e) {

                    //if the main container is hidden then show it
                    var searchContainerSelector = thisObject.selectorChecker(config.searchContainerClass);
                    if (searchContainerSelector != null && searchContainerSelector != undefined) {
                        if ($(searchContainerSelector).is(':hidden')) {
                            $(searchContainerSelector).show('slow');
                        }
                    }

                    if ($(thisObject.selectorChecker(config.directionInputContainerId)).is(':hidden')) {
                        selectedStoreId = $(this).attr('id');
                        $(thisObject.selectorChecker(config.directionInputContainerId)).show('slow');
                        $(thisObject.selectorChecker(config.directionInputId)).focus();
                    }

                    return false;
                });
            });

        },

        hideDirectionContainer: function () {
            $(this.selectorChecker(config.directionInputId)).val('');
            $(this.selectorChecker(config.directionInputContainerId)).fadeOut('slow');
        },

        addGoogleClick: function (curMarker) {

            google.maps.event.clearListeners(curMarker, 'click');
            google.maps.event.addListener(curMarker, 'click', function (event) {

                var config = getConfig();

                thisObject.setCenter(this);
                if (!this.Store) return;

                var html;
                if (this.Store.Address) { //stores
                    this.Store.Captions.MESSAGE_DEFAULT_DIRECTION_LINK_TEXT = config.messages.MESSAGE_DEFAULT_DIRECTION_LINK_TEXT;
                    html = thisObject.parseTemplateReturnHtml(config.infoWindowTemplate, this.Store);
                } else { //searched address
                    html = thisObject.parseTemplateReturnHtml(config.addressInfoWindowTemplate, this.Store);
                }

                thisObject.showInfoWindow(html, this);
                thisObject.hideDirectionContainer();

            });
        },

        setCenter: function (marker) {
            map.setCenter(marker.getPosition());
        },

        autoZoom: function (marker) {

            var config = getConfig();
            var latlngbounds = new google.maps.LatLngBounds();
            for (var i = 0; i < config.availableStores.length; i++) {
                latlngbounds.extend(config.availableStores[i].getPosition());
            }

            //include the searched address
            if (foundAddress) latlngbounds.extend(foundAddress.getPosition());
            map.fitBounds(latlngbounds);
        },

        searchNow: function () {

            var config = getConfig();
            if (config.searchInputId == locatorConstants.EMPTY_VALUE) {
                alert('plugin search input id is not properly set.');
                return;
            }

            var inputElement = $(thisObject.selectorChecker(config.searchInputId));
            if (!inputElement) {
                alert('plugin search input does not exist');
                return;
            }

            if ($(inputElement).val().trim() == locatorConstants.EMPTY_VALUE) {
                alert(config.messages.MESSAGE_VALIDATION_SEARCH_INPUT);
                return;
            }

            thisObject.hideStreetView();

            var addressInput = $(inputElement).val().trim();

            $("body").data("globalLoader").show();

            //Warning: Google MAP geocoder limits the request. 
            //so we need to set an interval before the next request.
            var timeOutId = 0;
            var activateSearch = function () {
                var geocoder = new google.maps.Geocoder();
                geocoder.geocode({ address: addressInput }, thisObject.geocoderCallback);
                clearTimeout(timeOutId);
            };

            timeOutId = setTimeout(activateSearch, 1000);

        },

        geocoderCallback: function (results, status) {

            var config = getConfig();

            if (status == google.maps.GeocoderStatus.OK) {

                thisObject.hideMapDirection();
                thisObject.clearFoundAddress();
                thisObject.showAddress(results);
                thisObject.autoZoom();

                var distance = thisObject.getDistanceByInput();
                var storesWithinRange = thisObject.getStoresBySearch(results[0].geometry.location.lat(), results[0].geometry.location.lng(), distance);

            } else {

                alert(config.messages.MESSAGE_VALIDATION_SEARCH_NOT_FOUND);
                var inputElement = $(thisObject.selectorChecker(config.searchInputId));

                $(inputElement).focus();
                $(inputElement).select();

                $("body").data("globalLoader").hide();
            }

        },

        showAddress: function (result) {

            var config = getConfig();

            var store = new Store();
            store.Title = result[0].formatted_address;
            store.Latitude = result[0].geometry.location.lat();
            store.Longtitude = result[0].geometry.location.lng();

            foundAddress = new google.maps.Marker({ position: new google.maps.LatLng(store.Latitude, store.Longtitude), title: store.Title, map: map });
            foundAddress.setIcon(config.addressIcon);
            foundAddress.Store = store;

            this.setCenter(foundAddress);
            this.addGoogleClick(foundAddress);

        },

        hideStreetView: function () {
            map.getStreetView().setVisible(false);
        },

        getStoresBySearch: function (resultLatitude, resultLongtitude, distance) {

            var object = new Object();
            object.longtitude = resultLongtitude;
            object.latitude = resultLatitude;
            object.distance = distance;
            object.storeTypeCode = thisObject.getSelectedStoreType();

            var jsonData = JSON.stringify(object);

            $.ajax({
                type: "POST",
                url: './ActionService.asmx/GetWarehouseByAddress',
                data: jsonData,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {

                    var jsonResult = result.d;

                    thisObject.clear();

                    if (jsonResult != '') {
                        var stores = $.parseJSON(jsonResult);
                        totalFoundStores = stores.length;
                        thisObject.addStores(stores);

                        //wait for the update to finish
                        var updateToStoreLocationViaGoogleSuccess = function () {
                            thisObject.addDirectionClick();
                            thisObject.showStores();
                            thisObject.autoZoom();
                            thisObject.showMenu();

                            $("body").data("globalLoader").hide();
                        }

                        thisObject.listenIfAllStoreLocationIsAlreadyUpdated(updateToStoreLocationViaGoogleSuccess);
                    }
                },
                error: function (result, textStatus, exception) {
                    alert(result);
                }

            });

        },

        listenIfAllStoreLocationIsAlreadyUpdated: function (callBack) {

            var config = getConfig();

            var id = 0;
            var listener = function () {

                if (totalFoundStores != config.availableStores.length) return;

                var markers = $.grep(config.availableStores, function (marker, i) {
                    return (!marker.Store.IsUpdated);
                });

                if (markers.length == 0) {
                    clearInterval(id);
                    if (typeof callBack != 'undefined') callBack();
                }

            };

            id = setInterval(listener, 200);

        },

        showMenu: function () {

            var config = getConfig();

            this.clearStoreMenu();

            var stores = this.getStoresArray();
            this.displayHeader(stores.length);

            for (var i = 0; i < stores.length; i++) {

                var marker = stores[i].StoreMarker;

                var menuItem = this.parseTemplate(config.storeMenuTemplate, stores[i]);
                $(menuItem).hide();
                $(menuItem).appendTo(this.selectorChecker(config.storeMenuId));
                $(menuItem).show("fast");

                var link = $(menuItem).find('a');
                if (link) {
                    this.addMenuClick(link, marker);
                } else {
                    link = $(menuItem);
                    this.addMenuClick(link, marker);
                }

            }

        },

        displayHeader: function (searchLength) {

            var config = getConfig();

            if (config.headerContainerClass == locatorConstants.EMPTY_VALUE) return;

            $(config.headerContainerClass).html(null);

            var textObject = {
                OrderCount: searchLength,
                Text: config.messages.MESSAGE_OUTPUT_SEARCH_FOUND,
                OtherText: ''
            }

            $(this.parseTemplate(config.headerTextTemplate, textObject)).appendTo(config.headerContainerClass);

        },

        highlightMenu: function (menu) {

            if (!menu) return;

            var config = getConfig();
            $(menu).addClass(config.menuHighlightClass);

        },

        addMenuClick: function (menu, marker) {

            if (!menu) return;

            $(menu).click(function () {
                if (marker) { google.maps.event.trigger(marker, 'click'); }
            });
        },

        showInfoWindow: function (content, currentMarker) {
            infoWindow.setContent(content);
            infoWindow.open(map, currentMarker);
        },

        clearStores: function () {

            var config = getConfig();
            for (var i = 0; i < config.availableStores.length; i++) {
                var curMarker = config.availableStores[i];
                curMarker.setMap(null);
            }
            config.availableStores = [];
        },

        hideStores: function () {
            var config = getConfig();
            for (var i = 0; i < config.availableStores.length; i++) {
                var curMarker = config.availableStores[i];
                curMarker.setMap(null);
            }
        },

        clearStoreMenu: function () {
            var config = getConfig();
            $(this.selectorChecker(config.storeMenuId)).html(null);
        },

        clear: function () {

            var config = getConfig();

            this.clearStores();

            $(config.headerContainerClass).html(null);

            this.setDefaultHeader();

            this.clearStoreMenu();

            selectedStoreId = null;

            this.hideMapDirection();

            this.hideDirectionContainer();
        },

        setDefaultHeader: function () {
            this.setHeaderMessage(config.messages.MESSAGE_DEFAULT_SEARCH_HEADER_TEXT);
        },

        setHeaderMessage: function (text) {
            var config = getConfig();
            var textObject = { Text: text }

            $(config.headerContainerClass).html(null);

            $(this.parseTemplate(config.headerTextTemplate, textObject)).appendTo(config.headerContainerClass);
        },

        clearFoundAddress: function () {
            if (!foundAddress) return;
            foundAddress.setMap(null);
            foundAddress = null;
        },

        getDistanceByInput: function () {

            var distance = 0;
            var config = getConfig();
            var id = this.selectorChecker(config.distanceInputId);

            if ($(id)) { distance = $(id).val(); }

            return distance;
        },

        getSelectedStoreType: function () {
            var config = getConfig();

            if (config.storeTypeDropdownId != locatorConstants.EMPTY_VALUE) {
                var storeTypeDropdown = $(this.selectorChecker(config.storeTypeDropdownId));
                if (storeTypeDropdown) {
                    return $(storeTypeDropdown).val();
                }
            }

        },

        attachEvents: function () {

            var config = getConfig();

            //click button search
            if (config.searchInputButtonId != locatorConstants.EMPTY_VALUE) {
                var searchButton = $(this.selectorChecker(config.searchInputButtonId));
                if (searchButton) {
                    $(searchButton).click(this.searchNow);
                }
            }

            //when pressing enter key in textbox execute search
            if (config.searchInputId != locatorConstants.EMPTY_VALUE) {
                var txtSearch = $(this.selectorChecker(config.searchInputId));
                if (txtSearch) {
                    $(txtSearch).keypress(function (e) {
                        if (e.which == '13') {
                            e.preventDefault();
                            thisObject.searchNow();
                        }
                    });
                }
            }

            //when distance change execute search
            if (config.distanceInputId != locatorConstants) {
                var distanceDropdownId = $(this.selectorChecker(config.distanceInputId));
                if (distanceDropdownId) {
                    $(distanceDropdownId).change(this.searchNow);
                }
            }

            if (config.storeTypeDropdownId != locatorConstants.EMPTY_VALUE) {
                var storeTypeDropdown = $(this.selectorChecker(config.storeTypeDropdownId));
                if (storeTypeDropdown) {
                    $(storeTypeDropdown).change(this.searchNow);
                }
            }

            if (config.getDirectionButtonId != locatorConstants.EMPTY_VALUE) {
                var buttonDirection = $(this.selectorChecker(config.getDirectionButtonId));
                if (buttonDirection) {
                    buttonDirection.click(function () {
                        thisObject.getDirection();
                    });
                }
            }

            if (config.directionInputId != locatorConstants.EMPTY_VALUE) {
                var inputDirection = $(this.selectorChecker(config.directionInputId));
                if (inputDirection) {
                    $(inputDirection).keyup(function (e) {
                        if (e.which == '13') {
                            e.preventDefault();
                            thisObject.getDirection();
                        }
                    });
                }
            }

            if (config.wideScreenButtonId != locatorConstants.EMPTY_VALUE) {
                var wideScreenButton = $(this.selectorChecker(config.wideScreenButtonId));
                if (wideScreenButton) {
                    $(wideScreenButton).click(function (e) {

                        thisObject.hideWideScreenButton();
                        thisObject.showInModal();
                        thisObject.resizeMap();

                    });
                }
            }

            if (config.expandCollapseButtonId != locatorConstants.EMPTY_VALUE) {
                var collapseButton = $(this.selectorChecker(config.expandCollapseButtonId));
                if (collapseButton) {
                    $(collapseButton).click(function (e) {
                        thisObject.doMapExpandCollapse(this);
                    });
                }
            }

            //auto resize dialog when page is resized or scrolled.
            if (config.mainLocatorContainerClass != locatorConstants.EMPTY_VALUE) {
                $(window).resize(function () {
                    thisObject.centerDialog();
                });

                $(window).scroll(function () {
                    thisObject.centerDialog();
                });
            }

            //            var currentInfowindow = getCurrentInfoWindow();
            //            if (currentInfowindow != null && currentInfowindow != undefined) {
            //                google.maps.event.addListener(currentInfowindow, 'closeclick', function () {
            //                    //thisObject.hideDirectionContainer();
            //                });
            //            }

        },

        doMapExpandCollapse: function (collapseButton) {

            var config = getConfig();

            var mapContainerSelector = this.selectorChecker(config.mapContainerClass);
            var searchContainerSelector = this.selectorChecker(config.searchContainerClass);

            if ($(searchContainerSelector).is(':hidden')) {
                $(searchContainerSelector).show('slow', function () {
                    //resize the map after the container is fully displayed
                    $(mapContainerSelector).height($(mapContainerSelector).height() - $(searchContainerSelector).height());
                });

                $(collapseButton).attr('title', config.messages.MESSAGE_DEFAULT_COLLAPSE_TITLE);
                $(collapseButton).removeClass('locator-expand-icon')
                $(collapseButton).addClass('locator-collapse-icon');
            }
            else {
                $(mapContainerSelector).height($(mapContainerSelector).height() + $(searchContainerSelector).height());
                $(searchContainerSelector).hide('slow');
                $(collapseButton).attr('title', config.messages.MESSAGE_DEFAULT_EXPAND_TITLE);
                $(collapseButton).removeClass('locator-collapse-icon')
                $(collapseButton).addClass('locator-expand-icon');
            }

            this.resizeMap();

        },

        resizeMap: function () {
            google.maps.event.trigger(map, "resize");
            map.setCenter();
        },

        centerDialog: function () {
            var config = getConfig();
            $(config.mainLocatorContainerClass).dialog("option", "position", "center");
        },

        hideWideScreenButton: function () {
            var config = getConfig();
            $(this.selectorChecker(config.wideScreenButtonId)).hide();
        },

        showInModal: function () {

            var config = getConfig();

            var curentDocWidth = ($(document).width() - $(document).width() * .2);
            var curentDocHeight = ($(document).height() - $(document).height() * .4);

            $(config.mainLocatorContainerClass).dialog({
                autoOpen: true,
                modal: true,
                position: "center",
                resizable: false,
                zIndex: 99999,
                width: curentDocWidth,
                height: curentDocHeight,
                resizable: false,
                beforeClose: function () {
                    location.reload();
                }
            });

        },

        getMarkerBySelectedStoreId: function () {

            var config = getConfig();

            if (!selectedStoreId || selectedStoreId == null) return;

            var markers = jQuery.grep(config.availableStores, function (marker, i) {
                return (marker.Store.Id == selectedStoreId);
            });

            return markers[0];
        },

        getDirection: function () {

            var config = getConfig();

            var dirInput = $(this.selectorChecker(config.directionInputId));
            if (!dirInput) return;

            var address = $(dirInput).val().trim();

            if (address == locatorConstants.EMPTY_VALUE) {
                alert(config.messages.MESSAGE_VALIDATION_SEARCH_INPUT);
                $(dirInput).focus();
                return;
            }

            var geocoder = new google.maps.Geocoder();
            geocoder.geocode({ address: address }, this.searchAddressForDirectionCallBack);

        },

        searchAddressForDirectionCallBack: function (results, status) {

            var config = getConfig();

            if (status != google.maps.GeocoderStatus.OK) return;

            var marker = thisObject.getMarkerBySelectedStoreId();

            thisObject.hideMapDirection();

            var request = {
                origin: marker.getPosition(),
                destination: new google.maps.LatLng(results[0].geometry.location.lat(), results[0].geometry.location.lng()),
                travelMode: google.maps.DirectionsTravelMode.DRIVING
            };

            $(thisObject.selectorChecker(config.storeMenuId)).html(null);

            thisObject.hideStores();

            directionsService.route(request, function (response, status) {

                if (status != google.maps.DirectionsStatus.OK) { return; }

                thisObject.setHeaderMessage(config.messages.MESSAGE_DEFAULT_DIRECTION_HEADER_TEXT);
                directionsDisplay.setPanel($(thisObject.selectorChecker(config.storeMenuId))[0]);
                directionsDisplay.setMap(map);
                directionsDisplay.setDirections(response);
                thisObject.autoZoom();

            });

            //The the direction input container
            thisObject.hideDirectionContainer();

        },

        hideMapDirection: function () {
            directionsDisplay.setMap(null);
            directionsDisplay.setPanel(null);
        },

        hideInfoWindow: function () {
            infoWindow.close();
        },

        getStoresArray: function () {

            var config = getConfig();
            var stores = [];

            for (var i = 0; i < config.availableStores.length; i++) {
                stores.push(config.availableStores[i].Store);
            }
            return stores;
        },

        config: function (args) {
            setConfig($.extend(defaults, args));
            return (getConfig());
        }

    };

    //jqueryBasePlugin is located inside core.js
    $.extend($.fn.StoreLocator, new jqueryBasePlugin());

    function setConfig(value) {
        config = value;
    }

    function getConfig() {
        return config;
    }

    function getCurrentInfoWindow() {
        return infoWindow;
    }

})(jQuery);