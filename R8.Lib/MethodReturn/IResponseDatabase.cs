using R8.Lib.Enums;

namespace R8.Lib.MethodReturn
{
    public interface IResponseDatabase : IResponse
    {
        DatabaseSaveState? Save { get; set; }
    }
}