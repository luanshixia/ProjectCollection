﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dreambuild.Mvc
{
    public class HtmlControlBuilder : ControlBuilder<HtmlControl, HtmlControlBuilder>, IDisposable
    {
        public HtmlControlBuilder(HtmlControl control)
            : base(control)
        {
        }

        public HtmlControlBuilder Text(string text)
        {
            this._control.Text = text;
            return this;
        }

        public HtmlControlBuilder Content(Func<object, object> content)
        {
            this._control.Content = content;
            return this;
        }

        internal HtmlControlBuilder Begin()
        {
            _control.BeginControl();
            return this;
        }

        void IDisposable.Dispose()
        {
            _control.EndControl();
        }
    }
}