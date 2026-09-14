using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DDoveFramework.Editor
{
    [InitializeOnLoad]
    internal static class DDovePlayStartScene
    {
        private const string BootScenePath = "Assets/Scenes/DDoveBoot.unity";

        static DDovePlayStartScene()
        {
            var boot = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScenePath);
            if (boot == null)
            {
                Debug.LogError($"[DDoveBoot] missing start scene: {BootScenePath}");
                return;
            }

            EditorSceneManager.playModeStartScene = boot;
        }
    }
}
