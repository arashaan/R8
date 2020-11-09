using System.ComponentModel.DataAnnotations;

namespace R8.Lib.Enums
{
    public enum ValueTypes
    {
        [Display(Name = "text")]
        String = 0,

        [Display(Name = "checkbox")]
        CheckBox = 1,

        [Display(Name = "select")]
        DropDownList = 2,

        [Display(Name = "date")]
        Date = 3,

        [Display(Name = "number")]
        Number = 4,

        [Display(Name = "radio")]
        Radio = 5,
    }
}