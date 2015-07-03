using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;

namespace C1.Web.Wijmo.Controls.C1TabWidget
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TabPage : IStateManager
    {
        #region Fields

        private bool _marked = false;

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the properties of the tab page.
        /// </summary>
        [Category("Default")]
        [Description("TabPage settings")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        public TabPageSetting Settings { get; set; }

        #endregion

        #region Constructors

        public TabPage()
        {
            Settings = new TabPageSetting();
        }

        public TabPage(string header, string content)
        {
            Settings = new TabPageSetting { Header = header, Content = content };
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("{0}|{1}", Settings.Header, Settings.Content);
        }

        #endregion

        #region IStateManager members

        bool IStateManager.IsTrackingViewState
        {
            get { return this._marked; }
        }

        public void LoadViewState(object state)
        {
            if (state != null)
            {
                this.Settings.LoadViewState(state);
            }
        }

        public object SaveViewState()
        {
            return this.Settings.SaveViewState();
        }

        public void TrackViewState()
        {
            this._marked = true;
            this.Settings.TrackViewState();
        }

        #endregion
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TabPageSetting : IStateManager
    {
        #region Fields

        private string _header;
        private string _content;
        private bool _marked = false;
        private bool _isDirty = false;

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the text on the tab page header.
        /// </summary>
        public string Header
        {
            get { return this._header; }
            set
            {
                this._header = value;
                if (this._marked)
                {
                    this._isDirty = true;
                }
            }
        }

        /// <summary>
        /// Get or set the content text of the tab page.
        /// </summary>
        public string Content
        {
            get { return this._content; }
            set
            {
                this._content = value;
                if (this._marked)
                {
                    this._isDirty = true;
                }
            }
        }

        #endregion

        #region IStateManager members

        bool IStateManager.IsTrackingViewState
        {
            get { return this._marked; }
        }

        public void LoadViewState(object state)
        {
            if (state != null)
            {
                var pair = state as Pair;
                this._header = pair.First as string;
                this._content = pair.Second as string;
            }
        }

        public object SaveViewState()
        {
            if (_isDirty)
            {
                return new Pair(this._header, this._content);
            }
            return null;
        }

        public void TrackViewState()
        {
            this._marked = true;
        }

        #endregion
    }
}
