using R8.Lib.Attributes;
using R8.Lib.Enums;

using System.Linq;
using System.Reflection;

namespace R8.Lib.MethodReturn
{
    public static class Extensions
    {
        public static Response<T> To<T>(this IResponse response, T model) where T : class
        {
            var newResp = new Response<T>(response.Localizer)
            {
                Status = response.Status,
                Errors = response.Errors,
                Result = model
            };
            return newResp;
        }

        public static string GetMessage(this IResponseBase response)
        {
            string error;
            var flagShow = typeof(Flags).GetMember(response.Status.ToString()).FirstOrDefault()
                ?.GetCustomAttribute<FlagShowAttribute>();
            if (flagShow == null)
            {
                var err = response.Localizer == null ? "Error" : response.Localizer["Error"];
                error = $"{err} {(int)response.Status}";
            }
            else
            {
                var err = response.Localizer == null
                ? response.Status.ToString()
                : response.Localizer[response.Status.ToString()].ToString();
                error = err;
            }

            return error;
        }
    }
}