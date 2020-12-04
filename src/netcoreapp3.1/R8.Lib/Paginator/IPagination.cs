namespace R8.Lib.Paginator
{
    public interface IPagination
    {
        /// <summary>
        /// An <see cref="int"/> value that representing page number that contains these data
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// An <see cref="int"/> value that representing page numbers
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// An <see cref="int"/> value that representing loaded data count
        /// </summary>
        public int CountAll { get; set; }
    }
}