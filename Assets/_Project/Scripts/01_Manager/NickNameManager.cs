using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pxp;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using Debug = UnityEngine.Debug;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;

public class NicknameManager : Singleton<NicknameManager>
{
    private const string NicknameKey = "NickName";

    public async Task<bool> IsNicknameAvailable(string nickname)
    {
        try
        {
            var query = new Query(
                new List<FieldFilter>
                {
                    new FieldFilter("NickName", nickname, FieldFilter.OpOptions.EQ, true),
                }
            );

            var queryTask = await CloudSaveService.Instance.Data.Player.QueryAsync(
                query,
                new QueryOptions()
            );

            return queryTask.Count == 0;
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Failed to query nickname: {e.Message}");
            return false;
        }
    }

    public async Task<Enum_NickNameValid> SetNickname(string nickname)
    {
        var state = LanguageManager.Inst.IsValidNickName(nickname);

        if (state != Enum_NickNameValid.Valid)
        {
            return state;
        }

        if (await IsNicknameAvailable(nickname))
        {
            try
            {
                var data = new Dictionary<string, object> {{NicknameKey, nickname}};
                await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));
                UserManager.Inst.NickName = nickname;
                EventManager.Inst.OnEventNickname(nickname);
                return Enum_NickNameValid.Valid;
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"Failed to save nickname: {e.Message}");
                return Enum_NickNameValid.Duplication;
            }
        }
        else
        {
            return Enum_NickNameValid.Duplication;
        }
    }

    public async UniTask<string> GetUserNickname()
    {
        try
        {
            var results = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {NicknameKey}, new LoadOptions(new PublicReadAccessClassOptions()));
            if (results.TryGetValue(NicknameKey, out var nicknameObject))
            {
                return nicknameObject.Value.GetAsString();
            }

            return null;
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Failed to load nickname: {e.Message}");
            return null;
        }
    }
}
