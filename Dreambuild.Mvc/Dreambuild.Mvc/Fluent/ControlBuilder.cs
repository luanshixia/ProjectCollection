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
        private TControl _control;

        public ControlBuilder(TControl control)
        {
            this._control = control;
        }

        string IHtmlString.ToHtmlString()
        {
            return (_control as IHtmlString).ToHtmlString();
        }
    }
}
