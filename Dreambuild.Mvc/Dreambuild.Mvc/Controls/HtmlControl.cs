using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dreambuild.Mvc
{
    public class HtmlControl : Control
    {
        protected string _tag;
        protected TagRenderMode _mode;

        public HtmlControl(HtmlHelper helper, string tag, TagRenderMode mode = TagRenderMode.Normal)
            : base(helper)
        {
            _tag = tag;
            _mode = mode;
            Content = obj => Text;
        }

        protected override string TagName
        {
            get
            {
                return _tag;
            }
        }

        protected override TagRenderMode TagRenderMode
        {
            get
            {
                return _mode;
            }
        }

        public string Text
        {
            get;
            set;
        }

        public Func<object, object> Content
        {
            get;
            set;
        }

        protected override IHtmlNode Template(TagRenderMode? mode)
        {
            return base.Template(mode).Html(Content(null).TryToString());
        }

        protected override void RenderStartupScript(System.IO.TextWriter writer)
        {

        }

        internal void BeginControl()
        {
            this.Template(TagRenderMode.StartTag).WriteTo(_htmlHelper.ViewContext.Writer);
        }

        internal void EndControl()
        {
            _htmlHelper.ViewContext.Writer.Write("</{0}>", TagName);
        }
    }

    public interface IChildElement<T>
        where T: HtmlControl
    {
        T Parent { get; }
    }
}
