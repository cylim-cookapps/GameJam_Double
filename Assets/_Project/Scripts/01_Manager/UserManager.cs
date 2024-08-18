using System.Collections;
using System.Collections.Generic;
using AnnulusGames.LucidTools.RandomKit;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Pxp.Data;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Pxp
{
    public class UserManager : Singleton<UserManager>
    {
        internal string PlayerId { get; private set; }
        internal string NickName { get; set; }
        internal bool IsNewUser { get; private set; }

        #region Field

        private UserInfo _info;
        private UserCurrency _currency;
        private UserHero _hero;

        #endregion

        #region UserData

        public UserInfo Info => _info;
        public UserCurrency Currency => _currency;
        public UserHero Hero => _hero;
        private Dictionary<Enum_UserData, UserData> _userDataList = new();
        private Dictionary<Enum_UserData, bool> _userDataSaveCheck = new();

        #endregion

        internal async UniTask OnInitialize()
        {
            await UnityServices.InitializeAsync();
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    PlayerId = AuthenticationService.Instance.PlayerId;
                    Debug.Log($"PlayerId: {AuthenticationService.Instance.PlayerId}");
                }
            }

            Dictionary<string, Item> userData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();

            Load(ref _info, userData, Enum_UserData.Info);
            Load(ref _currency, userData, Enum_UserData.Currency);
            Load(ref _hero, userData, Enum_UserData.Hero);

            NickName = await NicknameManager.Inst.GetUserNickname();
            await Save(true);
        }

        private T Load<T>(ref T data, Dictionary<string, Item> userDatas, Enum_UserData category)
            where T : UserData, new()
        {
            if (userDatas != null && userDatas.TryGetValue(category.ToString(), out var item))
            {
                data = item.Value.GetAs<T>();
                Debug.Log($"{category}/{item.Value.GetAsString()}");
            }
            else
            {
                if (category == Enum_UserData.Info)
                    IsNewUser = true;

                data = new T();
            }

            data.CheckAndCreate();
            _userDataList.Add(data.Category, data);

            return data;
        }

        public void SaveCheck(Enum_UserData category)
        {
            _userDataSaveCheck[category] = true;
        }

        public async UniTask Save(bool isAll = false)
        {
            using PooledObject<Dictionary<string, object>> _ =
                DictionaryPool<string, object>.Get(out var dic);
            foreach (var data in _userDataList)
            {
                if (!isAll && !_userDataSaveCheck[data.Key])
                    continue;

                dic.Add(data.Key.ToString(), data.Value);
                _userDataSaveCheck[data.Key] = false;
            }

            await CloudSaveService.Instance.Data.Player.SaveAsync(dic);
        }

        public void AddItem(IEnumerable<ItemInfo> infos)
        {
            foreach (var info in infos)
            {
                AddItem(info);
            }
        }

        public void AddItem(ItemInfo info)
        {
            if (info.ItemType == Enum_ItemType.None)
                return;

            var currency = Currency.GetCurrency(info.ItemType);
            if (currency != null)
            {
                currency.Increase(info.Count);
                return;
            }

            foreach (var hero in SpecDataManager.Inst.Hero.All)
            {
                if (hero.item_type == info.ItemType)
                {
                    var userHero = Hero.GetHero(hero.id);
                    if (userHero != null)
                    {
                        userHero.AddCard(info.Count);
                        return;
                    }
                }
            }
        }
    }

    public enum Enum_UserData
    {
        Info,
        Currency,
        Hero,
    }
}
