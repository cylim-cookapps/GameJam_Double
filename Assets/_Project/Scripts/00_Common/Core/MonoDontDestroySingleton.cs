using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine;

public class MonoDontDestroySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _inst = null;

    public static T Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = GameObject.FindObjectOfType<T>();

                if (_inst == null)
                {
                    var obj= Resources.Load<T>(typeof(T).Name);
                    if (obj == null)
                    {
                        var temp = new GameObject(typeof(T).Name);
                        _inst = temp.AddComponent<T>();
                        _inst.name = typeof(T).Name;
                    }
                    else
                    {
                        _inst = Instantiate(obj);
                        _inst.name = typeof(T).Name;
                    }
                }
            }

            return _inst;
        }
    }

    public static T SetCreate()
    {
        return Inst;
    }

    public static bool IsCreated => _inst != null;

    private void Awake()
    {
        if (_inst == null)
        {
            _inst = this as T;
            if (_inst.transform.parent != null)
                _inst.transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }
        else if (_inst != this)
        {
            Destroy(gameObject);
        }
    }
}


public class MonoPunDontDestroySingleton<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    private static T _inst = null;

    public static T Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = GameObject.FindObjectOfType<T>();

                if (_inst == null)
                {
                    var obj= Resources.Load<T>(typeof(T).Name);
                    if (obj == null)
                    {
                        var temp = new GameObject(typeof(T).Name);
                        _inst = temp.AddComponent<T>();
                        _inst.name = typeof(T).Name;
                    }
                    else
                    {
                        _inst = Instantiate(obj);
                        _inst.name = typeof(T).Name;
                    }
                }
            }

            return _inst;
        }
    }

    public static T SetCreate()
    {
        return Inst;
    }

    public static bool IsCreated => _inst != null;

    private void Awake()
    {
        if (_inst == null)
        {
            _inst = this as T;
            if (_inst.transform.parent != null)
                _inst.transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }
        else if (_inst != this)
        {
            Destroy(gameObject);
        }
    }
}
