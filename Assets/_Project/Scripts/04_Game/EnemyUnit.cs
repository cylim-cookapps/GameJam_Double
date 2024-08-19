using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pxp
{
    public class EnemyUnit : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
    {
        [SerializeField]
        private int _hp;

        public int Hp
        {
            get => _hp;
            private set
            {
                _hp = value;
                UpdateHPBar();
            }
        }

        public int MaxHp { get; private set; }
        public int Coin { get; private set; }
        public int Chip { get; private set; }
        public int BonusCoin { get; private set; }
        public Enum_monsterType MonsterType { get; private set; }

        public bool IsSlow { get; private set; }

        public Monster MonsterData { get; private set; }
        private List<Vector2> _waypoints;
        private bool _isDead;
        private float _moveSpeed;

        [SerializeField]
        private int currentWaypointIndex = 0;

        private Vector2 _lastPosition;

        private HPBar hpBar;

        private void RequestHPBar()
        {
            if (GameUI.Inst != null)
            {
                hpBar = GameUI.Inst.GetHPBar();
                if (hpBar != null)
                {
                    hpBar.SetTarget(transform);
                    UpdateHPBar();
                }
            }
        }

        private void UpdateHPBar()
        {
            if (hpBar != null)
            {
                hpBar.UpdateHP((float) Hp / MaxHp);
            }
        }

        private void Start()
        {
            _lastPosition = transform.position;
        }

        private void UpdateDirection()
        {
            Vector2 currentPosition = transform.position;
            Vector2 movementDirection = currentPosition - _lastPosition;

            if (movementDirection.x != 0)
            {
                bool isMovingRight = movementDirection.x > 0;
                transform.localScale = new Vector3(isMovingRight ? 1 : -1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(currentPosition.x < 0 ? 1 : -1, 1, 1);
            }

            _lastPosition = currentPosition;
        }

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MoveToNextWaypoint();
            }

            UpdateDirection();
        }

        private void MoveToNextWaypoint()
        {
            if (_waypoints == null || _waypoints.Count == 0) return;

            var speed = IsSlow ? _moveSpeed * 0.7f : _moveSpeed;
            Vector2 targetPosition = _waypoints[currentWaypointIndex];
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if ((Vector2) transform.position == targetPosition)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % _waypoints.Count;
                photonView.RPC(nameof(UpdateWaypointIndex), RpcTarget.Others, currentWaypointIndex);
            }
        }

        [PunRPC]
        private void UpdateWaypointIndex(int newIndex)
        {
            currentWaypointIndex = newIndex;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;

            MonsterData = SpecDataManager.Inst.Monster.Get((int) instantiationData[0]);
            Coin = (int) instantiationData[1];
            Chip = (int) instantiationData[2];
            BonusCoin = (int) instantiationData[3];
            MonsterType = MonsterData.monsterType;
            _waypoints = new List<Vector2>((Vector2[]) instantiationData[4]);
            _isDead = false;
            _moveSpeed = MonsterData.moveSpeed;
            Hp = MaxHp = MonsterData.hp;

            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                transform.rotation = Quaternion.Euler(0, 0, -180);
            }

            RequestHPBar();
        }

        [PunRPC]
        public void TakeDamage(int damage, float slowDuration = 0f)
        {
            if (_isDead) return;

            Hp -= damage;
            AudioController.Play("SFX_TakeDamage");
            if (Hp < 0) Hp = 0;
            if (slowDuration > 0)
            {
                StopCoroutine(nameof(SlowEffect));
                StartCoroutine(nameof(SlowEffect), slowDuration);
            }

            GameUI.Inst.ShowDamageText(damage, false, transform);

            if (Hp <= 0)
            {
                DestroyEnemy();
                _isDead = true;
            }
        }

        IEnumerator SlowEffect(float duration)
        {
            IsSlow = true;
            yield return new WaitForSeconds(duration);
            IsSlow = false;
        }

        public void DestroyEnemy()
        {
            if (hpBar != null)
            {
                GameUI.Inst.ReturnHPBar(hpBar);
                hpBar = null;
            }

            AudioController.Play("SFX_DestroyEnemy");

            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Inst.DeadEnemy(this);
            }
        }

        public void ReceiveAttack(int damage, float slowDuration = 0f)
        {
            photonView.RPC(nameof(TakeDamage), RpcTarget.All, damage, slowDuration);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // 데이터 쓰기 (마스터 클라이언트에서 실행)
                stream.SendNext(Hp);
            }
            else
            {
                // 데이터 읽기 (다른 클라이언트에서 실행)
                Hp = (int) stream.ReceiveNext();
            }
        }
    }
}
