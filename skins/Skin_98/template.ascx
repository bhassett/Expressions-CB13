<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Control Language="c#" AutoEventWireup="false" Inherits="InterpriseSuiteEcommerce.TemplateBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>(!METATITLE!)</title>
<meta name="description" content="(!METADESCRIPTION!)">
<meta name="keywords" content="(!METAKEYWORDS!)">
<link rel="stylesheet" href="skins/Skin_(!SKINID!)/style.css" type="text/css">

<!--TESTING-->

<!--<link rel="Stylesheet" href="skins/Skin_(!SKINID!)/ui-lightness/jquery-ui-1.8.16.custom.css" type="text/css" />-->
<script type="text/javascript" src="jscripts/core.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery.min.v1.6.4.js"></script>
<script type="text/javascript" src="jscripts/jquery/jquery-ui-1.8.16.custom.min.js"></script>
<script type="text/javascript" src="jscripts/jquery/menu.js"></script>
<script type="text/javascript" src="jscripts/attribute.selectors.js"></script>
<script type="text/javascript" src="jscripts/address.verification.js"></script>
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
<div class="topnavvybase1" style="margin-bottom: 0px !important;">
    <div class="menutop">

    <%--Uncomment code below to enable Live Chat--%>
    <%--<div style="float: right; width: 150px; height: 20px; margin: 0 -150px 0 0">
    <script type="text/javascript" src="http://localhost/LiveSupport/CuteSoft_Client/CuteChat/Support-Image-Button.js.aspx">
    </script>
    <script type="text/javascript" src="http://localhost/LiveSupport/CuteSoft_Client/CuteChat/Support-Visitor-monitor-crossdomain.js.aspx">
    </script>
    </div>--%>
    
    <div id="menu_container" runat="server"></div>
    <div class="searcher">(!XmlPackage Name="skin.search"!)</div>
    </div>
    </div>
<div class="wrapper">
  <div class="centerer">
    <div class="topnavvy">
    
    <a href="default.aspx"><img src="skins/Skin_(!SKINID!)/images/blank.gif" class="logo" /></a>
    <div style="  float:right; height: 20px; visibility: (!VATDIVVISIBILITY!); display: (!VATDIVDISPLAY!); margin:30px 50px 0 0; color:#CCC !important;">VAT Mode: (!VATSELECTLIST!)&nbsp;</div>
    <div style="visibility:(!CURRENCYDIVVISIBILITY!); display:(!CURRENCYDIVDISPLAY!); float: right; height: 20px; margin-right: 50px; color:#CCC !important;">Currency:(!CURRENCYSELECTLIST!)</div>

    
    <div class="topnavvybase">
    <div style="visibility:(!COUNTRYDIVVISIBILITY!); display:(!COUNTRYDIVDISPLAY!); float: left; clear:both; margin-left: 2px; height: 20px;"> Language: (!COUNTRYSELECTLIST!) </div>
    (!USERNAME!)&nbsp;&nbsp;<a href="(!SIGNINOUT_LINK!)">(!SIGNINOUT_TEXT!)</a></div>
    <div class="topnavvybase3">    
    <a id="shopping-cart" class="headblue" href="shoppingcart.aspx">Shopping Basket&nbsp;((!NUM_CART_ITEMS!))</a>
    
    <div id="mini-cart"></div>
    </div>
    </div>
    <div class="contentarea">
        <div class="leftarea">
        <div  class="EntityMenuAlignment">(!XmlPackage Name="rev.attributes" IsForAttributes="true"!)</div>
        <div class="leftnavvy"><img src="skins/Skin_(!SKINID!)/images/left_header_browse_cat.jpg" class="leftnavvy_header" />
            <div class="EntityMenuAlignment">(!XmlPackage Name="rev.categories"!)</div>
        </div>
        <div class="leftnavvy"><img src="skins/Skin_(!SKINID!)/images/left_header_browse_manu.jpg" class="leftnavvy_header" />
        <div class="EntityMenuAlignment">(!XmlPackage Name="rev.manufacturers"!)</div> 
        </div>
        <div class="leftnavvy">
            <img src="skins/Skin_(!SKINID!)/images/left_header_browse_dept.jpg" class="leftnavvy_header" />
            <div class="EntityMenuAlignment">(!XmlPackage Name="rev.departments"!)</div> 
        </div>
        <div class="leftnavvy">
        <div align="left" style="float: left;">(!POLL!)</div>
        <div align="left" style="float: left;">(!MINICART!)</div>
      </div>
        </div>
      <div class="rightmain">
      <!-- CONTENTS START -->
      <asp:placeholder id="PageContent" runat="server"></asp:placeholder>
      <!-- CONTENTS END -->
       (!ADDRESS_VERIFICATION_DIALOG!)
      </div>
    </div>
    <div class="bottomer">
    <div class="footerleft"></div>
    <div class="footermid">
        <p><a class="foot" href="t-copyright.aspx">Copyright &copy; Fairy Sales Ever After. All Rights Reserved.</a></p>
        <p><a href="default.aspx" class="foot">Home</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="t-contact.aspx" class="foot"> Contact Us</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="t-returns.aspx" class="foot"> Return Policy</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="t-privacy.aspx" class="foot">Privacy Policy</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="t-security.aspx" class="foot">Security Policy</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="sitemap2.aspx" class="foot"> Site Map</a> 
            <br/>
            <br/>
        </p>
    </div>
    </div>
  </div>
</div>
</body>
</html>
