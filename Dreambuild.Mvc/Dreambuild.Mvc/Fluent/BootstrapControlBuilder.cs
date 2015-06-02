using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dreambuild.Mvc
{
    public class BootstrapControlBuilder<TControl, TBuilder>
        : HtmlControlBuilderBase<TControl, TBuilder>
        where TControl : BootstrapControl
        where TBuilder : BootstrapControlBuilder<TControl, TBuilder>
    {
        public BootstrapControlBuilder(TControl control)
            : base(control)
        {
        }
    }
}
