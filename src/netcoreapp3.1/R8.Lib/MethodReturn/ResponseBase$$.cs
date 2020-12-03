using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public abstract class ResponseBase<TSource, TStatus> : ResponseBaseDatabase<TStatus> where TSource : class
    {
        protected ResponseBase()
        {
        }

        protected ResponseBase(ILocalizer localizer) : base(localizer)
        {
        }

        protected ResponseBase(TStatus status) : base(status)
        {
        }

        protected ResponseBase(TSource result)
        {
            Result = result;
        }

        protected ResponseBase(TStatus status, TSource result) : base(status)
        {
            Result = result;
        }

        protected ResponseBase(TStatus status, ValidatableResultCollection errors) : base(status, errors)
        {
        }

        protected ResponseBase(TStatus status, TSource result, ValidatableResultCollection errors) : base(status, errors)
        {
            Result = result;
        }

        public static implicit operator ResponseBaseCollection<TStatus>(ResponseBase<TSource, TStatus> responseBase)
        {
            return new ResponseBaseCollection<TStatus> { responseBase };
        }

        public static implicit operator bool(ResponseBase<TSource, TStatus> responseBase)
        {
            return responseBase.Success;
        }

        public virtual TSource Result { get; set; }

        public override string ToString()
        {
            var text = Status.ToString();
            if (Result != null)
                text += $" => {Result}";

            return text;
        }
    }
}