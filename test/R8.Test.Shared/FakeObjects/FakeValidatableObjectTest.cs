using R8.Lib;

using System.ComponentModel.DataAnnotations;

namespace R8.Test.Shared.FakeObjects
{
    public class FakeValidatableObjectTest : ValidatableObject
    {
        [Required]
        public string Name { get; set; }

        public string this[string key] => string.Empty;
    }
}