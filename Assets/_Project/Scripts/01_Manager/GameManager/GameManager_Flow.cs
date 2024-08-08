using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    public partial class GameManager
    {
        private float SpawnInterval = 0.3f;
        private int DeathCount = 100;
        private int WaveInterval = 20;
        private List<Wave> _waves = new();

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

        private IEnumerator GameLoop()
        {
            Debug.Log("GameLoop");

            foreach (var wave in SpecDataManager.Inst.Wave.All)
            {
                if (wave.mode == 0)
                    _waves.Add(wave);
            }

            Wave = 0;
            while (true)
            {
                if (_waves.Count <= _wave)
                    break;

                Second = WaveInterval;
                StartCoroutine(GameTimer());
                var data = _waves[Wave].monsterIndexToData;
                for (int i = 0; i < _waves[Wave].monsterCount; i++)
                {
                    SpawnMonster(data);
                    yield return new WaitForSeconds(SpawnInterval);
                }

                yield return new WaitUntil(() => Second == 0);
                Wave++;
            }
        }

        private IEnumerator GameTimer()
        {
            while (Second > 0)
            {
                yield return new WaitForSeconds(1);
                Second--;
            }
        }

        private void SpawnMonster(Monster data)
        {
            object[] instantiationData = new object[]
            {
                data.monsterIndex
            };

            GameObject monster = PhotonNetwork.InstantiateRoomObject(ZString.Format("Enemy/{0}", data.prefab_key), _waypoints[0].position, Quaternion.identity, 0, instantiationData);
            spawnedMonsters.Add(monster);
        }

        public void SyncData(int someData)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ReceiveData", RpcTarget.All, someData);
            }
        }
    }
}
