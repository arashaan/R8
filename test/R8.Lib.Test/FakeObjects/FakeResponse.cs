﻿using Newtonsoft.Json;
using R8.Lib.Localization;
using R8.Lib.MethodReturn;
using R8.Lib.Test.Enums;
using R8.Lib.Validatable;

namespace R8.Lib.Test.FakeObjects
{
    public class FakeResponse : ResponseBase<Flags>
    {
        public FakeResponse()
        {
        }

        public FakeResponse(ILocalizer localizer) : base(localizer)
        {
        }

        public FakeResponse(Flags status) : base(status)
        {
        }

        public string Message
        {
            get
            {
                var localizer = this.GetLocalizer();
                return localizer != null ? localizer[Status.ToString()] : Status.ToString();
            }
        }

        public override bool Success => Status == Flags.Success;

        [JsonProperty("sts")]
        public override Flags Status { get; set; }

        [JsonIgnore]
        public override ValidatableResultCollection Errors { get; protected set; }

        public override void SetStatus(Flags status)
        {
            base.SetStatus(status);
        }
    }
}