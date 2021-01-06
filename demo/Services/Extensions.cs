using R8.AspNetCore3_1.Demo.Services.Enums;
using R8.Lib.Validatable;

namespace R8.AspNetCore3_1.Demo.Services
{
    public static class Extensions
    {
        public static Response<TSource> ToResponse<TSource>(this ValidatableObject obj) where TSource : class
        {
            var valid = obj.Validate();
            var flags = valid ? Flags.Success : Flags.ModelIsNotValid;
            return new Response<TSource>(flags, obj.ValidationErrors);
        }
    }
}