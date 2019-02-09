using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
using System.Runtime;
#endif

namespace Shipwreck.Collections
{
#if SHIPWRECK_COLLECTIONS_PUBLIC

    public
#endif
    interface IBinarySearchable<T> : IReadOnlyList<T>, IComparer<T>
    { }

#if SHIPWRECK_COLLECTIONS_PUBLIC

    public
#endif
    static class BinarySearchHelper
    {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
        [TargetedPatchingOptOut("")]
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T, TSource>(this TSource source, T value)
            where TSource : IBinarySearchable<T>
            => BinarySearch(source, 0, source.Count, value);

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
        [TargetedPatchingOptOut("")]
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T, TSource>(this TSource source, int index, int length, T value)
            where TSource : IBinarySearchable<T>
        {
            var lo = index;
            var hi = index + length - 1;
            while (lo <= hi)
            {
                var i = lo + ((hi - lo) >> 1);
                var order = source.Compare(source[i], value);

                if (order == 0)
                {
                    return i;
                }

                if (order < 0)
                {
                    lo = i + 1;
                }
                else
                {
                    hi = i - 1;
                }
            }

            return ~lo;
        }
    }

#if !SHIPWRECK_COLLECTIONS_NO_ARRAY

#if SHIPWRECK_COLLECTIONS_PUBLIC

    public
#endif
    static class ArrayBinarySearchHelper
    {
        private struct ComparableWrapper<T> : IBinarySearchable<T>
            where T : IComparable<T>
        {
            private readonly T[] _List;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparableWrapper(T[] list)
                => _List = list;

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Length;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => x.CompareTo(y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T>(this T[] source, T value)
            where T : IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), value);

        public static int BinarySearch<T>(this T[] source, int index, int length, T value)
            where T : IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), index, length, value);

        #region IComparer<T>

        private struct ComparerWrapper<T, TComparer> : IBinarySearchable<T>
            where TComparer : IComparer<T>
        {
            private readonly T[] _List;
            private TComparer _Comparer;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparerWrapper(T[] list, TComparer comparer)
            {
                _List = list;
                _Comparer = comparer;
            }

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Length;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => _Comparer.Compare(x, y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T, TComparer>(this T[] source, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), value);

        public static int BinarySearch<T, TComparer>(this T[] source, int index, int length, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), index, length, value);

        #endregion IComparer<T>
    }

#endif

#if !SHIPWRECK_COLLECTIONS_NO_LIST

#if SHIPWRECK_COLLECTIONS_PUBLIC

    public
#endif
    static class ListBinarySearchHelper
    {
        private struct ComparableWrapper<T> : IBinarySearchable<T>
            where T : IComparable<T>
        {
            private readonly List<T> _List;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparableWrapper(List<T> list)
                => _List = list;

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Count;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => x.CompareTo(y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T>(this List<T> source, T value) where T : IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), value);

        public static int BinarySearch<T>(this List<T> source, int index, int length, T value) where T : IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), index, length, value);

        #region IComparer<T>

        private struct ComparerWrapper<T, TComparer> : IBinarySearchable<T>
            where TComparer : IComparer<T>
        {
            private readonly List<T> _List;
            private TComparer _Comparer;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparerWrapper(List<T> list, TComparer comparer)
            {
                _List = list;
                _Comparer = comparer;
            }

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Count;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => _Comparer.Compare(x, y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T, TComparer>(this List<T> source, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), value);

        public static int BinarySearch<T, TComparer>(this List<T> source, int index, int length, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), index, length, value);

        #endregion IComparer<T>
    }

#endif

#if !SHIPWRECK_COLLECTIONS_NO_COLLECTION

#if SHIPWRECK_COLLECTIONS_PUBLIC

    public
#endif
    static class CollectionBinarySearchHelper
    {
        private struct ComparableWrapper<T> : IBinarySearchable<T>
            where T : IComparable<T>
        {
            private readonly Collection<T> _List;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparableWrapper(Collection<T> list)
                => _List = list;

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Count;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => x.CompareTo(y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T>(this Collection<T> source, T value) where T : struct, IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), value);

        public static int BinarySearch<T>(this Collection<T> source, int index, int length, T value) where T : struct, IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), index, length, value);

        #region IComparer<T>

        private struct ComparerWrapper<T, TComparer> : IBinarySearchable<T>
            where TComparer : IComparer<T>
        {
            private readonly Collection<T> _List;
            private TComparer _Comparer;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparerWrapper(Collection<T> list, TComparer comparer)
            {
                _List = list;
                _Comparer = comparer;
            }

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Count;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => _Comparer.Compare(x, y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T, TComparer>(this Collection<T> source, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), value);

        public static int BinarySearch<T, TComparer>(this Collection<T> source, int index, int length, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), index, length, value);

        #endregion IComparer<T>
    }

#endif

#if !SHIPWRECK_COLLECTIONS_NO_ILIST

#if SHIPWRECK_COLLECTIONS_PUBLIC

    public
#endif
    static class IListBinarySearchHelper
    {
        private struct ComparableWrapper<T> : IBinarySearchable<T>
            where T : IComparable<T>
        {
            private readonly IList<T> _List;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparableWrapper(IList<T> list)
                => _List = list;

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Count;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => x.CompareTo(y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T>(this IList<T> source, T value) where T : IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), value);

        public static int BinarySearch<T>(this IList<T> source, int index, int length, T value) where T : IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), index, length, value);

        #region IComparer<T>

        private struct ComparerWrapper<T, TComparer> : IBinarySearchable<T>
            where TComparer : IComparer<T>
        {
            private readonly IList<T> _List;
            private TComparer _Comparer;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparerWrapper(IList<T> list, TComparer comparer)
            {
                _List = list;
                _Comparer = comparer;
            }

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Count;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => _Comparer.Compare(x, y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T, TComparer>(this IList<T> source, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), value);

        public static int BinarySearch<T, TComparer>(this IList<T> source, int index, int length, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), index, length, value);

        #endregion IComparer<T>
    }

#endif

#if !SHIPWRECK_COLLECTIONS_NO_IREADONLYLIST

#if SHIPWRECK_COLLECTIONS_PUBLIC

    public
#endif
    static class IReadOnlyListBinarySearchHelper
    {
        private struct ComparableWrapper<T> : IBinarySearchable<T>
            where T : IComparable<T>
        {
            private readonly IReadOnlyList<T> _List;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparableWrapper(IReadOnlyList<T> list)
                => _List = list;

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Count;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => x.CompareTo(y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T>(this IReadOnlyList<T> source, T value) where T : IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), value);

        public static int BinarySearch<T>(this IReadOnlyList<T> source, int index, int length, T value) where T : IComparable<T>
            => BinarySearchHelper.BinarySearch(new ComparableWrapper<T>(source), index, length, value);

        #region IComparer<T>

        private struct ComparerWrapper<T, TComparer> : IBinarySearchable<T>
            where TComparer : IComparer<T>
        {
            private readonly IReadOnlyList<T> _List;
            private TComparer _Comparer;

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComparerWrapper(IReadOnlyList<T> list, TComparer comparer)
            {
                _List = list;
                _Comparer = comparer;
            }

            public int Count
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List.Count;
            }

            public T this[int index]
            {
#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
                [TargetedPatchingOptOut("")]
#endif
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _List[index];
            }

#if !SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT
            [TargetedPatchingOptOut("")]
#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y)
                => _Comparer.Compare(x, y);

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)_List.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _List.GetEnumerator();
        }

        public static int BinarySearch<T, TComparer>(this IReadOnlyList<T> source, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), value);

        public static int BinarySearch<T, TComparer>(this IReadOnlyList<T> source, int index, int length, T value, TComparer comparer)
            where TComparer : IComparer<T>
            => BinarySearchHelper.BinarySearch(new ComparerWrapper<T, TComparer>(source, comparer), index, length, value);

        #endregion IComparer<T>
    }

#endif
}