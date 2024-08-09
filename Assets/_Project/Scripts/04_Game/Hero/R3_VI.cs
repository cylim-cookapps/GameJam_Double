using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Photon.Pun;
using UnityEngine;

namespace Pxp
{
    public class R3_VI : HeroUnit
    {
        [SerializeField]
        private GameObject _projectilePrefab2;

        private int count = 0;

        protected override void ShootProjectile(EnemyUnit target)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Vector3 spawnPosition = target.transform.position;
            object[] instantiationData = new object[]
            {
                target.photonView.ViewID,
                _attack
            };
            if (count == 5)
            {
                PhotonNetwork.InstantiateRoomObject(
                    ZString.Format("Projectile/{0}", _projectilePrefab2.name),
                    spawnPosition,
                    Quaternion.identity,
                    0,
                    instantiationData
                );
                count = 0;
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject(
                    ZString.Format("Projectile/{0}", _projectilePrefab.name),
                    spawnPosition,
                    Quaternion.identity,
                    0,
                    instantiationData
                );
            }

            count++;
        }
    }
}
