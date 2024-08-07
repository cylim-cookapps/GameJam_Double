using System.Collections;
using System.Collections.Generic;
using DG.DOTweenEditor;
using UnityEditor;
using UnityEngine;

namespace Pxp
{
    public class EditorScriptor : Editor
    {
        public static void BeginContentBox()
        {
            EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(2f);
        }

        public static void EndContentBox()
        {
            EditorGUILayout.Space(3f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        public static void TweenBeginDisabledGroup()
        {
            if (DOTweenEditorPreview.isPreviewing )
                EditorGUI.BeginDisabledGroup(true);
        }

        public static void TweenEndDisabledGroup()
        {
            if (DOTweenEditorPreview.isPreviewing)
                EditorGUI.EndDisabledGroup();
        }

        public static void EditorBeginDisabledGroup()
        {
            if (EditorApplication.isPlaying)
                EditorGUI.BeginDisabledGroup(true);
        }

        public static void EditorEndDisabledGroup()
        {
            if (EditorApplication.isPlaying)
                EditorGUI.EndDisabledGroup();
        }
    }
}
