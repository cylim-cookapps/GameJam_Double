using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class Popup_Gamble : Popup<Popup_Gamble>
    {
        [SerializeField]
        private List<Image> _imagesChar, _imagesBefor, _imagesFail;

        [SerializeField]
        private List<Animator> _animators;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnGamble, _btnReceive;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textPrice,_textChip;

        private int GambleChip;
        private bool isAnimation = false;

        public override void Initialize()
        {
            base.Initialize();
            GambleChip = (int) SpecDataManager.Inst.Option.Get("Gamble_ChipValue").value;
            _textPrice.SetText(GambleChip);
            _imagesChar.ForEach(_ => _.gameObject.SetActive(false));
            _imagesBefor.ForEach(_ => _.gameObject.SetActive(false));
            _imagesFail.ForEach(_ => _.gameObject.SetActive(false));
            _btnGamble.AddListener(OnClickGamble);
            _btnReceive.AddListener(OnClickReceive);
            EventManager.Inst.EventGameChip.AddListener(OnEventGameChip);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.Inst.EventGameChip.RemoveListener(OnEventGameChip);
        }


        public override void Show()
        {
            base.Show();

            _textChip.SetText(GameManager.Inst.MyInGameUserData.Chip);
            GameManager.Inst.IsFirstFail = false;

            Refresh();
        }

        public override void Hide()
        {
            if (isAnimation)
                return;

            base.Hide();
        }

        private void Refresh()
        {
            _imagesChar.ForEach(_ => _.gameObject.SetActive(false));
            _imagesBefor.ForEach(_ => _.gameObject.SetActive(false));
            _imagesFail.ForEach(_ => _.gameObject.SetActive(false));

            if (GameManager.Inst.Gamble.Count == 0)
            {
                _imagesBefor.ForEach(_ => _.gameObject.SetActive(true));
                if (GameManager.Inst.IsFirstFail)
                {
                    _imagesFail[0].SetActive(true);
                    _imagesBefor[0].SetActive(false);
                }
            }
            else
            {
                bool isEndCheck = false;
                for (int i = 0; i < 5; i++)
                {
                    if (GameManager.Inst.Gamble.Count > i)
                    {
                        _imagesChar[i].SetSprite(SpecDataManager.Inst.Hero.Get(GameManager.Inst.Gamble[i]).icon_key);
                        _imagesChar[i].SetActive(true);
                    }
                    else
                    {
                        if (isEndCheck == false && GameManager.Inst.IsGambleEnd)
                        {
                            _imagesFail[i].SetActive(true);
                            isEndCheck = true;
                        }
                        else
                        {
                            _imagesBefor[i].SetActive(true);
                        }
                    }
                }
            }

            _btnGamble.SetActive(!GameManager.Inst.IsGambleEnd);
            if (GameManager.Inst.Gamble.Count > 0 || GameManager.Inst.IsGambleEnd)
                _btnReceive.SetActive(true);
            else
                _btnReceive.SetActive(false);
        }

        #region Event

        private void OnClickGamble()
        {
            if (isAnimation)
                return;

            var result = GameManager.Inst.SetGamble(GambleChip);
            if (result.Item1 == -1)
            {
                StartCoroutine(CoLose(result.Item2));
            }
            else
            {
                StartCoroutine(CoWin(result.Item2));
            }
        }

        IEnumerator CoLose(int index)
        {
            if (index < 0)
                index = 0;

            isAnimation = true;
            _animators[index].Play("Gamble_Slot_Spin");
            yield return new WaitForSeconds(1f);
            Refresh();
            _animators[index].Play("Gamble_Slot_Lose");
            EventManager.Inst.OnEventToast("도박 실패..");

            yield return new WaitForSeconds(0.5f);
            isAnimation = false;
        }

        IEnumerator CoWin(int index)
        {
            if (index < 0)
                index = 0;

            isAnimation = true;
            _animators[index].Play("Gamble_Slot_Spin");
            yield return new WaitForSeconds(1f);
            Refresh();
            _animators[index].Play("Gamble_Slot_Win");
            EventManager.Inst.OnEventToast("도박 성공!!");
            yield return new WaitForSeconds(0.5f);
            isAnimation = false;
        }

        private void OnClickReceive()
        {
            if (GameManager.Inst.ReceiveGamble())
                Hide();
        }

        #endregion


        private void OnEventGameChip(int chip)
        {
            _textChip.SetText(chip);
        }
    }
}
