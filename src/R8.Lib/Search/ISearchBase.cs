namespace R8.Lib.Search
{
    /// <summary>
    /// An <see cref="ISearchBase"/> interface
    /// </summary>
    public interface ISearchBase
    {
        /// <summary>
        /// Represents an <see cref="int"/> value that representing Current Page Number
        /// </summary>
        int PageNo { get; set; }

        /// <summary>
        /// Represents an <see cref="int"/> value that representing Count of Items in Current Page
        /// </summary>
        int PageSize { get; set; }
    }
}