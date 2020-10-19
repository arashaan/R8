using System.Collections.Generic;

namespace R8.Lib
{
    public class ValidatableResult
    {
        public string Name { get; set; }
        public List<string> Errors { get; set; }

        public ValidatableResult(string name, List<string> errors)
        {
            Name = name;
            Errors = errors;
        }

        public ValidatableResult()
        {
        }

        public void Deconstruct(out string name, out List<string> errors)
        {
            name = Name;
            errors = Errors;
        }

        public override string ToString()
        {
            return $"{Name} => {string.Join(";", Errors.ToArray())}";
        }
    }
}