using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    public static class CollectionUtils
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static T[] Repeat<T>(Func<T> getNext, int count)
        {
            var r = new T[count];
            for (var i = 0; i < count; i++)
                r[i] = getNext();
            return r;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> forItem)
        {
            foreach (var item in items)
                forItem(item);
        }

        public static void RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> shouldRemove)
        {
            var toRemove = new List<T>();
            foreach (var item in collection)
            {
                if(shouldRemove(item))
                    toRemove.Add(item);
            }
            foreach (var item in toRemove)
            {
                collection.Remove(item);
            }
        }

        public static bool AreEquivalentNoOrder<T>(this IEnumerable<T> col1, IEnumerable<T> col2)
        {
            var col1Sorted = col1.OrderBy(i => i);
            var col2Sorted = col2.OrderBy(i => i);
            return Enumerable.SequenceEqual(col1Sorted, col2Sorted);
        }

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            return Batch(source, size, x => x);
        }

        public static IEnumerable<TElement> Randomize<TElement>(this IEnumerable<TElement> enumerable)
        {

        }

        public static IEnumerable<TResult> Batch<TSource, TResult>(this IEnumerable<TSource> source, int size,
            Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            switch (source)
            {
                case ICollection<TSource> collection when collection.Count <= size:
                {
                    return _();

                    IEnumerable<TResult> _()
                    {
                        var bucket = new TSource[collection.Count];
                        collection.CopyTo(bucket, 0);
                        yield return resultSelector(bucket);
                    }
                }
                case IReadOnlyList<TSource> list when list.Count <= size:
                {
                    return _();

                    IEnumerable<TResult> _()
                    {
                        var bucket = new TSource[list.Count];
                        for (var i = 0; i < list.Count; i++)
                            bucket[i] = list[i];
                        yield return resultSelector(bucket);
                    }
                }
                case IReadOnlyCollection<TSource> collection when collection.Count <= size:
                {
                    return Batch(collection.Count);
                }
                default:
                {
                    return Batch(size);
                }

                    IEnumerable<TResult> Batch(int size_)
                    {
                        TSource[] bucket = null;
                        var count = 0;

                        foreach (var item in source)
                        {
                            if (bucket == null)
                                bucket = new TSource[size_];

                            bucket[count++] = item;

                            // The bucket is fully buffered before it's yielded
                            if (count != size_)
                                continue;

                            yield return resultSelector(bucket);

                            bucket = null;
                            count = 0;
                        }

                        // Return the last bucket with all remaining elements
                        if (bucket != null && count > 0)
                        {
                            Array.Resize(ref bucket, count);
                            yield return resultSelector(bucket);
                        }
                    }
            }
        }

    }
}
