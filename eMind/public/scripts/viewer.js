class Viewer extends Widget {
  constructor($element, options) {
    super($element, options);

    this.center = [0, 0];
    this.scale = 1;
  }

  _create() {
    this.$element.html(this.options.text);
  }
}

WidgetFactory.register('dreambuild-viewer', 'viewer', Viewer);
