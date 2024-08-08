using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Pxp;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public partial class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameObject monsterPrefab;

    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private float spawnInterval = 5f;

    private float nextSpawnTime;
    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private bool _isGameStarted = false;
    private const byte PLAYER_LOADED_LEVEL = 0;
    private const byte PLAYER_DATA_EVENT = 1;

    private Dictionary<string, InGameUserData> playerDataDict = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!PhotonNetwork.IsMessageQueueRunning)
        {
            PhotonNetwork.IsMessageQueueRunning = true;
        }

        if (PhotonNetwork.InRoom)
        {
            SendPlayerLoadedLevel();
        }
    }

    private void SendPlayerLoadedLevel()
    {
        if (!PhotonNetwork.IsMessageQueueRunning)
        {
            return;
        }

        PhotonNetwork.RaiseEvent(PLAYER_LOADED_LEVEL, true, new RaiseEventOptions {Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
    }

    private void OnEvent(EventData photonEvent)
    {
        Debug.Log("EventData" + photonEvent.Code);
        if (photonEvent.Code == PLAYER_LOADED_LEVEL)
        {
            int playersInGame = PhotonNetwork.PlayerList.Length;
            int playersLoaded = 0;
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                if (player.Value.CustomProperties.ContainsKey("PlayerLoadedLevel"))
                    playersLoaded++;
            }

            if (playersLoaded == playersInGame)
            {
                StartGame();
            }
        }
        else if (photonEvent.Code == PLAYER_DATA_EVENT)
        {
            Hashtable playerDataHash = (Hashtable) photonEvent.CustomData;
            InGameUserData userData = InGameUserData.FromHashtable(playerDataHash);
            playerDataDict[userData.Name] = userData;
            Debug.Log(userData);
            if (playerDataDict.Count == 2)
            {
                EventManager.Inst.OnEventEquippedHero();
            }
        }
    }

    private void Update()
    {
        if (_isGameStarted && PhotonNetwork.IsMasterClient && Time.time >= nextSpawnTime)
        {
            SpawnMonster();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void StartGame()
    {
        _isGameStarted = true;
        nextSpawnTime = Time.time + spawnInterval;
        SendPlayerData();
    }

    private void SendPlayerData()
    {
        List<InGameHeroData> myHeroes = new List<InGameHeroData>();

        foreach (var heroId in UserManager.Inst.Hero.EquipHeroes)
        {
            var heroData = UserManager.Inst.Hero.GetEquippedHero(heroId);
            myHeroes.Add(new InGameHeroData(heroData.Id, heroData.Level, heroData.Star));
        }

        InGameUserData myData = new InGameUserData(
            UserManager.Inst.PlayerId,
            UserManager.Inst.Info.Level,
            myHeroes
        );

        Hashtable playerDataHash = myData.ToHashtable();
        playerDataHash.Add("ActorNumber", PhotonNetwork.LocalPlayer.ActorNumber);

        PhotonNetwork.RaiseEvent(PLAYER_DATA_EVENT, playerDataHash, new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient}, SendOptions.SendReliable);
    }

    [PunRPC]
    private void NotifyGameStarted()
    {
        Debug.Log("Game has started!");
        // 여기에 게임 시작 시 필요한 추가 로직을 구현할 수 있습니다.
    }

    private void SpawnMonster()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomSpawnIndex].position;

        GameObject monster = PhotonNetwork.Instantiate(monsterPrefab.name, spawnPosition, Quaternion.identity);
        spawnedMonsters.Add(monster);

        photonView.RPC("NotifyMonsterSpawned", RpcTarget.All, monster.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    private void NotifyMonsterSpawned(int viewId)
    {
        Debug.Log($"Monster spawned with ViewID: {viewId}");
        // 여기에 몬스터 생성 시 필요한 추가 로직을 구현할 수 있습니다.
    }

    public void DestroyMonster(GameObject monster)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnedMonsters.Remove(monster);
            PhotonNetwork.Destroy(monster);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
}
