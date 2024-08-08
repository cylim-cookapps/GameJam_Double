using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public class HeroUI : BoardBase<MainUI>
    {
        [SerializeField, GetComponentInChildrenOnly]
        private List<UIEquipHeroItem> _uiEquipHeroItems;

        [SerializeField]
        private UIUserHeroItem _prefab;

        [SerializeField]
        private GameObject _goScroll, _goSelected;

        [SerializeField]
        private Transform _trOwnedList, _trLockedList;

        public override MainUI Parent { get; protected set; }
        private List<UIUserHeroItem> _uiUserHeroItems = new();

        private UserHeroItem _selectedHero;

        public override void OnInitialize(MainUI parent)
        {
            base.OnInitialize(parent);

            for (int i = 0; i < _uiEquipHeroItems.Count; i++)
            {
                int index = i;
                _uiEquipHeroItems[i].OnInitialize(index);
            }

            EventManager.Inst.EventUnlockHero.AddListener(OnEventUnlockHero);
            EventManager.Inst.EventEquippedHero.AddListener(OnEventEquippedHero);
        }

        public void Refresh()
        {
            _uiEquipHeroItems.ForEach(_ => _.Refresh());

            for (int i = 0; i < _uiUserHeroItems.Count; i++)
            {
                _uiUserHeroItems[i].Dispose();
                PoolManager.Inst.Release(_uiUserHeroItems[i]);
            }

            _uiUserHeroItems.Clear();

            foreach (var hero in UserManager.Inst.Hero.Heroes)
            {
                UIUserHeroItem item = PoolManager.Inst.Get(_prefab, hero.Unlock ? _trOwnedList : _trLockedList);
                item.OnInitialize(hero);
                _uiUserHeroItems.Add(item);
            }
        }

        public void SetEquipMode(UserHeroItem hero)
        {
            _selectedHero = hero;
            if (_selectedHero != null)
            {
                _goScroll.SetActive(false);
                _goSelected.SetActive(true);
                _uiEquipHeroItems.ForEach(_ => _.SetEquipMode(true));
            }
            else
            {
                _goScroll.SetActive(true);
                _goSelected.SetActive(false);
                _uiEquipHeroItems.ForEach(_ => _.SetEquipMode(false));
            }
        }

        public override void Show()
        {
            Refresh();
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            SetEquipMode(null);
            gameObject.SetActive(false);
        }

        public void SetEquip(int index)
        {
            UserManager.Inst.Hero.SetEquipHero(index, _selectedHero);
        }

        #region Event

        private void OnClickEquipCancel()
        {
            SetEquipMode(null);
        }

        #endregion

        #region EventHandler

        private void OnEventUnlockHero()
        {
            Refresh();
        }

        private void OnEventEquippedHero()
        {
            Refresh();
            SetEquipMode(null);
        }

        #endregion"
    }
}
