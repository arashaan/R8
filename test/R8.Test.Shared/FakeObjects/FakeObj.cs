using System.ComponentModel.DataAnnotations;

namespace R8.Test.Shared.FakeObjects
{
    public class FakeObj
    {
        [Display(Name = "Arash")]
        public string Name { get; set; }

        public string LastName { get; set; }
    }
}