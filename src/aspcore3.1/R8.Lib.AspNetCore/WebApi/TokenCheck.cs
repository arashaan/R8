using R8.Lib.MethodReturn;

namespace R8.Lib.AspNetCore.WebApi
{
    public class TokenCheck : Response
    {
        public UserResponse User { get; set; }
    }
}