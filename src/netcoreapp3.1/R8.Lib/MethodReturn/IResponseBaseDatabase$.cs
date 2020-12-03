using R8.Lib.Enums;

namespace R8.Lib.MethodReturn
{
    public interface IResponseBaseDatabase<TStatus> : IResponseBase<TStatus>
    {
        DatabaseSaveState? Save { get; set; }
    }
}