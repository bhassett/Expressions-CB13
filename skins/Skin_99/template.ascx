<!DOCTYPE html>
<%@ Control Language="c#" AutoEventWireup="false" Inherits="InterpriseSuiteEcommerce.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<!--[if lt IE 7]><html class="lt-ie9 lt-ie8 lt-ie7" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if IE 7]><html class="lt-ie9 lt-ie8" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if IE 8]><html class="lt-ie9" xmlns="http://www.w3.org/1999/xhtml"><![endif]-->
<!--[if gt IE 8]><!-->
<html xmlns="http://www.w3.org/1999/xhtml"><!--<![endif]-->
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <meta name="viewport" content="width=device-width, minimum-scale=1.0, maximum-scale=1.0" />
        <title>(!METATITLE!)</title>
        <meta name="description" content="(!METADESCRIPTION!)" />
        <meta name="keywords" content="(!METAKEYWORDS!)" />

        <link rel="stylesheet" href="skins/Skin_(!SKINID!)/bootstrap/css/bootstrap.min.css" media="screen" />
        <link rel="stylesheet" href="skins/Skin_(!SKINID!)/font-awesome/css/font-awesome.min.css" type="text/css" />
        <link rel="stylesheet" href="skins/Skin_(!SKINID!)/ui-lightness/jquery-ui-1.8.16.custom.css" type="text/css" />
        <link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css" />
        <link rel="stylesheet" href="skins/Skin_(!SKINID!)/custom.css" type="text/css" />

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

        <!-- Google Analytics -->
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

    <!-- HEADER -->
    <div id="header" class="row">
        <div class="large-5 columns">
          <a href="default.aspx" class="logo"><img src="skins/Skin_(!SKINID!)/images/celcom-logo.png" alt="CelCom Inc"></a>
        </div>
        <div class="large-7 columns">
            <div id="accountMenu">
                <span>(!USERNAME!)</span>
                <span><a href="(!SIGNINOUT_LINK!)"><i class="icon-key"></i>(!SIGNINOUT_TEXT!)</a></span>
                <%--<span><a href="account.aspx"><i class="icon-user"></i>My Account</a></span>--%>
                <span>
                    <a id="shopping-cart" href="javascript:void(0)">
                        <i class="icon-shopping-cart"></i>(!CARTPROMPT!)((!NUM_CART_ITEMS!))
                    </a>
                </span>
                <!-- MiniCart -->
                <div id="mini-cart"></div>
            </div>
            <div id="storeMenu">
                <span id="storeVAT">VAT Mode: (!VATSELECTLIST!)</span>
                <span id="storeLanguage">Language: (!COUNTRYSELECTLIST!)</span>
            </div>
        </div>
    </div>
    <!-- END OF HEADER -->
    
    <!-- NAV -->
    <div id="navbar" class="row">
        <div class="navbar-inner">
            <div class="large-8 columns"><div id="menu_container" runat="server"></div></div>
            <div class="large-4 columns text-right">(!XmlPackage Name="rev.search"!)</div>
        </div>
    </div>
    <!-- END OF NAV -->

    <!-- SUBNAV -->
    <div id="subNav" class="row">
        <div class="large-10 columns">
            <ul>
                <li><a href="bestsellers.aspx"><i class="icon-tags"></i> Bestsellers</a></li>
                <li><a href="recentadditions.aspx"><i class="icon-plus-sign"></i> Recent Addition</a></li>
                <li><a href="giftregistry.aspx"><i class="icon-gift"></i> Gift Registry</a></li>
                <li><a href="storelocator.aspx"><i class="icon-map-marker"></i> Store Locator</a></li>
                <li><a href="wishlist.aspx"><i class="icon-star"></i> Wishlist</a></li>
            </ul>
        </div>
        <div class="large-2 columns text-right">
            <!-- LIVECHAT: Uncomment code below to enable  -->
            
           <%--<div class="livechat">
                <script type="text/javascript" src="http://localhost/LiveSupport/CuteSoft_Client/CuteChat/Support-Image-Button.js.aspx"></script>
                <script type="text/javascript" src="http://localhost/LiveSupport/CuteSoft_Client/CuteChat/Support-Visitor-monitor-crossdomain.js.aspx" 
                        id="LiveSupportVisitorMonitorScript"></script>
            </div>--%>
            <img src="skins/Skin_(!SKINID!)/images/online.gif" />
        </div>
    </div>
    <!-- END OF SUBNAV -->

    <!-- CONTENT -->
    <div class="row" id="content">
        <div class="large-3 columns">
            (!XmlPackage Name="rev.attributes" IsForAttributes="true"!)
            
            <div class="sidebar box">
                <h4>Browse by Categories</h4>
                (!XmlPackage Name="rev.categories"!)
            </div>
            <div class="sidebar box">
                <h4>Browse by Manufacturers</h4>
                (!XmlPackage Name="rev.manufacturers"!)
            </div>
            <div class="sidebar box">
                <h4>Browse by Departments</h4>
                (!XmlPackage Name="rev.departments"!)
            </div>
            (!POLL!)
            (!SOCIALMEDIA_FEEDBOX!)
        </div>
        <div class="large-9 columns">
            <!-- MAIN CONTENT -->
            <div class="main-content box">
                <div class="page-breadcrumb">Now In: (!SECTION_TITLE!)</div>
                <asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
                <div class="clear-both"></div>
            </div>
            <!-- END OF MAIN CONTENT -->
        </div>
        
    </div>
    <!-- END OF CONTENT -->

    <!-- FOOTER -->
    <div id="footer" class="row">
        <div class="banner"> 
            <div class="large-4 columns">
                <a href="t-shipping.aspx" class="btn btn-block btn-large">
                    <i class="icon-plane"></i> Shipping Policy <br />
                    <span class="tiny">Free Delivery on Certain Areas</span>
                </a> 
            </div>    
            <div class="large-4 columns">
                <a href="news.aspx" class="btn btn-block btn-large">
                    <i class="icon-rss-sign"></i> Latest News<br />
                    <span class="tiny">Get a regular feed of news and events</span>
                </a>
            </div>    
            <div class="large-4 columns">
                <a href="casehistory.aspx" class="btn btn-block btn-large">
                    <i class="icon-phone"></i> Our Support <br />
                    <span class="tiny">Call us: 1800-000-555</span>
                </a>
            </div>    
        </div>
        <hr />
        <div class="foot-links">
            <div class="large-3 columns">
                <h3>Navigation</h3>
                <ul>
                    <li><a href="default.aspx">Home</a></li>
                    <li><a href="contactus.aspx">Contact Us</a></li>
                    <li><a href="casehistory.aspx">Customer Support</a></li>
                    <li><a href="sitemap2.aspx">Sitemap</a></li>
                </ul>
            </div>
            <div class="large-3 columns">
                <h3>Information</h3>
                <ul>
                    <li><a href="t-faq.aspx">FAQs</a></li>
                    <li><a href="t-returns.aspx">Return Policy</a></li>
                    <li><a href="t-privacy.aspx">Privacy Policy</a></li>
                    <li><a href="t-security.aspx">Security Policy</a></li>
                    <li><a href="t-shipping.aspx">Shipping Info</a></li>
                </ul>
            </div>
            <div class="large-3 columns">
                <h3>Services</h3>
                <ul>
                    <li><a href="storelocator.aspx">Store Locator</a></li>
                    <li><a href="giftregistry.aspx">Gift Registry</a></li>
                    <li><a href="wishlist.aspx">Wishlist</a></li>
                    <li><a href="leadform.aspx">Lead Form</a></li>
                </ul>
            </div>
            <div class="large-3 columns">
                <h3>Follow Us</h3>
                <div class="social-media">
                    (!Topic Name="SocialMediaSubscribeBox"!)
                </div>
            </div>
        </div>

        
    </div>
    <div id="footerExt" class="row">
            <div class="large-6 columns">
                <span>Copyright © 2013. All Rights Reserved.</span><br />
                <span>Powered by <a href="http://connectedbusiness.com/" target="_blank" title="e-Commerce Shopping Cart">Connected Business eCommerce Shopping Cart</a></span>
            </div>
            <div class="large-6 columns text-right">

                (!MOBILE_FULLMODE_SWITCHER!)

                <!-- CUSTOMER SUPPORT -->
                <div class="customer-support">
                    <div id="request-container">
                        <span>(!stringresource name="main.content.1"!)</span>
                        <span class="request-generator-content request-code" >---------------</span>
                        <span title="Refresh"><a href="javascript:void(0)" class="generate-link"><i class="icon-refresh"></i></a></span>
                        <span title="Copy to Clipboard"><a href="javascript:void(0)" class="copy-link"><i class="icon-copy"></i></a></span>
                    </div>
                </div>
                <!-- END OF CUSTOMER SUPPORT -->
            </div>
    </div>
    <!-- END OF FOOTER -->

    <!-- BUBBLE MESSAGE -->
    <div id="ise-message-tips"><span id="ise-message" class="custom-font-style"></span><span id="message-pointer"></span></div>

     <!-- ADDRESS VERIFICATION -->
    (!ADDRESS_VERIFICATION_DIALOG_LISTING!)
    (!ADDRESS_VERIFICATION_DIALOG_OPTIONS!)

     <!-- GOOGLE ANALYTICS -->
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

    <script type="text/javascript">
        $(document).ready(function () {
            setupControls();
        });

        function setupControls() {
            //buttons
            $('.site-button').addClass('btn btn-info');
            $('#btnCheckOutNowTop').removeClass('btn-info');
            $('#btnCheckOutNowTop').addClass('btn-success');
            $('#btnCheckOutNowBottom').removeClass('btn-info');
            $('#btnCheckOutNowBottom').addClass ('btn-success');
            $('#btnUpateWishList1').removeClass('btn-info');
            $('#btnUpdateWishList1').addClass('btn-success');
            $('#btnUpateWishList2').removeClass('btn-info');
            $('#btnUpateWishList2').addClass('btn-success');
            $('input.addto').addClass('btn btn-info');

            //store language
            var showLanguage = false;
            var languageVisibility = '(!COUNTRYDIVVISIBILITY!)';
            var languageContainer = $('#storeLanguage');
            if (languageVisibility == 'visible') { showLanguage = true; }
            if (showLanguage) { languageContainer.show(); }

            //store vat
            var showVat = false;
            var vatVisibility = '(!VATDIVVISIBILITY!)';
            var vatContainer = $('#storeVAT');
            if (vatVisibility == 'visible') { showVat = true; }
            if (showVat) { vatContainer.show(); }
        }
    </script>
    <!-- BUY SAFE SEAL -->
    (!BUY_SAFE_SEAL!)
</body>
</html>
