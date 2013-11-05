if(!window.ComponentArt_Snap_Core_Loaded){ComponentArt.Web.UI.SnapDockEventArgs=function(_1,_2){if(window.ComponentArt_Atlas){ComponentArt.Web.UI.SnapDockEventArgs.initializeBase(this);}var _3=_1;var _4=_2;this.get_elementId=function(){return _3;};this.get_index=function(){return _4;};};if(window.ComponentArt_Atlas){ComponentArt.Web.UI.SnapDockEventArgs.registerClass("ComponentArt.Web.UI.SnapDockEventArgs",Sys.EventArgs);}var _q42=null;var _q44=null;var _q45=null;var _q43=null;var _q46=null;var _q41=null;var _q23=0;function ComponentArt_Snap(_5){if(window.ComponentArt_Atlas){ComponentArt.Web.UI.Snap.initializeBase(this,[document.getElementById(_5)]);}this.Id=_5;this.RenderOverWindowedObjects=false;this.BringToTopOnClick=true;this.CookieName=null;this.OriginalWidth=0;this.OriginalHeight=0;this.MustBeDocked=false;this.IsDockable=false;this.IsDocked=false;this.DockElement=null;this.DockIndex=-1;this.LastDockElement=null;this.LastDockIndex=-1;this.DockingContainers=null;this.DockingStyle="";this.DockedCssClass="";this.UndockedCssClass="";this.DraggingStyle="";this.DraggingMode="";this.OutlineElement=null;this.AllowHorizontal=true;this.AllowVertical=true;this.MinWidth=0;this.MinHeight=0;this.ResizingMode="";this.ResizeThreshold=5;this.EnableFloating=false;this.FloatAnimation=false;this.FloatAnimationTimer=null;this.AlignAnimationTower=null;this.OldScrollHandler=null;this.OldResizeHandler=null;this.FloatOffsetX;this.FloatOffsetY;this.MinLeft=0;this.MinTop=0;this.Alignment="";this.CollapseExpandState=1;this.ExpandedHeader=null;this.ExpandedFooter=null;this.CollapsedHeader=null;this.CollapsedFooter=null;this.RegularHeight=0;this.RegularInnerHeight=0;}ComponentArt_Snap.prototype.PublicProperties=[["BringToTopOnClick",Boolean],["Footer",String,1,1],["Header",String,1,1],["Id",String],["IsMinimized",Boolean,1],["IsCollapsed",Boolean,1],["MustBeDocked",Boolean],["PostbackOnCollapse",Boolean],["PostbackOnDock",Boolean],["PostbackOnExpand",Boolean]];ComponentArt_Snap.prototype.PublicMethods=[["Collapse"],["Dock",,,[["dockId",String],["dockIndex",Number]]],["Expand"],["FloatTo",,,[["x",Number],["y",Number]]],["StartDragging",,,[["object",Object],["event",Object]]],["ToggleExpand"],["ToggleMinimize"],["Undock",,,[["x",Number],["y",Number]]]];ComponentArt_Snap.prototype.PublicEvents=[["SnapCollapse"],["SnapDock"],["SnapExpand"]];_qE3(ComponentArt_Snap);window.ComponentArt.Web.UI.Snap=window.ComponentArt_Snap;if(window.ComponentArt_Atlas){ComponentArt.Web.UI.Snap.registerClass("ComponentArt.Web.UI.Snap",Sys.UI.Control);if(Sys.TypeDescriptor){Sys.TypeDescriptor.addType("componentArtWebUI","snap",ComponentArt.Web.UI.Snap);}}ComponentArt_Snap.prototype.GetProperty=function(_6){return this[_6];};ComponentArt_Snap.prototype.SetProperty=function(_7,_8){this[_7]=_8;};ComponentArt_Snap.prototype.get_isCollapsed=function(){return (this.CollapseExpandState==0);};ComponentArt_Snap.prototype.get_footer=function(){var _9=document.getElementById(this.ClientPrefix+"FooterSpan");if(_9){return _9.innerHTML;}};ComponentArt_Snap.prototype.set_footer=function(_a){var _b=document.getElementById(this.ClientPrefix+"FooterSpan");if(_b){_b.innerHTML=_a;}};ComponentArt_Snap.prototype.get_header=function(){var _c=document.getElementById(this.ClientPrefix+"HeaderSpan");if(_c){return _c.innerHTML;}};ComponentArt_Snap.prototype.set_header=function(_d){var _e=document.getElementById(this.ClientPrefix+"HeaderSpan");if(_e){_e.innerHTML=_d;}};ComponentArt_Snap.prototype.Expand=function(){_q14(this);};ComponentArt_Snap.prototype.Collapse=function(){_q2(this);};ComponentArt_Snap.prototype.Dock=function(_f,_10,_11){_q4(this.Id,_f,_10,_11);};ComponentArt_Snap.prototype.Undock=function(x,y){_q4F(this,x,y);};ComponentArt_Snap.prototype.FloatTo=function(x,y){art_AnimateSpan(this.Id,x,y);};ComponentArt_Snap.prototype.StartDragging=function(e){_q4C(e,this);};ComponentArt_Snap.prototype.ToggleExpand=function(){_q15(this);};ComponentArt_Snap.prototype.Minimize=function(_17){if(!_17&&this.MinimizeDirectionElement){var _18=document.getElementById(this.MinimizeDirectionElement);if(_18){this.MinimizeAnimate(1,this.Frame,_18);}}else{this.Frame.style.display="none";}this.IsMinimized=true;};ComponentArt_Snap.prototype.UnMinimize=function(){if(this.MinimizeDirectionElement){var _19=document.getElementById(this.MinimizeDirectionElement);if(_19){this.Frame.style.display="";this.MinimizeAnimate(0,_19,this.Frame);}}else{this.Frame.style.display="";}this.IsMinimized=false;};ComponentArt_Snap.prototype.ToggleMinimize=function(_1a){if(this.IsMinimized){if(this.AutoPostBackOnUnMinimize){__doPostBack(this.ControlId,"UNMINIMIZE");return;}this.UnMinimize();if(this.AutoCallBackOnUnMinimize){this.Callback();}}else{if(!_1a&&this.AutoPostBackOnMinimize){__doPostBack(this.ControlId,"MINIMIZE");return;}this.Minimize(_1a);if(!_1a&&this.AutoCallBackOnMinimize){this.Callback();}}_q52(this);};function art_InitCore(_1b,_1c,_1d){var _1e=art_GetInstance(_1b);_q121(_1e);_1e.RepositionFloater=eval("art_RepositionFloater_"+_1b+";");_1e.Situation=document.getElementById("Art_Situation_"+_1b);_1e.Frame=document.getElementById(_1b);_1e.Frame.isSnapFrame=true;_1e.InnerFrame=document.getElementById(_1e.ClientPrefix+"InnerSpan");_1e.IFrame=document.getElementById("Art_IFrame_"+_1b);if(_1e.IFrame){_1e.IFrame.style.height=_1e.Frame.offsetHeight+"px";}var _1f=_1e.Frame;_1e.OriginalWidth=_1f.offsetWidth;_1e.OriginalHeight=_1f.offsetHeight;if(_1f.style.zIndex>_q23){_q23=_1f.style.zIndex;}_1f.style.height="";if(_1d){_1e.ToggleMinimize(true);}_q17(_1b,_1c);}var ComponentArt_SnapToMinimize=null;ComponentArt_Snap.prototype.MinimizeAnimate=function(_20,_21,_22){var _23=_q85(_21);var _24=_q86(_21);var _25=_21.offsetWidth;var _26=_21.offsetHeight;var _27=_q85(_22);var _28=_q86(_22);var _29=_22.offsetWidth;var _2a=_22.offsetHeight;var _2b=this.MinimizeCssClass;var _2c=this.MinimizeDuration;ComponentArt_SnapToMinimize=this;art_MinimizeAnimate(_20,_2b,this.MinimizeSlide,_2c,_23,_24,_25,_26,_27,_28,_29,_2a);};var ComponentArt_SnapMinimizeObject=null;function art_MinimizeAnimate(_2d,_2e,_2f,_30,_31,_32,_33,_34,_35,_36,_37,_38){if(!ComponentArt_SnapMinimizeObject){ComponentArt_SnapMinimizeObject=document.createElement("div");if(_2e){ComponentArt_SnapMinimizeObject.className=_2e;}else{ComponentArt_SnapMinimizeObject.style.border="1px solid black";}ComponentArt_SnapMinimizeObject.style.position="absolute";ComponentArt_SnapMinimizeObject.style.visibility="visible";ComponentArt_SnapMinimizeObject.style.zIndex=90210;ComponentArt_SnapMinimizeObject.MinimizeStartTime=(new Date()).getTime();document.body.insertBefore(ComponentArt_SnapMinimizeObject,document.body.firstChild);}var _39=(new Date()).getTime()-ComponentArt_SnapMinimizeObject.MinimizeStartTime;var _3a=ComponentArt_SlidePortionCompleted(_39,_30,_2f);var _3b=_31+(_35-_31)*_3a;var _3c=_32+(_36-_32)*_3a;var _3d=_33+(_37-_33)*_3a;var _3e=_34+(_38-_34)*_3a;ComponentArt_SnapMinimizeObject.style.left=_3b+"px";ComponentArt_SnapMinimizeObject.style.top=_3c+"px";ComponentArt_SnapMinimizeObject.style.width=_3d+"px";ComponentArt_SnapMinimizeObject.style.height=_3e+"px";if(_3a==1){_q3(ComponentArt_SnapMinimizeObject);ComponentArt_SnapMinimizeObject=null;if(_2d){ComponentArt_SnapToMinimize.Frame.style.display="none";ComponentArt_SnapToMinimize.Frame.style.visibility="hidden";}else{ComponentArt_SnapToMinimize.Frame.style.visibility="visible";}ComponentArt_SnapToMinimize=null;}else{setTimeout("art_MinimizeAnimate("+_2d+",'"+_2e+"',"+_2f+","+_30+","+_31+","+_32+","+_33+","+_34+","+_35+","+_36+","+_37+","+_38+")",20);}}function _q19(o){while(o!=document.body){if(_q24(o)){return o;}o=o.parentNode;}return null;}function _q1C(_40){var _41=_q1E(_40);if(_41){return art_GetInstance(_41);}else{return null;}}function _q24(obj){return (obj&&obj.isSnapFrame);}function _q1D(_43){for(var i=_43.childNodes.length-1;i>=0;i--){var _45=_43.childNodes[i];if(_45&&_45.id&&_45.id.indexOf("Art_IFrame_")==0){return _45;}}return null;}function _q1E(_46){if(_46&&_46.id){return _46.id;}else{return null;}}function art_GetInstance(_47){var ret=eval("("+_47+"? "+_47+" : null)");return ret;}function _q28(obj,_4a){for(var i=10;i>0;i--){var _4c=document.createElement("div");var rs=_4c.style;rs.position="absolute";rs.left=i+"px";rs.top=i+"px";rs.width=Math.max(0,obj.offsetWidth-i*2)+"px";rs.height=Math.max(0,obj.offsetHeight-i*2)+"px";rs.zIndex=obj.style.zIndex-i;rs.backgroundColor=_4a;var _4e=1-i/(i+1);if(cart_browser_n6||cart_browser_mozilla){rs.opacity=_4e;rs.setProperty("-moz-opacity",_4e,"");}else{rs.filter="alpha(opacity="+(100*_4e)+")";}obj.appendChild(_4c);}}function _q27(obj,_50,_51){var _52=document.createElement("div");var rs=_52.style;rs.position="absolute";rs.left="0px";rs.top="0px";rs.width=obj.offsetWidth+"px";rs.height=obj.offsetHeight+"px";rs.zIndex=obj.style.zIndex+1;rs.backgroundColor=_51;rs.borderStyle="solid";rs.borderWidth=2;rs.borderColor=_50;if(cart_browser_n6||cart_browser_mozilla){rs.opacity=0.3;rs.setProperty("-moz-opacity",0.3,"");}else{rs.filter="alpha(opacity=30)";}obj.appendChild(_52);}function _q1(_54){if(_54.RenderOverWindowedObjects){return;}if(_q44==null){_q44=document.getElementsByTagName("select");_q43=new Array(_q44.length);_q45=new Array(_q44.length);_q41=new Array(_q44.length);_q46=new Array(_q44.length);}for(var i=0;i<_q44.length;i++){_q49(_q44[i],i);}}function _q49(obj,_57){_q45[_57]=_q35(obj);_q43[_57]=_q34(obj);_q46[_57]=obj.offsetWidth-1;_q41[_57]=obj.offsetHeight-1;}function _q33(x1,y1,w1,h1,x2,y2,w2,h2){return (!(x1+w1<=x2||y1+h1<=y2||x1>=x2+w2||y1>=y2+h2));}function _q4B(){if(!_q44){return;}for(var i=0;i<_q44.length;i++){_q44[i].style.visibility="inherit";}}function _q40(_61,_62,_63,_64,_65){if(_61.RenderOverWindowedObjects){return;}if(_q44==null){_q1(_61);}for(var i=0;i<_q44.length;i++){if(_q24(_q19(_q44[i]))){continue;}if(_q33(_q43[i],_q45[i],_q46[i],_q41[i],_62,_63,_64,_65)){_q44[i].style.visibility="hidden";}else{_q44[i].style.visibility="inherit";}}}function _q52(_67){if(!_67.Situation){return;}var _68=_67.Frame;if(!_68){return;}var _69;var _6a;if(_67.EnableFloating&&_67.FloatAlignment=="Default"){_69=_67.FloatOffsetX;_6a=_67.FloatOffsetY;}else{_69=parseInt(_68.style.left);_6a=parseInt(_68.style.top);}var _6b=_68.style.left+","+_68.style.top+","+_68.offsetWidth+","+_68.offsetHeight+","+(_67.DockElement?_67.DockElement:"")+","+_67.DockIndex+","+(_67.CollapseExpandState==0?1:0)+","+(_67.IsMinimized?1:0);_67.Situation.value=_6b;if(_67.CookieName){_q47(_67.CookieName,_6b,7);}}function _q47(_6c,_6d,_6e){var _6f=new Date();var _70=new Date();if(_6e==null||_6e==0){_6e=1;}_70.setTime(_6f.getTime()+3600000*24*_6e);document.cookie=_6c+"="+_6d+";expires="+_70.toGMTString();}function _q3(obj){if(obj){if(cart_browser_ie){obj.removeNode(true);}else{obj.parentNode.removeChild(obj);}}}function _q34(o){return _q85(o);}function _q35(o){return _q86(o);}function _q1A(e,obj){var x=e.offsetX;var o=e.srcElement;while(o!=obj&&o!=document.body){x+=o.offsetLeft;o=o.offsetParent;}return x;}function _q1B(e,obj){var y=e.offsetY;var o=e.srcElement;while(o!=obj&&o!=document.body){y+=o.offsetTop;o=o.offsetParent;}return y;}ComponentArt_Snap.prototype.Callback=function(){var url=this.CallbackPrefix;var _7d="Cart_"+this.Id+"_DockElement="+this.DockElement;_7d+="&Cart_"+this.Id+"_DockIndex="+this.DockIndex;_7d+="&Cart_"+this.Id+"_IsMinimized="+(this.IsMinimized?"true":"false");_7d+="&Cart_"+this.Id+"_IsCollapsed="+(this.CollapseExpandState==0?"true":"false");this.DoCallback(url,_7d);};ComponentArt_Snap.prototype.DoCallback=function(url,_7f){var _80=false;var _81;if(window.XMLHttpRequest){_80=true;var _81=new XMLHttpRequest();_81.open("POST",url,true);_81.setRequestHeader("Content-Type","application/x-www-form-urlencoded");_81.send(_7f);}else{if(document.implementation&&document.implementation.createDocument){_81=document.implementation.createDocument("","",null);}else{if(document.all){var _82=this.Id+"_island";var _83=document.getElementById(_82);if(!_83){_83=document.createElement("xml");_83.id=_82;document.body.appendChild(_83);}if(_83.XMLDocument){_81=_83.XMLDocument;}else{return false;}}else{return false;}}}if(!_80){_81.async=true;try{_81.load(url+"&"+_7f);}catch(ex){}}return true;};window.ComponentArt_Snap_Core_Loaded=true;}
