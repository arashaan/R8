using R8.Lib.Enums;

namespace R8.Lib.MethodReturn
{
    public interface IResponseBaseDatabase : IResponseBase
    {
        DatabaseSaveState? Save { get; set; }
        bool Success { get; }
    }
}