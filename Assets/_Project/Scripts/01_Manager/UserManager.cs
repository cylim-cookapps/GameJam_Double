using System.Collections;
using System.Collections.Generic;
using AnnulusGames.LucidTools.RandomKit;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
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
        internal string NickName { get; private set; }
        internal bool IsNewUser { get; private set; }

        #region Field

        private UserInfo _info;
        private UserCurrency _currency;

        #endregion

        #region UserData

        public UserInfo Info => _info;
        public UserCurrency Currency => _currency;
        private Dictionary<Enum_UserData, IUserData> _userDataList = new();

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

            if (IsNewUser)
            {
                NickName = $"Test {LucidRandom.Range(1000, 10000)}";
                await NicknameManager.Inst.SetNickname(NickName);
            }
            else
            {
                var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>() {"NickName"}, new LoadOptions(new PublicReadAccessClassOptions()));
                if (data.TryGetValue("NickName", out var item))
                    NickName = item.Value.GetAsString();
            }

            await Save();
        }

        private T Load<T>(ref T data, Dictionary<string, Item> userDatas, Enum_UserData category)
            where T : class, IUserData, new()
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

        private async UniTask Save()
        {
            using PooledObject<Dictionary<string, object>> _ =
                DictionaryPool<string, object>.Get(out var dic);
            foreach (var data in _userDataList)
            {
                dic.Add(data.Key.ToString(), data.Value);
            }

            await CloudSaveService.Instance.Data.Player.SaveAsync(dic);
        }
    }

    public enum Enum_UserData
    {
        Info,
        Currency,
    }
}
