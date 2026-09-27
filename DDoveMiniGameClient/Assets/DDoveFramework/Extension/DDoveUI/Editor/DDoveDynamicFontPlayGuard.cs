using UnityEditor;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    [InitializeOnLoad]
    static class DDoveDynamicFontPlayGuard
    {
        static DDoveDynamicFontPlayGuard()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode)
            {
                return;
            }

            var path = SessionState.GetString(DDoveUIKit.RebuiltDynamicFontSessionKey, string.Empty);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            SessionState.EraseString(DDoveUIKit.RebuiltDynamicFontSessionKey);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
