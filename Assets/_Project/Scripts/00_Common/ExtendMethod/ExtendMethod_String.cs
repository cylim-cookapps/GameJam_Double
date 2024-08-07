using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using UnityEngine;

public static partial class ExtendMethod
{
    private static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    private static class EnumStringCache<TEnum>
    {
        private static Dictionary<TEnum, string> Dict = null;

        public static string GetString(TEnum value)
        {
            if (Dict == null)
            {
                Dict = new Dictionary<TEnum, string>();
            }

            if (Dict.TryGetValue(value, out string result))
            {
                return result;
            }

            result = value.ToString();
            Dict.Add(value, result);

            return result;
        }
    }

    /// <summary>
    /// Enum String
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static string GetEnumString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return EnumStringCache<TEnum>.GetString(value);
    }

    /// <summary>
    /// Enum String
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static string ToEnumString<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return GetEnumString(value);
    }

    public static string GetAlphabetText(double num)
    {
        if (num < 0)
        {
            return "";
        }

        if (num < 1000)
        {
            return num.ToString("0");
        }

        var baseLog = Math.Floor(Math.Log(num, 1000));
        var count = (int) baseLog - 1;

        var result = new StringBuilder();
        if (count == 0)
        {
            result.Insert(0, alphabet[0]);
        }
        else
        {
            while (count > 0)
            {
                var index = count % 26;
                result.Insert(0, result.Length != 0 ? alphabet[index - 1] : alphabet[index]);
                count /= 26;
            }
        }

        var value = num / Math.Pow(1000, baseLog);
        return ZString.Format("{0:N2}{1}", value, result);
    }

    internal static string ToDoubleString(this double value)
    {
        return value.ToString("N0", CultureInfo.InvariantCulture);
    }
}
