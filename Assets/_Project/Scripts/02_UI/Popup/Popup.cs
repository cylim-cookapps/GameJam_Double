using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    [RequireComponent(typeof(Canvas))]
    public abstract class Popup<T> : Popup where T : Popup<T>
    {
        protected virtual void Awake()
        {
            transform.name = typeof(T).Name;
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnLocalizeEvent()
        {
        }
    }

    public abstract class BaseUI<T> : BaseUI where T : BaseUI<T>
    {
        protected virtual void Awake()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void CheckUI()
        {
        }
    }

    public abstract class Popup : BaseUI
    {
        [GetComponent] public Canvas canvas;
    }

    public abstract class BaseUI : CachingMono
    {
        [SerializeField] [GetComponentInChildrenName]
        private RectTransform dim;

        public bool isAnimation = false;

        public List<Button> closeButtons;

        protected bool isInit = false;

        protected virtual void Initialize()
        {
            isInit = true;
            if (closeButtons != null)
            {
                foreach (Button button in closeButtons)
                {
                    button.onClick.AddListener(Hide);
                }
            }
        }

        public virtual void Show()
        {
            if (!isInit)
            {
                Initialize();
            }

            PopupManager.Inst.PushUI(this);
        }

        public virtual void Hide()
        {
            PopupManager.Inst.PopUI(this);
        }
    }
}
