using R8.Lib.Enums;

namespace R8.Lib.MethodReturn
{
    public interface IResponse : IResponseBase
    {
        Flags Status { get; }
        string Message { get; }
        ILocalizer Localizer { get; }

        void SetLocalizer(ILocalizer localizer);

        void SetStatus(Flags status);
    }
}