using R8.Lib.Enums;
using R8.Lib.Localization;

using System;

namespace R8.Lib.MethodReturn
{
    public class Response<TSource> : ResponseDatabase where TSource : class
    {
        public Response()
        {
        }

        public Response(ILocalizer localizer) : base(localizer)
        {
        }

        public Response(Flags status) : base(status)
        {
        }

        public Response(TSource result) : base(Flags.Success)
        {
            Result = result;
        }

        public Response(Flags status, TSource result) : base(status)
        {
            Result = result;
        }

        public Response(Flags status, ValidatableResultCollection errors) : base(status, errors)
        {
        }

        public Response(Flags status, TSource result, ValidatableResultCollection errors) : base(status, errors)
        {
            Result = result;
        }

        public void Deconstruct(out Flags status, out TSource result)
        {
            status = Status;
            result = Result;
        }

        [Obsolete("Use '.ToResponse<TSource>()' instead.")]
        public static Response<TSource> FromValidatableObject<T>(T obj) where T : ValidatableObject
        {
            return obj.ToResponse<TSource>();
        }

        public static implicit operator ResponseCollection(Response<TSource> response)
        {
            return new ResponseCollection { response };
        }

        public static implicit operator bool(Response<TSource> response)
        {
            return response.Success;
        }

        public static implicit operator Flags(Response<TSource> response)
        {
            return response.Status;
        }

        public TSource Result { get; set; }

        public override bool Success => CheckSuccess(Status, Save);

        public override string ToString()
        {
            var text = Status.ToString();
            if (Result != null)
                text += $" => {Result}";

            return text;
        }
    }

    public class Response : ResponseBase, IResponse
    {
        public Response()
        {
        }

        public Response(ILocalizer localizer)
        {
            this.Localizer = localizer;
        }

        public Response(Flags status) : this()
        {
            Status = status;
        }

        public Response(ValidatableResultCollection errors)
        {
            Errors = errors;
        }

        public Response(Flags status, ValidatableResultCollection errors) : this(status)
        {
            Errors = errors;
        }

        public virtual bool Success => Status == Flags.Success;
        public ValidatableResultCollection Errors { get; set; }

        public void AddErrors(ValidatableResultCollection errors)
        {
            Errors ??= new ValidatableResultCollection();
            Errors.AddRange(errors);
        }

        public static implicit operator bool(Response response)
        {
            return response.Success;
        }

        public static implicit operator Flags(Response response)
        {
            return response.Status;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}