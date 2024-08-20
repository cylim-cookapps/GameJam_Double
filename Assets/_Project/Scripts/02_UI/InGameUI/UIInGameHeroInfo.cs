using System.Collections;
using System.Collections.Generic;
using Pxp.Data;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class UIInGameHeroInfo : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textName, _textTier, _textAtk, _textAtkSpeed;

        [SerializeField, GetComponentInChildrenOnly]
        private UIHeroItem _uiHeroItem;

        public void SetViewHeroUnit(HeroUnit unit)
        {
            _uiHeroItem.SetHero(unit.InGameHeroData);
            _textName.SetText(unit.HeroData.hero_name);
            _textTier.SetText(unit.HeroData.TierName);
            _textAtk.SetText(unit._attack);
            _textAtkSpeed.SetText(unit._attackSpeed);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
