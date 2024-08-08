using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class UIInGameProfile : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textName, _textLevel;

        public void Init(InGameUserData data)
        {
            _textName.SetText(data.Name);
            _textLevel.SetText(data.Level);
        }
    }
}
