using CookApps.Obfuscator;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PrimeTween;
using Pxp.Data;
using TMPro;

namespace Pxp
{
    public class CurrencyView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, GetComponentInChildrenName]
        private Image _imgIcon;

        [SerializeField, GetComponentInChildrenName]
        private TextMeshProUGUI _textCount;

        [SerializeField]
        private Enum_ItemType itemType;

        [SerializeField]
        private bool _isAnimation = false;

        private ObfuscatorDouble _lastValue;
        private Tween _tween;
        private UserCurrencyItem _userData;

        private void Awake()
        {
            _userData = UserManager.Inst.Currency.GetCurrency(itemType);
        }

        private void Update()
        {
            _lastValue.RegenerateEncryptionKey();
        }

        private void OnEnable()
        {
            _userData?.EventUpdate.AddListener(OnEventUpdate);

            Refresh();
        }

        private void OnDisable()
        {
            _userData?.EventUpdate.RemoveListener(OnEventUpdate);
        }

        void Refresh()
        {
            _lastValue = _userData.Count;
            _textCount.SetText(_lastValue.Value.ToDoubleString());
        }

        private void OnEventUpdate(double value)
        {
            if (_isAnimation)
            {
                if (_tween.isAlive)
                    _tween.Stop();

                if (_lastValue > value)
                {
                    _lastValue = value;
                    _textCount.SetText(_lastValue.Value.ToDoubleString());
                    return;
                }

                _tween = Tween.Custom(_lastValue.Value, value, 0.2f, (f) =>
                {
                    _lastValue = f;
                    _textCount.SetText(_lastValue.Value.ToDoubleString());
                });
            }
            else
            {
                _lastValue = value;
                _textCount.SetText(_lastValue.Value.ToDoubleString());
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
#if __DEV || UNITY_EDITOR
            _userData.Increase(10000);
#endif
        }
    }
}
