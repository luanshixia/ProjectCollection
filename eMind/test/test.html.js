const assert = chai.assert;

describe('General tests', function() {
  describe('Environment', function() {
    it('should be in browser mode', function() {
      assert.isTrue(!!window);
    });
  });
});

mocha.run();
