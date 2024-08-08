using System.Collections.Generic;
using Photon.Pun;
using Pxp.Data;
using UnityEngine;

namespace Pxp
{
    internal class Enemy : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
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

            if ((Vector2)transform.position == targetPosition)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % _waypoints.Count;
                photonView.RPC(nameof(UpdateWaypointIndex), RpcTarget.Others, currentWaypointIndex);
            }
        }

        [PunRPC]
        private void UpdateWaypointIndex(int newIndex)
        {
            currentWaypointIndex = newIndex;
            Debug.Log($"Waypoint updated: {currentWaypointIndex}");
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            var id = (int)instantiationData[0];
            _data = SpecDataManager.Inst.Monster.Get(id);
            Hp = MaxHp = _data.hp;
            _waypoints = GameManager.Inst.Waypoints;

            // 초기 위치 설정
            if (_waypoints != null && _waypoints.Count > 0)
            {
                transform.position = _waypoints[0].position;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // 마스터 클라이언트가 데이터를 보냄
                stream.SendNext(Hp);
                stream.SendNext(currentWaypointIndex);
            }
            else
            {
                // 다른 클라이언트들이 데이터를 받음
                Hp = (int)stream.ReceiveNext();
                currentWaypointIndex = (int)stream.ReceiveNext();
            }
        }

        [PunRPC]
        public void TakeDamage(int damage)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Hp -= damage;
            if (Hp < 0) Hp = 0;

            if (Hp <= 0)
            {
                DestroyEnemy();
            }
            else
            {
                photonView.RPC("SyncHp", RpcTarget.All, Hp);
            }
        }

        [PunRPC]
        private void SyncHp(int newHp)
        {
            Hp = newHp;
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
            photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }
    }
}
