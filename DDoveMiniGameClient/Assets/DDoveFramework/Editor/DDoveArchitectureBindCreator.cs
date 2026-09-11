using System;
using System.IO;
using System.Text;
using DDoveFramework.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DDoveFramework.Editor
{
    internal static class DDoveArchitectureBindCreator
    {
        private const string LastModuleKey = "DDove.Architecture.LastModule";

        [DDoveHotboxEntry("创建 Model", DDoveHotboxEntryAttribute.ArchitectureGroup, "在配置路径下按模块生成带 [DDoveBindModel] 的空 Model")]
        public static void CreateModel()
        {
            DDoveArchitectureCreateDialog.Open("Model", false);
        }

        [DDoveHotboxEntry("创建 System", DDoveHotboxEntryAttribute.ArchitectureGroup, "在配置路径下按模块生成带 [DDoveBindSystem] 的空 System")]
        public static void CreateSystem()
        {
            DDoveArchitectureCreateDialog.Open("System", false);
        }

        [DDoveHotboxEntry("创建 Utility", DDoveHotboxEntryAttribute.ArchitectureGroup, "在配置路径下按模块生成带 [DDoveBindUtility] 的空 Utility")]
        public static void CreateUtility()
        {
            DDoveArchitectureCreateDialog.Open("Utility", true);
        }

        public static string ClassNameFor(string name, string role)
        {
            if (string.IsNullOrEmpty(name))
            {
                return role;
            }

            return name.EndsWith(role, StringComparison.Ordinal) ? name : name + role;
        }

        public static bool TryCreate(string role, string name, string module, string asType)
        {
            if (!IsIdentifier(name))
            {
                DDoveDebug.LogError("Architecture", ("reason", "name is not a valid identifier"), ("name", name));
                return false;
            }

            if (!IsIdentifier(module))
            {
                DDoveDebug.LogError("Architecture", ("reason", "module is not a valid identifier"), ("module", module));
                return false;
            }

            if (!string.IsNullOrEmpty(asType) && !IsTypeName(asType))
            {
                DDoveDebug.LogError("Architecture", ("reason", "As is not a valid type name"), ("as", asType));
                return false;
            }

            var className = ClassNameFor(name, role);
            var config = DDoveArchitectureBindConfig.GetOrCreate();
            var folder = config.PathFor(role).TrimEnd('/') + "/" + module;
            var path = folder + "/" + className + ".cs";
            if (File.Exists(path))
            {
                DDoveDebug.LogError("Architecture", ("reason", "file already exists"), ("path", path));
                return false;
            }

            Directory.CreateDirectory(folder);
            File.WriteAllText(path, Render(role, className, asType), Encoding.UTF8);
            EditorPrefs.SetString(LastModuleKey, module);
            DDoveArchitectureBindGenerator.QueueGenerateAfterCompile();
            AssetDatabase.Refresh();
            return true;
        }

        public static string LastModule()
        {
            return EditorPrefs.GetString(LastModuleKey, string.Empty);
        }

        private static string Render(string role, string className, string asType)
        {
            var sb = new StringBuilder(256);
            sb.AppendLine("using DDoveFramework.Core;");
            sb.AppendLine();
            sb.AppendLine("namespace Game");
            sb.AppendLine("{");
            switch (role)
            {
                case "Model":
                    sb.AppendLine("    [DDoveBindModel]");
                    sb.Append("    public sealed class ").Append(className).AppendLine(" : AbstractModel");
                    sb.AppendLine("    {");
                    sb.AppendLine("        protected override void OnInit()");
                    sb.AppendLine("        {");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    break;
                case "System":
                    sb.AppendLine("    [DDoveBindSystem]");
                    sb.Append("    public sealed class ").Append(className).AppendLine(" : AbstractSystem");
                    sb.AppendLine("    {");
                    sb.AppendLine("        protected override void OnInit()");
                    sb.AppendLine("        {");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    break;
                default:
                    if (string.IsNullOrEmpty(asType))
                    {
                        sb.AppendLine("    [DDoveBindUtility]");
                    }
                    else
                    {
                        sb.Append("    [DDoveBindUtility(As = typeof(").Append(asType).AppendLine("))]");
                    }

                    sb.Append("    public sealed class ").Append(className).AppendLine(" : IUtility");
                    sb.AppendLine("    {");
                    sb.AppendLine("    }");
                    break;
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        internal static bool IsIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name) || !char.IsLetter(name[0]) && name[0] != '_')
            {
                return false;
            }

            for (var i = 1; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsTypeName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var parts = name.Split('.');
            for (var i = 0; i < parts.Length; i++)
            {
                if (!IsIdentifier(parts[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }

    internal sealed class DDoveArchitectureCreateDialog : EditorWindow
    {
        private string _role;
        private bool _askAs;
        private string _name = string.Empty;
        private string _module = string.Empty;
        private string _asType = string.Empty;

        public static void Open(string role, bool askAs)
        {
            var width = 440f;
            var height = askAs ? 220f : 192f;
            var window = CreateInstance<DDoveArchitectureCreateDialog>();
            window._role = role;
            window._askAs = askAs;
            window._module = DDoveArchitectureBindCreator.LastModule();
            window.titleContent = new GUIContent("创建 " + role);
            window.minSize = new Vector2(width, height);
            window.maxSize = new Vector2(width, height);
            window.position = DDoveEditorUi.Centered(width, height);
            window.ShowModalUtility();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            DDoveEditorUi.ApplySheet(root);
            root.style.paddingLeft = 12;
            root.style.paddingRight = 12;
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;

            var nameField = new TextField(_role + "Name") { value = _name };
            var moduleField = new TextField("模块") { value = _module };
            var classField = new TextField("类名") { value = PreviewClass(), isReadOnly = true };
            nameField.AddToClassList("ddove-field");
            moduleField.AddToClassList("ddove-field");
            classField.AddToClassList("ddove-field");
            nameField.RegisterValueChangedCallback(evt =>
            {
                _name = evt.newValue;
                classField.SetValueWithoutNotify(PreviewClass());
            });
            moduleField.RegisterValueChangedCallback(evt => _module = evt.newValue);

            root.Add(nameField);
            root.Add(moduleField);
            root.Add(classField);

            TextField asField = null;
            if (_askAs)
            {
                asField = new TextField("As（可选）") { value = _asType };
                asField.AddToClassList("ddove-field");
                asField.RegisterValueChangedCallback(evt => _asType = evt.newValue);
                root.Add(asField);
            }

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.FlexEnd;
            row.style.marginTop = 12;

            var cancel = new Button(Close) { text = "取消" };
            cancel.style.width = 72;
            var create = new Button(() =>
            {
                if (DDoveArchitectureBindCreator.TryCreate(
                        _role,
                        _name.Trim(),
                        _module.Trim(),
                        _askAs ? _asType.Trim() : string.Empty))
                {
                    Close();
                }
            })
            {
                text = "创建"
            };
            create.style.width = 72;
            row.Add(cancel);
            row.Add(create);
            root.Add(row);
        }

        private string PreviewClass()
        {
            return DDoveArchitectureBindCreator.ClassNameFor(_name.Trim(), _role);
        }
    }
}
