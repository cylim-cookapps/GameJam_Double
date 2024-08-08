using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoPunDontDestroySingleton<LobbyManager>
{
    private const int MaxPlayersPerRoom = 2;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public async UniTask Connect()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.JoinLobby();
        else
            PhotonNetwork.ConnectUsingSettings();

        await UniTask.WaitUntil(() => PhotonNetwork.InLobby);
    }

    public async UniTaskVoid QuickMatch()
    {
        await Connect();
        PhotonNetwork.JoinRandomRoom();
    }

    public void CancelMatch()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
    }

    public void CreatePrivateRoom()
    {
        string roomName = "Room_" + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
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
        SetStatus("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SetStatus("Joined Lobby");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        SetStatus("Failed to join random room. Creating a new one...");
        CreatePrivateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetStatus("Failed to create room: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetStatus("Failed to join room: " + message);
    }

    public override void OnJoinedRoom()
    {
        SetStatus("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetStatus("Loading Game Scene...");
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
            SetStatus("Current Room Name: " + PhotonNetwork.CurrentRoom.Name);
        }
        else
        {
            SetStatus("Not in a room");
        }
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);
    }
}
