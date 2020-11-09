using System.ComponentModel.DataAnnotations;

namespace R8.FileHandlers
{
    /// <summary>
    /// An enumerator constant to get complex file types
    /// </summary>
    public enum FileTypes
    {
        /// <summary>
        /// Zip file extensions ( <c>zip</c>, <c>rar</c> )
        /// </summary>
        [Display(Name = "zip|rar")]
        Zip = 0,

        /// <summary>
        /// Image file extensions ( <c>jpeg</c>, <c>jpg</c>, <c>png</c> )
        /// </summary>
        [Display(Name = "jpeg|jpg|png|bmp")]
        Image = 1,

        /// <summary>
        /// Video file extensions ( <c>mov</c>, <c>mp4</c> )
        /// </summary>
        [Display(Name = "mov|mp4")]
        Video = 2,

        /// <summary>
        /// Document file extensions ( <c>pdf</c>, <c>doc</c>, <c>docx</c>, <c>xls</c>, <c>xlsx</c>, <c>ppt</c>, <c>pptx</c> )
        /// </summary>
        [Display(Name = "pdf|doc|xls|ppt|docx|xlsx|pptx")]
        Document = 3,

        /// <summary>
        /// Svg file extension ( <c>svg</c> )
        /// </summary>
        [Display(Name = "svg")]
        Svg = 4,

        /// <summary>
        /// Audio file extension ( <c>mp3</c> )
        /// </summary>
        [Display(Name = "mp3")]
        Audio = 5,

        /// <summary>
        /// Unknown files
        /// </summary>
        Unknown = 100
    }
}