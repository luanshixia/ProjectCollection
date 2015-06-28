declare module wijmo {
    export function isFunction(value: any): boolean;
    export function isObject(value: any): boolean;
}

module c1 {
    "use strict";

    function extend(dst: any, src: any) {
        for (var key in src) {
            if (key in dst) { // check, but not throw.
                var value = src[key];
                if (!dst._copy || !dst._copy(key, value)) { // allow overrides
                    if (dst[key] instanceof Event && wijmo.isFunction(value)) {
                        dst[key].addHandler(value); // add event handler
                    } else if (wijmo.isObject(value) && dst[key]) {
                        extend(dst[key], value); // copy sub-objects
                    } else {
                        dst[key] = value; // assign values
                    }
                }
            }
        }
    }

    /*
     * Initializes the control by copying the properties from a given object.
     *
     * This method allows you to initialize controls using plain data objects
     * instead of setting the value of each property in code.
     *
     * ***C1 modified version does not throw.***
     *
     * @param control The wijmo control.
     * @param options Object that contains the initialization data.
     */
    export function initializeControl(control: any, options: any) {
        if (options) {
            control.beginUpdate();
            extend(control, options);
            control.endUpdate();
        }
    }
}