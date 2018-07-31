const assert = chai.assert;

describe('General tests', () => {
  describe('Environment', () => {
    it('should be in browser mode', () => {
      assert.isTrue(!!window);
    });
  });
});

mocha.run();
