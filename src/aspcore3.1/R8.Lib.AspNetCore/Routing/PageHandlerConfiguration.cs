namespace R8.Lib.AspNetCore.Routing
{
    public class PageHandlerConfiguration
    {
        public string PagesFolder { get; set; } = "Pages";
        public string IndexPage { get; set; } = "Index";

        public object RouteDictionary { get; set; }

        public bool StartWithPages { get; set; } = false;
        public bool UseBackslash { get; set; } = false;

        public bool EndWithIndex { get; set; } = true;
    }
}