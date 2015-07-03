var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var wijmo;
(function (wijmo) {
    (function (tabwidget) {
        "use strict";
        var $ = jQuery, widgetName = "wijtabwidget";
        var wijtabwidget = (function (_super) {
            __extends(wijtabwidget, _super);
            function wijtabwidget() {
                _super.apply(this, arguments);

            }
            wijtabwidget.prototype._create = function () {
                var o = this.options, self = this, id = this.element.attr("id");
                this.element.addClass(o.wijCSS.tabClass).addClass(o.wijCSS.widget);
                $("#" + id + ">ul").addClass(o.wijCSS.tabStripClass).addClass(o.wijCSS.header);
                $("#" + id + ">div").addClass(o.wijCSS.tabPageClass).addClass(o.wijCSS.content).hide();
                var lis = $("#" + id + ">ul>li");
                lis.addClass(o.wijCSS.tabStripItemClass).addClass(o.wijCSS.stateDefault);
                lis.find("a").on("click", function (e) {
                    e.preventDefault();
                });
                lis.filter(function (index) {
                    return o.disabledIndices.indexOf(index) >= 0;
                }).addClass(o.wijCSS.stateDisabled);
                lis.find("a").on(o.trigger.toLowerCase(), function () {
                    if(!$(this).parent().hasClass(o.wijCSS.stateDisabled) && !$(this).parent().hasClass(o.wijCSS.tabStripItemSystemClass)) {
                        var index = $(this).parent().index();
                        self.selectTab(index);
                    }
                });
                this._showStartUpPage();
            };
            wijtabwidget.prototype.addTab = function (header, content) {
                var o = this.options, self = this, id = this.element.attr("id");
                var widget = $("#" + id);
                widget.find("ul").append($("<li>").addClass(o.wijCSS.tabStripItemClass).addClass(o.wijCSS.stateDefault).append($("<a>").attr("href", "#").text(header).on(o.trigger.toLowerCase(), function () {
                    if(!$(this).parent().hasClass(o.wijCSS.stateDisabled) && !$(this).parent().hasClass(o.wijCSS.tabStripItemSystemClass)) {
                        var index = $(this).parent().index();
                        self.selectTab(index);
                    }
                }).click(function (e) {
                    e.preventDefault();
                })));
                widget.append($("<div>").addClass(o.wijCSS.tabPageClass).addClass(o.wijCSS.content).html(content).hide());
            };
            wijtabwidget.prototype._selectTab = function (index, raise) {
                var o = this.options, id = this.element.attr("id");
                var lis = $("#" + id + ">ul>li");
                if(index >= lis.length) {
                    return;
                }
                var li = lis.eq(index);
                var pageId = li.children("a").attr("href").replace("#", "");
                $("#" + id + ">div").hide();
                $("#" + id + ">div").eq(index).show();
                this.options.selectedIndex = index;
                lis.removeClass(o.wijCSS.stateFocus);
                li.addClass(o.wijCSS.stateFocus);
                if(raise) {
                    this._trigger("select", null, {
                        index: index,
                        pageId: pageId
                    });
                }
            };
            wijtabwidget.prototype.selectTab = function (index) {
                this._selectTab(index, true);
            };
            wijtabwidget.prototype._showStartUpPage = function () {
                this._selectTab(this.options.selectedIndex, true);
            };
            return wijtabwidget;
        })(wijmo.wijmoWidget);
        tabwidget.wijtabwidget = wijtabwidget;        
        var wijtabwidget_options = (function () {
            function wijtabwidget_options() {
                this.wijCSS = {
                    tabClass: "wijmo-wijtabwidget",
                    tabPageClass: "wijmo-wijtabwidget-page",
                    tabStripClass: "wijmo-wijtabwidget-strip",
                    tabStripItemClass: "wijmo-wijtabwidget-stripitem",
                    tabStripItemSystemClass: "wijmo-wijtabwidget-stripitem-system"
                };
                this.trigger = "click";
                this.disabledIndices = [];
                this.selectedIndex = 0;
                this.select = null;
            }
            return wijtabwidget_options;
        })();        
        wijtabwidget.prototype.options = $.extend(true, {
        }, wijmo.wijmoWidget.prototype.options, new wijtabwidget_options());
        $.wijmo.registerWidget(widgetName, wijtabwidget.prototype);
    })(wijmo.tabwidget || (wijmo.tabwidget = {}));
    var tabwidget = wijmo.tabwidget;
})(wijmo || (wijmo = {}));
