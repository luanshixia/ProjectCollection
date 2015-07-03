using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace C1.Web.Wijmo.Extenders.C1TabWidget
{
    /// <summary>
    /// Extender for the tabwidget widget.
    /// </summary>
    [TargetControlType(typeof(Panel))]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(C1TabWidgetExtender), "tabwidget.png")]
    [LicenseProvider]
    public partial class C1TabWidgetExtender : WidgetExtenderControlBase
    {
        private bool _productLicensed = false;

		/// <summary>
        /// Initializes a new instance of the <see cref="C1TabWidgetExtender"/> class.
		/// </summary>
        public C1TabWidgetExtender()
			: base()
		{
			VerifyLicense();
		}

		private void VerifyLicense()
		{
            var licinfo = C1.Util.Licensing.ProviderInfo.Validate(typeof(C1TabWidgetExtender), this);
#if GRAPECITY
			_productLicensed = licinfo.IsValid;
#else
			_productLicensed = licinfo.IsValid || licinfo.IsLocalHost;
#endif
		}

		/// <summary>
		/// Raises the <see cref="M:System.Web.UI.Control.OnPreRender(System.EventArgs)"/> event and registers the extender control with the <see cref="T:System.Web.UI.ScriptManager"/> control.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			C1.Web.Wijmo.Extenders.Licensing.LicCheck.OnPreRenderCheckLicense(_productLicensed, Page, this);
			base.OnPreRender(e);
		}

		/// <summary>
		/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered in the browser window.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			C1.Web.Wijmo.Extenders.Licensing.LicCheck.RenderLicenseWebComment(writer);
			base.Render(writer);
		}
    }
}
