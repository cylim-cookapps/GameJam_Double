using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class UIProfileInGameItem : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textName, _textLevel;

        public void SetProfile(string name, int level)
        {
            _textName.SetText(name);
            _textLevel.SetText(level);
        }
    }
}
