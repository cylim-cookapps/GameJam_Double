using System;
using System.Collections;
using System.Collections.Generic;
using AnnulusGames.LucidTools.RandomKit;
using CookApps.Inspector;
using Cysharp.Text;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Pxp
{
    public class HeroUnit : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        [SerializeField, GetComponentInChildrenOnly]
        private StarUI _starUI;

        [SerializeField, GetComponentInChildrenOnly]
        private PhotonAnimatorView _photonAnimatorView;

        [SerializeField, GetComponentInChildrenOnly]
        private SortingGroup _sortingGroup;

        [SerializeField]
        private GameObject _effectLevelUp, _effectMerge;

        [SerializeField, GetComponentInChildrenName]
        private SpriteRenderer _rangeSprite;

        public int HeroId { get; private set; }
        public int BoardIndex { get; private set; }
        public int Owner { get; private set; }
        public InGameHeroData InGameHeroData { get; private set; }
        public InGameUnitData InGameUnitData { get; private set; }
        public Hero HeroData { get; private set; }
        public float _sturnRate;
        public float _sturnTime;

        private int originalAtk
        {
            get
            {
                if (HeroData != null)
                {
                    return HeroData.attack + (HeroData.goldLevelup * (InGameHeroData.Level - 1)) + (HeroData.starLevelUp * InGameHeroData.Star);
                }

                return 0;
            }
        }

        public void SetViewAttackRange(bool isOn)
        {
            _rangeSprite.SetActive(isOn);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            HeroId = (int) instantiationData[0];
            BoardIndex = (int) instantiationData[1];
            Owner = (int) instantiationData[2];

            InGameHeroData = GameManager.Inst.GetPlayerData(Owner).Heroes.Find(x => x.HeroId == HeroId);
            InGameUnitData = GameManager.Inst.GetPlayerData(Owner).Units[BoardIndex];

            HeroData = SpecDataManager.Inst.Hero.Get(HeroId);
            _attackRange = HeroData.attackRange;
            _rangeSprite.transform.localScale = Vector3.one * _attackRange * 2f;
            _attackSpeed = HeroData.attackSpeed;

            _attack = originalAtk + (HeroData.attack_levelUp * InGameHeroData.InGameLevel) + (HeroData.attack_GradeUp * InGameUnitData.Grade);
            _effectLevelUp.SetActive(false);
            _effectMerge.SetActive(false);
            _starUI.SetGrade(InGameUnitData.Grade);

            GameManager.Inst.spawnedHeroes[Owner][BoardIndex] = this;
            SetInitialRotation();
            SetupPhotonAnimatorView();
        }

        [PunRPC]
        public void UpgradeHero()
        {
            _attack = originalAtk + (HeroData.attack_levelUp * InGameHeroData.InGameLevel) + (HeroData.attack_GradeUp * InGameUnitData.Grade);
            _starUI.SetGrade(InGameUnitData.Grade);
            _effectMerge.SetActive(false);
            _effectMerge.SetActive(true);
        }

        public void MoveHero(int index, Vector2 pos)
        {
            BoardIndex = index;
            if (PhotonNetwork.IsMasterClient)
                transform.position = pos;
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

        [FormerlySerializedAs("_attackCooldown")]
        public float _attackSpeed = 1f;

        private float lastAttackTime;

        private void Awake()
        {
            EventManager.Inst.EventGameHeroUpgrade.AddListener(OnEventGameHeroUpgrade);
        }

        private void OnDestroy()
        {
            EventManager.Inst.EventGameHeroUpgrade.RemoveListener(OnEventGameHeroUpgrade);
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (Time.time - lastAttackTime >= _attackSpeed)
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
                if (_sturnRate > 0f)
                {
                    if (LucidRandom.GetChance(_sturnRate))
                    {
                        target.ReceiveAttack(_attack, 0f, _sturnTime);
                    }
                }
                else
                {
                    target.ReceiveAttack(_attack);
                }
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

        #region EventHandler

        private void OnEventGameHeroUpgrade(int actor, int heroId)
        {
            if (Owner == actor && heroId == HeroId)
            {
                _attack = originalAtk + (HeroData.attack_levelUp * InGameHeroData.InGameLevel) + (HeroData.attack_GradeUp * InGameUnitData.Grade);
                _effectLevelUp.SetActive(false);
                _effectLevelUp.SetActive(true);
            }
        }

        #endregion

        private void OnDrawGizmos()
        {
            if (_attackRange > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, _attackRange);
            }
        }
    }

    public enum Enum_AttackType
    {
        Projectile,
        Melee
    }
}
