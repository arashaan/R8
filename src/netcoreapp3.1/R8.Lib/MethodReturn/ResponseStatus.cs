using R8.Lib.Enums;
using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public class ResponseStatus : IResponseStatus
    {
        public virtual object Status { get; set; }
        public ILocalizer Localizer { get; set; }

        public void SetLocalizer(ILocalizer localizer)
        {
            this.Localizer = localizer;
        }

        public virtual void SetStatus(object status)
        {
            this.Status = status;
        }
    }
}