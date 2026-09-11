using System;
using System.Collections.Generic;
using DDoveFramework.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using YooAsset;
using YooAsset.Editor;

namespace DDoveFramework.Extension.DDoveRes.Editor
{
    [DDoveEditorPanel("res", "Res", 100)]
    public sealed class DDoveResEditorPanel : IDDoveEditorPanel
    {
        private const string InitInfoAssetPath =
            "Assets/DDoveFramework/Extension/DDoveRes/Resources/DDoveResInitInfo.asset";

        private readonly List<Toggle> _defaultToggles = new List<Toggle>();
        private VisualElement _host;

        public void Build(VisualElement root)
        {
            _host = root;
            _defaultToggles.Clear();
            root.Clear();

            var info = GetOrCreateInitInfo();
            var packageCard = new VisualElement();
            packageCard.AddToClassList("ddove-card");
            packageCard.Add(CreateCardHeader());

            var packages = BundleCollectorSettingData.HasSettingAsset()
                ? BundleCollectorSettingData.Setting.Packages
                : null;
            if (packages == null || packages.Count == 0)
            {
                packageCard.Add(Hint("暂未设置Package 请在Yooasset 中添加Package"));
            }
            else
            {
                EnsureDefaultPackage(info, packages);
                foreach (var package in packages)
                {
                    packageCard.Add(CreatePackageRow(package, info));
                }
            }

            packageCard.Add(CreatePlayModeField(info));
            packageCard.Add(CreateLaunchSceneField(info));
            packageCard.Add(ActionButton("Yoo Collector", OpenYooCollector));
            root.Add(packageCard);
        }

        private VisualElement CreateCardHeader()
        {
            var header = new VisualElement();
            header.AddToClassList("ddove-row");
            header.style.marginTop = 0;
            header.style.justifyContent = Justify.SpaceBetween;
            header.Add(Title("YooAsset Package Info"));
            header.Add(ActionButton("刷新", Refresh));
            return header;
        }

        private static Button ActionButton(string text, Action onClick)
        {
            var button = new Button(onClick) { text = text };
            button.style.height = 22;
            button.style.minWidth = 88;
            button.style.marginTop = 8;
            button.style.marginRight = 6;
            return button;
        }

        private void Refresh()
        {
            if (_host == null)
            {
                return;
            }

            Build(_host);
        }

        private VisualElement CreatePackageRow(BundleCollectorPackage package, DDoveResInitInfo info)
        {
            var row = new VisualElement();
            row.AddToClassList("ddove-card-plate");

            var name = new Label(package.PackageName);
            name.AddToClassList("ddove-card-title");
            name.style.marginBottom = 0;
            row.Add(name);
            if (!string.IsNullOrEmpty(package.PackageDesc))
            {
                row.Add(Hint(package.PackageDesc));
            }

            var toggle = new Toggle("默认包")
            {
                value = info.PackageName == package.PackageName
            };
            toggle.AddToClassList("ddove-row");
            toggle.RegisterValueChangedCallback(evt => OnDefaultToggled(package.PackageName, evt.newValue, toggle, info));
            _defaultToggles.Add(toggle);
            row.Add(toggle);
            return row;
        }

        private void OnDefaultToggled(string packageName, bool isDefault, Toggle source, DDoveResInitInfo info)
        {
            if (!isDefault)
            {
                if (info.PackageName == packageName)
                {
                    source.SetValueWithoutNotify(true);
                }

                return;
            }

            SetInitField(info, "_packageName", packageName);
            foreach (var toggle in _defaultToggles)
            {
                if (toggle != source)
                {
                    toggle.SetValueWithoutNotify(false);
                }
            }
        }

        private static VisualElement CreatePlayModeField(DDoveResInitInfo info)
        {
            var field = new EnumField("加载模式", info.PlayMode);
            field.AddToClassList("ddove-row");
            field.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is EPlayMode playMode)
                {
                    SetInitField(info, "_playMode", playMode);
                }
            });
            return field;
        }

        private static VisualElement CreateLaunchSceneField(DDoveResInitInfo info)
        {
            var location = string.IsNullOrEmpty(info.LaunchSceneLocation)
                ? DDoveResKit.FallbackLaunchSceneLocation
                : info.LaunchSceneLocation;
            var field = new TextField("启动场景") { value = location };
            field.AddToClassList("ddove-row");
            field.RegisterValueChangedCallback(evt =>
            {
                var value = string.IsNullOrWhiteSpace(evt.newValue)
                    ? DDoveResKit.FallbackLaunchSceneLocation
                    : evt.newValue.Trim();
                if (value != evt.newValue)
                {
                    field.SetValueWithoutNotify(value);
                }

                SetInitField(info, "_launchSceneLocation", value);
            });
            return field;
        }

        private static void EnsureDefaultPackage(DDoveResInitInfo info, List<BundleCollectorPackage> packages)
        {
            foreach (var package in packages)
            {
                if (package.PackageName == info.PackageName)
                {
                    return;
                }
            }

            SetInitField(info, "_packageName", packages[0].PackageName);
        }

        private static DDoveResInitInfo GetOrCreateInitInfo()
        {
            var info = AssetDatabase.LoadAssetAtPath<DDoveResInitInfo>(InitInfoAssetPath);
            if (info != null)
            {
                return info;
            }

            var folder = "Assets/DDoveFramework/Extension/DDoveRes/Resources";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets/DDoveFramework/Extension/DDoveRes", "Resources");
            }

            info = ScriptableObject.CreateInstance<DDoveResInitInfo>();
            AssetDatabase.CreateAsset(info, InitInfoAssetPath);
            AssetDatabase.SaveAssets();
            return info;
        }

        private static void SetInitField(DDoveResInitInfo info, string property, string value)
        {
            var so = new SerializedObject(info);
            so.FindProperty(property).stringValue = value;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(info);
            AssetDatabase.SaveAssets();
        }

        private static void SetInitField(DDoveResInitInfo info, string property, EPlayMode value)
        {
            var so = new SerializedObject(info);
            so.FindProperty(property).enumValueIndex = (int)value;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(info);
            AssetDatabase.SaveAssets();
        }

        private static Label Title(string text)
        {
            var label = new Label(text);
            label.AddToClassList("ddove-card-title");
            return label;
        }

        private static Label Hint(string text)
        {
            var label = new Label(text);
            label.AddToClassList("ddove-hint");
            label.style.marginBottom = 8;
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        }

        private static void OpenYooCollector()
        {
            EditorApplication.ExecuteMenuItem("YooAsset/Bundle Collector");
        }
    }
}