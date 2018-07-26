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

    it('can select tags by selector', function() {
      const jq = $('#mocha');
      assert.equal(1, jq.elems.length);

      const jq1 = $('head script');
      assert.equal(2, jq1.elems.length);
    });

    it('can wrap tags', function() {
    });

    it('can wrap single tag', function() {
    });
  });
});

mocha.run();
