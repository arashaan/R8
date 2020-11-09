using R8.Lib.MethodReturn;

namespace R8.AspNetCore.WebApi
{
    public class TokenCheck : Response
    {
        public UserResponse User { get; set; }
    }
}