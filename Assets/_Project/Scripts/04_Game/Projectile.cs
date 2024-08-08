using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace Pxp
{
    public class Projectile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
    {
        public float speed = 10f;
        public int damage = 10;
        public float hitEffectTime = 1f;
        public GameObject projectile;
        public GameObject effect;

        private GameObject target;
        private bool isDestroyed = false;

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            if (instantiationData != null && instantiationData.Length > 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    int targetViewID = (int) instantiationData[0];
                    PhotonView targetView = PhotonView.Find(targetViewID);
                    if (targetView != null)
                    {
                        SetTarget(targetView.gameObject);
                    }
                }
            }

            isDestroyed = false;
            projectile.SetActive(true);
            effect.SetActive(false);
        }

        public void SetTarget(GameObject newTarget)
        {
            target = newTarget;
        }

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;

            if (isDestroyed)
                return;

            if (target == null)
            {
                PhotonNetwork.Destroy(gameObject);
                return;
            }

            // 타겟 방향 갱신 및 즉시 회전
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // 현재 바라보는 방향으로 이동
            transform.position += direction * speed * Time.deltaTime;

            // 타겟과의 충돌 체크
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    EnemyUnit monster = target.GetComponent<EnemyUnit>();
                    if (monster != null)
                    {
                        monster.ReceiveAttack(damage);
                    }

                    photonView.RPC(nameof(EffectRPC), RpcTarget.All);
                    isDestroyed = true;
                    StartCoroutine(DelayDestroy(hitEffectTime));
                }
            }
        }

        [PunRPC]
        private void EffectRPC()
        {
            projectile.SetActive(false);
            effect.SetActive(true);
        }

        private IEnumerator DelayDestroy(float delay)
        {
            yield return new WaitForSeconds(delay);
            PhotonNetwork.Destroy(gameObject);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }
    }
}
