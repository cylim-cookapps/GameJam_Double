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
        private ShopUI _shopUI;

        [SerializeField, GetComponentInChildrenOnly]
        private HeroUI _heroUI;

        [SerializeField, GetComponentInChildrenOnly]
        private HomeUI _homeUI;

        [SerializeField]
        private List<Toggle> _togglesTab;

        public ShopUI ShopUI => _shopUI;
        public HeroUI HeroUI => _heroUI;
        public HomeUI HomeUI => _homeUI;

        public override void OnInitialize()
        {
            _shopUI.OnInitialize(this);
            _heroUI.OnInitialize(this);
            _homeUI.OnInitialize(this);

            for (int i = 0; i < _togglesTab.Count; i++)
            {
                var index = i;
                _togglesTab[i].onValueChanged.AddListener(isOn => OnValueChangedTab(isOn, (Enum_OutGameTab) index));
            }
        }

        #region Event

        private void OnValueChangedTab(bool isOn, Enum_OutGameTab tab)
        {
            if (isOn)
            {
                switch (tab)
                {
                    case Enum_OutGameTab.Shop:
                        _shopUI.Show();
                        break;
                    case Enum_OutGameTab.Hero:
                        _heroUI.Show();
                        break;
                    case Enum_OutGameTab.Home:
                        _homeUI.Show();
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
                        _shopUI.Hide();
                        break;
                    case Enum_OutGameTab.Hero:
                        _heroUI.Hide();
                        break;
                    case Enum_OutGameTab.Home:
                        _homeUI.Hide();
                        break;
                    case Enum_OutGameTab.Relic:
                        break;
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
