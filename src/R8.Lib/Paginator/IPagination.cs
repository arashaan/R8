namespace R8.Lib.Paginator
{
    /// <summary>
    /// A base interface for <see cref="Pagination"/>.
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// An <see cref="int"/> value that representing page number that contains these data
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// An <see cref="int"/> value that representing page numbers
        /// </summary>
        public int Pages { get; }

        /// <summary>
        /// An <see cref="int"/> value that representing loaded data count
        /// </summary>
        public int CountAll { get; }
    }
}