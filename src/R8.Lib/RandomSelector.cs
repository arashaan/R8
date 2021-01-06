using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace R8.Lib
{
    public static class RandomSelector
    {
        /// <summary>
        /// Represents an specific-length digits
        /// </summary>
        /// <param name="length">An <see cref="int"/> value that representing digits length</param>
        /// <returns>An <see cref="string"/> value that representing generated token</returns>
        public static string GenerateDigitToken(int length = 6)
        {
            if (length < 4)
                throw new Exception($"{length} must be equal-greater than 4");

            var strMin = "";
            var strMax = "";
            for (var i = 0; i < length; i++)
            {
                strMin += "1";
                strMax += "9";
            }

            var min = int.Parse(strMin);
            var max = int.Parse(strMax);
            var random = Generate(min, max);
            return random.ToString();
        }

        /// <summary>
        /// Represents a random selected iterate from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">An <see cref="T"/> instance that representing generic model</typeparam>
        /// <param name="enumerable">An <see cref="IEnumerable{T}"/> list that representing original list of models</param>
        /// <returns>An <see cref="T"/> instance that representing random selected iterate</returns>
        public static T SelectRandom<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (enumerable?.Any() != true)
                return default;

            var list = enumerable.ToList();
            if (list.Count == 0)
                return default;

            var array = list.ToArray();
            return array[Generate(0, array.Length - 1)];
        }

        /// <summary>
        /// Represents a <see cref="List{T}"/> of  random selected iterates from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">An <see cref="T"/> instance that representing generic model</typeparam>
        /// <param name="enumerable">An <see cref="IEnumerable{T}"/> list that representing original list of models</param>
        /// <param name="count">An <see cref="int"/> value that representing MUST-HAVE iterates</param>
        /// <returns>An <see cref="List{T}"/> that representing random selected iterates</returns>
        public static List<T> SelectRandom<T>(this IEnumerable<T> enumerable, int count)
        {
            if (enumerable?.Any() != true)
                return default;

            var list = enumerable.ToList();
            if (list.Count == 0)
                return default;

            var array = list.ToArray();

            var result = new Dictionary<int, T>();
            var index = 1;

            var max = array.Length >= count ? count : array.Length;
            while (index <= max)
            {
                var generatedIndex = Generate(0, array.Length - 1);
                var hasDuplicate = result.Any(x => x.Key == generatedIndex);
                if (hasDuplicate)
                    continue;

                var item = array[generatedIndex];
                result.Add(generatedIndex, item);

                index++;
            }

            return result.Count > 0
                ? result.Select(x => x.Value).ToList()
                : null;
        }

        /// <summary>
        /// Generates a random index between <c>min</c> and <c>max</c>
        /// </summary>
        /// <param name="min">An <see cref="int"/> value that representing minimum index</param>
        /// <param name="max">An <see cref="int"/> value that representing maximum index</param>
        /// <returns>An <see cref="int"/> value</returns>
        public static int Generate(int min, int max) => RandomNumberGenerator.GetInt32(min, max);
    }
}