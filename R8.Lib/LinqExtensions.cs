using System;
using System.Collections.Generic;

namespace R8.Lib
{
    public static class LinqExtensions
    {
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

        /// <summary>
        /// Returns the element with the maximum value of a selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An IEnumerable collection values to determine the element with the maximum value of.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <exception cref="System.ArgumentNullException">source or keySelector is null.</exception>
        /// <exception cref="System.InvalidOperationException">source contains no elements.</exception>
        /// <returns>The element in source with the maximum value of a selector function.</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MaxOrMinBy(source, keySelector, 1);

        /// <summary>
        /// Returns the element with the minimum value of a selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An IEnumerable collection values to determine the element with the minimum value of.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <exception cref="System.ArgumentNullException">source or keySelector is null.</exception>
        /// <exception cref="System.InvalidOperationException">source contains no elements.</exception>
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