using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class HomeUI : BoardBase<MainUI>
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textNick, _textEnergy, _textLevel,_textWave,_textBox;

        [SerializeField, GetComponentInChildrenOnly]
        private List<UIHeroItem> _equippedHeroes;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnStart, _btnHell;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnProfile,_btnPass, _btnPack, _btnVip, _btnBoxOpen, _btnQuest, _btnAttendance, _btnRanking;

        [SerializeField, GetComponentInChildrenName]
        private Slider _sliderBox;

        public override MainUI Parent { get; protected set; }

        public override void OnInitialize(MainUI parent)
        {
            base.OnInitialize(parent);

            _btnProfile.AddListener(OnClickProfile);
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
            _textWave.SetTextFormat("최고 기록 {0} Wave", UserManager.Inst.Info.Wave);
            var energy = UserManager.Inst.Currency.GetCurrency(Enum_ItemType.Energy);
            _textEnergy.SetTextFormat("{0}/{1}", energy.Count, energy.Spec.auto_max_count);
            _sliderBox.maxValue = 10;

            EventManager.Inst.EventEquippedHero.AddListener(OnEventEquippedHero);
            EventManager.Inst.EventNickname.AddListener(OnEventNickname);
            OnEventEquippedHero();
        }

        public override void Show()
        {
            var count = UserManager.Inst.Currency.GetCurrency(Enum_ItemType.Ticket_NormalChest).Count;
            _textBox.SetTextFormat("{0}/{1}",count, 10);
            _sliderBox.value = count;
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

            AudioController.Play("SFX_Click");
            LobbyManager.Inst.QuickMatch(true).Forget();
        }

        private void OnClickHellMode()
        {
            EventManager.Inst.OnEventToast("업데이트 준비중입니다.");
            return;
            if (UserManager.Inst.Hero.EnterBattle() == false)
                return;

            AudioController.Play("SFX_Click");
            LobbyManager.Inst.QuickMatch(true).Forget();
        }

        private void OnClickProfile()
        {
            PopupManager.Inst.GetPopup<Popup_Profile>().Show();
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
            var list=UserManager.Inst.Currency.GetBox();
            if(list == null)
                EventManager.Inst.OnEventToast("상자가 부족합니다.");
            else
            {
                PopupManager.Inst.GetPopup<Popup_Reward>().SetView(list);
            }
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

        private void OnEventNickname(string nickName)
        {
            _textNick.SetText(nickName);
        }

        #endregion
    }
}
