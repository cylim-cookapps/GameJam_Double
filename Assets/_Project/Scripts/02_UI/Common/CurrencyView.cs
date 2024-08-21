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
        private bool _isAnimation = true;

        private Tween _tween;
        private UserCurrencyItem _userData;

        private void Awake()
        {
            _userData = UserManager.Inst.Currency.GetCurrency(itemType);
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
            _textCount.SetText(_userData.Count);
        }

        private void OnEventUpdate(int value)
        {
            _textCount.SetText(value);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
#if __DEV || UNITY_EDITOR
            _userData.Increase(10000);
#endif
        }
    }
}
