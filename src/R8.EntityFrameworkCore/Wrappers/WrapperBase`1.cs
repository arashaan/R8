using Newtonsoft.Json;
using R8.Lib.Localization;
using R8.Lib.Wrappers;

namespace R8.EntityFrameworkCore.Wrappers
{
    /// <summary>
    /// An abstract class for presenting Returning status of a method.
    /// </summary>
    /// <typeparam name="TStatus">Type of status property.</typeparam>
    public class WrapperBase<TStatus> : IWrapperBase<TStatus>
    {
        private ILocalizer _localizer;

        /// <summary>
        /// An abstract class for presenting Returning status of a method.
        /// </summary>
        public WrapperBase()
        {
            SetLocalizer();
        }

        /// <summary>
        /// An abstract class for presenting Returning status of a method.
        /// </summary>
        /// <param name="status">A value that representing status.</param>
        public WrapperBase(TStatus status) : this()
        {
            Status = status;
        }

        public WrapperBase(TStatus status, ILocalizer localizer) : this(localizer)
        {
            Status = status;
        }

        public WrapperBase(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        public virtual void SetLocalizer()
        {
            this._localizer = WrapperConnection.Options;
        }

        public virtual void SetLocalizer(ILocalizer localizer)
        {
            this._localizer = localizer;
        }

        public virtual ILocalizer Localizer => this._localizer;

        /// <summary>
        /// Gets status of procedure.
        /// </summary>
        [JsonIgnore]
        public virtual TStatus Status { get; set; }

        [JsonProperty("success")]
        public virtual bool Success { get; set; }

        public static implicit operator WrapperBase<TStatus>(TStatus flag)
        {
            return new WrapperBase<TStatus>(flag);
        }

        public static explicit operator TStatus(WrapperBase<TStatus> src)
        {
            return src.Status;
        }
    }
}