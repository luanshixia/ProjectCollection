using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
#if EXTENDER
using C1.Web.Wijmo.Extenders.Localization;

[assembly: TagPrefix("C1.Web.Wijmo.Extenders.C1TabWidget", "wijmo")]

namespace C1.Web.Wijmo.Extenders.C1TabWidget
#else
using C1.Web.Wijmo.Controls.Localization;

[assembly: TagPrefix("C1.Web.Wijmo.Controls.C1TabWidget", "wijmo")]

namespace C1.Web.Wijmo.Controls.C1TabWidget
#endif
{
    [WidgetDependencies(
        WidgetDependenciesResolver.WIJMO_OPEN_JS,
        WidgetDependenciesResolver.WIJMO_OPEN_CSS
#if !EXTENDER
, "extensions.c1tabwidget.js" // C1.Web.Wijmo.Controls.Resources.wijmo.extensions.c1tabwidget.js
#endif
)]
#if EXTENDER
    public partial class C1TabWidgetExtender
#else
	public partial class C1TabWidget
#endif
    {
        #region ** fields

        #endregion

        #region ** options

        /// <summary>
        /// Determines the event that triggers the tab switch.
        /// </summary>
        [C1Description("C1TabWidget.Trigger", "Determines the event that triggers the tab switch.")]
        [C1Category("Options")]
        [WidgetOption]
        [DefaultValue(TriggerMethod.Click)]
        public TriggerMethod Trigger
        {
            get
            {
                return GetPropertyValue("Trigger", TriggerMethod.Click);
            }
            set
            {
                SetPropertyValue("Trigger", value);
            }
        }

        /// <summary>
        /// Get or set the index of the selected tab.
        /// </summary>
        [C1Description("C1TabWidget.SelectedIndex", "Get or set the index of the selected tab.")]
        [C1Category("Options")]
        [WidgetOption]
        [Json(true)]
        [DefaultValue(0)]
        public int SelectedIndex
        {
            get
            {
                return GetPropertyValue("SelectedIndex", 0);
            }
            set
            {
                SetPropertyValue("SelectedIndex", value);
            }
        }

        /// <summary>
        /// Get or set the list of indices of the disabled tabs.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [C1Description("C1TabWidget.DisabledIndices", "Get or set the list of indices of the disabled tabs.")]
        [C1Category("Options")]
        [WidgetOption]
        [CollectionItemType(typeof(int))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> DisabledIndices
        {
            get
            {
                return GetPropertyValue("DisabledIndices", new List<int>());
            }
            set
            {
                SetPropertyValue("DisabledIndices", value);
            }
        }

        #endregion

        #region ** events

        /// <summary>
        /// A function called when a tab is selected.
        /// </summary>
        [C1Description("C1Tabs.OnClientAdd")]
        [Category("Client Events")]
        [WidgetOption]
        [WidgetEvent("e, data")]
        [DefaultValue("")]
        public string OnSelect
        {
            get
            {
                return GetPropertyValue("OnSelect", string.Empty);
            }
            set
            {
                SetPropertyValue("OnSelect", value);
            }
        }

        #endregion
    }

    /// <summary>
    /// Tab switching method.
    /// </summary>
    public enum TriggerMethod
    {
        /// <summary>
        /// Switch tab by click.
        /// </summary>
        Click,
        /// <summary>
        /// Switch tab by hover.
        /// </summary>
        MouseOver
    }
}
