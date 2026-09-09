using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DDoveFramework.Editor
{
    public sealed class DDoveEditorWindow : EditorWindow
    {
        private const string PanelPrefsKey = "DDove.Editor.SelectedPanel";
        private const string NavCollapsedPrefsKey = "DDove.Editor.NavCollapsed";
        private const float NavWidth = 220f;

        private List<DDoveEditorPanelEntry> _panels;
        private readonly List<VisualElement> _navItems = new List<VisualElement>();
        private VisualElement _nav;
        private VisualElement _content;
        private Button _toggle;
        private int _selectedIndex = -1;
        private bool _navCollapsed;

        [MenuItem("DDove/Editor")]
        public static void Open()
        {
            var window = GetWindow<DDoveEditorWindow>();
            window.titleContent = new GUIContent("DDove");
            window.minSize = new Vector2(880f, 520f);
        }

        private void CreateGUI()
        {
            _panels = DDoveEditorRegistry.Collect();
            _navItems.Clear();
            _selectedIndex = -1;
            _navCollapsed = EditorPrefs.GetBool(NavCollapsedPrefsKey, false);

            var root = rootVisualElement;
            root.Clear();
            root.style.flexGrow = 1;
            root.style.flexDirection = FlexDirection.Column;

            var folder = EditorFolder();
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{folder}/DDoveEditorWindow.uxml");
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{folder}/DDoveEditorWindow.uss");
            if (uxml == null || uss == null)
            {
                root.Add(new HelpBox("找不到 DDoveEditorWindow.uxml / .uss。等 Unity 导入后再开一次。", HelpBoxMessageType.Error));
                return;
            }

            uxml.CloneTree(root);
            root.styleSheets.Add(uss);

            _toggle = root.Q<Button>("Toggle");
            _nav = root.Q<VisualElement>("Nav");
            _content = root.Q<ScrollView>("Content");

            SetupToggle();
            ApplyNavCollapsed(false);
            BuildNav();

            if (_panels.Count == 0)
            {
                ShowEmpty();
                return;
            }

            Select(FindInitialIndex());
        }

        private void SetupToggle()
        {
            _toggle.text = string.Empty;
            _toggle.Clear();
            var frame = new VisualElement();
            frame.AddToClassList("ddove-toggle__frame");
            var bar = new VisualElement();
            bar.AddToClassList("ddove-toggle__bar");
            frame.Add(bar);
            _toggle.Add(frame);
            _toggle.clicked += ToggleNav;
        }

        private void ToggleNav()
        {
            _navCollapsed = !_navCollapsed;
            EditorPrefs.SetBool(NavCollapsedPrefsKey, _navCollapsed);
            ApplyNavCollapsed(true);
        }

        private void ApplyNavCollapsed(bool animate)
        {
            if (!animate)
            {
                _nav.style.transitionDuration = new List<TimeValue> { new TimeValue(0f) };
            }

            _nav.EnableInClassList("ddove-nav--collapsed", _navCollapsed);
            _nav.style.width = _navCollapsed ? 0f : NavWidth;
            _nav.style.minWidth = _navCollapsed ? 0f : NavWidth;
            _nav.style.maxWidth = _navCollapsed ? 0f : NavWidth;

            if (!animate)
            {
                _nav.schedule.Execute(() =>
                {
                    _nav.style.transitionDuration = StyleKeyword.Null;
                });
            }
        }

        private void BuildNav()
        {
            _nav.Clear();
            var group = new Label("总览");
            group.AddToClassList("ddove-nav-group");
            _nav.Add(group);

            for (var i = 0; i < _panels.Count; i++)
            {
                var index = i;
                var item = new VisualElement();
                item.AddToClassList("ddove-nav-item");
                item.pickingMode = PickingMode.Position;

                var label = new Label(_panels[i].Title);
                label.AddToClassList("ddove-nav-item__label");
                label.pickingMode = PickingMode.Ignore;
                item.Add(label);

                item.RegisterCallback<ClickEvent>(_ => Select(index));
                _nav.Add(item);
                _navItems.Add(item);
            }
        }

        private void Select(int index)
        {
            if (index < 0 || index >= _panels.Count)
            {
                return;
            }

            _selectedIndex = index;
            EditorPrefs.SetString(PanelPrefsKey, _panels[index].Id);

            for (var i = 0; i < _navItems.Count; i++)
            {
                _navItems[i].EnableInClassList("ddove-nav-item--active", i == index);
            }

            var entry = _panels[index];
            _content.Clear();

            var head = new VisualElement();
            head.AddToClassList("ddove-page-head");
            var title = new Label(entry.Title);
            title.AddToClassList("ddove-page-title");
            head.Add(title);
            _content.Add(head);

            var host = new VisualElement();
            _content.Add(host);
            DDoveEditorRegistry.Create(entry).Build(host);
        }

        private void ShowEmpty()
        {
            _content.Clear();
            var empty = new VisualElement();
            empty.AddToClassList("ddove-empty");
            empty.Add(new Label("还没有面板。实现 IDDoveEditorPanel，再挂 [DDoveEditorPanel]。"));
            _content.Add(empty);
        }

        private int FindInitialIndex()
        {
            var selectedId = EditorPrefs.GetString(PanelPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(selectedId))
            {
                for (var i = 0; i < _panels.Count; i++)
                {
                    if (_panels[i].Id == selectedId)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        private string EditorFolder()
        {
            var script = MonoScript.FromScriptableObject(this);
            var path = AssetDatabase.GetAssetPath(script);
            return Path.GetDirectoryName(path)?.Replace('\\', '/') ?? string.Empty;
        }
    }
}
