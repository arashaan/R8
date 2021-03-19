using R8.Lib.Validatable;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace R8.AspNetCore.Test.FakeObjects
{
    public class FakeObjHasReq : ValidatableObject<FakeObjHasReq>
    {
        public string Name { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }
    }
}