using System;
using System.Runtime.Serialization;
using CookApps.Obfuscator;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Pxp.Data;
using Sigtrap.Relays;

namespace Pxp
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class UserHeroItem : IData<Hero>
    {
        [JsonProperty("id")]
        public ObfuscatorInt Id { get; private set; }

        [JsonProperty("count")]
        public ObfuscatorInt Count { get; private set; }

        [JsonProperty("level")]
        public ObfuscatorInt Level { get; private set; }

        [JsonProperty("star")]
        public ObfuscatorInt Star { get; private set; }

        [JsonProperty("unlock")]
        public bool Unlock { get; private set; }

        public int Tier => (int) Spec.tier;

        public string TierName
        {
            get
            {
                switch (Spec.tier)
                {
                    default:
                    case Enum_TierType.Normal:
                        return "노멀";
                    case Enum_TierType.Rare:
                        return "레어";
                    case Enum_TierType.Epic:
                        return "에픽";
                    case Enum_TierType.Legend:
                        return "레전드";
                }
            }
        }

        public double Atk => Spec.attack + (Spec.goldLevelup * (Level - 1));
        public double AtkSpeed => Spec.attackSpeed;
        public double AtkRange => Spec.attackRange;

        public Hero Spec { get; private set; }
        public Relay EventUpdate { get; } = new();

        public int LevelUpGold => Spec.goldLevelup_DefaultCost + (Spec.goldLevelup_IncreaseCost * (Level - 1));

        public UserHeroItem(int id)
        {
            Id = id;
            UpdateSpec();
            Level = 1;
            Star = 0;
            Count = 0;
            Unlock = Spec.hero_unlock;
        }

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            UpdateSpec();
        }

        private void UpdateSpec()
        {
            Spec = SpecDataManager.Inst.Hero.Get(Id);
        }

        public bool CheckLevelUp()
        {
            return LevelUpGold <= UserManager.Inst.Currency.GetCurrency(Enum_ItemType.Gold).Count;
        }

        public void LevelUp()
        {
            if (CheckLevelUp() == false)
                return;

            UserManager.Inst.Currency.SubCurrency(Enum_ItemType.Gold, LevelUpGold);
            Level++;

            UserManager.Inst.SaveCheck(Enum_UserData.Hero);
            EventManager.Inst.OnEventHeroLevelUp(Id);
            EventManager.Inst.OnEventToast($"{Spec.hero_name} 레벨업!!");
            UserManager.Inst.Save().Forget();
        }

        public bool StarUp()
        {
            UserManager.Inst.SaveCheck(Enum_UserData.Hero);
            return false;
        }

        #region Editor

#if UNITY_EDITOR || __DEV
        public void Editor_UnlockHero()
        {
            Unlock = true;
        }
#endif

        #endregion
    }
}
