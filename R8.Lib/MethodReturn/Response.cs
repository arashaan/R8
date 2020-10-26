using Newtonsoft.Json;

using R8.Lib.Enums;

using System;
using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public class Response<TSource> : IResponseDatabase where TSource : class
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

        public Response(TSource result) : this(Flags.Success)
        {
            Result = result;
        }

        public Response(Flags status, TSource result) : this(status)
        {
            Result = result;
        }

        public Response(Flags status, ValidatableResultCollection errors) : this(status)
        {
            Errors = errors;
        }

        public Response(Flags status, TSource result, ValidatableResultCollection errors) : this(status, errors)
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

        public static implicit operator bool(Response<TSource> response)
        {
            return response.Success;
        }

        public static implicit operator Response<TSource>(bool boolean)
        {
            return boolean
                ? new Response<TSource>(Flags.Success)
                : new Response<TSource>(Flags.Failed);
        }

        public static implicit operator ResponseCollection(Response<TSource> response)
        {
            var group = new ResponseCollection { response };
            return group;
        }

        public static implicit operator Flags(Response<TSource> response)
        {
            return response.Status;
        }

        public static explicit operator Response<TSource>(Flags status)
        {
            var response = new Response<TSource>(status);
            return response;
        }

        [JsonProperty("r")]
        public TSource Result { get; set; }

        [JsonProperty("s")] public bool Success => ResponseDatabase.CheckSuccess(Status, Save);

        public Flags Status { get; set; }
        public string Message => this.GetMessage();
        public ValidatableResultCollection Errors { get; set; }

        public ILocalizer Localizer { get; set; }

        public void SetLocalizer(ILocalizer localizer)
        {
            this.Localizer = localizer;
        }

        public void AddErrors(ValidatableResultCollection errors)
        {
            Errors ??= new ValidatableResultCollection();
            Errors.AddRange(errors);
        }

        public void SetStatus(Flags status)
        {
            this.Status = status;
        }

        public override string ToString()
        {
            var text = Status.ToString();
            if (Result != null)
                text += $" => {Result}";

            return text;
        }

        public DatabaseSaveState? Save { get; set; }
    }

    public class Response : IResponse
    {
        public Response(ValidatableResultCollection errors)
        {
            Errors = errors;
        }

        [JsonProperty("s")]
        public bool Success => Status == Flags.Success;

        public Response()
        {
        }

        public Response(Flags status) : this()
        {
            Status = status;
        }

        public Response(Flags status, ValidatableResultCollection errors) : this(status)
        {
            Errors = errors;
        }

        public Flags Status { get; set; }
        public string Message => this.GetMessage();
        public ValidatableResultCollection Errors { get; set; }

        public ILocalizer Localizer { get; set; }

        public void SetLocalizer(ILocalizer localizer)
        {
            this.Localizer = localizer;
        }

        public void AddErrors(ValidatableResultCollection errors)
        {
            Errors ??= new ValidatableResultCollection();
            Errors.AddRange(errors);
        }

        public void SetStatus(Flags status)
        {
            this.Status = status;
        }

        public static implicit operator bool(Response response)
        {
            return response.Success;
        }

        public static implicit operator Response(bool boolean)
        {
            return boolean
                ? new Response(Flags.Success)
                : new Response(Flags.Failed);
        }

        public static implicit operator Flags(Response response)
        {
            return response.Status;
        }

        public static explicit operator Response(Flags status)
        {
            return new Response(status);
        }

        public override string ToString()
        {
            return Message;
        }
    }
}