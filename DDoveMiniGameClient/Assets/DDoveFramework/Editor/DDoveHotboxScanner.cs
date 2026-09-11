using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace DDoveFramework.Editor
{
    public readonly struct DDoveHotboxEntry
    {
        public DDoveHotboxEntry(string id, string label, string group, string tooltip, Action execute)
        {
            Id = id;
            Label = label;
            Group = group;
            Tooltip = tooltip;
            Execute = execute;
        }

        public string Id { get; }

        public string Label { get; }

        public string Group { get; }

        public string Tooltip { get; }

        public Action Execute { get; }
    }

    public static class DDoveHotboxScanner
    {
        public const string ConfigAssetPath = "Assets/DDoveFramework/Editor/DDoveHotboxConfig.asset";

        public static List<DDoveHotboxEntry> ScanAll()
        {
            var list = new List<DDoveHotboxEntry>();
            foreach (var method in TypeCache.GetMethodsWithAttribute<DDoveHotboxEntryAttribute>())
            {
                if (!method.IsStatic || method.ContainsGenericParameters || method.GetParameters().Length != 0)
                {
                    continue;
                }

                var attribute = method.GetCustomAttribute<DDoveHotboxEntryAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var id = method.DeclaringType?.FullName + "::" + method.Name;
                list.Add(new DDoveHotboxEntry(
                    id,
                    attribute.Label,
                    attribute.Group,
                    attribute.Tooltip,
                    () => method.Invoke(null, null)));
            }

            list.Sort((left, right) =>
            {
                var group = string.CompareOrdinal(left.Group, right.Group);
                return group != 0 ? group : string.CompareOrdinal(left.Label, right.Label);
            });
            return list;
        }

        public static bool TryFind(string id, out DDoveHotboxEntry entry)
        {
            foreach (var item in ScanAll())
            {
                if (item.Id == id)
                {
                    entry = item;
                    return true;
                }
            }

            entry = default;
            return false;
        }

        public static DDoveHotboxConfig GetOrCreateConfig()
        {
            var config = AssetDatabase.LoadAssetAtPath<DDoveHotboxConfig>(ConfigAssetPath);
            if (config != null)
            {
                return config;
            }

            config = UnityEngine.ScriptableObject.CreateInstance<DDoveHotboxConfig>();
            Seed(config);
            AssetDatabase.CreateAsset(config, ConfigAssetPath);
            AssetDatabase.SaveAssets();
            return config;
        }

        public static bool HasSlots(DDoveHotboxConfig config)
        {
            if (config == null)
            {
                return false;
            }

            foreach (var ring in config.Rings)
            {
                if (ring.Slots.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static void Seed(DDoveHotboxConfig config)
        {
            var ui = new DDoveHotboxRing { Name = "UI" };
            ui.Slots.Add(new DDoveHotboxSlot
            {
                EntryId = "DDoveFramework.Extension.DDoveUI.Editor.DDoveUISceneCreateWindow::Open"
            });
            ui.Slots.Add(new DDoveHotboxSlot
            {
                EntryId = "DDoveFramework.Extension.DDoveUI.Editor.DDoveUISceneEditor::LocatePrefab"
            });
            ui.Slots.Add(new DDoveHotboxSlot
            {
                EntryId = "DDoveFramework.Extension.DDoveUI.Editor.DDoveUISceneEditor::BindSelectedNodes"
            });
            ui.Slots.Add(new DDoveHotboxSlot
            {
                EntryId = "DDoveFramework.Extension.DDoveUI.Editor.DDoveUIPanelExporter::StartAutoBindExport"
            });
            config.Rings.Add(ui);

            var architecture = new DDoveHotboxRing { Name = "架构" };
            foreach (var entry in ScanAll())
            {
                if (entry.Group == DDoveHotboxEntryAttribute.ArchitectureGroup)
                {
                    architecture.Slots.Add(new DDoveHotboxSlot { EntryId = entry.Id });
                }
            }

            config.Rings.Add(architecture);
        }
    }
}
