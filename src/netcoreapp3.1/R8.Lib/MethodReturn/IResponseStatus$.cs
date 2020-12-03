using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public interface IResponseStatus<TStatus>
    {
        TStatus Status { get; }
        bool Success { get; }

        void SetLocalizer(ILocalizer localizer);

        ILocalizer GetLocalizer();

        void SetStatus(TStatus status);
    }
}