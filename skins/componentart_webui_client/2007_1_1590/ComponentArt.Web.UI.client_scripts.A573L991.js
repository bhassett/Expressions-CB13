window.ComponentArt_AjaxCall=function(_1,_2,_3){var _4=null;var _5;var _6=location.href;var _7=_6.indexOf("?")>0?"&":"?";var _8="Cart_Callback_Method="+_1;if(_3||_2 instanceof Array){for(var i=0;i<_2.length;i++){_8+="&Cart_Callback_Method_Param="+encodeURIComponent(_2[i]);}}else{_8+="&Cart_Callback_Method_Param="+encodeURIComponent(_2);}function _q18C(){if(_5.readyState&&_5.readyState!=4&&_5.readyState!="complete"){return;}if(_5&&_5.documentElement){_4=_5.documentElement.nodeValue;}else{alert("The data could not be loaded.");}}if(window.XMLHttpRequest){_5=new XMLHttpRequest();_5.open("POST",_6,false);_5.setRequestHeader("Content-Type","application/x-www-form-urlencoded");_5.send(_8);if(_5.responseXML&&_5.responseXML.documentElement){var _a=_5.responseXML.documentElement.firstChild.nodeValue;_4=_a.replace(/\$\$\$CART_CDATA_CLOSE\$\$\$/g,"]]>");return _4;}}else{if(document.implementation&&document.implementation.createDocument){_5=document.implementation.createDocument("","",null);_5.async=false;_5.onload=_q18C;}else{if(document.all){if(window.ActiveXObject){try{_5=new ActiveXObject("Microsoft.XMLHTTP");_5.open("POST",_6,false);_5.setRequestHeader("Content-Type","application/x-www-form-urlencoded");_5.send(_8);if(_5.responseXML&&_5.responseXML.documentElement){var _a=_5.responseXML.documentElement.firstChild.nodeValue;_4=_a.replace(/\$\$\$CART_CDATA_CLOSE\$\$\$/g,"]]>");return _4;}}catch(ex){}}if(_5==null){var _b=this.Id+"_island";var _c=document.getElementById(_b);if(!_c){_c=document.createElement("xml");_c.id=_b;document.body.appendChild(_c);}if(_c.XMLDocument){_5=_c.XMLDocument;_5.async=false;_5.onreadystatechange=_q18C;}else{return null;}}}}}_5.async=false;try{_5.load(_6+_7+_8);}catch(ex){alert("Data not loaded: "+(ex.message?ex.message:ex));}return _4;};