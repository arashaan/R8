using Newtonsoft.Json;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace R8.Lib.Paginator
{
    /// <summary>
    /// Initializes an <see cref="Pagination{TResult}"/> instance
    /// </summary>
    public struct Pagination<TModel> : IPagination, IList<TModel> where TModel : class
    {
        /// <summary>
        /// Initializes an <see cref="Pagination{TResult}"/> instance.
        /// </summary>
        /// <param name="items">n <see cref="List{T}"/> that representing values should be loaded into pagination</param>
        /// <param name="currentPage">An <see cref="int"/> value that representing page number that contains these data</param>
        /// <param name="pages">An <see cref="int"/> value that representing page numbers</param>
        /// <param name="countAll">An <see cref="int"/> value that representing loaded data count</param>
        public Pagination(IEnumerable<TModel> items, int currentPage, int pages, int countAll = 0)
        {
            Pages = pages < 0 ? 0 : pages;
            CurrentPage = Pages > 0
                ? currentPage <= 0 ? 1 : currentPage
                : 0;
            CountAll = countAll < 0 ? 0 : countAll;
            Items = items == null ? new List<TModel>() : items.ToList();
        }

        /// <summary>
        /// Initializes an <see cref="Pagination{TResult}"/> instance.
        /// </summary>
        /// <param name="currentPage">An <see cref="int"/> value that representing page number that contains these data</param>
        /// <param name="pages">An <see cref="int"/> value that representing page numbers</param>
        /// <param name="countAll">An <see cref="int"/> value that representing loaded data count</param>
        public Pagination(int currentPage, int pages, int countAll = 0) : this(null, currentPage, pages, countAll)
        {
        }

        /// <summary>
        /// An <see cref="List{T}"/> that representing values being loaded into pagination
        /// </summary>
        [JsonProperty("results")]
        public List<TModel> Items { get; set; }

        [JsonProperty("page")]
        public int CurrentPage { get; set; }

        [JsonProperty("pages")]
        public int Pages { get; set; }

        [JsonProperty("count")]
        public int CountAll { get; set; }

        public IEnumerator<TModel> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public override string ToString()
        {
            return Items == null 
                ? "No items." 
                : $"{Items.Count} item(s) in page {CurrentPage}/{Pages}";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TModel item)
        {
            Items.Add(item);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(TModel item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(TModel[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(TModel item)
        {
            return Items.Remove(item);
        }

        public int Count => Items.Count;
        public bool IsReadOnly => false;

        public int IndexOf(TModel item)
        {
            return Items.IndexOf(item);
        }

        public void Insert(int index, TModel item)
        {
            Items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        public TModel this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }
    }
}