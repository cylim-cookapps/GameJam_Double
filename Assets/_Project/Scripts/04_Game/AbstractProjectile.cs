using UnityEngine;
using Photon.Pun;

namespace Pxp
{
    public enum ProjectileMovementType
    {
        Linear,
        Parabolic,
        Homing
    }

    public enum ProjectileHitType
    {
        SingleTarget,
        Splash
    }

    public interface IProjectile
    {
        void Initialize(Vector3 targetPosition);
        void UpdateProjectile();
    }

    public abstract class AbstractProjectile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IProjectile
    {
        public float speed = 10f;
        public int damage = 10;
        public GameObject projectileObject;
        public GameObject effectObject;
        public ProjectileMovementType movementType = ProjectileMovementType.Linear;
        public ProjectileHitType hitType = ProjectileHitType.SingleTarget;
        public float splashRadius = 3f;
        public float arcHeight = 5f; // 포물선 운동의 최대 높이

        protected Vector3 startPosition;
        protected Vector3 targetPosition;
        protected bool isDestroyed = false;
        protected float journeyLength;
        protected float startTime;

        public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            if (instantiationData != null && instantiationData.Length > 0)
            {
                Initialize((Vector3)instantiationData[0]);
            }

            projectileObject.SetActive(true);
            effectObject.SetActive(false);
        }

        public virtual void Initialize(Vector3 targetPosition)
        {
            this.startPosition = transform.position;
            this.targetPosition = targetPosition;
            this.journeyLength = Vector3.Distance(startPosition, targetPosition);
            this.startTime = Time.time;
        }

        public virtual void UpdateProjectile()
        {
            if (isDestroyed) return;

            switch (movementType)
            {
                case ProjectileMovementType.Linear:
                    UpdateLinearMovement();
                    break;
                case ProjectileMovementType.Parabolic:
                    UpdateParabolicMovement();
                    break;
                case ProjectileMovementType.Homing:
                    UpdateHomingMovement();
                    break;
            }

            CheckHit();
        }

        protected virtual void UpdateLinearMovement()
        {
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
        }

        protected virtual void UpdateParabolicMovement()
        {
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            // 포물선 계산
            float parabola = (1 - 4 * (fractionOfJourney - 0.5f) * (fractionOfJourney - 0.5f)) * arcHeight;
            currentPos.y += parabola;

            transform.position = currentPos;
        }

        protected virtual void UpdateHomingMovement()
        {
            // 가장 가까운 적을 찾아 추적
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                targetPosition = nearestEnemy.transform.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        protected virtual void CheckHit()
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                OnHit();
            }
        }

        protected virtual void OnHit()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                switch (hitType)
                {
                    case ProjectileHitType.SingleTarget:
                        HitSingleTarget();
                        break;
                    case ProjectileHitType.Splash:
                        HitSplashArea();
                        break;
                }

                isDestroyed = true;
                projectileObject.SetActive(false);
                effectObject.SetActive(true);
                photonView.RPC("SyncHit", RpcTarget.All);
                Invoke("DestroyProjectile", 1f);
            }
        }

        protected virtual void HitSingleTarget()
        {
            EnemyUnit enemy = Physics2D.OverlapCircle(transform.position, 0.1f)?.GetComponent<EnemyUnit>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        protected virtual void HitSplashArea()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, splashRadius);
            foreach (Collider2D hitCollider in hitColliders)
            {
                EnemyUnit enemy = hitCollider.GetComponent<EnemyUnit>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        [PunRPC]
        protected virtual void SyncHit()
        {
            projectileObject.SetActive(false);
            effectObject.SetActive(true);
        }

        protected virtual void DestroyProjectile()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

        protected GameObject FindNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject nearest = null;
            float minDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies)
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
    }
}
