using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using UnityEngine;

namespace Pxp
{
    public class PopupManager : MonoDontDestroySingleton<PopupManager>
    {
        #region public

        [SerializeField, GetComponentInChildrenOnly]
        private Canvas _canvas;

        public bool IsEscapeEnabled { get; set; } = true;
        public int PopupCount => popupStack.Count;

        public T GetPopup<T>() where T : Popup<T>

        {
            if (_dicPopup.TryGetValue(typeof(T).Name, out Popup prefab))
            {
                return (T) prefab;
            }

            prefab = CreateInstance<T>();
            prefab.Initialize();
            _dicPopup.Add(typeof(T).Name, prefab);
            return (T) prefab;
        }

        public void PopUI(BaseUI baseUI)
        {
            if (popupStack.Contains(baseUI))
            {
                popupStack.Remove(baseUI);
                baseUI.SetActive(false);
                for (var i = 0; i < popupStack.Count; i++)
                {
                    var popupUI = popupStack[i] as Popup;
                    if (popupUI != null)
                    {
                        popupUI.canvas.sortingOrder = (i + 1) * 2;
                    }
                }
            }
        }

        public void PopUINotDisable(BaseUI baseUI)
        {
            if (popupStack.Contains(baseUI))
            {
                popupStack.Remove(baseUI);
                for (var i = 0; i < popupStack.Count; i++)
                {
                    var popupUI = popupStack[i] as Popup;
                    if (popupUI != null)
                    {
                        popupUI.canvas.sortingOrder = (i + 1) * 2;
                    }
                }
            }
        }

        public void PushUI(BaseUI baseUI)
        {
            if (!popupStack.Contains(baseUI))
            {
                popupStack.Add(baseUI);
                var popupUI = baseUI as Popup;
                baseUI.SetActive(true);

                if (popupUI != null)
                {
                    popupUI.canvas.overrideSorting = true;
                    popupUI.canvas.sortingOrder = popupStack.Count * 2;
                }
            }
            else
            {
                popupStack.Remove(baseUI);
                popupStack.Add(baseUI);
                baseUI.SetActive(true);
                for (var i = 0; i < popupStack.Count; i++)
                {
                    var popupUI = popupStack[i] as Popup;
                    if (popupUI != null)
                    {
                        popupUI.canvas.sortingOrder = (i + 1) * 2;
                    }
                }
            }
        }

        public void AllDestroy()
        {
            while (popupStack.Count > 0)
            {
                popupStack.Last().Hide();
            }

            foreach (var popup in _dicPopup)
            {
                Destroy(popup.Value.gameObject);
            }

            _dicPopup.Clear();
        }

        #endregion

        #region protected

        #endregion

        #region private

        private Dictionary<string, Popup> _dicPopup = new();
        private List<BaseUI> popupStack = new();

        private T CreateInstance<T>() where T : Popup
        {
            var prefab = Instantiate(Resources.Load<T>(ZString.Format("Popup/{0}", typeof(T).Name)), _canvas.transform);
            prefab.SetActive(false);
            return prefab;
        }

        private void ClearPopup()
        {
            while (popupStack.Count > 0)
            {
                popupStack.Last().Hide();
            }

            foreach (var popup in _dicPopup)
            {
                Destroy(popup.Value);
            }

            _dicPopup.Clear();
        }

        private void SetEscape()
        {
#if UNITY_EDITOR || UNITY_ANDROID

            if (!IsEscapeEnabled)
            {
                return;
            }

            if (popupStack.Count.Equals(0))
            {
//                PopupExit.Inst.Show();
            }
            else
            {
                popupStack.Last().Hide();
            }
#endif
        }

        #endregion

        #region lifecycle

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetEscape();
            }
        }

        #endregion

        #region EventHandler

        #endregion
    }
}
