using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class ExtendMethod
{
    /// <summary>
    /// oldDate기준으로 newDate가 날짜가 지났는지 확인하는 함수
    /// </summary>
    /// <param name="oldDate">기준 날짜</param>
    /// <param name="newDate">비교할 날짜</param>
    /// <returns></returns>
    public static bool HasDatePassed(DateTime oldDate, DateTime newDate, int dayCount = 1)
    {
        TimeSpan timeDifference = newDate.Date - oldDate.Date;
        return timeDifference.Days >= dayCount;
    }

    /// <summary>
    /// 다음주가 되었는지 체크하는
    /// </summary>
    /// <param name="date1"></param>
    /// <param name="date2"></param>
    /// <returns></returns>
    public static bool IsPastNextWeek(DateTime date1, DateTime date2)
    {
        // date1이 월요일이 아니면, 다음 월요일로 설정
        int daysUntilNextMonday = ((int) DayOfWeek.Monday - (int) date1.DayOfWeek + 7) % 7;
        if (daysUntilNextMonday == 0 && date1.DayOfWeek != DayOfWeek.Monday)
        {
            // 이미 월요일이면 다음 월요일로 설정해야 하므로 7일을 추가
            daysUntilNextMonday += 7;
        }

        // 다음 주 월요일을 계산
        DateTime nextWeek = date1.AddDays(daysUntilNextMonday);

        // date2가 nextWeek와 같거나 그 이후인지 검사
        return date2 >= nextWeek;
    }

    /// <summary>
    /// 해당 UnixTime을 DateTime으로 변환
    /// </summary>
    /// <param name="unixTime"></param>
    /// <returns></returns>
    public static DateTime ToDate(this long unixTime)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
        return dateTimeOffset.UtcDateTime;
    }

    /// <summary>
    /// DateTime을 UnixTime으로 변환
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long ToTick(this DateTime date)
    {
        var dateTimeOffset = new DateTimeOffset(date);
        return dateTimeOffset.ToUnixTimeSeconds();
    }
}
