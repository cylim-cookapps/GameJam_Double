using System;
using System.Runtime.Serialization;
using CookApps.Obfuscator;
using Newtonsoft.Json;
using Pxp.Data;

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
    }
}
