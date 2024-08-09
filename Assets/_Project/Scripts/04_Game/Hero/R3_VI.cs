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
            Vector3 directionToTarget = (target.transform.position - spawnPosition).normalized;
            Quaternion rotationToTarget = Quaternion.LookRotation(Vector3.forward, directionToTarget);
            GameObject projectileObj = null;
            if (count == 5)
            {
                projectileObj = PhotonNetwork.InstantiateRoomObject(
                    ZString.Format("Projectile/{0}", _projectilePrefab2.name),
                    spawnPosition,
                    rotationToTarget,
                    0,
                    instantiationData
                );
                count = 0;
            }
            else
            {
                projectileObj = PhotonNetwork.InstantiateRoomObject(
                    ZString.Format("Projectile/{0}", _projectilePrefab.name),
                    spawnPosition,
                    rotationToTarget,
                    0,
                    instantiationData
                );
            }

            count++;
        }
    }
}
