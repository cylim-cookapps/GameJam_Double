using System.Collections;
using System.Collections.Generic;
using Pxp.Data;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class OutGameUI_Top : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textNick, _textEnergy, _textGold, _textGem, _textLevel;

        public void OnInitialize()
        {
            _textNick.SetText(UserManager.Inst.NickName);
            _textLevel.SetText(UserManager.Inst.Level.ToString());
            _textEnergy.SetText(UserManager.Inst.Currency.GetCurrency(Enum_ItemType.Energy).Count.ToString());
        }

    }
}
