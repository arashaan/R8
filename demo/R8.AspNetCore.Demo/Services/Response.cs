using R8.AspNetCore.Demo.Services.Enums;
using R8.Lib;
using R8.Lib.Localization;
using R8.Lib.MethodReturn;

namespace R8.AspNetCore.Demo.Services
{
    public class Response : ResponseBase
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

        public Response(ValidatableResultCollection errors) : base(errors)
        {
        }

        public Response(Flags status, ValidatableResultCollection errors) : base(status, errors)
        {
        }

        public new Flags Status { get; set; }

        public string Message { get; set; }
    }

    public class Response<TSource> : ResponseBase<TSource> where TSource : class
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

        public Response(TSource result) : base(result)
        {
        }

        public Response(Flags status, TSource result) : base(status, result)
        {
        }

        public Response(Flags status, ValidatableResultCollection errors) : base(status, errors)
        {
        }

        public Response(Flags status, TSource result, ValidatableResultCollection errors) : base(status, result, errors)
        {
        }

        public void Deconstruct(out Flags status, out TSource source)
        {
            status = Status;
            source = this.Result;
        }

        // [Obsolete("Use '.ToResponse<TSource>()' instead.")]
        public static Response<TSource> FromValidatableObject<T>(T obj) where T : ValidatableObject
        {
            return obj.ToResponse<TSource>();
        }

        public new Flags Status { get; set; }
        public string Message { get; set; }
    }

    public static class ResponseExtensions
    {
        public static Response ToResponse(this bool status)
        {
            return new Response(status ? Flags.Success : Flags.Failed);
        }

        public static Response<TSource> ToResponse<TSource>(this bool status, TSource source) where TSource : class
        {
            return new Response<TSource>(status ? Flags.Success : Flags.Failed, source);
        }
    }
}