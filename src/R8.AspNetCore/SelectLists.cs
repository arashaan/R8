using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using R8.Lib;

namespace R8.AspNetCore
{
    public static class SelectLists
    {
        public const string NoneItem = "---";

        public static List<SelectListItem> AddNone(this IEnumerable<SelectListItem> selectList)
        {
            if (selectList?.Any() == true)
            {
                return selectList.Prepend(new SelectListItem
                {
                    Value = string.Empty,
                    Text = NoneItem
                }).ToList();
            }
            return new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = string.Empty,
                    Text = NoneItem
                }
            };
        }

        public static List<SelectListItem> ToSelectList<TEnum>(TEnum defaultValue, Func<TEnum, bool> predicate) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(predicate)
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString(),
                    Selected = Equals(e, defaultValue)
                }).ToList();
        }

        public static List<SelectListItem> ToSelectList<TEnum>(Func<TEnum, bool> predicate) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(predicate)
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString(),
                }).ToList();
        }

        public static List<SelectListItem> ToSelectList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString(),
                }).ToList();
        }

        public static List<SelectListItem> ToSelectList<TEnum>(TEnum defaultValue) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString(),
                    Selected = Equals(e, defaultValue)
                }).ToList();
        }

        public static List<SelectListItem> SelectFirstItem(this IEnumerable<SelectListItem> selectList)
        {
            var selectListItems = selectList.ToList();
            if (selectListItems.Count == 0)
                return default;

            for (var i = 0; i < selectListItems.Count; i++)
                if (i == 0)
                    selectListItems[i].Selected = true;

            return selectListItems;
        }
    }
}