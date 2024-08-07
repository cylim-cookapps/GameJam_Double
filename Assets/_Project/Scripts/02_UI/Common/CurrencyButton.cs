using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pxp
{
    [RequireComponent(typeof(Button))]
    public class CurrencyButton : MonoBehaviour
    {
        [SerializeField, GetComponent]
        private Button _btn;

        [SerializeField, GetComponentInChildrenName]
        private Image _imgIcon;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textPrice;

        public void SetPrice(double price, bool isAble)
        {
            _textPrice.SetText(price.ToDoubleString());
            _textPrice.color = isAble ? Color.white : Color.red;
        }

        public void SetOnClick(UnityAction onClick)
        {
            _btn.onClick.AddListener(onClick);
        }
    }
}
