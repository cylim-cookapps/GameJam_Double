using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Pxp;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class LobbyManager : MonoPunDontDestroySingleton<LobbyManager>
{
    public int MaxPlayersPerRoom => _isSingleMode ? 1 : 2;
    private bool _isSingleMode = false;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public async UniTask Connect()
    {
        PhotonNetwork.SendRate = 30; // 초당 30회 상태 업데이트 전송
        PhotonNetwork.SerializationRate = 30; // 초당 30회 OnPhotonSerializeView 호출

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.JoinLobby();
        else
            PhotonNetwork.ConnectUsingSettings();

        await UniTask.WaitUntil(() => PhotonNetwork.InLobby);
    }

    public async UniTaskVoid QuickMatch(bool isSingleMode)
    {
        MainUI.Inst.SetIndicator(true);
        _isSingleMode = isSingleMode;
        await Connect();
        PhotonNetwork.JoinRandomRoom();
    }

    public void CancelMatch()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
    }

    public async UniTaskVoid EndGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        SceneManager.LoadScene("Lobby");
    }

    public void CreatePrivateRoom()
    {
        string roomName = "Room_" + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayersPerRoom,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom(string value)
    {
        PhotonNetwork.JoinRoom(value);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room. Creating a new one...");
        CreatePrivateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room: " + message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        MainUI.Inst.SetIndicator(false);
        PopupManager.Inst.GetPopup<Popup_Wait>().Show();

        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            EventManager.Inst.OnEventMatch();
            PhotonNetwork.LoadLevel("Game");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            LoadGameScene();
        }
    }

    public void GetCurrentRoomName()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Current Room Name: " + PhotonNetwork.CurrentRoom.Name);
        }
        else
        {
            Debug.Log("Not in a room");
        }
    }
}
