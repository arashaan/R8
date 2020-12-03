namespace R8.Lib.MethodReturn
{
    public interface IResponseBase<TStatus> : IResponseStatus<TStatus>, IResponseErrors
    {
    }
}