using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dreambuild.Mvc
{
    public class BootstrapControl : HtmlControl
    {
        public BootstrapControl(HtmlHelper helper, string tag, TagRenderMode mode)
            : base(helper, tag, mode)
        {
        }

        protected virtual string FrameworkClasses
        {
            get
            {
                return string.Empty;
            }
        }

        protected override void Render(System.IO.TextWriter output)
        {
            var classes = this.FrameworkClasses.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Array.ForEach(classes, c => this.CssClasses.Add(c));

            base.Render(output);
        }
    }

    public class BootstrapContainer : BootstrapControl
    {
        private const string bs_container = "container";
        private const string bs_container_fluid = "container-fluid";

        public BootstrapContainer(HtmlHelper helper)
            : base(helper, "div", TagRenderMode.Normal)
        {
        }

        protected override string FrameworkClasses
        {
            get
            {
                return Fluid ? bs_container_fluid : bs_container;
            }
        }

        public bool Fluid
        {
            get;
            set;
        }
    }

    public class BootstrapRow : BootstrapControl, IChildElement<BootstrapContainer>
    {
        private const string bs_row = "row";

        public BootstrapRow(HtmlHelper helper)
            : base(helper, "div", TagRenderMode.Normal)
        {
        }

        public BootstrapContainer Parent
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class DeviceSize
    {
        public const string ExtraSmall = "xs";
        public const string Small = "sm";
        public const string Middle = "md";
        public const string Large = "lg";
    }

    public class BootstrapCol : BootstrapControl, IChildElement<BootstrapRow>
    {
        private const string bs_col = "col-{0}-{1}";

        public BootstrapCol(HtmlHelper helper)
            : base(helper, "div", TagRenderMode.Normal)
        {
        }

        public int ExtraSmall
        {
            get;
            set;
        }

        public int Small
        {
            get;
            set;
        }

        public int Middle
        {
            get;
            set;
        }

        public int Large
        {
            get;
            set;
        }

        protected override string FrameworkClasses
        {
            get
            {
                string xs = ExtraSmall > 0 ? string.Format(bs_col, DeviceSize.ExtraSmall, ExtraSmall) : null;
                string sm = ExtraSmall > 0 ? string.Format(bs_col, DeviceSize.Small, Small) : null;
                string md = ExtraSmall > 0 ? string.Format(bs_col, DeviceSize.Middle, Middle) : null;
                string lg = ExtraSmall > 0 ? string.Format(bs_col, DeviceSize.Large, Large) : null;
                var array = new[] { xs, sm, md, lg }.Where(x => x != null).ToArray();
                return string.Join(" ", array);
            }
        }

        public BootstrapRow Parent
        {
            get { throw new NotImplementedException(); }
        }
    }
}
