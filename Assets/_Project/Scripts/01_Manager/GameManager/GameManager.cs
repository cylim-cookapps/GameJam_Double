using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using AnnulusGames.LucidTools.RandomKit;
using Cysharp.Text;
using ExitGames.Client.Photon;
using Pxp.Data;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
        private DragUnit _dragUnit;

        [SerializeField]
        private GameObject _bg;

        [SerializeField]
        private List<Transform> _waypoints0, _waypoints1;

        [SerializeField]
        private List<Board> _batch0, _batch1;

        [SerializeField]
        private float spawnInterval = 5f;

        [SerializeField] private LayerMask heroLayerMask;
        [SerializeField] private LayerMask boardLyerMask;
        private List<GameObject> spawnedEnemy = new List<GameObject>();
        private Dictionary<int, List<HeroUnit>> spawnedHeroes = new Dictionary<int, List<HeroUnit>>();
        public Enum_GameState CurrGameState = Enum_GameState.Ready;

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
        private HeroUnit _selectedHero;

        public InGameUserData GetPlayerData(int actorNumber)
        {
            return playerDataDict[actorNumber];
        }

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
                Batch0.Add(batch.transform.position);
            }

            foreach (var batch in _batch1)
            {
                Batch1.Add(batch.transform.position);
            }

            spawnedHeroes.Add(1, new List<HeroUnit>());
            spawnedHeroes.Add(2, new List<HeroUnit>());

            for (int i = 0; i < 15; i++)
            {
                spawnedHeroes[1].Add(null);
                spawnedHeroes[2].Add(null);
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

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼 클릭
            {
                _selectedHero = GetClickedHeroUnit();
                if (_selectedHero != null)
                {
                    _dragUnit.SetUnit(_selectedHero, GetMouseWorldPosition());
                }
            }
            else if (Input.GetMouseButtonUp(0)) // 왼쪽 마우스 버튼 릴리즈
            {
                if (_selectedHero != null)
                {
                    var board = GetClickedUpBoard();
                    if (board != null)
                    {
                        int fromIndex = _selectedHero.BoardIndex;
                        int toIndex = board.BoardIndex;

                        if (fromIndex != toIndex)
                        {
                            var pos = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? Batch0[toIndex] : Batch1[toIndex];

                            // RPC 호출로 이동 동기화 (스왑 포함)
                            photonView.RPC(nameof(MoveHeroRpc), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, fromIndex, toIndex, pos);
                        }
                    }

                    _selectedHero = null;
                    _dragUnit.Dispose();
                }
            }
            else if (Input.GetMouseButton(0)) // 왼쪽 마우스 버튼 누름
            {
                if (_selectedHero != null)
                {
                    _dragUnit.transform.position = GetMouseWorldPosition();
                }
            }
        }

        [PunRPC]
        private void MoveHeroRpc(int actorNumber, int fromIndex, int toIndex, Vector2 newPosition)
        {
            if (spawnedHeroes.TryGetValue(actorNumber, out var heroList))
            {
                var movingHero = heroList[fromIndex];
                var existingHero = heroList[toIndex];

                if (movingHero != null)
                {
                    if (existingHero != null &&
                        existingHero.HeroId == movingHero.HeroId &&
                        existingHero.Grade == movingHero.Grade)
                    {
                        // 합성 로직
                        existingHero.photonView.RPC("UpgradeHero", RpcTarget.All);

                        // 이동하던 영웅 제거
                        PhotonNetwork.Destroy(movingHero.gameObject);
                        heroList[fromIndex] = null;

                        // 플레이어 데이터 업데이트
                        var playerData = playerDataDict[actorNumber];
                        playerData.Units[fromIndex].HeroId = 0;
                        playerData.Units[fromIndex].Grade = 0;
                        playerData.Units[toIndex].Grade++;

                        // 이벤트 발생 (UI 업데이트 등을 위해)
                       // EventManager.Inst.OnEventHeroMerged(actorNumber, toIndex, existingHero.Grade);
                    }
                    else
                    {
                        // 기존의 스왑 로직
                        heroList[fromIndex] = existingHero;
                        heroList[toIndex] = movingHero;
                        movingHero.MoveHero(toIndex, newPosition);

                        if (existingHero != null)
                        {
                            var oldPosition = actorNumber == 1 ? Batch0[fromIndex] : Batch1[fromIndex];
                            existingHero.MoveHero(fromIndex, oldPosition);
                        }

                        // 플레이어 데이터 업데이트
                        var playerData = playerDataDict[actorNumber];
                        var temp = playerData.Units[fromIndex];
                        playerData.Units[fromIndex] = playerData.Units[toIndex];
                        playerData.Units[toIndex] = temp;
                    }

                    // UI 업데이트 등 필요한 추가 작업
                  //  EventManager.Inst.OnEventHeroMoved(actorNumber, fromIndex, toIndex);
                }
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = -Camera.main.transform.position.z; // 카메라와 같은 z 위치 사용
            return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }

        private HeroUnit GetClickedHeroUnit()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, heroLayerMask);

            if (hit.collider != null)
            {
                var unit = hit.collider.GetComponent<HeroUnit>();
                if (unit.Owner == PhotonNetwork.LocalPlayer.ActorNumber)
                    return unit;
            }

            return null;
        }

        private Board GetClickedUpBoard()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, boardLyerMask);

            if (hit.collider != null)
            {
                var board = hit.collider.GetComponent<Board>();
                if (board.Actor == PhotonNetwork.LocalPlayer.ActorNumber)
                    return board;
            }

            return null;
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
                    yield return new WaitForSeconds(SpawnInterval * 0.5f);
                    SpawnMonster(data, WayPoint1);
                    yield return new WaitForSeconds(SpawnInterval * 0.5f);
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

        public void UpgradeHero(InGameHeroData data)
        {
            if (CurrGameState == Enum_GameState.Start)
            {
                int needCoin = LevelUp_Default + data.Upgrade * LevelUp_Increase;
                if (MyInGameUserData.Coin >= needCoin)
                {
                    data.Upgrade++;
                    MyInGameUserData.Coin -= needCoin;
                    EventManager.Inst.OnEventGameCoin(MyInGameUserData.Coin);
                    EventManager.Inst.OnEventGameHeroUpgrade(PhotonNetwork.LocalPlayer.ActorNumber, data.HeroId);
                    SendHeroLevelUp(data.HeroId, data.Upgrade, MyInGameUserData.Coin);
                }
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
                0,
            };

            var pos = actorNumber == 1 ? Batch0[batchIndex] : Batch1[batchIndex];

            GameObject obj = PhotonNetwork.InstantiateRoomObject(
                ZString.Format("Hero/{0}", heroSpec.prefab_key),
                pos,
                Quaternion.identity,
                0,
                instantiationData
            );

            var herUnit = obj.GetComponent<HeroUnit>();

            spawnedHeroes[actorNumber][batchIndex] = herUnit;
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
