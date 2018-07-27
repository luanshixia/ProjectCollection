// jq-shim.js

const version = '0.1.0';

function isArrayLike(obj) {
  const length = !!obj && "length" in obj && obj.length;

  return typeof obj === "array" || length === 0 ||
    typeof length === "number" && length > 0 && (length - 1) in obj;
}

function hyphenToCamel(value) {
  return value.replace(/-([a-z])/g, g => g[1].toUpperCase());
}

class jqShim {

  static extend(...args) {
    if (args.length === 1) {
      Object.assign(jqShim, args[0]);
      return Object.assign(jqShimFactory, args[0]);
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

  each(callback) {
    Array.from(this.elems, callback);
    return this;
  }

  attr(name, value) {
    if (!value && typeof name === 'string') {
      return this.elems[0].getAttribute(name);
    }

    if (typeof name === 'object') {
      for (const key in name) {
        this.elems.forEach(elem => elem.setAttribute(key, name[key]));
      }
    } else {
      this.elems.forEach(elem => elem.setAttribute(name, value));
    }

    return this;
  }

  prop(name, value) {
    if (!value && typeof name === 'string') {
      return this.elems[0][name];
    }

    if (typeof name === 'object') {
      for (const key in name) {
        this.elems.forEach(elem => elem[key] = name[key]);
      }
    } else {
      this.elems.forEach(elem => elem[name] = value);
    }

    return this;
  }

  data(name, value) {
    // TODO: make this more robust
    if (!value) {
      return this.elems[0][name];
    }

    this.elems.forEach(elem => elem[name] = value);
    return this;
  }

  css(name, value) {
    if (typeof name === 'string') {
      name = hyphenToCamel(name);
    }

    if (!value && typeof name === 'string') {
      return window
        .getComputedStyle(this.elems[0])
        .getPropertyValue(name);
    }

    if (typeof name === 'object') {
      for (let key in name) {
        key = hyphenToCamel(key);
        this.elems.forEach(elem => elem.style[key] = name[key]);
      }
    } else {
      this.elems.forEach(elem => elem.style[name] = value);
    }

    return this;
  }

}

function jqShimFactory(selector) {
  return new jqShim(selector);
}

Object.assign(jqShimFactory, {
  extend: jqShim.extend,
  each: jqShim.each,
  map: jqShim.map
});

window.jqShim = window.$ = jqShimFactory;
