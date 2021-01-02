using System.ComponentModel.DataAnnotations;

using R8.Lib;

namespace R8.Test.Constants.FakeObjects
{
    public class FakeValidatableObjectTest : ValidatableObject
    {
        [Required]
        public string Name { get; set; }

        public string this[string key] => string.Empty;
    }
}