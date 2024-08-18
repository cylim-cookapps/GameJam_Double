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

        [SerializeField, GetComponentInChildrenName]
        private Button _btnGamble, _btnReceive;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textPrice;

        private int GambleChip;

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
        }

        public override void Show()
        {
            base.Show();

            GameManager.Inst.IsFirstFail = false;

            Refresh();
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
            GameManager.Inst.SetGamble(GambleChip);
            Refresh();
        }

        private void OnClickReceive()
        {
            if (GameManager.Inst.ReceiveGamble())
                Hide();
        }

        #endregion
    }
}
