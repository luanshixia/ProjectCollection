using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Dreambuild.Mvc
{
    /// <summary>
    /// Provides public methods throught Html.Dreambuild() for creating controls.
    /// </summary>
    public class ControlFactory
    {
        protected readonly HtmlHelper _htmlHelper = null;

        /// <summary>
        /// Initializes a new instance of the WidgetFactory class.
        /// </summary>
        /// <param name="htmlHelper">
        /// The instance of the <seealso cref="System.Web.Mvc.HtmlHelper"/>
        /// </param>
        public ControlFactory(HtmlHelper htmlHelper)
        {
            this._htmlHelper = htmlHelper;
        }

        #region Basic HTML elements

        public HtmlControlBuilder Anchor(string text, string href, string title = null, string target = null)
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "a"))
                .Text(text)
                .HtmlAttribute("href", href)
                .HtmlAttribute("title", title)
                .HtmlAttribute("target", target);
        }

        public HtmlControlBuilder Image(string src, string alt = null, string title = null)
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "img", TagRenderMode.SelfClosing))
                .HtmlAttribute("src", src)
                .HtmlAttribute("alt", alt)
                .HtmlAttribute("title", title);
        }

        public HtmlControlBuilder Button(string text, string onclick)
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "button"))
                .Text(text)
                .HtmlAttribute("onclick", onclick);
        }

        public HtmlControlBuilder Span(Func<object, object> content)
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "span"))
                .Content(content);
        }

        public HtmlControlBuilder Div(Func<object, object> content)
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "div"))
                .Content(content);
        }

        public HtmlControlBuilder BeginDiv()
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "div", TagRenderMode.StartTag)).Begin();
        }

        public HtmlControlBuilder Table(Func<object, object> content)
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "table"))
                .Content(content);
        }

        public HtmlControlBuilder BeginTable()
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "table", TagRenderMode.StartTag)).Begin();
        }

        public HtmlControlBuilder Form(Func<object, object> content, string action = null, string method = "POST", bool multipart = false)
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "form"))
                .Content(content)
                .HtmlAttribute("action", action)
                .HtmlAttribute("method", method)
                .HtmlAttribute("enctype", multipart ? "multipart/form-data" : null);
        }

        public HtmlControlBuilder BeginForm()
        {
            return new HtmlControlBuilder(new HtmlControl(_htmlHelper, "form", TagRenderMode.StartTag)).Begin();
        }

        #endregion
    }

    /// <summary>
    /// Provides public methods throught Html.Dreambuild() for creating controls.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class ControlFactory<TModel> : ControlFactory
    {
        #region Infrastructures

        /// <summary>
        /// Initializes a new instance of the WidgetFactory class.
        /// </summary>
        /// <param name="htmlHelper">
        /// The instance of the <seealso cref="System.Web.Mvc.HtmlHelper"/>
        /// </param>
        public ControlFactory(HtmlHelper<TModel> htmlHelper)
            : base(htmlHelper)
        {
        }

        private string GetName(LambdaExpression expression)
        {
            return _htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }

        #endregion

        #region Controls

        #endregion
    }
}
