/// <reference path="../Base/jquery.wijmo.widget.ts" />

/*globals jQuery,$,window,alert,document,confirm,location,setTimeout*/

/*
* Depends:
*  jquery.ui.core.js
*  jquery.ui.widget.js
*  jquery.wijmo.wijutil.js
*  jquery.wijmo.wijtabwidget.js
*
*/

module wijmo.tabwidget {
    "use strict";
    var $ = jQuery,
        widgetName = "wijtabwidget";

    /** @widget */
    export class wijtabwidget extends wijmoWidget {
        _create() {
            var o = this.options, self = this, id = this.element.attr("id");
            // set class for basic elements
            this.element.addClass(o.wijCSS.tabClass).addClass(o.wijCSS.widget);
            $("#" + id + ">ul").addClass(o.wijCSS.tabStripClass).addClass(o.wijCSS.header);
            $("#" + id + ">div").addClass(o.wijCSS.tabPageClass).addClass(o.wijCSS.content).hide();
            var lis = $("#" + id + ">ul>li");
            lis.addClass(o.wijCSS.tabStripItemClass).addClass(o.wijCSS.stateDefault);
            // block default link behaviour
            lis.find("a").on("click", e => {
                e.preventDefault();
            });
            // set class for disabled tabs 
            lis.filter(index => {
                return o.disabledIndices.indexOf(index) >= 0;
            }).addClass(o.wijCSS.stateDisabled);
            // bind event handler for normal tabs
            lis.find("a").on(o.trigger.toLowerCase(), function () { // ? how to make this an arrow function with jQuery this binding
                if (!$(this).parent().hasClass(o.wijCSS.stateDisabled) && !$(this).parent().hasClass(o.wijCSS.tabStripItemSystemClass)) {
                    var index = $(this).parent().index();
                    self.selectTab(index);
                }
            });
            // show default page
            this._showStartUpPage();
        }

        addTab(header: string, content: string) {
            var o = this.options, self = this, id = this.element.attr("id");
            var widget = $("#" + id);
            widget.find("ul").append($("<li>")
                .addClass(o.wijCSS.tabStripItemClass)
                .addClass(o.wijCSS.stateDefault)
                .append($("<a>")
                    .attr("href", "#")
                    .text(header)
                    .on(o.trigger.toLowerCase(), function () {
                        if (!$(this).parent().hasClass(o.wijCSS.stateDisabled) && !$(this).parent().hasClass(o.wijCSS.tabStripItemSystemClass)) {
                            var index = $(this).parent().index();
                            self.selectTab(index);
                        }
                    }).click(e => {
                        e.preventDefault();
                    })
                )
            );
            widget.append($("<div>")
                .addClass(o.wijCSS.tabPageClass)
                .addClass(o.wijCSS.content)
                .html(content)
                .hide()
            );
        }

        _selectTab(index: number, raise: bool) {
            var o = this.options, id = this.element.attr("id");
            var lis = $("#" + id + ">ul>li");
            if (index >= lis.length) return;
            var li = lis.eq(index);
            var pageId = li.children("a").attr("href").replace("#", "");
            $("#" + id + ">div").hide();
            $("#" + id + ">div").eq(index).show();
            this.options.selectedIndex = index;
            lis.removeClass(o.wijCSS.stateFocus);
            li.addClass(o.wijCSS.stateFocus);
            if (raise) {
                this._trigger("select", null, { index: index, pageId: pageId });
            }
        }

        selectTab(index: number) {
            this._selectTab(index, true);
        }

        _showStartUpPage() {
            this._selectTab(this.options.selectedIndex, true);
        }
    }

    class wijtabwidget_options {
        /**  @ignore */
        wijCSS = {
            tabClass: "wijmo-wijtabwidget",
            tabPageClass: "wijmo-wijtabwidget-page",
            tabStripClass: "wijmo-wijtabwidget-strip",
            tabStripItemClass: "wijmo-wijtabwidget-stripitem",
            tabStripItemSystemClass: "wijmo-wijtabwidget-stripitem-system"
        };

        trigger = "click";
        disabledIndices = [];
        selectedIndex = 0;
        //loadSize = 4;
        select: (e: JQueryEventObject, args: IWijTabWidgetEventArgs) => void = null;
        //loadTab: (e: JQueryEventObject, args: IWijTabWidgetEventArgs) => void = null;
    }

    wijtabwidget.prototype.options = $.extend(true, {}, wijmo.wijmoWidget.prototype.options, new wijtabwidget_options());

    $.wijmo.registerWidget(widgetName, wijtabwidget.prototype);

    export interface IWijTabWidgetEventArgs {
        /** The index of the panel.*/
        index: number;
        /** The id of the panel.*/
        pageId: string;
    }
}

/** @ignore*/
interface JQuery {
    wijtabwidget: JQueryWidgetFunction;
}