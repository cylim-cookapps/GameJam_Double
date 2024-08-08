using System.Collections.Generic;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pxp
{
    public class EnemyUnit : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }

        public int Coin { get; private set; }
        public int Chip { get; private set; }
        public Enum_monsterType MonsterType { get; private set; }

        public Monster MonsterData { get; private set; }
        private List<Vector2> _waypoints;

        [SerializeField]
        private int currentWaypointIndex = 0;

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MoveToNextWaypoint();
                if (Input.GetKeyDown(KeyCode.A))
                {
                    ReceiveAttack(1);
                }
            }
        }

        private void MoveToNextWaypoint()
        {
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

            Hp = MaxHp = MonsterData.hp;

            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                transform.rotation = Quaternion.Euler(0, 0, -180);
            }
        }

        [PunRPC]
        public void TakeDamage(int damage)
        {
            Hp -= damage;
            if (Hp < 0) Hp = 0;

            if (Hp <= 0)
            {
                DestroyEnemy();
            }
        }

        private void DestroyEnemy()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Inst.DeadEnemy(this);
                OnEnemyDestroyed();
            }
        }

        private void OnEnemyDestroyed()
        {
            Debug.Log($"Enemy destroyed: {gameObject.name}");
        }

        public void ReceiveAttack(int damage)
        {
            photonView.RPC(nameof(TakeDamage), RpcTarget.All, damage);
        }
    }
}
