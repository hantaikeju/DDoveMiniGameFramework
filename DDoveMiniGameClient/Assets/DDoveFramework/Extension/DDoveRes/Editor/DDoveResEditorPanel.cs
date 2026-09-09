using DDoveFramework.Editor;
using UnityEditor;
using UnityEngine.UIElements;
using YooAsset.Editor;

namespace DDoveFramework.Extension.DDoveRes.Editor
{
    [DDoveEditorPanel("res", "Res", 100)]
    public sealed class DDoveResEditorPanel : IDDoveEditorPanel
    {
        public void Build(VisualElement root)
        {
            var packageCard = new VisualElement();
            packageCard.AddToClassList("ddove-card");
            packageCard.Add(Title("YooAsset Package Info"));

            var packages = BundleCollectorSettingData.HasSettingAsset()
                ? BundleCollectorSettingData.Setting.Packages
                : null;
            if (packages == null || packages.Count == 0)
            {
                packageCard.Add(Hint("暂未设置Package 请在Yooasset 中添加Package"));
            }
            else
            {
                foreach (var package in packages)
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

                    packageCard.Add(row);
                }
            }

            packageCard.Add(new Button(OpenYooCollector) { text = "Yoo Collector" });
            root.Add(packageCard);
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
