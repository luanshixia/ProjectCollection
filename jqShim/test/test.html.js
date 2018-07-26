const assert = chai.assert;

describe('General tests', function() {
  describe('Environment', function() {
    it('should be in browser mode', function() {
      assert.isTrue(!!window);
    });
  });
});

describe('API tests', function() {
  describe('Factory', function() {
    it('can create a tag', function() {
      const jq = $('<div>');
      assert.equal(1, jq.elems.length);
    });
  });
});

mocha.run();
