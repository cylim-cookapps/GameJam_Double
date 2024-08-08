using System.Collections.Generic;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    internal class Enemy : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }

        private Monster _data;
        private List<Transform> _waypoints;

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
            Vector2 targetPosition = _waypoints[currentWaypointIndex].position;
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
            var id = (int) instantiationData[0];
            _data = SpecDataManager.Inst.Monster.Get(id);
            Hp = MaxHp = _data.hp;
            _waypoints = GameManager.Inst.Waypoints;
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
                PhotonNetwork.Destroy(photonView);
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
