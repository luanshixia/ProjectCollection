using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dreambuild.Mvc
{
    public class HtmlControlBuilderBase<TControl, TBuilder>
        : ControlBuilder<TControl, TBuilder>, IDisposable
        where TControl : HtmlControl
        where TBuilder : HtmlControlBuilderBase<TControl, TBuilder>
    {
        public HtmlControlBuilderBase(TControl control)
            : base(control)
        {
        }

        public TBuilder Text(string text)
        {
            this._control.Text = text;
            return this as TBuilder;
        }

        public TBuilder Content(Func<object, object> content)
        {
            this._control.Content = content;
            return this as TBuilder;
        }

        internal TBuilder Begin()
        {
            _control.BeginControl();
            return this as TBuilder;
        }

        void IDisposable.Dispose()
        {
            _control.EndControl();
        }
    }

    public class HtmlControlBuilder : HtmlControlBuilderBase<HtmlControl, HtmlControlBuilder>
    {
        public HtmlControlBuilder(HtmlControl control)
            : base(control)
        {
        }
    }
}
