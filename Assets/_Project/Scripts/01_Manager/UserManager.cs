using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Pxp
{
    public class UserManager : Singleton<UserManager>
    {
        internal string PlayerId { get; private set; }

        internal async UniTask Initalize()
        {
            await UnityServices.InitializeAsync();
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    PlayerId = AuthenticationService.Instance.PlayerId;
                }
            }

            using PooledObject<HashSet<string>> _ =
                GenericPool<HashSet<string>>.Get(out HashSet<string> hashSet);

            hashSet.Clear();

            Dictionary<string, string> savedData =
                await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"key"});

            Debug.Log("Done: " + savedData["key"]);
        }

        // 부분적으로 저장
        public async UniTask SavePartialData(string key, string value)
        {
            var data = new Dictionary<string, object> {{key, value}};
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }

        // 부분적으로 로드
        public async UniTask<string> LoadPartialData(string key)
        {
            var results = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {key});
            return results.TryGetValue(key, out var value) ? value : null;
        }

        // 여러 키-값 쌍을 한 번에 저장
        public async UniTask SaveMultipleData(Dictionary<string, object> data)
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }

        // 여러 키에 대한 데이터를 한 번에 로드
        public async UniTask<Dictionary<string, string>> LoadMultipleData(HashSet<string> keys)
        {
            return await CloudSaveService.Instance.Data.LoadAsync(keys);
        }

        // // 모든 데이터 저장 (기존 데이터를 덮어씁니다)
        // public async UniTask SaveAllData(Dictionary<string, object> allData)
        // {
        //     await CloudSaveService.Instance.Data.ForceUpdateAsync(allData);
        // }

        // 모든 데이터 로드
        public async UniTask<Dictionary<string, string>> LoadAllData()
        {
            return await CloudSaveService.Instance.Data.LoadAllAsync();
        }

        // 특정 키의 데이터 삭제
        public async UniTask DeleteData(string key)
        {
            await CloudSaveService.Instance.Data.ForceDeleteAsync(key);
        }

        // 모든 데이터 삭제
        public async UniTask DeleteAllData()
        {
            var allData = await LoadAllData();
            foreach (var key in allData.Keys)
            {
                await DeleteData(key);
            }
        }
    }
}
