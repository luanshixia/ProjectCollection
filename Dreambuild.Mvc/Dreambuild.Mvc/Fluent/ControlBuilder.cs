namespace Dreambuild.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Define the basic class of the control builders.
    /// </summary>
    public class ControlBuilder<TControl, TBuilder> : IHtmlString
        where TControl : Control
        where TBuilder : ControlBuilder<TControl, TBuilder>
    {
        protected TControl _control;

        /// <summary>
        /// Create a ControlBuilder instance.
        /// </summary>
        /// <param name="control">The control.</param>
        public ControlBuilder(TControl control)
        {
            this._control = control;
        }

        string IHtmlString.ToHtmlString()
        {
            return (_control as IHtmlString).ToHtmlString();
        }

        /// <summary>
        /// Sets the ID property.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TBuilder ID(string id)
        {
            this._control.ID = id;
            return this as TBuilder;
        }

        /// <summary>
        /// Sets one HTML attribute.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TBuilder HtmlAttribute(string key, object value)
        {
            if (value != null)
            {
                this._control.HtmlAttributes.Add(key, value);
            }
            return this as TBuilder;
        }

        /// <summary>
        /// Sets HTML attributes.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public TBuilder HtmlAttributes(IDictionary<string, object> attributes)
        {
            foreach (var pair in attributes)
            {
                this._control.HtmlAttributes[pair.Key] = pair.Value;
            }
            return this as TBuilder;
        }

        /// <summary>
        /// Sets HTML attributes.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public TBuilder HtmlAttributes(object attributes)
        {
            return HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(attributes));
        }

        /// <summary>
        /// Sets one CSS style.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TBuilder CssStyle(string key, object value)
        {
            this._control.CssStyles.Add(key, value);
            return this as TBuilder;
        }

        /// <summary>
        /// Sets CSS styles.
        /// </summary>
        /// <param name="styles"></param>
        /// <returns></returns>
        public TBuilder CssStyles(IDictionary<string, object> styles)
        {
            foreach (var pair in styles)
            {
                this._control.CssStyles[pair.Key] = pair.Value;
            }
            return this as TBuilder;
        }

        /// <summary>
        /// Sets CSS styles.
        /// </summary>
        /// <param name="styles"></param>
        /// <returns></returns>
        public TBuilder CssStyles(object styles)
        {
            return CssStyles(HtmlHelper.AnonymousObjectToHtmlAttributes(styles));
        }

        /// <summary>
        /// Sets CSS class.
        /// </summary>
        /// <param name="class"></param>
        /// <returns></returns>
        public TBuilder CssClass(string @class)
        {
            _control.CssClass = @class;
            return this as TBuilder;
        }
    }
}
