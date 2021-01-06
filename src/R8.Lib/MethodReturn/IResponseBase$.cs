namespace R8.Lib.MethodReturn
{
    /// <summary>
    /// An base interface for <see cref="ResponseBase{TSource,TStatus}"/>.
    /// </summary>
    /// <typeparam name="TStatus">A type that representing status type.</typeparam>
    public interface IResponseBase<TStatus> : IResponseStatus<TStatus>, IResponseErrors
    {
    }
}