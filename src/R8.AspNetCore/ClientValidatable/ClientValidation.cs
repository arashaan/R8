using System;
using System.Collections.Generic;
using System.Text;

namespace R8.AspNetCore.ClientValidatable
{
    public class ClientValidation
    {
        public string Name { get; set; }
        public Dictionary<string, object> Validations { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public void Deconstruct(out string name, out Dictionary<string, object> validations)
        {
            name = Name;
            validations = Validations;
        }
    }
}