using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGAHelper.Lib
{
    public static class EnumerableExtensions
    {
        public static (List<T> yes, List<T> no) SplitBy<T>(this IEnumerable<T> source,
            Func<T, bool> predicate)
        {
            var trueList = new List<T>();
            var falseList = new List<T>();
            foreach (var item in source)
            {
                if (predicate(item))
                    trueList.Add(item);
                else
                    falseList.Add(item);
            }

            return (trueList, falseList);
        }

        public static (List<TTrue> yes, List<TFalse> no) SplitBy<T, TTrue, TFalse>(this IEnumerable<T> source,
            Func<T, bool> predicate,
            Func<T, TTrue> trueSelector,
            Func<T, TFalse> falseSelector)
        {
            var trueList = new List<TTrue>();
            var falseList = new List<TFalse>();
            foreach (var item in source)
            {
                if (predicate(item))
                    trueList.Add(trueSelector(item));
                else
                    falseList.Add(falseSelector(item));
            }

            return (trueList, falseList);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            return dict.TryGetValue(key, out var retVal) ? retVal : defaultValue;
        }

        public static IReadOnlyCollection<TValue> GetValuesOrEmpty<TKey, TValue>(this IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>> dict, TKey key)
        {
            return dict.TryGetValue(key, out var val) ? val : new TValue[0];
        }

        public static IEnumerable<TElement> GetValuesOrEmpty<TKey, TElement>(this ILookup<TKey, TElement> lookup, TKey key)
        {
            return lookup.Contains(key) ? lookup[key] : new TElement[0];
        }

        public static bool IsRepeatedUniqueValue<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    return false;
                var first = e.Current;
                var comparer = EqualityComparer<T>.Default;
                while (e.MoveNext())
                    if (!comparer.Equals(e.Current, first))
                        return false;
                return true;
            }
        }

        public static void Sort<T, TKey>(this ObservableCollection<T> collection, Func<ObservableCollection<T>, TKey> sort)
        {
            var sorted = (sort.Invoke(collection) as IOrderedEnumerable<T>)?.ToArray();
            if (sorted == null)
                throw new ArgumentException("invalid sort", nameof(sort));

            for (var idx = 0; idx < sorted.Length; idx++)
            {
                if (ReferenceEquals(collection[idx], sorted[idx]))
                    continue;
                collection.Move(collection.IndexOf(sorted[idx]), idx);
            }
        }

        public static T MaxBy<T>(this IEnumerable<T> source, Func<T, double> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    return default;

                var maxKey = keySelector(e.Current);
                var maxValue = e.Current;
                while (e.MoveNext())
                {
                    var key = keySelector(e.Current);
                    if (key.CompareTo(maxKey) <= 0)
                        continue;

                    maxKey = key;
                    maxValue = e.Current;
                }

                return maxValue;
            }
        }
    }
}
