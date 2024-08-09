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
        [SerializeField, GetComponentInChildrenOnly]
        private PhotonAnimatorView _photonAnimatorView;

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
            SetupPhotonAnimatorView();
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

        private void SetupPhotonAnimatorView()
        {
            // 동기화할 애니메이션 파라미터 설정
            _photonAnimatorView.SetLayerSynchronized(0, PhotonAnimatorView.SynchronizeType.Continuous);
            _photonAnimatorView.SetParameterSynchronized("Attack", PhotonAnimatorView.ParameterType.Trigger, PhotonAnimatorView.SynchronizeType.Discrete);
        }

        #region Attack

        [SerializeField]
        protected GameObject _projectilePrefab;

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

            if (Time.time - lastAttackTime >= _attackCooldown)
            {
                lastAttackTime = Time.time;
                EnemyUnit nearestMonster = FindNearestMonster();
                if (nearestMonster != null && Vector3.Distance(transform.position, nearestMonster.transform.position) <= _attackRange)
                {
                    _anim.SetTrigger("Attack");
                }
            }
        }

        public void TryAttack()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            EnemyUnit nearestMonster = FindNearestMonster();
            if (nearestMonster != null && Vector3.Distance(transform.position, nearestMonster.transform.position) <= _attackRange)
            {
                AudioController.Play("SFX_Hero_Attack");
                if (attackType == Enum_AttackType.Projectile)
                    ShootProjectile(nearestMonster);
                else
                    MeleeAttack(nearestMonster);
            }
        }

        protected virtual void MeleeAttack(EnemyUnit target)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (target != null)
            {
                _hitPrefab.transform.position = target.transform.position;
                _hitPrefab.gameObject.SetActive(false);
                _hitPrefab.gameObject.SetActive(true);
                target.TakeDamage(_attack);
            }
        }

        protected virtual void ShootProjectile(EnemyUnit target)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (_projectilePrefab == null) return;

            Vector3 spawnPosition = transform.position;
            object[] instantiationData = new object[]
            {
                target.photonView.ViewID,
                _attack,
                Owner
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
