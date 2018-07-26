// jq-shim.js

const version = '0.1.0';

function isArrayLike(obj) {
  const length = !!obj && "length" in obj && obj.length;

  return typeof obj === "array" || length === 0 ||
    typeof length === "number" && length > 0 && (length - 1) in obj;
}

class jqShim {

  static extend(...args) {
    if (args.length === 1) {
      return Object.assign(jqShim, args[0]);
    }

    return Object.assign(...args);
  }

  static each(obj, callback) {
    Array.from(obj, callback);
  }

  static map(elems, callback) {
    return Array.from(elems, callback);
  }

  get jquery() {
    return version;
  }

  get length() {
    return this.elems.length;
  }

  constructor(selector) {
    if (typeof (selector) === 'string') {
      if (selector.startsWith('<') && selector.endsWith('>') && selector.length > 2) {
        // element factory
        this.elems = [document.createElement(selector.substring(1, selector.length - 1))];
      } else {
        // selector
        this.elems = Array.from(document.querySelectorAll(selector));
      }
    } else if (isArrayLike(selector)) {
      // array like
      this.elems = Array.from(selector);
    } else {
      // single object
      this.elems = [selector];
    }
  }

  extend(...args) {
    if (args.length === 1) {
      return Object.assign(this, args[0]);
    }

    return Object.assign(...args);
  }

  each(callback) {
    Array.from(this, callback);
  }
}

window.jqShim = jqShim;
window.$ = function(selector) {
  return new jqShim(selector);
}
