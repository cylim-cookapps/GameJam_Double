using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class Popup_Profile : Popup<Popup_Profile>
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textName, _textLevel, _textExp, _textUID, _textStage, _textTryCount, _textKillCount;

        [SerializeField, GetComponentInChildrenName]
        private Slider _sliderExp;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnNickname;

        public override void Initialize()
        {
            base.Initialize();
            _btnNickname.AddListener(OnClickNickname);
            EventManager.Inst.EventNickname.AddListener(OnEventNickname);
        }

        public override void Show()
        {
            base.Show();
            _sliderExp.maxValue = UserManager.Inst.Info.CurrentLevel.exp;
            _textName.SetText(UserManager.Inst.NickName);
            _textLevel.SetText(UserManager.Inst.Info.Level);
            _sliderExp.value = UserManager.Inst.Info.Exp;
            _textUID.SetText(UserManager.Inst.PlayerId);
            _textStage.SetText(UserManager.Inst.Info.Stage);
            _textTryCount.SetText(UserManager.Inst.Info.TryCount);

            _textExp.SetTextFormat("{0}/{1}", _sliderExp.value, _sliderExp.maxValue);
        }

        #region Event

        private void OnClickNickname()
        {
            PopupManager.Inst.GetPopup<Popup_Change_Name>().Show();
        }

        #endregion

        #region EventHandler

        private void OnEventNickname(string nickName)
        {
            _textName.SetText(nickName);
        }

        #endregion
    }
}
