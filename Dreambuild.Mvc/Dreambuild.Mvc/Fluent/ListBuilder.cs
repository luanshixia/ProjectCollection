using System;
using System.Collections.Generic;

namespace Dreambuild.Mvc
{
    public class ListBuilder<TItem, TBuilder>
        where TItem : new()
        where TBuilder : IBuilder<TItem>, new()
    {
        private List<TItem> _list;

        public ListBuilder(List<TItem> list)
        {
            if (list == null)
            {
                list = new List<TItem>();
            }
            _list = list;
        }

        public TBuilder Add() // added by WangYang@20140715
        {
            TItem item = new TItem();
            TBuilder builder = new TBuilder();
            ((IBuilder<TItem>)builder).Object = item;
            _list.Add(item);
            return builder;
        }

        public ListBuilder<TItem, TBuilder> Add(Action<TBuilder> action)
        {
            TItem item = new TItem();
            TBuilder builder = new TBuilder();
            ((IBuilder<TItem>)builder).Object = item;
            action(builder);
            _list.Add(item);
            return this;
        }
    }
}
