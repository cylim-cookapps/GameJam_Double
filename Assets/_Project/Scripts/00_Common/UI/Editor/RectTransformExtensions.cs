using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class RectTransformExtensions
{
    [UnityEditor.MenuItem("CONTEXT/RectTransform/Adjust Scale")]
    public static void AdjustScale(UnityEditor.MenuCommand command)
    {
        RectTransform rectTransform = (RectTransform)command.context;

        if(rectTransform)
        {
            // Save the current scale
            Vector2 scaledSize = new Vector2(rectTransform.rect.width * rectTransform.localScale.x, rectTransform.rect.height * rectTransform.localScale.y);

            // adjust sizes to integral values by rounding
            int roundedWidth = Mathf.RoundToInt(scaledSize.x);
            int roundedHeight = Mathf.RoundToInt(scaledSize.y);

            // Adjust font size if there's a Text component
            TextMeshProUGUI text = rectTransform.GetComponent<TextMeshProUGUI>();
            if (text != null)
                text.fontSize = Mathf.RoundToInt(text.fontSize * ((rectTransform.localScale.x + rectTransform.localScale.y) / 2));

            // Set the scale to (1, 1, 1)
            rectTransform.localScale = Vector3.one;

            // Set the size to scaledSize with rounded integers
            rectTransform.sizeDelta = new Vector2(roundedWidth, roundedHeight);

            // Adjust the scale for children (if any)
            foreach(RectTransform child in rectTransform)
            {
                AdjustScale(new UnityEditor.MenuCommand(child));
            }
        }
    }
}
