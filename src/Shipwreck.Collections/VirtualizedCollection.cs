using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Shipwreck.Collections
{
    public abstract class VirtualizedCollection<T> : INotifyPropertyChanged, IList<T>, IReadOnlyList<T>
        where T : class
    {
        protected struct SearchResult
        {
            public SearchResult(int totalCount, int startIndex, IReadOnlyList<T> items)
            {
                TotalCount = totalCount;
                StartIndex = startIndex;
                Items = items;
            }

            public int TotalCount { get; }
            public int StartIndex { get; }
            public IReadOnlyList<T> Items { get; }
        }

        protected class ItemPage
        {
            public ItemPage(VirtualizedCollection<T> collection, int startIndex, int length)
            {
                Collection = collection;
                StartIndex = startIndex;
                Length = length;
            }

            public VirtualizedCollection<T> Collection { get; }
            public int StartIndex { get; }
            public int Length { get; }
            public int LastIndex => StartIndex + Length - 1;

            private T[] _Items;

            public T[] Items
                => _Items ?? (_Items = new T[Length]);

            public bool HasItems
                => _Items != null;

            private Task _LoadingTask;

            public Task LoadingTask
            {
                get => _LoadingTask;
                set
                {
                    _LoadingTask = value;
                    State = value == null ? PageState.NotLoaded : PageState.Loading;
                }
            }

            public PageState State { get; internal set; }

            public void SetItems(IReadOnlyList<T> items)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    Items[i] = items[i];
                }
                State = PageState.Loaded;
            }

            public void Invalidate() => State = PageState.Invalid;
        }

        protected enum PageState
        {
            NotLoaded,
            Loading,
            Loaded,
            Invalid
        }

        protected class ItemPageComparer : IComparer<ItemPage>
        {
            public static ItemPageComparer Default { get; } = new ItemPageComparer();

            public int Compare(ItemPage x, ItemPage y)
                => x.StartIndex - y.StartIndex;
        }

        public object SyncRoot { get; }
        private readonly List<ItemPage> _Pages;

        protected VirtualizedCollection(int pageSize)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }
            SyncRoot = new object();
            _PageSize = pageSize;
            _Pages = new List<ItemPage>();
        }

        #region PageSize

        private int _PageSize;

        public int PageSize
        {
            get => _PageSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                lock (SyncRoot)
                {
                    if (value != _PageSize)
                    {
                        _PageSize = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageSize)));
                    }
                }
            }
        }

        #endregion PageSize

        #region Count

        private int? _Count;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    if (_Count == null && _Pages.Count == 0)
                    {
                        BeginLoad(GetPageFor(0));
                    }
                    return _Count ?? 0;
                }
            }
            private set
            {
                lock (SyncRoot)
                {
                    if ((_Count ?? 0) != value)
                    {
                        _Count = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    }
                }
            }
        }

        #endregion Count

        public T this[int index]
        {
            get
            {
                lock (SyncRoot)
                {
                    var p = GetPageFor(index);
                    if (p.State == PageState.NotLoaded
                        || p.State == PageState.Invalid)
                    {
                        BeginLoad(p = GetOrCreatePage(index));
                    }
                    return p.Items[index - p.StartIndex];
                }
            }
        }

        private void BeginLoad(ItemPage page)
        {
            page.LoadingTask = SearchAsync(page.StartIndex, page.Length).ContinueWith(t =>
            {
                var r = t.Result;
                lock (SyncRoot)
                {
                    Count = r.TotalCount;
                    if (r.Items?.Count > 0)
                    {
                        GetOrCreatePage(r.StartIndex, r.Items.Count).SetItems(r.Items);
                    }
                }
            });
        }

        private ItemPage GetPageFor(int itemIndex)
        {
            var newPage = new ItemPage(this, itemIndex, 1);
            lock (SyncRoot)
            {
                var pageIndex = _Pages.BinarySearch(newPage, ItemPageComparer.Default);
                if (pageIndex >= 0)
                {
                    return _Pages[pageIndex];
                }
                else
                {
                    pageIndex = ~pageIndex;
                    if (pageIndex > 0)
                    {
                        var pp = _Pages[pageIndex - 1];
                        if (pp.StartIndex <= itemIndex && itemIndex <= pp.LastIndex)
                        {
                            return pp;
                        }
                    }
                }
                return GetOrCreatePage(itemIndex);
            }
        }

        private ItemPage GetOrCreatePage(int itemIndex)
        {
            var pi = itemIndex / PageSize;
            return GetOrCreatePage(pi, PageSize);
        }

        private ItemPage GetOrCreatePage(int startIndex, int pageSize)
        {
            var newPage = new ItemPage(this, startIndex, pageSize);
            lock (SyncRoot)
            {
                var pageIndex = _Pages.BinarySearch(newPage, ItemPageComparer.Default);
                if (pageIndex >= 0 && _Pages[pageIndex].Length == pageSize)
                {
                    return _Pages[pageIndex];
                }

                var lastIndex = startIndex + pageSize - 1;
                if (pageIndex < 0)
                {
                    pageIndex = ~pageIndex;

                    if ((pageIndex == 0 || _Pages[pageIndex - 1].LastIndex < startIndex)
                        && (pageIndex >= _Pages.Count || _Pages[pageIndex].StartIndex > lastIndex))
                    {
                        _Pages.Insert(pageIndex, newPage);
                        return newPage;
                    }
                }

                while (pageIndex < _Pages.Count - 1)
                {
                    var p = _Pages[pageIndex + 1];
                    if (p.StartIndex <= lastIndex)
                    {
                        pageIndex--;
                    }
                }

                for (; pageIndex >= 0; pageIndex--)
                {
                    ItemPage prevPart = null;
                    ItemPage aftPart = null;
                    var page = _Pages[pageIndex];

                    if (page.LastIndex < startIndex)
                    {
                        break;
                    }
                    if (page.LastIndex < lastIndex)
                    {
                        _Pages.RemoveAt(pageIndex);
                    }

                    for (var i = page.Length - 1; i >= 0; i--)
                    {
                        var ei = page.StartIndex + i;

                        var e = page.HasItems ? page.Items[i] : null;
                        if (ei > lastIndex)
                        {
                            if (e != null)
                            {
                                if (aftPart == null)
                                {
                                    _Pages[pageIndex] = aftPart = new ItemPage(this, lastIndex + 1, page.LastIndex - lastIndex)
                                    {
                                        LoadingTask = page.LoadingTask,
                                        State = page.State
                                    };
                                }
                                aftPart.Items[ei - aftPart.StartIndex] = e;
                            }
                        }
                        else if (startIndex <= ei)
                        {
                            if (ei == lastIndex)
                            {
                                if (aftPart == null)
                                {
                                    _Pages[pageIndex] = newPage;
                                }
                                else
                                {
                                    _Pages.Insert(pageIndex, newPage);
                                }
                            }

                            newPage.Items[ei - startIndex] = e;
                        }
                        else if (e != null)
                        {
                            if (prevPart == null)
                            {
                                prevPart = new ItemPage(this, page.StartIndex, startIndex - page.StartIndex)
                                {
                                    LoadingTask = page.LoadingTask,
                                    State = page.State
                                };
                                _Pages.Insert(--pageIndex, prevPart);
                            }
                            prevPart.Items[ei - page.StartIndex] = e;
                        }
                    }
                }

                return newPage;
            }
        }

        protected abstract Task<SearchResult> SearchAsync(int startIndex, int pageSize);

        #region IList<T>

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        int IList<T>.IndexOf(T item)
        {
            // TODO: support item=null

            lock (SyncRoot)
            {
                foreach (var p in _Pages)
                {
                    if (p.HasItems
                        && p.State != PageState.Invalid)
                    {
                        var li = Array.IndexOf(p.Items, item);
                        if (li >= 0)
                        {
                            return p.StartIndex + li;
                        }
                    }
                }
            }
            return -1;
        }

        void IList<T>.Insert(int index, T item)
            => throw new NotSupportedException();

        void IList<T>.RemoveAt(int index)
            => throw new NotSupportedException();

        #endregion IList<T>

        #region ICollection<T>

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item)
            => throw new NotSupportedException();

        void ICollection<T>.Clear()
            => throw new NotSupportedException();

        bool ICollection<T>.Contains(T item)
        {
            // TODO: support item=null

            lock (SyncRoot)
            {
                foreach (var p in _Pages)
                {
                    if (p.HasItems
                        && p.State != PageState.Invalid
                        && Array.IndexOf(p.Items, item) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            var s = 0;
            lock (SyncRoot)
            {
                foreach (var p in _Pages)
                {
                    if (p.HasItems)
                    {
                        if (p.StartIndex > s)
                        {
                            Array.Clear(array, s + arrayIndex, p.StartIndex - s);
                        }
                        Array.Copy(p.Items, 0, array, arrayIndex + p.StartIndex, p.Length);
                    }
                    else
                    {
                        Array.Clear(array, s + arrayIndex, p.LastIndex - s + 1);
                    }
                    s = p.LastIndex + 1;
                }
                if (s < Count)
                {
                    Array.Clear(array, s + arrayIndex, Count - s);
                }
            }
        }

        bool ICollection<T>.Remove(T item)
            => throw new NotSupportedException();

        #endregion ICollection<T>

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            lock (SyncRoot)
            {
                var c = Count;
                for (var i = 0; i < c; i++)
                {
                    yield return this[i];
                }
            }
        }

        #endregion IEnumerable<T>

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable
    }
}