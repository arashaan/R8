# R8.Lib

### R8.Lib.Dates

* Convert a `DateTime` to **Unix Timestamp**:
    ```
    var date = new DateTime(2021, 02, 01, 18, 53, 38);
    var unix = date.ToUnixTime();
    ```
    **Output will be: `1612205618000`**

* Convert back to `DateTime` from **Unix Timestamp**:
    ```
    long unix = 1612205618000;
    var date = Dates.FromUnixTime(unixTimeStamp: unix);
    ```
    **Output will be: `DateTime(2021, 02, 01, 18, 53, 38)`**

* Convert UTC DateTime to **Timezone-specific DateTime**:
    ```
    var date = new DateTime(2021, 02, 01, 18, 53, 38);
    var timeZoneName = "Asia/Tehran";
    var dateZoned = date.GetLocalDateTime(timeZonePlaceName: timeZoneName);
    ```
    ***timeZonePlaceName** parameter is "Asia/Tehran" by default*

    **Output will be: `DateTime(2021, 02, 04, 48, 53, 38)`**

* Get Noda's `DateTimeZone`:
  * By windows timezone id:
    ```
    var timeZoneName = "Iran Standard Time";
    var dateZoned = Dates.GetNodaTimeZone(timeZoneId: timeZoneName, isSystemTimeZone: true);
    ```

  * by javascript timezone id:
    ```
    var timeZoneName = "Asia/Tehran";
    var dateZoned = Dates.GetNodaTimeZone(timeZoneId: timeZoneName, isSystemTimeZone: false);
    ```
    **Output will be a [DateTimeZoneMore](https://github.com/iamr8/R8/blob/master/src/R8.Lib/DateTimeZoneMore.cs) object: *(UTC03:30) Tehran***

* Get List of timezones:
    (An alternative for `System.TimeZoneInfo.GetSystemTimeZones()`

    ```
    var timeZones = Dates.GetNodaTimeZones();
    ```

---
