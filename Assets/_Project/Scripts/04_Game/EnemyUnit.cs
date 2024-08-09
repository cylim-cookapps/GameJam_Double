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
            }
        }

        public int MaxHp { get; private set; }

        public int Coin { get; private set; }
        public int Chip { get; private set; }
        public Enum_monsterType MonsterType { get; private set; }

        public Monster MonsterData { get; private set; }
        private List<Vector2> _waypoints;
        private bool _isDead;

        [SerializeField]
        private int currentWaypointIndex = 0;

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MoveToNextWaypoint();
            }
        }

        private void MoveToNextWaypoint()
        {
            if (_waypoints == null || _waypoints.Count == 0) return;

            Vector2 targetPosition = _waypoints[currentWaypointIndex];
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, 1 * Time.deltaTime);

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
            MonsterType = MonsterData.monsterType;
            _waypoints = new List<Vector2>((Vector2[]) instantiationData[3]);
            _isDead = false;
            Hp = MaxHp = MonsterData.hp;

            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                transform.rotation = Quaternion.Euler(0, 0, -180);
            }
        }

        [PunRPC]
        public void TakeDamage(int damage)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (_isDead) return;

            Hp -= damage;
            AudioController.Play("SFX_TakeDamage");
            if (Hp < 0) Hp = 0;

            if (Hp <= 0)
            {
                DestroyEnemy();
                _isDead = true;
            }
        }

        private void DestroyEnemy()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Inst.DeadEnemy(this);
                AudioController.Play("SFX_DestroyEnemy");
                OnEnemyDestroyed();
            }
        }

        private void OnEnemyDestroyed()
        {
            Debug.Log($"Enemy destroyed: {gameObject.name}");
        }

        public void ReceiveAttack(int damage)
        {
            photonView.RPC(nameof(TakeDamage), RpcTarget.MasterClient, damage);
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
