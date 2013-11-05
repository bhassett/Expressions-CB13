<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Control Language="c#" AutoEventWireup="false" Inherits="InterpriseSuiteEcommerce.TemplateBase"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>(!METATITLE!)</title>
    <meta name="description" content="(!METADESCRIPTION!)" />
    <meta name="keywords" content="(!METAKEYWORDS!)" />
    <link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css" />
    <link rel="stylesheet" href="skins/Skin_(!SKINID!)/custom.css" type="text/css" />
    <link rel="Stylesheet" href="skins/Skin_(!SKINID!)/ui-lightness/jquery-ui-1.8.16.custom.css"
        type="text/css" />
    <script type="text/javascript" src="jscripts/core.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery.min.v1.7.2.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery-ui-1.8.16.custom.min.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery.tmpl.min.js"></script>
    <script type="text/javascript" src="jscripts/jquery/menu.js"></script>
    <script type="text/javascript" src="jscripts/attribute.selectors.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery.format.1.05.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery.cbe.address.dialog.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery.cbe.bubble.message.js"></script>
    <script type="text/javascript" src="jscripts/jquery/jquery.loader.js"></script>

    <script type="text/javascript">
        var _gaq = _gaq || [];
        $(window).load(function () {
            var pageTracking = ise.Configuration.getConfigValue("GoogleAnalytics.PageTracking");
            var ecTracking = ise.Configuration.getConfigValue("GoogleAnalytics.ConversionTracking");
            var gaAccount = ise.Configuration.getConfigValue("GoogleAnalytics.TrackingCode");

            var orderNumber = getQueryString()["ordernumber"]; // get ordernumber query string

            var imAtConfirmOrderPage = false;

            if (typeof orderNumber != "undefined" && ecTracking == 'true') { // verify if ordernumber query string is defined and ecTracking app config is  true

                imAtConfirmOrderPage = true;
            }

            if ((pageTracking == 'true' || imAtConfirmOrderPage) && gaAccount != "") { // verify if pageTracking app config is true OR current page is orderconfirmation.aspx

                //  uncomment to test your Google Analytics on localhost:
                // _gaq.push(['_setDomainName', 'none']); 

                _gaq.push(['_setAccount', gaAccount]); // set google analytics account (see app config GoogleAnalytics.TrackingCode
                _gaq.push(['_trackPageview']); // request page tracking

            }
        });
     </script>

    <!-- Global Loader -->
    <script type="text/javascript">
        $(document).ready(function () {
            $("body").globalLoader({
                autoHide: false,
                image: 'images/ajax-loader.gif',
                opacity: 0.3,
                text: 'loading...'
            });
            //sample implementation to display the loader
            //$("body").data("globalLoader").show();
        });
    </script>

    (!JAVASCRIPT_INCLUDES!)
</head>
<body>
    <asp:Panel ID="pnlForm" runat="server" Visible="false" />
    <!-- PAGE INVOCATION: '(!INVOCATION!)' -->
    <!-- PAGE REFERRER: '(!REFERRER!)' -->
    <!-- STORE LOCALE: '(!STORELOCALE!)' -->
    <!-- CUSTOMER LOCALE: '(!CUSTOMERLOCALE!)' -->
    <!-- STORE VERSION: '(!STORE_VERSION!)' -->
    <!-- CACHE MENUS: '(!AppConfig name="CacheMenus"!)' -->
    <!-- to center your pages, set text-align: center -->
    <!-- to left justify your pages, set text-align: center -->
    <!-- if using dynamic full width page sizes, the left-right align has no effect (obviously) -->
    <!-- Do not remove (for CMS Editing Menu) -->
    <div id="cms-user-panel">
        <div class="cms-user-panel-command">
            <input type="button" class="site-button" id="cms-user-panel-command-button" data-active="true"
                value="(!stringresource name='cms.browsemode'!)" onclick="ProcessCmsPanelButton(this);" />
        </div>
    </div>
    <!-- ------------------------------------ -->
    <div class="wrapper">
        <div class="wrapper2">
            <div class="topnavvy">
                <a href="default.aspx">
                    <img src="skins/Skin_(!SKINID!)/images/blank.gif" class="logo" /></a>
                <div class="topnavvybase">
                    (!USERNAME!)&nbsp;&nbsp;<a href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a
                        id="shopping-cart" class="headblue" href="javascript:void(1)">(!CARTPROMPT!)&nbsp;((!NUM_CART_ITEMS!))</a>
                    <div id="mini-cart">
                    </div>
                </div>
                <div class="topnavvybase">
                    <div style="visibility: (!COUNTRYDIVVISIBILITY!); display: (!COUNTRYDIVDISPLAY!);
                        float: right; clear: both; margin-left: 2px; height: 20px;">
                        Language: (!COUNTRYSELECTLIST!)
                    </div>
                    <div style="float: right; height: 20px; visibility: (!VATDIVVISIBILITY!); display: (!VATDIVDISPLAY!);">
                        VAT Mode: (!VATSELECTLIST!)&nbsp;</div>
                    <div style="visibility: (!CURRENCYDIVVISIBILITY!); display: (!CURRENCYDIVDISPLAY!);
                        float: right; height: 20px;">
                        Currency:(!CURRENCYSELECTLIST!)</div>
                </div>
                <div class="topnavvybase1">
                    <div id="menu_container" runat="server">
                    </div>
                </div>
            </div>
            <div class="lightgreytop">
                <div class="bread_area">
                    <span class="SectionTitleText">Now In: (!SECTION_TITLE!)</span></div>
            </div>
            <div class="clr"></div>
            <div class="lightgrey">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td width="33%">
                        </td>
                        <td width="33%">
                            <img src="skins/Skin_(!SKINID!)/images/main_top_light.png" width="1004" height="22" />
                        </td>
                        <td width="33%">
                        </td>
                    </tr>
                    <tr>
                        <td width="33%">
                        </td>
                        <td width="33%" class="centerer">
                            <div class="leftarea">
                                <div class="leftnavvy">
                                    <%--Uncomment code below to enable Live Chat--%>
                                    <%--<div style="float: right; width: 150px; height: 20px; margin: 0 -150px 0 0">
                                            <script type="text/javascript" src="http://localhost/LiveSupport/CuteSoft_Client/CuteChat/Support-Image-Button.js.aspx"></script>
                                            <script id="LiveSupportVisitorMonitorScript" src="http://localhost/LiveSupport/CuteSoft_Client/CuteChat/Support-Visitor-monitor-crossdomain.js.aspx"></script>
                                        </div>--%>
                                    <img src="skins/Skin_(!SKINID!)/images/online.gif" width="209" height="50" /></div>
                                <div class="leftnavvy">
                                    (!XmlPackage Name="skin.search"!)</div>
                                <div align="left" style="float: left;">
                                    (!XmlPackage Name="rev.attributes" IsForAttributes="true"!)</div>
                                <div class="leftnavvy">
                                    <img src="skins/Skin_(!SKINID!)/images/left_header_browse_manu.jpg" width="209" height="24"
                                        class="leftnavvy_header" />
                                    <div class="EntityMenuAlignment">
                                        (!XmlPackage Name="rev.manufacturers"!)</div>
                                </div>
                                <div class="leftnavvy">
                                    <img src="skins/Skin_(!SKINID!)/images/left_header_browse_cat.jpg" alt="" width="209"
                                        height="24" class="leftnavvy_header" />
                                    <div class="EntityMenuAlignment">
                                        (!XmlPackage Name="rev.categories"!)</div>
                                </div>
                                <div class="leftnavvy">
                                    <img src="skins/Skin_(!SKINID!)/images/left_header_browse_dept.jpg" width="209" height="24"
                                        class="leftnavvy_header" />
                                    <div class="EntityMenuAlignment">
                                        (!XmlPackage Name="rev.departments"!)</div>
                                </div>
                                <div align="left" style="float: left; width: 209px;">
                                    (!POLL!)</div>
                                <%--<div align="left" style="float: left; width: 209px;">
                                    (!MINICART!)</div>--%>
                                <div align="left" style="float: left; width: 212px; margin-left: 20px;">
                                    (!SOCIALMEDIA_FEEDBOX!)<br />
                                </div>
                            </div>
                            <div class="rightmain">
                                <!-- CONTENTS START -->
                                <asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
                                <!-- CONTENTS END -->
                                (!ADDRESS_VERIFICATION_DIALOG_LISTING!)
                               
                            </div>
                        </td>
                        <td width="33%">
                        </td>
                    </tr>
                    <tr>
                        <td width="33%">
                        </td>
                        <td width="33%">
                            <img src="skins/Skin_(!SKINID!)/images/main_bottom_light.png" width="1004" height="22" alt='' />
                        </td>
                        <td width="33%">
                        </td>
                    </tr>
                </table>
            </div>
            
            <div class="lightgreybot">
                <div class="bottomer">
                    <div class="footermid">
                        <a class="foot" href="t-copyright.aspx">Copyright &copy; 2013. All Rights Reserved.</a>
                        <div style="width: 100%; margin:auto; float:left;">
                            <ul id="footer-menu">
                                <li><a href="default.aspx" class="foot">Home</a></li>
                                <li><span>| </span><a href="contactus.aspx" class="foot">Contact Us</a></li>
                                <li><span>| </span><a href="t-about.aspx" class="foot">About Us</a></li>
                                <li><span>| </span><a href="t-returns.aspx" class="foot">Return Policy</a></li>
                                <li><span>| </span><a href="t-privacy.aspx" class="foot">Privacy Policy</a></li>
                                <li><span>| </span><a href="t-security.aspx" class="foot">Security Policy</a></li>
                                <li><span>| </span><a href="sitemap2.aspx" class="foot">Site Map</a></li>
                            </ul>
                        </div>
                        <br />
                        <small>Powered by <a href="http://connectedbusiness.com/" target="_blank"
                            title="e-Commerce Shopping Cart" class="foot">Connected Business eCommerce Shopping Cart</a></small><br />
                        <noscript>
                            Powered by <a href="http://connectedbusiness.com/" target="_blank">
                                Connected Business eCommerce Shopping Cart</a>
                        </noscript>

                        <div class="footer-mobile-link">
                            (!MOBILE_FULLMODE_SWITCHER!)
                        </div>

                        <div id="request-container">
                            <div class="request-caption-wrapper">
                                <span class="request-caption">(!stringresource name="main.content.1"!) </span>
                                <div class="request-code-wrapper">
                                    <div class="request-generator-content">
                                        <span class="request-code">---------------</span></div>
                                    <div style="margin-left: 10px;">
                                        <a href="javascript:void(0);" class="generate-link">
                                            <img src="skins/Skin_(!SKINID!)/images/refresh-captcha.png" alt="" title="" />
                                        </a>
                                    </div>
                                </div>
                                <div id="imgLoader" style="display: none">
                                    <img src="skins/Skin_(!SKINID!)/images/loading.gif" alt="" title="" />
                                </div>
                            </div>
                            <div style="margin-left: 10px; float: left;">
                                <a href="javascript:void(0);" class="copy-link">(!stringresource name="main.content.3"!)
                                </a>
                            </div>
                        </div>
                        <br />
                        <div class="footerright">
                            <div id="socialmedia-subscribebox">
                                (!Topic Name="SocialMediaSubscribeBox"!)</div>
                        </div>

                      
                    </div>
                </div>
            </div>

        </div>
    </div>

    <!-- Bubble Message Container -->
    <div id="ise-message-tips"><span id="ise-message" class="custom-font-style"></span><span id="message-pointer"></span></div>
    
    <!-- Address Verification -->
    (!ADDRESS_VERIFICATION_DIALOG_OPTIONS!)

     <!-- Google Analytics -->
    <script type="text/javascript">
    $(window).load(function () {

        var pageTracking = $.trim(ise.Configuration.getConfigValue("GoogleAnalytics.PageTracking"));
        var ecTracking = $.trim(ise.Configuration.getConfigValue("GoogleAnalytics.ConversionTracking"));
        var gaAccount = $.trim(ise.Configuration.getConfigValue("GoogleAnalytics.TrackingCode"));

        var orderNumber = getQueryString()["ordernumber"]; // get ordernumber query string
        var imAtConfirmOrderPage = false;

        if (typeof orderNumber != "undefined" && ecTracking == 'true') imAtConfirmOrderPage = true;

        if ((pageTracking == 'true' || imAtConfirmOrderPage) && gaAccount != "GoogleAnalytics.TrackingCode") {

            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);

        }

    });
    </script>
    <!-- Buy Safe Seal -->
    (!BUY_SAFE_SEAL!)

    <%-- do not touch <-- --%>
</body>
</html>
