using R8.Lib.Enums;
using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public interface IResponseStatus
    {
        object Status { get; }
        ILocalizer Localizer { get; }

        void SetLocalizer(ILocalizer localizer);

        void SetStatus(object status);
    }
}