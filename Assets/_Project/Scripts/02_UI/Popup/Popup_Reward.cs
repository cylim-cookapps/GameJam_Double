using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    public class Popup_Reward : Popup<Popup_Reward>
    {
        [SerializeField, GetComponentInChildrenOnly]
        private List<UIItem> _uiItems;

        [SerializeField, GetComponentInChildrenOnly]
        private List<UIHeroItem> _heroItems;

        private float _time;

        public void SetView(List<ItemInfo> rewards)
        {
            _uiItems.ForEach(_ => _.SetActive(false));
            _heroItems.ForEach(_ => _.SetActive(false));

            for (int i = 0; i < rewards.Count; i++)
            {
                var item = SpecDataManager.Inst.Currency.All.FirstOrDefault(_ => _.item_type == rewards[i].ItemType);
                var hero = SpecDataManager.Inst.Hero.All.FirstOrDefault(_ => _.item_type == rewards[i].ItemType);
                if (item != null)
                {
                    _uiItems[i].SetView(item, rewards[i].Count);
                    _uiItems[i].SetActive(true);
                    _uiItems[i].transform.SetSiblingIndex(i);
                }
                else if (hero != null)
                {
                    _heroItems[i].SetView(hero, rewards[i].Count);
                    _heroItems[i].SetActive(true);
                    _heroItems[i].transform.SetSiblingIndex(i);
                }
            }

            _time = Time.time;
            AudioController.Play("SFX_Reward");
            Show();
        }

        public override void Hide()
        {
            if (_time + 1f < Time.time)
            {
                base.Hide();
            }
        }
    }
}
