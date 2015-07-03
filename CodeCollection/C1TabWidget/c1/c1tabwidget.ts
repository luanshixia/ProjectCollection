/// <reference path="../../../../../Widgets/Wijmo/Base/jquery.wijmo.widget.ts"/>
/// <reference path="../../../../../Widgets/Wijmo/wijtabwidget/jquery.wijmo.wijtabwidget.ts"/>

declare var __doPostBack, WebForm_DoCallback, c1common;

module c1 {
    "use strict";
    var $ = jQuery;

    export class c1tabwidget extends wijmo.tabwidget.wijtabwidget {
        _init() {
            super._init();
        }

        _create() {
            super._create();
            this._createSystem();
        }

        _selectTab(index: number, raise: bool) {
            super._selectTab(index, raise);
            c1common.updateJsonHiddenField(this.element.attr("id"), c1common.JSON.stringify({ selectedIndex: index }));
        }

        _createSystem() {
            var o = this.options, self = this, id = this.element.attr("id");
            var widget = $("#" + id);
            widget.find("ul").append($("<li>")
                .addClass(o.wijCSS.tabStripItemClass)
                .addClass(o.wijCSS.tabStripItemSystemClass)
                .addClass(o.wijCSS.stateDefault)
                .append($("<a>")
                    .attr("href", "#")
                    .text("...")
                    .click(function (e) {
                        $(this).parent().remove();
                        self._showAllTabs();
                        e.preventDefault();
                    })
                )
            );
        }

        _showAllTabs() {
            this._doCallBack("showAllTabs", null, (returnValue, context) => {
                var data = $.parseJSON(returnValue);
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    this.addTab(item.header, item.content);
                }
                this._showStartUpPage();
            });
            this._trigger("loadTab", null, null);
        }

        _doCallBack(command: string, data, success: (returnValue: any, context: any) => void ) {
            var self = this, o = self.options, id = o.uniqueID,
            requestData = { CommandName: command, CommandData: data },
            successCallBack = success, errorCallBack = error => {
                alert(error);
            };
            var arg = c1common.JSON.stringify(requestData);
            WebForm_DoCallback(id, arg, successCallBack, null, errorCallBack, true);
        }
    }

    c1tabwidget.prototype.widgetEventPrefix = "c1tabwidget";

    c1tabwidget.prototype.options = $.extend(true, {}, $.wijmo.wijtabs.prototype.options, {
        postbackReference: null,
        uniqueID: "",
        loadSize: 4,
        loadTab: null
    });

    $.wijmo.registerWidget("c1tabwidget", $.wijmo.wijtabwidget, c1tabwidget.prototype);
}