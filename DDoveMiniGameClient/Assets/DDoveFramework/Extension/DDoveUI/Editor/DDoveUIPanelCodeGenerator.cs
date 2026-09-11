using System;
using System.Collections.Generic;
using System.IO;
using Scriban;
using UnityEditor;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    internal static class DDoveUIPanelCodeGenerator
    {
        public static bool Generate(string panelName, string stageName, DDoveUIPanelKind kind, IReadOnlyList<object> members)
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            var ns = info.NamespaceName;
            var baseClass = kind == DDoveUIPanelKind.Popup
                ? $"DDoveUIPopupPanelBase<{panelName}>"
                : $"DDoveUIPanelBase<{panelName}>";

            try
            {
                DDoveUIEditorPaths.EnsureFolder(info.BindScriptsPath);
                WriteAlways(
                    Path.Combine(info.BindScriptsPath, panelName + ".Generated.cs").Replace("\\", "/"),
                    Render("PanelGenerated.sbn", new
                    {
                        namespace_name = ns,
                        class_name = panelName,
                        members
                    }));

                WriteAlways(
                    Path.Combine(info.BindScriptsPath, panelName + ".IController.Generated.cs").Replace("\\", "/"),
                    Render("PanelController.sbn", new
                    {
                        namespace_name = ns,
                        class_name = panelName
                    }));

                var logicDir = Path.Combine(info.LogicScriptsPath, stageName).Replace("\\", "/");
                DDoveUIEditorPaths.EnsureFolder(logicDir);
                var logicPath = Path.Combine(logicDir, panelName + ".cs").Replace("\\", "/");
                if (!File.Exists(logicPath))
                {
                    File.WriteAllText(logicPath, Render("PanelLogic.sbn", new
                    {
                        namespace_name = ns,
                        class_name = panelName,
                        base_class = baseClass,
                        stage_name = stageName
                    }));
                }

                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DDoveUI] generate failed: {e.Message}");
                return false;
            }
        }

        private static void WriteAlways(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        private static string Render(string templateFile, object model)
        {
            var path = DDoveUIEditorPaths.TemplatePath(templateFile);
            var text = File.ReadAllText(path);
            var template = Template.Parse(text);
            if (template.HasErrors)
            {
                throw new InvalidOperationException(template.Messages.ToString());
            }

            return template.Render(model, member => member.Name);
        }
    }
}
