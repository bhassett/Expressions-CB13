function $bindMethod(object, method) {
  return function() {
    return method.apply(object, arguments);
  };
}

function $window_addLoad(handler) {
    if (window.addEventListener) { 
        window.addEventListener('load',handler,false);
    }
    else if (document.addEventListener) {
        document.addEventListener('load',handler,false);
    }
    else if (window.attachEvent) { 
        window.attachEvent('onload',handler);
    }
    else {
        if (typeof window.onload=='function') {
            var oldload=window.onload;
            window.onload = function(){
                oldload();
                handler();
            }
        } 
        else { window.onload=init; }
    }

}

function $getElementPosition(element) {
    var result = new Object();
    result.x = 0;
    result.y = 0;
    result.width = 0;
    result.height = 0;
    if (element.offsetParent) {
        result.x = element.offsetLeft;
        result.y = element.offsetTop;
        var parent = element.offsetParent;
        while (parent) {
            result.x += parent.offsetLeft;
            result.y += parent.offsetTop;
            var parentTagName = parent.tagName.toLowerCase();
            if (parentTagName != "table" &&
                parentTagName != "body" && 
                parentTagName != "html" && 
                parentTagName != "div" && 
                parent.clientTop && 
                parent.clientLeft) {
                result.x += parent.clientLeft;
                result.y += parent.clientTop;
            }
            parent = parent.offsetParent;
        }
    }
    else if (element.left && element.top) {
        result.x = element.left;
        result.y = element.top;
    }
    else {
        if (element.x) {
            result.x = element.x;
        }
        if (element.y) {
            result.y = element.y;
        }
    }
    if (element.offsetWidth && element.offsetHeight) {
        result.width = element.offsetWidth;
        result.height = element.offsetHeight;
    }
    else if (element.style && element.style.pixelWidth && element.style.pixelHeight) {
        result.width = element.style.pixelWidth;
        result.height = element.style.pixelHeight;
    }
    return result;
}

function $setElementX(element, x) {
    if (element && element.style) {
        element.style.left = x + "px";
    }
}
function $setElementY(element, y) {
    if (element && element.style) {
        element.style.top = y + "px";
    }
}

var ToolTip = function(target, className, text) {        
    this.target = document.getElementById(target);
    this.className = className;
    this.text = text;
    
    this.OFFSET_X = 10;
    this.OFFSET_Y = 20;
    
    this.pnl = null;
    this.ensurePanel(target);
}

ToolTip.prototype = {

    ensurePanel: function(specificTarget) {
        // NOTE:
        // Internet explorer complains that dynamically adding elements
        // should not be done before the DOM tree has been completely built.
        // We therefore wait for that and create our div
        var setPanelDelegate = $bindMethod(this, this.setPanel);
        var target = specificTarget;
        var className = this.className;

        var makeDiv = function() {
            var pnl = document.createElement('div');
            document.body.appendChild(pnl);

            this.pnl = pnl;
            this.pnl.id = target.id + '_tip';
            this.pnl.className = className;
            this.pnl.style.position = 'absolute';
            this.pnl.style.display = 'none';

            setPanelDelegate(pnl);
        }

        makeDiv();
    },

    setPanel: function(pnl) {
        this.pnl = pnl;
        this.attachHandlers();
    },

    attachHandlers: function() {
        if (this.target) {
            this.target.onmouseover = $bindMethod(this, this.showTip);
            this.target.onmouseout = $bindMethod(this, this.hideTip);
        }
    },

    showTip: function() {
        var pos = $getElementPosition(this.target);
        var x = pos.x;
        var y = pos.y;

        x += (pos.width + this.OFFSET_X);
        y -= (this.OFFSET_Y);

        this.pnl.style.display = '';
        this.pnl.style.visibility = 'visible';
        this.pnl.innerHTML = HtmlDecode(this.text);
        $setElementX(this.pnl, x);
        $setElementY(this.pnl, y);
    },

    hideTip: function() {
        this.pnl.style.display = 'none';
        this.pnl.style.innerHTML = '';
        this.pnl.style.visibility = 'hidden';
    }

}

