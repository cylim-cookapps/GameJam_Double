using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class Popup_Hero_Info : Popup<Popup_Hero_Info>
    {
        [SerializeField, GetComponentInChildrenOnly]
        private UIHeroItem _uiHeroItem;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textTier, _textName, _textAtk, _textAtkSpeed, _textCount;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnEquip, _btnStarUp, _btnUnlock;

        [SerializeField, GetComponentInChildrenName]
        private Slider _sliderCard;

        [SerializeField, GetComponentInChildrenName]
        private CurrencyButton _btnLevelUp;

        [SerializeField, GetComponentInChildrenOnly]
        private List<UIHeroSkillInfo> _heroSkill;

        private UserHeroItem _data;

        public override void Initialize()
        {
            base.Initialize();
            _btnEquip.AddListener(OnClickEquip);
            _btnUnlock.AddListener(OnClickUnlock);
            _btnStarUp.AddListener(OnClickStarUp);
            _btnLevelUp.SetOnClick(OnClickLevelUp);

            EventManager.Inst.EventHeroLevelUp.AddListener(OnEventHeroLevelUp);
        }

        public void SetViewHero(UserHeroItem heroData)
        {
            _data = heroData;
            Refresh();
            Show();
        }

        private void Refresh()
        {
            _uiHeroItem.SetHero(_data);
            _textTier.SetText(_data.Spec.TierName);
            _textName.SetText(_data.Spec.hero_name);
            _textAtk.SetText(_data.Atk);
            _textAtkSpeed.SetText(_data.AtkSpeed);
            _sliderCard.maxValue = _data.NeedCardCount;
            _sliderCard.value = _data.Count;

            _textCount.SetTextFormat("{0} / {1}", _data.Count, _data.NeedCardCount);
            _btnLevelUp.SetPrice(_data.LevelUpGold, _data.CheckLevelUp());
            _heroSkill[0].SetHeroSkill(_data, 0, _data.Spec.skill_star_1ToData);
            _heroSkill[1].SetHeroSkill(_data, 1, _data.Spec.skill_star_2ToData);
            _heroSkill[2].SetHeroSkill(_data, 2, _data.Spec.skill_star_3ToData);
            _heroSkill[3].SetHeroSkill(_data, 3, _data.Spec.skill_star_4ToData);
            _heroSkill[4].SetHeroSkill(_data, 4, _data.Spec.skill_star_5ToData);

            if (_data.Unlock)
            {
                _btnUnlock.SetActive(false);
                _btnEquip.SetActive(_data.Unlock);
                _btnStarUp.SetActive(!_data.IsMaxStar);
                _btnLevelUp.SetActive(true);
                if (_data.IsMaxStar)
                {
                    _sliderCard.maxValue = 1;
                    _sliderCard.value = 1;
                }
            }
            else
            {
                _btnUnlock.SetActive(true);
                _btnEquip.SetActive(false);
                _btnStarUp.SetActive(false);
                _btnLevelUp.SetActive(false);
            }
        }

        #region Event

        private void OnClickEquip()
        {
            MainUI.Inst.HeroUI.SetEquipMode(_data);
            AudioController.Play("SFX_Click");
            Hide();
        }

        private void OnClickUnlock()
        {
            if (_data.SetUnlock())
            {
                EventManager.Inst.OnEventToast($"{_data.Spec.hero_name}영웅이 해금되었습니다.");
                AudioController.Play("SFX_Hero_Merge");
                Refresh();
            }
        }

        private void OnClickStarUp()
        {
            if (_data.StarUp())
            {
                EventManager.Inst.OnEventToast($"{_data.Spec.hero_name}영웅에 성급이 상승했습니다.");
                AudioController.Play("SFX_Hero_Merge");
                Refresh();
            }
        }

        private void OnClickLevelUp()
        {
            _data.LevelUp();
        }

        #endregion

        #region EventHandler

        private void OnEventHeroLevelUp(int heroId)
        {
            Refresh();
        }

        #endregion
    }
}
