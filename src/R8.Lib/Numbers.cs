using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace R8.Lib
{
    public static class Numbers
    {
        public class HumanizedTelephoneNumber
        {
            public string Name { get; set; }
            public List<string> Numbers { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        /// <summary>
        /// Returns a humanized text for a list of telephone numbers.
        /// </summary>
        /// <param name="listOfPhoneNumbers"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>For example: 021 44447832 - 021 44447833 => 021 44447832-3</returns>
        public static IEnumerable<HumanizedTelephoneNumber> HumanizeTelephoneNumbers(List<string> listOfPhoneNumbers)
        {
            if (listOfPhoneNumbers == null) throw new ArgumentNullException(nameof(listOfPhoneNumbers));
            if (listOfPhoneNumbers.Count == 0)
                return default;

            var result = new List<HumanizedTelephoneNumber>();
            var complexList = new List<List<string>>();

            var phones = Enumerable.OrderBy(listOfPhoneNumbers, number => long.Parse(Regex.Replace(number, @"[^\d]", ""))).ToList();
            if (phones.Count > 1)
            {
                var groupedNumbers = new List<string>() { phones[0] };
                var formerNumber = string.Empty;

                do
                {
                    if (phones.Count != 1)
                        formerNumber = phones[0];
                    var formerSanitized = long.Parse(Regex.Replace(formerNumber, @"[^\d]", ""));

                    var currentNumber = phones.Count != 1 ? phones[1] : phones[0];
                    var currentSanitized = long.Parse(Regex.Replace(currentNumber, @"[^\d]", ""));

                    if (currentSanitized == (formerSanitized + 1))
                        groupedNumbers.Add(currentNumber);
                    else
                    {
                        if (groupedNumbers.Any())
                            complexList.Add(groupedNumbers);

                        groupedNumbers = new List<string>() { currentNumber };
                    }

                    if (phones.Count > 2)
                        phones.Remove(formerNumber);
                    else
                    {
                        complexList.Add(groupedNumbers);
                        phones.Remove(formerNumber);
                        phones.Remove(currentNumber);
                    }
                } while (phones.Count != 0);
            }
            else
            {
                complexList.Add(new List<string>() { phones[0] });
            }

            foreach (var phoneGroup in complexList)
            {
                var groupName = string.Empty;
                foreach (var phone in phoneGroup)
                {
                    if (phoneGroup.IndexOf(phone) == 0)
                        groupName = phone;
                    else
                    {
                        var formerPhone = phoneGroup[phoneGroup.IndexOf(phone) - 1];
                        var namer = string.Empty;

                        for (var phoneNum = 0; phoneNum < phone.Length; phoneNum++)
                        {
                            var formerNum = formerPhone.Substring(phoneNum, 1)[0];
                            var thisNum = phone.Substring(phoneNum, 1)[0];
                            if (thisNum == formerNum)
                                namer += thisNum;
                            else
                            {
                                var firstPhone =
                                    phoneGroup[0][phoneNum..];
                                var currentRemaining = phone[phoneNum..];
                                namer +=
                                    $"{firstPhone}-{currentRemaining}";
                                break;
                            }
                        }

                        groupName = namer;
                    }
                }

                result.Add(new HumanizedTelephoneNumber() { Name = groupName, Numbers = phoneGroup });
            }
            return result;
        }

        /// <summary>
        /// Converts price amount to words.
        /// </summary>
        /// <param name="priceStr"></param>
        /// <param name="zero"></param>
        /// <param name="thousand"></param>
        /// <param name="million"></param>
        /// <param name="billion"></param>
        /// <param name="thousandBillion"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static string HumanizeCurrency(string priceStr, string zero = "صفر", string thousand = "هزار", string million = "میلیون", string billion = "میلیارد", string thousandBillion = "هزار میلیارد")
        {
            if (priceStr == null) throw new ArgumentNullException(nameof(priceStr));
            priceStr = priceStr.FixUnicodeNumbers();
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
                  .Select(x => new string(x.ToArray())).Reverse().ToList().ConvertAll(x =>
                  {
                      var ca2 = x.ToCharArray();
                      Array.Reverse((Array)ca2);
                      return new string(ca2);
                  });

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

        /// <summary>
        /// Fixes non-unicode digits
        /// </summary>
        /// <param name="num">An <see cref="string"/> value that containing non-unicode digits</param>
        /// <returns>An <see cref="string"/> value</returns>
        public static string FixUnicodeNumbers(this string num)
        {
            var result = "";
            if (string.IsNullOrEmpty(num))
                return num;

            foreach (var digit in num)
            {
                var character = digit;
                if ('۰' <= character && character <= '۹')
                    character -= 'ۀ';
                result += character;
            }
            return result;
        }
    }
}