﻿using R8.Lib.MethodReturn;

namespace R8.AspNetCore.WebApi
{
    public abstract class TokenResponseBase<TStatus> : ResponseBase<TStatus>, IWebApiCredential
    {
        public virtual UserResponse User { get; set; }
    }
}