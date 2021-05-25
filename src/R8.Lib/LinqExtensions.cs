using System;
using System.Collections.Generic;
using System.Linq;

namespace R8.Lib
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Reports specified index of given value in string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <param name="nth"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static int FindByIndex(this string str, string value, int nth = 1)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (nth <= 0)
                throw new ArgumentOutOfRangeException(nameof(nth), "Can not find the 0th index of substring in string. Must start with 1");

            var offset = str.IndexOf(value, StringComparison.Ordinal);
            for (var i = 1; i < nth; i++)
            {
                if (offset == -1)
                    return -1;
                offset = str.IndexOf(value, offset + 1, StringComparison.Ordinal);
            }
            return offset;
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key, with priority of given list.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="orderList"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByList<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, params TKey[] orderList)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var indexedList = Enumerable.Range(0, orderList.Length).ToDictionary(r => orderList[r], r => r);
            var orderedTestsSafe = source.OrderBy(item =>
            {
                var foundIndex = indexedList.TryGetValue(keySelector.Invoke(item), out var index);
                return foundIndex
                    ? index
                    : int.MaxValue;
            });
            return orderedTestsSafe;
        }

        /// <summary>
        /// Reports index of given predicate in a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="predicate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                    return retVal;

                retVal++;
            }
            return -1;
        }

        /// <summary>Produces the set difference of two sequences by using the default equality comparer to compare values.</summary>
        /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
        /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that are not also in <paramref name="second" /> will be returned.</param>
        /// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="first" /> or <paramref name="second" /> is null.</exception>
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, TSource second)
        {
            return first.Except(new List<TSource> { second });
        }

        /// <summary>
        /// Returns the element with the maximum value of a selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An IEnumerable collection values to determine the element with the maximum value of.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <exception cref="ArgumentNullException">source or keySelector is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <returns>The element in source with the maximum value of a selector function.</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MaxOrMinBy(source, keySelector, 1);

        /// <summary>
        /// Returns the element with the minimum value of a selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An IEnumerable collection values to determine the element with the minimum value of.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <exception cref="ArgumentNullException">source or keySelector is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <returns>The element in source with the minimum value of a selector function.</returns>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MaxOrMinBy(source, keySelector, -1);

        private static TSource MaxOrMinBy<TSource, TKey>
            (IEnumerable<TSource> source, Func<TSource, TKey> keySelector, int sign)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            var comparer = Comparer<TKey>.Default;
            var value = default(TKey);
            var result = default(TSource);

            var hasValue = false;

            foreach (var element in source)
            {
                var x = keySelector(element);
                if (x == null)
                    continue;

                if (!hasValue)
                {
                    value = x;
                    result = element;
                    hasValue = true;
                }
                else if (sign * comparer.Compare(x, value) > 0)
                {
                    value = x;
                    result = element;
                }
            }

            if (result != null && !hasValue)
                throw new InvalidOperationException("The source sequence is empty");

            return result;
        }
    }
}