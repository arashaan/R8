namespace R8.Lib.Paginator
{
    /// <summary>
    /// An object to passing data from component to view.
    /// </summary>
    public class PaginationPageModel
    {
        /// <summary>
        /// Number of current page.
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// Is this number is current page ?!
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Do you have link for this page ?
        /// </summary>
        public string Link { get; set; }
    }
}