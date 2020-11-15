using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public abstract class ResponseBase<TSource> : ResponseBaseDatabase where TSource : class
    {
        protected ResponseBase()
        {
        }

        protected ResponseBase(ILocalizer localizer) : base(localizer)
        {
        }

        protected ResponseBase(object status) : base(status)
        {
        }

        protected ResponseBase(TSource result)
        {
            Result = result;
        }

        protected ResponseBase(object status, TSource result) : base(status)
        {
            Result = result;
        }

        protected ResponseBase(object status, ValidatableResultCollection errors) : base(status, errors)
        {
        }

        protected ResponseBase(object status, TSource result, ValidatableResultCollection errors) : base(status, errors)
        {
            Result = result;
        }

        public static implicit operator ResponseCollection(ResponseBase<TSource> responseBase)
        {
            return new ResponseCollection { responseBase };
        }

        public static implicit operator bool(ResponseBase<TSource> responseBase)
        {
            return responseBase.Success;
        }

        public TSource Result { get; set; }

        public override string ToString()
        {
            var text = Status.ToString();
            if (Result != null)
                text += $" => {Result}";

            return text;
        }
    }

    public abstract class ResponseBase : ResponseStatus, IResponseBase
    {
        protected ResponseBase()
        {
        }

        protected ResponseBase(ILocalizer localizer)
        {
            this.Localizer = localizer;
        }

        protected ResponseBase(object status) : this()
        {
            Status = status;
        }

        protected ResponseBase(ValidatableResultCollection errors)
        {
            Errors = errors;
        }

        protected ResponseBase(object status, ValidatableResultCollection errors) : this(status)
        {
            Errors = errors;
        }

        public ValidatableResultCollection Errors { get; set; }

        public void AddErrors(ValidatableResultCollection errors)
        {
            Errors ??= new ValidatableResultCollection();
            Errors.AddRange(errors);
        }
    }
}