using System;

namespace R8.Lib.AspNetCore.Sitemap
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SitemapSettingsAttribute : Attribute
    {
        public SitemapSettingsAttribute(SitemapChangeFrequency changeFrequency, double priority)
        {
            ChangeFrequency = changeFrequency;
            Priority = priority;
        }

        public SitemapChangeFrequency ChangeFrequency { get; }

        public double? Priority { get; }
    }
}