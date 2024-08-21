using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public void SetView(Hero data, int count)
        {
            if (data != null)
            {
                _imgIcon.SetSprite(data.icon_key);
                _textLevel.SetTextFormat("x{0}",count);

                for (int i = 0; i < _goTier.Count; i++)
                {
                    _goTier[i].SetActive(i == (int) data.tier - 1);
                }

                _stars.ForEach(_ => _.SetActive(false));
                _inGameStars.ForEach(_ => _.SetActive(false));
            }
        }

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

        public void SetHero(InGameHeroData data, InGameUnitData unitData)
        {
            var spec = SpecDataManager.Inst.Hero.Get(data.HeroId);

            for (int i = 0; i < _goTier.Count; i++)
            {
                _goTier[i].SetActive(i == (int) spec.tier - 1);
            }

            _inGameStars.ForEach(_ => _.SetActive(false));
            if (unitData.Grade > 2)
            {
                _inGameStars[3].SetActive(unitData.Grade > 2);
                _inGameStars[4].SetActive(unitData.Grade > 3);
                _inGameStars[5].SetActive(unitData.Grade > 4);
            }
            else
            {
                _inGameStars[0].SetActive(unitData.Grade >= 0);
                _inGameStars[1].SetActive(unitData.Grade > 0);
                _inGameStars[2].SetActive(unitData.Grade > 1);
            }

            _stars.ForEach(_ => _.SetActive(false));
            _imgIcon.SetSprite(spec.icon_key);
            _textLevel.SetTextFormat("Lv.{0}", data.InGameLevel + 1);
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
