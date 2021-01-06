using System;

namespace R8.AspNetCore.ClientValidatable.Types
{
    public class ClientValidatorInteger
    {
        public Action<ClientValidatorIntegerRange> Range { get; set; }
    }
}