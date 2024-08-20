using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class UIHeroUpgradeItem : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenOnly]
        private Button _btn;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textPrice;

        [SerializeField, GetComponentInChildrenName]
        private Image _imgIcon;

        [SerializeField]
        private List<GameObject> _goTier;

        [SerializeField]
        private List<GameObject> _goTierBottom;

        private Hero _heroData;
        private InGameHeroData _inGameHeroData;
        private int LevelUp_Default;
        private int LevelUp_Increase;

        private void Awake()
        {
            EventManager.Inst.EventGameCoin.AddListener(OnEventGameCoin);
            EventManager.Inst.EventGameState.AddListener(OnEventGameState);
        }

        private void OnDestroy()
        {
            EventManager.Inst.EventGameCoin.RemoveListener(OnEventGameCoin);
            EventManager.Inst.EventGameState.RemoveListener(OnEventGameState);
        }

        public void Init(int heroId)
        {
            LevelUp_Default = (int) SpecDataManager.Inst.Option.Get("LevelUp_Default").value;
            LevelUp_Increase = (int) SpecDataManager.Inst.Option.Get("LevelUp_Increase").value;
            _heroData = SpecDataManager.Inst.Hero.Get(heroId);
            _imgIcon.SetSprite(_heroData.icon_key);
            _textPrice.SetText(LevelUp_Default);

            for (int i = 0; i < _goTier.Count; i++)
            {
                _goTier[i].SetActive(i == (int) _heroData.tier - 1);
                _goTierBottom[i].SetActive(i == (int) _heroData.tier - 1);
            }

            _btn.AddListener(OnClickUpgrade);
        }

        #region Event

        private void OnClickUpgrade()
        {
            if (_inGameHeroData != null)
                GameManager.Inst.UpgradeHero(_inGameHeroData);
        }

        #endregion

        #region EventHandler

        private void OnEventGameCoin(int coin)
        {
            if (GameManager.Inst.CurrGameState == Enum_GameState.Start)
            {
                var needCoin = LevelUp_Default + LevelUp_Increase * _inGameHeroData.InGameLevel;
                _textPrice.SetText(needCoin);
                if (needCoin > coin)
                    _textPrice.color = Color.red;
                else
                    _textPrice.color = Color.white;
            }
        }

        private void OnEventGameState(Enum_GameState state)
        {
            if (state == Enum_GameState.Start)
            {
                _inGameHeroData = GameManager.Inst.MyInGameUserData.Heroes.Find(x => x.HeroId == _heroData.id);
                var needCoin = LevelUp_Default + LevelUp_Increase * _inGameHeroData.InGameLevel;
                _textPrice.SetText(needCoin);

                if (needCoin > GameManager.Inst.MyInGameUserData.Coin)
                    _textPrice.color = Color.red;
                else
                    _textPrice.color = Color.white;
            }
        }

        #endregion
    }
}
