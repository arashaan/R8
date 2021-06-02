using R8.Lib.Localization;

namespace R8.EntityFrameworkCore.Wrappers
{
    internal interface IWrapperBase
    {
        void SetLocalizer();

        void SetLocalizer(ILocalizer localizer);

        ILocalizer Localizer { get; }
    }
}