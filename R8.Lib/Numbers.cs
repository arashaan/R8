﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace R8.Lib
{
  public static class Numbers
  {
    public static int RoundToUp(double num)
    {
      return Convert.ToInt32(Math.Ceiling(num));
    }

    public static int FixPageNumber(this string pageNo)
    {
      var pg = string.IsNullOrEmpty(pageNo) ? 1 : int.TryParse(pageNo, out var page) ? page : 1;
      return pg;
    }

    public static string FixCurrency(this string text)
    {
      if (string.IsNullOrEmpty(text))
        return default;

      return decimal.TryParse(text, out var currency) ? $"{currency:#,###}" : text;
    }

    public static string CurrencyToWords(this string priceStr, string zero = "صفر", string thousand = "هزار", string million = "میلیون", string billion = "میلیارد", string thousandBillion = "هزار میلیارد")
    {
      var isPrice = ulong.TryParse(priceStr.Replace(",", "").Replace("/", ""), out var price);
      if (!isPrice)
        return priceStr;

      string processResult;
      if (price > 0)
      {
        var units = new[] { "", thousand, million, billion, thousandBillion };
        const double divisionLength = 3;
        var k = 0;

        var ca = price.ToString().ToCharArray();
        Array.Reverse(ca);
        var division = new string(ca).ToLookup(_ => Math.Floor(k++ / divisionLength))
          .Select(x => new string(x.ToArray())).Reverse().ToList().Select(x =>
          {
            var ca2 = x.ToCharArray();
            Array.Reverse((Array)ca2);
            return new string(ca2);
          }).ToList();

        var cnt = division.Count;
        var places = new List<string>();
        for (var i = 0; i < cnt; i++)
        {
          var currentUnit = units[cnt - i - 1];

          var currentNum = int.Parse(division[i]);
          if (currentNum == 0) continue;

          places.Add($"{currentNum} {currentUnit}");
        }

        processResult = string.Join(" و ", places);
        return processResult;
      }

      processResult = zero;
      return processResult;
    }

    public static string FixUnicodeNumbers(this string num)
    {
      var result = "";
      var length = num.Length;
      if (length == 0)
        return num;

      for (var index = 0; index < length; ++index)
      {
        var character = num[index];
        if ('۰' <= character && character <= '۹')
          character -= 'ۀ';
        result += character;
      }
      return result;
    }
  }
}