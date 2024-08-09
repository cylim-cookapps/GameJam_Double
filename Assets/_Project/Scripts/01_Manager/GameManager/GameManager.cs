using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using AnnulusGames.LucidTools.RandomKit;
using Cysharp.Text;
using ExitGames.Client.Photon;
using Pxp.Data;

namespace Pxp
{
    public partial class GameManager : MonoPunSingleton<GameManager>
    {
        #region RpcParamters

        private int _wave = 0;

        public int Wave
        {
            get => _wave;
            set
            {
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetWaveRpc), RpcTarget.All, value);
            }
        }

        [PunRPC]
        private void SetWaveRpc(int wave)
        {
            _wave = wave;
            EventManager.Inst.OnEventWave(wave);
        }

        private int _second = 0;

        public int Second
        {
            get => _second;
            set
            {
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetSecondRpc), RpcTarget.All, value);
            }
        }

        [PunRPC]
        private void SetSecondRpc(int sec)
        {
            _second = sec;
            EventManager.Inst.OnEventGameTimer(_second);
        }

        private int _monsterCount = 0;

        public int MonsterCount
        {
            get => _monsterCount;
            set
            {
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetMonsterCountRpc), RpcTarget.All, value);
            }
        }

        [PunRPC]
        private void SetMonsterCountRpc(int count)
        {
            _monsterCount = count;
            EventManager.Inst.OnEventMonsterCount(_monsterCount);
        }

        #endregion

        [SerializeField]
        private GameObject _bg;

        [SerializeField]
        private List<Transform> _waypoints0, _waypoints1;

        [SerializeField]
        private List<Transform> _batch0, _batch1;

        [SerializeField]
        private float spawnInterval = 5f;

        private List<GameObject> spawnedEnemy = new List<GameObject>();
        private List<GameObject> spawnedHeroes = new List<GameObject>();
        private bool _isGameStarted = false;

        private Dictionary<int, InGameUserData> playerDataDict = new();

        private float SpawnInterval = 0.3f;
        private int DeathCount = 100;
        private int WaveInterval = 20;
        private int Summon_Default = 50;
        private int Summon_Increase = 10;
        private int LevelUp_Default = 50;
        private int LevelUp_Increase = 100;

        private List<Wave> _waves = new();

        private List<Vector2> WayPoint0 = new List<Vector2>();
        private List<Vector2> WayPoint1 = new List<Vector2>();

        private List<Vector2> Batch0 = new List<Vector2>();
        private List<Vector2> Batch1 = new List<Vector2>();

        public InGameUserData MyInGameUserData => playerDataDict[PhotonNetwork.LocalPlayer.ActorNumber];

        private void Awake()
        {
            SpawnInterval = SpecDataManager.Inst.Option.Get("SpawnInterval")!.value;
            DeathCount = (int) SpecDataManager.Inst.Option.Get("DeathCount")!.value;
            WaveInterval = (int) SpecDataManager.Inst.Option.Get("WaveInterval")!.value;
            Summon_Default = (int) SpecDataManager.Inst.Option.Get("Summon_Default")!.value;
            Summon_Increase = (int) SpecDataManager.Inst.Option.Get("Summon_Increase")!.value;
            LevelUp_Default = (int) SpecDataManager.Inst.Option.Get("LevelUp_Default")!.value;
            LevelUp_Increase = (int) SpecDataManager.Inst.Option.Get("LevelUp_Increase")!.value;

            foreach (var waypoint in _waypoints0)
            {
                WayPoint0.Add(waypoint.position);
            }

            foreach (var waypoint in _waypoints1)
            {
                WayPoint1.Add(waypoint.position);
            }

            foreach (var batch in _batch0)
            {
                Batch0.Add(batch.position);
            }

            foreach (var batch in _batch1)
            {
                Batch1.Add(batch.position);
            }
        }

        private void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                _bg.transform.localScale = new Vector3(1, -1, 1);
                Camera.main.transform.SetPositionAndRotation(new Vector3(0, 0, 10), Quaternion.Euler(180, 0, 0));
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
                StartCoroutine(GameLoop());
            }
        }

        private IEnumerator GameLoop()
        {
            foreach (var wave in SpecDataManager.Inst.Wave.All)
            {
                if (wave.mode == 0)
                    _waves.Add(wave);
            }

            while (true)
            {
                if (_waves.Count <= _wave)
                    break;

                Second = WaveInterval;
                StartCoroutine(GameTimer());
                var data = _waves[Wave].monsterIndexToData;
                for (int i = 0; i < _waves[Wave].monsterCount; i++)
                {
                    SpawnMonster(data, WayPoint0);
                    SpawnMonster(data, WayPoint1);
                    yield return new WaitForSeconds(SpawnInterval);
                }

                yield return new WaitUntil(() => Second == 0);
                Wave++;
            }
        }

        private void GameEnd()
        {
            LobbyManager.Inst.EndGame().Forget();
        }

        private IEnumerator GameTimer()
        {
            while (Second > 0)
            {
                yield return new WaitForSeconds(1);
                Second--;
            }
        }

        public void SpawnHero()
        {
            var needCoin = Summon_Default + (Summon_Increase * MyInGameUserData.Summon);

            if (MyInGameUserData.Coin < needCoin)
                return;

            var playerData = playerDataDict[PhotonNetwork.LocalPlayer.ActorNumber];

            var indexList = new List<int>();
            for (int i = 0; i < playerData.Units.Count; i++)
            {
                if (playerData.Units[i].HeroId == 0)
                {
                    indexList.Add(i);
                }
            }

            if (indexList.Count == 0)
                return;

            var batchIndex = indexList[Random.Range(0, indexList.Count)];
            var heroData = playerData.Heroes[Random.Range(0, playerData.Heroes.Count)];

            if (PhotonNetwork.IsMasterClient)
            {
                SpawnHeroInternal(heroData.HeroId, batchIndex, PhotonNetwork.LocalPlayer.ActorNumber, needCoin);
            }
            else
            {
                // MasterClient에게 영웅 소환 요청
                photonView.RPC(nameof(RequestSpawnHero), RpcTarget.MasterClient, heroData.HeroId, batchIndex, PhotonNetwork.LocalPlayer.ActorNumber, needCoin);
            }
        }

        [PunRPC]
        private void RequestSpawnHero(int heroId, int batchIndex, int actorNumber, int needCoin)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnHeroInternal(heroId, batchIndex, actorNumber, needCoin);
            }
        }

        private void SpawnHeroInternal(int heroId, int batchIndex, int actorNumber, int needCoin)
        {
            var heroSpec = SpecDataManager.Inst.Hero.Get(heroId);

            object[] instantiationData = new object[]
            {
                heroId,
                batchIndex,
                actorNumber,
            };

            var pos = actorNumber == 1 ? Batch0[batchIndex] : Batch1[batchIndex];

            GameObject heroUnit = PhotonNetwork.InstantiateRoomObject(
                ZString.Format("Hero/{0}", heroSpec.prefab_key),
                pos,
                Quaternion.identity,
                0,
                instantiationData
            );

            spawnedHeroes.Add(heroUnit);
            AudioController.Play("SFX_Spawn_Hero");
            // 생성 후 모든 클라이언트에 동기화
            photonView.RPC(nameof(SyncHeroSpawn), RpcTarget.All, actorNumber, batchIndex, heroId, needCoin);
        }

        [PunRPC]
        private void SyncHeroSpawn(int actorNumber, int batchIndex, int heroId, int needCoin)
        {
            playerDataDict[actorNumber].Summon++;
            playerDataDict[actorNumber].Coin -= needCoin;
            playerDataDict[actorNumber].Units[batchIndex].HeroId = heroId;
            playerDataDict[actorNumber].Units[batchIndex].Grade = 0;

            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                EventManager.Inst.OnEventGameCoin(MyInGameUserData.Coin);
            }
        }

        public void UpgradeHero(int heroId)
        {
        }

        private void SpawnMonster(Monster data, List<Vector2> waypoint)
        {
            object[] instantiationData = new object[]
            {
                data.monsterIndex,
                data.coinAmount,
                data.chipAmount,
                waypoint.ToArray(), // List<Vector2>를 Vector2[]로 변환
            };

            GameObject monster = PhotonNetwork.InstantiateRoomObject(
                ZString.Format("Enemy/{0}", data.prefab_key),
                waypoint[0],
                Quaternion.identity,
                0,
                instantiationData
            );
            spawnedEnemy.Add(monster);
            MonsterCount = spawnedEnemy.Count;
            if (MonsterCount >= DeathCount)
            {
                SendGameEnd();
            }
        }

        public void DeadEnemy(EnemyUnit enemyUnit)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (enemyUnit.Coin > 0)
                    photonView.RPC(nameof(AddCoin), RpcTarget.All, enemyUnit.Coin);

                if (enemyUnit.Chip > 0)
                    photonView.RPC(nameof(AddChip), RpcTarget.All, enemyUnit.Chip);

                DestroyedEnemy(enemyUnit.gameObject);
            }
        }

        public void DestroyedEnemy(GameObject obj)
        {
            spawnedEnemy.Remove(obj);
            PhotonNetwork.Destroy(obj);
            MonsterCount = spawnedEnemy.Count;
        }

        [PunRPC]
        public void AddCoin(int coin)
        {
            foreach (var data in playerDataDict)
            {
                data.Value.Coin += coin;
            }

            EventManager.Inst.OnEventGameCoin(MyInGameUserData.Coin);
        }

        [PunRPC]
        public void AddChip(int chip)
        {
            foreach (var data in playerDataDict)
            {
                data.Value.Chip += chip;
            }

            EventManager.Inst.OnEventGameChip(MyInGameUserData.Chip);
        }
    }
}
