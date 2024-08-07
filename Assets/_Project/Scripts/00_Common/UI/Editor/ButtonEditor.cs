using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pxp
{
    [CustomEditor(typeof(Button))]
    [CanEditMultipleObjects]
    public class ButtonEditor : Editor
    {
        #region public

        #endregion

        #region protected

        #endregion

        #region private

        private Button _btn;

        private Tween _tween;
        private Tween _tween2;

        private Dictionary<string, SerializedProperty> _dic = new();

        private void PreviewAnimation()
        {
            EditorScriptor.BeginContentBox();
            EditorGUILayout.LabelField("[애니메이션 미리보기]", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("► Play"))
            {
                if (DOTweenEditorPreview.isPreviewing)
                {
                    return;
                }

                RemoveTween();

                _btn.transform.localScale = _btn.fromScale;

                if (_btn.isUniformCurve)
                {
                    _tween = _btn.transform.DOScale(_btn.toScale, _btn.duration).SetEase(_btn.curveX);
                    DOTweenEditorPreview.PrepareTweenForPreview(_tween, true, true, false);
                }
                else
                {
                    _tween = _btn.transform.DOScaleX(_btn.toScale.x, _btn.duration).SetEase(_btn.curveX);
                    DOTweenEditorPreview.PrepareTweenForPreview(_tween, true, true, false);
                    _tween2 = _btn.transform.DOScaleY(_btn.toScale.y, _btn.duration).SetEase(_btn.curveY);
                    DOTweenEditorPreview.PrepareTweenForPreview(_tween2, true, true, false);
                }

                DOTweenEditorPreview.Start();
            }

            if (GUILayout.Button("■ Stop"))
            {
                if (!DOTweenEditorPreview.isPreviewing)
                {
                    return;
                }

                RemoveTween();

                DOTweenPreviewManager.StopAllPreviews();
                _btn.transform.localScale = _btn.fromScale;
            }

            EditorGUILayout.EndHorizontal();

            EditorScriptor.EndContentBox();
        }

        private void RemoveTween()
        {
            if (_tween != null)
            {
                _tween.Complete();
                _tween.Kill();
                _tween = null;
            }

            if (_tween2 != null)
            {
                _tween2.Complete();
                _tween2.Kill();
                _tween2 = null;
            }
        }

        private void AnimationSetting()
        {
            EditorScriptor.BeginContentBox();

            //ViewPropertyField("IsOn","Interactable");
            ViewPropertyField("isAnimation", "애니메이션 설정");
            EditorGUILayout.LabelField("[효과음]", EditorStyles.boldLabel);
            ViewPropertyField("isSound", "효과음 설정");

            if (_btn.isSound)
            {
                ViewPropertyField("sfxType", "효과음");
            }

            EditorScriptor.EditorBeginDisabledGroup();

            if (_btn.isAnimation)
            {
                GUILayout.Space(20);

                EditorScriptor.TweenBeginDisabledGroup();

                EditorGUILayout.LabelField("[애니메이션]", EditorStyles.boldLabel);
                ViewPropertyField("isRepetition", "반복 설정");
                ViewPropertyField("duration", "Duration");
                ViewPropertyField("isUniformCurve", "UniformCurve");
                ViewPropertyField("fromScale", "FromScale");
                ViewPropertyField("toScale", "ToScale");

                if (_btn.isUniformCurve)
                {
                    ViewPropertyField("curveX", "Curve");
                }
                else
                {
                    ViewPropertyField("curveX", "CurveX");
                    ViewPropertyField("curveY", "CurveY");
                }

                EditorScriptor.TweenEndDisabledGroup();

                GUILayout.Space(10);
                PreviewAnimation();
            }

            EditorScriptor.EditorEndDisabledGroup();

            GUILayout.Space(10);

            EditorScriptor.TweenBeginDisabledGroup();

            ViewPropertyField("buttonExHelpers");

            if (GUILayout.Button("ButtonHelpCaching"))
            {
                foreach (Button btnEx in targets.OfType<Button>())
                {
                    if (btnEx.buttonExHelpers != null)
                    {
                        btnEx.buttonExHelpers.Clear();
                    }
                    else
                    {
                        btnEx.buttonExHelpers = new List<ButtonHelper>();
                    }

                    ButtonHelper[] helpers = btnEx.GetComponentsInChildren<ButtonHelper>();
                    foreach (ButtonHelper helper in helpers)
                    {
                        btnEx.buttonExHelpers.Add(helper);
                    }
                }
            }

            EditorScriptor.TweenEndDisabledGroup();

            EditorScriptor.EndContentBox();
        }

        #endregion

        #region lifecycle

        private void OnDisable()
        {
            if (DOTweenEditorPreview.isPreviewing)
            {
                RemoveTween();
                DOTweenPreviewManager.StopAllPreviews();

                _btn.transform.localScale = _btn.fromScale;
            }
        }

        private void ViewPropertyField(string value, string label = "")
        {
            if (!_dic.ContainsKey(value))
            {
                _dic.Add(value, serializedObject.FindProperty(value));
            }

            if (string.IsNullOrEmpty(label))
            {
                EditorGUILayout.PropertyField(_dic[value]);
            }
            else
            {
                EditorGUILayout.PropertyField(_dic[value], new GUIContent(label));
            }
        }

        private void OnEnable()
        {
            _dic.Clear();
        }

        public override void OnInspectorGUI()
        {
            _btn = target as Button;
            _btn.transition = Selectable.Transition.None;

            GUI.changed = false;
            AnimationSetting();
            ViewPropertyField("isAlwaysClickEvent", "isOn 상관없이 버튼 이벤트 호출 ");
            ViewPropertyField("onClick");

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_btn);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
