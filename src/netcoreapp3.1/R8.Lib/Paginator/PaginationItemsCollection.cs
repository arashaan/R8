using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.Paginator
{
    public class PaginationItemsCollection<TModel> : List<TModel>, IAsyncEnumerable<TModel> where TModel : class
    {
        IAsyncEnumerator<TModel> IAsyncEnumerable<TModel>.GetAsyncEnumerator(CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public virtual IAsyncEnumerable<TModel> AsAsyncEnumerable()
            => this;

        public async Task<List<TModel>> ToListAsync(CancellationToken cancellationToken = default)
        {
            var list = new List<TModel>();

            var asyncEnumerable = (IAsyncEnumerable<TModel>)this;
            var source = asyncEnumerable.WithCancellation(cancellationToken);
            await foreach (var element in source)
                list.Add(element);

            return list;
        }
    }
}