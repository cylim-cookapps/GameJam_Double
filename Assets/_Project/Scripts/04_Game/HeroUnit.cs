using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    public class HeroUnit : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        public int HeroId { get; private set; }
        public int Index { get; private set; }
        public int Owner { get; private set; }

        private int _grade;

        public int Grade
        {
            get => _grade;
            set
            {
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetGradeRpc), RpcTarget.All, value);
            }
        }

        [PunRPC]
        private void SetGradeRpc(int grade)
        {
            _grade = grade;
        }

        [SerializeField]
        private GameObject _projectilePrefab;

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            HeroId = (int)instantiationData[0];
            Index = (int)instantiationData[1];
            Owner = (int)instantiationData[2];
        }

        #region Attack

        public float attackRange = 5f;
        public float attackCooldown = 1f;

        private float lastAttackTime;

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (_projectilePrefab == null)
                return;

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                TryAttack();
            }
        }

        private void TryAttack()
        {
            EnemyUnit nearestMonster = FindNearestMonster();
            if (nearestMonster != null && Vector3.Distance(transform.position, nearestMonster.transform.position) <= attackRange)
            {
                ShootProjectile(nearestMonster.photonView.ViewID);
                lastAttackTime = Time.time;
            }
        }

        private void ShootProjectile(int targetViewID)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Vector3 spawnPosition = transform.position + transform.right * 0.5f;  // 유닛의 약간 앞에서 발사
            object[] instantiationData = new object[] { targetViewID };
            GameObject projectileObj = PhotonNetwork.InstantiateRoomObject(
                ZString.Format("Projectile/{0}", _projectilePrefab.name),
                spawnPosition,
                transform.rotation,  // 유닛의 현재 회전값 사용
                0,
                instantiationData
            );

            photonView.RPC("OnProjectileCreated", RpcTarget.All, projectileObj.GetComponent<PhotonView>().ViewID, targetViewID);
        }

        [PunRPC]
        private void OnProjectileCreated(int projectileViewID, int targetViewID)
        {
            PhotonView projectileView = PhotonView.Find(projectileViewID);
            PhotonView targetView = PhotonView.Find(targetViewID);

            if (projectileView != null && targetView != null)
            {
                Projectile projectile = projectileView.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.SetTarget(targetView.gameObject);
                }
            }
        }

        private EnemyUnit FindNearestMonster()
        {
            EnemyUnit[] enemies = FindObjectsOfType<EnemyUnit>();
            EnemyUnit nearest = null;
            float minDistance = float.MaxValue;

            foreach (EnemyUnit enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        #endregion
    }
}
