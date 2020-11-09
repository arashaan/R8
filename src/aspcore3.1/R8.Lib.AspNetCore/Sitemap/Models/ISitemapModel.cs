using System;

namespace R8.Lib.AspNetCore.Sitemap.Models
{
    public interface ISitemapModel
    {
        public string Url { get; set; }
        public DateTime? LastModificationDate { get; set; }
    }
}