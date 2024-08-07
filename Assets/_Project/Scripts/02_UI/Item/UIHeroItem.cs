using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;

namespace Pxp
{
    public class UIHeroItem : CachingMono
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textLevel;

        [SerializeField]
        private List<GameObject> _goTier;

        [SerializeField]
        private List<GameObject> _stars;

        private UserHeroItem _heroData;

        public void SetHero(UserHeroItem data)
        {
            _heroData?.EventUpdate.RemoveListener(OnEventUpdate);
            _heroData = null;

            if (data == null)
                return;

            _heroData = data;
            _heroData.EventUpdate.AddListener(OnEventUpdate);

            for (int i = 0; i < _goTier.Count; i++)
            {
                _goTier[i].SetActive(i == data.Tier - 1);
            }

            Refresh();
        }

        private void Refresh()
        {
            _textLevel.SetTextFormat("Lv.{0}", _heroData.Level);

            for (int i = 0; i < _stars.Count; i++)
            {
                _stars[i].SetActive(i < _heroData.Star);
            }
        }

        #region EventHandler

        private void OnEventUpdate()
        {
            Refresh();
        }

        #endregion
    }
}
