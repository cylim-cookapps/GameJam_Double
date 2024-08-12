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
        private Button _btnConfirm, _btnConfirmPaid;

        public override void Initialize()
        {
            base.Initialize();
            _btnConfirm.AddListener(OnClick);
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

            MainUI.Inst.SetIndicator(false);
        }

        #endregion
    }
}
