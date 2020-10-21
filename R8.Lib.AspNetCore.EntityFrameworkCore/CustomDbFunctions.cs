using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace R8.Lib.AspNetCore.EntityFrameworkCore
{
    public static class CustomDbFunctions
    {
        [DbFunction]
        public static string Right(this DbFunctions _, string input, int howMany) =>
          throw new InvalidOperationException(
            "This method is for use with Entity Framework Core only and has no in-memory implementation.");

        [DbFunction]
        public static DateTime DateAdd(this DbFunctions _, string datePart, long number, string date) =>
          throw new InvalidOperationException(
            "This method is for use with Entity Framework Core only and has no in-memory implementation.");

        [DbFunction]
        public static long RowNumber(this DbFunctions _, object orderBy) =>
          throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");

        [DbFunction]
        public static string ParseName(this DbFunctions _, string object_name, int object_piece) =>
          throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");

        [DbFunction]
        public static string JsonValue(this DbFunctions _, string expression, [NotParameterized] string path) =>
        throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");

        [DbFunction]
        public static int DateDiff(this DbFunctions _, string interval, string startingDate, string endingDate) =>
        throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");

        [DbFunction]
        public static int? IsNumeric(this DbFunctions _, string expression) =>
        throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");

        public static ModelBuilder AddCustomDbFunctions(this ModelBuilder modelBuilder)
        {
            var jsonMethod = typeof(CustomDbFunctions).GetMethod(nameof(JsonValue)) ??
                             throw new InvalidOperationException();
            modelBuilder.HasDbFunction(jsonMethod)
              .HasTranslation(args => SqlFunctionExpression.Create(
                "JSON_VALUE",
                args.Skip(1).ToArray(),
                typeof(string),
                null
              ))
              .HasParameter("_").Metadata.TypeMapping = new StringTypeMapping("string");

            var isNumeric = typeof(CustomDbFunctions).GetMethod(nameof(IsNumeric)) ??
                            throw new InvalidOperationException();
            modelBuilder.HasDbFunction(isNumeric)
              .HasTranslation(args => SqlFunctionExpression.Create(
                "ISNUMERIC",
                args.Skip(1).ToArray(),
                typeof(int?),
                null
              ))
              .HasParameter("_").Metadata.TypeMapping = new StringTypeMapping("string");

            var dateDiff = typeof(CustomDbFunctions).GetMethod(nameof(DateDiff)) ??
                           throw new InvalidOperationException();
            modelBuilder.HasDbFunction(dateDiff)
              .HasTranslation(args => SqlFunctionExpression.Create(
                "DATEDIFF",
                args.Skip(1).ToArray(),
                typeof(int),
                null
              ))
              .HasParameter("_").Metadata.TypeMapping = new StringTypeMapping("string");

            var parseName = typeof(CustomDbFunctions).GetMethod(nameof(ParseName)) ??
                           throw new InvalidOperationException();
            modelBuilder.HasDbFunction(parseName)
              .HasTranslation(args => SqlFunctionExpression.Create(
                "PARSENAME",
                args.Skip(1).ToArray(),
                typeof(string),
                null
              ))
              .HasParameter("_").Metadata.TypeMapping = new StringTypeMapping("string");

            var dateAdd = typeof(CustomDbFunctions).GetMethod(nameof(DateAdd)) ??
                            throw new InvalidOperationException();
            modelBuilder.HasDbFunction(dateAdd)
              .HasTranslation(args => SqlFunctionExpression.Create(
                "DATEADD",
                args.Skip(1).ToArray(),
                typeof(DateTime),
                null
              ))
              .HasParameter("_").Metadata.TypeMapping = new StringTypeMapping("string");

            var right = typeof(CustomDbFunctions).GetMethod(nameof(Right)) ??
                            throw new InvalidOperationException();
            modelBuilder.HasDbFunction(right)
              .HasTranslation(args => SqlFunctionExpression.Create(
                "RIGHT",
                args.Skip(1).ToArray(),
                typeof(string),
                null
              ))
              .HasParameter("_").Metadata.TypeMapping = new StringTypeMapping("string");

            return modelBuilder;
        }
    }
}