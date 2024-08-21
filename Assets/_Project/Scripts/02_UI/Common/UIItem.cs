using System.Collections;
using System.Collections.Generic;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class UIItem : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private Image _imgIcon;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textCount;

        public void SetView(Currency currency, int count)
        {
            _imgIcon.SetSprite(currency.icon_key);
            _textCount.SetText(count);
            gameObject.SetActive(true);
        }
    }
}
