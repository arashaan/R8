using Microsoft.AspNetCore.Http.Extensions;

namespace R8.Lib.AspNetCore.Routing
{
    public class ParsedUrl
    {
        public ParsedUrl(string absolutePath)
        {
            AbsolutePath = absolutePath;
        }

        public ParsedUrl(string absolutePath, QueryBuilder queryBuilder)
        {
            AbsolutePath = absolutePath;
            QueryBuilder = queryBuilder;
        }

        public void Deconstruct(out string absolutePath, out QueryBuilder queryBuilder)
        {
            absolutePath = AbsolutePath;
            queryBuilder = QueryBuilder;
        }

        public override string ToString()
        {
            return $"{AbsolutePath}{(QueryBuilder != null ? QueryBuilder.ToQueryString().ToString() : "")}";
        }

        public string AbsolutePath { get; set; }
        public QueryBuilder QueryBuilder { get; set; }
    }
}