/*
 * Util.js
 */

module dreambuild {

    /**
     * Utility functions.
     */
    export class Utils {

        /**
         * Generates a random number between min and max.
         */
        static random(min: number, max: number) {
            var rand = Math.random();
            return min + rand * (max - min);
        }

        /**
         * Mimic String.Format().
         */
        static formatString(format: string, ...args: string[]): string {
            return format.replace(/{(\d+)}/g,(match, index) => {
                return typeof args[index] !== 'undefined'
                    ? args[index]
                    : match;
            });
        }

        /**
         * Mimic jQuery.isPlainObject().
         */
        static isObject(value: any): boolean {
            return value != null && typeof value === 'object' && !(value instanceof Date);
        }

        /**
         * Mimic jQuery.extend() for two objects.
         */
        static copy(dst: any, src: any): any {
            var key: string, value;
            for (key in src) {
                value = src[key];
                if (!dst._copy || !dst._copy(key, value)) { // allow overrides
                    if (Utils.isObject(value) && dst[key]) {
                        Utils.copy(dst[key], value); // copy sub-objects
                    } else {
                        dst[key] = value; // assign values
                    }
                }
            }
            return dst;
        }

        /**
         * Mimic jQuery.extend().
         */
        static extend(dst: any, ...src: any[]): any {
            var list = src.map(s => Utils.copy({}, s)); // don't want src elements modified, so copy them
            list.unshift(dst);
            return list.reduceRight((s, d) => Utils.copy(d, s));
        }

        /**
         * Serialize object to form data format. Different from jQuery.param() in that it serializes complex value to inner json.
         */
        static formEncodedObject(src: Object): string {
            var arr = [], key: string, value;
            for (key in src) {
                value = src[key];
                if (Utils.isObject(value)) {
                    value = JSON.stringify(value);
                } else if (value) {
                    value = encodeURIComponent(value.toString());
                }
                arr.push(key + '=' + value);
            }
            return arr.join('&');
        }

        /**
         * Mimic jQuery.ajax().
         */
        static ajax(ajaxSettings: IAjaxSettings) {
            var xhr = new XMLHttpRequest(),
                settings = <IAjaxSettings>Utils.copy({
                    async: true,
                    cache: false,
                    type: 'GET',
                    postType: 'form'
                }, ajaxSettings),
                params: any,
                dataKey: string,
                headerKey: string,
                url = settings.url,
                forceLoad: string;

            if (typeof settings.data === 'string'
                || settings.data instanceof Blob
                || settings.data instanceof Document
                || settings.data instanceof FormData) {
                params = settings.data;
            } else if (Utils.isObject(settings.data)) {
                if (settings.type === 'GET') {
                    url += url.indexOf('?') >= 0 ? '&' : '?';
                    url += Utils.formEncodedObject(settings.data);
                } else {
                    if (settings.postType === 'form') {
                        params = Utils.formEncodedObject(settings.data);
                        settings.contentType = 'application/x-www-form-urlencoded';
                    } else if (settings.postType === 'json') {
                        params = JSON.stringify(settings.data);
                        settings.contentType = 'application/json';
                    } else if (settings.postType === 'multipart') {
                        params = new FormData();
                        for (dataKey in settings.data) {
                            params.append(dataKey, settings.data[dataKey]);
                        }
                    }
                }
            }
            xhr.onload = e => {
                var data;
                if (settings.dataType === 'json') {
                    data = JSON.parse(xhr.responseText);
                } else if (settings.dataType === 'text'
                    || settings.dataType === 'html'
                    || settings.dataType === 'script') {
                    data = xhr.responseText;
                } else if (settings.dataType === 'xml') {
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
        }

        static beginRequest(
            url: string,
            method: string,
            callback: Action1<any>,
            data: any = null,
            type: string = 'json') {
            Utils.ajax({
                async: true,
                data: data,
                dataType: type,
                type: method,
                success: callback,
                url: url
            });
        }

        /**
         * Mimic jQuery.get().
         */
        static ajaxGet(
            url: string,
            callback: Action1<any>,
            data: any = null,
            type: string = 'json') {
            Utils.beginRequest(url, 'GET', callback, data, type);
        }

        /**
         * Mimic jQuery.post().
         */
        static ajaxPost(
            url: string,
            callback: Action1<any>,
            data: any = null,
            type: string = 'json') {
            Utils.beginRequest(url, 'POST', callback, data, type);
        }

        /**
         * Mimic jQuery.load().
         */
        static ajaxLoad(selector: string, url: string, data: any = null) {
            Utils.beginRequest(url, 'GET', res => {
                var element = <HTMLElement>document.querySelector(selector);
                element.innerHTML = res;
            }, data, 'html');
        }

        /**
         * Mimic $(document).ready().
         */
        static documentReady(callback: Action0) {
            document.addEventListener('DOMContentLoaded', callback.bind(window));
        }

    }

    /**
     * Ajax settings, compatible with jQuery ajax API.
     */
    export interface IAjaxSettings {
        /*
         * Use async
         */
        async?: boolean;
        /*
         * Use cache
         */
        cache?: boolean;
        /*
         * Request data
         * Supported: string|plain object|Document|Blob|FormData
         */
        data?: any;
        /*
         * Accepted response data type.
         * Supported: json|text|html|script|xml
         */
        dataType?: string;
        /*
         * Request HTTP method
         */
        type?: string;
        /*
         * Callback if succeed
         */
        success?: Action1<any>;
        /*
         * Callback if fail
         */
        error?: Action1<ErrorEvent>;
        /*
         * Callback before sending
         */
        beforeSend?: Action2<XMLHttpRequest, IAjaxSettings>;
        /*
         * Request url
         */
        url?: string;
        /*
         * Request headers
         */
        headers?: any;
        /*
         * Request MIME, i.e. MIME of "data"
         */
        contentType?: string;
        /*
         * How to serialize "data", when "data" is plain object and "type" is "POST".
         * Supported: form|json|multipart
         */
        postType?: string;
    }

    /**
     * A function type: () => void
     */
    export interface Action0 {
        (): void;
    }

    /**
     * A function type: (T) => void
     */
    export interface Action1<T> {
        (p: T): void;
    }

    /**
     * A function type: (T1, T2) => void
     */
    export interface Action2<T1, T2> {
        (p1: T1, p2: T2): void;
    }

    /**
     * A function type: () => TResult
     */
    export interface Func0<TResult> {
        (): TResult
    }

    /**
     * A function type: (T) => TResult
     */
    export interface Func1<T, TResult> {
        (p: T): TResult
    }

    /**
     * A function type: (T1, T2) => TResult
     */
    export interface Func2<T1, T2, TResult> {
        (p1: T1, p2: T2): TResult
    }
}