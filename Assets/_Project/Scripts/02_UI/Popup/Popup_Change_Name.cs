using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class Popup_Change_Name : Popup<Popup_Change_Name>
    {
        [SerializeField, GetComponentInChildrenName]
        private TMP_InputField _input;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnConfirm, _btnConfirmPaid,_btnClose;

        public override void Initialize()
        {
            base.Initialize();
            _btnConfirm.AddListener(OnClick);
            _btnClose.AddListener(OnClickClose);
        }

        public override void Show()
        {
            base.Show();
            _input.text = "";
        }

        public override void Hide()
        {
        }

        #region Event

        private void OnClickClose()
        {
            if (string.IsNullOrEmpty(UserManager.Inst.NickName))
                return;

            base.Hide();
        }

        private async void OnClick()
        {
            if (string.IsNullOrEmpty(_input.text))
                return;

            MainUI.Inst.SetIndicator(true);
            var state = await NicknameManager.Inst.SetNickname(_input.text);
            if (state == Enum_NickNameValid.Valid)
            {
                base.Hide();
            }
            else if (state == Enum_NickNameValid.Duplication)
            {
                EventManager.Inst.OnEventToast("중복된 닉네임입니다.");
            }
            else
            {
                EventManager.Inst.OnEventToast("올바르지 않은 닉네임입니다.");
            }

            MainUI.Inst.SetIndicator(false);
        }

        #endregion
    }
}
