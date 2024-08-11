using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class Popup_Hero_Info : Popup<Popup_Hero_Info>
    {
        [SerializeField, GetComponentInChildrenOnly]
        private UIHeroItem _uiHeroItem;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textTier, _textName, _textAtk, _textAtkSpeed, _textCount;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnEquip, _btnStarUp, _btnUnEquip;

        [SerializeField, GetComponentInChildrenName]
        private CurrencyButton _btnLevelUp;

        private UserHeroItem _data;

        public override void Initialize()
        {
            base.Initialize();
            _btnEquip.AddListener(OnClickEquip);
            _btnUnEquip.AddListener(OnClickUnEquip);
            _btnStarUp.AddListener(OnClickStarUp);
            _btnLevelUp.SetOnClick(OnClickLevelUp);

            EventManager.Inst.EventHeroLevelUp.AddListener(OnEventHeroLevelUp);
        }

        public void SetViewHero(UserHeroItem heroData)
        {
            _data = heroData;
            _uiHeroItem.SetHero(_data);
            _textTier.SetText(heroData.TierName);
            _textName.SetText(heroData.Spec.hero_name);
            _textAtk.SetText(heroData.Atk);
            _textAtkSpeed.SetText(heroData.AtkSpeed);
            _textCount.SetTextFormat("{0} / {1}", heroData.Count, 50);
            _btnLevelUp.SetPrice(heroData.LevelUpGold, heroData.CheckLevelUp());

            Show();
        }

        #region Event

        private void OnClickEquip()
        {
            MainUI.Inst.HeroUI.SetEquipMode(_data);
            Hide();
        }

        private void OnClickUnEquip()
        {
            UserManager.Inst.Hero.SetUnEquipHero(_data);
        }

        private void OnClickStarUp()
        {
            _data.StarUp();
        }

        private void OnClickLevelUp()
        {
            _data.LevelUp();
        }

        #endregion

        #region EventHandler

        private void OnEventHeroLevelUp(int heroId)
        {
            SetViewHero(_data);
        }

        #endregion
    }
}
