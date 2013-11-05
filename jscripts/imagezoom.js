(function ($) {
    function CloudZoom(a, b) {
        var c = $("img", a);
        var d;
        var e;
        var f = null;
        var g = null;
        var h = null;
        var i = null;
        var j = null;
        var k = null;
        var l;
        var m = 0;
        var n, o;
        var p = 0;
        var q = 0;
        var r = 0;
        var s = 0;
        var t = 0;
        var u, v;
        var w = this,
            x;
        setTimeout(function () {
            if (g === null) {
                var b = a.width();
                a.parent().append(format('<div style="width:%0px;position:absolute;top:75%;left:%1px;text-align:center" class="cloud-zoom-loading" >Loading...</div>', b / 3, b / 2 - b / 6)).find(":last").css("opacity", .5)
            }
        }, 200);
        var y = function () {
            if (k !== null) {
                k.remove();
                k = null
            }
        };
        this.removeBits = function () {
            if (h) {
                h.remove();
                h = null
            }
            if (i) {
                i.remove();
                i = null
            }
            if (j) {
                j.remove();
                j = null
            }
            y();
            $(".cloud-zoom-loading", a.parent()).remove()
        };
        this.destroy = function () {
            a.data("zoom", null);
            if (g) {
                g.unbind();
                g.remove();
                g = null
            }
            if (f) {
                f.remove();
                f = null
            }
            this.removeBits()
        };
        this.fadedOut = function () {
            if (f) {
                f.remove();
                f = null
            }
            this.removeBits()
        };
        this.controlLoop = function () {
            if (h) {
                var a = u - c.offset().left - n * .5 >> 0;
                var d = v - c.offset().top - o * .5 >> 0;
                if (a < 0) {
                    a = 0
                } else if (a > c.outerWidth() - n) {
                    a = c.outerWidth() - n
                }
                if (d < 0) {
                    d = 0
                } else if (d > c.outerHeight() - o) {
                    d = c.outerHeight() - o
                }
                h.css({
                    left: a,
                    top: d
                });
                h.css("background-position", -a + "px " + -d + "px");
                p = a / c.outerWidth() * l.width >> 0;
                q = d / c.outerHeight() * l.height >> 0;
                s += (p - s) / b.smoothMove;
                r += (q - r) / b.smoothMove;
                f.css("background-position", -(s >> 0) + "px " + (-(r >> 0) + "px"))
            }
            m = setTimeout(function () {
                w.controlLoop()
            }, 30)
        };
        this.init2 = function (a, b) {
            t++;
            if (b === 1) {
                l = a
            }
            if (t === 2) {
                this.init()
            }
        };
        this.init = function () {
            $(".cloud-zoom-loading", a.parent()).remove();
            g = a.parent().append(format("<div class='mousetrap' style='background-image:url(\".\");z-index:999;position:absolute;width:%0px;height:%1px;left:%2px;top:%3px;'></div>", c.outerWidth(), c.outerHeight(), 0, 0)).find(":last");
            g.bind("mousemove", this, function (a) {
                u = a.pageX;
                v = a.pageY
            });
            g.bind("mouseleave", this, function (a) {
                clearTimeout(m);
                if (h) {
                    h.fadeOut(299)
                }
                if (i) {
                    i.fadeOut(299)
                }
                if (j) {
                    j.fadeOut(299)
                }
                f.fadeOut(300, function () {
                    w.fadedOut()
                });
                return false
            });
            g.bind("mouseenter", this, function (d) {
                u = d.pageX;
                v = d.pageY;
                x = d.data;
                if (f) {
                    f.stop(true, false);
                    f.remove()
                }
                var e = b.adjustX,
                    m = b.adjustY;
                var p = c.outerWidth();
                var q = c.outerHeight();
                var r = b.zoomWidth;
                var s = b.zoomHeight;
                if (b.zoomWidth == "auto") {
                    r = p
                }
                if (b.zoomHeight == "auto") {
                    s = q
                }
                var t = a.parent();
                switch (b.position) {
                    case "top":
                        m -= s;
                        break;
                    case "right":
                        e += p;
                        break;
                    case "bottom":
                        m += q;
                        break;
                    case "left":
                        e -= r;
                        break;
                    case "inside":
                        r = p;
                        s = q;
                        break;
                    default:
                        t = $("#" + b.position);
                        if (!t.length) {
                            t = a;
                            e += p;
                            m += q
                        } else {
                            r = t.innerWidth();
                            s = t.innerHeight()
                        }
                }
                f = t.append(format('<div id="cloud-zoom-big" class="cloud-zoom-big" style="display:none;position:absolute;left:%0px;top:%1px;width:%2px;height:%3px;background-image:url(\'%4\');z-index:99;"></div>', e, m, r, s, l.src)).find(":last");
                if (c.attr("title") && b.showTitle) {
                    f.append(format('<div class="cloud-zoom-title">%0</div>', c.attr("title"))).find(":last").css("opacity", b.titleOpacity)
                }
                if ($.browser.msie && $.browser.version < 7) {
                    k = $('<iframe frameborder="0" src="#"></iframe>').css({
                        position: "absolute",
                        left: e,
                        top: m,
                        zIndex: 99,
                        width: r,
                        height: s
                    }).insertBefore(f)
                }
                f.fadeIn(500);
                if (h) {
                    h.remove();
                    h = null
                }
                n = c.outerWidth() / l.width * f.width();
                o = c.outerHeight() / l.height * f.height();
                h = a.append(format("<div class = 'cloud-zoom-lens' style='display:none;z-index:98;position:absolute;width:%0px;height:%1px;'></div>", n, o)).find(":last");
                g.css("cursor", h.css("cursor"));
                var w = false;
                if (b.tint) {
                    h.css("background", 'url("' + c.attr("src") + '")');
                    i = a.append(format('<div style="display:none;position:absolute; left:0px; top:0px; width:%0px; height:%1px; background-color:%2;" />', c.outerWidth(), c.outerHeight(), b.tint)).find(":last");
                    i.css("opacity", b.tintOpacity);
                    w = true;
                    i.fadeIn(500)
                }
                if (b.softFocus) {
                    h.css("background", 'url("' + c.attr("src") + '")');
                    j = a.append(format('<div style="position:absolute;display:none;top:2px; left:2px; width:%0px; height:%1px;" />', c.outerWidth() - 2, c.outerHeight() - 2, b.tint)).find(":last");
                    j.css("background", 'url("' + c.attr("src") + '")');
                    j.css("opacity", .5);
                    w = true;
                    j.fadeIn(500)
                }
                if (!w) {
                    h.css("opacity", b.lensOpacity)
                }
                if (b.position !== "inside") {
                    h.fadeIn(500)
                }
                x.controlLoop();
                return
            })
        };
        d = new Image;
        $(d).load(function () {
            w.init2(this, 0)
        });
        d.src = c.attr("src");
        e = new Image;
        $(e).load(function () {
            w.init2(this, 1)
        });
        e.src = a.attr("href")
    }
    function format(a) {
        for (var b = 1; b < arguments.length; b++) {
            a = a.replace("%" + (b - 1), arguments[b])
        }
        return a
    }
    $.fn.CloudZoom = function (options) {
        try {
            document.execCommand("BackgroundImageCache", false, true)
        } catch (e) { }
        this.each(function () {
            var relOpts, opts;

            if ($(this).attr("rel") == null || $(this).attr("rel") == 'example_group') {
                return;
            }

            eval("var\ta = {" + $(this).attr("rel") + "}");
            relOpts = a;
            if ($(this).is(".cloud-zoom")) {
                $(this).css({
                    position: "relative",
                    display: "block"
                });
                $("img", $(this)).css({
                    display: "block"
                });
                if ($(this).parent().attr("id") != "wrap") {
                    $(this).wrap('<div id="wrap" style="top:0px;z-index:0;position:relative;"></div>')
                }
                opts = $.extend({}, $.fn.CloudZoom.defaults, options);
                opts = $.extend({}, opts, relOpts);
                $(this).data("zoom", new CloudZoom($(this), opts))
            } else if ($(this).is(".cloud-zoom-gallery")) {
                opts = $.extend({}, relOpts, options);
                $(this).data("relOpts", opts);
                $(this).bind("click", $(this), function (a) {
                    var b = a.data.data("relOpts");
                    $("#" + b.useZoom).data("zoom").destroy();
                    $("#" + b.useZoom).attr("href", a.data.attr("href"));
                    $("#" + b.useZoom + " img").attr("src", a.data.data("relOpts").smallImage);
                    $("#" + a.data.data("relOpts").useZoom).CloudZoom();
                    return false
                })
            }
        });
        return this
    };
    $.fn.CloudZoom.defaults = {
        zoomWidth: "auto",
        zoomHeight: "auto",
        position: "right",
        tint: false,
        tintOpacity: .5,
        lensOpacity: .5,
        softFocus: false,
        smoothMove: 3,
        showTitle: true,
        titleOpacity: .5,
        adjustX: 0,
        adjustY: 0
    }
})(jQuery);
(function (a) {
    var b, c, d, e, f, g, h, i, j, k, l = 0,
        m = {},
        n = [],
        o = 0,
        p = {},
        q = [],
        r = null,
        s = new Image,
        t = /\.(jpg|gif|png|bmp|jpeg)(.*)?$/i,
        u = /[^\.]\.(swf)\s*$/i,
        v, w = 1,
        x = 0,
        y = "",
        z, A, B = false,
        C = a.extend(a("<div/>")[0], {
            prop: 0
        }),
        D = a.browser.msie && a.browser.version < 7 && !window.XMLHttpRequest,
        E = function () {
            c.hide();
            s.onerror = s.onload = null;
            if (r) {
                r.abort()
            }
            b.empty()
        },
        F = function () {
            if (false === m.onError(n, l, m)) {
                c.hide();
                B = false;
                return
            }
            m.titleShow = false;
            m.width = "auto";
            m.height = "auto";
            b.html('<p id="fancybox-error">The requested content cannot be loaded.<br />Please try again later.</p>');
            H()
        },
        G = function () {
            var d = n[l],
                e, f, h, i, j, k;
            E();
            m = a.extend({}, a.fn.fancybox.defaults, typeof a(d).data("fancybox") == "undefined" ? m : a(d).data("fancybox"));
            k = m.onStart(n, l, m);
            if (k === false) {
                B = false;
                return
            } else if (typeof k == "object") {
                m = a.extend(m, k)
            }
            h = m.title || (d.nodeName ? a(d).attr("title") : d.title) || "";
            if (d.nodeName && !m.orig) {
                m.orig = a(d).children("img:first").length ? a(d).children("img:first") : a(d)
            }
            if (h === "" && m.orig && m.titleFromAlt) {
                h = m.orig.attr("alt")
            }
            e = m.href || (d.nodeName ? a(d).attr("href") : d.href) || null;
            if (/^(?:javascript)/i.test(e) || e == "#") {
                e = null
            }
            if (m.type) {
                f = m.type;
                if (!e) {
                    e = m.content
                }
            } else if (m.content) {
                f = "html"
            } else if (e) {
                if (e.match(t)) {
                    f = "image"
                } else if (e.match(u)) {
                    f = "swf"
                } else if (a(d).hasClass("iframe")) {
                    f = "iframe"
                } else if (e.indexOf("#") === 0) {
                    f = "inline"
                } else {
                    f = "ajax"
                }
            }
            if (!f) {
                F();
                return
            }
            if (f == "inline") {
                d = e.substr(e.indexOf("#"));
                f = a(d).length > 0 ? "inline" : "ajax"
            }
            m.type = f;
            m.href = e;
            m.title = h;
            if (m.autoDimensions) {
                if (m.type == "html" || m.type == "inline" || m.type == "ajax") {
                    m.width = "auto";
                    m.height = "auto"
                } else {
                    m.autoDimensions = false
                }
            }
            if (m.modal) {
                m.overlayShow = true;
                m.hideOnOverlayClick = false;
                m.hideOnContentClick = false;
                m.enableEscapeButton = false;
                m.showCloseButton = false
            }
            m.padding = parseInt(m.padding, 10);
            m.margin = parseInt(m.margin, 10);
            b.css("padding", m.padding + m.margin);
            a(".fancybox-inline-tmp").unbind("fancybox-cancel").bind("fancybox-change", function () {
                a(this).replaceWith(g.children())
            });
            switch (f) {
                case "html":
                    b.html(m.content);
                    H();
                    break;
                case "inline":
                    if (a(d).parent().is("#fancybox-content") === true) {
                        B = false;
                        return
                    }
                    a('<div class="fancybox-inline-tmp" />').hide().insertBefore(a(d)).bind("fancybox-cleanup", function () {
                        a(this).replaceWith(g.children())
                    }).bind("fancybox-cancel", function () {
                        a(this).replaceWith(b.children())
                    });
                    a(d).appendTo(b);
                    H();
                    break;
                case "image":
                    B = false;
                    a.fancybox.showActivity();
                    s = new Image;
                    s.onerror = function () {
                        F()
                    };
                    s.onload = function () {
                        B = true;
                        s.onerror = s.onload = null;
                        I()
                    };
                    s.src = e;
                    break;
                case "swf":
                    m.scrolling = "no";
                    i = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" width="' + m.width + '" height="' + m.height + '"><param name="movie" value="' + e + '"></param>';
                    j = "";
                    a.each(m.swf, function (a, b) {
                        i += '<param name="' + a + '" value="' + b + '"></param>';
                        j += " " + a + '="' + b + '"'
                    });
                    i += '<embed src="' + e + '" type="application/x-shockwave-flash" width="' + m.width + '" height="' + m.height + '"' + j + "></embed></object>";
                    b.html(i);
                    H();
                    break;
                case "ajax":
                    B = false;
                    a.fancybox.showActivity();
                    m.ajax.win = m.ajax.success;
                    r = a.ajax(a.extend({}, m.ajax, {
                        url: e,
                        data: m.ajax.data || {},
                        error: function (a, b, c) {
                            if (a.status > 0) {
                                F()
                            }
                        },
                        success: function (a, d, f) {
                            var g = typeof f == "object" ? f : r;
                            if (g.status == 200) {
                                if (typeof m.ajax.win == "function") {
                                    k = m.ajax.win(e, a, d, f);
                                    if (k === false) {
                                        c.hide();
                                        return
                                    } else if (typeof k == "string" || typeof k == "object") {
                                        a = k
                                    }
                                }
                                b.html(a);
                                H()
                            }
                        }
                    }));
                    break;
                case "iframe":
                    J();
                    break
            }
        },
        H = function () {
            var c = m.width,
                d = m.height;
            if (c.toString().indexOf("%") > -1) {
                c = parseInt((a(window).width() - m.margin * 2) * parseFloat(c) / 100, 10) + "px"
            } else {
                c = c == "auto" ? "auto" : c + "px"
            }
            if (d.toString().indexOf("%") > -1) {
                d = parseInt((a(window).height() - m.margin * 2) * parseFloat(d) / 100, 10) + "px"
            } else {
                d = d == "auto" ? "auto" : d + "px"
            }
            b.wrapInner('<div style="width:' + c + ";height:" + d + ";overflow: " + (m.scrolling == "auto" ? "auto" : m.scrolling == "yes" ? "scroll" : "hidden") + ';position:relative;"></div>');
            m.width = b.width();
            m.height = b.height();
            J()
        },
        I = function () {
            m.width = s.width;
            m.height = s.height;
            a("<img />").attr({
                id: "fancybox-img",
                src: s.src,
                alt: m.title
            }).appendTo(b);
            J()
        },
        J = function () {
            var f, r;
            c.hide();
            if (e.is(":visible") && false === p.onCleanup(q, o, p)) {
                a.event.trigger("fancybox-cancel");
                B = false;
                return
            }
            B = true;
            a(g.add(d)).unbind();
            a(window).unbind("resize.fb scroll.fb");
            a(document).unbind("keydown.fb");
            if (e.is(":visible") && p.titlePosition !== "outside") {
                e.css("height", e.height())
            }
            q = n;
            o = l;
            p = m;
            if (p.overlayShow) {
                d.css({
                    "background-color": p.overlayColor,
                    opacity: p.overlayOpacity,
                    cursor: p.hideOnOverlayClick ? "pointer" : "auto",
                    height: a(document).height()
                });
                if (!d.is(":visible")) {
                    if (D) {
                        a("select:not(#fancybox-tmp select)").filter(function () {
                            return this.style.visibility !== "hidden"
                        }).css({
                            visibility: "hidden"
                        }).one("fancybox-cleanup", function () {
                            this.style.visibility = "inherit"
                        })
                    }
                    d.show()
                }
            } else {
                d.hide()
            }
            A = R();
            L();
            if (e.is(":visible")) {
                a(h.add(j).add(k)).hide();
                f = e.position(), z = {
                    top: f.top,
                    left: f.left,
                    width: e.width(),
                    height: e.height()
                };
                r = z.width == A.width && z.height == A.height;
                g.fadeTo(p.changeFade, .3, function () {
                    var c = function () {
                        g.html(b.contents()).fadeTo(p.changeFade, 1, N)
                    };
                    a.event.trigger("fancybox-change");
                    g.empty().removeAttr("filter").css({
                        "border-width": p.padding,
                        width: A.width - p.padding * 2,
                        height: m.autoDimensions ? "auto" : A.height - x - p.padding * 2
                    });
                    if (r) {
                        c()
                    } else {
                        C.prop = 0;
                        a(C).animate({
                            prop: 1
                        }, {
                            duration: p.changeSpeed,
                            easing: p.easingChange,
                            step: P,
                            complete: c
                        })
                    }
                });
                return
            }
            e.removeAttr("style");
            g.css("border-width", p.padding);
            if (p.transitionIn == "elastic") {
                z = T();
                g.html(b.contents());
                e.show();
                if (p.opacity) {
                    A.opacity = 0
                }
                C.prop = 0;
                a(C).animate({
                    prop: 1
                }, {
                    duration: p.speedIn,
                    easing: p.easingIn,
                    step: P,
                    complete: N
                });
                return
            }
            if (p.titlePosition == "inside" && x > 0) {
                i.show()
            }
            g.css({
                width: A.width - p.padding * 2,
                height: m.autoDimensions ? "auto" : A.height - x - p.padding * 2
            }).html(b.contents());
            e.css(A).fadeIn(p.transitionIn == "none" ? 0 : p.speedIn, N)
        },
        K = function (a) {
            if (a && a.length) {
                if (p.titlePosition == "float") {
                    return '<table id="fancybox-title-float-wrap" cellpadding="0" cellspacing="0"><tr><td id="fancybox-title-float-left"></td><td id="fancybox-title-float-main">' + a + '</td><td id="fancybox-title-float-right"></td></tr></table>'
                }
                return '<div id="fancybox-title-' + p.titlePosition + '">' + a + "</div>"
            }
            return false
        },
        L = function () {
            y = p.title || "";
            x = 0;
            i.empty().removeAttr("style").removeClass();
            if (p.titleShow === false) {
                i.hide();
                return
            }
            y = a.isFunction(p.titleFormat) ? p.titleFormat(y, q, o, p) : K(y);
            if (!y || y === "") {
                i.hide();
                return
            }
            i.addClass("fancybox-title-" + p.titlePosition).html(y).appendTo("body").show();
            switch (p.titlePosition) {
                case "inside":
                    i.css({
                        width: A.width - p.padding * 2,
                        marginLeft: p.padding,
                        marginRight: p.padding
                    });
                    x = i.outerHeight(true);
                    i.appendTo(f);
                    A.height += x;
                    break;
                case "over":
                    i.css({
                        marginLeft: p.padding,
                        width: A.width - p.padding * 2,
                        bottom: p.padding
                    }).appendTo(f);
                    break;
                case "float":
                    i.css("left", parseInt((i.width() - A.width - 40) / 2, 10) * -1).appendTo(e);
                    break;
                default:
                    i.css({
                        width: A.width - p.padding * 2,
                        paddingLeft: p.padding,
                        paddingRight: p.padding
                    }).appendTo(e);
                    break
            }
            i.hide()
        },
        M = function () {
            if (p.enableEscapeButton || p.enableKeyboardNav) {
                a(document).bind("keydown.fb", function (b) {
                    if (b.keyCode == 27 && p.enableEscapeButton) {
                        b.preventDefault();
                        a.fancybox.close()
                    } else if ((b.keyCode == 37 || b.keyCode == 39) && p.enableKeyboardNav && b.target.tagName !== "INPUT" && b.target.tagName !== "TEXTAREA" && b.target.tagName !== "SELECT") {
                        b.preventDefault();
                        a.fancybox[b.keyCode == 37 ? "prev" : "next"]()
                    }
                })
            }
            if (!p.showNavArrows) {
                j.hide();
                k.hide();
                return
            }
            if (p.cyclic && q.length > 1 || o !== 0) {
                j.show()
            }
            if (p.cyclic && q.length > 1 || o != q.length - 1) {
                k.show()
            }
        },
        N = function () {
            if (!a.support.opacity) {
                g.get(0).style.removeAttribute("filter");
                e.get(0).style.removeAttribute("filter")
            }
            if (m.autoDimensions) {
                g.css("height", "auto")
            }
            e.css("height", "auto");
            if (y && y.length) {
                i.show()
            }
            if (p.showCloseButton) {
                h.show()
            }
            M();
            if (p.hideOnContentClick) {
                g.bind("click", a.fancybox.close)
            }
            if (p.hideOnOverlayClick) {
                d.bind("click", a.fancybox.close)
            }
            a(window).bind("resize.fb", a.fancybox.resize);
            if (p.centerOnScroll) {
                a(window).bind("scroll.fb", a.fancybox.center)
            }
            if (p.type == "iframe") {
                a('<iframe id="fancybox-frame" name="fancybox-frame' + (new Date).getTime() + '" frameborder="0" hspace="0" ' + (a.browser.msie ? 'allowtransparency="true""' : "") + ' scrolling="' + m.scrolling + '" src="' + p.href + '"></iframe>').appendTo(g)
            }
            e.show();
            B = false;
            a.fancybox.center();
            p.onComplete(q, o, p);
            O()
        },
        O = function () {
            var a, b;
            if (q.length - 1 > o) {
                a = q[o + 1].href;
                if (typeof a !== "undefined" && a.match(t)) {
                    b = new Image;
                    b.src = a
                }
            }
            if (o > 0) {
                a = q[o - 1].href;
                if (typeof a !== "undefined" && a.match(t)) {
                    b = new Image;
                    b.src = a
                }
            }
        },
        P = function (a) {
            var b = {
                width: parseInt(z.width + (A.width - z.width) * a, 10),
                height: parseInt(z.height + (A.height - z.height) * a, 10),
                top: parseInt(z.top + (A.top - z.top) * a, 10),
                left: parseInt(z.left + (A.left - z.left) * a, 10)
            };
            if (typeof A.opacity !== "undefined") {
                b.opacity = a < .5 ? .5 : a
            }
            e.css(b);
            g.css({
                width: b.width - p.padding * 2,
                height: b.height - x * a - p.padding * 2
            })
        },
        Q = function () {
            return [a(window).width() - p.margin * 2, a(window).height() - p.margin * 2, a(document).scrollLeft() + p.margin, a(document).scrollTop() + p.margin]
        },
        R = function () {
            var a = Q(),
                b = {},
                c = p.autoScale,
                d = p.padding * 2,
                e;
            if (p.width.toString().indexOf("%") > -1) {
                b.width = parseInt(a[0] * parseFloat(p.width) / 100, 10)
            } else {
                b.width = p.width + d
            }
            if (p.height.toString().indexOf("%") > -1) {
                b.height = parseInt(a[1] * parseFloat(p.height) / 100, 10)
            } else {
                b.height = p.height + d
            }
            if (c && (b.width > a[0] || b.height > a[1])) {
                if (m.type == "image" || m.type == "swf") {
                    e = p.width / p.height;
                    if (b.width > a[0]) {
                        b.width = a[0];
                        b.height = parseInt((b.width - d) / e + d, 10)
                    }
                    if (b.height > a[1]) {
                        b.height = a[1];
                        b.width = parseInt((b.height - d) * e + d, 10)
                    }
                } else {
                    b.width = Math.min(b.width, a[0]);
                    b.height = Math.min(b.height, a[1])
                }
            }
            b.top = parseInt(Math.max(a[3] - 20, a[3] + (a[1] - b.height - 40) * .5), 10);
            b.left = parseInt(Math.max(a[2] - 20, a[2] + (a[0] - b.width - 40) * .5), 10);
            return b
        },
        S = function (a) {
            var b = a.offset();
            b.top += parseInt(a.css("paddingTop"), 10) || 0;
            b.left += parseInt(a.css("paddingLeft"), 10) || 0;
            b.top += parseInt(a.css("border-top-width"), 10) || 0;
            b.left += parseInt(a.css("border-left-width"), 10) || 0;
            b.width = a.width();
            b.height = a.height();
            return b
        },
        T = function () {
            var b = m.orig ? a(m.orig) : false,
                c = {},
                d, e;
            if (b && b.length) {
                d = S(b);
                c = {
                    width: d.width + p.padding * 2,
                    height: d.height + p.padding * 2,
                    top: d.top - p.padding - 20,
                    left: d.left - p.padding - 20
                }
            } else {
                e = Q();
                c = {
                    width: p.padding * 2,
                    height: p.padding * 2,
                    top: parseInt(e[3] + e[1] * .5, 10),
                    left: parseInt(e[2] + e[0] * .5, 10)
                }
            }
            return c
        },
        U = function () {
            if (!c.is(":visible")) {
                clearInterval(v);
                return
            }
            a("div", c).css("top", w * -40 + "px");
            w = (w + 1) % 12
        };
    a.fn.fancybox = function (b) {
        if (!a(this).length) {
            return this
        }
        a(this).data("fancybox", a.extend({}, b, a.metadata ? a(this).metadata() : {})).unbind("click.fb").bind("click.fb", function (b) {
            b.preventDefault();
            if (B) {
                return
            }
            B = true;
            a(this).blur();
            n = [];
            l = 0;
            var c = a(this).attr("rel") || "";
            if (!c || c == "" || c === "nofollow") {
                n.push(this)
            } else {
                n = a("a[rel=" + c + "], area[rel=" + c + "]");
                l = n.index(this)
            }
            G();
            return
        });
        return this
    };
    a.fancybox = function (b) {
        var c;
        if (B) {
            return
        }
        B = true;
        c = typeof arguments[1] !== "undefined" ? arguments[1] : {};
        n = [];
        l = parseInt(c.index, 10) || 0;
        if (a.isArray(b)) {
            for (var d = 0, e = b.length; d < e; d++) {
                if (typeof b[d] == "object") {
                    a(b[d]).data("fancybox", a.extend({}, c, b[d]))
                } else {
                    b[d] = a({}).data("fancybox", a.extend({
                        content: b[d]
                    }, c))
                }
            }
            n = jQuery.merge(n, b)
        } else {
            if (typeof b == "object") {
                a(b).data("fancybox", a.extend({}, c, b))
            } else {
                b = a({}).data("fancybox", a.extend({
                    content: b
                }, c))
            }
            n.push(b)
        }
        if (l > n.length || l < 0) {
            l = 0
        }
        G()
    };
    a.fancybox.showActivity = function () {
        clearInterval(v);
        c.show();
        v = setInterval(U, 66)
    };
    a.fancybox.hideActivity = function () {
        c.hide()
    };
    a.fancybox.next = function () {
        return a.fancybox.pos(o + 1)
    };
    a.fancybox.prev = function () {
        return a.fancybox.pos(o - 1)
    };
    a.fancybox.pos = function (a) {
        if (B) {
            return
        }
        a = parseInt(a);
        n = q;
        if (a > -1 && a < q.length) {
            l = a;
            G()
        } else if (p.cyclic && q.length > 1) {
            l = a >= q.length ? 0 : q.length - 1;
            G()
        }
        return
    };
    a.fancybox.cancel = function () {
        if (B) {
            return
        }
        B = true;
        a.event.trigger("fancybox-cancel");
        E();
        m.onCancel(n, l, m);
        B = false
    };
    a.fancybox.close = function () {
        function b() {
            d.fadeOut("fast");
            i.empty().hide();
            e.hide();
            a.event.trigger("fancybox-cleanup");
            g.empty();
            p.onClosed(q, o, p);
            q = m = [];
            o = l = 0;
            p = m = {};
            B = false
        }
        if (B || e.is(":hidden")) {
            return
        }
        B = true;
        if (p && false === p.onCleanup(q, o, p)) {
            B = false;
            return
        }
        E();
        a(h.add(j).add(k)).hide();
        a(g.add(d)).unbind();
        a(window).unbind("resize.fb scroll.fb");
        a(document).unbind("keydown.fb");
        g.find("iframe").attr("src", D && /^https/i.test(window.location.href || "") ? "javascript:void(false)" : "about:blank");
        if (p.titlePosition !== "inside") {
            i.empty()
        }
        e.stop();
        if (p.transitionOut == "elastic") {
            z = T();
            var c = e.position();
            A = {
                top: c.top,
                left: c.left,
                width: e.width(),
                height: e.height()
            };
            if (p.opacity) {
                A.opacity = 1
            }
            i.empty().hide();
            C.prop = 1;
            a(C).animate({
                prop: 0
            }, {
                duration: p.speedOut,
                easing: p.easingOut,
                step: P,
                complete: b
            })
        } else {
            e.fadeOut(p.transitionOut == "none" ? 0 : p.speedOut, b)
        }
    };
    a.fancybox.resize = function () {
        if (d.is(":visible")) {
            d.css("height", a(document).height())
        }
        a.fancybox.center(true)
    };
    a.fancybox.center = function () {
        var a, b;
        if (B) {
            return
        }
        b = arguments[0] === true ? 1 : 0;
        a = Q();
        if (!b && (e.width() > a[0] || e.height() > a[1])) {
            return
        }
        e.stop().animate({
            top: parseInt(Math.max(a[3] - 20, a[3] + (a[1] - g.height() - 40) * .5 - p.padding)),
            left: parseInt(Math.max(a[2] - 20, a[2] + (a[0] - g.width() - 40) * .5 - p.padding))
        }, typeof arguments[0] == "number" ? arguments[0] : 200)
    };
    a.fancybox.init = function () {
        if (a("#fancybox-wrap").length) {
            return
        }
        a("body").append(b = a('<div id="fancybox-tmp"></div>'), c = a('<div id="fancybox-loading"><div></div></div>'), d = a('<div id="fancybox-overlay"></div>'), e = a('<div id="fancybox-wrap"></div>'));
        f = a('<div id="fancybox-outer"></div>').append('<div class="fancybox-bg" id="fancybox-bg-n"></div><div class="fancybox-bg" id="fancybox-bg-ne"></div><div class="fancybox-bg" id="fancybox-bg-e"></div><div class="fancybox-bg" id="fancybox-bg-se"></div><div class="fancybox-bg" id="fancybox-bg-s"></div><div class="fancybox-bg" id="fancybox-bg-sw"></div><div class="fancybox-bg" id="fancybox-bg-w"></div><div class="fancybox-bg" id="fancybox-bg-nw"></div>').appendTo(e);
        f.append(g = a('<div id="fancybox-content"></div>'), h = a('<a id="fancybox-close"></a>'), i = a('<div id="fancybox-title"></div>'), j = a('<a href="javascript:;" id="fancybox-left"><span class="fancy-ico" id="fancybox-left-ico"></span></a>'), k = a('<a href="javascript:;" id="fancybox-right"><span class="fancy-ico" id="fancybox-right-ico"></span></a>'));
        h.click(a.fancybox.close);
        c.click(a.fancybox.cancel);
        j.click(function (b) {
            b.preventDefault();
            a.fancybox.prev()
        });
        k.click(function (b) {
            b.preventDefault();
            a.fancybox.next()
        });
        if (a.fn.mousewheel) {
            e.bind("mousewheel.fb", function (b, c) {
                if (B) {
                    b.preventDefault()
                } else if (a(b.target).get(0).clientHeight == 0 || a(b.target).get(0).scrollHeight === a(b.target).get(0).clientHeight) {
                    b.preventDefault();
                    a.fancybox[c > 0 ? "prev" : "next"]()
                }
            })
        }
        if (!a.support.opacity) {
            e.addClass("fancybox-ie")
        }
        if (D) {
            c.addClass("fancybox-ie6");
            e.addClass("fancybox-ie6");
            a('<iframe id="fancybox-hide-sel-frame" src="' + (/^https/i.test(window.location.href || "") ? "javascript:void(false)" : "about:blank") + '" scrolling="no" border="0" frameborder="0" tabindex="-1"></iframe>').prependTo(f)
        }
    };
    a.fn.fancybox.defaults = {
        padding: 10,
        margin: 40,
        opacity: false,
        modal: false,
        cyclic: false,
        scrolling: "auto",
        width: 560,
        height: 340,
        autoScale: true,
        autoDimensions: true,
        centerOnScroll: false,
        ajax: {},
        swf: {
            wmode: "transparent"
        },
        hideOnOverlayClick: true,
        hideOnContentClick: false,
        overlayShow: true,
        overlayOpacity: .7,
        overlayColor: "#777",
        titleShow: true,
        titlePosition: "float",
        titleFormat: null,
        titleFromAlt: false,
        transitionIn: "fade",
        transitionOut: "fade",
        speedIn: 300,
        speedOut: 300,
        changeSpeed: 300,
        changeFade: "fast",
        easingIn: "swing",
        easingOut: "swing",
        showCloseButton: true,
        showNavArrows: true,
        enableEscapeButton: true,
        enableKeyboardNav: true,
        onStart: function () { },
        onCancel: function () { },
        onComplete: function () { },
        onCleanup: function () { },
        onClosed: function () { },
        onError: function () { }
    };
    a(document).ready(function () {
        a.fancybox.init()
    })
})(jQuery);
(function (a) {
    function b(b) {
        var c = b || window.event,
            e = [].slice.call(arguments, 1),
            f = 0,
            g = 0,
            h = 0;
        b = a.event.fix(c);
        b.type = "mousewheel";
        if (b.wheelDelta) f = b.wheelDelta / 120;
        if (b.detail) f = -b.detail / 3;
        h = f;
        if (c.axis !== undefined && c.axis === c.HORIZONTAL_AXIS) {
            h = 0;
            g = -1 * f
        }
        if (c.wheelDeltaY !== undefined) h = c.wheelDeltaY / 120;
        if (c.wheelDeltaX !== undefined) g = -1 * c.wheelDeltaX / 120;
        e.unshift(b, f, g, h);
        return a.event.handle.apply(this, e)
    }
    var c = ["DOMMouseScroll", "mousewheel"];
    a.event.special.mousewheel = {
        setup: function () {
            if (this.addEventListener) for (var a = c.length; a; ) this.addEventListener(c[--a], b, false);
            else this.onmousewheel = b
        },
        teardown: function () {
            if (this.removeEventListener) for (var a = c.length; a; ) this.removeEventListener(c[--a], b, false);
            else this.onmousewheel = null
        }
    };
    a.fn.extend({
        mousewheel: function (a) {
            return a ? this.bind("mousewheel", a) : this.trigger("mousewheel")
        },
        unmousewheel: function (a) {
            return this.unbind("mousewheel", a)
        }
    })
})(jQuery)