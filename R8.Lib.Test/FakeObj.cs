using R8.Lib.xUnit.TestOrder;

using System.ComponentModel.DataAnnotations;

namespace R8.Lib.Test
{
    public class FakeObj : TestClassBase
    {
        [Display(Name = "Arash")]
        public string Name { get; set; }

        public string LastName { get; set; }
    }
}