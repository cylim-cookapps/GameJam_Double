using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace Pxp
{
    public class PoolManager : MonoDontDestroySingleton<PoolManager>
    {
        private Dictionary<string, IObjectPool<GameObject>> pools = new Dictionary<string, IObjectPool<GameObject>>();

        public T Get<T>(T prefab, Transform parent = null) where T : Component
        {
            GameObject obj = GetGameObject(prefab.gameObject, parent);
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Prefab {prefab.name} doesn't have component of type {typeof(T)}");
                Release(obj);
                return null;
            }

            return component;
        }

        public GameObject Get(GameObject prefab, Transform parent = null)
        {
            return GetGameObject(prefab, parent);
        }

        private GameObject GetGameObject(GameObject prefab, Transform parent)
        {
            if (!pools.ContainsKey(prefab.name))
            {
                CreatePool(prefab);
            }

            GameObject obj = pools[prefab.name].Get();
            SetParent(obj, parent);
            obj.SetActive(true);
            return obj;
        }

        public void Release<T>(T component) where T : Component
        {
            if (component != null)
            {
                Release(component.gameObject);
            }
        }

        public void Release(GameObject obj)
        {
            if (obj == null) return;

            if (pools.ContainsKey(obj.name))
            {
                SetParent(obj, transform); // PoolManager를 부모로 설정
                pools[obj.name].Release(obj);
            }
            else
            {
                Debug.LogWarning($"Trying to release an object that is not pooled: {obj.name}");
                Destroy(obj);
            }
        }

        private void SetParent(GameObject obj, Transform parent)
        {
            obj.transform.SetParent(parent ? parent : transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

        private void CreatePool(GameObject prefab)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(prefab);
                    obj.name = prefab.name; // 풀에서 생성된 오브젝트의 이름을 프리팹 이름과 동일하게 설정
                    return obj;
                },
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: 5
            );

            pools.Add(prefab.name, pool);
        }

        public void ClearPool(string poolName)
        {
            if (pools.ContainsKey(poolName))
            {
                pools[poolName].Clear();
            }
        }

        public void ClearAllPools()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
        }
    }
}
