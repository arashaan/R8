using Xunit.Abstractions;

namespace R8.EntityFrameworkCore.Test
{
    public class QueryableExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public QueryableExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }
    }
}