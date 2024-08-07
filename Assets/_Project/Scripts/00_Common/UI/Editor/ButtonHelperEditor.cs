using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Pxp
{
    [InitializeOnLoad]
    [CustomEditor(typeof(ButtonHelper))]
    [CanEditMultipleObjects]
    public class ButtonHelperEditor : Editor
    {

        private ButtonHelper _btnHelper;
        private SerializedProperty _deInteractableSprite;
        private SerializedProperty _deInteractableColor;

        public override void OnInspectorGUI()
        {
            _btnHelper = target as ButtonHelper;
            GUI.changed = false;

            _btnHelper.type = (eButtonHelperType) EditorGUILayout.EnumFlagsField("HelpType", _btnHelper.type);

            if (_btnHelper.type.IsFlagSet(eButtonHelperType.Color))
            {
                EditorScriptor.BeginContentBox();
                EditorGUILayout.LabelField("[색상]", EditorStyles.boldLabel);
                _btnHelper.colorTarget = (Graphic) EditorGUILayout.ObjectField("타겟", _btnHelper.colorTarget, typeof(Graphic), true);
                EditorGUILayout.PropertyField(_deInteractableColor, new GUIContent("비활성화 색상"));
                EditorScriptor.EndContentBox();
            }

            if (_btnHelper.type.IsFlagSet(eButtonHelperType.Sprite))
            {
                EditorScriptor.BeginContentBox();
                EditorGUILayout.LabelField("[이미지]", EditorStyles.boldLabel);
                _btnHelper.imageTarget = (Image) EditorGUILayout.ObjectField("타겟", _btnHelper.imageTarget, typeof(Image), true);
                EditorGUILayout.PropertyField(_deInteractableSprite, new GUIContent("비활성화 이미지"));
                EditorScriptor.EndContentBox();
            }

            if (_btnHelper.type.IsFlagSet(eButtonHelperType.Text))
            {
                EditorScriptor.BeginContentBox();
                EditorGUILayout.LabelField("[텍스트]", EditorStyles.boldLabel);
                _btnHelper.text = (TextMeshProUGUI) EditorGUILayout.ObjectField("타겟", _btnHelper.text, typeof(TextMeshProUGUI), true);
                _btnHelper.interactableText = EditorGUILayout.TextField("활성화 텍스트", _btnHelper.interactableText);
                _btnHelper.deInteractableText = EditorGUILayout.TextField("비활성화 텍스트", _btnHelper.deInteractableText);
                _btnHelper.isLocalizeKey = EditorGUILayout.Toggle("로컬라이징 여부", _btnHelper.isLocalizeKey);

                EditorScriptor.EndContentBox();
            }

            if (_btnHelper.type.IsFlagSet(eButtonHelperType.GameObj))
            {
                EditorScriptor.BeginContentBox();
                EditorGUILayout.LabelField("[오브젝트]", EditorStyles.boldLabel);
                _btnHelper.obj = (GameObject) EditorGUILayout.ObjectField("타겟", _btnHelper.obj, typeof(GameObject), true);
                _btnHelper.inversionObj = EditorGUILayout.Toggle("오브젝트 활성화 반전", _btnHelper.inversionObj);

                EditorScriptor.EndContentBox();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_btnHelper);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _deInteractableSprite = serializedObject.FindProperty("deInteractableSprite");

            _deInteractableColor = serializedObject.FindProperty("deInteractableColor");
        }

    }
}
