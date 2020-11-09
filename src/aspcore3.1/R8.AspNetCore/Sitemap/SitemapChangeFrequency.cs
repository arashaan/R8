namespace R8.AspNetCore.Sitemap
{
    public enum SitemapChangeFrequency
    {
        /// <summary>
        /// The value "always" should be used to describe documents that change each time they are accessed.
        /// </summary>
        Always,

        /// <summary>Hourly change</summary>
        Hourly,

        /// <summary>Daily change</summary>
        Daily,

        /// <summary>Weekly change</summary>
        Weekly,

        /// <summary>Monthly change</summary>
        Monthly,

        /// <summary>Yearly change</summary>
        Yearly,

        /// <summary>
        /// The value "never" should be used to describe archived URLs.
        /// </summary>
        Never,
    }
}