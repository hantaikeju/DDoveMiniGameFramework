using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    internal static class DDoveUIPrefabBinder
    {
        public static void BindAndExport(string panelName)
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            var exportRoot = GameObject.Find(info.ExportRootName);
            if (exportRoot == null)
            {
                Debug.LogError($"[DDoveUI] bind failed: missing {info.ExportRootName}");
                return;
            }

            var desc = UnityEngine.Object.FindFirstObjectByType<DDoveUIPanelDescription>();
            var ns = desc != null && !string.IsNullOrEmpty(desc.Namespace) ? desc.Namespace : info.NamespaceName;
            var stage = desc != null && !string.IsNullOrEmpty(desc.StageName) ? desc.StageName : "Start";
            var type = FindType(ns + "." + panelName);
            if (type == null)
            {
                Debug.LogError($"[DDoveUI] bind failed: type {ns}.{panelName} not found");
                return;
            }

            var component = exportRoot.GetComponent(type) ?? exportRoot.AddComponent(type);
            foreach (var bind in exportRoot.GetComponentsInChildren<DDoveUINodeBind>(true))
            {
                var field = type.GetField(bind.GetMemberName(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    continue;
                }

                var target = bind.GetComponent(ToComponentType(bind.ComponentType));
                if (target != null)
                {
                    field.SetValue(component, target);
                }
            }

            EditorUtility.SetDirty(component);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            DDoveUIPrefabExporter.Save(exportRoot, stage, panelName);
        }

        private static Type FindType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                {
                    continue;
                }

                var type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private static Type ToComponentType(DDoveUINodeBindType bindType)
        {
            return bindType switch
            {
                DDoveUINodeBindType.Image => typeof(Image),
                DDoveUINodeBindType.Text => typeof(Text),
                DDoveUINodeBindType.Button => typeof(Button),
                DDoveUINodeBindType.TextMeshProUGUI => typeof(TextMeshProUGUI),
                _ => typeof(RectTransform)
            };
        }
    }
}
