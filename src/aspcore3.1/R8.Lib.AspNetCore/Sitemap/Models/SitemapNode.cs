using System;

namespace R8.Lib.AspNetCore.Sitemap.Models
{
    public class SitemapNode : ISitemapModel
    {
        /// <summary>
        /// URL of the page. This URL must begin with the protocol (such as http) and end with a trailing slash, if your web server requires it. This value must be less than 2,048 characters.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// How frequently the page is likely to change. This value provides general information to search engines and may not correlate exactly to how often they crawl the page. Valid values are:
        /// </summary>
        public SitemapChangeFrequency? ChangeFrequency { get; set; }

        /// <summary>
        /// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0. This value does not affect how your pages are compared to pages on other sites—it only lets the search engines know which pages you deem most important for the crawlers.
        /// The default priority of a page is 0.5.
        /// Please note that the priority you assign to a page is not likely to influence the position of your URLs in a search engine's result pages. Search engines may use this information when selecting between URLs on the same site, so you can use this tag to increase the likelihood that your most important pages are present in a search index.
        /// Also, please note that assigning a high priority to all of the URLs on your site is not likely to help you. Since the priority is relative, it is only used to select between URLs on your site.
        /// </summary>
        public double? Priority { get; set; }

        /// <summary>
        /// The date of last modification of the file. This date should be in W3C Datetime format. This format allows you to omit the time portion, if desired, and use YYYY-MM-DD.
        /// </summary>
        public DateTime? LastModificationDate { get; set; }
    }
}