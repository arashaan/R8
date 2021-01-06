namespace R8.AspNetCore.Sitemap
{
    /// <summary>
    /// Indicates how frequently the page is likely to be updated.
    /// </summary>
    public enum SitemapChangeFrequency
    {
        /// <summary>
        /// The value "always" should be used to describe documents that change each time they are accessed.
        /// </summary>
        Always = 0,

        /// <summary>Hourly change</summary>
        Hourly = 1,

        /// <summary>Daily change</summary>
        Daily = 2,

        /// <summary>Weekly change</summary>
        Weekly = 3,

        /// <summary>Monthly change</summary>
        Monthly = 4,

        /// <summary>Yearly change</summary>
        Yearly = 5,

        /// <summary>
        /// The value "never" should be used to describe archived URLs.
        /// </summary>
        Never = 6,

        NoNeed = 100
    }
}