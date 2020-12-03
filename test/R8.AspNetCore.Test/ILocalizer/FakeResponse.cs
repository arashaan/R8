using R8.Lib;
using R8.Lib.MethodReturn;

namespace R8.AspNetCore.Test.ILocalizer
{
    public class FakeResponse : ResponseBase<Flags>
    {
        public FakeResponse()
        {
        }

        public FakeResponse(Lib.Localization.ILocalizer localizer) : base(localizer)
        {
        }

        public FakeResponse(Flags status) : base(status)
        {
        }

        public string Message
        {
            get
            {
                var localizer = this.GetLocalizer();
                return localizer != null ? localizer[Status.ToString()] : Status.ToString();
            }
        }

        public override bool Success => Status == Flags.Success;
        public override Flags Status { get; set; }
        public override ValidatableResultCollection Errors { get; protected set; }

        public override void SetStatus(Flags status)
        {
            base.SetStatus(status);
        }
    }

    public class FakeResponse<TModel> : ResponseBase<TModel, Flags> where TModel : class
    {
        public FakeResponse()
        {
        }

        public FakeResponse(Lib.Localization.ILocalizer localizer) : base(localizer)
        {
        }

        public FakeResponse(Flags status) : base(status)
        {
        }

        public string Message
        {
            get
            {
                var localizer = this.GetLocalizer();
                return localizer != null ? localizer[Status.ToString()] : Status.ToString();
            }
        }

        public override Flags Status { get; set; }
        public override ValidatableResultCollection Errors { get; protected set; }

        public override void SetStatus(Flags status)
        {
            base.SetStatus(status);
        }
    }
}