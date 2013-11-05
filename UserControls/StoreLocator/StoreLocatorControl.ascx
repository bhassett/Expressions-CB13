<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StoreLocatorControl.ascx.cs"
    Inherits="UserControls_StoreLocator_StoreLocatorControl" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=<%= AuthenticationTokenKey %>&sensor=<%= Sensor.ToStringLower() %>&v=<%= Version %>"></script>
<script type="text/javascript" src="jscripts/store_locator/store-locator-plugin-template.js"></script>
<script type="text/javascript" src="jscripts/store_locator/store-locator-plugin.js"></script>
<asp:Panel ID="pnlMain" runat="server" CssClass="locator-main-container" Style="position: relative;">
    <div class="map-header-text">
        <h1>
            <%= AppLogic.GetString("menu.StoreLocator", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
        </h1>
    </div>
    <div class="locator-search-main">
        <div class="locator-search-header">
            <div class="locator-search-header-text">
                <h1>
                    <%= AppLogic.GetString("storelocator.aspx.3", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%></h1>
            </div>
            <div class="locator-icon">
                <a href="javascript:void(0);" id="lnkViewWide">
                    <span class="locator-wide-screen-icon"></span>
                </a>
            </div>
            <div class="locator-icon">
                <a href="javascript:void(0);" id="lnkExpanCollapse" class="locator-collapse-icon">
                </a>
            </div>
        </div>
        <div class="clr">
        </div>
        <div class="locator-search-detail">
            <table>
                <tr>
                    <td class="form-label">
                        <%= AppLogic.GetString("storelocator.aspx.4", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                    </td>
                    <td class="form-input">
                        <asp:DropDownList runat="server" ID="dlStoreType" class="storeTypeClass">
                        </asp:DropDownList>
                    </td>
                    <td class="form-label">
                        <%= AppLogic.GetString("storelocator.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                        <strong>(
                            <%= AppLogic.GetString("storelocator.aspx.21", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                            )</strong>
                    </td>
                    <td class="form-input">
                        <select id="search-miles">
                            <option value="3">3</option>
                            <option value="5">5</option>
                            <option value="10">10</option>
                            <option value="30">30</option>
                            <option value="50">50</option>
                            <option value="100">100</option>
                            <option value="200">200</option>
                            <option value="300">300</option>
                            <option value="500">500</option>
                            <option value="1000">1000</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td class="form-label">
                        <%= AppLogic.GetString("storelocator.aspx.10", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>
                    </td>
                    <td class="form-input" colspan="3">
                        <input type="text" id="txtInputAddress" class="locator-search-input" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="form-input" colspan="3">
                        <span class="locator-search-note">
                            <%= AppLogic.GetString("storelocator.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>
                        </span>
                    </td>
                </tr>
                <tr id="direction-input" class='direction-input'>
                    <td class="form-label">
                        <%= AppLogic.GetString("storelocator.aspx.24", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>
                    </td>
                    <td class="form-input">
                        <input type="text" id="txtAddressDirection" class="locator-search-input" />
                    </td>
                    <td>
                        <span class="locator-search-note">
                            <%= AppLogic.GetString("storelocator.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>
                        </span>
                        <%--<input type="button" class="site-button" id="btnGetDirection" value="<%= AppLogic.GetString("storelocator.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>" />--%>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="form-input locator-search-input-adjust-spacetop">
                        <input type="button" class="site-button content" id="btnSearchLocator" 
                        data-contentKey="storelocator.aspx.9"
                        data-contentValue="<%= AppLogic.GetString("storelocator.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>"
                        data-contentType="string resource"
                        value="<%= AppLogic.GetString("storelocator.aspx.9", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="map-wrapper">
        <div id="map">
        </div>
        <div id="direction-input" class='direction-input'>
            <div class="direction-input-wrapper">
                <span class="locator-search-note">
                    <%= AppLogic.GetString("storelocator.aspx.11", ThisCustomer.SkinID, ThisCustomer.LocaleSetting)%>
                </span>
                <br /><br />
                <input type="text" id="txtAddressDirection" class="locator-search-input"  />
                <br /><br />
                <input type="button" class="site-button" id="btnGetDirection" value="<%= AppLogic.GetString("storelocator.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>" />

            </div>
        </div>
        <div id="dirPanel">
        </div>
    </div>
    <div class="selectors-wrapper">
        <div class="header-selectors">
        </div>
        <div class="selectors-body">
            <ul id="store-menu">
            </ul>
        </div>
    </div>
    <h3>
        <asp:Label ID="lblError" runat="server" Style="color: red">
        </asp:Label>
    </h3>
</asp:Panel>
<script type="text/javascript">

    var options = {
        searchInputButtonId: 'btnSearchLocator',
        searchInputId: 'txtInputAddress',
        distanceInputId: 'search-miles',
        storeTypeInputClass: '.storeTypeClass',
        storeMenuId: 'store-menu',
        storeMenuTemplate: 'storeMenuTemplate',
        headerContainerClass: '.header-selectors',
        headerTextTemplate: 'headerTextTemplate',
        infoWindowTemplate: 'infoWindowTemplate',
        addressInfoWindowTemplate: 'addressInfoWindowTemplate',
        directionInputContainerId: 'direction-input',
        directionInputId: 'txtAddressDirection',
        directionlinkClass: 'store-infowindow-direction-link',
        getDirectionButtonId: 'btnGetDirection',
        storeTypeDropdownId: '<%= dlStoreType.ClientID %>',
        storesIcon: 'skins/Skin_<%= ThisCustomer.SkinID %>/images/storelocator/storeicon.png',
        addressIcon: 'skins/Skin_<%= ThisCustomer.SkinID %>/images/storelocator/addressIcon.png',
        wideScreenButtonId: 'lnkViewWide',
        expandCollapseButtonId: 'lnkExpanCollapse',
        mainLocatorContainerClass: '.locator-main-container',
        searchContainerClass: '.locator-search-detail',
        mapContainerClass: '.map-wrapper',
        messages: {
            MESSAGE_VALIDATION_SEARCH_INPUT: '<%= AppLogic.GetString("storelocator.aspx.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>',
            MESSAGE_VALIDATION_SEARCH_NOT_FOUND: '<%= AppLogic.GetString("storelocator.aspx.13", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>',
            MESSAGE_OUTPUT_SEARCH_FOUND: '<%= AppLogic.GetString("storelocator.aspx.14", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>',
            MESSAGE_DEFAULT_SEARCH_HEADER_TEXT: '<%= AppLogic.GetString("storelocator.aspx.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>',
            MESSAGE_DEFAULT_DIRECTION_HEADER_TEXT: '<%= AppLogic.GetString("storelocator.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>',
            MESSAGE_DEFAULT_DIRECTION_LINK_TEXT: '<%= AppLogic.GetString("storelocator.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>',
            MESSAGE_DEFAULT_WIDESCREEN_BUTTON_TITLE: '<%= AppLogic.GetString("storelocator.aspx.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>',
            MESSAGE_DEFAULT_COLLAPSE_TITLE: '<%= AppLogic.GetString("storelocator.aspx.22", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>',
            MESSAGE_DEFAULT_EXPAND_TITLE: '<%= AppLogic.GetString("storelocator.aspx.23", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true)%>'
        }
    }

    function InitializeMap() { $("#map").StoreLocator.initialize(options); }

    google.maps.event.addDomListener(window, 'load', InitializeMap);

</script>
