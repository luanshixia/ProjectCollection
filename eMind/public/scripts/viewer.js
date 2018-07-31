class ViewerOptions extends WidgetOptions {
}

class Viewer extends Widget {
  constructor($element, options) {
    super($element, options);

    this.$content = $element.find('dreambuild-viewer-content');
    this.origin = { x: 0, y: 0 };
    this.scale = 1;
  }

  _create() {
    this._init();
  }

  _init() {
    this.$element.addClass('dreambuild-viewer dreambuild-viewer-border');
  }

  pan(displacement) {
    this.origin.x += displacement.x;
    this.origin.y += displacement.y;
    this._render();
  }

  zoom(newScale, basePoint) {
    const v1x = basePoint.x - this.origin.x;
    const v1y = basePoint.y - this.origin.y;
    const v2x = (this.scale / newScale) * v1x;
    const v2y = (this.scale / newScale) * v1y;
    this.scale = newScale;
    this.origin.x += v1x - v2x;
    this.origin.y += v1y - v2y;
    this._render();
  }

  _render() {
    this.$content.css('transform', null);
  }
}

WidgetFactory.register('dreambuild-viewer', 'viewer', Viewer);
