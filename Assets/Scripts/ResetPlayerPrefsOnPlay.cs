using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ResetPlayerPrefsOnPlay : MonoBehaviour
{
    static ResetPlayerPrefsOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}