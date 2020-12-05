using System;

namespace R8.AspNetCore.Sitemap.Models
{
    public class SitemapIndexNode : ISitemapModel
    {
        /// <summary>
        /// Identifies the time that the corresponding Sitemap file was modified. It does not correspond to the time that any of the pages listed in that Sitemap were changed. The value for the lastmod tag should be in W3C Datetime format.
        /// By providing the last modification timestamp, you enable search engine crawlers to retrieve only a subset of the Sitemaps in the index i.e. a crawler may only retrieve Sitemaps that were modified since a certain date. This incremental Sitemap fetching mechanism allows for the rapid discovery of new URLs on very large sites.
        /// </summary>
        public DateTime LastModificationDate { get; set; }

        /// <summary>
        /// Identifies the location of the Sitemap.
        /// This location can be a Sitemap, an Atom file, RSS file or a simple text file.
        /// </summary>
        public string Url { get; set; }
    }
}