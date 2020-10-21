using System;

namespace R8.Lib.AspNetCore.Base
{
    public class ClientValidatorConfiguration
    {
        public Type PropertyType { get; set; }

        public bool IsRequired { get; set; }
        public Action<ClientValidatorString> String { get; set; }
        public Action<ClientValidatorInteger> Integer { get; set; }
    }
}