using System.Collections;
using System.Collections.Generic;
using CookApps.Inspector;
using Cysharp.Text;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;
using UnityEngine.Serialization;

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

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            HeroId = (int) instantiationData[0];
            Index = (int) instantiationData[1];
            Owner = (int) instantiationData[2];

            var heroData = SpecDataManager.Inst.Hero.Get(HeroId);
            _attackRange = heroData.attackRange;
            _attackCooldown = heroData.attackSpeed;
            _attack = heroData.attack;

            SetInitialRotation();
        }

        private void SetInitialRotation()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 2) // 플레이어 2의 유닛인 경우
            {
                transform.rotation = Quaternion.Euler(0, 0, 180); // 180도 회전
            }
            else
            {
                transform.rotation = Quaternion.identity; // 기본 방향 (오른쪽)
            }
        }

        #region Attack

        [SerializeField]
        private GameObject _projectilePrefab;

        [SerializeField, GetComponentInChildrenName]
        private Animator _anim;

        [SerializeField]
        private GameObject _hitPrefab;

        public Enum_AttackType attackType = Enum_AttackType.Projectile;

        public int _attack;
        public float _attackRange = 5f;
        public float _attackCooldown = 1f;

        private float lastAttackTime;

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (_projectilePrefab == null)
                return;

            if (Time.time - lastAttackTime >= _attackCooldown)
            {
                lastAttackTime = Time.time;
            }
        }

        public void TryAttack()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            EnemyUnit nearestMonster = FindNearestMonster();
            if (nearestMonster != null && Vector3.Distance(transform.position, nearestMonster.transform.position) <= _attackRange)
            {
                _anim.SetTrigger("Attack");
                if (attackType == Enum_AttackType.Projectile)
                    ShootProjectile(nearestMonster);
                else
                    MeleeAttack(nearestMonster);
            }
        }

        private void MeleeAttack(EnemyUnit target)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            _hitPrefab.gameObject.SetActive(false);
            _hitPrefab.gameObject.SetActive(true);
            target.TakeDamage(_attack);
        }

        private void ShootProjectile(EnemyUnit target)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Vector3 spawnPosition = transform.position;
            object[] instantiationData = new object[]
            {
                target.photonView.ViewID,
                _attack
            };
            Vector3 directionToTarget = (target.transform.position - spawnPosition).normalized;
            Quaternion rotationToTarget = Quaternion.LookRotation(Vector3.forward, directionToTarget);

            GameObject projectileObj = PhotonNetwork.InstantiateRoomObject(
                ZString.Format("Projectile/{0}", _projectilePrefab.name),
                spawnPosition,
                rotationToTarget,
                0,
                instantiationData
            );

            photonView.RPC("OnProjectileCreated", RpcTarget.All, projectileObj.GetComponent<PhotonView>().ViewID, target.photonView.ViewID);
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

    public enum Enum_AttackType
    {
        Projectile,
        Melee
    }
}
