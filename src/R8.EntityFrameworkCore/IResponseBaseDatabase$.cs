using R8.Lib.MethodReturn;

namespace R8.EntityFrameworkCore
{
    /// <summary>
    /// A base interface for <see cref="ResponseBaseDatabase{TStatus}"/>.
    /// </summary>
    /// <typeparam name="TStatus">A type that representing status type.</typeparam>
    public interface IResponseBaseDatabase<TStatus> : IResponseBase<TStatus>, IResponseBaseDatabase
    {
    }
}