using Cysharp.Threading.Tasks;
using Pxp;
using UnityEngine;
using UnityEditor;

public class EditorPlayModeStateChanged
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void OnSubsystemRegistration()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            UserManager.Inst.Save().Forget();
        }
    }
#endif
}
