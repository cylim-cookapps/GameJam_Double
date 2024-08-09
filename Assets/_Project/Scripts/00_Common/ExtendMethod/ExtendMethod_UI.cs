using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Pxp;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static partial class ExtendMethod
{
    #region Graphic

    public static void SetSprite(this Image image, Sprite sprite)
    {
        if (image == null) return;

        image.sprite = sprite;
    }
    public static void SetSprite(this Image image, string sprite)
    {
        if (image == null) return;

        image.sprite = SpriteManager.Inst.GetSprite(sprite);
    }

    public static void SetColor(this Graphic graphic, Color color, bool isAlpha = true)
    {
        if (graphic == null) return;
        if (isAlpha)
            graphic.color = color;
        else
        {
            color.a = graphic.color.a;
            graphic.color = color;
        }
    }

    public static void SetColor(this Graphic graphic, string hex, bool isAlpha = true)
    {
        if (graphic == null) return;
        if (hex[0] != '#')
            hex = hex.Insert(0, "#");

        if (isAlpha)
            graphic.color = HexColor(hex);
        else
        {
            var color = HexColor(hex);
            color.a = graphic.color.a;
            graphic.color = color;
        }
    }

    internal static void AddListener(this Pxp.Button btn, UnityAction action)
    {
        if (btn == null)
            return;

        //이벤트 중복 방지
        btn.onClick.RemoveListener(action);
        btn.onClick.AddListener(action);
    }

    #endregion

    #region Text

    public static void SetText(this TextMeshProUGUI text, int value, string format = "")
    {
        if (text == null) return;

        text.SetText(value.ToString(format));
    }

    public static void SetText(this TextMeshProUGUI text, long value, string format = "")
    {
        if (text == null) return;

        text.SetText(value.ToString(format));
    }

    public static void SetText(this TextMeshProUGUI text, float value, string format = "")
    {
        if (text == null) return;

        text.SetText(value.ToString(format));
    }

    public static void SetText(this TextMeshProUGUI text, double value, string format = "")
    {
        if (text == null) return;

        text.SetText(value.ToString(format));
    }

    #endregion
}
