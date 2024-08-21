using System.Collections.Generic;
using Cysharp.Text;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pxp
{
    public class Popup_Result : Popup<Popup_Result>
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textWave;

        [SerializeField, GetComponentInChildrenOnly]
        private List<UIItem> _items;

        private float _time;

        public void SetWave(int wave)
        {
            _items.ForEach(_ => _.SetActive(false));

            var waveData = SpecDataManager.Inst.Wave.Get(wave);
            _textWave.SetTextFormat("웨이브{0} 클리어", waveData.wave);
            for (int i = 0; i < waveData.rewardType.Length; i++)
            {
                var itemData = SpecDataManager.Inst.Currency.Get((int) waveData.rewardType[i]);
                if (itemData != null)
                    _items[i].SetView(itemData, waveData.rewardAmount[i]);
            }

            _time = Time.time;
            Show();
        }

        public override void Hide()
        {
            if (_time + 1f < Time.time)
            {
                base.Hide();
                SceneManager.LoadScene("Lobby");
            }
        }
    }
}
