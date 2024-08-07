using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pxp
{
    public class UIEquipHeroItem : MonoBehaviour
    {
        [SerializeField, GetComponent]
        private Button _btn;

        [SerializeField, GetComponentInChildrenOnly]
        private UIHeroItem _uiHeroItem;

        [SerializeField, GetComponentInChildrenName]
        private Image _imgArrow;

        private UserHeroItem _data;

        private UnityAction<int> _equipIndex;

        private int _index;
        private bool _isEquip;

        public void OnInitialize(int index)
        {
            EventManager.Inst.EventEquippedHero.AddListener(OnEventEquippedHero);
            _index = index;
            _btn.AddListener(OnClick);
            SetEquipMode(false);
            Refresh();
        }

        public void SetEquipMode(bool isEquip)
        {
            _isEquip = isEquip;
            _imgArrow.SetActive(isEquip);
        }

        public void Refresh()
        {
            _data = UserManager.Inst.Hero.GetEquippedHero(_index);
            if (_data != null)
            {
                _uiHeroItem.SetHero(_data);
                _uiHeroItem.SetActive(true);
            }
            else
            {
                _uiHeroItem.SetActive(false);
            }
        }

        #region Event

        public void OnClick()
        {
            if (_isEquip)
            {
                MainUI.Inst.OutGameUI.HeroUI.SetEquip(_index);
            }
            else
            {
                if (_data != null)
                    PopupManager.Inst.GetPopup<Popup_Hero_Info>().SetViewHero(_data);
            }
        }

        #endregion

        #region EventHandler

        private void OnEventEquippedHero()
        {
            Refresh();
        }

        #endregion
    }
}
