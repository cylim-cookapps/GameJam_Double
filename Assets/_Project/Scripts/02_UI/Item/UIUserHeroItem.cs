using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class UIUserHeroItem : MonoBehaviour
    {
        [SerializeField, GetComponent]
        private Button _btn;

        [SerializeField, GetComponentInChildrenOnly]
        private UIHeroItem _uiHeroItem;

        [SerializeField, GetComponentInChildrenName]
        private Slider _sliderGauge;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textCount;

        private UserHeroItem _data;

        private void Awake()
        {
            _btn.onClick.AddListener(OnClick);
        }

        public void OnInitialize(UserHeroItem data)
        {
            _data = data;
            _data.EventUpdate.AddListener(OnEventUpdate);

            Refresh();
        }

        public void Dispose()
        {
            _data?.EventUpdate.RemoveListener(OnEventUpdate);
            _data = null;
        }

        private void Refresh()
        {
            _uiHeroItem.SetHero(_data);
            _sliderGauge.maxValue = _data.NeedCardCount;
            _sliderGauge.value = _data.Count;
            _textCount.SetTextFormat("{0}/{1}", _data.Count, _data.NeedCardCount);
            if (_data.IsMaxStar)
            {
                _sliderGauge.maxValue = 1;
                _sliderGauge.value = 1;
            }
        }

        #region Event

        private void OnClick()
        {
            PopupManager.Inst.GetPopup<Popup_Hero_Info>().SetViewHero(_data);
        }

        #endregion

        #region EvenktHandler

        private void OnEventUpdate()
        {
            Refresh();
        }

        #endregion
    }
}
