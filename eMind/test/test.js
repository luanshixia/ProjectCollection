const assert = require('chai').assert;

describe('General tests', function() {
  describe('Environment', function() {
    it('should be in Node JS mode', function() {
      assert.isTrue(!!require);
      assert.isTrue(!!module);
    });
  });
});
