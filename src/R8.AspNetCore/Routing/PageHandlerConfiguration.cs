namespace R8.AspNetCore.Routing
{
    public class PageHandlerConfiguration
    {
        /// <summary>
        /// Default name of the folder that contains Root Pages.
        /// <remarks>default: <c>"Pages"</c></remarks>
        /// </summary>
        public string PagesFolder { get; set; } = "Pages";
        /// <summary>
        /// Default name of the Index Pages that will be passed by machine.
        /// <remarks>default: <c>"Index"</c></remarks>
        /// </summary>
        public string IndexPage { get; set; } = "Index";

        public object RouteDictionary { get; set; }

        public bool StartWithPages { get; set; } = false;
        /// <summary>
        /// Use backslash '\' instead of forward-slash '/'
        /// <remarks>default: <c>"false"</c></remarks>
        /// </summary>
        public bool UseBackslash { get; set; } = false;

        /// <summary>
        /// Whether remove and not remove Default page name ( based on <see cref="IndexPage"/> value ).
        /// <remarks>default: <c>"true"</c></remarks>
        /// </summary>
        public bool EndWithIndex { get; set; } = true;
    }
}