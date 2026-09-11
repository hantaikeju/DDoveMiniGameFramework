using UnityEditor;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    internal static class DDoveUIPrefabExporter
    {
        public static bool Save(GameObject exportRoot, string stageName, string panelName)
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            var prefabPath = info.GetPrefabPath(stageName, panelName);
            var folder = prefabPath.Substring(0, prefabPath.LastIndexOf('/'));
            DDoveUIEditorPaths.EnsureFolder(folder);

            var saved = PrefabUtility.SaveAsPrefabAsset(exportRoot, prefabPath, out var ok);
            if (!ok || saved == null)
            {
                Debug.LogError($"[DDoveUI] save prefab failed: {prefabPath}");
                return false;
            }

            var contents = PrefabUtility.LoadPrefabContents(prefabPath);
            var binds = contents.GetComponentsInChildren<DDoveUINodeBind>(true);
            for (var i = binds.Length - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(binds[i]);
            }

            PrefabUtility.SaveAsPrefabAsset(contents, prefabPath, out var cleaned);
            PrefabUtility.UnloadPrefabContents(contents);
            AssetDatabase.Refresh();
            return cleaned;
        }
    }
}
