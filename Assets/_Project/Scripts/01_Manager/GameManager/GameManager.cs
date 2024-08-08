using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Pxp.Data;

namespace Pxp
{
    public partial class GameManager : MonoPunSingleton<GameManager>
    {
        [SerializeField]
        private Transform _trBG;

        [SerializeField]
        private List<Transform> _waypoints = new();

        [SerializeField]
        private float spawnInterval = 5f;

        private List<GameObject> spawnedMonsters = new List<GameObject>();
        private bool _isGameStarted = false;

        private Dictionary<int, InGameUserData> playerDataDict = new();

        public List<Transform> Waypoints => _waypoints;

        private void Awake()
        {
            SpawnInterval = SpecDataManager.Inst.Option.Get("SpawnInterval")!.value;
            DeathCount = (int) SpecDataManager.Inst.Option.Get("DeathCount")!.value;
            WaveInterval = (int) SpecDataManager.Inst.Option.Get("WaveInterval")!.value;
        }

        private void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber ==2)
            {
                _trBG.rotation = Quaternion.Euler(0, 0, 180);
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
            SendPlayerLoadedLevel();
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        private void SendPlayerLoadedLevel()
        {
            PhotonNetwork.IsMessageQueueRunning = true;

            PhotonNetwork.RaiseEvent(PLAYER_LOADED_LEVEL, true, new RaiseEventOptions {Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.IsMasterClient)
            {
            }
        }
    }
}
