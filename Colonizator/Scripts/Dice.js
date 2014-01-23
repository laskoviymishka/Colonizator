Element.prototype.hasClassName = function(a) {
    return new RegExp("(?:^|\\s+)" + a + "(?:\\s+|$)").test(this.className);
};

Element.prototype.addClassName = function(a) {
    if (!this.hasClassName(a)) {
        this.className = [this.className, a].join(" ");
    }
};

Element.prototype.removeClassName = function(b) {
    if (this.hasClassName(b)) {
        var a = this.className;
        this.className = a.replace(new RegExp("(?:^|\\s+)" + b + "(?:\\s+|$)", "g"), " ");
    }
};

Element.prototype.toggleClassName = function(a) {
    this[this.hasClassName(a) ? "removeClassName" : "addClassName"](a);
};

(function() {


    var DDD = {};
    DDD.isTangible = !!('createTouch' in document);
    DDD.CursorStartEvent = DDD.isTangible ? 'touchstart' : 'mousedown';
    DDD.CursorMoveEvent = DDD.isTangible ? 'touchmove' : 'mousemove';
    DDD.CursorEndEvent = DDD.isTangible ? 'touchend' : 'mouseup';

    var transformProp = Modernizr.prefixed('transform');

    /* ==================== EventHandler ==================== */

    DDD.EventHandler = function() {};

    DDD.EventHandler.prototype.handleEvent = function(event) {
        if (this[event.type]) {
            this[event.type](event);
        }
    };


    /* ==================== RangeDisplay ==================== */

    DDD.RangeDisplay = function(range) {
        this.range = range;
        this.output = document.createElement('span');
        this.output.addClassName('range-display');


        this.units = this.range.getAttribute('data-units') || '';

        this.change();


        this.range.parentNode.appendChild(this.output);

        this.range.addEventListener('change', this, false);
    };

    DDD.RangeDisplay.prototype = new DDD.EventHandler();

    DDD.RangeDisplay.prototype.change = function(event) {
        this.output.textContent = this.range.value + this.units;
    };


    /* ==================== ProxyRange ==================== */

    DDD.ProxyRange = function(input) {
        this.input = input;

        this.slider = document.createElement('div');
        this.slider.addClassName('proxy-range');

        this.handle = document.createElement('div');
        this.handle.addClassName('handle');
        this.slider.appendChild(this.handle);


        this.width = parseInt(getComputedStyle(this.input).width, 10);
        this.slider.style.width = this.width + 'px';

        this.min = parseInt(this.input.getAttribute('min') || 0, 10);
        this.max = parseInt(this.input.getAttribute('max') || 100, 10);

        this.normalizeRatio = (this.max - this.min) / (this.width - DDD.ProxyRange.lineCap * 2);

        this.value = this.input.value;

        this.resetHandlePosition();

        this.slider.addEventListener(DDD.CursorStartEvent, this, false);
        this.handle.addEventListener(DDD.CursorStartEvent, this, false);

        this.input.parentNode.insertBefore(this.slider, this.input.nextSibling);
        this.input.style.display = 'none';

        this.x = this.slider.offsetLeft;

    };

    DDD.ProxyRange.lineCap = 15;

    DDD.ProxyRange.prototype = new DDD.EventHandler();

    DDD.ProxyRange.prototype.moveHandle = function(event) {
        var cursor = DDD.isTangible ? event.touches[0] : event,
            x = cursor.pageX - this.x;
        x = Math.max(DDD.ProxyRange.lineCap, Math.min(this.width - DDD.ProxyRange.lineCap, x));

        this.positionHandle(x);

        this.value = Math.round((x - DDD.ProxyRange.lineCap) * this.normalizeRatio + this.min);

        if (this.input.value != this.value) {
            this.input.value = this.value;

            var evt = document.createEvent("Event");
            evt.initEvent("change", true, true);
            this.input.dispatchEvent(evt);
        }

    };

    DDD.ProxyRange.prototype.positionHandle = function(x) {
        this.handle.style[transformProp] = DDD.translate(x, 0);
    };

    DDD.ProxyRange.prototype.resetHandlePosition = function() {
        var x = (this.value - this.min) / this.normalizeRatio + DDD.ProxyRange.lineCap;
        this.positionHandle(x);
    };


    DDD.ProxyRange.prototype[DDD.CursorStartEvent] = function(event) {
        this.slider.addClassName('highlighted');

        this.moveHandle(event);

        window.addEventListener(DDD.CursorMoveEvent, this, false);
        window.addEventListener(DDD.CursorEndEvent, this, false);

        event.preventDefault();
    };

    DDD.ProxyRange.prototype[DDD.CursorMoveEvent] = function(event) {

        this.moveHandle(event);

        event.preventDefault();
    };

    DDD.ProxyRange.prototype[DDD.CursorEndEvent] = function(event) {

        this.slider.removeClassName('highlighted');

        window.removeEventListener(DDD.CursorMoveEvent, this, false);
        window.removeEventListener(DDD.CursorEndEvent, this, false);
    };


    /* ==================== Test 3D transform support ==================== */

    DDD.translate = Modernizr.csstransforms3d ?
        function(x, y) {
            return 'translate3d(' + x + 'px, ' + y + 'px, 0 )';
        } :
        function(x, y) {
            return 'translate(' + x + 'px, ' + y + 'px)';
        };


    /* ==================== Start Up ==================== */


    DDD.init = function() {

        var ranges = document.querySelectorAll('input[type="range"]'),
            rangesLen = ranges.length,
            i;

        if (rangesLen) {

            for (i = 0; i < rangesLen; i++) {
                new DDD.RangeDisplay(ranges[i]);
            }

            var isRangeSupported = (function() {
                var isSupported = ranges[0].type !== 'text';
                if (isSupported) {
                    var appearanceProp = Modernizr.prefixed('appearance');
                    isSupported = getComputedStyle(ranges[0])[appearanceProp] !== 'textfield';
                }
                return isSupported;
            })();

            if (!isRangeSupported) {
                for (i = 0; i < rangesLen; i++) {
                    new DDD.ProxyRange(ranges[i]);
                }
            }

        }

    };


    window.addEventListener('DOMContentLoaded', DDD.init, false);

    window.DDD = DDD;

})();
var panelClas = ["show-front", "show-front"];
var togleCube = function(number, random) {
    var selector = '#cubecontainer' + number;
    var box = document.querySelector(selector).children[0];
    switch (random) {
    case 1:
        box.removeClassName(panelClas[number]);
        panelClas[number] = "show-front";
        box.addClassName(panelClas[number]);
        break;
    case 2:
        box.removeClassName(panelClas[number]);
        panelClas[number] = "show-back";
        box.addClassName(panelClas[number]);
        break;
    case 3:
        box.removeClassName(panelClas[number]);
        panelClas[number] = "show-right";
        box.addClassName(panelClas[number]);
        break;
    case 4:
        box.removeClassName(panelClas[number]);
        panelClas[number] = "show-left";
        box.addClassName(panelClas[number]);
        break;
    case 5:
        box.removeClassName(panelClas[number]);
        panelClas[number] = "show-top";
        box.addClassName(panelClas[number]);
        break;
    case 6:
        box.removeClassName(panelClas[number]);
        panelClas[number] = "show-bottom";
        box.addClassName(panelClas[number]);
        break;
    }
};