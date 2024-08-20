using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class UIHeroItem : CachingMono
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textLevel;

        [SerializeField, GetComponentInChildrenName]
        private Image _imgIcon;

        [SerializeField]
        private List<GameObject> _goTier;

        [SerializeField]
        private List<GameObject> _stars;

        [SerializeField]
        private List<GameObject> _inGameStars;

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

            _imgIcon.SetSprite(_heroData.Spec.icon_key);
            Refresh();
        }

        public void SetHero(InGameHeroData data)
        {
            var spec = SpecDataManager.Inst.Hero.Get(data.HeroId);

            for (int i = 0; i < _goTier.Count; i++)
            {
                _goTier[i].SetActive(i == (int) spec.tier - 1);
            }

            _inGameStars.ForEach(_ => _.SetActive(false));
            if (data.Merge > 2)
            {
                _inGameStars[3].SetActive(data.Merge > 2);
                _inGameStars[4].SetActive(data.Merge > 3);
                _inGameStars[5].SetActive(data.Merge > 4);
            }
            else
            {
                _inGameStars[0].SetActive(data.Merge == 0);
                _inGameStars[1].SetActive(data.Merge > 0);
                _inGameStars[2].SetActive(data.Merge > 1);
            }

            _stars.ForEach(_ => _.SetActive(false));
            _imgIcon.SetSprite(spec.icon_key);
            _textLevel.SetTextFormat("Lv.{0}", data.Upgrade+1);
        }

        private void Refresh()
        {
            _textLevel.SetTextFormat("Lv.{0}", _heroData.Level);

            for (int i = 0; i < _stars.Count; i++)
            {
                _stars[i].SetActive(i < _heroData.Star);
            }

            _inGameStars.ForEach(_ => _.SetActive(false));
        }

        #region EventHandler

        private void OnEventUpdate()
        {
            Refresh();
        }

        #endregion
    }
}
