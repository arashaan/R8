using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

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

    public static class FileTypesExtensions
    {
        private static TAttribute GetAttribute<TAttribute>(this Enum enumMember)
            where TAttribute : Attribute
        {
            if (enumMember == null) throw new ArgumentNullException(nameof(enumMember));
            var enumType = enumMember.GetType();
            var enumName = Enum.GetName(enumType, enumMember);
            if (enumName == null)
                return null;

            var memberInfos = enumType.GetMember(enumName);
            var memberInfo = memberInfos.Single();
            return memberInfo.GetCustomAttribute<TAttribute>();
        }

        public static string GetDisplayName(this Enum value)
        {
            return value.GetAttribute<DisplayAttribute>()?.GetName() ?? Enum.GetName(value.GetType(), value);
        }

        public static IEnumerable<string> GetFileExtensions(this FileTypes fileType)
        {
            return fileType.GetDisplayName().Split("|").ToList();
        }
    }
}