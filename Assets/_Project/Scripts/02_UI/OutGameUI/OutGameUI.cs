using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class OutGameUI : UIBoard
    {
        [SerializeField, GetComponentInChildrenOnly]
        private OutGameUI_Top _top;

        [SerializeField, GetComponentInChildrenOnly]
        private HeroUI _heroUI;

        [SerializeField]
        private List<UIHeroItem> _equippedHeroes;

        [SerializeField]
        private List<Toggle> _togglesTab;

        public HeroUI HeroUI => _heroUI;

        public override void OnInitialize()
        {
            _top.OnInitialize();
            _heroUI.OnInitialize(this);

            for (int i = 0; i < _togglesTab.Count; i++)
            {
                var index = i;
                _togglesTab[i].onValueChanged.AddListener(isOn => OnValueChangedTab(isOn, (Enum_OutGameTab) index));
            }

            EventManager.Inst.EventEquippedHero.AddListener(OnEventEquippedHero);
            OnEventEquippedHero();
        }

        #region Event

        private void OnValueChangedTab(bool isOn, Enum_OutGameTab tab)
        {
            if (isOn)
            {
                switch (tab)
                {
                    case Enum_OutGameTab.Shop:
                        break;
                    case Enum_OutGameTab.Hero:
                        _heroUI.Show();
                        break;
                    case Enum_OutGameTab.Relic:
                        break;
                }
            }
            else
            {
                switch (tab)
                {
                    case Enum_OutGameTab.Shop:
                        break;
                    case Enum_OutGameTab.Hero:
                        _heroUI.Hide();
                        break;
                    case Enum_OutGameTab.Relic:
                        break;
                }
            }
        }

        #endregion

        #region EventHandler

        private void OnEventEquippedHero()
        {
            for (int i = 0; i < UserManager.Inst.Hero.EquipHeroes.Count; i++)
            {
                UserHeroItem hero = UserManager.Inst.Hero.GetEquippedHero(i);
                if (hero != null)
                {
                    _equippedHeroes[i].SetHero(hero);
                    _equippedHeroes[i].SetActive(true);
                }
                else
                {
                    _equippedHeroes[i].SetActive(false);
                }
            }
        }

        #endregion
    }

    public enum Enum_OutGameTab
    {
        Shop = 0,
        Hero,
        Home,
        Relic,
        Lock,
    }
}
