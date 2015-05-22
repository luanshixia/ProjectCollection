/*
 * Util.js
 */
var dreambuild;
(function (dreambuild) {
    /**
     * Utility functions.
     */
    var Utils = (function () {
        function Utils() {
        }
        /**
         * Generates a random number between min and max.
         */
        Utils.random = function (min, max) {
            var rand = Math.random();
            return min + rand * (max - min);
        };
        /**
         * Mimic String.Format().
         */
        Utils.formatString = function (format) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            return format.replace(/{(\d+)}/g, function (match, index) {
                return typeof args[index] !== 'undefined' ? args[index] : match;
            });
        };
        /**
         * Mimic jQuery.isPlainObject().
         */
        Utils.isObject = function (value) {
            return value != null && typeof value === 'object' && !(value instanceof Date);
        };
        /**
         * Mimic jQuery.extend() for two objects.
         */
        Utils.copy = function (dst, src) {
            var key, value;
            for (key in src) {
                value = src[key];
                if (!dst._copy || !dst._copy(key, value)) {
                    if (Utils.isObject(value) && dst[key]) {
                        Utils.copy(dst[key], value); // copy sub-objects
                    }
                    else {
                        dst[key] = value; // assign values
                    }
                }
            }
            return dst;
        };
        /**
         * Mimic jQuery.extend().
         */
        Utils.extend = function (dst) {
            var src = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                src[_i - 1] = arguments[_i];
            }
            var list = src.map(function (s) { return Utils.copy({}, s); }); // don't want src elements modified, so copy them
            list.unshift(dst);
            return list.reduceRight(function (s, d) { return Utils.copy(d, s); });
        };
        /**
         * Serialize object to form data format. Different from jQuery.param() in that it serializes complex value to inner json.
         */
        Utils.formEncodedObject = function (src) {
            var arr = [], key, value;
            for (key in src) {
                value = src[key];
                if (Utils.isObject(value)) {
                    value = JSON.stringify(value);
                }
                else if (value) {
                    value = encodeURIComponent(value.toString());
                }
                arr.push(key + '=' + value);
            }
            return arr.join('&');
        };
        /**
         * Mimic jQuery.ajax().
         */
        Utils.ajax = function (ajaxSettings) {
            var xhr = new XMLHttpRequest(), settings = Utils.copy({
                async: true,
                cache: false,
                type: 'GET',
                postType: 'form'
            }, ajaxSettings), params, dataKey, headerKey, url = settings.url, forceLoad;
            if (typeof settings.data === 'string' || settings.data instanceof Blob || settings.data instanceof Document || settings.data instanceof FormData) {
                params = settings.data;
            }
            else if (Utils.isObject(settings.data)) {
                if (settings.type === 'GET') {
                    url += url.indexOf('?') >= 0 ? '&' : '?';
                    url += Utils.formEncodedObject(settings.data);
                }
                else {
                    if (settings.postType === 'form') {
                        params = Utils.formEncodedObject(settings.data);
                        settings.contentType = 'application/x-www-form-urlencoded';
                    }
                    else if (settings.postType === 'json') {
                        params = JSON.stringify(settings.data);
                        settings.contentType = 'application/json';
                    }
                    else if (settings.postType === 'multipart') {
                        params = new FormData();
                        for (dataKey in settings.data) {
                            params.append(dataKey, settings.data[dataKey]);
                        }
                    }
                }
            }
            xhr.onload = function (e) {
                var data;
                if (settings.dataType === 'json') {
                    data = JSON.parse(xhr.responseText);
                }
                else if (settings.dataType === 'text' || settings.dataType === 'html' || settings.dataType === 'script') {
                    data = xhr.responseText;
                }
                else if (settings.dataType === 'xml') {
                    data = xhr.responseXML;
                }
                settings.success.bind(xhr)(data);
            };
            if (settings.error) {
                xhr.onerror = settings.error.bind(xhr);
            }
            if (!settings.cache) {
                forceLoad = '_=' + Utils.random(1000, 9999).toFixed(0);
                url += url.indexOf('?') >= 0 ? '&' : '?';
                url += forceLoad;
            }
            xhr.open(settings.type, url, settings.async);
            if (settings.headers) {
                for (headerKey in settings.headers) {
                    xhr.setRequestHeader(headerKey, settings.headers[headerKey]);
                }
            }
            if (settings.contentType) {
                xhr.setRequestHeader('Content-Type', settings.contentType);
            }
            if (settings.beforeSend) {
                settings.beforeSend(xhr, settings);
            }
            xhr.send(params);
        };
        Utils.beginRequest = function (url, method, callback, data, type) {
            if (data === void 0) { data = null; }
            if (type === void 0) { type = 'json'; }
            Utils.ajax({
                async: true,
                data: data,
                dataType: type,
                type: method,
                success: callback,
                url: url
            });
        };
        /**
         * Mimic jQuery.get().
         */
        Utils.ajaxGet = function (url, callback, data, type) {
            if (data === void 0) { data = null; }
            if (type === void 0) { type = 'json'; }
            Utils.beginRequest(url, 'GET', callback, data, type);
        };
        /**
         * Mimic jQuery.post().
         */
        Utils.ajaxPost = function (url, callback, data, type) {
            if (data === void 0) { data = null; }
            if (type === void 0) { type = 'json'; }
            Utils.beginRequest(url, 'POST', callback, data, type);
        };
        /**
         * Mimic jQuery.load().
         */
        Utils.ajaxLoad = function (selector, url, data) {
            if (data === void 0) { data = null; }
            Utils.beginRequest(url, 'GET', function (res) {
                var element = document.querySelector(selector);
                element.innerHTML = res;
            }, data, 'html');
        };
        /**
         * Mimic $(document).ready().
         */
        Utils.documentReady = function (callback) {
            document.addEventListener('DOMContentLoaded', callback.bind(window));
        };
        return Utils;
    })();
    dreambuild.Utils = Utils;
})(dreambuild || (dreambuild = {}));
