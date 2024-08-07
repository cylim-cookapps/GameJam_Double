using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.Obfuscator;
using UnityEngine;
using Random = System.Random;

public static partial class ExtendMethod
{
    #region Common

    public static Color HexColor(string hexCode)
    {
        Color color;
        hexCode = hexCode.ToUpper();
        if (ColorUtility.TryParseHtmlString(hexCode, out color))
        {
            return color;
        }

        Debug.LogError("[UnityExtension::HexColor]invalid hex code - " + hexCode);
        return Color.white;
    }

    public static void SetActive(this Component component, bool isOn)
    {
        if (component == null)
        {
            Debug.LogError("Component가 존재하지 않습니다.");
            return;
        }

        if (component.gameObject.activeSelf != isOn)
            component.gameObject.SetActive(isOn);
    }

    public static void SetReactive(this Component component, bool isOn)
    {
        if (component == null)
        {
            Debug.LogError("Component가 존재하지 않습니다.");
            return;
        }

        component.gameObject.SetActive(!isOn);
        component.gameObject.SetActive(isOn);
    }

    public static void Swap<T>(T a, T b)
    {
        (a, b) = (b, a);
    }

    public static bool Swap<T>(this T[] objectArray, int x, int y)
    {
        // check for out of range
        if (objectArray.Length <= y || objectArray.Length <= x) return false;

        // swap index x and y
        (objectArray[x], objectArray[y]) = (objectArray[y], objectArray[x]);

        return true;
    }

    public static float InterpolatePercentage(float minValue, float maxValue, float percentage)
    {
        return minValue + (maxValue - minValue) * (percentage / 100.0f);
    }

    public static int InterpolatePercentage(int minValue, int maxValue, float percentage)
    {
        return minValue + (int)((maxValue - minValue) * (percentage / 100.0f));
    }

    #endregion

    #region Enum

    /// <summary>
    /// 문자를 Enum으로 변환
    /// </summary>
    public static T ParseToEnum<T>(this string value)
    {
        return (T) Enum.Parse(typeof(T), value);
    }

    /// <summary>
    /// 문자 List를 를 Enum으로 변환
    /// </summary>
    public static T[] ParseToEnums<T>(this IList<string> value)
    {
        T[] array = new T[value.Count];
        for (int i = 0; i < value.Count; i++)
            array[i] = value[i].ParseToEnum<T>();
        return array;
    }

    /// <summary>
    /// 이전 enum 값으로 이동
    /// 이전 값이 없으면 맨 마지막 값으로 이동
    /// </summary>
    public static T PrevLoop<T>(this T self)
    {
        List<int> temp;
        var intValue = Convert.ToInt32(self);
        var length = Enum.GetValues(typeof(T)).Length;
        var nextValue = (intValue - 1 + length) % length;
        var enumValue = Enum.ToObject(typeof(T), nextValue);

        return (T) enumValue;
    }

    /// <summary>
    /// 다음 enum 값으로 이동
    /// 다음 값이 없으면 맨 처음 값으로 이동
    /// </summary>
    public static T NextLoop<T>(this T self)
    {
        var intValue = Convert.ToInt32(self);
        var nextValue = (intValue + 1) % Enum.GetValues(typeof(T)).Length;
        var enumValue = Enum.ToObject(typeof(T), nextValue);

        return (T) enumValue;
    }

    /// <summary>
    /// 이전 enum 값으로 이동
    /// </summary>
    public static T Prev<T>(this T self)
    {
        var intValue = Convert.ToInt32(self);
        var nextValue = Mathf.Max(0, intValue - 1);
        var enumValue = Enum.ToObject(typeof(T), nextValue);

        return (T) enumValue;
    }

    /// <summary>
    /// 다음 enum 값으로 이동
    /// </summary>
    public static T Next<T>(this T self)
    {
        var intValue = Convert.ToInt32(self);
        var nextValue = Mathf.Min(Enum.GetValues(typeof(T)).Length - 1, intValue + 1);
        var enumValue = Enum.ToObject(typeof(T), nextValue);

        return (T) enumValue;
    }

    /// <summary>
    /// 해당 Flag에 포함 여부
    /// </summary>
    public static bool IsFlagSet<T>(this T value, T flag) where T : Enum
    {
        var intValue = Convert.ToInt64(value);
        var intFlag = Convert.ToInt64(flag);
        return (intValue & intFlag) == intFlag;
    }

    /// <summary>
    /// 해당 Flag에 포함
    /// </summary>
    public static T SetFlag<T>(this T value, T flag) where T : Enum
    {
        var intValue = Convert.ToInt64(value);
        var intFlag = Convert.ToInt64(flag);
        return (T) Enum.ToObject(typeof(T), intValue | intFlag);
    }

    /// <summary>
    /// Flag 초기화
    /// </summary>
    public static T ClearFlag<T>(this T value, T flag) where T : Enum
    {
        var intValue = Convert.ToInt64(value);
        var intFlag = Convert.ToInt64(flag);
        return (T) Enum.ToObject(typeof(T), intValue & ~intFlag);
    }

    /// <summary>
    /// 해당 Flag Toggle
    /// </summary>
    public static T ToggleFlag<T>(this T value, T flag) where T : Enum
    {
        var intValue = Convert.ToInt64(value);
        var intFlag = Convert.ToInt64(flag);
        return (T) Enum.ToObject(typeof(T), intValue ^ intFlag);
    }

    #endregion

    #region List

    private static readonly Random random = new Random();

    public static int GetRandomIndex(this IList<ObfuscatorFloat> probabilities)
    {
        if (probabilities.Count == 0)
            return -1;

        if (probabilities.Count == 1)
            return 0;

        float total = 0;
        foreach (var probability in probabilities)
        {
            total += probability;
        }

        float randomPoint = (float) random.NextDouble() * total;

        for (int i = 0; i < probabilities.Count; i++)
        {
            if (randomPoint < probabilities[i])
                return i;
            randomPoint -= probabilities[i];
        }

        return probabilities.Count - 1;
    }

    public static T RandomValue<T>(this IList<T> list)
    {
        if (list.Count > 0)
            return list[UnityEngine.Random.Range(0, list.Count)];
        else
            return default(T);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 0)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static bool AddUnique<T>(this List<T> list, T element)
    {
        if (list.Contains(element))
            return false;

        list.Add(element);
        return true;
    }

    /// <summary>
    /// 중간 index를 RemoveAt하는 경우 배열 복사가 일어나기때문에 삭제할 인덱스를 마지막 인덱스와 스왑하여 제거
    /// 유의사항 : list 항목의 순서가 변경되기때문에 조심해야됨
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    public static void RemoveUnorderedAt<T>(this IList<T> list, int index)
    {
        list[index] = list[^1];
        list.RemoveAt(list.Count - 1);
    }

    #endregion

    #region Curve

    public static ParabolaData CalculateBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 tangent = CalculateBezierTangent(p0, p1, p2, t);
        float rotation = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

        return new ParabolaData {Position = CalculateBezierPoint(p0, p1, p2, t), Rotation = rotation};
    }

    // 베지어 커브 계산
    public static Vector2 CalculateBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;

        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    // 베지어 커브의 접선 방향 계산
    public static Vector2 CalculateBezierTangent(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
    }

    public struct ParabolaData
    {
        public Vector2 Position;
        public float Rotation;
    }

    public static ParabolaData CalculateParabola(Vector2 startPoint, Vector2 endPoint, float height, float t)
    {
        Vector2 tangent = TangentToParabola(startPoint, endPoint, height, t);
        float rotation = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

        return new ParabolaData {Position = Parabola(startPoint, endPoint, height, t), Rotation = rotation};
    }

    private static Vector2 Parabola(Vector2 startPoint, Vector2 endPoint, float height, float t)
    {
        var parabola = -4 * height * (t * t - t);

        var x = Mathf.Lerp(startPoint.x, endPoint.x, t);
        var y = Mathf.Lerp(startPoint.y, endPoint.y, t) + parabola;
        return new Vector2(x, y);
    }

    private static Vector2 TangentToParabola(Vector2 startPoint, Vector2 endPoint, float height, float t)
    {
        float dx = (endPoint.x - startPoint.x);
        float dy = (endPoint.y - startPoint.y) - 8 * height * t + 4 * height;
        return new Vector2(dx, dy).normalized;
    }

    #endregion
}
