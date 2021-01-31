using Newtonsoft.Json;

namespace R8.Lib.Paginator
{
    /// <summary>
    /// Initializes an <see cref="Pagination"/> instance
    /// </summary>
    public struct Pagination : IPagination
    {
        /// <summary>
        /// Initializes an <see cref="Pagination"/> instance.
        /// </summary>
        /// <param name="currentPage">An <see cref="int"/> value that representing page number that contains these data</param>
        /// <param name="pages">An <see cref="int"/> value that representing page numbers</param>
        /// <param name="countAll">An <see cref="int"/> value that representing loaded data count</param>
        public Pagination(int currentPage, int pages, int countAll = 0)
        {
            Pages = pages < 0 ? 0 : pages;
            CurrentPage = Pages > 0
                ? currentPage <= 0 ? 1 : currentPage
                : 0;
            CountAll = countAll < 0 ? 0 : countAll;
        }

        [JsonProperty("page")]
        public int CurrentPage { get; set; }

        [JsonProperty("pages")]
        public int Pages { get; set; }

        [JsonProperty("count")]
        public int CountAll { get; set; }

        public override string ToString()
        {
            return $"Page {CurrentPage}/{Pages}";
        }
    }
}