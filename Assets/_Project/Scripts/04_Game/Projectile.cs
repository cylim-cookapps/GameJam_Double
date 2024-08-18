using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace Pxp
{
    public class Projectile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
    {
        public LayerMask enemyLayer;
        public float speed = 10f;
        public int damage = 10;
        public float hitEffectTime = 1f;
        public bool isRotation;
        public GameObject projectile;
        public GameObject effect;
        public bool isFollow;
        public float slowTime = 0f;
        public float hitRange = 0f;
        public int dotHit = 0;

        private GameObject target;
        private bool isDestroyed = false;
        protected int actor;

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            if (instantiationData != null && instantiationData.Length > 0)
            {
                int targetViewID = (int) instantiationData[0];
                damage = (int) instantiationData[1];
                actor = (int) instantiationData[2];
                PhotonView targetView = PhotonView.Find(targetViewID);
                if (targetView != null)
                {
                    SetTarget(targetView.gameObject);
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
            if (isDestroyed)
            {
                if (isFollow)
                {
                    if (target != null)
                    {
                        transform.position = target.transform.position;
                        effect.SetActive(false);
                    }
                }

                return;
            }

            if (PhotonNetwork.IsMasterClient == false)
                return;

            if (target == null)
            {
                PhotonNetwork.Destroy(gameObject);
                return;
            }

            // 타겟 방향 갱신 및 즉시 회전
            Vector3 direction = (target.transform.position - transform.position).normalized;

            if (isRotation)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // 현재 바라보는 방향으로 이동
            transform.position += direction * speed * Time.deltaTime;

            // 타겟과의 충돌 체크
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (hitRange > 0f)
                        ApplySplashDamage();
                    else
                        ApplySingleTargetDamage();

                    photonView.RPC(nameof(EffectRPC), RpcTarget.All);
                    isDestroyed = true;
                    if (dotHit > 0)
                        StartCoroutine(DotDamage());
                    else
                        StartCoroutine(DelayDestroy(hitEffectTime));
                }
            }
        }

        private void ApplySplashDamage()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, hitRange, enemyLayer);
            foreach (Collider2D hitCollider in hitColliders)
            {
                EnemyUnit enemy = hitCollider.GetComponent<EnemyUnit>();
                if (enemy != null)
                {
                    enemy.ReceiveAttack(damage);
                }
            }
        }

        private void ApplySingleTargetDamage()
        {
            if (target == null)
                return;

            if (target.TryGetComponent<EnemyUnit>(out var enemy))
            {
                enemy.ReceiveAttack(damage, slowTime);
            }
        }

        [PunRPC]
        private void EffectRPC()
        {
            isDestroyed = true;
            projectile.SetActive(false);
            effect.SetActive(true);
        }

        private IEnumerator DotDamage()
        {
            for (int i = 0; i < dotHit; i++)
            {
                yield return new WaitForSeconds(1f);
                if (hitRange > 0f)
                    ApplySplashDamage();
                else
                {
                    if (target == null)
                        break;

                    ApplySingleTargetDamage();
                }
            }

            PhotonNetwork.Destroy(gameObject);
        }

        private IEnumerator DelayDestroy(float delay)
        {
            yield return new WaitForSeconds(delay);
            PhotonNetwork.Destroy(gameObject);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }

        private void OnDrawGizmos()
        {
            if (hitRange > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, hitRange);
            }
        }
    }
}
