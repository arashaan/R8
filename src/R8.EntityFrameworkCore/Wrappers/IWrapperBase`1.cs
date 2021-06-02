namespace R8.EntityFrameworkCore.Wrappers
{
    internal interface IWrapperBase<TStatus> : IWrapperBase
    {
        TStatus Status { get; set; }
    }
}