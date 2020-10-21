using Org.BouncyCastle.Security;

using System;
using System.Collections.Generic;
using System.Linq;

namespace R8.Lib.Encryption
{
    public static class RandomSelector
    {
        public static int GenerateSixDigitToken(int length = 6)
        {
            if (length < 4)
                throw new Exception($"{length} must be equal-greater than 4");

            var strMin = "";
            var strMax = "";
            for (var i = 0; i < length - 1; i++)
            {
                strMin += "1";
                strMax += "9";
            }

            var min = int.Parse(strMin);
            var max = int.Parse(strMax);
            var random = Generate(min, max);
            return random;
        }

        public static T SelectRandom<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable?.Any() != true)
                return default;

            var list = enumerable.ToList();
            if (list.Count == 0)
                return default;

            var array = list.ToArray();
            return array[Generate(0, array.Length - 1)];
        }

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

        public static int Generate(int min, int max)
        {
            var secureRandom = SecureRandom.GetInstance("Sha1PRNG");
            secureRandom.SetSeed(max);
            return (Math.Abs(secureRandom.NextInt()) % max) + min;
        }
    }
}