using UnityEditor;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    public static class DDoveUIPrefabExporter
    {
        public static bool Save(GameObject exportRoot, string stageName, string panelName)
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            var prefabPath = info.GetPrefabPath(stageName, panelName);
            var folder = prefabPath.Substring(0, prefabPath.LastIndexOf('/'));
            DDoveUIEditorPaths.EnsureFolder(folder);

            DestroyUnsavedDescendants(exportRoot);

            var saved = PrefabUtility.SaveAsPrefabAsset(exportRoot, prefabPath, out var ok);
            if (!ok || saved == null)
            {
                Debug.LogError($"[DDoveUI] save prefab failed: {prefabPath}");
                return false;
            }

            var contents = PrefabUtility.LoadPrefabContents(prefabPath);
            DestroyUnsavedDescendants(contents);
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

        static void DestroyUnsavedDescendants(GameObject root)
        {
            if (root == null)
            {
                return;
            }

            var transforms = root.GetComponentsInChildren<Transform>(true);
            for (var i = transforms.Length - 1; i >= 0; i--)
            {
                var t = transforms[i];
                if (t == null)
                {
                    continue;
                }

                var go = t.gameObject;
                if (go == null || go == root || !HasUnsavedHideFlags(go.hideFlags))
                {
                    continue;
                }

                Object.DestroyImmediate(go);
            }
        }

        static bool HasUnsavedHideFlags(HideFlags flags)
        {
            return (flags & HideFlags.DontSave) == HideFlags.DontSave
                || (flags & HideFlags.HideAndDontSave) == HideFlags.HideAndDontSave;
        }
    }
}
