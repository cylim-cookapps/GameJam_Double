using System.Collections;
using System.Collections.Generic;
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
        private int wave = 0;

        private IEnumerator GameLoop()
        {
            foreach (var wave in SpecDataManager.Inst.Wave.All)
            {
                if (wave.mode == 0)
                    _waves.Add(wave);
            }

            while (true)
            {
                if (_waves.Count <= wave)
                    break;

                var data = _waves[wave].monsterIndexToData;

                for (int i = 0; i < _waves[wave].monsterCount; i++)
                {
                    SpawnMonster(data);
                    yield return new WaitForSeconds(1);
                }
            }
        }

        private void SpawnMonster(Monster data)
        {
            object[] instantiationData = new object[]
            {
                data.monsterIndex
            };

            GameObject monster = PhotonNetwork.InstantiateRoomObject(data.prefab_key, _waypoints[0].position, Quaternion.identity, 0, instantiationData);
            spawnedMonsters.Add(monster);
        }
    }
}
