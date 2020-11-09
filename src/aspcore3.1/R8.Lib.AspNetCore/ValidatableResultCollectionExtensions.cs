using System;
using System.Collections.Generic;

namespace R8.Lib.AspNetCore
{
    public static class ValidatableResultCollectionExtensions
    {
        public const string ExceptionDivider = "...";
        public const string ExceptionKeyValueDivider = "|";
        public const string ExceptionErrorsDivider = ";";

        public static string ToQueryString(this ValidatableResultCollection collection)
        {
            var errs = new List<string>();
            foreach (var (name, errors) in collection)
            {
                var errorExceptions = string.Join(ExceptionErrorsDivider, errors);

                var finalValue = name + ExceptionKeyValueDivider + errorExceptions;
                errs.Add(finalValue);
            }

            return string.Join(ExceptionDivider, errs);
        }

        public static string ToQueryString(this ValidatableResultCollection collection, Type modelType)
        {
            var errs = new List<string>();
            foreach (var (name, errors) in collection)
            {
                var keyProp = modelType.GetPublicProperties().Find(c => c.Name == name);
                if (keyProp == null)
                    continue;

                var key = keyProp.GetDisplayName();
                var errorExceptions = string.Join(ExceptionErrorsDivider, errors);

                var finalValue = key + ExceptionKeyValueDivider + errorExceptions;
                errs.Add(finalValue);
            }

            return string.Join(ExceptionDivider, errs);
        }
    }
}