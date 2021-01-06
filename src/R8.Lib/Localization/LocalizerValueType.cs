namespace R8.Lib.Localization
{
    /// <summary>
    /// Determines which type of value, container includes.
    /// </summary>
    public enum LocalizerValueType
    {
        /// <summary>
        /// A normal text without any formatting and matching input.
        /// </summary>
        Text = 0,

        /// <summary>
        /// A text with matching input.
        /// </summary>
        /// <remarks>like: <c>Hi {0}</c> => Hi Arash</remarks>
        FormattableText = 1,

        /// <summary>
        /// An html text with matching input tag.
        /// </summary>
        /// <remarks>like: Hi <c>&lt;0&gt;&lt;/0&gt;</c> =&gt; Hi <c>&lt;a href="#"&gt;Arash&lt;/a&gt;</c></remarks>
        Html = 2,

        /// <summary>
        /// Unknown type.
        /// </summary>
        Unknown = 100
    }
}