using R8.AspNetCore.Enums;

using System;

namespace R8.AspNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IconAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets whether '.svg' file location under 'wwwroot/img/' path or an FontAwesome font icon
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets whether url heads to an .svg file in Server or an FontAwesome font icon
        /// </summary>
        public bool IsIcon => Path.StartsWith("/");

        public IconType GetIconType()
        {
            return GetIconType(Path);
        }

        public static IconType GetIconType(string path)
        {
            if (path.Contains("fa ", StringComparison.InvariantCulture)
                || path.Contains("fas ", StringComparison.InvariantCulture)
                || path.Contains("far ", StringComparison.InvariantCulture)
                || path.Contains("fab ", StringComparison.InvariantCulture))
            {
                return IconType.Glyph;
            }

            if (path.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
                return IconType.Image;

            if (path.Contains("path", StringComparison.InvariantCultureIgnoreCase) ||
                path.Contains("d=", StringComparison.InvariantCultureIgnoreCase))
            {
                return IconType.SvgPath;
            }

            throw new ArgumentOutOfRangeException(nameof(path));
        }
    }
}