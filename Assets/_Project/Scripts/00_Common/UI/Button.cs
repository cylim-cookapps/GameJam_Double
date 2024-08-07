using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pxp
{
    [Serializable]
    public class ButtonClickedEvent : UnityEvent
    {
    }

    [RequireComponent(typeof(RectTransform))]
    public class Button : Selectable, IPointerClickHandler
    {
        #region public

        public bool isRepetition = false;
        public bool isAnimation;
        public bool isSound = true;
        //public eSfx sfxType = eSfx.SFX_Click;
        public bool isUniformCurve = true;
        public Vector3 fromScale = Vector3.one;
        public Vector3 toScale = Vector3.one;
        public AnimationCurve curveX = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve curveY = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float duration = 1f;

        public List<ButtonHelper> buttonExHelpers = new();
        public ButtonClickedEvent onClick = new();
        public bool isAlwaysClickEvent = false;

        private Sequence _sequence;
        private bool isHolding = false;

        public bool IsOn
        {
            get => interactable;
            set
            {
                interactable = value;
                if (!Application.isPlaying)
                {
                    return;
                }

                if (buttonExHelpers != null)
                {
                    foreach (ButtonHelper helper in buttonExHelpers)
                    {
                        if (helper != null)
                        {
                            helper.SetInteractable(interactable);
                        }
                    }
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (isRepetition)
            {
                isHolding = false;
                StartCoroutine(nameof(HoldTimer));
            }
            else
            {
                _sequence?.PlayForward();
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (isRepetition)
            {
                StopCoroutine(nameof(HoldTimer));
                if (!isHolding)
                {
                    onClick?.Invoke();
                }
            }
            else
            {
                _sequence?.PlayBackwards();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isRepetition)
            {
                return;
            }

            if (isAlwaysClickEvent || IsOn)
            {
                onClick?.Invoke();
                SetSound();
            }
        }

        #endregion

        #region protected

        #endregion

        #region private

        private IEnumerator HoldTimer()
        {
            while (true)
            {
                _sequence?.Restart();
                yield return new WaitForSeconds(duration);
                onClick?.Invoke();
                SetSound();
                isHolding = true;

                yield return null;
            }
        }

        private void SetSound()
        {
            // if (isSound && SoundManager.IsCreated)
            // {
            //     SoundManager.Inst.PlaySFX(sfxType);
            // }
        }

        #endregion

        #region lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (buttonExHelpers != null)
            {
                foreach (ButtonHelper helper in buttonExHelpers)
                {
                    if (helper != null)
                    {
                        helper.SetInteractable(IsOn);
                    }
                }
            }

            if (isAnimation)
            {
                _sequence = DOTween.Sequence().SetAutoKill(false).SetUpdate(true).Pause();

                if (isUniformCurve)
                {
                    _sequence.Append(transform.DOScale(toScale, duration).SetEase(curveX));
                }
                else
                {
                    _sequence.Append(transform.DOScaleX(toScale.x, duration).SetEase(curveX))
                        .Join(transform.DOScaleY(toScale.y, duration).SetEase(curveY));
                }
            }
            else
            {
                isRepetition = false;
            }
        }

        #endregion
    }
}
