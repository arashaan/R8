using System.Globalization;

namespace R8.Lib.Localization
{
    /// <summary>
    /// A Node for each culture and equivalent value in <see cref="LocalizerContainer"/>.
    /// </summary>
    public sealed class LocalizerCultureNode
    {
        /// <summary>
        /// Gets specific culture for this node and value.
        /// </summary>
        public CultureInfo Culture { get; internal set; }

        /// <summary>
        /// Gets equivalent value for given culture.
        /// </summary>
        public string Value { get; internal set; }

        public override string ToString()
        {
            return Value;
        }
    }
}