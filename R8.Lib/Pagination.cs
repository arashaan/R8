using System.Collections.Generic;
using Newtonsoft.Json;

namespace R8.Lib
{
    public class PaginationPageModel
    {
        public int Num { get; set; }
        public bool IsCurrent { get; set; }
        public string Link { get; set; }
    }

    public struct Pagination<TModel> : IPagination where TModel : class
    {
        [JsonProperty("page")]
        public int CurrentPage { get; set; }

        [JsonProperty("pages")]
        public int Pages { get; set; }

        [JsonProperty("count")]
        public int CountAll { get; set; }

        [JsonProperty("results")]
        public List<TModel> Items { get; set; }

        public Pagination(List<TModel> items, int currentPage, int pages)
        {
            Items = items;
            CurrentPage = currentPage <= 0 ? 1 : currentPage;
            Pages = pages;
            CountAll = items.Count;
        }

        public Pagination(List<TModel> items, int currentPage, int pages, int countAll)
        {
            Items = items;
            CurrentPage = currentPage <= 0 ? 1 : currentPage;
            Pages = pages;
            CountAll = countAll;
        }

        public override string ToString()
        {
            return $"{Items.Count} items in Page {CurrentPage}/{Pages}";
        }
    }

    public interface IPagination
    {
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        public int CountAll { get; set; }
    }

    public struct Pagination : IPagination
    {
        [JsonProperty("page")]
        public int CurrentPage { get; set; }

        [JsonProperty("pages")]
        public int Pages { get; set; }

        [JsonProperty("count")]
        public int CountAll { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Pagination(int currentPage, int pages)
        {
            CurrentPage = currentPage <= 0 ? 1 : currentPage;
            Pages = pages;
            CountAll = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Pagination(int currentPage, int pages, int countAll)
        {
            CurrentPage = currentPage <= 0 ? 1 : currentPage;
            Pages = pages;
            CountAll = countAll;
        }

        public override string ToString()
        {
            return $"Page {CurrentPage}/{Pages}";
        }
    }
}