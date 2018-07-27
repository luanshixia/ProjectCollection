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

  describe('Utility', function() {
    describe('#extend()', function() {
      it('should be able to merge objects by extend()', function() {
        const result = $.extend({}, { a: 1 }, { b: 2});
        assert.isTrue('a' in result);
        assert.isTrue('b' in result);
      });

      it('should be able to extend the library', function() {
        $.extend({ test: 1 });
        assert.isTrue('test' in $);
      });
    });
  });

  describe('Basics', function() {
    describe('#attr()', function() {
      it('should be able to get attribute', function() {
        assert.equal('mocha', $('#mocha').attr('id'));
      });

      it('should be able to set attribute', function() {
        $('#mocha').attr('class', 'test');
        assert.equal('test', $('#mocha').attr('class'));
      });
    });
  });
});

mocha.run();
