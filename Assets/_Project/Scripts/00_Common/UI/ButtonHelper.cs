using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pxp
{
    public class ButtonHelper : MonoBehaviour
    {
        #region public

        public eButtonHelperType type;

        public Graphic colorTarget;
        public Image imageTarget;
        public TextMeshProUGUI text;
        public GameObject obj;
        public bool inversionObj;
        public bool isLocalizeKey;
        public string interactableText;
        public string deInteractableText;

        public void SetInteractable(bool isInteractable)
        {
            Init();

            if (type.IsFlagSet(eButtonHelperType.Color))
            {
                if (colorTarget != null)
                {
                    colorTarget.SetColor(isInteractable ? interactableColor : deInteractableColor);
                    colorTarget.color = isInteractable ? interactableColor : deInteractableColor;
                }
            }

            if (type.IsFlagSet(eButtonHelperType.Sprite))
            {
                if (imageTarget != null)
                {
                    imageTarget.SetSprite(isInteractable ? interactableSprite : deInteractableSprite);
                }
            }

            if (type.IsFlagSet(eButtonHelperType.Text))
            {
                if (text != null)
                {
                    text.color = isInteractable ? interactableColor : deInteractableColor;
                }
            }

            if (type.IsFlagSet(eButtonHelperType.GameObj))
            {
                if (obj != null)
                {
                    if(inversionObj)
                        obj.SetActive(!isInteractable);
                    else
                        obj.SetActive(isInteractable);
                }
            }
        }

        #endregion

        #region protected

        #endregion

        #region private

        [SerializeField]
        private Color interactableColor, deInteractableColor = Color.white;

        [SerializeField]
        private Sprite interactableSprite, deInteractableSprite;

        private bool _isCaching = false;

        private void Init()
        {
            if (_isCaching) return;

            _isCaching = true;

            if (colorTarget != null)
                interactableColor = colorTarget.color;

            if (imageTarget != null)
                interactableSprite = imageTarget.sprite;
        }

        #endregion
    }

    [Flags]
    public enum eButtonHelperType
    {
        None = 0,
        Color = 1 << 0,
        Sprite = 1 << 1,
        Text = 1 << 2,
        GameObj = 1 << 3,
    }
}
