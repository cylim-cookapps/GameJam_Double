using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.Obfuscator;
using Newtonsoft.Json;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class UserCurrency : UserData
    {
        [JsonProperty("currencies")]
        public List<UserCurrencyItem> Currencies { get; private set; }

        private Dictionary<Enum_ItemType, UserCurrencyItem> _dicCurrency;

        public override Enum_UserData Category => Enum_UserData.Currency;

        public override void CheckAndCreate()
        {
            Currencies ??= new();

            foreach (var spec in SpecDataManager.Inst.Currency.All)
            {
                if (Currencies.Find(x => x.Id == spec.id) == null)
                {
                    Currencies.Add(new UserCurrencyItem(spec.id));
                }
            }

            _dicCurrency ??= new();
            foreach (var currency in Currencies)
            {
                _dicCurrency[currency.Type] = currency;
            }
        }

        public UserCurrencyItem GetCurrency(Enum_ItemType type)
        {
            return _dicCurrency.GetValueOrDefault(type);
        }

        public void AddCurrency(Enum_ItemType type, int count)
        {
            var currency = GetCurrency(type);
            currency.Increase(count);
        }

        public void SubCurrency(Enum_ItemType type, int count)
        {
            var currency = GetCurrency(type);
            currency.Decrease(count);
        }
    }
}
