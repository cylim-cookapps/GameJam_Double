using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachingMono : MonoBehaviour
{
    private Transform _cachedTransform;
    private GameObject _cachedGameObject;
    private RectTransform _cachedRectTransform;

    public new Transform transform
    {
        get
        {
            if (ReferenceEquals(_cachedTransform ,null))
            {
                _cachedTransform = base.transform;
            }
            return _cachedTransform;
        }
    }

    public new GameObject gameObject
    {
        get
        {
            if (ReferenceEquals(_cachedGameObject ,null))
            {
                _cachedGameObject = base.gameObject;
            }
            return _cachedGameObject;
        }
    }

    public RectTransform rectTransform
    {
        get
        {
            if (ReferenceEquals(_cachedRectTransform ,null))
            {
                TryGetComponent(out _cachedRectTransform);
            }
            return _cachedRectTransform;
        }
    }
}
