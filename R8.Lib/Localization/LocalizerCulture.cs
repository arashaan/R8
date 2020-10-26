using System.Globalization;

namespace R8.Lib.Localization
{
    public sealed class LocalizerCulture
    {
        public CultureInfo Culture { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}