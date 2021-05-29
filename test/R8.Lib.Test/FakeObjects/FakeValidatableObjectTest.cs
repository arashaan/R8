using R8.Lib.Validatable;

using System.ComponentModel.DataAnnotations;

namespace R8.Lib.Test.FakeObjects
{
    public class FakeValidatableObjectTest : ValidatableObject<FakeValidatableObjectTest>
    {
        [Required]
        public string Name { get; set; }

        public string this[string key] => string.Empty;
    }
}