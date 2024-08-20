using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookApps.Obfuscator;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class UserHero : UserData
    {
        [JsonProperty("heroes")]
        public List<UserHeroItem> Heroes { get; private set; }

        [JsonProperty("equip_heroes")]
        public List<int> EquipHeroes { get; private set; }

        private Dictionary<int, UserHeroItem> _dicHero = new();

        public override Enum_UserData Category => Enum_UserData.Hero;

        public List<UserHeroItem> SortHeroes { get; private set; }

        private int Sort(UserHeroItem x, UserHeroItem y)
        {
            if(x.Spec.tier != y.Spec.tier)
                return x.Spec.tier.CompareTo(y.Spec.tier);
            return x.Id.CompareTo(y.Id);
        }

        public override void CheckAndCreate()
        {
            Heroes ??= new();

            foreach (var spec in SpecDataManager.Inst.Hero.All)
            {
                if (Heroes.Find(x => x.Id == spec.id) == null)
                {
                    Heroes.Add(new UserHeroItem(spec.id));
                }
            }

            if (EquipHeroes == null)
            {
                EquipHeroes = new List<int>() {1, 2, 3, 4, 5};
            }

            foreach (var hero in Heroes)
            {
                _dicHero[hero.Id] = hero;
            }

            SortHeroes = Heroes.ToList();
        }

        public UserHeroItem GetHero(int id)
        {
            return _dicHero.GetValueOrDefault(id);
        }

        public void SetEquipHero(int index, UserHeroItem hero)
        {
            if (hero.Unlock == false)
                return;

            var equipped = EquipHeroes.IndexOf(hero.Id);
            if (equipped == -1)
            {
                EquipHeroes[index] = hero.Id;
            }
            else
            {
                (EquipHeroes[equipped], EquipHeroes[index]) = (EquipHeroes[index], EquipHeroes[equipped]);
            }

            UserManager.Inst.SaveCheck(Enum_UserData.Hero);
            UserManager.Inst.Save().Forget();
            EventManager.Inst.OnEventEquippedHero();
        }

        public void SetUnEquipHero(UserHeroItem hero)
        {
            var index = EquipHeroes.IndexOf(hero.Id);
            if (index == -1)
                return;

            EquipHeroes[index] = 0;
            UserManager.Inst.SaveCheck(Enum_UserData.Hero);
            UserManager.Inst.Save().Forget();
            EventManager.Inst.OnEventEquippedHero();
        }

        public UserHeroItem GetEquippedHero(int index)
        {
            return GetHero(EquipHeroes[index]);
        }

        public bool EnterBattle()
        {
            foreach (var hero in EquipHeroes)
            {
                if (hero == 0)
                    return false;
            }

            return true;
        }

        #region Editor

#if UNITY_EDITOR || __DEV
        public void Editor_AllUnlockHero()
        {
            foreach (var hero in Heroes)
            {
                hero.Editor_UnlockHero();
            }
        }
#endif

        #endregion
    }
}
