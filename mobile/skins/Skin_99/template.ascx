<%@ Control Language="C#" ClassName="template" Inherits="InterpriseSuiteEcommerce.TemplateBase" %>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="description" content="(!METADESCRIPTION!)" />
    <meta name="keywords" content="(!METAKEYWORDS!)" />
    <meta name="viewport" content="width = device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;" />
    <title>(!METATITLE!)</title>
    <link rel="Stylesheet" href="skins/Skin_(!SKINID!)/portrait.css" type="text/css" />
    <link rel="stylesheet" href="skins/Skin_(!SKINID!)/custom.css" type="text/css" />
    <link href='http://fonts.googleapis.com/css?family=Lato:400,300italic,700,400italic,300' rel='stylesheet' type='text/css' />
    <script type="text/javascript" src="js/core.js"></script>
    <script type="text/javascript" src="js/jquery/jquery.min.v1.7.2.js"></script>
    <script type="text/javascript" src="js/jquery/jquery.loader.js"></script>
    <script type="text/javascript" src="js/jquery/jquery.tmpl.min.js"></script>
    <script type="text/javascript">
        var pageTracking = '(!GA_PAGE_TRACKING!)'; // assign token GA_PAGE_TRACKING (see parser.cs or is->app config: GoogleAnalytics.PageTracking)
        var ecTracking = '(!GA_ECOMMERCE_TRACKING!)'; // assign token GA_ECOMMERCE_TRACKING (see parser.cs or is->app config: GoogleAnalytics.ConversionTracking)
        var gaAccount = '(!GA_ACCOUNT!)'; // assign token GA_ACCOUNT (see parser.cs or is->app config: GoogleAnalytics.TrackingCode)

        var orderNumber = getQueryString()["ordernumber"]; // get ordernumber query string
        var imAtConfirmOrderPage = 'true';

        if (typeof orderNumber != "undefined" && ecTracking == 'true') { // verify if ordernumber query string is defined and ecTracking app config is  true

            imAtConfirmOrderPage = 'true';

        }

        var _gaq = _gaq || [];

        if (pageTracking == 'true' || imAtConfirmOrderPage == 'true') { // verify if pageTracking app config is true OR current page is orderconfirmation.aspx

            _gaq.push(['_setAccount', gaAccount]); // set google analytics account (see app config GoogleAnalytics.TrackingCode
            _gaq.push(['_trackPageview']); // request page tracking

        }
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
    <div id="main">
      <div class="logowrapper" style="background: #FFF;" >
        <div class="shoppingCart"> <a href="default.aspx">
          <div class="logo"> <img src="skins/Skin_(!SKINID!)/images/celcom-logo.png" alt="CellCom" style="width: 150px;"> </div>
          </a> <b><a href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a></b><br />
          <b><a href="shoppingcart.aspx">(!stringresource name="AppConfig.MobileCartPrompt"!)</a></b>: <span class="itemnum">(!NUM_CART_ITEMS!)</span> item(s) </div>
        <div style="margin: 10px; float: left; width: 300px;" > (!XmlPackage Name="mobile.skin.search"!)</div>
        <div class="loginwelcome"> <span>(!USER_WELCOMETEXT!)</span> <a class="username" href="account.aspx">(!USERFULLNAME!)</a> </div>
      </div>
      <div class="headerLinkscontainer borderTop">
       <a href="bestsellers.aspx">(!stringresource name="mobile.default.aspx.2"!)</a> &bull; <a href="default.aspx">(!stringresource name="mobile.default.aspx.1"!)</a> &bull; <a href="account.aspx">(!stringresource name="mobile.default.aspx.3"!)</a><br />
      <a href="manufacturers.aspx">(!stringresource name="mobile.default.aspx.4"!)</a> &bull; <a href="category.aspx">(!stringresource name="mobile.default.aspx.5"!)</a> &bull; <a href="departments.aspx">(!stringresource name="mobile.default.aspx.6"!)</a>
      </div>
      <!-- CONTENTS START -->
      <div id="main_content" style="background: #FFF;">
        <asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
      </div>
      <!-- CONTENTS END --> 
      <script type="text/javascript" >
            //load handler is inside core.js
            $(document).ready(function () {
                //alert(navigator.userAgent);
                //loadHandlers();
            })
        </script>
      <div class="footer" style="background: #FFF;"> 
        <a href="t-contact.aspx">
            <span class="product_description">Contact Us</span>
        </a> | 
        <a href="t-returns.aspx">Return Policy</a> | 
        <a href="sitemap2.aspx">Site Map</a> <br />

        <p>
            (!MOBILE_FULLMODE_SWITCHER!)
        </p>

        <a href="t-copyright.aspx">Copyright &copy; 2011. All Rights Reserved.</a> <br />
        <br />
        Powered by <a href="http://connectedbusiness.com/" target="_blank"
                title="e-Commerce Shopping Cart">Connected Business eCommerce Shopping Cart</a> <br />
        <br />
      </div>
      <noscript>
      Powered by <a href="http://connectedbusiness.com/" target="_blank"> Connected Business eCommerce Shopping Cart</a>
      </noscript>
    </div>
</body>
</html>
