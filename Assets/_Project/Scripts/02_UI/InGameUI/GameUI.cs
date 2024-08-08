using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textWave, _textMonster, _textTimer;

        private void Awake()
        {
            OnEventGameTimer(0);
            OnEventGameTimer(0);

            EventManager.Inst.EventGameTimer.AddListener(OnEventGameTimer);
            EventManager.Inst.EventWave.AddListener(OnEventWave);
        }

        private void OnDestroy()
        {
            EventManager.Inst.EventGameTimer.RemoveListener(OnEventGameTimer);
            EventManager.Inst.EventWave.RemoveListener(OnEventWave);
        }

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

        #endregion
    }
}
