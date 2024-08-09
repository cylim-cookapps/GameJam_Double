using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace Pxp
{
    public class MechaMong : HeroUnit
    {
        protected override void ShootProjectile(EnemyUnit target)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Vector3 spawnPosition = target.transform.position;
            object[] instantiationData = new object[]
            {
                target.photonView.ViewID,
                _attack
            };

            PhotonNetwork.InstantiateRoomObject(
                ZString.Format("Projectile/{0}", _projectilePrefab.name),
                spawnPosition,
                Quaternion.identity,
                0,
                instantiationData
            );
        }
    }
}
