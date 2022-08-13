var EventsHelper = (function () {
    function EventsHelper() {
        var _this = this;
        this.events = [];
        this.bind = function (name, fn) {
            _this.events[name] = [];
            var id = Math.random().toString(36).replace(/[^a-z0-9]+/g, '').substring(0, 8);
            _this.events[name][id] = {
                id: id,
                callback: fn
            };
            return id;
        };
        this.call = function (name, param) {
            if (param === void 0) { param = null; }
            if (_this.events[name] != null) {
                for (var x in _this.events[name]) {
                    console.debug(name, param);
                    _this.events[name][x].callback(param);
                }
            }
        };
        this.on = function (name) {
            if (_this.events[name] != null) {
                return true;
            }
            else {
                return false;
            }
            ;
        };
        this.unbind = function (name, id) {
            if (_this.events[name] != null) {
                if (_this.events[name][id] != null) {
                    delete (_this.events[name][id]);
                }
            }
        };
    }
    return EventsHelper;
}());
