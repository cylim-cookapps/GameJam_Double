using System;
using Cysharp.Threading.Tasks;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pxp
{
    public class AppInitialize : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textState;

        [SerializeField]
        private GameObject _goLoading;

        public async void Start()
        {
            AppInitializeProcess().Forget();
        }

        private async UniTaskVoid AppInitializeProcess()
        {
            _textState.SetText("테이블 로드중");
            await SpecDataManager.Inst.LoadSpecData();
            _textState.SetText("로그인 중");
            await UserManager.Inst.OnInitialize();
            _goLoading.SetActive(true);
            await SceneManager.LoadSceneAsync(1,LoadSceneMode.Additive);
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            await SceneManager.UnloadSceneAsync(0);
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
