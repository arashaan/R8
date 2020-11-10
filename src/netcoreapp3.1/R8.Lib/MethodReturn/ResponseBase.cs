﻿using R8.Lib.Enums;
using R8.Lib.Localization;

namespace R8.Lib.MethodReturn
{
    public class ResponseBase : IResponseBase
    {
        public Flags Status { get; set; }
        public string Message => this.GetMessage();
        public ILocalizer Localizer { get; set; }

        public void SetLocalizer(ILocalizer localizer)
        {
            this.Localizer = localizer;
        }

        public void SetStatus(Flags status)
        {
            this.Status = status;
        }
    }
}