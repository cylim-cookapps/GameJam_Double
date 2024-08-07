using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pxp.Data;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Pxp
{
    public class AppInitialize : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textState;

        public async void Start()
        {
            AppInitializeProcess().Forget();
        }

        private async UniTaskVoid AppInitializeProcess()
        {
            await UnityServices.InitializeAsync();
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.Log(AuthenticationService.Instance.PlayerId);
                }
            }

            _textState.SetText("테이블 로드중");
            await SpecDataManager.Inst.LoadSpecData();
            _textState.SetText("로그인 중");

        }
    }

    public enum Enum_AppInitialize
    {
        Splash,
        SpecLoad,
        Login,
        Start,
    }
}
