using System.ComponentModel.DataAnnotations;

namespace R8.Lib.FileHandlers
{
    public enum FileTypes
    {
        [Display(Name = "zip|rar")]
        Zip = 0,

        [Display(Name = "jpeg|jpg|png")]
        Image = 1,

        [Display(Name = "mov|mp4")]
        Video = 2,

        [Display(Name = "pdf|doc|xls|ppt|docx|xlsx|pptx")]
        Document = 3,

        [Display(Name = "svg")]
        Svg = 4,

        Unknown = 100
    }
}