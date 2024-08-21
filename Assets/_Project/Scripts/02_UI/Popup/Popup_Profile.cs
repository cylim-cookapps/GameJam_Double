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
        private Button _btnNickname,_btnCopy;

        public override void Initialize()
        {
            base.Initialize();
            _btnNickname.AddListener(OnClickNickname);
            _btnCopy.AddListener(OnClickCopy);
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
            _textStage.SetTextFormat("{0} 웨이브", UserManager.Inst.Info.Wave);
            _textTryCount.SetText(UserManager.Inst.Info.TryCount);
            _textKillCount.SetText(UserManager.Inst.Info.KillCount);

            _textExp.SetTextFormat("{0}/{1}", _sliderExp.value, _sliderExp.maxValue);
        }

        #region Event

        private void OnClickNickname()
        {
            PopupManager.Inst.GetPopup<Popup_Change_Name>().Show();
        }

        private void OnClickCopy()
        {
            GUIUtility.systemCopyBuffer = UserManager.Inst.PlayerId;
            EventManager.Inst.OnEventToast("클립보드에 복사되었습니다.");
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
