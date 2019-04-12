using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipwreck.Collections
{
    public abstract class VirtualizedCollection<T>
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

            public Task LoadingTask { get; set; }

            public void SetItems(IReadOnlyList<T> items)
            {

            }
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
            SyncRoot = new object();
            PageSize = pageSize;
            _Pages = new List<ItemPage>();
        }

        protected int PageSize { get; }

        #region Count

        private int? _Count;

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    if (_Count == null && _Pages.Count == 0)
                    {
                        BeginLoad(0, PageSize);
                    }
                    return _Count ?? 0;
                }
            }
            private set => _Count = value;
        }

        #endregion Count

        //public T this[int index]
        //{
        //    get
        //    {
        //    }
        //}

        protected void BeginLoad(int startIndex, int pageSize)
        {
            lock (SyncRoot)
            {
                GetOrCreatePage(startIndex, pageSize).LoadingTask = SearchAsync(startIndex, pageSize).ContinueWith(t =>
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
                                    _Pages[pageIndex] = aftPart = new ItemPage(this, lastIndex + 1, page.LastIndex - lastIndex);
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
                                prevPart = new ItemPage(this, page.StartIndex, startIndex - page.StartIndex);
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
    }
}