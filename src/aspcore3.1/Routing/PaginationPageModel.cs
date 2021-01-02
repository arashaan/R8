namespace R8.AspNetCore.Routing
{
    /// <summary>
    /// An object to passing data from component to view.
    /// </summary>
    public class PaginationPageModel
    {
        internal PaginationPageModel(int pageNo, bool isCurrentPage, string pageLink)
        {
            PageNo = pageNo;
            IsCurrentPage = isCurrentPage;
            PageLink = pageLink;
        }

        /// <summary>
        /// Number of current page.
        /// </summary>
        public int PageNo { get; }

        /// <summary>
        /// Is this number is current page ?!
        /// </summary>
        public bool IsCurrentPage { get; }

        /// <summary>
        /// Do you have link for this page ?
        /// </summary>
        public string PageLink { get; }
    }
}