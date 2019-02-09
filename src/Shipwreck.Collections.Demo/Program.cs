using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Shipwreck.Collections.Demo
{
    internal class Program
    {
        private const int DATA_COUNT = 10000;
        private const int RUN_COUNT = 3;
        private const int SAMPLE_COUNT = 10000;

        private struct Comparer : IComparer<int>, IComparer<ValueClass>
        {
            public int Compare(int x, int y)
                => x - y;

            public int Compare(ValueClass x, ValueClass y)
                => x.Value - y.Value;
        }
        private class  ClassComparer : IComparer<int>, IComparer<ValueClass>
        {
            public int Compare(int x, int y)
                => x - y;

            public int Compare(ValueClass x, ValueClass y)
                => x.Value - y.Value;
        }

        private class ValueClass : IComparable<ValueClass>
        {
            public ValueClass(int value) => Value = value;
            public int Value { get; }

            public int CompareTo(ValueClass other)
                => Value - other.Value;
        }

        private static void Main(string[] args)
        {
            var r = new Random();

            var array = new int[DATA_COUNT];

            Console.WriteLine("Populating array of {0} int elements", array.Length);

            var prev = 0;
            for (var i = 1; i < array.Length; i++)
            {
                prev = array[i] = prev + r.Next(1, 10);
            }

            Console.WriteLine();

            var sw = new Stopwatch();
            TestArray(r, array, sw);
            TestArrayWithComparer(r, array, sw);
            TestList(r, array, sw);
            TestListWithComparer(r, array, sw);
            TestCollection(r, array, sw);
            TestCollectionWithComparer(r, array, sw);

            Console.WriteLine("Populating array of {0} references", array.Length);

            var sa = new ValueClass[array.Length];
            for (var i = 0; i < sa.Length; i++)
            {
                sa[i] = new ValueClass(array[i]);
            }
            Console.WriteLine();

            TestValueClassArrayWithComparer(r, sa, sw);
            TestValueClassListWithComparer(r, sa, sw);
            TestValueClassCollectionWithComparer(r, sa, sw);

            Console.WriteLine("Hit any key to exit");
            Console.Read();
        }

        #region int

        private static void TestArray(Random r, int[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing Array");

            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = r.Next(array.Last() + 1);

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = array.BinarySearch(v);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = Array.BinarySearch(array, v);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("Array: {0} vs ArrayBinarySearchHelper: {1} ({2:0%})", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }

        private static void TestArrayWithComparer(Random r, int[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing Array with IComparer<T>");

            IComparer<int> bc = new ClassComparer();
            var c = new Comparer();

            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = r.Next(array.Last() + 1);

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = array.BinarySearch(v, c);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = Array.BinarySearch(array, v, bc);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("Array: {0} vs ArrayBinarySearchHelper: {1} ({2:0%})", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }

        private static void TestList(Random r, int[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing List<T>");

            var list = new List<int>(array);
            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = r.Next(list.Last() + 1);

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = list.BinarySearch<int>(v);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = list.BinarySearch(v);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("List<T>: {0} vs ListBinarySearchHelper: {1} ({2:0%})", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }

        private static void TestListWithComparer(Random r, int[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing List<T> with IComparer<T>");

            var list = new List<int>(array);
            IComparer<int> bc = new ClassComparer();
            var c = new Comparer();

            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = r.Next(list.Last() + 1);

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = list.BinarySearch<int, Comparer>(v, c);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = list.BinarySearch(v, bc);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("List<T>: {0} vs ListBinarySearchHelper: {1} ({2:0%})", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }

        private static void TestCollection(Random r, int[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing Collection<T>");

            var list = new Collection<int>(array);
            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = r.Next(list.Last() + 1);

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = list.BinarySearch<int>(v);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = Array.BinarySearch(array, v);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("Array: {0} vs CollectionBinarySearchHelper: {1} ({2:0%} to Array)", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }

        private static void TestCollectionWithComparer(Random r, int[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing Collection<T> with IComparer<T>");

            var list = new Collection<int>(array);
            IComparer<int> bc = new ClassComparer();
            var c = new Comparer();

            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = r.Next(list.Last() + 1);

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = list.BinarySearch<int, Comparer>(v, c);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = Array.BinarySearch(array, v, bc);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("Array: {0} vs CollectionBinarySearchHelper: {1} ({2:0%} to Array)", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }

        #endregion int

        #region ValueClass

        private static void TestValueClassArrayWithComparer(Random r, ValueClass[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing Array with IComparer<T>");

            IComparer<ValueClass> bc = new ClassComparer();
            var c = new Comparer();

            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = new ValueClass(r.Next(array.Last().Value + 1));

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v.Value, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = array.BinarySearch(v, c);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = Array.BinarySearch(array, v, bc);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("Array: {0} vs ArrayBinarySearchHelper: {1} ({2:0%})", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }
         
        private static void TestValueClassListWithComparer(Random r, ValueClass[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing List<T> with IComparer<T>");

            var list = new List<ValueClass>(array);
            IComparer<ValueClass> bc = new ClassComparer();
            var c = new Comparer();

            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = new ValueClass(r.Next(array.Last().Value + 1));

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v.Value, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = list.BinarySearch<ValueClass, Comparer>(v, c);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = list.BinarySearch(v, bc);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("List<T>: {0} vs ListBinarySearchHelper: {1} ({2:0%})", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }

        private static void TestValueClassCollectionWithComparer(Random r, ValueClass[] array, Stopwatch sw)
        {
            Console.WriteLine("Testing Collection<T> with IComparer<T>");

            var list = new Collection<ValueClass>(array);
            IComparer<ValueClass> bc = new ClassComparer();
            var c = new Comparer();

            for (var i = 1; i <= RUN_COUNT; i++)
            {
                var v = new ValueClass(r.Next(array.Last().Value + 1));

                Console.WriteLine("{0}: BinarySearch {1} * {2} times ", i, v.Value, SAMPLE_COUNT);

                var at = TimeSpan.Zero;
                var ct = TimeSpan.Zero;

                for (var j = 0; j < SAMPLE_COUNT; j++)
                {
                    sw.Restart();
                    var cr = list.BinarySearch<ValueClass, Comparer>(v, c);
                    sw.Stop();
                    ct += sw.Elapsed;

                    sw.Restart();
                    var ar = Array.BinarySearch(array, v, bc);
                    sw.Stop();
                    at += sw.Elapsed;

                    if (ar != cr)
                    {
                        throw new ApplicationException();
                    }
                }

                Console.WriteLine("Array: {0} vs CollectionBinarySearchHelper: {1} ({2:0%} to Array)", at, ct, ct.Ticks / (double)at.Ticks);
            }

            Console.WriteLine();
        }

        #endregion string
    }
}