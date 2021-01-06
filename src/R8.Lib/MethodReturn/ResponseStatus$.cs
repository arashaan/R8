using Newtonsoft.Json;

using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public class ResponseStatus<TStatus> : IResponseStatus<TStatus>
    {
        [JsonIgnore]
        private ILocalizer _localizer;

        public ResponseStatus(TStatus status) : this()
        {
            Status = status;
        }

        public ResponseStatus(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        public ResponseStatus()
        {
            if (ResponseConnection.Options != null)
                this._localizer = ResponseConnection.Options;
        }

        public virtual TStatus Status { get; set; }
        public virtual bool Success { get; }

        public virtual ILocalizer GetLocalizer() => _localizer;

        public virtual void SetLocalizer(ILocalizer localizer) => this._localizer = localizer;

        public virtual void SetStatus(TStatus status) => this.Status = status;
    }
}