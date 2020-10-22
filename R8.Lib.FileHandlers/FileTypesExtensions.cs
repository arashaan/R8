using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace R8.Lib.FileHandlers
{
    public static class FileTypesExtensions
    {
        public static IEnumerable<string> GetFileExtensions(this FileTypes fileType)
        {
            var attribute = fileType.GetType().GetCustomAttribute<DisplayAttribute>();
            var display = attribute?.GetName() ?? Enum.GetName(fileType.GetType(), fileType);
            return display.Split("|").ToList();
        }
    }
}