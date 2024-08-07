using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;

public class NicknameManager : Singleton<NicknameManager>
{
    private const string NicknameKey = "NickName";

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

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

    public async Task<bool> SetNickname(string nickname)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("User is not signed in.");
            return false;
        }

        if (await IsNicknameAvailable(nickname))
        {
            try
            {
                var data = new Dictionary<string, object> {{NicknameKey, nickname}};
                await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));
                Debug.Log("Nickname set successfully.");
                return true;
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"Failed to save nickname: {e.Message}");
                return false;
            }
        }
        else
        {
            Debug.Log("Nickname is already taken.");
            return false;
        }
    }

    public async Task<string> GetUserNickname()
    {
        try
        {
            var results = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {NicknameKey});
            if (results.TryGetValue(NicknameKey, out var nicknameObject))
            {
                return nicknameObject as string;
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
