// jq-shim.js

const version = '0.1.0';

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
    this.elems = [];
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
