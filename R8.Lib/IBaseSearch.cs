namespace R8.Lib
{
    /// <summary>
    /// An <see cref="IBaseSearch"/> interface
    /// </summary>
    public interface IBaseSearch
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