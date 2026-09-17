using System.Collections.Generic;
using System.Text.RegularExpressions;
using DDoveFramework.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    public static class DDoveUIPanelExporter
    {
        private const string PendingKey = "DDoveUI.AutoBind";
        private const string PendingPanelKey = "DDoveUI.PendingPanel";

        public static void ExportPrefabOnly()
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            var desc = Object.FindFirstObjectByType<DDoveUIPanelDescription>();
            var exportRoot = GameObject.Find(info.ExportRootName);
            if (desc == null || exportRoot == null)
            {
                EditorUtility.DisplayDialog("DDoveUI", "当前场景没有 PanelDescription 或 UIRoot。", "确定");
                return;
            }

            if (DDoveUIPrefabExporter.Save(exportRoot, desc.StageName, EditorSceneManager.GetActiveScene().name))
            {
                EditorUtility.DisplayDialog("DDoveUI", "Prefab 已导出。", "确定");
            }
        }

        [DDoveHotboxEntry("导出 UI", "UI 制作", "生成 UI 绑定代码并导出 Prefab")]
        public static void StartAutoBindExport()
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            var desc = Object.FindFirstObjectByType<DDoveUIPanelDescription>();
            var exportRoot = GameObject.Find(info.ExportRootName);
            if (desc == null || exportRoot == null)
            {
                EditorUtility.DisplayDialog("DDoveUI", "当前场景没有 PanelDescription 或 UIRoot。", "确定");
                return;
            }

            var panelName = EditorSceneManager.GetActiveScene().name;
            var members = new List<object>();
            var used = new HashSet<string>();
            foreach (var bind in exportRoot.GetComponentsInChildren<DDoveUINodeBind>(true))
            {
                var name = bind.GetMemberName();
                if (!IsValidName(name, out var error) || !used.Add(name))
                {
                    EditorUtility.DisplayDialog("DDoveUI", $"绑定名非法或重复：{name}\n{error}", "确定");
                    return;
                }

                members.Add(new { name, type = TypeName(bind.ComponentType) });
            }

            if (!DDoveUIPanelCodeGenerator.Generate(panelName, desc.StageName, desc.PanelKind, members))
            {
                return;
            }

            EditorPrefs.SetBool(PendingKey, true);
            EditorPrefs.SetString(PendingPanelKey, panelName);
            AssetDatabase.Refresh();
            if (!EditorApplication.isCompiling)
            {
                OnScriptsReloaded();
            }
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (!EditorPrefs.GetBool(PendingKey, false))
            {
                return;
            }

            EditorPrefs.SetBool(PendingKey, false);
            var panelName = EditorPrefs.GetString(PendingPanelKey, string.Empty);
            if (string.IsNullOrEmpty(panelName))
            {
                return;
            }

            EditorApplication.delayCall += () => WaitThenBind(panelName);
        }

        private static void WaitThenBind(string panelName)
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += () => WaitThenBind(panelName);
                return;
            }

            DDoveUIPrefabBinder.BindAndExport(panelName);
        }

        private static bool IsValidName(string name, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                error = "空名称";
                return false;
            }

            if (!Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                error = "只能是字母数字下划线，且不能以数字开头";
                return false;
            }

            return true;
        }

        private static string TypeName(DDoveUINodeBindType bindType)
        {
            return bindType switch
            {
                DDoveUINodeBindType.Image => "UnityEngine.UI.Image",
                DDoveUINodeBindType.Button => "UnityEngine.UI.Button",
                DDoveUINodeBindType.TextMeshProUGUI => "TMPro.TextMeshProUGUI",
                _ => "UnityEngine.RectTransform"
            };
        }
    }
}
