using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;

namespace C1.Web.Wijmo.Controls.C1TabWidget
{
    public class TabPageCollection : Collection<TabPage>, IStateManager
    {
        private bool _marked = false;
        private bool _saveAll = false;

        public TabPageCollection()
        {
        }

        public TabPageCollection(IEnumerable<TabPage> collection)
        {
            collection.ToList().ForEach(x => this.Add(x));
        }

        #region Collection<> members

        protected override void ClearItems()
        {
            base.ClearItems();
            if (this._marked)
            {
                this._saveAll = true;
            }
        }

        protected override void InsertItem(int index, TabPage item)
        {
            base.InsertItem(index, item);
            if (this._marked)
            {
                this._saveAll = true;
            }
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            if (this._marked)
            {
                this._saveAll = true;
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
                if (state is List<TabPageSetting>) // save all, load all
                {
                    base.Clear();
                    var stateList = state as List<TabPageSetting>;
                    foreach (var data in stateList)
                    {
                        var page = new TabPage();
                        page.Settings = data;
                        this.Add(page);
                    }
                }
                else if (state is Dictionary<int, object>) // save change, load change
                {
                    var dict = state as Dictionary<int, object>;
                    foreach (var entry in dict)
                    {
                        int index = entry.Key;
                        var data = entry.Value;
                        if (index < this.Count)
                        {
                            this[index].LoadViewState(data);
                        }
                        else
                        {
                            var page = new TabPage();
                            page.LoadViewState(data);
                            this.Add(page);
                        }
                    }
                }
            }
        }

        public object SaveViewState()
        {
            if (_saveAll)
            {
                return this.Select(x => x.Settings).ToList(); // do not use child SaveViewState() here
            }
            return this
                .Select((x, i) => Tuple.Create(x.SaveViewState(), i))
                .Where(x => x.Item1 != null)
                .ToDictionary(x => x.Item2, x => x.Item1);
        }

        public void TrackViewState()
        {
            this._marked = true;
            this.ToList().ForEach(x => x.TrackViewState());
        }

        #endregion
    }
}
