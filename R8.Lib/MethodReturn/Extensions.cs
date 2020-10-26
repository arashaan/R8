using R8.Lib.Attributes;
using R8.Lib.Enums;
using R8.Lib.Localization;

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
                LocalizerContainer tempError;
                var flagShow = typeof(Flags).GetMember(response.Status.ToString()).FirstOrDefault()
                    ?.GetCustomAttribute<FlagShowAttribute>();
                if (flagShow == null)
                {
                    tempError = response.Localizer["Error"];
                    error = $"{tempError} {(int)response.Status}";
                }
                else
                {
                    tempError = response.Localizer[response.Status.ToString()];
                    error = tempError.ToString();
                }
            }

            return error;
        }
    }
}