using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using C1.Web.Wijmo.Controls.Localization;

// reviews by jcc on 6/5/2014
// 1. DisabledIndices designer bug
// 2. hidden field redundancy
// 3. c1tabwidget error
// 4. callback support: tab header paging

namespace C1.Web.Wijmo.Controls.C1TabWidget
{
    [ToolboxData("<{0}:C1TabWidget runat=server></{0}:C1TabWidget>")]
    [ToolboxBitmap(typeof(C1TabWidget), "C1TabWidget.png")]
    [ParseChildren(true)]
    [PersistChildren(false)]
    [LicenseProvider]
#if ASP_NET4
    //[Designer("C1.Web.Wijmo.Controls.Design.C1TabWidget.C1TabWidgetDesigner, C1.Web.Wijmo.Controls.Design.4" + C1.Wijmo.Licensing.VersionConst.DesignerVer)]
#elif ASP_NET35
    [Designer("C1.Web.Wijmo.Controls.Design.C1Tabs.C1TabsDesigner, C1.Web.Wijmo.Controls.Design.3"+C1.Wijmo.Licensing.VersionConst.DesignerVer)]
#else
    [Designer("C1.Web.Wijmo.Controls.Design.C1Tabs.C1TabsDesigner, C1.Web.Wijmo.Controls.Design.2"+C1.Wijmo.Licensing.VersionConst.DesignerVer)]
#endif
    public partial class C1TabWidget : C1TargetControlBase, IPostBackDataHandler, ICallbackEventHandler
    {
        #region Fields

        protected TabPageCollection _tabPages = new TabPageCollection();
        private bool _productLicensed = false;
        private bool _shouldNag;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new instance of C1TabWidget.
        /// </summary>
        [C1Description("C1TabWidget.Constructor")]
        public C1TabWidget()
            : base(HtmlTextWriterTag.Div)
        {
            VerifyLicense();
        }

        #endregion

        #region Licensing

        internal virtual void VerifyLicense()
        {
            var licinfo = C1.Util.Licensing.ProviderInfo.Validate(typeof(C1TabWidget), this, false);
            _shouldNag = licinfo.ShouldNag;
#if GRAPECITY
            _productLicensed = licinfo.IsValid;
#else
            _productLicensed = licinfo.IsValid || licinfo.IsLocalHost;
#endif
        }

        #endregion

        #region Properties

        /// <summary>
        /// Pages collection
        /// </summary>
        [C1Category("Appearance")]
        [C1Description("C1TabWidget.TabPages")]
        [TypeConverter(typeof(CollectionConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [NotifyParentProperty(true)]
        public TabPageCollection TabPages
        {
            get
            {
                return _tabPages;
            }
        }

        /// <summary>
        /// Get or set the initial number of tabs to show.
        /// </summary>
        [C1Category("Options")]
        [C1Description("C1TabWidget.LoadSize")]
        [WidgetOption]
        [DefaultValue(4)]
        public int LoadSize
        {
            get
            {
                return GetPropertyValue("LoadSize", 4);
            }
            set
            {
                SetPropertyValue("LoadSize", value);
            }
        }

        /// <summary>
        /// Gets the uniqueID of this control
        /// </summary>
        [Json(true)]
        [Browsable(false)]
        public override string UniqueID
        {
            get
            {
                return base.UniqueID;
            }
        }

        #endregion

        #region Events

        public event EventHandler SelectChanged;
        protected virtual void OnSelectChanged(EventArgs e)
        {
            if (SelectChanged != null)
            {
                SelectChanged(this, e);
            }
        }

        public event EventHandler LoadTab;
        protected virtual void OnLoadTab(EventArgs e)
        {
            if (LoadTab != null)
            {
                LoadTab(this, e);
            }
        }

        #endregion

        #region Viewstates

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _tabPages.LoadViewState(ViewState["TabPages"]);
        }

        protected override object SaveViewState()
        {
            ViewState["TabPages"] = _tabPages.SaveViewState();
            return base.SaveViewState();
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            _tabPages.TrackViewState();
        }

        #endregion

        #region Render

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            C1.Web.Wijmo.Controls.Licensing.LicCheck.OnPreRenderCheckLicense(_productLicensed, Page, this);
            base.OnPreRender(e);

            Page.ClientScript.GetCallbackEventReference(this, null, null, null);
        }

        protected override void CreateChildControls()
        {
            if (this.TabPages.Count == 0) // for design time display
            {
                this.Controls.Add(new LiteralControl("TabWidget"));
            }
            else
            {
                var ul = new HtmlGenericControl("ul");
                foreach (var page in this.TabPages.Take(this.LoadSize))
                {
                    var li = new HtmlGenericControl("li");
                    var link = new HtmlAnchor { InnerText = page.Settings.Header, HRef = "#" };
                    li.Controls.Add(link);
                    ul.Controls.Add(li);
                }
                this.Controls.Add(ul);
                foreach (var page in this.TabPages.Take(this.LoadSize))
                {
                    var div = new HtmlGenericControl("div");
                    div.InnerText = page.Settings.Content;
                    this.Controls.Add(div);
                }
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            if (IsDesignMode)
            {
                writer.Write("<div>TabWidget</div>");
            }
        }

        #endregion

        #region IPostBackDataHandler members

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            var data = this.JsonSerializableHelper.GetJsonData(postCollection);
            var index0 = this.SelectedIndex;
            this.RestoreStateFromJson(data);
            var index = Convert.ToInt32(data["selectedIndex"]);
            return index != index0;
        }

        public void RaisePostDataChangedEvent()
        {
            OnSelectChanged(EventArgs.Empty);
        }

        #endregion

        #region C1 overrides

        #endregion

        #region ICallBackEventHandler members

        private string _callbackCommand;
        private string _callbackData;

        public string GetCallbackResult()
        {
            if (_callbackCommand == "showAllTabs")
            {
                var result = string.Join(",", this.TabPages
                    .Skip(this.LoadSize)
                    .Select(p => string.Format("{{ \"header\": \"{0}\", \"content\": \"{1}\" }}", p.Settings.Header, p.Settings.Content))
                    .ToArray());
                result = string.Format("[{0}]", result);
                return result;
            }
            return string.Empty;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            var data = JsonHelper.StringToObject(eventArgument);
            _callbackCommand = data["CommandName"].TryToString();
            _callbackData = data["CommandData"].TryToString();
            if (_callbackCommand == "showAllTabs")
            {
                OnLoadTab(EventArgs.Empty);
            }
        }

        #endregion
    }

    public static class MyExtensions
    {
        public static string TryToString(this object source)
        {
            return source == null ? string.Empty : source.ToString();
        }
    }
}
