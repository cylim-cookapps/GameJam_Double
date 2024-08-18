using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class Popup_Double : Popup<Popup_Double>
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textBonusCoin, _textSuccess;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnTry, _btnReceive;

        public override void Initialize()
        {
            base.Initialize();
            _btnTry.AddListener(OnClickTry);
            _btnReceive.AddListener(OnClickReceive);
        }

        public override void Show()
        {
            base.Show();
            Refresh();
        }

        protected override void Awake()
        {
            base.Awake();
            EventManager.Inst.EventGameBonusCoin.AddListener(OnEventGameBonusCoin);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.Inst.EventGameBonusCoin.RemoveListener(OnEventGameBonusCoin);
        }

        private void Refresh()
        {
            if (GameManager.Inst.MyInGameUserData.BonusCoin == 0)
            {
                Hide();
                return;
            }

            _textBonusCoin.SetText(GameManager.Inst.MyInGameUserData.BonusCoin);
            _textSuccess.SetTextFormat("더블 성공 시 {0}코인 획득", GameManager.Inst.MyInGameUserData.BonusCoin * 2);
        }

        #region Event

        private void OnClickTry()
        {
            GameManager.Inst.SetDoubleBonusCoin(Random.Range(0, 10) >= 5);
        }

        private void OnClickReceive()
        {
            GameManager.Inst.ReceiveBonusCoin();
        }

        #endregion

        #region EventHandler

        private void OnEventGameBonusCoin(int bonusCoin)
        {
            Refresh();
        }

        #endregion
    }
}
