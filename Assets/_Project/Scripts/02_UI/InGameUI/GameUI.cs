using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textWave, _textMonster, _textTimer, _textCoin, _textChip;

        [SerializeField, GetComponentInChildrenName]
        private Slider _sliderMonster;

        [SerializeField, GetComponentInChildrenName]
        private Button _btnSummon;

        private void Awake()
        {
            OnEventGameTimer(0);
            OnEventGameTimer(0);
            OnEventGameCoin((int) SpecDataManager.Inst.Option.Get("StartCoin").value);
            OnEventGameChip(0);
            _sliderMonster.maxValue = 100;
            _btnSummon.AddListener(OnClickSummon);

            EventManager.Inst.EventGameTimer.AddListener(OnEventGameTimer);
            EventManager.Inst.EventWave.AddListener(OnEventWave);
            EventManager.Inst.EventMonsterCount.AddListener(OnEventMonsterCount);
            EventManager.Inst.EventGameCoin.AddListener(OnEventGameCoin);
            EventManager.Inst.EventGameChip.AddListener(OnEventGameChip);
        }

        private void OnDestroy()
        {
            EventManager.Inst.EventGameTimer.RemoveListener(OnEventGameTimer);
            EventManager.Inst.EventWave.RemoveListener(OnEventWave);
            EventManager.Inst.EventMonsterCount.RemoveListener(OnEventMonsterCount);
            EventManager.Inst.EventGameCoin.RemoveListener(OnEventGameCoin);
            EventManager.Inst.EventGameChip.RemoveListener(OnEventGameChip);
        }

        #region Event

        private void OnClickSummon()
        {
            GameManager.Inst.SpawnHero();
        }

        #endregion

        #region EventHandler

        private void OnEventGameTimer(int sec)
        {
            int minutes = Mathf.FloorToInt(sec / 60f);
            int seconds = Mathf.FloorToInt(sec % 60f);
            _textTimer.SetTextFormat("{0:00}:{1:00}", minutes, seconds);
        }

        private void OnEventWave(int wave)
        {
            _textWave.SetTextFormat("WAVE {0}", wave + 1);
        }

        private void OnEventMonsterCount(int count)
        {
            _sliderMonster.value = count;
            _textMonster.SetTextFormat("{0}/{1}", count, _sliderMonster.maxValue);
        }

        private void OnEventGameChip(int chip)
        {
            _textChip.SetText(chip);
        }

        private void OnEventGameCoin(int coin)
        {
            _textCoin.SetText(coin);
        }

        #endregion
    }
}
