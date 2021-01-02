using System;

namespace R8.AspNetCore.Sitemap
{
    /// <summary>
    /// An attribute to let Sitemap scan for this page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SitemapSettingsAttribute : Attribute
    {
        public SitemapChangeFrequency ChangeFrequency { get; set; } = SitemapResult.FrequencyDefault;

        public double Priority { get; set; } = SitemapResult.PriorityDefault;
    }
}