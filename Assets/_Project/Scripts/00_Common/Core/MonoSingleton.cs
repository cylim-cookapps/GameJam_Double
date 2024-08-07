using System;
using UnityEngine;
using System.Collections;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _inst = null;

    public static T Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = FindAnyObjectByType<T>();

                if (_inst == null)
                {
                    var temp = new GameObject(typeof(T).Name);
                    _inst = (T) temp.AddComponent(typeof(T));
                }
            }

            return _inst;
        }
    }

    public static bool IsCreated => _inst != null;

    protected virtual void OnDestroy()
    {
        _inst = null;
    }
}
