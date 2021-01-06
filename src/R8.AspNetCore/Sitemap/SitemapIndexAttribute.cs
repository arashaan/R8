using System;

namespace R8.AspNetCore.Sitemap
{
    /// <summary>
    /// An attribute to let Sitemap scan for this page as an Sitemap Index.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SitemapIndexAttribute : Attribute
    {
    }
}