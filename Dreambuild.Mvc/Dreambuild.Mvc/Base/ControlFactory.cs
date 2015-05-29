using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Dreambuild.Mvc
{
    /// <summary>
    /// Provides public methods throught Html.Dreambuild() for creating controls.
    /// </summary>
    public class ControlFactory
    {
        #region Fields

        protected readonly HtmlHelper _htmlHelper = null;

        #endregion

        #region Constructors

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

        #endregion

        #region Properties

        protected HtmlHelper HtmlHelper
        {
            get
            {
                return _htmlHelper;
            }
        }

        protected ViewContext ViewContext
        {
            get
            {
                return _htmlHelper.ViewContext;
            }
        }

        #endregion

        #region Controls - Input

        public Control Control(string selector = null)
        {
            return new Control(this.ViewContext, selector);
        }

        #endregion

        #region Controls - Chart

        #endregion

        #region Controls - Gauge

        #endregion

        #region Controls - Grid

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

        protected new HtmlHelper<TModel> HtmlHelper
        {
            get
            {
                return base.HtmlHelper as HtmlHelper<TModel>;
            }
        }

        private string GetName(LambdaExpression expression)
        {
            return ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }

        #endregion

        #region Controls

        #endregion
    }
}
