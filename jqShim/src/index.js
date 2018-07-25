// jq-shim.js

const version = '0.1.0';

function jqShim(selector, context) {
  return new jqShim.fn.init(selector, context);
}

jqShim.fn = jqShim.prototype = {

  jquery: version,

  constructor: jqShim,

  length: 0,

  each(callback) {
    return jqShim.each(this, callback);
  }
};

jqShim.extend = jqShim.fn.extend = function extend(...args) {
  if (args.length === 1) {
    return extend(this, args[0]);
  }

  return Object.assign(...args);
};

jqShim.extend({
  each(obj, callback) {
    Array.from(obj, callback);
  },

  map(elems, callback) {
    return Array.from(elems, callback);
  }
});
