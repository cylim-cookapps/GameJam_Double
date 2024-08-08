using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Pxp.Data;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class HomeUI : BoardBase<MainUI>
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textNick, _textEnergy, _textLevel;

        [SerializeField, GetComponentInChildrenOnly]
        private List<UIHeroItem> _equippedHeroes;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnStart, _btnHell;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnPass, _btnPack, _btnVip, _btnBoxOpen, _btnQuest, _btnAttendance, _btnRanking;

        public override MainUI Parent { get; protected set; }

        public override void OnInitialize(MainUI parent)
        {
            base.OnInitialize(parent);

            _btnPass.AddListener(OnClickPass);
            _btnPack.AddListener(OnClickPack);
            _btnVip.AddListener(OnClickVip);
            _btnBoxOpen.AddListener(OnClickBoxOpen);
            _btnQuest.AddListener(OnClickQuest);
            _btnAttendance.AddListener(OnClickAttendance);
            _btnRanking.AddListener(OnClickRanking);

            _btnHell.AddListener(OnClickHellMode);
            _btnStart.AddListener(OnClickGameStart);

            _textNick.SetText(UserManager.Inst.NickName);
            _textLevel.SetText(UserManager.Inst.Info.Level);
            var energy = UserManager.Inst.Currency.GetCurrency(Enum_ItemType.Energy);
            _textEnergy.SetTextFormat("{0}/{1}", energy.Count, energy.Spec.auto_max_count);

            EventManager.Inst.EventEquippedHero.AddListener(OnEventEquippedHero);
            OnEventEquippedHero();
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        #region Event

        private void OnClickGameStart()
        {
            if (UserManager.Inst.Hero.EnterBattle() == false)
                return;

            LobbyManager.Inst.QuickMatch(false).Forget();
        }

        private void OnClickHellMode()
        {
            if (UserManager.Inst.Hero.EnterBattle() == false)
                return;

            LobbyManager.Inst.QuickMatch(true).Forget();
        }

        private void OnClickPass()
        {
            EventManager.Inst.OnEventToast("업데이트 준비중입니다.");
        }

        private void OnClickPack()
        {
            EventManager.Inst.OnEventToast("업데이트 준비중입니다.");
        }

        private void OnClickVip()
        {
            EventManager.Inst.OnEventToast("업데이트 준비중입니다.");
        }

        private void OnClickBoxOpen()
        {
            EventManager.Inst.OnEventToast("업데이트 준비중입니다.");
        }

        private void OnClickQuest()
        {
            EventManager.Inst.OnEventToast("업데이트 준비중입니다.");
        }

        private void OnClickAttendance()
        {
            EventManager.Inst.OnEventToast("업데이트 준비중입니다.");
        }

        private void OnClickRanking()
        {
            EventManager.Inst.OnEventToast("업데이트 준비중입니다.");
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
}
