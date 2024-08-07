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
    public class UserCurrency : IUserData
    {
        [JsonProperty("currencies")]
        public List<UserCurrencyItem> Currencies { get; private set; }

        private Dictionary<Enum_ItemType, UserCurrencyItem> _dicCurrency;

        public Enum_UserData Category => Enum_UserData.Currency;

        public void CheckAndCreate()
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

        public void NextDay(bool isNextWeek)
        {
        }

        public UserCurrencyItem GetCurrency(Enum_ItemType type)
        {
            return _dicCurrency.GetValueOrDefault(type);
        }
    }
}
