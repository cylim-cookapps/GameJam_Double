using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pxp
{
    public class MainUI : MonoSingleton<MainUI>
    {
        [SerializeField, GetComponentInChildrenOnly]
        private InGameUI _inGameUI;

        [SerializeField, GetComponentInChildrenOnly]
        private OutGameUI _outGameUI;

        [SerializeField, GetComponentInChildrenOnly]
        private ToastUI _toastUI;

        public InGameUI InGameUI => _inGameUI;
        public OutGameUI OutGameUI => _outGameUI;

        private void Awake()
        {
            _inGameUI.OnInitialize();
            _outGameUI.OnInitialize();
            _toastUI.OnInitialize();
        }
    }
}
