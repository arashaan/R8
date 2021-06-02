using R8.AspNetCore3_1.Demo.Services.Enums;
using R8.EntityFrameworkCore;
using R8.Lib.Localization;
using R8.Lib.Validatable;

namespace R8.AspNetCore3_1.Demo.Services
{
    //public class Response : WrapperBase<Flags>
    //{
    //    public Response()
    //    {
    //    }

  

    //    public Response(Flags status) : base(status)
    //    {
    //    }


    //    public Flags Status { get; set; }

    //    public string Message
    //    {
    //        get
    //        {
    //            var localizer = this.GetLocalizer();
    //            return localizer != null ? localizer[Status.ToString()] : Status.ToString();
    //        }
    //    }
    //}

    //public class Response<TSource> : WrapperBase<TSource, Flags> where TSource : class
    //{
    //    public Response()
    //    {
    //    }

    //    public Response(ILocalizer localizer) : base(localizer)
    //    {
    //    }

    //    public Response(Flags status) : base(status)
    //    {
    //    }

    //    public Response(TSource result) : base(result)
    //    {
    //    }

    //    public Response(Flags status, TSource result) : base(status, result)
    //    {
    //    }

    //    public Response(Flags status, ValidatableResultCollection errors) : base(status, errors)
    //    {
    //    }

    //    public Response(Flags status, TSource result, ValidatableResultCollection errors) : base(status, result, errors)
    //    {
    //    }

    //    public void Deconstruct(out Flags status, out TSource source)
    //    {
    //        status = Status;
    //        source = this.Result;
    //    }

    //    public static Response<TSource> FromValidatableObject<T>(T obj) where T : ValidatableObject
    //    {
    //        return obj.ToResponse<TSource>();
    //    }

    //    public Flags Status { get; set; }

    //    public string Message
    //    {
    //        get
    //        {
    //            var localizer = this.GetLocalizer();
    //            return localizer != null ? localizer[Status.ToString()] : Status.ToString();
    //        }
    //    }
    //}

    //public static class ResponseExtensions
    //{
    //    public static Response ToResponse(this bool status)
    //    {
    //        return new Response(status ? Flags.Success : Flags.Failed);
    //    }

    //    public static Response<TSource> ToResponse<TSource>(this bool status, TSource source) where TSource : class
    //    {
    //        return new Response<TSource>(status ? Flags.Success : Flags.Failed, source);
    //    }
    //}
}