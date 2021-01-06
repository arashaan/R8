using System.Collections.Generic;

namespace R8.Lib.Validatable
{
    public class ValidatableResultCollection : List<ValidatableResult>
    {
        public void Add(string key, List<string> errors)
        {
            var obj = new ValidatableResult(key, errors);
            this.Add(obj);
        }
    }
}