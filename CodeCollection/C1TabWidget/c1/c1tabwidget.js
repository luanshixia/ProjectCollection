var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var c1;
(function (c1) {
    "use strict";
    var $ = jQuery;
    var c1tabwidget = (function (_super) {
        __extends(c1tabwidget, _super);
        function c1tabwidget() {
            _super.apply(this, arguments);

        }
        c1tabwidget.prototype._init = function () {
            _super.prototype._init.call(this);
        };
        c1tabwidget.prototype._create = function () {
            _super.prototype._create.call(this);
            this._createSystem();
        };
        c1tabwidget.prototype._selectTab = function (index, raise) {
            _super.prototype._selectTab.call(this, index, raise);
            c1common.updateJsonHiddenField(this.element.attr("id"), c1common.JSON.stringify({
                selectedIndex: index
            }));
        };
        c1tabwidget.prototype._createSystem = function () {
            var o = this.options, self = this, id = this.element.attr("id");
            var widget = $("#" + id);
            widget.find("ul").append($("<li>").addClass(o.wijCSS.tabStripItemClass).addClass(o.wijCSS.tabStripItemSystemClass).addClass(o.wijCSS.stateDefault).append($("<a>").attr("href", "#").text("...").click(function (e) {
                $(this).parent().remove();
                self._showAllTabs();
                e.preventDefault();
            })));
        };
        c1tabwidget.prototype._showAllTabs = function () {
            var _this = this;
            this._doCallBack("showAllTabs", null, function (returnValue, context) {
                var data = $.parseJSON(returnValue);
                for(var i = 0; i < data.length; i++) {
                    var item = data[i];
                    _this.addTab(item.header, item.content);
                }
                _this._showStartUpPage();
            });
            this._trigger("loadTab", null, null);
        };
        c1tabwidget.prototype._doCallBack = function (command, data, success) {
            var self = this, o = self.options, id = o.uniqueID, requestData = {
                CommandName: command,
                CommandData: data
            }, successCallBack = success, errorCallBack = function (error) {
                alert(error);
            };
            var arg = c1common.JSON.stringify(requestData);
            WebForm_DoCallback(id, arg, successCallBack, null, errorCallBack, true);
        };
        return c1tabwidget;
    })(wijmo.tabwidget.wijtabwidget);
    c1.c1tabwidget = c1tabwidget;    
    c1tabwidget.prototype.widgetEventPrefix = "c1tabwidget";
    c1tabwidget.prototype.options = $.extend(true, {
    }, $.wijmo.wijtabs.prototype.options, {
        postbackReference: null,
        uniqueID: "",
        loadSize: 4,
        loadTab: null
    });
    $.wijmo.registerWidget("c1tabwidget", $.wijmo.wijtabwidget, c1tabwidget.prototype);
})(c1 || (c1 = {}));
