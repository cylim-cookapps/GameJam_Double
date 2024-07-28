using System;
using UnityEngine;

namespace Pxp
{
    /// <summary>
    /// MonoBehaviour Singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _inst;

        public static T Inst
        {
            get
            {
                if (!IsInstantiated)
                {
                    CreateInstance();
                }

                return _inst;
            }
        }

        public static Transform CachedTransform { get; private set; }

        public static bool IsInstantiated { get; private set; }

        public static bool IsDestroyed { get; private set; }

        public abstract bool IsDontDestroy { get; }

        private static void CreateInstance()
        {
            if (IsDestroyed)
            {
                return;
            }

            Type type = typeof(T);
            T[] objects = FindObjectsOfType<T>();
            if (0 < objects.Length)
            {
                if (1 < objects.Length)
                {
                    Debug.LogWarning(
                        $"There is more than one instance of Singleton of type \"{type}\". Keeping the first one. Destroying the others.");

                    for (var i = 1; i < objects.Length; i++)
                    {
                        Destroy(objects[i].gameObject);
                    }
                }

                _inst = objects[0];
                _inst.gameObject.SetActive(true);
                IsInstantiated = true;
                IsDestroyed = false;
                return;
            }

            var noAutoAttribute = Attribute.GetCustomAttribute(type, typeof(NotAutoSingletonAttribute)) as NotAutoSingletonAttribute;
            if (noAutoAttribute != null)
                return;

            string prefabName;
            GameObject gameObject;
            var attribute = Attribute.GetCustomAttribute(type, typeof(PrefabAttribute)) as PrefabAttribute;
            if (attribute == null || string.IsNullOrEmpty(attribute.Name))
            {
                prefabName = type.ToString();
                gameObject = new GameObject();
            }
            else
            {
                prefabName = attribute.Name;
                gameObject = Instantiate(Resources.Load<GameObject>(prefabName));
                if (!gameObject)
                {
                    throw new Exception(
                        $"Could not find Prefab \"{prefabName}\" on Resources for Singleton of type \"{type}\".");
                }
            }

            gameObject.name = prefabName;
            if (!_inst)
            {
                _inst = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
            }

            CachedTransform = gameObject.transform;
            IsInstantiated = true;
            IsDestroyed = false;
        }

        protected virtual void Awake()
        {
            if (_inst == null)
            {
                CreateInstance();
            }

            if (IsDontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (GetInstanceID() != _inst.GetInstanceID())
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            IsDestroyed = IsDontDestroy;
            IsInstantiated = false;

            if (!IsDontDestroy)
            {
                _inst = null;
            }
        }

        #region RuntimeInitialize

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            _inst = null;
        }

        #endregion
    }
}
