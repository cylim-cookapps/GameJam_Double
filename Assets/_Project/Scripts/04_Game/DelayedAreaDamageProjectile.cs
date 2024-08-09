using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Pxp
{
    public class DelayedAreaDamageProjectile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
    {
        public LayerMask enemyLayer;
        public float hitRange;
        public float hitTime;
        public float destroyTime;

        private int _damage;
        private bool _isDestroyed = false;

        private GameObject target;

        IEnumerator CoHit()
        {
            yield return new WaitForSeconds(hitTime);
            if (PhotonNetwork.IsMasterClient)
            {
                if (hitRange > 0)
                {
                    ApplySplashDamage();
                }
                else if (target != null)
                {
                    ApplySingleTargetDamage();
                }

                _isDestroyed = true;
                StartCoroutine(DelayDestroy(destroyTime));
            }
        }

        private void ApplySingleTargetDamage()
        {
            EnemyUnit enemy = target.GetComponent<EnemyUnit>();
            if (enemy != null)
            {
                enemy.ReceiveAttack(_damage);
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
                    float distance = Vector2.Distance(transform.position, enemy.transform.position);
                    float damageFactor = 1 - (distance / hitRange);
                    int adjustedDamage = Mathf.RoundToInt(_damage * damageFactor);
                    enemy.ReceiveAttack(adjustedDamage);
                }
            }
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            if (instantiationData != null && instantiationData.Length > 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    int targetViewID = (int) instantiationData[0];
                    _damage = (int) instantiationData[1];
                    PhotonView targetView = PhotonView.Find(targetViewID);
                    if (targetView != null)
                    {
                        SetTarget(targetView.gameObject);
                    }

                    StartCoroutine(CoHit());
                }
            }
        }

        public void SetTarget(GameObject newTarget)
        {
            target = newTarget;
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
