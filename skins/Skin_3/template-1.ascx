﻿<!DOCTYPE html>
<%@ Control Language="c#" AutoEventWireup="false" Inherits="InterpriseSuiteEcommerce.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>

<!-- paulirish.com/2008/conditional-stylesheets-vs-css-hacks-answer-neither/ -->
<!--[if lt IE 7]> <html class="no-js lt-ie9 lt-ie8 lt-ie7" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="no-js lt-ie9 lt-ie8" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="no-js lt-ie9" lang="en"> <![endif]-->
<!--[if gt IE 8]><!--> <html class="no-js" lang="en"> <!--<![endif]-->

<head>
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width" />

<title>(!METATITLE!)</title>

<meta name="description" content="(!METADESCRIPTION!)">
<meta name="keywords" content="(!METAKEYWORDS!)">

<link rel="shortcut icon" type="image/x-icon" href="favicon.ico">

<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css">
<link rel="Stylesheet" href="skins/Skin_(!SKINID!)/ui-lightness/jquery-ui-1.8.16.custom.css" type="text/css" />

<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/foundation.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/app.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/icons-main.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/icons-general.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/icons-social.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/megamenu.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/megamenu_ie.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/slide.css">

<!--[if lt IE 9]>
    <link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/ie.css">
<![endif]-->



<script type="text/javascript" src="jscripts/core.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery-ui-1.8.16.custom.min.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery.min.v1.6.4.js"></script>
<script type="text/javascript" src="jscripts/attribute.selectors.js"></script>
<script type="text/javascript" src="jscripts/address.verification.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery.min.v1.6.4.js"></script>
<script src="javascripts/modernizr.foundation.js"></script>
<script src="javascripts/jquery.easing.js"></script>
<script src="javascripts/app.js"></script>
<!--<script src="javascripts/megamenu_plugins.js"></script>
<script src="javascripts/megamenu.js"></script>
<script src="javascripts/megamenuconf.js"></script>-->
<script src="javascripts/google_tc.js"></script>

<!-- IE Fix for HTML5 Tags -->
<!--[if lt IE 9]>
	<script src="javascripts/html5.js"></script>
<![endif]-->

<script type="text/javascript">
$(document).ready(function(){
  $(".manuslideclose").click(function(){
    $(".manuslide").slideUp();
  });
  $(".manuslideopen").click(function(){
    $(".manuslide").slideDown();
  });
});
</script>

(!JAVASCRIPT_INCLUDES!)

</head>



<body>
<asp:Panel ID="pnlForm" runat="server" Visible="false"  />
<!-- PAGE INVOCATION: '(!INVOCATION!)' -->
<!-- PAGE REFERRER: '(!REFERRER!)' -->
<!-- STORE LOCALE: '(!STORELOCALE!)' -->
<!-- CUSTOMER LOCALE: '(!CUSTOMERLOCALE!)' -->
<!-- STORE VERSION: '(!STORE_VERSION!)' -->
<!-- CACHE MENUS: '(!AppConfig name="CacheMenus"!)' -->
<!-- to center your pages, set text-align: center -->
<!-- to left justify your pages, set text-align: center -->
<!-- if using dynamic full width page sizes, the left-right align has no effect (obviously) -->

<div id="wrap">
<div id="main" class="clearfix">
<div id="menu_container" runat="server" visible="false"></div>

<!-- header [begin] -->
<!-- expHeader [begin] -->
  <div id="expHeader" class="container">
    <div class="row hide-on-phones">

      <div class="four columns">
        <a href="default.aspx"><img id="headerlogo" src="skins/Skin_(!SKINID!)/images/expressions-site-logo.png"></a>
      </div>

      <div class="seven columns">
      	<div id="headeruserinfo">(!USERNAME!)&#32;&#32;<a href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a>&#32;&#124;&#32;<a href="account.aspx">My&#32;Account</a></div>
            <div id="navSearch">(!XmlPackage Name="skin.search"!)<!--&#32;&#45;&#32;or&#32;&#45;&#32;--><br><a href="searchadv.aspx">Advanced Search <i class="main foundicon-search"></i></a></div>
      </div>
	
	<div class="one column">
	<div class="minibagmenu">
		<a href="shoppingcart.aspx" id="togminibag"><img src="skins/Skin_(!SKINID!)/images/bag_icon.png" class="bagicon" alt="Shopping Bag"></a>
		  <div class="bagiconnum">(!NUM_CART_ITEMS!)</div>
		  <!--<div id="minibag">(!MINICART_PLAIN!)</div>-->
		</div>
	</div>

    </div>
    
    <div class="row show-on-phones">

      <div class="twelve columns text-center">
        <a href="../"><img id="headerlogo" src="skins/Skin_(!SKINID!)/images/expressions-site-logo.png"></a>
      </div>

    </div>
    
    <div class="row show-on-phones">
    	<div class="twelve columns text-center"><div id="headeruserinfo">(!USERNAME!)&#32;&#32;<a href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a>&#32;&#124;&#32;<a href="account.aspx">My&#32;Account</a></div></div>
    </div>
    
  </div>
 <!-- expHeader [end] -->


<!-- navbar [begin] -->
<div id="topNav" class="container">
<div class="row">
  <div class="ten columns hide-on-phones">
  
<div class="megamenu_container"><!-- Begin Mega Menu Container -->
    <ul class="megamenu"><!-- Begin Mega Menu -->
    	<li><span class="nodrop"><a href="default.aspx">Home</a></span></li>
        <li><span class="drop"><a href="c-2-accessories.aspx">Accessories</a></span><!-- Begin Item -->
            <div class="megamenu_fullwidth"><!-- Begin Item Container -->
                (!Topic Name="MMenuAccessories"!)
            </div><!-- End Item Container -->
        </li><!-- End Item -->
        <!--<li><span class="drop"><a href="c-3-tabletop.aspx">Tabletop</a></span>
            <div class="megamenu_fullwidth">
                (!Topic Name="MMenuTabletop"!)
            </div>
        </li>-->
        <li><span class="drop"><a href="c-4-wall-decor.aspx">Wall Decor</a></span><!-- Begin Item -->
            <div class="megamenu_fullwidth"><!-- Begin Item Container -->
                (!Topic Name="MMenuWallDecor"!)
            </div><!-- End Item Container -->
        </li><!-- End Item -->
        <li><span class="drop"><a href="c-5-lighting.aspx">Lighting</a></span><!-- Begin Item -->
            <div class="megamenu_fullwidth"><!-- Begin Item Container -->
                (!Topic Name="MMenuLighting"!)
            </div><!-- End Item Container -->
        </li><!-- End Item -->
        <li><span class="drop"><a href="c-6-furniture.aspx">Furniture</a></span><!-- Begin Item -->
            <div class="megamenu_fullwidth"><!-- Begin Item Container -->
                (!Topic Name="MMenuFurniture"!)
            </div><!-- End Item Container -->
        </li><!-- End Item -->
        <!--<li><span class="nodrop"><a href="c-7-clearance.aspx">Clearance</a></span></li>-->
    </ul><!-- End Mega Menu -->
</div><!-- End Mega Menu Container -->


  </div>

<div class="row show-on-phones">
  <div class="twelve columns">
  
  <ul id="mobilenav" class="block-grid three-up text-center">
    <li><a href="c-2-accessories.aspx">ACCESSORIES</a></li>
    <!--<li><a href="c-3-tabletop.aspx">TABLETOP</a></li>-->
    <li><a href="c-4-wall-decor.aspx">WALL DECOR</a></li>
    <li><a href="c-5-lighting.aspx">LIGHTING</a></li>
    <li><a href="c-6-furniture.aspx">FURNITURE</a></li>
    <!--<li><a href="c-7-clearance.aspx">CLEARANCE</a></li>-->
  </ul>
  
  </div>
</div>

</div>

</div>


<!-- Panel -->
<div id="dprotop">
	<div id="dpro">
		<div class="content clearfix">
			<div>
				(!XmlPackage Name="rev.manufacturers"!)
			</div>
		</div>
	</div>

	<!-- The tab on top -->
	<div class="tabcontainer">
	<div class="tab hide-on-tablets">
		<ul class="login">
			<li class="left">&nbsp;</li>
			<li id="toggle">
				<a id="open" class="open" href="#"><i class="general foundicon-plus"></i> Manufacturers</a>
				<a id="close" style="display: none;" class="close" href="#"><i class="general foundicon-remove"></i> Close Tab</a>
			</li>
			<li class="right">&nbsp;</li>
		</ul> 
	</div>
	</div>
	<!-- / top -->
	
</div>
<!--panel -->


<!-- navbar [end] -->



<!-- breadcrumbs [begin] -->
<div class="row">
<div class="twelve columns">
<ul class="breadcrumbs">
	<li class="current"><span>(!SECTION_TITLE!)</span></li>
</ul>
</div>
</div>
<!-- breadcrumbs [end] -->
<!-- header [end] -->


<div class="row">
<div class="twelve columns maincontent">
<!-- CONTENTS START -->
<asp:placeholder id="PageContent" runat="server"></asp:placeholder>
<!-- CONTENTS END -->
</div>
</div>

</div>
</div>


<!-- footer [begin] -->
<div id="footer">

<div class="row socialBox hide-on-phones">
	<div class="seven columns">
		<div class="news">(!XmlPackage Name="page.newsheadlines"!)</div>
	</div>
	<div class="five columns">
		<a href="http://www.house2home.us" target="_blank"><div class="expservices"></div></a>
	</div>
</div>

<div class="footerhr"></div>

  <div class="row">
	<div class="two columns">
		<dl class="footer-nav">
                  <dt>Accessories</dt>
                  <dd><a href="c-12-decorative-accents.aspx">Decorative Accents</a></dd>
                  <dd><a href="c-14-candleholders.aspx">Candleholders</a></dd>
                  <dd><a href="c-15-baskets-boxes.aspx">Baskets & Boxes</a></dd>
                  <dd><a href="c-10-picture-frames.aspx">Picture Frames</a></dd>
                  <dd><a href="c-9-urns-jars.aspx">Urns & Jars</a></dd>
                  <dd><a href="c-11-decorative-bowls.aspx">Decorative Bowls</a></dd>
                  <dd><a href="c-8-vases.aspx">Vases</a></dd>
			<dd><a href="c-29-trays.aspx">Trays</a></dd>
			<dd><a href="c-30-finials.aspx">Finials</a></dd>
			<dd><a href="c-31-planters.aspx">Planters</a></dd>
			<dd><a href="c-32-sculpture.aspx">Sculpture</a></dd>
			<dd><a href="c-45-floral.aspx">Floral</a></dd>
		</dl>
            
      </div>
      <div class="two columns">
		<dl class="footer-nav">
                  <dt>Lighting</dt>
                  <dd><a href="c-24-floor-lamps.aspx">Floor Lamps</a></dd>
                  <dd><a href="c-25-table-lamps.aspx">Table Lamps</a></dd>
                  <dd><a href="c-27-hanging-lamps.aspx">Hanging Lamps</a></dd>
			<dd><a href="c-33-lighted-sconces.aspx">Lighted Sconces</a></dd>
		</dl>
            <!--<dl class="footer-nav">
                  <dt>Tabletop</dt>
                  <dd><a href="c-18-entertaining.aspx">Entertaining</a></dd>
                  <dd><a href="c-17-dining.aspx">Dining</a></dd>
                  <dd><a href="c-16-kitchen.aspx">Kitchen</a></dd>
                  <dd><a href="c-19-barware-accessories.aspx">Barware & Accessories</a></dd>
		</dl>-->
            <dl class="footer-nav">
                  <dt>Wall Décor</dt>
                  <dd><a href="c-20-racks-shelving.aspx">Racks & Shelving</a></dd>
                  <dd><a href="c-21-sconces.aspx">Sconces</a></dd>
                  <dd><a href="c-22-wall-clocks.aspx">Wall Clocks</a></dd>
                  <dd><a href="c-23-wall-art.aspx">Wall Art</a></dd>
                  <dd><a href="c-26-mirrors.aspx">Mirrors</a></dd>
			<dd><a href="c-28-hanging-picture-frames.aspx">Hanging Frames</a></dd>
		</dl>
            
	</div>
      <div class="two columns">
            
            <dl class="footer-nav">
                  <dt><a href="c-6-furniture.aspx">Furniture</a></dt>
			<dd><a href="c-34-chairs.aspx">Chairs</a></dd>
			<dd><a href="c-35-benches-ottomans.aspx">Benches & Ottomans</a></dd>
			<dd><a href="c-36-pet-beds.aspx">Pet Beds</a></dd>
			<dd><a href="c-40-tables-desks.aspx">Tables & Desks</a></dd>
			<dd><a href="c-41-chests-cabinets.aspx">Chests & Cabinets</a></dd>
			<dd><a href="c-43-fireplace.aspx">Fireplace</a></dd>
			<dd><a href="c-44-accent-furniture.aspx">Accent Furniture</a></dd>
		</dl>
            <!--<dl class="footer-nav">
                  <dt><a href="c-7-clearance.aspx">Clearance</a></dt>
		</dl>-->
            
	</div>
    <div class="five columns dotborder">
  	<div id="socialMedia">
      find&nbsp;us&nbsp;on<br />
<!--        <a href="http://www.facebook.com/expressionsdecor" target="_new" class="socialinkF"><i class="social foundicon-facebook"></i></a>
        <a href="http://plus.google.com/u/0/b/117223946102304177432/117223946102304177432/posts" target="_new" class="socialinkG"><i class="social foundicon-google-plus"></i></a>
        <a href="http://www.twitter.com/ExpressionsDeco" target="_new" class="socialinkT"><i class="social foundicon-twitter"></i></a>
        <a href="http://www.pinterest.com/expressionsdeco/" target="_new" class="socialinkP"><i class="social foundicon-pinterest"></i></a>-->

<!-- AddThis Follow BEGIN -->
<div class="addthis_toolbox addthis_32x32_style addthis_default_style">
<a class="addthis_button_facebook_follow" addthis:userid="expressionsdecor"></a>
<a class="addthis_button_twitter_follow" addthis:userid="ExpressionsDeco"></a>
<a class="addthis_button_google_follow" addthis:userid="117223946102304177432"></a>
<a class="addthis_button_pinterest_follow" addthis:userid="expressionsdeco"></a>
</div>
<script type="text/javascript">var addthis_config = {"data_track_addressbar":true};</script>
<script type="text/javascript" src="//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-50dc8ab51b6823ca"></script>
<!-- AddThis Follow END -->

	  <ul>
		<li><a href="t-about.aspx">About us</a></li>
		<li><a href="t-shipping.aspx">Shipping Info</a></li>
		<li><a href="t-returns.aspx">Return Policy</a></li>
		<li><a href="t-termsandconditions.aspx">Terms &#38; Conditions</a></li>
		<li><a href="t-privacy.aspx">Your Privacy</a></li>
		<li><a href="http://www.house2home.us" target="_blank">Design Services</a></li>
      </ul>
    </div>
    <div>
	<dl class="footer-nav">
            	<dt>Customer Service</dt>
                  <dd><a href="account.aspx"><i class="main foundicon-settings"></i> My Account</a></dd>
                  <dd><a href="shoppingcart.aspx"><i class="main foundicon-cart"></i> My Shopping Bag</a></dd>
                  <dd><a href="t-shopping.aspx"><i class="main foundicon-monitor"></i> Shopping Our Site</a></dd>
                  <dd><a href="t-contact.aspx"><i class="main foundicon-phone"></i> Contact Us</a></dd>
            </dl>
    </div>
    </div>
    
  </div>
  
  <div id="copyright" class="row">
    <div class="six columns">&#169; 2011(!COPYRIGHTYEARS!) Expressions by Décor &#38; More, Inc. All rights reserved.</div>
    <div class="six columns">
<a href="http://www.freesitemapgenerator.com/"><img
 border="0" alt="Sitemap Generator"
 style="border:0; margin:3px 3px 3px 3px; padding:0; float:right"
 src="http://live.freesitemapgenerator.com/img/fsgbig51.gif"></a><script
 language="javascript">
/* FSG 0.96 script */ 
var fsg_Live_date_package_version='0.96';
var fsg_image = 'big5';
var fsg_serial = 'cdb00fba4857e35035624eda54a83b18';
</script><script language="javascript"
 src="http://live.freesitemapgenerator.com/scripts/fsg096.js"></script>
    </div>
  </div>

</div>
<!-- footer [end] -->


<!-- Included JS Files -->
<!--<script src="javascripts/jquery.min.js"></script>-->
<script src="javascripts/foundation.js"></script>
<script src="javascripts/app.js"></script>

<script type="text/javascript"> 
   $(window).load(function() {
       $('#homefeature').orbit({
		animation: 'horizontal-push',			// fade, horizontal-slide, vertical-slide, horizontal-push
		animationSpeed: 800,				// how fast animtions are
		timer: true,					// true or false to have the timer
		resetTimerOnClick: true,			// true resets the timer instead of pausing slideshow progress
		advanceSpeed: 8000,				// if timer is enabled, time between transitions 
		pauseOnHover: true,				// if you hover pauses the slider
		startClockOnMouseOut: true,			// if clock should start on MouseOut
		startClockOnMouseOutAfter: 1000,		// how long after MouseOut should the timer start again
		directionalNav: false,				// manual advancing directional navs
		captions: false,					// do you want captions?
		captionAnimation: 'fade',			// fade, slideOpen, none
		captionAnimationSpeed: 800,			// if so how quickly should they animate in
		bullets: true,					// true or false to activate the bullet navigation
		bulletThumbs: false,				// thumbnails for the bullets
		bulletThumbLocation: '',			// location from this file where thumbs will be
		afterSlideChange: function(){},		// empty function 
		fluid: '980x367'
	});
	$('#recentFeature').orbit({
		animation: 'horizontal-push',			// fade, horizontal-slide, vertical-slide, horizontal-push
		animationSpeed: 1600,                // how fast animtions are
		timer: false, 			 // true or false to have the timer
		resetTimerOnClick: false,           // true resets the timer instead of pausing slideshow progress
		advanceSpeed: 8000, 		 // if timer is enabled, time between transitions 
		pauseOnHover: true, 		 // if you hover pauses the slider
		startClockOnMouseOut: true, 	 // if clock should start on MouseOut
		startClockOnMouseOutAfter: 1000, 	 // how long after MouseOut should the timer start again
		directionalNav: true, 		 // manual advancing directional navs
		captions: false, 			 // do you want captions?
		captionAnimation: 'fade', 		 // fade, slideOpen, none
		captionAnimationSpeed: 800, 	 // if so how quickly should they animate in
		bullets: false,			 // true or false to activate the bullet navigation
		bulletThumbs: false,		 // thumbnails for the bullets
		bulletThumbLocation: '',		 // location from this file where thumbs will be
		afterSlideChange: function(){}, 	 // empty function 
		fluid: '809x116'
	});
   });
</script>
<script src="javascripts/slide.js" type="text/javascript"></script>

(!ADDRESS_VERIFICATION_DIALOG!)

</body>
</html>

(!PAGEINFO!)