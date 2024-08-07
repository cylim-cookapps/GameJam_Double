using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Tween = PrimeTween.Tween;

namespace Pxp
{
    public class ToastUI : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textToast;

        [SerializeField]
        private GameObject _goToast;

        public void OnInitialize()
        {
            _goToast.SetActive(false);
            EventManager.Inst.EventToast.AddListener(OnEventToast);
        }

        private IEnumerator CoToast()
        {
            _goToast.SetActive(true);
            yield return new WaitForSeconds(2f);
            _goToast.SetActive(false);
        }

        #region EventHandler

        private void OnEventToast(string value)
        {
            _textToast.SetText(value);
            StopCoroutine(nameof(CoToast));
            StartCoroutine(nameof(CoToast));
        }

        #endregion
    }
}
