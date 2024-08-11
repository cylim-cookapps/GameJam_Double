using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
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
        [GetComponent]
        public Canvas canvas;
    }

    public abstract class BaseUI : CachingMono
    {
        [SerializeField]
        [GetComponentInChildrenName]
        private RectTransform dim;

        [SerializeField, GetComponentInChildrenName]
        private Transform _root;

        [SerializeField]
        private POPUP_ANIMATION_TYPE _popupAnimationType = POPUP_ANIMATION_TYPE.SlideUpDown;

        public bool isAnimation = false;

        public List<Button> closeButtons;

        protected bool isInit = false;
        private Tween _tweenerMain;

        public virtual void Initialize()
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

            if (_root != null)
            {
                _root.localScale = Vector3.zero;
                _tweenerMain = Tween.Custom(0, 1f, 0.2f, (value) => { _root.localScale = Vector3.one * value; });
            }

            PopupManager.Inst.PushUI(this);
        }

        public virtual void Hide()
        {
            if (_tweenerMain.isAlive)
            {
                _tweenerMain.Stop();
            }

            PopupManager.Inst.PopUI(this);
        }
    }

    [Flags]
    public enum POPUP_ANIMATION_TYPE
    {
        NONE = 0,
        Scale = 1 << 0,
        Fade = 1 << 1,
        SlideUpDown = 1 << 2,
    }
}
