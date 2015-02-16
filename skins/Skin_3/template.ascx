<!DOCTYPE html>
<%@ Control Language="c#" AutoEventWireup="false" Inherits="InterpriseSuiteEcommerce.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>

<!-- Google Analytics -->
<script type="text/javascript">
  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  })(window,document,'script','//www.google-analytics.com/analytics.js','ga');
  ga('create', 'UA-23764710-1', 'expressionsdecor.com');
  ga('require', 'displayfeatures');
  ga('require', 'ecommerce');
  ga('send', 'pageview');
</script>

<!-- paulirish.com/2008/conditional-stylesheets-vs-css-hacks-answer-neither/ -->
<!--[if lt IE 7]> <html class="no-js lt-ie9 lt-ie8 lt-ie7" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="no-js lt-ie9 lt-ie8" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="no-js lt-ie9" lang="en"> <![endif]-->
<!--[if gt IE 8]><!--> <html class="no-js" lang="en"> <!--<![endif]-->

<head>
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width" />
<meta name="description" content="(!METADESCRIPTION!)">
<meta name="keywords" content="(!METAKEYWORDS!)">

<title>(!METATITLE!)</title>

<link rel="shortcut icon" type="image/x-icon" href="favicon.ico">

<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css">
<link rel="Stylesheet" href="skins/Skin_(!SKINID!)/ui-lightness/jquery-ui-1.8.16.custom.css" type="text/css" />
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/foundation.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/app.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/icons-main.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/icons-general.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/icons-social.css">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/slide.css">

<link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/demo.css">


<!--[if lt IE 9]>
    <link rel="stylesheet" href="skins/Skin_(!SKINID!)/styles/ie.css">
<![endif]-->

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
<script type="text/javascript" src="javascripts/modernizr.foundation.js"></script>
<script type="text/javascript" src="javascripts/jquery.easing.js"></script>


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

<div id="main" class="clearfix">
    <div id="menu_container" runat="server" visible="false"></div>

    <!-- header [begin] -->
    <!-- expHeader [begin] -->
    <div id="expHeader" class="container">
      <!-- Standard Header Display -->
      <div class="row hide-on-phones">

        <div class="four columns">
          <a href="/"><img id="headerlogo" src="skins/Skin_(!SKINID!)/images/expressions-site-logo.png"></a>
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
      
      <!--  Mobile Header Display  -->
      <div class="row show-on-phones">
        <div class="twelve columns text-center">
          <a href="/"><img id="headerlogo" src="skins/Skin_(!SKINID!)/images/expressions-site-logo.png"></a>
        </div>
      </div>
      
      <div class="row show-on-phones">
      	<div class="twelve columns text-center">
          <div id="headeruserinfo">(!USERNAME!)&#32;&#32;<a href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a>&#32;&#124;&#32;<a href="account.aspx">My&#32;Account</a></div>
          <div id="navSearch">(!XmlPackage Name="skin.search"!)<!--&#32;&#45;&#32;or&#32;&#45;&#32;--><br><a href="searchadv.aspx">Advanced Search <i class="main foundicon-search"></i></a></div>
        </div>
      </div>
    </div>
    <!-- expHeader [end] -->

    <!-- navbar [begin] -->
    <div id="topNav" class="container">
      <div class="row">
        <div class="twelve columns">
          <ul id="mainmenu">
            <li><a href="/">HOME</a></li>
            <li><a href="c-2-accessories.aspx">ACCESSORIES</a></li>
            <li><a href="c-4-wall-decor.aspx">WALL DECOR</a></li>
            <li><a href="c-5-lighting.aspx">LIGHTING</a></li>
            <li><a href="c-6-furniture.aspx">FURNITURE</a></li>
          </ul>
        </div>
      </div>
    </div>
    <!-- navbar [end] -->

  <div class="bodypanel">

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


    <div class="row hide-on-phones">
      <div class="twelve columns freeship"><h1>FREE Shipping on ALL orders... ALWAYS!</h1>Free Standard Shipping (3–5 business days). Oversized handling fees may apply. Valid on shipping to the 48 contiguous states, as well as APO/FPO.</div>
    </div>

    <div class="row show-on-phones">
      <div class="twelve columns freeship"><h1>FREE Shipping</h1>Free Standard Shipping (3–5 business days). Oversized handling fees may apply. Valid on shipping to the 48 contiguous states, as well as APO/FPO.</div>
    </div>


    <div class="row">
      
      <!-- <div class="leftnavvy">
          <img src="skins/Skin_(!SKINID!)/images/left_header_browse_cat.jpg" alt="" width="209"height="24" class="leftnavvy_header" />
          <div class="EntityMenuAlignment">(!XmlPackage Name="rev.categories"!)</div>
      </div> -->

      <div class="twelve columns maincontent">
        <!-- CONTENTS START -->
        <asp:placeholder id="PageContent" runat="server"></asp:placeholder>
        <!-- CONTENTS END -->
      </div>
    </div>

  </div>

<!--   </div>
</div> -->


<!-- footer [begin] -->
<div id="footer">

  <!-- <div class="row socialBox hide-on-phones">
  	<div class="seven columns">
  		<div class="news">(!NEWS_SUMMARY!)</div>
  	</div>
  	<div class="five columns">
  		<a href="http://www.house2home.us" target="_blank"><div class="expservices"></div></a>
  	</div>
  </div> -->

  <div class="footerhr"></div>

  <div class="footerpaymethod">
    <div class="paymethod">
      <span>We Accept <img src="skins/Skin_(!SKINID!)/images/billmethod-icons.png"></span>
    </div>
  </div>

  <div class="row">
  	<div class="three columns phone-two">
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

    <div class="three columns phone-two">
      <dl class="footer-nav">
        <dt>Wall Décor</dt>
        <dd><a href="c-20-racks-shelving.aspx">Racks & Shelving</a></dd>
        <dd><a href="c-21-sconces.aspx">Sconces</a></dd>
        <dd><a href="c-22-wall-clocks.aspx">Wall Clocks</a></dd>
        <dd><a href="c-23-wall-art.aspx">Wall Art</a></dd>
        <dd><a href="c-26-mirrors.aspx">Mirrors</a></dd>
        <dd><a href="c-28-hanging-picture-frames.aspx">Hanging Frames</a></dd>
      </dl>
      <dl class="footer-nav">
        <dt>Lighting</dt>
        <dd><a href="c-24-floor-lamps.aspx">Floor Lamps</a></dd>
        <dd><a href="c-25-table-lamps.aspx">Table Lamps</a></dd>
        <dd><a href="c-27-hanging-lamps.aspx">Hanging Lamps</a></dd>
        <dd><a href="c-33-lighted-sconces.aspx">Lighted Sconces</a></dd>
      </dl> 
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
  	</div>

    <div class="three columns phone-two" style="margin-left:0">
      <dl class="footer-nav">
        <dt>Customer Service</dt>
        <dd><a href="account.aspx"><i class="main foundicon-settings"></i> My Account</a></dd>
        <dd><a href="shoppingcart.aspx"><i class="main foundicon-cart"></i> My Shopping Bag</a></dd>
        <dd><a href="t-shopping.aspx"><i class="main foundicon-monitor"></i> Shopping Our Site</a></dd>
        <dd><a href="t-contact.aspx"><i class="main foundicon-phone"></i> Contact Us</a></dd>
      </dl>      
  	</div>

    <div class="three columns phone-two" style="margin-left:0">
    	<div id="socialMedia">find&nbsp;us&nbsp;on<br />
        
        <!-- AddThis Follow BEGIN -->
        <div class="addthis_toolbox addthis_32x32_style addthis_default_style">
        <a class="addthis_button_facebook_follow" addthis:userid="expressionsdecor"></a>
        <a class="addthis_button_twitter_follow" addthis:userid="ExpressionsDeco"></a>
        <a class="addthis_button_google_follow" addthis:userid="117223946102304177432"></a>
        <br />
        <a class="addthis_button_pinterest_follow" addthis:userid="expressionsdeco"></a>
        <a class="addthis_button_instagram_follow" addthis:userid="expressionsdecor"></a>
        <a class="addthis_button_linkedin_follow" addthis:userid="117223946102304177432" addthis:usertype="company"></a>
        </div>
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
    </div>
    
  </div>
  
  <div id="copyright" class="row">
    <div class="twelve columns phone-four">&#169; 2011(!COPYRIGHTYEARS!) Expressions by Décor &#38; More, Inc. All rights reserved.</div>
  </div>

</div>
<!-- footer [end] -->

</div>
<!-- Customer Support -->
<!-- <div id="request-container">
  <div class="request-caption-wrapper">
    <span class="request-caption">(!stringresource name="main.content.1"!) </span>
    <div class="request-code-wrapper">
      <div class="request-generator-content">
        <span class="request-code">---------------</span>
      </div>
    </div>

    <div id="imgLoader" style="display: none">
      <img src="/skins/Skin_(!SKINID!)/images/loading.gif" alt="" title="" />
    </div>
  </div>
            
  <%--*width:155px; width:155px\0; ie8 and ie6/7 css hack--%>
  <div style="margin-left: 10px; float: left; width: 155px;">
    <a href="javascript:void(0);" class="generate-link"><img src="/skins/Skin_(!SKINID!)/images/refresh-captcha.png" alt="" title="" /></a>
    <a href="javascript:void(0);" class="copy-link">(!stringresource name="main.content.3"!)</a>
  </div>
</div> -->

<!-- <div class="footer-mobile-link">
(!MOBILE_FULLMODE_SWITCHER!)
</div> -->

<!-- <div class="leftnavvy">
  <h3>Browse Categories</h3>
  <div class="EntityMenuAlignment">(!XmlPackage Name="rev.categories"!)</div>
</div> -->

<!-- Included JS Files -->
<script type="text/javascript" src="javascripts/foundation.js"></script>
<script type="text/javascript" src="javascripts/app.js"></script>
<script type="text/javascript" src="javascripts/orbit.js"></script>
<script type="text/javascript" src="javascripts/slide.js"></script>

(!ADDRESS_VERIFICATION_DIALOG_LISTING!)

<!-- Address Verification -->
(!ADDRESS_VERIFICATION_DIALOG_OPTIONS!)

<!-- AddThis Smart Layers BEGIN -->
<script type="text/javascript" src="//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-50dc8ab51b6823ca"></script>
<script type="text/javascript">
  addthis.layers({
    'theme' : 'transparent',
    'share' : {
      'position' : 'left',
      'numPreferredServices' : 5
    }, 
    'follow' : {
      'services' : [
        {'service': 'facebook', 'id': 'expressionsdecor'},
        {'service': 'twitter', 'id': 'ExpressionsDeco'},
        {'service': 'google_follow', 'id': '117223946102304177432'}
      ]
    }
  });
</script>
<!-- AddThis Smart Layers END -->

<!-- Google Code for Remarketing Tag -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 986485184;
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="//googleads.g.doubleclick.net/pagead/viewthroughconversion/986485184/?value=0&amp;guid=ON&amp;script=0"/>
</div>
</noscript>

</body>
</html>