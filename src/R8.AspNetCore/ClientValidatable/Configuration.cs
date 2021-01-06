using R8.AspNetCore.ClientValidatable.Types;

using System;

namespace R8.AspNetCore.ClientValidatable
{
    public class Configuration
    {
        public Type PropertyType { get; set; }

        public bool IsRequired { get; set; }
        public Action<ClientValidatorString> String { get; set; }
        public Action<ClientValidatorInteger> Integer { get; set; }
    }
}