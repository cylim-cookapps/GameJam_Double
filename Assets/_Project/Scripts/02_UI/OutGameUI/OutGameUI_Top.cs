using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
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
            _textLevel.SetText(UserManager.Inst.Info.Level);
            var energy = UserManager.Inst.Currency.GetCurrency(Enum_ItemType.Energy);

            _textEnergy.SetTextFormat("{0}/{1}", energy.Count, energy.Spec.auto_max_count);
            _textGold.SetText(UserManager.Inst.Currency.GetCurrency(Enum_ItemType.Gold).Count);
            _textGem.SetText(UserManager.Inst.Currency.GetCurrency(Enum_ItemType.Gem).Count);
        }
    }
}
