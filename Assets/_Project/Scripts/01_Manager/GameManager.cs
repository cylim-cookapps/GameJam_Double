using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public static GameManager Instance;

    // 플레이어 세션 정보를 저장할 딕셔너리
    private Dictionary<ulong, PlayerSession> playerSessions = new Dictionary<ulong, PlayerSession>();
    private const float SESSION_TIMEOUT = 300f; // 5분

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (playerSessions.TryGetValue(clientId, out PlayerSession session))
        {
            // 기존 세션 복원
            Debug.Log($"Restoring session for client {clientId}");
            RestorePlayerSession(clientId, session);
        }
        else
        {
            // 새 세션 생성
            Debug.Log($"Creating new session for client {clientId}");
            CreateNewPlayerSession(clientId);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (playerSessions.TryGetValue(clientId, out PlayerSession session))
        {
            Debug.Log($"Client {clientId} disconnected. Starting timeout coroutine.");
            StartCoroutine(SessionTimeoutCoroutine(clientId));
        }
    }

    private void CreateNewPlayerSession(ulong clientId)
    {
        PlayerSession newSession = new PlayerSession
        {
            ClientId = clientId,
            // 여기에 필요한 추가 플레이어 정보 설정
        };
        playerSessions[clientId] = newSession;

        // 새 플레이어 생성 로직
        SpawnPlayerForClient(clientId);
    }

    private void RestorePlayerSession(ulong clientId, PlayerSession session)
    {
        // 기존 세션 정보를 사용하여 플레이어 상태 복원
        SpawnPlayerForClient(clientId, session.Position);
        // 추가적인 상태 복원 로직
    }

    private void SpawnPlayerForClient(ulong clientId, Vector3? position = null)
    {
        // 플레이어 스폰 로직
        GameObject playerPrefab = PlayerPrefab;
        Vector3 spawnPos = position ?? Vector3.zero;
        NetworkObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity).GetComponent<NetworkObject>();
        playerInstance.Spawn();
        SpawnEnemy();
    }

    private void Update()
    {
        if (IsServer)
        {

        }
    }

    private void SpawnEnemy()
    {
        GameObject playerPrefab = EnemyPrefab;
        Vector3 spawnPos = Vector3.zero;
        NetworkObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity).GetComponent<NetworkObject>();
        playerInstance.Spawn(true);
    }

    private IEnumerator SessionTimeoutCoroutine(ulong clientId)
    {
        yield return new WaitForSeconds(SESSION_TIMEOUT);

        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            Debug.Log($"Session for client {clientId} has timed out. Removing session.");
            playerSessions.Remove(clientId);
        }
    }
}

// 플레이어 세션 정보를 저장할 클래스
public class PlayerSession
{
    public ulong ClientId;
    public Vector3 Position;
    // 추가적인 플레이어 상태 정보
}
