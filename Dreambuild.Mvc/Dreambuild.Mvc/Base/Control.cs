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
    /// Represents the Control class to provide the common properties for the all controls.
    /// </summary>
    public class Control : IHtmlString
    {
        #region Fields

        private string _id = string.Empty;
        private IDictionary<string, object> _htmlAttributes = null;
        private IDictionary<string, object> _styles = null;

        #endregion

        public Control(ViewContext context, string selector = null)
        {
            this.ViewContext = context;
            this.Selector = selector;
        }

        /// <summary>
        /// Set self with chained methods.
        /// </summary>
        /// <param name="setter"></param>
        /// <returns></returns>
        public Control With(Action<Control> setter)
        {
            setter(this);
            return this;
        }

        /// <summary>
        /// A value indicates the id of the widget
        /// </summary>
        public string ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.HtmlAttributes["id"] = value;
                }
                else
                {
                    this.HtmlAttributes.Remove("id");
                }
            }
        }

        /// <summary>
        /// A IDictionary value that save the attributes for the outmost html tag.
        /// </summary>
        public IDictionary<string, object> HtmlAttributes
        {
            get
            {
                if (this._htmlAttributes == null)
                {
                    this._htmlAttributes = new RouteValueDictionary();
                }
                return this._htmlAttributes;
            }
        }

        /// <summary>
        /// A IDictionary value that save the styles for the outmost html tag.
        /// </summary>
        public IDictionary<string, object> Styles
        {
            get
            {
                if (this._styles == null)
                {
                    this._styles = new RouteValueDictionary();
                }
                return this._styles;
            }
        }

        protected string Selector
        {
            get;
            set;
        }

        protected ViewContext ViewContext
        {
            get;
            set;
        }

        protected virtual string ControlName
        {
            get
            {
                return "Control";
            }
        }

        protected virtual string ControlFullName
        {
            get
            {
                return "Dreambuild.Mvc.Control";
            }
        }

        protected virtual string TagName
        {
            get
            {
                return "div";
            }
        }

        protected virtual TagRenderMode TagRenderMode
        {
            get
            {
                return TagRenderMode.Normal;
            }
        }

        protected virtual IHtmlNode Template()
        {
            var element = new HtmlElement(this.TagName, this.TagRenderMode)
                .Attributes(this.HtmlAttributes);
            this.Styles.ForEach(s => element.Css(s.Key.ToHyphenedName(), s.Value.ToString()));
            return element;
        }

        protected virtual void RenderMarkup(TextWriter writer)
        {
            this.Template().WriteTo(writer);
        }

        protected virtual string SerializeOptions()
        {
            return string.Empty;
        }

        protected virtual void RenderStartupScript(TextWriter writer)
        {
            var script = new HtmlElement("script")
                .Attributes(new { type = "text/javascript" })
                .Html(this.StartupScript());
            script.WriteTo(writer);
        }

        protected virtual bool IsDecorator()
        {
            return !string.IsNullOrEmpty(this.Selector);
        }

        protected virtual bool NeedRenderMarkup()
        {
            return !this.IsDecorator();
        }

        protected virtual string StartupScript()
        {
            var seletor = this.NeedRenderMarkup() ? "#" + this.ID : this.Selector;
            var instanceName = this.ID.ToCamelRemoveHyphen();
            var inner = string.Format("var {0} = new {1}('{2}', {3});", instanceName, this.ControlFullName, seletor, this.SerializeOptions());

            // jQuery way
            //return string.Format("$(function () {{ {0} }});", inner);
            // native way
            return string.Format("document.addEventListener('DOMContentLoaded', function () {{ {0} }});", inner);
        }

        protected virtual bool TakeCharge()
        {
            var cbk = new ControlCallbackManager(this.ViewContext.HttpContext);
            if (!cbk.IsCallback || cbk.ControlId != this.ID)
            {
                return false;
            }
            ProcessCallbackData(cbk);
            return true;
        }

        protected virtual void ProcessCallbackData(ControlCallbackManager cbk)
        {
            cbk.WriteJson(new
            {
                info = string.Format("[Callback Service] Control: {0} {1} | Command: {2} | Parameter count: {3}", cbk.ControlName, cbk.ControlId, cbk.CommandName, cbk.CommandParameter.Count),
                msg = string.Format(cbk.CommandParameter["msg"].ToString(), cbk.CommandParameter["obj"])
            });
        }

        protected virtual void Render(TextWriter output)
        {
            // check license here.

            this.GenerateIdIfNeeded();

            if (TakeCharge())
            {
                return;
            }
            if (this.NeedRenderMarkup())
            {
                RenderMarkup(output);
            }
            RenderStartupScript(output);
        }

        private void GenerateIdIfNeeded()
        {
            if (string.IsNullOrEmpty(this.ID))
            {
                int num = this.ViewContext.ViewData.ContainsKey(this.ControlFullName)
                    ? (int)this.ViewContext.ViewData[this.ControlFullName]
                    : 0;
                num++;
                this.ID = this.ControlName.ToCamel() + num;
                this.ViewContext.ViewData[this.ControlFullName] = num;
            }
        }

        string IHtmlString.ToHtmlString()
        {
            using (var output = new StringWriter())
            {
                Render(output);
                return output.ToString();
            }
        }
    }

    public enum MarkupRenderType
    {
        PureHtml,
        AngularDirective,
        KnockoutHtml
    }
}
