using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField, GetComponent]
        public Slider slider;

        [SerializeField, GetComponent]
        private RectTransform rectTransform;

        private Transform _targetTransform;

        public void ReturnToPool()
        {
            _targetTransform = null;
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (_targetTransform != null)
            {
                Vector2 screenPoint;
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                    screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position + Vector3.up);
                else
                    screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position + Vector3.down);
                rectTransform.position = screenPoint;
            }
            else
            {
                GameUI.Inst.ReturnHPBar(this);
            }
        }

        public void UpdateHP(float fillAmount)
        {
            slider.value = fillAmount;
        }

        public void SetTarget(Transform target)
        {
            _targetTransform = target;
            if (_targetTransform != null)
            {
                Vector2 screenPoint;
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                    screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position + Vector3.up);
                else
                    screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position + Vector3.down);
                rectTransform.position = screenPoint;
                gameObject.SetActive(true);
            }
        }
    }
}
