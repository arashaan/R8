using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace R8.Lib
{
    public static class RandomSelector
    {
        public static int GenerateDigitToken(int length = 6)
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

        public static int Next(this RandomNumberGenerator generator, int min, int max)
        {
            // match Next of Random
            // where max is exclusive
            max--;

            var bytes = new byte[sizeof(int)]; // 4 bytes
            generator.GetNonZeroBytes(bytes);
            var val = BitConverter.ToInt32(bytes);
            // constrain our values to between our min and max
            // https://stackoverflow.com/a/3057867/86411
            var result = ((val - min) % (max - min + 1) + (max - min + 1)) % (max - min + 1) + min;
            return result;
        }

        public static int Generate(int min, int max)
        {
            return RandomNumberGenerator.GetInt32(min, max);
        }
    }
}