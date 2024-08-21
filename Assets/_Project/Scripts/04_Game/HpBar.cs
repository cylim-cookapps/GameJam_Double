using Cysharp.Text;
using Photon.Pun;
using Pxp.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField, GetComponentInChildrenName]
        private RectTransform _hpBar;

        [SerializeField, GetComponent]
        public Slider slider;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textHp, _textTimer;

        [SerializeField]
        private GameObject _goTimer;

        [SerializeField]
        private Vector3 _offset;

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
                    screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position + _offset);
                else
                    screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position - _offset);
                transform.position = screenPoint;
            }
            else
            {
                GameUI.Inst.ReturnHPBar(this);
            }
        }

        public void UpdateHP(int hp, float fillAmount)
        {
            slider.value = fillAmount;
            if (_textHp != null)
                _textHp.SetText(hp);
        }

        public void UpdateTimer(float time)
        {
            _textTimer.SetTextFormat("{0:0.0}s", time);
        }

        public void SetTarget(Enum_monsterType type, Transform target)
        {
            if (type == Enum_monsterType.BOSS)
            {
                _textHp.SetActive(true);
                _goTimer.SetActive(false);
                _hpBar.rect.Set(0, 0, 100, 30);
            }
            else if (type == Enum_monsterType.CAPTAIN)
            {
                _textHp.SetActive(true);
                _goTimer.SetActive(true);
                _hpBar.rect.Set(0, 0, 100, 30);
            }
            else
            {
                _textHp.SetActive(false);
                _goTimer.SetActive(false);
                _hpBar.rect.Set(0, 0, 50, 30);
            }

            _targetTransform = target;
            if (_targetTransform != null)
            {
                Vector2 screenPoint;
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                    screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position + Vector3.up);
                else
                    screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position + Vector3.down);
                transform.position = screenPoint;
                gameObject.SetActive(true);
            }
        }
    }
}
