using System.Collections.Generic;
using System.Reflection;

namespace R8.Lib.Validatable
{
    /// <summary>
    /// Initializes a <see cref="ValidatableResult"/> object to show specific-property's errors.
    /// </summary>
    public class ValidatableResult
    {
        /// <summary>
        /// An <see cref="string"/> value that representing <see cref="PropertyInfo"/> name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An <see cref="List{T}"/> that representing errors found.
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Initializes a <see cref="ValidatableResult"/> object to show specific-property's errors.
        /// </summary>
        /// <param name="name">An <see cref="string"/> value that representing <see cref="PropertyInfo"/> name.</param>
        /// <param name="errors">An <see cref="List{T}"/> that representing errors found.</param>
        public ValidatableResult(string name, List<string> errors) : this()
        {
            Name = name;
            Errors = errors;
        }

        /// <summary>
        /// Initializes a <see cref="ValidatableResult"/> object to show specific-property's errors.
        /// </summary>
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