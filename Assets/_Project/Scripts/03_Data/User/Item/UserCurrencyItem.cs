using System;
using System.Runtime.Serialization;
using CookApps.Obfuscator;
using Newtonsoft.Json;
using Pxp.Data;
using Sigtrap.Relays;

namespace Pxp
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class UserCurrencyItem : IData<Currency>
    {
        [JsonProperty("id")]
        public ObfuscatorInt Id { get; private set; }

        [JsonProperty("count")]
        public ObfuscatorDouble Count { get; private set; }

        public Currency Spec { get; private set; }
        public Enum_ItemType Type => (Enum_ItemType) Id.Value;

        public Relay<double> EventUpdate { get; } = new();

        public UserCurrencyItem(int id)
        {
            Id = id;
            UpdateSpec();
            if (Spec != null)
            {
                Count = Spec.init_count;
            }
        }

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            UpdateSpec();
        }

        private void UpdateSpec()
        {
            Spec = SpecDataManager.Inst.Currency.Get(Id);
        }

        public void Increase(double value)
        {
            Count += value;
            EventUpdate.Dispatch(Count.Value);
            UserManager.Inst.SaveCheck(Enum_UserData.Currency);
        }

        public void Decrease(double value)
        {
            Count -= value;
            EventUpdate.Dispatch(Count.Value);
            UserManager.Inst.SaveCheck(Enum_UserData.Currency);
        }
    }
}
