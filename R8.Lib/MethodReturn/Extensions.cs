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

        public static string GetMessage(this IResponse response)
        {
            string error;
            if (response.Localizer == null)
            {
                error = response.Status.ToString();
            }
            else
            {
                var flagShow = typeof(Flags).GetMember(response.Status.ToString()).FirstOrDefault()
                    ?.GetCustomAttribute<FlagShowAttribute>();
                if (flagShow == null)
                {
                    var hasValue = response.Localizer.TryGetValue("Error", out error);
                    error = hasValue
                        ? $"{error} {(int)response.Status}"
                        : $"Error {(int)response.Status}";
                }
                else
                {
                    var hasValue = response.Localizer.TryGetValue(response.Status.ToString(), out error);
                    error = hasValue
                        ? response.Status.ToString().ToNormalized()
                        : response.Status.ToString();
                }
            }

            return error;
        }
    }
}