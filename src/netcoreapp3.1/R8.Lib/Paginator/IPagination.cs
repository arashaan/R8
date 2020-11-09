namespace R8.Lib.Paginator
{
    /// <summary>
    /// An <see cref="IPagination"/> interface
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// An <see cref="int"/> value that representing page number that contains these data
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// An <see cref="int"/> value that representing page numbers
        /// </summary>
        int Pages { get; set; }

        /// <summary>
        /// An <see cref="int"/> value that representing loaded data count
        /// </summary>
        int CountAll { get; set; }
    }
}